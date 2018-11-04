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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.Conditions
{
    /**
     * Binary condition for Numeric equality comparison. Supports both integral and floating point numeric types.
     *
     * @see <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
     * @see Choice
     */
    public sealed class TimestampGreaterThanCondition : IBinaryCondition<DateTime>
    {
        private TimestampGreaterThanCondition()
        {
        }

        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; private set; }

        [JsonProperty(PropertyNames.TIMESTAMP_GREATER_THAN)]
        public DateTime ExpectedValue { get; private set; }

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
        public sealed class Builder : IBinaryConditionBuilder<Builder, TimestampGreaterThanCondition, DateTime>
        {
            private DateTime _expectedValue;
            
            private string _variable;

            internal Builder()
            {
            }

            public string Type => PropertyNames.TIMESTAMP_GREATER_THAN;

            /**
             * Sets the expected value for this condition.
             *
             * @param expectedValue Expected value.
             * @return This object for method chaining.
             */
            public Builder ExpectedValue(DateTime expectedValue)
            {
                _expectedValue = expectedValue;
                return this;
            }

            /**
             * @return An immutable {@link NumericEqualsCondition} object.
             */
            public TimestampGreaterThanCondition Build()
            {
                return new TimestampGreaterThanCondition
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
                return input.SelectToken(Variable)?.Value<DateTime>().CompareTo(ExpectedValue) > 0;
            }
            catch (FormatException e)
            {
                return false;
            }
        }
    }
}