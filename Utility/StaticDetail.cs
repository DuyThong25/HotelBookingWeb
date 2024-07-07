using System.Globalization;

namespace HotelBookingWeb.Utility
{
    public class StaticDetail
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";

        public const string RoomStatus_Available = "Available";
        public const string RoomStatus_Unavailable = "Unavailable";

        public static string OrderStatus_Pending = "Pending";
        public static string OrderStatus_Approved = "Approved";

        public static string PaymentStatus_Pending = "Pending";
        public static string PaymentStatus_Approved = "Approved";
        //VND format
        public static string VndCurrency(double? input)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return input?.ToString("#,### VNĐ", cul.NumberFormat);
        }
    }
}
