using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Internal;
using StatesLanguage.IntrinsicFunctions;
using StatesLanguage.ReferencePaths;
using StatesLanguage.States;

namespace StatesLanguage
{
    
    public class InputOutputProcessor : IInputOutputProcessor
    {
        private readonly IntrinsicFunctionRegistry _registry;
        private const string ROOT_MEMBER_OBJECT = "$";

        public InputOutputProcessor(IntrinsicFunctionRegistry registry)
        {
            _registry = registry;
        }
        
        public JToken GetEffectiveInput(JToken input, OptionalString inputPath, JObject payload, JObject context)
        {
            if (!inputPath.IsSet)
                inputPath.Value = ROOT_MEMBER_OBJECT;
            
            return TransformPayloadTemplate(ExtractTokenFromJsonPath(input, inputPath.Value), payload, context);
        }

        public JToken GetEffectiveResult(JToken output, JObject payload, JObject context)
        {
            return TransformPayloadTemplate(output, payload, context);
        }

        public JToken GetEffectiveOutput(JToken input, JToken result, OptionalString outputPath, OptionalString resultPath)
        {
            if (!outputPath.IsSet)
            {
                outputPath.Value = ROOT_MEMBER_OBJECT;
            }
            else if (!outputPath.HasValue)
            {
                //If the value of OutputPath is null, that means the input and result are discarded,
                //and the effective output from the state is an empty JSON object, {}
                return new JObject();
            }
            
            if (!resultPath.IsSet)
            {
                resultPath.Value = ROOT_MEMBER_OBJECT;
            }
            return ExtractTokenFromJsonPath(HandleResultPath(input, result, resultPath.Value),outputPath.Value);
        }

        public JToken GetFailPathValue(JToken input, OptionalString failPath, JObject payload, JObject context)
        {
            // A JSONPath Fail state MAY have "ErrorPath" and "CausePath" fields whose values
            // MUST be Reference Paths or Intrinsic Functions which,
            // when resolved, MUST be string values

            if (!failPath.IsSet)
                failPath.Value = ROOT_MEMBER_OBJECT;

            JToken result;

            if (IntrinsicFunction.TryParse(failPath, out var intrinsicFunction))
            {
                result =  _registry.CallFunction(intrinsicFunction, input, context);
            }
            else
            {
                result = ExtractTokenFromJsonPath(input, failPath);

                // if (extractedToken != null && IntrinsicFunction.TryParse(extractedToken.Value<string>(), out var extractedTokenFunction))
                // {
                //     result = _registry.CallFunction(extractedTokenFunction, input, context);
                // }
            }

            if (result.Type != JTokenType.String)
            {
                throw new PathMatchFailureException($"Failed to extract value from Fail Path: '{failPath}' value must resolve as a string");
            }

            return result;
        }

        private static JToken ExtractTokenFromJsonPath(JToken input, string path)
        {
            if (input == null)
            {
                throw new PathMatchFailureException($"Input is null, unable to extract Path '{path}'");
            }
            
            if (path == null)
            {
                return new JObject();
            }
            
            if(!path.StartsWith(ROOT_MEMBER_OBJECT))
                throw new PathMatchFailureException($"Invalid JsonPath '{path}', must start with '{ROOT_MEMBER_OBJECT}'");
            
            if (path.Equals(ROOT_MEMBER_OBJECT))
            {
                return input;
            }
            
            var tokens = input.SelectTokens(path).ToArray();

            switch (tokens.Length)
            {
                case 0:
                    throw new PathMatchFailureException($"Input Path '{path}' does not exists in Input received: '{input}'");
                case 1:
                    return tokens[0];
                default:
                {
                    var arr = new JArray();
                    foreach(var t in tokens)
                        arr.Add(t);
                    return arr;
                }
            }
        }

