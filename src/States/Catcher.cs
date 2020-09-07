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

using System.Collections.Generic;
using Newtonsoft.Json;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public class Catcher
    {
        private Catcher()
        {
        }

        [JsonProperty(PropertyNames.ERROR_EQUALS)]
        public List<string> ErrorEquals { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public OptionalString ResultPath { get; private set; }

        [JsonIgnore]
        public ITransition Transition { get; private set; }

        /**
         * @return Builder instance to construct a {@link Catcher}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IBuildable<Catcher>
        {
            [JsonProperty(PropertyNames.ERROR_EQUALS)]
            private List<string> _errorEquals = new List<string>();

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private OptionalString _resultPath;

            [JsonIgnore] private ITransitionBuilder<NextStateTransition> _transition =
                NullTransitionBuilder<NextStateTransition>.Instance;

            [JsonProperty(PropertyNames.NEXT)]
            private string NextStateName
            {
                set => Transition(NextStateTransition.GetBuilder().NextStateName(value));
            }

            /**
             * @return An immutable {@link Catcher} object.
             */
            public Catcher Build()
            {
                return new Catcher
                {
                    ErrorEquals = new List<string>(_errorEquals),
                    ResultPath = _resultPath,
                    Transition = _transition.Build()
                };
            }

            /// <summary>
            ///     Adds to the error codes that this catcher handles. If the catcher matches an error code then the state machine
            ///     transitions to the state identified by {@link #nextStateName(String)}.
            /// </summary>
            /// <param name="errorEquals">New error codes to add to this catchers handled errors.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder ErrorEquals(params string[] errorEquals)
            {
                _errorEquals.AddRange(errorEquals);
                return this;
            }

            /// <summary>
            ///     Makes this catcher handle all errors. This method should not be used with {@link #errorEquals}.
            /// </summary>
            /// <returns>This object for method chaining.</returns>
            public Builder CatchAll()
            {
                _errorEquals.Clear();
                _errorEquals.Add(ErrorCodes.ALL);
                return this;
            }

            /// <summary>
            ///     JSON Path expression that can be used to combine the error output with the input to the state. If
            ///     not specified the result will solely consist of the error output. See
            ///     <a href="https://states-language.net/spec.html#filters">
            ///         https://states-language.net/spec.html#filters
            ///     </a>
            ///     for more information.
            /// </summary>
            /// <param name="resultPath"></param>
            /// <returns>This object for method chaining.</returns>
            public Builder ResultPath(OptionalString resultPath)
            {
                _resultPath = resultPath;
                return this;
            }

            /// <summary>
            ///     Sets the transition that will occur if this catcher is evaluated. Currently only supports transitioning to another
            ///     state.
            /// </summary>
            /// <param name="transition">New transition.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Transition(NextStateTransition.Builder transition)
            {
                _transition = transition;
                return this;
            }
        }
    }
}