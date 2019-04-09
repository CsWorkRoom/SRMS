//排序
var sort = { field: "ID", type: "DESC" };

layui.use(["table", "form", "jquery"], function () {
    var table = layui.table, form = layui.form, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
    });
    //表格排序
    table.on('sort(tablefilter)', function (obj) {
        sort = obj;
        Query();
    });
});
$(function () {
    if ($("#TaskId").val() == null) {
        $(body).html("对不起！参数有误，页面访问失败，请关闭此页");
    }
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
                message: $.trim($("#MESSAGE").val())
                , TaskId: $.trim($("#TaskId").val())
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
    url = "../AfScriptTaskLog/ExportFile?message=" + $.trim($("#MESSAGE").val()) + "&TaskId=" + $.trim($("#TaskId").val());
    window.location = url;
}