using System;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    // Fills and selects the lists the forms share (ManageRestaurant, MakeReservation, ManageReservations,
    // ReviewForm), so each list is built the same way everywhere.
    public class ListHelper
    {
        // Adds every half hour of the day: text "7:30 PM", value "19:30"
        // (the same format GetRestaurantHours returns, so saved times can be selected)
        public static void FillTimes(ListControl list)
        {
            for (int i = 0; i < 48; i++)
            {
                DateTime time = DateTime.Today.AddMinutes(i * 30);
                list.Items.Add(new ListItem(time.ToString("h:mm tt"), time.ToString("HH:mm")));
            }
        }

        // Adds party sizes 1 to 20: "1 person", "2 people", ...
        public static void FillPartySizes(ListControl list)
        {
            for (int size = 1; size <= 20; size++)
            {
                list.Items.Add(new ListItem(size + (size == 1 ? " person" : " people"), size.ToString()));
            }
        }

        // Selects a value only if it's in the list (avoids a crash on old or unexpected data)
        public static void SelectValue(ListControl list, string value)
        {
            if (list.Items.FindByValue(value) != null)
            {
                list.SelectedValue = value;
            }
        }
    }
}
