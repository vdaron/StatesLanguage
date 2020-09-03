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
    ///     Corresponds to the <see cref="PropertyNames.TIMESTAMP_PATH" /> field in the JSON document.
    /// </summary>
    public class WaitForTimestampPath : IWaitFor
    {
        private WaitForTimestampPath()
        {
        }

        /// <summary>
        ///     Path to a date in the input to this state.The {@link WaitState} will wait until the date specified
        ///     in the input.
        /// </summary>
        public string TimestampPath { get; private set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IWaitForBuilder<WaitForTimestampPath>
        {
            [JsonProperty(PropertyNames.TIMESTAMP_PATH)]
            private string _timestampPath;

            internal Builder()
            {
            }

            public WaitForTimestampPath Build()
            {
                return new WaitForTimestampPath
                {
                    TimestampPath = _timestampPath
                };
            }

            /// <summary>
            ///     REQUIRED. Sets the path to a date in the input to this state. The <see cref="WaitState" /> will wait until the date
            ///     specified
            ///     in the input. Date in input must be in the format described
            ///     <a href="https://states-language.net/spec.html#timestamps">here</a>.
            /// </summary>
            /// <param name="timestampPath"> Reference path to date in the input.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder TimestampPath(ReferencePath timestampPath)
            {
                _timestampPath = timestampPath;
                return this;
            }
        }
    }
}