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
    public class WaitForTimestamp : IWaitFor
    {
        private WaitForTimestamp()
        {
        }

        public DateTime Timestamp { get; private set; }

        /**
         * @return Builder instance to construct a {@link WaitForTimestamp}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link WaitForTimestamp}.
         */
        public sealed class Builder : IWaitForBuilder<WaitForTimestamp>
        {
            [JsonProperty(PropertyNames.TIMESTAMP)]
            private DateTime _timestamp;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link WaitForTimestamp} object.
             */
            public WaitForTimestamp Build()
            {
                return new WaitForTimestamp
                       {
                           Timestamp = _timestamp
                       };
            }

            /**
             * REQUIRED. Sets the date that this state should wait until before proceeding.
             *
             * @param time stamp Date to wait until.
             * @return This object for method chaining.
             */
            public Builder Timestamp(DateTime timestamp)
            {
                _timestamp = timestamp;
                return this;
            }
        }
    }
}