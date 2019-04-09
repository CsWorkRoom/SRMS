layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
    var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
    //----------------------------------------字段配置------------------------------------------
    table.render({
        elem: '#tableField'
        , even: true
        , size: 'sm'
        , cols: [[
          { field: 'EN_NAME', title: '字段英文名', minWidth: 180, sort: true }
          , { field: 'CN_NAME', title: '字段中文名', edit: 'text', minWidth: 150 }
          , { field: 'FIELD_DATA_TYPE', title: '数据类型', width: 120, templet: '#fieldDataType' }
          , { field: 'IS_SHOW', title: '是否显示', templet: '#checkboxShow', minWidth: 100 }
          , { field: 'IS_FIXED', title: '是否冻结', templet: '#checkboxFixed', minWidth: 100 }
          , { field: 'IS_SORT', title: '是否排序', templet: '#checkboxSort', minWidth: 100 }
          , { field: 'IS_ENCRYPT', title: '是否加密', templet: '#checkboxEncrypt', minWidth: 100 }
          , { field: 'SHOW_WIDTH', title: '显示宽度', edit: 'text', width: 100 }]]
        , data: fieldData
        , limit: fieldData.length
    });
    form.on('checkbox(fltShow)', function (obj) {
        fieldData[this.value][this.name] = obj.elem.checked ? 1 : 0;
    });
    form.on('checkbox(fltFixed)', function (obj) {
        fieldData[this.value][this.name] = obj.elem.checked ? 1 : 0;
    });
    form.on('checkbox(fltSort)', function (obj) {
        fieldData[this.value][this.name] = obj.elem.checked ? 1 : 0;
        //layer.tips(this.value + ' ' + this.name + '：' + obj.elem.checked, obj.othis);
    });
    form.on('checkbox(fltEncrypt)', function (obj) {
        if (fieldData[this.value]["FIELD_DATA_TYPE"] != "21") {
            fieldData[this.value][this.name] = 0;
            layer.tips("非文本类型不可加密");
            return;
        }

        fieldData[this.value][this.name] = obj.elem.checked ? 1 : 0;
        //layer.tips(this.value + ' ' + this.name + '：' + obj.elem.checked, obj.othis);
    });
});

//----------------------------------------筛选配置------------------------------------------
var selectQueryInfo = new Object();
selectQueryInfo.dbID = 0;
selectQueryInfo.tableName = "";
selectQueryInfo.valueField = "";
selectQueryInfo.textField = "";
selectQueryInfo.parentField = "";
selectQueryInfo.where = "";

