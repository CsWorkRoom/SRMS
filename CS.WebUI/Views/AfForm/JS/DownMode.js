
layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
    var index = GetUrlParam("index");
    var ParentWindObj = GetParentWindObj();//得到父级页面对像
    var TableRowData = ParentWindObj.fieldData[index];
    var inputType = GetUrlParam("inputtype");//文本类型
    if (inputType == $("#treexl").val()) {
        $("#Enumjson").attr("placeholder", "请输入下拉选项，输入方式：每行一个选项（包含值和显示名称及父级值），值和显示名称之间用逗号分隔，如：\n A,类型,P\n B,类型,P \n C,类型,P \n C,类型,P");
        $("#sqlText").attr("placeholder", "请输入SQL语句，SQL字段别名值为V，名为k，父级为P。如：\n SELECT A V,B K,C P FROM TABLE");
        $("#pidDiv").show();
    } else {
        $("#pidDiv").hide();
    }

    ///加载数据
    //加载枚举值
    var enumData = TableRowData.SELECT_ENUM_OPTIONS;
    GetEnumList(enumData);
    //加载表查询配置
    var selectSourceData = TableRowData.SELECT_QUERY_INFO;
    if (selectSourceData != null) {
        var dbId = selectSourceData.DB_ID;
        var tableName = selectSourceData.TABLE_NAME;//表名
        var valueField = selectSourceData.VALUE_FIELD;//值
        var nameField = selectSourceData.NAME_FIELD;//名
        var pidField = selectSourceData.PID_FIELD;//父ID

        var where = selectSourceData.WHERE;
        $("#DB_ID").val(dbId);
        GetTableData(tableName, valueField, nameField, pidField);//加载数据库表
        $("#WHERE").val(where);
    }
    //加载SQL配置
    var sqlInfo = TableRowData.SQL;
    if (sqlInfo != null && sqlInfo.length > 0 && sqlInfo.split('◎').length >= 2) {
        var info = sqlInfo.split('◎');
        if (info[0] != null) {
            $("#SQL_DBID").val(info[0]);
        }
        if (info[1] != null) {
            $("#sqlText").val(info[1]);
        }
    }

    var selectType = TableRowData.SELECT_INPUT_TYPE;//数据来源类型
    switch (selectType) {
        case 1://加载枚举字段
            element.tabChange("selectTab", "t1");
            $("#t1").attr("checked", true);
            break;
        case 2://加载表查询配置
            element.tabChange("selectTab", "t2");
            $("#t2").attr("checked", true);
            break;
        case 3://加载表查询配置
            element.tabChange("selectTab", "t3");
            $("#t3").attr("checked", true);
            break;
        default:
            element.tabChange("selectTab", "t1");
            $("#t1").attr("checked", true);
            break;
    }
    //同步选项卡
    if ($("#t1").is(":checked")) {
        element.tabChange("selectTab", "t1");
    }
    else if ($("#t2").is(":checked")) {
        element.tabChange("selectTab", "t2");
    } else {
        element.tabChange("selectTab", "t3");
    }
    //选择数据库表
    form.on('select(DB_ID)', function (data) {
        GetTableData(null, null, null);//动态生成表列表
    });

    form.on('select(TABLE_NAME)', function (data) {
        GetFields(null, null);//动态生成表字段列表
    });
    form.render();
});

//获取数据库表信息
function GetTableData(tableName, valueField, nameField, pidField) {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        var dbId = $("#DB_ID").val();
        $.post("../AfDB/GetTableList?dbID=" + dbId + "&isImport=false", function (data) {
            var html = "";
            if (data != null) {
                var tables = JSON.parse(data);
                html += "<option value=''>请选择表</option>";
                for (var i = 0; i < tables.length; i++) {
                    if (tableName != null && tableName == tables[i]) {
                        html += "<option value='" + tables[i] + "' selected='selected'>" + tables[i] + "</option>";
                    } else {
                        html += "<option value='" + tables[i] + "'>" + tables[i] + "</option>";
                    }
                }
                $('#TABLE_NAME').html(html);
                form.render();
                GetFields(valueField, nameField, pidField);//生成字段
            }
        });
    });
}

//得到表字段列表
function GetFields(valueField, nameField, pidField) {
    var dbId = $("#DB_ID").val();
    var tableName = $('#TABLE_NAME').val();
    if (dbId >= 0 && tableName.length > 0) {
        var Vhtml = "<option value=''>请选择字段</option>";
        var Nhtml = "<option value=''>请选择字段</option>";
        var Phtml = "<option value=''>请选择字段</option>";
        layui.use(['form', 'element', 'layer', 'jquery'], function () {
            var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
            $.post("../AfDB/GetFieldsList?dbID=" + dbId + "&tableName=" + tableName, function (Field) {
                $.each(Field, function () {
                    if (valueField != null && valueField == this.EN_NAME) {
                        Vhtml += "<option value='" + this.EN_NAME + "' selected='selected'>" + this.EN_NAME + "</option>";
                    } else {
                        Vhtml += "<option value='" + this.EN_NAME + "'>" + this.EN_NAME + "</option>";
                    }

                    if (nameField != null && nameField == this.EN_NAME) {
                        Nhtml += "<option value='" + this.EN_NAME + "' selected='selected'>" + this.EN_NAME + "</option>";
                    } else {
                        Nhtml += "<option value='" + this.EN_NAME + "'>" + this.EN_NAME + "</option>";
                    }

                    if (pidField != null && pidField == this.EN_NAME) {
                        Phtml += "<option value='" + this.EN_NAME + "' selected='selected'>" + this.EN_NAME + "</option>";
                    } else {
                        Phtml += "<option value='" + this.EN_NAME + "'>" + this.EN_NAME + "</option>";
                    }
                });
                $("#VALUE_FIELD").html(Vhtml);
                $("#NAME_FIELD").html(Nhtml);
                $("#PID_FIELD").html(Phtml);
                form.render();
            }, "json");
        });
    }
}

