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
using StatesLanguage.Model.Internal;
using Newtonsoft.Json;

namespace StatesLanguage.Model.States
{
    public class Retrier
    {
        private Retrier()
        {
        }
        
        public List<string> ErrorEquals { get; private set; }


        [JsonProperty(PropertyNames.INTERVAL_SECONDS)]
        private int? _interval;
        [JsonProperty(PropertyNames.MAX_ATTEMPTS)]
        private int? _attempts;
        [JsonProperty(PropertyNames.BACKOFF_RATE)]
        private double? _backoff;

        [JsonIgnore]
        public int IntervalSeconds => _interval ?? 1;
        [JsonIgnore]
        public int MaxAttempts => _attempts ?? 3;
        [JsonIgnore]
        public double BackoffRate => _backoff ?? 2.0;

        /**
         * @return Builder instance to construct a {@link Retrier}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Builder for a {@link Retrier}.
         */
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

            internal Builder()
            {
            }

            /**
             * @return An immutable {@link Retrier} object.
             */
            public Retrier Build()
            {
                return new Retrier
                       {
                           ErrorEquals = new List<string>(_errorEquals),
                           _interval = _intervalSeconds,
                           _attempts = _maxAttempts,
                           _backoff = _backoffRate
                       };
            }

            /**
             * REQUIRED. Adds the codes to the list of error codes that this retrier handles. If the retrier matches an error code
             * then the state may be retried according to the retry parameters.
             *
             * @param errorEquals New error codes to add to this retrier's handled errors.
             * @return This object for method chaining.
             */
            public Builder ErrorEquals(params string[] errorEquals)
            {
                _errorEquals.AddRange(errorEquals);
                return this;
            }

            /**
             * OPTIONAL. Makes this retrier handle all errors. This method should not be used with {@link #errorEquals}.
             *
             * @return This object for method chaining.
             */
            public Builder RetryOnAllErrors()
            {
                _errorEquals.Clear();
                ErrorEquals(ErrorCodes.ALL);
                return this;
            }

            /**
             * OPTIONAL. Delay before the first retry attempt. The default value is 1 second. The delay for subsequent retries will
             * be
             * computed by applying the {@link #backoffRate} multiplier to the previous delay.
             *
             * @param intervalSeconds Delay in seconds. Positive integer.
             * @return This object for method chaining.
             */
            public Builder IntervalSeconds(int intervalSeconds)
            {
                _intervalSeconds = intervalSeconds;
                return this;
            }

            /**
             * OPTIONAL. Max number of retry attempts this retrier can perform. The default value is 3 if not specified. <p>Note that
             * 0 is a legal value for MaxAttempts and represents that the error should not be retried.</p>
             *
             * @param maxAttempts Number of max attempts. Non-negative integer.
             * @return This object for method chaining.
             */
            public Builder MaxAttempts(int maxAttempts)
            {
                _maxAttempts = maxAttempts;
                return this;
            }

            /**
             * OPTIONAL. Multiplier that increases the {@link #intervalSeconds(Integer)} on each attempt. The default value is 2.0.
             *
             * @param backoffRate Multiplier for {@link #intervalSeconds(Integer)}. Must be greater than or equal to 1.0.
             * @return This object for method chaining.
             */
            public Builder BackoffRate(double backoffRate)
            {
                _backoffRate = backoffRate;
                return this;
            }
        }
    }
}