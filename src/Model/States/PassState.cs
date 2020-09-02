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
using System.IO;
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.States
{
    public class PassState : TransitionState
    {
        private PassState()
        {
        }

        [JsonProperty(PropertyNames.RESULT)]
        public JToken Result { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public OptionalString ResultPath { get; private set; }

        [JsonProperty(PropertyNames.PARAMETERS)]
        public JObject Parameters { get; private set; }


        public override StateType Type => StateType.Pass;

        /**
         * @return Builder instance to construct a {@link PassState}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /**
         * Builder for a {@link PassState}.
         */
        public sealed class Builder : ParameterStateBuilder<PassState, Builder>
        {
            [JsonProperty(PropertyNames.RESULT)]
            private JToken _result;

            internal Builder()
            {
            }

            /**
             * OPTIONAL. Sets the "virtual" result of the pass state. Must be a POJO that can be serialized into JSON.
             *
             * @param result Object that will be serialized into the JSON document representing this states result.
             * @return This object for method chaining.
             */
            public Builder Result(object result)
            {
                _result = JToken.FromObject(result);
                return this;
            }

            public Builder Result(object result, JsonSerializerSettings serializerSettings)
            {
                _result = JToken.FromObject(result, JsonSerializer.Create(serializerSettings));
                return this;
            }

            /**
             * OPTIONAL. Sets the "virtual" result of the pass state. Must be a valid JSON document.
             *
             * @param result JSON result represented as a string.
             * @return This object for method chaining.
             */
            public Builder Result(string result)
            {
                try
                {
                    using (var reader = new StringReader(result))
                    {
                        _result = JToken.Load(new JsonTextReader(reader));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Result must be a JSON document", e);
                }

                return this;
            }



            /**
             * @return An immutable {@link PassState} object.
             */
            public override PassState Build()
            {
                return new PassState
                       {
                           Comment = _comment,
                           Result = _result,
                           InputPath = _inputPath,
                           OutputPath = _outputPath,
                           ResultPath = _resultPath,
                           Parameters = _parameters,
                           Transition = _transition.Build()
                       };
            }
        }
    }
}