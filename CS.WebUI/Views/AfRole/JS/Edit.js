layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;

});

$(function () {
    var ztreenode = JSON.parse($("#dicMenus").val());
    $.fn.zTree.init($("#ztreeId"), setting, ztreenode);

    FuncInitZtreeValue("MENU_IDS", "ztreeId");
});

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../AfRole/Edit";
        SaveForm('form', url);
        return;
    });
}

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
        chkboxType: { "Y": "ps", "N": "ps" },
    },
    callback: {
        onCheck: onCheck
    }
}

function onCheck(event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj(treeId);
    var nodeval = FuncAssignment(treeId, zTree.getCheckedNodes());
    $("#MENU_IDS").val(nodeval.ids);
}

function FuncAssignment(treeId, nodes) {
    var names = "";
    var ids = "";
    for (var i = 0, l = nodes.length; i < l; i++) {
        names += nodes[i].name + ",";
        ids += nodes[i].id + ",";
    }
    if (names.length > 0) {
        names = names.substring(0, names.length - 1);
        ids = ids.substring(0, ids.length - 1);
    }
    return { "names": names, "ids": ids };
}

function FuncInitZtreeValue(id, ztreeId) {
    var control = $("#" + id);
    var ztree = $.fn.zTree.getZTreeObj(ztreeId);

    var val = control.attr("value");
    if (val === null || val === "" || val === undefined) return;
    var idArrays = val.split(",");
    for (var i = 0; i < idArrays.length; i++) {
        //获取节点
        var node = ztree.getNodesByParam("id", idArrays[i])[0];
        //选中节点
        //勾选单个节点
        if (node == null) {
            continue;
        }
        ztree.checkNode(node, true, false);
    }
}