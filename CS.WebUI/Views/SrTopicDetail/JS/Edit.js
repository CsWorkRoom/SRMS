//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopicDetail/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        subjectMust: function (value) {
            if (Number(value) <= 0) {
                return '请选择学科！';
            }
        },
        deptMust: function (value) {
            if (Number(value) <= 0) {
                return '请选择所属单位！';
            }
        },
    });

    //提交
    form.on('submit(submit)', function (data) {//验证提交
        $("#Companys").val(JSON.stringify(company.companyArr));//将参与单位集合信息存放到表单控件
        save();//保存
    });

    //#region 三个下拉绑定
    var subjectNodes = JSON.parse($("#SubjectSelect").val());
    $.comboztree("SUBJECT_ID", { ztreenode: subjectNodes });

    var deptNodes = JSON.parse($("#DepartmentSelect").val());
    $.comboztree("DEPARTMENT_ID", { ztreenode: deptNodes });

    var acountTypeNodes = JSON.parse($("#AccountingTypeSelect").val());
    $.comboztree("ACCOUNTING_TYPE_ID", { ztreenode: acountTypeNodes });
    //#endregion

    company.companyArr = company.getCompanyArr();//参与单位信息集合
    company.companyRender();//初始化绑定参与单位信息
});


//#region 参与单位信息
var company = {
    companyArr: [],//绑定的单位信息(支持增删改)
    companyRender: function () {
        layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
            var form = layui.form, layer = layui.layer, laydate = layui.laydate, $ = layui.jquery, table = layui.table, element = layui.element;
            table.render({
                elem: '#tbCompany'
          , even: true
          , page: false
          , limit: 90
          , size: 'sm'
          , cols: [[
            { field: 'ID', title: '编号', hidden: true }
            , { field: 'NAME', title: '参与单位', edit: 'text', width: 200}
            , { field: 'LINK_NAME', title: '单位联系人', edit: 'text', width: 150 }
            , { field: 'PHONE', title: '联系电话', edit: 'text', width: 120 }
            , { field: 'IS_CONTRACT', title: '是否有合作协议', templet: '#checkboxContract', width: 180 }
            , { field: 'REMARK', title: '说明', edit: 'text', minWidth: 200 }
            , { title: '<span class="layui-badge layui-bg-green" style="cursor:default" href="#" onclick="company.addCompany(0);">+ 操作</span>', width: 120, templet: '#addDelCompany' }
          ]]
          , data: company.companyArr
          , done: function () {
              $("[data-field='ID']").css('display', 'none');
          }
            });

            //监听‘是否有合作协议’
            form.on('switch(ftContract)', function (obj) {
                if (obj.elem.checked == true) {
                    company.companyArr[this.value][this.name] = 1;
                    layer.tips("有合作协议", obj.othis);
                } else {
                    company.companyArr[this.value][this.name] = 0;
                    layer.tips("无合作协议", obj.othis);
                }
            });
        });
    },

    //添加一个单位
    addCompany: function (index) {
        var item = {
            'IS_CONTRACT': 1
        };
        company.companyArr.push(item);
        company.companyRender();
    },
    //删除指定单位
    deleteCompany: function (index) {
        company.companyArr.splice(index, 1);
        company.companyRender();
    },
    //从控件中获得参与单位集合信息
    getCompanyArr: function () {
        var companys = $("#Companys").val();
        if (!company.isEmpty(companys)) {
            return $.parseJSON(companys);
        } else { return []; }
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
