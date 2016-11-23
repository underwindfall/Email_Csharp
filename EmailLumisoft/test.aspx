   <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="Email_Test.test" %>

<%@ Register Src="~/ToolFileUpload.ascx" TagPrefix="uc1" TagName="ToolFileUpload" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="scripte/jquery-1.7.1.js"></script>
   
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <uc1:ToolFileUpload runat="server" ID="ToolFileUpload" />
        <br />
        <br />
        <br />
        <br />

        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
    </div>
    </form>
</body>
</html>
