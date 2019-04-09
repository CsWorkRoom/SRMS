layui.define(["jquery", "layer", "laytpl", "element"], function (e) {
    var f = layui.$,
		s = layui.layer,
		c = layui.laytpl,
		n = layui.device(),
		i = layui.element,
		y = document,
		t = false,
		r = {
		    modules: {}
		},
		a = "larryms",
		u = ['<div class="larryms-message" id="messageBox">', '<div class="larryms-message-box clearfix">', '<i class="larry-icon {{d.ICONS}}"></i>', '<div class="resultmsg">{{d.MSG}}</div>', "</div>", "</div>"].join(""),
		d = function (e) {
		    var i = new Array;
		    if (e !== undefined) {
		        scripts = y.getElementsByTagName("script");
		        for (var t = 0; t < scripts.length; t++) {
		            if (e == scripts[t].src.substring(scripts[t].src.lastIndexOf("/") + 1)) {
		                var n = scripts[t].src.split("//")[1];
		                i["commonPath"] = n.substring(n.indexOf("/"), n.lastIndexOf("/layui/") + 1);
		                i["url"] = n.substring(n.indexOf("/"), n.lastIndexOf("/layui/") + 1) + "plus/";
		                i["index"] = t
		            }
		        }
		        i["obj"] = scripts
		    } else {
		        i["url"] = y.currentScript ? y.currentScript.src : function () {
		            var e = y.scripts,
						i = e.length - 1,
						t;
		            for (var n = i; n > 0; n--) {
		                if (e[n].readyState == "interactive") {
		                    t = e[n].src;
		                    break
		                }
		            }
		            return t || e[i].src
		        }();
		        if (i["url"] !== undefined && i["url"] !== "") {
		            i["names"] = i["url"].substring(i["url"].lastIndexOf("/") + 1)
		        } else {
		            i["names"] = "layui.js"
		        }
		    }
		    return i
		},
		l = function () {
		    this.version = " 2.07 Beta2", this.tit = ["系统提示您", "系统错误提示", "larryMS参数配置错误提示", "数据源配置错误", "关闭失败提示", "操作成功", "操作失败", "系统 Ajax 调试信息控制台"], this.config = {
		        plusDir: "/assets/plus/",
		        jqDefined: undefined,
		        jqUrl: undefined
		    }, this.debug = true, this.fonts = {
		        icon: undefined,
		        url: undefined,
		        online: false
		    }
		};

    function o(e) {
        var i = document.getElementsByTagName("head")[0],
			t = document.getElementById(e);
        if (t !== null) {
            i.removeChild(t)
        }
    }
    function p(e, i, t) {
        var n = d("layui.js").commonPath;
        return;
        if (i == undefined) {
            if (f(".larry-icon").length > 0) {
                //larryFonts = n + "css/fonts/larry-icon.css";
                //layui.link(larryFonts)
            }
            if (f(".fa").length > 0) {
                //awesomeFonts = n + "css/fonts/font-awesome.min.css";
                //layui.link(awesomeFonts)
            }
        } else {
            if (t) {
                //o("layuicss-commoncssfontslarry-iconcss");
                //layui.link(i)
            } else {
                //layui.link(i)
            }
        }
    }
    function m() {
        if (n.ie && n.ie < 9) {
            s.alert("本系统最低支持ie8，您当前使用的是古老的 IE" + n.ie + " \n 建议使用IE9及以上版本的现代浏览器", {
                title: "友情提示",
                skin: "larry-debug",
                icon: 2,
                resize: false,
                zIndex: s.zIndex,
                anim: Math.ceil(Math.random() * 6)
            })
        }
        if (n.android || n.ios || f(window).width() < 768) {
            f("body").addClass("larryms-mobile");
            var e = f("#larrymsMobileMenu"),
				i = f("#larrymsMobileShade");
            e.on("click", function () {
                if (f("#larry_layout").hasClass("larryms-mobile-layout")) {
                    f("#larry_layout").removeClass("larryms-mobile-layout")
                } else {
                    f("#larry_layout").addClass("larryms-mobile-layout")
                }
            });
            i.on("click", function () {
                f("#larry_layout").removeClass("larryms-mobile-layout")
            })
        } else {
            f("body").removeClass("larryms-mobile")
        }
        if (n.ie) {
            f(".larryms-layout").addClass("larryms-ie")
        }
        var t = "";
        f("input.larry-input[type='text'],input.larry-input[type='password']").on("focus", function () {
            t = f(this).attr("placeholder");
            f(this).attr("placeholder", "")
        });
        f("input.larry-input[type='text'],input.larry-input[type='password']").on("blur", function () {
            f(this).attr("placeholder", t)
        });
        p()
    }
    l.prototype.set = function (e) {
        var i = this;
        f.extend(true, i.config, e);
        return i
    };
    l.prototype.font = function (e, i, t) {
        if (e == "larry-icon" || e == "font-awesome") {
            if (f(".larry-icon").length > 0 || f(".fa").length > 0) {
                this.alert("默认支持的字体库已完成自动加载，或请检查是否修改了默认目录结构未成功加载icon样式，对于默认icon字体请勿使用该方法加载");
                return false
            }
        }
        p(e, i, t)
    };
    l.prototype.fontset = function (e) {
        var i = this;
        f.extend(true, i.fonts, e);
        i.font(i.fonts.icon, i.fonts.url, i.fonts.online)
    };
    l.prototype.getPath = function (e) {
        return d(e)
    };
    l.prototype.close = function () {
        return s.close(this.index)
    };
    l.prototype.success = function (e, i, t) {
        this.close();
        return this.index = parent.layer.alert(e, {
            title: i,
            skin: "larry-green",
            icon: 1,
            time: (t || 0) * 1e3,
            resize: false,
            zIndex: s.zIndex,
            anim: Math.ceil(Math.random() * 6)
        })
    };
    l.prototype.error = function (e, i, t) {
        this.close();
        return this.index = parent.layer.alert(e, {
            title: i,
            skin: "larry-debug",
            icon: 2,
            time: 0,
            resize: false,
            zIndex: s.zIndex,
            anim: Math.ceil(Math.random() * 6)
        })
    };
    l.prototype.alert = function (e, i) {
        this.close();
        return this.index = s.alert(e, {
            end: i,
            scrollbar: false
        })
    };
    l.prototype.tips = function (e, i, t) {
        s.tips(e, i, t)
    };
    l.prototype.confirm = function (e, i, t) {
        var n = this;
        return this.index = s.confirm(e, {
            icon: 3,
            skin: "larry-green",
            title: n.tit[0],
            closeBtn: 0,
            skin: "layui-layer-molv",
            anim: Math.ceil(Math.random() * 6),
            btn: ["确定", "取消"]
        }, function () {
            if (i && typeof i === "function") {
                i.call(this)
            }
            n.close()
        }, function () {
            if (t && typeof t === "function") {
                t.call(this)
            }
            n.close()
        })
    };
    l.prototype.message = function (e, n, i, t) {
        var r = this,
			e = e || "default",
			n = n || "other",
			s = i || "larry-xiaolian1",
			a = 0,
			l;
        if (t !== undefined && t !== 0) {
            l = t * 1e3
        } else if (t == 0) {
            l = 0;
            a = 1
        } else {
            l = 2500
        }
        if (e != "default") {
            if (n == "success" || n == "error" || n == "waring") {
                if (!i) {
                    if (n == "success") {
                        s = "larry-gou"
                    } else if (n == "error") {
                        s = "larry-cuowu3"
                    } else if (n == "waring") {
                        s = "larry-jinggao3"
                    }
                } else {
                    s = i
                }
            }
            o()
        } else {
            o()
        }
        function o() {
            return this.index = parent.layer.open({
                type: 1,
                closeBtn: a,
                anim: Math.ceil(Math.random() * 6),
                shadeClose: false,
                shade: 0,
                title: false,
                time: l,
                area: ["600px", "auto"],
                resize: false,
                content: c(u).render({
                    MSG: e,
                    ICONS: s
                }),
                offset: "200px",
                success: function (e, i) {
                    if (n == "error") {
                        f("#messageBox").addClass("larry-message-error")
                    } else if (n == "waring") {
                        f("#messageBox").addClass("larry-message-waring")
                    }
                    var t = (f("#messageBox").height() - 80) / 2;
                    f("#messageBox i").css({
                        "margin-top": t
                    });
                    f("#messageBox .resultmsg").width(f("#messageBox").width() - 130)
                }
            })
        }
    };
    l.prototype.plugin = function (e, i, t) {
        if (f.isPlainObject(t)) {
            if (t.plusDir == undefined) {
                t.plusDir = d("layui.js").url
            }
            if (t.jqUrl == undefined) {
                t.jqUrl = d("layui.js").url
            }
            this.set(t)
        } else if (!f.isPlainObject(t) && t !== undefined) {
            this.error("第三方jQuery插件路径参数配置错误，请书写正确配置格式，否则将从默认路径加载插件！", this.tit[2])
        } else {
            this.set({
                plusDir: d("layui.js").url,
                jqDefined: null,
                jqUrl: null
            })
        }
        if (typeof e == "string") {
            var n = this.config.plusDir + e,
				r = y.createElement("script");
            r.type = "text/javascript";
            r.src = n;
            r.async = false;
            var s = d().names,
				a = d(s).obj[d(s).index],
				l = y.getElementsByTagName("body")[0];
            if (!this.config.jqDefined) {
                if (!window.jQuery && f) {
                    window.jQuery = f;
                    if (s != "layui.js") {
                        try {
                            l.insertBefore(r, a)
                        } catch (e) { } finally {
                            a = d("layui.js").obj[d("layui.js").index], l.insertBefore(r, a.nextSibling)
                        }
                    } else {
                        l.insertBefore(r, a.nextSibling.nextSibling)
                    }
                } else if (window.jQuery && f) {
                    if (s != "layui.js") {
                        try {
                            l.insertBefore(r, a)
                        } catch (e) { } finally {
                            a = d("layui.js").obj[d("layui.js").index], l.insertBefore(r, a.nextSibling)
                        }
                    } else {
                        l.insertBefore(r, a.nextSibling)
                    }
                } else {
                    this.error("上下文环境中未检测jQuery对象，请正确配置自定义jq插件或手动在页面中引入 否则任何依赖jquery的第三方插件将不能正常运行！！！", this.tit[1])
                }
                u()
            } else {
                var o = this.config.jqUrl + this.config.jqDefined,
					c = y.getElementsByTagName("head")[0];
                jq = y.createElement("script");
                jq.type = "text/javascript";
                jq.src = o;
                c.appendChild(jq);
                if (y.all) {
                    jq.onreadystatechange = function () {
                        var e = this.readyState;
                        if (e === "loaded" || e === "complete") {
                            window.jQuery = f;
                            if (s != "layui.js") {
                                try {
                                    l.insertBefore(r, a)
                                } catch (e) { } finally {
                                    l.appendChild(r)
                                }
                            } else {
                                l.insertBefore(r, a.nextSibling)
                            }
                            u()
                        }
                    }
                } else {
                    jq.onload = function () {
                        window.jQuery = f;
                        if (s != "layui.js") {
                            try {
                                l.insertBefore(r, a)
                            } catch (e) { } finally {
                                l.appendChild(r)
                            }
                        } else {
                            l.insertBefore(r, a.nextSibling)
                        }
                        u()
                    }
                }
            }
        }
        function u() {
            if (y.all) {
                r.onreadystatechange = function () {
                    var e = this.readyState;
                    if (e === "loaded" || e === "complete") {
                        i()
                    }
                }
            } else {
                r.onload = function () {
                    i()
                }
            }
        }
    };
    l.prototype.htmlRender = function (e, i) {
        var t = layui.cache.base + e + ".html";
        f.ajax({
            url: t,
            type: "get",
            dataType: "html",
            async: false,
            success: function (e) {
                f(i).html(e)
            }
        })
    };
    l.prototype.ajaxDebug = function (e, i, t, n) {
        if (this.debug) {
            if (e.readyState == 4) {
                var r = s.open({
                    type: 1,
                    skin: "larry-debug",
                    shadeClose: false,
                    shade: 0,
                    title: this.tit[7],
                    area: ["800px", "660px"],
                    content: e.responseText,
                    resize: true,
                    btn: ["关闭不刷新", "关闭并刷新当前页"],
                    yes: function (e, i) {
                        this.close(e);
                        f(n).removeClass("layui-disabled").prop("disabled", false)
                    },
                    btn2: function (e, i) {
                        this.close(e);
                        location.reload()
                    }
                })
            }
        } else {
            this.error("数据提交失败！", this.tit[0])
        }
    };
    l.prototype.cleanCached = function (e) {
        layui.data(e, null);
        localStorage.clear();
        sessionStorage.clear();
        g()
    };
    l.prototype.logOut = function (e, i, t) {
        this.confirm("确定退出系统吗！", function () {
            top.location.href = e
        }, t = h)
    };

    function h() {
        s.msg("成功返回系统", {
            time: 1e3,
            btnAlign: "c"
        })
    }
    function g() {
        var e = document.cookie.match(/[^ =;]+(?=\=)/g);
        if (e) {
            for (var i = e.length; i--;) document.cookie = e[i] + "=0;expires=" + new Date(0).toUTCString()
        }
    }
    if (window.top == window.self) {
        m()
    }
    var x = new l;
    e(a, x)
});