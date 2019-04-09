/// <reference path="jsPlumbNode.js" />





var stastcParam = {
    Jsplumb: {
        SelectNode: "selectnode"
    }
}

var jsPlumbNode = {
    nodeId: "node_",
    panelNode: "nodePanel",
    dataId: "data",
    /**
    * @name   构建节点
    * @param id         节点id
    * @param ztreeObj   树节点对象(可为空)
    * @param title      节点标题
    * @param nodeName   节点名称
    * @param left       X坐标
    * @param top        Y坐标
    * @return           node节点对象
    */
    node: function (id, ztreeObj, title, nodeName, left, top) {
        if (left == undefined) left = 0;
        if (top == undefined) top = 0;
        if (ztreeObj == null) {
            ztreeObj = $.extend({}, ztreeObj, { ztree_parent_id: "", ztree_parentTid: "" });
        }

        var node = $("<div>", {
            class: "node",
            id: id,
            title: title,
            name: id,
            //作为定位表示
            data_node: "node",
            //节点id
            data_id: id.replace(this.nodeId, ""),
            //父节点id
            data_parentid: ztreeObj.ztree_parent_id,
            //
            data_parenttid: ztreeObj.ztree_parentTid,
            style: "position:absolute;"
        });
        //添加文字
        //var div = $("<div>", {
        //    class: "nodecell",
        //    style: "vertical-align: middle;text-align: center;display:table-cell"
        //});
        //div.text(nodeName);
        //div.appendTo(node);
        //添加文字
        node.text(nodeName)
        //定位节点位置
        node.css("left", left).css("top", top);
        node.appendTo("#" + this.panelNode);

        //node.bind("click", function () {
        //    var selectnode = stastcParam.Jsplumb.SelectNode;
        //    if (!$(this).hasClass(selectnode))
        //        $(this).addClass(selectnode);
        //    else
        //        $(this).removeClass(selectnode);

        //    if (!$(this).hasClass("active"))
        //        $(this).addClass("active");
        //    else
        //        $(this).removeClass("active");
        //});

        return node;
    },
    /**
     * @name  将数据保存在节点上
     * @param node    节点
     * @param obj     数据对象
     * @return        无
     */
    nodeDataSave: function (node, obj) {
        var d = node.data(this.dataId);
        if (d == undefined || d == null)
            node.data(this.dataId, obj);
        else
            node.data(this.dataid, $.extend({}, d, obj));
    },
    nodeGetData: function (node) {
        return node.data(this.dataid);
    },
    nodeInside: function (node) {

        var t = $("<div>", {
            style: "display:table-cell;vertical-align: middle;text-align: center;"
        });
        t.text(node.text());
        node.text("");

        var w = $("<div>", {
            style: "vertical-align: middle;text-align: center;"
        });

        w.appendTo(t);
        t.appendTo(node);

        node.unbind("click");
        node.bind("click", function () {

            //console.log(node.data("data"));
            var b = $("<div>", {
                class: "layui-btn-group"
            });
            var nodefunc = new nodeBth(node);
            var Distin = ["log", "start", "havingin"];
            for (var i = 0; i < Distin.length; i++) {
                var nodeBth = nodefunc.Distinguish(Distin[i]);
                nodeBth.appendTo(b)
            }

            var div = $("<div>", {

            });
            b.appendTo(div)
            $("<div><lable>任务名称：</lable>V网用户净增</div> <div>").appendTo(div);
            $("<div><lable>执行状态：</lable>等待</div>").appendTo(div);
            $("<div><lable>执行结果：</lable>脚本流【1】的实例【19】中的节点【1】的实例【20】的运行状态已经更新为【执行中】，下面将执行节点脚本内容。</div>").appendTo(div);
            layer.tips(div[0].outerHTML, "#" + node[0].id);


            function nodeBth(node) {
                this.TaskId = node.TaskId;
                //日志
                this.log_bth = function () {
                    var nodeBtn = $('<button class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon">&#xe60e;</i></button>');
                    nodeBtn.bind("click", function () {
                        OpenTopWindow("任务节点日志", 800, 300, "../AfScriptTaskFlowNodeLog/Index?TaskId=" + this.TaskId);
                    })
                    return nodeBtn;
                }
                //启动
                this.Start_bth = function () {
                    var nodeBtn = $('<button class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon">&#xe652;</i></button>');
                    nodeBtn.bind("click", function () {
                        layer.confirm('是否确定启动？', function (index) {
                            layer.msg("测试已经启动")
                        });
                    })
                    return nodeBtn;
                }
                //
                this.HavingIn_btn = function () {
                    var nodeBtn = $('<button class="layui-btn layui-btn-primary layui-btn-sm"><i class="layui-icon">&#xe651;</i></button>');
                    return nodeBtn;
                }
                this.Distinguish = function (Dis) {
                    switch (Dis) {
                        case "log":
                            return this.log_bth();
                        case "start":
                            return this.Start_bth();
                        case "havingin":
                            return this.HavingIn_btn();
                        default:
                            return null;
                    }
                }
            }
        });

    },
    formatNode: function () {
        var nodes = this.nodeGetAll();
        nodes.each(function (i, e) {
            var f = format($(e).position().left, $(e).position().top);
            jsPlumb.draggable(e.id);
        })

        function format(left, top) {
            var forleft = 0;
            var fortop = 0;
            var Vleft = 100;
            var Vtop = 100;

            var i = 0;
            while (forleft < left) {
                i++;
                forleft += Vleft * i;
            }

            i = 0;
            while (fortop < top) {
                i++;
                fortop += Vtop * i;
            }

            return { l: forleft, t: fortop };
        }
    },
    GetNodeId: function (id) {
        return this.nodeId + id;
    },
    //添加节点
    addNode: function (obj, id, title, nodeName, left, top) {
        var nodeId = this.GetNodeId(id);
        //创建节点在页面
        var node = this.node(nodeId, obj, title, nodeName, left, top);
        //保存数据
        this.nodeDataSave(node, obj);
        //添加节点对应锚点
        this.EndPoint_add(nodeId);
        //节点移动
        this.nodeDraggable(nodeId);
    },
    //添加展示节点
    addShowNode: function (id, title, nodeName, left, top, obj) {
        var nodeId = this.GetNodeId(id);
        //创建节点
        var node = this.node(nodeId, null, nodeName, nodeName, left, top);
        //
        this.nodeDataSave(node, obj);
        //添加节点对应锚点
        this.EndPoint_add(nodeId, {
            //是否可以拖动（作为连线起点）
            isSource: false,
            //是否可以放置（连线终点）
            isTarget: false,
            //
            endpoint: "Blank",
            //
            paintStyle: { fillStyle: "#f0ad4e", radius: 0 },
        });
        //深
        this.nodeInside(node);
        //
        this.nodeDraggable(nodeId);
    },
    //删除节点
    removeNode: function (nodeId) {
        //删除节点
        $("#" + nodeId).remove();
        //删除对应的锚点以及连接线
        this.jsPlumbRemove(nodeId);
    },
    nodeGetSelectNode: function (tagert) {
        return $("#" + this.panelNode + " >div[data_node='node'][class*='" + stastcParam.Jsplumb.SelectNode + "']");
    },
    //节点移动
    nodeDraggable: function (nodeId) {
        $("#" + nodeId).draggable({
            containment: "parent",
            stop: function () {//可以拖放的元素     拖放结束后触发事件
                // alert($(this).offset().left);
            }
        });
    },
    //获取所有节点
    nodeGetAll: function () {
        return $("#" + this.panelNode + " >div[data_node]");
    },
    //删除节点相关（连接线,锚点）
    jsPlumbRemove: function (nodeId) {
        jsPlumb.remove(nodeId);
    },
    //添加锚点
    EndPoint_add: function (nodeId, obj) {
        var Point = $.extend({}, EndPoint, obj);
        //加载锚点
        jsPlumb.addEndpoint(nodeId, { anchors: "RightMiddle" }, Point);
        jsPlumb.addEndpoint(nodeId, { anchors: "LeftMiddle" }, Point);
        //让锚点移动
        this.EndPoint_draggable(nodeId);
    },
    //锚点移动
    EndPoint_draggable: function (nodeId) {
        jsPlumb.draggable(nodeId);
    },
    //锚点连接线
    EndPoint_ConnectLine: function (beginNodeId, endNodeId) {

        var node = {
            source: beginNodeId,
            target: endNodeId
        }
        var sourceNode = $("#" + beginNodeId);
        var targetNode = $("#" + endNodeId);
        //节点对象
        node = $.extend(node, BaseLine);
        //线
        var conn = jsPlumb.connect(node);
    },
    //画线终止触发事件
    jsPlumbOnDragStop: function () {
        var connectline = this.EndPoint_ConnectLine;
        jsPlumb.bind("connectionDragStop", function (e) {
            //锚点不能连接已连接的节点另外一侧锚点
            if (e.sourceId == e.targetId) return jsPlumb.detach(e);
            //锚点不能同时连接同一节点锚点 获取原节点
            var line = jsPlumb.getConnections({
                source: e.sourceId,
                target: e.targetId
            });
            if (line.length >= 1) return jsPlumb.detach(e);

            //锚点不能同时连接同一节点锚点 获取原节点
            var oline = jsPlumb.getConnections({
                source: e.targetId,
                target: e.sourceId
            })
            if (oline.length >= 1) return jsPlumb.detach(e);

            //连接成线
            jsPlumbNode.EndPoint_ConnectLine(e.sourceId, e.targetId);
            jsPlumb.detach(e);
        });
    },
    //获取结果对象
    nodeGetData: function () {
        var obj = this.nodeGetAll();
        var nodelist = new Array();

        $(obj).each(function (i, e) {
            var NODE_ID = e.getAttribute("data_id");
            var DIV_X = e.offsetLeft;
            var DIV_Y = e.offsetTop;
            var lineList = jsPlumb.getConnections({ target: e.id });

            var perId = new Array();
            for (var i = 0; i < lineList.length; i++) {
                perId.push(lineList[i].source.getAttribute("data_id"));
            }
            if (perId.length == 0) perId = null;
            else perId = perId.join(",");
            nodelist.push(new FlowNode(NODE_ID, perId, DIV_X, DIV_Y));
        });

        return nodelist;

        function FlowNode(NODE_ID, PRE_NODE_IDS, DIV_X, DIV_Y) {
            this.NODE_ID = NODE_ID,
                this.PRE_NODE_IDS = PRE_NODE_IDS,
                this.DIV_X = DIV_X,
                this.DIV_Y = DIV_Y
        }
    }
};

