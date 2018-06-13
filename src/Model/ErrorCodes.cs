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
namespace StatesLanguage.Model
{
    public static class ErrorCodes
    {
        //
        //A wild-card which matches any Error Name.
        ////
        public const string ALL = "States.ALL";

        //
        //A Task State either ran longer than the “TimeoutSeconds” value, or failed to heartbeat for a time longer than the
        //“HeartbeatSeconds” value.
        //
        public const string TIMEOUT = "States.Timeout";

        //
        //A Task State failed during the execution.
        //
        public const string TASK_FAILED = "States.TaskFailed";

        //
        //A Task State failed because it had insufficient privileges to execute the specified code.
        //
        public const string PERMISSIONS = "States.Permissions";

        //
        //A Task State’s “ResultPath” field cannot be applied to the input the state received.
        //
        public const string RESULT_PATH_MATCH_FAILURE = "States.ResultPathMatchFailure";

        //
        //A branch of a Parallel state failed.
        //
        public const string BRANCH_FAILED = "States.BranchFailed";

        //
        //A Choice state failed to find a match for the condition field extracted from its input.
        //
        public const string NO_CHOICE_MATCHED = "States.NoChoiceMatched";
    }
}