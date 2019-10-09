

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrPatent/Edit";
        
        SaveForm('form', url);
        return;
    });
}
function SaveFlowForm() {
    var url = "../SrPatent/Edit";
    var data = $("#form").serialize();
    var achieName = $("#ACHIEVEMENTS_NAME").val();
    if (achieName.length == 0) {
        layer.alert("成果名称不能为空", { icon: 2 });
        return;
    }
    var patentName = $("#PATENT_NAME").val();
    if (patentName.length == 0) {
        layer.alert("专利号不能为空", { icon: 2 });
        return;
    }
    var periName = $("#PERIODICAL_NAME").val();
    if (periName.length == 0) {
        layer.alert("期刊名称不能为空", { icon: 2 });
        return;
    }

    var resData = "";
    $.ajax({
        url: url,
        type: "post",
        data: data,
        async: false,
        success: function (res) {
            resData = res;
        }
    });
    return resData;
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



//ztree树形结构
$(function () {
    var zNodes = JSON.parse($("#TypeSelect").val());
    $.comboztree("TYPE_ID", {
        ztreenode: zNodes,
        onClick: function (event, treeId, treeNode) {
        }
    });

    var subjects = JSON.parse($("#SubjectSelect").val());
    $.comboztree("SUBJECT_ID", {
        ztreenode: subjects,
        onClick: function (event, treeId, treeNode) {
        }
    });
});

