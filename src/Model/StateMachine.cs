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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StatesLanguage.Model.Internal;
using StatesLanguage.Model.Internal.Validation;
using StatesLanguage.Model.States;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.Serialization;

namespace StatesLanguage.Model
{
    /// <summary>
    ///     Represents a StepFunctions state machine. A state machine must have at least one state.
    ///     <a href="https://states-language.net/spec.html#toplevelfields" >See spec</a>
    /// </summary>
    public class StateMachine
    {
        private StateMachine()
        {

        }

        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; private set; }

        [JsonProperty(PropertyNames.START_AT)]
        public string StartAt { get; private set; }

        [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
        public int? TimeoutSeconds { get; private set; }

        [JsonProperty(PropertyNames.STATES)]
        public Dictionary<string, State> States { get; private set; }

        /**
         * Deserializes a JSON representation of a state machine into a {@link StateMachine.Builder} .
         *
         * @param json JSON representing State machine.
         * @return Mutable {@link StateMachine.Builder} deserialized from JSON representation.
         */
        public static Builder FromJson(string json)
        {
            try
            {
                using (var stringReader = new StringReader(json))
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return GetJsonSerializer().Deserialize<Builder>(jsonTextReader);
                }
            }
            catch (Exception e)
            {
                throw new StatesLanguageException($"Could not deserialize state machine.\n{json}", e);
            }
        }

        public static Builder FromJObject(JObject json)
        {
            try
            {
                return json.ToObject<Builder>(GetJsonSerializer());
            }
            catch (Exception e)
            {
                throw new StatesLanguageException($"Could not deserialize state machine.\n{json}", e);
            }
        }
        
        /**
         * @return Compact JSON representation of this StateMachine.
         */
        public string ToJson()
        {
            try
            {
                var result = new StringBuilder();
                using (var stringWriter = new StringWriter(result))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    GetJsonSerializer().Serialize(jsonWriter, this);
                }

                return result.ToString();
            }
            catch (Exception e)
            {
                throw new StatesLanguageException("Could not serialize state machine.", e);
            }
        }

        public JObject ToJObject()
        {
            try
            {
                return JObject.FromObject(this, GetJsonSerializer());
            }
            catch (Exception e)
            {
                throw new StatesLanguageException("Could not serialize state machine.", e);
            }
        }

        /**
         * @return Builder instance to construct a {@link StateMachine}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        internal static JsonSerializer GetJsonSerializer()
        {
            JsonSerializer jsonSerializer = new JsonSerializer();

            jsonSerializer.Converters.Add(new StateConverter());
            jsonSerializer.Converters.Add(new ChoiceDeserializer());
            jsonSerializer.Converters.Add(new WaitStateDeserializer());
            jsonSerializer.Converters.Add(new TransitionStateDeserializer());

            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.ContractResolver = EmptyCollectionContractResolver.Instance;
            return jsonSerializer;
        }

        [JsonObject(MemberSerialization.Fields)]
        public sealed class Builder
        {
            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.START_AT)]
            private string _startAt;

            [JsonProperty(PropertyNames.STATES)]
            private Dictionary<string, State.IBuilder<State>> _states = new Dictionary<string, State.IBuilder<State>>();

            [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
            private int? _timeoutSeconds;

            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
            }

            /**
             * REQUIRED. Name of the state to start execution at. Must match a state name provided via {@link #state(String,
             * State.Builder)}.
             *
             * @param startAt Name of starting state.
             * @return This object for method chaining.
             */
            public Builder StartAt(string startAt)
            {
                _startAt = startAt;
                return this;
            }

            /**
             * OPTIONAL. Timeout, in seconds, that a state machine is allowed to run. If the machine execution runs longer than this
             * timeout the execution fails with a {@link ErrorCodes#TIMEOUT} error
             *
             * @param timeoutSeconds Timeout value.
             * @return This object for method chaining.
             */
            public Builder TimeoutSeconds(int timeoutSeconds)
            {
                _timeoutSeconds = timeoutSeconds;
                return this;
            }

            /**
             * REQUIRED. Adds a new state to the state machine. A state machine MUST have at least one state.
             *
             * @param stateName    Name of the state
             * @param stateBuilder Instance of {@link State.Builder}. Note that the {@link State}
             *                     object is not built until the {@link StateMachine} is built so any modifications on the state
             *                     model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder State<T>(string stateName, State.IBuilder<T> stateBuilder) where T : State
            {
                _states.Add(stateName, stateBuilder);
                return this;
            }

            /**
             * @return An immutable {@link StateMachine} object that can be transformed to JSON via {@link StateMachine#toJson()}.
             */
            public StateMachine Build()
            {
                return new StateMachineValidator(new StateMachine
                                                 {
                                                     Comment = _comment,
                                                     StartAt = _startAt,
                                                     TimeoutSeconds = _timeoutSeconds,
                                                     States = BuildableUtils.Build(_states)
                                                 }).Validate();
            }


        }
    }
}