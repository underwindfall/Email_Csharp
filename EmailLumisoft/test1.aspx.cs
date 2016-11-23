﻿using System;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace GridFormView
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iEditIndex = 0;
            switch (e.CommandName)
            {
                case "Edit":
                    //编辑模式
                    iEditIndex = GetEditIndex((GridView)sender, ((GridViewRow)((Button)e.CommandSource).NamingContainer).RowIndex);
                    FormView1.PageIndex = iEditIndex;
                    FormView1.ChangeMode(FormViewMode.Edit);
                    //FormView 切换为编辑模式
                    FormView1.Visible = true;
                    //FormView 显示
                    GridView1.Visible = false;
                    //GridView 隐藏
                    break;
                case "Insert":
                    //新增模式
                    //因为只有使用 EditItemTemplate，故将 InsertItemTemplate 设为 EditItemTemplate
                    FormView1.InsertItemTemplate = FormView1.EditItemTemplate;
                    FormView1.ChangeMode(FormViewMode.Insert);
                    //FormView 切换为新增模式
                    FormView1.Visible = true;
                    //FormView 显示
                    GridView1.Visible = false;
                    //GridView 隐藏
                    break;
            }
        }

        /// <summary>
        /// 取得编辑列索引。
        /// </summary>
        /// <param name="GridView">GridView 控制项。</param>
        /// <param name="RowIndex">GridView 的资料列索引。</param>
        private int GetEditIndex(GridView GridView, int RowIndex)
        {
            int iEditIndex = 0;

            if (GridView.AllowPaging)
            {
                //GridView 有分页时，要把考虑目前的页数及每页笔数
                iEditIndex = (GridView.PageIndex) * GridView.PageSize + RowIndex;
            }
            else
            {
                //GridView 无分页时，直接使用 e.NewSelectedIndex
                iEditIndex = RowIndex;
            }
            return iEditIndex;
        }

        protected void FormView1_PreRender(object sender, EventArgs e)
        {
            FormView oFormView = default(FormView);
            LinkButton oLinkButton = default(LinkButton);
            TextBox oTextBox = default(TextBox);

            oFormView = (FormView)sender;
            if (!oFormView.Visible)
                return;
            switch (oFormView.CurrentMode)
            {
                case FormViewMode.Edit:
                    //编辑模式
                    //隐藏新增钮
                    oLinkButton = (LinkButton)oFormView.FindControl("InsertButton");
                    oLinkButton.Visible = false;
                    //显示更新钮
                    oLinkButton = (LinkButton)oFormView.FindControl("UpdateButton");
                    oLinkButton.Visible = true;
                    //显示 EmployeeID 的 TextBox
                    oTextBox = (TextBox)oFormView.FindControl("txtEmployeeID");
                    oTextBox.Visible = false;
                    break;
                case FormViewMode.Insert:
                    //显示新增钮
                    oLinkButton = (LinkButton)oFormView.FindControl("InsertButton");
                    oLinkButton.Visible = true;
                    //隐藏更新钮
                    oLinkButton = (LinkButton)oFormView.FindControl("UpdateButton");
                    oLinkButton.Visible = false;
                    //显示 EmployeeID 的 TextBox
                    oTextBox = (TextBox)oFormView.FindControl("txtEmployeeID");
                    oTextBox.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 切换为浏览模式。
        /// </summary>
        private void ChangeViewMode()
        {
            FormView1.Visible = false;
            GridView1.Visible = true;
            GridView1.EditIndex = -1;
        }

        protected void FormView1_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                //取消后，切换为浏览模式
                ChangeViewMode();
            }
        }

        protected void FormView1_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            //新增后，切换为浏览模式
            ChangeViewMode();
        }

        protected void FormView1_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            //更新后，切换为浏览模式
            ChangeViewMode();
        }
    }
}