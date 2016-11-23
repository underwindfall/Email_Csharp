<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="recevoirmail.aspx.cs" Inherits="Email_Test.recevoirmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="v1" runat="server">
  <br />
        <br /><br /><br /><br /><br />   <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" OnPageIndexChanging="GridView1_PageIndexChanging" Width="1456px">   <Columns>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="checkSelect" runat="server" />
                                                    <asp:CheckBox ID="checkImportant" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="mailsender" />
                                            <asp:BoundField DataField="mailfrom" />
                                            <asp:BoundField DataField="mailsubject" />
                                            <asp:BoundField DataField="mailbodySimple" />
                                            <asp:BoundField DataField="maildateTime" />
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Button" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns></asp:GridView>
        <div id="div1" runat="server">
            
        </div>
            </asp:View>
        </asp:MultiView>
      
     
         <br />
        <br /><br /><br /><br /><br />
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
    </form>
</body>
</html>
