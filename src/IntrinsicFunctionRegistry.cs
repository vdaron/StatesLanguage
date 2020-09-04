using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal;

namespace StatesLanguage
{
    public delegate JToken IntrinsicFunctionFunc(IntrinsicFunction function, JToken input, JObject context,
        IntrinsicFunctionRegistry registry);

    public class IntrinsicFunctionRegistry
    {
        private readonly Dictionary<string, IntrinsicFunctionFunc> _intrinsicFunctions =
            new Dictionary<string, IntrinsicFunctionFunc>();

        public IntrinsicFunctionRegistry()
        {
            Register("States.Format", StandardIntrinsicFunctions.Format);
            Register("States.StringToJson", StandardIntrinsicFunctions.StringToJson);
            Register("States.JsonToString", StandardIntrinsicFunctions.JsonToString);
            Register("States.Array", StandardIntrinsicFunctions.Array);
        }

        public void Register(string name, IntrinsicFunctionFunc func)
        {
            if (_intrinsicFunctions.ContainsKey(name))
            {
                _intrinsicFunctions[name] = func;
            }
            else
            {
                _intrinsicFunctions.Add(name, func);
            }
        }

        public JToken CallFunction(IntrinsicFunction function, JToken input, JObject context)
        {
            if (_intrinsicFunctions.ContainsKey(function.Name))
            {
                return _intrinsicFunctions[function.Name](function, input, context, this);
            }

            throw new StatesLanguageException("Invalid Intrinsic function name");
        }
    }
}