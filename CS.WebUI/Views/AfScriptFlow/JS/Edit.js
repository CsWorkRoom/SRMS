function offsetHelp() {
    layui.use(['layer'], function () {
        var layer = layui.layer;
        layer.alert("自动创建口径任务时，任务的截止日期根据此偏移量计算得到：如果是月口径，指偏移的月份数；如果是日口径，则指偏移的天数。正数往后偏移；负数向前偏移。");
    });
}

layui.use(['form', 'layer', 'jquery'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery;
    $(".layui-fluid").css("overflow", "hidden");

    //表单验证
    form.verify({
        name: function (val, item) {
            if (val === "" || val === undefined || val === null)
                return "脚本流名称不能为空!";
        },
        cron: function (val, item) {
            if (val === "" || val === undefined || val === null)
                return "时间表达式不能为空!";
        },
        retry_time: function (val, item) {
            if (val === "" || val === undefined || val === null)
                return "失败重试次数不能为空!";
        }
    });
});



$(function () {
    Panel.init();
    $.get("../AfScriptFlow/SciptFlowZtree", {}, function (e) {
        var currztree = new ztree("ztree", e);
        currztree.destroy();
        currztree.init();

        Panel.initData();
    }, "json")
});

/**
 * 初始化面板（初始化，初始化控件，初始化事件，初始化数据）
 */
var Panel = {
    init: function () {
        this.initControl();
        this.initEvent();
    },
    //实例化控件
    initControl: function () {
        //实例化控件
        $.comboztree("TYPE_ID", {
            ztreenode: JSON.parse($("#dictype").val())
        })
    },
    //初始化事件
    initEvent: function () {
        jsPlumbNode.jsPlumbOnDragStop();
    },
    //初始化数据
    initData: function () {
        var treeObj = $.fn.zTree.getZTreeObj("ztree");
        var data = JSON.parse($("#dicFlowNode").val());

        var tsknode = new TaskNode();
        //画节点
        for (var i = 0; i < data.length; i++) {
            var treeNode = treeObj.getNodeByParam("id", data[i].NODE_ID, null);
            if (treeNode == null || treeNode == undefined) continue;
            var X = data[i].DIV_X;
            var Y = data[i].DIV_Y;
            var _ztreeObj = {};
            _ztreeObj.ztree_parent_id = treeNode.pId;
            _ztreeObj.ztree_parentTid = treeNode.parentTId;
            //
            var _ztreeObj = $.extend(_ztreeObj, data[i]);
            //
            var treename = treeNode.name.replace('<span style="color: whitesmoke;background-color: darkred;">', '');
            treename = treename.replace("</span>", "");

            //添加节点
            tsknode.AddNode(_ztreeObj, treeNode.id, treename, treename, X, Y);
            //删除树节点
            treeObj.removeNode(treeNode);
        }
        //画线
        for (var i = 0; i < data.length; i++) {
            //前节点为空，剔除
            if (data[i].PRE_NODE_IDS == null) continue;
            var ids = data[i].PRE_NODE_IDS.split(",");
            var sourceId = jsPlumbNode.GetNodeId(data[i].NODE_ID);
            for (var z = 0; z < ids.length; z++) {
                var targetId = jsPlumbNode.GetNodeId(ids[z]);
                jsPlumbNode.EndPoint_ConnectLine(targetId, sourceId);
            }
        }
        
    },
    //结束编辑表单提交
    endSumbit: function () {
        var newData = jsPlumbNode.nodeGetData();
        $("#dicFlowNode").val(JSON.stringify(newData));
    },
    //全局变量-》节点是否点击
    VariableNodeIsActice: false,
}
/**
 * ztree
 * @@param ztreeid   树id
 * @@param ztreenode 树的数据集
 */
