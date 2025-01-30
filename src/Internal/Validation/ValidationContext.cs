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
using System.Collections;
using Newtonsoft.Json.Linq;
using StatesLanguage.IntrinsicFunctions;
using StatesLanguage.ReferencePaths;

namespace StatesLanguage.Internal.Validation
{
    /**
     * Contains context about the current validation scope and factory methods
     * for new sub-contexts and for reporting various common problems.
     */
    public class ValidationContext
    {
        private ValidationContext()
        {
        }

        public string Identifier { get; private set; }
        public Location Location { get; private set; }
        public ValidationContext ParentContext { get; private set; }

        public ProblemReporter ProblemReporter { get; private set; }

        /**
         * @return Path to validation error given current context.
         */
        public string Path
        {
            get
            {
                var parentPath = ParentContext == null ? "" : ParentContext.Path + ".";
                switch (Location)
                {
                    case Location.Branch:
                    case Location.Retrier:
                    case Location.Catcher:
                    case Location.Choice:
                    case Location.State:
                        return $"{parentPath}{Location.ToString()}[{Identifier}]";
                    case Location.StateMachine:
                        return "StateMachine";
                    default:
                        return "Unknown";
                }
            }
        }

        /**
         * @return Builder instance to construct a {@link ValidationContext}.
         */
        public static Builder GetBuilder()
        {
            return new Builder();
        }

