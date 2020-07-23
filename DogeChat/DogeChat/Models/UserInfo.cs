namespace DogeChat.Models
{
    /// <summary>
    /// Holds information about a specific user.
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the profile image.
        /// </summary>
        public string Image { get; set; }
    }
}
