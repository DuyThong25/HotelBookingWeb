namespace HotelBookingWeb.Models.ViewModel
{
    public class DetailHotelVM
    {
        public IEnumerable<Hotel>? ListHotel { get; set; }
        public Hotel? Hotel { get; set; }
        public string? CheckInDate { get; set; }
        public string? CheckOutDate { get; set; }
    }
}
