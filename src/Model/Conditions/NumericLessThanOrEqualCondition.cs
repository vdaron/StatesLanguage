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
using System.Globalization;
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.Conditions
{
    /**
     * Binary condition for Numeric greather than comparison. Supports both integral and floating point numeric types.
     *
     * @see <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
     * @see Choice
     */
    public sealed class NumericLessThanOrEqualCondition<T> : IBinaryCondition<T>
        where T : IComparable<T>
    {
        private NumericLessThanOrEqualCondition()
        {
        }

        [JsonProperty(PropertyNames.NUMERIC_LESS_THAN_EQUALS)]
        public T ExpectedValue { get; private set; }

        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; private set; }

        /**
         * @return Builder instance to construct a {@link NumericEqualsCondition}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link NumericEqualsCondition}.
         */
        public sealed class Builder : IBinaryConditionBuilder<Builder, NumericLessThanOrEqualCondition<T>,T>
        {
            private T _expectedValue;
            private string _variable;

            internal Builder()
            {
            }

            public string Type => PropertyNames.NUMERIC_LESS_THAN_EQUALS;

            /**
             * Sets the expected value for this condition.
             *
             * @param expectedValue Expected value.
             * @return This object for method chaining.
             */
            public Builder ExpectedValue(T expectedValue)
            {
                _expectedValue = expectedValue;
                return this;
            }


            /**
             * @return An immutable {@link NumericEqualsCondition} object.
             */
            public NumericLessThanOrEqualCondition<T> Build()
            {
                return new NumericLessThanOrEqualCondition<T>
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
            try
            {
                return input.SelectToken(Variable)?.Value<T>().CompareTo(ExpectedValue) <= 0;
            }
            catch (FormatException e)
            {
                return false;
            }
        }
    }
}