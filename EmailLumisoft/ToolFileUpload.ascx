<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolFileUpload.ascx.cs" Inherits="Email_Test.WebUserControl1" %>
<script src="scripte/jquery-1.7.1.js"></script>
<script src="scripte/jquery-1.7.1.min.js"></script>
<script type="text/javascript">
    function EquFileUpload_OnChange() {
        document.getElementById("<%=Btn_upFile.ClientID%>").click();
    }
</script>
<style>
    .over {
        background: #ffff00;
        box-shadow: 0 0 20px #999 inset;
    }

    .in {
        border-color: #000000;
        border-width: 5px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=drop_area.ClientID%>").children("p:eq(1)").hide();

        $(document.getElementById("<%=partright.ClientID%>")).on({
            dragleave: function (e) {  //mettre le ficher a l'autre endroit
                e.preventDefault();
                if ($("#<%=Panel1.ClientID%>").hasClass('over')) {
                    $("#<%=Panel1.ClientID%>").removeClass('over');
                };

            },
            drop: function (e) { // mettre le ficher a la derniere
                e.preventDefault();
            },

            dragenter: function (e) {  //mettre le ficher a ce tableau
                e.preventDefault();
                $("#<%=drop_area.ClientID%>").children("p:eq(0)").hide();
                $("#<%=drop_area.ClientID%>").children("p:eq(1)").show();
                if (!$("#<%=Panel1.ClientID%>").hasClass('over')) {
                    $("#<%=Panel1.ClientID%>").addClass('over');
                };
            },
            dragover: function (e) {
                e.preventDefault();
                if (!$("#<%=Panel1.ClientID%>").hasClass('over')) {
                    $("#<%=Panel1.ClientID%>").addClass('over');
                };
            }
        });

        $(document).on({
            dragleave: function (e) {  ////mettre le ficher a l'autre endroit
                e.preventDefault();
                if ($("#<%=Panel1.ClientID%>").hasClass('in')) {
                    $("#<%=Panel1.ClientID%>").removeClass('in');
                };
            },

            drop: function (e) { //mettre le ficher a la derniere
                e.preventDefault();
            },

            dragenter: function (e) {  // mettre le ficher a ce tableau
                e.preventDefault();
                $("#<%=drop_area.ClientID%>").children("p:eq(0)").hide();
                $("#<%=drop_area.ClientID%>").children("p:eq(1)").show();
                if (!$("#<%=Panel1.ClientID%>").hasClass('in')) {
                    $("#<%=Panel1.ClientID%>").addClass('in');
                };
            },
          


            //},
            dragover: function (e) {
                e.preventDefault();
            }
        });


        //teledeposer les fichers
        var box = document.getElementById("<%=Panel1.ClientID%>");
        box.addEventListener("drop", function (e) {
            e.preventDefault();
            var fileList = e.dataTransfer.files;
            xhr = new XMLHttpRequest();
            xhr.open("post", "Default.aspx", false);
            xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
          
            var fd = new FormData();

            for (var i = 0 ; i < fileList.length ; i++) {

                fd.append('file' + i, fileList[i]);
            }
            xhr.send(fd);
            document.getElementById("<%=Btn_upFile.ClientID%>").click();
        });

    })

</script>









<%--float: left--%>

<div>
    <table>
        <tr>
            <td>
                <asp:Button ID="Btn_anulle" runat="server" OnClick="Btn_anulle_Click" Style="position: relative; -webkit-border-radius: 15px; left: 0px;" Text="Suppr All" />
            </td>
            <td colspan="2" class="auto-style1">
                <div style="z-index: 1; cursor: pointer">
                <asp:Button ID="Btn_upFile" runat="server" Text="Joindre" Style="position: relative;top:18px; left: 0px; width: 70px; -webkit-border-radius: 15px;" OnClick="Btn_upFile_Click" />
                </div>
                <div style=" z-index: 2; cursor: pointer;">
                 <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" Style="border: solid 1px #999999; background-color: #ffffff; background-image: none; margin-top: 4px; width: 4px; z-index: 5; filter: alpha(opacity=0); opacity: 0; cursor: pointer; position: relative; top: -9px; height: 29px;" />
                </div>
            </td>
            <td colspan="10" >
                <div id="partright" runat="server" style=" height: 40px; vertical-align: middle; width: 1200px; top: 0px; left: 0px;height:70px">
                  <asp:Panel ID="Panel1" runat="server" BorderStyle="dotted" BorderColor="gray" BorderWidth="1px" Height="70px" Wrap="True" Style="-webkit-border-radius: 15px;">
                <div id="drop_area" runat="server">
                    <p id="lblbefore" style="text-align: center; color: gray; top: -44px; width: 100%">Remarque : Pour joindre un ou plusieurs fichier, il vous suffit de les faire glisser depuis leur emplacement de stockage                              </p>
                    <p id="lblafter" style="text-align: center; color: gray; top: -44px; width: 100%">glisser le fichier ici  </p>
                </div>
            </asp:Panel>

                </div>
            </td>
        </tr>
        </table>
</div>

    <br />
   
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:DataList ID="dtlImage" runat="server" CellSpacing="5" RepeatDirection="Horizontal" OnDeleteCommand="dtlImage_DeleteCommand" OnItemDataBound="dtlImage_ItemDataBound" RepeatColumns="5">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <div >
                            <asp:Image ID="ImageFile" runat="server" ImageUrl='<%#DataBinder.Eval(Container.DataItem,"iconpath") %>' Width="16" Height="16" />
                            <asp:Label ID="lblFile" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"iconName") %>'></asp:Label>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnSuppr" CommandName="Delete" runat="server" ImageUrl="image/delete.png" Width="12" Height="12" OnClick="imgBtnSuppr_Click" />
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </ContentTemplate>
        </asp:UpdatePanel>
