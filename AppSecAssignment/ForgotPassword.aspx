<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="AppSecAssignment.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Change Password </h1>
    <table style="width: 100%;">
        <tr>
            <td style="width: 311px">
                <asp:Label ID="Email" runat="server" Text="Email"></asp:Label>
            </td>
            <td style="width: 185px">
                <asp:TextBox ID="email_tb" runat="server" TextMode="Email" autocomplete="off"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="message_lbl" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 311px; height: 22px;">
                <asp:Label ID="Label7" runat="server" Text="New Password"></asp:Label>
            </td>
            <td style="width: 185px; height: 22px;">
                <asp:TextBox ID="password_tb" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
            </td>
            <td style="height: 22px"></td>
        </tr>
        <tr>
            <td style="width: 311px">
                <asp:Label ID="Label6" runat="server" Text="Confirm New Password"></asp:Label>
            </td>
            <td style="width: 185px">
                <asp:TextBox ID="confirmPassword_tb" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 311px">&nbsp;</td>
            <td style="width: 185px">
                <asp:Button ID="backBtn" runat="server" OnClick="backBtn_Click" Text="Back" />
            </td>
            <td>
                <asp:Button ID="changePasswordBtn" runat="server" Text="Change Password" OnClick="changePasswordBtn_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
