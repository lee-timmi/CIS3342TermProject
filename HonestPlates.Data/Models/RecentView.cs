namespace HonestPlates.Data.Models
{
    public class RecentView
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; } = "";
        public DateTime ViewedDate { get; set; }
    }
}