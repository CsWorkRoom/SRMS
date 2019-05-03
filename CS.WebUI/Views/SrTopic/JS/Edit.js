layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;
    var index = layedit.build('remark');
    //初始化开始时间
    laydate.render({
        elem: '#startTime'
    });
    //初始化结束时间
    laydate.render({
        elem: '#endTime'
    });
    $("#tipUser").on("click", function () {
        layer.tips($("#tipUserShow").html(), this, {
            tips: [2, '#fff'],
            //shade: [0.1, '#fff'],//增加遮罩层，后面控件不能操作
            time: 0,
            closeBtn: 1,
            area: ['500px', '200px']
        });

    })
    //自定义验证规则
    form.verify({
        name: function (value) {
            if (value.length == 0) {
                return '名称不能为空';
            }
        }
        , time: function (value) {
            if (value.length == 0) {
                return '时间不能为空';
            }
        }
        , remark: function (value) {
            if (value.length == 0) {
                return '备注不能为空';
            }
        }
        , type: function (value) {
            if (value.length == 0) {
                return '课题类型不能为空';
            }
        }
    });
    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

});

//切换是否第一负责人
function linkShow(span) {
    var spanHtml = $(span);
    if (spanHtml.html() == "普") {
        spanHtml.html("主");
    }
    else {
        spanHtml.html("普");
    }
}
//删除已选择的参与人员
function deleteLi(uid) {
    $("#ulUsers li").filter(function (index) {
        return $(this).attr("uid") == uid;
    }).remove();
}

//选择参与人员
function selectedLi(uid, uname) {
    var data = { //数据
        "list": [{ "id": uid, "name": uname }]
    }
    var isexsit = isExsit(uid);
    if (!isexsit) {
        layui.use('laytpl',
            function() {
                var laytpl = layui.laytpl;
                var getTpl = userItem.innerHTML, view = $("#ulUsers");
                laytpl(getTpl).render(data,
                    function(html) {
                        view.append(html);
                    });
                layer.closeAll('tips');
            })
    } else {
        layer.msg('此用户已存在');
    }
}

//判断是否存在
function isExsit(uid) {
    return $("#ulUsers li[uid='" + uid + "']").length > 0;
}

//ztree树形结构
$(function () {
    var zNodes = JSON.parse($("#TypeSelect").val());
    $.fn.zTree.init($("#ztreeId"), setting, zNodes);
    FuncInitZtreeValue("TOPIC_TYPE_ID", "ztreeId");
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
        chkStyle: "radio",
        chkboxType: { "Y": "s", "N": "s" },
        radioType: "all"
    },
    callback: {
        onCheck: onCheck
    }
}

//监听选择事件
function onCheck(event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj(treeId);
    var nodeval = FuncAssignment(treeId, zTree.getCheckedNodes());
    $("#TOPIC_TYPE_ID").val(nodeval.values);
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