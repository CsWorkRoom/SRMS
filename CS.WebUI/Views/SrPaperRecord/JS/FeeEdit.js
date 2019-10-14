

function save()
{
    layui.use(['form', 'layer', 'jquery'], function ()
    {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "SrPaperRecord/FeeEdit";

        SaveForm('form', url);
        return;
    });
}
function SaveFlowForm()
{
    var url = "../SrPaperRecord/FeeEdit";
    $("#IS_DEFAULT_BANK").val(0);
    $("#DefaultCk:checked").each(function ()
    { // 遍历指标维度多选框
        $("#IS_DEFAULT_BANK").val(1);//1为选中
    });
    var data = $("#form").serialize();

    var resData = "";
    $.ajax({
        url: url,
        type: "post",
        data: data,
        async: false,
        success: function (res)
        {
            resData = res;
        }
    });
    return resData;
}


layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function ()
{
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;

    //提交
    form.on('submit(submit)', function (data)
    {//验证提交
        save();//保存
    });
    form.on('select(selectGroup)', function (data)
    {
        funds.ChangeBankSelect();
    });
});

var funds = {
    //判断字符是否为空的方法
    isEmpty: function (obj)
    {
        if (typeof obj == "undefined" || obj == null || obj == "" || obj == 'null')
        {
            return true;
        } else
        {
            return false;
        }
    },
    //显示树弹框
    ShowTree: function ()
    {
        layui.use(['layer', 'jquery'], function ()
        {
            var layer = layui.layer, $ = layui.$;
            layer.open({
                type: 1
                , title: '请勾选查询字段'
                , btn: '关闭'
                , btnAlign: 'c' //按钮居中
                , area: ['390px', '300px']
                , shade: 0 //不显示遮罩
                , content: $('.csZtree') //这里content是一个DOM，注意：最好该元素要存放在body最外层，否则可能被其它的相对元素所影响
            });
        });
    },
    //选择银行卡后的动作
    ChangeBankSelect: function ()
    {
        var bankId = $("#bankSelect").val();
        if (bankId == -1)//未选择
        {

        }
        else//赋值后关闭弹框
        {
            var banks = $("#Banks").val();
            if (!funds.isEmpty(banks) && banks.length > 0)
            {
                var bankArr = $.parseJSON(banks);
                $.each(bankArr, function (i, n)
                {
                    if (n.ID == bankId)
                    {
                        $("#BANK_NAME").val(n.BANK_NAME);
                        $("#BANK_NO").val(n.BANK_NO);
                        $("#BANK_ADDRESS").val(n.BANK_ADDRESS);
                        $("#USER_NAME").val(n.USER_NAME);
                        $("#USER_PHONE").val(n.USER_PHONE);
                        layer.closeAll();//关闭所有弹框
                        return false;
                    }
                });
            }
        }
    }
}




