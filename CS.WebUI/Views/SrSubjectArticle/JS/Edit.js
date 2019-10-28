//表单提交
function save() {
    layui.use(['form', 'layer', 'jquery'], function () {
        var form = layui.form, layer = layui.layer, $ = layui.$;
        var url = "../SrSubjectArticle/Edit";
        SaveForm('form', url);
        return;
    });
}

//#region 基础信息-
layui.use(['form', 'layer', 'jquery', 'laydate', 'table', 'element'], function () {
    var form = layui.form, layer = layui.layer, $ = layui.jquery, table = layui.table, element = layui.element;

    //自定义验证规则
    form.verify({
        nameVer: function (value) {
            if (isEmpty(value)) {
                return '请填写学习文章名！';
            }
        },
        subjectVer: function (value) {
            if (isEmpty(value) || value == "0") {
                return '请选择学科！';
            }
        },
    });

    //导出选项
    form.on('switch(switchUse)', function (data) {
        if (this.checked) {
            $("#IS_ENABLE").val(1);
        } else {
            $("#IS_ENABLE").val(0);
        }
    });

    var editor = new Simditor({
        textarea: $('#editor'),
        placeholder: '',
        toolbar: [
            'title', 'bold', 'italic', 'underline', 'strikethrough', 'fontScale', 'color', '|', 'ol', 'ul',
            'blockquote', 'code', 'table', '|', 'link', 'image', 'hr', '|', 'indent', 'outdent', 'alignment'
        ], //工具条都包含哪些内容
        pasteImage: true, //允许粘贴图片
        //defaultImage: '@Url.Content("~/Content/simditor/images/image.png")', //编辑器插入的默认图片，此处可以删除
        upload: {
            url: '../UpFile/ImgUpload', //文件上传的接口地址
            params: { pathName: 'ArticlePath' }, //键值对,指定文件上传接口的额外参数,上传的时候随文件一起提交
            fileKey: 'file', //服务器端获取文件数据的参数名
            connectionCount: 3,
            leaveConfirm: '正在上传文件'
        }
        //optional options
    });

    $("#contentDiv .simditor-body").attr("id", "editorBody");

    editor.uploader.on('uploadsuccess',
        (res, file, mask) => {
            //获得上传的文件对象
            var img = file.img;
            img.attr('src', '../../' + mask.Result); //重新给img标签的src属性赋值图片路径
            //img.attr("width", "100%").attr("height", "auto").attr("style", "max-width:100%;height:auto;display:block");
            img.removeAttr("width").removeAttr("height").removeAttr("data-image-size").removeAttr("class");
        });


    //提交
    form.on('submit(submit)', function (data) {
        var content = $(".simditor-body").html();
        if (isEmpty(content) || content == "<p><br></p>") {
            layer.alert("文章内容不能为空！");
            return;
        }
        else {
            $("#CONTENT").val(content);
            save();//保存
        }
    });

    var subjectNodes = JSON.parse($("#SubjectSelect").val());
    $.comboztree("SUBJECT_ID", { ztreenode: subjectNodes });
});


