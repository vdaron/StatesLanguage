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
    public class WaitForTimestampPath : IWaitFor
    {
        private WaitForTimestampPath()
        {
        }

        public string TimestampPath { get; private set; }

        /**
         * @return Builder instance to construct a {@link WaitForTimestampPath}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link WaitForTimestampPath}.
         */
        public sealed class Builder : IWaitForBuilder<WaitForTimestampPath>
        {
            [JsonProperty(PropertyNames.TIMESTAMP_PATH)]
            private string _timestampPath;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link WaitForTimestampPath} object.
             */
            public WaitForTimestampPath Build()
            {
                return new WaitForTimestampPath
                       {
                           TimestampPath = _timestampPath
                       };
            }

            /**
             * REQUIRED. Sets the path to a date in the input to this state. The {@link WaitState} will wait until the date specified
             * in the input. Date in input must be in the format described
             * <a href="https://states-language.net/spec.html#timestamps">here</a>.
             *
             * @param timestampPath Reference path to date in the input.
             * @return This object for method chaining.
             */
            public Builder TimestampPath(string timestampPath)
            {
                _timestampPath = timestampPath;
                return this;
            }
        }
    }
}