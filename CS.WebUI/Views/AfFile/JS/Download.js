layui.use(['jquery', 'layer', 'form', 'table'], function () {
    var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table;
    var data = new Array();
    if ($("#FILES").val().length > 1) {
        data = JSON.parse($("#FILES").val());
    }
    //console.log(data);
    //执行渲染
    table.render({
        elem: '#datatable'
        , even: true
        , cols: [[
          { field: '上传时间', title: '上传时间', width: 200, sort: true }
          , { field: '上传者', title: '上传者', width: 150 }
          , { field: '文件名', title: '文件名', minWidth: 100 }
          , { title: '操作', width: 120, align: 'center', toolbar: '#operation', fixed: 'right' }
        ]]
        , data: data
        , limit: data.length
    });

    //行事件
    table.on('tool(tablefilter)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        var fileName = encodeURI(data.原文件名);
        if (layEvent === 'down') {
            var url = "../AfFile/Download?id=" + $("#ID").val() + "&rowkey=" + $("#ROW_KEY").val() + "&yyyymmdd=" + $("#YYYYMMDD").val() + "&filename=" + fileName;
            window.location = url;
        } else if (layEvent === 'del') {
            layer.confirm('真的删除行么', function (index) {
                layer.close(index);
                var url = "../AfFile/DeleteFile?id=" + $("#ID").val() + "&rowkey=" + $("#ROW_KEY").val() + "&yyyymmdd=" + $("#YYYYMMDD").val() + "&filename=" + fileName;
                $.post(url, function (result) {
                    if (result.IsSuccess == true) {
                        obj.del();
                        layer.alert("删除成功");
                    } else {
                        layer.alert("删除失败<br/>" + result.Message);
                    }
                });
            });
        }
    });
});