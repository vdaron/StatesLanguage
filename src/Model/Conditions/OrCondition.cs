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
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.Conditions
{
    public class OrCondition : INaryCondition
    {
        private OrCondition()
        {
        }

        [JsonProperty(PropertyNames.OR)]
        public IEnumerable<ICondition> Conditions { get; private set; }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IConditionBuilder<OrCondition>
        {
            private readonly IList<IBuildable<ICondition>> _conditions = new List<IBuildable<ICondition>>();

            internal Builder()
            {
            }

            public OrCondition Build()
            {
                return new OrCondition
                       {
                           Conditions = new List<ICondition>(BuildableUtils.Build(_conditions))
                       };
            }

            public Builder Condition(IConditionBuilder<ICondition> conditionBuilder)
            {
                _conditions.Add(conditionBuilder);
                return this;
            }

            public Builder Conditions(params IBuildable<ICondition>[] conditionBuilders)
            {
                foreach (var c in conditionBuilders)
                {
                    _conditions.Add(c);
                }

                return this;
            }
        }
    }
}