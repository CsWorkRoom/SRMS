$(function () {
    var myChart = echarts.init(document.getElementById('container'));
    var app = {};
    option = null;
    option = {
        title: {
            text: '薪酬统计'
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ['计件薪酬', '抢盘薪酬', '获取分享薪酬']
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        toolbox: {
            feature: {
                saveAsImage: {}
            }
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: ['201806', '201807', '201808', '201809', '201810', '201811', '201812', '201901']
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: '计件薪酬',
                type: 'line',
                stack: '总量',
                data: [120, 132, 101, 134, 90, 230, 210]
            },
            {
                name: '抢盘薪酬',
                type: 'line',
                stack: '总量',
                data: [220, 182, 191, 234, 290, 330, 310]
            },
            {
                name: '获取分享薪酬',
                type: 'line',
                stack: '总量',
                data: [150, 232, 201, 154, 190, 330, 410]
            }
        ]
    };
    
    if (option && typeof option === "object") {
        myChart.setOption(option, true);
        $(window).resize();
    }
    window.onresize = function () {
        myChart.resize();
    }
    
});
