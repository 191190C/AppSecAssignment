<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="AppSecAssignment.Success" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1> User Profile </h1>
    <asp:Label ID="email_lbl" runat="server" Text="Email: "></asp:Label>
    <table style="width:100%;">
        <tr>
            <td>
                <asp:Label ID="message_lbl" runat="server"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="logOutBtn" runat="server" OnClick="logOutBtn_Click" Text="Log Out" />
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>
