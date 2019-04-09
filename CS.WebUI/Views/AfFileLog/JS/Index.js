//DIV尺寸
function Resize() {
    $("#divLeft").height($(window).height() - 20);
    $("#divRight").height($(window).height() - 10);
    $("#divFileList").height($("#divLeft").height() - 40);
}

layui.use(['laydate'], function () {
    var laydate = layui.laydate;
    //日期范围
    laydate.render({
        elem: '#daterange'
      , range: true
    });

    Resize();
    //尺寸变化
    $(window).resize(function () {
        Resize();
    });
    QueryFile();
});

//查询文件列表
function QueryFile() {
    layui.use(['jquery', 'code','form'], function () {
        var $ = layui.jquery;
        var daterange = $("#daterange").val();
        $.post("../AfFileLog/Index?daterange=" + daterange, function (data) {
            var html = "<ul>";
            $.each(data, function (index, file) {
                html += "<li><a href='#' onclick='ShowFile(\"" + file + "\");'>" + file + "</a></li>";
                if (index == 0) {
                    ShowFile(file);
                }
            });
            html += "</ul>";
            $("#divFileList").html(html);
        }, "json");
    });
}

//查询文件
function ShowFile(filename) {
    layui.use(['jquery', 'code'], function () {
        var $ = layui.jquery;
        $.post("../AfFileLog/Index?filename=" + filename, function (data) {
            $("#divFileContent").html(data);
            layui.code({
                title: '日志内容  ' + filename,
                encode: true,
                about: false
            });
        }, "text");
    });
}