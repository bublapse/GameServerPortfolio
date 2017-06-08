using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LoginWithSuperBitID : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Response.Write(Player.Manager.LoginWithSuperBitID(new RequestTable.Instance(false)));
    }
}