//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicTask/Edit";
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

    //自定义验证规则
    form.verify({
        //datetimeVer: function (value) {
        //    if (isEmpty(value)) {
        //        return '请选择时间范围！';
        //    }
        //},
        nameVer: function (value) {
            if (isEmpty(value)) {
                return '请填写检查任务名！';
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
        if (isEmpty($("#beginEndData").val()))
        {
            layer.alert('请选择时间范围：开始结束时间！');
            return;
        }
        save();//保存
    });

    var topicNodes = JSON.parse($("#TopicSelect").val());
    $.comboztree("TOPIC_ID", { ztreenode: topicNodes });
});


