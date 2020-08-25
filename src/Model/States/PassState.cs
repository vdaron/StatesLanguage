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

        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; private set; }

        [JsonProperty(PropertyNames.INPUT_PATH)]
        public OptionalString InputPath { get; private set; }

        [JsonProperty(PropertyNames.RESULT)]
        public JToken Result { get; private set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public OptionalString OutputPath { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public OptionalString ResultPath { get; private set; }

        [JsonProperty(PropertyNames.PARAMETERS)]
        public JToken Parameters { get; private set; }


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
        public sealed class Builder : TransitionStateBuilder<PassState, Builder>
        {
            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private OptionalString _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private OptionalString _outputPath;

            [JsonProperty(PropertyNames.RESULT)]
            private JToken _result;

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private OptionalString _resultPath;

            [JsonProperty(PropertyNames.PARAMETERS)]
            private JToken _parameters;

            private ITransitionBuilder<ITransition> _transition = NullTransitionBuilder<ITransition>.Instance;

            internal Builder()
            {
            }

            /**
             * OPTIONAL. Human readable description for the state.
             *
             * @param comment New comment.
             * @return This object for method chaining.
             */
            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
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
             * OPTIONAL. The value of “InputPath” MUST be a Path, which is applied to a State’s raw input to select some or all of
             * it;
             * that selection is used by the state. If not provided then the whole output from the previous state is used as input to
             * this state.
             *
             * @param inputPath New path value.
             * @return This object for method chaining.
             */
            public Builder InputPath(string inputPath)
            {
                _inputPath = inputPath;
                return this;
            }

            /**
             * OPTIONAL. The value of “OutputPath” MUST be a path, which is applied to the state’s output after the application of
             * ResultPath,
             * leading in the generation of the raw input for the next state. If not provided then the whole output is used.
             *
             * @param outputPath New path value.
             * @return This object for method chaining.
             */
            public Builder OutputPath(string outputPath)
            {
                _outputPath = outputPath;
                return this;
            }

            /**
             * OPTIONAL. The value of “ResultPath” MUST be a Reference Path, which specifies the combination with or replacement of
             * the state’s result with its raw input. If not provided then the output completely replaces the input.
             *
             * @param resultPath New path value.
             * @return This object for method chaining.
             */
            public Builder ResultPath(string resultPath)
            {
                _resultPath = resultPath;
                return this;
            }

            public Builder Parameters(JToken parameters)
            {
                _parameters = parameters;
                return this;
            }

            /**
             * REQUIRED. Sets the transition that will occur when this state completes successfully.
             *
             * @param transition New transition.
             * @return This object for method chaining.
             */

            public override Builder Transition<U>(ITransitionBuilder<U> transition)
            {
                _transition = (ITransitionBuilder<ITransition>) transition;
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