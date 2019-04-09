layui.define(["larry", "form", "larryms"], function (i) {
	"use strict";
	var e = layui.$, a = layui.layer, r = layui.larryms, s = layui.form;

	function n() {
		e.supersized({
			slide_interval: 3e3,
			transition: 1,
			transition_speed: 1e3,
			performance: 1,
			min_width: 0,
			min_height: 0,
			vertical_center: 1,
			horizontal_center: 1,
			fit_always: 0,
			fit_portrait: 1,
			fit_landscape: 0,
			slide_links: "blank",
			slides: [{image: "../../images/login/1.jpg"}, {image: "../../images/login/2.jpg"}, {image: "../../images/login/3.jpg"}]
		})
	}

	r.plugin("jquery.supersized.min.js", n);
	s.on("submit(submit)", function (i) {
		if (i.field.uname == "larry" && i.field.password == "larry") {
			a.msg("登录成功", {icon: 1, time: 1e3});
			setTimeout(function () {
				window.location.href = "index.html"
			}, 1e3)
		} else {
			a.tips("用户名:larry 密码：larry 无需输入验证码", e("#password"), {tips: [3, "#FF5722"]})
		}
		return false
	});
	i("login", {})
});