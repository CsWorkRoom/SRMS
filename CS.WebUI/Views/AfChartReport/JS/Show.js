window.onload = function () {
    analysisSql();
    ChartCodeClick();
    if ($.trim($("#topCode").html()) != "") {
        $("#topCode").append("<button class='layui-btn search_btn' style='margin-left: 10px;' onclick='analysisSql();ChartCodeClick()'><i class='layui-icon layui-icon-search'></i>查询</button>");
    }
}

//子页面处理
function ChartCodeClick() {
    var code = $("#CHART_CODE").val();
    $("#ifm").contents().find("#endCode").val(code);
    $("#ifm").contents().find("#SearchBtn").click();//触发CHART子页面的查询按钮
}

//获取图表的配置代码
function getChartCode() {
    $("#ifm").contents().find("#getEndCode").click();
    var ec = $("#ifm").contents().find("#endCode").val();
    if (ec != null && ec != "" && ec.length > 0) {
        $("#CHART_CODE").val(ec);
    }
}

//解析sql
function analysisSql() {
    ////////////////解析搜索区HTML表单控件并替换SQL////////////
    var sqlCode = Decrypt($("#SQL_CODE").val());
    var parm = sqlCode.match(/@\([\w]+\)/ig);
    if (parm != null) {
        for (var i = 0; i < parm.length; i++) {
            var item = parm[i].replace("@(", "").replace(")", "");
            if ($("#" + item) != null) {//对像是否存在
                var v = $("#" + item).val();
                if (v == null || $.trim(v) == "")
                    v = $("#" + item).text();
                sqlCode = sqlCode.replace(parm[i], v);
            }
        }
    }
    /////发送请求////
    var data = $.ajax({
        type: "post",
        data: { DB_ID: $("#DB_ID").val(), SQL_CODE: sqlCode },
        url: applicationPath + "/AfChartReport/GetDataBySql",
        async: false
    });
    if (data != null && data.responseText != null && data.responseText.length > 0) {
        $("#ifm").contents().find("#seachData").val(data.responseText);
        //var dataArr = $.parseJSON(data.responseText);
    }
    $("#ifm").contents().find("#childRun").click();//
}

$(function () {
    $("#ifm").height($("#divContent").height() - $("#topCode").height() - $("#botCode").height());
});

$(window).resize(function () {
    $("#ifm").height($("#divContent").height() - $("#topCode").height() - $("#botCode").height());
});