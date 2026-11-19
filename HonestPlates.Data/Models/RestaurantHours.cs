namespace HonestPlates.Data.Models
{
    public class RestaurantHours
    {
        public int RepresentativeUserID { get; set; }
        public bool[] Closed { get; set; } = new bool[7];
        public string[] OpenTime { get; set; } = new string[7];
        public string[] CloseTime { get; set; } = new string[7];
    }
}