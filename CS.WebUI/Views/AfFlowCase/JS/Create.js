//表单提交
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

////#region 基础信息-
//layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
//    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
//    //提交
//    form.on('submit(SysFlowSubmit)', function (data) {//验证提交
//        alert("调用");
//        debugger;
//        //#region 调用子页面的提交函数
//        var $mainFun = $("#SysCsMainFun").val();
//        eval($mainFun);
//        //#endregion

//        sysCsFlowSave();//保存外部流程信息
//    });

//});

function FlowSubmit() {
    layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
        var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
        //#region 调用子页面的提交函数
        var $mainFun = $("#SysCsMainFun").val();
        //$("#core_content")[0].contentWindow.testIframe2("11");
        //$("#FlowMainPage")[0].contentWindow.save();
        //eval($mainFun);

        $($("#FlowMainPage")[0].contentWindow[$mainFun]).eval();
        //#endregion

        sysCsFlowSave();//保存外部流程信息

    });
}


