$(function ()
{
    setTotal();
    setFee();
});

function setFee() {
    $.post("../Home/GetFee",
        function (data) {
            data = JSON.parse(data);
            var myChart = echarts.init(document.getElementById('sumMoney'));
            var option = {
                legend: {},
                tooltip: {
                    trigger: 'axis',
                    showContent: false
                },
                dataset: {
                    source:data.source
                },
                xAxis: { type: 'category' },
                yAxis: { gridIndex: 0 },
                grid: {},
                series: data.series
            };
            myChart.on('updateAxisPointer',
                function(event) {
                    var xAxisInfo = event.axesInfo[0];
                    if (xAxisInfo) {
                        var dimension = xAxisInfo.value + 1;
                        myChart.setOption({
                            series: {
                                id: 'pie',
                                label: {
                                    formatter: '{b}: {@[' + dimension + ']} ({d}%)'
                                },
                                encode: {
                                    value: dimension,
                                    tooltip: dimension
                                }
                            }
                        });
                    }
                });

            myChart.setOption(option);
        });
}


function setTotal()
{
    $.post("../Home/GetTotal", function (data)
    {
        data = JSON.parse(data);
        var myChart = echarts.init(document.getElementById('sumTotal'));
        var option = null;
        option = {
            title: {
                text: ''
            },
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data:data.legend
            },
            grid: {
                left: '3%',
                right: '4%',
                bottom: '0%',
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
                data: data.xaxis
            },
            yAxis: {
                type: 'value'
            },
            series: data.series
        };


        if (option && typeof option === "object")
        {
            myChart.setOption(option, true);
            $(window).resize();
        }
        //window.onresize = function ()
        //{
        //    myChart.resize();
        //}
    });
}
