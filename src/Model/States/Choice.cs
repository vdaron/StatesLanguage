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
using Newtonsoft.Json.Linq;
using System;

namespace StatesLanguage.Model.States
{
    public sealed class Choice
    {
        private Choice()
        {
        }

        /// <summary>
        /// Name of the state that the state machine will transition to if the condition evaluates to true
        /// </summary>
        [JsonIgnore]
        public ICondition Condition { get; set; }

        /// <summary>
        /// The transition for this choice rule.
        /// </summary>
        [JsonIgnore]
        public ITransition Transition { get; set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IBuildable<Choice>
        {
            private IBuildable<ICondition> _condition = NullConditionBuilder.Instance;

            private ITransitionBuilder<NextStateTransition> _transition = NullTransitionBuilder<NextStateTransition>.Instance;

            internal Builder()
            {
            }
            
            /// <summary>
            /// Sets the name of the state that the state machine will transition to if the condition evaluates to true
            /// </summary>
            [JsonProperty(PropertyNames.NEXT)]
            private string NextStateName
            {
                set => Transition(NextStateTransition.GetBuilder().NextStateName(value));
            }

            /// <summary>
            /// Build the <see cref="Choice"/>
            /// </summary>
            /// <returns></returns>
            public Choice Build()
            {
                return new Choice
                       {
                           Condition = _condition.Build(),
                           Transition = _transition.Build()
                       };
            }
            
            /// <summary>
            /// REQUIRED. Sets the condition for this choice rule.
            /// </summary>
            /// <param name="conditionBuilder">Instance of {@link Builder}. Note that the <see cref="State"/> object is not built until the
            ///  <see cref="Choice"/> is built so any modifications on the state model will be reflected in this object.</param>
            /// <typeparam name="T"></typeparam>
            /// <returns>This object for method chaining.</returns>
            public Builder Condition<T>(IConditionBuilder<T> conditionBuilder) where T : ICondition
            {
                _condition = (IBuildable<ICondition>) conditionBuilder;
                return this;
            }

            /// <summary>
            /// Sets the transition for this choice rule.
            /// </summary>
            /// <param name="transition">Transition that occurs if the choice rule condition evaluates to true.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Transition(NextStateTransition.Builder transition)
            {
                _transition = transition;
                return this;
            }
        }
    }
}