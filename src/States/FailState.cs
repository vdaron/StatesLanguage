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
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public class FailState : State
    {
        private FailState()
        {
        }

        /// <summary>
        ///     Error code that can be referenced in <see cref="Retrier" />s or <see cref="Catcher" />s and can also be used for
        ///     diagnostic purposes.
        /// </summary>
        [JsonProperty(PropertyNames.ERROR)]
        public string Error { get; protected set; }

        /// <summary>
        ///     Human readable message describing the failure. Used for diagnostic purposes only.
        /// </summary>
        [JsonProperty(PropertyNames.CAUSE)]
        public string Cause { get; protected set; }
        
        /// <summary>
        ///  Json Path to the <see cref="Error"/>
        /// </summary>
        [JsonProperty(PropertyNames.ERROR_PATH)]
        public string ErrorPath { get; protected set; }

        /// <summary>
        ///  json path to the <see cref="Cause"/>
        /// </summary>
        [JsonProperty(PropertyNames.CAUSE_PATH)]
        public string CausePath { get; protected set; }

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
        public sealed class Builder : StateBuilder<FailState, Builder>
        {
            [JsonProperty(PropertyNames.CAUSE)]
            private string _cause;

            [JsonProperty(PropertyNames.ERROR)] 
            private string _error;
            
            [JsonProperty(PropertyNames.CAUSE_PATH)] 
            private string _causePath;
            
            [JsonProperty(PropertyNames.ERROR_PATH)] 
            private string _errorPath;

            internal Builder()
            {
            }

            public override FailState Build()
            {
                if (!string.IsNullOrWhiteSpace(_error) && !string.IsNullOrWhiteSpace(_errorPath))
                    throw new StatesLanguageException("You cannot specify Error and ErrorPath at the same time");
                
                if (!string.IsNullOrWhiteSpace(_cause) && !string.IsNullOrWhiteSpace(_causePath))
                    throw new StatesLanguageException("You cannot specify Cause and CausePath at the same time");

                if (string.IsNullOrWhiteSpace(_error) && string.IsNullOrWhiteSpace(_errorPath))
                    throw new StatesLanguageException("You have to specify at least Error or ErrorPath");
                
                if (string.IsNullOrWhiteSpace(_cause) && string.IsNullOrWhiteSpace(_causePath))
                    throw new StatesLanguageException("You have to specify at least Cause and CausePath");
                
                return new FailState
                {
                    Comment = _comment,
                    Error = _error,
                    ErrorPath = _errorPath,
                    Cause = _cause,
                    CausePath = _causePath
                };
            }

            /// <summary>
            ///     REQUIRED. Error code that can be referenced in <see cref="Retrier" />s or <see cref="Catcher" />s and can also be
            ///     used for
            ///     diagnostic purposes.
            /// </summary>
            /// <param name="error">Error code value</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Error(string error)
            {
                _error = error;
                return this;
            }

            /// <summary>
            ///     REQUIRED. Human readable message describing the failure. Used for diagnostic purposes only.
            /// </summary>
            /// <param name="cause">Cause description.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Cause(string cause)
            {
                _cause = cause;
                return this;
            }
            
            
            /// <summary>
            /// Json Path to the <see cref="Error"/>
            /// </summary>
            /// <param name="errorPath">Error code value</param>
            /// <returns>This object for method chaining.</returns>
            public Builder ErrorPath(string errorPath)
            {
                _errorPath = errorPath;
                return this;
            }

            /// <summary>
            /// Json Path to the <see cref="Cause"/>
            /// </summary>
            /// <param name="causePath">Cause description.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder CausePath(string causePath)
            {
                _causePath = causePath;
                return this;
            }
        }
    }
}