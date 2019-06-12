//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicFunds/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        totalFeeMust: function (value) {
            if (funds.isEmpty(value)) {
                return '请填写报销总额！';
            }
            if (!$.isNumeric(value))
            {
                return '报销总额不是数值类型';
            }
            if(Number(value)<=0)
            {
                return '报销总额应大于0';
            }
        },
        topicVer: function (value) {
            if (value.indexOf("type_") != -1) {
                return '请选择课题(勿选择课题类型)';
            }
        }
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        $("#FundsDetails").val(JSON.stringify(funds.fundsDetailArr));

        $("#IS_DEFAULT_BANK").val(0);
        $("#DefaultCk:checked").each(function () { // 遍历指标维度多选框
            $("#IS_DEFAULT_BANK").val(1);//1为选中
        });
        //#region 验证报销清单金额之和是否等于总报销金额
        var calTotalFee = funds.getTotalFee();
        var totalFee = $("#TOTAL_FEE").val();
        if (calTotalFee == Number(totalFee))
        { }
        else
        {
            layer.alert('提示：报销总金额【' + totalFee + '】不等于报销清单金额之和【' + calTotalFee + '】');
            return;
        }
        //#endregion
        save();//保存
    });

    form.on('select(selectGroup)', function (data) {
        funds.ChangeBankSelect();
    });

    //#region 课题下拉绑定
    var topicNodes = JSON.parse($("#TopicSelect").val());
    $.comboztree("TOPIC_ID", { ztreenode: topicNodes });
    //#endregion

    funds.fundsDetailArr = funds.getFundsDetailArr();
    funds.fundsDetailRender();
});


//#region 经费清单
var funds = {
    fundsDetailArr: [],//(支持增删改)
    fundsDetailRender: function () {
        layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
            var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
            table.render({
                elem: '#tbFundsDetail'
          , even: true
          , page: false
          , limit: 90
          , size: 'sm'
          , cols: [[
            { field: 'ID', title: '编号', hidden: true }
            , { field: 'NAME', title: '报销项目', edit: 'text', width: 200}
            , { field: 'FEE', title: '报销金额(单位：元)', edit: 'text', width: 200 }
            , { field: 'REMARK', title: '说明', edit: 'text', minWidth: 250 }
            , { title: '<span class="layui-badge layui-bg-green" style="cursor:default" href="#" onclick="funds.addFundsDetail(0);">+ 操作</span>', width: 120, templet: '#addDelFundsDetail' }
          ]]
          , data: funds.fundsDetailArr
          , done: function () {
              $("[data-field='ID']").css('display', 'none');
          }
            });
            //监听表格单元格编辑
            table.on('edit(tbFundsDetail)', function (obj) {
                var value = obj.value //得到修改后的值
                    , data = obj.data //得到所在行所有键值
                    , field = obj.field //得到字段
                    , preValue = $(this).get(0).previousSibling.innerText;//修改前的值
                //layer.msg('[ID: ' + data.ID + '] ' + field + ' 字段更改为：' + value);

                if (field == "FEE") {
                    //#region 验证预算值是否为数值
                    if (!$.isNumeric(data.FEE)) {
                        $(this).val(preValue);//恢复为修改前的值
                        data.FEE = preValue;//恢复为修改前的值
                        layer.alert("报销金额[" + data.FEE + "]不是数值类型！");//抛出错误
                        return;
                    }
                    //#endregion

                    //#region 验证预算值(判断值是否大于0) 
                    if (parseFloat(data.FEE) < 0) {
                        $(this).val(preValue);//恢复为修改前的值
                        data.FEE = preValue;//恢复为修改前的值
                        layer.alert('提示：报销金额[' + value + ']应大于0');
                        return;
                    }
                    //#endregion

                    $("#tipFee").html(funds.getTotalFee());//总预算的提示信息
                }
            });
        });
        $("#tipFee").html(funds.getTotalFee());//总预算的提示信息
    },

    //添加一个单位
    addFundsDetail: function (index) {
        var item = {
            'FEE': 0
        };
        funds.fundsDetailArr.push(item);
        funds.fundsDetailRender();
    },
    //删除指定单位
    deleteFundsDetail: function (index) {
        funds.fundsDetailArr.splice(index, 1);
        funds.fundsDetailRender();
    },
    //从控件中获得参与单位集合信息
    getFundsDetailArr: function () {
        var fundsDetails = $("#FundsDetails").val();
        if (!funds.isEmpty(fundsDetails)) {
            return $.parseJSON(fundsDetails);
        } else { return []; }
    },
    //获得预算总金额
    getTotalFee: function () {
        var totalFee = 0;
        $.each(funds.fundsDetailArr, function (i, n) {
            totalFee = totalFee + Number(n.FEE);
        });
        return totalFee;
    },
    //判断字符是否为空的方法
    isEmpty: function (obj) {
        if (typeof obj == "undefined" || obj == null || obj == "" || obj == 'null') {
            return true;
        } else {
            return false;
        }
    },
    //显示树弹框
    ShowTree: function () {
        layui.use(['layer', 'jquery'], function () {
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
    ChangeBankSelect: function () {
        var bankId = $("#bankSelect").val();
        if (bankId == -1)//未选择
        {

        }
        else//赋值后关闭弹框
        {
            var banks = $("#Banks").val();
            if(!funds.isEmpty(banks)&&banks.length>0)
            {
                var bankArr = $.parseJSON(banks);
                $.each(bankArr, function (i, n) {
                    if(n.ID==bankId)
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
//#endregion
