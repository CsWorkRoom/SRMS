//下载模板文件
function DownloadFile() {
    var url = "../AfImport/Import?id=" + $("#ID").val() + "&isdownload=true";
    window.location = url;
}

//文件上传
layui.use(['layer', 'upload', 'laydate'], function () {
    var $ = layui.jquery, layer = layui.layer, upload = layui.upload, laydate = layui.laydate;
    if ($("#CREATE_TABLE_MODE").val().length > 0) {
        $("#divSelectBaseDate").show();
        //常规用法
        laydate.render({
            elem: '#baseDate'
            , isInitValue: true
            , value: new Date()
        });
    }

    var url = "../AfImport/Import?id=" + $("#ID").val() + "&reqmode=3";
    upload.render({
        elem: '#upload'
      , url: url
      , auto: true
      //, data: { date: $('#baseDate').val() }
      , data: {
          date: function () {
              return $('#baseDate').val();
          }
      }
      , field: 'file'
      , method: 'post'
      , accept: 'file'
      , acceptMime: 'application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      , exts: 'xls|xlsx'
      , done: function (res) {
          //console.log(res)
          var content = res.Message;
          if (res.Result != null && res.Result.length > 0) {
              content += "<br/>失败的行号及原因如下：<br/><textarea rows='10' cols='30'>" + res.Result + "</textarea>";
          }
          if (res.IsSuccess == true) {
              layer.alert("导入数据成功<br/>" + content);
          } else {
              layer.alert("导入数据失败<br/>" + content);
          }
      }
      , error: function () {
          alert("导入数据出错");
      }
    });
});