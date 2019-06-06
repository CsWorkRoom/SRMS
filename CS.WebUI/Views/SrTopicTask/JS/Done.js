//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicTask/Done";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //日期范围
    laydate.render({
        elem: '#beginEndData'
        , range: true
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

    var topicNodes = JSON.parse($("#TopicSelect").val());
    $.comboztree("TASK_TOPIC_ID", { ztreenode: topicNodes });
});


