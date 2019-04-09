//排序
var sort = { field: "ID", type: "ASC" };

layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') {
            OpenWindow("编辑角色", 0, 0, "../AfRole/Edit?id=" + data.ID);
        } else if (layEvent === 'enable') {
            var url = "../AfRole/SetEnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    RefreshData();
                } else {
                    layer.alert("启用失败 " + result.message);
                }
            });
        } else if (layEvent === 'disable') {
            var url = "../AfRole/SetUnable?id=" + data.ID;
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
                var url = "../AfRole/Delete?id=" + data.ID;
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
    OpenWindow("添加角色", 0, 0, "../AfRole/Edit?id=0");
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
    url = "../AfRole/ExportFile?name=" + $("#NAME").val();
    window.location = url;
}