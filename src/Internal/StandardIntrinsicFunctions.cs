using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;

namespace StatesLanguage.Internal
{
    internal static class StandardIntrinsicFunctions
    {
        public static JToken Array(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            var result = new JArray();
            foreach (var p in function.Parameters)
            {
                result.Add(p switch
                {
                    NullIntrinsicParam _ => null,
                    StringIntrinsicParam s => s.Value,
                    NumberIntrinsicParam n => n.Number,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException("Invalid Parameter type")
                });
            }

            return result;
        }

        public static JToken JsonToString(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1 || !(function.Parameters[0] is PathIntrinsicParam pathParam))
            {
                throw new InvalidIntrinsicFunctionException("Requires a single path parameter");
            }

            return GetPathValue(pathParam, input, context).ToString(Formatting.None);
        }

        public static JToken StringToJson(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1)
            {
                throw new InvalidIntrinsicFunctionException("Requires a single parameter");
            }

            switch (function.Parameters[0])
            {
                case StringIntrinsicParam str:
                    return JToken.Parse(str.Value);
                case PathIntrinsicParam pathParam:
                {
                    var v = GetPathValue(pathParam, input, context);
                    if (v.Type == JTokenType.String)
                    {
                        return JToken.Parse(v.Value<string>());
                    }

                    break;
                }
                case IntrinsicFunction functionParam:
                    var fv = registry.CallFunction(functionParam, input, context);
                    if (fv.Type == JTokenType.String)
                    {
                        return JToken.Parse(fv.Value<string>());
                    }

                    break;
            }

            throw new InvalidIntrinsicFunctionException("Parameter must be a string");
        }

        public static JToken Format(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length == 0)
            {
                throw new InvalidIntrinsicFunctionException("States.Format requires at least one argument");
            }

            var format = function.Parameters[0] as StringIntrinsicParam;
            if (format == null)
            {
                throw new InvalidIntrinsicFunctionException("States.Format requires a first parameter of type string");
            }

            var result = new StringBuilder();

            var parameters = new List<IntrinsicParam>(function.Parameters);
            parameters.RemoveAt(0); // remove format

            var currentIndex = 0;

            var i = format.Value.IndexOf("{}", currentIndex, StringComparison.Ordinal);
            while (i >= 0 && parameters.Count > 0)
            {
                result.Append(format.Value.Substring(currentIndex, i - currentIndex));
                result.Append(GetParamStringValue(parameters[0], input, context, registry));
                currentIndex = i + 2;
                i = format.Value.IndexOf("{}", currentIndex, StringComparison.Ordinal);
                parameters.RemoveAt(0);
            }

            if (i >= 0)
            {
                throw new InvalidIntrinsicFunctionException("States.Format too few parameters");
            }

            if (parameters.Count > 0)
            {
                throw new InvalidIntrinsicFunctionException("States.Format too much parameters");
            }

            result.Append(format.Value.Substring(currentIndex));

            return result.Replace("\\{", "{").Replace("\\}", "}").ToString();
        }

        private static string GetParamStringValue(
            IntrinsicParam functionParameter,
            JToken input,
            JObject context,
            IntrinsicFunctionRegistry intrinsicFunctionRegistry)
        {
            switch (functionParameter)
            {
                case StringIntrinsicParam str:
                    return str.Value;
                case NullIntrinsicParam _:
                    return string.Empty;
                case NumberIntrinsicParam number:
                    return number.Number.ToString(CultureInfo.InvariantCulture);
                case PathIntrinsicParam path:
                    var result = GetPathValue(path, input, context);

                    if (result is JContainer)
                    {
                        throw new InvalidIntrinsicFunctionException("Result cannot be an Object nor an Array");
                    }

                    return result.ToString();

                case IntrinsicFunction func:
                    var funcResult = intrinsicFunctionRegistry.CallFunction(func, input, context);
                    if (funcResult is JContainer)
                    {
                        throw new InvalidIntrinsicFunctionException("Result cannot be an Object nor an Array");
                    }

                    return funcResult.ToString();
            }

            throw new InvalidIntrinsicFunctionException("Invalid parameter type");
        }

        private static JToken GetPathValue(PathIntrinsicParam path, JToken input, JObject context)
        {
            JToken result;

            if (path.Path.StartsWith("$$"))
            {
                result = context.SelectToken(path.Path.Substring(1));
            }
            else if (path.Path.StartsWith("$"))
            {
                result = input.SelectToken(path.Path);
            }
            else
            {
                throw new InvalidIntrinsicFunctionException("JsonPath must starts with '$'");
            }

            return result;
        }
    }
}