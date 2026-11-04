using System;

namespace RestaurantReview
{
    // Sends one photo to the browser:
    //   <img src="ImageGrab.aspx?ID=5" />        a restaurant photo (RestaurantImages)
    //   <img src="ImageGrab.aspx?ReviewID=12" /> a review's photo (ReviewsTable, Session 9)
    public partial class ImageGrab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            byte[] imageData = null;
            string contentType = "";

            // Unlike the professor's example, the ID goes into a stored procedure parameter
            // (not into SQL text), and a missing or non-number ID is just "not found"
            int imageID;
            int reviewID;
            try
            {
                if (int.TryParse(Request.QueryString["ID"], out imageID))
                {
                    RestaurantImage image = new RestaurantImage();
                    if (image.LoadImage(imageID))
                    {
                        imageData = image.ImageData;
                        contentType = image.ContentType;
                    }
                }
                else if (int.TryParse(Request.QueryString["ReviewID"], out reviewID))
                {
                    Review review = new Review();
                    if (review.LoadImage(reviewID))
                    {
                        imageData = review.ImageData;
                        contentType = review.ImageType;
                    }
                }
            }
            catch (Exception)
            {
                imageData = null;
            }

            // Write the image bytes (or a 404) instead of the page's HTML
            Response.Clear();
            if (imageData != null)
            {
                Response.ContentType = contentType;
                Response.OutputStream.Write(imageData, 0, imageData.Length);
            }
            else
            {
                Response.StatusCode = 404;
            }

            // Outside the try: Response.End() works by throwing a ThreadAbortException
            // (same reason the Login redirect sits after the catch in Session 5)
            Response.End();
        }
    }
}
