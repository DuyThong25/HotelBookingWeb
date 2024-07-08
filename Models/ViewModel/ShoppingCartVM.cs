namespace HotelBookingWeb.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> RoomCartSelectedList { get; set; }
    }
}
