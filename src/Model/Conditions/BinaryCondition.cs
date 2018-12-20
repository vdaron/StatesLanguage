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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.Conditions
{
    public enum Operator
    {
        Eq,Gt,Gte,Lt,Lte
    }
    public interface BinaryCondition : ICondition
    {
        string Variable { get; }
    }

    public abstract class BinaryCondition<T> : BinaryCondition where T : IComparable<T>
    {
        private readonly Operator _operator;

        protected BinaryCondition(Operator op)
        {
            _operator = op;
        }

        public abstract T ExpectedValue { get; protected set; }

        /// <inheritdoc />
        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; protected set; }

        /// <inheritdoc />
        public bool Match(JToken token)
        {
            try
            {
                if (!string.IsNullOrEmpty(Variable) && token.Type != JTokenType.Object)
                {
                    return false;
                }

                var tmp = string.IsNullOrEmpty(Variable) ? token : ((JObject)token).SelectToken(Variable);
                if (tmp == null || (tmp.Type != JTokenType.Boolean
                                 && tmp.Type != JTokenType.Date
                                 && tmp.Type != JTokenType.Integer
                                 && tmp.Type != JTokenType.Float
                                 && tmp.Type != JTokenType.String
                                 && tmp.Type != JTokenType.Guid
                                 && tmp.Type != JTokenType.Uri))
                {
                    return false;
                }

                switch (_operator)
                {
                    case Operator.Eq:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) == 0;
                    case Operator.Gt:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) > 0;
                    case Operator.Gte:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) >= 0;
                    case Operator.Lt:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) < 0;
                    case Operator.Lte:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) <= 0;
                }
            }
            catch (FormatException)
            {
            }
            return false;

        }

    }
}