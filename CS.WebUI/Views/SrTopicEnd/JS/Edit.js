//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicEnd/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        selectGroup: function (value) {
            if (isEmpty(value)) {
                return '请选择课题成果状态！';
            }
        },
        topicVer: function (value) {
            if (isEmpty(value)||value=="0")
            {
                return '请选择课题！';
            }
            if (value.indexOf("type_") != -1) {
                return '请选择课题(勿选择课题类型)';
            }
        },
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

    var topicNodes = JSON.parse($("#TopicSelect").val());
    $.comboztree("TOPIC_ID", { ztreenode: topicNodes });
});


