namespace Messenger.Domain.Models;

public class SendMessageRequest
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
}