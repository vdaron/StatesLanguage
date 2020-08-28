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
using StatesLanguage.Model.Internal;
using StatesLanguage.Model.States;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.Conditions
{
    /// <summary>
    ///     Binary condition for Boolean equality comparison.
    ///     <see cref="Choice" />
    /// </summary>
    public sealed class IsNullCondition : IBinaryCondition
    {
        private IsNullCondition()
        {
        }

        [JsonProperty(PropertyNames.IS_NULL)]
        public bool IsNull { get; private set; }
        
        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; set; }

        public bool Match(JToken token)
        {
            var t = token.SelectToken(Variable)?.Type;
            return IsNull ? t == JTokenType.Null : t != JTokenType.Null;
        }

        /// <returns>Builder instance to construct a <see cref="IsNullCondition" /></returns>
        public static Builder GetBuilder()
        {
            return new Builder();
        }


        /// <summary>
        ///     Builder for a <see cref="IsNullCondition" />.
        /// </summary>
        public sealed class Builder : IBinaryConditionBuilder<Builder, IsNullCondition, bool>
        {
            private bool _expectedValue;
            private string _variable;

            internal Builder()
            {
            }

            public string Type => PropertyNames.IS_NULL;

            /// <summary>
            ///     Sets the expected value for this condition.
            /// </summary>
            /// <param name="expectedValue">Expected value.</param>
            /// <returns> This object for method chaining.</returns>
            public Builder ExpectedValue(bool expectedValue)
            {
                _expectedValue = expectedValue;
                return this;
            }

            /// <returns>An immutable <see cref="BooleanEqualsCondition" /> object.</returns>
            public IsNullCondition Build()
            {
                return new IsNullCondition
                       {
                           Variable = _variable,
                           IsNull = _expectedValue
                       };
            }

            /// <summary>
            ///     Sets the JSONPath expression that determines which piece of the input document is used for the comparison.
            /// </summary>
            /// <param name="variable">variable Reference path.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Variable(string variable)
            {
                _variable = variable;
                return this;
            }
        }
    }
}