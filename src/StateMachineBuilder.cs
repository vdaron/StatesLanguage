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

using System;
using StatesLanguage.Conditions;
using StatesLanguage.Internal;
using StatesLanguage.States;

namespace StatesLanguage
{
    /**
     * Fluent API for creating a {@link StateMachine} object.
     */
    public static class StateMachineBuilder
    {
        /**
         * Represents a StepFunctions state machine. A state machine must have at least one state.
         * 
         * @return Builder used to configure a {@link StateMachine}.
         * @see
         * <a href="https://states-language.net/spec.html#toplevelfields">https://states-language.net/spec.html#toplevelfields</a>
         */
        public static StateMachine.Builder StateMachine()
        {
            return StatesLanguage.StateMachine.GetBuilder();
        }

        /**
         * State that allows for parallel execution of {@link Branch}s. A Parallel state causes the interpreter to execute each
         * branch starting with the state named in its “StartAt” field, as concurrently as possible, and wait until each branch
         * terminates
         * (reaches a terminal state) before processing the Parallel state's “Next” field.
         * 
         * @return Builder used to configure a {@link ParallelState}.
         * @see
         * <a href="https://states-language.net/spec.html#parallel-state">https://states-language.net/spec.html#parallel-state</a>
         */
        public static ParallelState.Builder ParallelState()
        {
            return States.ParallelState.GetBuilder();
        }
        
        public static SubStateMachine.Builder SubStateMachine()
        {
            return States.SubStateMachine.GetBuilder();
        }

        /**
         * The Pass State simply passes its input to its output, performing no work. Pass States are useful when constructing and
         * debugging state machines.
         * <p>
         *     A Pass State MAY have a field named “Result”. If present, its value is treated as the output of a virtual task, and
         *     placed as prescribed by the “ResultPath” field, if any, to be passed on to the next state.
         * </p>
         * @return Builder used to configure a {@link PassState}.
         * @see
         * <a href="https://states-language.net/spec.html#pass-state">https://states-language.net/spec.html#pass-state</a>
         */
        public static PassState.Builder PassState()
        {
            return States.PassState.GetBuilder();
        }

        /**
         * The Succeed State terminates a state machine successfully. The Succeed State is a useful target for Choice-state branches
         * that don't do anything but terminate the machine.
         * 
         * @return Builder used to configure a {@link SucceedState}.
         * @see
         * <a href="https://states-language.net/spec.html#succeed-state">https://states-language.net/spec.html#succeed-state</a>
         */
        public static SucceedState.Builder SucceedState()
        {
            return States.SucceedState.GetBuilder();
        }

        /**
         * Terminal state that terminates the state machine and marks it as a failure.
         * 
         * @return Builder used to configure a {@link FailState}.
         * @see
         * <a href="https://states-language.net/spec.html#fail-state">https://states-language.net/spec.html#fail-state</a>
         */
        public static FailState.Builder FailState()
        {
            return States.FailState.GetBuilder();
        }

        /**
         * A Wait state causes the interpreter to delay the machine from continuing for a specified time. The time can be specified
         * as a wait duration, specified in seconds, or an absolute expiry time, specified as an ISO-8601 extended offset date-time
         * format string.
         * 
         * @return Builder used to configure a {@link WaitState}.
         * @see
         * <a href="https://states-language.net/spec.html#wait-state">https://states-language.net/spec.html#wait-state</a>
         */
        public static WaitState.Builder WaitState()
        {
            return States.WaitState.GetBuilder();
        }

        /**
         * The Task State causes the interpreter to execute the work identified by the state’s “Resource” field.
         * <p>Currently allowed resources include Lambda functions and States activities.</p>
         * @return Builder used to configure a {@link TaskState}.
         * @see
         * <a href="https://states-language.net/spec.html#task-state">https://states-language.net/spec.html#task-state</a>
         */
        public static TaskState.Builder TaskState()
        {
            return States.TaskState.GetBuilder();
        }

        /**
         * A Choice state adds branching logic to a state machine. A Choice state consists of a list of choices, each of which
         * contains a potential transition state and a condition that determines if that choice is evaluated, and a default state
         * that the state machine transitions to if no choice branches are matched.
         * 
         * @return Builder used to configure a {@link ChoiceState}.
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         */
        public static ChoiceState.Builder ChoiceState()
        {
            return States.ChoiceState.GetBuilder();
        }

