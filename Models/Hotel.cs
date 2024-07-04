namespace HotelBookingWeb.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Street { get; set; }
        public string Distinct { get; set; }

        public string Ward { get; set; }
        public string City { get; set; }
    }
}
