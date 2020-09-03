using System;
using System.Text.RegularExpressions;

namespace StatesLanguage.Model
{
    internal static class Ensure
    {
        public static void IsNotNullNorEmpty<T>(string param) where T : Exception, new()
        {
            if (string.IsNullOrEmpty(param))
            {
                throw new T();
            }
        }

        public static void IsAtMostNCharacter<T>(string param, int numberOfChars) where T : Exception, new()
        {
            if (!string.IsNullOrEmpty(param) && param.Length > numberOfChars)
            {
                throw new T();
            }
        }

        public static void MatchRegex<T>(string param, string regex) where T : Exception, new()
        {
            if (!Regex.IsMatch(param, regex))
            {
                throw new T();
            }
        }

        public static void IsNull<T>(object exec) where T : Exception, new()
        {
            if (exec != null)
            {
                throw new T();
            }
        }

        public static void IsNotNull<T>(object exec) where T : Exception, new()
        {
            if (exec == null)
            {
                throw new T();
            }
        }

        public static void IsNot<T>(Enum value, Enum forbiddenValue) where T : Exception, new()
        {
            if (Equals(value, forbiddenValue))
            {
                throw new T();
            }
        }
    }
}