        public static MapState.Builder MapState()
        {
            return States.MapState.GetBuilder();
        }

        /**
         * Class representing a choice rule to be included in a {@link ChoiceState}. A choice consists of a condition and a state
         * that the state machine will transition to if the condition evaluates to true.
         * 
         * @return Builder used to configure a {@link Choice}.
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         */
        public static Choice.Builder Choice()
        {
            return States.Choice.GetBuilder();
        }

        /**
         * Describes retry behavior for a state. A Retrier consists of a list of error codes that the retrier applies to and
         * parameters controlling the behavior when a retry is performed.
         * 
         * @return Builder used to configure a {@link Retrier}.
         * @see
         * <a href="https://states-language.net/spec.html#errors">https://states-language.net/spec.html#errors</a>
         */
        public static Retrier.Builder Retrier()
        {
            return States.Retrier.GetBuilder();
        }

        /**
         * Catches an error from a {@link ParallelState} or a {@link TaskState} and transitions into the specified recovery state.
         * The
         * recovery state will receive the error output as input unless otherwise specified by a ResultPath.
         * 
         * @return Builder used to configure a {@link Catcher}.
         * @see
         * <a href="https://states-language.net/spec.html#errors">https://states-language.net/spec.html#errors</a>
         */
        public static Catcher.Builder Catcher()
        {
            return States.Catcher.GetBuilder();
        }


