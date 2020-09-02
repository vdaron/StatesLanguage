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
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.States
{
    public class WaitState : TransitionState
    {
        private WaitState()
        {
        }

        public override StateType Type => StateType.Wait;

        [JsonIgnore]
        public IWaitFor WaitFor { get; private set; }

        /**
         * @return Builder instance to construct a {@link WaitState}.
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
         * Builder for a {@link WaitState}.
         */
        public sealed class Builder : TransitionStateBuilder<WaitState, Builder>
        {
            private IWaitForBuilder<IWaitFor> _waitFor = NullWaitForBuilder.Instance;

            internal Builder()
            {
            }
            
            /**
             * REQUIRED. Implementation of {@link WaitFor} that indicates how long the state should wait before proceeding.
             *
             * @param WaitFor Implementation of {@link WaitFor}
             * @return This object for method chaining.
             */
            public Builder WaitFor<T>(IWaitForBuilder<T> waitFor) where T : IWaitFor
            {
                _waitFor = (IWaitForBuilder<IWaitFor>) waitFor;
                return this;
            }
            
            /**
             * @return An immutable {@link WaitState} object.
             */
            public override WaitState Build()
            {
                return new WaitState
                       {
                           Comment = _comment,
                           WaitFor = _waitFor.Build(),
                           InputPath = _inputPath,
                           OutputPath = _outputPath,
                           Transition = _transition.Build()
                       };
            }

#pragma warning disable S2376 // Write-only properties should not be used
            // Needed for deserialization
            [JsonProperty(PropertyNames.SECONDS)]
            internal int Seconds
            {
                set => WaitFor(WaitForSeconds.GetBuilder().Seconds(value));
            }

            // Needed for deserialization
            [JsonProperty(PropertyNames.TIMESTAMP)]
            internal DateTime Timestamp
            {
                set => WaitFor(WaitForTimestamp.GetBuilder().Timestamp(value));
            }

            // Needed for deserialization
            [JsonProperty(PropertyNames.TIMESTAMP_PATH)]

            internal string TimestampPath
            {
                set => WaitFor(WaitForTimestampPath.GetBuilder().TimestampPath(value));
            }

            // Needed for deserialization
            [JsonProperty(PropertyNames.SECONDS_PATH)]
            internal string SecondsPath
            {
                set => WaitFor(WaitForSecondsPath.GetBuilder().SecondsPath(value));
            }
#pragma warning restore S2376 // Write-only properties should not be used
        }
    }
}