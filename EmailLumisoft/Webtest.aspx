<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Email_Test.Default" %>

<%@ Register Src="~/ToolFileUpload.ascx" TagPrefix="uc1" TagName="ToolFileUpload" %>
<link href="css/Styles.css" rel="stylesheet" />
<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="scripte/jquery-1.7.1.js"></script>
    <link href="StyleEmailMenu.css" rel="stylesheet" />
    <script src="scripte/jquery-1.7.1.min.js"></script>
    <script src="scripte/ckeditor/ckeditor.js"></script>
    <script type="text/javascript">
        function slide() {
            $(document).ready(function () {

                $("#menu_left").find("div").live("mouseover", function () {
                    $(this).addClass('moved');
                    $(this).children("input[type=submit]").addClass('moved');
                });
                $("#menu_left").find("div").live("mouseout", function () {
                    $(this).removeClass('moved');
                    $(this).children("input[type=submit]").removeClass('moved');
                });
                $("#menu_left").find("div").live("click", function () {
                    $(this).addClass('clicked');
                    $(this).children("input[type=submit]").addClass('clicked');
                });
                CKEDITOR.replace('tbxContent');
                var editor = CKEDITOR.instances.tbxContent;
                var editorHtml = document.getElementById("_editorHtml");
                editor.setData(decodeURIComponent(editorHtml.value));
                CKEDITOR.on('instanceReady', function (e) {
                    var td = document.getElementsByClassName('cke_contents cke_reset');
                    td[0].style.height = '570px';

                });
                //setInterval(function () {
                //    $("#menu_left").load(location.href + " #menu_left>*", "");
                //}, 5000);
            });

            function updateJs() {
                var h = document.getElementById('tbxContent').offsetHeight;
                CKEDITOR.replace('tbxContent');
                var editor = CKEDITOR.instances.tbxContent;
                var editorHtml = document.getElementById("_editorHtml");
                editor.setData(decodeURIComponent(editorHtml.value));
                CKEDITOR.on('instanceReady', function (e) {
                    var td = document.getElementsByClassName('cke_contents cke_reset');
                    td[0].style.height = '570px';

                    //var tbody = td.parentNode;
                    //var tbody2 = td.parentNode.parentNode;
                    //td.style.height = h - tbody.rows[0].offsetHeight - tbody.rows[2].offsetHeight + 'px';
                });
            };
        };
        function load() { Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler); };
        function EndRequestHandler() { slide(); };
    </script>
    <%--<script type="text/javascript">
        function onSubmit() {
            var etat = '<%= getMultiviewEtat() %>';
            if (etat == '0') {
                var editor = CKEDITOR.instances.tbxContent;
                var editorHtml = document.getElementById("_editorHtml");
                var editorText = document.getElementById("_editorText");
                if (editor.getData().length != 0) {
                    editorHtml.value = encodeURIComponent(editor.getData());
                }
                if (editor.getData().length != 0) {
                    editorText.value = editor.document.getBody().getText();
                }
                editor.setData("");//清空ckeditor
            }
            return true;
        }
        //function doSubmit() {
        //    document.forms[0].submit();
        //}
    </script>--%>

</head>

<body>
    <form id="form1" runat="server" onsubmit="return onSubmit()">
   
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"></asp:ScriptManager>
         <div id="headerEmail" style="border: 3px solid #fef8e6; height: 50px; width: 100%; -webkit-border-radius: 15px; -moz-border-radius: 15px;">
            <asp:Image ID="Image1" runat="server" Height="40px" ImageUrl="~/image/logo_sipcom.gif" Width="160px" />
            <asp:Label ID="Label2" runat="server" Text="Email" Style="position: relative; font-size: 30px; margin-left: 10px; top: -10px" BorderStyle="None"></asp:Label>
        </div>
     <div style="width: 100%; margin-top: 5px;">
                     <asp:Panel ID="menu_left" runat="server" Style="float: left; position: relative; padding: 5px; display: table-cell; border: 3px solid  #fef8e6; -webkit-border-radius: 15px;" Width="200px">

                <div id="divEmailNouveau" class="div_normal" runat="server">
                    <asp:Button ID="btnEmailNouveau" runat="server" CssClass="button_normal" Text="Nouveau message" OnClick="btnEmailNouveau_Click" />
                </div>
                <div id="divEmailRecu" class="div_normal" runat="server">
                    <asp:Button ID="btnEmailRecu" CssClass="button_normal" runat="server" Text="Boîte de réception" OnClick="btnEmailRecu_Click" /><asp:Label ID="lblnbrmailNoseen" runat="server" Font-Bold="True"></asp:Label>/<asp:Label ID="lblnbrmail" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailEnvoye" class="div_normal" runat="server">
                    <asp:Button ID="btnEmailEnvoye" CssClass="button_normal" runat="server" Text="Messages envoyés" OnClick="btnEmailEnvoye_Click" /><asp:Label ID="lblnbrsend" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailBrouillons" class="div_normal" runat="server">

                    <asp:Button ID="btnEmailBrouillons" CssClass="button_normal" runat="server" Text="Brouillons" OnClick="btnEmailBrouillons_Click" Height="21px" /><asp:Label ID="lblnbrbrou" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailCorbeille" class="div_normal" runat="server">
                    <asp:Button ID="btnEmailCorbeille" CssClass="button_normal" runat="server" Text="Corbeille" OnClick="btnEmailCorbeille_Click" /><asp:Label ID="lblnbrcorb" runat="server" Text=""></asp:Label>
                </div>
                <%-- <div id="divEmailMsgImpor" class="div_normal" runat="server" visible="false">
                    <asp:Button ID="btnEmailMsgImpor" CssClass="button_normal" runat="server" Text="Messages importants"  /><asp:Label ID="lblnbrmailimp" runat="server" Text=""></asp:Label>
                </div>--%>
            </asp:Panel>
           <div style="display: table-cell;">
               <asp:Panel ID="viewMail" runat="server" Style="padding: 5px; height: 100%; border: 3px solid  #fef8e6; -webkit-border-radius: 15px;">
                 <div id="divmessage" style="visibility: hidden">
                                <asp:HiddenField ID="messagehid" runat="server" />
                                <asp:Button ID="btnHid" runat="server" Text="Button" OnClick="btnHid_Click" />
                  </div>
        
               <br />
               <br />
                
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
                                </div>
                        </div>
       </asp:Panel>
           </div>
     </div>
    </form>
</body>
</html>
