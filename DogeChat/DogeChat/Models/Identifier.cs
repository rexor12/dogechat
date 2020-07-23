using DogeChat.Utility;
using System;

namespace DogeChat.Models
{
    /// <summary>
    /// Represents an identifier.
    /// </summary>
    public readonly struct Identifier
    {
        private readonly Guid _guid;

        /// <summary>
        /// Initializes a new instance of <see cref="Identifier"/>.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to be used as the identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument's value falls outside of the valid range.</exception>
        public Identifier(Guid guid)
        {
            ExceptionHelper.ThrowIfOutOfRange(nameof(guid), guid, Guid.Empty);

            _guid = guid;
        }

        /// <summary>
        /// Casts the given <see cref="Identifier"/> to its <see cref="string"/> representation.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to be cast.</param>
        public static implicit operator string(Identifier identifier) => identifier.ToString();

        /// <summary>
        /// Casts the given <paramref name="guid"/> to an <see cref="Identifier"/>.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to be cast.</param>
        public static explicit operator Identifier(Guid guid) => new Identifier(guid);

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => _guid.ToString();
    }
}
