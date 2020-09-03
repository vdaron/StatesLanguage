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

namespace StatesLanguage.Model.Conditions
{
    /**
     * Binary condition for String equal comparison.
     * 
     * @see
     * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
     * @see Choice
     */
    public sealed class StringEqualsCondition : BinaryCondition<string>
    {
        private StringEqualsCondition() : base(Operator.Eq)
        {
        }

        [JsonProperty(PropertyNames.STRING_EQUALS)]
        public override string ExpectedValue { get; protected set; }

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
    }
}