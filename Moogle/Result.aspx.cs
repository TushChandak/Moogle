using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Moogle
{
    public partial class Result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strPath = Convert.ToString(Request.QueryString["rs"]);
            string title = Convert.ToString(Request.QueryString["title"]);
            PageTitle.Text = title;
            urIframe.Attributes.Add("src", "ResultOutput.aspx?rs=" + strPath);
        }
    }
}