<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test1.aspx.cs" Inherits="Email_Test.test1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div style="text-align: center">
        <strong><span style="color: #3333ff">GridView+FormView 新增/修改/删除<br />
        </span></strong>
        <br />
        <Center>
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                CellPadding="4" DataKeyNames="EmployeeID" DataSourceID="SqlDataSource1" EmptyDataText="没有数据可显示。"
                ForeColor="#333333" GridLines="None" PageSize="5" OnRowCommand="GridView1_RowCommand" >
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#EFF3FB" />
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <HeaderTemplate>
                            <asp:Button ID="btnInsert" runat="server" CausesValidation="False" CommandName="Insert"
                                Text="新增"></asp:Button>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                Text="编辑"></asp:Button>
                            <asp:Button ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="删除"></asp:Button>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmployeeID" HeaderText="EmployeeID" InsertVisible="False"
                        ReadOnly="True" SortExpression="EmployeeID" />
                    <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                    <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                </Columns>
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#2461BF" />
                <AlternatingRowStyle BackColor="White" />
            </asp:GridView>
            <asp:FormView ID="FormView1" runat="server" CellPadding="4" DataKeyNames="EmployeeID"
                DataSourceID="SqlDataSource1" DefaultMode="Edit" ForeColor="#333333" Visible="False" OnPreRender="FormView1_PreRender" OnItemCommand="FormView1_ItemCommand" OnItemInserted="FormView1_ItemInserted" OnItemUpdated="FormView1_ItemUpdated" >
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                <EditItemTemplate>
                    <table style="width: 400px">
                        <tr>
                            <td style="width: 100px; text-align: left">
                                EmployeeID</td>
                            <td style="width: 392px; text-align: left">
                                <asp:Label ID="EmployeeIDLabel1" runat="server" Text='<%# Eval("EmployeeID") %>'></asp:Label>
                                <asp:TextBox ID="txtEmployeeID" runat="server" Text='<%# Bind("EmployeeID") %>'></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 100px; text-align: left">
                                LastName</td>
                            <td style="width: 392px; text-align: left">
                                <asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName") %>'></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 100px; text-align: left">
                                FirstName</td>
                            <td style="width: 392px; text-align: left">
                                <asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 100px; text-align: left">
                                Title</td>
                            <td style="width: 392px; text-align: left">
                                <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title") %>'></asp:TextBox></td>
                        </tr>
                    </table>
                    <br />
                    <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="False" CommandName="Insert"
                        Text="新增"></asp:LinkButton>
                    <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                        Text="更新"></asp:LinkButton>
                    <asp:LinkButton ID="CancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                        Text="取消"></asp:LinkButton>
                </EditItemTemplate>
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            </asp:FormView>
        </Center>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NorthwindConnectionString1 %>"
            DeleteCommand="DELETE FROM [Employees] WHERE [EmployeeID] = @original_EmployeeID" InsertCommand="INSERT INTO [Employees] ([LastName], [FirstName], [Title]) VALUES (@LastName, @FirstName, @Title)"
            ProviderName="<%$ ConnectionStrings:NorthwindConnectionString1.ProviderName %>"
            SelectCommand="SELECT [EmployeeID], [LastName], [FirstName], [Title] FROM [Employees]"
            UpdateCommand="UPDATE [Employees] SET [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title WHERE [EmployeeID] = @original_EmployeeID" OldValuesParameterFormatString="original_{0}">
            <DeleteParameters>
                <asp:Parameter Name="original_EmployeeID" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter Name="Title" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter Name="Title" Type="String" />
                <asp:Parameter Name="original_EmployeeID" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    </div>
    </form>
</body>
</html>
