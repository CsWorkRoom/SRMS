//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicDetail/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        subjectMust: function (value) {
            if (Number(value) <= 0) {
                return '请选择学科！';
            }
        },
        deptMust: function (value) {
            if (Number(value) <= 0) {
                return '请选择所属单位！';
            }
        },
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

    //#region 三个下拉绑定
    var subjectNodes = JSON.parse($("#SubjectSelect").val());
    $.comboztree("SUBJECT_ID", { ztreenode: subjectNodes });

    var deptNodes = JSON.parse($("#DepartmentSelect").val());
    $.comboztree("DEPARTMENT_ID", { ztreenode: deptNodes });

    var acountTypeNodes = JSON.parse($("#AccountingTypeSelect").val());
    $.comboztree("ACCOUNTING_TYPE_ID", { ztreenode: acountTypeNodes });
    //#endregion
});

