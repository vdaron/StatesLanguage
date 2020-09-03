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

using Newtonsoft.Json;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    /// <summary>
    ///     <see cref="IWaitFor" /> implementation that can be used in a <see cref="WaitState" />.
    ///     Corresponds to the <see cref="PropertyNames.SECONDS" /> field in the JSON document.
    /// </summary>
    public class WaitForSeconds : IWaitFor
    {
        private WaitForSeconds()
        {
        }

        /// <summary>
        ///     number of seconds the <see cref="WaitState" /> will wait for.
        /// </summary>
        public int Seconds { get; private set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IWaitForBuilder<WaitForSeconds>
        {
            [JsonProperty(PropertyNames.SECONDS)] private int _seconds;

            internal Builder()
            {
            }

            public WaitForSeconds Build()
            {
                return new WaitForSeconds
                {
                    Seconds = _seconds
                };
            }

            /// <summary>
            ///     REQUIRED. Sets the number of seconds the <see cref="WaitState" /> will wait for.
            /// </summary>
            /// <param name="seconds">Number of seconds. Must be positive.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Seconds(int seconds)
            {
                _seconds = seconds;
                return this;
            }
        }
    }
}