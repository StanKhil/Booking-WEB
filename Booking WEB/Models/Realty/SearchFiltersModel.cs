namespace Booking_WEB.Models.Realty
{
    public class SearchFiltersModel
    {
        public decimal Price { get; set; }
        public List<string> Checkboxes { get; set; } = [];
        public int Rating { get; set; }
    }
}
