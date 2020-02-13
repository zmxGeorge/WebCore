function CreateControl(e) {
    var p = e;
    var top = 0;
    var left = 0;
    var width = e.clientWidth;
    var height = e.clientHeight;
    if (width <= 0 || height <= 0) {
        return undefineds;
    }
    while (p != null && p != undefined) {
        top += p.offsetTop;
        left += p.offsetLeft;
        p = p.offsetParent;
    }
    return _createControl(left, top, width, height);
}

/*
 * 这里的e为Jq对象
 */
function GetHandle(e) {
    var hID = $(e).attr("controlID");
    if (hID == null || hID == "" || hID == undefined) {
        return hID;
    }
    return parseInt(hID);
}


/*
 * 调用下载接口
 */
function DownLoadWithURL(url) {
    _downLoadURL(url);
}


$(document).ready(function () {
    /*
      * 所有属性download="true"的a标签都会开启下载器
    */
    $("a[download=\"true\"]").click(function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        DownLoadWithURL(url);
    });
    /*
     * 所有属性control="true"的元素，会在这个元素位置创建控件
     */
    $("[control=\"true\"]").each(function () {
        var e = $(this)[0];
        var handle = CreateControl(e);
        if (handle != undefined) {
            $(this).attr("controlID", handle);
        }
    });
});

