//字段
var fieldData = new Array();
var isContainsCreateUser = false;
var isContainsCreateTime = false;

layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
    
    //加载表
    getTableNameList();
    //加载字段
    getFieldsList();

    //选择数据库
    form.on('select(DB_ID)', function (data) {
        $("#FIELDS").val("");
        getTableNameList();
        getFieldsList();
    });
    //选择数据库表
    form.on('select(S_TABLE_NAME)', function (data) {
        $("#TABLE_NAME").val(data.value);
        $("#FIELDS").val("");
        getFieldsList();
    });

    //是否允许更新数据
    form.on('switch(switchCheckbox)', function (data) {
        if (this.checked) {
            $("#IS_ALLOW_UPDATE").val("1");
            layer.tips('导入时，违反“唯一约定”的记录将自动更新', data.othis)
        } else {
            $("#IS_ALLOW_UPDATE").val("0");
            layer.tips('导入时，违反“唯一约定”的记录作失败处理', data.othis)
        }
    });
});

//获取表名列表
function getTableNameList() {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        var dbid = $("#DB_ID").val();
        $.post("../AfDB/GetTableList?dbID=" + dbid + "&isImport=true", function (data) {
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
                $("#FIELDS").val("");
                fieldData = new Array();
            }
            $('#S_TABLE_NAME').html(html);
            form.render();
        })
    });
}

//获取字段列表
function getFieldsList() {
    layui.use(['element', 'table', 'form'], function () {
        var element = layui.element, table = layui.table, form = layui.form;
        fieldData = new Array();
        isContainsCreateUser = false;
        isContainsCreateTime = false;
        if ($("#FIELDS").val().length > 0) {
            fieldData = JSON.parse($("#FIELDS").val());
            //渲染表格
            renderTable();
        } else {
            var dbid = $("#DB_ID").val();
            var tableName = $("#TABLE_NAME").val();
            if (dbid >= 0 && tableName.length > 0) {
                $.post("../AfDB/GetFieldsList?dbID=" + dbid + "&tableName=" + tableName, function (data) {
                    $.each(data, function (index, obj) {
                        fieldData[index] = new Object();
                        fieldData[index].EN_NAME = obj.EN_NAME;
                        fieldData[index].CN_NAME = obj.CN_NAME;
                        fieldData[index].IS_AUTO_INCREMENT = obj.EN_NAME == "ID" ? 1 : 0;
                        fieldData[index].IS_IMPORT = obj.EN_NAME == "ID" ? 0 : 1;
                        fieldData[index].IS_UNIQUE = 0;
                        fieldData[index].COMMENT = obj.COMMENT;
                    });
                    //console.log(fieldData);
                    //渲染表格
                    renderTable();
                }, "json");
            }
        };

        //渲染表格
        function renderTable() {
            //检查必需字段
            $.each(fieldData, function (index, obj) {
                if (obj.EN_NAME == "IMPORT_UID") {
                    fieldData[index].IS_IMPORT = 0;
                    isContainsCreateUser = true;
                }
                else if (obj.EN_NAME == "IMPORT_TIME") {
                    fieldData[index].IS_IMPORT = 0;
                    isContainsCreateTime = true;
                }
            });
            //执行渲染
            table.render({
                elem: '#tableField'
                , even: true
                , cols: [[
                  { field: 'EN_NAME', title: '字段英文名', width: 180, sort: true }
                  , { field: 'CN_NAME', title: '字段中文名', edit: 'text', width: 150 }
                  , { field: 'IS_AUTO_INCREMENT', title: '是否自增长', templet: '#checkboxAuto', minWidth: 100 }
                  , { field: 'IS_IMPORT', title: '是否导入', templet: '#checkboxImport', minWidth: 100 }
                  , { field: 'IS_UNIQUE', title: '是否唯一', templet: '#checkboxUnique', minWidth: 100 }
                  , { field: 'COMMENT', title: '字段说明', edit: 'text', width: 200 }
                ]]
                , data: fieldData
                , limit: fieldData.length
            });

            //监听锁定操作
            form.on('checkbox(fltAuto)', function (obj) {
                $.each(fieldData, function (index, obj) {
                    fieldData[index].IS_AUTO_INCREMENT = 0;
                });
                fieldData[this.value].IS_AUTO_INCREMENT = obj.elem.checked ? 1 : 0;
                if (obj.elem.checked) {
                    fieldData[this.value].IS_IMPORT = 0;
                }
                renderTable();
            });
            form.on('checkbox(fltImport)', function (obj) {
                fieldData[this.value][this.name] = obj.elem.checked ? 1 : 0;
            });
            form.on('checkbox(fltUnique)', function (obj) {
                $.each(fieldData, function (index, obj) {
                    fieldData[index].IS_UNIQUE = 0;
                });
                fieldData[this.value].IS_UNIQUE = obj.elem.checked ? 1 : 0;
                renderTable();
                //layer.tips(this.value + ' ' + this.name + '：' + obj.elem.checked, obj.othis);
            });
        }
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
        if (fieldData == null || fieldData.length < 1) {
            layer.alert("无字段信息，请重新选择表");
            return;
        }
        $("#FIELDS").val(JSON.stringify(fieldData));
        if (isContainsCreateUser == false) {
            layer.alert("所选表未包含字段：IMPORT_UID，建议包含该字段，以便记录导入数据的用户");
            //layer.alert(tableFields.join(","));
            //return;
        }
        if (isContainsCreateTime == false) {
            layer.alert("所选表未包含字段：IMPORT_TIME，建议包含该字段，以便记录导入数据的时间");
            //return;
        }
        
        var url = "../AfImport/Edit";
        SaveForm('form', url);
        return;
    });
}