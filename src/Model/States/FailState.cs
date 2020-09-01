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
using StatesLanguage.Interfaces;

namespace StatesLanguage.Model.States
{
    public class FailState : State
    {
        private FailState()
        {
        }

        [JsonProperty(PropertyNames.ERROR)]
        public string Error { get; set; }

        [JsonProperty(PropertyNames.CAUSE)]
        public string Cause { get; set; }

        public override StateType Type => StateType.Fail;
        public override bool IsTerminalState => true;

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /**
         * @return Builder instance to construct a {@link FailState}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link FailState}.
         */
        public sealed class Builder : IBuilder<FailState>
        {
            [JsonProperty(PropertyNames.CAUSE)]
            private string _cause;

            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.ERROR)]
            private string _error;

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link FailState} object.
             */
            public FailState Build()
            {
                return new FailState
                       {
                           Comment = _comment,
                           Error = _error,
                           Cause = _cause
                       };
            }

            /**
             * OPTIONAL. Human readable description for the state.
             *
             * @param comment New comment.
             * @return This object for method chaining.
             */
            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
            }

            /**
             * REQUIRED. Error code that can be referenced in {@link Retrier}s or {@link Catcher}s and can also be used for
             * diagnostic
             * purposes.
             *
             * @param error Error code value.
             * @return This object for method chaining.
             */
            public Builder Error(string error)
            {
                _error = error;
                return this;
            }

            /**
             * REQUIRED. Human readable message describing the failure. Used for diagnostic purposes only.
             *
             * @param cause Cause description.
             * @return This object for method chaining.
             */
            public Builder Cause(string cause)
            {
                _cause = cause;
                return this;
            }
        }
    }
}