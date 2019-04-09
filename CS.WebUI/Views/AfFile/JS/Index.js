//表格事件
layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    //行事件
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') {
            OpenWindow("编辑附件", 0, 0, "../AfFile/Edit?id=" + data.ID);
        } else if (layEvent === 'enable') {
            var url = "../AfFile/SetEnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    RefreshData();
                } else {
                    layer.alert("启用失败 " + result.message);
                }
            });
        } else if (layEvent === 'disable') {
            var url = "../AfFile/SetUnable?id=" + data.ID;
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
                var url = "../AfFile/Delete?id=" + data.ID;
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
});

//添加
function Add() {
    OpenWindow("添加附件", 0, 0, "../AfFile/Edit?id=0");
}

//查询
var pageindex = 1;
function Query() {
    pageindex = 1;
    RefreshData();
}
//排序
var sort = { field: "ID", type: "ASC" };
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
    url = "Export?name=" + $("#NAME").val() + "&ip=" + $("#IP").val() + "&type" + $("#DB_TYPE").val();
    window.location = url;
}