
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopic/ExpertScore";
        var nodes = getExpertScore();
        $("#ScoreItems").val(nodes);
        SaveForm('form', url);
        return;
    });
}

function getExpertScore() {  
    var result = [];
    var trs = $("#subitemTable tbody").find('tr');
    trs.each(function (index) {
        var topicSubItem = $('#topicSubItemId' + index);
        var subItem = $('#subItemId' + index);
        var weight = $('#weight' + index);
        var score = $('#txtScore' + index);
        var remark = $('#txtRemark' + index);
        var node = {
            ID: 0,
            TOPIC_ID: 0,
            TOPIC_SUB_ITEM_ID: topicSubItem.val(),
            SUB_ITEM_ID: subItem.val(),
            WEIGHT: weight.val(),
            SCORE: score.val(),
            REMARK: remark.val()
        };
        result.push(node);
    });
    return JSON.stringify(result);
}

layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;
    var remark = layedit.build('REMARK');
    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });


});

