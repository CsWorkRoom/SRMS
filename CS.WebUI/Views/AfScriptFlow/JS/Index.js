
//是否成功
var StIsEnable = new Array();
StIsEnable[0] = "未启用";
StIsEnable[1] = "启用";

//是否成功
var LastIsSuccess = new Array();
LastIsSuccess[0] = "否";
LastIsSuccess[1] = "是";

layui.use(["laydate", "table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery; laydate = layui.laydate;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        switch (obj.event) {
            case "start":
                onBtnStart(obj, laydate);
                break;
            case "stop":
                onBtnStop(obj);
                break;
            case "log":
                //OpenTopWindow("编辑信息", 1024, 540, "../AfScriptTask/look?id=" + data.LAST_TASK_ID);
                OpenTopWindow('日志信息', 0, 0, "../AfScriptFlow/Look?id=" + data.ID);
                break;
            case "edit"://编辑
                //OpenTopWindow("编辑信息", 1024, 540, "../AfScriptFlow/Edit?id=" + data.ID);
                OpenTopWindow('编辑信息', 0, 0, "../AfScriptFlow/Edit?id=" + data.ID);
                break;
            case "disable": //停用
                var url = "../AfScriptFlow/SetUnable?id=" + data.ID;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        RefreshData();
                    } else {
                        layer.alert("停用失败 " + result.Message);
                    }
                });
                break;
            case "enable": //启用
                var url = "../AfScriptFlow/SetEnable?id=" + data.ID;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        RefreshData();
                    } else {
                        layer.alert("启用失败 " + result.Message);
                    }
                });
                break;
        }
    });
    /**
    *  @@method  启动按钮方法
    *  @@param   obj  table.on('tool(tablefilter)')结果
    *  @@return  null
    */
    function onBtnStart(obj, laydate) {
        var data = obj.data;
        var html = "";
        html += "<div class='layui-form'>";

        html += "<div class='layui-form-item'>";
        html += "<label class='layui-form-label'>基准日期</label>";
        html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='sdate' placeholder='yyyy-MM-dd' /></div>";
        html += "<div class='layui-form-mid layui-word-aux'>脚本中日期函数以此为基准计算得到</div>";
        html += "</div>";

        html += "<div class='layui-form-item'>";
        html += "<label class='layui-form-label'>起始日期</label>";
        html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='bdate' placeholder='yyyy-MM-dd' /></div>";
        html += "<div class='layui-form-mid layui-word-aux'>替换脚本中参数值：@BEGIN_DATE</div>";
        html += "</div>";

        html += "<div class='layui-form-item'>";
        html += "<label class='layui-form-label'>截止日期</label>";
        html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='edate' placeholder='yyyy-MM-dd' /></div>";
        html += "<div class='layui-form-mid layui-word-aux'>替换脚本中参数值：@END_DATE</div>";
        html += "</div>";

        html += "<div class='layui-form-item'>";
        html += "<label class='layui-form-label'>参数值</label>";
        html += "<div class='layui-input-block'><textarea class='layui-textarea' id='param' placeholder='替换脚本中参数值：@PARAM，没有参数值则留空，多个参数值则逗号分隔，多组参数值之间分号分隔' /></div>";
        html += "</div>";

        html += "</div>";
        layer.open({
            type: 1
            , title: '启动任务'
            , area: ['750px', '430px']
            , offset: 'auto'
            , id: 'layerDemo'
            , content: html
            , btn: '启动'
            , btnAlign: 'c' //按钮居中
            , shade: 0 //不显示遮罩
            , yes: function () {
                var sdate = $("#sdate").val();
                var bdate = $("#bdate").val();
                var edate = $("#edate").val();
                var para = $("#param").val();
                if (sdate == null) {
                    layer.alert("请先选择基准日期,再启动！");
                    return;
                }
                if (bdate == null) {
                    layer.alert("请先选择起始日期,再启动！");
                    return;
                }
                if (edate == null) {
                    layer.alert("请先选择截止日期,再启动！");
                    return;
                }
                var url = "../AfScriptFlow/Start?id=" + data.ID;
                $.post(url, { sdate: sdate, bdate: bdate, edate: edate, para: para }, function (result) {
                    layer.alert(result.Message);
                });
                layer.closeAll();
            }
        });
        laydate.render({
            elem: '#sdate',
            isInitValue: true,
            value: new Date()
        });
        laydate.render({
            elem: '#bdate',
            isInitValue: true,
            value: new Date()
        });
        laydate.render({
            elem: '#edate',
            isInitValue: true,
            value: new Date()
        });
    }

    /**
    *  @@method  停止按钮方法
    *  @@param   obj  table.on('tool(tablefilter)')结果
    *  @@return  null
    */
    function onBtnStop(obj) {
        var data = obj.data;
        layer.confirm('是否确定停止？', function (index) {
            layer.close(index);
            var url = "../AfScriptFlow/Stop?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert(result.Message);
                }
            });
        });
    }
});
//添加
function Add() {
    OpenTopWindow("编辑信息", 0, 0, "../AfScriptFlow/Edit?id=0");
}
//刷新表格
function RefreshData() {
    layui.use(['table', 'jquery'], function () {
        var table = layui.table, $ = layui.jquery;

        //执行重载
        table.reload('datatable', {
            where: {
                name: $("#NAME").val(),
                runState: $("#RUN_STATE").val(),
                isEnable: $("#IS_ENABLE").val(),
                lastTaskIs: $("#LAST_TASK_IS").val(),
                self: $.trim($("#SHOW_SELF").val())
            }
        });
    });
}

//导出
function ExportFile() {
    var obj = {
        name: $("#NAME").val(),
        runState: $("#RUN_STATE").val(),
        isEnable: $("#IS_ENABLE").val(),
        lastTaskIs: $("#LAST_TASK_IS").val(),
        self: $.trim($("#SHOW_SELF").val())
    };
    //console.log($.param(obj));
    url = "../AfFlow/ExportFile?cron=" + $("#CRON").val() + "&name=" + $("#NAME").val() + "&self=" + $.trim($("#SHOW_SELF").val());
    window.location = url;
}
//查询
var pageindex = 1;
function Query() {
    pageindex = 1;
    RefreshData();
}