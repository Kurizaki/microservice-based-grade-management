namespace authentification_service.Models
{
    /// <summary>
    /// A Data Transfer Object for user credentials.
    /// Used to capture the username and password sent by the client.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// The username provided by the user.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// The plain-text password provided by the user.
        /// Must be hashed before storing.
        /// </summary>
        public required string Password { get; set; }
    }
}