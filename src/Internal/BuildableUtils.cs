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
using System.Linq;
using StatesLanguage.States;

namespace StatesLanguage.Internal
{
    public static class BuildableUtils
    {
        /// <summary>
        ///     Converts the List of <see cref="IBuildable{T}" />s into a List of the built object.
        /// </summary>
        /// <typeparam name="T">  Type of object being built</typeparam>
        /// <param name="buildables">buildableList List of <see cref="IBuildable{T}" />s to build.</param>
        /// <returns> Unmodifiable list of built objects.</returns>
        public static List<T> Build<T>(IEnumerable<IBuildable<T>> buildables)
        {
            return buildables.Select(buildable => buildable.Build()).ToList();
        }

        /// <summary>
        ///     Converts the Map of <see cref="IBuildable{T}" />s into a Map of the built object.
        /// </summary>
        /// <typeparam name="T"> Key type</typeparam>
        /// <param name="buildableMap">buildableMap Map where values are {@link Buildable}s.</param>
        /// <returns> Unmodifiable map of built objects.</returns>
        public static Dictionary<string, T> Build<T>(Dictionary<string, State.IBuilder<T>> buildableMap) where T : State
        {
            var builtMap = new Dictionary<string, T>();
            foreach (var entry in buildableMap)
            {
                builtMap.Add(entry.Key, entry.Value.Build());
            }

            return builtMap;
        }
    }
}