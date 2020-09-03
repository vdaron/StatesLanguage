/*
 * Copyright 2010-2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * Copyright 2018- Vincent DARON All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System.IO;
using System.Text;
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StatesLanguage.Interfaces;

namespace StatesLanguage.Model.States
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class State : IState
    {
        /// <summary>
        /// State Type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyNames.TYPE)]
        public abstract StateType Type { get; }
        
        /// <summary>
        /// Human readable description for the state.
        /// </summary>
        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; set; }

        [JsonIgnore]
        public abstract bool IsTerminalState { get; }

        public abstract T Accept<T>(StateVisitor<T> visitor);

        public string ToJson()
        {
            StringBuilder result = new StringBuilder();
            using (StringWriter writer = new StringWriter(result))
            {
                StateMachine.GetJsonSerializer().Serialize(writer,this);
            }

            return result.ToString();
        }

        public static State FromJson(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonTextReader = new JsonTextReader(stringReader))
            {
                return StateMachine.GetJsonSerializer().Deserialize<IBuilder<State>>(jsonTextReader).Build();
            }
        }

        public interface IBuilder<out T> : IBuildable<T> where T : State
        {
        }
    }
    
    public abstract class StateBuilder<T, B> : State.IBuilder<T> 
        where T : State
        where B : StateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.COMMENT)]
        protected string _comment;

        internal StateBuilder()
        {
        }

        /// <summary>
        /// OPTIONAL. Human readable description for the state.
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns>This object for method chaining.</returns>
        public B Comment(string comment)
        {
            _comment = comment;
            return (B)this;
        }
        
        public abstract T Build();
    }
}