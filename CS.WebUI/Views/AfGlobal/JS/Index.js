layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
    var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
    table.render({
        elem: '#tableConfig'
        , even: true
        , height: 'full-65'
        , cols: [[
          { field: 'Name', title: '配置项', width: 180 }
          , { field: 'ValueType', title: '值类型', width: 100 }
          , { field: 'Value', title: '配置值', edit: 'text', width: 180 }
          , { field: 'Remark', title: '备注', edit: 'text', minWidth: 120 }
          , { field: 'UpdateTime', title: '更新时间', width: 180 }
        ]]
        , data: configData
        , limit: configData.length
    });
});

function save() {
    var url = "../AfGlobal/Edit";
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        var json = JSON.stringify(configData);
        $.post("../AfGlobal/Edit", { configjson: json }, function (result) {
            if (result.IsSuccess == true) {
                layer.alert(result.Message, { icon: 1 });
            } else {
                layer.alert(result.Message, { icon: 2 });
            }
        });
    });
}