﻿/*
 Highmaps JS v1.1.5 (2015-04-13)

 (c) 2009-2014 Torstein Honsi

 License: www.highcharts.com/license
*/
(function () {
    function A() { var a, b = arguments, c, d = {}, e = function (a, b) { var c, d; typeof a !== "object" && (a = {}); for (d in b) b.hasOwnProperty(d) && (c = b[d], a[d] = c && typeof c === "object" && Object.prototype.toString.call(c) !== "[object Array]" && d !== "renderTo" && typeof c.nodeType !== "number" ? e(a[d] || {}, c) : b[d]); return a }; b[0] === !0 && (d = b[1], b = Array.prototype.slice.call(b, 2)); c = b.length; for (a = 0; a < c; a++) d = e(d, b[a]); return d } function D(a, b) { return parseInt(a, b || 10) } function Ba(a) { return typeof a === "string" } function ea(a) {
        return a &&
        typeof a === "object"
    } function Ca(a) { return Object.prototype.toString.call(a) === "[object Array]" } function qa(a) { return typeof a === "number" } function Qa(a) { return J.log(a) / J.LN10 } function na(a) { return J.pow(10, a) } function va(a, b) { for (var c = a.length; c--;) if (a[c] === b) { a.splice(c, 1); break } } function r(a) { return a !== q && a !== null } function H(a, b, c) { var d, e; if (Ba(b)) r(c) ? a.setAttribute(b, c) : a && a.getAttribute && (e = a.getAttribute(b)); else if (r(b) && ea(b)) for (d in b) a.setAttribute(d, b[d]); return e } function ra(a) {
        return Ca(a) ?
            a : [a]
    } function F(a, b) { if (wa && !ca && b && b.opacity !== q) b.filter = "alpha(opacity=" + b.opacity * 100 + ")"; s(a.style, b) } function $(a, b, c, d, e) { a = C.createElement(a); b && s(a, b); e && F(a, { padding: 0, border: O, margin: 0 }); c && F(a, c); d && d.appendChild(a); return a } function aa(a, b) { var c = function () { return q }; c.prototype = new a; s(c.prototype, b); return c } function Da(a, b) { return Array((b || 2) + 1 - String(a).length).join(0) + a } function Ja(a, b) {
        for (var c = "{", d = !1, e, f, g, h, i, j = []; (c = a.indexOf(c)) !== -1;) {
            e = a.slice(0, c); if (d) {
                f = e.split(":");
                g = f.shift().split("."); i = g.length; e = b; for (h = 0; h < i; h++) e = e[g[h]]; if (f.length) f = f.join(":"), g = /\.([0-9])/, h = I.lang, i = void 0, /f$/.test(f) ? (i = (i = f.match(g)) ? i[1] : -1, e !== null && (e = z.numberFormat(e, i, h.decimalPoint, f.indexOf(",") > -1 ? h.thousandsSep : ""))) : e = Ka(f, e)
            } j.push(e); a = a.slice(c + 1); c = (d = !d) ? "}" : "{"
        } j.push(a); return j.join("")
    } function sb(a, b, c, d, e) {
        var f, g = a, c = o(c, 1); f = a / c; b || (b = [1, 2, 2.5, 5, 10], d === !1 && (c === 1 ? b = [1, 2, 5, 10] : c <= 0.1 && (b = [1 / c]))); for (d = 0; d < b.length; d++) if (g = b[d], e && g * c >= a || !e && f <= (b[d] +
        (b[d + 1] || b[d])) / 2) break; g *= c; return g
    } function hb(a, b) { var c = a.length, d, e; for (e = 0; e < c; e++) a[e].ss_i = e; a.sort(function (a, c) { d = b(a, c); return d === 0 ? a.ss_i - c.ss_i : d }); for (e = 0; e < c; e++) delete a[e].ss_i } function Ra(a) { for (var b = a.length, c = a[0]; b--;) a[b] < c && (c = a[b]); return c } function Ea(a) { for (var b = a.length, c = a[0]; b--;) a[b] > c && (c = a[b]); return c } function $a(a, b) { for (var c in a) a[c] && a[c] !== b && a[c].destroy && a[c].destroy(), delete a[c] } function La(a) { ab || (ab = $(Fa)); a && ab.appendChild(a); ab.innerHTML = "" } function ba(a,
    b) { var c = "Highcharts error #" + a + ": www.highcharts.com/errors/" + a; if (b) throw c; K.console && console.log(c) } function xa(a) { return parseFloat(a.toPrecision(14)) } function tb() { var a = I.global, b = a.useUTC, c = b ? "getUTC" : "get", d = b ? "setUTC" : "set"; Ga = a.Date || window.Date; ub = b && a.timezoneOffset; ib = b && a.getTimezoneOffset; vb = c + "Minutes"; wb = c + "Hours"; xb = c + "Day"; yb = c + "Date"; jb = c + "Month"; kb = c + "FullYear"; zb = d + "Month"; Ab = d + "FullYear" } function P() { } function Ma(a, b, c, d) {
        this.axis = a; this.pos = b; this.type = c || ""; this.isNew = !0;
        !c && !d && this.addLabel()
    } function Bb(a, b) { var c, d, e, f, g = !1, h = a.x, i = a.y; for (c = 0, d = b.length - 1; c < b.length; d = c++) e = b[c][1] > i, f = b[d][1] > i, e !== f && h < (b[d][0] - b[c][0]) * (i - b[c][1]) / (b[d][1] - b[c][1]) + b[c][0] && (g = !g); return g } function Cb(a, b, c, d, e, f, g, h, i) { a = a["stroke-width"] % 2 / 2; b -= a; c -= a; return ["M", b + f, c, "L", b + d - g, c, "C", b + d - g / 2, c, b + d, c + g / 2, b + d, c + g, "L", b + d, c + e - h, "C", b + d, c + e - h / 2, b + d - h / 2, c + e, b + d - h, c + e, "L", b + i, c + e, "C", b + i / 2, c + e, b, c + e - i / 2, b, c + e - i, "L", b, c + f, "C", b, c + f / 2, b + f / 2, c, b + f, c, "Z"] } var q, C = document, K = window,
    J = Math, x = J.round, fa = J.floor, sa = J.ceil, w = J.max, N = J.min, Q = J.abs, ka = J.cos, oa = J.sin, lb = J.PI, la = lb * 2 / 360, ya = navigator.userAgent, Db = K.opera, wa = /(msie|trident)/i.test(ya) && !Db, bb = C.documentMode === 8, mb = /AppleWebKit/.test(ya), Ha = /Firefox/.test(ya), Eb = /(Mobile|Android|Windows Phone)/.test(ya), za = "http://www.w3.org/2000/svg", ca = !!C.createElementNS && !!C.createElementNS(za, "svg").createSVGRect, Jb = Ha && parseInt(ya.split("Firefox/")[1], 10) < 4, ga = !ca && !wa && !!C.createElement("canvas").getContext, Na, Sa, Fb = {}, nb = 0, ab,
    I, Ka, ha, ob, cb, T = function () { return q }, W = [], Ta = 0, Fa = "div", O = "none", Kb = /^[0-9]+$/, db = ["plotTop", "marginRight", "marginBottom", "plotLeft"], Lb = "stroke-width", Ga, ub, ib, vb, wb, xb, yb, jb, kb, zb, Ab, u = {}, z; z = K.Highcharts = K.Highcharts ? ba(16, !0) : {}; z.seriesTypes = u; var s = z.extend = function (a, b) { var c; a || (a = {}); for (c in b) a[c] = b[c]; return a }, o = z.pick = function () { var a = arguments, b, c, d = a.length; for (b = 0; b < d; b++) if (c = a[b], c !== q && c !== null) return c }, da = z.wrap = function (a, b, c) {
        var d = a[b]; a[b] = function () {
            var a = Array.prototype.slice.call(arguments);
            a.unshift(d); return c.apply(this, a)
        }
    }; Ka = function (a, b, c) {
        if (!r(b) || isNaN(b)) return "Invalid date"; var a = o(a, "%Y-%m-%d %H:%M:%S"), d = new Ga(b - (ib && ib(b) || ub || 0) * 6E4), e, f = d[wb](), g = d[xb](), h = d[yb](), i = d[jb](), j = d[kb](), k = I.lang, l = k.weekdays, d = s({ a: l[g].substr(0, 3), A: l[g], d: Da(h), e: h, w: g, b: k.shortMonths[i], B: k.months[i], m: Da(i + 1), y: j.toString().substr(2, 2), Y: j, H: Da(f), I: Da(f % 12 || 12), l: f % 12 || 12, M: Da(d[vb]()), p: f < 12 ? "AM" : "PM", P: f < 12 ? "am" : "pm", S: Da(d.getSeconds()), L: Da(x(b % 1E3), 3) }, z.dateFormats); for (e in d) for (; a.indexOf("%" +
        e) !== -1;) a = a.replace("%" + e, typeof d[e] === "function" ? d[e](b) : d[e]); return c ? a.substr(0, 1).toUpperCase() + a.substr(1) : a
    }; cb = { millisecond: 1, second: 1E3, minute: 6E4, hour: 36E5, day: 864E5, week: 6048E5, month: 24192E5, year: 314496E5 }; z.numberFormat = function (a, b, c, d) {
        var e = I.lang, a = +a || 0, f = b === -1 ? N((a.toString().split(".")[1] || "").length, 20) : isNaN(b = Q(b)) ? 2 : b, b = c === void 0 ? e.decimalPoint : c, d = d === void 0 ? e.thousandsSep : d, e = a < 0 ? "-" : "", c = String(D(a = Q(a).toFixed(f))), g = c.length > 3 ? c.length % 3 : 0; return e + (g ? c.substr(0, g) +
            d : "") + c.substr(g).replace(/(\d{3})(?=\d)/g, "$1" + d) + (f ? b + Q(a - c).toFixed(f).slice(2) : "")
    }; ob = {
        init: function (a, b, c) {
            var b = b || "", d = a.shift, e = b.indexOf("C") > -1, f = e ? 7 : 3, g, b = b.split(" "), c = [].concat(c), h, i, j = function (a) { for (g = a.length; g--;) a[g] === "M" && a.splice(g + 1, 0, a[g + 1], a[g + 2], a[g + 1], a[g + 2]) }; e && (j(b), j(c)); a.isArea && (h = b.splice(b.length - 6, 6), i = c.splice(c.length - 6, 6)); if (d <= c.length / f && b.length === c.length) for (; d--;) c = [].concat(c).splice(0, f).concat(c); a.shift = 0; if (b.length) for (a = c.length; b.length < a;) d =
            [].concat(b).splice(b.length - f, f), e && (d[f - 6] = d[f - 2], d[f - 5] = d[f - 1]), b = b.concat(d); h && (b = b.concat(h), c = c.concat(i)); return [b, c]
        }, step: function (a, b, c, d) { var e = [], f = a.length; if (c === 1) e = d; else if (f === b.length && c < 1) for (; f--;) d = parseFloat(a[f]), e[f] = isNaN(d) ? a[f] : c * parseFloat(b[f] - d) + d; else e = b; return e }
    }; (function (a) {
        K.HighchartsAdapter = K.HighchartsAdapter || a && {
            init: function (b) {
                var c = a.fx; a.extend(a.easing, { easeOutQuad: function (a, b, c, g, h) { return -g * (b /= h) * (b - 2) + c } }); a.each(["cur", "_default", "width", "height",
                "opacity"], function (b, e) { var f = c.step, g; e === "cur" ? f = c.prototype : e === "_default" && a.Tween && (f = a.Tween.propHooks[e], e = "set"); (g = f[e]) && (f[e] = function (a) { var c, a = b ? a : this; if (a.prop !== "align") return c = a.elem, c.attr ? c.attr(a.prop, e === "cur" ? q : a.now) : g.apply(this, arguments) }) }); da(a.cssHooks.opacity, "get", function (a, b, c) { return b.attr ? b.opacity || 0 : a.call(this, b, c) }); this.addAnimSetter("d", function (a) {
                    var c = a.elem, f; if (!a.started) f = b.init(c, c.d, c.toD), a.start = f[0], a.end = f[1], a.started = !0; c.attr("d", b.step(a.start,
                    a.end, a.pos, c.toD))
                }); this.each = Array.prototype.forEach ? function (a, b) { return Array.prototype.forEach.call(a, b) } : function (a, b) { var c, g = a.length; for (c = 0; c < g; c++) if (b.call(a[c], a[c], c, a) === !1) return c }; a.fn.highcharts = function () { var a = "Chart", b = arguments, c, g; if (this[0]) { Ba(b[0]) && (a = b[0], b = Array.prototype.slice.call(b, 1)); c = b[0]; if (c !== q) c.chart = c.chart || {}, c.chart.renderTo = this[0], new z[a](c, b[1]), g = this; c === q && (g = W[H(this[0], "data-highcharts-chart")]) } return g }
            }, addAnimSetter: function (b, c) {
                a.Tween ?
                a.Tween.propHooks[b] = { set: c } : a.fx.step[b] = c
            }, getScript: a.getScript, inArray: a.inArray, adapterRun: function (b, c) { return a(b)[c]() }, grep: a.grep, map: function (a, c) { for (var d = [], e = 0, f = a.length; e < f; e++) d[e] = c.call(a[e], a[e], e, a); return d }, offset: function (b) { return a(b).offset() }, addEvent: function (b, c, d) { a(b).bind(c, d) }, removeEvent: function (b, c, d) { var e = C.removeEventListener ? "removeEventListener" : "detachEvent"; C[e] && b && !b[e] && (b[e] = function () { }); a(b).unbind(c, d) }, fireEvent: function (b, c, d, e) {
                var f = a.Event(c),
                g = "detached" + c, h; !wa && d && (delete d.layerX, delete d.layerY, delete d.returnValue); s(f, d); b[c] && (b[g] = b[c], b[c] = null); a.each(["preventDefault", "stopPropagation"], function (a, b) { var c = f[b]; f[b] = function () { try { c.call(f) } catch (a) { b === "preventDefault" && (h = !0) } } }); a(b).trigger(f); b[g] && (b[c] = b[g], b[g] = null); e && !f.isDefaultPrevented() && !h && e(f)
            }, washMouseEvent: function (a) { var c = a.originalEvent || a; if (c.pageX === q) c.pageX = a.pageX, c.pageY = a.pageY; return c }, animate: function (b, c, d) {
                var e = a(b); if (!b.style) b.style =
                {}; if (c.d) b.toD = c.d, c.d = 1; e.stop(); c.opacity !== q && b.attr && (c.opacity += "px"); b.hasAnim = 1; e.animate(c, d)
            }, stop: function (b) { b.hasAnim && a(b).stop() }
        }
    })(K.jQuery); var ta = K.HighchartsAdapter, E = ta || {}; ta && ta.init.call(ta, ob); var eb = E.adapterRun, Mb = E.getScript, Ua = E.inArray, m = z.each = E.each, pb = E.grep, Nb = E.offset, Oa = E.map, M = E.addEvent, Y = E.removeEvent, L = E.fireEvent, Ob = E.washMouseEvent, fb = E.animate, Va = E.stop; I = {
        colors: "#3B9C9C,#434348,#90ed7d,#f7a35c,#8085e9,#f15c80,#e4d354,#2b908f,#f45b5b,#91e8e1".split(","),
        symbols: ["circle", "diamond", "square", "triangle", "triangle-down"], lang: { loading: "Loading...", months: "January,February,March,April,May,June,July,August,September,October,November,December".split(","), shortMonths: "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec".split(","), weekdays: "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday".split(","), decimalPoint: ".", numericSymbols: "k,M,G,T,P,E".split(","), resetZoom: "Reset zoom", resetZoomTitle: "Reset zoom level 1:1", thousandsSep: " " }, global: {
            useUTC: !0,
            canvasToolsURL: "http://code.highcharts.com/maps/1.1.5/modules/canvas-tools.js", VMLRadialGradientURL: "http://code.highcharts.com/maps/1.1.5/gfx/vml-radial-gradient.png"
        }, chart: { borderColor: "#4572A7", borderRadius: 0, defaultSeriesType: "line", ignoreHiddenSeries: !0, spacing: [10, 10, 15, 10], backgroundColor: "#FFFFFF", plotBorderColor: "#C0C0C0", resetZoomButton: { theme: { zIndex: 20 }, position: { align: "right", x: -10, y: 10 } } }, title: { text: "Chart title", align: "center", margin: 15, style: { color: "#333333", fontSize: "18px" } }, subtitle: {
            text: "",
            align: "center", style: { color: "#555555" }
        }, plotOptions: {
            line: {
                allowPointSelect: !1, showCheckbox: !1, animation: { duration: 1E3 }, events: {}, lineWidth: 2, marker: { lineWidth: 0, radius: 4, lineColor: "#FFFFFF", states: { hover: { enabled: !0, lineWidthPlus: 1, radiusPlus: 2 }, select: { fillColor: "#FFFFFF", lineColor: "#000000", lineWidth: 2 } } }, point: { events: {} }, dataLabels: {
                    align: "center", formatter: function () { return this.y === null ? "" : z.numberFormat(this.y, -1) }, style: { color: "contrast", fontSize: "11px", fontWeight: "bold", textShadow: "0 0 6px contrast, 0 0 3px contrast" },
                    verticalAlign: "bottom", x: 0, y: 0, padding: 5
                }, cropThreshold: 300, pointRange: 0, states: { hover: { lineWidthPlus: 1, marker: {}, halo: { size: 10, opacity: 0.25 } }, select: { marker: {} } }, stickyTracking: !0, turboThreshold: 1E3
            }
        }, labels: { style: { position: "absolute", color: "#3E576F" } }, legend: {
            enabled: !0, align: "center", layout: "horizontal", labelFormatter: function () { return this.name }, borderColor: "#909090", borderRadius: 0, navigation: { activeColor: "#274b6d", inactiveColor: "#CCC" }, shadow: !1, itemStyle: { color: "#333333", fontSize: "12px", fontWeight: "bold" },
            itemHoverStyle: { color: "#000" }, itemHiddenStyle: { color: "#CCC" }, itemCheckboxStyle: { position: "absolute", width: "13px", height: "13px" }, symbolPadding: 5, verticalAlign: "bottom", x: 0, y: 0, title: { style: { fontWeight: "bold" } }
        }, loading: { labelStyle: { fontWeight: "bold", position: "relative", top: "45%" }, style: { position: "absolute", backgroundColor: "white", opacity: 0.5, textAlign: "center" } }, tooltip: {
            enabled: !0, animation: ca, backgroundColor: "rgba(249, 249, 249, .85)", borderWidth: 1, borderRadius: 3, dateTimeLabelFormats: {
                millisecond: "%A, %b %e, %H:%M:%S.%L",
                second: "%A, %b %e, %H:%M:%S", minute: "%A, %b %e, %H:%M", hour: "%A, %b %e, %H:%M", day: "%A, %b %e, %Y", week: "Week from %A, %b %e, %Y", month: "%B %Y", year: "%Y"
            }, footerFormat: "", headerFormat: '<span style="font-size: 10px">{point.key}</span><br/>', pointFormat: '<span style="color:{point.color}">\u25CF</span> {series.name}: <b>{point.y}</b><br/>', shadow: !0, snap: Eb ? 25 : 10, style: { color: "#333333", cursor: "default", fontSize: "12px", padding: "8px", whiteSpace: "nowrap" }
        }, credits: {
            enabled: !0, text: "Highcharts.com", href: "http://www.highcharts.com",
            position: { align: "right", x: -10, verticalAlign: "bottom", y: -5 }, style: { cursor: "pointer", color: "#909090", fontSize: "9px" }
        }
    }; var U = I.plotOptions, ta = U.line; tb(); var Pb = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]?(?:\.[0-9]+)?)\s*\)/, Qb = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/, Rb = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/, V = function (a) {
        var b = [], c, d; (function (a) {
            a && a.stops ? d = Oa(a.stops, function (a) { return V(a[1]) }) : (c = Pb.exec(a)) ? b = [D(c[1]), D(c[2]),
            D(c[3]), parseFloat(c[4], 10)] : (c = Qb.exec(a)) ? b = [D(c[1], 16), D(c[2], 16), D(c[3], 16), 1] : (c = Rb.exec(a)) && (b = [D(c[1]), D(c[2]), D(c[3]), 1])
        })(a); return {
            get: function (c) { var f; d ? (f = A(a), f.stops = [].concat(f.stops), m(d, function (a, b) { f.stops[b] = [f.stops[b][0], a.get(c)] })) : f = b && !isNaN(b[0]) ? c === "rgb" ? "rgb(" + b[0] + "," + b[1] + "," + b[2] + ")" : c === "a" ? b[3] : "rgba(" + b.join(",") + ")" : a; return f }, brighten: function (a) {
                if (d) m(d, function (b) { b.brighten(a) }); else if (qa(a) && a !== 0) {
                    var c; for (c = 0; c < 3; c++) b[c] += D(a * 255), b[c] < 0 && (b[c] =
                    0), b[c] > 255 && (b[c] = 255)
                } return this
            }, rgba: b, setOpacity: function (a) { b[3] = a; return this }, raw: a
        }
    }; P.prototype = {
        opacity: 1, textProps: "fontSize,fontWeight,fontFamily,fontStyle,color,lineHeight,width,textDecoration,textShadow".split(","), init: function (a, b) { this.element = b === "span" ? $(b) : C.createElementNS(za, b); this.renderer = a }, animate: function (a, b, c) { b = o(b, ha, !0); Va(this); if (b) { b = A(b, {}); if (c) b.complete = c; fb(this, a, b) } else this.attr(a), c && c(); return this }, colorGradient: function (a, b, c) {
            var d = this.renderer,
            e, f, g, h, i, j, k, l, n, p, v = []; a.linearGradient ? f = "linearGradient" : a.radialGradient && (f = "radialGradient"); if (f) {
                g = a[f]; h = d.gradients; j = a.stops; n = c.radialReference; Ca(g) && (a[f] = g = { x1: g[0], y1: g[1], x2: g[2], y2: g[3], gradientUnits: "userSpaceOnUse" }); f === "radialGradient" && n && !r(g.gradientUnits) && (g = A(g, { cx: n[0] - n[2] / 2 + g.cx * n[2], cy: n[1] - n[2] / 2 + g.cy * n[2], r: g.r * n[2], gradientUnits: "userSpaceOnUse" })); for (p in g) p !== "id" && v.push(p, g[p]); for (p in j) v.push(j[p]); v = v.join(","); h[v] ? a = h[v].attr("id") : (g.id = a = "highcharts-" +
                nb++, h[v] = i = d.createElement(f).attr(g).add(d.defs), i.stops = [], m(j, function (a) { a[1].indexOf("rgba") === 0 ? (e = V(a[1]), k = e.get("rgb"), l = e.get("a")) : (k = a[1], l = 1); a = d.createElement("stop").attr({ offset: a[0], "stop-color": k, "stop-opacity": l }).add(i); i.stops.push(a) })); c.setAttribute(b, "url(" + d.url + "#" + a + ")")
            }
        }, applyTextShadow: function (a) {
            var b = this.element, c, d = a.indexOf("contrast") !== -1, e = this.renderer.forExport || b.style.textShadow !== q && !wa; d && (a = a.replace(/contrast/g, this.renderer.getContrast(b.style.fill)));
            e ? d && F(b, { textShadow: a }) : (this.fakeTS = !0, this.ySetter = this.xSetter, c = [].slice.call(b.getElementsByTagName("tspan")), m(a.split(/\s?,\s?/g), function (a) {
                var d = b.firstChild, e, i, a = a.split(" "); e = a[a.length - 1]; (i = a[a.length - 2]) && m(c, function (a, c) {
                    var f; c === 0 && (a.setAttribute("x", b.getAttribute("x")), c = b.getAttribute("y"), a.setAttribute("y", c || 0), c === null && b.setAttribute("y", 0)); f = a.cloneNode(1); H(f, { "class": "highcharts-text-shadow", fill: e, stroke: e, "stroke-opacity": 1 / w(D(i), 3), "stroke-width": i, "stroke-linejoin": "round" });
                    b.insertBefore(f, d)
                })
            }))
        }, attr: function (a, b) {
            var c, d, e = this.element, f, g = this, h; typeof a === "string" && b !== q && (c = a, a = {}, a[c] = b); if (typeof a === "string") g = (this[a + "Getter"] || this._defaultGetter).call(this, a, e); else {
                for (c in a) {
                    d = a[c]; h = !1; this.symbolName && /^(x|y|width|height|r|start|end|innerR|anchorX|anchorY)/.test(c) && (f || (this.symbolAttr(a), f = !0), h = !0); if (this.rotation && (c === "x" || c === "y")) this.doTransform = !0; h || (this[c + "Setter"] || this._defaultSetter).call(this, d, c, e); this.shadows && /^(width|height|visibility|x|y|d|transform|cx|cy|r)$/.test(c) &&
                    this.updateShadows(c, d)
                } if (this.doTransform) this.updateTransform(), this.doTransform = !1
            } return g
        }, updateShadows: function (a, b) { for (var c = this.shadows, d = c.length; d--;) c[d].setAttribute(a, a === "height" ? w(b - (c[d].cutHeight || 0), 0) : a === "d" ? this.d : b) }, addClass: function (a) { var b = this.element, c = H(b, "class") || ""; c.indexOf(a) === -1 && H(b, "class", c + " " + a); return this }, symbolAttr: function (a) {
            var b = this; m("x,y,r,start,end,width,height,innerR,anchorX,anchorY".split(","), function (c) { b[c] = o(a[c], b[c]) }); b.attr({
                d: b.renderer.symbols[b.symbolName](b.x,
                b.y, b.width, b.height, b)
            })
        }, clip: function (a) { return this.attr("clip-path", a ? "url(" + this.renderer.url + "#" + a.id + ")" : O) }, crisp: function (a) { var b, c = {}, d, e = a.strokeWidth || this.strokeWidth || 0; d = x(e) % 2 / 2; a.x = fa(a.x || this.x || 0) + d; a.y = fa(a.y || this.y || 0) + d; a.width = fa((a.width || this.width || 0) - 2 * d); a.height = fa((a.height || this.height || 0) - 2 * d); a.strokeWidth = e; for (b in a) this[b] !== a[b] && (this[b] = c[b] = a[b]); return c }, css: function (a) {
            var b = this.styles, c = {}, d = this.element, e, f, g = ""; e = !b; if (a && a.color) a.fill = a.color;
            if (b) for (f in a) a[f] !== b[f] && (c[f] = a[f], e = !0); if (e) { e = this.textWidth = a && a.width && d.nodeName.toLowerCase() === "text" && D(a.width) || this.textWidth; b && (a = s(b, c)); this.styles = a; e && (ga || !ca && this.renderer.forExport) && delete a.width; if (wa && !ca) F(this.element, a); else { b = function (a, b) { return "-" + b.toLowerCase() }; for (f in a) g += f.replace(/([A-Z])/g, b) + ":" + a[f] + ";"; H(d, "style", g) } e && this.added && this.renderer.buildText(this) } return this
        }, on: function (a, b) {
            var c = this, d = c.element; Sa && a === "click" ? (d.ontouchstart = function (a) {
                c.touchEventFired =
                Ga.now(); a.preventDefault(); b.call(d, a)
            }, d.onclick = function (a) { (ya.indexOf("Android") === -1 || Ga.now() - (c.touchEventFired || 0) > 1100) && b.call(d, a) }) : d["on" + a] = b; return this
        }, setRadialReference: function (a) { this.element.radialReference = a; return this }, translate: function (a, b) { return this.attr({ translateX: a, translateY: b }) }, invert: function () { this.inverted = !0; this.updateTransform(); return this }, updateTransform: function () {
            var a = this.translateX || 0, b = this.translateY || 0, c = this.scaleX, d = this.scaleY, e = this.inverted,
            f = this.rotation, g = this.element; e && (a += this.attr("width"), b += this.attr("height")); a = ["translate(" + a + "," + b + ")"]; e ? a.push("rotate(90) scale(-1,1)") : f && a.push("rotate(" + f + " " + (g.getAttribute("x") || 0) + " " + (g.getAttribute("y") || 0) + ")"); (r(c) || r(d)) && a.push("scale(" + o(c, 1) + " " + o(d, 1) + ")"); a.length && g.setAttribute("transform", a.join(" "))
        }, toFront: function () { var a = this.element; a.parentNode.appendChild(a); return this }, align: function (a, b, c) {
            var d, e, f, g, h = {}; e = this.renderer; f = e.alignedObjects; if (a) {
                if (this.alignOptions =
                a, this.alignByTranslate = b, !c || Ba(c)) this.alignTo = d = c || "renderer", va(f, this), f.push(this), c = null
            } else a = this.alignOptions, b = this.alignByTranslate, d = this.alignTo; c = o(c, e[d], e); d = a.align; e = a.verticalAlign; f = (c.x || 0) + (a.x || 0); g = (c.y || 0) + (a.y || 0); if (d === "right" || d === "center") f += (c.width - (a.width || 0)) / { right: 1, center: 2 }[d]; h[b ? "translateX" : "x"] = x(f); if (e === "bottom" || e === "middle") g += (c.height - (a.height || 0)) / ({ bottom: 1, middle: 2 }[e] || 1); h[b ? "translateY" : "y"] = x(g); this[this.placed ? "animate" : "attr"](h); this.placed =
            !0; this.alignAttr = h; return this
        }, getBBox: function (a) {
            var b, c = this.renderer, d, e = this.rotation, f = this.element, g = this.styles, h = e * la; d = this.textStr; var i, j = f.style, k, l; d !== q && (l = ["", e || 0, g && g.fontSize, f.style.width].join(","), l = d === "" || Kb.test(d) ? "num:" + d.toString().length + l : d + l); l && !a && (b = c.cache[l]); if (!b) {
                if (f.namespaceURI === za || c.forExport) {
                    try {
                        k = this.fakeTS && function (a) { m(f.querySelectorAll(".highcharts-text-shadow"), function (b) { b.style.display = a }) }, Ha && j.textShadow ? (i = j.textShadow, j.textShadow =
                        "") : k && k(O), b = f.getBBox ? s({}, f.getBBox()) : { width: f.offsetWidth, height: f.offsetHeight }, i ? j.textShadow = i : k && k("")
                    } catch (n) { } if (!b || b.width < 0) b = { width: 0, height: 0 }
                } else b = this.htmlGetBBox(); if (c.isSVG) { a = b.width; d = b.height; if (wa && g && g.fontSize === "11px" && d.toPrecision(3) === "16.9") b.height = d = 14; if (e) b.width = Q(d * oa(h)) + Q(a * ka(h)), b.height = Q(d * ka(h)) + Q(a * oa(h)) } c.cache[l] = b
            } return b
        }, show: function (a) {
            a && this.element.namespaceURI === za ? this.element.removeAttribute("visibility") : this.attr({
                visibility: a ? "inherit" :
                "visible"
            }); return this
        }, hide: function () { return this.attr({ visibility: "hidden" }) }, fadeOut: function (a) { var b = this; b.animate({ opacity: 0 }, { duration: a || 150, complete: function () { b.attr({ y: -9999 }) } }) }, add: function (a) { var b = this.renderer, c = this.element, d; if (a) this.parentGroup = a; this.parentInverted = a && a.inverted; this.textStr !== void 0 && b.buildText(this); this.added = !0; if (!a || a.handleZ || this.zIndex) d = this.zIndexSetter(); d || (a ? a.element : b.box).appendChild(c); if (this.onAdd) this.onAdd(); return this }, safeRemoveChild: function (a) {
            var b =
            a.parentNode; b && b.removeChild(a)
        }, destroy: function () {
            var a = this, b = a.element || {}, c = a.shadows, d = a.renderer.isSVG && b.nodeName === "SPAN" && a.parentGroup, e, f; b.onclick = b.onmouseout = b.onmouseover = b.onmousemove = b.point = null; Va(a); if (a.clipPath) a.clipPath = a.clipPath.destroy(); if (a.stops) { for (f = 0; f < a.stops.length; f++) a.stops[f] = a.stops[f].destroy(); a.stops = null } a.safeRemoveChild(b); for (c && m(c, function (b) { a.safeRemoveChild(b) }) ; d && d.div && d.div.childNodes.length === 0;) b = d.parentGroup, a.safeRemoveChild(d.div), delete d.div,
            d = b; a.alignTo && va(a.renderer.alignedObjects, a); for (e in a) delete a[e]; return null
        }, shadow: function (a, b, c) {
            var d = [], e, f, g = this.element, h, i, j, k; if (a) {
                i = o(a.width, 3); j = (a.opacity || 0.15) / i; k = this.parentInverted ? "(-1,-1)" : "(" + o(a.offsetX, 1) + ", " + o(a.offsetY, 1) + ")"; for (e = 1; e <= i; e++) {
                    f = g.cloneNode(0); h = i * 2 + 1 - 2 * e; H(f, { isShadow: "true", stroke: a.color || "black", "stroke-opacity": j * e, "stroke-width": h, transform: "translate" + k, fill: O }); if (c) H(f, "height", w(H(f, "height") - h, 0)), f.cutHeight = h; b ? b.element.appendChild(f) :
                    g.parentNode.insertBefore(f, g); d.push(f)
                } this.shadows = d
            } return this
        }, xGetter: function (a) { this.element.nodeName === "circle" && (a = { x: "cx", y: "cy" }[a] || a); return this._defaultGetter(a) }, _defaultGetter: function (a) { a = o(this[a], this.element ? this.element.getAttribute(a) : null, 0); /^[\-0-9\.]+$/.test(a) && (a = parseFloat(a)); return a }, dSetter: function (a, b, c) { a && a.join && (a = a.join(" ")); /(NaN| {2}|^$)/.test(a) && (a = "M 0 0"); c.setAttribute(b, a); this[b] = a }, dashstyleSetter: function (a) {
            var b; if (a = a && a.toLowerCase()) {
                a =
                a.replace("shortdashdotdot", "3,1,1,1,1,1,").replace("shortdashdot", "3,1,1,1").replace("shortdot", "1,1,").replace("shortdash", "3,1,").replace("longdash", "8,3,").replace(/dot/g, "1,3,").replace("dash", "4,3,").replace(/,$/, "").split(","); for (b = a.length; b--;) a[b] = D(a[b]) * this["stroke-width"]; a = a.join(",").replace("NaN", "none"); this.element.setAttribute("stroke-dasharray", a)
            }
        }, alignSetter: function (a) { this.element.setAttribute("text-anchor", { left: "start", center: "middle", right: "end" }[a]) }, opacitySetter: function (a,
        b, c) { this[b] = a; c.setAttribute(b, a) }, titleSetter: function (a) { var b = this.element.getElementsByTagName("title")[0]; b || (b = C.createElementNS(za, "title"), this.element.appendChild(b)); b.textContent = String(o(a), "").replace(/<[^>]*>/g, "") }, textSetter: function (a) { if (a !== this.textStr) delete this.bBox, this.textStr = a, this.added && this.renderer.buildText(this) }, fillSetter: function (a, b, c) { typeof a === "string" ? c.setAttribute(b, a) : a && this.colorGradient(a, b, c) }, zIndexSetter: function (a, b) {
            var c = this.renderer, d = this.parentGroup,
            c = (d || c).element || c.box, e, f, g = this.element, h; e = this.added; var i; r(a) && (g.setAttribute(b, a), a = +a, this[b] === a && (e = !1), this[b] = a); if (e) { if ((a = this.zIndex) && d) d.handleZ = !0; d = c.childNodes; for (i = 0; i < d.length && !h; i++) if (e = d[i], f = H(e, "zIndex"), e !== g && (D(f) > a || !r(a) && r(f))) c.insertBefore(g, e), h = !0; h || c.appendChild(g) } return h
        }, _defaultSetter: function (a, b, c) { c.setAttribute(b, a) }
    }; P.prototype.yGetter = P.prototype.xGetter; P.prototype.translateXSetter = P.prototype.translateYSetter = P.prototype.rotationSetter = P.prototype.verticalAlignSetter =
    P.prototype.scaleXSetter = P.prototype.scaleYSetter = function (a, b) { this[b] = a; this.doTransform = !0 }; P.prototype["stroke-widthSetter"] = P.prototype.strokeSetter = function (a, b, c) { this[b] = a; if (this.stroke && this["stroke-width"]) this.strokeWidth = this["stroke-width"], P.prototype.fillSetter.call(this, this.stroke, "stroke", c), c.setAttribute("stroke-width", this["stroke-width"]), this.hasStroke = !0; else if (b === "stroke-width" && a === 0 && this.hasStroke) c.removeAttribute("stroke"), this.hasStroke = !1 }; var ia = function () {
        this.init.apply(this,
        arguments)
    }; ia.prototype = {
        Element: P, init: function (a, b, c, d, e) {
            var f = location, g, d = this.createElement("svg").attr({ version: "1.1" }).css(this.getStyle(d)); g = d.element; a.appendChild(g); a.innerHTML.indexOf("xmlns") === -1 && H(g, "xmlns", za); this.isSVG = !0; this.box = g; this.boxWrapper = d; this.alignedObjects = []; this.url = (Ha || mb) && C.getElementsByTagName("base").length ? f.href.replace(/#.*?$/, "").replace(/([\('\)])/g, "\\$1").replace(/ /g, "%20") : ""; this.createElement("desc").add().element.appendChild(C.createTextNode("Created with Highmaps 1.1.5"));
            this.defs = this.createElement("defs").add(); this.forExport = e; this.gradients = {}; this.cache = {}; this.setSize(b, c, !1); var h; if (Ha && a.getBoundingClientRect) this.subPixelFix = b = function () { F(a, { left: 0, top: 0 }); h = a.getBoundingClientRect(); F(a, { left: sa(h.left) - h.left + "px", top: sa(h.top) - h.top + "px" }) }, b(), M(K, "resize", b)
        }, getStyle: function (a) { return this.style = s({ fontFamily: '"Lucida Grande", "Lucida Sans Unicode", Arial, Helvetica, sans-serif', fontSize: "12px" }, a) }, isHidden: function () { return !this.boxWrapper.getBBox().width },
        destroy: function () { var a = this.defs; this.box = null; this.boxWrapper = this.boxWrapper.destroy(); $a(this.gradients || {}); this.gradients = null; if (a) this.defs = a.destroy(); this.subPixelFix && Y(K, "resize", this.subPixelFix); return this.alignedObjects = null }, createElement: function (a) { var b = new this.Element; b.init(this, a); return b }, draw: function () { }, buildText: function (a) {
            for (var b = a.element, c = this, d = c.forExport, e = o(a.textStr, "").toString(), f = e.indexOf("<") !== -1, g = b.childNodes, h, i, j = H(b, "x"), k = a.styles, l = a.textWidth,
            n = k && k.lineHeight, p = k && k.textShadow, v = k && k.textOverflow === "ellipsis", t = g.length, S = l && !a.added && this.box, B = function (a) { return n ? D(n) : c.fontMetrics(/(px|em)$/.test(a && a.style.fontSize) ? a.style.fontSize : k && k.fontSize || c.style.fontSize || 12, a).h }, y = function (a) { return a.replace(/&lt;/g, "<").replace(/&gt;/g, ">") }; t--;) b.removeChild(g[t]); !f && !p && !v && e.indexOf(" ") === -1 ? b.appendChild(C.createTextNode(y(e))) : (h = /<.*style="([^"]+)".*>/, i = /<.*href="(http[^"]+)".*>/, S && S.appendChild(b), e = f ? e.replace(/<(b|strong)>/g,
            '<span style="font-weight:bold">').replace(/<(i|em)>/g, '<span style="font-style:italic">').replace(/<a/g, "<span").replace(/<\/(b|strong|i|em|a)>/g, "</span>").split(/<br.*?>/g) : [e], e[e.length - 1] === "" && e.pop(), m(e, function (e, f) {
                var g, n = 0, e = e.replace(/<span/g, "|||<span").replace(/<\/span>/g, "</span>|||"); g = e.split("|||"); m(g, function (e) {
                    if (e !== "" || g.length === 1) {
                        var p = {}, t = C.createElementNS(za, "tspan"), m; h.test(e) && (m = e.match(h)[1].replace(/(;| |^)color([ :])/, "$1fill$2"), H(t, "style", m)); i.test(e) && !d &&
                        (H(t, "onclick", 'location.href="' + e.match(i)[1] + '"'), F(t, { cursor: "pointer" })); e = y(e.replace(/<(.|\n)*?>/g, "") || " "); if (e !== " ") {
                            t.appendChild(C.createTextNode(e)); if (n) p.dx = 0; else if (f && j !== null) p.x = j; H(t, p); b.appendChild(t); !n && f && (!ca && d && F(t, { display: "block" }), H(t, "dy", B(t))); if (l) {
                                for (var p = e.replace(/([^\^])-/g, "$1- ").split(" "), o = g.length > 1 || f || p.length > 1 && k.whiteSpace !== "nowrap", S, q, r, s = [], Pa = B(t), w = 1, x = a.rotation, A = e, u = A.length; (o || v) && (p.length || s.length) ;) a.rotation = 0, S = a.getBBox(!0), r =
                                S.width, !ca && c.forExport && (r = c.measureSpanWidth(t.firstChild.data, a.styles)), S = r > l, q === void 0 && (q = S), v && q ? (u /= 2, A === "" || !S && u < 0.5 ? p = [] : (S && (q = !0), A = e.substring(0, A.length + (S ? -1 : 1) * sa(u)), p = [A + "â€¦"], t.removeChild(t.firstChild))) : !S || p.length === 1 ? (p = s, s = [], p.length && (w++, t = C.createElementNS(za, "tspan"), H(t, { dy: Pa, x: j }), m && H(t, "style", m), b.appendChild(t)), r > l && (l = r)) : (t.removeChild(t.firstChild), s.unshift(p.pop())), p.length && t.appendChild(C.createTextNode(p.join(" ").replace(/- /g, "-"))); q && a.attr("title",
                                a.textStr); a.rotation = x
                            } n++
                        }
                    }
                })
            }), S && S.removeChild(b), p && a.applyTextShadow && a.applyTextShadow(p))
        }, getContrast: function (a) { a = V(a).rgba; return a[0] + a[1] + a[2] > 384 ? "#000" : "#FFF" }, button: function (a, b, c, d, e, f, g, h, i) {
            var j = this.label(a, b, c, i, null, null, null, null, "button"), k = 0, l, n, p, v, t, m, a = { x1: 0, y1: 0, x2: 0, y2: 1 }, e = A({ "stroke-width": 1, stroke: "#CCCCCC", fill: { linearGradient: a, stops: [[0, "#FEFEFE"], [1, "#F6F6F6"]] }, r: 2, padding: 5, style: { color: "black" } }, e); p = e.style; delete e.style; f = A(e, {
                stroke: "#68A", fill: {
                    linearGradient: a,
                    stops: [[0, "#FFF"], [1, "#ACF"]]
                }
            }, f); v = f.style; delete f.style; g = A(e, { stroke: "#68A", fill: { linearGradient: a, stops: [[0, "#9BD"], [1, "#CDF"]] } }, g); t = g.style; delete g.style; h = A(e, { style: { color: "#CCC" } }, h); m = h.style; delete h.style; M(j.element, wa ? "mouseover" : "mouseenter", function () { k !== 3 && j.attr(f).css(v) }); M(j.element, wa ? "mouseout" : "mouseleave", function () { k !== 3 && (l = [e, f, g][k], n = [p, v, t][k], j.attr(l).css(n)) }); j.setState = function (a) { (j.state = k = a) ? a === 2 ? j.attr(g).css(t) : a === 3 && j.attr(h).css(m) : j.attr(e).css(p) };
            return j.on("click", function () { k !== 3 && d.call(j) }).attr(e).css(s({ cursor: "default" }, p))
        }, crispLine: function (a, b) { a[1] === a[4] && (a[1] = a[4] = x(a[1]) - b % 2 / 2); a[2] === a[5] && (a[2] = a[5] = x(a[2]) + b % 2 / 2); return a }, path: function (a) { var b = { fill: O }; Ca(a) ? b.d = a : ea(a) && s(b, a); return this.createElement("path").attr(b) }, circle: function (a, b, c) { a = ea(a) ? a : { x: a, y: b, r: c }; b = this.createElement("circle"); b.xSetter = function (a) { this.element.setAttribute("cx", a) }; b.ySetter = function (a) { this.element.setAttribute("cy", a) }; return b.attr(a) },
        arc: function (a, b, c, d, e, f) { if (ea(a)) b = a.y, c = a.r, d = a.innerR, e = a.start, f = a.end, a = a.x; a = this.symbol("arc", a || 0, b || 0, c || 0, c || 0, { innerR: d || 0, start: e || 0, end: f || 0 }); a.r = c; return a }, rect: function (a, b, c, d, e, f) { var e = ea(a) ? a.r : e, g = this.createElement("rect"), a = ea(a) ? a : a === q ? {} : { x: a, y: b, width: w(c, 0), height: w(d, 0) }; if (f !== q) a.strokeWidth = f, a = g.crisp(a); if (e) a.r = e; g.rSetter = function (a) { H(this.element, { rx: a, ry: a }) }; return g.attr(a) }, setSize: function (a, b, c) {
            var d = this.alignedObjects, e = d.length; this.width = a; this.height =
            b; for (this.boxWrapper[o(c, !0) ? "animate" : "attr"]({ width: a, height: b }) ; e--;) d[e].align()
        }, g: function (a) { var b = this.createElement("g"); return r(a) ? b.attr({ "class": "highcharts-" + a }) : b }, image: function (a, b, c, d, e) { var f = { preserveAspectRatio: O }; arguments.length > 1 && s(f, { x: b, y: c, width: d, height: e }); f = this.createElement("image").attr(f); f.element.setAttributeNS ? f.element.setAttributeNS("http://www.w3.org/1999/xlink", "href", a) : f.element.setAttribute("hc-svg-href", a); return f }, symbol: function (a, b, c, d, e, f) {
            var g,
            h = this.symbols[a], h = h && h(x(b), x(c), d, e, f), i = /^url\((.*?)\)$/, j, k; if (h) g = this.path(h), s(g, { symbolName: a, x: b, y: c, width: d, height: e }), f && s(g, f); else if (i.test(a)) k = function (a, b) { a.element && (a.attr({ width: b[0], height: b[1] }), a.alignByTranslate || a.translate(x((d - b[0]) / 2), x((e - b[1]) / 2))) }, j = a.match(i)[1], a = Fb[j] || f && f.width && f.height && [f.width, f.height], g = this.image(j).attr({ x: b, y: c }), g.isImg = !0, a ? k(g, a) : (g.attr({ width: 0, height: 0 }), $("img", { onload: function () { k(g, Fb[j] = [this.width, this.height]) }, src: j }));
            return g
        }, symbols: {
            circle: function (a, b, c, d) { var e = 0.166 * c; return ["M", a + c / 2, b, "C", a + c + e, b, a + c + e, b + d, a + c / 2, b + d, "C", a - e, b + d, a - e, b, a + c / 2, b, "Z"] }, square: function (a, b, c, d) { return ["M", a, b, "L", a + c, b, a + c, b + d, a, b + d, "Z"] }, triangle: function (a, b, c, d) { return ["M", a + c / 2, b, "L", a + c, b + d, a, b + d, "Z"] }, "triangle-down": function (a, b, c, d) { return ["M", a, b, "L", a + c, b, a + c / 2, b + d, "Z"] }, diamond: function (a, b, c, d) { return ["M", a + c / 2, b, "L", a + c, b + d / 2, a + c / 2, b + d, a, b + d / 2, "Z"] }, arc: function (a, b, c, d, e) {
                var f = e.start, c = e.r || c || d, g = e.end - 0.001,
                d = e.innerR, h = e.open, i = ka(f), j = oa(f), k = ka(g), g = oa(g), e = e.end - f < lb ? 0 : 1; return ["M", a + c * i, b + c * j, "A", c, c, 0, e, 1, a + c * k, b + c * g, h ? "M" : "L", a + d * k, b + d * g, "A", d, d, 0, e, 0, a + d * i, b + d * j, h ? "" : "Z"]
            }, callout: function (a, b, c, d, e) {
                var f = N(e && e.r || 0, c, d), g = f + 6, h = e && e.anchorX, i = e && e.anchorY, e = x(e.strokeWidth || 0) % 2 / 2; a += e; b += e; e = ["M", a + f, b, "L", a + c - f, b, "C", a + c, b, a + c, b, a + c, b + f, "L", a + c, b + d - f, "C", a + c, b + d, a + c, b + d, a + c - f, b + d, "L", a + f, b + d, "C", a, b + d, a, b + d, a, b + d - f, "L", a, b + f, "C", a, b, a, b, a + f, b]; h && h > c && i > b + g && i < b + d - g ? e.splice(13, 3, "L", a +
                c, i - 6, a + c + 6, i, a + c, i + 6, a + c, b + d - f) : h && h < 0 && i > b + g && i < b + d - g ? e.splice(33, 3, "L", a, i + 6, a - 6, i, a, i - 6, a, b + f) : i && i > d && h > a + g && h < a + c - g ? e.splice(23, 3, "L", h + 6, b + d, h, b + d + 6, h - 6, b + d, a + f, b + d) : i && i < 0 && h > a + g && h < a + c - g && e.splice(3, 3, "L", h - 6, b, h, b - 6, h + 6, b, c - f, b); return e
            }
        }, clipRect: function (a, b, c, d) { var e = "highcharts-" + nb++, f = this.createElement("clipPath").attr({ id: e }).add(this.defs), a = this.rect(a, b, c, d, 0).add(f); a.id = e; a.clipPath = f; a.count = 0; return a }, text: function (a, b, c, d) {
            var e = ga || !ca && this.forExport, f = {}; if (d && !this.forExport) return this.html(a,
            b, c); f.x = Math.round(b || 0); if (c) f.y = Math.round(c); if (a || a === 0) f.text = a; a = this.createElement("text").attr(f); e && a.css({ position: "absolute" }); if (!d) a.xSetter = function (a, b, c) { var d = c.getElementsByTagName("tspan"), e, f = c.getAttribute(b), n; for (n = 0; n < d.length; n++) e = d[n], e.getAttribute(b) === f && e.setAttribute(b, a); c.setAttribute(b, a) }; return a
        }, fontMetrics: function (a, b) {
            a = a || this.style.fontSize; if (b && K.getComputedStyle) b = b.element || b, a = K.getComputedStyle(b, "").fontSize; var a = /px/.test(a) ? D(a) : /em/.test(a) ?
            parseFloat(a) * 12 : 12, c = a < 24 ? a + 3 : x(a * 1.2), d = x(c * 0.8); return { h: c, b: d, f: a }
        }, rotCorr: function (a, b, c) { var d = a; b && c && (d = w(d * ka(b * la), 4)); return { x: -a / 3 * oa(b * la), y: d } }, label: function (a, b, c, d, e, f, g, h, i) {
            function j() {
                var a, b; a = v.element.style; o = (ja === void 0 || w === void 0 || p.styles.textAlign) && r(v.textStr) && v.getBBox(); p.width = (ja || o.width || 0) + 2 * y + Pa; p.height = (w || o.height || 0) + 2 * y; D = y + n.fontMetrics(a && a.fontSize, v).b; if (E) {
                    if (!t) a = x(-B * y), b = h ? -D : 0, p.box = t = d ? n.symbol(d, a, b, p.width, p.height, G) : n.rect(a, b, p.width,
                    p.height, 0, G[Lb]), t.attr("fill", O).add(p); t.isImg || t.attr(s({ width: x(p.width), height: x(p.height) }, G)); G = null
                }
            } function k() { var a = p.styles, a = a && a.textAlign, b = Pa + y * (1 - B), c; c = h ? 0 : D; if (r(ja) && o && (a === "center" || a === "right")) b += { center: 0.5, right: 1 }[a] * (ja - o.width); if (b !== v.x || c !== v.y) v.attr("x", b), c !== q && v.attr(v.element.nodeName === "SPAN" ? "y" : "translateY", c); v.x = b; v.y = c } function l(a, b) { t ? t.attr(a, b) : G[a] = b } var n = this, p = n.g(i), v = n.text("", 0, 0, g).attr({ zIndex: 1 }), t, o, B = 0, y = 3, Pa = 0, ja, w, u, z, C = 0, G = {}, D, E; p.onAdd =
            function () { v.add(p); p.attr({ text: a || a === 0 ? a : "", x: b, y: c }); t && r(e) && p.attr({ anchorX: e, anchorY: f }) }; p.widthSetter = function (a) { ja = a }; p.heightSetter = function (a) { w = a }; p.paddingSetter = function (a) { if (r(a) && a !== y) y = p.padding = a, k() }; p.paddingLeftSetter = function (a) { r(a) && a !== Pa && (Pa = a, k()) }; p.alignSetter = function (a) { B = { left: 0, center: 0.5, right: 1 }[a] }; p.textSetter = function (a) { a !== q && v.textSetter(a); j(); k() }; p["stroke-widthSetter"] = function (a, b) { a && (E = !0); C = a % 2 / 2; l(b, a) }; p.strokeSetter = p.fillSetter = p.rSetter = function (a,
            b) { b === "fill" && a && (E = !0); l(b, a) }; p.anchorXSetter = function (a, b) { e = a; l(b, a + C - u) }; p.anchorYSetter = function (a, b) { f = a; l(b, a - z) }; p.xSetter = function (a) { p.x = a; B && (a -= B * ((ja || o.width) + y)); u = x(a); p.attr("translateX", u) }; p.ySetter = function (a) { z = p.y = x(a); p.attr("translateY", z) }; var F = p.css; return s(p, {
                css: function (a) { if (a) { var b = {}, a = A(a); m(p.textProps, function (c) { a[c] !== q && (b[c] = a[c], delete a[c]) }); v.css(b) } return F.call(p, a) }, getBBox: function () { return { width: o.width + 2 * y, height: o.height + 2 * y, x: o.x - y, y: o.y - y } },
                shadow: function (a) { t && t.shadow(a); return p }, destroy: function () { Y(p.element, "mouseenter"); Y(p.element, "mouseleave"); v && (v = v.destroy()); t && (t = t.destroy()); P.prototype.destroy.call(p); p = n = j = k = l = null }
            })
        }
    }; Na = ia; s(P.prototype, {
        htmlCss: function (a) { var b = this.element; if (b = a && b.tagName === "SPAN" && a.width) delete a.width, this.textWidth = b, this.updateTransform(); if (a && a.textOverflow === "ellipsis") a.whiteSpace = "nowrap", a.overflow = "hidden"; this.styles = s(this.styles, a); F(this.element, a); return this }, htmlGetBBox: function () {
            var a =
            this.element; if (a.nodeName === "text") a.style.position = "absolute"; return { x: a.offsetLeft, y: a.offsetTop, width: a.offsetWidth, height: a.offsetHeight }
        }, htmlUpdateTransform: function () {
            if (this.added) {
                var a = this.renderer, b = this.element, c = this.translateX || 0, d = this.translateY || 0, e = this.x || 0, f = this.y || 0, g = this.textAlign || "left", h = { left: 0, center: 0.5, right: 1 }[g], i = this.shadows, j = this.styles; F(b, { marginLeft: c, marginTop: d }); i && m(i, function (a) { F(a, { marginLeft: c + 1, marginTop: d + 1 }) }); this.inverted && m(b.childNodes, function (c) {
                    a.invertChild(c,
                    b)
                }); if (b.tagName === "SPAN") { var k = this.rotation, l, n = D(this.textWidth), p = [k, g, b.innerHTML, this.textWidth].join(","); if (p !== this.cTT) { l = a.fontMetrics(b.style.fontSize).b; r(k) && this.setSpanRotation(k, h, l); i = o(this.elemWidth, b.offsetWidth); if (i > n && /[ \-]/.test(b.textContent || b.innerText)) F(b, { width: n + "px", display: "block", whiteSpace: j && j.whiteSpace || "normal" }), i = n; this.getSpanCorrection(i, l, h, k, g) } F(b, { left: e + (this.xCorr || 0) + "px", top: f + (this.yCorr || 0) + "px" }); if (mb) l = b.offsetHeight; this.cTT = p }
            } else this.alignOnAdd =
            !0
        }, setSpanRotation: function (a, b, c) { var d = {}, e = wa ? "-ms-transform" : mb ? "-webkit-transform" : Ha ? "MozTransform" : Db ? "-o-transform" : ""; d[e] = d.transform = "rotate(" + a + "deg)"; d[e + (Ha ? "Origin" : "-origin")] = d.transformOrigin = b * 100 + "% " + c + "px"; F(this.element, d) }, getSpanCorrection: function (a, b, c) { this.xCorr = -a * c; this.yCorr = -b }
    }); s(ia.prototype, {
        html: function (a, b, c) {
            var d = this.createElement("span"), e = d.element, f = d.renderer; d.textSetter = function (a) { a !== e.innerHTML && delete this.bBox; e.innerHTML = this.textStr = a }; d.xSetter =
            d.ySetter = d.alignSetter = d.rotationSetter = function (a, b) { b === "align" && (b = "textAlign"); d[b] = a; d.htmlUpdateTransform() }; d.attr({ text: a, x: x(b), y: x(c) }).css({ position: "absolute", fontFamily: this.style.fontFamily, fontSize: this.style.fontSize }); e.style.whiteSpace = "nowrap"; d.css = d.htmlCss; if (f.isSVG) d.add = function (a) {
                var b, c = f.box.parentNode, j = []; if (this.parentGroup = a) {
                    if (b = a.div, !b) {
                        for (; a;) j.push(a), a = a.parentGroup; m(j.reverse(), function (a) {
                            var d; b = a.div = a.div || $(Fa, { className: H(a.element, "class") }, {
                                position: "absolute",
                                left: (a.translateX || 0) + "px", top: (a.translateY || 0) + "px"
                            }, b || c); d = b.style; s(a, { translateXSetter: function (b, c) { d.left = b + "px"; a[c] = b; a.doTransform = !0 }, translateYSetter: function (b, c) { d.top = b + "px"; a[c] = b; a.doTransform = !0 }, visibilitySetter: function (a, b) { d[b] = a } })
                        })
                    }
                } else b = c; b.appendChild(e); d.added = !0; d.alignOnAdd && d.htmlUpdateTransform(); return d
            }; return d
        }
    }); var Wa; if (!ca && !ga) E = {
        init: function (a, b) {
            var c = ["<", b, ' filled="f" stroked="f"'], d = ["position: ", "absolute", ";"], e = b === Fa; (b === "shape" || e) && d.push("left:0;top:0;width:1px;height:1px;");
            d.push("visibility: ", e ? "hidden" : "visible"); c.push(' style="', d.join(""), '"/>'); if (b) c = e || b === "span" || b === "img" ? c.join("") : a.prepVML(c), this.element = $(c); this.renderer = a
        }, add: function (a) { var b = this.renderer, c = this.element, d = b.box, d = a ? a.element || a : d; a && a.inverted && b.invertChild(c, d); d.appendChild(c); this.added = !0; this.alignOnAdd && !this.deferUpdateTransform && this.updateTransform(); if (this.onAdd) this.onAdd(); return this }, updateTransform: P.prototype.htmlUpdateTransform, setSpanRotation: function () {
            var a =
            this.rotation, b = ka(a * la), c = oa(a * la); F(this.element, { filter: a ? ["progid:DXImageTransform.Microsoft.Matrix(M11=", b, ", M12=", -c, ", M21=", c, ", M22=", b, ", sizingMethod='auto expand')"].join("") : O })
        }, getSpanCorrection: function (a, b, c, d, e) {
            var f = d ? ka(d * la) : 1, g = d ? oa(d * la) : 0, h = o(this.elemHeight, this.element.offsetHeight), i; this.xCorr = f < 0 && -a; this.yCorr = g < 0 && -h; i = f * g < 0; this.xCorr += g * b * (i ? 1 - c : c); this.yCorr -= f * b * (d ? i ? c : 1 - c : 1); e && e !== "left" && (this.xCorr -= a * c * (f < 0 ? -1 : 1), d && (this.yCorr -= h * c * (g < 0 ? -1 : 1)), F(this.element,
            { textAlign: e }))
        }, pathToVML: function (a) { for (var b = a.length, c = []; b--;) if (qa(a[b])) c[b] = x(a[b] * 10) - 5; else if (a[b] === "Z") c[b] = "x"; else if (c[b] = a[b], a.isArc && (a[b] === "wa" || a[b] === "at")) c[b + 5] === c[b + 7] && (c[b + 7] += a[b + 7] > a[b + 5] ? 1 : -1), c[b + 6] === c[b + 8] && (c[b + 8] += a[b + 8] > a[b + 6] ? 1 : -1); return c.join(" ") || "x" }, clip: function (a) { var b = this, c; a ? (c = a.members, va(c, b), c.push(b), b.destroyClip = function () { va(c, b) }, a = a.getCSS(b)) : (b.destroyClip && b.destroyClip(), a = { clip: bb ? "inherit" : "rect(auto)" }); return b.css(a) }, css: P.prototype.htmlCss,
        safeRemoveChild: function (a) { a.parentNode && La(a) }, destroy: function () { this.destroyClip && this.destroyClip(); return P.prototype.destroy.apply(this) }, on: function (a, b) { this.element["on" + a] = function () { var a = K.event; a.target = a.srcElement; b(a) }; return this }, cutOffPath: function (a, b) { var c, a = a.split(/[ ,]/); c = a.length; if (c === 9 || c === 11) a[c - 4] = a[c - 2] = D(a[c - 2]) - 10 * b; return a.join(" ") }, shadow: function (a, b, c) {
            var d = [], e, f = this.element, g = this.renderer, h, i = f.style, j, k = f.path, l, n, p, v; k && typeof k.value !== "string" &&
            (k = "x"); n = k; if (a) {
                p = o(a.width, 3); v = (a.opacity || 0.15) / p; for (e = 1; e <= 3; e++) { l = p * 2 + 1 - 2 * e; c && (n = this.cutOffPath(k.value, l + 0.5)); j = ['<shape isShadow="true" strokeweight="', l, '" filled="false" path="', n, '" coordsize="10 10" style="', f.style.cssText, '" />']; h = $(g.prepVML(j), null, { left: D(i.left) + o(a.offsetX, 1), top: D(i.top) + o(a.offsetY, 1) }); if (c) h.cutOff = l + 1; j = ['<stroke color="', a.color || "black", '" opacity="', v * e, '"/>']; $(g.prepVML(j), null, null, h); b ? b.element.appendChild(h) : f.parentNode.insertBefore(h, f); d.push(h) } this.shadows =
                d
            } return this
        }, updateShadows: T, setAttr: function (a, b) { bb ? this.element[a] = b : this.element.setAttribute(a, b) }, classSetter: function (a) { this.element.className = a }, dashstyleSetter: function (a, b, c) { (c.getElementsByTagName("stroke")[0] || $(this.renderer.prepVML(["<stroke/>"]), null, null, c))[b] = a || "solid"; this[b] = a }, dSetter: function (a, b, c) {
            var d = this.shadows, a = a || []; this.d = a.join && a.join(" "); c.path = a = this.pathToVML(a); if (d) for (c = d.length; c--;) d[c].path = d[c].cutOff ? this.cutOffPath(a, d[c].cutOff) : a; this.setAttr(b,
            a)
        }, fillSetter: function (a, b, c) { var d = c.nodeName; if (d === "SPAN") c.style.color = a; else if (d !== "IMG") c.filled = a !== O, this.setAttr("fillcolor", this.renderer.color(a, c, b, this)) }, opacitySetter: T, rotationSetter: function (a, b, c) { c = c.style; this[b] = c[b] = a; c.left = -x(oa(a * la) + 1) + "px"; c.top = x(ka(a * la)) + "px" }, strokeSetter: function (a, b, c) { this.setAttr("strokecolor", this.renderer.color(a, c, b)) }, "stroke-widthSetter": function (a, b, c) { c.stroked = !!a; this[b] = a; qa(a) && (a += "px"); this.setAttr("strokeweight", a) }, titleSetter: function (a,
        b) { this.setAttr(b, a) }, visibilitySetter: function (a, b, c) { a === "inherit" && (a = "visible"); this.shadows && m(this.shadows, function (c) { c.style[b] = a }); c.nodeName === "DIV" && (a = a === "hidden" ? "-999em" : 0, bb || (c.style[b] = a ? "visible" : "hidden"), b = "top"); c.style[b] = a }, xSetter: function (a, b, c) { this[b] = a; b === "x" ? b = "left" : b === "y" && (b = "top"); this.updateClipping ? (this[b] = a, this.updateClipping()) : c.style[b] = a }, zIndexSetter: function (a, b, c) { c.style[b] = a }
    }, z.VMLElement = E = aa(P, E), E.prototype.ySetter = E.prototype.widthSetter = E.prototype.heightSetter =
    E.prototype.xSetter, E = {
        Element: E, isIE8: ya.indexOf("MSIE 8.0") > -1, init: function (a, b, c, d) {
            var e; this.alignedObjects = []; d = this.createElement(Fa).css(s(this.getStyle(d), { position: "relative" })); e = d.element; a.appendChild(d.element); this.isVML = !0; this.box = e; this.boxWrapper = d; this.cache = {}; this.setSize(b, c, !1); if (!C.namespaces.hcv) {
                C.namespaces.add("hcv", "urn:schemas-microsoft-com:vml"); try { C.createStyleSheet().cssText = "hcv\\:fill, hcv\\:path, hcv\\:shape, hcv\\:stroke{ behavior:url(#default#VML); display: inline-block; } " } catch (f) {
                    C.styleSheets[0].cssText +=
                    "hcv\\:fill, hcv\\:path, hcv\\:shape, hcv\\:stroke{ behavior:url(#default#VML); display: inline-block; } "
                }
            }
        }, isHidden: function () { return !this.box.offsetWidth }, clipRect: function (a, b, c, d) {
            var e = this.createElement(), f = ea(a); return s(e, {
                members: [], count: 0, left: (f ? a.x : a) + 1, top: (f ? a.y : b) + 1, width: (f ? a.width : c) - 1, height: (f ? a.height : d) - 1, getCSS: function (a) {
                    var b = a.element, c = b.nodeName, a = a.inverted, d = this.top - (c === "shape" ? b.offsetTop : 0), e = this.left, b = e + this.width, f = d + this.height, d = {
                        clip: "rect(" + x(a ? e : d) + "px," +
                        x(a ? f : b) + "px," + x(a ? b : f) + "px," + x(a ? d : e) + "px)"
                    }; !a && bb && c === "DIV" && s(d, { width: b + "px", height: f + "px" }); return d
                }, updateClipping: function () { m(e.members, function (a) { a.element && a.css(e.getCSS(a)) }) }
            })
        }, color: function (a, b, c, d) {
            var e = this, f, g = /^rgba/, h, i, j = O; a && a.linearGradient ? i = "gradient" : a && a.radialGradient && (i = "pattern"); if (i) {
                var k, l, n = a.linearGradient || a.radialGradient, p, v, t, o, B, y = "", a = a.stops, q, ja = [], r = function () {
                    h = ['<fill colors="' + ja.join(",") + '" opacity="', t, '" o:opacity2="', v, '" type="', i, '" ',
                    y, 'focus="100%" method="any" />']; $(e.prepVML(h), null, null, b)
                }; p = a[0]; q = a[a.length - 1]; p[0] > 0 && a.unshift([0, p[1]]); q[0] < 1 && a.push([1, q[1]]); m(a, function (a, b) { g.test(a[1]) ? (f = V(a[1]), k = f.get("rgb"), l = f.get("a")) : (k = a[1], l = 1); ja.push(a[0] * 100 + "% " + k); b ? (t = l, o = k) : (v = l, B = k) }); if (c === "fill") if (i === "gradient") c = n.x1 || n[0] || 0, a = n.y1 || n[1] || 0, p = n.x2 || n[2] || 0, n = n.y2 || n[3] || 0, y = 'angle="' + (90 - J.atan((n - a) / (p - c)) * 180 / lb) + '"', r(); else {
                    var j = n.r, s = j * 2, w = j * 2, x = n.cx, A = n.cy, u = b.radialReference, z, j = function () {
                        u && (z =
                        d.getBBox(), x += (u[0] - z.x) / z.width - 0.5, A += (u[1] - z.y) / z.height - 0.5, s *= u[2] / z.width, w *= u[2] / z.height); y = 'src="' + I.global.VMLRadialGradientURL + '" size="' + s + "," + w + '" origin="0.5,0.5" position="' + x + "," + A + '" color2="' + B + '" '; r()
                    }; d.added ? j() : d.onAdd = j; j = o
                } else j = k
            } else if (g.test(a) && b.tagName !== "IMG") f = V(a), h = ["<", c, ' opacity="', f.get("a"), '"/>'], $(this.prepVML(h), null, null, b), j = f.get("rgb"); else { j = b.getElementsByTagName(c); if (j.length) j[0].opacity = 1, j[0].type = "solid"; j = a } return j
        }, prepVML: function (a) {
            var b =
            this.isIE8, a = a.join(""); b ? (a = a.replace("/>", ' xmlns="urn:schemas-microsoft-com:vml" />'), a = a.indexOf('style="') === -1 ? a.replace("/>", ' style="display:inline-block;behavior:url(#default#VML);" />') : a.replace('style="', 'style="display:inline-block;behavior:url(#default#VML);')) : a = a.replace("<", "<hcv:"); return a
        }, text: ia.prototype.html, path: function (a) { var b = { coordsize: "10 10" }; Ca(a) ? b.d = a : ea(a) && s(b, a); return this.createElement("shape").attr(b) }, circle: function (a, b, c) {
            var d = this.symbol("circle"); if (ea(a)) c =
            a.r, b = a.y, a = a.x; d.isCircle = !0; d.r = c; return d.attr({ x: a, y: b })
        }, g: function (a) { var b; a && (b = { className: "highcharts-" + a, "class": "highcharts-" + a }); return this.createElement(Fa).attr(b) }, image: function (a, b, c, d, e) { var f = this.createElement("img").attr({ src: a }); arguments.length > 1 && f.attr({ x: b, y: c, width: d, height: e }); return f }, createElement: function (a) { return a === "rect" ? this.symbol(a) : ia.prototype.createElement.call(this, a) }, invertChild: function (a, b) {
            var c = this, d = b.style, e = a.tagName === "IMG" && a.style; F(a, {
                flip: "x",
                left: D(d.width) - (e ? D(e.top) : 1), top: D(d.height) - (e ? D(e.left) : 1), rotation: -90
            }); m(a.childNodes, function (b) { c.invertChild(b, a) })
        }, symbols: {
            arc: function (a, b, c, d, e) { var f = e.start, g = e.end, h = e.r || c || d, c = e.innerR, d = ka(f), i = oa(f), j = ka(g), k = oa(g); if (g - f === 0) return ["x"]; f = ["wa", a - h, b - h, a + h, b + h, a + h * d, b + h * i, a + h * j, b + h * k]; e.open && !c && f.push("e", "M", a, b); f.push("at", a - c, b - c, a + c, b + c, a + c * j, b + c * k, a + c * d, b + c * i, "x", "e"); f.isArc = !0; return f }, circle: function (a, b, c, d, e) {
                e && (c = d = 2 * e.r); e && e.isCircle && (a -= c / 2, b -= d / 2); return ["wa",
                a, b, a + c, b + d, a + c, b + d / 2, a + c, b + d / 2, "e"]
            }, rect: function (a, b, c, d, e) { return ia.prototype.symbols[!r(e) || !e.r ? "square" : "callout"].call(0, a, b, c, d, e) }
        }
    }, z.VMLRenderer = Wa = function () { this.init.apply(this, arguments) }, Wa.prototype = A(ia.prototype, E), Na = Wa; ia.prototype.measureSpanWidth = function (a, b) { var c = C.createElement("span"), d; d = C.createTextNode(a); c.appendChild(d); F(c, b); this.box.appendChild(c); d = c.offsetWidth; La(c); return d }; var Gb; if (ga) z.CanVGRenderer = E = function () { za = "http://www.w3.org/1999/xhtml" }, E.prototype.symbols =
    {}, Gb = function () { function a() { var a = b.length, d; for (d = 0; d < a; d++) b[d](); b = [] } var b = []; return { push: function (c, d) { b.length === 0 && Mb(d, a); b.push(c) } } }(), Na = E; Ma.prototype = {
        addLabel: function () {
            var a = this.axis, b = a.options, c = a.chart, d = a.categories, e = a.names, f = this.pos, g = b.labels, h = a.tickPositions, i = f === h[0], j = f === h[h.length - 1], e = d ? o(d[f], e[f], f) : f, d = this.label, h = h.info, k; a.isDatetimeAxis && h && (k = b.dateTimeLabelFormats[h.higherRanks[f] || h.unitName]); this.isFirst = i; this.isLast = j; b = a.labelFormatter.call({
                axis: a,
                chart: c, isFirst: i, isLast: j, dateTimeLabelFormat: k, value: a.isLog ? xa(na(e)) : e
            }); r(d) ? d && d.attr({ text: b }) : (this.labelLength = (this.label = d = r(b) && g.enabled ? c.renderer.text(b, 0, 0, g.useHTML).css(A(g.style)).add(a.labelGroup) : null) && d.getBBox().width, this.rotation = 0)
        }, getLabelSize: function () { return this.label ? this.label.getBBox()[this.axis.horiz ? "height" : "width"] : 0 }, handleOverflow: function (a) {
            var b = this.axis, c = a.x, d = b.chart.chartWidth, e = b.chart.spacing, f = o(b.labelLeft, e[3]), e = o(b.labelRight, d - e[1]), g = this.label,
            h = this.rotation, i = { left: 0, center: 0.5, right: 1 }[b.labelAlign], j = g.getBBox().width, k = b.slotWidth, l; if (h) h < 0 && c - i * j < f ? l = x(c / ka(h * la) - f) : h > 0 && c + i * j > e && (l = x((d - c) / ka(h * la))); else { d = c - i * j; c += i * j; if (d < f) k -= f - d, a.x = f, g.attr({ align: "left" }); else if (c > e) k -= c - e, a.x = e, g.attr({ align: "right" }); if (j > k || b.autoRotation && g.styles.width) l = k } l && g.css({ width: l, textOverflow: "ellipsis" })
        }, getPosition: function (a, b, c, d) {
            var e = this.axis, f = e.chart, g = d && f.oldChartHeight || f.chartHeight; return {
                x: a ? e.translate(b + c, null, null, d) +
                e.transB : e.left + e.offset + (e.opposite ? (d && f.oldChartWidth || f.chartWidth) - e.right - e.left : 0), y: a ? g - e.bottom + e.offset - (e.opposite ? e.height : 0) : g - e.translate(b + c, null, null, d) - e.transB
            }
        }, getLabelPosition: function (a, b, c, d, e, f, g, h) { var i = this.axis, j = i.transA, k = i.reversed, l = i.staggerLines, n = i.tickRotCorr || { x: 0, y: 0 }, c = o(e.y, n.y + (i.side === 2 ? 8 : -(c.getBBox().height / 2))), a = a + e.x + n.x - (f && d ? f * j * (k ? -1 : 1) : 0), b = b + c - (f && !d ? f * j * (k ? 1 : -1) : 0); l && (b += g / (h || 1) % l * (i.labelOffset / l)); return { x: a, y: x(b) } }, getMarkPath: function (a,
        b, c, d, e, f) { return f.crispLine(["M", a, b, "L", a + (e ? 0 : -c), b + (e ? c : 0)], d) }, render: function (a, b, c) {
            var d = this.axis, e = d.options, f = d.chart.renderer, g = d.horiz, h = this.type, i = this.label, j = this.pos, k = e.labels, l = this.gridLine, n = h ? h + "Grid" : "grid", p = h ? h + "Tick" : "tick", v = e[n + "LineWidth"], t = e[n + "LineColor"], m = e[n + "LineDashStyle"], B = e[p + "Length"], n = e[p + "Width"] || 0, y = e[p + "Color"], r = e[p + "Position"], p = this.mark, ja = k.step, s = !0, w = d.tickmarkOffset, x = this.getPosition(g, j, w, b), u = x.x, x = x.y, A = g && u === d.pos + d.len || !g && x === d.pos ?
            -1 : 1, c = o(c, 1); this.isActive = !0; if (v) { j = d.getPlotLinePath(j + w, v * A, b, !0); if (l === q) { l = { stroke: t, "stroke-width": v }; if (m) l.dashstyle = m; if (!h) l.zIndex = 1; if (b) l.opacity = 0; this.gridLine = l = v ? f.path(j).attr(l).add(d.gridGroup) : null } if (!b && l && j) l[this.isNew ? "attr" : "animate"]({ d: j, opacity: c }) } if (n && B) r === "inside" && (B = -B), d.opposite && (B = -B), h = this.getMarkPath(u, x, B, n * A, g, f), p ? p.animate({ d: h, opacity: c }) : this.mark = f.path(h).attr({ stroke: y, "stroke-width": n, opacity: c }).add(d.axisGroup); if (i && !isNaN(u)) i.xy = x = this.getLabelPosition(u,
            x, i, g, k, w, a, ja), this.isFirst && !this.isLast && !o(e.showFirstLabel, 1) || this.isLast && !this.isFirst && !o(e.showLastLabel, 1) ? s = !1 : g && !d.isRadial && !k.step && !k.rotation && !b && c !== 0 && this.handleOverflow(x), ja && a % ja && (s = !1), s && !isNaN(x.y) ? (x.opacity = c, i[this.isNew ? "attr" : "animate"](x), this.isNew = !1) : i.attr("y", -9999)
        }, destroy: function () { $a(this, this.axis) }
    }; var X = z.Axis = function () { this.init.apply(this, arguments) }; X.prototype = {
        defaultOptions: {
            dateTimeLabelFormats: {
                millisecond: "%H:%M:%S.%L", second: "%H:%M:%S", minute: "%H:%M",
                hour: "%H:%M", day: "%e. %b", week: "%e. %b", month: "%b '%y", year: "%Y"
            }, endOnTick: !1, gridLineColor: "#D8D8D8", labels: { enabled: !0, style: { color: "#606060", cursor: "default", fontSize: "11px" }, x: 0, y: 15 }, lineColor: "#C0D0E0", lineWidth: 1, minPadding: 0.01, maxPadding: 0.01, minorGridLineColor: "#E0E0E0", minorGridLineWidth: 1, minorTickColor: "#A0A0A0", minorTickLength: 2, minorTickPosition: "outside", startOfWeek: 1, startOnTick: !1, tickColor: "#C0D0E0", tickLength: 10, tickmarkPlacement: "between", tickPixelInterval: 100, tickPosition: "outside",
            tickWidth: 1, title: { align: "middle", style: { color: "#707070" } }, type: "linear"
        }, defaultYAxisOptions: { endOnTick: !0, gridLineWidth: 1, tickPixelInterval: 72, showLastLabel: !0, labels: { x: -8, y: 3 }, lineWidth: 0, maxPadding: 0.05, minPadding: 0.05, startOnTick: !0, tickWidth: 0, title: { rotation: 270, text: "Values" }, stackLabels: { enabled: !1, formatter: function () { return z.numberFormat(this.total, -1) }, style: A(U.line.dataLabels.style, { color: "#000000" }) } }, defaultLeftAxisOptions: { labels: { x: -15, y: null }, title: { rotation: 270 } }, defaultRightAxisOptions: {
            labels: {
                x: 15,
                y: null
            }, title: { rotation: 90 }
        }, defaultBottomAxisOptions: { labels: { autoRotation: [-45], x: 0, y: null }, title: { rotation: 0 } }, defaultTopAxisOptions: { labels: { autoRotation: [-45], x: 0, y: -15 }, title: { rotation: 0 } }, init: function (a, b) {
            var c = b.isX; this.horiz = a.inverted ? !c : c; this.coll = (this.isXAxis = c) ? "xAxis" : "yAxis"; this.opposite = b.opposite; this.side = b.side || (this.horiz ? this.opposite ? 0 : 2 : this.opposite ? 1 : 3); this.setOptions(b); var d = this.options, e = d.type; this.labelFormatter = d.labels.formatter || this.defaultLabelFormatter;
            this.userOptions = b; this.minPixelPadding = 0; this.chart = a; this.reversed = d.reversed; this.zoomEnabled = d.zoomEnabled !== !1; this.categories = d.categories || e === "category"; this.names = this.names || []; this.isLog = e === "logarithmic"; this.isDatetimeAxis = e === "datetime"; this.isLinked = r(d.linkedTo); this.ticks = {}; this.labelEdge = []; this.minorTicks = {}; this.plotLinesAndBands = []; this.alternateBands = {}; this.len = 0; this.minRange = this.userMinRange = d.minRange || d.maxZoom; this.range = d.range; this.offset = d.offset || 0; this.stacks = {};
            this.oldStacks = {}; this.min = this.max = null; this.crosshair = o(d.crosshair, ra(a.options.tooltip.crosshairs)[c ? 0 : 1], !1); var f, d = this.options.events; Ua(this, a.axes) === -1 && (c && !this.isColorAxis ? a.axes.splice(a.xAxis.length, 0, this) : a.axes.push(this), a[this.coll].push(this)); this.series = this.series || []; if (a.inverted && c && this.reversed === q) this.reversed = !0; this.removePlotLine = this.removePlotBand = this.removePlotBandOrLine; for (f in d) M(this, f, d[f]); if (this.isLog) this.val2lin = Qa, this.lin2val = na
        }, setOptions: function (a) {
            this.options =
            A(this.defaultOptions, this.isXAxis ? {} : this.defaultYAxisOptions, [this.defaultTopAxisOptions, this.defaultRightAxisOptions, this.defaultBottomAxisOptions, this.defaultLeftAxisOptions][this.side], A(I[this.coll], a))
        }, defaultLabelFormatter: function () {
            var a = this.axis, b = this.value, c = a.categories, d = this.dateTimeLabelFormat, e = I.lang.numericSymbols, f = e && e.length, g, h = a.options.labels.format, a = a.isLog ? b : a.tickInterval; if (h) g = Ja(h, this); else if (c) g = b; else if (d) g = Ka(d, b); else if (f && a >= 1E3) for (; f-- && g === q;) c = Math.pow(1E3,
            f + 1), a >= c && e[f] !== null && (g = z.numberFormat(b / c, -1) + e[f]); g === q && (g = Q(b) >= 1E4 ? z.numberFormat(b, 0) : z.numberFormat(b, -1, q, "")); return g
        }, getSeriesExtremes: function () {
            var a = this, b = a.chart; a.hasVisibleSeries = !1; a.dataMin = a.dataMax = a.ignoreMinPadding = a.ignoreMaxPadding = null; a.buildStacks && a.buildStacks(); m(a.series, function (c) {
                if (c.visible || !b.options.chart.ignoreHiddenSeries) {
                    var d; d = c.options.threshold; var e; a.hasVisibleSeries = !0; a.isLog && d <= 0 && (d = null); if (a.isXAxis) {
                        if (d = c.xData, d.length) a.dataMin = N(o(a.dataMin,
                        d[0]), Ra(d)), a.dataMax = w(o(a.dataMax, d[0]), Ea(d))
                    } else { c.getExtremes(); e = c.dataMax; c = c.dataMin; if (r(c) && r(e)) a.dataMin = N(o(a.dataMin, c), c), a.dataMax = w(o(a.dataMax, e), e); if (r(d)) if (a.dataMin >= d) a.dataMin = d, a.ignoreMinPadding = !0; else if (a.dataMax < d) a.dataMax = d, a.ignoreMaxPadding = !0 }
                }
            })
        }, translate: function (a, b, c, d, e, f) {
            var g = 1, h = 0, i = d ? this.oldTransA : this.transA, d = d ? this.oldMin : this.min, j = this.minPixelPadding, e = (this.doPostTranslate || this.isLog && e) && this.lin2val; if (!i) i = this.transA; if (c) g *= -1, h = this.len;
            this.reversed && (g *= -1, h -= g * (this.sector || this.len)); b ? (a = a * g + h, a -= j, a = a / i + d, e && (a = this.lin2val(a))) : (e && (a = this.val2lin(a)), f === "between" && (f = 0.5), a = g * (a - d) * i + h + g * j + (qa(f) ? i * f * this.pointRange : 0)); return a
        }, toPixels: function (a, b) { return this.translate(a, !1, !this.horiz, null, !0) + (b ? 0 : this.pos) }, toValue: function (a, b) { return this.translate(a - (b ? 0 : this.pos), !0, !this.horiz, null, !0) }, getPlotLinePath: function (a, b, c, d, e) {
            var f = this.chart, g = this.left, h = this.top, i, j, k = c && f.oldChartHeight || f.chartHeight, l = c &&
            f.oldChartWidth || f.chartWidth, n; i = this.transB; var p = function (a, b, c) { if (a < b || a > c) d ? a = N(w(b, a), c) : n = !0; return a }, e = o(e, this.translate(a, null, null, c)), a = c = x(e + i); i = j = x(k - e - i); isNaN(e) ? n = !0 : this.horiz ? (i = h, j = k - this.bottom, a = c = p(a, g, g + this.width)) : (a = g, c = l - this.right, i = j = p(i, h, h + this.height)); return n && !d ? null : f.renderer.crispLine(["M", a, i, "L", c, j], b || 1)
        }, getLinearTickPositions: function (a, b, c) {
            var d, e = xa(fa(b / a) * a), f = xa(sa(c / a) * a), g = []; if (b === c && qa(b)) return [b]; for (b = e; b <= f;) {
                g.push(b); b = xa(b + a); if (b ===
                d) break; d = b
            } return g
        }, getMinorTickPositions: function () { var a = this.options, b = this.tickPositions, c = this.minorTickInterval, d = [], e, f = this.min; e = this.max; var g = e - f; if (g && g / c < this.len / 3) if (this.isLog) { a = b.length; for (e = 1; e < a; e++) d = d.concat(this.getLogTickPositions(c, b[e - 1], b[e], !0)) } else if (this.isDatetimeAxis && a.minorTickInterval === "auto") d = d.concat(this.getTimeTicks(this.normalizeTimeTickInterval(c), f, e, a.startOfWeek)); else for (b = f + (b[0] - f) % c; b <= e; b += c) d.push(b); this.trimTicks(d); return d }, adjustForMinRange: function () {
            var a =
            this.options, b = this.min, c = this.max, d, e = this.dataMax - this.dataMin >= this.minRange, f, g, h, i, j; if (this.isXAxis && this.minRange === q && !this.isLog) r(a.min) || r(a.max) ? this.minRange = null : (m(this.series, function (a) { i = a.xData; for (g = j = a.xIncrement ? 1 : i.length - 1; g > 0; g--) if (h = i[g] - i[g - 1], f === q || h < f) f = h }), this.minRange = N(f * 5, this.dataMax - this.dataMin)); if (c - b < this.minRange) {
                var k = this.minRange; d = (k - c + b) / 2; d = [b - d, o(a.min, b - d)]; if (e) d[2] = this.dataMin; b = Ea(d); c = [b + k, o(a.max, b + k)]; if (e) c[2] = this.dataMax; c = Ra(c); c - b < k &&
                (d[0] = c - k, d[1] = o(a.min, c - k), b = Ea(d))
            } this.min = b; this.max = c
        }, setAxisTranslation: function (a) {
            var b = this, c = b.max - b.min, d = b.axisPointRange || 0, e, f = 0, g = 0, h = b.linkedParent, i = !!b.categories, j = b.transA, k = b.isXAxis; if (k || i || d) if (h ? (f = h.minPointOffset, g = h.pointRangePadding) : m(b.series, function (a) { var h = i ? 1 : k ? a.pointRange : b.axisPointRange || 0, j = a.options.pointPlacement, v = a.closestPointRange; h > c && (h = 0); d = w(d, h); b.single || (f = w(f, Ba(j) ? 0 : h / 2), g = w(g, j === "on" ? 0 : h)); !a.noSharedTooltip && r(v) && (e = r(e) ? N(e, v) : v) }), h = b.ordinalSlope &&
            e ? b.ordinalSlope / e : 1, b.minPointOffset = f *= h, b.pointRangePadding = g *= h, b.pointRange = N(d, c), k) b.closestPointRange = e; if (a) b.oldTransA = j; b.translationSlope = b.transA = j = b.len / (c + g || 1); b.transB = b.horiz ? b.left : b.bottom; b.minPixelPadding = j * f
        }, setTickInterval: function (a) {
            var b = this, c = b.chart, d = b.options, e = b.isLog, f = b.isDatetimeAxis, g = b.isXAxis, h = b.isLinked, i = d.maxPadding, j = d.minPadding, k = d.tickInterval, l = d.tickPixelInterval, n = b.categories; !f && !n && !h && this.getTickAmount(); h ? (b.linkedParent = c[b.coll][d.linkedTo],
            c = b.linkedParent.getExtremes(), b.min = o(c.min, c.dataMin), b.max = o(c.max, c.dataMax), d.type !== b.linkedParent.options.type && ba(11, 1)) : (b.min = o(b.userMin, d.min, b.dataMin), b.max = o(b.userMax, d.max, b.dataMax)); if (e) !a && N(b.min, o(b.dataMin, b.min)) <= 0 && ba(10, 1), b.min = xa(Qa(b.min)), b.max = xa(Qa(b.max)); if (b.range && r(b.max)) b.userMin = b.min = w(b.min, b.max - b.range), b.userMax = b.max, b.range = null; b.beforePadding && b.beforePadding(); b.adjustForMinRange(); if (!n && !b.axisPointRange && !b.usePercentage && !h && r(b.min) && r(b.max) &&
            (c = b.max - b.min)) { if (!r(d.min) && !r(b.userMin) && j && (b.dataMin < 0 || !b.ignoreMinPadding)) b.min -= c * j; if (!r(d.max) && !r(b.userMax) && i && (b.dataMax > 0 || !b.ignoreMaxPadding)) b.max += c * i } if (qa(d.floor)) b.min = w(b.min, d.floor); if (qa(d.ceiling)) b.max = N(b.max, d.ceiling); b.tickInterval = b.min === b.max || b.min === void 0 || b.max === void 0 ? 1 : h && !k && l === b.linkedParent.options.tickPixelInterval ? b.linkedParent.tickInterval : o(k, this.tickAmount ? (b.max - b.min) / w(this.tickAmount - 1, 1) : void 0, n ? 1 : (b.max - b.min) * l / w(b.len, l)); g && !a && m(b.series,
            function (a) { a.processData(b.min !== b.oldMin || b.max !== b.oldMax) }); b.setAxisTranslation(!0); b.beforeSetTickPositions && b.beforeSetTickPositions(); if (b.postProcessTickInterval) b.tickInterval = b.postProcessTickInterval(b.tickInterval); if (b.pointRange) b.tickInterval = w(b.pointRange, b.tickInterval); a = o(d.minTickInterval, b.isDatetimeAxis && b.closestPointRange); if (!k && b.tickInterval < a) b.tickInterval = a; if (!f && !e && !k) b.tickInterval = sb(b.tickInterval, null, J.pow(10, fa(J.log(b.tickInterval) / J.LN10)), o(d.allowDecimals,
            !(b.tickInterval > 0.5 && b.tickInterval < 5 && b.max > 1E3 && b.max < 9999)), !!this.tickAmount); if (!this.tickAmount && this.len) b.tickInterval = b.unsquish(); this.setTickPositions()
        }, setTickPositions: function () {
            var a = this.options, b, c = a.tickPositions, d = a.tickPositioner, e = a.startOnTick, f = a.endOnTick, g; this.tickmarkOffset = this.categories && a.tickmarkPlacement === "between" && this.tickInterval === 1 ? 0.5 : 0; this.minorTickInterval = a.minorTickInterval === "auto" && this.tickInterval ? this.tickInterval / 5 : a.minorTickInterval; this.tickPositions =
            b = a.tickPositions && a.tickPositions.slice(); if (!b && (this.tickPositions = b = this.isDatetimeAxis ? this.getTimeTicks(this.normalizeTimeTickInterval(this.tickInterval, a.units), this.min, this.max, a.startOfWeek, this.ordinalPositions, this.closestPointRange, !0) : this.isLog ? this.getLogTickPositions(this.tickInterval, this.min, this.max) : this.getLinearTickPositions(this.tickInterval, this.min, this.max), d && (d = d.apply(this, [this.min, this.max])))) this.tickPositions = b = d; if (!this.isLinked) this.trimTicks(b, e, f), this.min ===
            this.max && r(this.min) && !this.tickAmount && (g = !0, this.min -= 0.5, this.max += 0.5), this.single = g, !c && !d && this.adjustTickAmount()
        }, trimTicks: function (a, b, c) { var d = a[0], e = a[a.length - 1], f = this.minPointOffset || 0; b ? this.min = d : this.min - f > d && a.shift(); c ? this.max = e : this.max + f < e && a.pop(); a.length === 0 && r(d) && a.push((e + d) / 2) }, getTickAmount: function () {
            var a = {}, b, c = this.options, d = c.tickAmount, e = c.tickPixelInterval; !r(c.tickInterval) && this.len < e && !this.isRadial && !this.isLog && c.startOnTick && c.endOnTick && (d = 2); !d && this.chart.options.chart.alignTicks !==
            !1 && c.alignTicks !== !1 && (m(this.chart[this.coll], function (c) { var d = c.options, c = c.horiz, d = [c ? d.left : d.top, c ? d.width : d.height, d.pane].join(","); a[d] ? b = !0 : a[d] = 1 }), b && (d = sa(this.len / e) + 1)); if (d < 4) this.finalTickAmt = d, d = 5; this.tickAmount = d
        }, adjustTickAmount: function () {
            var a = this.tickInterval, b = this.tickPositions, c = this.tickAmount, d = this.finalTickAmt, e = b && b.length; if (e < c) { for (; b.length < c;) b.push(xa(b[b.length - 1] + a)); this.transA *= (e - 1) / (c - 1); this.max = b[b.length - 1] } else e > c && (this.tickInterval *= 2, this.setTickPositions());
            if (r(d)) { for (a = c = b.length; a--;) (d === 3 && a % 2 === 1 || d <= 2 && a > 0 && a < c - 1) && b.splice(a, 1); this.finalTickAmt = q }
        }, setScale: function () {
            var a = this.stacks, b, c, d, e; this.oldMin = this.min; this.oldMax = this.max; this.oldAxisLength = this.len; this.setAxisSize(); e = this.len !== this.oldAxisLength; m(this.series, function (a) { if (a.isDirtyData || a.isDirty || a.xAxis.isDirty) d = !0 }); if (e || d || this.isLinked || this.forceRedraw || this.userMin !== this.oldUserMin || this.userMax !== this.oldUserMax) {
                if (!this.isXAxis) for (b in a) for (c in a[b]) a[b][c].total =
                null, a[b][c].cum = 0; this.forceRedraw = !1; this.getSeriesExtremes(); this.setTickInterval(); this.oldUserMin = this.userMin; this.oldUserMax = this.userMax; if (!this.isDirty) this.isDirty = e || this.min !== this.oldMin || this.max !== this.oldMax
            } else if (!this.isXAxis) { if (this.oldStacks) a = this.stacks = this.oldStacks; for (b in a) for (c in a[b]) a[b][c].cum = a[b][c].total }
        }, setExtremes: function (a, b, c, d, e) {
            var f = this, g = f.chart, c = o(c, !0); m(f.series, function (a) { delete a.kdTree }); e = s(e, { min: a, max: b }); L(f, "setExtremes", e, function () {
                f.userMin =
                a; f.userMax = b; f.eventArgs = e; f.isDirtyExtremes = !0; c && g.redraw(d)
            })
        }, zoom: function (a, b) { var c = this.dataMin, d = this.dataMax, e = this.options; this.allowZoomOutside || (r(c) && a <= N(c, o(e.min, c)) && (a = q), r(d) && b >= w(d, o(e.max, d)) && (b = q)); this.displayBtn = a !== q || b !== q; this.setExtremes(a, b, !1, q, { trigger: "zoom" }); return !0 }, setAxisSize: function () {
            var a = this.chart, b = this.options, c = b.offsetLeft || 0, d = this.horiz, e = o(b.width, a.plotWidth - c + (b.offsetRight || 0)), f = o(b.height, a.plotHeight), g = o(b.top, a.plotTop), b = o(b.left, a.plotLeft +
            c), c = /%$/; c.test(f) && (f = parseFloat(f) / 100 * a.plotHeight); c.test(g) && (g = parseFloat(g) / 100 * a.plotHeight + a.plotTop); this.left = b; this.top = g; this.width = e; this.height = f; this.bottom = a.chartHeight - f - g; this.right = a.chartWidth - e - b; this.len = w(d ? e : f, 0); this.pos = d ? b : g
        }, getExtremes: function () { var a = this.isLog; return { min: a ? xa(na(this.min)) : this.min, max: a ? xa(na(this.max)) : this.max, dataMin: this.dataMin, dataMax: this.dataMax, userMin: this.userMin, userMax: this.userMax } }, getThreshold: function (a) {
            var b = this.isLog, c = b ? na(this.min) :
            this.min, b = b ? na(this.max) : this.max; c > a || a === null ? a = c : b < a && (a = b); return this.translate(a, 0, 1, 0, 1)
        }, autoLabelAlign: function (a) { a = (o(a, 0) - this.side * 90 + 720) % 360; return a > 15 && a < 165 ? "right" : a > 195 && a < 345 ? "left" : "center" }, unsquish: function () {
            var a = this.ticks, b = this.options.labels, c = this.horiz, d = this.tickInterval, e = d, f = this.len / (((this.categories ? 1 : 0) + this.max - this.min) / d), g, h = b.rotation, i = this.chart.renderer.fontMetrics(b.style.fontSize, a[0] && a[0].label), j, k = Number.MAX_VALUE, l, n = function (a) {
                a /= f || 1; a = a > 1 ?
                sa(a) : 1; return a * d
            }; c ? (l = r(h) ? [h] : f < o(b.autoRotationLimit, 80) && !b.staggerLines && !b.step && b.autoRotation) && m(l, function (a) { var b; if (a === h || a && a >= -90 && a <= 90) j = n(Q(i.h / oa(la * a))), b = j + Q(a / 360), b < k && (k = b, g = a, e = j) }) : e = n(i.h); this.autoRotation = l; this.labelRotation = g; return e
        }, renderUnsquish: function () {
            var a = this.chart, b = a.renderer, c = this.tickPositions, d = this.ticks, e = this.options.labels, f = this.horiz, g = a.margin, h = this.slotWidth = f && !e.step && !e.rotation && (this.staggerLines || 1) * a.plotWidth / c.length || !f && (g[3] &&
            g[3] - a.spacing[3] || a.chartWidth * 0.33), i = w(1, x(h - 2 * (e.padding || 5))), j = {}, g = b.fontMetrics(e.style.fontSize, d[0] && d[0].label), k, l = 0; if (!Ba(e.rotation)) j.rotation = e.rotation; if (this.autoRotation) m(c, function (a) { if ((a = d[a]) && a.labelLength > l) l = a.labelLength }), l > i && l > g.h ? j.rotation = this.labelRotation : this.labelRotation = 0; else if (h) {
                k = { width: i + "px", textOverflow: "clip" }; for (h = c.length; !f && h--;) if (i = c[h], i = d[i].label) if (i.styles.textOverflow === "ellipsis" && i.css({ textOverflow: "clip" }), i.getBBox().height > this.len /
                c.length - (g.h - g.f)) i.specCss = { textOverflow: "ellipsis" }
            } j.rotation && (k = { width: (l > a.chartHeight * 0.5 ? a.chartHeight * 0.33 : a.chartHeight) + "px", textOverflow: "ellipsis" }); this.labelAlign = j.align = e.align || this.autoLabelAlign(this.labelRotation); m(c, function (a) { var b = (a = d[a]) && a.label; if (b) k && b.css(A(k, b.specCss)), delete b.specCss, b.attr(j), a.rotation = j.rotation }); this.tickRotCorr = b.rotCorr(g.b, this.labelRotation || 0, this.side === 2)
        }, getOffset: function () {
            var a = this, b = a.chart, c = b.renderer, d = a.options, e = a.tickPositions,
            f = a.ticks, g = a.horiz, h = a.side, i = b.inverted ? [1, 0, 3, 2][h] : h, j, k, l = 0, n, p = 0, v = d.title, t = d.labels, S = 0, B = b.axisOffset, b = b.clipOffset, y = [-1, 1, 1, -1][h], q; a.hasData = j = a.hasVisibleSeries || r(a.min) && r(a.max) && !!e; a.showAxis = k = j || o(d.showEmpty, !0); a.staggerLines = a.horiz && t.staggerLines; if (!a.axisGroup) a.gridGroup = c.g("grid").attr({ zIndex: d.gridZIndex || 1 }).add(), a.axisGroup = c.g("axis").attr({ zIndex: d.zIndex || 2 }).add(), a.labelGroup = c.g("axis-labels").attr({ zIndex: t.zIndex || 7 }).addClass("highcharts-" + a.coll.toLowerCase() +
            "-labels").add(); if (j || a.isLinked) { if (m(e, function (b) { f[b] ? f[b].addLabel() : f[b] = new Ma(a, b) }), a.renderUnsquish(), m(e, function (b) { if (h === 0 || h === 2 || { 1: "left", 3: "right" }[h] === a.labelAlign) S = w(f[b].getLabelSize(), S) }), a.staggerLines) S *= a.staggerLines, a.labelOffset = S } else for (q in f) f[q].destroy(), delete f[q]; if (v && v.text && v.enabled !== !1) {
                if (!a.axisTitle) a.axisTitle = c.text(v.text, 0, 0, v.useHTML).attr({ zIndex: 7, rotation: v.rotation || 0, align: v.textAlign || { low: "left", middle: "center", high: "right" }[v.align] }).addClass("highcharts-" +
                this.coll.toLowerCase() + "-title").css(v.style).add(a.axisGroup), a.axisTitle.isNew = !0; if (k) l = a.axisTitle.getBBox()[g ? "height" : "width"], n = v.offset, p = r(n) ? 0 : o(v.margin, g ? 5 : 10); a.axisTitle[k ? "show" : "hide"]()
            } a.offset = y * o(d.offset, B[h]); a.tickRotCorr = a.tickRotCorr || { x: 0, y: 0 }; c = h === 2 ? a.tickRotCorr.y : 0; g = S + p + (S && y * (g ? o(t.y, a.tickRotCorr.y + 8) : t.x) - c); a.axisTitleMargin = o(n, g); B[h] = w(B[h], a.axisTitleMargin + l + y * a.offset, g); b[i] = w(b[i], fa(d.lineWidth / 2) * 2)
        }, getLinePath: function (a) {
            var b = this.chart, c = this.opposite,
            d = this.offset, e = this.horiz, f = this.left + (c ? this.width : 0) + d, d = b.chartHeight - this.bottom - (c ? this.height : 0) + d; c && (a *= -1); return b.renderer.crispLine(["M", e ? this.left : f, e ? d : this.top, "L", e ? b.chartWidth - this.right : f, e ? d : b.chartHeight - this.bottom], a)
        }, getTitlePosition: function () {
            var a = this.horiz, b = this.left, c = this.top, d = this.len, e = this.options.title, f = a ? b : c, g = this.opposite, h = this.offset, i = D(e.style.fontSize || 12), d = { low: f + (a ? 0 : d), middle: f + d / 2, high: f + (a ? d : 0) }[e.align], b = (a ? c + this.height : b) + (a ? 1 : -1) * (g ? -1 : 1) *
            this.axisTitleMargin + (this.side === 2 ? i : 0); return { x: a ? d : b + (g ? this.width : 0) + h + (e.x || 0), y: a ? b - (g ? this.height : 0) + h : d + (e.y || 0) }
        }, render: function () {
            var a = this, b = a.chart, c = b.renderer, d = a.options, e = a.isLog, f = a.isLinked, g = a.tickPositions, h = a.axisTitle, i = a.ticks, j = a.minorTicks, k = a.alternateBands, l = d.stackLabels, n = d.alternateGridColor, p = a.tickmarkOffset, v = d.lineWidth, t, o = b.hasRendered && r(a.oldMin) && !isNaN(a.oldMin); t = a.hasData; var B = a.showAxis, y, s; a.labelEdge.length = 0; a.overlap = !1; m([i, j, k], function (a) {
                for (var b in a) a[b].isActive =
                !1
            }); if (t || f) {
                a.minorTickInterval && !a.categories && m(a.getMinorTickPositions(), function (b) { j[b] || (j[b] = new Ma(a, b, "minor")); o && j[b].isNew && j[b].render(null, !0); j[b].render(null, !1, 1) }); if (g.length && (m(g, function (b, c) { if (!f || b >= a.min && b <= a.max) i[b] || (i[b] = new Ma(a, b)), o && i[b].isNew && i[b].render(c, !0, 0.1), i[b].render(c) }), p && (a.min === 0 || a.single))) i[-1] || (i[-1] = new Ma(a, -1, null, !0)), i[-1].render(-1); n && m(g, function (b, c) {
                    if (c % 2 === 0 && b < a.max) k[b] || (k[b] = new z.PlotLineOrBand(a)), y = b + p, s = g[c + 1] !== q ? g[c +
                    1] + p : a.max, k[b].options = { from: e ? na(y) : y, to: e ? na(s) : s, color: n }, k[b].render(), k[b].isActive = !0
                }); if (!a._addedPlotLB) m((d.plotLines || []).concat(d.plotBands || []), function (b) { a.addPlotBandOrLine(b) }), a._addedPlotLB = !0
            } m([i, j, k], function (a) { var c, d, e = [], f = ha ? ha.duration || 500 : 0, g = function () { for (d = e.length; d--;) a[e[d]] && !a[e[d]].isActive && (a[e[d]].destroy(), delete a[e[d]]) }; for (c in a) if (!a[c].isActive) a[c].render(c, !1, 0), a[c].isActive = !1, e.push(c); a === k || !b.hasRendered || !f ? g() : f && setTimeout(g, f) }); if (v) t =
            a.getLinePath(v), a.axisLine ? a.axisLine.animate({ d: t }) : a.axisLine = c.path(t).attr({ stroke: d.lineColor, "stroke-width": v, zIndex: 7 }).add(a.axisGroup), a.axisLine[B ? "show" : "hide"](); if (h && B) h[h.isNew ? "attr" : "animate"](a.getTitlePosition()), h.isNew = !1; l && l.enabled && a.renderStackTotals(); a.isDirty = !1
        }, redraw: function () { this.render(); m(this.plotLinesAndBands, function (a) { a.render() }); m(this.series, function (a) { a.isDirty = !0 }) }, destroy: function (a) {
            var b = this, c = b.stacks, d, e = b.plotLinesAndBands; a || Y(b); for (d in c) $a(c[d]),
            c[d] = null; m([b.ticks, b.minorTicks, b.alternateBands], function (a) { $a(a) }); for (a = e.length; a--;) e[a].destroy(); m("stackTotalGroup,axisLine,axisTitle,axisGroup,cross,gridGroup,labelGroup".split(","), function (a) { b[a] && (b[a] = b[a].destroy()) }); this.cross && this.cross.destroy()
        }, drawCrosshair: function (a, b) {
            var c, d = this.crosshair, e = d.animation; if (!this.crosshair || (r(b) || !o(this.crosshair.snap, !0)) === !1) this.hideCrosshair(); else if (o(d.snap, !0) ? r(b) && (c = this.isXAxis ? b.plotX : this.len - b.plotY) : c = this.horiz ? a.chartX -
            this.pos : this.len - a.chartY + this.pos, c = this.isRadial ? this.getPlotLinePath(this.isXAxis ? b.x : o(b.stackY, b.y)) || null : this.getPlotLinePath(null, null, null, null, c) || null, c === null) this.hideCrosshair(); else if (this.cross) this.cross.attr({ visibility: "visible" })[e ? "animate" : "attr"]({ d: c }, e); else { e = this.categories && !this.isRadial; e = { "stroke-width": d.width || (e ? this.transA : 1), stroke: d.color || (e ? "rgba(155,200,255,0.2)" : "#C0C0C0"), zIndex: d.zIndex || 2 }; if (d.dashStyle) e.dashstyle = d.dashStyle; this.cross = this.chart.renderer.path(c).attr(e).add() }
        },
        hideCrosshair: function () { this.cross && this.cross.hide() }
    }; s(X.prototype, void 0); X.prototype.getLogTickPositions = function (a, b, c, d) {
        var e = this.options, f = this.len, g = []; if (!d) this._minorAutoInterval = null; if (a >= 0.5) a = x(a), g = this.getLinearTickPositions(a, b, c); else if (a >= 0.08) for (var f = fa(b), h, i, j, k, l, e = a > 0.3 ? [1, 2, 4] : a > 0.15 ? [1, 2, 4, 6, 8] : [1, 2, 3, 4, 5, 6, 7, 8, 9]; f < c + 1 && !l; f++) { i = e.length; for (h = 0; h < i && !l; h++) j = Qa(na(f) * e[h]), j > b && (!d || k <= c) && k !== q && g.push(k), k > c && (l = !0), k = j } else if (b = na(b), c = na(c), a = e[d ? "minorTickInterval" :
        "tickInterval"], a = o(a === "auto" ? null : a, this._minorAutoInterval, (c - b) * (e.tickPixelInterval / (d ? 5 : 1)) / ((d ? f / this.tickPositions.length : f) || 1)), a = sb(a, null, J.pow(10, fa(J.log(a) / J.LN10))), g = Oa(this.getLinearTickPositions(a, b, c), Qa), !d) this._minorAutoInterval = a / 5; if (!d) this.tickInterval = a; return g
    }; var Hb = z.Tooltip = function () { this.init.apply(this, arguments) }; Hb.prototype = {
        init: function (a, b) {
            var c = b.borderWidth, d = b.style, e = D(d.padding); this.chart = a; this.options = b; this.crosshairs = []; this.now = { x: 0, y: 0 }; this.isHidden =
            !0; this.label = a.renderer.label("", 0, 0, b.shape || "callout", null, null, b.useHTML, null, "tooltip").attr({ padding: e, fill: b.backgroundColor, "stroke-width": c, r: b.borderRadius, zIndex: 8 }).css(d).css({ padding: 0 }).add().attr({ y: -9999 }); ga || this.label.shadow(b.shadow); this.shared = b.shared
        }, destroy: function () { if (this.label) this.label = this.label.destroy(); clearTimeout(this.hideTimer); clearTimeout(this.tooltipTimeout) }, move: function (a, b, c, d) {
            var e = this, f = e.now, g = e.options.animation !== !1 && !e.isHidden && (Q(a - f.x) > 1 ||
            Q(b - f.y) > 1), h = e.followPointer || e.len > 1; s(f, { x: g ? (2 * f.x + a) / 3 : a, y: g ? (f.y + b) / 2 : b, anchorX: h ? q : g ? (2 * f.anchorX + c) / 3 : c, anchorY: h ? q : g ? (f.anchorY + d) / 2 : d }); e.label.attr(f); if (g) clearTimeout(this.tooltipTimeout), this.tooltipTimeout = setTimeout(function () { e && e.move(a, b, c, d) }, 32)
        }, hide: function (a) {
            var b = this, c; clearTimeout(this.hideTimer); if (!this.isHidden) c = this.chart.hoverPoints, this.hideTimer = setTimeout(function () { b.label.fadeOut(); b.isHidden = !0 }, o(a, this.options.hideDelay, 500)), c && m(c, function (a) { a.setState() }),
            this.chart.hoverPoints = null, this.chart.hoverSeries = null
        }, getAnchor: function (a, b) {
            var c, d = this.chart, e = d.inverted, f = d.plotTop, g = d.plotLeft, h = 0, i = 0, j, k, a = ra(a); c = a[0].tooltipPos; this.followPointer && b && (b.chartX === q && (b = d.pointer.normalize(b)), c = [b.chartX - d.plotLeft, b.chartY - f]); c || (m(a, function (a) { j = a.series.yAxis; k = a.series.xAxis; h += a.plotX + (!e && k ? k.left - g : 0); i += (a.plotLow ? (a.plotLow + a.plotHigh) / 2 : a.plotY) + (!e && j ? j.top - f : 0) }), h /= a.length, i /= a.length, c = [e ? d.plotWidth - i : h, this.shared && !e && a.length > 1 &&
            b ? b.chartY - f : e ? d.plotHeight - h : i]); return Oa(c, x)
        }, getPosition: function (a, b, c) {
            var d = this.chart, e = this.distance, f = {}, g = c.h, h, i = ["y", d.chartHeight, b, c.plotY + d.plotTop], j = ["x", d.chartWidth, a, c.plotX + d.plotLeft], k = o(c.ttBelow, d.inverted && !c.negative || !d.inverted && c.negative), l = function (a, b, c, d) { var h = c < d - e, i = d + e + c < b, j = d - e - c; d += e; if (k && i) f[a] = d; else if (!k && h) f[a] = j; else if (h) f[a] = j - g < 0 ? j : j - g; else if (i) f[a] = d + g + c > b ? d : d + g; else return !1 }, n = function (a, b, c, d) {
                if (d < e || d > b - e) return !1; else f[a] = d < c / 2 ? 1 : d > b - c /
                2 ? b - c - 2 : d - c / 2
            }, p = function (a) { var b = i; i = j; j = b; h = a }, v = function () { l.apply(0, i) !== !1 ? n.apply(0, j) === !1 && !h && (p(!0), v()) : h ? f.x = f.y = 0 : (p(!0), v()) }; (d.inverted || this.len > 1) && p(); v(); return f
        }, defaultFormatter: function (a) { var b = this.points || ra(this), c; c = [a.tooltipFooterHeaderFormatter(b[0])]; c = c.concat(a.bodyFormatter(b)); c.push(a.tooltipFooterHeaderFormatter(b[0], !0)); return c.join("") }, refresh: function (a, b) {
            var c = this.chart, d = this.label, e = this.options, f, g, h = {}, i, j = []; i = e.formatter || this.defaultFormatter;
            var h = c.hoverPoints, k, l = this.shared; clearTimeout(this.hideTimer); this.followPointer = ra(a)[0].series.tooltipOptions.followPointer; g = this.getAnchor(a, b); f = g[0]; g = g[1]; l && (!a.series || !a.series.noSharedTooltip) ? (c.hoverPoints = a, h && m(h, function (a) { a.setState() }), m(a, function (a) { a.setState("hover"); j.push(a.getLabelConfig()) }), h = { x: a[0].category, y: a[0].y }, h.points = j, this.len = j.length, a = a[0]) : h = a.getLabelConfig(); i = i.call(h, this); h = a.series; this.distance = o(h.tooltipOptions.distance, 16); i === !1 ? this.hide() :
            (this.isHidden && (Va(d), d.attr("opacity", 1).show()), d.attr({ text: i }), k = e.borderColor || a.color || h.color || "#606060", d.attr({ stroke: k }), this.updatePosition({ plotX: f, plotY: g, negative: a.negative, ttBelow: a.ttBelow, h: a.shapeArgs && a.shapeArgs.height || 0 }), this.isHidden = !1); L(c, "tooltipRefresh", { text: i, x: f + c.plotLeft, y: g + c.plotTop, borderColor: k })
        }, updatePosition: function (a) {
            var b = this.chart, c = this.label, c = (this.options.positioner || this.getPosition).call(this, c.width, c.height, a); this.move(x(c.x), x(c.y), a.plotX +
            b.plotLeft, a.plotY + b.plotTop)
        }, getXDateFormat: function (a, b, c) { var d, b = b.dateTimeLabelFormats, e = c && c.closestPointRange, f, g = { millisecond: 15, second: 12, minute: 9, hour: 6, day: 3 }, h, i; if (e) { h = Ka("%m-%d %H:%M:%S.%L", a.x); for (f in cb) { if (e === cb.week && +Ka("%w", a.x) === c.options.startOfWeek && h.substr(6) === "00:00:00.000") { f = "week"; break } else if (cb[f] > e) { f = i; break } else if (g[f] && h.substr(g[f]) !== "01-01 00:00:00.000".substr(g[f])) break; f !== "week" && (i = f) } f && (d = b[f]) } else d = b.day; return d || b.year }, tooltipFooterHeaderFormatter: function (a,
        b) { var c = b ? "footer" : "header", d = a.series, e = d.tooltipOptions, f = e.xDateFormat, g = d.xAxis, h = g && g.options.type === "datetime" && qa(a.key), c = e[c + "Format"]; h && !f && (f = this.getXDateFormat(a, e, g)); h && f && (c = c.replace("{point.key}", "{point.key:" + f + "}")); return Ja(c, { point: a, series: d }) }, bodyFormatter: function (a) { return Oa(a, function (a) { var c = a.series.tooltipOptions; return (c.pointFormatter || a.point.tooltipFormatter).call(a.point, c.pointFormat) }) }
    }; var pa; Sa = C.documentElement.ontouchstart !== q; var Aa = z.Pointer = function (a,
    b) { this.init(a, b) }; Aa.prototype = {
        init: function (a, b) { var c = b.chart, d = c.events, e = ga ? "" : c.zoomType, c = a.inverted, f; this.options = b; this.chart = a; this.zoomX = f = /x/.test(e); this.zoomY = e = /y/.test(e); this.zoomHor = f && !c || e && c; this.zoomVert = e && !c || f && c; this.hasZoom = f || e; this.runChartClick = d && !!d.click; this.pinchDown = []; this.lastValidTouch = {}; if (z.Tooltip && b.tooltip.enabled) a.tooltip = new Hb(a, b.tooltip), this.followTouchMove = o(b.tooltip.followTouchMove, !0); this.setDOMEvents() }, normalize: function (a, b) {
            var c, d, a =
            a || window.event, a = Ob(a); if (!a.target) a.target = a.srcElement; d = a.touches ? a.touches.length ? a.touches.item(0) : a.changedTouches[0] : a; if (!b) this.chartPosition = b = Nb(this.chart.container); d.pageX === q ? (c = w(a.x, a.clientX - b.left), d = a.y) : (c = d.pageX - b.left, d = d.pageY - b.top); return s(a, { chartX: x(c), chartY: x(d) })
        }, getCoordinates: function (a) { var b = { xAxis: [], yAxis: [] }; m(this.chart.axes, function (c) { b[c.isXAxis ? "xAxis" : "yAxis"].push({ axis: c, value: c.toValue(a[c.horiz ? "chartX" : "chartY"]) }) }); return b }, runPointActions: function (a) {
            var b =
            this.chart, c = b.series, d = b.tooltip, e = d ? d.shared : !1, f = b.hoverPoint, g = b.hoverSeries, h, i = b.chartWidth, j = b.chartWidth, k, l = [], n, p; if (!e && !g) for (h = 0; h < c.length; h++) if (c[h].directTouch || !c[h].options.stickyTracking) c = []; !e && g && g.directTouch && f ? n = f : (m(c, function (b) { k = b.noSharedTooltip && e; b.visible && !k && o(b.options.enableMouseTracking, !0) && (p = b.searchPoint(a)) && l.push(p) }), m(l, function (a) {
                if (a && r(a.plotX) && r(a.plotY) && (a.dist.distX < i || (a.dist.distX === i || a.series.kdDimensions > 1) && a.dist.distR < j)) i = a.dist.distX,
                j = a.dist.distR, n = a
            })); if (n && (n !== f || d && d.isHidden)) if (e && !n.series.noSharedTooltip) { for (h = l.length; h--;) (l[h].clientX !== n.clientX || l[h].series.noSharedTooltip) && l.splice(h, 1); l.length && d && d.refresh(l, a); m(l, function (b) { if (b !== n) b.onMouseOver(a) }); (g && g.directTouch && f || n).onMouseOver(a) } else d && d.refresh(n, a), n.onMouseOver(a); else c = g && g.tooltipOptions.followPointer, d && c && !d.isHidden && (c = d.getAnchor([{}], a), d.updatePosition({ plotX: c[0], plotY: c[1] })); if (d && !this._onDocumentMouseMove) this._onDocumentMouseMove =
            function (a) { if (W[pa]) W[pa].pointer.onDocumentMouseMove(a) }, M(C, "mousemove", this._onDocumentMouseMove); m(b.axes, function (b) { b.drawCrosshair(a, o(n, f)) })
        }, reset: function (a, b) {
            var c = this.chart, d = c.hoverSeries, e = c.hoverPoint, f = c.tooltip, g = f && f.shared ? c.hoverPoints : e; (a = a && f && g) && ra(g)[0].plotX === q && (a = !1); if (a) f.refresh(g), e && (e.setState(e.state, !0), m(c.axes, function (b) { o(b.options.crosshair && b.options.crosshair.snap, !0) ? b.drawCrosshair(null, a) : b.hideCrosshair() })); else {
                if (e) e.onMouseOut(); if (d) d.onMouseOut();
                f && f.hide(b); if (this._onDocumentMouseMove) Y(C, "mousemove", this._onDocumentMouseMove), this._onDocumentMouseMove = null; m(c.axes, function (a) { a.hideCrosshair() }); this.hoverX = null
            }
        }, scaleGroups: function (a, b) { var c = this.chart, d; m(c.series, function (e) { d = a || e.getPlotBox(); e.xAxis && e.xAxis.zoomEnabled && (e.group.attr(d), e.markerGroup && (e.markerGroup.attr(d), e.markerGroup.clip(b ? c.clipRect : null)), e.dataLabelsGroup && e.dataLabelsGroup.attr(d)) }); c.clipRect.attr(b || c.clipBox) }, dragStart: function (a) {
            var b = this.chart;
            b.mouseIsDown = a.type; b.cancelClick = !1; b.mouseDownX = this.mouseDownX = a.chartX; b.mouseDownY = this.mouseDownY = a.chartY
        }, drag: function (a) {
            var b = this.chart, c = b.options.chart, d = a.chartX, e = a.chartY, f = this.zoomHor, g = this.zoomVert, h = b.plotLeft, i = b.plotTop, j = b.plotWidth, k = b.plotHeight, l, n = this.mouseDownX, p = this.mouseDownY, m = c.panKey && a[c.panKey + "Key"]; d < h ? d = h : d > h + j && (d = h + j); e < i ? e = i : e > i + k && (e = i + k); this.hasDragged = Math.sqrt(Math.pow(n - d, 2) + Math.pow(p - e, 2)); if (this.hasDragged > 10) {
                l = b.isInsidePlot(n - h, p - i); if (b.hasCartesianSeries &&
                (this.zoomX || this.zoomY) && l && !m && !this.selectionMarker) this.selectionMarker = b.renderer.rect(h, i, f ? 1 : j, g ? 1 : k, 0).attr({ fill: c.selectionMarkerFill || "rgba(69,114,167,0.25)", zIndex: 7 }).add(); this.selectionMarker && f && (d -= n, this.selectionMarker.attr({ width: Q(d), x: (d > 0 ? 0 : d) + n })); this.selectionMarker && g && (d = e - p, this.selectionMarker.attr({ height: Q(d), y: (d > 0 ? 0 : d) + p })); l && !this.selectionMarker && c.panning && b.pan(a, c.panning)
            }
        }, drop: function (a) {
            var b = this, c = this.chart, d = this.hasPinched; if (this.selectionMarker) {
                var e =
                { xAxis: [], yAxis: [], originalEvent: a.originalEvent || a }, f = this.selectionMarker, g = f.attr ? f.attr("x") : f.x, h = f.attr ? f.attr("y") : f.y, i = f.attr ? f.attr("width") : f.width, j = f.attr ? f.attr("height") : f.height, k; if (this.hasDragged || d) m(c.axes, function (c) { if (c.zoomEnabled && r(c.min) && (d || b[{ xAxis: "zoomX", yAxis: "zoomY" }[c.coll]])) { var f = c.horiz, p = a.type === "touchend" ? c.minPixelPadding : 0, m = c.toValue((f ? g : h) + p), f = c.toValue((f ? g + i : h + j) - p); e[c.coll].push({ axis: c, min: N(m, f), max: w(m, f) }); k = !0 } }), k && L(c, "selection", e, function (a) {
                    c.zoom(s(a,
                    d ? { animation: !1 } : null))
                }); this.selectionMarker = this.selectionMarker.destroy(); d && this.scaleGroups()
            } if (c) F(c.container, { cursor: c._cursor }), c.cancelClick = this.hasDragged > 10, c.mouseIsDown = this.hasDragged = this.hasPinched = !1, this.pinchDown = []
        }, onContainerMouseDown: function (a) { a = this.normalize(a); a.preventDefault && a.preventDefault(); this.dragStart(a) }, onDocumentMouseUp: function (a) { W[pa] && W[pa].pointer.drop(a) }, onDocumentMouseMove: function (a) {
            var b = this.chart, c = this.chartPosition, a = this.normalize(a, c);
            c && !this.inClass(a.target, "highcharts-tracker") && !b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && this.reset()
        }, onContainerMouseLeave: function () { var a = W[pa]; if (a) a.pointer.reset(), a.pointer.chartPosition = null }, onContainerMouseMove: function (a) { var b = this.chart; pa = b.index; a = this.normalize(a); a.returnValue = !1; b.mouseIsDown === "mousedown" && this.drag(a); (this.inClass(a.target, "highcharts-tracker") || b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop)) && !b.openMenu && this.runPointActions(a) }, inClass: function (a,
        b) { for (var c; a;) { if (c = H(a, "class")) if (c.indexOf(b) !== -1) return !0; else if (c.indexOf("highcharts-container") !== -1) return !1; a = a.parentNode } }, onTrackerMouseOut: function (a) { var b = this.chart.hoverSeries, c = (a = a.relatedTarget || a.toElement) && a.point && a.point.series; if (b && !b.options.stickyTracking && !this.inClass(a, "highcharts-tooltip") && c !== b) b.onMouseOut() }, onContainerClick: function (a) {
            var b = this.chart, c = b.hoverPoint, d = b.plotLeft, e = b.plotTop, a = this.normalize(a); a.originalEvent = a; a.cancelBubble = !0; b.cancelClick ||
            (c && this.inClass(a.target, "highcharts-tracker") ? (L(c.series, "click", s(a, { point: c })), b.hoverPoint && c.firePointEvent("click", a)) : (s(a, this.getCoordinates(a)), b.isInsidePlot(a.chartX - d, a.chartY - e) && L(b, "click", a)))
        }, setDOMEvents: function () {
            var a = this, b = a.chart.container; b.onmousedown = function (b) { a.onContainerMouseDown(b) }; b.onmousemove = function (b) { a.onContainerMouseMove(b) }; b.onclick = function (b) { a.onContainerClick(b) }; M(b, "mouseleave", a.onContainerMouseLeave); Ta === 1 && M(C, "mouseup", a.onDocumentMouseUp);
            if (Sa) b.ontouchstart = function (b) { a.onContainerTouchStart(b) }, b.ontouchmove = function (b) { a.onContainerTouchMove(b) }, Ta === 1 && M(C, "touchend", a.onDocumentTouchEnd)
        }, destroy: function () { var a; Y(this.chart.container, "mouseleave", this.onContainerMouseLeave); Ta || (Y(C, "mouseup", this.onDocumentMouseUp), Y(C, "touchend", this.onDocumentTouchEnd)); clearInterval(this.tooltipTimeout); for (a in this) this[a] = null }
    }; s(z.Pointer.prototype, {
        pinchTranslate: function (a, b, c, d, e, f) {
            (this.zoomHor || this.pinchHor) && this.pinchTranslateDirection(!0,
            a, b, c, d, e, f); (this.zoomVert || this.pinchVert) && this.pinchTranslateDirection(!1, a, b, c, d, e, f)
        }, pinchTranslateDirection: function (a, b, c, d, e, f, g, h) {
            var i = this.chart, j = a ? "x" : "y", k = a ? "X" : "Y", l = "chart" + k, n = a ? "width" : "height", p = i["plot" + (a ? "Left" : "Top")], m, t, o = h || 1, B = i.inverted, y = i.bounds[a ? "h" : "v"], q = b.length === 1, s = b[0][l], r = c[0][l], x = !q && b[1][l], w = !q && c[1][l], u, c = function () { !q && Q(s - x) > 20 && (o = h || Q(r - w) / Q(s - x)); t = (p - r) / o + s; m = i["plot" + (a ? "Width" : "Height")] / o }; c(); b = t; b < y.min ? (b = y.min, u = !0) : b + m > y.max && (b = y.max -
            m, u = !0); u ? (r -= 0.8 * (r - g[j][0]), q || (w -= 0.8 * (w - g[j][1])), c()) : g[j] = [r, w]; B || (f[j] = t - p, f[n] = m); f = B ? 1 / o : o; e[n] = m; e[j] = b; d[B ? a ? "scaleY" : "scaleX" : "scale" + k] = o; d["translate" + k] = f * p + (r - f * s)
        }, pinch: function (a) {
            var b = this, c = b.chart, d = b.pinchDown, e = a.touches, f = e.length, g = b.lastValidTouch, h = b.hasZoom, i = b.selectionMarker, j = {}, k = f === 1 && (b.inClass(a.target, "highcharts-tracker") && c.runTrackerClick || b.runChartClick), l = {}; h && !k && a.preventDefault(); Oa(e, function (a) { return b.normalize(a) }); if (a.type === "touchstart") m(e,
            function (a, b) { d[b] = { chartX: a.chartX, chartY: a.chartY } }), g.x = [d[0].chartX, d[1] && d[1].chartX], g.y = [d[0].chartY, d[1] && d[1].chartY], m(c.axes, function (a) { if (a.zoomEnabled) { var b = c.bounds[a.horiz ? "h" : "v"], d = a.minPixelPadding, e = a.toPixels(o(a.options.min, a.dataMin)), f = a.toPixels(o(a.options.max, a.dataMax)), g = N(e, f), e = w(e, f); b.min = N(a.pos, g - d); b.max = w(a.pos + a.len, e + d) } }), b.res = !0; else if (d.length) {
                if (!i) b.selectionMarker = i = s({ destroy: T }, c.plotBox); b.pinchTranslate(d, e, j, i, l, g); b.hasPinched = h; b.scaleGroups(j,
                l); if (!h && b.followTouchMove && f === 1) this.runPointActions(b.normalize(a)); else if (b.res) b.res = !1, this.reset(!1, 0)
            }
        }, onContainerTouchStart: function (a) { var b = this.chart; pa = b.index; a.touches.length === 1 ? (a = this.normalize(a), b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && !b.openMenu ? (this.runPointActions(a), this.pinch(a)) : this.reset()) : a.touches.length === 2 && this.pinch(a) }, onContainerTouchMove: function (a) { (a.touches.length === 1 || a.touches.length === 2) && this.pinch(a) }, onDocumentTouchEnd: function (a) {
            W[pa] &&
            W[pa].pointer.drop(a)
        }
    }); if (K.PointerEvent || K.MSPointerEvent) {
        var ua = {}, qb = !!K.PointerEvent, Sb = function () { var a, b = []; b.item = function (a) { return this[a] }; for (a in ua) ua.hasOwnProperty(a) && b.push({ pageX: ua[a].pageX, pageY: ua[a].pageY, target: ua[a].target }); return b }, rb = function (a, b, c, d) { a = a.originalEvent || a; if ((a.pointerType === "touch" || a.pointerType === a.MSPOINTER_TYPE_TOUCH) && W[pa]) d(a), d = W[pa].pointer, d[b]({ type: c, target: a.currentTarget, preventDefault: T, touches: Sb() }) }; s(Aa.prototype, {
            onContainerPointerDown: function (a) {
                rb(a,
                "onContainerTouchStart", "touchstart", function (a) { ua[a.pointerId] = { pageX: a.pageX, pageY: a.pageY, target: a.currentTarget } })
            }, onContainerPointerMove: function (a) { rb(a, "onContainerTouchMove", "touchmove", function (a) { ua[a.pointerId] = { pageX: a.pageX, pageY: a.pageY }; if (!ua[a.pointerId].target) ua[a.pointerId].target = a.currentTarget }) }, onDocumentPointerUp: function (a) { rb(a, "onDocumentTouchEnd", "touchend", function (a) { delete ua[a.pointerId] }) }, batchMSEvents: function (a) {
                a(this.chart.container, qb ? "pointerdown" : "MSPointerDown",
                this.onContainerPointerDown); a(this.chart.container, qb ? "pointermove" : "MSPointerMove", this.onContainerPointerMove); a(C, qb ? "pointerup" : "MSPointerUp", this.onDocumentPointerUp)
            }
        }); da(Aa.prototype, "init", function (a, b, c) { a.call(this, b, c); this.hasZoom && F(b.container, { "-ms-touch-action": O, "touch-action": O }) }); da(Aa.prototype, "setDOMEvents", function (a) { a.apply(this); (this.hasZoom || this.followTouchMove) && this.batchMSEvents(M) }); da(Aa.prototype, "destroy", function (a) { this.batchMSEvents(Y); a.call(this) })
    } var Xa =
    z.Legend = function (a, b) { this.init(a, b) }; Xa.prototype = {
        init: function (a, b) { var c = this, d = b.itemStyle, e = b.itemMarginTop || 0; this.options = b; if (b.enabled) c.itemStyle = d, c.itemHiddenStyle = A(d, b.itemHiddenStyle), c.itemMarginTop = e, c.padding = d = o(b.padding, 8), c.initialItemX = d, c.initialItemY = d - 5, c.maxItemWidth = 0, c.chart = a, c.itemHeight = 0, c.symbolWidth = o(b.symbolWidth, 16), c.pages = [], c.render(), M(c.chart, "endResize", function () { c.positionCheckboxes() }) }, colorizeItem: function (a, b) {
            var c = this.options, d = a.legendItem, e =
            a.legendLine, f = a.legendSymbol, g = this.itemHiddenStyle.color, c = b ? c.itemStyle.color : g, h = b ? a.legendColor || a.color || "#CCC" : g, g = a.options && a.options.marker, i = { fill: h }, j; d && d.css({ fill: c, color: c }); e && e.attr({ stroke: h }); if (f) { if (g && f.isMarker) for (j in i.stroke = h, g = a.convertAttribs(g), g) d = g[j], d !== q && (i[j] = d); f.attr(i) }
        }, positionItem: function (a) {
            var b = this.options, c = b.symbolPadding, b = !b.rtl, d = a._legendItemPos, e = d[0], d = d[1], f = a.checkbox; a.legendGroup && a.legendGroup.translate(b ? e : this.legendWidth - e - 2 * c - 4, d);
            if (f) f.x = e, f.y = d
        }, destroyItem: function (a) { var b = a.checkbox; m(["legendItem", "legendLine", "legendSymbol", "legendGroup"], function (b) { a[b] && (a[b] = a[b].destroy()) }); b && La(a.checkbox) }, clearItems: function () { var a = this; m(a.getAllItems(), function (b) { a.destroyItem(b) }) }, destroy: function () { var a = this.group, b = this.box; if (b) this.box = b.destroy(); if (a) this.group = a.destroy() }, positionCheckboxes: function (a) {
            var b = this.group.alignAttr, c, d = this.clipHeight || this.legendHeight; if (b) c = b.translateY, m(this.allItems, function (e) {
                var f =
                e.checkbox, g; f && (g = c + f.y + (a || 0) + 3, F(f, { left: b.translateX + e.checkboxOffset + f.x - 20 + "px", top: g + "px", display: g > c - 6 && g < c + d - 6 ? "" : O }))
            })
        }, renderTitle: function () { var a = this.padding, b = this.options.title, c = 0; if (b.text) { if (!this.title) this.title = this.chart.renderer.label(b.text, a - 3, a - 4, null, null, null, null, null, "legend-title").attr({ zIndex: 1 }).css(b.style).add(this.group); a = this.title.getBBox(); c = a.height; this.offsetWidth = a.width; this.contentGroup.attr({ translateY: c }) } this.titleHeight = c }, renderItem: function (a) {
            var b =
            this.chart, c = b.renderer, d = this.options, e = d.layout === "horizontal", f = this.symbolWidth, g = d.symbolPadding, h = this.itemStyle, i = this.itemHiddenStyle, j = this.padding, k = e ? o(d.itemDistance, 20) : 0, l = !d.rtl, n = d.width, p = d.itemMarginBottom || 0, m = this.itemMarginTop, t = this.initialItemX, q = a.legendItem, B = a.series && a.series.drawLegendSymbol ? a.series : a, y = B.options, y = this.createCheckboxForItem && y && y.showCheckbox, s = d.useHTML; if (!q) {
                a.legendGroup = c.g("legend-item").attr({ zIndex: 1 }).add(this.scrollGroup); a.legendItem = q = c.text(d.labelFormat ?
                Ja(d.labelFormat, a) : d.labelFormatter.call(a), l ? f + g : -g, this.baseline || 0, s).css(A(a.visible ? h : i)).attr({ align: l ? "left" : "right", zIndex: 2 }).add(a.legendGroup); if (!this.baseline) this.fontMetrics = c.fontMetrics(h.fontSize, q), this.baseline = this.fontMetrics.f + 3 + m, q.attr("y", this.baseline); B.drawLegendSymbol(this, a); this.setItemEvents && this.setItemEvents(a, q, s, h, i); this.colorizeItem(a, a.visible); y && this.createCheckboxForItem(a)
            } c = q.getBBox(); f = a.checkboxOffset = d.itemWidth || a.legendItemWidth || f + g + c.width + k +
            (y ? 20 : 0); this.itemHeight = g = x(a.legendItemHeight || c.height); if (e && this.itemX - t + f > (n || b.chartWidth - 2 * j - t - d.x)) this.itemX = t, this.itemY += m + this.lastLineHeight + p, this.lastLineHeight = 0; this.maxItemWidth = w(this.maxItemWidth, f); this.lastItemY = m + this.itemY + p; this.lastLineHeight = w(g, this.lastLineHeight); a._legendItemPos = [this.itemX, this.itemY]; e ? this.itemX += f : (this.itemY += m + g + p, this.lastLineHeight = g); this.offsetWidth = n || w((e ? this.itemX - t - k : f) + j, this.offsetWidth)
        }, getAllItems: function () {
            var a = []; m(this.chart.series,
            function (b) { var c = b.options; if (o(c.showInLegend, !r(c.linkedTo) ? q : !1, !0)) a = a.concat(b.legendItems || (c.legendType === "point" ? b.data : b)) }); return a
        }, adjustMargins: function (a, b) { var c = this.chart, d = this.options, e = d.align[0] + d.verticalAlign[0] + d.layout[0]; this.display && !d.floating && m([/(lth|ct|rth)/, /(rtv|rm|rbv)/, /(rbh|cb|lbh)/, /(lbv|lm|ltv)/], function (f, g) { f.test(e) && !r(a[g]) && (c[db[g]] = w(c[db[g]], c.legend[(g + 1) % 2 ? "legendHeight" : "legendWidth"] + [1, -1, -1, 1][g] * d[g % 2 ? "x" : "y"] + o(d.margin, 12) + b[g])) }) }, render: function () {
            var a =
            this, b = a.chart, c = b.renderer, d = a.group, e, f, g, h, i = a.box, j = a.options, k = a.padding, l = j.borderWidth, n = j.backgroundColor; a.itemX = a.initialItemX; a.itemY = a.initialItemY; a.offsetWidth = 0; a.lastItemY = 0; if (!d) a.group = d = c.g("legend").attr({ zIndex: 7 }).add(), a.contentGroup = c.g().attr({ zIndex: 1 }).add(d), a.scrollGroup = c.g().add(a.contentGroup); a.renderTitle(); e = a.getAllItems(); hb(e, function (a, b) { return (a.options && a.options.legendIndex || 0) - (b.options && b.options.legendIndex || 0) }); j.reversed && e.reverse(); a.allItems = e;
            a.display = f = !!e.length; a.lastLineHeight = 0; m(e, function (b) { a.renderItem(b) }); g = (j.width || a.offsetWidth) + k; h = a.lastItemY + a.lastLineHeight + a.titleHeight; h = a.handleOverflow(h); h += k; if (l || n) { if (i) { if (g > 0 && h > 0) i[i.isNew ? "attr" : "animate"](i.crisp({ width: g, height: h })), i.isNew = !1 } else a.box = i = c.rect(0, 0, g, h, j.borderRadius, l || 0).attr({ stroke: j.borderColor, "stroke-width": l || 0, fill: n || O }).add(d).shadow(j.shadow), i.isNew = !0; i[f ? "show" : "hide"]() } a.legendWidth = g; a.legendHeight = h; m(e, function (b) { a.positionItem(b) });
            f && d.align(s({ width: g, height: h }, j), !0, "spacingBox"); b.isResizing || this.positionCheckboxes()
        }, handleOverflow: function (a) {
            var b = this, c = this.chart, d = c.renderer, e = this.options, f = e.y, f = c.spacingBox.height + (e.verticalAlign === "top" ? -f : f) - this.padding, g = e.maxHeight, h, i = this.clipRect, j = e.navigation, k = o(j.animation, !0), l = j.arrowSize || 12, n = this.nav, p = this.pages, v, t = this.allItems; e.layout === "horizontal" && (f /= 2); g && (f = N(f, g)); p.length = 0; if (a > f && !e.useHTML) {
                this.clipHeight = h = w(f - 20 - this.titleHeight - this.padding,
                0); this.currentPage = o(this.currentPage, 1); this.fullHeight = a; m(t, function (a, b) { var c = a._legendItemPos[1], d = x(a.legendItem.getBBox().height), e = p.length; if (!e || c - p[e - 1] > h && (v || c) !== p[e - 1]) p.push(v || c), e++; b === t.length - 1 && c + d - p[e - 1] > h && p.push(c); c !== v && (v = c) }); if (!i) i = b.clipRect = d.clipRect(0, this.padding, 9999, 0), b.contentGroup.clip(i); i.attr({ height: h }); if (!n) this.nav = n = d.g().attr({ zIndex: 1 }).add(this.group), this.up = d.symbol("triangle", 0, 0, l, l).on("click", function () { b.scroll(-1, k) }).add(n), this.pager =
                d.text("", 15, 10).css(j.style).add(n), this.down = d.symbol("triangle-down", 0, 0, l, l).on("click", function () { b.scroll(1, k) }).add(n); b.scroll(0); a = f
            } else if (n) i.attr({ height: c.chartHeight }), n.hide(), this.scrollGroup.attr({ translateY: 1 }), this.clipHeight = 0; return a
        }, scroll: function (a, b) {
            var c = this.pages, d = c.length, e = this.currentPage + a, f = this.clipHeight, g = this.options.navigation, h = g.activeColor, g = g.inactiveColor, i = this.pager, j = this.padding; e > d && (e = d); if (e > 0) b !== q && (ha = o(b, this.chart.animation)), this.nav.attr({
                translateX: j,
                translateY: f + this.padding + 7 + this.titleHeight, visibility: "visible"
            }), this.up.attr({ fill: e === 1 ? g : h }).css({ cursor: e === 1 ? "default" : "pointer" }), i.attr({ text: e + "/" + d }), this.down.attr({ x: 18 + this.pager.getBBox().width, fill: e === d ? g : h }).css({ cursor: e === d ? "default" : "pointer" }), c = -c[e - 1] + this.initialItemY, this.scrollGroup.animate({ translateY: c }), this.currentPage = e, this.positionCheckboxes(c)
        }
    }; var Ya = z.LegendSymbolMixin = {
        drawRectangle: function (a, b) {
            var c = a.options.symbolHeight || a.fontMetrics.f; b.legendSymbol =
            this.chart.renderer.rect(0, a.baseline - c + 1, a.symbolWidth, c, a.options.symbolRadius || 0).attr({ zIndex: 3 }).add(b.legendGroup)
        }, drawLineMarker: function (a) {
            var b = this.options, c = b.marker, d; d = a.symbolWidth; var e = this.chart.renderer, f = this.legendGroup, a = a.baseline - x(a.fontMetrics.b * 0.3), g; if (b.lineWidth) { g = { "stroke-width": b.lineWidth }; if (b.dashStyle) g.dashstyle = b.dashStyle; this.legendLine = e.path(["M", 0, a, "L", d, a]).attr(g).add(f) } if (c && c.enabled !== !1) b = c.radius, this.legendSymbol = d = e.symbol(this.symbol, d / 2 - b,
            a - b, 2 * b, 2 * b).add(f), d.isMarker = !0
        }
    }; (/Trident\/7\.0/.test(ya) || Ha) && da(Xa.prototype, "positionItem", function (a, b) { var c = this, d = function () { b._legendItemPos && a.call(c, b) }; d(); setTimeout(d) }); var ma = z.Chart = function () { this.init.apply(this, arguments) }; ma.prototype = {
        callbacks: [], init: function (a, b) {
            var c, d = a.series; a.series = null; c = A(I, a); c.series = a.series = d; this.userOptions = a; d = c.chart; this.margin = this.splashArray("margin", d); this.spacing = this.splashArray("spacing", d); var e = d.events; this.bounds = { h: {}, v: {} };
            this.callback = b; this.isResizing = 0; this.options = c; this.axes = []; this.series = []; this.hasCartesianSeries = d.showAxes; var f = this, g; f.index = W.length; W.push(f); Ta++; d.reflow !== !1 && M(f, "load", function () { f.initReflow() }); if (e) for (g in e) M(f, g, e[g]); f.xAxis = []; f.yAxis = []; f.animation = ga ? !1 : o(d.animation, !0); f.pointCount = f.colorCounter = f.symbolCounter = 0; f.firstRender()
        }, initSeries: function (a) { var b = this.options.chart; (b = u[a.type || b.type || b.defaultSeriesType]) || ba(17, !0); b = new b; b.init(this, a); return b }, isInsidePlot: function (a,
        b, c) { var d = c ? b : a, a = c ? a : b; return d >= 0 && d <= this.plotWidth && a >= 0 && a <= this.plotHeight }, redraw: function (a) {
            var b = this.axes, c = this.series, d = this.pointer, e = this.legend, f = this.isDirtyLegend, g, h, i = this.hasCartesianSeries, j = this.isDirtyBox, k = c.length, l = k, n = this.renderer, p = n.isHidden(), v = []; ha = o(a, this.animation); p && this.cloneRenderTo(); for (this.layOutTitles() ; l--;) if (a = c[l], a.options.stacking && (g = !0, a.isDirty)) { h = !0; break } if (h) for (l = k; l--;) if (a = c[l], a.options.stacking) a.isDirty = !0; m(c, function (a) {
                a.isDirty &&
                a.options.legendType === "point" && (f = !0)
            }); if (f && e.options.enabled) e.render(), this.isDirtyLegend = !1; g && this.getStacks(); if (i && !this.isResizing) this.maxTicks = null, m(b, function (a) { a.setScale() }); this.getMargins(); i && (m(b, function (a) { a.isDirty && (j = !0) }), m(b, function (a) { if (a.isDirtyExtremes) a.isDirtyExtremes = !1, v.push(function () { L(a, "afterSetExtremes", s(a.eventArgs, a.getExtremes())); delete a.eventArgs }); (j || g) && a.redraw() })); j && this.drawChartBox(); m(c, function (a) {
                a.isDirty && a.visible && (!a.isCartesian ||
                a.xAxis) && a.redraw()
            }); d && d.reset(!0); n.draw(); L(this, "redraw"); p && this.cloneRenderTo(!0); m(v, function (a) { a.call() })
        }, get: function (a) { var b = this.axes, c = this.series, d, e; for (d = 0; d < b.length; d++) if (b[d].options.id === a) return b[d]; for (d = 0; d < c.length; d++) if (c[d].options.id === a) return c[d]; for (d = 0; d < c.length; d++) { e = c[d].points || []; for (b = 0; b < e.length; b++) if (e[b].id === a) return e[b] } return null }, getAxes: function () {
            var a = this, b = this.options, c = b.xAxis = ra(b.xAxis || {}), b = b.yAxis = ra(b.yAxis || {}); m(c, function (a,
            b) { a.index = b; a.isX = !0 }); m(b, function (a, b) { a.index = b }); c = c.concat(b); m(c, function (b) { new X(a, b) })
        }, getSelectedPoints: function () { var a = []; m(this.series, function (b) { a = a.concat(pb(b.points || [], function (a) { return a.selected })) }); return a }, getSelectedSeries: function () { return pb(this.series, function (a) { return a.selected }) }, getStacks: function () {
            var a = this; m(a.yAxis, function (a) { if (a.stacks && a.hasVisibleSeries) a.oldStacks = a.stacks }); m(a.series, function (b) {
                if (b.options.stacking && (b.visible === !0 || a.options.chart.ignoreHiddenSeries ===
                !1)) b.stackKey = b.type + o(b.options.stack, "")
            })
        }, setTitle: function (a, b, c) { var g; var d = this, e = d.options, f; f = e.title = A(e.title, a); g = e.subtitle = A(e.subtitle, b), e = g; m([["title", a, f], ["subtitle", b, e]], function (a) { var b = a[0], c = d[b], e = a[1], a = a[2]; c && e && (d[b] = c = c.destroy()); a && a.text && !c && (d[b] = d.renderer.text(a.text, 0, 0, a.useHTML).attr({ align: a.align, "class": "highcharts-" + b, zIndex: a.zIndex || 4 }).css(a.style).add()) }); d.layOutTitles(c) }, layOutTitles: function (a) {
            var b = 0, c = this.title, d = this.subtitle, e = this.options,
            f = e.title, e = e.subtitle, g = this.renderer, h = this.spacingBox.width - 44; if (c && (c.css({ width: (f.width || h) + "px" }).align(s({ y: g.fontMetrics(f.style.fontSize, c).b - 3 }, f), !1, "spacingBox"), !f.floating && !f.verticalAlign)) b = c.getBBox().height; d && (d.css({ width: (e.width || h) + "px" }).align(s({ y: b + (f.margin - 13) + g.fontMetrics(f.style.fontSize, d).b }, e), !1, "spacingBox"), !e.floating && !e.verticalAlign && (b = sa(b + d.getBBox().height))); c = this.titleOffset !== b; this.titleOffset = b; if (!this.isDirtyBox && c) this.isDirtyBox = c, this.hasRendered &&
            o(a, !0) && this.isDirtyBox && this.redraw()
        }, getChartSize: function () { var a = this.options.chart, b = a.width, a = a.height, c = this.renderToClone || this.renderTo; if (!r(b)) this.containerWidth = eb(c, "width"); if (!r(a)) this.containerHeight = eb(c, "height"); this.chartWidth = w(0, b || this.containerWidth || 600); this.chartHeight = w(0, o(a, this.containerHeight > 19 ? this.containerHeight : 400)) }, cloneRenderTo: function (a) {
            var b = this.renderToClone, c = this.container; a ? b && (this.renderTo.appendChild(c), La(b), delete this.renderToClone) : (c &&
            c.parentNode === this.renderTo && this.renderTo.removeChild(c), this.renderToClone = b = this.renderTo.cloneNode(0), F(b, { position: "absolute", top: "-9999px", display: "block" }), b.style.setProperty && b.style.setProperty("display", "block", "important"), C.body.appendChild(b), c && b.appendChild(c))
        }, getContainer: function () {
            var a, b = this.options.chart, c, d, e; this.renderTo = a = b.renderTo; e = "highcharts-" + nb++; if (Ba(a)) this.renderTo = a = C.getElementById(a); a || ba(13, !0); c = D(H(a, "data-highcharts-chart")); !isNaN(c) && W[c] && W[c].hasRendered &&
            W[c].destroy(); H(a, "data-highcharts-chart", this.index); a.innerHTML = ""; !b.skipClone && !a.offsetWidth && this.cloneRenderTo(); this.getChartSize(); c = this.chartWidth; d = this.chartHeight; this.container = a = $(Fa, { className: "highcharts-container" + (b.className ? " " + b.className : ""), id: e }, s({ position: "relative", overflow: "hidden", width: c + "px", height: d + "px", textAlign: "left", lineHeight: "normal", zIndex: 0, "-webkit-tap-highlight-color": "rgba(0,0,0,0)" }, b.style), this.renderToClone || a); this._cursor = a.style.cursor; this.renderer =
            b.forExport ? new ia(a, c, d, b.style, !0) : new Na(a, c, d, b.style); ga && this.renderer.create(this, a, c, d); this.renderer.chartIndex = this.index
        }, getMargins: function (a) { var b = this.spacing, c = this.margin, d = this.titleOffset; this.resetMargins(); if (d && !r(c[0])) this.plotTop = w(this.plotTop, d + this.options.title.margin + b[0]); this.legend.adjustMargins(c, b); this.extraBottomMargin && (this.marginBottom += this.extraBottomMargin); this.extraTopMargin && (this.plotTop += this.extraTopMargin); a || this.getAxisMargins() }, getAxisMargins: function () {
            var a =
            this, b = a.axisOffset = [0, 0, 0, 0], c = a.margin; a.hasCartesianSeries && m(a.axes, function (a) { a.getOffset() }); m(db, function (d, e) { r(c[e]) || (a[d] += b[e]) }); a.setChartSize()
        }, reflow: function (a) {
            var b = this, c = b.options.chart, d = b.renderTo, e = c.width || eb(d, "width"), f = c.height || eb(d, "height"), c = a ? a.target : K, d = function () { if (b.container) b.setSize(e, f, !1), b.hasUserSize = null }; if (!b.hasUserSize && !b.isPrinting && e && f && (c === K || c === C)) {
                if (e !== b.containerWidth || f !== b.containerHeight) clearTimeout(b.reflowTimeout), a ? b.reflowTimeout =
                setTimeout(d, 100) : d(); b.containerWidth = e; b.containerHeight = f
            }
        }, initReflow: function () { var a = this, b = function (b) { a.reflow(b) }; M(K, "resize", b); M(a, "destroy", function () { Y(K, "resize", b) }) }, setSize: function (a, b, c) {
            var d = this, e, f, g; d.isResizing += 1; g = function () { d && L(d, "endResize", null, function () { d.isResizing -= 1 }) }; ha = o(c, d.animation); d.oldChartHeight = d.chartHeight; d.oldChartWidth = d.chartWidth; if (r(a)) d.chartWidth = e = w(0, x(a)), d.hasUserSize = !!e; if (r(b)) d.chartHeight = f = w(0, x(b)); (ha ? fb : F)(d.container, {
                width: e +
                "px", height: f + "px"
            }, ha); d.setChartSize(!0); d.renderer.setSize(e, f, c); d.maxTicks = null; m(d.axes, function (a) { a.isDirty = !0; a.setScale() }); m(d.series, function (a) { a.isDirty = !0 }); d.isDirtyLegend = !0; d.isDirtyBox = !0; d.layOutTitles(); d.getMargins(); d.redraw(c); d.oldChartHeight = null; L(d, "resize"); ha === !1 ? g() : setTimeout(g, ha && ha.duration || 500)
        }, setChartSize: function (a) {
            var b = this.inverted, c = this.renderer, d = this.chartWidth, e = this.chartHeight, f = this.options.chart, g = this.spacing, h = this.clipOffset, i, j, k, l; this.plotLeft =
            i = x(this.plotLeft); this.plotTop = j = x(this.plotTop); this.plotWidth = k = w(0, x(d - i - this.marginRight)); this.plotHeight = l = w(0, x(e - j - this.marginBottom)); this.plotSizeX = b ? l : k; this.plotSizeY = b ? k : l; this.plotBorderWidth = f.plotBorderWidth || 0; this.spacingBox = c.spacingBox = { x: g[3], y: g[0], width: d - g[3] - g[1], height: e - g[0] - g[2] }; this.plotBox = c.plotBox = { x: i, y: j, width: k, height: l }; d = 2 * fa(this.plotBorderWidth / 2); b = sa(w(d, h[3]) / 2); c = sa(w(d, h[0]) / 2); this.clipBox = {
                x: b, y: c, width: fa(this.plotSizeX - w(d, h[1]) / 2 - b), height: w(0, fa(this.plotSizeY -
                w(d, h[2]) / 2 - c))
            }; a || m(this.axes, function (a) { a.setAxisSize(); a.setAxisTranslation() })
        }, resetMargins: function () { var a = this; m(db, function (b, c) { a[b] = o(a.margin[c], a.spacing[c]) }); a.axisOffset = [0, 0, 0, 0]; a.clipOffset = [0, 0, 0, 0] }, drawChartBox: function () {
            var a = this.options.chart, b = this.renderer, c = this.chartWidth, d = this.chartHeight, e = this.chartBackground, f = this.plotBackground, g = this.plotBorder, h = this.plotBGImage, i = a.borderWidth || 0, j = a.backgroundColor, k = a.plotBackgroundColor, l = a.plotBackgroundImage, n = a.plotBorderWidth ||
            0, p, m = this.plotLeft, o = this.plotTop, q = this.plotWidth, B = this.plotHeight, y = this.plotBox, s = this.clipRect, r = this.clipBox; p = i + (a.shadow ? 8 : 0); if (i || j) if (e) e.animate(e.crisp({ width: c - p, height: d - p })); else { e = { fill: j || O }; if (i) e.stroke = a.borderColor, e["stroke-width"] = i; this.chartBackground = b.rect(p / 2, p / 2, c - p, d - p, a.borderRadius, i).attr(e).addClass("highcharts-background").add().shadow(a.shadow) } if (k) f ? f.animate(y) : this.plotBackground = b.rect(m, o, q, B, 0).attr({ fill: k }).add().shadow(a.plotShadow); if (l) h ? h.animate(y) :
            this.plotBGImage = b.image(l, m, o, q, B).add(); s ? s.animate({ width: r.width, height: r.height }) : this.clipRect = b.clipRect(r); if (n) g ? g.animate(g.crisp({ x: m, y: o, width: q, height: B, strokeWidth: -n })) : this.plotBorder = b.rect(m, o, q, B, 0, -n).attr({ stroke: a.plotBorderColor, "stroke-width": n, fill: O, zIndex: 1 }).add(); this.isDirtyBox = !1
        }, propFromSeries: function () {
            var a = this, b = a.options.chart, c, d = a.options.series, e, f; m(["inverted", "angular", "polar"], function (g) {
                c = u[b.type || b.defaultSeriesType]; f = a[g] || b[g] || c && c.prototype[g];
                for (e = d && d.length; !f && e--;) (c = u[d[e].type]) && c.prototype[g] && (f = !0); a[g] = f
            })
        }, linkSeries: function () { var a = this, b = a.series; m(b, function (a) { a.linkedSeries.length = 0 }); m(b, function (b) { var d = b.options.linkedTo; if (Ba(d) && (d = d === ":previous" ? a.series[b.index - 1] : a.get(d))) d.linkedSeries.push(b), b.linkedParent = d }) }, renderSeries: function () { m(this.series, function (a) { a.translate(); a.render() }) }, renderLabels: function () {
            var a = this, b = a.options.labels; b.items && m(b.items, function (c) {
                var d = s(b.style, c.style), e = D(d.left) +
                a.plotLeft, f = D(d.top) + a.plotTop + 12; delete d.left; delete d.top; a.renderer.text(c.html, e, f).attr({ zIndex: 2 }).css(d).add()
            })
        }, render: function () {
            var a = this.axes, b = this.renderer, c = this.options, d, e, f, g; this.setTitle(); this.legend = new Xa(this, c.legend); this.getStacks(); this.getMargins(!0); this.setChartSize(); d = this.plotWidth; e = this.plotHeight -= 13; m(a, function (a) { a.setScale() }); this.getAxisMargins(); f = d / this.plotWidth > 1.1; g = e / this.plotHeight > 1.1; if (f || g) this.maxTicks = null, m(a, function (a) {
                (a.horiz && f || !a.horiz &&
                g) && a.setTickInterval(!0)
            }), this.getMargins(); this.drawChartBox(); this.hasCartesianSeries && m(a, function (a) { a.render() }); if (!this.seriesGroup) this.seriesGroup = b.g("series-group").attr({ zIndex: 3 }).add(); this.renderSeries(); this.renderLabels(); this.showCredits(c.credits); this.hasRendered = !0
        }, showCredits: function (a) { if (a.enabled && !this.credits) this.credits = this.renderer.text(a.text, 0, 0).on("click", function () { if (a.href) location.href = a.href }).attr({ align: a.position.align, zIndex: 8 }).css(a.style).add().align(a.position) },
        destroy: function () {
            var a = this, b = a.axes, c = a.series, d = a.container, e, f = d && d.parentNode; L(a, "destroy"); W[a.index] = q; Ta--; a.renderTo.removeAttribute("data-highcharts-chart"); Y(a); for (e = b.length; e--;) b[e] = b[e].destroy(); for (e = c.length; e--;) c[e] = c[e].destroy(); m("title,subtitle,chartBackground,plotBackground,plotBGImage,plotBorder,seriesGroup,clipRect,credits,pointer,scroller,rangeSelector,legend,resetZoomButton,tooltip,renderer".split(","), function (b) { var c = a[b]; c && c.destroy && (a[b] = c.destroy()) }); if (d) d.innerHTML =
            "", Y(d), f && La(d); for (e in a) delete a[e]
        }, isReadyToRender: function () { var a = this; return !ca && K == K.top && C.readyState !== "complete" || ga && !K.canvg ? (ga ? Gb.push(function () { a.firstRender() }, a.options.global.canvasToolsURL) : C.attachEvent("onreadystatechange", function () { C.detachEvent("onreadystatechange", a.firstRender); C.readyState === "complete" && a.firstRender() }), !1) : !0 }, firstRender: function () {
            var a = this, b = a.options, c = a.callback; if (a.isReadyToRender()) {
                a.getContainer(); L(a, "init"); a.resetMargins(); a.setChartSize();
                a.propFromSeries(); a.getAxes(); m(b.series || [], function (b) { a.initSeries(b) }); a.linkSeries(); L(a, "beforeRender"); if (z.Pointer) a.pointer = new Aa(a, b); a.render(); a.renderer.draw(); c && c.apply(a, [a]); m(a.callbacks, function (b) { a.index !== q && b.apply(a, [a]) }); L(a, "load"); a.cloneRenderTo(!0)
            }
        }, splashArray: function (a, b) { var c = b[a], c = ea(c) ? c : [c, c, c, c]; return [o(b[a + "Top"], c[0]), o(b[a + "Right"], c[1]), o(b[a + "Bottom"], c[2]), o(b[a + "Left"], c[3])] }
    }; var Z = function () { }; Z.prototype = {
        init: function (a, b, c) {
            this.series = a; this.color =
            a.color; this.applyOptions(b, c); this.pointAttr = {}; if (a.options.colorByPoint && (b = a.options.colors || a.chart.options.colors, this.color = this.color || b[a.colorCounter++], a.colorCounter === b.length)) a.colorCounter = 0; a.chart.pointCount++; return this
        }, applyOptions: function (a, b) { var c = this.series, d = c.options.pointValKey || c.pointValKey, a = Z.prototype.optionsToObject.call(this, a); s(this, a); this.options = this.options ? s(this.options, a) : a; if (d) this.y = this[d]; if (this.x === q && c) this.x = b === q ? c.autoIncrement() : b; return this },
        optionsToObject: function (a) { var b = {}, c = this.series, d = c.options.keys, e = d || c.pointArrayMap || ["y"], f = e.length, g = 0, h = 0; if (typeof a === "number" || a === null) b[e[0]] = a; else if (Ca(a)) { if (!d && a.length > f) { c = typeof a[0]; if (c === "string") b.name = a[0]; else if (c === "number") b.x = a[0]; g++ } for (; h < f;) b[e[h++]] = a[g++] } else if (typeof a === "object") { b = a; if (a.dataLabels) c._hasPointLabels = !0; if (a.marker) c._hasPointMarkers = !0 } return b }, destroy: function () {
            var a = this.series.chart, b = a.hoverPoints, c; a.pointCount--; if (b && (this.setState(),
            va(b, this), !b.length)) a.hoverPoints = null; if (this === a.hoverPoint) this.onMouseOut(); if (this.graphic || this.dataLabel) Y(this), this.destroyElements(); this.legendItem && a.legend.destroyItem(this); for (c in this) this[c] = null
        }, destroyElements: function () { for (var a = "graphic,dataLabel,dataLabelUpper,group,connector,shadowGroup".split(","), b, c = 6; c--;) b = a[c], this[b] && (this[b] = this[b].destroy()) }, getLabelConfig: function () {
            return {
                x: this.category, y: this.y, key: this.name || this.category, series: this.series, point: this,
                percentage: this.percentage, total: this.total || this.stackTotal
            }
        }, tooltipFormatter: function (a) { var b = this.series, c = b.tooltipOptions, d = o(c.valueDecimals, ""), e = c.valuePrefix || "", f = c.valueSuffix || ""; m(b.pointArrayMap || ["y"], function (b) { b = "{point." + b; if (e || f) a = a.replace(b + "}", e + b + "}" + f); a = a.replace(b + "}", b + ":,." + d + "f}") }); return Ja(a, { point: this, series: this.series }) }, firePointEvent: function (a, b, c) {
            var d = this, e = this.series.options; (e.point.events[a] || d.options && d.options.events && d.options.events[a]) && this.importEvents();
            a === "click" && e.allowPointSelect && (c = function (a) { d.select(null, a.ctrlKey || a.metaKey || a.shiftKey) }); L(this, a, b, c)
        }
    }; var R = z.Series = function () { }; R.prototype = {
        isCartesian: !0, type: "line", pointClass: Z, sorted: !0, requireSorting: !0, pointAttrToOptions: { stroke: "lineColor", "stroke-width": "lineWidth", fill: "fillColor", r: "radius" }, axisTypes: ["xAxis", "yAxis"], colorCounter: 0, parallelArrays: ["x", "y"], init: function (a, b) {
            var c = this, d, e, f = a.series, g = function (a, b) { return o(a.options.index, a._i) - o(b.options.index, b._i) };
            c.chart = a; c.options = b = c.setOptions(b); c.linkedSeries = []; c.bindAxes(); s(c, { name: b.name, state: "", pointAttr: {}, visible: b.visible !== !1, selected: b.selected === !0 }); if (ga) b.animation = !1; e = b.events; for (d in e) M(c, d, e[d]); if (e && e.click || b.point && b.point.events && b.point.events.click || b.allowPointSelect) a.runTrackerClick = !0; c.getColor(); c.getSymbol(); m(c.parallelArrays, function (a) { c[a + "Data"] = [] }); c.setData(b.data, !1); if (c.isCartesian) a.hasCartesianSeries = !0; f.push(c); c._i = f.length - 1; hb(f, g); this.yAxis && hb(this.yAxis.series,
            g); m(f, function (a, b) { a.index = b; a.name = a.name || "Series " + (b + 1) })
        }, bindAxes: function () { var a = this, b = a.options, c = a.chart, d; m(a.axisTypes || [], function (e) { m(c[e], function (c) { d = c.options; if (b[e] === d.index || b[e] !== q && b[e] === d.id || b[e] === q && d.index === 0) c.series.push(a), a[e] = c, c.isDirty = !0 }); !a[e] && a.optionalAxis !== e && ba(18, !0) }) }, updateParallelArrays: function (a, b) {
            var c = a.series, d = arguments; m(c.parallelArrays, typeof b === "number" ? function (d) { var f = d === "y" && c.toYData ? c.toYData(a) : a[d]; c[d + "Data"][b] = f } : function (a) {
                Array.prototype[b].apply(c[a +
                "Data"], Array.prototype.slice.call(d, 2))
            })
        }, autoIncrement: function () { var a = this.options, b = this.xIncrement, c, d = a.pointIntervalUnit, b = o(b, a.pointStart, 0); this.pointInterval = c = o(this.pointInterval, a.pointInterval, 1); if (d === "month" || d === "year") a = new Ga(b), a = d === "month" ? +a[zb](a[jb]() + c) : +a[Ab](a[kb]() + c), c = a - b; this.xIncrement = b + c; return b }, getSegments: function () {
            var a = -1, b = [], c, d = this.points, e = d.length; if (e) if (this.options.connectNulls) { for (c = e; c--;) d[c].y === null && d.splice(c, 1); d.length && (b = [d]) } else m(d,
            function (c, g) { c.y === null ? (g > a + 1 && b.push(d.slice(a + 1, g)), a = g) : g === e - 1 && b.push(d.slice(a + 1, g + 1)) }); this.segments = b
        }, setOptions: function (a) {
            var b = this.chart, c = b.options.plotOptions, b = b.userOptions || {}, d = b.plotOptions || {}, e = c[this.type]; this.userOptions = a; c = A(e, c.series, a); this.tooltipOptions = A(I.tooltip, I.plotOptions[this.type].tooltip, b.tooltip, d.series && d.series.tooltip, d[this.type] && d[this.type].tooltip, a.tooltip); e.marker === null && delete c.marker; this.zoneAxis = c.zoneAxis; a = this.zones = (c.zones || []).slice();
            if ((c.negativeColor || c.negativeFillColor) && !c.zones) a.push({ value: c[this.zoneAxis + "Threshold"] || c.threshold || 0, color: c.negativeColor, fillColor: c.negativeFillColor }); a.length && r(a[a.length - 1].value) && a.push({ color: this.color, fillColor: this.fillColor }); return c
        }, getCyclic: function (a, b, c) { var d = this.userOptions, e = "_" + a + "Index", f = a + "Counter"; b || (r(d[e]) ? b = d[e] : (d[e] = b = this.chart[f] % c.length, this.chart[f] += 1), b = c[b]); this[a] = b }, getColor: function () {
            this.options.colorByPoint || this.getCyclic("color", this.options.color ||
            U[this.type].color, this.chart.options.colors)
        }, getSymbol: function () { var a = this.options.marker; this.getCyclic("symbol", a.symbol, this.chart.options.symbols); if (/^url/.test(this.symbol)) a.radius = 0 }, drawLegendSymbol: Ya.drawLineMarker, setData: function (a, b, c, d) {
            var e = this, f = e.points, g = f && f.length || 0, h, i = e.options, j = e.chart, k = null, l = e.xAxis, n = l && !!l.categories, p = i.turboThreshold, v = this.xData, t = this.yData, s = (h = e.pointArrayMap) && h.length, a = a || []; h = a.length; b = o(b, !0); if (d !== !1 && h && g === h && !e.cropped && !e.hasGroupedData &&
            e.visible) m(a, function (a, b) { f[b].update(a, !1, null, !1) }); else {
                e.xIncrement = null; e.pointRange = n ? 1 : i.pointRange; e.colorCounter = 0; m(this.parallelArrays, function (a) { e[a + "Data"].length = 0 }); if (p && h > p) { for (c = 0; k === null && c < h;) k = a[c], c++; if (qa(k)) { n = o(i.pointStart, 0); i = o(i.pointInterval, 1); for (c = 0; c < h; c++) v[c] = n, t[c] = a[c], n += i; e.xIncrement = n } else if (Ca(k)) if (s) for (c = 0; c < h; c++) i = a[c], v[c] = i[0], t[c] = i.slice(1, s + 1); else for (c = 0; c < h; c++) i = a[c], v[c] = i[0], t[c] = i[1]; else ba(12) } else for (c = 0; c < h; c++) if (a[c] !== q && (i =
                { series: e }, e.pointClass.prototype.applyOptions.apply(i, [a[c]]), e.updateParallelArrays(i, c), n && i.name)) l.names[i.x] = i.name; Ba(t[0]) && ba(14, !0); e.data = []; e.options.data = a; for (c = g; c--;) f[c] && f[c].destroy && f[c].destroy(); if (l) l.minRange = l.userMinRange; e.isDirty = e.isDirtyData = j.isDirtyBox = !0; c = !1
            } b && j.redraw(c)
        }, processData: function (a) {
            var b = this.xData, c = this.yData, d = b.length, e; e = 0; var f, g, h = this.xAxis, i, j = this.options; i = j.cropThreshold; var k = this.isCartesian, l, n; if (k && !this.isDirty && !h.isDirty && !this.yAxis.isDirty &&
            !a) return !1; if (h) a = h.getExtremes(), l = a.min, n = a.max; if (k && this.sorted && (!i || d > i || this.forceCrop)) if (b[d - 1] < l || b[0] > n) b = [], c = []; else if (b[0] < l || b[d - 1] > n) e = this.cropData(this.xData, this.yData, l, n), b = e.xData, c = e.yData, e = e.start, f = !0; for (i = b.length - 1; i >= 0; i--) d = b[i] - b[i - 1], d > 0 && (g === q || d < g) ? g = d : d < 0 && this.requireSorting && ba(15); this.cropped = f; this.cropStart = e; this.processedXData = b; this.processedYData = c; if (j.pointRange === null) this.pointRange = g || 1; this.closestPointRange = g
        }, cropData: function (a, b, c, d) {
            var e =
            a.length, f = 0, g = e, h = o(this.cropShoulder, 1), i; for (i = 0; i < e; i++) if (a[i] >= c) { f = w(0, i - h); break } for (; i < e; i++) if (a[i] > d) { g = i + h; break } return { xData: a.slice(f, g), yData: b.slice(f, g), start: f, end: g }
        }, generatePoints: function () {
            var a = this.options.data, b = this.data, c, d = this.processedXData, e = this.processedYData, f = this.pointClass, g = d.length, h = this.cropStart || 0, i, j = this.hasGroupedData, k, l = [], n; if (!b && !j) b = [], b.length = a.length, b = this.data = b; for (n = 0; n < g; n++) i = h + n, j ? l[n] = (new f).init(this, [d[n]].concat(ra(e[n]))) : (b[i] ?
            k = b[i] : a[i] !== q && (b[i] = k = (new f).init(this, a[i], d[n])), l[n] = k), l[n].index = i; if (b && (g !== (c = b.length) || j)) for (n = 0; n < c; n++) if (n === h && !j && (n += g), b[n]) b[n].destroyElements(), b[n].plotX = q; this.data = b; this.points = l
        }, getExtremes: function (a) {
            var b = this.yAxis, c = this.processedXData, d, e = [], f = 0; d = this.xAxis.getExtremes(); var g = d.min, h = d.max, i, j, k, l, a = a || this.stackedYData || this.processedYData; d = a.length; for (l = 0; l < d; l++) if (j = c[l], k = a[l], i = k !== null && k !== q && (!b.isLog || k.length || k > 0), j = this.getExtremesFromAll || this.cropped ||
            (c[l + 1] || j) >= g && (c[l - 1] || j) <= h, i && j) if (i = k.length) for (; i--;) k[i] !== null && (e[f++] = k[i]); else e[f++] = k; this.dataMin = Ra(e); this.dataMax = Ea(e)
        }, translate: function () {
            this.processedXData || this.processData(); this.generatePoints(); for (var a = this.options, b = a.stacking, c = this.xAxis, d = c.categories, e = this.yAxis, f = this.points, g = f.length, h = !!this.modifyValue, i = a.pointPlacement, j = i === "between" || qa(i), k = a.threshold, l, n, p, m = Number.MAX_VALUE, a = 0; a < g; a++) {
                var t = f[a], s = t.x, B = t.y; n = t.low; var y = b && e.stacks[(this.negStacks &&
                B < k ? "-" : "") + this.stackKey]; if (e.isLog && B !== null && B <= 0) t.y = B = null, ba(10); t.plotX = l = c.translate(s, 0, 0, 0, 1, i, this.type === "flags"); if (b && this.visible && y && y[s]) y = y[s], B = y.points[this.index + "," + a], n = B[0], B = B[1], n === 0 && (n = o(k, e.min)), e.isLog && n <= 0 && (n = null), t.total = t.stackTotal = y.total, t.percentage = y.total && t.y / y.total * 100, t.stackY = B, y.setOffset(this.pointXOffset || 0, this.barW || 0); t.yBottom = r(n) ? e.translate(n, 0, 1, 0, 1) : null; h && (B = this.modifyValue(B, t)); t.plotY = n = typeof B === "number" && B !== Infinity ? N(w(-1E5,
                e.translate(B, 0, 1, 0, 1)), 1E5) : q; t.isInside = n !== q && n >= 0 && n <= e.len && l >= 0 && l <= c.len; t.clientX = j ? c.translate(s, 0, 0, 0, 1) : l; t.negative = t.y < (k || 0); t.category = d && d[t.x] !== q ? d[t.x] : t.x; a && (m = N(m, Q(l - p))); p = l
            } this.closestPointRangePx = m; this.getSegments()
        }, setClip: function (a) {
            var b = this.chart, c = b.renderer, d = b.inverted, e = this.clipBox, f = e || b.clipBox, g = this.sharedClipKey || ["_sharedClip", a && a.duration, a && a.easing, f.height].join(","), h = b[g], i = b[g + "m"]; if (!h) {
                if (a) f.width = 0, b[g + "m"] = i = c.clipRect(-99, d ? -b.plotLeft :
                -b.plotTop, 99, d ? b.chartWidth : b.chartHeight); b[g] = h = c.clipRect(f)
            } a && (h.count += 1); if (this.options.clip !== !1) this.group.clip(a || e ? h : b.clipRect), this.markerGroup.clip(i), this.sharedClipKey = g; a || (h.count -= 1, h.count <= 0 && g && b[g] && (e || (b[g] = b[g].destroy()), b[g + "m"] && (b[g + "m"] = b[g + "m"].destroy())))
        }, animate: function (a) {
            var b = this.chart, c = this.options.animation, d; if (c && !ea(c)) c = U[this.type].animation; a ? this.setClip(c) : (d = this.sharedClipKey, (a = b[d]) && a.animate({ width: b.plotSizeX }, c), b[d + "m"] && b[d + "m"].animate({
                width: b.plotSizeX +
                99
            }, c), this.animate = null)
        }, afterAnimate: function () { this.setClip(); L(this, "afterAnimate") }, drawPoints: function () {
            var a, b = this.points, c = this.chart, d, e, f, g, h, i, j, k, l = this.options.marker, n = this.pointAttr[""], p, m, t, r = this.markerGroup, B = o(l.enabled, this.xAxis.isRadial, this.closestPointRangePx > 2 * l.radius); if (l.enabled !== !1 || this._hasPointMarkers) for (f = b.length; f--;) if (g = b[f], d = fa(g.plotX), e = g.plotY, k = g.graphic, p = g.marker || {}, m = !!g.marker, a = B && p.enabled === q || p.enabled, t = g.isInside, a && e !== q && !isNaN(e) && g.y !==
            null) if (a = g.pointAttr[g.selected ? "select" : ""] || n, h = a.r, i = o(p.symbol, this.symbol), j = i.indexOf("url") === 0, k) k[t ? "show" : "hide"](!0).animate(s({ x: d - h, y: e - h }, k.symbolName ? { width: 2 * h, height: 2 * h } : {})); else { if (t && (h > 0 || j)) g.graphic = c.renderer.symbol(i, d - h, e - h, 2 * h, 2 * h, m ? p : l).attr(a).add(r) } else if (k) g.graphic = k.destroy()
        }, convertAttribs: function (a, b, c, d) { var e = this.pointAttrToOptions, f, g, h = {}, a = a || {}, b = b || {}, c = c || {}, d = d || {}; for (f in e) g = e[f], h[f] = o(a[g], b[f], c[f], d[f]); return h }, getAttribs: function () {
            var a =
            this, b = a.options, c = U[a.type].marker ? b.marker : b, d = c.states, e = d.hover, f, g = a.color, h = a.options.negativeColor; f = { stroke: g, fill: g }; var i = a.points || [], j, k = [], l, n = a.pointAttrToOptions; l = a.hasPointSpecificOptions; var p = c.lineColor, o = c.fillColor; j = b.turboThreshold; var t = a.zones, q = a.zoneAxis || "y", B; b.marker ? (e.radius = e.radius || c.radius + e.radiusPlus, e.lineWidth = e.lineWidth || c.lineWidth + e.lineWidthPlus) : (e.color = e.color || V(e.color || g).brighten(e.brightness).get(), e.negativeColor = e.negativeColor || V(e.negativeColor ||
            h).brighten(e.brightness).get()); k[""] = a.convertAttribs(c, f); m(["hover", "select"], function (b) { k[b] = a.convertAttribs(d[b], k[""]) }); a.pointAttr = k; g = i.length; if (!j || g < j || l) for (; g--;) {
                j = i[g]; if ((c = j.options && j.options.marker || j.options) && c.enabled === !1) c.radius = 0; if (t.length) { l = 0; for (f = t[l]; j[q] >= f.value;) f = t[++l]; j.color = j.fillColor = f.color } l = b.colorByPoint || j.color; if (j.options) for (B in n) r(c[n[B]]) && (l = !0); if (l) {
                    c = c || {}; l = []; d = c.states || {}; f = d.hover = d.hover || {}; if (!b.marker) f.color = f.color || !j.options.color &&
                    e[j.negative && h ? "negativeColor" : "color"] || V(j.color).brighten(f.brightness || e.brightness).get(); f = { color: j.color }; if (!o) f.fillColor = j.color; if (!p) f.lineColor = j.color; c.hasOwnProperty("color") && !c.color && delete c.color; l[""] = a.convertAttribs(s(f, c), k[""]); l.hover = a.convertAttribs(d.hover, k.hover, l[""]); l.select = a.convertAttribs(d.select, k.select, l[""])
                } else l = k; j.pointAttr = l
            }
        }, destroy: function () {
            var a = this, b = a.chart, c = /AppleWebKit\/533/.test(ya), d, e = a.data || [], f, g, h; L(a, "destroy"); Y(a); m(a.axisTypes ||
            [], function (b) { if (h = a[b]) va(h.series, a), h.isDirty = h.forceRedraw = !0 }); a.legendItem && a.chart.legend.destroyItem(a); for (d = e.length; d--;) (f = e[d]) && f.destroy && f.destroy(); a.points = null; clearTimeout(a.animationTimeout); for (g in a) a[g] instanceof P && !a[g].survive && (d = c && g === "group" ? "hide" : "destroy", a[g][d]()); if (b.hoverSeries === a) b.hoverSeries = null; va(b.series, a); for (g in a) delete a[g]
        }, getSegmentPath: function (a) {
            var b = this, c = [], d = b.options.step; m(a, function (e, f) {
                var g = e.plotX, h = e.plotY, i; b.getPointSpline ?
                c.push.apply(c, b.getPointSpline(a, e, f)) : (c.push(f ? "L" : "M"), d && f && (i = a[f - 1], d === "right" ? c.push(i.plotX, h) : d === "center" ? c.push((i.plotX + g) / 2, i.plotY, (i.plotX + g) / 2, h) : c.push(g, i.plotY)), c.push(e.plotX, e.plotY))
            }); return c
        }, getGraphPath: function () { var a = this, b = [], c, d = []; m(a.segments, function (e) { c = a.getSegmentPath(e); e.length > 1 ? b = b.concat(c) : d.push(e[0]) }); a.singlePoints = d; return a.graphPath = b }, drawGraph: function () {
            var a = this, b = this.options, c = [["graph", b.lineColor || this.color, b.dashStyle]], d = b.lineWidth,
            e = b.linecap !== "square", f = this.getGraphPath(), g = this.fillGraph && this.color || O; m(this.zones, function (d, e) { c.push(["zoneGraph" + e, d.color || a.color, d.dashStyle || b.dashStyle]) }); m(c, function (c, i) { var j = c[0], k = a[j]; if (k) Va(k), k.animate({ d: f }); else if ((d || g) && f.length) k = { stroke: c[1], "stroke-width": d, fill: g, zIndex: 1 }, c[2] ? k.dashstyle = c[2] : e && (k["stroke-linecap"] = k["stroke-linejoin"] = "round"), a[j] = a.chart.renderer.path(f).attr(k).add(a.group).shadow(i < 2 && b.shadow) })
        }, applyZones: function () {
            var a = this, b = this.chart,
            c = b.renderer, d = this.zones, e, f, g = this.clips || [], h, i = this.graph, j = this.area, k = w(b.chartWidth, b.chartHeight), l = this[(this.zoneAxis || "y") + "Axis"], n = l.reversed, p = l.horiz, v = !1; if (d.length && (i || j)) i && i.hide(), j && j.hide(), m(d, function (d, m) {
                e = o(f, n ? p ? b.plotWidth : 0 : p ? 0 : l.toPixels(l.min)); f = x(l.toPixels(o(d.value, l.max), !0)); e = l.isXAxis ? e > f ? f : e : e < f ? f : e; v && (e = f = l.toPixels(l.max)); if (l.isXAxis) { if (h = { x: n ? f : e, y: 0, width: Math.abs(e - f), height: k }, !p) h.x = b.plotHeight - h.x } else if (h = {
                        x: 0, y: n ? e : f, width: k, height: Math.abs(e -
                f)
                }, p) h.y = b.plotWidth - h.y; b.inverted && c.isVML && (h = l.isXAxis ? { x: 0, y: n ? e : f, height: h.width, width: b.chartWidth } : { x: h.y - b.plotLeft - b.spacingBox.x, y: 0, width: h.height, height: b.chartHeight }); g[m] ? g[m].animate(h) : (g[m] = c.clipRect(h), i && a["zoneGraph" + m].clip(g[m]), j && a["zoneArea" + m].clip(g[m])); v = d.value > l.max
            }), this.clips = g
        }, invertGroups: function () {
            function a() { var a = { width: b.yAxis.len, height: b.xAxis.len }; m(["group", "markerGroup"], function (c) { b[c] && b[c].attr(a).invert() }) } var b = this, c = b.chart; if (b.xAxis) M(c,
            "resize", a), M(b, "destroy", function () { Y(c, "resize", a) }), a(), b.invertGroups = a
        }, plotGroup: function (a, b, c, d, e) { var f = this[a], g = !f; g && (this[a] = f = this.chart.renderer.g(b).attr({ visibility: c, zIndex: d || 0.1 }).add(e)); f[g ? "attr" : "animate"](this.getPlotBox()); return f }, getPlotBox: function () { var a = this.chart, b = this.xAxis, c = this.yAxis; if (a.inverted) b = c, c = this.xAxis; return { translateX: b ? b.left : a.plotLeft, translateY: c ? c.top : a.plotTop, scaleX: 1, scaleY: 1 } }, render: function () {
            var a = this, b = a.chart, c, d = a.options, e = (c = d.animation) &&
            !!a.animate && b.renderer.isSVG && o(c.duration, 500) || 0, f = a.visible ? "visible" : "hidden", g = d.zIndex, h = a.hasRendered, i = b.seriesGroup; c = a.plotGroup("group", "series", f, g, i); a.markerGroup = a.plotGroup("markerGroup", "markers", f, g, i); e && a.animate(!0); a.getAttribs(); c.inverted = a.isCartesian ? b.inverted : !1; a.drawGraph && (a.drawGraph(), a.applyZones()); m(a.points, function (a) { a.redraw && a.redraw() }); a.drawDataLabels && a.drawDataLabels(); a.visible && a.drawPoints(); a.drawTracker && a.options.enableMouseTracking !== !1 && a.drawTracker();
            b.inverted && a.invertGroups(); d.clip !== !1 && !a.sharedClipKey && !h && c.clip(b.clipRect); e && a.animate(); if (!h) e ? a.animationTimeout = setTimeout(function () { a.afterAnimate() }, e) : a.afterAnimate(); a.isDirty = a.isDirtyData = !1; a.hasRendered = !0
        }, redraw: function () {
            var a = this.chart, b = this.isDirtyData, c = this.isDirty, d = this.group, e = this.xAxis, f = this.yAxis; d && (a.inverted && d.attr({ width: a.plotWidth, height: a.plotHeight }), d.animate({ translateX: o(e && e.left, a.plotLeft), translateY: o(f && f.top, a.plotTop) })); this.translate();
            this.render(); b && L(this, "updatedData"); (c || b) && delete this.kdTree
        }, kdDimensions: 1, kdTree: null, kdAxisArray: ["clientX", "plotY"], kdComparer: "distX", searchPoint: function (a) { var b = this.xAxis, c = this.yAxis, d = this.chart.inverted; return this.searchKDTree({ clientX: d ? b.len - a.chartY + b.pos : a.chartX - b.pos, plotY: d ? c.len - a.chartX + c.pos : a.chartY - c.pos }) }, buildKDTree: function () {
            function a(b, d, g) {
                var h, i; if (i = b && b.length) return h = c.kdAxisArray[d % g], b.sort(function (a, b) { return a[h] - b[h] }), i = Math.floor(i / 2), {
                    point: b[i],
                    left: a(b.slice(0, i), d + 1, g), right: a(b.slice(i + 1), d + 1, g)
                }
            } function b() { var b = pb(c.points, function (a) { return a.y !== null }); c.kdTree = a(b, d, d) } var c = this, d = c.kdDimensions; delete c.kdTree; c.options.kdSync ? b() : setTimeout(b)
        }, searchKDTree: function (a) {
            function b(a, h, i, j) {
                var k = h.point, l = c.kdAxisArray[i % j], n, p = k; n = r(a[e]) && r(k[e]) ? Math.pow(a[e] - k[e], 2) : null; var m = r(a[f]) && r(k[f]) ? Math.pow(a[f] - k[f], 2) : null, o = (n || 0) + (m || 0); n = {
                    distX: r(n) ? Math.sqrt(n) : Number.MAX_VALUE, distY: r(m) ? Math.sqrt(m) : Number.MAX_VALUE, distR: r(o) ?
                    Math.sqrt(o) : Number.MAX_VALUE
                }; k.dist = n; l = a[l] - k[l]; n = l < 0 ? "left" : "right"; h[n] && (n = b(a, h[n], i + 1, j), p = n.dist[d] < p.dist[d] ? n : k, k = l < 0 ? "right" : "left", h[k] && Math.sqrt(l * l) < p.dist[d] && (a = b(a, h[k], i + 1, j), p = a.dist[d] < p.dist[d] ? a : p)); return p
            } var c = this, d = this.kdComparer, e = this.kdAxisArray[0], f = this.kdAxisArray[1]; this.kdTree || this.buildKDTree(); if (this.kdTree) return b(a, this.kdTree, this.kdDimensions, this.kdDimensions)
        }
    }; s(ma.prototype, {
        addSeries: function (a, b, c) {
            var d, e = this; a && (b = o(b, !0), L(e, "addSeries", { options: a },
            function () { d = e.initSeries(a); e.isDirtyLegend = !0; e.linkSeries(); b && e.redraw(c) })); return d
        }, addAxis: function (a, b, c, d) { var e = b ? "xAxis" : "yAxis", f = this.options; new X(this, A(a, { index: this[e].length, isX: b })); f[e] = ra(f[e] || {}); f[e].push(a); o(c, !0) && this.redraw(d) }, showLoading: function (a) {
            var b = this, c = b.options, d = b.loadingDiv, e = c.loading, f = function () { d && F(d, { left: b.plotLeft + "px", top: b.plotTop + "px", width: b.plotWidth + "px", height: b.plotHeight + "px" }) }; if (!d) b.loadingDiv = d = $(Fa, { className: "highcharts-loading" },
            s(e.style, { zIndex: 10, display: O }), b.container), b.loadingSpan = $("span", null, e.labelStyle, d), M(b, "redraw", f); b.loadingSpan.innerHTML = a || c.lang.loading; if (!b.loadingShown) F(d, { opacity: 0, display: "" }), fb(d, { opacity: e.style.opacity }, { duration: e.showDuration || 0 }), b.loadingShown = !0; f()
        }, hideLoading: function () { var a = this.options, b = this.loadingDiv; b && fb(b, { opacity: 0 }, { duration: a.loading.hideDuration || 100, complete: function () { F(b, { display: O }) } }); this.loadingShown = !1 }
    }); s(Z.prototype, {
        update: function (a, b, c, d) {
            function e() {
                f.applyOptions(a);
                if (ea(a) && !Ca(a)) f.redraw = function () { if (h) a && a.marker && a.marker.symbol ? f.graphic = h.destroy() : h.attr(f.pointAttr[f.state || ""]); if (a && a.dataLabels && f.dataLabel) f.dataLabel = f.dataLabel.destroy(); f.redraw = null }; i = f.index; g.updateParallelArrays(f, i); if (l && f.name) l[f.x] = f.name; k.data[i] = f.options; g.isDirty = g.isDirtyData = !0; if (!g.fixedBox && g.hasCartesianSeries) j.isDirtyBox = !0; j.legend.display && k.legendType === "point" && (g.updateTotals(), j.legend.clearItems()); b && j.redraw(c)
            } var f = this, g = f.series, h = f.graphic,
            i, j = g.chart, k = g.options, l = g.xAxis && g.xAxis.names, b = o(b, !0); d === !1 ? e() : f.firePointEvent("update", { options: a }, e)
        }, remove: function (a, b) { this.series.removePoint(Ua(this, this.series.data), a, b) }
    }); s(R.prototype, {
        addPoint: function (a, b, c, d) {
            var e = this, f = e.options, g = e.data, h = e.graph, i = e.area, j = e.chart, k = e.xAxis && e.xAxis.names, l = h && h.shift || 0, n = ["graph", "area"], h = f.data, p, v = e.xData; ha = o(d, j.animation); if (c) { for (d = e.zones.length; d--;) n.push("zoneGraph" + d, "zoneArea" + d); m(n, function (a) { if (e[a]) e[a].shift = l + 1 }) } if (i) i.isArea =
            !0; b = o(b, !0); i = { series: e }; e.pointClass.prototype.applyOptions.apply(i, [a]); n = i.x; d = v.length; if (e.requireSorting && n < v[d - 1]) for (p = !0; d && v[d - 1] > n;) d--; e.updateParallelArrays(i, "splice", d, 0, 0); e.updateParallelArrays(i, d); if (k && i.name) k[n] = i.name; h.splice(d, 0, a); p && (e.data.splice(d, 0, null), e.processData()); f.legendType === "point" && e.generatePoints(); c && (g[0] && g[0].remove ? g[0].remove(!1) : (g.shift(), e.updateParallelArrays(i, "shift"), h.shift())); e.isDirty = !0; e.isDirtyData = !0; b && (e.getAttribs(), j.redraw())
        },
        removePoint: function (a, b, c) { var d = this, e = d.data, f = e[a], g = d.points, h = d.chart, i = function () { e.length === g.length && g.splice(a, 1); e.splice(a, 1); d.options.data.splice(a, 1); d.updateParallelArrays(f || { series: d }, "splice", a, 1); f && f.destroy(); d.isDirty = !0; d.isDirtyData = !0; b && h.redraw() }; ha = o(c, h.animation); b = o(b, !0); f ? f.firePointEvent("remove", null, i) : i() }, remove: function (a, b) {
            var c = this, d = c.chart, a = o(a, !0); if (!c.isRemoving) c.isRemoving = !0, L(c, "remove", null, function () {
                c.destroy(); d.isDirtyLegend = d.isDirtyBox =
                !0; d.linkSeries(); a && d.redraw(b)
            }); c.isRemoving = !1
        }, update: function (a, b) {
            var c = this, d = this.chart, e = this.userOptions, f = this.type, g = u[f].prototype, h = ["group", "markerGroup", "dataLabelsGroup"], i; if (a.type && a.type !== f || a.zIndex !== void 0) h.length = 0; m(h, function (a) { h[a] = c[a]; delete c[a] }); a = A(e, { animation: !1, index: this.index, pointStart: this.xData[0] }, { data: this.options.data }, a); this.remove(!1); for (i in g) this[i] = q; s(this, u[a.type || f].prototype); m(h, function (a) { c[a] = h[a] }); this.init(d, a); d.linkSeries(); o(b,
            !0) && d.redraw(!1)
        }
    }); s(X.prototype, {
        update: function (a, b) { var c = this.chart, a = c.options[this.coll][this.options.index] = A(this.userOptions, a); this.destroy(!0); this._addedPlotLB = q; this.init(c, s(a, { events: q })); c.isDirtyBox = !0; o(b, !0) && c.redraw() }, remove: function (a) { for (var b = this.chart, c = this.coll, d = this.series, e = d.length; e--;) d[e] && d[e].remove(!1); va(b.axes, this); va(b[c], this); b.options[c].splice(this.options.index, 1); m(b[c], function (a, b) { a.options.index = b }); this.destroy(); b.isDirtyBox = !0; o(a, !0) && b.redraw() },
        setTitle: function (a, b) { this.update({ title: a }, b) }, setCategories: function (a, b) { this.update({ categories: a }, b) }
    }); E = aa(R); u.line = E; U.column = A(ta, { borderColor: "#FFFFFF", borderRadius: 0, groupPadding: 0.2, marker: null, pointPadding: 0.1, minPointLength: 0, cropThreshold: 50, pointRange: null, states: { hover: { brightness: 0.1, shadow: !1, halo: !1 }, select: { color: "#C0C0C0", borderColor: "#000000", shadow: !1 } }, dataLabels: { align: null, verticalAlign: null, y: null }, stickyTracking: !1, tooltip: { distance: 6 }, threshold: 0 }); E = aa(R, {
        type: "column",
        pointAttrToOptions: { stroke: "borderColor", fill: "color", r: "borderRadius" }, cropShoulder: 0, directTouch: !0, trackerGroups: ["group", "dataLabelsGroup"], negStacks: !0, init: function () { R.prototype.init.apply(this, arguments); var a = this, b = a.chart; b.hasRendered && m(b.series, function (b) { if (b.type === a.type) b.isDirty = !0 }) }, getColumnMetrics: function () {
            var a = this, b = a.options, c = a.xAxis, d = a.yAxis, e = c.reversed, f, g = {}, h, i = 0; b.grouping === !1 ? i = 1 : m(a.chart.series, function (b) {
                var c = b.options, e = b.yAxis; if (b.type === a.type && b.visible &&
                d.len === e.len && d.pos === e.pos) c.stacking ? (f = b.stackKey, g[f] === q && (g[f] = i++), h = g[f]) : c.grouping !== !1 && (h = i++), b.columnIndex = h
            }); var c = N(Q(c.transA) * (c.ordinalSlope || b.pointRange || c.closestPointRange || c.tickInterval || 1), c.len), j = c * b.groupPadding, k = (c - 2 * j) / i, l = b.pointWidth, b = r(l) ? (k - l) / 2 : k * b.pointPadding, l = o(l, k - 2 * b); return a.columnMetrics = { width: l, offset: b + (j + ((e ? i - (a.columnIndex || 0) : a.columnIndex) || 0) * k - c / 2) * (e ? -1 : 1) }
        }, translate: function () {
            var a = this, b = a.chart, c = a.options, d = a.borderWidth = o(c.borderWidth,
            a.closestPointRange * a.xAxis.transA < 2 ? 0 : 1), e = a.yAxis, f = a.translatedThreshold = e.getThreshold(c.threshold), g = o(c.minPointLength, 5), h = a.getColumnMetrics(), i = h.width, j = a.barW = w(i, 1 + 2 * d), k = a.pointXOffset = h.offset, l = -(d % 2 ? 0.5 : 0), n = d % 2 ? 0.5 : 1; b.inverted && (f -= 0.5, b.renderer.isVML && (n += 1)); c.pointPadding && (j = sa(j)); R.prototype.translate.apply(a); m(a.points, function (c) {
                var d = o(c.yBottom, f), h = N(w(-999 - d, c.plotY), e.len + 999 + d), m = c.plotX + k, q = j, s = N(h, d), r, u; r = w(h, d) - s; Q(r) < g && g && (r = g, u = !e.reversed && !c.negative || e.reversed &&
                c.negative, s = x(Q(s - f) > g ? d - g : f - (u ? g : 0))); c.barX = m; c.pointWidth = i; c.tooltipPos = b.inverted ? [e.len + e.pos - b.plotLeft - h, a.xAxis.len - m - q / 2] : [m + q / 2, h + e.pos - b.plotTop]; q = x(m + q) + l; m = x(m) + l; q -= m; d = Q(s) < 0.5; r = N(x(s + r) + n, 9E4); s = x(s) + n; r -= s; d && (s -= 1, r += 1); c.shapeType = "rect"; c.shapeArgs = { x: m, y: s, width: q, height: r }
            })
        }, getSymbol: T, drawLegendSymbol: Ya.drawRectangle, drawGraph: T, drawPoints: function () {
            var a = this, b = this.chart, c = a.options, d = b.renderer, e = c.animationLimit || 250, f, g; m(a.points, function (h) {
                var i = h.plotY, j = h.graphic;
                if (i !== q && !isNaN(i) && h.y !== null) f = h.shapeArgs, i = r(a.borderWidth) ? { "stroke-width": a.borderWidth } : {}, g = h.pointAttr[h.selected ? "select" : ""] || a.pointAttr[""], j ? (Va(j), j.attr(i)[b.pointCount < e ? "animate" : "attr"](A(f))) : h.graphic = d[h.shapeType](f).attr(i).attr(g).add(a.group).shadow(c.shadow, null, c.stacking && !c.borderRadius); else if (j) h.graphic = j.destroy()
            })
        }, animate: function (a) {
            var b = this.yAxis, c = this.options, d = this.chart.inverted, e = {}; if (ca) a ? (e.scaleY = 0.001, a = N(b.pos + b.len, w(b.pos, b.toPixels(c.threshold))),
            d ? e.translateX = a - b.len : e.translateY = a, this.group.attr(e)) : (e.scaleY = 1, e[d ? "translateX" : "translateY"] = b.pos, this.group.animate(e, this.options.animation), this.animate = null)
        }, remove: function () { var a = this, b = a.chart; b.hasRendered && m(b.series, function (b) { if (b.type === a.type) b.isDirty = !0 }); R.prototype.remove.apply(a, arguments) }
    }); u.column = E; U.scatter = A(ta, {
        lineWidth: 0, marker: { enabled: !0 }, tooltip: {
            headerFormat: '<span style="color:{series.color}">\u25CF</span> <span style="font-size: 10px;"> {series.name}</span><br/>',
            pointFormat: "x: <b>{point.x}</b><br/>y: <b>{point.y}</b><br/>"
        }
    }); ta = aa(R, { type: "scatter", sorted: !1, requireSorting: !1, noSharedTooltip: !0, trackerGroups: ["group", "markerGroup", "dataLabelsGroup"], takeOrdinalPosition: !1, kdDimensions: 2, kdComparer: "distR", drawGraph: function () { this.options.lineWidth && R.prototype.drawGraph.call(this) } }); u.scatter = ta; R.prototype.drawDataLabels = function () {
        var a = this, b = a.options, c = b.cursor, d = b.dataLabels, e = a.points, f, g, h = a.hasRendered || 0, i, j, k = a.chart.renderer; if (d.enabled ||
        a._hasPointLabels) a.dlProcessOptions && a.dlProcessOptions(d), j = a.plotGroup("dataLabelsGroup", "data-labels", d.defer ? "hidden" : "visible", d.zIndex || 6), o(d.defer, !0) && (j.attr({ opacity: +h }), h || M(a, "afterAnimate", function () { a.visible && j.show(); j[b.animation ? "animate" : "attr"]({ opacity: 1 }, { duration: 200 }) })), g = d, m(e, function (e) {
            var h, m = e.dataLabel, v, t, w = e.connector, B = !0, y, x = {}; f = e.dlOptions || e.options && e.options.dataLabels; h = o(f && f.enabled, g.enabled); if (m && !h) e.dataLabel = m.destroy(); else if (h) {
                d = A(g, f); y = d.style;
                h = d.rotation; v = e.getLabelConfig(); i = d.format ? Ja(d.format, v) : d.formatter.call(v, d); y.color = o(d.color, y.color, a.color, "black"); if (m) if (r(i)) m.attr({ text: i }), B = !1; else { if (e.dataLabel = m = m.destroy(), w) e.connector = w.destroy() } else if (r(i)) {
                    m = { fill: d.backgroundColor, stroke: d.borderColor, "stroke-width": d.borderWidth, r: d.borderRadius || 0, rotation: h, padding: d.padding, zIndex: 1 }; if (y.color === "contrast") x.color = d.inside || d.distance < 0 || b.stacking ? k.getContrast(e.color || a.color) : "#000000"; if (c) x.cursor = c; for (t in m) m[t] ===
                    q && delete m[t]; m = e.dataLabel = k[h ? "text" : "label"](i, 0, -999, d.shape, null, null, d.useHTML).attr(m).css(s(y, x)).add(j).shadow(d.shadow)
                } m && a.alignDataLabel(e, m, d, null, B)
            }
        })
    }; R.prototype.alignDataLabel = function (a, b, c, d, e) {
        var f = this.chart, g = f.inverted, h = o(a.plotX, -999), i = o(a.plotY, -999), j = b.getBBox(), k = f.renderer.fontMetrics(c.style.fontSize).b, l = this.visible && (a.series.forceDL || f.isInsidePlot(h, x(i), g) || d && f.isInsidePlot(h, g ? d.x + 1 : d.y + d.height - 1, g)); if (l) d = s({
            x: g ? f.plotWidth - i : h, y: x(g ? f.plotHeight - h : i),
            width: 0, height: 0
        }, d), s(c, { width: j.width, height: j.height }), c.rotation ? (a = f.renderer.rotCorr(k, c.rotation), b[e ? "attr" : "animate"]({ x: d.x + c.x + d.width / 2 + a.x, y: d.y + c.y + d.height / 2 }).attr({ align: c.align })) : (b.align(c, null, d), g = b.alignAttr, o(c.overflow, "justify") === "justify" ? this.justifyDataLabel(b, c, g, j, d, e) : o(c.crop, !0) && (l = f.isInsidePlot(g.x, g.y) && f.isInsidePlot(g.x + j.width, g.y + j.height)), c.shape && b.attr({ anchorX: a.plotX, anchorY: a.plotY })); if (!l) b.attr({ y: -999 }), b.placed = !1
    }; R.prototype.justifyDataLabel =
    function (a, b, c, d, e, f) { var g = this.chart, h = b.align, i = b.verticalAlign, j, k, l = a.box ? 0 : a.padding || 0; j = c.x + l; if (j < 0) h === "right" ? b.align = "left" : b.x = -j, k = !0; j = c.x + d.width - l; if (j > g.plotWidth) h === "left" ? b.align = "right" : b.x = g.plotWidth - j, k = !0; j = c.y + l; if (j < 0) i === "bottom" ? b.verticalAlign = "top" : b.y = -j, k = !0; j = c.y + d.height - l; if (j > g.plotHeight) i === "top" ? b.verticalAlign = "bottom" : b.y = g.plotHeight - j, k = !0; if (k) a.placed = !f, a.align(b, null, e) }; if (u.pie) u.pie.prototype.drawDataLabels = function () {
        var a = this, b = a.data, c, d = a.chart,
        e = a.options.dataLabels, f = o(e.connectorPadding, 10), g = o(e.connectorWidth, 1), h = d.plotWidth, i = d.plotHeight, j, k, l = o(e.softConnector, !0), n = e.distance, p = a.center, q = p[2] / 2, t = p[1], s = n > 0, r, y, u, A = [[], []], z, C, D, E, G, F = [0, 0, 0, 0], L = function (a, b) { return b.y - a.y }; if (a.visible && (e.enabled || a._hasPointLabels)) {
            R.prototype.drawDataLabels.apply(a); m(b, function (a) { a.dataLabel && a.visible && A[a.half].push(a) }); for (E = 2; E--;) {
                var J = [], M = [], I = A[E], K = I.length, H; if (K) {
                    a.sortByAngle(I, E - 0.5); for (G = b = 0; !b && I[G];) b = I[G] && I[G].dataLabel &&
                    (I[G].dataLabel.getBBox().height || 21), G++; if (n > 0) { y = N(t + q + n, d.plotHeight); for (G = w(0, t - q - n) ; G <= y; G += b) J.push(G); y = J.length; if (K > y) { c = [].concat(I); c.sort(L); for (G = K; G--;) c[G].rank = G; for (G = K; G--;) I[G].rank >= y && I.splice(G, 1); K = I.length } for (G = 0; G < K; G++) { c = I[G]; u = c.labelPos; c = 9999; var P, O; for (O = 0; O < y; O++) P = Q(J[O] - u[1]), P < c && (c = P, H = O); if (H < G && J[G] !== null) H = G; else for (y < K - G + H && J[G] !== null && (H = y - K + G) ; J[H] === null;) H++; M.push({ i: H, y: J[H] }); J[H] = null } M.sort(L) } for (G = 0; G < K; G++) {
                        c = I[G]; u = c.labelPos; r = c.dataLabel;
                        D = c.visible === !1 ? "hidden" : "inherit"; c = u[1]; if (n > 0) { if (y = M.pop(), H = y.i, C = y.y, c > C && J[H + 1] !== null || c < C && J[H - 1] !== null) C = N(w(0, c), d.plotHeight) } else C = c; z = e.justify ? p[0] + (E ? -1 : 1) * (q + n) : a.getX(C === t - q - n || C === t + q + n ? c : C, E); r._attr = { visibility: D, align: u[6] }; r._pos = { x: z + e.x + ({ left: f, right: -f }[u[6]] || 0), y: C + e.y - 10 }; r.connX = z; r.connY = C; if (this.options.size === null) y = r.width, z - y < f ? F[3] = w(x(y - z + f), F[3]) : z + y > h - f && (F[1] = w(x(z + y - h + f), F[1])), C - b / 2 < 0 ? F[0] = w(x(-C + b / 2), F[0]) : C + b / 2 > i && (F[2] = w(x(C + b / 2 - i), F[2]))
                    }
                }
            } if (Ea(F) ===
            0 || this.verifyDataLabelOverflow(F)) this.placeDataLabels(), s && g && m(this.points, function (b) {
                j = b.connector; u = b.labelPos; if ((r = b.dataLabel) && r._pos) D = r._attr.visibility, z = r.connX, C = r.connY, k = l ? ["M", z + (u[6] === "left" ? 5 : -5), C, "C", z, C, 2 * u[2] - u[4], 2 * u[3] - u[5], u[2], u[3], "L", u[4], u[5]] : ["M", z + (u[6] === "left" ? 5 : -5), C, "L", u[2], u[3], "L", u[4], u[5]], j ? (j.animate({ d: k }), j.attr("visibility", D)) : b.connector = j = a.chart.renderer.path(k).attr({ "stroke-width": g, stroke: e.connectorColor || b.color || "#606060", visibility: D }).add(a.dataLabelsGroup);
                else if (j) b.connector = j.destroy()
            })
        }
    }, u.pie.prototype.placeDataLabels = function () { m(this.points, function (a) { var a = a.dataLabel, b; if (a) (b = a._pos) ? (a.attr(a._attr), a[a.moved ? "animate" : "attr"](b), a.moved = !0) : a && a.attr({ y: -999 }) }) }, u.pie.prototype.alignDataLabel = T, u.pie.prototype.verifyDataLabelOverflow = function (a) {
        var b = this.center, c = this.options, d = c.center, e = c = c.minSize || 80, f; d[0] !== null ? e = w(b[2] - w(a[1], a[3]), c) : (e = w(b[2] - a[1] - a[3], c), b[0] += (a[3] - a[1]) / 2); d[1] !== null ? e = w(N(e, b[2] - w(a[0], a[2])), c) : (e =
        w(N(e, b[2] - a[0] - a[2]), c), b[1] += (a[0] - a[2]) / 2); e < b[2] ? (b[2] = e, this.translate(b), m(this.points, function (a) { if (a.dataLabel) a.dataLabel._pos = null }), this.drawDataLabels && this.drawDataLabels()) : f = !0; return f
    }; if (u.column) u.column.prototype.alignDataLabel = function (a, b, c, d, e) {
        var f = this.chart.inverted, g = a.series, h = a.dlBox || a.shapeArgs, i = a.below || a.plotY > o(this.translatedThreshold, g.yAxis.len), j = o(c.inside, !!this.options.stacking); if (h && (d = A(h), f && (d = {
            x: g.yAxis.len - d.y - d.height, y: g.xAxis.len - d.x - d.width, width: d.height,
            height: d.width
        }), !j)) f ? (d.x += i ? 0 : d.width, d.width = 0) : (d.y += i ? d.height : 0, d.height = 0); c.align = o(c.align, !f || j ? "center" : i ? "right" : "left"); c.verticalAlign = o(c.verticalAlign, f || j ? "middle" : i ? "top" : "bottom"); R.prototype.alignDataLabel.call(this, a, b, c, d, e)
    }; (function (a) {
        var b = a.Chart, c = a.each, d = HighchartsAdapter.addEvent; b.prototype.callbacks.push(function (a) {
            function b() {
                var d = []; c(a.series, function (a) {
                    var b = a.options.dataLabels; (b.enabled || a._hasPointLabels) && !b.allowOverlap && a.visible && c(a.points, function (a) {
                        if (a.dataLabel) a.dataLabel.labelrank =
                        a.labelrank, d.push(a.dataLabel)
                    })
                }); a.hideOverlappingLabels(d)
            } b(); d(a, "redraw", b)
        }); b.prototype.hideOverlappingLabels = function (a) {
            var b = a.length, c, d, i, j; for (d = 0; d < b; d++) if (c = a[d]) c.oldOpacity = c.opacity, c.newOpacity = 1; for (d = 0; d < b; d++) {
                i = a[d]; for (c = d + 1; c < b; ++c) if (j = a[c], i && j && i.placed && j.placed && i.newOpacity !== 0 && j.newOpacity !== 0 && !(j.alignAttr.x > i.alignAttr.x + i.width || j.alignAttr.x + j.width < i.alignAttr.x || j.alignAttr.y > i.alignAttr.y + i.height || j.alignAttr.y + j.height < i.alignAttr.y)) (i.labelrank < j.labelrank ?
                        i : j).newOpacity = 0
            } for (d = 0; d < b; d++) if (c = a[d]) { if (c.oldOpacity !== c.newOpacity && c.placed) c.alignAttr.opacity = c.newOpacity, c[c.isOld && c.newOpacity ? "animate" : "attr"](c.alignAttr); c.isOld = !0 }
        }
    })(z); da(X.prototype, "getSeriesExtremes", function (a) {
        var b = this.isXAxis, c, d, e = [], f; b && m(this.series, function (a, b) { if (a.useMapGeometry) e[b] = a.xData, a.xData = [] }); a.call(this); if (b && (c = o(this.dataMin, Number.MAX_VALUE), d = o(this.dataMax, -Number.MAX_VALUE), m(this.series, function (a, b) {
        if (a.useMapGeometry) c = Math.min(c, o(a.minX,
        c)), d = Math.max(d, o(a.maxX, c)), a.xData = e[b], f = !0
        }), f)) this.dataMin = c, this.dataMax = d
    }); da(X.prototype, "setAxisTranslation", function (a) {
        var b = this.chart, c = b.plotWidth / b.plotHeight, b = b.xAxis[0], d; a.call(this); this.coll === "yAxis" && b.transA !== q && m(this.series, function (a) { a.preserveAspectRatio && (d = !0) }); if (d && (this.transA = b.transA = Math.min(this.transA, b.transA), a = c / ((b.max - b.min) / (this.max - this.min)), a = a < 1 ? this : b, c = (a.max - a.min) * a.transA, a.pixelPadding = a.len - c, a.minPixelPadding = a.pixelPadding / 2, c = a.fixTo)) {
            c =
            c[1] - a.toValue(c[0], !0); c *= a.transA; if (Math.abs(c) > a.minPixelPadding || a.min === a.dataMin && a.max === a.dataMax) c = 0; a.minPixelPadding -= c
        }
    }); da(X.prototype, "render", function (a) { a.call(this); this.fixTo = null }); var Za = z.ColorAxis = function () { this.isColorAxis = !0; this.init.apply(this, arguments) }; s(Za.prototype, X.prototype); s(Za.prototype, {
        defaultColorAxisOptions: {
            lineWidth: 0, gridLineWidth: 1, tickPixelInterval: 72, startOnTick: !0, endOnTick: !0, offset: 0, marker: { animation: { duration: 50 }, color: "gray", width: 0.01 }, labels: { overflow: "justify" },
            minColor: "#EFEFFF", maxColor: "#003875", tickLength: 5
        }, init: function (a, b) { var c = a.options.legend.layout !== "vertical", d; d = A(this.defaultColorAxisOptions, { side: c ? 2 : 1, reversed: !c }, b, { isX: c, opposite: !c, showEmpty: !1, title: null, isColor: !0 }); X.prototype.init.call(this, a, d); b.dataClasses && this.initDataClasses(b); this.initStops(b); this.isXAxis = !0; this.horiz = c; this.zoomEnabled = !1 }, tweenColors: function (a, b, c) {
            var d; !b.rgba.length || !a.rgba.length ? a = b.raw || "none" : (a = a.rgba, b = b.rgba, d = b[3] !== 1 || a[3] !== 1, a = (d ? "rgba(" :
            "rgb(") + Math.round(b[0] + (a[0] - b[0]) * (1 - c)) + "," + Math.round(b[1] + (a[1] - b[1]) * (1 - c)) + "," + Math.round(b[2] + (a[2] - b[2]) * (1 - c)) + (d ? "," + (b[3] + (a[3] - b[3]) * (1 - c)) : "") + ")"); return a
        }, initDataClasses: function (a) {
            var b = this, c = this.chart, d, e = 0, f = this.options, g = a.dataClasses.length; this.dataClasses = d = []; this.legendItems = []; m(a.dataClasses, function (a, i) {
                var j, a = A(a); d.push(a); if (!a.color) f.dataClassColor === "category" ? (j = c.options.colors, a.color = j[e++], e === j.length && (e = 0)) : a.color = b.tweenColors(V(f.minColor), V(f.maxColor),
                g < 2 ? 0.5 : i / (g - 1))
            })
        }, initStops: function (a) { this.stops = a.stops || [[0, this.options.minColor], [1, this.options.maxColor]]; m(this.stops, function (a) { a.color = V(a[1]) }) }, setOptions: function (a) { X.prototype.setOptions.call(this, a); this.options.crosshair = this.options.marker; this.coll = "colorAxis" }, setAxisSize: function () {
            var a = this.legendSymbol, b = this.chart, c, d, e; if (a) this.left = c = a.attr("x"), this.top = d = a.attr("y"), this.width = e = a.attr("width"), this.height = a = a.attr("height"), this.right = b.chartWidth - c - e, this.bottom =
            b.chartHeight - d - a, this.len = this.horiz ? e : a, this.pos = this.horiz ? c : d
        }, toColor: function (a, b) { var c, d = this.stops, e, f = this.dataClasses, g, h; if (f) for (h = f.length; h--;) { if (g = f[h], e = g.from, d = g.to, (e === q || a >= e) && (d === q || a <= d)) { c = g.color; if (b) b.dataClass = h; break } } else { this.isLog && (a = this.val2lin(a)); c = 1 - (this.max - a) / (this.max - this.min || 1); for (h = d.length; h--;) if (c > d[h][0]) break; e = d[h] || d[h + 1]; d = d[h + 1] || e; c = 1 - (d[0] - c) / (d[0] - e[0] || 1); c = this.tweenColors(e.color, d.color, c) } return c }, getOffset: function () {
            var a = this.legendGroup,
            b = this.chart.axisOffset[this.side]; if (a) { X.prototype.getOffset.call(this); if (!this.axisGroup.parentGroup) this.axisGroup.add(a), this.gridGroup.add(a), this.labelGroup.add(a), this.added = !0, this.labelLeft = 0, this.labelRight = this.width; this.chart.axisOffset[this.side] = b }
        }, setLegendColor: function () { var a, b = this.options; a = this.reversed; a = this.horiz ? [+a, 0, +!a, 0] : [0, +!a, 0, +a]; this.legendColor = { linearGradient: { x1: a[0], y1: a[1], x2: a[2], y2: a[3] }, stops: b.stops || [[0, b.minColor], [1, b.maxColor]] } }, drawLegendSymbol: function (a,
        b) { var c = a.padding, d = a.options, e = this.horiz, f = o(d.symbolWidth, e ? 200 : 12), g = o(d.symbolHeight, e ? 12 : 200), h = o(d.labelPadding, e ? 16 : 30), d = o(d.itemDistance, 10); this.setLegendColor(); b.legendSymbol = this.chart.renderer.rect(0, a.baseline - 11, f, g).attr({ zIndex: 1 }).add(b.legendGroup); b.legendSymbol.getBBox(); this.legendItemWidth = f + c + (e ? d : h); this.legendItemHeight = g + c + (e ? h : 0) }, setState: T, visible: !0, setVisible: T, getSeriesExtremes: function () {
            var a; if (this.series.length) a = this.series[0], this.dataMin = a.valueMin, this.dataMax =
            a.valueMax
        }, drawCrosshair: function (a, b) { var c = b && b.plotX, d = b && b.plotY, e, f = this.pos, g = this.len; if (b) e = this.toPixels(b[b.series.colorKey]), e < f ? e = f - 2 : e > f + g && (e = f + g + 2), b.plotX = e, b.plotY = this.len - e, X.prototype.drawCrosshair.call(this, a, b), b.plotX = c, b.plotY = d, this.cross && this.cross.attr({ fill: this.crosshair.color }).add(this.legendGroup) }, getPlotLinePath: function (a, b, c, d, e) {
            return typeof e === "number" ? this.horiz ? ["M", e - 4, this.top - 6, "L", e + 4, this.top - 6, e, this.top, "Z"] : ["M", this.left, e, "L", this.left - 6, e + 6, this.left -
            6, e - 6, "Z"] : X.prototype.getPlotLinePath.call(this, a, b, c, d)
        }, update: function (a, b) { m(this.series, function (a) { a.isDirtyData = !0 }); X.prototype.update.call(this, a, b); this.legendItem && (this.setLegendColor(), this.chart.legend.colorizeItem(this, !0)) }, getDataClassLegendSymbols: function () {
            var a = this, b = this.chart, c = this.legendItems, d = b.options.legend, e = d.valueDecimals, f = d.valueSuffix || "", g; c.length || m(this.dataClasses, function (d, i) {
                var j = !0, k = d.from, l = d.to; g = ""; k === q ? g = "< " : l === q && (g = "> "); k !== q && (g += z.numberFormat(k,
                e) + f); k !== q && l !== q && (g += " - "); l !== q && (g += z.numberFormat(l, e) + f); c.push(s({ chart: b, name: g, options: {}, drawLegendSymbol: Ya.drawRectangle, visible: !0, setState: T, setVisible: function () { j = this.visible = !j; m(a.series, function (a) { m(a.points, function (a) { a.dataClass === i && a.setVisible(j) }) }); b.legend.colorizeItem(this, j) } }, d))
            }); return c
        }, name: ""
    }); m(["fill", "stroke"], function (a) { HighchartsAdapter.addAnimSetter(a, function (b) { b.elem.attr(a, Za.prototype.tweenColors(V(b.start), V(b.end), b.pos)) }) }); da(ma.prototype,
    "getAxes", function (a) { var b = this.options.colorAxis; a.call(this); this.colorAxis = []; b && new Za(this, b) }); da(Xa.prototype, "getAllItems", function (a) { var b = [], c = this.chart.colorAxis[0]; c && (c.options.dataClasses ? b = b.concat(c.getDataClassLegendSymbols()) : b.push(c), m(c.series, function (a) { a.options.showInLegend = !1 })); return b.concat(a.call(this)) }); var Ia = {
        pointAttrToOptions: { stroke: "borderColor", "stroke-width": "borderWidth", fill: "color", dashstyle: "dashStyle" }, pointArrayMap: ["value"], axisTypes: ["xAxis", "yAxis",
        "colorAxis"], optionalAxis: "colorAxis", trackerGroups: ["group", "markerGroup", "dataLabelsGroup"], getSymbol: T, parallelArrays: ["x", "y", "value"], colorKey: "value", translateColors: function () { var a = this, b = this.options.nullColor, c = this.colorAxis, d = this.colorKey; m(this.data, function (e) { var f = e[d]; if (f = f === null ? b : c && f !== void 0 ? c.toColor(f, e) : e.color || a.color) e.color = f }) }
    }, gb = document.documentElement.style.vectorEffect !== void 0; U.map = A(U.scatter, {
        allAreas: !0, animation: !1, nullColor: "#F8F8F8", borderColor: "silver",
        borderWidth: 1, marker: null, stickyTracking: !1, dataLabels: { formatter: function () { return this.point.value }, inside: !0, verticalAlign: "middle", crop: !1, overflow: !1, padding: 0 }, turboThreshold: 0, tooltip: { followPointer: !0, pointFormat: "{point.name}: {point.value}<br/>" }, states: { normal: { animation: !0 }, hover: { brightness: 0.2, halo: null } }
    }); var Ib = aa(Z, {
        applyOptions: function (a, b) {
            var c = Z.prototype.applyOptions.call(this, a, b), d = this.series, e = d.joinBy; if (d.mapData) if (e = c[e[1]] !== void 0 && d.mapMap[c[e[1]]]) {
                if (d.xyFromShape) c.x =
                e._midX, c.y = e._midY; s(c, e)
            } else c.value = c.value || null; return c
        }, setVisible: function (a) { var b = this, c = a ? "show" : "hide"; m(["graphic", "dataLabel"], function (a) { if (b[a]) b[a][c]() }) }, onMouseOver: function (a) { clearTimeout(this.colorInterval); if (this.value !== null) Z.prototype.onMouseOver.call(this, a); else this.series.onMouseOut(a) }, onMouseOut: function () {
            var a = this, b = +new Ga, c = V(a.color), d = V(a.pointAttr.hover.fill),
                e = a.series.options.states.normal.animation, f = e && (e.duration || 500), g; if (f && c.rgba.length === 4 && d.rgba.length ===
            4 && a.state !== "select") g = a.pointAttr[""].fill, delete a.pointAttr[""].fill, clearTimeout(a.colorInterval), a.colorInterval = setInterval(function () { var e = (new Ga - b) / f, g = a.graphic; e > 1 && (e = 1); g && g.attr("fill", Za.prototype.tweenColors.call(0, d, c, e)); e >= 1 && clearTimeout(a.colorInterval) }, 13); Z.prototype.onMouseOut.call(a); if (g) a.pointAttr[""].fill = g
        }, zoomTo: function () { var a = this.series; a.xAxis.setExtremes(this._minX, this._maxX, !1); a.yAxis.setExtremes(this._minY, this._maxY, !1); a.chart.redraw() }
    }); u.map = aa(u.scatter,
    A(Ia, {
        type: "map", pointClass: Ib, supportsDrilldown: !0, getExtremesFromAll: !0, useMapGeometry: !0, forceDL: !0, searchPoint: T, preserveAspectRatio: !0, getBox: function (a) {
            var b = Number.MAX_VALUE, c = -b, d = b, e = -b, f = b, g = b, h = this.xAxis, i = this.yAxis, j; m(a || [], function (a) {
                if (a.path) {
                    if (typeof a.path === "string") a.path = z.splitPath(a.path); var h = a.path || [], i = h.length, m = !1, q = -b, t = b, s = -b, r = b, u = a.properties; if (!a._foundBox) {
                        for (; i--;) typeof h[i] === "number" && !isNaN(h[i]) && (m ? (q = Math.max(q, h[i]), t = Math.min(t, h[i])) : (s = Math.max(s,
                        h[i]), r = Math.min(r, h[i])), m = !m); a._midX = t + (q - t) * (a.middleX || u && u["hc-middle-x"] || 0.5); a._midY = r + (s - r) * (a.middleY || u && u["hc-middle-y"] || 0.5); a._maxX = q; a._minX = t; a._maxY = s; a._minY = r; a.labelrank = o(a.labelrank, (q - t) * (s - r)); a._foundBox = !0
                    } c = Math.max(c, a._maxX); d = Math.min(d, a._minX); e = Math.max(e, a._maxY); f = Math.min(f, a._minY); g = Math.min(a._maxX - a._minX, a._maxY - a._minY, g); j = !0
                }
            }); if (j) {
                this.minY = Math.min(f, o(this.minY, b)); this.maxY = Math.max(e, o(this.maxY, -b)); this.minX = Math.min(d, o(this.minX, b)); this.maxX =
                Math.max(c, o(this.maxX, -b)); if (h && h.options.minRange === void 0) h.minRange = Math.min(5 * g, (this.maxX - this.minX) / 5, h.minRange || b); if (i && i.options.minRange === void 0) i.minRange = Math.min(5 * g, (this.maxY - this.minY) / 5, i.minRange || b)
            }
        }, getExtremes: function () { R.prototype.getExtremes.call(this, this.valueData); this.chart.hasRendered && this.isDirtyData && this.getBox(this.options.data); this.valueMin = this.dataMin; this.valueMax = this.dataMax; this.dataMin = this.minY; this.dataMax = this.maxY }, translatePath: function (a) {
            var b =
            !1, c = this.xAxis, d = this.yAxis, e = c.min, f = c.transA, c = c.minPixelPadding, g = d.min, h = d.transA, d = d.minPixelPadding, i, j = []; if (a) for (i = a.length; i--;) typeof a[i] === "number" ? (j[i] = b ? (a[i] - e) * f + c : (a[i] - g) * h + d, b = !b) : j[i] = a[i]; return j
        }, setData: function (a, b) {
            var c = this.options, d = c.mapData, e = c.joinBy, f = e === null, g = [], h, i, j; f && (e = "_i"); e = this.joinBy = z.splat(e); e[1] || (e[1] = e[0]); a && m(a, function (b, c) { typeof b === "number" && (a[c] = { value: b }); if (f) a[c]._i = c }); this.getBox(a); if (d) {
                if (d.type === "FeatureCollection") {
                    if (d["hc-transform"]) for (h in this.chart.mapTransforms =
                    i = d["hc-transform"], i) if (i.hasOwnProperty(h) && h.rotation) h.cosAngle = Math.cos(h.rotation), h.sinAngle = Math.sin(h.rotation); d = z.geojson(d, this.type, this)
                } this.getBox(d); this.mapData = d; this.mapMap = {}; for (j = 0; j < d.length; j++) h = d[j], i = h.properties, h._i = j, e[0] && i && i[e[0]] && (h[e[0]] = i[e[0]]), this.mapMap[h[e[0]]] = h; c.allAreas && (a = a || [], e[1] && m(a, function (a) { g.push(a[e[1]]) }), g = "|" + g.join("|") + "|", m(d, function (b) { (!e[0] || g.indexOf("|" + b[e[0]] + "|") === -1) && a.push(A(b, { value: null })) }))
            } R.prototype.setData.call(this,
            a, b)
        }, drawGraph: T, drawDataLabels: T, doFullTranslate: function () { return this.isDirtyData || this.chart.isResizing || this.chart.renderer.isVML || !this.baseTrans }, translate: function () { var a = this, b = a.xAxis, c = a.yAxis, d = a.doFullTranslate(); a.generatePoints(); m(a.data, function (e) { e.plotX = b.toPixels(e._midX, !0); e.plotY = c.toPixels(e._midY, !0); if (d) e.shapeType = "path", e.shapeArgs = { d: a.translatePath(e.path) }, gb && (e.shapeArgs["vector-effect"] = "non-scaling-stroke") }); a.translateColors() }, drawPoints: function () {
            var a =
            this, b = a.xAxis, c = a.yAxis, d = a.group, e = a.chart, f = e.renderer, g, h = this.baseTrans; if (!a.transformGroup) a.transformGroup = f.g().attr({ scaleX: 1, scaleY: 1 }).add(d), a.transformGroup.survive = !0; a.doFullTranslate() ? (e.hasRendered && a.pointAttrToOptions.fill === "color" && m(a.points, function (a) { a.graphic && a.graphic.attr("fill", a.color) }), gb || m(a.points, function (b) { b = b.pointAttr[""]; b["stroke-width"] === a.pointAttr[""]["stroke-width"] && (b["stroke-width"] = "inherit") }), a.group = a.transformGroup, u.column.prototype.drawPoints.apply(a),
            a.group = d, m(a.points, function (a) { a.graphic && (a.name && a.graphic.addClass("highcharts-name-" + a.name.replace(" ", "-").toLowerCase()), a.properties && a.properties["hc-key"] && a.graphic.addClass("highcharts-key-" + a.properties["hc-key"].toLowerCase()), gb || (a.graphic["stroke-widthSetter"] = T)) }), this.baseTrans = { originX: b.min - b.minPixelPadding / b.transA, originY: c.min - c.minPixelPadding / c.transA + (c.reversed ? 0 : c.len / c.transA), transAX: b.transA, transAY: c.transA }, this.transformGroup.animate({
                translateX: 0, translateY: 0,
                scaleX: 1, scaleY: 1
            })) : (g = b.transA / h.transAX, d = c.transA / h.transAY, b = b.toPixels(h.originX, !0), c = c.toPixels(h.originY, !0), g > 0.99 && g < 1.01 && d > 0.99 && d < 1.01 && (d = g = 1, b = Math.round(b), c = Math.round(c)), this.transformGroup.animate({ translateX: b, translateY: c, scaleX: g, scaleY: d })); gb || a.group.element.setAttribute("stroke-width", a.options.borderWidth / (g || 1)); this.drawMapDataLabels()
        }, drawMapDataLabels: function () { R.prototype.drawDataLabels.call(this); this.dataLabelsGroup && this.dataLabelsGroup.clip(this.chart.clipRect) },
        render: function () { var a = this, b = R.prototype.render; a.chart.renderer.isVML && a.data.length > 3E3 ? setTimeout(function () { b.call(a) }) : b.call(a) }, animate: function (a) { var b = this.options.animation, c = this.group, d = this.xAxis, e = this.yAxis, f = d.pos, g = e.pos; if (this.chart.renderer.isSVG) b === !0 && (b = { duration: 1E3 }), a ? c.attr({ translateX: f + d.len / 2, translateY: g + e.len / 2, scaleX: 0.001, scaleY: 0.001 }) : (c.animate({ translateX: f, translateY: g, scaleX: 1, scaleY: 1 }, b), this.animate = null) }, animateDrilldown: function (a) {
            var b = this.chart.plotBox,
            c = this.chart.drilldownLevels[this.chart.drilldownLevels.length - 1], d = c.bBox, e = this.chart.options.drilldown.animation; if (!a) a = Math.min(d.width / b.width, d.height / b.height), c.shapeArgs = { scaleX: a, scaleY: a, translateX: d.x, translateY: d.y }, m(this.points, function (a) { a.graphic.attr(c.shapeArgs).animate({ scaleX: 1, scaleY: 1, translateX: 0, translateY: 0 }, e) }), this.animate = null
        }, drawLegendSymbol: Ya.drawRectangle, animateDrillupFrom: function (a) { u.column.prototype.animateDrillupFrom.call(this, a) }, animateDrillupTo: function (a) {
            u.column.prototype.animateDrillupTo.call(this,
            a)
        }
    })); (function (a) {
        var b = a.Chart, c = a.each, d = HighchartsAdapter.addEvent; b.prototype.callbacks.push(function (a) { function b() { var d = []; c(a.series, function (a) { var b = a.options.dataLabels; (b.enabled || a._hasPointLabels) && !b.allowOverlap && a.visible && c(a.points, function (a) { if (a.dataLabel) a.dataLabel.labelrank = a.labelrank, d.push(a.dataLabel) }) }); a.hideOverlappingLabels(d) } b(); d(a, "redraw", b) }); b.prototype.hideOverlappingLabels = function (a) {
            var b = a.length, c, d, i, j; for (d = 0; d < b; d++) if (c = a[d]) c.oldOpacity = c.opacity,
            c.newOpacity = 1; for (d = 0; d < b; d++) { i = a[d]; for (c = d + 1; c < b; ++c) if (j = a[c], i && j && i.placed && j.placed && i.newOpacity !== 0 && j.newOpacity !== 0 && !(j.alignAttr.x > i.alignAttr.x + i.width || j.alignAttr.x + j.width < i.alignAttr.x || j.alignAttr.y > i.alignAttr.y + i.height || j.alignAttr.y + j.height < i.alignAttr.y)) (i.labelrank < j.labelrank ? i : j).newOpacity = 0 } for (d = 0; d < b; d++) if (c = a[d]) { if (c.oldOpacity !== c.newOpacity && c.placed) c.alignAttr.opacity = c.newOpacity, c[c.isOld && c.newOpacity ? "animate" : "attr"](c.alignAttr); c.isOld = !0 }
        }
    })(z); s(ma.prototype,
    {
        renderMapNavigation: function () {
            var a = this, b = this.options.mapNavigation, c = b.buttons, d, e, f, g, h = function () { this.handler.call(a) }; if (o(b.enableButtons, b.enabled) && !a.renderer.forExport) for (d in c) if (c.hasOwnProperty(d)) f = A(b.buttonOptions, c[d]), e = f.theme, e.style = A(f.theme.style, f.style), g = e.states, e = a.renderer.button(f.text, 0, 0, h, e, g && g.hover, g && g.select, 0, d === "zoomIn" ? "topbutton" : "bottombutton").attr({ width: f.width, height: f.height, title: a.options.lang[d], zIndex: 5 }).add(), e.handler = f.onclick, e.align(s(f,
            { width: e.width, height: 2 * e.height }), null, f.alignTo)
        }, fitToBox: function (a, b) { m([["x", "width"], ["y", "height"]], function (c) { var d = c[0], c = c[1]; a[d] + a[c] > b[d] + b[c] && (a[c] > b[c] ? (a[c] = b[c], a[d] = b[d]) : a[d] = b[d] + b[c] - a[c]); a[c] > b[c] && (a[c] = b[c]); a[d] < b[d] && (a[d] = b[d]) }); return a }, mapZoom: function (a, b, c, d, e) {
            var f = this.xAxis[0], g = f.max - f.min, h = o(b, f.min + g / 2), i = g * a, g = this.yAxis[0], j = g.max - g.min, k = o(c, g.min + j / 2); j *= a; h = this.fitToBox({ x: h - i * (d ? (d - f.pos) / f.len : 0.5), y: k - j * (e ? (e - g.pos) / g.len : 0.5), width: i, height: j },
            { x: f.dataMin, y: g.dataMin, width: f.dataMax - f.dataMin, height: g.dataMax - g.dataMin }); if (d) f.fixTo = [d - f.pos, b]; if (e) g.fixTo = [e - g.pos, c]; a !== void 0 ? (f.setExtremes(h.x, h.x + h.width, !1), g.setExtremes(h.y, h.y + h.height, !1)) : (f.setExtremes(void 0, void 0, !1), g.setExtremes(void 0, void 0, !1)); this.redraw()
        }
    }); da(ma.prototype, "render", function (a) {
        var b = this, c = b.options.mapNavigation; b.renderMapNavigation(); a.call(b); (o(c.enableDoubleClickZoom, c.enabled) || c.enableDoubleClickZoomTo) && M(b.container, "dblclick", function (a) { b.pointer.onContainerDblClick(a) });
        o(c.enableMouseWheelZoom, c.enabled) && M(b.container, document.onmousewheel === void 0 ? "DOMMouseScroll" : "mousewheel", function (a) { b.pointer.onContainerMouseWheel(a); return !1 })
    }); s(Aa.prototype, {
        onContainerDblClick: function (a) {
            var b = this.chart, a = this.normalize(a); b.options.mapNavigation.enableDoubleClickZoomTo ? b.pointer.inClass(a.target, "highcharts-tracker") && b.hoverPoint.zoomTo() : b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && b.mapZoom(0.5, b.xAxis[0].toValue(a.chartX), b.yAxis[0].toValue(a.chartY),
            a.chartX, a.chartY)
        }, onContainerMouseWheel: function (a) { var b = this.chart, c, a = this.normalize(a); c = a.detail || -(a.wheelDelta / 120); b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && b.mapZoom(Math.pow(2, c), b.xAxis[0].toValue(a.chartX), b.yAxis[0].toValue(a.chartY), a.chartX, a.chartY) }
    }); da(Aa.prototype, "init", function (a, b, c) { a.call(this, b, c); if (o(c.mapNavigation.enableTouchZoom, c.mapNavigation.enabled)) this.pinchX = this.pinchHor = this.pinchY = this.pinchVert = this.hasZoom = !0 }); da(Aa.prototype, "pinchTranslate",
    function (a, b, c, d, e, f, g) { a.call(this, b, c, d, e, f, g); this.chart.options.chart.type === "map" && this.hasZoom && (a = d.scaleX > d.scaleY, this.pinchTranslateDirection(!a, b, c, d, e, f, g, a ? d.scaleX : d.scaleY)) }); U.mapline = A(U.map, { lineWidth: 1, fillColor: "none" }); u.mapline = aa(u.map, { type: "mapline", pointAttrToOptions: { stroke: "color", "stroke-width": "lineWidth", fill: "fillColor", dashstyle: "dashStyle" }, drawLegendSymbol: u.line.prototype.drawLegendSymbol }); U.mappoint = A(U.scatter, {
        dataLabels: {
            enabled: !0, formatter: function () { return this.point.name },
            crop: !1, defer: !1, overflow: !1, style: { color: "#000000" }
        }
    }); u.mappoint = aa(u.scatter, { type: "mappoint", forceDL: !0, pointClass: aa(Z, { applyOptions: function (a, b) { var c = Z.prototype.applyOptions.call(this, a, b); a.lat !== void 0 && a.lon !== void 0 && (c = s(c, this.series.chart.fromLatLonToPoint(c))); return c } }) }); U.bubble = A(U.scatter, {
        dataLabels: { formatter: function () { return this.point.z }, inside: !0, verticalAlign: "middle" }, marker: { lineColor: null, lineWidth: 1 }, minSize: 8, maxSize: "20%", states: { hover: { halo: { size: 5 } } }, tooltip: { pointFormat: "({point.x}, {point.y}), Size: {point.z}" },
        turboThreshold: 0, zThreshold: 0, zoneAxis: "z"
    }); var Tb = aa(Z, { haloPath: function () { return Z.prototype.haloPath.call(this, this.shapeArgs.r + this.series.options.states.hover.halo.size) }, ttBelow: !1 }); u.bubble = aa(u.scatter, {
        type: "bubble", pointClass: Tb, pointArrayMap: ["y", "z"], parallelArrays: ["x", "y", "z"], trackerGroups: ["group", "dataLabelsGroup"], bubblePadding: !0, zoneAxis: "z", pointAttrToOptions: { stroke: "lineColor", "stroke-width": "lineWidth", fill: "fillColor" }, applyOpacity: function (a) {
            var b = this.options.marker,
            c = o(b.fillOpacity, 0.5), a = a || b.fillColor || this.color; c !== 1 && (a = V(a).setOpacity(c).get("rgba")); return a
        }, convertAttribs: function () { var a = R.prototype.convertAttribs.apply(this, arguments); a.fill = this.applyOpacity(a.fill); return a }, getRadii: function (a, b, c, d) { var e, f, g, h = this.zData, i = [], j = this.options.sizeBy !== "width"; for (f = 0, e = h.length; f < e; f++) g = b - a, g = g > 0 ? (h[f] - a) / (b - a) : 0.5, j && g >= 0 && (g = Math.sqrt(g)), i.push(J.ceil(c + g * (d - c)) / 2); this.radii = i }, animate: function (a) {
            var b = this.options.animation; if (!a) m(this.points,
            function (a) { var d = a.graphic, a = a.shapeArgs; d && a && (d.attr("r", 1), d.animate({ r: a.r }, b)) }), this.animate = null
        }, translate: function () { var a, b = this.data, c, d, e = this.radii; u.scatter.prototype.translate.call(this); for (a = b.length; a--;) c = b[a], d = e ? e[a] : 0, d >= this.minPxSize / 2 ? (c.shapeType = "circle", c.shapeArgs = { x: c.plotX, y: c.plotY, r: d }, c.dlBox = { x: c.plotX - d, y: c.plotY - d, width: 2 * d, height: 2 * d }) : c.shapeArgs = c.plotY = c.dlBox = q }, drawLegendSymbol: function (a, b) {
            var c = D(a.itemStyle.fontSize) / 2; b.legendSymbol = this.chart.renderer.circle(c,
            a.baseline - c, c).attr({ zIndex: 3 }).add(b.legendGroup); b.legendSymbol.isMarker = !0
        }, drawPoints: u.column.prototype.drawPoints, alignDataLabel: u.column.prototype.alignDataLabel, buildKDTree: T, applyZones: T
    }); X.prototype.beforePadding = function () {
        var a = this, b = this.len, c = this.chart, d = 0, e = b, f = this.isXAxis, g = f ? "xData" : "yData", h = this.min, i = {}, j = J.min(c.plotWidth, c.plotHeight), k = Number.MAX_VALUE, l = -Number.MAX_VALUE, n = this.max - h, p = b / n, r = []; m(this.series, function (b) {
            var d = b.options; if (b.bubblePadding && (b.visible ||
            !c.options.chart.ignoreHiddenSeries)) if (a.allowZoomOutside = !0, r.push(b), f) m(["minSize", "maxSize"], function (a) { var b = d[a], c = /%$/.test(b), b = D(b); i[a] = c ? j * b / 100 : b }), b.minPxSize = i.minSize, b = b.zData, b.length && (k = o(d.zMin, J.min(k, J.max(Ra(b), d.displayNegative === !1 ? d.zThreshold : -Number.MAX_VALUE))), l = o(d.zMax, J.max(l, Ea(b))))
        }); m(r, function (a) {
            var b = a[g], c = b.length, j; f && a.getRadii(k, l, i.minSize, i.maxSize); if (n > 0) for (; c--;) typeof b[c] === "number" && (j = a.radii[c], d = Math.min((b[c] - h) * p - j, d), e = Math.max((b[c] -
            h) * p + j, e))
        }); r.length && n > 0 && o(this.options.min, this.userMin) === q && o(this.options.max, this.userMax) === q && (e -= b, p *= (b + d - e) / b, this.min += d / p, this.max += e / p)
    }; if (u.bubble) U.mapbubble = A(U.bubble, { animationLimit: 500, tooltip: { pointFormat: "{point.name}: {point.z}" } }), u.mapbubble = aa(u.bubble, {
        pointClass: aa(Z, {
            applyOptions: function (a, b) { var c; a.lat !== void 0 && a.lon !== void 0 ? (c = Z.prototype.applyOptions.call(this, a, b), c = s(c, this.series.chart.fromLatLonToPoint(c))) : c = Ib.prototype.applyOptions.call(this, a, b); return c },
            ttBelow: !1
        }), xyFromShape: !0, type: "mapbubble", pointArrayMap: ["z"], getMapData: u.map.prototype.getMapData, getBox: u.map.prototype.getBox, setData: u.map.prototype.setData
    }); ma.prototype.transformFromLatLon = function (a, b) {
        if (window.proj4 === void 0) return ba(21), { x: 0, y: null }; var c = window.proj4(b.crs, [a.lon, a.lat]), d = b.cosAngle || b.rotation && Math.cos(b.rotation), e = b.sinAngle || b.rotation && Math.sin(b.rotation), c = b.rotation ? [c[0] * d + c[1] * e, -c[0] * e + c[1] * d] : c; return {
            x: ((c[0] - (b.xoffset || 0)) * (b.scale || 1) + (b.xpan || 0)) *
            (b.jsonres || 1) + (b.jsonmarginX || 0), y: (((b.yoffset || 0) - c[1]) * (b.scale || 1) + (b.ypan || 0)) * (b.jsonres || 1) - (b.jsonmarginY || 0)
        }
    }; ma.prototype.transformToLatLon = function (a, b) {
        if (window.proj4 === void 0) ba(21); else {
            var c = { x: ((a.x - (b.jsonmarginX || 0)) / (b.jsonres || 1) - (b.xpan || 0)) / (b.scale || 1) + (b.xoffset || 0), y: ((-a.y - (b.jsonmarginY || 0)) / (b.jsonres || 1) + (b.ypan || 0)) / (b.scale || 1) + (b.yoffset || 0) }, d = b.cosAngle || b.rotation && Math.cos(b.rotation), e = b.sinAngle || b.rotation && Math.sin(b.rotation), c = window.proj4(b.crs, "WGS84",
            b.rotation ? { x: c.x * d + c.y * -e, y: c.x * e + c.y * d } : c); return { lat: c.y, lon: c.x }
        }
    }; ma.prototype.fromPointToLatLon = function (a) { var b = this.mapTransforms, c; if (b) { for (c in b) if (b.hasOwnProperty(c) && b[c].hitZone && Bb({ x: a.x, y: -a.y }, b[c].hitZone.coordinates[0])) return this.transformToLatLon(a, b[c]); return this.transformToLatLon(a, b["default"]) } else ba(22) }; ma.prototype.fromLatLonToPoint = function (a) {
        var b = this.mapTransforms, c, d; if (!b) return ba(22), { x: 0, y: null }; for (c in b) if (b.hasOwnProperty(c) && b[c].hitZone && (d = this.transformFromLatLon(a,
        b[c]), Bb({ x: d.x, y: -d.y }, b[c].hitZone.coordinates[0]))) return d; return this.transformFromLatLon(a, b["default"])
    }; z.geojson = function (a, b, c) {
        var d = [], e = [], f = function (a) { var b = 0, c = a.length; for (e.push("M") ; b < c; b++) b === 1 && e.push("L"), e.push(a[b][0], -a[b][1]) }, b = b || "map"; m(a.features, function (a) {
            var c = a.geometry, i = c.type, c = c.coordinates, a = a.properties, j; e = []; b === "map" || b === "mapbubble" ? (i === "Polygon" ? (m(c, f), e.push("Z")) : i === "MultiPolygon" && (m(c, function (a) { m(a, f) }), e.push("Z")), e.length && (j = { path: e })) : b ===
            "mapline" ? (i === "LineString" ? f(c) : i === "MultiLineString" && m(c, f), e.length && (j = { path: e })) : b === "mappoint" && i === "Point" && (j = { x: c[0], y: -c[1] }); j && d.push(s(j, { name: a.name || a.NAME, properties: a }))
        }); if (c && a.copyrightShort) c.chart.mapCredits = '<a href="http://www.highcharts.com">Highcharts</a> Â© <a href="' + a.copyrightUrl + '">' + a.copyrightShort + "</a>", c.chart.mapCreditsFull = a.copyright; return d
    }; da(ma.prototype, "showCredits", function (a, b) {
        if (I.credits.text === this.options.credits.text && this.mapCredits) b.text = this.mapCredits,
        b.href = null; a.call(this, b); this.credits && this.credits.attr({ title: this.mapCreditsFull })
    }); s(I.lang, { zoomIn: "Zoom in", zoomOut: "Zoom out" }); I.mapNavigation = { buttonOptions: { alignTo: "plotBox", align: "left", verticalAlign: "top", x: 0, width: 18, height: 18, style: { fontSize: "15px", fontWeight: "bold", textAlign: "center" }, theme: { "stroke-width": 1 } }, buttons: { zoomIn: { onclick: function () { this.mapZoom(0.5) }, text: "+", y: 0 }, zoomOut: { onclick: function () { this.mapZoom(2) }, text: "-", y: 28 } } }; z.splitPath = function (a) {
        var b, a = a.replace(/([A-Za-z])/g,
        " $1 "), a = a.replace(/^\s*/, "").replace(/\s*$/, ""), a = a.split(/[ ,]+/); for (b = 0; b < a.length; b++) /[a-zA-Z]/.test(a[b]) || (a[b] = parseFloat(a[b])); return a
    }; z.maps = {}; ia.prototype.symbols.topbutton = function (a, b, c, d, e) { return Cb(e, a, b, c, d, e.r, e.r, 0, 0) }; ia.prototype.symbols.bottombutton = function (a, b, c, d, e) { return Cb(e, a, b, c, d, 0, 0, e.r, e.r) }; Na === Wa && m(["topbutton", "bottombutton"], function (a) { Wa.prototype.symbols[a] = ia.prototype.symbols[a] }); z.Map = function (a, b) {
        var c = {
            endOnTick: !1, gridLineWidth: 0, lineWidth: 0,
            minPadding: 0, maxPadding: 0, startOnTick: !1, title: null, tickPositions: []
        }, d; d = a.series; a.series = null; a = A({ chart: { panning: "xy", type: "map" }, xAxis: c, yAxis: A(c, { reversed: !0 }) }, a, { chart: { inverted: !1, alignTicks: !1 } }); a.series = d; return new ma(a, b)
    }; I.plotOptions.heatmap = A(I.plotOptions.scatter, {
        animation: !1, borderWidth: 0, nullColor: "#F8F8F8", dataLabels: { formatter: function () { return this.point.value }, inside: !0, verticalAlign: "middle", crop: !1, overflow: !1, padding: 0 }, marker: null, pointRange: null, tooltip: { pointFormat: "{point.x}, {point.y}: {point.value}<br/>" },
        states: { normal: { animation: !0 }, hover: { halo: !1, brightness: 0.2 } }
    }); u.heatmap = aa(u.scatter, A(Ia, {
        type: "heatmap", pointArrayMap: ["y", "value"], hasPointSpecificOptions: !0, supportsDrilldown: !0, getExtremesFromAll: !0, init: function () { var a; u.scatter.prototype.init.apply(this, arguments); a = this.options; this.pointRange = a.pointRange = o(a.pointRange, a.colsize || 1); this.yAxis.axisPointRange = a.rowsize || 1 }, translate: function () {
            var a = this.options, b = this.xAxis, c = this.yAxis; this.generatePoints(); m(this.points, function (d) {
                var e =
                (a.colsize || 1) / 2, f = (a.rowsize || 1) / 2, g = Math.round(b.len - b.translate(d.x - e, 0, 1, 0, 1)), e = Math.round(b.len - b.translate(d.x + e, 0, 1, 0, 1)), h = Math.round(c.translate(d.y - f, 0, 1, 0, 1)), f = Math.round(c.translate(d.y + f, 0, 1, 0, 1)); d.plotX = d.clientX = (g + e) / 2; d.plotY = (h + f) / 2; d.shapeType = "rect"; d.shapeArgs = { x: Math.min(g, e), y: Math.min(h, f), width: Math.abs(e - g), height: Math.abs(f - h) }
            }); this.translateColors(); this.chart.hasRendered && m(this.points, function (a) { a.shapeArgs.fill = a.options.color || a.color })
        }, drawPoints: u.column.prototype.drawPoints,
        animate: T, getBox: T, drawLegendSymbol: Ya.drawRectangle, getExtremes: function () { R.prototype.getExtremes.call(this, this.valueData); this.valueMin = this.dataMin; this.valueMax = this.dataMax; R.prototype.getExtremes.call(this) }
    })); Ia = z.TrackerMixin = {
        drawTrackerPoint: function () {
            var a = this, b = a.chart, c = b.pointer, d = a.options.cursor, e = d && { cursor: d }, f = function (a) { for (var c = a.target, d; c && !d;) d = c.point, c = c.parentNode; if (d !== q && d !== b.hoverPoint) d.onMouseOver(a) }; m(a.points, function (a) {
                if (a.graphic) a.graphic.element.point =
                a; if (a.dataLabel) a.dataLabel.element.point = a
            }); if (!a._hasTracking) m(a.trackerGroups, function (b) { if (a[b] && (a[b].addClass("highcharts-tracker").on("mouseover", f).on("mouseout", function (a) { c.onTrackerMouseOut(a) }).css(e), Sa)) a[b].on("touchstart", f) }), a._hasTracking = !0
        }, drawTrackerGraph: function () {
            var a = this, b = a.options, c = b.trackByArea, d = [].concat(c ? a.areaPath : a.graphPath), e = d.length, f = a.chart, g = f.pointer, h = f.renderer, i = f.options.tooltip.snap, j = a.tracker, k = b.cursor, l = k && { cursor: k }, k = a.singlePoints, n,
            o = function () { if (f.hoverSeries !== a) a.onMouseOver() }, q = "rgba(192,192,192," + (ca ? 1.0E-4 : 0.002) + ")"; if (e && !c) for (n = e + 1; n--;) d[n] === "M" && d.splice(n + 1, 0, d[n + 1] - i, d[n + 2], "L"), (n && d[n] === "M" || n === e) && d.splice(n, 0, "L", d[n - 2] + i, d[n - 1]); for (n = 0; n < k.length; n++) e = k[n], d.push("M", e.plotX - i, e.plotY, "L", e.plotX + i, e.plotY); j ? j.attr({ d: d }) : (a.tracker = h.path(d).attr({ "stroke-linejoin": "round", visibility: a.visible ? "visible" : "hidden", stroke: q, fill: c ? q : O, "stroke-width": b.lineWidth + (c ? 0 : 2 * i), zIndex: 2 }).add(a.group), m([a.tracker,
            a.markerGroup], function (a) { a.addClass("highcharts-tracker").on("mouseover", o).on("mouseout", function (a) { g.onTrackerMouseOut(a) }).css(l); if (Sa) a.on("touchstart", o) }))
        }
    }; if (u.column) E.prototype.drawTracker = Ia.drawTrackerPoint; if (u.pie) u.pie.prototype.drawTracker = Ia.drawTrackerPoint; if (u.scatter) ta.prototype.drawTracker = Ia.drawTrackerPoint; s(Xa.prototype, {
        setItemEvents: function (a, b, c, d, e) {
            var f = this; (c ? b : a.legendGroup).on("mouseover", function () {
                a.setState("hover"); b.css(f.options.itemHoverStyle)
            }).on("mouseout",
            function () {
                b.css(a.visible ? d : e); a.setState()
            }).on("click", function (b) { var c = function () { a.setVisible() }, b = { browserEvent: b }; a.firePointEvent ? a.firePointEvent("legendItemClick", b, c) : L(a, "legendItemClick", b, c) })
        }, createCheckboxForItem: function (a) { a.checkbox = $("input", { type: "checkbox", checked: a.selected, defaultChecked: a.selected }, this.options.itemCheckboxStyle, this.chart.container); M(a.checkbox, "click", function (b) { L(a.series || a, "checkboxClick", { checked: b.target.checked, item: a }, function () { a.select() }) }) }
    });
    I.legend.itemStyle.cursor = "pointer"; s(ma.prototype, {
        showResetZoom: function () { var a = this, b = I.lang, c = a.options.chart.resetZoomButton, d = c.theme, e = d.states, f = c.relativeTo === "chart" ? null : "plotBox"; this.resetZoomButton = a.renderer.button(b.resetZoom, null, null, function () { a.zoomOut() }, d, e && e.hover).attr({ align: c.position.align, title: b.resetZoomTitle }).add().align(c.position, !1, f) }, zoomOut: function () { var a = this; L(a, "selection", { resetSelection: !0 }, function () { a.zoom() }) }, zoom: function (a) {
            var b, c = this.pointer,
            d = !1, e; !a || a.resetSelection ? m(this.axes, function (a) { b = a.zoom() }) : m(a.xAxis.concat(a.yAxis), function (a) { var e = a.axis, h = e.isXAxis; if (c[h ? "zoomX" : "zoomY"] || c[h ? "pinchX" : "pinchY"]) b = e.zoom(a.min, a.max), e.displayBtn && (d = !0) }); e = this.resetZoomButton; if (d && !e) this.showResetZoom(); else if (!d && ea(e)) this.resetZoomButton = e.destroy(); b && this.redraw(o(this.options.chart.animation, a && a.animation, this.pointCount < 100))
        }, pan: function (a, b) {
            var c = this, d = c.hoverPoints, e; d && m(d, function (a) { a.setState() }); m(b === "xy" ?
            [1, 0] : [1], function (b) { var d = a[b ? "chartX" : "chartY"], h = c[b ? "xAxis" : "yAxis"][0], i = c[b ? "mouseDownX" : "mouseDownY"], j = (h.pointRange || 0) / 2, k = h.getExtremes(), l = h.toValue(i - d, !0) + j, j = h.toValue(i + c[b ? "plotWidth" : "plotHeight"] - d, !0) - j, i = i > d; if (h.series.length && (i || l > N(k.dataMin, k.min)) && (!i || j < w(k.dataMax, k.max))) h.setExtremes(l, j, !1, !1, { trigger: "pan" }), e = !0; c[b ? "mouseDownX" : "mouseDownY"] = d }); e && c.redraw(!1); F(c.container, { cursor: "move" })
        }
    }); s(Z.prototype, {
        select: function (a, b) {
            var c = this, d = c.series, e = d.chart,
            a = o(a, !c.selected); c.firePointEvent(a ? "select" : "unselect", { accumulate: b }, function () { c.selected = c.options.selected = a; d.options.data[Ua(c, d.data)] = c.options; c.setState(a && "select"); b || m(e.getSelectedPoints(), function (a) { if (a.selected && a !== c) a.selected = a.options.selected = !1, d.options.data[Ua(a, d.data)] = a.options, a.setState(""), a.firePointEvent("unselect") }) })
        }, onMouseOver: function (a) {
            var b = this.series, c = b.chart, d = c.tooltip, e = c.hoverPoint; if (c.hoverSeries !== b) b.onMouseOver(); if (e && e !== this) e.onMouseOut();
            this.firePointEvent("mouseOver"); d && (!d.shared || b.noSharedTooltip) && d.refresh(this, a); this.setState("hover"); c.hoverPoint = this
        }, onMouseOut: function () { var a = this.series.chart, b = a.hoverPoints; this.firePointEvent("mouseOut"); if (!b || Ua(this, b) === -1) this.setState(), a.hoverPoint = null }, importEvents: function () { if (!this.hasImportedEvents) { var a = A(this.series.options.point, this.options).events, b; this.events = a; for (b in a) M(this, b, a[b]); this.hasImportedEvents = !0 } }, setState: function (a, b) {
            var c = this.plotX, d = this.plotY,
            e = this.series, f = e.options.states, g = U[e.type].marker && e.options.marker, h = g && !g.enabled, i = g && g.states[a], j = i && i.enabled === !1, k = e.stateMarkerGraphic, l = this.marker || {}, m = e.chart, o = e.halo, q, a = a || ""; q = this.pointAttr[a] || e.pointAttr[a]; if (!(a === this.state && !b || this.selected && a !== "select" || f[a] && f[a].enabled === !1 || a && (j || h && i.enabled === !1) || a && l.states && l.states[a] && l.states[a].enabled === !1)) {
                if (this.graphic) g = g && this.graphic.symbolName && q.r, this.graphic.attr(A(q, g ? { x: c - g, y: d - g, width: 2 * g, height: 2 * g } : {})),
                k && k.hide(); else { if (a && i) if (g = i.radius, l = l.symbol || e.symbol, k && k.currentSymbol !== l && (k = k.destroy()), k) k[b ? "animate" : "attr"]({ x: c - g, y: d - g }); else if (l) e.stateMarkerGraphic = k = m.renderer.symbol(l, c - g, d - g, 2 * g, 2 * g).attr(q).add(e.markerGroup), k.currentSymbol = l; if (k) k[a && m.isInsidePlot(c, d, m.inverted) ? "show" : "hide"]() } if ((c = f[a] && f[a].halo) && c.size) { if (!o) e.halo = o = m.renderer.path().add(m.seriesGroup); o.attr(s({ fill: V(this.color || e.color).setOpacity(c.opacity).get() }, c.attributes))[b ? "animate" : "attr"]({ d: this.haloPath(c.size) }) } else o &&
                o.attr({ d: [] }); this.state = a
            }
        }, haloPath: function (a) { var b = this.series, c = b.chart, d = b.getPlotBox(), e = c.inverted; return c.renderer.symbols.circle(d.translateX + (e ? b.yAxis.len - this.plotY : this.plotX) - a, d.translateY + (e ? b.xAxis.len - this.plotX : this.plotY) - a, a * 2, a * 2) }
    }); s(R.prototype, {
        onMouseOver: function () { var a = this.chart, b = a.hoverSeries; if (b && b !== this) b.onMouseOut(); this.options.events.mouseOver && L(this, "mouseOver"); this.setState("hover"); a.hoverSeries = this }, onMouseOut: function () {
            var a = this.options, b = this.chart,
            c = b.tooltip, d = b.hoverPoint; if (d) d.onMouseOut(); this && a.events.mouseOut && L(this, "mouseOut"); c && !a.stickyTracking && (!c.shared || this.noSharedTooltip) && c.hide(); this.setState(); b.hoverSeries = null
        }, setState: function (a) { var b = this.options, c = this.graph, d = b.states, e = b.lineWidth, b = 0, a = a || ""; if (this.state !== a && (this.state = a, !(d[a] && d[a].enabled === !1) && (a && (e = d[a].lineWidth || e + (d[a].lineWidthPlus || 0)), c && !c.dashstyle))) { a = { "stroke-width": e }; for (c.attr(a) ; this["zoneGraph" + b];) this["zoneGraph" + b].attr(a), b += 1 } },
        setVisible: function (a, b) {
            var c = this, d = c.chart, e = c.legendItem, f, g = d.options.chart.ignoreHiddenSeries, h = c.visible; f = (c.visible = a = c.userOptions.visible = a === q ? !h : a) ? "show" : "hide"; m(["group", "dataLabelsGroup", "markerGroup", "tracker"], function (a) { if (c[a]) c[a][f]() }); if (d.hoverSeries === c || (d.hoverPoint && d.hoverPoint.series) === c) c.onMouseOut(); e && d.legend.colorizeItem(c, a); c.isDirty = !0; c.options.stacking && m(d.series, function (a) { if (a.options.stacking && a.visible) a.isDirty = !0 }); m(c.linkedSeries, function (b) {
                b.setVisible(a,
                !1)
            }); if (g) d.isDirtyBox = !0; b !== !1 && d.redraw(); L(c, f)
        }, show: function () { this.setVisible(!0) }, hide: function () { this.setVisible(!1) }, select: function (a) { this.selected = a = a === q ? !this.selected : a; if (this.checkbox) this.checkbox.checked = a; L(this, a ? "select" : "unselect") }, drawTracker: Ia.drawTrackerGraph
    }); s(z, {
        Color: V, Point: Z, Tick: Ma, Renderer: Na, SVGElement: P, SVGRenderer: ia, arrayMin: Ra, arrayMax: Ea, charts: W, dateFormat: Ka, error: ba, format: Ja, pathAnim: ob, getOptions: function () { return I }, hasBidiBug: Jb, isTouchDevice: Eb,
        setOptions: function (a) { I = A(!0, I, a); tb(); return I }, addEvent: M, removeEvent: Y, createElement: $, discardElement: La, css: F, each: m, map: Oa, merge: A, splat: ra, extendClass: aa, pInt: D, svg: ca, canvas: ga, vml: !ca && !ga, product: "Highmaps", version: "1.1.5"
    })
})();