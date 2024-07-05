namespace HotelBookingWeb.Utility
{
    public class StaticDetail
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";

        public const string RoomStatus_Available = "Available";
        public const string RoomStatus_Unavailable = "Unavailable";

        //VND format
        public static string VndCurrency()
        {
            return "#,##0.00 VNĐ";
        }
    }
}
