using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Moogle
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Form.DefaultButton = BtnSearch.UniqueID;
            if (!IsPostBack)
            {
                Session["SearchKeyword"] = null;
                TxtSearchkeyword.Focus();
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (TxtSearchkeyword.Text != "")
            {
                Session["SearchKeyword"] = TxtSearchkeyword.Text;
                //this.Response.Redirect("MainSearch.aspx?q=" + this.TxtSearchkeyword.Text + "&Page=All");
                this.Response.Redirect("MainSearch.aspx?Page=All");
            }
        }
    }
}