        private JToken TransformPayloadTemplate(JToken input, JToken payload, JObject context)
        {
            if (payload == null)
            {
                return input;
            }

            return payload.Type switch
            {
                JTokenType.Array => TransformPayloadArray(input, (JArray) payload, context),
                JTokenType.Object => TransformPayloadObject(input, (JObject) payload, context),
                _ => payload
            };
        }
        
        private JToken TransformPayloadArray(JToken input, JArray parameters, JObject context)
        {
            foreach (var element in parameters)
            {
                TransformPayloadTemplate(input, element, context);
            }

            return parameters;
        }
        
        private JToken TransformPayloadObject(JToken input, JObject parameters, JObject context)
        {
            var changes = new Dictionary<string, JToken>();
            foreach (var element in parameters)
            {
                if (element.Value is JContainer container)
                {
                    TransformPayloadTemplate(input, container, context);
                }
                else if (element.Value.Type == JTokenType.String && element.Key.EndsWith(".$"))
                {
                    var elementValue = element.Value.Value<string>();
                    var newPropertyName = element.Key.Substring(0, element.Key.Length - 2);

                    if (elementValue.StartsWith("$$."))
                    {
                        Ensure.IsNotNull(context, new ParameterPathFailureException($"Context is null, unable to extract Input Path '{elementValue}'"));
                        var contextToken = context.SelectToken(elementValue.Substring(1, elementValue.Length - 1));
                        Ensure.IsNotNull(contextToken, new ParameterPathFailureException($"Input Path '{elementValue}' does not exists in Context received: '{context}'"));
                        changes.Add(element.Key, new JProperty(newPropertyName, contextToken));
                    }
                    else if (elementValue.StartsWith('$'))
                    {
                        var token = input.SelectToken(elementValue);
                        Ensure.IsNotNull(token, new ParameterPathFailureException($"Input Path '{elementValue}' does not exists in Input received: '{input}'"));
                        changes.Add(element.Key, new JProperty(newPropertyName, token));
                    }
                    else // Intrinsic functions
                    {
                        var f = IntrinsicFunction.Parse(elementValue);
                        changes.Add(element.Key,
                            new JProperty(
                                newPropertyName,
                                _registry.CallFunction(f, input, context)));
                    }
                }
            }

            foreach (var keyVal in changes)
            {
                parameters.Remove(keyVal.Key);
                parameters.Add(keyVal.Value);
            }

            return parameters; }

        private static JToken HandleResultPath(JToken input, JToken result, string resultPath)
        {
            switch (resultPath)
            {
                case null:
                    return input;
                case ROOT_MEMBER_OBJECT:
                    return result;
                default:
                    // Check if token already exists
                    var token = input.SelectToken(resultPath);
                    if (token != null)
                    {
                        token.Replace(result);
                        return input;
                    }

                    var refPath = ReferencePath.Parse(resultPath);
                    token = CreateJTokenFromResult(result, refPath.Parts);

                    if (token.Type == input.Type && input is JContainer container)
                    {
                        container.Merge(token,new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Merge,
                            MergeNullValueHandling = MergeNullValueHandling.Ignore
                        });
                        return container;
                    }

                    throw new ResultPathMatchFailureException($"Unable to apply result path '{resultPath}' to input '{input}'");
            }
        }
        
        private static JToken CreateJTokenFromResult(JToken result, List<PathToken> filters)
        {
            if (filters.Count == 0)
                return result;
            
            var token = filters[0];
            filters.RemoveAt(0);
            var r = CreateJTokenFromResult(result, filters);

            if (token is FieldToken fieldFilter)
            {
                var tmp = new JObject {{fieldFilter.Name, r}};
                result = tmp;
            }
            else if (token is ArrayIndexToken arrayIndexFilter)
            {
                var tmp = new JArray();
                for (int i = 0; i < arrayIndexFilter.Index; i++)
                {
                    tmp.Add(null);
                }
                tmp.Add(r);
                result = tmp;
            }
            
            return result;
        }
    }
}