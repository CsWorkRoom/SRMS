var Plumbs = Plumbs || {};
Plumbs.UI = Plumbs.UI || {};
Plumbs.UI.Views = Plumbs.UI.Views || {};
Plumbs.UI.Views.Plumb = {
    init: function () {
        jsPlumb.importDefaults({
            // default drag options
            DragOptions: { cursor: 'pointer', zIndex: 2000 },
            // default to blue at one end and green at the other
            EndpointStyles: [{ fillStyle: '#225588' }, { fillStyle: '#225588'}],
            // blue endpoints 7 px; green endpoints 11.
            Endpoints: [["Dot", { radius: 3}], ["Dot", { radius: 3}]],
            // the overlays to decorate each connection with.  note that the label overlay uses a function to generate the label text; in this
            // case it returns the 'labelText' member that we set on each connection in the 'init' method below.
            ConnectionOverlays: [
                    ["PlainArrow", { location: 1, width: 15, length: 12 }],
                    ["Arrow", { location: 0.9 }]
                    
                ]
        });
    },

    addPannel: function (e) {
        jsPlumb.makeSource($(e), {
            filter: ".ep",
            anchor: "AutoDefault",
            connector: "Straight",
            connectorStyle: { strokeStyle: nextColour(), lineWidth: 2 },
            maxConnections: 30,
            onMaxConnections: function (info, e) {
                alert("最大不能超过 (" + info.maxConnections + ") 条连接");
            }
        });

        jsPlumb.bind("connection", function (info) {
            info.connection.setPaintStyle({ strokeStyle: nextColour() });
            var joinType = info.connection.target.attr("title");
            if (joinType == "" || typeof(joinType)=="undefined") {
                joinType = "left join";
            }
            var cline;
            var connects = $(jsPlumb.getAllConnections().jsPlumb_DefaultScope);
            connects.each(function () {
                if (this.target == info.connection.target && this.source == info.connection.source) {
                    cline = this;
                    cline.title=info.connection.target.attr("tips");
                    return;
                }
            });

            var label = info.connection.getOverlay("label");
            if (label == null) {
                info.connection.addOverlay(["Label", {
                    label: joinType, id: "label", cssClass: "aLabel", title: info.connection.id, tip: "条件提示", events: {
                        click: function (labelOverlay, originalEvent) {
                            dialogWindow_Line(labelOverlay, info.connection.id);
                        }
                    }
                }]);
                info.connection.title = joinType;
            }

          //  info.connection.getOverlay("label").setLabel(info.connection.id + ":" + info.title);
          
            //info.connection.getOverlay("label").setLabel(info.connection.sourceId + " TO " + info.connection.targetId);
           // info.connection.getOverlay("label").id ="label_" +info.connection.id;
           
          //  dialogWindow();
        });

       
        jsPlumb.makeTarget(e, {
            dropOptions: { hoverClass: "dragHover" },
            anchor: "AutoDefault"
        });
        jsPlumb.bind("dblclick", function (c) {
            //清空连线       
            jsPlumb.detach(c);
            //更新逻辑关系
           // var sourceid = c.sourceId;
           // var targerid = c.targetId;
           // var parentid = $("#" + targerid).attr("parentId");
           // parentid = parentid.replace("|" + sourceid, "").replace(sourceid + "|", "");
            //$("#" + targerid).attr("parentId", parentid);
        });

        //jsPlumb.bind("click", function (conn, originalEvent) {
        //    alert("you clicked ");
        //    if (conn.getOverlay("label") != null) {
        //        // connection.getOverlay("label").setLabel("<img src='Images/edit.gif'/>" + connection.sourceId);
        //    }
        //    else {//不存在样式对象，则动态创建                  
        //        //connection.overlays = overlay;
        //        //alert(connection.getOverlay("label"))
        //        conn.addOverlay(DragFlow.overlay(""), "");
        //        //connection.getOverlay("label").setLabel("<img src='Images/edit.gif'/>" + connection.sourceId);
        //    }
        //    var type = $("#" + conn.targetId).attr("itemtype");

        //    conn.addOverlay(DragFlow.overlay(""), "");

        //    return false;
        //    //console.log(conn);
        //    //console.log(originalEvent);
        //    //if ($.ligerDialog.confirm("确定要删除该连接吗？", "温馨提示", function (e) {
        //    //    if (e) {
        //    //     jsPlumb.detach(conn);
        //    //} else {
        //    //     return false;
        //    //}
        //    //}));
              
        //});
     
    },

    delPannel: function (id) {
        jsPlumb.removeAllEndpoints(id);
    },

    ConnectNode: function (sourceid, targetid,jointype,joinexpression) {
        $("#" + targetid).attr("title", jointype);  
        $("#" + targetid).attr("tips", joinexpression);
        jsPlumb.connect({
            source: sourceid, target: targetid
        });
    }

}

var curColourIndex = 1, maxColourIndex = 24, nextColour = function () {
    var R, G, B;
    R = parseInt(128 + Math.sin((curColourIndex * 3 + 0) * 1.3) * 128);
    G = parseInt(128 + Math.sin((curColourIndex * 3 + 1) * 1.3) * 128);
    B = parseInt(128 + Math.sin((curColourIndex * 3 + 2) * 1.3) * 128);
    curColourIndex = curColourIndex + 1;
    if (curColourIndex > maxColourIndex) curColourIndex = 1;
    return "rgb(" + R + "," + G + "," + B + ")";
};

//Page load events
$(document).ready(
    function () {
        //all JavaScript that needs to be call onPageLoad can be put here.
        Plumbs.UI.Views.Plumb.init();
    }
);