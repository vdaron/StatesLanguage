using System.Collections.Generic;
using System.Text;
using StatesLanguage.Model.Internal.Validation;
using StatesLanguage.Model.ReferencePathTokens;

namespace StatesLanguage.Model
{
    public class ReferencePath
    {
        private int _currentIndex;

        private ReferencePath(string path)
        {
            Path = path;
            Parts = new List<PathToken>();

            ParseMain();
        }

        public List<PathToken> Parts { get; }
        public string Path { get; }

        public static ReferencePath Parse(string expression)
        {
            return new ReferencePath(expression);
        }

        public static implicit operator string(ReferencePath referencePath)
        {
            return referencePath.Path;
        }

        public static implicit operator ReferencePath(string referencePath)
        {
            return Parse(referencePath);
        }

        private void ParseMain()
        {
            if (string.IsNullOrEmpty(Path) || Path[_currentIndex] != '$')
            {
                throw new InvalidReferencePathException("Reference path must start with '$'");
            }

            if (Path.Length == 1)
            {
                return;
            }

            _currentIndex++;

            ParsePath();
        }

        private void ParsePath()
        {
            while (_currentIndex < Path.Length)
            {
                var currentChar = Path[_currentIndex];

                switch (currentChar)
                {
                    case '[':
                        Parts.Add(ParseIndexer());
                        break;
                    case '.':
                        if (_currentIndex + 1 < Path.Length && Path[_currentIndex + 1] == '.')
                        {
                            throw new InvalidReferencePathException(
                                "Scan operation '..' are not allowed in reference paths");
                        }

                        Parts.Add(ParseProperty());
                        break;
                    default:
                        throw new InvalidReferencePathException(
                            $"Unexpected character '{currentChar}' in Reference path at pos {_currentIndex}");
                }
            }
        }

        private PathToken ParseProperty()
        {
            _currentIndex++;

            EnsureLength("Path ended with open indexer.");

            return CreatePathFilter(ReadUnquotedString());
        }

        private static PathToken CreatePathFilter(string member)
        {
            return new FieldToken(member);
        }

        private PathToken ParseIndexer()
        {
            _currentIndex++;

            EnsureLength("Path ended with open indexer.");
            return Path[_currentIndex] switch
            {
                '\'' => ParseQuotedField(),
                '?' => throw new InvalidReferencePathException("Filter expression '[?(xxx)]' are not supported"),
                _ => ParseArrayIndexer()
            };
        }

        private PathToken ParseArrayIndexer()
        {
            var end = false;
            var builder = new StringBuilder();

            while (_currentIndex < Path.Length && !end)
            {
                var currentCharacter = Path[_currentIndex];

                switch (currentCharacter)
                {
                    case ']':
                        end = true;
                        break;
                    case ',':
                        throw new InvalidReferencePathException("Operator ',' not supported in ReferencePath");
                    case '*':
                        throw new InvalidReferencePathException("Operator '*' not supported in ReferencePath");
                    case ':':
                        throw new InvalidReferencePathException("Operator ':' not supported in ReferencePath");
                    default:
                        if (!char.IsDigit(currentCharacter))
                        {
                            throw new InvalidReferencePathException(
                                $"Unexpected character while parsing path indexer: {currentCharacter}");
                        }

                        builder.Append(currentCharacter);
                        break;
                }

                _currentIndex++;
            }

            return new ArrayIndexToken(int.Parse(builder.ToString()));
        }

        private PathToken ParseQuotedField()
        {
            var field = ReadQuotedString();

            EnsureLength("Path ended with open indexer.");

            switch (Path[_currentIndex])
            {
                case ']':
                    _currentIndex++;
                    return CreatePathFilter(field);
                case ',':
                    throw new InvalidReferencePathException("Operator ',' not permitted");
                default:
                    throw new InvalidReferencePathException(
                        $"Unexpected character while parsing path indexer: {Path[_currentIndex]}");
            }
        }

        private string ReadQuotedString()
        {
            var sb = new StringBuilder();
            var end = false;
            _currentIndex++;
            while (_currentIndex < Path.Length && !end)
            {
                var currentChar = Path[_currentIndex];
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
                throw new InvalidReferencePathException("Path ended with an open string.");
            }

            return sb.ToString();
        }

        private string ReadUnquotedString()
        {
            var end = false;
            var result = new StringBuilder();

            while (_currentIndex < Path.Length && !end)
            {
                var currentChar = Path[_currentIndex];
                switch (currentChar)
                {
                    case '.':
                    case '[':
                        end = true;
                        break;
                    case ']':
                    case '*':
                    case '?':
                    case '\'':
                        throw new InvalidReferencePathException(
                            $"Invalid character '{currentChar}' in property name at pos {_currentIndex}");
                    case '\\':
                        result.Append(ReadEscapedChar());
                        break;
                    default:
                        result.Append(currentChar);
                        break;
                }

                if (!end)
                {
                    _currentIndex++;
                }
            }

            if (result.Length == 0)
            {
                throw new InvalidReferencePathException("Field name cannot be empty");
            }

            return result.ToString();
        }

        private char ReadEscapedChar()
        {
            _currentIndex++;
            EnsureLength("Incomplete escape sequence");
            return Path[_currentIndex];
        }

        private void EnsureLength(string message)
        {
            if (_currentIndex >= Path.Length)
            {
                throw new InvalidReferencePathException(message);
            }
        }
    }
}