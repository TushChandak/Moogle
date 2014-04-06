<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainSearch.aspx.cs" Inherits="Moogle.MainSearch" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Soogle</title>
    <meta http-equiv="X-UA-Compatible" content="IE9" />
    <link href="main.css" rel="stylesheet" />

    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery-ui.min.js"></script>
    <link href="Scripts/jquery-ui.css" rel="stylesheet" />

    <script type="text/javascript">


        function OnCheckBoxOfTreeview(evt) {

            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");

            if (isChkBoxClick) {

                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;

                if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node 
                {

                    if (nxtSibling.tagName.toLowerCase() == "div") //if node has children 
                    {
                        //check or uncheck children at all levels 
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                //check or uncheck parents at all levels 
                CheckUncheckParents(src, src.checked);
            }
        }

        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }

        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;

            if (parentNodeTable) {

                var checkUncheckSwitch;
                if (check) //checkbox checked 
                {
                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                    if (isAllSiblingsChecked)
                        checkUncheckSwitch = true;
                    else
                        return; //do not need to check parent if any(one or more) child not checked 
                }
                else //checkbox unchecked 
                {
                    checkUncheckSwitch = false;
                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    //do the same recursively 
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }

        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;

            for (var i = 0; i < childCount; i++) {

                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node 
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false 
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        //utility function to get the container of an element by tag name 
        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }

        function GetSession(url, obj) {
            if (obj != "null") {
                document.getElementById("<%=hnkPageCount.ClientID %>").value = obj.text;
            }
            else {
                document.getElementById("<%=hnkPageCount.ClientID %>").value = "0";
            }

            document.getElementById("<%=hnkClickLink.ClientID %>").value = url;
            document.getElementById("<%=btnSetSessionValue.ClientID %>").click();
        }

        function OnTextChange() {
            document.getElementById("<%=ButtonSearch.ClientID %>").click();
        }

    </script>

    <%--<script type="text/javascript">

        jQuery(function ($) {
            $(document).ready(function () {
                $('#panelHandle').hover(function () {
                    $('#sidePanel').stop(true, false).animate({
                        'left': '0px'
                    }, 900);
                }, function () {
                    jQuery.noConflict();
                });

                jQuery('#sidePanel').hover(function () {
                    // Do nothing
                }, function () {

                    jQuery.noConflict();
                    jQuery('#sidePanel').animate({
                        left: '-201px'
                    }, 800);

                });

                $(window).scroll(function () {
                    //after window scroll fire it will add define pixel added to that element.
                    set = $(document).scrollTop() + "px";
                    //this is the jQuery animate function to fixed the div position after scrolling.
                    $('#floatDiv').animate({ top: set }, { duration: 1000, queue: false });
                });
            });
        });

        function pageLoad() {
            jQuery(function ($) {
                $(document).ready(function () {
                    $('#panelHandle').hover(function () {
                        $('#sidePanel').stop(true, false).animate({
                            'left': '0px'
                        }, 900);
                    }, function () {
                        jQuery.noConflict();
                    });

                    jQuery('#sidePanel').hover(function () {
                        // Do nothing
                    }, function () {

                        jQuery.noConflict();
                        jQuery('#sidePanel').animate({
                            left: '-201px'
                        }, 800);

                    });

                    $(window).scroll(function () {
                        //after window scroll fire it will add define pixel added to that element.
                        set = $(document).scrollTop() + "px";
                        //this is the jQuery animate function to fixed the div position after scrolling.
                        $('#floatDiv').animate({ top: set }, { duration: 1000, queue: false });
                    });
                });
            });

            $(function () {
                $("#TextBoxQuery").autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "SearchKeywordList.asmx/FetchSearchKeywordList",
                            data: "{ 'searchkeyword': '" + request.term + "' }",
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            dataFilter: function (data) { return data; },
                            async: true,
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        value: item
                                    }
                                }))
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                //alert(XMLHttpRequest.status);
                                alert(XMLHttpRequest.responseText);
                            }
                        });
                    },

                    select: function (e, i) {
                        document.getElementById("<%=ButtonSearch.ClientID %>").click();
                    },
                    minLength: 2
                });
            });

            }

            $(function () {
                $("#TextBoxQuery").autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "SearchKeywordList.asmx/FetchSearchKeywordList",
                            data: "{ 'searchkeyword': '" + request.term + "' }",
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            dataFilter: function (data) { return data; },
                            async: true,
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        value: item
                                    }
                                }))
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                //alert(XMLHttpRequest.status);
                                alert(XMLHttpRequest.responseText);
                            }
                        });
                    },

                    select: function (e, i) {
                        document.getElementById("<%=ButtonSearch.ClientID %>").click();
                    },
                    minLength: 2
                });
            });
    </script>--%>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600" EnablePageMethods="true"></asp:ScriptManager>

        <div class="divMain">
            

            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table style="width: 100%;" align="center">
                        <tr>
                            <td style="text-align: left; width: 15%;">
                                <a href="Home.aspx">
                                    <img src="Images/soogle_Small.jpg" alt="Soogle" />
                                </a>
                            </td>
                            <td style="text-align: left; width: 85%;">
                                <table>
                                    <tr>
                                        <td style="text-align: left;">
                                            <div class="ui-widget">
                                                <asp:TextBox ID="TextBoxQuery" ClientIDMode="Static" runat="server" Width="312px" Text="<%# Query %>" CssClass="txtBox"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td style="text-align: left;">
                                            <asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click" CssClass="btn"></asp:Button></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; width: 15%;">&nbsp;</td>
                            <td style="text-align: left; vertical-align: top; width: 85%;">
                                <table align="left">
                                    <tr>
                                        <td>
                                            <div class="hdtb_mitem" id="All" runat="server"><a href="#" onclick="GetSession('All','null');">All</a></div>
                                        </td>
                                        <td>
                                            <div class="hdtb_mitem" id="Doc" runat="server"><a href="#" onclick="GetSession('Doc','null');">Documents</a></div>
                                        </td>
                                        <td>
                                            <div class="hdtb_mitem" id="Code" runat="server"><a href="#" onclick="GetSession('Code','null');">Code and Text</a></div>
                                        </td>
                                        <td>
                                            <div class="hdtb_mitem" id="Images" runat="server"><a href="#" onclick="GetSession('Images','null');">Images</a></div>
                                        </td>
                                        <td>
                                            <div class="hdtb_mitem" id="Other" runat="server"><a href="#" onclick="GetSession('Other','null');">Other</a></div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: left;">
                                <hr />
                            </td>
                        </tr>
                        <tr>


                            <td style="text-align: left; vertical-align: top; width: 15%;">
                                <div id="sidePanel">
                                    <div id="panelContent">
                                        <asp:Panel ID="PnlFolderStructure" runat="server" ScrollBars="Auto" CssClass="Panel">
                                            <asp:TreeView ID="tvFolderStructure" runat="server" CssClass="Treeview" ExpandDepth="1" ShowCheckBoxes="All">
                                            </asp:TreeView>

                                        </asp:Panel>
                                    </div>
                                    <div id="panelHandle">
                                        <img src="Images/arrow.gif" width="25px" height="25px"></img>
                                    </div>
                                </div>
                            </td>
                            <td style="text-align: left; vertical-align: top; width: 85%;">
                                <table style="width: 100%;" align="left">
                                    <tr>
                                        <td style="text-align: left; vertical-align: top;">
                                            <table id="Table1" width="100%">
                                                <tr>
                                                    <td style="text-align: left;">
                                                        <asp:HiddenField ID="hnkClickLink" runat="server" />
                                                        <asp:HiddenField ID="hnkPageCount" runat="server" />
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <table style="width: 100%">
                                                                    <tr>
                                                                        <td>
                                                                            <fieldset>
                                                                                <table align="center" width="100%">
                                                                                    <tr>
                                                                                        <td style="vertical-align: top; text-align: left;">
                                                                                            <div class="target">
                                                                                                API Documentation
                                                                                            </div>
                                                                                        </td>
                                                                                        <td style="vertical-align: top; text-align: right;">
                                                                                            <div class="summary">
                                                                                                <asp:Label ID="LabelSummary" runat="server" Text="<%# Summary %>"></asp:Label>
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </fieldset>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="text-align: left; vertical-align: top;">
                                                                            <table align="left" width="60%">
                                                                                <tr>
                                                                                    <td style="text-align: left;">
                                                                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:Repeater ID="Repeater1" runat="server">
                                                                                                    <ItemTemplate>
                                                                                                        <div class="Repater">
                                                                                                            <p>
                                                                                                                <a class="link" href='Result.aspx?rs=<%# DataBinder.Eval(Container.DataItem, "path")  %>&title=<%# DataBinder.Eval(Container.DataItem, "title")  %>' target="_blank" type='<%# DataBinder.Eval(Container.DataItem, "Type")  %>'><%# DataBinder.Eval(Container.DataItem, "title")  %></a>
                                                                                                                <img src="ImageHandler.ashx?rs=<%# DataBinder.Eval(Container.DataItem, "path")  %>" style="visibility: <%# (Eval("sample") == "") ? "visible" : "hidden" %>" height="25px" width="25px" />
                                                                                                                <br />
                                                                                                                <span class="sample"><%# (Convert.ToString(DataBinder.Eval(Container.DataItem, "sample")) == "NULL") ? "" : DataBinder.Eval(Container.DataItem, "sample") %></span>
                                                                                                                <br />
                                                                                                                <span class="path"><%# DataBinder.Eval(Container.DataItem, "url")  %></span>
                                                                                                            </p>
                                                                                                        </div>
                                                                                                    </ItemTemplate>
                                                                                                </asp:Repeater>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <div class="paging">
                                                                                Result page:
                                                                                                <%--<asp:Repeater ID="Repeater2" runat="server" DataSource="<%# Paging %>">
                                                                                                    <ItemTemplate>
                                                                                                        <%# DataBinder.Eval(Container.DataItem, "html") %>
                                                                                                    </ItemTemplate>
                                                                                                </asp:Repeater>--%>

                                                                                <asp:Button ID="btnPrev" runat="server" Text="Prev" OnClick="btnPrev_Click" CssClass="btn" />
                                                                                <asp:Repeater ID="rptPages" runat="server" OnItemCommand="rptPages_ItemCommand">

                                                                                    <ItemTemplate>

                                                                                        <asp:LinkButton ID="btnPage" CommandName="Page" CommandArgument="<%# Container.DataItem %>" runat="server"><%# Container.DataItem %></asp:LinkButton>

                                                                                    </ItemTemplate>

                                                                                </asp:Repeater>
                                                                                <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" CssClass="btn" />
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </td>

                                                </tr>

                                            </table>

                                            <div class="footer"> <a href="smsjuju.com">See Smsjuju Products</a> 
                                                <br />
                                                <a href="help.aspx">Help</a> 

                                            </div>

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
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                <ProgressTemplate>
                    <div class="overlay">
                        <img alt="" src="Images/progressbar.gif" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </form>
</body>
</html>
