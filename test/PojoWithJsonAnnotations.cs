﻿/*
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

using Newtonsoft.Json;

namespace StatesLanguage.Tests
{
    public class PojoWithJacksonAnnotations
    {
        [JsonIgnore]
        public string baz { get; set; }

        [JsonProperty("FooProperty")]
        public string foo { get; set; }

        [JsonProperty("BarProperty")]
        public string bar { get; set; }
    }
}