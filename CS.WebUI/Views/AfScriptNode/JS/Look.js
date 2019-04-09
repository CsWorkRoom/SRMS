//是否成功
var Task = new Array();
Task[0] = "失败";
Task[1] = "成功";

$(function () {
    $("#CREATE_UID").val(UserName[parseInt($("#CREATE_UID").val())]);
    $("#UPDATE_UID").val(UserName[parseInt($("#UPDATE_UID").val())]);
    $("#RUN_STATUS").val(Status[parseInt($("#RUN_STATUS").val())]);
    $("#LAST_TASK_IS").val(Task[parseInt($("#LAST_TASK_IS").val())]);

    var zNodes = JSON.parse($("#ScriptTypeSelect").val());
    $.comboztree("TYPE_ID", { ztreenode: zNodes });
    zNodes = JSON.parse($("#DatabaseSelect").val());
    $.comboztree("DB_ID", { ztreenode: zNodes });
    $("input").attr("disabled", "disabled");//禁用所有
    $("textarea").attr("readonly", "readonly");
});

layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery, layedit = layui.layedit, laydate = layui.laydate;
});