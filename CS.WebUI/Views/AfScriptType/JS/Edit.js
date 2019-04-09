$(function () {
    var zNodes = JSON.parse($("#ScriptSelect").val());
    $.comboztree("PID", { ztreenode: zNodes });
});

layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery, layedit = layui.layedit, laydate = layui.laydate;
    //自定义验证规则
    form.verify({
        name: function (value) {
            if (value.length <= 0) {
                return '类别名称不能为空！';
            }
        }
    });


    //提交结果，刷新列表数据
    if ($("#msg").val() != "") {
        var msg = $("#msg").val() + " 刷新列表数据吗？";
        layer.confirm(msg, function (index) {
            layer.closeAll();
            parent.layer.closeAll();
            parent.RefreshData();
        });
    }
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfScriptType/Edit";
        SaveForm('form', url);
        return;
    });
}