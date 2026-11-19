namespace HonestPlates.Data.Models
{
    public class SavedSearch
    {
        public List<String> Cuisines { get; set; } = new List<string>();

        public string Location { get; set; } = "";
        public DateTime SavedDate { get; set; }

        public string Description()
        {
            string cuisineText = "Any cuisine";
            if (Cuisines.Count > 0)
            {
                cuisineText = string.Join(", ", Cuisines);
            }
            string place = "";
            string[] parts = Location.Split('|');
            if (parts.Length == 2)
            {
                if (parts[0] != "")
                {
                    place = parts[0] + ", " + parts[1];
                }
                else
                {
                    place = parts[1];
                }
            }

            if (place == "")
            {
                return cuisineText;
            }
            return cuisineText + " in " + place;
        }

        public bool SameAs(SavedSearch other)
        {
            return string.Join(",", Cuisines) == string.Join(",", other.Cuisines) && Location == other.Location;
        }

        public string QueryString()
        {
            string query = "";
            foreach (string cuisine in Cuisines)
            {
                query += "&cuisines=" + Uri.EscapeDataString(cuisine);
            }
            query += "&location=" + Uri.EscapeDataString(Location);
            return "?" + query.Substring(1);
        }
    }
}
