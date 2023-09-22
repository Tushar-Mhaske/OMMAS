//var colorArr = ["#000066", "#FFFF66", "#003366", "#FFCC66", "#006666", "#FF9966", "#009966", "#FF6666", "#336666", "#993300", "#CC0033", "#6699CC", "#CC66FF",
//                "#66FF00", "#66CCCC", "#66CCFF", "#9900FF", "#FFCC00", "#FF6600", "#FF6666", "#CCFF99", "#CC6699", "#3333CC", "#00FFCC", "#00CCFF", "#0099CC",
//];
var blockCBRPie = null;

var currState, currDistrict, isBlockClick = false;
$(function () {

    var data = Highcharts.geojson(Highcharts.maps['in-all']),
    // Some responsiveness
    small = $('#container').width() < 400;

    // Set drilldown pointers
    $.each(data, function (i) {
        this.drilldown = this.properties['hc-key'];
        this.value = 0; // You can assign dynamic here to show on tooltip
        this.borderColor = "#FFFFFF";
        this.borderWidth = 1.5;
    });


    // Instantiate the map
    $('#container').highcharts('Map', {
        chart: {
            events: {
                drilldown: function (e) {

                    if (!e.seriesOptions) {
                        var chart = this,
                            mapKey = '' + e.point.drilldown + '-all',
                            // Handle error, the timeout is cleared on success
                            fail = setTimeout(function () {
                                if (!Highcharts.maps[mapKey]) {
                                    chart.showLoading('<i class="icon-frown"></i> Failed loading ' + e.point.name);

                                    fail = setTimeout(function () {
                                        chart.hideLoading();
                                    }, 1000);
                                }
                            }, 3000);

                        // Show the spinner
                        chart.showLoading('<i class="icon-spinner icon-spin icon-3x"></i>'); // Font Awesome spinner

                        // Load the drilldown map of 1st Level
                        $.getScript('/Scripts/Maps/Highmaps/' + mapKey + '.js', function () {

                            data = Highcharts.geojson(Highcharts.maps[mapKey]);

                            var cbrData = [];
                            var isFirstCall = true;
                            $.each(data, function (i) {
                                this.drilldown = this.properties['hc-key'];
                                var type = this.properties['TYPE_2'];
                                var currId = this.properties['ID_3'];

                                //console.log("currState " + currState + " currDistrict " + currDistrict);
                                if (type != "District") {

                                    currState = this.properties['ID_1'];
                                    currDistrict = this.properties['ID_2'];
                                    var currName = this.properties['name'];

                                    // Ajax call to get CBR Data
                                    if (isFirstCall) { //On only first call send request on server side
                                        $.ajax({
                                            url: "/Home/GetCBRData",
                                            dataType: 'json',
                                            type: "POST",
                                            async: false,
                                            data: { state: currState, district: currDistrict },
                                            success: function (response) {
                                                isFirstCall = false;   //call already sent to server & got response, so set this flag to false
                                                $.each(response, function (key, val) {
                                                    cbrData.push({
                                                        LOCATION_CODE: val.LOCATION_CODE,
                                                        LOCATION_NAME: val.LOCATION_NAME,
                                                        CBR_MIN: val.CBR_MIN,
                                                        CBR_MAX: val.CBR_MAX,
                                                        CBR_AVG: val.CBR_AVG
                                                    });
                                                });
                                            }
                                        });
                                    }
                                    $.each(cbrData, function (index, value) {
                                        if (currId == value.LOCATION_CODE) {
                                            data.name = this.LOCATION_NAME + "<br/>Min : " + this.CBR_MIN + "<br/>Max : " + this.CBR_MAX;
                                            data.value = this.CBR_AVG;
                                        }
                                    });

                                    if (currId == 0) {
                                        this.name = "NA";
                                        this.value = 0;
                                    }
                                    else {
                                        this.value = data.value;
                                        this.name = data.name;
                                        //this.color = this.properties['COLOR_2'];
                                    }
                                    isBlockClick = true;
                                } //--type != district
                                else {
                                    isBlockClick = false;
                                }
                                this.borderColor = "#FFFFFF";
                                this.borderWidth = 2;

                            });

                            // Hide loading and add series
                            chart.hideLoading();
                            clearTimeout(fail);
                            chart.addSeriesAsDrilldown(e.point, {
                                name: e.point.name,
                                data: data,
                                dataLabels: {
                                    enabled: true,
                                    style: {
                                        fontSize: '10.5px',
                                        width: '100px',
                                        fontWeight: "bold"
                                    },
                                    format: '{point.name}'
                                },

                                events: {
                                    click: function (e) {
                                        var state = e.point.properties["ID_1"];
                                        var district = e.point.properties["ID_2"];
                                        var block = e.point.properties["ID_3"];
                                        var currType = e.point.properties["TYPE_3"];
                                        var currName = e.point.properties["name"];
                                        //console.log("IN --" + state + "--" + district + "--" + block + "--" + currType);
                                        if (currType == "Taluk") {
                                            if (block > 0) {
                                                $("#divDetailChart").show("slow");

                                                renderGrid(state, district, block);
                                                renderColumnChart(state, district, block, blockCBRPie, "colChartContainer");

                                                $("#spnLabelChart").html("Subgrade CBR Pattern (Last 5 Years) for " + currName);
                                                $("#spnLabelChart").css('width', '395px');
                                            }
                                        }
                                    }
                                }

                                //-----------------------------------------------------------
                                // 2nd level Drilldown
                                //events: {
                                //    drilldown: function (e) {
                                //        if (!e.seriesOptions) {

                                //            var chart = this,
                                //                mapKey = '' + e.point.drilldown + '-all',
                                //                // Handle error, the timeout is cleared on success
                                //                fail = setTimeout(function () {
                                //                    if (!Highcharts.maps[mapKey]) {
                                //                        chart.showLoading('<i class="icon-frown"></i> Failed loading ' + e.point.name);

                                //                        fail = setTimeout(function () {
                                //                            chart.hideLoading();
                                //                        }, 1000);
                                //                    }
                                //                }, 3000);

                                //            // Show the spinner
                                //            chart.showLoading('<i class="icon-spinner icon-spin icon-3x"></i>'); // Font Awesome spinner
                                //            $.getScript('/Scripts/Districts/' + mapKey + '.js', function () {
                                //                data = Highcharts.geojson(Highcharts.maps[mapKey]);

                                //                // Set a non-random bogus value
                                //                $.each(data, function (i) {
                                //                    this.value = i;
                                //                });

                                //                // Hide loading and add series
                                //                chart.hideLoading();
                                //                clearTimeout(fail);
                                //                chart.addSeriesAsDrilldown(e.point, {
                                //                    name: e.point.name,
                                //                    data: data,
                                //                    dataLabels: {
                                //                        enabled: true,
                                //                        format: '{point.name}'
                                //                    }

                                //                });// --addSeriesAsDrilldown
                                //            });// -- $.getScript of 2nd level
                                //        }//-- If of 2nd Level DrillDown
                                //    }// -- drilldown of 2nd level
                                //}// -- Events of 2nd level
                                //-----------------------------------------------------------


                            }); // -- chart.addSeriesAsDrilldown

                        });// -- $.getScript of 1st level
                    }// -- If of 1st Level DrillDown

                    this.setTitle(null, { text: e.point.name });
                },
                drillup: function () {
                    this.setTitle(null, { text: 'India' });

                    $("#divDetailChart").hide("slow");
                    //$("#tblContainer").hide("slow");
                    //$("#colChartContainer").hide("slow");
                    //console.log(this.options.series[0].data[0].name);
                }
            }// --events ends
        },

        //colors: ['#ddd', '#FFF', '#333', '#EED18A'],
        title: {
            text: 'CBR Details',
            style: {
                fontSize: '15px'
            }
        },

        subtitle: {
            text: 'India',
            floating: true,
            align: 'right',
            y: 50,
            style: {
                fontSize: '14px'
            }
        },

        legend: small ? {} : {
            layout: 'vertical',
            align: 'left',
            verticalAlign: 'middle'
        },

        colorAxis: {
            min: 0,
            max: 20,
            //minColor: '#E6E7E8', //'#E6E7E8',
            //maxColor: '#4f8a7e' //'#005645'
            dataClasses: [{
                from: 0,
                to: 2.99,
                name: '<3',
                color: "#3B9C9C"
            }, {
                from: 3,
                to: 4.99,
                name: '3 - 4.99',
                color: "#368BC1"
            }, {
                from: 5,
                to: 6.99,
                name: '5 - 6.99',
                color: "#3B9C9C"
            }, {
                from: 7,
                to: 8.99,
                name: '7 - 8.99',
                color: "#FFE87C"
            }, {
                from: 9,
                to: 10.99,
                name: '9 - 10.99',
                color: "#F9966B"
            }, {
                from: 11,
                to: 30,
                name: '>=11',
                color: "#9900FF"
            }]
        },

        mapNavigation: {
            enabled: true,
            buttonOptions: {
                verticalAlign: 'bottom'
            }
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b>';
            }
        },
        plotOptions: {
            map: {
                states: {
                    hover: {
                        //enabled:false
                        color: '#EEDD66'
                    }
                }
            }
        },

        series: [{
            data: data,
            name: 'India',
            //nullColor: 'red',
            dataLabels: {
                enabled: true,
                format: '{point.properties.name}'
            }
        }],

        drilldown: {
            activeDataLabelStyle: {
                color: '#FFFFFF',
                textDecoration: 'none',
                textShadow: '0 0 3px #000000'
            },
            drillUpButton: {
                relativeTo: 'spacingBox',
                position: {
                    x: 0,
                    y: 60
                }
            }
        }
    });

    $('#colChartContainerOuter').bind('dblclick', function () {
        $(this).toggleClass('modal-chart');
        $('.chart', this).highcharts().reflow();
    });
}); //function() ends



