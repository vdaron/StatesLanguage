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
using System.Collections.Generic;
using System.Linq;
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.States
{
    public class ChoiceState : State
    {
        private ChoiceState()
        {
        }

        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; set; }

        [JsonProperty(PropertyNames.DEFAULT_STATE)]
        public string DefaultStateName { get; set; }

        [JsonProperty(PropertyNames.INPUT_PATH)]
        public string InputPath { get; set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public string OutputPath { get; set; }

        [JsonProperty(PropertyNames.CHOICES)]
        public List<Choice> Choices { get; set; }

        public override StateType Type => StateType.Choice;

        public override bool IsTerminalState => false;

        /**
         * @return Builder instance to construct a {@link ChoiceState}.
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
         * Builder for a {@link ChoiceState}.
         */
        public sealed class Builder : IBuilder<ChoiceState>
        {
            [JsonProperty(PropertyNames.CHOICES)]
            private List<Choice.Builder> _choices = new List<Choice.Builder>();

            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.DEFAULT_STATE)]
            private string _defaultStateName;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private string _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private string _outputPath;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link ChoiceState} object.
             */
            public ChoiceState Build()
            {
                return new ChoiceState
                       {
                           Comment = _comment,
                           DefaultStateName = _defaultStateName,
                           Choices = BuildableUtils.Build(_choices).ToList(),
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
             * OPTIONAL. Name of state to transition to if no {@link Choice} rules match. If a default state is not provided and no
             * choices match then a {@link ErrorCodes#NO_CHOICE_MATCHED} error is thrown.
             *
             * @param defaultStateName Name of default state.
             * @return This object for method chaining.
             */
            public Builder DefaultStateName(string defaultStateName)
            {
                _defaultStateName = defaultStateName;
                return this;
            }

            /**
             * REQUIRED. Adds a new {@link Choice} rule to the {@link ChoiceState}. A {@link ChoiceState} must contain at least one
             * choice rule.
             *
             * @param choiceBuilder Instance of {@link Choice.Builder}. Note that the {@link
             *                      Choice} object is not built until the {@link ChoiceState} is built so any modifications on the
             *                      state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Choice(Choice.Builder choiceBuilder)
            {
                _choices.Add(choiceBuilder);
                return this;
            }

            /**
             * REQUIRED. Adds the {@link Choice} rules to the {@link ChoiceState}. A {@link ChoiceState} must contain at least one
             * choice rule.
             *
             * @param choiceBuilders Instances of {@link Choice.Builder}. Note that the {@link
             *                       Choice} object is not built until the {@link ChoiceState} is built so any modifications on the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Choices(params Choice.Builder[] choiceBuilders)
            {
                _choices.AddRange(choiceBuilders);
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