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
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.States
{
    public class SucceedState : InputOutputState
    {
        private SucceedState()
        {
        }

        public override StateType Type => StateType.Succeed;

        [JsonIgnore]
        public override bool IsTerminalState => true;

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /**
         * @return Builder instance to construct a {@link SucceedState}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link SucceedState}.
         */
        [JsonObject(MemberSerialization.OptIn)]
        public sealed class Builder : IBuilder<SucceedState>
        {
            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private OptionalString _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private OptionalString _outputPath;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link SucceedState} object.
             */
            public SucceedState Build()
            {
                return new SucceedState
                       {
                           Comment = _comment,
                           InputPath = _inputPath,
                           OutputPath = _outputPath
                       };
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
             * ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
             * used.
             *
             * @param outputPath New path value.
             * @return This object for method chaining.
             */
            public Builder OutputPath(string outputPath)
            {
                _outputPath = outputPath;
                return this;
            }
        }
    }
}