$(function ()
{
    $("#btnSearch").click(function ()
    {
        var phonenum = $("#key").val();
        if (!phonenum)
        {
            alert("请输入相关文件名称！");
            $("#key").focus();
            return;
        }
        var data = new DataList();
        data.GetList(phonenum);
    });
});
layui.use('laytpl', function ()
{
    var laytpl = layui.laytpl;

    $.post("../SrTopic/SearchUser", {
        keyword: $("#keyword").val()
    }, function (_r)
    {
        var data = { //数据
            "list": _r
        }
        var getTpl = searchItem.innerHTML
            , view = document.getElementById('searchDiv');
        laytpl(getTpl).render(data, function (html)
        {
            view.innerHTML = html;
        });
        if (_r === undefined || _r.length == 0)
        {
            $("#searchDiv").html("暂无匹配用户");
        }
    }, "json")


})
var DataList = function ()
{
    this.GetList = function (phone)
    {
        $(".content").show();
        layui.use('laytpl', function() {
            var laytpl = layui.laytpl;
            $.ajax({
                type: "POST",
                async: true,
                dataType: "json",
                data: { key: phone },
                url: "../SrSearch/DoSearch",
                beforeSend: function(XMLHttpRequest) {
                    $("#datalist").html(common.loading);
                },
                success: function(res) {
                    if (res.Type) {
                        var data = res.Files;
                        if (data.length == 0) {
                            $("#datalist").html(common.none);
                            return;
                        }
                        var data = { //数据
                            "list": res.Files
                        }
                        var getTpl = fileItem.innerHTML
                            , view = document.getElementById('datalist');
                        laytpl(getTpl).render(data, function (html)
                        {
                            view.innerHTML = html;
                        });
                        
                    } else {
                        $("#datalist").html(common.error);
                    }
                },
                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    $("#datalist").html(common.error);
                }
            });
        })
    }
}



//公告样式对象
var common = {
    none: "<p style='line-height: 14px; text-align:center;'>(╯﹏╰）暂无数据！</p>",
    error: "<p>(╯﹏╰）呜呜~~~数据加载失败了 ！</p>",
    loading: "<p  style='line-height: 14px; text-align:center;font-family:sans-serif;font-size:16px;font-weight:500; '>正在很努力加载，请稍候...</p>",
    isNull: function (val)
    {
        var result = val;
        if (!val)
        {
            result = "";
        }
        return result;
    },
    getday: function ()
    {
        var da = new Date();
        var month = common.time(da.getUTCMonth() + 1);
        var day = common.time(da.getUTCDate());
        return da.getUTCFullYear() + "-" + month + "-" + day;
    },
    //获取当前时间后5天的时间,若时间超过最后一天，则为最后一天
    get5daylater: function ()
    {
        var da = new Date();
        var year = da.getUTCFullYear();
        var month = common.time(da.getUTCMonth() + 1);
        var day = common.time(da.getUTCDate()) + 5;
        var daynew = new Date(year, month, 0);
        var lastdate = year + '-' + month + '-' + daynew.getDate();    //获取月份的最后一天
        if (day > daynew.getDate())
        {
            day = daynew.getDate();
        }
        return year + "-" + month + "-" + day;
    },
    getlastday: function ()
    {
        var da = new Date();
        var year = da.getUTCFullYear();
        var month = common.time(da.getUTCMonth() + 1);
        var day = new Date(year, month, 0);
        var lastdate = year + '-' + month + '-' + day.getDate();    //获取月份的最后一天
        return lastdate;
    },
    getlastdaybytime: function (value)
    {
        var da = new Date(value);
        var year = da.getUTCFullYear();
        var month = common.time(da.getUTCMonth() + 1);
        var day = new Date(year, month, 0);
        var lastdate = year + '-' + month + '-' + day.getDate();    //获取月份的最后一天
        return lastdate;
    },
    getlastmonth: function (value)
    {
        var da = new Date(value);
        var year = da.getUTCFullYear();
        var month = common.time(da.getUTCMonth());
        var lastdate = year + '-' + month;    //获取月份的最后一天
        return lastdate;
    },
    getfristday: function ()
    {
        var da = new Date();
        var year = da.getUTCFullYear();
        var month = common.time(da.getUTCMonth() + 1);
        var day = "01";
        return year + "-" + month + "-" + day;
    },
    nowtime: function ()
    {
        var date = new Date();
        var year = date.getFullYear();
        var mouth = (parseInt(date.getMonth()) + 1);
        var myday = (parseInt(date.getDate()));
        if (mouth <= 9)
        {
            mouth = "0" + mouth;
        }
        if (myday <= 9)
        {
            myday = "0" + myday;
        }
        return year + "-" + mouth + "-" + myday;
    }
}