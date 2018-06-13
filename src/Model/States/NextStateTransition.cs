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
    public class NextStateTransition : ITransition
    {
        private NextStateTransition()
        {
        }

        [JsonProperty(PropertyNames.NEXT)]
        public string NextStateName { get; private set; }

        [JsonIgnore]
        public bool IsTerminal => false;

        /**
         * @return Builder instance to construct a {@link NextStateTransition}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
     * Builder for {@link NextStateTransition}
     */
        public sealed class Builder : ITransitionBuilder<NextStateTransition>
        {
            internal string _nextStateName;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link WaitState} object.
             */
            public NextStateTransition Build()
            {
                return new NextStateTransition
                       {
                           NextStateName = _nextStateName
                       };
            }

            /**
             * REQUIRED. Sets the name of the state to transition to. Must be a valid state in the state machine.
             *
             * @param nextStateName State name
             * @return This object for method chaining.
             */
            public Builder NextStateName(string nextStateName)
            {
                _nextStateName = nextStateName;
                return this;
            }
        }
    }
}