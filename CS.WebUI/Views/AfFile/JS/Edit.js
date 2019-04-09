//字段
var keyField = "";

layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;

    //加载表
    getTableNameList();
    //加载字段
    getFieldsList();

    //选择数据库
    form.on('select(DB_ID)', function (data) {
        $("#KEY_FIELD").html("");
        getTableNameList();
        getFieldsList();
    });
    //选择数据库表
    form.on('select(S_TABLE_NAME)', function (data) {
        $("#TABLE_NAME").val(data.value);
        $("#KEY_FIELD").html("");
        getFieldsList();
    });

    //是否允许更新数据
    form.on('switch(switchCheckbox)', function (data) {
        if (this.checked) {
            $("#IS_ALLOW_DELETE").val("1");
            layer.tips('允许删除自己上传的附件', data.othis)
        } else {
            $("#IS_ALLOW_DELETE").val("0");
            layer.tips('不允许任何人删除附件', data.othis)
        }
    });
});

//获取表名列表
function getTableNameList() {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        var dbid = $("#DB_ID").val();
        $.post("../AfDB/GetTableList?dbID=" + dbid + "&isImport=false", function (data) {
            var html = "";
            if (data.length > 0) {
                var tables = JSON.parse(data);
                html += "<option value=''>请选择表</option>";
                for (var i = 0; i < tables.length; i++) {
                    if ($('#TABLE_NAME').val() == [tables[i]]) {
                        selectTableName = tables[i];
                        html += "<option value='" + tables[i] + "' selected='selected'>" + tables[i] + "</option>";
                    } else {
                        html += "<option value='" + tables[i] + "'>" + tables[i] + "</option>";
                    }
                }
            } else {
                $("#TABLE_NAME").val("");
            }
            $('#S_TABLE_NAME').html(html);
            $("#KEY_FIELD").html("");
            form.render();
        })
    });
}

//获取字段列表
function getFieldsList() {
    layui.use(['element', 'table', 'form'], function () {
        var element = layui.element, table = layui.table, form = layui.form;
        if ($("#KEY_FIELD").html().length == 0) {
            var dbid = $("#DB_ID").val();
            var tableName = $("#TABLE_NAME").val();
            if (dbid >= 0 && tableName.length > 0) {
                $.post("../AfDB/GetFieldsList?dbID=" + dbid + "&tableName=" + tableName, function (data) {
                    var html = "";
                    html += "<option value=''>请选择主键（必须）</option>";
                    $.each(data, function (index, obj) {
                        html += "<option value='" + obj.EN_NAME + "'>" + obj.CN_NAME + "</option>";
                    });
                    $("#KEY_FIELD").html(html);
                    $("#KEY_FIELD").val($("#H_KEY_FIELD").val());
                    form.render();
                }, "json");
            }
        };
    });
}

//保存
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        if ($('#NAME').val().length < 1) {
            layer.alert("请输入名称");
            return;
        }
        if ($('#DB_ID').val() < 0) {
            layer.alert("请选择数据库");
            return;
        }
        if ($('#TABLE_NAME').val().length < 1) {
            layer.alert("请选择表，进行本操作前请先在数据库创建表");
            return;
        }
        keyField = $("#KEY_FIELD").val();
        if (keyField == null || keyField.length < 1) {
            layer.alert("请选择主键字段，作为记录的唯一标识");
            return;
        }

        var url = "../AfFile/Edit";
        SaveForm('form', url);
        return;
    });
}