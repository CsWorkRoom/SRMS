//排序
var sort = { field: "ID", type: "ASC" };

layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') {
            OpenWindow("编辑信息", 0, 0, "../AfScriptType/Edit?id=" + data.ID);
        } else if (layEvent === 'del') {
            layer.confirm('真的删除行么', function (index) {
                layer.close(index);
                var url = "../AfScriptType/Delete?id=" + data.ID;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        RefreshData();
                    } else {
                        layer.alert(result.Message);
                    }
                });
            });
        }
    });
    //表格排序
    table.on('sort(tablefilter)', function (obj) {
        sort = obj;
        Query();
    });
});

//添加
function Add() {
    OpenWindow("添加信息", 0, 0, "../AfScriptType/Edit?id=0");
}

//查询
var pageindex = 1;
function Query() {
    pageindex = 1;
    RefreshData();
}

//刷新表格
function RefreshData() {
    layui.use(['table', 'jquery'], function () {
        var table = layui.table, $ = layui.jquery;
        //执行重载
        table.reload('datatable', {
            initSort: sort
            , where: {
                pid: $("#PID").val()
                , name: $("#NAME").val()
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

//导出
function ExportFile() {
    url = "../AfScriptType/ExportFile?pid=" + $("#PID").val() + "&name=" + $("#NAME").val();
    window.location = url;
}

$(function () {
    var zNodes = JSON.parse($("#ScriptSelect").val());
    $.comboztree("PID", { ztreenode: zNodes });
});