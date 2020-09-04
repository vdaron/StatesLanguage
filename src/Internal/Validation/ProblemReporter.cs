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
using System.Text;

namespace StatesLanguage.Internal.Validation
{
    public class ProblemReporter
    {
        private readonly List<Problem> _problems = new List<Problem>();

        public void Report(Problem problem)
        {
            _problems.Add(problem);
        }

        public bool HasProblems()
        {
            return _problems.Count > 0;
        }

        public ValidationException GetException()
        {
            var exceptionMessage = new StringBuilder();
            foreach (var p in _problems)
            {
                exceptionMessage.AppendFormat("\n{0}: {1}", p.Context.Path, p.Message);
            }

            return new ValidationException(exceptionMessage.ToString());
        }
    }
}