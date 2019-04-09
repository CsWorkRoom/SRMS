layui.define(["jquery", "larry"], function (i) {
	"use strict";
	var n = layui.$, e = layui.larry, s = layui.larryms;
	s.plugin("clipboard.min.js", function () {
		t()
	});

	function t() {
		n(".icon_lists").find("li").each(function (i, e) {
			n(e).attr("data-clipboard-text", n(e).children(".fontclass").text())
		});
		var i = document.querySelectorAll("li");
		var e = new Clipboard(i);
		e.on("success", function (i) {
			s.message("已成功复制" + i.text, "success", 1500)
		})
	}

	n("#nums").text(n(".icon_lists").find("li").length);
	n("#search_text").focus();
	n("#search_icon").click(function () {
		var i = n("#search_text").val();
		if (n.trim(i) != "") {
			n(".icon_lists li").hide().filter(":contains('" + i + "')").show()
		} else {
			s.message("请输入点什么再搜索吧！", "error", 1500);
			n(".icon_lists li").show()
		}
	});
	n("#search_text").keydown(function () {
		if (event.keyCode == "13") {
			var i = n("#search_text").val();
			if (n.trim(i) != "") {
				n(".icon_lists li").hide().filter(":contains('" + i + "')").show()
			} else {
				s.message("请输入点什么再搜索吧！", "error", 1500);
				n(".icon_lists li").show()
			}
		}
	});
	if (layui.cache.identify && layui.cache.identify == "animate") {
		s.plugin("jquery-migrate.min.js", function () {
			n(function () {
				if (n.browser.msie && n.browser.version < 10) {
					n(".jq22-explain").show()
				}
				var i = n("#animate");
				var e = n(".tabCnt").find("li");
				e.click(function () {
					n(this).addClass("active").siblings().removeClass("active");
					i.removeClass().addClass(n(this).text() + " animated infinite");
					setTimeout(s, 1e3)
				});

				function s() {
					i.removeClass()
				}

				var t = n(".tabNav").find("a");
				var a = n(".tabPane");
				t.each(function (i) {
					n(this).click(function () {
						n(this).parent().addClass("active").siblings().removeClass("active");
						a.eq(i).addClass("active").siblings().removeClass("active");
						return false
					})
				})
			})
		})
	}
	i("demo", {})
});