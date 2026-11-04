using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LabDemo2
{
    public partial class PhillyForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            String name = txtName.Text;
            String team = ddlTeam.SelectedValue;
            Boolean join = chkJoin.Checked;

            lblDisplay.Text = "Thanks For Joining:" +
                "name: " + name + "| team: " + team + "| join: " + join;
        }
    }
}