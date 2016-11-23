<%@ Page Title="" Language="C#" MasterPageFile="~/MailMasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Email_Test.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <br />
    <br />
    <div align="center">
        <div align="center" style="background-image:url(image/cadre_gray_340_01.gif);width:340px;height:19px;margin-top:100px;"></div>
        <div align="center" style="background-image: url(image/cadre_gray_340_02.gif); width: 340px; height: 100px;">
            <table>
                <tr>
                    <td>Nom de Utilisateur:</td>
                    <td>
                        <asp:TextBox ID="tblogin" runat="server" MaxLength="20" Width="140px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="tblogin" />
                    </td>
                </tr>
                <tr>
                    <td>Mot de Passe:</td>
                    <td>
                        <asp:TextBox ID="tbmdp" runat="server" MaxLength="20" TextMode="Password" Width="140px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="tbmdp" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                         <asp:Button ID="btnLogin" runat="server" Text="Enter" OnClick="btnLogin_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div align="center" style="background-image:url(image/cadre_gray_340_03.gif);width:340px;height:19px;"></div>
        <br />
        <img alt="" src="image/squares - 复制.gif"
    </div>
</asp:Content>
