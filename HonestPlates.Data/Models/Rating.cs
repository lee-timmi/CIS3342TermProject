using System;
using System.Data;

namespace RestaurantReview
{
    // A restaurant's rating summary, built from the averages GetRestaurantRatings or SearchRestaurants return.
    // Overall = average of Food, Service and Atmosphere; Price is a cost scale (1-4), shown as $ signs.
    public class Rating
    {
        public int ReviewCount { get; set; }
        public double Overall { get; set; }
        public double Food { get; set; }
        public double Service { get; set; }
        public double Atmosphere { get; set; }
        public double Price { get; set; }
        public bool HasAtmosphere { get; set; }     // older reviews have no Atmosphere rating

        // Reads the averages from one result row (they are NULL when there are no reviews)
        public Rating(DataRow row)
        {
            this.ReviewCount = Convert.ToInt32(row["ReviewCount"]);
            if (this.ReviewCount == 0)
            {
                return;
            }

            this.Overall = Convert.ToDouble(row["AvgOverall"]);
            this.Food = Convert.ToDouble(row["AvgFood"]);
            this.Service = Convert.ToDouble(row["AvgService"]);
            this.Price = Convert.ToDouble(row["AvgPrice"]);

            if (row.Table.Columns.Contains("AvgAtmosphere") && row["AvgAtmosphere"] != DBNull.Value)
            {
                this.Atmosphere = Convert.ToDouble(row["AvgAtmosphere"]);
                this.HasAtmosphere = true;
            }
        }

        public bool HasReviews()
        {
            return this.ReviewCount > 0;
        }

        // One decimal place (4.25 -> "4.3"), or a dash when there's nothing to average
        public string Format(double average, bool hasValue)
        {
            if (!hasValue)
            {
                return "&ndash;";
            }
            return average.ToString("0.0");
        }

        // The average price as $ signs, e.g. 2.6 rounds to 3 -> "$$$"
        public string PriceSigns()
        {
            if (!HasReviews())
            {
                return "&ndash;";
            }
            return Dollars((int)Math.Round(this.Price));
        }

        // The one-line summary on the search result cards
        public string SummaryText()
        {
            if (!HasReviews())
            {
                return "No reviews yet";
            }
            return "&#9733; " + this.Overall.ToString("0.0") + " overall &middot; Food " + this.Food.ToString("0.0")
                + " &middot; Service " + this.Service.ToString("0.0") + " &middot; " + PriceSigns();
        }

        // One review's price rating as $ signs (1 -> "$", 4 -> "$$$$")
        public static string Dollars(int priceRating)
        {
            return new string('$', priceRating);
        }
    }
}