showFilterData();
function showFilterData() {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        //执行渲染
        table.render({
            elem: '#tableFilter'
            , even: true
            , size: 'sm'
            , cols: [[
                { field: 'FILTER_TYPE', title: '位置', width: 80, templet: '#switchType' }
              , { field: 'FIELD_NAME', title: '筛选字段', width: 200, templet: '#selectField' }
              , { field: 'FILTER_NAME', title: '字段名称', width: 150, edit: 'text' }
              , { field: 'FORM_QUERY_TYPE', title: '搜索框类型', width: 160, templet: '#selectQueryType' }
              , { field: 'DEFAULT_VALUE', title: '默认值 <span class="layui-badge layui-bg-green showSqlHelp_a" onclick="ShowSqlHelp();">Help</span>', minWidth: 180, edit: 'text' }
              , { field: 'INPUT_WIDTH', title: '输入框宽度', width: 120, edit: 'text' }
              , { title: '增/删', width: 80, templet: '#addordel' }]]
            , data: filterData
            , limit: filterData.length

        });

        if (filterData.length < 1) {
            $("#divFilterAdd").html("<a href='#' onclick='addFilter(0);'>添加</a>");
        }
        else {
            $("#divFilterAdd").html("");
        }

        //监听筛选项位置
        form.on('switch(filterType)', function (obj) {
            if (obj.elem.checked == true) {
                filterData[this.value][this.name] = 1;
                layer.tips("默认位置，显示在表格上方", obj.othis);
            } else {
                filterData[this.value][this.name] = 2;
                layer.tips("高级筛选，点击按钮时展开", obj.othis);
            }
        });
    });
}
//添加筛选项
function addFilter(index) {
    var item = {
        'FILTER_TYPE': 1
        , 'FIELD_NAME': fieldData[0]['EN_NAME']
        , 'FILTER_NAME': fieldData[0]['CN_NAME']
        , 'INPUT_NAME': fieldData[0]['EN_NAME']
        , 'FIELD_DATA_TYPE': fieldData[0]['FIELD_DATA_TYPE']
        , 'FILTER_OPERATOR': 0
        , 'DEFAULT_VALUE': ''
        , 'INPUT_WIDTH': 160
    };
    filterData.splice(index, 0, item);
    showFilterData();
}
//删除筛选项
function deleteFiletr(index) {
    filterData.splice(index, 1);
    showFilterData();
}
//修改字段名称
function changeFieldName(index, obj) {
    //console.log(obj);
    filterData[index]['FIELD_NAME'] = obj.value;
    filterData[index]['FILTER_NAME'] = obj.options[obj.options.selectedIndex].text;
    filterData[index]['INPUT_NAME'] = obj.value;
    for (i = 0; i < fieldData.length; i++) {
        if (fieldData[i]['EN_NAME'] == obj.value) {
            filterData[index]['FIELD_DATA_TYPE'] = fieldData[i]['FIELD_DATA_TYPE'];
        }
    }
    showFilterData();
}
//修改操作类型
function changeOperator(index, obj) {
    filterData[index]['FORM_QUERY_TYPE'] = obj.value;
    showFilterData();
}

//设置下拉框
var divhtml = $("#divSelectDetail").html();

//设置下拉选项
function setSelectDetail(index, queryType) {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        var div = $("#divSelectDetail");
        if (div != null) {
            div.remove();
        }

        layer.open({
            title: '设置下拉框选项'
            , area: ['700px', '520px']
            , content: divhtml
            , yes: function () {
                var selectdetail = "";
                var typeOp = $("input[name='selectSoruceType']:checked").val();
                if (typeOp == "1") {
                    selectdetail = "1:" + $("#selectEnumOptions").val();
                } else if (typeOp == "2") {
                    selectdetail = "2:" + $("#selectDbID").val();
                    selectdetail += "," + $("#selectTableName").val();
                    selectdetail += "," + $("#selectValueField").val();
                    selectdetail += "," + $("#selectTextField").val();
                    if ($("#treexl").val() == queryType) {
                        selectdetail += "," + $("#selectParentField").val();
                    }
                    selectdetail += "," + $("#selectWhere").val();
                } else {
                    selectdetail = "3:" + $("#selectTreaDbID").val() + "◎" + $("#selectSql").val();
                }
                filterData[index]["SELECT_DETAIL"] = selectdetail;
                layer.closeAll();
            }
        });

        form.on('radio(selectSoruceType)', function (data) {
            $("#divSelectTypeEnum").hide();
            $("#divSelectTypeQuery").hide();
            $("#divSelectTypeSql").hide();
            switch (data.value) {
                case "1":
                    $("#divSelectTypeEnum").show();
                    break;
                case "2":
                    $("#divSelectTypeQuery").show();
                    break;
                default:
                    $("#divSelectTypeSql").show();
            }
        });

        //选择数据库
        form.on('select(selectDbID)', function (data) {
            selectQueryInfo.tableName = "";
            selectQueryInfo.valueField = "";
            selectQueryInfo.textField = "";
            selectQueryInfo.parentField = "";
            selectQueryInfo.where = "";
            $("#selectWhere").val("");
            getTableNameList();
        });
        //选择数据库表
        form.on('select(selectTableName)', function (data) {
            $("#selectWhere").val("");
            getFieldsList();
        });

        //是否允许更新数据
        form.on('switch(switchCheckbox)', function (data) {
            if (this.checked) {
                $("#IS_ALLOW_DELETE").val("1");
                layer.tips('允许删除自己上传的附件', data.othis)
            } else {
                $("#IS_ALLOW_DELETE").val("0");
                layer.tips('不允许任何人删除附件', data.othis)
            }
        });

        form.render();

        //加载初始值
        loadSelectDetail(index, queryType);
        var OptionsPlaceholder = "请输入下拉选项，输入方式：每行一个选项（包含值和显示名），值和显示名之间用逗号分隔，如：\n1,类型A\n2,类型B\n3,类型C";
        var SqlPlaceholder = "请输入SQL语句，SQL字段别名值为V，名为K。如：\n SELECT A V,B K FROM TABLE";
        //是否显示父级字段
        if (queryType == $("#treexl").val()) {
            OptionsPlaceholder = "请输入下拉选项，输入方式：每行一个选项（包含值、显示名、父级字段），值和显示名及父级字段之间用逗号分隔，如：\n1,类型A,父级P\n2,类型B,父级P\n3,类型C,父级P";
            SqlPlaceholder = "请输入SQL语句，SQL字段别名值为V，名为k，父级为P。如：\n SELECT A V,B K,C P FROM TABLE";
            $("#parentDiv").show();
        } else {
            OptionsPlaceholder = "请输入下拉选项，输入方式：每行一个选项（包含值和显示名），值和显示名之间用逗号分隔，如：\n1,类型A\n2,类型B\n3,类型C";
            SqlPlaceholder = "请输入SQL语句，SQL字段别名值为V，名为K。如：\n SELECT A V,B K FROM TABLE";
            $("#parentDiv").hide();
        }
        $("#selectEnumOptions").attr("placeholder", OptionsPlaceholder);
        $("#selectSql").attr("placeholder", SqlPlaceholder);
    });
}

