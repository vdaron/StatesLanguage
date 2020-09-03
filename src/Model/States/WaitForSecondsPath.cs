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
    ///     Corresponds to the <see cref="PropertyNames.SECONDS_PATH" /> field in the JSON document.
    /// </summary>
    public class WaitForSecondsPath : IWaitFor
    {
        private WaitForSecondsPath()
        {
        }

        /// <summary>
        ///     Path to a number in the input to this state representing
        /// </summary>
        public string SecondsPath { get; private set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IWaitForBuilder<WaitForSecondsPath>
        {
            [JsonProperty(PropertyNames.SECONDS_PATH)]
            private string _secondsPath;

            internal Builder()
            {
            }

            public WaitForSecondsPath Build()
            {
                return new WaitForSecondsPath
                {
                    SecondsPath = _secondsPath
                };
            }

            /// <summary>
            ///     REQUIRED. Sets the path to a number in the input to this state representing
            ///     the number of seconds to wait.
            /// </summary>
            /// <param name="secondsPath">Reference path to seconds in the input.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder SecondsPath(ReferencePath secondsPath)
            {
                _secondsPath = secondsPath;
                return this;
            }
        }
    }
}