function ztree(ztreeid, ztreenode) {

    var tsknode = new TaskNode();

    var setting = {
        view: {
            dblClickExpand: false,
            showLine: false,
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom
        },
        data: {
            simpleData: {
                enable: true
            }
        },
    }

    this.ztreenode = ztreenode;
    this.init = function () {
        $.fn.zTree.init($("#" + ztreeid), setting, this.ztreenode);
        //模糊查询
        fuzzySearch("ztree", "#search", null, true);
    },
    this.destroy = function () {
        var treeObj = $.fn.zTree.getZTreeObj(ztreeid);
        if (treeObj != null)
            treeObj.destroy();
    }

    /**
     * 
     * @@param treeId
     * @@param treeNode
     */
    function addHoverDom(treeId, treeNode) {
        //禁止父节点进行编辑
        if (treeNode.dataType == "Type") return;
        var sObj = $("#" + treeNode.tId + "_span");
        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;
        var addbtn = "<span id='addBtn_" + treeNode.tId + "',style='width:10px;height:10px;color:green'>添加</span>"
        sObj.after(addbtn);
        var btn = $("#addBtn_" + treeNode.tId);
        btn.bind("click", function () {;
            var _ztreeObj = {};
            _ztreeObj.ztree_parent_id = treeNode.pId;
            _ztreeObj.ztree_parentTid = treeNode.parentTId;
            treeNode.DIV_X == undefined ? _ztreeObj.DIV_X = 500 : _ztreeObj.DIV_X = treeNode.DIV_X;
            treeNode.DIV_Y == undefined ? _ztreeObj.DIV_Y = 40 : _ztreeObj.DIV_Y = treeNode.DIV_Y;

            //获取文本信息，过滤ztree传递过来的div或span
            var treename = treeNode.name;
            if (treeNode.name.indexOf('span') > -1 || treeNode.name.indexOf('div') > -1)
                treename = $(treeNode.name).text();

            //添加节点
            tsknode.AddNode(_ztreeObj, treeNode.id, treename, treename, _ztreeObj.DIV_X, _ztreeObj.DIV_Y);
            //删除树节点 删除时候存在异步延迟，造成浏览器报错，所以需要用setTimeout延迟删除
            var ztreeBase = $.fn.zTree.getZTreeObj("ztree");
            var deltreeNode = ztreeBase.getNodeByTId(treeNode.tId);
            ztreeBase.removeNode(deltreeNode);
        });
    }
    /**
     * 
     * @@param treeId
     * @@param treeNode
     */
    function removeHoverDom(treeId, treeNode) {
        $("#addBtn_" + treeNode.tId).unbind().remove();
    };

}
/**
 * 头部表单变化
 * @@param active     原选择对象
 * @@param clickobj   被点击对象
 * @@param clicktype  点击类型（click-》点击|cancel-》取消）
 */
