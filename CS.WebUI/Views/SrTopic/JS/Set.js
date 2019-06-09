
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrTopic/Set";
        //#region 选择的人员信息
        //#endregion
        var nodes = getNodeUsers();
        $("#SelectExpert").val(nodes);
        //获取锁配置的评审规则
        var items = getNodeSubItems();
        $("#SubItems").val(items);
        SaveForm('form', url);
        return;
    });
}

///获取选中的评审规则
function getNodeSubItems() {
    var result = [];
    var trs = $("#subitemTable tbody").find('tr');
    trs.each(function (index) {
        index = index + 1;
        var sub = $('#selectID' + index);
        var subItem = $('#selectItem' + index); 
        var weight = $('#txtWeight' + index); 
        var remark = $('#txtRemark' + index); 
        var node = {
            ID: 0,
            TOPIC_ID: 0,
            SUB_ITEM_ID: subItem.val(),
            WEIGHT: weight.val(),
            REMARK: remark.val()
        };
        result.push(node);
    });
    return JSON.stringify(result);
}
///获取选中的评审老师
function getNodeUsers() {
    var result = [];
    $("#ulUsers li").each(function () {
        var n = $(this);
        var node = {
            ID: 0,
            TOPIC_ID: 0,
            USER_ID: n.attr("uid")
        };
        result.push(node);
    });
    return JSON.stringify(result);
}

function SetChooseUser() {
    layui.use(['laytpl','form'], function(){
        var laytpl = layui.laytpl,
            form = layui.form;
           
        var result = [];
        $("#chooseUser li").each(function () {
            var n = $(this);
            var key = n.find(".badge");
            var node = {
                id: n.attr("uid").replace("user_",""),
                name:n.attr("name")
            };
            var isexsit = isExsit(n.attr("uid").replace("user_", ""));
            if (!isexsit) {
                result.push(node);
            }
        });
            var data = { //数据
                "list":result
            }
            var getTpl = userItem.innerHTML
                ,view = $("#ulUsers");
            laytpl(getTpl).render(data, function(html){
                view.append(html);
            });
        form.render();
        layer.closeAll();

    })
}

function isExsit(uid) {
    return $("#ulUsers li[uid='" + uid + "']").length > 0;
}

layui.use(['form', 'layer', 'jquery', 'layedit', 'laydate'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery
    layedit = layui.layedit, laydate = layui.laydate;
    var remark = layedit.build('REMARK');
    $("#tipUser").on("click",
        function() {
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
    //提交
    form.on('submit(submit)', function (data) {//验证提交
        save();//保存
    });

    bindUsers();
    bindSubItems();
});

//绑定已选择的评审专家
function bindUsers() {
    layui.use(['laytpl', 'form'], function () {
        var laytpl = layui.laytpl,
            form = layui.form;
        $.post("../SrTopic/GetTopicExperts",
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

//绑定已设置的评审规则

function bindSubItems() {
    layui.use(['laytpl', 'form'], function () {
        var laytpl = layui.laytpl,
            form = layui.form;
        $.post("../SrTopic/GetTopicSubItems",
            {
                topicId: $("#ID").val()
            },
            function (_r) {
                console.log(_r)
                $.each(_r, function (index, value) {
                    addRow(this);
                });
            },
            "json");
    })

}

var setting = {
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
            var data= {
                list:usernodes
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
function GetAllUser() {
    layui.use('form', function() {
        var form = layui.form;
        var url = "/SrTopic/GetBraceUserAndDepTree";
        $.ajax({
            url: url,
            type: "post",
            beforeSend: function(XMLHttpRequest) {
                // $("#userList").html(common.loading);
            },
            success: function(data) {
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
                $.fn.zTree.init($("#userList"), setting, zNodes);
                form.render();
            }
        });
    })
    }
