<%@ Page Title="" Language="C#" MasterPageFile="~/MailMasterPage.Master" AutoEventWireup="true" CodeBehind="EmailBrouillons.aspx.cs" Inherits="Email_Test.EmailBrouillons" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> <asp:ScriptManager runat="server"></asp:ScriptManager>
    <asp:GridView ID="gvmailBrouillons" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" Width="1400px"  BorderStyle="None" BorderWidth="0px" CellPadding="5" GridLines="None" OnRowDeleted="gvmailBrouillons_RowDeleted" OnSelectedIndexChanged="gvmailBrouillons_SelectedIndexChanged">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailto"></asp:BoundField>
                                    <asp:BoundField DataField="mailsubject">
                                        <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbody">
                                        <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                       
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow: ellipsis; overflow: hidden"></asp:Label></ItemTemplate>
                                        <ControlStyle Width="800px" />
                                        <ItemStyle Width="800px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Edit" /><asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Delete" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
</asp:Content>
