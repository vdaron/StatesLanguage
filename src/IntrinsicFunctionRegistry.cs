using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Internal;

namespace StatesLanguage
{
    public delegate JToken IntrinsicFunctionFunc(IntrinsicFunction function, JToken input, JObject context,
        IntrinsicFunctionRegistry registry);

    public class IntrinsicFunctionRegistry : IIntrinsicFunctionRegistry
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
            Ensure.IsNotNullNorEmpty<ArgumentException>(name);
            Ensure.IsNotNull<ArgumentNullException>(func);
            
            if (_intrinsicFunctions.ContainsKey(name))
            {
                _intrinsicFunctions[name] = func;
            }
            else
            {
                _intrinsicFunctions.Add(name, func);
            }
        }

        public void Unregister(string name)
        {
            Ensure.IsNotNullNorEmpty<ArgumentException>(name);
            
            if (_intrinsicFunctions.ContainsKey(name))
            {
                _intrinsicFunctions.Remove(name);
            }
        }

        internal JToken CallFunction(IntrinsicFunction function, JToken input, JObject context)
        {
            if (_intrinsicFunctions.ContainsKey(function.Name))
            {
                return _intrinsicFunctions[function.Name](function, input, context, this);
            }

            throw new StatesLanguageException("Invalid Intrinsic function name");
        }
    }
}