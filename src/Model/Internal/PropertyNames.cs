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
namespace StatesLanguage.Model.Internal
{
    public static class PropertyNames
    {
        // Common property names
        public const string TYPE = "Type";
        public const string COMMENT = "Comment";
        public const string NEXT = "Next";
        public const string TIMEOUT_SECONDS = "TimeoutSeconds";
        public const string START_AT = "StartAt";
        public const string STATES = "States";
        public const string RESULT = "Result";
        public const string RESULT_PATH = "ResultPath";
        public const string INPUT_PATH = "InputPath";
        public const string OUTPUT_PATH = "OutputPath";
        public const string PARAMETERS = "Parameters";

        public const string END = "End";

        // TaskState property names
        public const string RESOURCE = "Resource";

        public const string HEARTBEAT_SECONDS = "HeartbeatSeconds";

        // ParallelState property names
        public const string BRANCHES = "Branches";

        // FailState property names
        public const string ERROR = "Error";

        public const string CAUSE = "Cause";

        // ChoiceState propert names
        public const string DEFAULT_STATE = "Default";

        public const string CHOICES = "Choices";

        // Retrier/Catcher property names
        public const string RETRY = "Retry";
        public const string CATCH = "Catch";
        public const string ERROR_EQUALS = "ErrorEquals";
        public const string INTERVAL_SECONDS = "IntervalSeconds";
        public const string MAX_ATTEMPTS = "MaxAttempts";

        public const string BACKOFF_RATE = "BackoffRate";

        // WaitState property names
        public const string SECONDS = "Seconds";
        public const string TIMESTAMP = "Timestamp";
        public const string TIMESTAMP_PATH = "TimestampPath";

        public const string SECONDS_PATH = "SecondsPath";

        // Binary condition property names
        public const string VARIABLE = "Variable";

        // Binary string condition property names
        public const string STRING_EQUALS = "StringEquals";
        public const string STRING_LESS_THAN = "StringLessThan";
        public const string STRING_GREATER_THAN = "StringGreaterThan";
        public const string STRING_GREATER_THAN_EQUALS = "StringGreaterThanEquals";

        public const string STRING_LESS_THAN_EQUALS = "StringLessThanEquals";

        // Binary numeric condition property names
        public const string NUMERIC_EQUALS = "NumericEquals";
        public const string NUMERIC_LESS_THAN = "NumericLessThan";
        public const string NUMERIC_GREATER_THAN = "NumericGreaterThan";
        public const string NUMERIC_GREATER_THAN_EQUALS = "NumericGreaterThanEquals";

        public const string NUMERIC_LESS_THAN_EQUALS = "NumericLessThanEquals";

        // Binary timestamp condition property names
        public const string TIMESTAMP_EQUALS = "TimestampEquals";
        public const string TIMESTAMP_LESS_THAN = "TimestampLessThan";
        public const string TIMESTAMP_GREATER_THAN = "TimestampGreaterThan";
        public const string TIMESTAMP_GREATER_THAN_EQUALS = "TimestampGreaterThanEquals";

        public const string TIMESTAMP_LESS_THAN_EQUALS = "TimestampLessThanEquals";

        // Binary boolean condition property names
        public const string BOOLEAN_EQUALS = "BooleanEquals";

        // Composite conditions property names
        public const string AND = "And";
        public const string OR = "Or";
        public const string NOT = "Not";
    }
}