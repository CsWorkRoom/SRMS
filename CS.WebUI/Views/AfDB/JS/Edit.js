layui.use(['form', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;

    //自定义验证规则
    form.verify({
        name: function (value) {
            if (value.length < 5) {
                return '登录名不能少于5个字母';
            }
        }
        , role: function (value) {
            if (value.length < 2) {
                return '必须至少选择一个角色';
            }
        }
        , pass: [/(.+){6,12}$/, '密码必须6到12位']
        , content: function (value) {
            layedit.sync(editIndex);
        }
    });
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfDB/Edit";
        SaveForm('form', url);
        return;
    });
}