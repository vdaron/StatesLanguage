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
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.States
{
    public class ParallelState : RetryCatchState
    {
        private ParallelState()
        {
        }

        [JsonIgnore]
        public List<SubStateMachine> Branches { get; private set; }

        public override StateType Type => StateType.Parallel;

        public override bool IsTerminalState => Transition.IsTerminal;

        /**
         * @return Builder instance to construct a {@link ParallelState}.
         */
        internal static Builder GetBuilder()
        {
            return new Builder();
        }

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public sealed class Builder : RetryCatchStateBuilder<ParallelState, Builder>
        {
            [JsonProperty(PropertyNames.BRANCHES)]
            private List<SubStateMachine.Builder> _branches = new List<SubStateMachine.Builder>();

            internal Builder()
            {
            }
            
            /**
             * REQUIRED. Adds a new branch of execution to this states branches. A parallel state must have at least one branch.
             *
             * @param branchBuilder Instance of {@link Branch.Builder}. Note that the {@link
             *                      Branch} object is not built until the {@link ParallelState} is built so any modifications on the
             *                      state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Branch(SubStateMachine.Builder branchBuilder)
            {
                _branches.Add(branchBuilder);
                return this;
            }

            /**
             * REQUIRED. Adds the branches of execution to this states branches. A parallel state must have at least one branch.
             *
             * @param branchBuilders Instances of {@link Branch.Builder}. Note that the {@link
             *                       Branch} object is not built until the {@link ParallelState} is built so any modifications on the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Branches(params SubStateMachine.Builder[] branchBuilders)
            {
                _branches.AddRange(branchBuilders);
                return this;
            }
            
            /**
             * @return An immutable {@link ParallelState} object.
             */
            public override ParallelState Build()
            {
                return new ParallelState()
                       {
                           Comment = _comment,
                           Branches = BuildableUtils.Build(_branches),
                           InputPath = _inputPath,
                           ResultPath = _resultPath,
                           OutputPath = _outputPath,
                           Parameters = _parameters,
                           ResultSelector = _resultSelector,
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}