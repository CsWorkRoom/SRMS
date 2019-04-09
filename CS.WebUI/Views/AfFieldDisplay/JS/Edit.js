layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;

    $(function () {
        if ($("#IS_DEFAULT").val() == "1") {
            $("#lbInfo").text("内置字段，只允许编辑部分信息，不可编辑字段英文名及数据类型");
            $("#FIELD_DATA_TYPE").attr("disabled", "disabled");
            form.render('select')
        } else {
            $("#lbInfo").text("");
        }
        if (!$("#IS_SHOW").attr("checked")) {
            $("#SHOW_LENGTH").addClass("layui-disabled");
            $("#SHOW_WIDTH").addClass("layui-disabled");
        }
    });

    form.verify({
        reportId: function (values, item) {
            if (values == "") return "请选择模块对应报表！";
        },
        nurl: function (values, item) {
            if (values.trim() == "") return "请输入Url地址";
        }
    })
    //监听switch开关
    form.on('switch(IS_SHOW)', function (data) {
        if ((data.elem.checked == true)) {
            $("#SHOW_LENGTH").removeClass("layui-disabled");
            $("#SHOW_WIDTH").removeClass("layui-disabled");
            var value = dicTypeWith[$("#FIELD_DATA_TYPE").val()];
            if (value == undefined) return;
            $("#SHOW_WIDTH").val(value);
        } else {
            $("#SHOW_LENGTH").addClass("layui-disabled");
            $("#SHOW_WIDTH").addClass("layui-disabled");
        }
    });
    form.on('select(FIELD_DATA_TYPE)', function (data) {
        //判断是否IS_SHOW是否被选中
        if ($("#IS_SHOW").next().hasClass("layui-form-onswitch")) {
            var value = dicTypeWith[data.value];
            if (value == undefined) return;
            $("#SHOW_WIDTH").val(value);
        }
    });
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfFieldDisplay/Edit";
        SaveForm('form', url);
        return;
    });
}
