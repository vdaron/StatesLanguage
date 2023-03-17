using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;

namespace StatesLanguage.Internal
{
    internal static class StandardIntrinsicFunctions
    {
        private const int MAX_STRING_LENGTH = 10000;
        private const int MAX_RANGE_ARRAY_LENGTH = 1000;

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
                    BooleanIntrinsicParam b => b.Value,
                    StringIntrinsicParam s => s.Value,
                    DecimalIntrinsicParam n => n.Number,
                    IntegerIntrinsicParam n => n.Number,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException("Invalid Parameter type")
                });
            }

            return result;
        }

        public static JToken ArrayContains(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.ArrayContains requires exactly two arguments");
            }

            var arrayParam = function.Parameters[0] switch
            {
                NullIntrinsicParam => null,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayContains: Invalid array parameter.")
            };

            if (arrayParam == null)
            {
                return false;
            }

            var array = arrayParam switch
            {
                JArray a => a,
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayContains: Invalid array parameter.")
            };

            var lookingFor = function.Parameters[1] switch
            {
                BooleanIntrinsicParam b => b.Value,
                StringIntrinsicParam s => s.Value,
                DecimalIntrinsicParam n => n.Number,
                IntegerIntrinsicParam n => n.Number,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    "States.ArrayContains: Invalid 'looking for' parameter")
            };

            return array.Any(x => JToken.DeepEquals(x, lookingFor));
        }

        public static JToken ArrayGetItem(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.ArrayGetItem requires exactly two arguments");
            }

            var arrayParam = function.Parameters[0] switch
            {
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayGetItem: Invalid array parameter.")
            };

            var array = arrayParam switch
            {
                JArray a => a,
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayGetItem: Invalid array parameter.")
            };

            var indexToken = function.Parameters[1] switch
            {
                IntegerIntrinsicParam n => n.Number,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    "States.ArrayGetItem: Invalid integer 'index' parameter")
            };

            if (indexToken.Type != JTokenType.Integer)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.ArrayGetItem: Invalid index type {indexToken.Type}");
            }

            int index = indexToken.Value<int>();

            if (index < 0 || index >= array.Count)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.ArrayGetItem: Index {index} out of bounds of array of length {array.Count}");
            }

            return array[index];
        }

        public static JToken ArrayLength(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1)
            {
                throw new InvalidIntrinsicFunctionException(
                    "States.ArrayLength requires exactly one array argument");
            }

            var arrayParam = function.Parameters[0] switch
            {
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayLength: Invalid array parameter.")
            };

            var array = arrayParam switch
            {
                JArray a => a,
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayLength: Invalid array parameter.")
            };

            return array.Count;
        }

        public static JToken ArrayPartition(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.ArrayPartition requires exactly two arguments");
            }

            var arrayParam = function.Parameters[0] switch
            {
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayPartition: Invalid array parameter.")
            };

            var array = arrayParam switch
            {
                JArray a => a,
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayPartition: Invalid array parameter.")
            };

            var chunkSizeToken = function.Parameters[1] switch
            {
                IntegerIntrinsicParam n => n.Number,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    "States.ArrayPartition: Invalid integer 'chunk size' parameter")
            };

            if (chunkSizeToken.Type != JTokenType.Integer)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.ArrayPartition: Invalid 'chunk size' parameter type {chunkSizeToken.Type}");
            }

            int chunkSize = chunkSizeToken.Value<int>();

            if (chunkSize <= 0)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.ArrayPartition: Invalid 'chunk size' \"{chunkSize}\"");
            }

            var ret = new JArray();
            var current = new JArray();

            for (int i = 0; i < array.Count; i++)
            {
                current.Add(array[i]);

                if ((i + 1) % chunkSize == 0)
                {
                    ret.Add(current);
                    current = new JArray();
                }
            }

            if (current.Any())
            {
                ret.Add(current);
            }

            return ret;
        }

        public static JToken ArrayRange(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 3)
            {
                throw new InvalidIntrinsicFunctionException("States.ArrayRange requires exactly three arguments");
            }

            var paramNames = new List<string> { "start", "end", "step" };
            var paramValues = new List<int>();

            for (int i = 0; i < paramNames.Count; i++)
            {
                var paramName = paramNames[i];
                var token = function.Parameters[i] switch
                {
                    IntegerIntrinsicParam n => n.Number,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.ArrayRange: Invalid integer '{paramName}' parameter")
                };

                if (token.Type != JTokenType.Integer)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.ArrayRange: Invalid '{paramName}' parameter type {token.Type}");
                }

                paramValues.Add(token.Value<int>());
            }

            int start = paramValues[0];
            int end = paramValues[1];
            int step = paramValues[2];

            if (step == 0)
            {
                throw new InvalidIntrinsicFunctionException("States.ArrayRange: step parameter cannot be 0");
            }

            if (start == end)
            {
                return new JArray(start);
            }

            if (end < start && step > 0 || start < end && step < 0)
            {
                return new JArray();
            }

            if (Math.Abs(end - start) / Math.Abs(step) + 1 >= MAX_RANGE_ARRAY_LENGTH)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.ArrayRange: cannot generate more than {MAX_RANGE_ARRAY_LENGTH} values.");
            }

            var ret = new JArray();
            for (int i = start; step > 0 ? i <= end : i >= end; i += step)
            {
                ret.Add(i);
            }

            return ret;
        }

        public static JToken ArrayUnique(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1)
            {
                throw new InvalidIntrinsicFunctionException(
                    "States.ArrayUnique requires exactly one array argument");
            }

            var arrayParam = function.Parameters[0] switch
            {
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayUnique: Invalid array parameter.")
            };

            var array = arrayParam switch
            {
                JArray a => a,
                _ => throw new InvalidIntrinsicFunctionException("States.ArrayUnique: Invalid array parameter.")
            };

            var ret = new JArray();
            foreach (var token in array.Distinct(new JTokenDeepComparer()))
            {
                ret.Add(token);
            }

            return ret;
        }

        public static JToken Base64Decode(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1)
            {
                throw new InvalidIntrinsicFunctionException("States.Base64Decode requires exactly one argument");
            }

            var base64Token = function.Parameters[0] switch
            {
                StringIntrinsicParam s => s.Value,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    "States.Base64Decode: Invalid string 'base64 string' parameter")
            };

            if (base64Token.Type != JTokenType.String)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Base64Decode: Invalid parameter type {base64Token.Type}");
            }

            var str = base64Token.Value<string>();
            if (str.Length > MAX_STRING_LENGTH)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Base64Decode: Cannot decode string with more than {MAX_STRING_LENGTH} characters.");
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        public static JToken Base64Encode(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 1)
            {
                throw new InvalidIntrinsicFunctionException("States.Base64Encode requires exactly one argument");
            }

            var dataToken = function.Parameters[0] switch
            {
                StringIntrinsicParam s => s.Value,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    "States.Base64Encode: Invalid string 'data string' parameter")
            };

            if (dataToken.Type != JTokenType.String)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Base64Encode: Invalid parameter type {dataToken.Type}");
            }

            var str = dataToken.Value<string>();
            if (str.Length > MAX_STRING_LENGTH)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Base64Encode: Cannot encode string with more than {MAX_STRING_LENGTH} characters.");
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToken.Value<string>()));
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

        public static JToken Hash(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.Hash requires exactly two arguments");
            }

            var paramNames = new List<string> { "data", "algorithm" };
            var paramValues = new List<string>();

            for (int i = 0; i < paramNames.Count; ++i)
            {
                var paramToken = function.Parameters[i] switch
                {
                    StringIntrinsicParam s => s.Value,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.Hash: Invalid string '{paramNames[i]}' parameter")
                };

                if (paramToken.Type != JTokenType.String)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.Hash: Invalid \"{paramNames[i]}\" parameter type {paramToken.Type}");
                }

                paramValues.Add(paramToken.Value<string>());
            }

            var data = paramValues[0];
            var algorithm = paramValues[1];

            if (data.Length > MAX_STRING_LENGTH)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Hash: Cannot hash string with more than {MAX_STRING_LENGTH} characters.");
            }

            using HashAlgorithm hashAlgorithm = algorithm switch
            {
                "MD5" => MD5.Create(),
                "SHA-1" => SHA1.Create(),
                "SHA-256" => SHA256.Create(),
                "SHA-384" => SHA384.Create(),
                "SHA-512" => SHA512.Create(),
                _ => throw new InvalidIntrinsicFunctionException(
                    $"States.Hash: Invalid hashing algorithm \"{algorithm}\". " +
                    "Must be one of: MD5, SHA-1, SHA-256, SHA-384, SHA-512")
            };

            var ret = BitConverter
                .ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data)))
                .Replace("-", string.Empty)
                .ToLower();

            return ret;
        }

        public static JToken JsonMerge(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 3)
            {
                throw new InvalidIntrinsicFunctionException("States.JsonMerge requires exactly three arguments");
            }

            var objParamNames = new List<string> { "object1", "object2" };
            var objParamValues = new List<JObject>();

            for (int i = 0; i < objParamNames.Count; ++i)
            {
                var paramToken = function.Parameters[i] switch
                {
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.Hash: Invalid JSON object '{objParamNames[i]}' parameter")
                };

                if (paramToken.Type != JTokenType.Object)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.Hash: Invalid \"{objParamNames[i]}\" parameter type {paramToken.Type}");
                }

                objParamValues.Add(paramToken as JObject);
            }

            var boolToken = function.Parameters[2] switch
            {
                BooleanIntrinsicParam boolean => boolean.Value,
                PathIntrinsicParam pa => GetPathValue(pa, input, context),
                IntrinsicFunction func => registry.CallFunction(func, input, context),
                _ => throw new InvalidIntrinsicFunctionException(
                    $"States.Hash: Invalid boolean 'deep merging' parameter")
            };

            if (boolToken.Type != JTokenType.Boolean)
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.Hash: Invalid \"deep merging\" parameter type {boolToken.Type}");
            }

            var object1 = objParamValues[0];
            var object2 = objParamValues[1];
            var deepMerging = boolToken.Value<bool>();

            if (deepMerging)
            {
                MergeRecursiveDeep(object1, object2);
            }
            else
            {
                foreach (var token in object2)
                {
                    object1[token.Key] = token.Value;
                }
            }

            return object1;
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

        public static JToken MathAdd(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.MathAdd requires exactly two arguments");
            }

            var paramNames = new List<string> { "number1", "number2" };
            var paramValues = new List<JToken>();

            for (int i = 0; i < paramNames.Count; i++)
            {
                var paramName = paramNames[i];
                var token = function.Parameters[i] switch
                {
                    IntegerIntrinsicParam n => n.Number,
                    DecimalIntrinsicParam n => n.Number,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.MathAdd: Invalid integer '{paramName}' parameter")
                };

                if (token.Type != JTokenType.Integer && token.Type != JTokenType.Float)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.MathAdd: Invalid '{paramName}' parameter type {token.Type}");
                }

                paramValues.Add(token);
            }

            if (paramValues[0].Type == JTokenType.Integer && paramValues[1].Type == JTokenType.Integer)
            {
                return paramValues[0].Value<int>() + paramValues[1].Value<int>();
            }
            else
            {
                return paramValues[0].Value<double>() + paramValues[1].Value<double>();
            }
        }

        public static JToken MathRandom(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2 && function.Parameters.Length != 3)
            {
                throw new InvalidIntrinsicFunctionException("States.MathRandom requires two or three arguments");
            }

            var paramNames = new List<string> { "start", "end" };
            var paramValues = new List<int>();

            if (function.Parameters.Length == 3)
            {
                paramNames.Add("seed");
            }

            for (int i = 0; i < paramNames.Count; i++)
            {
                var paramName = paramNames[i];
                var token = function.Parameters[i] switch
                {
                    IntegerIntrinsicParam n => n.Number,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.MathRandom: Invalid integer '{paramName}' parameter")
                };

                if (token.Type != JTokenType.Integer && token.Type != JTokenType.Float)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.MathRandom: Invalid '{paramName}' parameter type {token.Type}");
                }

                paramValues.Add(token.Value<int>());
            }

            int start = paramValues[0];
            int end = paramValues[1];
            System.Random rand = function.Parameters.Length != 3 ? Random.Shared : new System.Random(paramValues[2]);

            return rand.Next(start, end);
        }

        public static JToken StringSplit(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            Ensure.IsNotNull<ArgumentNullException>(function);
            Ensure.IsNotNull<ArgumentNullException>(registry);

            if (function.Parameters.Length != 2)
            {
                throw new InvalidIntrinsicFunctionException("States.StringSplit requires exactly two arguments");
            }

            var paramNames = new List<string> { "string", "splitter" };
            var paramValues = new List<string>();

            for (int i = 0; i < paramNames.Count; ++i)
            {
                var paramToken = function.Parameters[i] switch
                {
                    StringIntrinsicParam s => s.Value,
                    PathIntrinsicParam pa => GetPathValue(pa, input, context),
                    IntrinsicFunction func => registry.CallFunction(func, input, context),
                    _ => throw new InvalidIntrinsicFunctionException(
                        $"States.StringSplit: Invalid string '{paramNames[i]}' parameter")
                };

                if (paramToken.Type != JTokenType.String)
                {
                    throw new InvalidIntrinsicFunctionException(
                        $"States.StringSplit: Invalid \"{paramNames[i]}\" parameter type {paramToken.Type}");
                }

                paramValues.Add(paramToken.Value<string>());
            }

            string str = paramValues[0];
            string splitter = paramValues[1];

            if (string.IsNullOrEmpty(splitter))
            {
                throw new InvalidIntrinsicFunctionException(
                    $"States.StringSplit: Invalid splitter parameter, it cannot be empty.");
            }

            var array = new JArray();
            foreach (var part in str.Split(new[]{splitter}, StringSplitOptions.None))
            {
                array.Add(part);
            }

            return array;
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

        public static JToken Uuid(IntrinsicFunction function, JToken input, JObject context,
            IntrinsicFunctionRegistry registry)
        {
            if (function.Parameters.Any())
            {
                throw new InvalidIntrinsicFunctionException("States.Uuid: Function expects no parameters");
            }
            return Guid.NewGuid().ToString();
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
                case BooleanIntrinsicParam boolean:
                    return boolean.Value ? "true" : "false";
                case IntegerIntrinsicParam number:
                    return number.Number.ToString(CultureInfo.InvariantCulture);
                case DecimalIntrinsicParam number:
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

        private static void MergeRecursiveDeep(JObject object1, JObject object2)
        {
            foreach (var nameTokenPair in object2)
            {
                var name = nameTokenPair.Key;
                var token = nameTokenPair.Value;

                if (object1.ContainsKey(name) &&
                    object1[name].Type == JTokenType.Object &&
                    token.Type == JTokenType.Object)
                {
                    MergeRecursiveDeep(object1[name] as JObject, token as JObject);
                }
                else
                {
                    object1[name] = token;
                }
            }
        }

        private sealed class JTokenDeepComparer : IEqualityComparer<JToken>
        {
            public bool Equals(JToken t1, JToken t2)
            {
                return JToken.DeepEquals(t1, t2);
            }

            public int GetHashCode(JToken token)
            {
                // We hash the string representation of the token,
                // since default hash does not take full object into account
                return token.ToString().GetHashCode();
            }
        }
    }
}

#if NETSTANDARD2_1
public static class Random
{
    public static readonly System.Random Shared = new System.Random();
}
#endif