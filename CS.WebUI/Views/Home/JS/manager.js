///总金额
$(function () {
    layui.use(['form', 'element', 'layer', 'jquery'], function () {
        var form = layui.form, element = layui.element; layer = layui.layer, $ = layui.jquery;
        form.on('select(YYYYMM)', function (data) {
            GetRegionChart();
        })
        //读取地区薪酬的报表
        GetRegionChart();
        //历史薪酬报表情况
        GetHistoryChart();
    });
        var myChart = echarts.init(document.getElementById('sumMony'));
        var option = {
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                orient: 'vertical',
                left: 'left'
            },
            series: [
                {
                    name: '薪酬情况',
                    type: 'pie',
                    radius: '55%',
                    center: ['49%', '44%'],
                    label: {
                        normal: {
                            formatter: '{a|{a}}{abg|}\n{hr|}\n  {b|{b}:}{c}元  {per|{d}%}  ',
                            backgroundColor: '#eee',
                            borderColor: '#aaa',
                            borderWidth: 1,
                            borderRadius: 4,
                            rich: {
                                a: {
                                    color: '#999',
                                    lineHeight: 18,
                                    align: 'center'
                                },
                                hr: {
                                    borderColor: '#aaa',
                                    width: '100%',
                                    borderWidth: 0.5,
                                    height: 0
                                },
                                b: {
                                    fontSize: 13,
                                    lineHeight: 33
                                },
                            }
                        }
                    },
                    data: [
                        { value: $("#qpfxEchart").val(), name: '抢盘' },
                        { value: $("#jjfxEchart").val(), name: '计件' },
                        { value: $("#fxfxEchart").val(), name: '分享' }
                        //{ value: 262, name: '抢盘' },
                        //{ value: 324.34, name: '计件' },
                        //{ value: 200, name: '分享' }
                    ],
                    itemStyle: {
                        emphasis: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    }
                }
            ]
        };
        if (option && typeof option === "object") {
            myChart.setOption(option, true);
        }
        $(window).resize(function () {
            myChart.resize();
        });
        
});

// 滚动公告代码
(function ($) {
    $.fn.myScroll = function (options) {
        //默认配置
        var defaults = {
            speed: 40,  //滚动速度,值越大速度越慢
            rowHeight: 20 //每行的高度
        };

        var opts = $.extend({}, defaults, options), intId = [];

        function marquee(obj, step) {

            obj.find("ul").animate({
                marginTop: '-=1'
            }, 0, function () {
                var s = Math.abs(parseInt($(this).css("margin-top")));
                if (s >= step) {
                    $(this).find("li").slice(0, 1).appendTo($(this));
                    $(this).css("margin-top", 0);
                }
            });
        }

        this.each(function (i) {
            var sh = opts["rowHeight"], speed = opts["speed"], _this = $(this);
            intId[i] = setInterval(function () {
                if (_this.find("ul").height() <= _this.height()) {
                    clearInterval(intId[i]);
                } else {
                    marquee(_this, sh);
                }
            }, speed);

            _this.hover(function () {
                clearInterval(intId[i]);
            }, function () {
                intId[i] = setInterval(function () {
                    if (_this.find("ul").height() <= _this.height()) {
                        clearInterval(intId[i]);
                    } else {
                        marquee(_this, sh);
                    }
                }, speed);
            });
        });
    }
})(jQuery);
$(document).ready(function () {
    $('.list_lh li:even').addClass('lieven');
})
$(function () {
    $("div.list_lh").myScroll({
        speed: 40, //数值越大，速度越慢
        rowHeight: 50 //li的高度
    });
});
$(function () {
    $("#noticeMore").click(function () {
        parent.OperNoBull();
    });
});
// END滚动公告代码

//加载公告信息
function NoticeList() {
    var NoticeListLi = "";

}

///各地市报表情况
function GetRegionChart() {
    var date = $("#YYYYMM").val();
    $.post("../Home/GetRegion?date=" + date, function (data) {
        var html = "";
        if (data.length > 0) {
            var dataInfo = data.split('◎');
            if (dataInfo.length <= 1) {
                return;
            }
            var regionChart = echarts.init(document.getElementById('bottomLeft'));
            var regionOption = {
                title: {
                    text: '地区薪酬分析'
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: ['计件', '抢盘', '获取分享']
                },
                grid: {
                    left: '15%',
                    right: '1%',
                    bottom: '10%',
                    containLabel: false
                },
                toolbox: {
                    feature: {
                        saveAsImage: {}
                    }
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: eval(dataInfo[0]),
                    axisTick: {
                        alignWithLabel: true
                    },
                    nameLocation: 'end',//坐标轴名称显示位置。
                    axisLabel: {//坐标轴刻度标签的相关设置。
                        interval: 0,
                        rotate: 20
                    }
                },
                yAxis: {
                    type: 'value'
                },
                series: eval(dataInfo[1])
            };
            if (regionOption && typeof regionOption === "object") {
                regionChart.setOption(regionOption, false);
            }
            $(window).resize(function () {
                regionChart.resize();
            });

        }
    });

}

///历史报表情况
function GetHistoryChart() {
    $.post("../Home/GetHistory", function (data) {
        var html = "";
        if (data.length > 0) {
            var dataInfo = data.split('◎');
            if (dataInfo.length <= 1) {
                return;
            }
            var historyChart = echarts.init(document.getElementById('bottomRight'));
            var historyOption = {
                title: {
                    text: '历史薪酬分析'
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: ['计件', '抢盘', '获取分享']
                },
                grid: {
                    left: '15%',
                    right: '5%',
                    bottom: '10%',
                    containLabel: false
                },
                toolbox: {
                    feature: {
                        saveAsImage: {}
                    }
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: eval(dataInfo[0]),
                    axisTick: {
                        alignWithLabel: true
                    },
                    nameLocation: 'end',//坐标轴名称显示位置。
                    axisLabel: {//坐标轴刻度标签的相关设置。
                        interval: 0,
                        rotate: 40
                    }
                },
                yAxis: {
                    type: 'value'
                },
                series: eval(dataInfo[1])
            };
            if (historyOption && typeof historyOption === "object") {
                historyChart.setOption(historyOption, false);
            }
            $(window).resize(function () {
                historyChart.resize();
            });

        }
    });

}