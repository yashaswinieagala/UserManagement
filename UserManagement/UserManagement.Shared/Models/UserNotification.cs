namespace UserManagement.Shared.Models
{
    public class UserNotification
    {
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public UserDetail? User { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
