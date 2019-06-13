

function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopic/Edit";
        //#region 选择的人员信息
        //#endregion
        var nodes = getNodeUsers();
        $("#SelectUser").val(nodes);
        SaveForm('form', url);
        return;
    });
}
async  function SaveFlowForm() {
    var url = "../SrTopic/Edit";
    var nodes = getNodeUsers();
    $("#SelectUser").val(nodes);
    var data = {
        ID: $("#ID").val(),
        CREATE_USER_ID: $("#CREATE_USER_ID").val(),
        CREATE_TIME: $("#CREATE_TIME").val(),
        NAME: $("#NAME").val(),
        TOPIC_TYPE_ID: $("#TOPIC_TYPE_ID").val(),
        START_TIME: $("#START_TIME").val(),
        END_TIME: $("#END_TIME").val(),
        REMARK: $("#REMARK").val(),
        SelectUser: $("#SelectUser").val()
    };

    var p = new Promise((resolve, reject) => $.post(url, data,
        function (_r) {
            var obj = {
                ID: _r.Result
            };
            var res = {
                IsSuccess: _r.IsSuccess,
                Message: _r.Message,
                Result: obj
            };
            resolve(res);
        },
        "json"));
    return await p
   
    
}
function getNodeUsers() {
    var result = [];
    $("#ulUsers li").each(function () {
        var n = $(this);
        var key = n.find(".badge");
        var node = {
            ID: 0,
            TOPIC_ID: 0,
            USER_ID: n.attr("uid"),
            IS_PERSON_LIABLE: key.text() == "主" ? 1 : 0
        };
        result.push(node);
    });
    return JSON.stringify(result);
}

function getCurrDay() {
    var data = new Date();
    var yearCurr = data.getFullYear();
    var monthCurr = data.getMonth();
    var dayCurr = data.getDay();
    var monLength = monthCurr.toString().length;
    if (monLength == 1) {
        monthCurr = "0" + monthCurr;
    }
    var dayLength = dayCurr.toString().length;
    if (dayLength == 1) {
        dayCurr = "0" + dayCurr;
    }
    return yearCurr + "-" + monthCurr + "-" + dayCurr;
}

layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;

    var currDay = getCurrDay();
    //初始化开始时间
    laydate.render({
        elem: '#START_TIME',
        value: currDay
        , isInitValue: true
    });
    //初始化结束时间
    laydate.render({
        elem: '#END_TIME',
        format: 'yyyy-MM-dd',
        isInitValue: true,
        value: currDay
    });
    var remark = layedit.build('REMARK');

    $("#tipUser").on("click",
        function () {
            var url = $("#add_users");

            layer.open({
                type: 1,
                shade: false,
                offset: ['5%', '20%'],
                // shade: [0.1, '#fff'],
                area: ['600px', '450px'],
                title: '人员管理', //不显示标题
                content: url //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响

            });
            $("#chooseUser").html("");
            GetAllUser();
        });
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
        , type: function (value) {
            if (value.length == 0) {
                return '课题类型不能为空';
            }
        },
        remark:
        function (value) {
            layedit.sync(remark);
        }
    });
    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

    bindUsers();
});

function bindUsers() {
    layui.use(['laytpl', 'form'], function () {
        var laytpl = layui.laytpl,
            form = layui.form;
        $.post("../SrTopic/GetTopicUsers",
            {
                topicId: $("#ID").val()
            },
            function (_r) {
                var data = { //数据
                    "list": _r
                }
                var getTpl = userItem.innerHTML
                    , view = $("#ulUsers");
                laytpl(getTpl).render(data, function (html) {
                    view.append(html);
                });
                form.render();
            },
            "json");
    })

}

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
    $("#chooseUser li").filter(function (index) {
        return $(this).attr("uid") == uid;
    }).remove();
}
//删除已选择的参与人员
function deleteCheckLi(uid) {
    $("#ulUsers li").filter(function (index) {
        return $(this).attr("uid") == uid;
    }).remove();
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

function SetChooseUser() {
    layui.use(['laytpl', 'form'], function () {
        var laytpl = layui.laytpl,
            form = layui.form;

        var result = [];
        $("#chooseUser li").each(function () {
            var n = $(this);
            var key = n.find(".badge");
            var node = {
                id: n.attr("uid").replace("user_", ""),
                name: n.attr("name")
            };
            var isexsit = isExsit(n.attr("uid").replace("user_", ""));
            if (!isexsit) {
                result.push(node);
            }
        });
        var data = { //数据
            "list": result
        }
        var getTpl = userItem.innerHTML
            , view = $("#ulUsers");
        laytpl(getTpl).render(data, function (html) {
            view.append(html);
        });
        form.render();
        layer.closeAll();

    })
}

var settingUser = {
    callback: {
        onCheck: function (event, treeId, treeNode) {
            var treeObj = $.fn.zTree.getZTreeObj("userList");
            var nodes = treeObj.getCheckedNodes(true);
            var usernodes = [];
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].id.indexOf("user") >= 0) {
                    usernodes.push(nodes[i]);
                }
            }
            var data = {
                list: usernodes
            }
            layui.use('laytpl',
                function () {
                    var laytpl = layui.laytpl;
                    $("#chooseUser").html("");
                    var getTpl = chooseItem.innerHTML, view = $("#chooseUser");
                    laytpl(getTpl).render(data,
                        function (html) {
                            view.append(html);
                        });
                })
        }
    },
    view: {
        fontCss: {
            //"padding-left": "5px",
            "color": "#73879C",
            "font-size": "15px !important"
        },
        showIcon: true
    },
    check: {
        enable: true
    },
    data: {
        simpleData: {
            enable: true
        }
    }
};
function GetAllUser() {
    layui.use('form', function () {
        var form = layui.form;
        var url = "/SrTopic/GetBraceUserAndDepTree";
        $.ajax({
            url: url,
            type: "post",
            beforeSend: function (XMLHttpRequest) {
                // $("#userList").html(common.loading);
            },
            success: function (data) {
                var data = data;
                var zNodes = [];

                for (var i = 0; i < data.length; i++) {
                    var obj = {
                        id: data[i].id,
                        pId: data[i].pId,
                        name: data[i].name,
                        open: false,
                        icon: "../../Content/Images/organ1.png",
                        sort: data[i].ORGANL,
                    }
                    if (data[i].OBJECT_SORT == "1") {
                        obj.icon = "../../Content/Images/organ2.png";
                    } else if (data[i].OBJECT_SORT == "2") {
                        obj.icon = "../../Content/Images/organ3.png";
                    } else if (data[i].OBJECT_SORT == "6") {
                        obj.icon = "../../Content/Images/person-1.png";
                    }
                    if (obj.id == 0) {
                        obj.open = true;
                    }
                    if (data[i].id.indexOf("user") >= 0) {
                        obj.icon = "../../Content/Images/person-1.png";
                    }
                    zNodes.push(obj);
                }
                //  MsgInfo.nodes = zNodes;
                $.fn.zTree.init($("#userList"), settingUser, zNodes);
                form.render();
            }
        });
    })
}