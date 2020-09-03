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

namespace StatesLanguage.Model.States
{
    /// <summary>
    ///     Interface for different waiting strategies used by <see cref="WaitState" />
    /// </summary>
    public interface IWaitFor
    {
    }

    public interface IWaitForBuilder<out T> : IBuildable<T> where T : IWaitFor
    {
    }

    public class NullWaitForBuilder : IWaitForBuilder<IWaitFor>
    {
        public static readonly NullWaitForBuilder Instance = new NullWaitForBuilder();

        private NullWaitForBuilder()
        {
        }

        public IWaitFor Build()
        {
            return null;
        }
    }
}