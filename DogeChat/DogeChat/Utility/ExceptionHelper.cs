using System;
using System.Linq;

namespace DogeChat.Utility
{
    /// <summary>
    /// Provides methods for throwing exceptions conditionally.
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Throws a new <see cref="ArgumentNullException"/>
        /// if the given <paramref name="value"/>
        /// of the argument with the given <paramref name="name"/>
        /// is null.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the argument's value is null.
        /// </exception>
        public static void ThrowIfNull(string name, object value)
        {
            if (value != null) return;

            throw new ArgumentNullException(name, "Argument cannot be null.");
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>
        /// if the given <paramref name="value"/>
        /// of the argument with the given <paramref name="name"/>
        /// is null or contains white-space characters only.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the argument's value is null or consists of white-space characters only.
        /// </exception>
        public static void ThrowIfNullOrWhiteSpace(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return;

            throw new ArgumentException("Argument cannot be null or contain white-space characters only.", name);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>
        /// if the given <paramref name="value"/>
        /// of the argument with the given <paramref name="name"/>
        /// falls outside the valid range.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="invalidValue">The value the argument cannot take.</param>
        /// <param name="invalidValues">Optional values the argument cannot take.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the argument's value falls outside of the valid range.
        /// </exception>
        public static void ThrowIfOutOfRange<T>(string name, T value, T invalidValue, params T[] invalidValues)
            where T : IComparable
        {
            var isInvalidValue = value.CompareTo(invalidValue) == 0
                                 || invalidValues?.Any(v => value.CompareTo(v) == 0) == true;
            if (!isInvalidValue) return;

            ThrowArgumentOutOfRangeException(name, value);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>
        /// if the given <paramref name="value"/>
        /// of the argument with the given <paramref name="name"/>
        /// falls outside the valid range.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="lowerBound">The minimum allowed value.</param>
        /// <param name="upperBound">The maximum allowed value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the argument's value falls outside of the valid range.
        /// </exception>
        public static void ThrowIfOutOfRange<T>(string name, T value, T lowerBound, T upperBound)
            where T : IComparable
        {
            var isInvalidValue = value.CompareTo(lowerBound) < 0
                                 || value.CompareTo(upperBound) > 0;
            if (!isInvalidValue) return;

            ThrowArgumentOutOfRangeException(name, value);
        }

        private static void ThrowArgumentOutOfRangeException(string name, object value) =>
            throw new ArgumentOutOfRangeException(name, value, "Argument cannot be a value that falls outside the valid range.");
    }
}
