var loading;

//转换URL（在URL前面加上虚拟目录）
function TransURL(url) {
    var u = url.toLowerCase().replace('../', '/');
    if (u.indexOf(applicationPath.toLowerCase()) + "/" != 0) {
        u = applicationPath + u;
    }
    return u;
}

//保存表单
function SaveForm(formid, url) {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        loading = layer.msg("数据保存中，请稍候……", { time: false, shade :0.3 });
        $.post(TransURL(url), $("#" + formid).serialize(), function (result) {
            layer.close(loading);//关闭保存中

            if (result.IsSuccess == true) {
                var msg = result.Message + "<br/> 刷新列表数据吗？";
                layer.confirm(msg, function (index) {
                    layer.closeAll();
                    parent.layer.closeAll();
                    parent.RefreshData();
                });
            } else {
                var msg = "保存失败<br/>" + result.Message;
                layer.alert(msg, { icon: 2 });
            }
        });
    });
}

//关闭表单
function CloseForm() {
    layui.use(['layer'], function () {
        var layer = layui.layer;
        //layer.closeAll();
        //当你在iframe页面关闭自身时
        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭   
        parent.layer.closeAll();
    });
}

//编辑窗体
layui.use(['jquery', 'layer'], function () {
    var $ = layui.jquery;
    $(function () {
        var $formTopContent = $(".form-top-content");
        var $formBottomContent = $(".form-bottom-submit");

        if ($formTopContent.length > 0 && $formBottomContent.length > 0) {
            var bottomHeight = $formBottomContent.height() + 10;
            $formTopContent.css({
                'max-height': $formTopContent.parent().is('form') ? '100%' : 'none',
                'margin-bottom': bottomHeight + 'px'
            });
            $formBottomContent.css({
                'position': 'fixed',
                'bottom': '0px',
                'background-color': 'white',
                'left': '0px',
                'right': '0px',
                'padding': '5px 20px 5px 20px',
                'z-index': 1000,
            });
        }
    });

});

//返回当前日期yyyy-MM-dd
function GetNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate;
    return currentdate;
}

//返回某月1号的日期
function GetFirstDateOfMonth(months) {
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth();

    month = month * 1 + months;
    date = new Date(year, month, 1);
    year = date.getFullYear();
    month = date.getMonth() + 1;
    if (month < 10) {
        return year + "-0" + month + "-01";
    }
    return year + "-" + month + "-01";
}

//返回某月最后一天的
function GetLastDateOfMonth(months) {
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth();

    month = month * 1 + months;
    date = new Date(year, month + 1, 1);
    date = new Date(date.valueOf() - 24 * 60 * 60 * 1000);
    year = date.getFullYear();
    month = date.getMonth() + 1;
    var day = date.getDate();
    var m = "-" + (month < 10 ? "0" : "") + month;
    var d = "-" + (day < 10 ? "0" : "") + day;
    return year + m + d;
}

//返回“上一月”的起止日期
function GetLastMonth(beginDateString) {
    var dates = new Array();
    var beginDate = new Date();
    var year = beginDate.getFullYear();
    var month = beginDate.getMonth();
    month = month * 1 + 1;
    if (beginDateString != "") {
        var array = beginDateString.split("-");
        if (array.length == 3) {
            year = array[0];
            month = array[1];
        }
    }
    //截止日期
    var endDate = new Date(year, month - 1, 1);
    endDate = new Date(endDate.valueOf() - 24 * 60 * 60 * 1000);

    if (month <= 1) {
        month = 12;
        year = year - 1;
    } else {
        month = month - 1;
    }

    dates[0] = year + "-" + (month < 10 ? "0" : "") + month + "-01";
    dates[1] = year + "-" + (month < 10 ? "0" : "") + month + "-" + endDate.getDate();

    return dates;
}

//返回“下一月”的起止日期
function GetNextMonth(beginDateString) {
    var dates = new Array();
    var beginDate = new Date();
    var year = beginDate.getFullYear();
    var month = beginDate.getMonth();
    month = month * 1 + 1;
    if (beginDateString != "") {
        var array = beginDateString.split("-");
        if (array.length == 3) {
            year = array[0];
            month = array[1] * 1;
        }
    }

    //截止日期
    var endDate = new Date(year, month + 1, 1);
    endDate = new Date(endDate.valueOf() - 24 * 60 * 60 * 1000);

    if (month >= 12) {
        month = 1;
        year = year * 1 + 1;
    } else {
        month = month + 1;
    }

    dates[0] = year + "-" + (month < 10 ? "0" : "") + month + "-01";
    dates[1] = year + "-" + (month < 10 ? "0" : "") + month + "-" + endDate.getDate();

    return dates;
}

