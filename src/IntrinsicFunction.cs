using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using StatesLanguage.Internal.Validation;

namespace StatesLanguage
{
    public abstract class IntrinsicParam
    {
    }

    public class NullIntrinsicParam : IntrinsicParam
    {
    }

    public class NumberIntrinsicParam : IntrinsicParam
    {
        public NumberIntrinsicParam(decimal number)
        {
            Number = number;
        }

        public decimal Number { get; }
    }

    public class PathIntrinsicParam : IntrinsicParam
    {
        public PathIntrinsicParam(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    public class StringIntrinsicParam : IntrinsicParam
    {
        public StringIntrinsicParam(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class IntrinsicFunction : IntrinsicParam
    {
        public string Name { get; set; }
        public IntrinsicParam[] Parameters { get; set; }

        public static IntrinsicFunction Parse(string intrinsicFunctionDefinition)
        {
            return IntrinsicFunctionParser.Parse(intrinsicFunctionDefinition);
        }
    }

    internal class IntrinsicFunctionParser
    {
        private readonly string _intrinsicFunction;
        private int _currentIndex;

        private IntrinsicFunctionParser(string intrinsicFunction)
        {
            _intrinsicFunction = intrinsicFunction;
        }

        public static IntrinsicFunction Parse(string function)
        {
            var parser = new IntrinsicFunctionParser(function);

            var f = parser.ReadUnquotedParam();
            if (f is IntrinsicFunction fun)
            {
                return fun;
            }

            throw new InvalidIntrinsicFunctionException("Invalid Intrinsic Function");
        }

        private IEnumerable<IntrinsicParam> ParseParameters()
        {
            var forbiddenComma = true;
            var end = false;

            while (_currentIndex < _intrinsicFunction.Length && !end)
            {
                var current = _intrinsicFunction[_currentIndex];

                switch (current)
                {
                    case ')':
                        end = true;
                        break;
                    case ' ':
                        _currentIndex++;
                        break;
                    case ',':
                        if (forbiddenComma)
                        {
                            throw new InvalidIntrinsicFunctionException("Parameter Excepted");
                        }

                        forbiddenComma = true;
                        _currentIndex++;
                        break;
                    case '\'':
                        yield return ReadQuotedString();
                        forbiddenComma = false;
                        break;
                    default:
                        yield return ReadUnquotedParam();
                        forbiddenComma = false;
                        break;
                }
            }

            if (!end)
            {
                throw new InvalidIntrinsicFunctionException("Missing ')' while parsing function");
            }
        }

        private IntrinsicParam ReadUnquotedParam()
        {
            var sb = new StringBuilder();
            var end = false;
            var isIntrinsicFunction = false;

            while (_currentIndex < _intrinsicFunction.Length && !end)
            {
                var currentChar = _intrinsicFunction[_currentIndex];
                switch (currentChar)
                {
                    case '(':
                        isIntrinsicFunction = true;
                        end = true;
                        _currentIndex++;
                        break;
                    case ')':
                    case ',':
                    case ' ':
                        end = true;
                        break;
                    case '\\':
                        sb.Append(ReadEscapedChar());
                        _currentIndex++;
                        break;
                    default:
                        sb.Append(currentChar);
                        _currentIndex++;
                        break;
                }
            }

            if (!end)
            {
                throw new InvalidIntrinsicFunctionException("Path ended with an open string.");
            }

            var p = sb.ToString();

            if (isIntrinsicFunction)
            {
                return new IntrinsicFunction
                {
                    Name = p,
                    Parameters = ParseParameters().ToArray()
                };
            }

            if (p == "null")
            {
                return new NullIntrinsicParam();
            }

            if (p.StartsWith("$"))
            {
                return new PathIntrinsicParam(p);
            }

            decimal i;
            if (decimal.TryParse(p, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out i))
            {
                return new NumberIntrinsicParam(i);
            }

            throw new InvalidIntrinsicFunctionException("Invalid parameter");
        }

        private StringIntrinsicParam ReadQuotedString()
        {
            var sb = new StringBuilder();
            var end = false;

            // Skip ' 
            _currentIndex++;
            while (_currentIndex < _intrinsicFunction.Length && !end)
            {
                var currentChar = _intrinsicFunction[_currentIndex];
                switch (currentChar)
                {
                    case '\'':
                        end = true;
                        break;
                    case '\\':
                        sb.Append(ReadEscapedChar());
                        break;
                    default:
                        sb.Append(currentChar);
                        break;
                }

                _currentIndex++;
            }

            if (!end)
            {
                throw new InvalidIntrinsicFunctionException("Path ended with an open string.");
            }

            return new StringIntrinsicParam(sb.ToString());
        }

        private char ReadEscapedChar()
        {
            _currentIndex++;
            if (_currentIndex >= _intrinsicFunction.Length)
            {
                throw new InvalidIntrinsicFunctionException("Missing Escaped char");
            }

            return _intrinsicFunction[_currentIndex];
        }
    }
}