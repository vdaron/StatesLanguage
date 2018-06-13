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
    /// <summary>
    ///     * {@link WaitFor} implementation that can be used in a {@link WaitState}. Corresponds to the "{@value
    ///     PropertyNames#SECONDS}"
    ///     field in the JSON document.
    /// </summary>
    public class WaitForSeconds : IWaitFor
    {
        private WaitForSeconds()
        {
        }

        public int Seconds { get; private set; }

        /**
         * @return Builder instance to construct a {@link WaitForSeconds}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link WaitForSeconds}.
         */
        public sealed class Builder : IWaitForBuilder<WaitForSeconds>
        {
            [JsonProperty(PropertyNames.SECONDS)]
            private int _seconds;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link WaitForSeconds} object.
             */
            public WaitForSeconds Build()
            {
                return new WaitForSeconds
                       {
                           Seconds = _seconds
                       };
            }

            /**
             * REQUIRED. Sets the number of seconds the {@link WaitState} will wait for.
             *
             * @param seconds Number of seconds. Must be positive.
             * @return This object for method chaining.
             */
            public Builder Seconds(int seconds)
            {
                _seconds = seconds;
                return this;
            }
        }
    }
}