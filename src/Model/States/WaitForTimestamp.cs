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
using Newtonsoft.Json;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    /// <summary>
    ///     <see cref="IWaitFor" /> implementation that can be used in a <see cref="WaitState" />.
    ///     Corresponds to the <see cref="PropertyNames.TIMESTAMP" /> field in the JSON document.
    /// </summary>
    public class WaitForTimestamp : IWaitFor
    {
        private WaitForTimestamp()
        {
        }

        /// <summary>
        ///     Date that this state should wait until before proceeding.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IWaitForBuilder<WaitForTimestamp>
        {
            [JsonProperty(PropertyNames.TIMESTAMP)]
            private DateTime _timestamp;

            internal Builder()
            {
            }

            public WaitForTimestamp Build()
            {
                return new WaitForTimestamp
                {
                    Timestamp = _timestamp
                };
            }

            /// <summary>
            ///     REQUIRED. Sets the date that this state should wait until before proceeding.
            /// </summary>
            /// <param name="timestamp">Date to wait until.</param>
            /// <returns> This object for method chaining.</returns>
            public Builder Timestamp(DateTime timestamp)
            {
                _timestamp = timestamp;
                return this;
            }
        }
    }
}