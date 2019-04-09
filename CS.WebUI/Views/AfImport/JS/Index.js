//排序
var sort = { field: "ID", type: "ASC" };

//表格事件
layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    //行事件
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') {
            OpenWindow("编辑外导", 0, 0, "../AfImport/Edit?id=" + data.ID);
        } else if (layEvent === 'enable') {
            var url = "../AfImport/SetEnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    RefreshData();
                } else {
                    layer.alert("启用失败 " + result.message);
                }
            });
        } else if (layEvent === 'disable') {
            var url = "../AfImport/SetUnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    RefreshData();
                } else {
                    layer.alert("停用失败 " + result.message);
                }
            });
        } else if (layEvent === 'del') {
            layer.confirm('真的删除行么', function (index) {
                layer.close(index);
                var url = "../AfImport/Delete?id=" + data.ID;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        RefreshData();
                    } else {
                        layer.alert("删除失败" + result.Message);
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
    OpenWindow("添加外导", 0, 0, "../AfImport/Edit?id=0");
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
                name: $("#NAME").val()
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
    url = "../AfImport/ExportFile?name=" + $("#NAME").val();
    window.location = url;
}