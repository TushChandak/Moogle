<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="ContentSearchDetails.aspx.cs" Inherits="Moogle.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .TRNormal {
            background-color: lightgray;
        }

        .TRAlt {
            background-color: whitesmoke;
        }
    </style>

    <table style="width: 100%">
        <tr>
            <td>
                <fieldset>
                    <table align="center" width="100%">

                        <tr>

                            <td style="vertical-align: top; text-align: left;">
                                <div class="target">API Documentation</div>
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
            <td>
                <table align="center" width="100%">
                    <tr>
                        <td style="text-align: left;">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <div class="Repater">
                                                <p class="<%# Container.ItemIndex % 2 == 0 ? "TRNormal" : "TRAlt" %>">
                                                    <a href='Result.aspx?rs=<%# DataBinder.Eval(Container.DataItem, "path")  %>&title=<%# DataBinder.Eval(Container.DataItem, "title")  %>' class="link" type='<%# DataBinder.Eval(Container.DataItem, "Type")  %>' target="_blank"><%# DataBinder.Eval(Container.DataItem, "title")  %></a>
                                                    <img src="ImageHandler.ashx?rs=<%# DataBinder.Eval(Container.DataItem, "path")  %>" style="visibility: <%# (Convert.ToString(Eval("sample")) == "") ? "visible" : "hidden" %>" height="25px" width="25px" />
                                                    <br />

                                                    <span class="sample">

                                                        <%# DataBinder.Eval(Container.DataItem, "sample")  %>
                                                    </span>
                                                    <br>
                                                    <span class="path">
                                                        <%# DataBinder.Eval(Container.DataItem, "url")  %>
                                                    </span>
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
				        <asp:Repeater ID="Repeater2" runat="server" DataSource="<%# Paging %>">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "html") %>
                            </ItemTemplate>
                        </asp:Repeater>
                </div>
            </td>

        </tr>
    </table>

</asp:Content>
