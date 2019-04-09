//此字段千万不能修改名称，因为，子页面是通过此变量名识别的页面
var pageKeyTile = "表单编辑页";
var pageIsSave = false;//是否需要保存，默认不需要
var fieldData = new Array();

layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
    $(function () {
        $(".form-top-content").css("margin-bottom", "0px");
    });

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

    //是否允许删除数据
    form.on('switch(switchDeleteCheckbox)', function (data) {
        if (this.checked) {
            $("#IS_ALLOW_DELETE").val("1");
            layer.tips('允许删除数据', data.othis)
        } else {
            $("#IS_ALLOW_DELETE").val("0");
            layer.tips('禁止删除数据', data.othis)
        }
    });

    //是否启用数据
    form.on('switch(switchEnableCheckbox)', function (data) {
        if (this.checked) {
            $("#IS_ENABLE").val("1");
            layer.tips('启用数据', data.othis)
        } else {
            $("#IS_ENABLE").val("0");
            layer.tips('禁用数据', data.othis)
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
                        fieldData[index].DEFAULT = obj.DEFAULT;
                        fieldData[index].FIELD_DATA_TYPE = obj.FIELD_DATA_TYPE;
                        fieldData[index].IS_KEY_FIELD = obj.IS_KEY_FIELD;
                        fieldData[index].IS_INSERT = obj.IS_INSERT;
                        fieldData[index].IS_UPDATE = obj.IS_UPDATE;
                        fieldData[index].IS_READONLY = obj.IS_READONLY;
                        fieldData[index].IS_NOT_NULL = obj.IS_NOT_NULL;
                        fieldData[index].IS_AUTO_INCREMENT = obj.IS_AUTO_INCREMENT;
                        fieldData[index].IS_UNIQUE = obj.IS_UNIQUE;
                        fieldData[index].ORDER_NUM = (obj.ORDER_NUM == null || obj.ORDER_NUM == "" ? "0" : obj.ORDER_NUM);
                        fieldData[index].SELECT_INPUT_TYPE = obj.SELECT_INPUT_TYPE;
                        fieldData[index].SELECT_ENUM_OPTIONS = obj.SELECT_ENUM_OPTIONS;
                        fieldData[index].SELECT_QUERY_INFO = obj.SELECT_QUERY_INFO;
                    });
                    //渲染表格
                    renderTable();
                }, "json");
            }
        };
    });
}

//渲染表格
function renderTable() {
    layui.use(['element', 'table', 'form'], function () {
        var element = layui.element, table = layui.table, form = layui.form;
        //执行渲染
        table.render({
            elem: '#tableField'
            , cols: [[
              { field: 'EN_NAME', title: '字段英文名', sort: true, width: 116 }
              , { field: 'CN_NAME', title: '字段中文名', edit: 'text', width: 120 }
              , { field: 'DEFAULT', title: '默认值', edit: 'text', width: 95 }
              , { field: 'FIELD_DATA_TYPE', title: '类型', align: 'center', templet: '#tempDataType', width: 38 }
              , { field: 'IS_KEY_FIELD', title: '为主键', templet: '#tempKey', width: 87 }
              , { field: 'IS_INSERT', title: '新增', templet: '#tempAdd', width: 87 }
              , { field: 'IS_UPDATE', title: '更新', templet: '#tempUp', width: 87 }
              , { field: 'IS_READONLY', title: '编辑时只读', templet: '#tempReadOnly', width: 87 }
              , { field: 'IS_NOT_NULL', title: '不为空', templet: '#tempNull', width: 100 }
              , { field: 'IS_AUTO_INCREMENT', title: '自增长', templet: '#tempAuto', width: 87 }
              , { field: 'IS_UNIQUE', title: '唯一', templet: '#tempUnique', width: 87 }
              , { field: 'ORDER_NUM', title: '排序', edit: 'number', width: 40 }
              , { field: 'INPUT_TYPE', title: '输入框类型', templet: '#tempInputType', minWidth: 160 }
            ]]
            , data: fieldData
            , limit: fieldData.length
        });
        table.on('edit(tableField)', function (obj) {
            var value = obj.value //得到修改后的值            
            , field = obj.field; //得到字段
            if (field == "ORDER_NUM") {
                if (value != null && $.trim(value) != "" && $.isNumeric(value) == false) {
                    layer.msg(field + ' 字段的值只能为数字。您填入的值（' + value + '）,类型不对！请重新输入。');
                }
            }
        });
        //监听锁定操作
        form.on('checkbox(fltKey)', function (obj) {//是否为主键
            $.each(fieldData, function (index) {
                fieldData[index].IS_KEY_FIELD = 0;
            });
            fieldData[this.value].IS_KEY_FIELD = obj.elem.checked ? 1 : 0;
            renderTable();
        });

        form.on('checkbox(fltAdd)', function (obj) {//新增                
            fieldData[this.value].IS_INSERT = obj.elem.checked ? 1 : 0;
        });

        form.on('checkbox(fltUp)', function (obj) {//更新                
            fieldData[this.value].IS_UPDATE = obj.elem.checked ? 1 : 0;
        });

        form.on('checkbox(fltReadOnly)', function (obj) {//只读                
            fieldData[this.value].IS_READONLY = obj.elem.checked ? 1 : 0;
        });

        form.on('checkbox(fltNull)', function (obj) {//不为空
            fieldData[this.value].IS_NOT_NULL = obj.elem.checked ? 1 : 0;
        });

        form.on('checkbox(fltAuto)', function (obj) {//自增长
            $.each(fieldData, function (index) {
                fieldData[index].IS_AUTO_INCREMENT = 0;
            });
            fieldData[this.value].IS_AUTO_INCREMENT = obj.elem.checked ? 1 : 0;
            renderTable();
        });
        form.on('checkbox(fltUnique)', function (obj) {//唯一
            //$.each(fieldData, function (index) {
            //    fieldData[index].IS_UNIQUE = 0;
            //});
            fieldData[this.value].IS_UNIQUE = obj.elem.checked ? 1 : 0;
            // renderTable();
        });
        //选择数据库表
        form.on('select(DB_ID)', function (data) {
            GetTableData(null);//动态生成表列表
        });
    });
}

