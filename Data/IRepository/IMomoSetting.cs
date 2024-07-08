using HotelBookingWeb.Models;
using HotelBookingWeb.Models.ViewModel;
using RestSharp;

namespace HotelBookingWeb.Data.IRepository
{
    public interface IMomoSetting
    {
        public string ComputeHmacSha256(string message, string secretKey);
        dynamic CreatePaymentAsync(ShoppingCartVM model);
    }
}
