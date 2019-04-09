//排序
var sort = { field: "ID", type: "DESC" };

//查询
var pageindex = 1;
function Query() {
    pageindex = 1;
    RefreshData();
}

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

//刷新表格
function RefreshData() {
    layui.use(["table", "form", "jquery"], function () {
        var table = layui.table, form = layui.form, $ = layui.jquery;
        //执行重载
        table.reload('datatable', {
            initSort: sort
           , where: {
               nodeId: $.trim($("#nodeId").val()),
               taskId: $.trim($("#taskId").val()),
               message: $.trim($("#MESSAGE").val()),
               sql: $.trim($("#SQL").val()),
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

//查询源代码
function ShowCode() {
    layui.use(['layer', 'code'], function () {
        layui.layer.open({
            type: 1
            , title: '脚本代码'
            , maxmin: true
            , area: ['950px', '450px']
            , content: "<pre class='layui-code' lay-title=''>" + $("#code").val() + "</pre>"
        });
        layui.code({ about: false });
    });
}