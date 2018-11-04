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
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.Conditions
{
    /**
     * Binary condition for String greater than comparison.
     *
     * @see <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
     * @see Choice
     */
    public sealed class StringEqualsCondition : IBinaryCondition<string>
    {
        private StringEqualsCondition()
        {
        }

        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; private set; }

        [JsonProperty(PropertyNames.STRING_EQUALS)]
        public string ExpectedValue { get; private set; }

        /**
         * @return Builder instance to construct a {@link StringGreaterThanCondition}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link StringGreaterThanCondition}.
         */
        public sealed class Builder : IBinaryConditionBuilder<Builder, StringEqualsCondition, string>
        {
            private string _expectedValue;
            private string _variable;

            public string Type => PropertyNames.STRING_EQUALS;

            /**
             * Sets the expected value for this condition.
             *
             * @param expectedValue Expected value.
             * @return This object for method chaining.
             */
            public Builder ExpectedValue(string expectedValue)
            {
                _expectedValue = expectedValue;
                return this;
            }

            /**
             * @return An immutable {@link StringGreaterThanCondition} object.
             */
            public StringEqualsCondition Build()
            {
                return new StringEqualsCondition
                       {
                           Variable = _variable,
                           ExpectedValue = _expectedValue
                       };
            }

            /**
             * Sets the JSONPath expression that determines which piece of the input document is used for the comparison.
             *
             * @param variable Reference path.
             * @return This object for method chaining.
             */

            public Builder Variable(string variable)
            {
                _variable = variable;
                return this;
            }
        }

        public bool Match(JObject input)
        {
            return input.SelectToken(Variable)?.Value<string>()?.CompareTo(ExpectedValue) == 0;
        }
    }
}