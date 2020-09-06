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
using StatesLanguage.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.Model.Validation
{
    public class CycleTest
    {
        private void AssertCycle(StateMachine.Builder stateMachineBuilder)
        {
            try
            {
                Validate(stateMachineBuilder);
            }
            catch (ArgumentException expected)
            {
                Assert.Contains("Cycle detected", expected.Message);
            }
        }

        private void AssertDoesNotHaveTerminalPath(StateMachine.Builder stateMachineBuilder)
        {
            try
            {
                Validate(stateMachineBuilder);
            }
            catch (ArgumentException expected)
            {
                Assert.Contains("No path to a terminal state exists in the state machine", expected.Message);
            }
        }

        private void AssertHasPathToTerminal(StateMachine.Builder stateMachineBuilder)
        {
            Validate(stateMachineBuilder);
        }

        private void AssertNoCycle(StateMachine.Builder stateMachineBuilder)
        {
            Validate(stateMachineBuilder);
        }

        private void Validate(StateMachine.Builder stateMachineBuilder)
        {
            new StateMachineValidator(stateMachineBuilder.Build()).Validate();
        }

        [Fact(Skip = "skip for now")]
        public void ChoiceStateWithClosedCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertCycle(StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("Choice")))
                    .State("Choice", StateMachineBuilder.ChoiceState()
                        .DefaultStateName("Terminal")
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder.Next("Terminal"))
                            .Condition(StateMachineBuilder.StringEquals("$.foo", "bar")))
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder.Next("NonTerminal"))
                            .Condition(StateMachineBuilder.StringEquals("$.foo", "bar"))))
                    .State("Terminal", StateMachineBuilder.PassState().Transition(StateMachineBuilder.End()))
                    .State("NonTerminal",
                        StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("Cyclic")))
                    .State("Cyclic",
                        StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("NonTerminal")))));
        }

        [Fact]
        public void ChoiceStateWithOnlyCycles_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertDoesNotHaveTerminalPath(StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("Choice")))
                    .State("Choice", StateMachineBuilder.ChoiceState()
                        .DefaultStateName("Default")
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder
                                .Next("Initial"))
                            .Condition(StateMachineBuilder
                                .StringEquals("$.foo",
                                    "bar")))
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder
                                .Next("Default"))
                            .Condition(StateMachineBuilder
                                .StringEquals("$.foo",
                                    "bar"))))
                    .State("Default",
                        StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("Choice")))));
        }

        /**
         * While the nested ChoiceTwo state only has cycles, it has a cycle out of the choice state to
         * a state that contains a path to a terminal. The validator doesn't validate that the path out actually
         * has a path to the terminal so there are some invalid state machines that will pass validation.
         */
        [Fact]
        public void ChoiceStateWithPathOut_IsValid()
        {
            AssertNoCycle(
                StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("ChoiceOne")))
                    .State("ChoiceOne", StateMachineBuilder.ChoiceState()
                        .DefaultStateName("DefaultOne")
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder.Next("ChoiceTwo"))
                            .Condition(StateMachineBuilder.StringEquals("$.foo", "bar"))))
                    .State("DefaultOne", StateMachineBuilder.SucceedState())
                    .State("ChoiceTwo", StateMachineBuilder.ChoiceState()
                        .DefaultStateName("DefaultTwo")
                        .Choice(StateMachineBuilder.Choice()
                            .Transition(StateMachineBuilder.Next("ChoiceOne"))
                            .Condition(StateMachineBuilder.StringEquals("$.foo", "bar"))))
                    .State("DefaultTwo",
                        StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("ChoiceTwo"))));
        }

        [Fact]
        public void ChoiceStateWithPathToTerminal_IsValid()
        {
            AssertHasPathToTerminal(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.PassState()
                    .Transition(StateMachineBuilder.Next("Choice")))
                .State("Choice", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("Default")
                    .Choice(StateMachineBuilder.Choice()
                        .Transition(StateMachineBuilder.Next("Initial"))
                        .Condition(StateMachineBuilder.StringEquals("$.foo", "bar")))
                    .Choice(StateMachineBuilder.Choice()
                        .Transition(StateMachineBuilder.Next("Default"))
                        .Condition(StateMachineBuilder.StringEquals("$.foo", "bar"))))
                .State("Default", StateMachineBuilder.PassState().Transition(StateMachineBuilder.End())));
        }

        [Fact(Skip = "skip for now")]
        public void ParallelState_BranchContainsChoiceStateWithClosedCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertCycle(
                    StateMachineBuilder.StateMachine()
                        .StartAt("Initial")
                        .State("Initial", StateMachineBuilder.PassState()
                            .Transition(StateMachineBuilder.Next("Choice")))
                        .State("Choice", StateMachineBuilder.ChoiceState()
                            .DefaultStateName("Terminal")
                            .Choice(StateMachineBuilder.Choice()
                                .Transition(StateMachineBuilder.Next("Terminal"))
                                .Condition(StateMachineBuilder.StringEquals("$.foo", "bar")))
                            .Choice(StateMachineBuilder.Choice()
                                .Transition(StateMachineBuilder.Next("NonTerminal"))
                                .Condition(StateMachineBuilder.StringEquals("$.foo", "bar"))))
                        .State("Terminal", StateMachineBuilder.PassState().Transition(StateMachineBuilder.End()))
                        .State("NonTerminal",
                            StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("Cyclic")))
                        .State("Cyclic",
                            StateMachineBuilder.PassState().Transition(StateMachineBuilder.Next("NonTerminal")))));
        }

        [Fact]
        public void ParallelState_ChoiceStateWithTerminalPath_IsValid()
        {
            AssertHasPathToTerminal(
                StateMachineBuilder.StateMachine()
                    .StartAt("Parallel")
                    .State("Parallel", StateMachineBuilder.ParallelState()
                        .Transition(StateMachineBuilder.End())
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("Initial")
                            .State("Initial", StateMachineBuilder.PassState()
                                .Transition(StateMachineBuilder
                                    .Next("Choice")))
                            .State("Choice", StateMachineBuilder.ChoiceState()
                                .DefaultStateName("Default")
                                .Choice(StateMachineBuilder.Choice()
                                    .Transition(StateMachineBuilder
                                        .Next("Initial"))
                                    .Condition(StateMachineBuilder
                                        .StringEquals("$.foo",
                                            "bar")))
                                .Choice(StateMachineBuilder.Choice()
                                    .Transition(StateMachineBuilder
                                        .Next("Default"))
                                    .Condition(StateMachineBuilder
                                        .StringEquals("$.foo",
                                            "bar"))))
                            .State("Default",
                                StateMachineBuilder
                                    .PassState().Transition(StateMachineBuilder.End())))));
        }

        [Fact]
        public void ParallelState_NoCycles()
        {
            AssertNoCycle(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.ParallelState()
                    .Branch(StateMachineBuilder.SubStateMachine()
                        .StartAt("BranchOneStart")
                        .State("BranchOneStart", StateMachineBuilder.SucceedState()))
                    .Branch(StateMachineBuilder.SubStateMachine()
                        .StartAt("BranchTwoStart")
                        .State("BranchTwoStart", StateMachineBuilder.PassState()
                            .Transition(StateMachineBuilder
                                .Next("NextState")))
                        .State("NextState", StateMachineBuilder.SucceedState()))
                    .Transition(StateMachineBuilder.End())));
        }

        [Fact]
        public void ParallelState_WithChoiceThatHasNoTerminalPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertDoesNotHaveTerminalPath(
                    StateMachineBuilder.StateMachine()
                        .StartAt("Parallel")
                        .State("Parallel", StateMachineBuilder.ParallelState()
                            .Transition(StateMachineBuilder.End())
                            .Branch(StateMachineBuilder.SubStateMachine()
                                .StartAt("Initial")
                                .State("Initial",
                                    StateMachineBuilder
                                        .PassState()
                                        .Transition(StateMachineBuilder
                                            .Next("Choice")))
                                .State("Choice",
                                    StateMachineBuilder
                                        .ChoiceState()
                                        .DefaultStateName("Default")
                                        .Choice(StateMachineBuilder
                                            .Choice()
                                            .Transition(StateMachineBuilder
                                                .Next("Initial"))
                                            .Condition(StateMachineBuilder
                                                .StringEquals("$.foo",
                                                    "bar")))
                                        .Choice(StateMachineBuilder
                                            .Choice()
                                            .Transition(StateMachineBuilder
                                                .Next("Default"))
                                            .Condition(StateMachineBuilder
                                                .StringEquals("$.foo",
                                                    "bar"))))
                                .State("Default",
                                    StateMachineBuilder
                                        .PassState()
                                        .Transition(StateMachineBuilder
                                            .Next("Choice")))))));
        }

        [Fact]
        public void ParallelState_WithCycles_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertCycle(StateMachineBuilder.StateMachine()
                    .StartAt("Parallel")
                    .State("Parallel", StateMachineBuilder.ParallelState()
                        .Branch(StateMachineBuilder.SubStateMachine()
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StateMachineBuilder
                                .PassState()
                                .Transition(StateMachineBuilder
                                    .Next("CyclicState")))
                            .State("CyclicState", StateMachineBuilder
                                .PassState()
                                .Transition(StateMachineBuilder
                                    .Next("BranchOneInitial"))))
                        .Transition(StateMachineBuilder.End()))));
        }

        [Fact]
        public void SimpleStateMachine_WithCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                AssertCycle(StateMachineBuilder.StateMachine()
                    .StartAt("Initial")
                    .State("Initial", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("Next")))
                    .State("Next", StateMachineBuilder.PassState()
                        .Transition(StateMachineBuilder.Next("Initial")))));
        }

        [Fact]
        public void SingleTerminalState_HasNoCycle_IsValid()
        {
            AssertNoCycle(StateMachineBuilder.StateMachine()
                .StartAt("Initial")
                .State("Initial", StateMachineBuilder.SucceedState()));
        }
    }
}