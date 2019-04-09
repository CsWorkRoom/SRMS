//高级筛选
function ShowAdvancedFilter() {
    layui.use(['layer'], function () {
        var layer = layui.layer, $ = layui.jquery;
        $('#divAdvancedFilter').show(200);
    });
}

//关闭高级筛选
function HideAdvancedFilter() {
    layui.use(['layer'], function () {
        var layer = layui.layer, $ = layui.jquery;
        $('#divAdvancedFilter').hide(200);
    });
}

//显示调试信息
function ShowDebug() {
    layui.use(['layer'], function () {
        var layer = layui.layer;
        var sql = executeSQL.replace(/\</g, '&lt;').replace(/\>/g, '&gt;');
        var para = executeParam.replace(/\</g, '&lt;').replace(/\>/g, '&gt;');
        layer.open({
            title: '调试'
            , area: ['900px', '450px']
            , content: "SQL语句<pre>" + sql + "</pre>参数列表<pre>" + para + "</pre>"
        });
    });
}

//导出数据到excel
function ExportExcel() {
    layui.use(['layer', 'jquery'], function () {
        var layer = layui.layer, $ = layui.$;
        layer.confirm('确定导出到Excel（记录太多将自动导为txt）？', function (index) {
            layer.close(index);
            var url = "../AfTableReport/ExportExcel?" + $('#QUERY_STRING').val() + "&input=" + encodeURI(inputstring) + "&where=" + encodeURI(wherestring);
            document.getElementById("iframeExport").src = url;
        });
    });
}

//查询
function Query() {
    loading = layer.msg("数据加载中，请稍候……", { time: false });
    pageindex = 1;
    rowscount = 0;
    RefreshData();
}

//刷新表格
function RefreshData() {
    layui.use(['table', 'jquery'], function () {
        var table = layui.table, $ = layui.jquery;
        //自定义输入项
        var index = 0;
        var inputArray = new Array();
        $.each($("#divTopCode").find("*"), function (i, j) {
            var id = $(j).attr('id');
            if (id != null) {
                inputArray[index] = new Object();
                inputArray[index].Name = id;
                inputArray[index].Value = $(j).val();
                index++;
            }
        });
        inputstring = JSON.stringify(inputArray);
        //筛选项
        index = 0;
        var filterArray = new Array();
        $.each($('[filter]'), function (i, j) {
            var value = null;
            if ($(j).attr('type') == 'checkbox') {
                value = $(j)[0].checked == true ? "1" : "0";
            } else {
                value = $(j).val();
            }
            if (value != null && value.length > 0 && value != "-999") {
                filterArray[index] = new Object();
                filterArray[index].Field = $(j).attr('filter');
                filterArray[index].DataType = $(j).attr('datatype');
                filterArray[index].Operator = $("#OP_" + $(j).attr('filter')).val();
                filterArray[index].Value = value;
                index++;
            }
        });
        wherestring = JSON.stringify(filterArray);
        //执行重载
        var t = table.reload('reporttable', {
            initSort: sort
            , where: {
                  count: rowscount
                , input: inputstring
                , where: wherestring
                , orderByField: sort.field
                , orderByType: sort.type
            }
            , page: {
                curr: pageindex
            }
            , done: function (res, curr, count) {
                pageindex = curr;
                rowscount = count;
                executeSQL = res.sql;
                executeParam = res.sqlparam;
                //console.log($(".layui-table-body"));
                $('.layui-table-body').height($(window).height() - $('#divTop').height() - 96);
                layer.close(loading);//关闭加载中
            }
        });
    });
}