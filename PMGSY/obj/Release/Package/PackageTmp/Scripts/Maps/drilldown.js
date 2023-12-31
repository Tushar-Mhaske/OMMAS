﻿(function (f) {
    function A(a, b, c) { var d; !b.rgba.length || !a.rgba.length ? a = b.raw || "none" : (a = a.rgba, b = b.rgba, d = b[3] !== 1 || a[3] !== 1, a = (d ? "rgba(" : "rgb(") + Math.round(b[0] + (a[0] - b[0]) * (1 - c)) + "," + Math.round(b[1] + (a[1] - b[1]) * (1 - c)) + "," + Math.round(b[2] + (a[2] - b[2]) * (1 - c)) + (d ? "," + (b[3] + (a[3] - b[3]) * (1 - c)) : "") + ")"); return a } var t = function () { }, q = f.getOptions(), h = f.each, l = f.extend, B = f.format, u = f.pick, r = f.wrap, m = f.Chart, p = f.seriesTypes, v = p.pie, n = p.column, w = f.Tick, x = HighchartsAdapter.fireEvent, y = HighchartsAdapter.inArray,
    z = 1; h(["fill", "stroke"], function (a) { HighchartsAdapter.addAnimSetter(a, function (b) { b.elem.attr(a, A(f.Color(b.start), f.Color(b.end), b.pos)) }) }); l(q.lang, { drillUpText: "<< Back to {series.name}" }); q.drilldown = { activeAxisLabelStyle: { cursor: "pointer", color: "#0d233a", fontWeight: "bold", textDecoration: "underline" }, activeDataLabelStyle: { cursor: "pointer", color: "#0d233a", fontWeight: "bold", textDecoration: "underline" }, animation: { duration: 500 }, drillUpButton: { position: { align: "right", x: -10, y: 10 } } }; f.SVGRenderer.prototype.Element.prototype.fadeIn =
    function (a) { this.attr({ opacity: 0.1, visibility: "inherit" }).animate({ opacity: u(this.newOpacity, 1) }, a || { duration: 250 }) }; m.prototype.addSeriesAsDrilldown = function (a, b) { this.addSingleSeriesAsDrilldown(a, b); this.applyDrilldown() }; m.prototype.addSingleSeriesAsDrilldown = function (a, b) {
        var c = a.series, d = c.xAxis, g = c.yAxis, e; e = a.color || c.color; var i, f = [], j = [], k, o; if (!this.drilldownLevels) this.drilldownLevels = []; k = c.options._levelNumber || 0; (o = this.drilldownLevels[this.drilldownLevels.length - 1]) && o.levelNumber !==
        k && (o = void 0); b = l({ color: e, _ddSeriesId: z++ }, b); i = y(a, c.points); h(c.chart.series, function (a) { if (a.xAxis === d && !a.isDrilling) a.options._ddSeriesId = a.options._ddSeriesId || z++, a.options._colorIndex = a.userOptions._colorIndex, a.options._levelNumber = a.options._levelNumber || k, o ? (f = o.levelSeries, j = o.levelSeriesOptions) : (f.push(a), j.push(a.options)) }); e = {
            levelNumber: k, seriesOptions: c.options, levelSeriesOptions: j, levelSeries: f, shapeArgs: a.shapeArgs, bBox: a.graphic ? a.graphic.getBBox() : {}, color: e, lowerSeriesOptions: b,
            pointOptions: c.options.data[i], pointIndex: i, oldExtremes: { xMin: d && d.userMin, xMax: d && d.userMax, yMin: g && g.userMin, yMax: g && g.userMax }
        }; this.drilldownLevels.push(e); e = e.lowerSeries = this.addSeries(b, !1); e.options._levelNumber = k + 1; if (d) d.oldPos = d.pos, d.userMin = d.userMax = null, g.userMin = g.userMax = null; if (c.type === e.type) e.animate = e.animateDrilldown || t, e.options.animation = !0
    }; m.prototype.applyDrilldown = function () {
        var a = this.drilldownLevels, b; if (a && a.length > 0) b = a[a.length - 1].levelNumber, h(this.drilldownLevels,
        function (a) { a.levelNumber === b && h(a.levelSeries, function (a) { a.options && a.options._levelNumber === b && a.remove(!1) }) }); this.redraw(); this.showDrillUpButton()
    }; m.prototype.getDrilldownBackText = function () { var a = this.drilldownLevels; if (a && a.length > 0) return a = a[a.length - 1], a.series = a.seriesOptions, B(this.options.lang.drillUpText, a) }; m.prototype.showDrillUpButton = function () {
        var a = this, b = this.getDrilldownBackText(), c = a.options.drilldown.drillUpButton, d, g; this.drillUpButton ? this.drillUpButton.attr({ text: b }).align() :
        (g = (d = c.theme) && d.states, this.drillUpButton = this.renderer.button(b, null, null, function () { a.drillUp() }, d, g && g.hover, g && g.select).attr({ align: c.position.align, zIndex: 9 }).add().align(c.position, !1, c.relativeTo || "plotBox"))
    }; m.prototype.drillUp = function () {
        for (var a = this, b = a.drilldownLevels, c = b[b.length - 1].levelNumber, d = b.length, g = a.series, e, i, f, j, k = function (b) {
        var c; h(g, function (a) { a.options._ddSeriesId === b._ddSeriesId && (c = a) }); c = c || a.addSeries(b, !1); if (c.type === f.type && c.animateDrillupTo) c.animate = c.animateDrillupTo;
        b === i.seriesOptions && (j = c)
        }; d--;) if (i = b[d], i.levelNumber === c) {
            b.pop(); f = i.lowerSeries; if (!f.chart) for (e = g.length; e--;) if (g[e].options.id === i.lowerSeriesOptions.id && g[e].options._levelNumber === c + 1) { f = g[e]; break } f.xData = []; h(i.levelSeriesOptions, k); x(a, "drillup", { seriesOptions: i.seriesOptions }); if (j.type === f.type) j.drilldownLevel = i, j.options.animation = a.options.drilldown.animation, f.animateDrillupFrom && f.chart && f.animateDrillupFrom(i); j.options._levelNumber = c; f.remove(!1); if (j.xAxis) e = i.oldExtremes,
            j.xAxis.setExtremes(e.xMin, e.xMax, !1), j.yAxis.setExtremes(e.yMin, e.yMax, !1)
        } this.redraw(); this.drilldownLevels.length === 0 ? this.drillUpButton = this.drillUpButton.destroy() : this.drillUpButton.attr({ text: this.getDrilldownBackText() }).align(); this.ddDupes.length = []
    }; n.prototype.supportsDrilldown = !0; n.prototype.animateDrillupTo = function (a) {
        if (!a) {
            var b = this, c = b.drilldownLevel; h(this.points, function (a) { a.graphic && a.graphic.hide(); a.dataLabel && a.dataLabel.hide(); a.connector && a.connector.hide() }); setTimeout(function () {
                b.points &&
                h(b.points, function (a, b) { var e = b === (c && c.pointIndex) ? "show" : "fadeIn", f = e === "show" ? !0 : void 0; if (a.graphic) a.graphic[e](f); if (a.dataLabel) a.dataLabel[e](f); if (a.connector) a.connector[e](f) })
            }, Math.max(this.chart.options.drilldown.animation.duration - 50, 0)); this.animate = t
        }
    }; n.prototype.animateDrilldown = function (a) {
        var b = this, c = this.chart.drilldownLevels, d, g = this.chart.options.drilldown.animation, e = this.xAxis; if (!a) h(c, function (a) {
            if (b.options._ddSeriesId === a.lowerSeriesOptions._ddSeriesId) d = a.shapeArgs,
            d.fill = a.color
        }), d.x += u(e.oldPos, e.pos) - e.pos, h(this.points, function (a) { a.graphic && a.graphic.attr(d).animate(l(a.shapeArgs, { fill: a.color }), g); a.dataLabel && a.dataLabel.fadeIn(g) }), this.animate = null
    }; n.prototype.animateDrillupFrom = function (a) {
        var b = this.chart.options.drilldown.animation, c = this.group, d = this; h(d.trackerGroups, function (a) { if (d[a]) d[a].on("mouseover") }); delete this.group; h(this.points, function (d) {
            var e = d.graphic, i = function () { e.destroy(); c && (c = c.destroy()) }; e && (delete d.graphic, b ? e.animate(l(a.shapeArgs,
            { fill: a.color }), f.merge(b, { complete: i })) : (e.attr(a.shapeArgs), i()))
        })
    }; v && l(v.prototype, {
        supportsDrilldown: !0, animateDrillupTo: n.prototype.animateDrillupTo, animateDrillupFrom: n.prototype.animateDrillupFrom, animateDrilldown: function (a) {
            var b = this.chart.drilldownLevels[this.chart.drilldownLevels.length - 1], c = this.chart.options.drilldown.animation, d = b.shapeArgs, g = d.start, e = (d.end - g) / this.points.length; if (!a) h(this.points, function (a, h) {
                a.graphic.attr(f.merge(d, { start: g + h * e, end: g + (h + 1) * e, fill: b.color }))[c ?
                "animate" : "attr"](l(a.shapeArgs, { fill: a.color }), c)
            }), this.animate = null
        }
    }); f.Point.prototype.doDrilldown = function (a, b) {
        var c = this.series.chart, d = c.options.drilldown, f = (d.series || []).length, e; if (!c.ddDupes) c.ddDupes = []; for (; f-- && !e;) d.series[f].id === this.drilldown && y(this.drilldown, c.ddDupes) === -1 && (e = d.series[f], c.ddDupes.push(this.drilldown)); x(c, "drilldown", { point: this, seriesOptions: e, category: b, points: b !== void 0 && this.series.xAxis.ddPoints[b].slice(0) }); e && (a ? c.addSingleSeriesAsDrilldown(this, e) :
        c.addSeriesAsDrilldown(this, e))
    }; f.Axis.prototype.drilldownCategory = function (a) { var b, c, d = this.ddPoints[a]; for (b in d) (c = d[b]) && c.series && c.series.visible && c.doDrilldown && c.doDrilldown(!0, a); this.chart.applyDrilldown() }; f.Axis.prototype.getDDPoints = function (a, b) { var c = this.ddPoints; if (!c) this.ddPoints = c = {}; c[a] || (c[a] = []); if (c[a].levelNumber !== b) c[a].length = 0; return c[a] }; w.prototype.drillable = function () {
        var a = this.pos, b = this.label, c = this.axis, d = c.ddPoints && c.ddPoints[a]; if (b && d && d.length) {
            if (!b.basicStyles) b.basicStyles =
            f.merge(b.styles); b.addClass("highcharts-drilldown-axis-label").css(c.chart.options.drilldown.activeAxisLabelStyle).on("click", function () { c.drilldownCategory(a) })
        } else if (b && b.basicStyles) b.styles = {}, b.css(b.basicStyles), b.on("click", null)
    }; r(w.prototype, "addLabel", function (a) { a.call(this); this.drillable() }); r(f.Point.prototype, "init", function (a, b, c, d) {
        var g = a.call(this, b, c, d), a = (c = b.xAxis) && c.ticks[d], d = c && c.getDDPoints(d, b.options._levelNumber); if (g.drilldown && (f.addEvent(g, "click", function () { g.doDrilldown() }),
        d)) d.push(g), d.levelNumber = b.options._levelNumber; a && a.drillable(); return g
    }); r(f.Series.prototype, "drawDataLabels", function (a) { var b = this.chart.options.drilldown.activeDataLabelStyle; a.call(this); h(this.points, function (a) { a.drilldown && a.dataLabel && a.dataLabel.attr({ "class": "highcharts-drilldown-data-label" }).css(b) }) }); var s, q = function (a) { a.call(this); h(this.points, function (a) { a.drilldown && a.graphic && a.graphic.attr({ "class": "highcharts-drilldown-point" }).css({ cursor: "pointer" }) }) }; for (s in p) p[s].prototype.supportsDrilldown &&
    r(p[s].prototype, "drawTracker", q)
})(Highcharts);