//加载初始值
function loadSelectDetail(index, queryType) {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        if (index >= filterData.length) {
            form.render();
            return;
        }
        var detail = filterData[index]["SELECT_DETAIL"];
        if (detail == null || detail.length < 1) {
            $(":radio[name='selectSoruceType'][value='1']").prop("checked", "checked");
            form.render();
            return;
        }

        $("#divSelectTypeEnum").hide();
        $("#divSelectTypeQuery").hide();
        $("#divSelectTypeSql").hide();
        var arrs = detail.split(":");
        if (arrs[0] == "1") {
            $("#divSelectTypeEnum").show();
            $(":radio[name='selectSoruceType'][value='1']").prop("checked", "checked");
            $("#selectEnumOptions").val(arrs[1]);
        } else if (arrs[0] == "3") {
            $("#divSelectTypeSql").show();
            $(":radio[name='selectSoruceType'][value='3']").prop("checked", "checked");
            var info = arrs[1].split("◎");//LABE页内容
            if (info.length < 2) {
                return;
            }
            $("#selectTreaDbID").val(info[0]);//数据库名
            $("#selectSql").val(info[1]);//SQL表
        } else {
            $(":radio[name='selectSoruceType'][value='2']").prop("checked", "checked");
            $("#divSelectTypeQuery").show();
            var info = arrs[1].split(",");
            if (info.length < 4) {
                return;
            }
            selectQueryInfo.dbID = info[0];
            selectQueryInfo.tableName = info[1];
            selectQueryInfo.valueField = info[2];
            selectQueryInfo.textField = info[3];
            if ($("#treexl").val() == queryType) {//是否为下拉树
                selectQueryInfo.parentField = info[4];
                selectQueryInfo.where = info[(info.length - 1)];
            } else {
                selectQueryInfo.where = info[(info.length - 1)];
            }

            $("#selectDbID").val(selectQueryInfo.dbID);
            $("#selectWhere").val(selectQueryInfo.where);
            //加载表
            getTableNameList();
        }
        form.render();
    });
}

