
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopic/TopicScore";
        //获取锁配置的评审规则
        SaveForm('form', url);
        return;
    });
}


function target_clicked(el,userid,topicid,expertname) {
    var ds = $("#tab-expert").find(".border_blue");
    ds.each(function () {
        $(this).removeClass("border_blue");
        $(this).attr("goals_clicked", "no");
    });

    $(el).addClass("border_blue");
    $(el).attr("goals_clicked", "yes");
    var url = $("#scorediv");
    layer.open({
        type: 1,
        shade: false,
        offset: ['5%', '20%'],
        // shade: [0.1, '#fff'],
        area: ['800px', '400px'],
        title: '评分明细', //不显示标题
        content: url //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响

    });
    $("#lblExpertName").html(expertname);
    BindScoreDetail(userid, topicid);
    
}
//绑定评分明细
function BindScoreDetail(userid, topicid) {
    var index = layer.load(1, {
        shade: [0.1, '#fff'] //0.1透明度的白色背景
    });
    layui.use(['laytpl', 'form'], function () {
        $.post("../SrTopic/GetTopicExpertDetailByTopicandUser",
            {
                topicId: topicid,
                userid: userid
            },
            function (_r) {
                var data = { //数据
                    "list": _r
                }
                console.log(_r);
                var getTpl = scoreTemplate.innerHTML, view = $("#subItemBody");
                view.html("");
                laytpl(getTpl).render(data,
                    function (html) {
                        view.append(html);
                    });
                if (_r === undefined || _r.length == 0) {
                    $("#subItemBody").html("暂未设置评分项");
                }
                layer.close(index);
            },
            "json");
    })
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

