//流程的数据提交
function sysCsFlowSave(mainTableKey) {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
       
        var formFd = {
            SysCsFlowID: $("#SysCsFlowID").val(),
            sysCsMainTableKey: mainTableKey
        }
        var url = "../AfFlowCase/Create";

        loading = layer.msg("数据保存中，请稍候……", { time: false, shade: 0.3 });
        $.post(TransURL(url), formFd, function (result) {
            layer.close(loading);//关闭保存中

            if (result.IsSuccess == true) {
                var msg = result.Message + "<br/> 刷新列表数据吗？";
                layer.confirm(msg, function (index) {
                    layer.closeAll();
                    parent.layer.closeAll();
                    parent.RefreshData();
                });
            } else {
                var msg = "保存失败<br/>" + result.Message;
                layer.alert(msg, { icon: 2 });
            }
        });
        return;
    });
}

/*
  注意：约定流程主表单的提交按钮函数名为：SaveFlowForm()
  返回为JsonResultData类型的json字符串
*/
function FlowSubmit() {
    layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
        var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
        //#region 调用子页面的提交函数
        //#region 自定义的函数调用（已作废）
        //var $mainFun = $("#SysCsMainFun").val();
        //$($("#FlowMainPage")[0].contentWindow[$mainFun]).eval();
        //#endregion
        
        //#region 调用子页面约定函数
        var p= $("#FlowMainPage")[0].contentWindow.SaveFlowForm();//调用子页面提交函数获得主键信息
        //#endregion
        //#endregion

        p.then(function(data) {
            if (data.IsSuccess)
            {
                //保存流程流转信息
                sysCsFlowSave(data.Result.ID);//保存外部流程信息
            }
            else//保存失败
            {
                layer.alert(data.Message, { icon: 2 });
            }
        });
    });
}



