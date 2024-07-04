using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class RoomCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Không được bỏ trống")]
        [Display(Name ="Loại phòng")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Display(Name ="Sức chứa")]
        public int Capacity { get; set; }

    }
}
