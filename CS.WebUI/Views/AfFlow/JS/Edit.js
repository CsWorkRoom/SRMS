//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfFlow/Edit";
        //#region 节点及节点关联信息
        //#endregion

        $("#FlowNodes").val("[]");
        $("#FlowNodeJoins").val("[]");
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        mainTable: function (value) {
            if (value.length == "-1") {
                return '请选择主表';
            }
        }
    });

    //导出选项
    form.on('switch(switchUse)', function (data) {
        if (this.checked) {
            $("#IS_ENABLE").val(1);
            //layer.tips('流程将启用', data.othis)
        } else {
            $("#IS_ENABLE").val(0);
            //layer.tips('流程将停用', data.othis)
        }
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });
    //#region 流程类型
    var zNodes = JSON.parse($("#FlowTypes").val());
    $.comboztree("FLOW_TYPE_ID", { ztreenode: zNodes });
    //#endregion

    //label_fd.FieldArr = label_fd.GetFields();
    //label_fd.InitFieldArr = label_fd.FieldArr;//记录初始值
    //label_fd.fieldTableRender();//初始化字段表

    //label_rl.RelationArr = label_rl.GetRelations();
    //label_rl.relationTableRender();//初始化关联信息表

});

$(function () {
    ////#region 流程类型
    //var zNodes = JSON.parse($("#FlowTypes").val());
    //$.comboztree("FLOW_TYPE_ID", { ztreenode: zNodes });
    ////#endregion
});
//#endregion

var flow = {
    FlowNodeArr: [],//流程节点初始化
    FlowNodeJoinArr: [],//流程节点关系初始化

}


