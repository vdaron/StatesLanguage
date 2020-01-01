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
using StatesLanguage.Model;
using StatesLanguage.Model.Conditions;
using StatesLanguage.Model.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.Model.Validation
{
    public class StateMachineValidatorTest
    {

        [Fact]
        public void NothingSet_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine().Build());
        }

        [Fact]
        public void NoStates_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine().StartAt("Foo").Build());
        }

        [Fact]
        public void StartAtStateDoesNotExist_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Foo")
                       .State("Initial", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]//
        public void ValidMinimalStateMachine_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.SucceedState())
                    .Build();
        }

        [Fact]
        public void MissingResourceInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void MissingTransitionInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void InvalidTransitionInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.Next("NoSuchState"))
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void NegativeTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .TimeoutSeconds(-1)
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void ZeroTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .TimeoutSeconds(0)
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void NegativeHeartbeatSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .HeartbeatSeconds(-1)
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void ZeroHeartbeatSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .HeartbeatSeconds(0)
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void HeartbeatSecondsGreaterThanTimeoutSecondsInTaskState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .HeartbeatSeconds(60)
                               .TimeoutSeconds(30)
                               .Resource("arn"))
                       .Build());
        }

        [Fact]//
        public void RetrierInTaskState_OnlyErrorEqualsSet_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.TaskState()
                            .Transition(StepFunctionBuilder.End())
                            .Retrier(StepFunctionBuilder.Retrier()
                                             .RetryOnAllErrors())
                            .Resource("arn"))
                    .Build();
        }

        [Fact]
        public void RetrierInTaskState_MaxAttemptsNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .MaxAttempts(-1)
                                                .RetryOnAllErrors())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]//
        public void RetrierInTaskState_MaxAttemptsZero_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.TaskState()
                            .Transition(StepFunctionBuilder.End())
                            .Retrier(StepFunctionBuilder.Retrier()
                                             .MaxAttempts(0)
                                             .RetryOnAllErrors())
                            .Resource("arn"))
                    .Build();
        }

        [Fact]
        public void RetrierInTaskState_IntervalSecondsNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .IntervalSeconds(-1)
                                                .RetryOnAllErrors())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void RetrierInTaskState_IntervalSecondsZero_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .IntervalSeconds(0)
                                                .RetryOnAllErrors())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]//
        public void RetrierInTaskState_IntervalSecondsPositive_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.TaskState()
                            .Transition(StepFunctionBuilder.End())
                            .Retrier(StepFunctionBuilder.Retrier()
                                             .IntervalSeconds(10)
                                             .RetryOnAllErrors())
                            .Resource("arn"))
                    .Build();
        }

        [Fact]
        public void RetrierInTaskState_BackoffRateNegative_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .BackoffRate(-1.0)
                                                .RetryOnAllErrors())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void RetrierInTaskState_BackoffRateLessThanOne_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .BackoffRate(0.5)
                                                .RetryOnAllErrors())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]//
        public void RetrierInTaskState_BackoffRateGreaterThanOne_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.TaskState()
                            .Transition(StepFunctionBuilder.End())
                            .Retrier(StepFunctionBuilder.Retrier()
                                             .BackoffRate(1.5)
                                             .RetryOnAllErrors())
                            .Resource("arn"))
                    .Build();
        }

        [Fact]
        public void RetrierInTaskState_RetryAllHasOtherErrorCodes_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .ErrorEquals("Foo", "Bar", ErrorCodes.ALL))
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void RetrierInTaskState_RetryAllIsNotLastRetrier_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .RetryOnAllErrors())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .ErrorEquals("Foo", "Bar"))
                               .Resource("arn"))
                       .Build());
        }

        [Fact]//
        public void CatcherInTaskState_ValidTransition_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StepFunctionBuilder.TaskState()
                    .Transition(StepFunctionBuilder.End())
                    .Catcher(StepFunctionBuilder.Catcher()
                        .Transition(StepFunctionBuilder.Next("Terminal"))
                        .CatchAll())
                    .Resource("arn"))
                .State("Terminal", StepFunctionBuilder.SucceedState())
                .Build();
        }


        [Fact]
        public void CatcherInTaskState_InvalidTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Catcher(StepFunctionBuilder.Catcher()
                                                .Transition(StepFunctionBuilder.Next("NoSuchState"))
                                                .CatchAll())
                               .Resource("arn"))
                       .Build());
        }

        [Fact]
        public void CatcherInTaskState_CatchAllHasOtherErrorCodes_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Catcher(StepFunctionBuilder.Catcher()
                                                .Transition(StepFunctionBuilder.Next("Terminal"))
                                                .ErrorEquals("Foo", "Bar", ErrorCodes.ALL))
                               .Resource("arn"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void CatcherInTaskState_CatchAllIsNotLastCatcher_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.TaskState()
                               .Transition(StepFunctionBuilder.End())
                               .Catcher(StepFunctionBuilder.Catcher()
                                                .Transition(StepFunctionBuilder.Next("Terminal"))
                                                .CatchAll())
                               .Catcher(StepFunctionBuilder.Catcher()
                                                .Transition(StepFunctionBuilder.Next("Terminal"))
                                                .ErrorEquals("Foo", "Bar"))
                               .Resource("arn"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void InvalidTransitionInWaitState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.Seconds(10))
                               .Transition(StepFunctionBuilder.Next("NoSuchState")))
                       .Build());
        }

        [Fact]
        public void NoWaitForSupplied_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void WaitForSeconds_NegativeSeconds_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.Seconds(-1))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void WaitForSeconds_ZeroSeconds_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.Seconds(0))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        //NA

        //    [Fact]
        //public void WaitForTimestamp_NullDate_IsNotValid()
        //    {Assert.Throws<ValidationException>(() =>
        //        StepFunctionBuilder.StateMachine()
        //                .StartAt("Initial")
        //                .State("Initial", StepFunctionBuilder.WaitState()
        //                        .WaitFor(StepFunctionBuilder.Timestamp(null))
        //                        .Transition(StepFunctionBuilder.End()))
        //                .Build());
        //    }

        [Fact]
        public void WaitForTimestampPath_MissingPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.TimestampPath(null))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void WaitForTimestampPath_EmptyPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.TimestampPath(""))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact(Skip = "Unable to test JsonPath queries")]
        public void WaitForTimestampPath_InvalidJsonPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.TimestampPath("$."))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact(Skip = "Unable to test jsonpath")]
        public void WaitForTimestampPath_InvalidReferencePath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.TimestampPath("$.Foo[*]"))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void WaitForSecondsPath_MissingPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.SecondsPath(null))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void WaitForSecondsPath_EmptyPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.SecondsPath(""))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact(Skip = "Unable to test jsonpath")]
        public void WaitForSecondsPath_InvalidJsonPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.SecondsPath("$."))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact(Skip = "Unable to test jsonpath")]
        public void WaitForSecondsPath_InvalidReferencePath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.WaitState()
                               .WaitFor(StepFunctionBuilder.SecondsPath("$.Foo[*]"))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void InvalidTransitionInPassState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.PassState()
                               .Transition(StepFunctionBuilder.Next("NoSuchState")))
                       .Build());
        }

        [Fact]//
        public void ValidTransitionInPassState_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.PassState()
                            .Transition(StepFunctionBuilder.Next("Terminal")))
                    .State("Terminal", StepFunctionBuilder.SucceedState())
                    .Build();
        }

        [Fact]
        public void MissingCauseInFailState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.FailState()
                               .Error("Error"))
                       .Build());
        }

        [Fact]//
        public void MissingErrorInFailState_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.FailState()
                            .Cause("Cause"))
                    .Build();
        }

        [Fact]//
        public void FailStateWithErrorAndCause_IsValid()
        {
            StepFunctionBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StepFunctionBuilder.FailState()
                            .Error("Error")
                            .Cause("Cause"))
                    .Build();
        }

        [Fact]
        public void ChoiceStateWithNoChoices_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void ChoiceStateWithInvalidDefaultState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(StepFunctionBuilder.Eq("$.Foo", "bar"))
                                               .Transition(StepFunctionBuilder.Next("Terminal")))
                               .DefaultStateName("NoSuchState"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void ChoiceStateWithInvalidChoiceTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(StepFunctionBuilder.Eq("$.Foo", "bar"))
                                               .Transition(StepFunctionBuilder.Next("NoSuchState")))
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void MissingVariable_StringEqualsCondition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(StepFunctionBuilder.Eq(null, "foo"))
                                               .Transition(StepFunctionBuilder.Next("Terminal")))
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void MissingExpectedValue_StringEqualsCondition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(StepFunctionBuilder.Eq("$.Foo", null))
                                               .Transition(StepFunctionBuilder.Next("Terminal")))
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void NoConditionsInAnd_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(StepFunctionBuilder.And())
                                               .Transition(StepFunctionBuilder.Next("Terminal")))
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void NoConditionSetForNot_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ChoiceState()
                               .Choice(StepFunctionBuilder.Choice()
                                               .Condition(NotCondition.GetBuilder())
                                               .Transition(StepFunctionBuilder.Next("Terminal")))
                               .DefaultStateName("Terminal"))
                       .State("Terminal", StepFunctionBuilder.SucceedState())
                       .Build());
        }

        [Fact]
        public void ParallelStateWithNoBranches_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void ParallelStateWithInvalidTransition_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Branch(StepFunctionBuilder.SubStateMachine()
                                               .StartAt("InitialBranchState")
                                               .State("InitialBranchState", StepFunctionBuilder.SucceedState()))
                               .Transition(StepFunctionBuilder.Next("NoSuchState")))
                       .Build());
        }

        [Fact]
        public void ParallelStateBranchStartAtStateInvalid_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Branch(StepFunctionBuilder.SubStateMachine()
                                               .StartAt("NoSuchState")
                                               .State("InitialBranchState", StepFunctionBuilder.SucceedState()))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void ParallelStateInvalidBranchState_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Branch(StepFunctionBuilder.SubStateMachine()
                                               .StartAt("InitialBranchState")
                                               .State("InitialBranchState", StepFunctionBuilder.FailState()))
                               .Transition(StepFunctionBuilder.End()))
                       .Build());
        }

        [Fact]
        public void ParallelStateInvalidRetrier_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Branch(StepFunctionBuilder.SubStateMachine()
                                               .StartAt("InitialBranchState")
                                               .State("InitialBranchState", StepFunctionBuilder.SucceedState()))
                               .Transition(StepFunctionBuilder.End())
                               .Retrier(StepFunctionBuilder.Retrier()
                                                .IntervalSeconds(-1)))
                       .Build());
        }

        [Fact]
        public void ParallelStateInvalidCatcher_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
               StepFunctionBuilder.StateMachine()
                       .StartAt("Initial")
                       .State("Initial", StepFunctionBuilder.ParallelState()
                               .Branch(StepFunctionBuilder.SubStateMachine()
                                               .StartAt("InitialBranchState")
                                               .State("InitialBranchState", StepFunctionBuilder.SucceedState()))
                               .Transition(StepFunctionBuilder.End())
                               .Catcher(StepFunctionBuilder.Catcher()
                                                .Transition(StepFunctionBuilder.Next("NoSuchState"))))
                       .Build());
        }
        
        [Fact]
        public void MapStateWithoutIterator_IsNotValid()
        {
                Assert.Throws<ValidationException>(() =>
                        StepFunctionBuilder.StateMachine()
                                .StartAt("Initial")
                                .State("Initial", StepFunctionBuilder.MapState()
                                        .Transition(StepFunctionBuilder.End()))
                                .Build());
        }
    }

}
