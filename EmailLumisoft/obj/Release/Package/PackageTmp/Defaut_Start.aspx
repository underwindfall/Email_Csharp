<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Defaut_Start.aspx.cs" Inherits="Email_Test.Defaut_Start" %>

<%@ Register Src="~/ToolFileUpload.ascx" TagPrefix="uc1" TagName="ToolFileUpload" %>

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


</head>



<body>
    <form id="form1" runat="server" onsubmit="return onSubmit()">
        <%-- --%>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"></asp:ScriptManager>
        <div id="headerEmail" style="border: 3px solid #fef8e6; height: 50px; width: 100%;-webkit-border-radius: 15px;-moz-border-radius: 15px;">
            <asp:Image ID="Image1" runat="server" Height="40px" ImageUrl="~/image/logo_sipcom.gif" Width="160px" />
            <asp:Label ID="Label2" runat="server" Text="Email" Style="position: relative; font-size: 30px; margin-left: 10px; top: -10px" BorderStyle="None"></asp:Label>
       <div style="float:right">
           <asp:ImageButton ID="imgBtnUser" runat="server" /><asp:Label ID="lbluser" runat="server" Text="UserName"></asp:Label><asp:ImageButton ID="imgBtnSetting" runat="server" />

       </div>
          </div>   
       
        <div style="width: 100%;margin-top:5px;">
          
            <asp:Panel ID="menu_left" runat="server" Style="float: left; position: relative; padding: 5px;display:table-cell;border: 3px solid  #fef8e6;-webkit-border-radius: 15px;" Width="200px">

                <div id="divEmailNouveau" class="<%# divEmailNouveauCss %>" runat="server">
                    <asp:Button ID="btnEmailNouveau" runat="server" CssClass="<%#btnEmailNouveauCss %>" Text="Nouveau message" OnClick="btnEmailNouveau_Click" />
                </div>
                <div id="divEmailRecu" class="<%# divEmailRecuCss %>" runat="server">
                    <asp:Button ID="btnEmailRecu" CssClass="<%# btnEmailRecuCss %>" runat="server" Text="Boîte de réception" OnClick="btnEmailRecu_Click" /><asp:Label ID="lblnbrmailNoseen" runat="server" Font-Bold="True"></asp:Label>/<asp:Label ID="lblnbrmail" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailEnvoye" class="<%# divEmailEnvoyeCss %>" runat="server">
                    <asp:Button ID="btnEmailEnvoye" CssClass="<%# btnEmailEnvoyeCss %>" runat="server" Text="Messages envoyés" OnClick="btnEmailEnvoye_Click" /><asp:Label ID="lblnbrsend" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailMsgImpor" class="<%# divEmailMsgImporCss %>" runat="server">
                    <asp:Button ID="btnEmailMsgImpor" CssClass="<%# btnEmailMsgImporCss %>" runat="server" Text="Messages importants" OnClick="btnEmailMsgImpor_Click" /><asp:Label ID="lblnbrmailimp" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailBrouillons" class="<%# divEmailBrouillonsCss %>" runat="server">

                    <asp:Button ID="btnEmailBrouillons" CssClass="<%# btnEmailBrouillonsCss %>" runat="server" Text="Brouillons" OnClick="btnEmailBrouillons_Click" Height="21px" /><asp:Label ID="lblnbrbrou" runat="server" Text=""></asp:Label>
                </div>
                <div id="divEmailCorbeille" class="<%# divEmailCorbeilleCss %>" runat="server">
                    <asp:Button ID="btnEmailCorbeille" CssClass="<%# btnEmailCorbeilleCss %>" runat="server" Text="Corbeille" OnClick="btnEmailCorbeille_Click" /><asp:Label ID="lblnbrcorb" runat="server" Text=""></asp:Label>
                </div>


            </asp:Panel>

            <div style="display: table-cell;">
                <%--<div style="position: relative; display: table-cell;margin-left: 200px; height: 100%; top: 0px; left: 0px;width:100%">--%>
                <asp:Panel ID="viewMail" runat="server" Style="padding: 5px; height: 100%;border: 3px solid  #fef8e6;-webkit-border-radius: 15px;">

                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="1">
                        <asp:View ID="ViewEcrire" runat="server" OnActivate="ViewEcrire_Activate" OnDeactivate="ViewEcrire_Deactivate">
                            <div>
                                <asp:Label ID="lblNewmail" runat="server" Text="Nouveau Message"></asp:Label>
                                <asp:Button ID="btnEnvoyer" runat="server" Text="Envoyer" OnClick="btnEnvoyer_Click" />
                                <asp:Button ID="btnAnnuler" runat="server" Text="Annuler" OnClick="btnAnnuler_Click" />
                                <asp:Button ID="btnEnregistre" runat="server" Text="Enregistre le brouillon" OnClick="btnEnregistre_Click" />
                                <asp:Button ID="Button4" runat="server" Text="Button" OnClick="Button4_Click" />
                            </div>
                            <div style="margin-top:5px">
                                <asp:Button ID="btnAdresseRecu" runat="server" Text="A" CssClass="button_style1" BackColor="White" style="-webkit-border-radius: 15px"/>
                                <asp:TextBox ID="tbxAdresseRecu" runat="server" Style="width: 90%"></asp:TextBox>

                            </div>
                           
                            <div style="margin-top:5px">
                                <asp:Button ID="btnAdresseCC" runat="server" Text="CC" CssClass="button_style1" style="-webkit-border-radius: 15px" BackColor="White" />
                                <asp:TextBox ID="tbxAdresseCC" runat="server" Style="width: 90%"></asp:TextBox>

                            </div>
                            <div style="margin-top:5px">
                                <asp:Label ID="btnSujet" runat="server" Text="Sujet" Style="width: 100px; text-align: center" BorderStyle="None"></asp:Label>
                                <asp:TextBox ID="tbxSujet" runat="server" Style="width: 77%"></asp:TextBox>
                                <asp:Label ID="lblPriorite" runat="server" Text="Priorité" Style="width: 100px; text-align: center" BorderStyle="None"></asp:Label>
                                <asp:DropDownList ID="ddlPriorite" runat="server">
                                    <asp:ListItem Selected="True">Normal</asp:ListItem>
                                    <asp:ListItem>High</asp:ListItem>
                                    <asp:ListItem>Low</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div id="fichier_joindre" style="margin-top:5px">
                                <uc1:ToolFileUpload runat="server" ID="ToolFileUpload" />
                            </div>
                             <div id="divtex" style="margin-top:5px">
                                    <asp:HiddenField ID="_editorHtml" runat="server" />
                                        <asp:HiddenField ID="_editorText" runat="server" />
                                    <div style="height: 100%">
                                        <asp:TextBox ID="tbxContent"  runat="server" CssClass="ckeditor" TextMode="MultiLine" Style="position: relative; width: 100%; height: auto; top: 0px; left: 0px;"></asp:TextBox>
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
                              <%--  </ContentTemplate>
                            </asp:UpdatePanel>--%>

                        </asp:View>

                        <asp:View ID="ViewPrincipal" runat="server" OnActivate="ViewPrincipal_Activate">
                            <asp:Button ID="btnactualiser" runat="server" Text="Actualiser" OnClick="btnactualiser_Click" /><asp:Button ID="btnsuppr" runat="server" Text="Suppr" OnClick="btnsuppr_Click" />
                            <br />
                           
                            <asp:GridView ID="gvViewEmail" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="mailID" OnPageIndexChanging="gvViewEmail_PageIndexChanging" OnSelectedIndexChanged="gvViewEmail_SelectedIndexChanged" PageSize="20" Width="1400px" OnRowDeleting="gvViewEmail_RowDeleting" BorderStyle="None" BorderWidth="0px" OnRowDataBound="gvViewEmail_RowDataBound" CellPadding="5" GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" />
                                        </ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailsender">
                                    <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailfrom">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailsubject">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbodySimple">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NotSeen">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>                                   
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow:ellipsis;overflow:hidden" ></asp:Label>
                                        </ItemTemplate>
                                        <ControlStyle Width="800px" />
                                        <ItemStyle Width="800px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Detail" />
                                            <asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Suppr" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>

                        <asp:View ID="ViewDetail" runat="server" OnActivate="ViewDetail_Activate">
                            <asp:Panel ID="Panel1" runat="server" style="height:850px;border: 3px solid  #fef8e6;Width:1400px;-webkit-border-radius: 15px;">
                            <div id="divmenu">
                                <asp:Button ID="btnreturnINDetail" runat="server" Text="Retour" OnClick="btnreturnINDetail_Click" />
                                <asp:Button ID="btnSupprINDetail" runat="server" Text="Suppr" OnClick="btnSupprINDetail_Click" />
                                <asp:Button ID="Button6" runat="server" Text="Button" OnClick="Button6_Click" />
                                <asp:Button ID="Button7" runat="server" Text="Button" OnClick="Button7_Click" />
                            </div>
                            <div id="divsubject">
                                <asp:Label ID="lblsubject" runat="server" Text="Label"></asp:Label>
                            </div>
                            <div id="divfrom">
                                From :
                                <asp:Label ID="lblsender" runat="server" Text="Label"></asp:Label>
                                <asp:Label ID="lblfrom" runat="server" Text="Label"></asp:Label>
                                <asp:Label ID="lbldate" runat="server" Text="Label"></asp:Label>
                            </div>
                            <div id="divto">
                                To :
                                <asp:Label ID="lblto" runat="server" Text="Label"></asp:Label>
                            </div>
                            <div id="divcc">
                                Cc :
                                <asp:Label ID="lblcc" runat="server" Text="Label"></asp:Label>
                            </div>
                            <div id="divattatement" runat="server">
                                <asp:DataList ID="dtlAttatement" runat="server" RepeatDirection="Horizontal">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="linkbtn" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"pathfile") %>' OnClick="linkbtn_Click">LinkButton</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:DataList>
                                <asp:Panel ID="Panelattatement" runat="server">
                                </asp:Panel>
                            </div>
                            <div id="divcontenu" runat="server">
                            </div>
