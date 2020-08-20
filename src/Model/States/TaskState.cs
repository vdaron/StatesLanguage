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
    public class TaskState : RetryCatchState
    {
        private TaskState()
        {
        }

        [JsonProperty(PropertyNames.RESOURCE)]
        public string Resource { get; private set; }

        [JsonProperty(PropertyNames.INPUT_PATH)]
        public string InputPath { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public string ResultPath { get; private set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public string OutputPath { get; private set; }

        [JsonProperty(PropertyNames.PARAMETERS)]
        public JToken Parameters { get; private set; }

        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; private set; }

        [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
        private int? _timeout;
        [JsonIgnore]
        public int TimeoutSeconds => _timeout ?? 60;

        [JsonProperty(PropertyNames.HEARTBEAT_SECONDS)]
        public int? HeartbeatSeconds { get; private set; }

        [JsonProperty(PropertyNames.TYPE)]
        public override StateType Type => StateType.Task;

        [JsonIgnore]
        public override bool IsTerminalState => Transition.IsTerminal;

        /**
         * @return Builder instance to construct a {@link TaskState}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /**
     * Builder for a {@link TaskState}.
     */
        public sealed class Builder : RetryCatchStateBuilder<TaskState, Builder>
        {
            [JsonProperty(PropertyNames.COMMENT)]
            private string _comment;

            [JsonProperty(PropertyNames.HEARTBEAT_SECONDS)]
            private int? _heartbeatSeconds;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private string _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private string _outputPath;

            [JsonProperty(PropertyNames.PARAMETERS)]
            private JToken _parameters;

            [JsonProperty(PropertyNames.RESOURCE)]
            private string _resource;

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private string _resultPath;

            [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
            private int? _timeoutSeconds;

            [JsonIgnore]
            private ITransitionBuilder<ITransition> _transition;

            internal Builder()
            {
                _transition = NullTransitionBuilder<ITransition>.Instance;
            }

            /**
             * REQUIRED. Sets the resource to be executed by this task. Must be a URI that uniquely identifies the specific task to
             * execute. This may be the ARN of a Lambda function or a States Activity.
             *
             * @param resource URI of resource.
             * @return This object for method chaining.
             */
            public Builder Resource(string resource)
            {
                _resource = resource;
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
             * OPTIONAL. Timeout, in seconds, that a task is allowed to run. If the task execution runs longer than this timeout the
             * execution fails with a {@link ErrorCodes#TIMEOUT} error.
             *
             * @param timeoutSeconds Timeout value.
             * @return This object for method chaining.
             */
            public Builder TimeoutSeconds(int timeoutSeconds)
            {
                _timeoutSeconds = timeoutSeconds;
                return this;
            }

            /**
             * OPTIONAL. Allowed time between "Heartbeats". If the task does not send "Heartbeats" within the timeout then execution
             * fails with a {@link ErrorCodes#TIMEOUT}. If not set then no heartbeats are required. Heartbeats are a more granular
             * way
             * for a task to report it's progress to the state machine.
             *
             * @param heartbeatSeconds Heartbeat value.
             * @return This object for method chaining.
             */
            public Builder HeartbeatSeconds(int heartbeatSeconds)
            {
                _heartbeatSeconds = heartbeatSeconds;
                return this;
            }

            /**
             * REQUIRED. Sets the transition that will occur when the task completes successfully.
             *
             * @param transition New transition.
             * @return This object for method chaining.
             */
            public override Builder Transition<T>(ITransitionBuilder<T> transition)
            {
                _transition = (ITransitionBuilder<ITransition>) transition;
                return this;
            }

            /**
             * @return An immutable {@link TaskState} object.
             */
            public override TaskState Build()
            {
                return new TaskState
                       {
                           Resource = _resource,
                           InputPath = _inputPath,
                           ResultPath = _resultPath,
                           OutputPath = _outputPath,
                           Parameters = _parameters,
                           Comment = _comment,
                           _timeout = _timeoutSeconds,
                           HeartbeatSeconds = _heartbeatSeconds,
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}