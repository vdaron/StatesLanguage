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

namespace StatesLanguage.Model.States
{
    public class SucceedState : InputOutputState
    {
        private SucceedState()
        {
        }

        public override StateType Type => StateType.Succeed;

        [JsonIgnore]
        public override bool IsTerminalState => true;

        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /**
         * @return Builder instance to construct a {@link SucceedState}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link SucceedState}.
         */
        [JsonObject(MemberSerialization.OptIn)]
        public sealed class Builder : InputOutputStateBuilder<SucceedState, Builder>
        {
            internal Builder()
            {
            }

            /// <summary>
            /// return An immutable {@link SucceedState} object
            /// </summary>
            /// <returns></returns>
            public override SucceedState Build()
            {
                return new SucceedState
                       {
                           Comment = _comment,
                           InputPath = _inputPath,
                           OutputPath = _outputPath
                       };
            }
        }
    }
}