</asp:Panel>
                        </asp:View>

                         <asp:View ID="ViewSend" runat="server" OnActivate="ViewSend_Activate">
                            <asp:Button ID="btnactualiserINSend" runat="server" Text="Actualiser" OnClick="btnactualiserINSend_Click" /><asp:Button ID="btnsupprINSend" runat="server" Text="Suppr" OnClick="btnsupprINSend_Click" />
                            <br />                          
                            <asp:GridView ID="gvmailSend" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="mailID" OnPageIndexChanging="gvmailSend_PageIndexChanging" OnSelectedIndexChanged="gvmailSend_SelectedIndexChanged" PageSize="20" Width="1400px" OnRowDeleting="gvmailSend_RowDeleting" BorderStyle="None" BorderWidth="0px" OnRowDataBound="gvmailSend_RowDataBound" CellPadding="5" GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" />
                                        </ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailto">
                                   
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailsubject">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbodySimple">
                                    <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow:ellipsis;overflow:hidden" ></asp:Label>
                                        </ItemTemplate>
                                        <ControlStyle Width="800px" />
                                        <ItemStyle Width="800px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select"  Text="Detail" />
                                            <asp:Button ID="btnsuppr" runat="server" CommandName="Delete"  Text="Suppr" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>
                    </asp:MultiView>

                </asp:Panel>
            </div>


        </div>
    </form>
    <script type="text/javascript">
        function onSubmit() {
            var etat = '<%= getMultiviewEtat() %>';
             if (etat == '0') {
                 var editor = CKEDITOR.instances.tbxContent;
                 var editorHtml = document.getElementById("_editorHtml");
                 var editorText = document.getElementById("_editorText");
                 if (editor.getData().length!= 0) {
                     editorHtml.value = encodeURIComponent(editor.getData());
                 }
                 if (editor.getData().length!= 0) {
                     editorText.value = editor.document.getBody().getText();
                 }
                 editor.setData("");//清空ckeditor
             }
             return true;
         }
         //function doSubmit() {
         //    document.forms[0].submit();
         //}
    </script>  
   
</body>
</html>