//获取表名列表
function getTableNameList() {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        var dbid = $("#selectDbID").val();
        if (dbid == null || dbid < 0) {
            $("#selectTableName").html("");
            $("#selectValueField").html("");
            $("#selectTextField").html("");
            $("#selectParentField").html("");
            $("#selectWhere").val("");
            form.render();
            return;
        }
        $.post("../AfDB/GetTableList?dbID=" + dbid + "&isImport=false", function (data) {
            var html = "";
            if (data.length > 0) {
                var tables = JSON.parse(data);
                html += "<option value=''>请选择表</option>";
                for (var i = 0; i < tables.length; i++) {
                    if (selectQueryInfo.tableName == [tables[i]]) {
                        html += "<option value='" + tables[i] + "' selected='selected'>" + tables[i] + "</option>";
                    } else {
                        html += "<option value='" + tables[i] + "'>" + tables[i] + "</option>";
                    }
                }
            }
            $('#selectTableName').html(html);
            form.render();
            getFieldsList();
        })
    });
}

//获取字段列表
function getFieldsList() {
    layui.use(['element', 'table', 'form'], function () {
        var element = layui.element, table = layui.table, form = layui.form;
        var dbid = $("#selectDbID").val();
        var tableName = $("#selectTableName").val();
        if (dbid == null || dbid < 0 || tableName == null || tableName.length < 1) {
            $("#selectValueField").html("");
            $("#selectTextField").html("");
            $("#selectParentField").html("");//父级字段
            $("#selectWhere").val("");
            form.render();
            return;
        }
        if (dbid >= 0 && tableName.length > 0) {
            $.post("../AfDB/GetFieldsList?dbID=" + dbid + "&tableName=" + tableName, function (data) {
                var htmlValue = "";
                var htmlText = "";
                var htmlParentText = "";
                htmlValue += "<option value=''>请选择值字段（必须）</option>";
                htmlText += "<option value=''>请选择名称字段（必须）</option>";
                htmlParentText += "<option value=''>请选择名称字段（必须）</option>";
                $.each(data, function (index, obj) {
                    htmlValue += "<option value='" + obj.EN_NAME + "' " + (selectQueryInfo.valueField == obj.EN_NAME ? "selected='selected'" : "") + ">" + obj.CN_NAME + "</option>";
                    htmlText += "<option value='" + obj.EN_NAME + "' " + (selectQueryInfo.textField == obj.EN_NAME ? "selected='selected'" : "") + ">" + obj.CN_NAME + "</option>";
                    htmlParentText += "<option value='" + obj.EN_NAME + "' " + (selectQueryInfo.parentField == obj.EN_NAME ? "selected='selected'" : "") + ">" + obj.CN_NAME + "</option>";
                });
                $("#selectValueField").html(htmlValue);
                $("#selectTextField").html(htmlText);
                $("#selectParentField").html(htmlParentText);//父级字段
                form.render();
            }, "json");
        }
    });
}

//----------------------------------------事件配置------------------------------------------
showEventData();
function showEventData() {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        //执行渲染
        table.render({
            elem: '#tableEvent'
            , even: true
            , size: 'sm'
            , cols: [[
                { field: 'EVENT_TYPE', title: '类型', width: 80, templet: '#switchEventType' }
              , { field: 'EVENT_NAME', title: '事件名称', width: 120, edit: 'text' }
              , { field: 'BUTTON_TEXT', title: '按钮文字', width: 120, edit: 'text' }
              , { field: 'REQUEST_MODE', title: '请求模式', width: 200, templet: '#selectRequestMode' }
              , { field: 'REQUEST_URL', title: '请求URL地址 <span class="layui-badge layui-bg-green showSqlHelp_a" onclick="ShowSqlHelp();">Help</span>', minWidth: 400, edit: 'text' }
              , { field: 'STYLE_TYPE', title: '按钮样式', templet: '#tempStyleType', width: 120 }
              //, { field: 'EVENT_STYLE', title: '样式内容',  minWidth: 100 }
              , { field: 'SHOW_WIDTH', title: '窗口宽度', width: 80, edit: 'text' }
              , { field: 'SHOW_HEIGHT', title: '窗口高度', width: 80, edit: 'text' }
              , { title: '增/删', width: 80, templet: '#addDelEvent' }]]
            , data: eventData
            , limit: eventData.length

        });

        if (eventData.length < 1) {
            $("#divEventAdd").html("<a href='#' onclick='addEvent(0);'>添加</a>");
        }
        else {
            $("#divEventAdd").html("");
        }

        //监听筛选项位置
        form.on('switch(eventType)', function (obj) {
            if (obj.elem.checked == true) {
                eventData[this.value][this.name] = 1;
                layer.tips("表头事件，显示在表格上方", obj.othis);
            } else {
                eventData[this.value][this.name] = 2;
                layer.tips("行事件，显示在每行右侧", obj.othis);
            }
        });
    });
}


