
layui.use(['form', 'layer', 'laydate', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery;
    var msg = $.trim($("#msg").val());
    if (msg != "") {//出现异常停止显示其它信息
        $(".layui-form").html(msg);
        return;
    }

    if ($("#isShowSubmit") != null && $("#isShowSubmit").val() == "0") {
        form.on('submit(submit)', function (data) {//验证提交
            save();//保存
        });
        $("#submit").show();
    } else {
        $("#submit").hide();
    }

    //渲染控件
    $("input").each(function () {
        //日期的加载 
        if ($(this).attr("lay-verify") != null && $(this).attr("lay-verify") == "datetime") {
            laydate.render({
                elem: "#" + $(this).attr("id")//指定元素
                , type: 'datetime'
            });
        }
        //复选框的加载
        if ($(this).attr("type") == "checkbox" && $(this).attr("lay-filter") != null && $(this).attr("lay-filter") != "") {
            var layfilter = $(this).attr("lay-filter");
            form.on('checkbox(' + layfilter + ')', function (obj) {
                $("#" + layfilter).val(obj.elem.checked ? 1 : 0);
            });
        }
    });
    form.render();
});


//保存
function save() {
    layui.use(['form', 'layer', 'laydate', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery;
        var strParam = "";
        var strUrl = window.location.href;
        if (strUrl.indexOf("?") != -1) {
            strParam = "?" + strUrl.split("?")[1];
        }
        var url = "../AfForm/Template" + strParam;
        SaveForm('form', url);
        return;
    });
}