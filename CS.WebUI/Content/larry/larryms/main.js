layui.define(["jquery", "larry"], function (e) {
	"use strict";
	var t = layui.$, a = layui.larry, i = layui.larryms;
	t("#closeInfo").on("click", function () {
		t("#Minfo").hide()
	});
	t("#shortcut a").off("click").on("click", function () {
		var e = {
			href: t(this).data("url"),
			icon: t(this).data("icon"),
			ids: t(this).data("id"),
			group: t(this).data("group"),
			title: t(this).children(".larry-value").children("cite").text()
		}
	});
	a.panel();
	var r = new Date, n = r.getFullYear(), l = r.getMonth() + 1, o = r.getDate(), s = r.toLocaleTimeString();
	t("#weather").html("您好，现在时间是：" + n + "年" + l + "月" + o + "日 " + s + "（欢迎你使用）");
	t("#versionT").text(i.version);
	// i.plugin("echarts.js", u);

	function u() {
		var e = echarts.init(document.getElementById("larryCount")), t = {
			title: {text: "用户访问来源", subtext: "纯属虚构", x: "center"},
			tooltip: {trigger: "item", formatter: "{a} <br/>{b} : {c} ({d}%)"},
			legend: {orient: "vertical", left: "left", data: ["直接访问", "邮件营销", "联盟广告", "视频广告", "搜索引擎"]},
			series: [{
				name: "访问来源",
				type: "pie",
				radius: "55%",
				center: ["50%", "60%"],
				data: [{value: 335, name: "直接访问"}, {value: 310, name: "邮件营销"}, {value: 234, name: "联盟广告"}, {
					value: 135,
					name: "视频广告"
				}, {value: 1548, name: "搜索引擎"}],
				itemStyle: {emphasis: {shadowBlur: 10, shadowOffsetX: 0, shadowColor: "rgba(0, 0, 0, 0.5)"}}
			}]
		};
		e.setOption(t);
		window.onresize = function () {
			e.resize()
		}
	}

	e("main", {})
});