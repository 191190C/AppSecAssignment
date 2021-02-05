<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AppSecAssignment.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    &nbsp;&nbsp;&nbsp;
    <script type="text/javascript">
        function validatePassword() {
            var pwd = document.getElementById("password_tb").value;

            // If password length is less than 8, do not allow
            if (pwd.length < 8) {
                document.getElementById("pwdRating_lbl").innerHTML = "Password length must be at least 8 characters.";
                document.getElementById("pwdRating_lbl").style.color = red;
                return ("too_short");
            }

            // If password does not have at least one numeral, do not allow
            else if (pwd.search(/[0-9]/) == -1)) {
                document.getElementById("pwdRating_lbl").innerHTML = "Password requires at least one number.";
                document.getElementById("pwdRating_lbl").style.color = red;
            }


            document.getElementById("pwdRating_lbl").innerHTML = "Password is strong.";
            document.getElementById("pwdRating_lbl").style.color = green;
        }
    </script>
    <!-- Google Captcha v3 -->
    <script src="https://www.google.com/recaptcha/api.js?render=6LdegD4aAAAAAATC_wmJTXHY2-DCXgGxIJ7QRch3"></script>

    <h1>Register </h1>
        <table style="width: 100%;">
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label1" runat="server" Text="First Name"></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="fname_tb" runat="server" autocomplete="off"></asp:TextBox>
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label2" runat="server" Text="Last Name"></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="lname_tb" runat="server" autocomplete="off"></asp:TextBox>
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label3" runat="server" Text="Credit Card No."></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="ccno_tb" runat="server" Style="margin-bottom: 0" autocomplete="off"></asp:TextBox>
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label7" runat="server" Text="Email Address"></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="email_tb" runat="server" TextMode="Email" autocomplete="off"></asp:TextBox>
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label6" runat="server" Text="Password"></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="password_tb" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
                </td>
                <td style="width: 270px">
                    <asp:Label ID="pwdRating_lbl" runat="server"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    &nbsp;</td>
                <td style="width: 181px">
                    <asp:Button ID="checkPasswordBtn" runat="server" Text="Check Password" OnClick="checkPasswordBtn_Click" />
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    &nbsp;</td>
                <td style="width: 181px">
                    &nbsp;</td>
                <td style="width: 270px">
                    &nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">
                    <asp:Label ID="Label8" runat="server" Text="Date of Birth"></asp:Label>
                </td>
                <td style="width: 181px">
                    <asp:TextBox ID="dob_tb" type="date" runat="server"></asp:TextBox>
                </td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px; height: 22px;">
                    &nbsp;</td>
                <td style="width: 181px; height: 22px;">
                    <asp:Button ID="backBtn" runat="server" OnClick="backBtn_Click" Text="Back" />
                </td>
                <td style="width: 270px; height: 22px;">
                    <asp:Button ID="submitBtn" runat="server" Text="Submit" OnClick="submitBtn_Click" />
                </td>
                <td style="height: 22px"></td>
            </tr>
            <tr>
                <td style="width: 281px">&nbsp;</td>
                <td style="width: 181px">
                    <asp:Label ID="error_lbl" runat="server"></asp:Label>
                </td>
                <td style="width: 270px">
                    &nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 281px">&nbsp;</td>
                <td style="width: 181px">
                    &nbsp;</td>
                <td style="width: 270px">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>

    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/> 
    <br />
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
