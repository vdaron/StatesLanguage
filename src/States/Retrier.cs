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
using Newtonsoft.Json;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public class Retrier
    {
        /// <summary>
        ///     Max number of retry attempts this retrier can perform. The default value is 3 if not specified.
        /// </summary>
        [JsonProperty(PropertyNames.MAX_ATTEMPTS)]
        private int? _attempts;

        /// <summary>
        ///     Multiplier that increases the <see cref="IntervalSeconds" /> on each attempt. The default value is 2.0.
        /// </summary>
        [JsonProperty(PropertyNames.BACKOFF_RATE)]
        private double? _backoff;
        
        /// <summary>
        ///     A positive integer that sets the maximum value, in seconds, up to which a retry interval can increase. Value must greater than 0 and less than 31622401. 
        /// </summary>
        [JsonProperty(PropertyNames.MAX_DELAY_SECONDS)]
        private int? _maxDelaySeconds;
        
        /// <summary>
        ///  A string that determines whether or not to include jitter in the wait times between consecutive retry attempts.
        ///  Jitter reduces simultaneous retry attempts by spreading these out over a randomized delay interval.
        ///  This string accepts FULL or NONE as its values. The default value is NONE.
        /// </summary>
        [JsonProperty(PropertyNames.JITTER_STRATEGY)]
        private string _jitterStrategy = "NONE";

        /// <summary>
        ///     Delay before the first retry attempt.
        /// </summary>
        [JsonProperty(PropertyNames.INTERVAL_SECONDS)]
        private int? _interval;

        private Retrier()
        {
        }

        /// <summary>
        ///     codes to the list of error codes that this retrier handles
        /// </summary>
        [JsonProperty(PropertyNames.ERROR_EQUALS)]
        public List<string> ErrorEquals { get; private set; }

        [JsonIgnore]
        public int IntervalSeconds => _interval ?? 1;

        [JsonIgnore]
        public int MaxAttempts => _attempts ?? 3;

        [JsonIgnore]
        public double BackoffRate => _backoff ?? 2.0;

        [JsonIgnore]
        public int? MaxDelaySeconds => _maxDelaySeconds;

        [JsonIgnore]
        public string JitterStrategy => _jitterStrategy;

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public sealed class Builder : IBuildable<Retrier>
        {
            [JsonProperty(PropertyNames.BACKOFF_RATE)]
            private double? _backoffRate;

            [JsonProperty(PropertyNames.ERROR_EQUALS)]
            private List<string> _errorEquals = new List<string>();

            [JsonProperty(PropertyNames.INTERVAL_SECONDS)]
            private int? _intervalSeconds;

            [JsonProperty(PropertyNames.MAX_ATTEMPTS)]
            private int? _maxAttempts;
            
            [JsonProperty(PropertyNames.MAX_DELAY_SECONDS)]
            private int? _maxDelaySeconds;

            [JsonProperty(PropertyNames.JITTER_STRATEGY)]
            private string _jitterStrategy;

            internal Builder()
            {
            }

            public Retrier Build()
            {
                if (_maxDelaySeconds is <= 0 or >= 31622401)
                    throw new StatesLanguageException("MaxDelaySeconds must be bigger than 0 and less than 31622401");
                
                return new Retrier
                {
                    ErrorEquals = new List<string>(_errorEquals),
                    _interval = _intervalSeconds,
                    _attempts = _maxAttempts,
                    _backoff = _backoffRate,
                    _jitterStrategy = _jitterStrategy,
                    _maxDelaySeconds = _maxDelaySeconds
                };
            }

            /// <summary>
            ///     REQUIRED. Adds the codes to the list of error codes that this retrier handles. If the retrier matches an error code
            ///     then the state may be retried according to the retry parameters.
            /// </summary>
            /// <param name="errorEquals">New error codes to add to this retrier's handled errors.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder ErrorEquals(params string[] errorEquals)
            {
                _errorEquals.AddRange(errorEquals);
                return this;
            }

            /// <summary>
            ///     OPTIONAL. Makes this retrier handle all errors. This method should not be used with <see cref="ErrorEquals" />.
            /// </summary>
            /// <returns>This object for method chaining.</returns>
            public Builder RetryOnAllErrors()
            {
                _errorEquals.Clear();
                ErrorEquals(ErrorCodes.ALL);
                return this;
            }

            /// <summary>
            ///     OPTIONAL. Delay before the first retry attempt. The default value is 1 second. The delay for subsequent retries
            ///     will be
            ///     computed by applying the <see cref="BackoffRate" /> multiplier to the previous delay.
            /// </summary>
            /// <param name="intervalSeconds">Delay in seconds. Positive integer.</param>
            /// <returns>This object for method chaining.</returns>
            public Builder IntervalSeconds(int intervalSeconds)
            {
                _intervalSeconds = intervalSeconds;
                return this;
            }

            /// <summary>
            ///     OPTIONAL. Max number of retry attempts this retrier can perform. The default value is 3 if not specified.
            ///     <p>
            ///         Note that
            ///         0 is a legal value for MaxAttempts and represents that the error should not be retried.
            ///     </p>
            /// </summary>
            /// <param name="maxAttempts">Number of max attempts. Non-negative integer.</param>
            /// <returns> This object for method chaining.</returns>
            public Builder MaxAttempts(int maxAttempts)
            {
                _maxAttempts = maxAttempts;
                return this;
            }

            /// <summary>
            ///     OPTIONAL. Multiplier that increases the <see cref="IntervalSeconds" /> on each attempt. The default value is 2.0.
            /// </summary>
            /// <param name="backoffRate">Multiplier for <see cref="IntervalSeconds" />. Must be greater than or equal to 1.0.</param>
            /// <returns> This object for method chaining.</returns>
            public Builder BackoffRate(double backoffRate)
            {
                _backoffRate = backoffRate;
                return this;
            }
            
            /// <summary>
            ///     OPTIONAL.  A positive integer that sets the maximum value, in seconds, up to which a retry interval can increase. Value must greater than 0 and less than 31622401. 
            /// </summary>
            /// <param name="maxDelaySeconds"></param>
            /// <returns> This object for method chaining.</returns>
            public Builder MaxDelaySeconds(int maxDelaySeconds)
            {
                _maxDelaySeconds = maxDelaySeconds;
                return this;
            }
            
            /// <summary>
            ///     OPTIONAL. Use the Full Jitter strategy
            /// </summary>
            /// <returns> This object for method chaining.</returns>
            public Builder FullJitterStrategy()
            {
                _jitterStrategy = "FULL";
                return this;
            }
            
            /// <summary>
            ///     OPTIONAL. Disable Jitter strategy
            /// </summary>
            /// <returns> This object for method chaining.</returns>
            public Builder NoJitterStrategy()
            {
                _jitterStrategy = "NONE";
                return this;
            }
        }
    }
}