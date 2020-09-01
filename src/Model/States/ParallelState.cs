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

            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private OptionalString _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private OptionalString _outputPath;

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private OptionalString _resultPath;

            [JsonProperty(PropertyNames.PARAMETERS)]
            private JObject _parameters;
            
            [JsonProperty(PropertyNames.RESULT_SELECTOR)]
            private JObject _resultSelector;
            
            private ITransitionBuilder<ITransition> _transition = NullTransitionBuilder<ITransition>.Instance;

            internal Builder()
            {
            }

            /**
             * OPTIONAL. Human readable description for the state.
             *
             * @param comment New comment.
             * @return This object for method chaining.
             */
            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
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
             * OPTIONAL. The value of “InputPath” MUST be a Path, which is applied to a State’s raw input to select some or all of
             * it;
             * that selection is used by the state. If not provided then the whole output from the previous state is used as input to
             * this state.
             *
             * @param inputPath New path value.
             * @return This object for method chaining.
             */
            public Builder InputPath(string inputPath)
            {
                _inputPath = inputPath;
                return this;
            }

            /**
             * OPTIONAL. The value of “ResultPath” MUST be a Reference Path, which specifies the combination with or replacement of
             * the state’s result with its raw input. If not provided then the output completely replaces the input.
             *
             * @param resultPath New path value.
             * @return This object for method chaining.
             */
            public Builder ResultPath(ReferencePath resultPath)
            {
                _resultPath = resultPath.Path;
                return this;
            }

            /**
             * OPTIONAL. The value of “OutputPath” MUST be a path, which is applied to the state’s output after the application of
             * ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
             * used.
             *
             * @param outputPath New path value.
             * @return This object for method chaining.
             */
            public Builder OutputPath(string outputPath)
            {
                _outputPath = outputPath;
                return this;
            }

            public Builder Parameters(JObject parameters)
            {
                _parameters = parameters;
                return this;
            }
            
            public Builder ResultSelector(JObject resultSelector)
            {
                _resultSelector = resultSelector;
                return this;
            }

            /**
             * REQUIRED. Sets the transition that will occur when all branches in this parallel
             * state have executed successfully.
             *
             * @param transition New transition.
             * @return This object for method chaining.
             */
            public override Builder Transition<U>(ITransitionBuilder<U> transition)
            {
                _transition = (ITransitionBuilder<ITransition>) transition;
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