using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Export
{
    /// <summary>
    /// 定义进度更新的提供程序
    /// </summary>
    /// <typeparam name="T">进度更新值的类型。</typeparam>
    public class Progress<T> : IProgress<T>
    {
        /// <summary>
        /// 进度条发生变动时的事件
        /// </summary>
        /// <param name="sender"><see cref="Progress{T}"/></param>
        /// <param name="e">进度更新值的类型。</param>
        public delegate void ProgressChangedEventHandler(object sender, T e);
        /// <summary>
        /// 进度条发生变动时
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;
        /// <summary>
        /// 进度条发生变动时的执行的方法
        /// </summary>
        public Action<T> Handler;
        /// <summary>
        /// 报告进度更新。
        /// </summary>
        /// <param name="value">进度更新之后的值。</param>
        public void Report(T value)
        {
            try
            {
                ProgressChanged?.Invoke(this, value);
                Handler?.Invoke(value);
            }
            catch { }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public Progress()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handler">进度条发生变动时的执行的方法</param>
        public Progress(Action<T> handler) : this()
        {
            Handler = handler;
        }
    }
}
