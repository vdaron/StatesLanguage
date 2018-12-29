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
    public class ParallelState : TransitionState
    {
        private ParallelState()
        {
        }

        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; private set; }

        [JsonProperty(PropertyNames.INPUT_PATH)]
        public string InputPath { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public string ResultPath { get; private set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public string OutputPath { get; private set; }

        [JsonProperty(PropertyNames.PARAMETERS)]
        public JToken Parameters { get; private set; }
        
        [JsonIgnore]
        public List<Branch> Branches { get; private set; }

        [JsonProperty(PropertyNames.RETRY)]
        public List<Retrier> Retriers { get; private set; }

        [JsonProperty(PropertyNames.CATCH)]
        public List<Catcher> Catchers { get; private set; }

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

        public sealed class Builder : TransitionStateBuilder<ParallelState, Builder>
        {
            [JsonProperty(PropertyNames.BRANCHES)]
            private List<Branch.Builder> _branches = new List<Branch.Builder>();

            [JsonProperty(PropertyNames.CATCH)]
            private List<Catcher.Builder> _catchers = new List<Catcher.Builder>();

            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private string _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private string _outputPath;

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private string _resultPath;

            [JsonProperty(PropertyNames.PARAMETERS)]
            private JToken _parameters;

            [JsonProperty(PropertyNames.RETRY)]
            private List<Retrier.Builder> _retriers = new List<Retrier.Builder>();

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
            public Builder Branch(Branch.Builder branchBuilder)
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
            public Builder Branches(params Branch.Builder[] branchBuilders)
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
            public Builder ResultPath(string resultPath)
            {
                _resultPath = resultPath;
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

            public Builder Parameters(JToken parameters)
            {
                _parameters = parameters;
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
             * OPTIONAL. Adds the {@link Retrier}s to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilders Instances of {@link Retrier.Builder}. Note that the {@link
             *                        Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Retriers(params Retrier.Builder[] retrierBuilders)
            {
                _retriers.AddRange(retrierBuilders);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Retrier} to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilder Instance of {@link Retrier.Builder}. Note that the {@link
             *                       Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Retrier(Retrier.Builder retrierBuilder)
            {
                _retriers.Add(retrierBuilder);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Catcher}s to this states catchers.  If a single branch fails then the entire parallel state
             * is considered failed and eligible to be caught.
             *
             * @param catcherBuilders Instances of {@link Catcher.Builder}. Note that the {@link
             *                        Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Catchers(params Catcher.Builder[] catcherBuilders)
            {
                _catchers.AddRange(catcherBuilders);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Catcher} to this states catchers.  If a single branch fails then the entire parallel state
             * is
             * considered failed and eligible to be caught.
             *
             * @param catcherBuilder Instance of {@link Catcher.Builder}. Note that the {@link
             *                       Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Catcher(Catcher.Builder catcherBuilder)
            {
                _catchers.Add(catcherBuilder);
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
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}