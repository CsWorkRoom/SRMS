//排序
var sort = { field: "ID", type: "ASC" };

layui.use(["laydate", "form", "table", "jquery"], function () {
    var table = layui.table, form = layui.form, $ = layui.jquery;
    var laydate = layui.laydate;
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        switch (obj.event) {
            case "start"://启动
                onBtnStart(obj, laydate);
                break;
            case "stop"://停止
                layer.confirm('是否确定停止？', function (index) {
                    layer.close(index);
                    var url = "../AfScriptNode/Stop?id=" + data.ID;
                    $.post(url, function (result) {
                        if (result.IsSuccess == true) {
                            layer.alert(result.Message);
                            RefreshData();
                        } else {
                            layer.alert(result.Message);
                        }
                    });
                });
                break;
            case "look":
                OpenWindow("查看详细", 0, 0, "../AfScriptNode/look?id=" + data.ID);
                break;
            case "log":
                OpenWindow("查看任务日志", 0, 0, "../AfScriptTaskFlowNodeLog/Index?nodeId=" + data.ID + "&TaskId=" + data.LAST_TASK_ID);
                break;
            case "edit":
                OpenWindow("编辑信息", 0, 0, "../AfScriptNode/Edit?id=" + data.ID);
                break;
            case "del":
                layer.confirm('真的删除行么', function (index) {
                    layer.close(index);
                    var url = "../AfScriptNode/Delete?id=" + data.ID;
                    $.post(url, function (result) {
                        if (result.IsSuccess == true) {
                            RefreshData();
                        } else {
                            layer.alert("删除失败 " +result.Message);
                        }
                    });
                });
                break;
        }
    });
    //表格排序
    table.on('sort(tablefilter)', function (obj) {
        sort = obj;
        Query();
    });
    //  form.render();
});

function onBtnStart(obj, laydate) {
    var data = obj.data;
    var html = "";
    html += "<div class='layui-form'>";

    html += "<div class='layui-form-item'>";
    html += "<label class='layui-form-label'>基准日期</label>";
    html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='sdate' placeholder='yyyy-MM-dd' /></div>";
    html += "<div class='layui-form-mid layui-word-aux'>脚本中日期函数以此为基准计算得到</div>";
    html += "</div>";

    html += "<div class='layui-form-item'>";
    html += "<label class='layui-form-label'>起始日期</label>";
    html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='bdate' placeholder='yyyy-MM-dd' /></div>";
    html += "<div class='layui-form-mid layui-word-aux'>替换脚本中参数值：@BEGIN_DATE</div>";
    html += "</div>";

    html += "<div class='layui-form-item'>";
    html += "<label class='layui-form-label'>截止日期</label>";
    html += "<div class='layui-input-inline'><input type='text' class='layui-input' id='edate' placeholder='yyyy-MM-dd' /></div>";
    html += "<div class='layui-form-mid layui-word-aux'>替换脚本中参数值：@END_DATE</div>";
    html += "</div>";

    html += "<div class='layui-form-item'>";
    html += "<label class='layui-form-label'>参数值</label>";
    html += "<div class='layui-input-block'><textarea class='layui-textarea' id='param' placeholder='替换脚本中参数值：@PARAM，没有参数值则留空，多个参数值则逗号分隔，多组参数值之间分号分隔' /></div>";
    html += "</div>";

    html += "</div>";
    layer.open({
        type: 1
        , title: '启动任务'
        , area: ['750px', '430px']
        , offset: 'auto'
        , id: 'layerDemo'
        , content: html
        , btn: '启动'
        , btnAlign: 'c' //按钮居中
        , shade: 0 //不显示遮罩
        , yes: function () {
            var sdate = $("#sdate").val();
            var bdate = $("#bdate").val();
            var edate = $("#edate").val();
            var para = $("#param").val();
            if (sdate == null) {
                layer.alert("请先选择基准日期,再启动！");
                return;
            }
            if (bdate == null) {
                layer.alert("请先选择起始日期,再启动！");
                return;
            }
            if (edate == null) {
                layer.alert("请先选择截止日期,再启动！");
                return;
            }

            var url = "../AfScriptNode/Start?id=" + data.ID;
            $.post(url, { sdate: sdate, bdate: bdate, edate: edate, para: para }, function (result) {
                layer.alert(result.Message);
            });
            layer.closeAll();
        }
    });
    laydate.render({
        elem: '#sdate',
        isInitValue: true,
        value: new Date()
    });
    laydate.render({
        elem: '#bdate',
        isInitValue: true,
        value: new Date()
    });
    laydate.render({
        elem: '#edate',
        isInitValue: true,
        value: new Date()
    });
}

//添加
function Add() {
    OpenWindow("添加信息", 0, 0, "../AfScriptNode/Edit?id=0");
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
                typeId: $.trim($("#TYPE_ID").val())
                , name: $.trim($("#NAME").val())
                , dbId: $.trim($("#DB_ID").val())
                , self: $.trim($("#SHOW_SELF").val())
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
    var zNodes = JSON.parse($("#ScriptTypeSelect").val());
    $.comboztree("TYPE_ID", { ztreenode: zNodes });
});

//导出
function ExportFile() {
    url = "../AfScriptNode/ExportFile?typeId=" + $.trim($("#TYPE_ID").val()) + "&name=" + $.trim($("#NAME").val()) + "&dbId" + $.trim($("#DB_ID").val() + "&self=" + $.trim($("#SHOW_SELF").val()));
    window.location = url;
}

//备份
function Backups() {
    url = "../AfScriptNode/Backups";
    window.location = url;
}