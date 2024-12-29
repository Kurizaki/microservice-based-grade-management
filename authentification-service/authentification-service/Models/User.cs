namespace authentification_service.Models
{
    /// <summary>
    /// Represents a User entity in the authentication system,
    /// stored in the database via Entity Framework.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key for the User entity.
        /// Used by EF Core to identify each record uniquely.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The username associated with this user.
        /// Stored as a string and used for login identification.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Stores the hashed version of the user's password.
        /// Do NOT store plain-text passwords.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
    }
}