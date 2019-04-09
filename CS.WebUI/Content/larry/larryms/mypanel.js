layui.define(["larry", "form", "upload"], function (r) {
	"use strict";
	var a = layui.$, l = layui.larry, u = layui.form, o = layui.upload;
	var e = o.render({
		elem: "#larry_photo", url: "/upload/", done: function (r) {
		}, error: function () {
		}
	});
	var i = {};
	r("mypanel", {})
});