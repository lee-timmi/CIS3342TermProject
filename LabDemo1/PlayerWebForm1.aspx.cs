using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LabDemo1
{
    public partial class PlayerWebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string name = Request["txtName"];
            string goBirds = Request["chkGoBirds"];
            string player = Request["ddlPlayer"];

            lblDisplay.Text = "Thank You For Your Submission!" +
                              "| Name: " + name +
                              "| Go Birds: " + goBirds +
                              "| Player: " + player;
        }
    }
}