//当前页弹出窗口
function OpenWindow(title, width, height, url) {
    var w = width;
    var h = height;
    if (w <= 0) {
        if (w < 0)
            w = window.innerWidth;
        else
            w = window.innerWidth - 50;
    }

    if (h <= 0) {
        if (h < 0)
            h = window.innerHeight;
        else
            h = window.innerHeight - 20;
    }

    layui.use('layer', function () {
        layui.layer.open({
            type: 2
            , title: title
            , maxmin: true
            , area: [w + 'px', h + 'px']
            , content: TransURL(url)
        });
    });
}

//顶级弹出窗口
function OpenTopWindow(title, width, height, url) {
    var w = width;
    var h = height;
    if (w <= 0) {
        if (w < 0)
            w = parent.window.innerWidth;
        else
            w = parent.window.innerWidth - 50;
    }

    if (h <= 0) {
        if (h < 0)
            h = parent.window.innerHeight;
        else
            h = parent.window.innerHeight - 20;
    }

    layui.use('layer', function () {
        try {
            top.RecordActiveIframeID();
            top.layer.open({
                type: 2
                , title: title
                , maxmin: true
                , area: [w + 'px', h + 'px']
                , content: TransURL(url)
            });
        } catch (err) {
            layer.open({
                type: 2
                , title: title
                , maxmin: true
                , area: [w + 'px', h + 'px']
                , content: TransURL(url)
            });
        }
    });
}

//普通提示框
function OpenPromptWindow(title, width, height, url) {
    layui.use('layer', function () {
        layui.layer.open({
            type: 2
            , title: title
            , area: [width + 'px', height + 'px']
            , content: TransURL(url)
        });
    });
}

//右下提示框
function OpenRDPromptWindow(title, width, height, url) {
    layui.use('layer', function () {
        layui.layer.open({
            type: 2
            , title: title
            , area: [width + 'px', height + 'px']
            , content: TransURL(url)
        });
    });
}

//框架新窗口
function OpenFrameWindow(title, url) {
    try {
        top.AddTabWindow(title, TransURL(url));
    } catch (err) {
        window.open(TransURL(url));
    }
}

//浏览器新窗口
function OpenBrowserWindow(url) {
    window.open(TransURL(url));
}

//AJAX异步GET请求
function AjaxGet(url) {
    layui.use(['jquery'], function () {
        var $ = layui.$;
        $.ajax({
            url: TransURL(url)
            , success: function () {
                return true;
            }
        });
    });
}

//AJAX异步POST请求
function AjaxPost(url) {
    layui.use('jquery', function () {
        var $ = layui.$;
        $.post(TransURL(url), function (result) {
            //console.log(result);
            //alert(result.Message.length);
            //debugger;wlf-6-28：新增提示框和刷新
            if (result.IsSuccess == true) {
                var msg = result.Message + " <br/>要刷新列表数据吗？";
                layer.confirm(msg, { area: ['900px', '500px'] }, function (index) {
                    //layer.closeAll();
                    //parent.layer.closeAll();
                    //parent.RefreshData();
                    //刷新当前页面
                    location.reload();

                });

            } else {
                layer.alert(result.Message, { area: ['900px', '500px'] });
            }
        });
    });
}

//当前页面跳转
function Redirect(url) {
    window.location.href = TransURL(url);
}

