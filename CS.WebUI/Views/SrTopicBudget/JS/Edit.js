//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicBudget/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        $("#Budgets").val(JSON.stringify(budget.budgetArr));
        save();//保存
    });

    budget.budgetArr = budget.getBudgetArr();
    budget.budgetRender();
});
function SaveFlowForm()
{
    var url = "../SrTopicBudget/Edit";
    $("#Budgets").val(JSON.stringify(budget.budgetArr));
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

//#region 参与单位信息
var budget = {
    budgetArr: [],//绑定的单位信息(支持增删改)
    budgetRender: function () {
        layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
            var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
            table.render({
                elem: '#tbBudget'
          , even: true
          , page: false
          , limit: 90
          , size: 'sm'
          , cols: [[
            { field: 'ID', title: '编号'}
            , { field: 'BUDGET_TYPE_ID', title: '预算类型', templet: '#selectBudgetType', width: 200 }
            , { field: 'NAME', title: '预算名称', edit: 'text', width: 200 }
            , { field: 'FEE', title: '预算金额(单位：元)', edit: 'text', width: 200 }
            , { field: 'REMARK', title: '预算说明', edit: 'text', minWidth: 250 }
            , { title: '<span class="layui-badge layui-bg-green" style="cursor:default" href="#" onclick="budget.addBudget(0);">+ 操作</span>', width: 120, templet: '#addDelBudget' }
          ]]
          , data: budget.budgetArr
          , done: function () {
              $("[data-field='ID']").css('display', 'none');
          }
            });

            //监听表格单元格编辑
            table.on('edit(tbBudget)', function (obj) {
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
                        layer.alert("预算值[" + data.FEE + "]不是数值类型！");//抛出错误
                        return;
                    }
                    //#endregion

                    //#region 验证预算值(判断值是否大于0) 
                    if (parseFloat(data.FEE) < 0) {
                        $(this).val(preValue);//恢复为修改前的值
                        data.FEE = preValue;//恢复为修改前的值
                        layer.alert('提示：预算值[' + value + ']应大于0');
                        return;
                    }
                    //#endregion

                    $("#tipFee").html(budget.getTotalFee());//总预算的提示信息
                }
            });
        });
        $("#tipFee").html(budget.getTotalFee());//总预算的提示信息
    },

    //添加一个单位
    addBudget: function (index) {
        var item = {
            'BUDGET_TYPE_ID': 1,//此处应该给一个类型表中有的值作为默认值
            'FEE':0
        };
        budget.budgetArr.push(item);
        budget.budgetRender();
    },
    //删除指定单位
    deleteBudget: function (index) {
        budget.budgetArr.splice(index, 1);
        budget.budgetRender();
    },
    //从控件中获得参与单位集合信息
    getBudgetArr: function () {
        var budgets = $("#Budgets").val();
        if (!budget.isEmpty(budgets)) {
            return $.parseJSON(budgets);
        } else { return []; }
    },
    //预算类型修改后
    changeBudgetType: function (index, obj) {
        //console.log(obj);
        var val = $(obj).val();//当前选择的值
        budget.budgetArr[index]['BUDGET_TYPE_ID'] = val;
        budget.budgetRender();
    },
    //获得预算总金额
    getTotalFee: function () {
        var totalFee = 0;
        $.each(budget.budgetArr, function (i, n) {
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
}
//#endregion
