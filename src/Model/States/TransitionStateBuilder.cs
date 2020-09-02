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
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StatesLanguage.Model.States
{
    public abstract class ParameterStateBuilder<T, B> : TransitionStateBuilder<T,B>
        where T : State
        where B : ParameterStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.RESULT_PATH)]
        protected OptionalString _resultPath;

        [JsonProperty(PropertyNames.PARAMETERS)]
        protected JObject _parameters;
        
        /**
             * OPTIONAL. The value of “ResultPath” MUST be a Reference Path, which specifies the combination with or replacement of
             * the state’s result with its raw input. If not provided then the output completely replaces the input.
             *
             * @param resultPath New path value.
             * @return This object for method chaining.
             */
        public B ResultPath(ReferencePath resultPath)
        {
            _resultPath = resultPath.Path;
            return (B) this;
        }

        public B Parameters(JObject parameters)
        {
            _parameters = parameters;
            return (B) this;
        }
    }
    
    
    /**
     * Base class for states that allow transitions to either another state or
     * machine termination.
     */
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TransitionStateBuilder<T, B> : InputOutputStateBuilder<T, B> 
        where T : State
        where B : TransitionStateBuilder<T, B>
    {
        protected ITransitionBuilder<ITransition> _transition = NullTransitionBuilder<ITransition>.Instance;

        [JsonProperty(PropertyNames.END)]
        internal bool End
        {
            set
            {
                if (value)
                {
                    Transition(EndTransition.GetBuilder());
                }
            }
        }

        [JsonProperty(PropertyNames.NEXT)]
        internal string Next
        {
            set => Transition(NextStateTransition.GetBuilder().NextStateName(value));
        }

        /**
             * REQUIRED. Sets the transition that will occur when this state completes successfully.
             *
             * @param transition New transition.
             * @return This object for method chaining.
             */

        public B Transition<U>(ITransitionBuilder<U> transition) where U : ITransition
        {
            _transition = (ITransitionBuilder<ITransition>) transition;
            return (B) this;
        }
    }
}