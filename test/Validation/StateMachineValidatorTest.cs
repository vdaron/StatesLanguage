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

using StatesLanguage.Conditions;
using StatesLanguage.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.Model.Validation
{
    public class StateMachineValidatorTest
    {
        [Fact]
        public void NothingSet_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine().Build());
        }

        [Fact]
        public void NoStates_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine().StartAt("Foo").Build());
        }

        [Fact]
        public void StartAtStateDoesNotExist_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Foo")
                    .State("Initial", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact] //
        public void ValidMinimalStateMachine_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.SucceedState())
                .Build();
        }

        [Fact]
        public void MissingResourceInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void MissingTransitionInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void InvalidTransitionInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.Next("NoSuchState"))
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void NegativeTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .TimeoutSeconds(-1)
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void ZeroTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .TimeoutSeconds(0)
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void NegativeHeartbeatSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .HeartbeatSeconds(-1)
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void ZeroHeartbeatSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .HeartbeatSeconds(0)
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void HeartbeatSecondsGreaterThanTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .HeartbeatSeconds(60)
                        .TimeoutSeconds(30)
                        .Resource("arn"))
                    .Build());
        }

        [Fact] //
        public void RetrierInTaskState_OnlyErrorEqualsSet_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.End())
                    .Retrier(StateMachineBuilder.Retrier()
                        .RetryOnAllErrors())
                    .Resource("arn"))
                .Build();
        }

        [Fact]
        public void RetrierInTaskState_MaxAttemptsNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .MaxAttempts(-1)
                            .RetryOnAllErrors())
                        .Resource("arn"))
                    .Build());
        }

        [Fact] //
        public void RetrierInTaskState_MaxAttemptsZero_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.End())
                    .Retrier(StateMachineBuilder.Retrier()
                        .MaxAttempts(0)
                        .RetryOnAllErrors())
                    .Resource("arn"))
                .Build();
        }

        [Fact]
        public void RetrierInTaskState_IntervalSecondsNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .IntervalSeconds(-1)
                            .RetryOnAllErrors())
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void RetrierInTaskState_IntervalSecondsZero_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .IntervalSeconds(0)
                            .RetryOnAllErrors())
                        .Resource("arn"))
                    .Build());
        }

        [Fact] //
        public void RetrierInTaskState_IntervalSecondsPositive_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.End())
                    .Retrier(StateMachineBuilder.Retrier()
                        .IntervalSeconds(10)
                        .RetryOnAllErrors())
                    .Resource("arn"))
                .Build();
        }

        [Fact]
        public void RetrierInTaskState_BackoffRateNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .BackoffRate(-1.0)
                            .RetryOnAllErrors())
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void RetrierInTaskState_BackoffRateLessThanOne_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .BackoffRate(0.5)
                            .RetryOnAllErrors())
                        .Resource("arn"))
                    .Build());
        }

        [Fact] //
        public void RetrierInTaskState_BackoffRateGreaterThanOne_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.End())
                    .Retrier(StateMachineBuilder.Retrier()
                        .BackoffRate(1.5)
                        .RetryOnAllErrors())
                    .Resource("arn"))
                .Build();
        }

        [Fact]
        public void RetrierInTaskState_RetryAllHasOtherErrorCodes_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar", ErrorCodes.ALL))
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void RetrierInTaskState_RetryAllIsNotLastRetrier_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .RetryOnAllErrors())
                        .Retrier(StateMachineBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar"))
                        .Resource("arn"))
                    .Build());
        }

        [Fact] //
        public void CatcherInTaskState_ValidTransition_IsValid()
        {
            StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.End())
                    .Catcher(StateMachineBuilder.Catcher()
                        .Transition(StateMachineBuilder.Next("Terminal"))
                        .CatchAll())
                    .Resource("arn"))
                .State("Terminal", StateMachineBuilder.SucceedState())
                .Build();
        }


        [Fact]
        public void CatcherInTaskState_InvalidTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Catcher(StateMachineBuilder.Catcher()
                            .Transition(StateMachineBuilder.Next("NoSuchState"))
                            .CatchAll())
                        .Resource("arn"))
                    .Build());
        }

        [Fact]
        public void CatcherInTaskState_CatchAllHasOtherErrorCodes_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Catcher(StateMachineBuilder.Catcher()
                            .Transition(StateMachineBuilder.Next("Terminal"))
                            .ErrorEquals("Foo", "Bar", ErrorCodes.ALL))
                        .Resource("arn"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void CatcherInTaskState_CatchAllIsNotLastCatcher_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.TaskState()
                        .Transition(StateMachineBuilder.End())
                        .Catcher(StateMachineBuilder.Catcher()
                            .Transition(StateMachineBuilder.Next("Terminal"))
                            .CatchAll())
                        .Catcher(StateMachineBuilder.Catcher()
                            .Transition(StateMachineBuilder.Next("Terminal"))
                            .ErrorEquals("Foo", "Bar"))
                        .Resource("arn"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void InvalidTransitionInWaitState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.Seconds(10))
                        .Transition(StateMachineBuilder.Next("NoSuchState")))
                    .Build());
        }

        [Fact]
        public void NoWaitForSupplied_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSeconds_NegativeSeconds_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.Seconds(-1))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSeconds_ZeroSeconds_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.Seconds(0))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForTimestampPath_MissingPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.TimestampPath(null))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForTimestampPath_EmptyPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.TimestampPath(""))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForTimestampPath_InvalidJsonPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.TimestampPath("$."))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForTimestampPath_InvalidReferencePath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.TimestampPath("$.Foo[*]"))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSecondsPath_MissingPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.SecondsPath(null))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSecondsPath_EmptyPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.SecondsPath(""))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSecondsPath_InvalidJsonPath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.SecondsPath("$."))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void WaitForSecondsPath_InvalidReferencePath_IsNotValid()
        {
            Assert.ThrowsAny<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.WaitState()
                        .WaitFor(StateMachineBuilder.SecondsPath("$.Foo[*]"))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void InvalidTransitionInPassState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("NoSuchState")))
                    .Build());
        }

        [Fact] //
        public void ValidTransitionInPassState_IsValid()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.PassState()
                    .Transition(StateMachineBuilder.Next("Terminal")))
                .State("Terminal", StateMachineBuilder.SucceedState())
                .Build());
        }

        [Fact]
        public void MissingCauseInFailState_IsValid()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.FailState()
                    .Error("Error"))
                .Build());
        }

        [Fact]
        public void MissingErrorInFailState_IsValid()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.FailState()
                    .Cause("Cause"))
                .Build());
        }

        [Fact]
        public void FailStateErrorPath_CanBeAPathOrIntrinsicFunction()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Err")
                .State("Err", StateMachineBuilder.FailState().ErrorPath("$.Cause"))
                .State("Err2", StateMachineBuilder.FailState().ErrorPath("String.Concat('a','e')"))
                .Build());
        }

        [Fact]
        public void FailStateCausePath_CanBeAPathOrIntrinsicFunction()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Cause")
                .State("Cause", StateMachineBuilder.FailState().CausePath("$.Cause"))
                .State("Cause2", StateMachineBuilder.FailState().CausePath("String.Concat('a','e')"))
                .Build());
        }

        [Fact]
        public void FailStateErrorPath_CannotBeAContant()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                   .StartAt("Cause")
                   .State("Cause", StateMachineBuilder.FailState().ErrorPath("InvalidFailPath"))
                   .Build());
        }

        [Fact]
        public void FailStateCausePath_CannotBeAContant()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Cause")
                    .State("Cause", StateMachineBuilder.FailState().CausePath("InvalidFailPath"))
                    .Build());
        }

        [Fact] //
        public void FailStateWithErrorAndCause_IsValid()
        {
            Assert.NotNull(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.FailState()
                    .Error("Error")
                    .Cause("Cause"))
                .Build());
        }

        [Fact]
        public void ChoiceStateWithNoChoices_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void ChoiceStateWithInvalidDefaultState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(StateMachineBuilder.StringEquals("$.Foo", "bar"))
                            .Transition(StateMachineBuilder.Next("Terminal")))
                        .DefaultStateName("NoSuchState"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void ChoiceStateWithInvalidChoiceTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(StateMachineBuilder.StringEquals("$.Foo", "bar"))
                            .Transition(StateMachineBuilder.Next("NoSuchState")))
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void MissingVariable_StringEqualsCondition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(StateMachineBuilder.StringEquals(null, "foo"))
                            .Transition(StateMachineBuilder.Next("Terminal")))
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void MissingExpectedValue_StringEqualsCondition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(StateMachineBuilder.StringEquals("$.Foo", null))
                            .Transition(StateMachineBuilder.Next("Terminal")))
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void NoConditionsInAnd_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(StateMachineBuilder.And())
                            .Transition(StateMachineBuilder.Next("Terminal")))
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void NoConditionSetForNot_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ChoiceState()
                        .Choice(StateMachineBuilder.Choice()
                            .Condition(NotCondition.GetBuilder())
                            .Transition(StateMachineBuilder.Next("Terminal")))
                        .DefaultStateName("Terminal"))
                    .State("Terminal", StateMachineBuilder.SucceedState())
                    .Build());
        }

        [Fact]
        public void ParallelStateWithNoBranches_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void ParallelStateWithInvalidTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("InitialBranchState")
                            .State("InitialBranchState", StateMachineBuilder.SucceedState()))
                        .Transition(StateMachineBuilder.Next("NoSuchState")))
                    .Build());
        }

        [Fact]
        public void ParallelStateBranchStartAtStateInvalid_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("NoSuchState")
                            .State("InitialBranchState", StateMachineBuilder.SucceedState()))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void ParallelStateInvalidBranchState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("InitialBranchState")
                            .State("InitialBranchState", StateMachineBuilder.FailState().Error("error").ErrorPath("$.notAllowed")))
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void ParallelStateInvalidRetrier_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("InitialBranchState")
                            .State("InitialBranchState", StateMachineBuilder.SucceedState()))
                        .Transition(StateMachineBuilder.End())
                        .Retrier(StateMachineBuilder.Retrier()
                            .IntervalSeconds(-1)))
                    .Build());
        }

        [Fact]
        public void ParallelStateInvalidCatcher_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("InitialBranchState")
                            .State("InitialBranchState", StateMachineBuilder.SucceedState()))
                        .Transition(StateMachineBuilder.End())
                        .Catcher(StateMachineBuilder.Catcher()
                            .Transition(StateMachineBuilder.Next("NoSuchState"))))
                    .Build());
        }

        [Fact]
        public void MapStateWithoutIterator_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.MapState()
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void InputPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .InputPath("['invalidPath")
                        .Transition(StateMachineBuilder.End()))
                    .Build());
        }

        [Fact]
        public void FailState_ErrorPathAndCausePath()
        {
            StateMachineBuilder.StateMachine()
                .Comment("A Hello World example of the Amazon States Language using a Pass state")
                .StartAt("0001")
                .State("0001", StateMachineBuilder.FailState()
                    .ErrorPath("$.Error")
                    .CausePath("$.Cause"))
                .Build();
        }

        [Fact]
        public void FailState_ErrorPathAndCausePathWithIntrinsicFunction()
        {
            StateMachineBuilder.StateMachine()
                .Comment("A Hello World example of the Amazon States Language using a Pass state")
                .StartAt("0001")
                .State("0001", StateMachineBuilder.FailState()
                    .ErrorPath("States.Format('{}', $.Error)")
                    .CausePath("States.Format('This is a custom error message for {}, caused by {}.', $.Error, $.Cause)"))
                .Build();
        }
    }
}