//加载枚举列表
function GetEnumList(enumData) {
    if (enumData == null || enumData.length <= 0)
        return;
    var html = "";
    for (var i = 0; i < enumData.length; i++) {
        var inputType = GetUrlParam("inputtype");//文本类型
        html += enumData[i].VALUE + "," + enumData[i].NAME;
        if (inputType == $("#treexl").val()) {
            html += "," + enumData[i].PID;
        }
        html += "\n";
    }
    $("#Enumjson").html(html);
}

//获取上级页面对像
function GetParentWindObj() {
    var returnObj = null;
    for (var i = 0; i < window.parent.length; i++) {
        if (window.parent[i].pageKeyTile == "表单编辑页") {//查找父级页面识别
            returnObj = window.parent[i];
            break;
        }
    }
    return returnObj;
}

//保存
function save() {
    var ParentWindObj = GetParentWindObj();//得到父级页面对像
    var index = GetUrlParam("index");
    var TableRowData = ParentWindObj.fieldData[index];

    if ($("#t1").is(":checked")) {
        ModeT1(ParentWindObj, index, TableRowData)
    } else if ($("#t2").is(":checked")) {
        ModeT2(ParentWindObj, index, TableRowData)
    } if ($("#t3").is(":checked")) {
        ModeT3(ParentWindObj, index, TableRowData)
    }
}

///枚举模式
function ModeT1(ParentWindObj, index, TableRowData) {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.jquery;
        TableRowData.SELECT_INPUT_TYPE = 1;
        var EnumData = $.trim($("#Enumjson").val());
        if (EnumData.length <= 0) {
            layer.alert("请先填写枚举配置！");
            return;
        }
        var json = [];
        var fieldData = EnumData.replace(/，/g, ",").split("\n");//过虑中文符号
        for (var i = 0; i < fieldData.length; i++) {
            var info = {};
            var field = fieldData[i].split(",");
            info.VALUE = field[0];
            info.NAME = field[1];
            if (field.length >= 3) {
                info.PID = field[2];
            }
            json.push(info);
        }
        TableRowData.SELECT_ENUM_OPTIONS = json;
        layer.open({
            type: 1
            , id: 'closeFrom' //防止重复弹出
            , content: '<div style="padding: 20px 100px;">枚举配置，保存成功！</div>'
            , btn: '关闭'
            , btnAlign: 'c' //按钮居中
            , shade: 0 //不显示遮罩
            , yes: function () {
                layer.closeAll();
            }
        });
    });
}

///查询模式
function ModeT2(ParentWindObj, index, TableRowData) {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.jquery;
        //表查询配置
        if ($('#DB_ID').val().length <= 0) {
            layer.alert("请选择数据库！");
            return;
        }
        if ($('#TABLE_NAME').val().length < 1) {
            layer.alert("请选择数据库表！");
            return;
        }
        if ($('#VALUE_FIELD').val().length < 1) {
            layer.alert("请选择表值字段！");
            return;
        }

        if ($('#NAME_FIELD').val().length < 1) {
            layer.alert("请选择表名称字段！");
            return;
        }
        TableRowData.SELECT_INPUT_TYPE = 2;
        //准备数据
        var info = {};
        info.DB_ID = $.trim($('#DB_ID').val());
        info.TABLE_NAME = $.trim($('#TABLE_NAME').val());
        info.VALUE_FIELD = $.trim($('#VALUE_FIELD').val());
        info.NAME_FIELD = $.trim($('#NAME_FIELD').val());
        info.PID_FIELD = $.trim($('#PID_FIELD').val());
        info.WHERE = $.trim($('#WHERE').val());
        TableRowData.SELECT_QUERY_INFO = info;

        layer.open({
            type: 1
                , id: 'closeFrom' //防止重复弹出
                , content: '<div style="padding: 20px 100px;">表查询配置，保存成功！</div>'
                , btn: '关闭'
                , btnAlign: 'c' //按钮居中
                , shade: 0 //不显示遮罩
                , yes: function () {
                    layer.closeAll();
                }
        });
    });
}

///SQL模式
function ModeT3(ParentWindObj, index, TableRowData) {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.jquery;
        TableRowData.SELECT_INPUT_TYPE = 3;
        var DBID = $.trim($("#SQL_DBID").val());
        if (DBID.length <= 0) {
            layer.alert("请先填写数据库！");
            return;
        }
        var sql = $.trim($("#sqlText").val());
        if (sql.length <= 0) {
            layer.alert("请先填写SQL语句");
            return;
        }
        TableRowData.SQL = DBID + "◎" + sql;
        layer.open({
            type: 1
            , id: 'closeFrom' //防止重复弹出
            , content: '<div style="padding: 20px 100px;">枚举配置，保存成功！</div>'
            , btn: '关闭'
            , btnAlign: 'c' //按钮居中
            , shade: 0 //不显示遮罩
            , yes: function () {
                layer.closeAll();
            }
        });
    });
}

//关闭
function CloseWind() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var layer = layui.layer;
        layer.closeAll();
    });
}