//更新下拉字段
function ChangeField(index, field, obj) {
    fieldData[index][field] = obj.value;
    if (obj.value == $.trim($("#xlId").val()) || obj.value == $.trim($("#duofx").val()) || obj.value == $.trim($("#treexl").val())) {//判断下拉
        SetField(index, field);
    } else {
        $("#bt" + index).remove();//移出按钮
    }
    renderTable();//重新渲染
}

//设置下拉字段信息
function SetField(index, field) {
    OpenTopWindow("下拉数据来源模式", 0, 0, "../AfForm/DownModel?index=" + index + "&inputtype=" + fieldData[index][field]);
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
        var isNullKey = true;//检测是否存在主键
        $.each(fieldData, function (index) {
            if (fieldData[index].IS_KEY_FIELD == 1) {
                if (isNullKey) {
                    isNullKey = false;
                }
            }
            if (fieldData[index].INPUT_TYPE == null || fieldData[index].INPUT_TYPE == "") {//设置默认为普通文本
                fieldData[index].INPUT_TYPE = 1;
            }
        });
        if (isNullKey) {
            layer.alert("对不起！表单配置中必须含主键，否则不能被提交。");
            return;
        }
        $("#FIELDS").val(JSON.stringify(fieldData));
        var url = "../AfForm/Edit";
        SaveForm('form', url);
        return;
    });
}

//获取数据库表信息
function GetTableData(tableName) {
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
                $('#S_TABLE_NAME').html(html);
                form.render();
            }
        });
    });
}

//显示SQL帮助
function showSqlHelp() {
    layui.use(['layer'], function () {
        var $ = layui.jquery, layer = layui.layer;
        var content = "在SQL语句中，可以使用使用占位符的方式，来插入HTML输入框的值或者C#程序中的变量及方法，语法如下：<br/>";
        content += "一、插入固定的值<br/>";
        content += "    直接输入具体的值 <br/>";
        content += "二、插入C#的系统变量<br/>";
        content += "    @{USER_ID}当前登录用户ID <br/>";
        content += "    @{USER_NAME}当前登录用户登录名 <br/>";
        content += "    @{DEPARTMENT_ID}当前登录用户所属组织机构ID <br/>";
        content += "    @{DEPARTMENT_CODE}当前登录用户所属组织机构编码 <br/>";
        content += "    @{DEPARTMENT_NAME}当前登录用户所属组织机构名称 <br/>";
        content += "    @{DEPARTMENT_LEVEL}当前登录用户所属组织机构层级 <br/>";
        content += "    @{ALLROLE} 当前登录用户所属权限 <br/>";
        content += "三、插入C#中定义的方法（日期函数）：<br/>";
        content += "    @{yyyy(n)}返回年份，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymm(n)}返回年月，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymmdd(n)}返回年月日，参数n为整数，默认为0 <br/>";
        content += "    @{DATETIME}返回完整的日期时间格式（日期时间如：2018-08-18 18:18:18） <br/>";
        content += "    @{DATE}返回完整的日期格式（日期部份如：2018-08-18） <br/>";
        content += "四、注意：<br/>";
        content += "    当字段设为新增或编辑，但又不显示时，一定要给出默认值，否则表单会报错<br/>";

        layer.open({
            title: '字段插入默认值'
            , content: content
            , btn: ['关闭']
            , moveType: 1
            , area: ['800px;', '400px;']
        });
    });
}