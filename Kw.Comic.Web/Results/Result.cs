﻿using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kw.Comic.Results
{
    /// <summary>
    /// 表示默认的结果
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly Result SucceedResult = new Result(true);
        /// <summary>
        /// 繁忙
        /// </summary>
        public static readonly Result BusyResult = new Result(KnowResultCodes.Busy, null, true);

        private readonly bool readOnly;
        private int code;
        private string msg;
        /// <summary>
        /// 初始化类型<see cref="Result"/>
        /// </summary>
        public Result()
        {
        }
        /// <summary>
        /// 从参数<paramref name="readOnly"/>创建类型<see cref="Result"/>
        /// </summary>
        /// <param name="readOnly">是否只读模式</param>
        public Result(bool readOnly)
        {
            this.readOnly = readOnly;
        }
        /// <summary>
        /// 从参数<see cref="code"/>,<see cref="msg"/>,<see cref="readOnly"/>创建类型<see cref="Result"/>
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="msg">消息</param>
        /// <param name="readOnly">是否只读</param>
        public Result(int code, string msg, bool readOnly)
        {
            this.code = code;
            this.msg = msg;
            this.readOnly = readOnly;
        }

        /// <summary>
        /// 获取或设置一个值，表示结果代码
        /// </summary>
        public int Code
        {
            get => code;
            set
            {
                ThrowIfReadOnly();
                code = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，表示附带的信息
        /// </summary>
        public string Msg
        {
            get => msg;
            set
            {
                ThrowIfReadOnly();
                msg = value;
            }
        }
        /// <summary>
        /// 获取一个值，表示是否成功
        /// </summary>
        public bool Succeed => Code == 0;
        /// <summary>
        /// 调用以确定当前类型是否只读
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly() => readOnly;
        /// <summary>
        /// 抛出，当是只读模式时
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfReadOnly()
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Readonly model can't be modify!");
            }
        }
    }
}
