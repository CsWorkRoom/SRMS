layui.use(["laydate", 'form', 'jquery', 'element'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery; element = layui.element; laydate = layui.laydate;
    laydate.render({
        elem: '#REFERENCE_DATE'
    });
    $(".layui-fluid").css("overflow", "hidden");
});

$("#btnStart").bind("click", function () {
    var btn = $(this);
    if (btn.attr("data_nodeId") == undefined) {
        //console.log("系统异常,请联系管理员");
        return;
    }
    if (confirm("请问是否重启任务!")) {
        $.post("../AfScriptTask/TaskNodeRestart", {
            reference_date: $("#REFERENCE_DATE").val(),
            content: $("#CONTENT").val(),
            nodeId: btn.attr("data_nodeId"),
            taskId: $("#TASK_ID").val()
        }, function (_r) {
            if (_r.IsSuccess) {
                btn.attr("disabled", "disabled");
                $("#CONTENT").attr("disabled", "disabled");
                $("#REFERENCE_DATE").attr("disabled", "disabled");
            }
            layer.alert(_r.Message, { icon: 1 });
            layer.closeAll();
        }, "json")
    }
});

$(function () {
    Panel.init();
});


var Panel = {
    init: function () {
        this.initControl();
        this.initEvent();
        this.initData();
    },
    //实例化控件
    initControl: function () {
        //实例化控件
        $.comboztree("TYPE_ID", {
            ztreenode: JSON.parse($("#dictype").val())
        })
    },
    initEvent: function () {
        jsPlumbNode.jsPlumbOnDragStop();
    },
    //初始化数据
    initData: function () {
        var dicNode = $("#dicFlowNode").val();
        if (dicNode === undefined || dicNode === "") return;
        //解析
        var data = JSON.parse(dicNode);
        var taskNode = new TaskNode();
        //画节点
        for (var i = 0; i < data.length; i++) {
            taskNode.AddNode(data[i]);
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
    endSumbit: function () {
        var newData = jsPlumbNode.nodeGetData();
        $("#dicFlowNode").val(JSON.stringify(newData));
    }
}


//事件绑定
$(".top li").bind("click", function () {
    var active = $(".w li[class='active']");
    var clickobj = $(this);
    //区分"点击"或“取消"
    var clicktype = "click"
    if (active.attr("data-info") === clickobj.attr("data-info"))
        clicktype = "cancel";

    var panel = new TopChangePanel(active, clickobj, clicktype);
    panel.panelchange();
});
//选项卡
function TopChangePanel(active, clickobj, clicktype) {
    var type = {
        cancel: "cancel",
        click: "click"
    };
    var top = "del|save|update|add"
    var liattr = "data-info";
    var contontobj = $(".left");
    var rightobj = $(".right");
    var TopPanelHide = $("#TopPanelHide");

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
                    jsPlumbNode.removeNode(node[i].id);
                }
            }
        }
    this.sriptFlowLogchange = function () {
        OpenTopWindow("任务组日志", 0, 0, "../AfScriptTaskLog/index?TaskId=" + $("#TASK_ID").val());
    },
        this.panelchange = function () {
            switch (clickobj.attr(liattr)) {
                case "sriptFlowLog":
                    this.sriptFlowLogchange(); return;
                case "sriptFlow":
                    this.infochange(); break;
                case "Help":
                    this.Helpchange(); break;
                case "Refresh":
                    window.location.reload(); return;
                default: break;
            }
            this.topchange();
        }
}
function TaskNode() {
    this.AddNode = function (obj) {
        //参数变量  @@nodeId 节点ID @@nodename 节点名称
        var nodeId = jsPlumbNode.GetNodeId(obj.NODE_ID);
        var nodeName = obj.NODE_NAME;
        var left = (obj.DIV_X == 0 ? 466 : obj.DIV_X);
        var top = (obj.DIV_Y == 0 ? 155 : obj.DIV_Y);
        //创建节点
        var node = jsPlumbNode.node(nodeId, {}, nodeName, nodeName, left, top);
        //节点样式
        NodeStyle(node, obj);
        //保存节点
        jsPlumbNode.nodeDataSave(node, obj);
        //添加节点对应锚点
        jsPlumbNode.EndPoint_add(nodeId, {
            //是否可以拖动（作为连线起点）
            isSource: false,
            //是否可以放置（连线终点）
            isTarget: false,
            //
            endpoint: "Blank",
            //
            paintStyle: { fillStyle: "#f0ad4e", radius: 0 },
        });
        //节点扩展
        NodeInside(node);
        //添加节点滑动时间
        jsPlumbNode.nodeDraggable(nodeId);
    }
    //节点样式
    function NodeStyle(node, obj) {
        //不同的状态,节点样式不同
        if (obj.RUN_STATUS == 0)
            return node.addClass("nodewiat");
        if (obj.RUN_STATUS == 1)
            return node.addClass("nodeHavein");
        if (obj.IS_SUCCESS === 1)
            return node.addClass("nodeEndSuccess");
        else
            return node.addClass("nodeEndFail");
    }
    //节点扩展,（添加日志按钮）
    function NodeInside(node) {
        //取消原点击事件
        node.unbind("click");
        //
        var text = node.text();
        node.text("");
        //
        var nodetext = $('<div style="text-align:center;height:35px;"></div>');
        nodetext.text(text);
        nodetext.appendTo(node);

        //获取节点上的原数据
        var nodeData = jsPlumbNode.nodeGetData(node);

        var div = $('<div style="text-align:center;"></div>')
        div.appendTo(node);
        //
        var btn = $('<lable class="daily">日志</lable>');
        btn.bind("click", function () {

            var clickbtn = $(this);
            if (clickbtn.attr("data-click") == "true")
                return;
            else {
                clickbtn.attr("data-click", "true");
            }

            //脚本流
            var nTaskId = $("#TASK_ID").val();
            //节点数据
            var nNodeVal = node.data("data");
            openDiv(950, 450);

            function openDiv(width, height) {
                Valiable(function () {
                    layui.use('layer', function () {
                        layui.layer.open({
                            type: 1
                            , title: "日志信息"
                            , area: [width + 'px', height + 'px']
                            , content: $("#layuiTab")
                        });
                    });
                    //取消遮罩层  后续还需要优化
                    $(".layui-layer-shade").remove();
                    clickbtn.removeAttr("data-click");
                });
            }

            function Valiable(func) {
                //报表刷新
                $(".layui-tab-item > iframe[id='log']").attr("src", "../AfScriptTaskFlowNodeLog/Index?TaskId=" + nTaskId + "&nodeId=" + nNodeVal.NODE_ID);
                //异步提交
                $.get("../AfScriptTask/TaskNodeShow", { TaskId: nTaskId, NodeId: nNodeVal.NODE_ID }, function (_d) {
                    if (_d.IsEidt == undefined) return;
                    if (_d.IsEidt) {
                        var _r = _d.data;
                        $(".deal").show();
                        $("#btnStart").attr("data_nodeId", _r.NODE_ID);
                        if (typeof (_r.REFERENCE_DATE) === "string" && _r.REFERENCE_DATE.length >= 10)
                            $("#REFERENCE_DATE").val(_r.REFERENCE_DATE.substr(0, 10));
                        $("#IS_SUCCESS").val("失败");
                        $("#CONTENT").val(_r.CONTENT);
                    } else {
                        $(".deal").hide();
                    }
                    if (func != undefined)
                        func();


                }, "json");
            }
        });
        btn.appendTo(div);
        //创建外部div
        //var div = $("<div>", {
        //    style: "vertical-align: middle;text-align: center;"
        //});
        //将按钮添加到div中
        //btn.appendTo(div);
        //div.appendTo(node.find(".nodecell"));
    }
}