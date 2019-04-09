function showSqlHelp() {
    layui.use(['layer'], function () {
        var $ = layui.jquery, layer = layui.layer;
        var content = "在SQL语句中，可以使用使用占位符的方式，来插入HTML输入框的值或者C#程序中的变量及方法，语法如下：<br/>";
        content += "一、插入HTML输入框的值<br/>";
        content += "    @(INPUT_NAME)<br/>";
        content += "二、插入C#的系统变量<br/>";
        content += "    @{USER_ID}当前登录用户ID <br/>";
        content += "    @{USER_NAME}当前登录用户登录名 <br/>";
        content += "    @{DEPARTMENT_ID}当前登录用户所属组织机构ID <br/>";
        content += "    @{DEPARTMENT_CODE}当前登录用户所属组织机构编码 <br/>";
        content += "    @{DEPARTMENT_NAME}当前登录用户所属组织机构名称 <br/>";
        content += "    @{DEPARTMENT_LEVEL}当前登录用户所属组织机构层级 <br/>";
        content += "    @{ALLROLE}当前登录用户所属权限 <br/>";
        content += "三、插入C#中定义的方法（日期函数）：<br/>";
        content += "    @{DATETIME}返回完整的日期格式（日期时间） <br/>";
        content += "    @{DATE}返回完整的日期格式（日期部份）<br/>";
        content += "    @{yyyy(n)}返回年份，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymm(n)}返回年月，参数n为整数，默认为0 <br/>";
        content += "    @{yyyymmdd(n)}返回年月日，参数n为整数，默认为0 <br/>";

        layer.open({
            title: 'SQL语句中可用参数'
            , content: content
            , btn: ['关闭']
            , moveType: 1
            , area: ['800px;', '400px;']
        });
    });

    //终止表列头事件冒泡
    this.addEventListener('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
    }, false);
}

function chartHelp() {
    layui.use(['layer'], function () {
        var $ = layui.jquery, layer = layui.layer;
        var content = "Echart动态图表配置需要从后台获取数据原，系统默认支持的全局变量和方法如下：<br/><br/>";
        content += "dataArr：全局变量。从后端返回的数据json对象集合,用户可以直接在编辑时使用<br/>";
        content += "GetArrByFd(字段英文名)：方法。从dataArr集合中获得指定字段的数据集合。如：GetArrByFd(\"USER_NAME\"),返回：['用户一','用户二']<br/>";
        content += "GetSeries(name,value)：方法。返回集合：[{value:\"\",name:\"\"},{value:\"\",name:\"\"}...]<br/>";

        layer.open({
            title: 'Echart动态图表说明'
            , content: content
            , btn: ['关闭']
            , moveType: 1
            , area: ['800px;', '400px;']
        });
        return;
        //layer.open({
        //    type: 1
        //    , title: '不显示标题栏'
        //    , closeBtn: true
        //    , area: ['800px;', '300px;']
        //    , shade: 0.8
        //    , id: 'lay_sql_help'
        //    , btn: ['关闭']
        //    , btnAlign: 'c'
        //    , moveType: 1 //拖拽模式，0或者1
        //    , content: content
        //    , success: function (layero) {
        //        //var btn = layero.find('.layui-layer-btn');
        //        //alert('aaa');
        //    }
        //});
    });

    //终止表列头事件冒泡
    this.addEventListener('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
    }, false);
}


layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
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

    ////是否显示复选框
    //form.on('switch(switchCheckbox)', function (data) {
    //    if (this.checked) {
    //        $("#IS_SHOW_CHECKBOX").val("1");
    //        layer.tips('将在表格左侧显示复选框', data.othis)
    //    } else {
    //        $("#IS_SHOW_CHECKBOX").val("0");
    //        layer.tips('将不会在表格左侧显示复选框', data.othis)
    //    }
    //});

    //自定义验证规则
    form.verify({
        name: function (value) {
            if (value.length < 3) {
                return '报表名称不能少于3个字母';
            }
            getChartCode();//获取图表的配置代码
        }
        , role: function (value) {
            if (value.length < 2) {
                return '必须至少选择一个角色';
            }
        }
        , pass: [/(.+){6,12}$/, '密码必须6到12位']
        , content: function (value) {
            layedit.sync(editIndex);
        }
    });

    //提交结果，刷新列表数据
    if ($("#msg").val() != "") {
        var msg = $("#msg").val() + " 刷新列表数据吗？";
        layer.confirm(msg, function (index) {
            layer.closeAll();
            parent.layer.closeAll();
            parent.RefreshData();
        });
    }

});

//----------------------------------------图表配置代码-----------------------------------------
window.onload = function () {
    analysisSql();
    ChartCodeClick();
}

//子页面处理(这个模块已被移到对应的页面了)
function ChartCodeClick() {
    var code = $("#CHART_CODE").val();
    $("#ifm").contents().find("#endCode").val(Encrypt(code));
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
    var data = $.ajax({
        type: "post",
        data: { DB_ID: $("#DB_ID").val(), SQL_CODE: $("#SQL_CODE").val() },
        url: "../AfChartReport/GetDataBySql",
        async: false
    });
    if (data != null && data.responseText != null && data.responseText.length > 0) {
        $("#ifm").contents().find("#seachData").val(data.responseText);
        //var dataArr = $.parseJSON(data.responseText);
    }

    $("#ifm").contents().find("#childRun").click();//
}

var isCheckedDefaultInputValues = false;
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
                    var msg = "测试成功<br/>" + result.Message;
                    isCheckedDefaultInputValues = true;
                    layer.alert(msg, { icon: 1 });
                    analysisSql();
                } else {
                    var msg = "测试失败<br/>" + result.Message;
                    layer.alert(msg, { icon: 2 });
                }
            });
            return;
        }

        var html = "<div class='layui-form'>";
        var inputNames = new Array();
        for (var i = 0; i < matchs.length; i++) {
            var name = matchs[i].substr(2, matchs[i].length - 3);
            if ($.inArray(name, inputNames) < 0) {
                inputNames.push(name);
                html += name + "<input type='text' id='" + name + "'/>";
            }
        }
        html += "</div>"

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

                $.post(url, {
                    dbid: $("#DB_ID").val(),
                    sql: sql,
                    inputjson: inputjson
                }, function (result) {
                    if (result.IsSuccess == true) {
                        isCheckedDefaultInputValues = true;
                        var msg = "测试成功<br/>" + result.Message;
                        layer.alert(msg, { icon: 1 });
                        analysisSql();
                    } else {
                        var msg = "测试失败<br/>" + result.Message;
                        layer.alert(msg, { icon: 2 });
                    }
                });
            }
        });
        return;
    });
}
//提交事件
$("form").submit(function (e) {
     $("#TOP_CODE").val(Encrypt($.trim($("#TOP_CODE").val())));
     $("#BOTTOM_CODE").val(Encrypt($.trim($("#BOTTOM_CODE").val())));
});