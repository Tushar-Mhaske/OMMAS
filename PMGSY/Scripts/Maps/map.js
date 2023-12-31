﻿/*
 Highmaps JS v1.1.5 (2015-04-13)
 Highmaps as a plugin for Highcharts 4.0.x or Highstock 2.0.x (x being the patch version of this file)

 License: www.highcharts.com/license
*/
(function (k) {
    function I(a, b) { var c, d, e, f, g = !1, h = a.x, i = a.y; for (c = 0, d = b.length - 1; c < b.length; d = c++) e = b[c][1] > i, f = b[d][1] > i, e !== f && h < (b[d][0] - b[c][0]) * (i - b[c][1]) / (b[d][1] - b[c][1]) + b[c][0] && (g = !g); return g } function J(a, b, c, d, e, f, g, h, i) { a = a["stroke-width"] % 2 / 2; b -= a; c -= a; return ["M", b + f, c, "L", b + d - g, c, "C", b + d - g / 2, c, b + d, c + g / 2, b + d, c + g, "L", b + d, c + e - h, "C", b + d, c + e - h / 2, b + d - h / 2, c + e, b + d - h, c + e, "L", b + i, c + e, "C", b + i / 2, c + e, b, c + e - i / 2, b, c + e - i, "L", b, c + f, "C", b, c + f / 2, b + f / 2, c, b + f, c, "Z"] } var p = k.Axis, s = k.Chart, x = k.Color, u = k.Point,
    F = k.Pointer, C = k.Legend, G = k.LegendSymbolMixin, N = k.Renderer, y = k.Series, H = k.SVGRenderer, K = k.VMLRenderer, L = k.addEvent, j = k.each, D = k.error, q = k.extend, v = k.extendClass, o = k.merge, m = k.pick, z = k.getOptions(), l = k.seriesTypes, w = z.plotOptions, t = k.wrap, r = function () { }; t(p.prototype, "getSeriesExtremes", function (a) {
        var b = this.isXAxis, c, d, e = [], f; b && j(this.series, function (a, b) { if (a.useMapGeometry) e[b] = a.xData, a.xData = [] }); a.call(this); if (b && (c = m(this.dataMin, Number.MAX_VALUE), d = m(this.dataMax, -Number.MAX_VALUE), j(this.series,
        function (a, b) { if (a.useMapGeometry) c = Math.min(c, m(a.minX, c)), d = Math.max(d, m(a.maxX, c)), a.xData = e[b], f = !0 }), f)) this.dataMin = c, this.dataMax = d
    }); t(p.prototype, "setAxisTranslation", function (a) {
        var b = this.chart, c = b.plotWidth / b.plotHeight, b = b.xAxis[0], d; a.call(this); this.coll === "yAxis" && b.transA !== void 0 && j(this.series, function (a) { a.preserveAspectRatio && (d = !0) }); if (d && (this.transA = b.transA = Math.min(this.transA, b.transA), a = c / ((b.max - b.min) / (this.max - this.min)), a = a < 1 ? this : b, c = (a.max - a.min) * a.transA, a.pixelPadding =
        a.len - c, a.minPixelPadding = a.pixelPadding / 2, c = a.fixTo)) { c = c[1] - a.toValue(c[0], !0); c *= a.transA; if (Math.abs(c) > a.minPixelPadding || a.min === a.dataMin && a.max === a.dataMax) c = 0; a.minPixelPadding -= c }
    }); t(p.prototype, "render", function (a) { a.call(this); this.fixTo = null }); var B = k.ColorAxis = function () { this.isColorAxis = !0; this.init.apply(this, arguments) }; q(B.prototype, p.prototype); q(B.prototype, {
        defaultColorAxisOptions: {
            lineWidth: 0, gridLineWidth: 1, tickPixelInterval: 72, startOnTick: !0, endOnTick: !0, offset: 0, marker: {
                animation: { duration: 50 },
                color: "gray", width: 0.01
            }, labels: { overflow: "justify" }, minColor: "#EFEFFF", maxColor: "#003875", tickLength: 5
        }, init: function (a, b) { var c = a.options.legend.layout !== "vertical", d; d = o(this.defaultColorAxisOptions, { side: c ? 2 : 1, reversed: !c }, b, { isX: c, opposite: !c, showEmpty: !1, title: null, isColor: !0 }); p.prototype.init.call(this, a, d); b.dataClasses && this.initDataClasses(b); this.initStops(b); this.isXAxis = !0; this.horiz = c; this.zoomEnabled = !1 }, tweenColors: function (a, b, c) {
            var d; !b.rgba.length || !a.rgba.length ? a = b.raw || "none" :
            (a = a.rgba, b = b.rgba, d = b[3] !== 1 || a[3] !== 1, a = (d ? "rgba(" : "rgb(") + Math.round(b[0] + (a[0] - b[0]) * (1 - c)) + "," + Math.round(b[1] + (a[1] - b[1]) * (1 - c)) + "," + Math.round(b[2] + (a[2] - b[2]) * (1 - c)) + (d ? "," + (b[3] + (a[3] - b[3]) * (1 - c)) : "") + ")"); return a
        }, initDataClasses: function (a) {
            var b = this, c = this.chart, d, e = 0, f = this.options, g = a.dataClasses.length; this.dataClasses = d = []; this.legendItems = []; j(a.dataClasses, function (a, i) {
                var n, a = o(a); d.push(a); if (!a.color) f.dataClassColor === "category" ? (n = c.options.colors, a.color = n[e++], e === n.length &&
                (e = 0)) : a.color = b.tweenColors(x(f.minColor), x(f.maxColor), g < 2 ? 0.5 : i / (g - 1))
            })
        }, initStops: function (a) { this.stops = a.stops || [[0, this.options.minColor], [1, this.options.maxColor]]; j(this.stops, function (a) { a.color = x(a[1]) }) }, setOptions: function (a) { p.prototype.setOptions.call(this, a); this.options.crosshair = this.options.marker; this.coll = "colorAxis" }, setAxisSize: function () {
            var a = this.legendSymbol, b = this.chart, c, d, e; if (a) this.left = c = a.attr("x"), this.top = d = a.attr("y"), this.width = e = a.attr("width"), this.height =
            a = a.attr("height"), this.right = b.chartWidth - c - e, this.bottom = b.chartHeight - d - a, this.len = this.horiz ? e : a, this.pos = this.horiz ? c : d
        }, toColor: function (a, b) {
            var c, d = this.stops, e, f = this.dataClasses, g, h; if (f) for (h = f.length; h--;) { if (g = f[h], e = g.from, d = g.to, (e === void 0 || a >= e) && (d === void 0 || a <= d)) { c = g.color; if (b) b.dataClass = h; break } } else {
                this.isLog && (a = this.val2lin(a)); c = 1 - (this.max - a) / (this.max - this.min || 1); for (h = d.length; h--;) if (c > d[h][0]) break; e = d[h] || d[h + 1]; d = d[h + 1] || e; c = 1 - (d[0] - c) / (d[0] - e[0] || 1); c = this.tweenColors(e.color,
                d.color, c)
            } return c
        }, getOffset: function () { var a = this.legendGroup, b = this.chart.axisOffset[this.side]; if (a) { p.prototype.getOffset.call(this); if (!this.axisGroup.parentGroup) this.axisGroup.add(a), this.gridGroup.add(a), this.labelGroup.add(a), this.added = !0, this.labelLeft = 0, this.labelRight = this.width; this.chart.axisOffset[this.side] = b } }, setLegendColor: function () {
            var a, b = this.options; a = this.reversed; a = this.horiz ? [+a, 0, +!a, 0] : [0, +!a, 0, +a]; this.legendColor = {
                linearGradient: { x1: a[0], y1: a[1], x2: a[2], y2: a[3] },
                stops: b.stops || [[0, b.minColor], [1, b.maxColor]]
            }
        }, drawLegendSymbol: function (a, b) { var c = a.padding, d = a.options, e = this.horiz, f = m(d.symbolWidth, e ? 200 : 12), g = m(d.symbolHeight, e ? 12 : 200), h = m(d.labelPadding, e ? 16 : 30), d = m(d.itemDistance, 10); this.setLegendColor(); b.legendSymbol = this.chart.renderer.rect(0, a.baseline - 11, f, g).attr({ zIndex: 1 }).add(b.legendGroup); b.legendSymbol.getBBox(); this.legendItemWidth = f + c + (e ? d : h); this.legendItemHeight = g + c + (e ? h : 0) }, setState: r, visible: !0, setVisible: r, getSeriesExtremes: function () {
            var a;
            if (this.series.length) a = this.series[0], this.dataMin = a.valueMin, this.dataMax = a.valueMax
        }, drawCrosshair: function (a, b) { var c = b && b.plotX, d = b && b.plotY, e, f = this.pos, g = this.len; if (b) e = this.toPixels(b[b.series.colorKey]), e < f ? e = f - 2 : e > f + g && (e = f + g + 2), b.plotX = e, b.plotY = this.len - e, p.prototype.drawCrosshair.call(this, a, b), b.plotX = c, b.plotY = d, this.cross && this.cross.attr({ fill: this.crosshair.color }).add(this.legendGroup) }, getPlotLinePath: function (a, b, c, d, e) {
            return typeof e === "number" ? this.horiz ? ["M", e - 4, this.top -
            6, "L", e + 4, this.top - 6, e, this.top, "Z"] : ["M", this.left, e, "L", this.left - 6, e + 6, this.left - 6, e - 6, "Z"] : p.prototype.getPlotLinePath.call(this, a, b, c, d)
        }, update: function (a, b) { j(this.series, function (a) { a.isDirtyData = !0 }); p.prototype.update.call(this, a, b); this.legendItem && (this.setLegendColor(), this.chart.legend.colorizeItem(this, !0)) }, getDataClassLegendSymbols: function () {
            var a = this, b = this.chart, c = this.legendItems, d = b.options.legend, e = d.valueDecimals, f = d.valueSuffix || "", g; c.length || j(this.dataClasses, function (d,
            i) { var n = !0, A = d.from, m = d.to; g = ""; A === void 0 ? g = "< " : m === void 0 && (g = "> "); A !== void 0 && (g += k.numberFormat(A, e) + f); A !== void 0 && m !== void 0 && (g += " - "); m !== void 0 && (g += k.numberFormat(m, e) + f); c.push(q({ chart: b, name: g, options: {}, drawLegendSymbol: G.drawRectangle, visible: !0, setState: r, setVisible: function () { n = this.visible = !n; j(a.series, function (a) { j(a.points, function (a) { a.dataClass === i && a.setVisible(n) }) }); b.legend.colorizeItem(this, n) } }, d)) }); return c
        }, name: ""
    }); j(["fill", "stroke"], function (a) {
        HighchartsAdapter.addAnimSetter(a,
        function (b) { b.elem.attr(a, B.prototype.tweenColors(x(b.start), x(b.end), b.pos)) })
    }); t(s.prototype, "getAxes", function (a) { var b = this.options.colorAxis; a.call(this); this.colorAxis = []; b && new B(this, b) }); t(C.prototype, "getAllItems", function (a) { var b = [], c = this.chart.colorAxis[0]; c && (c.options.dataClasses ? b = b.concat(c.getDataClassLegendSymbols()) : b.push(c), j(c.series, function (a) { a.options.showInLegend = !1 })); return b.concat(a.call(this)) }); C = {
        pointAttrToOptions: {
            stroke: "borderColor", "stroke-width": "borderWidth",
            fill: "color", dashstyle: "dashStyle"
        }, pointArrayMap: ["value"], axisTypes: ["xAxis", "yAxis", "colorAxis"], optionalAxis: "colorAxis", trackerGroups: ["group", "markerGroup", "dataLabelsGroup"], getSymbol: r, parallelArrays: ["x", "y", "value"], colorKey: "value", translateColors: function () { var a = this, b = this.options.nullColor, c = this.colorAxis, d = this.colorKey; j(this.data, function (e) { var f = e[d]; if (f = f === null ? b : c && f !== void 0 ? c.toColor(f, e) : e.color || a.color) e.color = f }) }
    }; q(s.prototype, {
        renderMapNavigation: function () {
            var a =
            this, b = this.options.mapNavigation, c = b.buttons, d, e, f, g, h = function () { this.handler.call(a) }; if (m(b.enableButtons, b.enabled) && !a.renderer.forExport) for (d in c) if (c.hasOwnProperty(d)) f = o(b.buttonOptions, c[d]), e = f.theme, e.style = o(f.theme.style, f.style), g = e.states, e = a.renderer.button(f.text, 0, 0, h, e, g && g.hover, g && g.select, 0, d === "zoomIn" ? "topbutton" : "bottombutton").attr({ width: f.width, height: f.height, title: a.options.lang[d], zIndex: 5 }).add(), e.handler = f.onclick, e.align(q(f, { width: e.width, height: 2 * e.height }),
            null, f.alignTo)
        }, fitToBox: function (a, b) { j([["x", "width"], ["y", "height"]], function (c) { var d = c[0], c = c[1]; a[d] + a[c] > b[d] + b[c] && (a[c] > b[c] ? (a[c] = b[c], a[d] = b[d]) : a[d] = b[d] + b[c] - a[c]); a[c] > b[c] && (a[c] = b[c]); a[d] < b[d] && (a[d] = b[d]) }); return a }, mapZoom: function (a, b, c, d, e) {
            var f = this.xAxis[0], g = f.max - f.min, h = m(b, f.min + g / 2), i = g * a, g = this.yAxis[0], n = g.max - g.min, A = m(c, g.min + n / 2); n *= a; h = this.fitToBox({ x: h - i * (d ? (d - f.pos) / f.len : 0.5), y: A - n * (e ? (e - g.pos) / g.len : 0.5), width: i, height: n }, {
                x: f.dataMin, y: g.dataMin, width: f.dataMax -
                f.dataMin, height: g.dataMax - g.dataMin
            }); if (d) f.fixTo = [d - f.pos, b]; if (e) g.fixTo = [e - g.pos, c]; a !== void 0 ? (f.setExtremes(h.x, h.x + h.width, !1), g.setExtremes(h.y, h.y + h.height, !1)) : (f.setExtremes(void 0, void 0, !1), g.setExtremes(void 0, void 0, !1)); this.redraw()
        }
    }); t(s.prototype, "render", function (a) {
        var b = this, c = b.options.mapNavigation; b.renderMapNavigation(); a.call(b); (m(c.enableDoubleClickZoom, c.enabled) || c.enableDoubleClickZoomTo) && L(b.container, "dblclick", function (a) { b.pointer.onContainerDblClick(a) }); m(c.enableMouseWheelZoom,
        c.enabled) && L(b.container, document.onmousewheel === void 0 ? "DOMMouseScroll" : "mousewheel", function (a) { b.pointer.onContainerMouseWheel(a); return !1 })
    }); q(F.prototype, {
        onContainerDblClick: function (a) { var b = this.chart, a = this.normalize(a); b.options.mapNavigation.enableDoubleClickZoomTo ? b.pointer.inClass(a.target, "highcharts-tracker") && b.hoverPoint.zoomTo() : b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && b.mapZoom(0.5, b.xAxis[0].toValue(a.chartX), b.yAxis[0].toValue(a.chartY), a.chartX, a.chartY) }, onContainerMouseWheel: function (a) {
            var b =
            this.chart, c, a = this.normalize(a); c = a.detail || -(a.wheelDelta / 120); b.isInsidePlot(a.chartX - b.plotLeft, a.chartY - b.plotTop) && b.mapZoom(Math.pow(2, c), b.xAxis[0].toValue(a.chartX), b.yAxis[0].toValue(a.chartY), a.chartX, a.chartY)
        }
    }); t(F.prototype, "init", function (a, b, c) { a.call(this, b, c); if (m(c.mapNavigation.enableTouchZoom, c.mapNavigation.enabled)) this.pinchX = this.pinchHor = this.pinchY = this.pinchVert = this.hasZoom = !0 }); t(F.prototype, "pinchTranslate", function (a, b, c, d, e, f, g) {
        a.call(this, b, c, d, e, f, g); this.chart.options.chart.type ===
        "map" && this.hasZoom && (a = d.scaleX > d.scaleY, this.pinchTranslateDirection(!a, b, c, d, e, f, g, a ? d.scaleX : d.scaleY))
    }); var E = document.documentElement.style.vectorEffect !== void 0; w.map = o(w.scatter, {
        allAreas: !0, animation: !1, nullColor: "#F8F8F8", borderColor: "silver", borderWidth: 1, marker: null, stickyTracking: !1, dataLabels: { formatter: function () { return this.point.value }, inside: !0, verticalAlign: "middle", crop: !1, overflow: !1, padding: 0 }, turboThreshold: 0, tooltip: { followPointer: !0, pointFormat: "{point.name}: {point.value}<br/>" },
        states: { normal: { animation: !0 }, hover: { brightness: 0.2, halo: null } }
    }); var M = v(u, {
        applyOptions: function (a, b) { var c = u.prototype.applyOptions.call(this, a, b), d = this.series, e = d.joinBy; if (d.mapData) if (e = c[e[1]] !== void 0 && d.mapMap[c[e[1]]]) { if (d.xyFromShape) c.x = e._midX, c.y = e._midY; q(c, e) } else c.value = c.value || null; return c }, setVisible: function (a) { var b = this, c = a ? "show" : "hide"; j(["graphic", "dataLabel"], function (a) { if (b[a]) b[a][c]() }) }, onMouseOver: function (a) {
            clearTimeout(this.colorInterval); if (this.value !== null) u.prototype.onMouseOver.call(this,
            a); else this.series.onMouseOut(a)
        }, onMouseOut: function () {
            var a = this, b = +new Date, c = x(a.color), d = x(a.pointAttr.hover.fill), e = a.series.options.states.normal.animation, f = e && (e.duration || 500), g; if (f && c.rgba.length === 4 && d.rgba.length === 4 && a.state !== "select") g = a.pointAttr[""].fill, delete a.pointAttr[""].fill, clearTimeout(a.colorInterval), a.colorInterval = setInterval(function () { var e = (new Date - b) / f, g = a.graphic; e > 1 && (e = 1); g && g.attr("fill", B.prototype.tweenColors.call(0, d, c, e)); e >= 1 && clearTimeout(a.colorInterval) },
            13); u.prototype.onMouseOut.call(a); if (g) a.pointAttr[""].fill = g
        }, zoomTo: function () { var a = this.series; a.xAxis.setExtremes(this._minX, this._maxX, !1); a.yAxis.setExtremes(this._minY, this._maxY, !1); a.chart.redraw() }
    }); l.map = v(l.scatter, o(C, {
        type: "map", pointClass: M, supportsDrilldown: !0, getExtremesFromAll: !0, useMapGeometry: !0, forceDL: !0, searchPoint: r, preserveAspectRatio: !0, getBox: function (a) {
            var b = Number.MAX_VALUE, c = -b, d = b, e = -b, f = b, g = b, h = this.xAxis, i = this.yAxis, n; j(a || [], function (a) {
                if (a.path) {
                    if (typeof a.path ===
                    "string") a.path = k.splitPath(a.path); var h = a.path || [], i = h.length, j = !1, l = -b, o = b, q = -b, p = b, r = a.properties; if (!a._foundBox) { for (; i--;) typeof h[i] === "number" && !isNaN(h[i]) && (j ? (l = Math.max(l, h[i]), o = Math.min(o, h[i])) : (q = Math.max(q, h[i]), p = Math.min(p, h[i])), j = !j); a._midX = o + (l - o) * (a.middleX || r && r["hc-middle-x"] || 0.5); a._midY = p + (q - p) * (a.middleY || r && r["hc-middle-y"] || 0.5); a._maxX = l; a._minX = o; a._maxY = q; a._minY = p; a.labelrank = m(a.labelrank, (l - o) * (q - p)); a._foundBox = !0 } c = Math.max(c, a._maxX); d = Math.min(d, a._minX);
                    e = Math.max(e, a._maxY); f = Math.min(f, a._minY); g = Math.min(a._maxX - a._minX, a._maxY - a._minY, g); n = !0
                }
            }); if (n) { this.minY = Math.min(f, m(this.minY, b)); this.maxY = Math.max(e, m(this.maxY, -b)); this.minX = Math.min(d, m(this.minX, b)); this.maxX = Math.max(c, m(this.maxX, -b)); if (h && h.options.minRange === void 0) h.minRange = Math.min(5 * g, (this.maxX - this.minX) / 5, h.minRange || b); if (i && i.options.minRange === void 0) i.minRange = Math.min(5 * g, (this.maxY - this.minY) / 5, i.minRange || b) }
        }, getExtremes: function () {
            y.prototype.getExtremes.call(this,
            this.valueData); this.chart.hasRendered && this.isDirtyData && this.getBox(this.options.data); this.valueMin = this.dataMin; this.valueMax = this.dataMax; this.dataMin = this.minY; this.dataMax = this.maxY
        }, translatePath: function (a) { var b = !1, c = this.xAxis, d = this.yAxis, e = c.min, f = c.transA, c = c.minPixelPadding, g = d.min, h = d.transA, d = d.minPixelPadding, i, j = []; if (a) for (i = a.length; i--;) typeof a[i] === "number" ? (j[i] = b ? (a[i] - e) * f + c : (a[i] - g) * h + d, b = !b) : j[i] = a[i]; return j }, setData: function (a, b) {
            var c = this.options, d = c.mapData, e = c.joinBy,
            f = e === null, g = [], h, i, n; f && (e = "_i"); e = this.joinBy = k.splat(e); e[1] || (e[1] = e[0]); a && j(a, function (b, c) { typeof b === "number" && (a[c] = { value: b }); if (f) a[c]._i = c }); this.getBox(a); if (d) {
                if (d.type === "FeatureCollection") { if (d["hc-transform"]) for (h in this.chart.mapTransforms = i = d["hc-transform"], i) if (i.hasOwnProperty(h) && h.rotation) h.cosAngle = Math.cos(h.rotation), h.sinAngle = Math.sin(h.rotation); d = k.geojson(d, this.type, this) } this.getBox(d); this.mapData = d; this.mapMap = {}; for (n = 0; n < d.length; n++) h = d[n], i = h.properties,
                h._i = n, e[0] && i && i[e[0]] && (h[e[0]] = i[e[0]]), this.mapMap[h[e[0]]] = h; c.allAreas && (a = a || [], e[1] && j(a, function (a) { g.push(a[e[1]]) }), g = "|" + g.join("|") + "|", j(d, function (b) { (!e[0] || g.indexOf("|" + b[e[0]] + "|") === -1) && a.push(o(b, { value: null })) }))
            } y.prototype.setData.call(this, a, b)
        }, drawGraph: r, drawDataLabels: r, doFullTranslate: function () { return this.isDirtyData || this.chart.isResizing || this.chart.renderer.isVML || !this.baseTrans }, translate: function () {
            var a = this, b = a.xAxis, c = a.yAxis, d = a.doFullTranslate(); a.generatePoints();
            j(a.data, function (e) { e.plotX = b.toPixels(e._midX, !0); e.plotY = c.toPixels(e._midY, !0); if (d) e.shapeType = "path", e.shapeArgs = { d: a.translatePath(e.path) }, E && (e.shapeArgs["vector-effect"] = "non-scaling-stroke") }); a.translateColors()
        }, drawPoints: function () {
            var a = this, b = a.xAxis, c = a.yAxis, d = a.group, e = a.chart, f = e.renderer, g, h = this.baseTrans; if (!a.transformGroup) a.transformGroup = f.g().attr({ scaleX: 1, scaleY: 1 }).add(d), a.transformGroup.survive = !0; a.doFullTranslate() ? (e.hasRendered && a.pointAttrToOptions.fill === "color" &&
            j(a.points, function (a) { a.graphic && a.graphic.attr("fill", a.color) }), E || j(a.points, function (b) { b = b.pointAttr[""]; b["stroke-width"] === a.pointAttr[""]["stroke-width"] && (b["stroke-width"] = "inherit") }), a.group = a.transformGroup, l.column.prototype.drawPoints.apply(a), a.group = d, j(a.points, function (a) {
                a.graphic && (a.name && a.graphic.addClass("highcharts-name-" + a.name.replace(" ", "-").toLowerCase()), a.properties && a.properties["hc-key"] && a.graphic.addClass("highcharts-key-" + a.properties["hc-key"].toLowerCase()),
                E || (a.graphic["stroke-widthSetter"] = r))
            }), this.baseTrans = { originX: b.min - b.minPixelPadding / b.transA, originY: c.min - c.minPixelPadding / c.transA + (c.reversed ? 0 : c.len / c.transA), transAX: b.transA, transAY: c.transA }, this.transformGroup.animate({ translateX: 0, translateY: 0, scaleX: 1, scaleY: 1 })) : (g = b.transA / h.transAX, d = c.transA / h.transAY, b = b.toPixels(h.originX, !0), c = c.toPixels(h.originY, !0), g > 0.99 && g < 1.01 && d > 0.99 && d < 1.01 && (d = g = 1, b = Math.round(b), c = Math.round(c)), this.transformGroup.animate({
                translateX: b, translateY: c,
                scaleX: g, scaleY: d
            })); E || a.group.element.setAttribute("stroke-width", a.options.borderWidth / (g || 1)); this.drawMapDataLabels()
        }, drawMapDataLabels: function () { y.prototype.drawDataLabels.call(this); this.dataLabelsGroup && this.dataLabelsGroup.clip(this.chart.clipRect) }, render: function () { var a = this, b = y.prototype.render; a.chart.renderer.isVML && a.data.length > 3E3 ? setTimeout(function () { b.call(a) }) : b.call(a) }, animate: function (a) {
            var b = this.options.animation, c = this.group, d = this.xAxis, e = this.yAxis, f = d.pos, g = e.pos;
            if (this.chart.renderer.isSVG) b === !0 && (b = { duration: 1E3 }), a ? c.attr({ translateX: f + d.len / 2, translateY: g + e.len / 2, scaleX: 0.001, scaleY: 0.001 }) : (c.animate({ translateX: f, translateY: g, scaleX: 1, scaleY: 1 }, b), this.animate = null)
        }, animateDrilldown: function (a) {
            var b = this.chart.plotBox, c = this.chart.drilldownLevels[this.chart.drilldownLevels.length - 1], d = c.bBox, e = this.chart.options.drilldown.animation; if (!a) a = Math.min(d.width / b.width, d.height / b.height), c.shapeArgs = { scaleX: a, scaleY: a, translateX: d.x, translateY: d.y }, j(this.points,
            function (a) { a.graphic.attr(c.shapeArgs).animate({ scaleX: 1, scaleY: 1, translateX: 0, translateY: 0 }, e) }), this.animate = null
        }, drawLegendSymbol: G.drawRectangle, animateDrillupFrom: function (a) { l.column.prototype.animateDrillupFrom.call(this, a) }, animateDrillupTo: function (a) { l.column.prototype.animateDrillupTo.call(this, a) }
    })); w.mapline = o(w.map, { lineWidth: 1, fillColor: "none" }); l.mapline = v(l.map, {
        type: "mapline", pointAttrToOptions: { stroke: "color", "stroke-width": "lineWidth", fill: "fillColor", dashstyle: "dashStyle" },
        drawLegendSymbol: l.line.prototype.drawLegendSymbol
    }); w.mappoint = o(w.scatter, { dataLabels: { enabled: !0, formatter: function () { return this.point.name }, crop: !1, defer: !1, overflow: !1, style: { color: "#000000" } } }); l.mappoint = v(l.scatter, { type: "mappoint", forceDL: !0, pointClass: v(u, { applyOptions: function (a, b) { var c = u.prototype.applyOptions.call(this, a, b); a.lat !== void 0 && a.lon !== void 0 && (c = q(c, this.series.chart.fromLatLonToPoint(c))); return c } }) }); if (l.bubble) w.mapbubble = o(w.bubble, { animationLimit: 500, tooltip: { pointFormat: "{point.name}: {point.z}" } }),
    l.mapbubble = v(l.bubble, { pointClass: v(u, { applyOptions: function (a, b) { var c; a.lat !== void 0 && a.lon !== void 0 ? (c = u.prototype.applyOptions.call(this, a, b), c = q(c, this.series.chart.fromLatLonToPoint(c))) : c = M.prototype.applyOptions.call(this, a, b); return c }, ttBelow: !1 }), xyFromShape: !0, type: "mapbubble", pointArrayMap: ["z"], getMapData: l.map.prototype.getMapData, getBox: l.map.prototype.getBox, setData: l.map.prototype.setData }); z.plotOptions.heatmap = o(z.plotOptions.scatter, {
        animation: !1, borderWidth: 0, nullColor: "#F8F8F8",
        dataLabels: { formatter: function () { return this.point.value }, inside: !0, verticalAlign: "middle", crop: !1, overflow: !1, padding: 0 }, marker: null, pointRange: null, tooltip: { pointFormat: "{point.x}, {point.y}: {point.value}<br/>" }, states: { normal: { animation: !0 }, hover: { halo: !1, brightness: 0.2 } }
    }); l.heatmap = v(l.scatter, o(C, {
        type: "heatmap", pointArrayMap: ["y", "value"], hasPointSpecificOptions: !0, supportsDrilldown: !0, getExtremesFromAll: !0, init: function () {
            var a; l.scatter.prototype.init.apply(this, arguments); a = this.options;
            this.pointRange = a.pointRange = m(a.pointRange, a.colsize || 1); this.yAxis.axisPointRange = a.rowsize || 1
        }, translate: function () {
            var a = this.options, b = this.xAxis, c = this.yAxis; this.generatePoints(); j(this.points, function (d) {
                var e = (a.colsize || 1) / 2, f = (a.rowsize || 1) / 2, g = Math.round(b.len - b.translate(d.x - e, 0, 1, 0, 1)), e = Math.round(b.len - b.translate(d.x + e, 0, 1, 0, 1)), h = Math.round(c.translate(d.y - f, 0, 1, 0, 1)), f = Math.round(c.translate(d.y + f, 0, 1, 0, 1)); d.plotX = d.clientX = (g + e) / 2; d.plotY = (h + f) / 2; d.shapeType = "rect"; d.shapeArgs =
                { x: Math.min(g, e), y: Math.min(h, f), width: Math.abs(e - g), height: Math.abs(f - h) }
            }); this.translateColors(); this.chart.hasRendered && j(this.points, function (a) { a.shapeArgs.fill = a.options.color || a.color })
        }, drawPoints: l.column.prototype.drawPoints, animate: r, getBox: r, drawLegendSymbol: G.drawRectangle, getExtremes: function () { y.prototype.getExtremes.call(this, this.valueData); this.valueMin = this.dataMin; this.valueMax = this.dataMax; y.prototype.getExtremes.call(this) }
    })); s.prototype.transformFromLatLon = function (a, b) {
        if (window.proj4 ===
        void 0) return D(21), { x: 0, y: null }; var c = window.proj4(b.crs, [a.lon, a.lat]), d = b.cosAngle || b.rotation && Math.cos(b.rotation), e = b.sinAngle || b.rotation && Math.sin(b.rotation), c = b.rotation ? [c[0] * d + c[1] * e, -c[0] * e + c[1] * d] : c; return { x: ((c[0] - (b.xoffset || 0)) * (b.scale || 1) + (b.xpan || 0)) * (b.jsonres || 1) + (b.jsonmarginX || 0), y: (((b.yoffset || 0) - c[1]) * (b.scale || 1) + (b.ypan || 0)) * (b.jsonres || 1) - (b.jsonmarginY || 0) }
    }; s.prototype.transformToLatLon = function (a, b) {
        if (window.proj4 === void 0) D(21); else {
            var c = {
                x: ((a.x - (b.jsonmarginX ||
                0)) / (b.jsonres || 1) - (b.xpan || 0)) / (b.scale || 1) + (b.xoffset || 0), y: ((-a.y - (b.jsonmarginY || 0)) / (b.jsonres || 1) + (b.ypan || 0)) / (b.scale || 1) + (b.yoffset || 0)
            }, d = b.cosAngle || b.rotation && Math.cos(b.rotation), e = b.sinAngle || b.rotation && Math.sin(b.rotation), c = window.proj4(b.crs, "WGS84", b.rotation ? { x: c.x * d + c.y * -e, y: c.x * e + c.y * d } : c); return { lat: c.y, lon: c.x }
        }
    }; s.prototype.fromPointToLatLon = function (a) {
        var b = this.mapTransforms, c; if (b) {
            for (c in b) if (b.hasOwnProperty(c) && b[c].hitZone && I({ x: a.x, y: -a.y }, b[c].hitZone.coordinates[0])) return this.transformToLatLon(a,
            b[c]); return this.transformToLatLon(a, b["default"])
        } else D(22)
    }; s.prototype.fromLatLonToPoint = function (a) { var b = this.mapTransforms, c, d; if (!b) return D(22), { x: 0, y: null }; for (c in b) if (b.hasOwnProperty(c) && b[c].hitZone && (d = this.transformFromLatLon(a, b[c]), I({ x: d.x, y: -d.y }, b[c].hitZone.coordinates[0]))) return d; return this.transformFromLatLon(a, b["default"]) }; k.geojson = function (a, b, c) {
        var d = [], e = [], f = function (a) { var b = 0, c = a.length; for (e.push("M") ; b < c; b++) b === 1 && e.push("L"), e.push(a[b][0], -a[b][1]) }, b =
        b || "map"; j(a.features, function (a) { var c = a.geometry, i = c.type, c = c.coordinates, a = a.properties, k; e = []; b === "map" || b === "mapbubble" ? (i === "Polygon" ? (j(c, f), e.push("Z")) : i === "MultiPolygon" && (j(c, function (a) { j(a, f) }), e.push("Z")), e.length && (k = { path: e })) : b === "mapline" ? (i === "LineString" ? f(c) : i === "MultiLineString" && j(c, f), e.length && (k = { path: e })) : b === "mappoint" && i === "Point" && (k = { x: c[0], y: -c[1] }); k && d.push(q(k, { name: a.name || a.NAME, properties: a })) }); if (c && a.copyrightShort) c.chart.mapCredits = '<a href="http://www.highcharts.com">Highcharts</a> Â© <a href="' +
        a.copyrightUrl + '">' + a.copyrightShort + "</a>", c.chart.mapCreditsFull = a.copyright; return d
    }; t(s.prototype, "showCredits", function (a, b) { if (z.credits.text === this.options.credits.text && this.mapCredits) b.text = this.mapCredits, b.href = null; a.call(this, b); this.credits && this.credits.attr({ title: this.mapCreditsFull }) }); q(z.lang, { zoomIn: "Zoom in", zoomOut: "Zoom out" }); z.mapNavigation = {
        buttonOptions: {
            alignTo: "plotBox", align: "left", verticalAlign: "top", x: 0, width: 18, height: 18, style: {
                fontSize: "15px", fontWeight: "bold",
                textAlign: "center"
            }, theme: { "stroke-width": 1 }
        }, buttons: { zoomIn: { onclick: function () { this.mapZoom(0.5) }, text: "+", y: 0 }, zoomOut: { onclick: function () { this.mapZoom(2) }, text: "-", y: 28 } }
    }; k.splitPath = function (a) { var b, a = a.replace(/([A-Za-z])/g, " $1 "), a = a.replace(/^\s*/, "").replace(/\s*$/, ""), a = a.split(/[ ,]+/); for (b = 0; b < a.length; b++) /[a-zA-Z]/.test(a[b]) || (a[b] = parseFloat(a[b])); return a }; k.maps = {}; H.prototype.symbols.topbutton = function (a, b, c, d, e) { return J(e, a, b, c, d, e.r, e.r, 0, 0) }; H.prototype.symbols.bottombutton =
    function (a, b, c, d, e) { return J(e, a, b, c, d, 0, 0, e.r, e.r) }; N === K && j(["topbutton", "bottombutton"], function (a) { K.prototype.symbols[a] = H.prototype.symbols[a] }); k.Map = function (a, b) { var c = { endOnTick: !1, gridLineWidth: 0, lineWidth: 0, minPadding: 0, maxPadding: 0, startOnTick: !1, title: null, tickPositions: [] }, d; d = a.series; a.series = null; a = o({ chart: { panning: "xy", type: "map" }, xAxis: c, yAxis: o(c, { reversed: !0 }) }, a, { chart: { inverted: !1, alignTicks: !1 } }); a.series = d; return new s(a, b) }
})(Highcharts);