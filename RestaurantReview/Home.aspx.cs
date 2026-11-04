using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class Home : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCuisines();
                LoadTopRated();
                SetJoinLinks();
            }
        }

        // The "Top rated in Philadelphia" cards (same cards and columns as the search results)
        private void LoadTopRated()
        {
            try
            {
                Restaurant restaurant = new Restaurant();
                DataTable topRated = restaurant.GetTopRated("Philadelphia", "PA", 6);
                if (topRated.Rows.Count > 0)
                {
                    AddCardColumns(topRated);
                    rptTopRated.DataSource = topRated;
                    rptTopRated.DataBind();
                    pnlTopRated.Visible = true;
                }
            }
            catch (Exception)
            {
                // The cards are extra; the search still works without them
                pnlTopRated.Visible = false;
            }
        }

        // "How it works": a logged-in user already has an account, so those buttons go to the Dashboard
        private void SetJoinLinks()
        {
            if (Session["UserID"] != null)
            {
                lnkJoinReviewer.NavigateUrl = "Dashboard.aspx";
                lnkJoinReviewer.Text = "Go to your dashboard";
                lnkJoinRep.NavigateUrl = "Dashboard.aspx";
                lnkJoinRep.Text = "Go to your dashboard";
            }
        }

        private void LoadCuisines()
        {
            try
            {
                Restaurant restaurant = new Restaurant();
                cblCuisine.DataSource = restaurant.GetCuisineList();
                cblCuisine.DataTextField = "RestaurantCuisine";
                cblCuisine.DataValueField = "RestaurantCuisine";
                cblCuisine.DataBind();
            }
            catch (Exception)
            {
                lblError.Text = "Unable to load cuisine options. Please try again later.";
                lblError.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            // Collect checked cuisines
            List<string> selectedCuisines = new List<string>();
            foreach (ListItem item in cblCuisine.Items)
            {
                if (item.Selected)
                    selectedCuisines.Add(item.Value);
            }

            string location = txtLocation.Text.Trim();

            // Require at least one filter
            if (selectedCuisines.Count == 0 && location.Length == 0)
            {
                lblError.Text = "Please enter a location or select at least one cuisine.";
                lblError.Visible = true;
                return;
            }
            if (Validation.HasHtml(location))
            {
                lblError.Text = "The location can't contain &lt; or &gt; (HTML isn't allowed).";
                lblError.Visible = true;
                return;
            }

            string cuisineList = string.Join(",", selectedCuisines);

            try
            {
                Restaurant restaurant = new Restaurant();
                DataTable results = restaurant.Search(cuisineList, location);
                int recordCount = results.Rows.Count;

                if (recordCount == 0)
                {
                    lblError.Text = "No restaurants found. Try different filters.";
                    lblError.Visible = true;
                    pnlResults.Visible = false;
                    return;
                }

                lblResultCount.Text = recordCount + " restaurant" + (recordCount == 1 ? "" : "s") + " found";
                AddCardColumns(results);
                rptResults.DataSource = results;
                rptResults.DataBind();
                pnlResults.Visible = true;
                pnlTopRated.Visible = false;     // the results take its place; Clear brings it back
            }
            catch (Exception)
            {
                lblError.Text = "An error occurred while searching. Please try again later.";
                lblError.Visible = true;
                pnlResults.Visible = false;
            }
        }

        // Adds two columns the result cards show (same idea as Prof. Pascucci's ProductImageViewer,
        // which adds an "imgFile" column to the DataSet and fills it with an ImageGrab.aspx link):
        // - imgFile: the uploaded Logo, or the old image link for restaurants without one
        // - RatingText: "★ 4.2 overall · Food 4.5 · Service 4.0 · $$", or "No reviews yet" (from the Rating class)
        private void AddCardColumns(DataTable results)
        {
            results.Columns.Add("imgFile");
            results.Columns.Add("RatingText");

            foreach (DataRow row in results.Rows)
            {
                if (row["LogoImageID"] != DBNull.Value)
                {
                    row["imgFile"] = "ImageGrab.aspx?ID=" + row["LogoImageID"];
                }
                else
                {
                    row["imgFile"] = row["RestaurantImage"].ToString();
                }

                Rating rating = new Rating(row);
                row["RatingText"] = rating.SummaryText();
            }
        }

        // Clears the search: unchecks every cuisine, empties the location and hides the results
        protected void btnClear_Click(object sender, EventArgs e)
        {
            cblCuisine.ClearSelection();
            txtLocation.Text = "";
            lblError.Visible = false;
            rptResults.DataSource = null;
            rptResults.DataBind();
            pnlResults.Visible = false;
            LoadTopRated();
        }
    }
}