function renderColumnChart(state, district, block, chart, containerID) {
    $.ajax({
        type: "POST",
        url: '/Home/GetCBRColumnChartData?' + Math.random(),
        data: { "state": state, "district": district, "block": block },
        error: function (xhr, status, error) {
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null; var series2 = null; var series3 = null; var series4 = null;

            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                series1 = {
                    data: []
                };
                series2 = {
                    data: []
                };
                series3 = {
                    data: []
                };
                series4 = {
                    data: []
                };


                $.each(data, function (item) {
                    series1.name = "No. of Roads (New Connectivity)";
                    series1.type = "column";
                    series1.data.push({ x: this.SrNo, y: parseFloat(this.NCount) });

                    series2.name = "Percentage (New Connectivity)";
                    series2.type = "column";
                    series2.data.push({ x: this.SrNo, y: parseFloat(this.NPerc) });

                    series3.name = "No. of Roads (Upgradation)";
                    series3.type = "column";
                    series3.data.push({ x: this.SrNo, y: parseFloat(this.UCount) });

                    series4.name = "Percentage (Upgradation)";
                    series4.type = "column";
                    series4.data.push({ x: this.SrNo, y: parseFloat(this.UPerc) });
                });

                optionsColumn = CommonOptionsForColumnChart(containerID);
                optionsColumn.series.push(series1);
                optionsColumn.series.push(series2);
                optionsColumn.series.push(series3);
                optionsColumn.series.push(series4);

                chart = new Highcharts.Chart(optionsColumn);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
                chart.series[3].setVisible(true, true);
            }

        }
    });
}


