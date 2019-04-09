layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;

    //自定义验证规则
    form.verify({
        title: function (value) {
            if (value.length == 0) {
                return '标题不能为空';
            }
        }
        , content: function (value) {
            if (value.length == 0) {
                return '公告内容不能为空';
            }
        }
        , summary: function (value) {
            if (value.length == 0) {
                return '摘要不能为空';
            }
        }
    });
    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

});
//提交方法
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfBulletin/Edit";
        SaveForm('form', url);
        return;
    });
}
//删除上传附件
function deleteFile(ID, FILE_PATH) {
    //取消删除文件源的操作。

    var $tr = $(curCtr).parent().parent();
    $tr.remove();

    //从附件隐藏控件中删除对应的FILE_PATH
    if (FILE_PATH != null && FILE_PATH.length > 0) {
        var files = $("#FILES").val();
        var reg = new RegExp("," + FILE_PATH, "g");
        files = files.replace(reg, '');
        $("#FILES").val(files);
    }
    layer.alert("删除成功！");
}


//ztree树形结构
$(function () {
    var zNodes = JSON.parse($("#DepartmentSelect").val());
    debugger;
    //$.comboztree("RECV_DEPT_IDS", { ztreenode: zNodes });
    $.fn.zTree.init($("#ztreeId"), setting, zNodes);
    FuncInitZtreeValue("RECV_DEPT_IDS", "ztreeId");
});

var setting = {
    view: {
        dblClickExpand: false,
        showLine: false
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    check: {
        enable: true,
        chkboxType: { "Y": "s", "N": "s" },
    },
    callback: {
        onCheck: onCheck
    }
}

//监听选择事件
function onCheck(event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj(treeId);
    var nodeval = FuncAssignment(treeId, zTree.getCheckedNodes());
    $("#RECV_DEPT_IDS").val(nodeval.values);
}

function FuncAssignment(treeId, nodes) {
    var names = "";
    var ids = "";
    var values = "";
    for (var i = 0, l = nodes.length; i < l; i++) {
        names += nodes[i].name + ",";
        ids += nodes[i].id + ",";
        if (nodes[i].value != null) {
            values += nodes[i].value + ",";
        }
    }
    if (names.length > 0) {
        names = names.substring(0, names.length - 1);
        ids = ids.substring(0, ids.length - 1);
    }
    if (values.length > 0) {
        values = values.substring(0, values.length - 1);
    } else {
        values = ids;
    }
    return { "names": names, "ids": ids, "values": values };
}
///设置选中项
function FuncInitZtreeValue(id, ztreeId) {
    var control = $("#" + id);
    var ztree = $.fn.zTree.getZTreeObj(ztreeId);
    var val = control.attr("value");
    if (val === null || val === "" || val === undefined) {
        return;
    }
    var idArrays = val.split(",");
    for (var i = 0; i < idArrays.length; i++) {
        //获取节点
        var node = ztree.getNodesByParam("value", idArrays[i])[0];
        if (node == undefined) {
            continue;
        }
        //选中节点
        //勾选单个节点
        if (node === null) {
            return;
        }
        ztree.checkNode(node, true, false);
    }
}