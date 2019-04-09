$(window).resize(function () {
    var w = $(document).width();
    var h = $(document).height();
    try {
        DrawWaterMark(w, h, sysName, userName);
    } catch (e) {
        return;
    }
});

//#region 校验页面和事件权限
$(document).ready(function () {
    //ValidataUrlRole(); //注释则关闭页面端验证
    // 初始化水印
    InitBgWatermarkByCanvas("body");
    try {
        // 绘制水印
        DrawWaterMark($(document).width(), $(document).height(), sysName, userName);
    } catch (e) {
        return;
    }
});


function InitBgWatermarkByCanvas(target) {
    // 声明画布
    var convas = '<canvas class="watermark" width = "200px"  height = "150px" style="display:none;"></canvas>' + '<canvas class="repeat-watermark" style="display:none;"></canvas>';
    var currentTime = new Date();
    userName =  userName + "   " 
        + currentTime.getFullYear() + "-"
        + (currentTime.getMonth() + 1) + "-"
        + currentTime.getDate();

    if (userName != "") {
        $(target).append(convas);
    }
}

function DrawWaterMark(docWidth, docHeight, sysName, userName) {  
    var cw = $('.watermark')[0];
    var crw = $('.repeat-watermark')[0];

    crw.width = docWidth;
    crw.height = docHeight;

    var ctx = cw.getContext("2d");

    //清除小画布
    ctx.clearRect(0, 0, 200, 150);
    ctx.font = "14px Microsoft YaHei";

    //文字倾斜角度
    ctx.rotate(-20 * Math.PI / 180);

    ctx.fillStyle = "rgba(219, 219, 234, 0.7)";
    //第一行文字
    ctx.fillText(sysName, -20, 80);

    //第二行文字 
    ctx.fillText(userName, -20, 100);

    //坐标系还原
    ctx.rotate(20 * Math.PI / 180);
    var ctxr = crw.getContext("2d");
    //清除整个画布
    ctxr.clearRect(0, 0, crw.width, crw.height);
    //平铺--重复小块的canvas
    var pat = ctxr.createPattern(cw, "repeat");
    ctxr.fillStyle = pat;

    ctxr.fillRect(0, 0, crw.width, crw.height);

    $('.layui-fluid').css("background-image", 'url("' + ctxr.canvas.toDataURL() + '")').css("z-index", 99999); //ctxr.canvas.toDataURL();

}

