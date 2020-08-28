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
    // {@link WaitFor} implementation that can be used in a {@link WaitState}. Corresponds to the
    //"{@value PropertyNames#SECONDS_PATH}" field in the JSON document.
    /// <summary>
    /// </summary>
    public class WaitForSecondsPath : IWaitFor
    {
        private WaitForSecondsPath()
        {
        }

        public string SecondsPath { get; private set; }

        /**
         * @return Builder instance to construct a {@link WaitForSecondsPath}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link WaitForSecondsPath}.
         */
        public sealed class Builder : IWaitForBuilder<WaitForSecondsPath>
        {
            [JsonProperty(PropertyNames.SECONDS_PATH)]
            private string _secondsPath;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link WaitForSecondsPath} object.
             */
            public WaitForSecondsPath Build()
            {
                return new WaitForSecondsPath
                       {
                           SecondsPath = _secondsPath
                       };
            }

            /**
             * REQUIRED. Sets the path to a number in the input to this state representing
             * the number of seconds to wait.
             *
             * @param secondsPath Reference path to seconds in the input.
             * @return This object for method chaining.
             */
            public Builder SecondsPath(ReferencePath secondsPath)
            {
                _secondsPath = secondsPath;
                return this;
            }
        }
    }
}