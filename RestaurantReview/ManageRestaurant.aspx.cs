using System;
using System.Data;
using System.Web.UI.WebControls;

namespace RestaurantReview
{
    public partial class ManageRestaurant : System.Web.UI.Page
    {
        // Index = DayOfWeek number (0 = Sunday), same as the RestaurantHours table
        private string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        private string[] states = { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "DC", "FL", "GA", "HI", "ID", "IL",
            "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM",
            "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };

        protected void Page_Load(object sender, EventArgs e)
        {
            // Only logged-in Representatives can use this page
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (Session["UserType"].ToString() != "Representative")
            {
                Response.Redirect("Dashboard.aspx");
            }

            // (No lblGreeting line: the master page shows "Hi, {FirstName}" in the nav now)

            if (!IsPostBack)
            {
                try
                {
                    FillDropDowns();
                    LoadRestaurant();
                }
                catch (Exception)
                {
                    ShowMessage("Sorry, we couldn't load your restaurant. Please try again later.", false);
                    btnSave.Enabled = false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Check the input before touching the database
            string errors = ValidateForm();
            if (errors != "")
            {
                ShowMessage("Please fix the following:<br />" + errors, false);
                return;
            }

            try
            {
                // 2. Look up this rep's restaurant again (never trust an ID from the page)
                int userID = Convert.ToInt32(Session["UserID"]);
                Restaurant restaurant = new Restaurant();
                bool alreadyExists = restaurant.LoadByRepresentative(userID);

                // 3. Copy the form into the object
                restaurant.RepresentativeUserID = userID;
                restaurant.Name = txtName.Text.Trim();
                restaurant.Street = txtStreet.Text.Trim();
                restaurant.City = txtCity.Text.Trim();
                restaurant.State = ddlState.SelectedValue;
                restaurant.Zip = txtZip.Text.Trim();
                restaurant.Cuisine = ddlCuisine.SelectedValue;
                restaurant.Description = txtDescription.Text.Trim();
                restaurant.Website = txtWebsite.Text.Trim();
                restaurant.SocialMedia = txtSocialMedia.Text.Trim();
                restaurant.Phone = txtPhone.Text.Trim();
                restaurant.Owner = txtOwner.Text.Trim();

                // 4. Save the profile (update or add)
                bool saved;
                if (alreadyExists)
                {
                    saved = restaurant.UpdateRestaurant();
                }
                else
                {
                    saved = restaurant.AddRestaurant();
                }

                if (!saved)
                {
                    ShowMessage("Sorry, your restaurant couldn't be saved. Please try again.", false);
                    return;
                }

                // 5. Save all 7 days of hours
                if (!SaveAllHours(restaurant))
                {
                    ShowMessage("Your restaurant was saved, but some of the hours weren't. Please click Save again.", false);
                    return;
                }

                ShowEditMode(restaurant);
                if (alreadyExists)
                {
                    ShowMessage("Your changes were saved.", true);
                }
                else
                {
                    ShowMessage("Your restaurant was created! You can now add photos below.", true);
                }
            }
            catch (Exception)
            {
                ShowMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            // 1. Check the file before touching the database
            if (!fuPhoto.HasFile)
            {
                ShowPhotoMessage("Choose a photo to upload.", false);
                return;
            }
            int imageSize = fuPhoto.PostedFile.ContentLength;
            if (imageSize > RestaurantImage.MaxImageBytes)
            {
                ShowPhotoMessage("That photo is larger than 1 MB. Please choose a smaller one.", false);
                return;
            }

            // File extension check (Validation class, the same way as the professor's ProductImageLoader example)
            if (!Validation.IsImageFile(fuPhoto.PostedFile.FileName))
            {
                ShowPhotoMessage("Only JPG, PNG, or GIF photos can be uploaded.", false);
                return;
            }

            // Same rule as the profile form (the message uses &lt; &gt; because the Label shows HTML)
            if (Validation.HasHtml(txtCaption.Text))
            {
                ShowPhotoMessage("The caption can't contain &lt; or &gt; (HTML isn't allowed).", false);
                return;
            }

            try
            {
                // 2. Always upload to this rep's own restaurant
                Restaurant restaurant = new Restaurant();
                if (!restaurant.LoadByRepresentative(Convert.ToInt32(Session["UserID"])))
                {
                    ShowPhotoMessage("Create your restaurant before adding photos.", false);
                    return;
                }

                // Read the uploaded file into a byte[] (same as ProductImageLoader)
                byte[] imageData = new byte[imageSize];
                fuPhoto.PostedFile.InputStream.Read(imageData, 0, imageSize);

                RestaurantImage image = new RestaurantImage();
                image.RestaurantID = restaurant.RestaurantID;
                image.ImageData = imageData;
                image.ContentType = fuPhoto.PostedFile.ContentType;
                image.Caption = txtCaption.Text.Trim();
                image.Category = ddlPhotoCategory.SelectedValue;

                if (image.AddImage())
                {
                    ShowPhotoMessage("Your " + image.Category.ToLower() + " photo was uploaded.", true);
                    txtCaption.Text = "";
                    LoadPhotos(restaurant.RestaurantID);
                }
                else
                {
                    ShowPhotoMessage("Sorry, the photo couldn't be saved. Please try again.", false);
                }
            }
            catch (Exception)
            {
                ShowPhotoMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        // Runs when a Delete button inside rptPhotos is clicked (same idea as Repeater Example 3)
        protected void rptPhotos_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeletePhoto")
            {
                return;
            }

            try
            {
                Restaurant restaurant = new Restaurant();
                if (!restaurant.LoadByRepresentative(Convert.ToInt32(Session["UserID"])))
                {
                    return;
                }

                // Find the hidden ImageID label in the card whose button was clicked
                int rowIndex = e.Item.ItemIndex;
                Label lblImageID = (Label)rptPhotos.Items[rowIndex].FindControl("lblImageID");
                int imageID = Convert.ToInt32(lblImageID.Text);

                // DeleteRestaurantImage also checks the RestaurantID, so a rep can only delete their own photos
                RestaurantImage image = new RestaurantImage();

                if (image.DeleteImage(imageID, restaurant.RestaurantID))
                {
                    ShowPhotoMessage("The photo was deleted.", true);
                }
                else
                {
                    ShowPhotoMessage("Sorry, that photo couldn't be deleted.", false);
                }
                LoadPhotos(restaurant.RestaurantID);
            }
            catch (Exception)
            {
                ShowPhotoMessage("Sorry, we couldn't reach the database. Please try again later.", false);
            }
        }

        // Shows this restaurant's photos in the Repeater (or the "No photos yet" message)
        private void LoadPhotos(int restaurantID)
        {
            RestaurantImage image = new RestaurantImage();
            DataTable photos = image.GetImagesForRestaurant(restaurantID);
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
            lblNoPhotos.Visible = (photos.Rows.Count == 0);
        }

        private void ShowPhotoMessage(string message, bool success)
        {
            lblPhotoMessage.Text = message;
            lblPhotoMessage.CssClass = success ? "d-block alert alert-success" : "d-block alert alert-danger";
            lblPhotoMessage.Visible = true;
        }

        // Returns the problems with the form, one per line ("" = everything is OK)
        private string ValidateForm()
        {
            string errors = "";

            if (txtName.Text.Trim() == "") errors += "Enter the restaurant name.<br />";
            if (ddlCuisine.SelectedValue == "") errors += "Choose a cuisine.<br />";
            if (txtStreet.Text.Trim() == "") errors += "Enter the street address.<br />";
            if (txtCity.Text.Trim() == "") errors += "Enter the city.<br />";
            if (ddlState.SelectedValue == "") errors += "Choose a state.<br />";

            string zip = txtZip.Text.Trim();
            if (zip.Length != 5 || Validation.CountDigits(zip) != 5) errors += "ZIP must be 5 digits.<br />";

            string phone = txtPhone.Text.Trim();
            if (phone != "" && Validation.CountDigits(phone) != 10) errors += "Phone must have 10 digits, e.g. 215-555-0123.<br />";

            if (txtDescription.Text.Trim().Length > 2000) errors += "Description can be at most 2,000 characters.<br />";

            string website = txtWebsite.Text.Trim();
            if (website != "" && !Validation.IsLink(website)) errors += "Website must be a full link starting with http:// or https://.<br />";

            string socialMedia = txtSocialMedia.Text.Trim();
            if (socialMedia != "" && !Validation.IsLink(socialMedia)) errors += "Social Media must be a full link starting with http:// or https://.<br />";

            // No < or > anywhere (Home, Search, and the profile page show this text without encoding it).
            // The message uses &lt; &gt; because lblMessage shows its text as HTML.
            TextBox[] textBoxes = { txtName, txtDescription, txtOwner, txtStreet, txtCity, txtZip, txtPhone, txtWebsite, txtSocialMedia };
            foreach (TextBox box in textBoxes)
            {
                if (Validation.HasHtml(box.Text))
                {
                    errors += "Text can't contain &lt; or &gt; (HTML isn't allowed).<br />";
                    break;
                }
            }

            // Hours: an open day needs different open and close times
            for (int day = 0; day < 7; day++)
            {
                RepeaterItem item = rptHours.Items[day];
                CheckBox chkClosed = (CheckBox)item.FindControl("chkClosed");
                DropDownList ddlOpen = (DropDownList)item.FindControl("ddlOpen");
                DropDownList ddlClose = (DropDownList)item.FindControl("ddlClose");

                if (!chkClosed.Checked && ddlOpen.SelectedValue == ddlClose.SelectedValue)
                {
                    errors += dayNames[day] + ": opening and closing time can't be the same (or check Closed).<br />";
                }
            }

            return errors;
        }

        // Fills the State, Cuisine, and hours DropDownLists (first page load only)
        private void FillDropDowns()
        {
            ddlState.Items.Add(new ListItem("-- State --", ""));
            foreach (string state in states)
            {
                ddlState.Items.Add(state);
            }

            Restaurant restaurant = new Restaurant();
            ddlCuisine.DataSource = restaurant.GetCuisineList();
            ddlCuisine.DataTextField = "RestaurantCuisine";
            ddlCuisine.DataValueField = "RestaurantCuisine";
            ddlCuisine.DataBind();
            ddlCuisine.Items.Insert(0, new ListItem("-- Choose a cuisine --", ""));

            // One row per day, then fill each row's time lists (default 11:00 AM - 10:00 PM)
            rptHours.DataSource = dayNames;
            rptHours.DataBind();
            foreach (RepeaterItem item in rptHours.Items)
            {
                DropDownList ddlOpen = (DropDownList)item.FindControl("ddlOpen");
                DropDownList ddlClose = (DropDownList)item.FindControl("ddlClose");
                ListHelper.FillTimes(ddlOpen);
                ListHelper.FillTimes(ddlClose);
                ddlOpen.SelectedValue = "11:00";
                ddlClose.SelectedValue = "22:00";
            }
        }

        // Shows the rep's restaurant if they have one, otherwise the empty "Add" form
        private void LoadRestaurant()
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            Restaurant restaurant = new Restaurant();

            if (!restaurant.LoadByRepresentative(userID))
            {
                return;   // no restaurant yet: the page already shows "Add Your Restaurant"
            }

            txtName.Text = restaurant.Name;
            ListHelper.SelectValue(ddlCuisine, restaurant.Cuisine);
            txtDescription.Text = restaurant.Description;
            txtOwner.Text = restaurant.Owner;
            txtStreet.Text = restaurant.Street;
            txtCity.Text = restaurant.City;
            ListHelper.SelectValue(ddlState, restaurant.State);
            txtZip.Text = restaurant.Zip;
            txtPhone.Text = restaurant.Phone;
            txtWebsite.Text = restaurant.Website;
            txtSocialMedia.Text = restaurant.SocialMedia;

            DataTable hours = restaurant.GetHours();
            foreach (DataRow row in hours.Rows)
            {
                int day = Convert.ToInt32(row["DayOfWeek"]);
                RepeaterItem item = rptHours.Items[day];
                CheckBox chkClosed = (CheckBox)item.FindControl("chkClosed");
                DropDownList ddlOpen = (DropDownList)item.FindControl("ddlOpen");
                DropDownList ddlClose = (DropDownList)item.FindControl("ddlClose");

                chkClosed.Checked = Convert.ToBoolean(row["IsClosed"]);
                ListHelper.SelectValue(ddlOpen, row["OpenTime"].ToString());
                ListHelper.SelectValue(ddlClose, row["CloseTime"].ToString());
            }

            ShowEditMode(restaurant);
        }

        // Saves every day in the hours list. Returns false if any day failed.
        private bool SaveAllHours(Restaurant restaurant)
        {
            bool allSaved = true;
            for (int day = 0; day < 7; day++)
            {
                RepeaterItem item = rptHours.Items[day];
                CheckBox chkClosed = (CheckBox)item.FindControl("chkClosed");
                DropDownList ddlOpen = (DropDownList)item.FindControl("ddlOpen");
                DropDownList ddlClose = (DropDownList)item.FindControl("ddlClose");

                if (!restaurant.SaveHours(day, chkClosed.Checked, ddlOpen.SelectedValue, ddlClose.SelectedValue))
                {
                    allSaved = false;
                }
            }
            return allSaved;
        }

        // Switches the page from "Add" to "Edit" once the rep has a restaurant
        private void ShowEditMode(Restaurant restaurant)
        {
            lblHeading.Text = "Edit Your Restaurant";
            lblInstructions.Text = "Update any part of " + Server.HtmlEncode(restaurant.Name) + "'s profile, then click Save Changes.";
            btnSave.Text = "Save Changes";
            lnkPublicPage.NavigateUrl = "RestaurantProfile.aspx?id=" + restaurant.RestaurantID;
            lnkPublicPage.Visible = true;
            pnlPhotos.Visible = true;
            LoadPhotos(restaurant.RestaurantID);
        }

        // Shows lblMessage as a green (success) or red (error) Bootstrap alert
        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = success ? "d-block alert alert-success" : "d-block alert alert-danger";
            lblMessage.Visible = true;
        }
    }
}
