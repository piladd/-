namespace Messenger.Domain.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}