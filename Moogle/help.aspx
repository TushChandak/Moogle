<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="help.aspx.cs" Inherits="Moogle.help" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div id="floatDiv">
                <fieldset>
                    <table class="tbl">
                        <tr>
                            <td style="text-align: left;">Menu</td>
                            <td style="text-align: left;">File Types</td>
                        </tr>

                        <tr>
                            <td style="text-align: left;">All:</td>
                            <td style="text-align: left;">Show all types of Files</td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">Documents:</td>
                            <td style="text-align: left;">docx, doc, pdf</td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">Code and Text:</td>
                            <td style="text-align: left;">txt, cs, xml, css, aspx, htm, js</td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">Images:</td>
                            <td style="text-align: left;">png, jpg, bmp, gif</td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">Other:</td>
                            <td style="text-align: left;">exe, dll, rpt, sql etc</td>
                        </tr>
                    </table>
                </fieldset>
            </div>
</asp:Content>