        /// <summary>
        ///     Binary condition for String match comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison
        /// </param>
        /// <param name="expectedValue">The value MUST be a String which MAY contain one or more "*" characters</param>
        /// <returns></returns>
        public static StringMatchesCondition.Builder Match(string variable, string expectedValue)
        {
            return StringMatchesCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for String equality comparison.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return StringEqualsCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static StringEqualsCondition.Builder StringEquals(string variable, string expectedValue)
        {
            return StringEqualsCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for String equality comparison using Json Path.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition</param>
        /// <returns>
        ///     <see cref="StringEqualsPathCondition.Builder" />
        /// </returns>
        public static StringEqualsPathCondition.Builder StringEqualsPath(string variable, string expectedValuePath)
        {
            return StringEqualsPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric equality comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericEqualsCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericEqualsCondition<long>.Builder NumericEquals(string variable, long expectedValue)
        {
            return NumericEqualsCondition<long>.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Numeric equality comparison using path. Supports both integral and floating point numeric
        ///     types.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition</param>
        /// <returns>
        ///     <see cref="NumericEqualsPathCondition.Builder" />
        /// </returns>
        public static NumericEqualsPathCondition.Builder NumericEqualsPath(string variable, string expectedValuePath)
        {
            return NumericEqualsPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric equality comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericEqualsCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericEqualsCondition<double>.Builder NumericEquals(string variable, double expectedValue)
        {
            return NumericEqualsCondition<double>.GetBuilder()
                .Variable(variable)
                .ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Boolean equality comparison.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return BooleanEqualsCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static BooleanEqualsCondition.Builder BooleanEquals(string variable, bool expectedValue)
        {
            return BooleanEqualsCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Boolean equality comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the comparison.</
        ///     <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        ///     <returns></returns>
        public static BooleanEqualsPathCondition.Builder BooleanEqualsPath(string variable, string expectedValuePath)
        {
            return BooleanEqualsPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Timestamp equality comparison. Dates are converted to ISO8601 UTC timestamps.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return TimestampEqualsCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static TimestampEqualCondition.Builder TimestampEquals(string variable, DateTime expectedValue)
        {
            return TimestampEqualCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Timestamp equality comparison. Dates are converted to ISO8601 UTC timestamps.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static TimestampEqualPathCondition.Builder TimestampEqualsPath(string variable, string expectedValuePath)
        {
            return TimestampEqualPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for String greater than comparison.
         * 
         * @param variable      
         * @param expectedValue The expected value for this condition.
         * @return StringGreaterThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static StringGreaterThanCondition.Builder StringGreaterThan(string variable, string expectedValue)
        {
            return StringGreaterThanCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for String greater than path comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns>
        ///     <see cref="StringGreaterThanPathCondition.Builder" />
        /// </returns>
        public static StringGreaterThanPathCondition.Builder StringGreaterThanPath(string variable,
            string expectedValuePath)
        {
            return StringGreaterThanPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric greater than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericGreaterThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericGreaterThanCondition<long>.Builder NumericGreaterThan(string variable, long expectedValue)
        {
            return NumericGreaterThanCondition<long>.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Numeric greater than comparison using path. Supports both integral and floating point numeric
        ///     types.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static NumericGreaterThanPathCondition.Builder NumericGreaterThanPath(string variable,
            string expectedValuePath)
        {
            return NumericGreaterThanPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric greater than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericGreaterThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericGreaterThanCondition<double>.Builder NumericGreaterThan(string variable,
            double expectedValue)
        {
            return NumericGreaterThanCondition<double>.GetBuilder().Variable(variable)
                .ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Timestamp greater than comparison. Dates are converted to ISO8601 UTC timestamps.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return TimestampGreaterThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static TimestampGreaterThanCondition.Builder TimestampGreaterThan(string variable,
            DateTime expectedValue)
        {
            return TimestampGreaterThanCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Timestamp greater than comparison. Dates are converted to ISO8601 UTC timestamps.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static TimestampGreaterThanPathCondition.Builder TimestampGreaterThanPath(string variable,
            string expectedValuePath)
        {
            return TimestampGreaterThanPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for String greater than or equal to comparison.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return StringGreaterThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static StringGreaterThanOrEqualCondition.Builder StringGreaterThanEquals(string variable,
            string expectedValue)
        {
            return StringGreaterThanOrEqualCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for String greater than or equal to path comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns>
        ///     <see cref="StringGreaterThanOrEqualPathCondition.Builder" />
        /// </returns>
        public static StringGreaterThanOrEqualPathCondition.Builder StringGreaterThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return StringGreaterThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /// <summary>
        ///     Binary condition for Numeric greater than comparison. Supports both integral and floating point numeric types.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static NumericGreaterThanOrEqualPathCondition.Builder NumericGreaterThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return NumericGreaterThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric greater than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericGreaterThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericGreaterThanOrEqualCondition<long>.Builder NumericGreaterThanEquals(string variable,
            long expectedValue)
        {
            return NumericGreaterThanOrEqualCondition<long>.GetBuilder()
                .Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Numeric greater than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericGreaterThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericGreaterThanOrEqualCondition<double>.Builder NumericGreaterThanEquals(string variable,
            double expectedValue)
        {
            return NumericGreaterThanOrEqualCondition<double>.GetBuilder()
                .Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Timestamp greater than or equal to comparison. Dates are converted to ISO8601 UTC timestamps.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return TimestampGreaterThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static TimestampGreaterThanOrEqualCondition.Builder TimestampGreaterThanEquals(string variable,
            DateTime expectedValue)
        {
            return TimestampGreaterThanOrEqualCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Timestamp greater than or equal to comparison. Dates are converted to ISO8601 UTC timestamps.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static TimestampGreaterThanOrEqualPathCondition.Builder TimestampGreaterThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return TimestampGreaterThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for String less than comparison.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return StringLessThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static StringLessThanCondition.Builder StringLessThan(string variable, string expectedValue)
        {
            return StringLessThanCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for String less than path comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns>
        ///     <see cref="StringLessThanPathCondition.Builder" />
        /// </returns>
        public static StringLessThanPathCondition.Builder StringLessThanPath(string variable, string expectedValuePath)
        {
            return StringLessThanPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /// <summary>
        ///     Binary condition for Numeric less than comparison. Supports both integral and floating point numeric types.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static NumericLessThanPathCondition.Builder NumericLessThanPath(string variable,
            string expectedValuePath)
        {
            return NumericLessThanPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric less than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericLessThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericLessThanCondition<long>.Builder NumericLessThan(string variable, long expectedValue)
        {
            return NumericLessThanCondition<long>.GetBuilder().Variable(variable)
                .ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Numeric less than comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericLessThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericLessThanCondition<double>.Builder NumericLessThan(string variable, double expectedValue)
        {
            return NumericLessThanCondition<double>.GetBuilder().Variable(variable)
                .ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Timestamp less than comparison. Dates are converted to ISO8601 UTC timestamps.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return TimestampLessThanCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static TimestampLessThanCondition.Builder TimestampLessThan(string variable, DateTime expectedValue)
        {
            return TimestampLessThanCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Timestamp less than comparison using Path. Dates are converted to ISO8601 UTC timestamps.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static TimestampLessThanPathCondition.Builder TimestampLessThanPath(string variable,
            string expectedValuePath)
        {
            return TimestampLessThanPathCondition.GetBuilder().Variable(variable).ExpectedValuePath(expectedValuePath);
        }


        /**
         * Binary condition for String less than or equal to comparison.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return StringLessThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static StringLessThanOrEqualCondition.Builder StringLessThanEquals(string variable, string expectedValue)
        {
            return StringLessThanOrEqualCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for String less than or equal path comparison.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns>
        ///     <see cref="StringLessThanPathCondition.Builder" />
        /// </returns>
        public static StringLessThanOrEqualPathCondition.Builder StringLessThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return StringLessThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /// <summary>
        ///     Binary condition for Numeric less than or equal to comparison. Supports both integral and floating point numeric
        ///     types.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static NumericLessThanOrEqualPathCondition.Builder NumericLessThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return NumericLessThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /**
         * Binary condition for Numeric less than or equal to comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericLessThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericLessThanOrEqualCondition<long>.Builder NumericLessThanEquals(string variable,
            long expectedValue)
        {
            return NumericLessThanOrEqualCondition<long>.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Numeric less than or equal to comparison. Supports both integral and floating point numeric types.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return NumericLessThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static NumericLessThanOrEqualCondition<double>.Builder NumericLessThanEquals(string variable,
            double expectedValue)
        {
            return NumericLessThanOrEqualCondition<double>.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Binary condition for Timestamp less than or equal to comparison. Dates are converted to ISO8601 UTC timestamps.
         * 
         * @param variable      The JSONPath expression that determines which piece of the input document is used for the comparison.
         * @param expectedValue The expected value for this condition.
         * @return TimestampLessThanOrEqualCondition.Builder
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         * @see Choice
         */
        public static TimestampLessThanOrEqualCondition.Builder TimestampLessThanEquals(string variable,
            DateTime expectedValue)
        {
            return TimestampLessThanOrEqualCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for Timestamp less than or equal to comparison using path. Dates are converted to ISO8601 UTC
        ///     timestamps.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValuePath">The JSONPath expression that determines the expected value for this condition.</param>
        /// <returns></returns>
        public static TimestampLessThanOrEqualPathCondition.Builder TimestampLessThanEqualsPath(string variable,
            string expectedValuePath)
        {
            return TimestampLessThanOrEqualPathCondition.GetBuilder().Variable(variable)
                .ExpectedValuePath(expectedValuePath);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is null.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue">Must be null or not</param>
        /// <returns></returns>
        public static IsNullCondition.Builder IsNull(string variable, bool expectedValue)
        {
            return IsNullCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is present.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static IsPresentCondition.Builder IsPresent(string variable, bool expectedValue)
        {
            return IsPresentCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is a Numeric (integer or float).
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static IsNumericCondition.Builder IsNumeric(string variable, bool expectedValue)
        {
            return IsNumericCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is a String.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static IsStringCondition.Builder IsString(string variable, bool expectedValue)
        {
            return IsStringCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is a Boolean.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static IsBooleanCondition.Builder IsBoolean(string variable, bool expectedValue)
        {
            return IsBooleanCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /// <summary>
        ///     Binary condition for testing if a Path is a Timestamp.
        /// </summary>
        /// <param name="variable">
        ///     The JSONPath expression that determines which piece of the input document is used for the
        ///     comparison.
        /// </param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static IsTimestampCondition.Builder IsTimestamp(string variable, bool expectedValue)
        {
            return IsTimestampCondition.GetBuilder().Variable(variable).ExpectedValue(expectedValue);
        }

        /**
         * Represents the logical NOT of a single condition. May be used in a {@link ChoiceState}.
         * 
         * @param conditionBuilder The condition to be negated. May be another composite condition or a simple condition.
         * @return Builder used to configure a {@link NotCondition}.
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         */
        public static NotCondition.Builder Not<T>(IConditionBuilder<T> conditionBuilder) where T : ICondition
        {
            return NotCondition.GetBuilder().Condition(conditionBuilder);
        }

        /**
         * Represents the logical AND of multiple conditions. May be used in a {@link ChoiceState}.
         * 
         * @param conditionBuilders The conditions to AND together. May be another composite condition or a simple condition.
         * @return Builder used to configure a {@link AndCondition}.
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         */
        public static AndCondition.Builder And(params IBuildable<ICondition>[] conditionBuilders)
        {
            return AndCondition.GetBuilder().Conditions(conditionBuilders);
        }

        /**
         * Represents the logical OR of multiple conditions. May be used in a {@link ChoiceState}.
         * 
         * @param conditionBuilders The conditions to OR together. May be another composite condition or a simple condition.
         * @return Builder used to configure a {@link OrCondition}.
         * @see
         * <a href="https://states-language.net/spec.html#choice-state">https://states-language.net/spec.html#choice-state</a>
         */
        public static OrCondition.Builder Or(params IBuildable<ICondition>[] conditionBuilders)
        {
            return OrCondition.GetBuilder().Conditions(conditionBuilders);
        }

        /**
         * A transition to another state in the state machine.
         * 
         * @param nextStateName Name of state to transition to.
         * @return Transition to another state in the state machine.
         * @see
         * <a href="https://states-language.net/spec.html#transition">https://states-language.net/spec.html#transition</a>
         */
        public static NextStateTransition.Builder Next(string nextStateName)
        {
            return NextStateTransition.GetBuilder().NextStateName(nextStateName);
        }

        /**
         * Transition indicating the state machine should terminate execution.
         * 
         * @return EndTransition
         * @see
         * <a href="https://states-language.net/spec.html#transition">https://states-language.net/spec.html#transition</a>
         */
        public static ITransitionBuilder<EndTransition> End()
        {
            return EndTransition.GetBuilder();
        }

        /**
         * {@link WaitFor} that can be used in a {@link WaitState}. Instructs the {@link WaitState} to wait for the
         * given number of seconds.
         * 
         * @param seconds Number of seconds to wait. Must be positive.
         * @return WaitFor.Builder
         * @see
         * <a href="https://states-language.net/spec.html#wait-state">https://states-language.net/spec.html#wait-state</a>
         */
        public static IWaitForBuilder<WaitForSeconds> Seconds(int seconds)
        {
            return WaitForSeconds.GetBuilder().Seconds(seconds);
        }

        /**
         * {@link WaitFor} that can be used in a {@link WaitState}. Instructs the {@link WaitState} to wait for the
         * number of seconds specified at the reference path in the input to the state.
         * 
         * @param secondsPath Reference path to the location in the input data containing the number of seconds to wait.
         * @return WaitFor.Builder
         * @see
         * <a href="https://states-language.net/spec.html#wait-state">https://states-language.net/spec.html#wait-state</a>
         */
        public static IWaitForBuilder<WaitForSecondsPath> SecondsPath(string secondsPath)
        {
            return WaitForSecondsPath.GetBuilder().SecondsPath(secondsPath);
        }

        /**
         * {@link WaitFor} that can be used in a {@link WaitState}. Instructs the {@link WaitState} to wait until
         * the given timestamp.
         * 
         * @param timestamp DateTime to wait until before proceeding.
         * @return WaitFor.Builder
         * @see
         * <a href="https://states-language.net/spec.html#wait-state">https://states-language.net/spec.html#wait-state</a>
         */
        public static IWaitForBuilder<WaitForTimestamp> Timestamp(DateTime timestamp)
        {
            return WaitForTimestamp.GetBuilder().Timestamp(timestamp);
        }

        /**
         * {@link WaitFor} that can be used in a {@link WaitState}. Instructs the {@link WaitState} to wait until
         * the date specified at the reference path in the input to the state.
         * 
         * @param timestampPath Reference path to the location in the input data containing the date to wait until.
         * @return WaitFor.Builder
         * @see
         * <a href="https://states-language.net/spec.html#wait-state">https://states-language.net/spec.html#wait-state</a>
         */
        public static IWaitForBuilder<WaitForTimestampPath> TimestampPath(string timestampPath)
        {
            return WaitForTimestampPath.GetBuilder().TimestampPath(timestampPath);
        }
    }
}