//实心圆样式
var EndPoint = {
    //端点的形状
    endpoint: ["Dot", { radius: 3 }],
    //锚点样式 (PS:如果paintStyle提供并且提供，则优先)
    //endpointStyle:"",
    //锚点悬停样式
    //endpointHoverStyle: "",
    //端点的颜色样式
    paintStyle: { fillStyle: "#f0ad4e", radius: 6 },
    //端点悬停样式
    hoverPaintStyle: {
        fillStyle: "#216477",
        strokeStyle: "#216477"
    },
    //线 线样式
    connectorStyle: { strokeStyle: "#8ca8f1", lineWidth: 1 },	  //连接线的颜色，大小样式
    //线 连接线的样式种类
    connector: ["Flowchart"], //连接线的样式种类有[Bezier],[Flowchart],[StateMachine ],[Straight ]       ,{ stub: [40, 60], gap: 10, cornerRadius: 5, alwaysRespectStubs: true }
    // 设置连接点最多可以连接几条线
    maxConnections: -1,
    //叠加层
    connectorOverlays: [
        //["Label", { label: "删除", location: 0.5, id: "ceshi" }],
        //["Arrow", { width: 10, length: 15, location: 0.95, paintStyle: { fillStyle: "#8ca8f1" } }]
    ],
    scope: "blueline",
    //是否可以拖动（作为连线起点）
    isSource: true,
    //是否可以放置（连线终点）
    isTarget: true
};


var BaseLine = {
    //线 连接线的样式种类有[Bezier],[Flowchart],[StateMachine ],[Straight ]
    connector: ["Flowchart"],
    //线 线样式
    connectorStyle: {
        lineWidth: 100,
        strokeStyle: "#8ca8f1",
        //joinstyle: "round",
        //outlineColor: "white",
        //outlineWidth: 2
    },
    //线悬停样式
    connectorHoverStyle: {
        lineWidth: 20,
        strokeStyle: "#216477",
        //outlineWidth: 2,
        //outlineColor: "white"
    },
    //锚点 端点形状
    endpoint: "Blank",//blank 空白的, Dot 点   ["Dot", { radius: 8 }],
    //锚点 设置连接锚点位置
    anchors: ["Right", "Left"],
    //端点的颜色样式
    paintStyle: {
        fillStyle: "#8ca8f1",
        radius: 10,
        strokeStyle: "#8ca8f1",
        lineWidth: 1
    },
    ////线 叠加层
    overlays: [     //线上内容
        //["Label", { label: "删除", location: 0.5, id: "ceshi" }],
        ["Arrow", { width: 10, length: 15, location: 1, paintStyle: { fillStyle: "#8ca8f1" } }]
    ],
};