//**********事件按钮样式***************
//点设置事件
function SetDiv(rowId) {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        $(".openIcon").remove();//移出以前的弹出
        var eventIcon = eventData[rowId]['BUTTON_ICON'] == null ? "" : $.trim(eventData[rowId]['BUTTON_ICON']);//得到图标样式
        var eventBut = eventData[rowId]['BUTTON_STYLE'] == null ? "" : $.trim(eventData[rowId]['BUTTON_STYLE']);//得到按钮样式

        var contentHtml = '<div class="openIcon" ><div id="divIconSelect" class="layui-select-mb MenuDiv_select"></div>';
        contentHtml += '<div style="float: left;"><div id="butList" class="butList">';
        contentHtml += '<button style="margin-left:10px" class="layui-btn layui-btn-sm layui-bg-orange" onclick="SetButton(this,\'layui-bg-orange\')">橙色</button>';
        contentHtml += '<button class="layui-btn layui-bg-green ' + (eventBut == null || eventBut == "" ? "" : "layui-btn-sm") + '" onclick="SetButton(this,\'layui-bg-green\')">绿色</button>';
        contentHtml += '<button class="layui-btn layui-btn-sm layui-bg-cyan" onclick="SetButton(this,\'layui-bg-cyan\')">青色</button>';
        contentHtml += '<button class="layui-btn layui-btn-sm layui-bg-blue" onclick="SetButton(this,\'layui-bg-blue\')">蓝色</button>';
        contentHtml += '<button class="layui-btn layui-btn-sm layui-bg-black" onclick="SetButton(this,\'layui-bg-black\')">黑色</button>';
        contentHtml += '<button class="layui-btn layui-btn-sm layui-bg-gray" onclick="SetButton(this,\'layui-bg-gray\')">禁用</button></div>';
        contentHtml += '<fieldset class="layui-elem-field yulanDiv">';
        contentHtml += '<legend>预览</legend>';
        contentHtml += '<div class="layui-field-box" >';
        if ((eventBut == null || eventBut == "") && (eventIcon != null && eventIcon != "")) {//是否只显示图标
            contentHtml += '<button id="yulan" class="layui-btn layui-bg-green" style="display:none" onclick="SetButton(\'layui-btn\')">预览</button><i id="yulanIcon" class="layui-icon ' + eventIcon + '"></i>';
        } else {
            eventBut = (eventBut != null && eventBut != "" ? eventBut : 'layui-bg-green');
            contentHtml += '<button id="yulan" class="layui-btn ' + eventBut + '" onclick="SetButton(\'' + eventBut + '\')"><i id="yulanIcon" class="layui-icon ' + eventIcon + '"></i>预览</button>';
        }
        contentHtml += '</div>';
        contentHtml += '</fieldset></div>';
        contentHtml += '<button class="layui-btn SaveButStyle" onclick="SaveButStyle(' + rowId + ')">确定</button></div>';

        layer.open({
            type: 1
            , title: "选择按钮样式"
             , area: ['363px', '425px']
                , offset: 0
                , id: 'layerIcon' + rowId
                , content: '<div style="padding: 10px 10px">' + contentHtml + '</div>'
                , btnAlign: 'c' //按钮居中
                , shade: 0 //不显示遮罩
        });

        ShowHideSelectIconDiv(rowId);//加载图标
        form.render();
    });
}


