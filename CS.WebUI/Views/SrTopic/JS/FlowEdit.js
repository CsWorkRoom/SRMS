﻿


layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;

    var subjectNodes = JSON.parse($("#SubjectSelect").val());
    $.comboztree("SUBJECT_ID", { ztreenode: subjectNodes });
});
