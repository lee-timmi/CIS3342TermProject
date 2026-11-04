using System;

namespace RestaurantReview
{
    // Input checks shared by the forms (ManageRestaurant, MakeReservation, ManageReservations,
    // ReviewForm, RespondToReviews), so each rule is written once.
    public class Validation
    {
        // Counts the digits (0-9) in a piece of text, e.g. "215-555-0123" -> 10
        public static int CountDigits(string text)
        {
            int count = 0;
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    count++;
                }
            }
            return count;
        }

        // A link must start with http:// or https:// (and have something after it)
        public static bool IsLink(string text)
        {
            string lower = text.ToLower();
            return (lower.StartsWith("http://") && lower.Length > 7)
                || (lower.StartsWith("https://") && lower.Length > 8);
        }

        // An ID from a query string: the number if it's a whole number above 0, otherwise 0
        public static int ParseID(string text)
        {
            int id;
            if (int.TryParse(text, out id) && id > 0)
            {
                return id;
            }
            return 0;
        }

        // A simple email check: something before the @, a "." after it, not ending in "." and no spaces
        public static bool IsEmail(string text)
        {
            int at = text.IndexOf("@");
            return at >= 1 && text.LastIndexOf(".") > at && !text.EndsWith(".") && !text.Contains(" ");
        }

        // True if an uploaded file's name ends in .jpg, .jpeg, .png or .gif (the same check as the
        // professor's ProductImageLoader; a name with no "." gets no extension, so it fails instead of crashing)
        public static bool IsImageFile(string fileName)
        {
            string fileExtension = "";
            if (fileName.LastIndexOf(".") >= 0)
            {
                fileExtension = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
            }
            return fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif";
        }

        // True if the text has < or >. HTML isn't allowed in anything users type.
        public static bool HasHtml(string text)
        {
            return text.Contains("<") || text.Contains(">");
        }
    }
}
