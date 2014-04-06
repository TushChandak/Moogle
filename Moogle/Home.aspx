<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Moogle._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


    <link href="main.css" rel="stylesheet" />
    <table style="height: 450px; vertical-align: middle; align-content: center; width: 100%;">
        <tr>
            <td>
                <div style="vertical-align: central; margin: auto;">
                    <p align="center">
                        <img src="Images/soogle.jpg" alt="Soogle" />
                    </p>
                    <p align="center">
                        <asp:TextBox ID="TxtSearchkeyword" runat="server" Width="300px" size="32" CssClass="txtBox"/>&nbsp;<asp:Button runat="server" ID="BtnSearch" OnClick="BtnSearch_Click" Text="Search" CssClass="btn"/>
                    </p>
                </div>
            </td>
        </tr>
    </table>

</asp:Content>
