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
using StatesLanguage.Model.States;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.Conditions
{
    /// <summary>
    ///     Represents the logical NOT of a single condition. May be used in a <see cref="Choice" />.
    /// </summary>
    public sealed class NotCondition : ICondition
    {
        private NotCondition()
        {
        }

        [JsonProperty(PropertyNames.NOT)]
        public ICondition Condition { get; private set; }


        /// <returns>Builder instance to construct a  <see cref="NotCondition" />.</returns>
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link NotCondition}.
         */
        public sealed class Builder : IConditionBuilder<NotCondition>
        {
            private IBuildable<ICondition> _condition = NullConditionBuilder.Instance;

            /**
             * @return An immutable {@link NotCondition} object.
             */
            public NotCondition Build()
            {
                return new NotCondition
                       {
                           Condition = _condition.Build()
                       };
            }

            /// <summary>
            ///     Sets the condition to be negated. May be another composite condition or a simple condition.
            ///     Note that the {@link Condition} object is not built until the <see cref="NotCondition" /> is built so any
            ///     modifications on the state model will be reflected in this object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="conditionBuilder">conditionBuilder Instance of {@link Condition.Builder}.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Condition<T>(IConditionBuilder<T> conditionBuilder) where T : ICondition
            {
                _condition = (IBuildable<ICondition>) conditionBuilder;
                return this;
            }
        }

        public bool Match(JObject input)
        {
            return !Condition.Match(input);
        }
    }
}