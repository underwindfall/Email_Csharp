<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Email_Test.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<%@ Register Src="~/ToolFileUpload.ascx" TagPrefix="uc1" TagName="ToolFileUpload" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>


</head>

<body>
      <link href="css/Styles.css" rel="stylesheet" />  
    <script src="scripte/jquery-1.7.1.js"></script>
    <link href="css/StyleEmailMenu.css" rel="stylesheet" />
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
    <script type="text/javascript">
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
    </script>
    <form id="form1" runat="server" onsubmit="return onSubmit()">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"></asp:ScriptManager>
        <div id="headerEmail" style="border: 3px solid #fef8e6; height: 50px; width: 100%; -webkit-border-radius: 15px; -moz-border-radius: 15px;">
            <asp:Image ID="Image1" runat="server" Height="40px" ImageUrl="~/image/logo_sipcom - 复制.gif" Width="160px" />
            <asp:Label ID="Label2" runat="server" Text="Email" Style="position: relative; font-size: 30px; margin-left: 10px; top: -10px " BorderStyle="None"></asp:Label>
            <asp:TextBox ID="tbxRechercher" runat="server" AutoPostBack="True" OnTextChanged="tbxRechercher_TextChanged"></asp:TextBox>
            <asp:Button ID="btnrecherche" runat="server" Text="Rechercher" OnClick="btnrecherche_Click" CssClass="button_control" />
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
                <%--<div style="position: relative; display: table-cell;margin-left: 200px; height: 100%; top: 0px; left: 0px;width:100%">--%>
                <asp:Panel ID="viewMail" runat="server" Style="padding: 5px; height: 100%; border: 3px solid  #fef8e6; -webkit-border-radius: 15px;">

                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="1">
                        <asp:View ID="ViewEcrire" runat="server" OnActivate="ViewEcrire_Activate" OnDeactivate="ViewEcrire_Deactivate">
                            <div id="divmessage" style="visibility: hidden">
                                <asp:HiddenField ID="messagehid" runat="server" />
                                <asp:Button ID="btnHid" runat="server" Text="Button" OnClick="btnHid_Click" />
                            </div>

                            <div>
                                <asp:Label ID="lblNewmail" runat="server" Text="Nouveau Message"></asp:Label>
                                <asp:Button ID="btnEnvoyer" runat="server" Text="Envoyer" OnClick="btnEnvoyer_Click" CssClass="button_control"/>
                                <asp:Button ID="btnAnnuler" runat="server" Text="Annuler" OnClick="btnAnnuler_Click" CssClass="button_control"/>
                                <asp:Button ID="btnEnregistre" runat="server" Text="Enregistrer le brouillon" OnClick="btnEnregistre_Click" CssClass="button_control"/>

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
                            <%--  </ContentTemplate>
                            </asp:UpdatePanel>--%>
                        </asp:View>

                        <asp:View ID="ViewPrincipal" runat="server" OnActivate="ViewPrincipal_Activate">
                            <asp:Button ID="btnactualiser" runat="server" Text="Actualiser" OnClick="ViewPrincipal_Activate" CssClass="button_control"/><asp:Button ID="btnsuppr" runat="server" Text="Suppr" OnClick="btnsuppr_Click" CssClass="button_control" />
                            <br />
                        
                            <asp:GridView ID="gvViewEmail" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="mailID" OnPageIndexChanging="gvViewEmail_PageIndexChanging" OnSelectedIndexChanged="gvViewEmail_SelectedIndexChanged" PageSize="20" Width="1400px" OnRowDeleting="gvViewEmail_RowDeleting" BorderStyle="None" BorderWidth="0px" OnRowDataBound="gvViewEmail_RowDataBound" CellPadding="5" GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailsender" HeaderText="De">
                                        <ItemStyle Wrap="False" /> <ControlStyle Width="20px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailfrom" HeaderText="Sujet">
                                        <ItemStyle CssClass="hidden" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailsubject"  HeaderText="Horiares">
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
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow: ellipsis; overflow: hidden"></asp:Label></ItemTemplate>
                                        <ControlStyle Width="800px" />
                                        <ItemStyle Width="800px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Detail" CssClass="button_control"/><asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Suppr" CssClass="button_control"/></ItemTemplate>
                                   
                                   </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>

                        <asp:View ID="ViewDetail" runat="server" OnActivate="ViewDetail_Activate">
                            <asp:Panel ID="Panel1" runat="server" Style="height: 850px; border: 3px solid  #fef8e6; width: 1400px; -webkit-border-radius: 15px;">
                                <div id="divmenu">
                                    <asp:Button ID="btnreturnINDetail" runat="server" Text="Retour" OnClick="btnreturnINDetail_Click" CssClass="button_control"/>
                                    <asp:Button ID="btnSupprINDetail" runat="server" Text="Suppr" OnClick="btnSupprINDetail_Click" CssClass="button_control"/>
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
                             <asp:Button ID="btnactualiserINSend" runat="server" Text="Actualiser" CssClass="button_control" /><asp:Button ID="btnsupprINSend" runat="server" Text="Suppr" OnClick="btnsupprINSend_Click" CssClass="button_control" />
                            <br />
                           <%-- <table style="width:100%">
                                <tr>
                                    <th style="width:120px;">A</th>
                                     <th style="width:620px;">Sujet</th>
                                    <th style="width:650px;">Horiares</th>
                                    <th style="width:20px;"></th>
                                </tr>
                            </table>--%>
                            <asp:GridView ID="gvmailSend" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="mailID" OnPageIndexChanging="gvmailSend_PageIndexChanging" OnSelectedIndexChanged="gvmailSend_SelectedIndexChanged" PageSize="20" Width="1400px" OnRowDeleting="gvmailSend_RowDeleting" BorderStyle="None" BorderWidth="0px" OnRowDataBound="gvmailSend_RowDataBound" CellPadding="5" GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailto" HeaderText="A"></asp:BoundField>
                                    <asp:BoundField DataField="mailsubject" HeaderText="Sujet" >
                                        <%--<ItemStyle CssClass="hidden" />--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbodySimple" HeaderText="Horaires">
                                      <%--  <ItemStyle CssClass="hidden" />--%>
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow: ellipsis; overflow: hidden"></asp:Label></ItemTemplate>
                                        <ControlStyle Width="500px" />
                                        <ItemStyle Width="500px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Detail" CssClass="button_control"/><asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Suppr" CssClass="button_control"/></ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>

                        <asp:View ID="ViewCorbeille" runat="server" OnActivate="ViewCorbeille_Activate">
                            
                            <asp:GridView ID="gvmailCorbeille" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="mailID" OnPageIndexChanging="gvmailCorbeille_PageIndexChanging" OnSelectedIndexChanged="gvmailCorbeille_SelectedIndexChanged" PageSize="20" Width="1500px" OnRowDeleting="gvmailCorbeille_RowDeleting" BorderStyle="None" BorderWidth="0px" CellPadding="5" GridLines="None" OnRowDataBound="gvmailCorbeille_RowDataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailfrom" HeaderText="De"></asp:BoundField>
                                    <asp:BoundField DataField="mailsubject" HeaderText="Subjet">
                                   
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbodySimple" HeaderText="MailContenu">
                                       
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow: ellipsis; overflow: hidden"></asp:Label></ItemTemplate>
                                        <ControlStyle Width="500px" />
                                        <ItemStyle Width="500px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" HeaderText="Horaires" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Remettre" CssClass="button_control"/><asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Delete" CssClass="button_control"/></ItemTemplate>
                                    <ControlStyle Width="100px" />
                                        <ItemStyle Width="100px" Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>

                        <asp:View ID="ViewBrouillons" runat="server" OnActivate="ViewBrouillons_Activate">
                            <asp:GridView ID="gvmailBrouillons" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" Width="1500px"  BorderStyle="None" BorderWidth="0px" CellPadding="5" GridLines="None" OnSelectedIndexChanged="gvmailBrouillons_SelectedIndexChanged" DataKeyNames="mailNo" OnRowDeleting="gvmailBrouillons_RowDeleting" OnRowDataBound="gvmailBrouillons_RowDataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate>
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="mailto" HeaderText="A"></asp:BoundField>
                                    <asp:BoundField DataField="mailsubject" HeaderText="Subjet">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mailbody" HeaderText="MailContenu">                                       
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblmailSujBody" runat="server" Text="" Style="text-overflow: ellipsis; overflow: hidden"></asp:Label></ItemTemplate>
                                        <ControlStyle Width="500px" />
                                        <ItemStyle Width="500px" Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="maildateTime" HeaderText="Horaires" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Edit" CssClass="button_control"/><asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Delete" CssClass="button_control"/>
                                        </ItemTemplate>
                                         <ControlStyle Width="50px" />
                                        <ItemStyle Width="100px" Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings Mode="NumericFirstLast" />
                            </asp:GridView>
                        </asp:View>
                             <asp:View ID="ViewRechercher" runat="server" OnActivate="ViewRechercher_Activate" >
                                  <asp:Button ID="btnsupprcheck" runat="server" Text="Supprchecked" OnClick="btnsupprcheck_Click"  CssClass="button_control"/>
                                  <asp:DropDownList ID="ddlselection" runat="server" OnSelectedIndexChanged="ddlselection_SelectedIndexChanged" AutoPostBack="True">
                                      <asp:ListItem Value="0">Boîte à Envoyer </asp:ListItem>
                                      <asp:ListItem Value="1">Boîte Reçu </asp:ListItem>
                                      <asp:ListItem Value="2" Selected="True">Tout</asp:ListItem>
                                  </asp:DropDownList>
                               &nbsp; 
                                 DateDebut: 
                                 <asp:TextBox ID="tbRechercherDateDebut" runat="server" MaxLength="10" AutoPostBack="True" OnTextChanged="tbRechercherDateDebut_TextChanged" CausesValidation="true" ValidationGroup="ValidationRechercher" Width="80px">
                                 </asp:TextBox>
                                 <ajaxToolkit:CalendarExtender ID="tbRechercherDateDebut_CalendarExtender" runat="server" TargetControlID="tbRechercherDateDebut" Format="yyyy-MM-dd" />
                                 <asp:CompareValidator ID="cvdatedebut" runat="server" ErrorMessage="aaaa-mm-jj" ControlToValidate="tbRechercherDateDebut" Display="Dynamic" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationRechercher" ForeColor="Red">
                                 </asp:CompareValidator>
                                &nbsp; DateFin:
                                 <asp:TextBox ID="tbRechercherDateAu" runat="server" MaxLength="10" AutoPostBack="true" OnTextChanged="tbRechercherDateAu_TextChanged" CausesValidation="true" ValidationGroup="ValidationRechercher" Width="80px">
                                 </asp:TextBox>
                                 <ajaxToolkit:CalendarExtender ID="tbRechercherDateAu_CalendarExtender" runat="server" TargetControlID="tbRechercherDateAu" Format="yyyy-MM-dd" />
                                 <asp:CompareValidator ID="cvdateau" runat="server" ErrorMessage="aaaa-mm-jj" ControlToValidate="tbRechercherDateAu" Display="Dynamic" Operator="DataTypeCheck" Type="Date" ValidationGroup="ValidationRechercher" ForeColor="Red">
                                 </asp:CompareValidator>
                                 <br />
                                   <asp:GridView ID="gvmailRechercher" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" Width="1500px"  BorderStyle="None" BorderWidth="0px" CellPadding="5" GridLines="None" DataKeyNames="mailID" OnSelectedIndexChanged="gvmailrechercher_SelectedIndexChanged" OnRowDeleting="gvmailrechercher_RowDeleting" OnPageIndexChanging="gvmailRechercher_PageIndexChanging" OnRowDataBound="gvmailRechercher_RowDataBound" >
                                     <Columns>
                                       <asp:TemplateField>
                                           <ItemTemplate><asp:CheckBox ID="checkSelect" runat="server" /></ItemTemplate> 
                                           <ControlStyle Width="20px" />
                                       </asp:TemplateField>
                                       <asp:BoundField DataField="mailfrom" HeaderText="De" HeaderStyle-CssClass="Freezing" ></asp:BoundField>
                                       <asp:BoundField DataField="mailto" HeaderText="A" HeaderStyle-CssClass="Freezing"></asp:BoundField>
                                       <asp:BoundField DataField="mailsubject" HeaderText="Sujet" HeaderStyle-CssClass="Freezing"></asp:BoundField>
                                       <asp:BoundField DataField="maildateTime" HeaderText="Horaires" HeaderStyle-CssClass="Freezing"></asp:BoundField>
                                        <asp:BoundField DataField="mailbodySimple"><ItemStyle CssClass="hidden" /></asp:BoundField>
                                       
                                       <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnselect" runat="server" CommandName="Select" Text="Detail" CssClass="button_control" />
                                            <asp:Button ID="btnsuppr" runat="server" CommandName="Delete" Text="Suppr"  CssClass="button_control" />
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


</body>
</html>
