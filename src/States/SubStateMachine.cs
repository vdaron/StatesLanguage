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
using Newtonsoft.Json;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    /// <summary>
    ///     A single branch of parallel execution in a state machine. See <see cref="ParallelState" />.
    /// </summary>
    public class SubStateMachine
    {
        private SubStateMachine()
        {
        }

        public string StartAt { get; private set; }
        public string Comment { get; private set; }
        public Dictionary<string, State> States { get; private set; }

        /// <returns>Builder instance to construct a <see cref="SubStateMachine" />.</returns>
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IBuildable<SubStateMachine>
        {
            [JsonProperty(PropertyNames.COMMENT)] private string _comment;

            [JsonProperty(PropertyNames.START_AT)] private string _startAt;

            [JsonProperty(PropertyNames.STATES)] private Dictionary<string, State.IBuilder<State>> _stateBuilders =
                new Dictionary<string, State.IBuilder<State>>();

            internal Builder()
            {
            }

            /// <summary>
            ///     An immutable <see cref="SubStateMachine" /> object.
            /// </summary>
            /// <returns></returns>
            public SubStateMachine Build()
            {
                return new SubStateMachine
                {
                    StartAt = _startAt,
                    Comment = _comment,
                    States = BuildableUtils.Build(_stateBuilders)
                };
            }

            /// <summary>
            ///     REQUIRED. Name of the state to start branch execution at. Must match a state name provided via States.
            /// </summary>
            /// <param name="startAt">startAt Name of starting state.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder StartAt(string startAt)
            {
                _startAt = startAt;
                return this;
            }

            /// <summary>
            ///     OPTIONAL. Human readable description for the state machine.
            /// </summary>
            /// <param name="comment">Comment</param>
            /// <returns>This object for method chaining.</returns>
            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
            }

            /// <summary>
            ///     REQUIRED. Adds a new state to the branch. A branch MUST have at least one state.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="stateName"> Name of the state</param>
            /// <param name="stateBuilder">
            ///     Instance of {@link Builder}. Note that the {@link State} object is not built until the {@link Branch} is
            ///     built so any modifications on the state model will be reflected in this object.
            /// </param>
            /// <returns> This object for method chaining.</returns>
            public Builder State<T>(string stateName, State.IBuilder<T> stateBuilder) where T : State
            {
                _stateBuilders.Add(stateName, stateBuilder);
                return this;
            }
        }
    }
}