        /**
         * Asserts the value is not null, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param propertyValue Value to Assert on.
         * @param propertyName  Name of property.
         */
        public void AssertNotNull(object propertyValue, string propertyName)
        {
            if (propertyValue == null)
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} is a required property."));
            }
        }

        /**
         * Asserts the string is not null and not empty, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param propertyValue Value to Assert on.
         * @param propertyName  Name of property.
         */
        public void AssertStringNotEmpty(string propertyValue, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyValue))
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} is a required property."));
            }
        }
        /**
 * Asserts the string is null or empty, reporting to {@link ProblemReporter} with this context if it is.
 *
 * @param propertyValue Value to Assert on.
 * @param propertyName  Name of property.
 */
        public void AssertStringEmpty(string propertyValue, string propertyName, string message)
        {
            if (!string.IsNullOrEmpty(propertyValue))
            {
                ProblemReporter.Report(new Problem(this, message));
            }
        }
        

        /**
         * Asserts the collection is not null and not empty, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param collection   Collection to Assert on.
         * @param propertyName Name of property.
         */
        public void AssertNotEmpty(IEnumerable collection, string propertyName)
        {
            if (!collection.GetEnumerator().MoveNext())
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} requires one or more items"));
            }
        }

        /**
         * Asserts the map is not null and not empty, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param map          Map to Assert on.
         * @param propertyName Name of property.
         */
        public void AssertNotEmpty(IDictionary map, string propertyName)
        {
            if (map == null || map.Count == 0)
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} requires one or more entries"));
            }
        }

        /**
         * Asserts the integer is either null or positive, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param integer      Value to Assert on.
         * @param propertyName Name of property.
         */
        public void AssertIsPositiveIfPresent(int? integer, string propertyName)
        {
            if (integer.HasValue && integer <= 0)
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} must be positive"));
            }
        }

        /**
         * Asserts the integer is either null or non-negative, reporting to {@link ProblemReporter} with this context if it is.
         *
         * @param integer      Value to Assert on.
         * @param propertyName Name of property.
         */
        public void AssertIsNotNegativeIfPresent(int? integer, string propertyName)
        {
            if (integer.HasValue && integer < 0)
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} must be non negative"));
            }
        }

        /**
         * Asserts that the string represents a valid JsonPath expression.
         *
         * @param path Path expression to validate.
         */
        public void AssertIsValidInputPath(string path)
        {
            AssertIsValidJsonPath(path, PropertyNames.INPUT_PATH);
        }

        /**
         * Asserts that the string represents a valid JsonPath expression.
         *
         * @param path Path expression to validate.
         */
        public void AssertIsValidOutputPath(string path)
        {
            AssertIsValidJsonPath(path, PropertyNames.OUTPUT_PATH);
        }

        /**
         * Asserts that the string represents a valid JsonPath expression.
         *
         * @param path Path expression to validate.
         */
        public void AssertIsValidResultPath(string path)
        {
            AssertIsValidReferencePath(path, PropertyNames.RESULT_PATH);
        }

        public void AssertIsValidItemPath(string path)
        {
            AssertIsValidReferencePath(path, PropertyNames.ITEMS_PATH);
        }

        /**
         * Asserts that the string represents a valid JsonPath expression.
         *
         * @param path         Path expression to validate.
         * @param propertyName Name of property.
         */
        public void AssertIsValidJsonPath(string path, string propertyName)
        {
            if (path == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} cannot be empty"));
            }

            // Newtonsoft.Json seems to allow JsonPath to not start with $
            if (path[0] != '$')
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} with value '{path}' is not a valid JsonPath. Must Start with '$'"));
            }

            try
            {
                var token = JToken.Parse("{}").SelectTokens(path);
            }
            catch (Exception e)
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} with value '{path}' is not a valid JsonPath. {e.Message}"));
            }
        }

        /**
         * Asserts that the string represents a valid reference path expression.
         *
         * @param path         Path expression to validate.
         * @param propertyName Name of property.
         */
        public void AssertIsValidReferencePath(string path, string propertyName)
        {
            if (path == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} cannot be empty"));
            }

            try
            {
                var r = ReferencePath.Parse(path);
            }
            catch (InvalidReferencePathException e)
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} with value '{path}' is not a valid ReferencePath. {e.Message}"));
            }
        }

        /**
         * Asserts that the string represents a valid intrinsic function.
         *
         * @param path         Intrinsic Function to validate.
         * @param propertyName Name of property.
         */
        public void AssertIsValidIntrinsicFunction(string path, string propertyName)
        {
            if (path == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                ProblemReporter.Report(new Problem(this, $"{propertyName} cannot be empty"));
            }

            try
            {
                var r = IntrinsicFunction.Parse(path);
            }
            catch (InvalidIntrinsicFunctionException e)
            {
                ProblemReporter.Report(new Problem(this,
                    $"{propertyName} with value '{path}' is not a valid IntrinsicFunction. {e.Message}"));
            }
        }

        /**
         * @param stateName Name of state.
         * @return State sub-context.
         */
        public ValidationContext State(string stateName)
        {
            return NewChildContext()
                .Identifier(stateName)
                .Location(Location.State)
                .Build();
        }

        /**
         * @param index Index of retrier.
         * @return Retrier sub-context.
         */
        public ValidationContext Retrier(int index)
        {
            return NewChildContext()
                .Identifier(index.ToString())
                .Location(Location.Retrier)
                .Build();
        }

        /**
         * @param index Index of branch.
         * @return Branch sub-context.
         */
        public ValidationContext Branch(int index)
        {
            return NewChildContext()
                .Identifier(index.ToString())
                .Location(Location.Branch)
                .Build();
        }

        public ValidationContext Iterator()
        {
            return NewChildContext()
                .Location(Location.Iterator)
                .Build();
        }

        /**
         * @param index Index of choice.
         * @return Choice sub-context.
         */
        public ValidationContext Choice(int index)
        {
            return NewChildContext()
                .Identifier(index.ToString())
                .Location(Location.Choice)
                .Build();
        }

        /**
         * @param index Index of catcher.
         * @return Catcher sub-context.
         */
        public ValidationContext Catcher(int index)
        {
            return NewChildContext()
                .Identifier(index.ToString())
                .Location(Location.Catcher)
                .Build();
        }

        /**
         * @return Sub-context with this context as the parent and the
         * same problem reporter.
         */
        private Builder NewChildContext()
        {
            return GetBuilder()
                .ParentContext(this)
                .ProblemReporter(ProblemReporter);
        }

        /**
         * Builder for a {@link ValidationContext}.
         */
        public sealed class Builder
        {
            private string _identifier;
            private Location _location;
            private ValidationContext _parentContext;
            private ProblemReporter _problemReporter;

            internal Builder()
            {
            }

            /**
             * Sets the parent of the new context. May be null if at the root.
             *
             * @param parentContext Parent of context being built.
             * @return This object for method chaining.
             */
            public Builder ParentContext(ValidationContext parentContext)
            {
                _parentContext = parentContext;
                return this;
            }

            /**
             * Sets the location of the context.
             *
             * @return This object for method chaining.
             */
            public Builder Location(Location location)
            {
                _location = location;
                return this;
            }

            /**
             * Sets an additional identifier (i.e. state name, branch index, etc) for the context. May be null.
             *
             * @return This object for method chaining.
             */
            public Builder Identifier(string identifier)
            {
                _identifier = identifier;
                return this;
            }

            /**
             * Sets the problem reporter to report problems in Assertion methods.
             *
             * @return This object for method chaining.
             */
            public Builder ProblemReporter(ProblemReporter problemReporter)
            {
                _problemReporter = problemReporter;
                return this;
            }

            /**
             * @return An immutable {@link ValidationContext} object.
             */
            public ValidationContext Build()
            {
                return new ValidationContext
                {
                    ParentContext = _parentContext,
                    Location = _location,
                    Identifier = _identifier,
                    ProblemReporter = _problemReporter
                };
            }
        }
    }
}