///得到URL地址的单个参数
///strKey：URL地址中的参数名称
var GetUrlParam = function (strKey) {
    var reg = new RegExp("(^|&)" + strKey + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

///解析URL参数为JSON集
///strUrl:URL地址
var GetParamJson = function (strUrl) {
    var strParam = "";
    var strJson = "";
    if (strUrl.indexOf("?") != -1) {
        strParam = strUrl.split("?")[1];
    }
    strParam = strParam.replace(new RegExp("&", "gm"), "','");
    var strJson = strParam.replace(new RegExp("=", "gm"), "':'");
    return strJson;
};

//当前页面跳转
function GetParamStr(url) {
    if (url.indexOf("?") != -1) {
        return url.split("?")[1];
    }
    return "";
}

//显示SQL帮助
function ShowSqlHelp() {
    layui.use(['layer'], function () {
        var $ = layui.jquery, layer = layui.layer;
        var content = "在SQL语句中，可以使用使用占位符的方式，来插入HTML输入框的值或者C#程序中的变量及方法，语法如下：<br/>";
        content += "一、插入URL地址参数的值<br/>";
        content += "    @(参数名) <br/>";
        content += "二、插入C#的系统变量<br/>";
        content += "    @{USER_ID} 当前登录用户ID <br/>";
        content += "    @{USER_NAME} 当前登录用户登录名 <br/>";
        content += "    @{DEPARTMENT_ID} 当前登录用户所属组织机构ID <br/>";
        content += "    @{DEPARTMENT_CODE} 当前登录用户所属组织机构编码 <br/>";
        content += "    @{DEPARTMENT_NAME} 当前登录用户所属组织机构名称 <br/>";
        content += "    @{DEPARTMENT_LEVEL} 当前登录用户所属组织机构层级 <br/>";
        content += "    @{ALLROLE} 当前登录用户所属权限 <br/>";
        content += "三、插入C#中定义的方法（日期函数）：<br/>";
        content += "    @{DATETIME} 返回完整的日期格式（日期时间） <br/>";
        content += "    @{DATE} 返回完整的日期格式（日期部份）<br/>";
        content += "    @{yyyy(n)} 返回年份，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymm(n)} 返回年月，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymmdd(n)} 返回年月日，参数n为整数，默认为0 <br/>";

        layer.open({
            title: 'SQL语句中可用参数'
            , content: content
            , btn: ['关闭']
            , moveType: 1
            , area: ['800px;', '400px;']
        });
    });
}

//显示SQL帮助
function AlertSqlHelp() {
    var content = "在SQL语句中，可以使用使用占位符的方式，来插入HTML输入框的值或者C#程序中的变量及方法，语法如下：\n";
    content += "一、插入URL地址参数的值 \n";
    content += "    @(参数名) \n";
    content += "二、插入C#的系统变量 \n";
    content += "    @{USER_ID} 当前登录用户ID \n";
    content += "    @{USER_NAME} 当前登录用户登录名 \n";
    content += "    @{DEPARTMENT_ID} 当前登录用户所属组织机构ID \n";
    content += "    @{DEPARTMENT_CODE} 当前登录用户所属组织机构编码 \n";
    content += "    @{DEPARTMENT_NAME} 当前登录用户所属组织机构名称 \n";
    content += "    @{DEPARTMENT_LEVEL} 当前登录用户所属组织机构层级 \n";
    content += "    @{ALLROLE} 当前登录用户所属权限 \n";
    content += "三、插入C#中定义的方法（日期函数）：\n";
    content += "    @{DATETIME} 返回完整的日期格式（日期时间） \n";
    content += "    @{DATE} 返回完整的日期格式（日期部份）\n";
    content += "    @{yyyy(n)} 返回年份，参数n为整数，默认为0 \n";
    content += "    @{yyyymm(n)} 返回年月，参数n为整数，默认为0 \n";
    content += "    @{yyyymmdd(n)} 返回年月日，参数n为整数，默认为0 \n";
    alert(content);
}


//以下用于LAYUI框架左右侧拖动缩放的JS
$(function () {
    //var oBox = $("#larryms_body"), oLeft = $("#larry_left"), oRight = $("#larry_right"), oLine = $("#line");
    //oLine.mousedown(function (e) {
    //    var disX = e.clientX;
    //    oLine.left = oLine.offset().left;
    //    $(document).mousemove(function (e) {
    //        var iT = oLine.left + (e.clientX - disX);
    //        oRight.css("width", (oBox.width() - iT) + "px");
    //        oRight.css("left", iT + "px");
    //        oLeft.css("width", iT + "px");
    //        $(".layui-side-scroll").css("width", iT);
    //        $(".larryms-nav-tree").css("width", iT);
    //        oLine.css("left", iT + "px");
    //        return false
    //    });

    //    var dv = document.getElementById("line");
    //    $(document).mouseup(function () {
    //        $(document).unbind("mousemove");
    //        $(document).unbind("mouseup");
    //        if (dv.releaseCapture)
    //            dv.releaseCapture();
    //        else if ($(document).releaseEvents) {
    //            $(document).releaseEvents(Event.MOUSEMOVE | Event.MOUSEUP);
    //        }
    //    });

    //    if (dv.setCapture)
    //        dv.setCapture();
    //    else if ($(document).captureEvents) {
    //        $(document).captureEvents(Event.MOUSEMOVE | Event.MOUSEUP);
    //    }
    //    return false;
    //});
});

//判断字符串是否为空的方法
function isEmpty(obj) {
    if (typeof obj == "undefined" || obj == null || obj == "" || obj == 'null') {
        return true;
    } else {
        return false;
    }
}
function IsEmpty(obj) {
    if (typeof obj == "undefined" || obj == null || obj == "" || obj == 'null') {
        return true;
    } else {
        return false;
    }
}