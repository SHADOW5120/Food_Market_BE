namespace Food_Market_BE.Modules.SellerModule.DTOs.SellerNotifications
{
    public class SellerNotificationDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}