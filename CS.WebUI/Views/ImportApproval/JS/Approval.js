layui.use(['jquery', 'layer', 'form', 'table'], function () {
    var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table;

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var id = $("#ID").val();
        var level = $("#LEVEL").val();
        var isApproval = $("#IS_APPROVAL").val();
        var message = $("#MESSAGE").val();
        if (isApproval != 1 && message.length < 1) {
            layer.alert("请填写不通过原因");
            return;
        }
        var url = "../ImportApproval/Approval?id=" + id + "&level=" + level;
        SaveForm('form', url);
        return;
    });
}