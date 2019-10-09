
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopic/ExpertScore";
        var nodes = getExpertScore();
        $("#ScoreItems").val(nodes);
        if (nodes != "[]" && nodes != "null") {
            SaveForm('form', url);
            return;
        } else if (items == "[]") {
            layer.alert("必须设置评分值", { icon: 2 });
        }
       
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
        if (score.val() == 0 || score.val() == null) {
            layer.alert("单项得分不能为空", { icon: 2 });
            result = null;
            return;
        }
        if (!/^\d+|\d+\.\d{1,2}$/gi.test(score.val())) {
            layer.alert("单项得分只能是数字类型", { icon: 2 });
            result = null;
            return;
        }
        if (parseFloat(score.val())>100) {
            layer.alert("单项得分不能超过100分", { icon: 2 });
            result = null;
            return;
        }
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

