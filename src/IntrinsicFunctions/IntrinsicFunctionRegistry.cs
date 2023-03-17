using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Internal;
using StatesLanguage.IntrinsicFunctions;

namespace StatesLanguage.IntrinsicFunctions
{
    public delegate JToken IntrinsicFunctionFunc(
        IntrinsicFunction function, 
        JToken input,
        JObject context,
        IntrinsicFunctionRegistry registry);

    public class IntrinsicFunctionRegistry : IIntrinsicFunctionRegistry
    {
        private readonly Dictionary<string, IntrinsicFunctionFunc> _intrinsicFunctions =
            new Dictionary<string, IntrinsicFunctionFunc>();

        public IntrinsicFunctionRegistry()
        {
            Register("States.Array", StandardIntrinsicFunctions.Array);
            Register("States.ArrayContains", StandardIntrinsicFunctions.ArrayContains);
            Register("States.ArrayGetItem", StandardIntrinsicFunctions.ArrayGetItem);
            Register("States.ArrayLength", StandardIntrinsicFunctions.ArrayLength);
            Register("States.ArrayPartition", StandardIntrinsicFunctions.ArrayPartition);
            Register("States.ArrayRange", StandardIntrinsicFunctions.ArrayRange);
            Register("States.ArrayUnique", StandardIntrinsicFunctions.ArrayUnique);
            Register("States.Base64Encode", StandardIntrinsicFunctions.Base64Encode);
            Register("States.Base64Decode", StandardIntrinsicFunctions.Base64Decode);
            Register("States.Format", StandardIntrinsicFunctions.Format);
            Register("States.Hash", StandardIntrinsicFunctions.Hash);
            Register("States.JsonMerge", StandardIntrinsicFunctions.JsonMerge);
            Register("States.JsonToString", StandardIntrinsicFunctions.JsonToString);
            Register("States.MathAdd", StandardIntrinsicFunctions.MathAdd);
            Register("States.MathRandom", StandardIntrinsicFunctions.MathRandom);
            Register("States.StringSplit", StandardIntrinsicFunctions.StringSplit);
            Register("States.StringToJson", StandardIntrinsicFunctions.StringToJson);
            Register("States.UUID", StandardIntrinsicFunctions.Uuid);
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