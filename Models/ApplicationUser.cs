using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDay { get; set; }
        [Display(Name = "Tên đường")]
        public string? Street { get; set; }
        [Display(Name = "Phường")]
        public string? Ward { get; set; }
        [Display(Name = "Quận")]
        public string? District { get; set; }
        [Display(Name = "Thành phố")]
        public string? City { get; set; }

    }
}
