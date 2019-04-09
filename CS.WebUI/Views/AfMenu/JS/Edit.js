layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;
    form.verify({
        reportId: function (values, item) {
            if (values == "") return "请选择模块对应报表！";
        },
        nurl: function (values, item) {
            if (values.trim() == "") return "请输入Url地址";
        }
    })
    $(function () {
        updateReport($("#REPORT_TYPE"))
    })
    //监听下拉
    form.on('select(REPORT_TYPE)', function (data) {
        $("#URL").val();
        updateReport($(data.elem));
    });
    //监听下拉
    form.on('select(REPORT_ID)', function (data) {
        var c_url = $("#URL");
        //清空地址栏值
        c_url.val("");
        //
        c_url.val($(data.elem).find("option:selected")[0].getAttribute("data-url"));
    });

    //图标
    var iconhtml = "<ul>";
    iconhtml += "";
    $.each(iconNames, function (index, name) {
        iconhtml += "<li style='float:left;margin:10px;' onclick='SelectIcon(\"" + name + "\");'><i  class='layui-icon layui-icon-" + name + "' style='font-size:14px;color:#333;'></i></li>";
    });
    iconhtml += "</ul>";
    $("#divIconSelect").html(iconhtml);

    //更新报表
    function updateReport(target) {
        var url_taget = $("#URL"), url_two = $(".report_two");
        var report_taget = $("#REPORT_ID"), report_one = $(".report_one");

        url_two.hide();
        url_two.removeAttr("lay-verify")
        report_one.hide();
        report_one.removeAttr("lay-verify");

        if (target.val() == "0") {
            url_two.show();
            url_two.attr("lay-verify", "nurl");
            form.render();
        } else {
            report_one.show();
            report_one.attr("lay-verify", "reportId");
            AjaxGetReport(target.val());
        }

        //获取报表列表
        function AjaxGetReport(id) {
            $.get("../AfMenu/GetReport", {
                ReportType: id
            }, function (row) {
                var html = "<option value=''>请选择</option>";
                for (var i = 0; i < row.length; i++) {
                    if (report_taget.attr("value") == row[i].id) {
                        var h = ["<option value='", row[i].id, "' data-url='" + row[i].id + "', selected='selected'>", row[i].name, "</option>"]
                        html += h.join("");
                    } else {
                        var h = ["<option value='", row[i].id, "' data-url='" + row[i].url + "' >", row[i].name, "</option>"]
                        html += h.join("");
                    }
                }
                report_taget.html(html);
                form.render();
            }, "json")
        }
    }
});

//选择图标
function SelectIcon(name) {
    $("#ICON").val("layui-icon-" + name);
    $("#iconSample").removeClass();
    $("#iconButton").removeClass();
    if (name.length > 0) {
        $("#iconSample").addClass("layui-icon layui-icon-" + name);
    }
    $("#divIconSelect").hide();
    $("#iconButton").addClass("layui-icon layui-icon-zhankaida");
}

//展开/隐藏选择图标下拉框
function ShowHideSelectIconDiv() {
    layui.use(['jquery', 'layer'], function () {
        var $ = layui.$, layer = layui.layer;
        if ($("#iconButton").hasClass("layui-icon-less")) {
            $("#divIconSelect").slideUp("fast");
            $("#iconButton").removeClass();
            $("#iconButton").addClass("layui-icon layui-icon-zhankai3");
        } else {
            $("#divIconSelect").slideDown("fast");
            $("#iconButton").removeClass();
            $("#iconButton").addClass("layui-icon layui-icon-less");
        }
    });
}

$(function () {
    var zNodes = JSON.parse($("#dicMenus").val());
    $.comboztree("PID", {
        ztreenode: zNodes
    });
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfMenu/Edit";
        SaveForm('form', url);
        return;
    });
}