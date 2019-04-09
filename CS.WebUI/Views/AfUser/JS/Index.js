//排序
var sort = { field: "ID", type: "ASC" };

//部门
$(function () {
    var zNodes = JSON.parse($("#DepartmentSelect").val());
    $.comboztree("DEPT_CODE", { ztreenode: zNodes });
});

layui.use(["table", "jquery"], function () {
    var table = layui.table, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) { //注：tool是工具条事件名，tablefilter是table原始容器的属性 lay-filter="对应的值"
        var data = obj.data; //获得当前行数据
        var layEvent = obj.event; //获得 lay-event 对应的值（也可以是表头的 event 参数对应的值）
        //var tr = obj.tr; //获得当前行 tr 的DOM对象
        //console.log("当前操作：" + layEvent);
        if (layEvent === 'edit') {  //编辑
            OpenWindow("编辑用户", 0, 0, "../AfUser/Edit?id=" + data.ID);
        }
        else if (layEvent === 'enable') { //启用
            var url = "../AfUser/SetEnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("启用失败 " + result.Message);
                }
            });
        }
        else if (layEvent === 'locked') { //解锁
            var url = "../AfUser/Unlock?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("解锁失败" + result.Message);
                }
            });
        }
        else if (layEvent === 'disable') { //禁用
            var url = "../AfUser/SetUnable?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("停用失败 " + result.Message);
                }
            });
        }
        else if (layEvent === 'reset') { //重置密码
            var url = "../AfUser/ResetPassword?id=" + data.ID;
            $.post(url, function (result) {
                if (result.IsSuccess == true) {
                    layer.alert(result.Message);
                    RefreshData();
                } else {
                    layer.alert("重置密码失败 " + result.Message);
                }
            });
        }
        else if (layEvent === 'del') { //删除
            layer.confirm('确认删除', function (index) {
                //obj.del(); //删除对应行（tr）的DOM结构，并更新缓存
                layer.close(index);
                var url = "../AfUser/Delete?id=" + data.ID;
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
    OpenWindow("添加用户", 0, 0, "../AfUser/Edit?id=0");
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
                deptCode: $("#DEPT_CODE").val()
                , name: $("#NAME").val()
                , fullName: $("#FULL_NAME").val()
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
    url = "../AfUser/ExportFile?name=" + $("#NAME").val() + "&fullName=" + $("#FULL_NAME").val();
    window.location = url;
}

//重置全部
function ResetAll() {
    var url = "../AfUser/ResetAll";
    $.post(url, function (result) {
        if (result.IsSuccess == true) {
            layer.alert(result.Message);
            RefreshData();
        } else {
            layer.alert("重置密码失败 " + result.Message);
        }
    });
}