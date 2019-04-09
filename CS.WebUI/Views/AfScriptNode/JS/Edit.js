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
});

layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery, layedit = layui.layedit, laydate = layui.laydate;
    //自定义验证规则
    form.verify({
        name: function (value) {
            if ($.trim(value).length <= 0) {
                return '节点名称不能为空！';
            }
        },
        typeId: function (value) {
            if ($.trim(value).length <= 0) {
                return '类型不能为空！';
            }
        },
        dbId: function (value) {
            if ($.trim(value).length <= 0) {
                return '数据库不能为空！';
            }
        },
        content: function (value) {
            if ($.trim(value).length <= 0) {
                return '数据库不能为空！';
            }
        }

    });
});

//保存
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfScriptNode/Edit";
        SaveForm('form', url);
        return;
    });
}