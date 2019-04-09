$(function () {
    document.oncontextmenu = function () {//禁用浏览器自带菜单
        return false;
    }
    $(document).bind("click mousewheel", function () {
        $(".menuDiv").hide();
    });

    //抢盘    
    $("#qiangpan").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#qiangpanMenu").css("left", e.pageX + "px");
            $("#qiangpanMenu").css("top", e.pageY + "px");
            $("#qiangpanMenu").show();
        }
    });

    //计件
    $("#jijian").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#jijianMenu").css("left", e.pageX + "px");
            $("#jijianMenu").css("top", e.pageY + "px");
            $("#jijianMenu").show();
        }
    });
    //指标
    $("#zhibiao").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#zhibiaoMenu").css("left", e.pageX + "px");
            $("#zhibiaoMenu").css("top", e.pageY + "px");
            $("#zhibiaoMenu").show();
        }
    });

    //分享
    $("#fenxiang").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#fenxiangMenu").css("left", e.pageX + "px");
            $("#fenxiangMenu").css("top", e.pageY + "px");
            $("#fenxiangMenu").show();
        }
    });

    //过滤
    $("#guolu").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#guoluMenu").css("left", e.pageX + "px");
            $("#guoluMenu").css("top", e.pageY + "px");
            $("#guoluMenu").show();
        }
    });

    //标签
    $("#biaoqian").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#biaoqianMenu").css("left", e.pageX + "px");
            $("#biaoqianMenu").css("top", e.pageY + "px");
            $("#biaoqianMenu").show();
        }
    });

    //任务
    $("#renwu").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#renwuMenu").css("left", e.pageX + "px");
            $("#renwuMenu").css("top", e.pageY + "px");
            $("#renwuMenu").show();
        }
    });

    //外导
    $("#waidao").mousedown(function (e) {
        if (3 == e.which) {//鼠标右击
            $(".menuDiv").hide();
            $("#waidaoMenu").css("left", e.pageX + "px");
            $("#waidaoMenu").css("top", e.pageY + "px");
            $("#waidaoMenu").show();
        }
    });
});


//添加TAB
function AddTab(title, url) {
    debugger;
    var showHtml = "";
    switch (title) {//检测是否为帮助
        case "抢盘帮助":
            showHtml = "抢盘帮助,抢盘帮助";
            break;
        case "计件帮助":
            showHtml = "计件帮助,计件帮助,计件帮助";
            break;
        case "指标帮助":
            showHtml = "指标帮助，指标帮助，指标帮助";
            break;
        case "获取分享帮助":
            showHtml = "获取分享帮助，获取分享帮助，获取分享帮助";
            break;
        case "过滤帮助":
            showHtml = "过滤帮助，过滤帮助，过滤帮助";
            break;
        case "标签帮助":
            showHtml = "标签帮助，标签帮助，标签帮助";
            break;
        case "任务帮助":
            showHtml = "任务帮助，任务帮助，任务帮助";
            break;
        case "外导帮助":
            showHtml = "外导帮助，外导帮助，外导帮助";
            break;
    }
    if (showHtml != null && $.trim(showHtml) != "") {//弹出式帮助框
        layui.use('layer', function () { //独立版的layer无需执行这一句
            var $ = layui.jquery, layer = layui.layer; //独立版的layer无需执行这一句
            layer.open({
                type: 1,
                title: '<i class="layui-icon layui-icon-903caidan_bangzhu" ></i> ' + title,
                offset: "auto",
                id: 'layerAlet', //防止重复弹出
                content: '<div style="padding: 20px 20px;">' + showHtml + '</div>',
                btn: '关闭',
                btnAlign: 'c', //按钮居中
                shade: 0, //不显示遮罩
                yes: function () {
                    layer.closeAll();
                }
            });

        });
        return;
    }
    OpenFrameWindow(title, url);//添加TAB
}