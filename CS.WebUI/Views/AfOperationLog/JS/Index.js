//排序
var sort = { field: "ID", type: "DESC" };

layui.use(["table", "form", "jquery", "laydate"], function () {
    var table = layui.table, laydate = layui.laydate, form = layui.form, $ = layui.jquery;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
    });
    //表格排序
    table.on('sort(tablefilter)', function (obj) {
        sort = obj;
        Query();
    });
    laydate.render({
        elem: '#daterange'
      , range: true
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
                content: $.trim($("#content").val()),
                daterange: $.trim($("#daterange").val()),
                userName: $.trim($("#userName").val()),
                orderByField: sort.field,
                orderByType: sort.type
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
    url = "../AfOperationLog/ExportFile?content=" + $.trim($("#CONTENT").val());
    window.location = url;
}