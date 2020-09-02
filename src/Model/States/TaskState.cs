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

        [JsonProperty(PropertyNames.TIMEOUT_SECONDS_PATH)]
        public string TimeoutSecondPath { get; private set; }

        [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
        public int? TimeoutSeconds { get; private set; }
        
        [JsonProperty(PropertyNames.HEARTBEAT_SECONDS)]
        public int? HeartbeatSeconds { get; private set; }
        
        [JsonProperty(PropertyNames.HEARTBEAT_SECONDS_PATH)]
        public string HeartbeatSecondsPath { get; private set; }

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
            [JsonProperty(PropertyNames.HEARTBEAT_SECONDS)]
            private int? _heartbeatSeconds;
            
            [JsonProperty(PropertyNames.HEARTBEAT_SECONDS_PATH)]
            private string _heartbeatSecondsPath;

            [JsonProperty(PropertyNames.RESOURCE)]
            private string _resource;

            [JsonProperty(PropertyNames.TIMEOUT_SECONDS)]
            private int? _timeoutSeconds;
            
            [JsonProperty(PropertyNames.TIMEOUT_SECONDS_PATH)]
            private string _timeoutSecondsPath;

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
            
            public Builder TimeoutSecondsPath(ReferencePath timeoutSecondsPath)
            {
                _timeoutSecondsPath = timeoutSecondsPath;
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
            
            public Builder HeartbeatSecondsPath(ReferencePath heartbeatSecondsPath)
            {
                _heartbeatSecondsPath = heartbeatSecondsPath;
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
                           ResultSelector = _resultSelector,
                           Comment = _comment,
                           TimeoutSeconds = _timeoutSeconds,
                           HeartbeatSeconds = _heartbeatSeconds,
                           HeartbeatSecondsPath = _heartbeatSecondsPath,
                           TimeoutSecondPath = _timeoutSecondsPath,
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}