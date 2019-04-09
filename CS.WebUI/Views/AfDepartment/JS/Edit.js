$(function () {
    var zNodes = JSON.parse($("#DepartmentSelect").val());
    $.comboztree("P_CODE", {
        ztreenode: zNodes,
        onClick: function (event, treeId, treeNode) {
            console.log(treeId);
            console.log(treeNode);
        }
    });
});

layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery, layedit = layui.layedit, laydate = layui.laydate;
    return;
    form.verify({
        code: function (value) {
            if (value.length <= 0) {
                return '部门编号不能为空！';
            }
        },
        name: function (value) {
            if (value.length <= 0) {
                return '部门名称不能为空！';
            }
        }
    });
});


function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfDepartment/Edit";
        SaveForm('form', url);
        return;
    });
}

