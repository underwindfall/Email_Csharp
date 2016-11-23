<%@ Page Title="" Language="C#" MasterPageFile="~/MailMasterPage.Master" AutoEventWireup="true" CodeBehind="EmailEcrire.aspx.cs" Inherits="Email_Test.EmailEcrire" %> 
<%@ Register Src="~/ToolFileUpload.ascx" TagPrefix="uc1" TagName="ToolFileUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script src="scripte/jquery-1.7.1.js"></script>
    <link href="StyleEmailMenu.css" rel="stylesheet" />
    <link href="css/Styles.css" rel="stylesheet" />
    <script src="scripte/jquery-1.7.1.min.js"></script>
    <script src="scripte/ckeditor/ckeditor.js"></script>  
    <script type="text/javascript">
        CKEDITOR.replace('tbxContent');
        var editor = CKEDITOR.instances.tbxContent;
        var editorHtml = document.getElementById("_editorHtml");
        editor.setData(decodeURIComponent(editorHtml.value));
        CKEDITOR.on('instanceReady', function (e) {
            var td = document.getElementsByClassName('cke_contents cke_reset');
            td[0].style.height = '570px';
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>
              <div id="divmessage" style="visibility: hidden">
                                <asp:HiddenField ID="messagehid" runat="server" />
                                <asp:Button ID="btnHid" runat="server" Text="Button" OnClick="btnHid_Click" />
                            </div>

                            <div>
                                <asp:Label ID="lblNewmail" runat="server" Text="Nouveau Message"></asp:Label>
                                <asp:Button ID="btnEnvoyer" runat="server" Text="Envoyer" OnClick="btnEnvoyer_Click" />
                                <asp:Button ID="btnAnnuler" runat="server" Text="Annuler" OnClick="btnAnnuler_Click" />
                                <asp:Button ID="btnEnregistre" runat="server" Text="Enregistrer le brouillon" OnClick="btnEnregistre_Click" />

                            </div>
                            <div style="margin-top: 5px">
                                <asp:Button ID="btnAdresseRecu" runat="server" Text="A" CssClass="button_style1" BackColor="White" Style="-webkit-border-radius: 15px" />
                                <asp:TextBox ID="tbxAdresseRecu" runat="server" Style="width: 90%"></asp:TextBox>

                            </div>

                            <div style="margin-top: 5px">
                                <asp:Button ID="btnAdresseCC" runat="server" Text="CC" CssClass="button_style1" Style="-webkit-border-radius: 15px" BackColor="White" />
                                <asp:TextBox ID="tbxAdresseCC" runat="server" Style="width: 90%"></asp:TextBox>

                            </div>
                            <div style="margin-top: 5px">
                                <asp:Label ID="btnSujet" runat="server" Text="Sujet" Style="width: 100px; text-align: center" BorderStyle="None"></asp:Label>
                                <asp:TextBox ID="tbxSujet" runat="server" Style="width: 77%"></asp:TextBox>
                                <asp:Label ID="lblPriorite" runat="server" Text="Priorité" Style="width: 100px; text-align: center" BorderStyle="None"></asp:Label>
                                <asp:DropDownList ID="ddlPriorite" runat="server">
                                    <asp:ListItem Selected="True">Normal</asp:ListItem>
                                    <asp:ListItem>High</asp:ListItem>
                                    <asp:ListItem>Low</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div id="fichier_joindre" style="margin-top: 5px; width: 100%">
                                <uc1:ToolFileUpload runat="server" ID="ToolFileUpload" />
                            </div>
                            <div id="divtex" style="margin-top: 5px">
                                <asp:HiddenField ID="_editorHtml" runat="server" />
                                <asp:HiddenField ID="_editorText" runat="server" />
                                <div style="height: 100%">
                                    <asp:TextBox ID="tbxContent" runat="server" CssClass="ckeditor" TextMode="MultiLine" Style="position: relative; width: 100%; height: auto; top: 0px; left: 0px;"></asp:TextBox>
                                  

                                </div>
                            </div>













</asp:Content>
