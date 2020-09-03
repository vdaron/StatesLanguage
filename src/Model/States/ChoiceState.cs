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
using Newtonsoft.Json;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public class ChoiceState : InputOutputState
    {
        private ChoiceState()
        {
        }

        [JsonProperty(PropertyNames.DEFAULT_STATE)]
        public string DefaultStateName { get; set; }

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
        public sealed class Builder : InputOutputStateBuilder<ChoiceState, Builder>
        {
            [JsonProperty(PropertyNames.CHOICES)] private List<Choice.Builder> _choices = new List<Choice.Builder>();

            [JsonProperty(PropertyNames.DEFAULT_STATE)]
            private string _defaultStateName;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link ChoiceState} object.
             */
            public override ChoiceState Build()
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

            /// <summary>
            ///     OPTIONAL. Name of state to transition to if no {@link Choice} rules match. If a default state is not provided and
            ///     no
            ///     choices match then a  <see cref="ErrorCodes.NO_CHOICE_MATCHED" /> error is thrown.
            /// </summary>
            /// <param name="defaultStateName">Name of default state.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder DefaultStateName(string defaultStateName)
            {
                _defaultStateName = defaultStateName;
                return this;
            }

            /// <summary>
            ///     REQUIRED. Adds a new {@link Choice} rule to the <see cref="ChoiceState" />. A <see cref="ChoiceState" /> must
            ///     contain at least one
            ///     choice rule.
            /// </summary>
            /// <param name="choiceBuilder">
            ///     Instance of <see cref="Choice.Builder" />. Note that the <see cref="Choice" />
            ///     object is not built until the <see cref="ChoiceState" /> is built so any modifications on the
            ///     state model will be reflected in this object.
            /// </param>
            /// <returns>This object for method chaining.</returns>
            public Builder Choice(Choice.Builder choiceBuilder)
            {
                _choices.Add(choiceBuilder);
                return this;
            }

            /// <summary>
            ///     Adds the <see cref="Choice" /> rules to the <see cref="ChoiceState" />. A <see cref="ChoiceState" /> must contain
            ///     at least one
            ///     choice rule.
            /// </summary>
            /// <param name="choiceBuilders">
            ///     Instances of <see cref="Choice.Builder" />. Note that the <see cref="Choice" />
            ///     object is not built until the <see cref="ChoiceState" /> is built so any modifications on the
            ///     state model will be reflected in this object.
            /// </param>
            /// <returns>This object for method chaining.</returns>
            public Builder Choices(params Choice.Builder[] choiceBuilders)
            {
                _choices.AddRange(choiceBuilders);
                return this;
            }
        }
    }
}