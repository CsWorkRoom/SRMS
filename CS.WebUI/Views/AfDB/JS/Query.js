var isQueryPage = true;
layui.use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
    Resize();
    //尺寸变化
    $(window).resize(function () {
        Resize();
    });
    
    //选择数据库
    form.on('select(DB_ID)', function (data) {
        getTableNameList();
    });
});

//DIV尺寸
function Resize() {
    $("#divLeft").height($(window).height() - 5);
    $("#divRight").height($(window).height() - 5);
    $("#divTableNames").height($("#divLeft").height() - 40);
    //$("#divRight").width($(window).width() - $("#divLeft").width() - 20);
}
//获取表名列表
function getTableNameList() {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        var dbID = $("#DB_ID").val();
        $.post("../AfDB/GetTableList?dbID=" + dbID + "&isImport=false", function (data) {
            var html = "";
            if (data.length > 0) {
                var tables = JSON.parse(data);
                for (var i = 0; i < tables.length; i++) {
                    html += "<a href='#' onclick=\"selectTable('" + tables[i] + "')\";>" + tables[i] + "</a><br/>";
                }
            }
            $('#divTableNames').html(html);
            form.render();
        });
    });
}

//选择数据库表
function selectTable(tableName) {
    layui.use(['jquery', 'form'], function () {
        var $ = layui.jquery, form = layui.form;
        $("#sql").val("SELECT * FROM " + tableName);
        form.render();
        QueryPage();
    });
}

//导出数据
function CheckExportQuery() {
    var dbID = $("#DB_ID").val();
    if (dbID == null || dbID < 0) {
        alert("请选择数据库");
        return false;
    }
    var sql = $("#sql").val();
    if (sql == null || sql.length < 1) {
        alert("请输入SQL语句或者点击左侧表名");
        return false;
    }

    return true;
}

//分页查询
function QueryPage() {
    isQueryPage = true;
    Query(1);
}

//查询全部
function QueryAll() {
    layui.use(['layer'], function () {
        var layer = layui.layer;
        layer.confirm('数据可能较多，确定要查询全部数据吗？', {
            icon: 3, title: '性能预警', btn: ['确定', '取消']
        }, function (index, layero) {
            layer.close(index);
            isQueryPage = false;
            Query(1);
        }, function (index) {
            return;
        });
    });
}

var pageSize = 10;
//查询
function Query(pageIndex) {
    layui.use(['form', 'element', 'layer', 'jquery', 'table', 'laypage'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery, table = layui.table, laypage = layui.laypage;
        var dbID = $("#DB_ID").val();
        if (dbID == null || dbID < 0) {
            layer.alert("请选择数据库");
            return;
        }
        var sql = $("#sql").val();
        if (sql == null || sql.length < 1) {
            layer.alert("请输入SQL语句或者点击左侧表名");
            return;
        }
        if (isQueryPage == false) {
            pageSize = 0;
        }
        var isExecuteRowsCount = pageSize > 0 && pageIndex == 1;
        //查询数据
        $.post("../AfDB/ExecuteSelectSQL", {
            dbID: dbID
            , sql: sql
            , pageSize: pageSize
            , pageIndex: pageIndex
            , isExecuteRowsCount: isExecuteRowsCount
        }, function (result) {
            //console.log(result);
            if (result.code != 0) {
                if (result.msg == null) {
                    layer.alert(result.Message);
                } else {
                    layer.alert(result.msg);
                }
            } else {
                var cols = new Array();
                cols[0] = new Array();
                for (var key in result.data[0]) {
                    var col = new Object();
                    col.field = key;
                    col.title = key;
                    col.minWidth = 120;
                    col.sort = false;
                    cols[0].push(col);
                }
                table.render({
                    elem: '#datatable'
                    , even: false
                    , size: 'sm'
                    , height: isQueryPage ? 'full-210' : 'full-182'
                    , page: false
                    , cols: cols
                    , data: result.data
                    , limit: result.data.length
                });
                if (isQueryPage == false) {
                    $("#divPage").html("共有 " + result.data.length + " 条记录");
                } else if (pageIndex == 1) {
                    laypage.render({
                        elem: 'divPage'
                        , curr: 1
                        , count: result.count
                        , limit: pageSize
                        , layout: ['refresh', 'prev', 'page', 'next', 'skip', 'limit', 'count']
                        , jump: function (obj, first) {
                            pageSize = obj.limit;
                            //首次不执行
                            if (!first) {
                                Query(obj.curr);
                            }
                        }
                    });
                }
            }
            //table.render();
        }, "json");
    });
}

//显示输入密码框
function ShowInputPassword() {
    layui.use(['layer', 'jquery'], function () {
        var layer = layui.layer, $ = layui.jquery;
        layer.open({
            title: "二次验证"
            , icon: 0
            , area: ['400px', '200px']
            , content: "执行高危操作，请输入二次验证密码：<br/><br/>密码：<input type='password' id='psd' autocomplete='off'/>"
            , btn: ['确定', '取消']
            , yes: function (index, layero) {
                var psd = $("#psd").val();
                layer.close(index);
                Execute(psd);
            }
        });
    });
}

//执行SQL语句
function Execute(psd) {
    layui.use(['form', 'element', 'layer', 'jquery', 'table', 'laypage'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery, table = layui.table, laypage = layui.laypage;
        var dbID = $("#DB_ID").val();
        if (dbID == null || dbID < 0) {
            layer.alert("请选择数据库");
            return;
        }
        var sql = $("#sql").val();
        if (sql == null || sql.length < 1) {
            layer.alert("请输入SQL语句");
            return;
        }
        //查询数据
        $.post("../AfDB/ExecuteSelectSQL", {
            dbID: dbID
            , sql: sql
            , psd: psd
        }, function (result) {
            //console.log(result);
            layer.open({
                title: "执行结果"
                , icon: result.IsSuccess == true ? 1 : 2
                , area: result.IsSuccess == true ? ['400px', '200px'] : ['800px', '450px']
                , content: "执行结果：<br/><br/>" + result.Message
            });
        }, "json");
    });
}