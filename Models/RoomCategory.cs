using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class RoomCategory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Loại phòng")]
        public string Name { get; set; }

        [Display(Name ="Sức chứa")]
        public int? Capacity { get; set; }

    }
}
