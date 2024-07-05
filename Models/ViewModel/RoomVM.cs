using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBookingWeb.Models.ViewModel
{
    public class RoomVM
    {
        public Room Room { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryRoomList { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem>? StatusRoomList { get; set; }
    }
}
