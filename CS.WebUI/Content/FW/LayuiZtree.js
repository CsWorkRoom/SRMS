jQuery.extend({
    LayuizTree: {
        id: "PID",
        obj: null,
        demoId: "",
        inputId: "",
        ztreeId: "",
        ztreehideId: "",
        ztreenode: [],
        ztreeisMultiple: false,
        ztreechkboxType: { "Y": "ps", "N": "ps" },
        onClick: function (event, treeId, treeNode) { },
        init: function (id) {
            this.id = id;
            this.obj = $("#" + this.id);
            this.obj.attr("hidden", "hidden");
            this.initstyle();
            this.initMain();
            this.initHtmlInput();
            this.initHtmlztree();
            this.initControl();
            this.init_ztree();
            this.init_value();
        },
        initstyle: function () {
            var stylecss = [
                '.tree-content{',
                'display: none;',
                'position: absolute;',
                'height: 300px;',
                'left: 0 !important;',
                'top: 38px !important;',
                'background: #ffffff;',
                'z-index: 9999999;',
                'border: 1px solid #C9C9C9 !important;',
                'overflow-y: auto;',
                '}'
            ];

            var style = $("style");
            if (style == null || style.length == 0) {
                $("head").append("<style></style>");
                style = $("style");
            }
            style.text(stylecss.join(""));
        },
        initMain: function () {
            this.demoId = "demo2_" + this.id;
            var s = [
                '<div id="', this.demoId, '" class="select-tree layui-form-select">',
                '</div>'
            ]
            this.obj.parent().append(s.join(""));
        },
        initHtmlInput: function () {
            this.inputId = "Show_" + this.demoId;
            var html = [
                '<div class = "layui-select-title" >',
                '<input id="' + this.inputId + '"' + 'type = "text" placeholder = "请选择" value = "" class = "layui-input" readonly>',
                '<i class= "layui-edge" ></i>',
                '</div>'
            ]
            $("#" + this.demoId).append(html.join(""));
        },
        initHtmlztree: function () {
            this.ztreeId = "ztree_" + this.demoId;
            this.ztreehideId = "ztreeHide" + this.demoId;
            var html = [
                '<div class="tree-content scrollbar">',
                '<input hidden id="', this.ztreehideId, '" name="', this.demoId, '">',
                '<ul id="', this.ztreeId, '" class="ztree scrollbar" style="margin-top:0;"></ul>',
                '</div>'
            ]
            $("#" + this.demoId).parent().append(html.join(""));
        },
        initControl: function () {
            var demoId = this.demoId;
            $("#" + demoId).bind("click", function () {
                if ($(this).parent().find(".tree-content").css("display") !== "none") {
                    FuncHideMenu(demoId);
                } else {
                    FuncShowMenu(this);
                }
                $("body").bind("mousedown", onBodyDown);
            });


            var isMultiple = this.ztreeisMultiple;
            var chkboxType = this.ztreechkboxType;

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
                    enable: isMultiple,
                    chkboxType: chkboxType
                },
                callback: {
                    onClick: onClick,
                    onCheck: onCheck
                }
            }

            var id = this.id;
            var demoId = this.demoId;
            var ztreehideId = this.ztreehideId;
            var inputId = this.inputId;
            var ztreeClick = this.onClick;
            function onClick(event, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(treeId);
                if (zTree.setting.check.enable == true) {
                    zTree.checkNode(treeNode, !treeNode.checked, false);
                    var s = $.zTreeFunc.FuncAssignment(treeId, zTree.getSelectedNodes());
                    FuncChangeValue(s, inputId, id, ztreehideId);
                } else {
                    var s = $.zTreeFunc.FuncAssignment(treeId, zTree.getSelectedNodes());
                    FuncChangeValue(s, inputId, id, ztreehideId);
                    FuncHideMenu(demoId);
                }
                ztreeClick(event, treeId, treeNode);
            }

            function onCheck(event, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(treeId);
                var s = $.zTreeFunc.FuncAssignment(treeId, zTree.getCheckedNodes());
                FuncChangeValue(s, inputId, id, ztreehideId);
            }

            var ztree = $("#" + this.ztreeId);
            var ztreenode = this.ztreenode;
            $.fn.zTree.init(ztree, setting, ztreenode);

            //隐藏下拉
            function FuncHideMenu() {
                $("#" + demoId).parent().find(".select-tree").removeClass("layui-form-selected");
                $("#" + demoId).parent().find(".tree-content").fadeOut("fast");
                $("body").unbind("mousedown", onBodyDown);
            }
            //显示下拉
            function FuncShowMenu(taget) {
                $(taget).addClass("layui-form-selected");
                var Offset = $(taget).offset();
                var width = $(taget).width() - 2;
                $(taget).parent().find(".tree-content").css({
                    left: Offset.left + "px",
                    top: Offset.top + $(taget).height() + "px"
                }).slideDown("fast");
                $(taget).parent().find(".tree-content").css({
                    width: width
                });
                $("body").bind("mousedown", onBodyDown);
            }
            //变更input标签值
            function FuncChangeValue(s, inputId, id, ztreehideId) {
                $("#" + inputId).attr("value", s.names);
                $("#" + inputId).attr("title", s.names);
                //$("#" + id).attr("value", s.ids);
                $("#" + id).attr("value", s.values);
                $("#" + ztreehideId).attr("value", s.ids);
            }
            //点击空白处事件
            function onBodyDown(event) {
                //var obj = $(event.target);
                //var parentobj = obj.parents(".tree-content");
                ////还存在问题
                //if (parentobj.html() == null) {
                //    FuncHideMenu();
                //}

                var obj = $(event.target);

                var istc = false;
                var isst = false;
                $.each($(".tree-content"), function (i, n) {
                    if (n == obj.get(0)) {
                        istc = true;
                        return false;
                    }
                });

                $.each($(".select-tree"), function (i, n) {
                    if (n == obj.get(0)) {
                        isst = true;
                        return false;
                    }
                });

                var parentobj1 = obj.parents(".tree-content");
                var parentobj2 = obj.parents(".select-tree");
                if (parentobj1.html() == null
                    && parentobj2.html() == null
                    && !istc
                    && !isst) {
                    FuncHideMenu();
                }
            }

        },
        init_ztree: function () {

        },
        init_value: function () {
            var val = this.obj.attr("value");
            if (val === null || val === "" || val === undefined) return;
            var idArrays = val.split(",");
            var ztree = $.fn.zTree.getZTreeObj(this.ztreeId);
            for (var i = 0; i < idArrays.length; i++) {
                if (idArrays[i] == "" || idArrays[i] == null || idArrays[i] == undefined || idArrays[i] == "undefined") {
                    continue;
                }
                //获取节点
                var node = ztree.getNodesByParam("value", idArrays[i])[0];
                if (node == undefined) {
                    node = ztree.getNodesByParam("id", idArrays[i])[0];
                }
                //选中节点
                if (!this.ztreeisMultiple)
                    ztree.selectNode(node);
                else {
                    //勾选单个节点
                    if (node === null || node == undefined) return;
                    ztree.checkNode(node, true, false);
                }
            }
            //
            var s = {};
            if (!this.ztreeisMultiple)
                var s = $.zTreeFunc.FuncAssignment("000000", ztree.getSelectedNodes());
            else
                var s = $.zTreeFunc.FuncAssignment("000000", ztree.getCheckedNodes());

            FuncChangeValue(s, this.inputId, this.id, this.ztreehideId);

            //变更input标签值
            function FuncChangeValue(s, inputId, id, ztreehideId) {
                $("#" + inputId).attr("value", s.names);
                $("#" + inputId).attr("title", s.names);
                //$("#" + id).attr("value", s.ids);
                //$("#" + id).attr("value", $("#" + id).attr("value") + "," + s.values);
                $("#" + ztreehideId).attr("value", s.ids);
            }
        }
    },
    comboztree: function (id, s) {
        s = jQuery.extend({}, jQuery.LayuizTree, s);
        s.init(id);
    },
    zTreeFunc: {
        FuncAssignment: function (treeId, nodes) {
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
            //treeId = treeId.substring(0, treeId.length - 4);
            return { "names": names, "ids": ids, "values": values };
        }
    }
})



