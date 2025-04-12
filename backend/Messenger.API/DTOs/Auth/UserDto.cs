namespace Messenger.API.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }

    public string Token { get; set; }
}