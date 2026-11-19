using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace HonestPlates.Data.Models
{
    // One user's saved searches and recently viewed restaurants.
    // The whole object is serialized to JSON text and stored in TP_UserHistory (one row per user),
    // then deserialized back into these lists on their next visit.

    public class UserHistory
    {
        public const int MaxSavedSearches = 10;
        public const int MaxRecentlyViewed = 5;

        public List<SavedSearch> SavedSearches { get; set; } = new List<SavedSearch>();
        public List<RecentView> RecentlyViewed { get; set; } = new List<RecentView>();

        // Loads (deserializes) one user's history. A user with no row, or unreadable JSON, gets empty lists.
        public static UserHistory Load(int userID)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_GetUserHistory";
            cmd.Parameters.AddWithValue("@UserID", userID);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new UserHistory();
            }

            string json = ds.Tables[0].Rows[0]["HistoryJson"].ToString();
            try
            {
                UserHistory history = JsonSerializer.Deserialize<UserHistory>(json);
                if (history == null)
                {
                    return new UserHistory();
                }
                if (history.SavedSearches == null)
                {
                    history.SavedSearches = new List<SavedSearch>();
                }
                if (history.RecentlyViewed == null)
                {
                    history.RecentlyViewed = new List<RecentView>();
                }
                return history;
            }
            catch (JsonException)
            {
                // Broken JSON never crashes the page; the user just starts over with empty lists
                return new UserHistory();
            }
        }

        // Saves (serializes) this whole object as JSON text (TP_SaveUserHistory).
        public void Save(int userID)
        {
            string json = JsonSerializer.Serialize(this);

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_SaveUserHistory";
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@HistoryJson", json);

            db.DoUpdateUsingCmdObj(cmd);
        }

        // Adds a search to the top of the list. Returns "" if it was added, otherwise the reason it wasn't.
        public string AddSearch(List<string> cuisines, string location)
        {
            if (cuisines == null)
            {
                cuisines = new List<string>();
            }
            if (location == null)
            {
                location = "";
            }
            if (cuisines.Count == 0 && location == "")
            {
                return "Pick at least one cuisine or a location before saving a search.";
            }

            SavedSearch search = new SavedSearch();
            search.Cuisines = cuisines;
            search.Location = location;
            search.SavedDate = DateTime.Now;

            foreach (SavedSearch saved in SavedSearches)
            {
                if (saved.SameAs(search))
                {
                    return "You already saved this search.";
                }
            }

            if (SavedSearches.Count >= MaxSavedSearches)
            {
                return "You can save up to " + MaxSavedSearches + " searches. Remove one first.";
            }

            SavedSearches.Insert(0, search);
            return "";
        }

        // Removes one saved search by its position in the list. False if the position doesn't exist.
        public bool RemoveSearch(int index)
        {
            if (index < 0 || index >= SavedSearches.Count)
            {
                return false;
            }
            SavedSearches.RemoveAt(index);
            return true;
        }

        // Records a profile visit: moves the restaurant to the top and keeps only the newest few.
        public void AddView(int restaurantID, string name)
        {
            // Take out any older visit to the same restaurant (backwards, so removing doesn't skip one)
            for (int i = RecentlyViewed.Count - 1; i >= 0; i--)
            {
                if (RecentlyViewed[i].RestaurantID == restaurantID)
                {
                    RecentlyViewed.RemoveAt(i);
                }
            }

            RecentView view = new RecentView();
            view.RestaurantID = restaurantID;
            view.Name = name;
            view.ViewedDate = DateTime.Now;
            RecentlyViewed.Insert(0, view);

            while (RecentlyViewed.Count > MaxRecentlyViewed)
            {
                RecentlyViewed.RemoveAt(RecentlyViewed.Count - 1);
            }
        }

        // Records one profile visit for a logged-in user: load the history, add the visit, save it back.
        public static void RecordView(int userID, int restaurantID, string name)
        {
            UserHistory history = Load(userID);
            history.AddView(restaurantID, name);
            history.Save(userID);
        }
    }
}