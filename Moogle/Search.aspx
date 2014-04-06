<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Search.aspx.cs" Inherits="Moogle.Search" EnableViewState="true" %>

<!DOCTYPE html>

<html>
<head>
    <title>Soogle</title>
    <link href="main.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function OnCheckBoxOfTreeview(event) {
            var obj = window.event.srcElement;
            var treeNodeFound = false;
            var checkedState;
            if (obj.tagName == "INPUT" && obj.type == "checkbox") {
                var treeNode = obj;
                checkedState = treeNode.checked;
                do {
                    obj = obj.parentElement;
                }

                while (obj.tagName != "TABLE")
                var parentTreeLevel = obj.rows[0].cells.length;
                var parentTreeNode = obj.rows[0].cells[0];
                var tables = obj.parentElement.getElementsByTagName("TABLE");
                var numTables = tables.length
                if (numTables >= 1) {
                    for (i = 0; i < numTables; i++) {
                        if (tables[i] == obj) {
                            treeNodeFound = true;
                            i++;
                            if (i == numTables) {
                                return;
                            }
                        }
                        if (treeNodeFound == true) {
                            var childTreeLevel = tables[i].rows[0].cells.length;
                            if (childTreeLevel > parentTreeLevel) {
                                var cell = tables[i].rows[0].cells[childTreeLevel - 1];
                                var inputs = cell.getElementsByTagName("INPUT");
                                if (inputs.length > 0)
                                    inputs[0].checked = checkedState;
                            }
                            else {
                                return;
                            }
                        }
                    }
                }
            }
        }

        function GetSession(url) {
            document.getElementById("<%=hnkClickLink.ClientID %>").value = url;
            document.getElementById("<%=btnSetSessionValue.ClientID%>").click();
            //document.getElementById("<%= Show.ClientID %>").setAttribute("src", url);
        }

    </script>


    <style type="text/css">
        .auto-style1 {
            height: 89px;
        }

        .auto-style2 {
            width: 90%;
            height: 89px;
        }
    </style>


</head>
<body>
    <form id="Form1" method="post" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>



                <div style="vertical-align: central; align-self: center;">
                    <table style="width: 100%;" align="center">
                        <tr>
                            <td style="text-align: left;" class="auto-style1">
                                <img src="Images/soogle_Small.jpg" alt="Soogle" /></td>
                            <td style="text-align: left;" class="auto-style2">&nbsp;
                                <asp:TextBox ID="TextBoxQuery" runat="server" Width="312px" Text="<%# Query %>"></asp:TextBox>&nbsp;
						<asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click"></asp:Button>

                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: left;">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: top;">
                                <asp:Panel ID="PnlFolderStructure" runat="server" ScrollBars="Auto" CssClass="Panel">
                                    <asp:TreeView ID="tvFolderStructure" runat="server" CssClass="Treeview" ExpandDepth="1" ShowCheckBoxes="All">
                                    </asp:TreeView>
                                </asp:Panel>

                            </td>
                            <td style="text-align: left;">
                                <table style="width: 100%;" align="left">
                                    <tr>
                                        <td style="text-align: center;">

                                            <table id="Table1" align="center" width="100%">


                                                <tr>
                                                    <td style="text-align: center;" colspan="2">

                                                        <table align="center">
                                                            <tr>
                                                                <td>
                                                                    <div class="hdtb_mitem" id="All" runat="server"><a class="q qs" href="#" onclick="GetSession('SearchResultDetails.aspx?Page=All&q=<%= TextBoxQuery.Text %>');">All</a></div>
                                                                </td>
                                                                <td>
                                                                    <div class="hdtb_mitem" id="Doc" runat="server"><a class="q qs" href="#" onclick="GetSession('SearchResultDetails.aspx?Page=A&q=<%= TextBoxQuery.Text %>');">Documents</a></div>
                                                                </td>
                                                                <td>
                                                                    <div class="hdtb_mitem" id="Code" runat="server"><a class="q qs" href="#" onclick="GetSession('SearchResultDetails.aspx?Page=B&q=<%= TextBoxQuery.Text %>');">Code and Text</a></div>
                                                                </td>
                                                                <td>
                                                                    <div class="hdtb_mitem" id="Other" runat="server"><a class="q qs" href="#" onclick="GetSession('SearchResultDetails.aspx?Page=C&q=<%= TextBoxQuery.Text %>');">Other</a></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>

                                                </tr>

                                                <tr>
                                                    <td style="text-align: left;" colspan="2">
                                                        <asp:HiddenField ID="hnkClickLink" runat="server" />
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <iframe id="Show" name="ifrmShow" runat="server" width="100%" height="900px" style="border: none; overflow: hidden;"></iframe>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </td>

                                                </tr>
                                            </table>

                                            <div class="footer" >See <a href="#">Sagitec</a> Tutorials</div>

                                        </td>
                                        <td style="text-align: center; vertical-align: top;">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="display: none;">
                                <asp:Button ID="btnSetSessionValue" runat="server" Text="Button" OnClick="btnSetSessionValue_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
