using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebCore.Miniblink
{
    /// <summary>
    /// HTML文档加载完成时发生
    /// </summary>
    /// <param name="view"></param>
    public delegate void OnDocumentReady(WebView view);

    public delegate void OnDownLoad(WebView view,string url);

    public delegate void OnTitleChanged(WebView view,string title);

    public enum NavigationType
    {
        WKE_NAVIGATION_TYPE_LINKCLICK,
        WKE_NAVIGATION_TYPE_FORMSUBMITTE,
        WKE_NAVIGATION_TYPE_BACKFORWARD,
        WKE_NAVIGATION_TYPE_RELOAD,
        WKE_NAVIGATION_TYPE_FORMRESUBMITT,
        WKE_NAVIGATION_TYPE_OTHER
    }

    /// <summary>
    /// 发生任意跳转时
    /// </summary>
    /// <param name="view"></param>
    /// <param name="navigationType"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public delegate bool OnNavigation(WebView view,NavigationType navigationType,string url);

    public enum PopMessageBoxType
    {
        Alert,
        Confirm,
        Prompt
    }

    /// <summary>
    /// 提示框弹出时发生
    /// </summary>
    /// <param name="view"></param>
    /// <param name="popMessageType"></param>
    /// <param name="message"></param>
    public delegate void OnPopMessageBox(WebView view, PopMessageBoxType popMessageType, string message);

    public enum MessageType
    {
        WKE_MESSAGE_TYPE_LOG,
        WKE_MESSAGE_TYPE_DIR,
        WKE_MESSAGE_TYPE_DIR_XML,
        WKE_MESSAGE_TYPE_TRACE,
        WKE_MESSAGE_TYPE_START_GROUP,
        WKE_MESSAGE_TYPE_START_GROUP_COLLAPSED,
        WKE_MESSAGE_TYPE_END_GROUP,
        WKE_MESSAGE_TYPE_ASSERT
    }

    public enum MessageSource
    {
        WKE_MESSAGE_SOURCE_HTML,
        WKE_MESSAGE_SOURCE_XML,
        WKE_MESSAGE_SOURCE_JS,
        WKE_MESSAGE_SOURCE_NETWORK,
        WKE_MESSAGE_SOURCE_CONSOLE_API,
        WKE_MESSAGE_SOURCE_OTHER
    }

    public enum MessageLevel
    {
        WKE_MESSAGE_LEVEL_TIP,
        WKE_MESSAGE_LEVEL_LOG,
        WKE_MESSAGE_LEVEL_WARNING,
        WKE_MESSAGE_LEVEL_ERROR,
        WKE_MESSAGE_LEVEL_DEBUG
    }

    /// <summary>
    /// 控制台输出消息时发生
    /// </summary>
    /// <param name="view"></param>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public delegate void OnConselMessage(WebView view,
        string message,
        string sourceName,
        int lineNumber,
        string stackTrace
        );

    public enum UrlLoadResult
    {
        /// <summary>
        /// 加载成功
        /// </summary>
        Successed,
        /// <summary>
        /// 取消加载
        /// </summary>
        Failed,
        /// <summary>
        /// 加载失败
        /// </summary>
        Canceled
    }

    /// <summary>
    /// 加载完成时发生
    /// </summary>
    /// <param name="view"></param>
    /// <param name="message"></param>
    public delegate void OnLoadingComplete(WebView view, string url,string reason, UrlLoadResult result);
}