//弹出按钮选择器
function SetButton(thisObj, styleContent) {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        var oldCs = $(thisObj).attr("class");
        $.each($("#butList .layui-btn"), function () {//替成默认大小
            $(this).addClass("layui-btn-sm");
        });
        var iconHtml = $("#yulanIcon")[0].outerHTML;//预览图标
        $("#yulanIcon").remove();
        //切换选中状态
        if (oldCs.indexOf("layui-btn-sm") > -1) {//预览按钮
            $(thisObj).removeClass("layui-btn-sm");
            $("#yulan").attr("class", "layui-btn " + styleContent);//显示预览区
            $("#yulan").show();
            $("#yulan").prepend(iconHtml);//将图标标签放到按钮里面
        } else {
            $(thisObj).addClass("layui-btn-sm");
            $("#yulan").attr("class", "");
            $("#yulan").hide();
            $("#yulan").after(iconHtml);//将图标标签放到按钮外面
        }
    });
}

//点设置事件
function ShowHideSelectIconDiv(rowId) {
    // 加载图标
    var iconhtml = "<ul><li style='float:left;margin:10px;cursor: pointer;' onclick='SelectIcon(" + rowId + ",\"\");'>无</li>";
    $.each(iconNames, function (index, name) {
        iconhtml += "<li style='float:left;margin:10px;' onclick='SelectIcon(" + rowId + ",\"" + name + "\");'><i  class='layui-icon layui-icon-" + name + "' style='font-size:14px;color:#333;'></i></li>";
    });
    iconhtml += "</ul>";
    $("#divIconSelect").html(iconhtml);
    if ($("#iconButton").hasClass("layui-icon-less")) {
        $("#divIconSelect").slideUp("fast");
        $("#iconButton").removeClass();
        $("#iconButton").attr("class", "layui-icon layui-icon-zhankai3");
    } else {
        $("#divIconSelect").slideDown("fast");
        $("#iconButton").removeClass();
        $("#iconButton").attr("class", "layui-icon layui-icon-less");
    }
}

//选择图标
function SelectIcon(rowId, name) {
    if (name == "") {
        $("#yulanIcon").attr("class", "layui-icon");
        return;
    }
    $("#yulanIcon").attr("class", "layui-icon layui-icon-" + name);
}

//保存修改的样式
function SaveButStyle(index) {
    layui.use(['jquery', 'layer', 'form', 'table', 'element'], function () {
        var $ = layui.jquery, layer = layui.layer, form = layui.form, table = layui.table, element = layui.element;
        var but = $.trim($("#yulan").attr("class").replace("layui-btn", "").replace("layui-btn-sm", ""));
        var icon = $.trim($("#yulanIcon").attr("class").replace("layui-icon", ""));
        //保存数据
        eventData[index]['BUTTON_ICON'] = icon;
        eventData[index]['BUTTON_STYLE'] = but;

        //将结果同步至行预览中
        var RowYulanHtml = '<button class="layui-btn layui-btn-xs ' + but + '"><i class="layui-icon ' + icon + '"></i>按钮</button>';
        if ((but == null || but == "") && (icon != null && icon != "")) {
            RowYulanHtml = '<i class="layui-icon ' + icon + '"></i>';
        }
        $("#rowYulan" + index).html(RowYulanHtml);
        layer.closeAll();
    });
}

//***********END 事件按钮样式****************


//添加事件
function addEvent(index) {
    var item = {
        'EVENT_TYPE': 1
        , 'REQUEST_MODE': 0
        , 'SHOW_WIDTH': 0
        , 'SHOW_HEIGHT': 0
    };
    eventData.splice(index, 0, item);
    showEventData();
}
//删除事件
function deleteEvent(index) {
    eventData.splice(index, 1);
    showEventData();
}
//修改事件
function changeRequestMode(index, obj) {
    eventData[index]['REQUEST_MODE'] = obj.value;
    showEventData();
}


//----------------------------------------提交表单------------------------------------------
function save() {
    layui.use(['jquery'], function () {
        var $ = layui.jquery;
        var url = "../AfTableReport/Configure?id=" + $('#ID').val();
        $("#FIELD_CONFIG").val(JSON.stringify(fieldData));
        $("#FILTER_CONFIG").val(JSON.stringify(filterData));
        $("#EVENT_CONFIG").val(JSON.stringify(eventData));
        SaveForm('form', url);
        return;
    });
}