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

/**
 * 创建设备对象
 * @param {any} dc
 * @param {any} categoryName
 * @param {any} adapterName
 * @param {any} deviceName
 * @param {any} deviceInfo
 */
function CreateDevice(
    dc,
    categoryName, adapterName,
    deviceName, deviceInfo) {
    var req = {
        CategoryName: categoryName,
        AdapterName: adapterName,
        DeviceName: deviceName,
        DeviceInfo: deviceInfo,
        NoUseCache: true
    };
    return dc.CreateDevice(JSON.stringify(req));
}

/**
 * 执行设备对象中的接口
 * @param {any} dc
 * @param {any} deviceID
 * @param {any} commandName
 * @param {any} paramter
 */
function DeviceCommand(dc, deviceID, commandName, paramter) {
    var paramters = [];
    var index = 0;
    for (var key in paramter) {
        var value = paramter[key];
        var obj = {};
        obj.Name = key;
        if (value == undefined) {
            value = null;
        }
        if (typeof (value) == "function") {
            obj.Value = parseInt(dc.CreateFunction(value));
            obj.CallBackType = 1;
        }
        else {
            obj.Value = value;
            obj.CallBackType = 0;
            obj.IsOut = true;
        }
        paramters[index] = obj;
        index++;
    }
    var req = {
        Handle: deviceID,
        CommandName: commandName,
        Paramters: paramters
    };
    var resStr = dc.DoCommand(JSON.stringify(req));
    if (resStr == null || resStr == undefined) {
        //如果调用失败则返回undefined
        return undefined;
    }
    //如果调用成功字符串转换成js对象
    return JSON.parse(resStr);
}


$(document).ready(function () {
    window._loadAssembly("Newtonsoft.Json.dll");
    window._loadAssembly("DeviceLib.dll");
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

