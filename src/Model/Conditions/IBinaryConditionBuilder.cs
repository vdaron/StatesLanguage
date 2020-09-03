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

namespace StatesLanguage.Model.Conditions
{
    public interface IBinaryConditionBuilder<out TBuilder, out TCondition, in TValue> : IConditionBuilder<TCondition>
        where TCondition : ICondition
        where TBuilder : IBinaryConditionBuilder<TBuilder, TCondition, TValue>
    {
        string Type { get; }

        TBuilder Variable(string variable);
        TBuilder ExpectedValue(TValue expectedValue);
    }

    public interface IBinaryConditionPathBuilder<out TBuilder, out TCondition> : IConditionBuilder<TCondition>
        where TCondition : ICondition
        where TBuilder : IBinaryConditionPathBuilder<TBuilder, TCondition>
    {
        string Type { get; }
        TBuilder Variable(string variable);
        TBuilder ExpectedValuePath(string expectedValuePath);
    }
}