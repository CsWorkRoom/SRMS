//排序
var sort = { field: "ID", type: "DESC" };

//是否成功
var Bit = new Array();
Bit[0] = "是";
Bit[1] = "否";
var Success = new Array();
Success[0] = "成功";
Success[1] = "失败";

layui.use(["layer", "table", "form", "layedit", "jquery", "laydate"], function () {
    var layer = layui.layer, table = layui.table, form = layui.form, layedit = layui.layedit, $ = layui.jquery, laydate = layui.laydate;

    laydate.render({
        elem: '#daterange'
      , range: true
    });

    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        switch (obj.event) {
            case "start"://开始
                layer.confirm('是否确定启动？', function (index) {
                    layer.close(index);
                    var url = "../AfScriptTask/Start?id=" + data.ID;
                    $.post(url, function (result) {
                        if (result.IsSuccess == true) {
                            RefreshData();
                        } else {
                            layer.alert(result.Message);
                        }
                    });
                });
                break;
            case "stop"://停止
                layer.confirm('是否确定停止？', function (index) {
                    layer.close(index);
                    var url = "../AfScriptTask/Stop?id=" + data.ID;
                    $.post(url, function (result) {
                        if (result.IsSuccess == true) {
                            RefreshData();
                        } else {
                            layer.alert(result.Message);
                        }
                    });
                });
                break;
            case "TaskLog":
                OpenTopWindow("运行情况", 0, 0, "../AfScriptTask/look?id=" + data.ID);
                break;
            case "TaskNodeLog":
                OpenTopWindow("运行情况", 0, 0, "../AfScriptTaskFlowNodeLog/index?TaskId=" + data.ID);
                break;
        }
    });
    //表格排序
    table.on('sort(tablefilter)', function (obj) {
        sort = obj;
        Query();
    });
});

//查询
var pageindex = 1;
function Query() {
    pageindex = 1;
    RefreshData();
}

//刷新表格
function RefreshData() {
    layui.use(['table', 'layedit', 'form', 'layer', 'jquery'], function () {
        var table = layui.table, layedit = layui.layedit, laydate = layui.form, layer = layui.layer, $ = layui.jquery;
        //执行重载
        table.reload('datatable', {
            initSort: sort
            , where: {
                daterange: $.trim($("#daterange").val())
                , flowName: $.trim($("#FLOW_ID").val())
                , nodeName: $.trim($("#NODE_ID").val())
                , param: $.trim($("#PARAMETER").val())
                , statusId: $.trim($("#RUN_STATUS").val())
                , successId: $.trim($("#IS_SUCCESS").val())
                , orderByField: sort.field
                , orderByType: sort.type
            }
            , page: {
                curr: pageindex
            }
            , done: function (res, curr, count) {
                pageindex = curr;
            }
        });
    });
}

$(function () {
    if ($.trim($("#StatusSelect").val()) != "") {
        var zNodes = JSON.parse($("#StatusSelect").val());
        $.comboztree("RUN_STATUS", { ztreenode: zNodes });
    }
});

//导出
function ExportFile() {
    url = "../AfScriptTask/ExportFile?daterange=" + $.trim($("#daterange").val())
        + "&flowName=" + $.trim($("#FLOW_ID").val()) + "&nodeName=" + $.trim($("#NODE_ID").val())
        + "&param=" + $.trim($("#PARAMETER").val())
        + "&statusId" + $.trim($("#RUN_STATUS").val()) + "&successId" + $.trim($("#IS_SUCCESS").val());
    window.location = url;
}