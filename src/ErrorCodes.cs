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

namespace StatesLanguage
{
    public static class ErrorCodes
    {
        /// <summary>
        /// A wild-card which matches any Error Name.
        /// </summary>
        public const string ALL = "States.ALL";

        /// <summary>
        /// A Task State either ran longer than the “TimeoutSeconds” value, or failed to heartbeat for a time
        /// longer than the “HeartbeatSeconds” value.
        /// </summary>
        public const string TIMEOUT = "States.Timeout";

        /// <summary>
        /// A Task State failed during the execution.
        /// </summary>
        public const string TASK_FAILED = "States.TaskFailed";

        /// <summary>
        /// A Task State failed because it had insufficient privileges to execute the specified code.
        /// </summary>
        public const string PERMISSIONS = "States.Permissions";

        /// <summary>
        /// A Task State’s “ResultPath” field cannot be applied to the input the state received.
        /// </summary>
        public const string RESULT_PATH_MATCH_FAILURE = "States.ResultPathMatchFailure";

        public const string PARAMETER_PATH_FAILURE = "States.ParameterPathFailure";

        /// <summary>
        /// A branch of a Parallel state failed.
        /// </summary>
        public const string BRANCH_FAILED = "States.BranchFailed";

        /// <summary>
        /// A Choice state failed to find a match for the condition field extracted from its input.
        /// </summary>
        public const string NO_CHOICE_MATCHED = "States.NoChoiceMatched";
        
        /// <summary>
        /// Within a Payload Template, the attempt to invoke an Intrinsic Function failed.
        /// </summary>
        public const string INTRINSIC_FAILURE = "States.IntrinsicFailure";
    }
}