function TopChangePanel(active, clickobj, clicktype) {
    var type = {
        cancel: "cancel",
        click: "click"
    };

    var top = "del|save|update|add|batch|cancel"
    //标识
    var liattr = "data-info";
    //内容区域
    var contontobj = $(".left");
    //
    var rightobj = $(".right");
    //
    var TopPanelHide = $("#TopPanelHide");
    //树对象
    var treeObj = $.fn.zTree.getZTreeObj("ztree");

    this.active = active;
    this.clickobj = clickobj,
    this.clicktype = clicktype;
    this.topchange = function () {
        if (top.indexOf(clickobj.attr(liattr)) > -1) return;
        active.removeClass("active");
        if (this.clicktype === type.click) {
            rightobj.removeClass("all");
            contontobj.removeClass("hide");
            clickobj.addClass("active");
        } else {
            rightobj.addClass("all");
            contontobj.addClass("hide");

        }
    },
        this.listchange = function () {
            var datainfo = "list";
            if (active.length > 0)
                TopPanelHide.append($("#" + active.attr(liattr)));
            contontobj.empty();
            if (this.clicktype == type.click) {
                contontobj.append($("#" + datainfo));
                TopPanelHide.remove("#" + datainfo);
            }
        },
        this.infochange = function () {
            var datainfo = "sriptFlow";
            if (active.length > 0)
                TopPanelHide.append($("#" + active.attr(liattr)));
            contontobj.empty();
            if (this.clicktype == type.click) {
                contontobj.append($("#" + datainfo));
                TopPanelHide.remove("#" + datainfo);
            }
            //layui.ready();
        },
        this.Helpchange = function () {
            var datainfo = "Help";
            if (active.length > 0)
                TopPanelHide.append($("#" + active.attr(liattr)));
            contontobj.empty();
            if (this.clicktype == type.click) {
                contontobj.append($("#" + datainfo));
                TopPanelHide.remove("#" + datainfo);
            }
        },
        this.delchange = function () {
            var node = $(".right >div[class*='active']");
            if (node.length == 0) return;
            if (confirm("请问是否删除节点")) {
                for (var i = 0; i < node.length; i++) {
                    //新增节点
                    var ztreenode = {
                        id: node[i].getAttribute("data_id"),
                        pId: node[i].getAttribute("data_parentid"),
                        name: node[i].innerHTML,
                        DIV_X: node[i].offsetLeft,
                        DIV_Y: node[i].offsetTop
                    };
                    //获取父节点
                    var parentNode = treeObj.getNodeByTId(node[i].getAttribute("data_parenttid"));
                    //
                    treeObj.addNodes(parentNode, ztreenode);
                    //
                    jsPlumbNode.removeNode(node[i].id);
                }
            }
        },
        this.savechange = function (formid, url) {
            Panel.endSumbit();
            if (confirm("请问是否保存!")) {
                layui.use(['form', 'layer', 'jquery'], function () {
                    var form = layui.form, layer = layui.layer, $ = layui.$;
                    $.post(url, $("#" + formid).serialize(), function (result) {
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
                });
            }
        },
        this.betchchange = function () {
            //新增取消节点
            var li = $('<li data-info="cancel"><i class="layui-icon iconfont layui-icon-cancel"></i>取消</li>')
            $("li[data-info='Help']").before(li);
            //新增删除节点
            var li_del = $('<li data-info="del"><i class="layui-icon iconfont layui-icon-shanchu"></i>删除任务</li>');
            clickobj.before(li_del);
            //删除批量节点
            var li_betch = clickobj;
            li_betch.remove();
            //节点点击
            Panel.VariableNodeIsActice = true;
        },
        this.cancelchange = function () {
            //清楚选中节点
            $(".right > .active").removeClass("active");
            //删除取消节点
            var li = clickobj;
            li.remove();
            //删除删除节点
            var li_del = $("li[data-info='del']");
            li_del.remove();
            //新增批量节点
            var li_bet = $('<li data-info="batch"><i class="layui-icon iconfont layui-icon-piliangxiugaizuocedaohang"></i>批量处理</li>');
            $("li[data-info='Help']").before(li_bet);
            //取消节点点击
            Panel.VariableNodeIsActice = false;
        },
        this.panelchange = function () {
            switch (clickobj.attr(liattr)) {
                case "list":
                    this.listchange(); break;
                case "sriptFlow":
                    this.infochange(); break;
                case "Help":
                    this.Helpchange(); break;
                case "del":
                    this.delchange(); return;
                case "save":
                    this.savechange("Flowform", "Edit"); return;
                case "batch":
                    this.betchchange(); return;
                case "cancel":
                    this.cancelchange(); return;
                default: break;
            }
            this.topchange();
        }
}
/**
 * @@method 节点创建·
 */
function TaskNode() {
    this.AddNode = function (obj, id, title, nodeName, left, top) {
        var nodeId = jsPlumbNode.GetNodeId(id);
        //创建节点在页面
        var node = jsPlumbNode.node(nodeId, obj, title, nodeName, left, top);
        //节点点击事件
        nodeEventClick(node);
        //保存数据
        jsPlumbNode.nodeDataSave(node, obj);
        //添加节点对应锚点
        jsPlumbNode.EndPoint_add(nodeId);
        //节点移动
        jsPlumbNode.nodeDraggable(nodeId);
    }

    function nodeEventClick(node) {
        node.bind("click", function () {
            var selectnode = stastcParam.Jsplumb.SelectNode;
            if (!$(this).hasClass(selectnode))
                $(this).addClass(selectnode);
            else
                $(this).removeClass(selectnode);
            if (Panel.VariableNodeIsActice) {
                if (!$(this).hasClass("active"))
                    $(this).addClass("active");
                else
                    $(this).removeClass("active");
            }
        });
    }
}

//事件绑定 
$(".top").bind("click", function (e) {
    var active = $(".w li[class='active']");
    //获取选中的对象
    var target = GetTarget(e.target || e.srcElement);
    if (target == undefined) return;
    //
    var clickobj = $(target);
    //区分"点击"或“取消"
    var clicktype = "click"
    if (active.attr("data-info") === clickobj.attr("data-info"))
        clicktype = "cancel";

    var panel = new TopChangePanel(active, clickobj, clicktype);
    panel.panelchange();
    /**
     * @@method 获取<li data-info=''>对象
     * @@param 点击对象
     */
    function GetTarget(Target) {
        if (Target.tagName == "LI")
            return Target;
        else if (Target.tagName == "I")
            return Target.parentNode;
        else
            return undefined;
    }
})