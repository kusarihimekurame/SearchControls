using System;
using System.Collections.Generic;
using System.Text;

namespace SearchControls.Interface
{
    /// <summary>
    /// 自定义按钮群的方法用的接口
    /// </summary>
    public interface IButtonFlowLayoutPanelMethod
    {
        /// <summary>
        /// 按钮群
        /// </summary>
        ButtonFlowLayoutPanel ButtonFlowLayoutPanel { get; set; }
        /// <summary>
        /// 点击第一按钮
        /// </summary>
        void BtnFirst_Click();
        /// <summary>
        /// 点击向下按钮
        /// </summary>
        void BtnDown_Click();
        /// <summary>
        /// 点击向上按钮
        /// </summary>
        void BtnUp_Click();
        /// <summary>
        /// 点击最后按钮
        /// </summary>
        void BtnLast_Click();
        /// <summary>
        /// 点击添加按钮
        /// </summary>
        void BtnInsert_Click();
        /// <summary>
        /// 点击删除按钮
        /// </summary>
        void BtnDelete_Click();
        /// <summary>
        /// 点击提交按钮
        /// </summary>
        void BtnUpdate_Click();
        /// <summary>
        /// 点击查找按钮
        /// </summary>
        void BtnFound_Click();
        /// <summary>
        /// 点击Excel按钮
        /// </summary>
        void BtnExcel_Click();
        /// <summary>
        /// 点击Word按钮
        /// </summary>
        void BtnWord_Click();
        /// <summary>
        /// 点击撤销按钮
        /// </summary>
        void BtnCancel_Click();
        /// <summary>
        /// 点击退出按钮
        /// </summary>
        void BtnQuit_Click();
    }
}
