layui.define(["jquery", "form", "larryTab", "laytpl", "larry"], function (r) {
	var l = layui.$, o = layui.form, e = layui.larryTab({tab_elem: "#larry_tab", tabMax: 30}), a = layui.layer,
		i = layui.laytpl, s = layui.larry, n = layui.larryms, y = l(window), m = l("body");
	var d = layui.data("larryms").lockscreen;
	if (d === "locked") {
		c()
	}
	e.menuSet({type: "POST", url: layui.cache.menusUrl, left_menu: "#larryms_left_menu", leftFilter: "LarrySide"});
	e.menu();
	if (e.config.tabSession) {
		e.session(function (r) {
			if (r.getItem("tabMenu")) {
				l("#larry_tab_title li.layui-this").trigger("click")
			}
		})
	}
	l("#larryms_version").text(n.version);
	l("#menufold").on("click", function () {
		if (l("#larry_layout").hasClass("larryms-fold")) {
			l("#larry_layout").addClass("larryms-unfold").removeClass("larryms-fold");
			l(this).children("i").addClass("larry-fold7").removeClass("larry-unfold")
		} else {
			l("#larry_layout").addClass("larryms-fold").removeClass("larryms-unfold");
			l(this).children("i").addClass("larry-unfold").removeClass("larry-fold7")
		}
	});
	l("#larryTheme").on("click", function () {
		var r = a.open({
			type: 1,
			id: "larry_theme_R",
			title: false,
			anim: Math.ceil(Math.random() * 6),
			offset: "r",
			closeBtn: false,
			shade: .2,
			shadeClose: true,
			skin: "layui-anim layui-anim-rl larryms-layer-right",
			area: "320px",
			success: function (r, t) {
				var o = layui.cache.base + "templets/style/theme.css";
				layui.link(o);
				n.htmlRender("templets/theme", r)
			}
		})
	});
	l("#clearCached").off("click").on("click", function () {
		n.cleanCached("larry_menu");
		a.alert("缓存清除完成!本地存储数据也清理成功！", {
			icon: 1, title: "系统提示", end: function () {
				top.location.reload()
			}
		})
	});
	l("#logout").off("click").on("click", function () {
		var r = l(this).data("url");
		n.logOut(r)
	});
	l("#lock").mouseover(function () {
		a.tips("请按Alt+L快速锁屏！", l(this), {tips: [1, "#FF5722"], time: 1500})
	});
	l("#lock").off("click").on("click", function () {
		c()
	});

	function c() {
		var r = l("#user_photo").attr("src"), t = l("#uname").text();
		h({Display: "block", UserPhoto: r, UserName: t});
		layui.data("larryms", {key: "lockscreen", value: "locked"});
		p()
	}

	function f() {
		var r = l("#user_photo").attr("src"), t = l("#uname").text();
		if (l("#unlock_pass").val() === "easyman") {
			h({Display: "none", UserPhoto: r, UserName: t})
		} else {
			a.tips("模拟锁屏，输入密码：easyman 解锁", l("#unlock"), {tips: [2, "#FF5722"], time: 1e3});
			return
		}
	}

	l(document).keydown(function () {
		return u(arguments[0])
	});

	function u(r) {
		var t;
		if (window.event) {
			t = r.keyCode
		} else if (r.which) {
			t = r.which
		}
		if (r.altKey && t == 76) {
			c()
		}
	}

	function h(r) {
		var t = "larry_lock_screen", o = document.createElement("div"),
			e = i(['<div class="lock-screen" style="display: {{d.Display}};">', '<div class="lock-wrapper" id="lock-screen">', '<div id="time"></div>', '<div class="lock-box">', '<img src="{{d.UserPhoto}}" alt="">', "<h1>{{d.UserName}}</h1>", '<form action="" class="layui-form lock-form">', '<div class="layui-form-item">', '<input type="password" id="unlock_pass" name="lock_password" lay-verify="pass" placeholder="锁屏状态，请输入密码解锁" autocomplete="off" class="layui-input"  autofocus="">', "</div>", '<div class="layui-form-item">', '<span class="layui-btn larry-btn" id="unlock">立即解锁</span>', "</div>", "</form>", "</div>", "</div>", "</div>"].join("")).render(r),
			a = document.getElementById(t);
		o.id = t;
		o.innerHTML = e;
		a && m[0].removeChild(a);
		if (r.Display !== "none") {
			m[0].appendChild(o)
		} else {
			l("#larry_lock_screen").empty()
		}
		l("#unlock").off("click").on("click", function () {
			f();
			layui.data("larryms", {key: "lockscreen", value: "unlock"})
		});
		l("#unlock_pass").keypress(function (r) {
			if (window.event && window.event.keyCode == 13) {
				l("#unlock").click();
				return false
			}
		})
	}

	function p() {
		var r = new Date;
		var o = r.getHours();
		var e = r.getMinutes();
		var a = r.getSeconds();
		e = e < 10 ? "0" + e : e;
		a = a < 10 ? "0" + a : a;
		l("#time").html(o + ":" + e + ":" + a);
		t = setTimeout(function () {
			p()
		}, 500)
	}

	var v = function () {
		this.themeColor = {
			default: {
			    topColor: "#0067b8",
				topThis: "#1958A6",
				topBottom: "#01AAED",
				leftColor: "#2f3a4f",
				leftRight: "#258ED8",
				navThis: "#0067b8",
				titBottom: "#1E9FFF",
				footColor: "#245c87",
				name: "default"
			},
			deepBlue: {
			    topColor: "#0067b8",
				topThis: "#1958A6",
				topBottom: "#01AAED",
				leftColor: "#2f3a4f",
				leftRight: "#258ED8",
				navThis: "#0067b8",
				titBottom: "#1E9FFF",
				footColor: "#245c87",
				name: "deepBlue"
			},
			green: {
				topColor: "#2a877b",
				topThis: "#5FB878",
				topBottom: "#50A66F",
				leftColor: "#343742",
				leftRight: "#50A66F",
				navThis: "#56a66c",
				titBottom: "#50A66F",
				footColor: "#3e4e63",
				name: "green"
			},
			navy: {
				topColor: "#2f4056",
				topThis: "#0d51a9",
				topBottom: "#01AAED",
				leftColor: "#393d49",
				leftRight: "#1E9FFF",
				navThis: "#1E9FFF",
				titBottom: "#01AAED",
				footColor: "#343742",
				name: "navy"
			},
			orange: {
				topColor: "#F39C34",
				topThis: "#CD7013",
				topBottom: "#FF5722",
				leftColor: "#1d1f26",
				leftRight: "#FFB800",
				navThis: "#df7700",
				titBottom: "#FFB800",
				footColor: "#f2f2f2",
				footFont: "#666",
				name: "orange"
			}
		}
	};
	v.prototype.theme = function (r) {
		var t = "Larryms_theme_style", o = document.createElement("style"), e = layui.data("larryms"),
			a = i([".layui-header{background-color:{{d.topColor}} !important;border-bottom:3px solid {{d.topBottom}};}", ".larryms-extend{border-left:1px solid {{d.topThis}} }", ".larryms-nav-bar{background-color:{{d.topBottom}} !important;}", ".larryms-extend .larryms-nav li.larryms-this{background:{{d.topThis}} !important; }", ".larryms-extend .larryms-nav li.larryms-nav-item:hover{background:{{d.topThis}} !important; }", ".larryms-extend .larryms-nav li.larryms-this:hover{background:{{d.topThis}} }", ".larryms-fold .larryms-header .larryms-topbar-left .larryms-switch{border-left:1px solid {{d.topThis}} !important;}", ".larryms-extend  ul.layui-nav li.layui-nav-item:hover{background:{{d.topThis}} !important;}", ".larryms-topbar-right .layui-nav-bar{background-color: {{d.navThis}} !important;}", ".larryms-nav-tree .larryms-this,", ".larryms-nav-tree .larryms-this>a{background-color:{{d.navThis}} !important;}", ".larryms-body .larryms-left{border-right:2px solid {{d.leftRight}} !important;}", ".layui-bg-black{background-color:{{d.leftColor}} !important;}", ".larryms-body .larryms-left{background:{{d.leftColor}} !important;}", "ul.larryms-tab-title .layui-this{background:{{d.navThis}} !important;}", ".larryms-right .larryms-tab .larryms-title-box{border-bottom:1px solid  {{d.titBottom}};}", ".larryms-right .larryms-tab .larryms-title-box .larryms-tab-title{border-bottom:1px solid  {{d.titBottom}};}", ".larryms-layout .larryms-footer{background:{{d.footColor}} !important;color:{{d.footFont}} !important;}"].join("")).render(r),
			l = document.getElementById(t);
		if ("styleSheet" in o) {
			o.setAttribute("type", "text/css");
			o.styleSheet.cssText = a
		} else {
			o.innerHTML = a
		}
		o.id = t;
		l && m[0].removeChild(l);
		m[0].appendChild(o);
		e.theme = e.theme || {};
		layui.each(r, function (r, t) {
			e.theme[r] = t
		});
		layui.data("larryms", {key: "theme", value: e.theme})
	};
	v.prototype.init = function () {
		var r = this, t = layui.data("larryms").theme, o = layui.data("larryms").systemSet;
		if (t !== undefined) {
			//console.log(t.name);
			r.theme(t);
			if (t.name == "default") {
				l("#Larryms_theme_style").empty()
			}
		}
		if (o !== undefined) {
			e.tabSet({tabSession: o.tabCache, autoRefresh: o.tabRefresh});
			l("#larry_footer").data("show", o.footSet)
		} else {
			layui.data("larryms", {
				key: "systemSet",
				value: {
					tabCache: e.config.tabSession,
					tabRefresh: e.config.autoRefresh,
					fullScreen: false,
					footSet: l("#larry_footer").data("show")
				}
			})
		}
		b()
	};
	v.prototype.footInit = function (r) {
		l("#larry_footer").data("show", r);
		b()
	};

	function b() {
		if (l("#larry_footer").data("show") !== "on") {
			l("#larry_footer").hide();
			l("#larry_right").css({bottom: "0px"})
		} else {
			l("#larry_footer").show();
			l("#larry_right").css({bottom: "40px"})
		}
	}

	l(window).on("resize", function () {
		var r = l(window).width();
		if (r >= 1200) {
			l("#larry_layout").removeClass("larryms-mobile-layout");
			l("#larry_layout").addClass("larryms-unfold").removeClass("larryms-fold");
			l("#menufold").children("i.larry-icon").addClass("larry-fold7").removeClass("larry-unfold")
		} else if (r > 767 && r < 1200) {
			l("#larry_layout").removeClass("larryms-mobile-layout");
			l("#larry_layout").addClass("larryms-fold").removeClass("larryms-unfold");
			l("#menufold").children("i.larry-icon").addClass("larry-unfold").removeClass("larry-fold7")
		} else if (r <= 767 && r > 319) {
			l("#larry_layout").removeClass("larryms-fold");
			l("#larry_layout").removeClass("larryms-unfold")
		} else if (r <= 319) {
			n.error("你丫的别拖了，没有屏幕宽度小于320的，布局会乱的！", n.tit[1])
		}
	}).resize();
	var k = new v;
	k.init();
	r("index", k)
});