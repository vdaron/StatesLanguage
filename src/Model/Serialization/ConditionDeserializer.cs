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

using Newtonsoft.Json.Linq;
using StatesLanguage.Model.Conditions;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.Serialization
{
    /// <summary>
    ///     Custom deserializer for a <see cref="ICondition" />
    /// </summary>
    internal class ConditionDeserializer
    {
        public IConditionBuilder<ICondition> DeserializeCondition(JObject node)
        {
            if (node.Property(PropertyNames.VARIABLE) != null)
            {
                if (node.Property(PropertyNames.STRING_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(StringEqualsCondition.GetBuilder(), node);
                }
                
                if (node.Property(PropertyNames.STRING_EQUALS_PATH) != null)
                {
                    return DeserializeBinaryPathCondition(StringEqualsPathCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.STRING_GREATER_THAN) != null)
                {
                    return DeserializeBinaryCondition(StringGreaterThanCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.STRING_GREATER_THAN_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(StringGreaterThanOrEqualCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.STRING_LESS_THAN) != null)
                {
                    return DeserializeBinaryCondition(StringLessThanCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.STRING_LESS_THAN_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(StringLessThanOrEqualCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.TIMESTAMP_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(TimestampEqualCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.TIMESTAMP_GREATER_THAN) != null)
                {
                    return DeserializeBinaryCondition(TimestampGreaterThanCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.TIMESTAMP_GREATER_THAN_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(TimestampGreaterThanOrEqualCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.TIMESTAMP_LESS_THAN) != null)
                {
                    return DeserializeBinaryCondition(TimestampLessThanCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.TIMESTAMP_LESS_THAN_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(TimestampLessThanOrEqualCondition.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.NUMERIC_EQUALS) != null)
                {
                    if (node.Property(PropertyNames.NUMERIC_EQUALS).Value.Type == JTokenType.Integer)
                        return DeserializeBinaryCondition(NumericEqualsCondition<long>.GetBuilder(), node);
                    else
                        return DeserializeBinaryCondition(NumericEqualsCondition<decimal>.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.NUMERIC_GREATER_THAN) != null)
                {
                    if (node.Property(PropertyNames.NUMERIC_GREATER_THAN).Value.Type == JTokenType.Integer)
                        return DeserializeBinaryCondition(NumericGreaterThanCondition<long>.GetBuilder(), node);
                    else
                        return DeserializeBinaryCondition(NumericGreaterThanCondition<decimal>.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.NUMERIC_GREATER_THAN_EQUALS) != null)
                {
                    if (node.Property(PropertyNames.NUMERIC_GREATER_THAN_EQUALS).Value.Type == JTokenType.Integer)
                        return DeserializeBinaryCondition(NumericGreaterThanOrEqualCondition<long>.GetBuilder(), node);
                    else
                        return DeserializeBinaryCondition(NumericGreaterThanOrEqualCondition<decimal>.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.NUMERIC_LESS_THAN) != null)
                {
                    if (node.Property(PropertyNames.NUMERIC_LESS_THAN).Value.Type == JTokenType.Integer)
                        return DeserializeBinaryCondition(NumericLessThanCondition<long>.GetBuilder(), node);
                    else
                        return DeserializeBinaryCondition(NumericLessThanCondition<decimal>.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.NUMERIC_LESS_THAN_EQUALS) != null)
                {
                    if (node.Property(PropertyNames.NUMERIC_LESS_THAN_EQUALS).Value.Type == JTokenType.Integer)
                        return DeserializeBinaryCondition(NumericLessThanOrEqualCondition<long>.GetBuilder(), node);
                    else
                        return DeserializeBinaryCondition(NumericLessThanOrEqualCondition<decimal>.GetBuilder(), node);
                }

                if (node.Property(PropertyNames.BOOLEAN_EQUALS) != null)
                {
                    return DeserializeBinaryCondition(BooleanEqualsCondition.GetBuilder(), node);
                }
            }
            else if (node.Property(PropertyNames.AND) != null)
            {
                var builder = AndCondition.GetBuilder();
                foreach (var inner in node[PropertyNames.AND].Children())
                {
                    builder.Condition(DeserializeCondition((JObject) inner));
                }

                return builder;
            }
            else if (node.Property(PropertyNames.OR) != null)
            {
                var builder = OrCondition.GetBuilder();
                foreach (var inner in node[PropertyNames.OR].Children())
                {
                    builder.Condition(DeserializeCondition((JObject) inner));
                }

                return builder;
            }
            else if (node.Property(PropertyNames.NOT) != null)
            {
                return NotCondition.GetBuilder()
                    .Condition(DeserializeCondition((JObject) node.Property(PropertyNames.NOT).First));
            }

            throw new StatesLanguageException("Condition must be provided");
        }

        private TBuilder DeserializeBinaryCondition<TBuilder, TValue>(IBinaryConditionBuilder<TBuilder, ICondition, TValue> builder,
                                                                      JObject node)
        where TBuilder : IBinaryConditionBuilder<TBuilder, ICondition, TValue>
        {
            return builder
                .Variable(node.Property(PropertyNames.VARIABLE).Value.Value<string>())
                .ExpectedValue(node.Property(builder.Type).Value.Value<TValue>());
        }
        
        private TBuilder DeserializeBinaryPathCondition<TBuilder>(IBinaryConditionPathBuilder<TBuilder, ICondition> builder,
            JObject node)
            where TBuilder : IBinaryConditionPathBuilder<TBuilder, ICondition>
        {
            return builder
                .Variable(node.Property(PropertyNames.VARIABLE).Value.Value<string>())
                .ExpectedValuePath(node.Property(builder.Type).Value.Value<string>());
        }
    }
}