function CommonOptionsForColumnChart(containerDivID) {
    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID
            },
        title: {
            text: '',
            x: -20 //center
        },
        subtitle: {
            text: 'Double Click on Chart to Resize',
            x: -20
        },
        xAxis: {
            title: {
                text: 'Sr No'
            },
            gridLineWidth: 1,
            categories: []
        },
        yAxis: [{
            title: {
                text: 'No. of Roads & %'
            },
            min: 0,
            labels: {
                format: '{value}',
                style: {
                    color: '#4572A7'
                }
            }
        }],
        tooltip:
            {
                headerFormat: '<span style="font-size:11px;"> Sr No: {point.key} </span>' +
                              '<table>',
                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
        //legend: {
        //    layout: 'horizontal',
        //    align: 'right',
        //    verticalAlign: 'middle',
        //    borderWidth: 0
        //},
        series: [],
        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248']

    };

    return options;
}


function renderGrid(state, district, block) {
    $.ajax({
        url: "/Home/GetCBRGridData?" + Math.random(),
        dataType: 'json',
        type: "POST",
        async: false,
        data: { state: state, district: district, block: block },
        success: function (result) {
            //if table is already on the page
            var table = $("#tblCBR");
            table.find("tr").remove(); //remove all previous rows if needed

            $.each(result, function (i, item) {
                var tr = $("<tr></tr>");
                table.append(tr);

                var td = $("<td>" + item.SrNo + "</td>");
                tr.append(td);

                var td = $("<td>" + item.RangeName + "</td>");
                tr.append(td);

                var td = $("<td>" + item.NCount + "</td>");
                tr.append(td);

                var td = $("<td>" + item.NPerc + "</td>");
                tr.append(td);

                var td = $("<td>" + item.UCount + "</td>");
                tr.append(td);

                var td = $("<td>" + item.UPerc + "</td>");
                tr.append(td);
            });
        }
    });
}