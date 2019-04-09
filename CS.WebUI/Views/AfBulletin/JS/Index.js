//排序
var sort = { field: "ID", type: "DESC" };

layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) { //注：tool是工具条事件名，tablefilter是table原始容器的属性 lay-filter="对应的值"
        var data = obj.data; //获得当前行数据
        var layEvent = obj.event; //获得 lay-event 对应的值（也可以是表头的 event 参数对应的值）
        //var tr = obj.tr; //获得当前行 tr 的DOM对象
        //console.log("当前操作：" + layEvent);
        if (layEvent === 'edit') {  //编辑
            OpenWindow("编辑公告", 0, 0, "../AfBulletin/Edit?id=" + data.ID);
        }
        else if (layEvent === 'show') { //查看
            OpenWindow("查看公告", 0, 0, "../AfBulletin/Show?id=" + data.ID);
        }
        else if (layEvent === 'enable') { //启用
            var url = "../AfBulletin/SetEnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("启用失败 " + result.Message);
                }
            });
        }
        else if (layEvent === 'disable') { //禁用
            var url = "../AfBulletin/SetUnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("停用失败 " + result.Message);
                }
            });
        }

        else if (layEvent === 'del') { //删除
            layer.confirm('确认删除', function (index) {
                //obj.del(); //删除对应行（tr）的DOM结构，并更新缓存
                layer.close(index);
                var url = "../AfBulletin/Delete?id=" + data.ID;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        layer.alert(result.Message);
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
    OpenWindow("添加公告", 0, 0, "../AfBulletin/Edit?id=0");
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
                name: $("#TITLE").val()
                , fullName: $("#SUMMARY").val()
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
    url = "../AfBulletin/ExportFile?title=" + $("#TITLE").val() + "&summary=" + $("#SUMMARY").val();
    window.location = url;
}