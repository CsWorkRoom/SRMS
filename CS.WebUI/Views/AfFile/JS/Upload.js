//多文件列表示例
layui.use('upload', function () {
    var $ = layui.jquery, upload = layui.upload;
    var url = "../AfFile/Upload?id=" + $("#ID").val() + "&rowkey=" + $("#ROW_KEY").val() + "&yyyymmdd=" + $("#YYYYMMDD").val();
    var fileListView = $('#fileList');
    var uploadListIns = upload.render({
        elem: '#testList'
      , url: url
      , field: 'file'
      , method: 'post'
      , accept: 'file'
      , acceptMime: $('#MIME_TYPES').val()
      , exts: $('#EXT_NAMES').val()
      , multiple: true
      , auto: false
      , bindAction: '#testListAction'
      , choose: function (obj) {
          var files = this.files = obj.pushFile(); //将每次选择的文件追加到文件队列
          //读取本地文件
          obj.preview(function (index, file, result) {
              var tr = $(['<tr id="upload-' + index + '">'
                , '<td>' + file.name + '</td>'
                , '<td>' + (file.size / 1014).toFixed(1) + 'KB</td>'
                , '<td>等待上传</td>'
                , '<td>'
                , '<button class="layui-btn layui-btn-mini demo-reload layui-hide">重传</button>'
                , '<button class="layui-btn layui-btn-mini layui-btn-danger demo-delete">删除</button>'
                , '</td>'
                , '</tr>'].join(''));

              //单个重传
              tr.find('.demo-reload').on('click', function () {
                  obj.upload(index, file);
              });

              //删除
              tr.find('.demo-delete').on('click', function () {
                  delete files[index]; //删除对应的文件
                  tr.remove();
                  uploadListIns.config.elem.next()[0].value = ''; //清空 input file 值，以免删除后出现同名文件不可选
              });

              fileListView.append(tr);
          });
      }
      , done: function (res, index, upload) {
          if (res.IsSuccess == true) { //上传成功
              var tr = fileListView.find('tr#upload-' + index), tds = tr.children();
              tds.eq(2).html('<span style="color: #5FB878;">上传成功</span>');
              tds.eq(3).html(''); //清空操作
              return delete this.files[index]; //删除文件队列已经上传成功的文件
          }
          this.error(index, upload, res.Message);
      }
      , error: function (index, upload, message) {
          var tr = fileListView.find('tr#upload-' + index)
          , tds = tr.children();
          tds.eq(2).html('<span style="color: #FF5722;">上传失败</span><br/>' + message);
          tds.eq(3).find('.demo-reload').removeClass('layui-hide'); //显示重传
      }
    });

});