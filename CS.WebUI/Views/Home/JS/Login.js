//layui.use('form', function () {
//    var form = layui.form; //只有执行了这一步，部分表单元素才会自动修饰成功
//    form.render();
//});


layui.use(['form', 'layer'], function () {
    var $ = layui.$, form = layui.form, layer = layui.layer;
    //监听提交
    form.on('submit(login)', function (data) {
        encode();
    });

    (function setPOS() {
        var posY = ($(window).height() - $('.layadmin-user-login-main').height()) / 2 - $('.leoweather1').height();
        if (posY > 0) { $('.layadmin-user-login-main').css({ 'margin-top': posY + 'px' }); };

        // 显示tips 
        layer.tips('扫码下载APP', '#animation_btn', {
            tips: [1, '#1e9fff'],
            area: 'auto',
            time: 2000,
        });
    }())
    
});

function encode() {
    layui.use(['jquery'], function () {
        var $ = jQuery = layui.jquery;
        var psd = $("#password").val();
        var len = psd.length;
        var p = String.fromCharCode(60 + len);
        var n = 100;
        var c = 0;
        for (var i = 0; i < len; i++) {
            p += psd.charAt(i);
            n += psd.charCodeAt(i);
            c = Math.ceil(n / (p.length));
            p += String.fromCharCode(c);
            n += c;
            //console.log(p + "," + n + "," + c);
            if (i % 3 == 0) {
                c = Math.ceil(n / (p.length));
                p += String.fromCharCode(c);
                n += c;
            }
        }
        $("#password").val(p);
    });
}

layui.use(['layer','jquery'], function () {
    var layer = layui.layer;
    var $ = jQuery = layui.jquery;

//鼠标移动显示隐藏
    $('.layui-icon-erweima').mouseover(function () {
        layer.tips('扫码下载APP', '#animation_btn', {
            tips: [1, '#1e9fff'],
            area: 'auto',
            time: 0 })
    }).mouseout(function(){layer.closeAll('tips');});

    // 翻转动画
    $('.layui-icon-erweima').on('click', function () {
        $('.login-animated').css({ 'transform': 'rotateY(180deg)', 'z-index': '1' });
        $('.ewm-code').css({ 'transform': 'rotateY(0deg)', 'z-index': '2' });
        layer.closeAll('tips');
    });

    $('.pwd-login-btn').on('click', function () {
        $('.login-animated').css({ 'transform': 'rotateY(0deg)', 'z-index': '2' });
        $('.ewm-code').css({ 'transform': 'rotateY(-180deg)', 'z-index': '1' });
        setTimeout(function () {
            layer.tips('扫码下载APP', '#animation_btn', {
                tips: [1, '#1e9fff'],
            area: 'auto',
            time: 4000,
            })
        }, 1000);  
    });
});
