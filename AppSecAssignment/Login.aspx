<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAssignment.Login1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Google Captcha v3 -->
    <script src="https://www.google.com/recaptcha/api.js?render=6LdegD4aAAAAAATC_wmJTXHY2-DCXgGxIJ7QRch3"></script>
    <h1>Login </h1>
        <table style="width: 100%;">
            <tr>
                <td style="width: 311px">
                    <asp:Label ID="Label7" runat="server" Text="Email Address"></asp:Label>
                </td>
                <td style="width: 185px">
                    <asp:TextBox ID="email_tb" runat="server" TextMode="Email" autocomplete="off"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="message_lbl" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 311px">
                    <asp:Label ID="Label6" runat="server" Text="Password"></asp:Label>
                </td>
                <td style="width: 185px">
                    <asp:TextBox ID="password_tb" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
                </td>
                <td>
                    <asp:HyperLink ID="forgotPassword_hl" NavigateUrl="ForgotPassword.aspx" runat="server">Forgot Password?</asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td style="width: 311px">&nbsp;</td>
                <td style="width: 185px">
                    <asp:Button ID="backBtn" runat="server" OnClick="backBtn_Click" Text="Back" />
                </td>
                <td>
                    <asp:Button ID="submitBtn" runat="server" Text="Submit" OnClick="submitBtn_Click" />
                </td>
            </tr>
        </table>

    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>    
    <br />
    <br />

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LdegD4aAAAAAATC_wmJTXHY2-DCXgGxIJ7QRch3', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</asp:Content>
