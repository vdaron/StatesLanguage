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
using StatesLanguage.Model.Conditions;
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.States
{
    public sealed class Choice
    {
        private Choice()
        {
        }

        [JsonIgnore]
        public ICondition Condition { get; set; }

        [JsonIgnore]
        public ITransition Transition { get; set; }

        /**
         * @return Builder instance to construct a {@link Choice}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        //@JsonDeserialize(using = Choice.ChoiceDeserializer.class)
        public sealed class Builder : IBuildable<Choice>
        {
            private IBuildable<ICondition> _condition = NullConditionBuilder.Instance;

            private ITransitionBuilder<NextStateTransition> _transition = NullTransitionBuilder<NextStateTransition>.Instance;

            internal Builder()
            {
            }

            /**
             * Sets the name of the state that the state machine will transition to if the condition evaluates to true.
             *
             * @param nextStateName Name of the state.
             * @return This object for method chaining.
             */
            [JsonProperty(PropertyNames.NEXT)]
            private string NextStateName
            {
                set => Transition(NextStateTransition.GetBuilder().NextStateName(value));
            }

            /**
             * @return An immutable {@link Choice} object.
             */
            public Choice Build()
            {
                return new Choice
                       {
                           Condition = _condition.Build(),
                           Transition = _transition.Build()
                       };
            }

            /**
             * REQUIRED. Sets the condition for this choice rule.
             *
             * @param conditionBuilder Instance of {@link Builder}. Note that the {@link State} object is not built until the
             *     {@link Choice} is built so any modifications on the state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Condition<T>(IConditionBuilder<T> conditionBuilder) where T : ICondition
            {
                _condition = (IBuildable<ICondition>) conditionBuilder;
                return this;
            }

            /**
             * REQUIRED. Sets the transition for this choice rule.
             *
             * @param transition Transition that occurs if the choice rule condition evaluates to true.
             * @return This object for method chaining.
             */
            public Builder Transition(NextStateTransition.Builder transition)
            {
                _transition = transition;
                return this;
            }
        }
    }
}