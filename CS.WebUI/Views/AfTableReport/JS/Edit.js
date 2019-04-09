//是否已经验证SQL语句
var isCheckedDefaultInputValues = false;

//表单操作
layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
    if ($("#ID").val() > 0) {
        isCheckedDefaultInputValues = true;
    }

    //导出选项
    form.on('switch(switchExport)', function (data) {
        if (this.checked) {
            $("#IS_SHOW_EXPORT").val("1");
            layer.tips('将在表格上方显示“导出”按钮', data.othis)
        } else {
            $("#IS_SHOW_EXPORT").val("0");
            layer.tips('将不会在表格上方显示“导出”按钮', data.othis)
        }
    });

    //调试选项
    form.on('switch(switchDebug)', function (data) {
        if (this.checked) {
            $("#IS_SHOW_DEBUG").val("1");
            layer.tips('将在表格上方显示“导出”按钮', data.othis)
        } else {
            $("#IS_SHOW_DEBUG").val("0");
            layer.tips('将不会在表格上方显示“导出”按钮', data.othis)
        }
    });

    //是否显示复选框
    form.on('switch(switchCheckbox)', function (data) {
        if (this.checked) {
            $("#IS_SHOW_CHECKBOX").val("1");
            layer.tips('将在表格左侧显示复选框', data.othis)
        } else {
            $("#IS_SHOW_CHECKBOX").val("0");
            layer.tips('将不会在表格左侧显示复选框', data.othis)
        }
    });

    //SQL变化之后，必须重新分析
    $("#SQL_CODE").change(function (data) {
        isCheckedDefaultInputValues = false;
        layer.tips("编辑好SQL之后，请分析SQL语句，并得到成功提示再保存", "#aTestSql");
    });
});

//显示SQL帮助
function showSqlHelp() {
    layui.use(['layer'], function () {
        var $ = layui.jquery, layer = layui.layer;
        var content = "在SQL语句中，可以使用使用占位符的方式，来插入HTML输入框的值或者C#程序中的变量及方法，语法如下：<br/>";
        content += "一、插入HTML输入框的值<br/>";
        content += "    @(INPUT_NAME) <br/>";
        content += "二、插入C#的系统变量<br/>";
        content += "    @{USER_ID} 当前登录用户ID <br/>";
        content += "    @{USER_NAME} 当前登录用户登录名 <br/>";
        content += "    @{DEPARTMENT_ID} 当前登录用户所属组织机构ID <br/>";
        content += "    @{DEPARTMENT_CODE} 当前登录用户所属组织机构编码 <br/>";
        content += "    @{DEPARTMENT_NAME} 当前登录用户所属组织机构名称 <br/>";
        content += "    @{DEPARTMENT_LEVEL} 当前登录用户所属组织机构层级 <br/>";
        content += "    @{ALLROLE} 当前登录用户所属权限 <br/>";
        content += "三、插入C#中定义的方法（日期函数）：<br/>";
        content += "    @{DATETIME} 返回完整的日期格式（日期时间） <br/>";
        content += "    @{DATE} 返回完整的日期格式（日期部份）<br/>";
        content += "    @{yyyy(n)} 返回年份，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymm(n)} 返回年月，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymmdd(n)} 返回年月日，参数n为整数，默认为0 <br/>";
        content += "四、计件特有数据库函数：<br/>";
        content += "    select * from table(GET_USERS(@{DEPARTMENT_CODE})) 根据组织编号(DEPT_CODE)获得当前组织及子组织的用户集合 <br/>";
        content += "    select * from table(GET_DEPARTMENT_TREE(@{DEPARTMENT_CODE})) 根据组织编号(DEPT_CODE)获得当前组织及子组织的集合 <br/>";
        content += "    @{CITY} 当前登录用户所在地市,未找到时默认为0 <br/>";
        content += "    @{COUNTY} 当前登录用户所在区县,未找到时默认为0 <br/>";

        layer.open({
            title: 'SQL语句中可用参数'
            , content: content
            , btn: ['关闭']
            , moveType: 1
            , area: ['800px;', '400px;']
        });
    });
}

//分析SQL语句
function testSql() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var sql = $("#SQL_CODE").val();
        if (sql == null || sql.length < 1) {
            layer.alert("请输入SQL语句");
            return;
        }
        var url = "../AfTableReport/Edit";
        var inputjson = "";
        isCheckedDefaultInputValues = false;
        $("#DEFAULT_INPUT_VALUES").val("");
        //正则匹配SQL中的自定义参数
        var regx = /@\(([a-zA-Z0-9_\-]+?)\)/g;
        var matchs = sql.match(regx);
        if (matchs == null) {
            $.post(url, {
                dbid: $("#DB_ID").val(),
                sql: sql,
                inputjson: inputjson
            }, function (result) {
                if (result.IsSuccess == true) {
                    var msg = "测试成功，若字段有变化，请进行报表配置<br/>" + result.Message;
                    isCheckedDefaultInputValues = true;
                    layer.alert(msg, { icon: 1 });
                } else {
                    var msg = "测试失败<br/>" + result.Message;
                    layer.alert(msg, { icon: 2 });
                }
            });
            return;
        }

        //console.log(matchs);

        var html = "<div class='layui-form'>";
        var inputNames = new Array();
        for (var i = 0; i < matchs.length; i++) {
            var name = matchs[i].substr(2, matchs[i].length - 3);
            if ($.inArray(name, inputNames) < 0) {
                inputNames.push(name);
                html += name + "<input type='text' id='" + name + "'/>";
            }
        }
        html+="</div>"
        //console.log(inputNames);

        layer.open({
            title: '请输入如下参数的值，以供测试'
            , area: ['500px', '300px']
            , content: html
            , yes: function () {
                var inputValues = new Array();
                for (var i = 0; i < inputNames.length; i++) {
                    inputValues[i] = new Object();
                    inputValues[i].Name = inputNames[i];
                    inputValues[i].Value = $("#" + inputNames[i]).val();
                }
                var inputjson = JSON.stringify(inputValues);
                //console.log(inputjson);

                $.post(url, {
                    dbid: $("#DB_ID").val(),
                    sql: sql,
                    inputjson: inputjson
                }, function (result) {
                    if (result.IsSuccess == true) {
                        isCheckedDefaultInputValues = true;
                        $("#DEFAULT_INPUT_VALUES").val(inputjson);
                        var msg = "测试成功<br/>" + result.Message;
                        layer.alert(msg, { icon: 1 });
                    } else {
                        var msg = "测试失败<br/>" + result.Message;
                        layer.alert(msg, { icon: 2 });
                    }
                });
                //layer.closeAll();
            }
        });

        return;
    });
}

//保存
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        if (isCheckedDefaultInputValues == false) {
            layer.alert("请在保存前，先分析SQL并通过验证");
            return;
        }
        var url = "../AfTableReport/Edit";
        SaveForm('form', url);
        return;
    });
}