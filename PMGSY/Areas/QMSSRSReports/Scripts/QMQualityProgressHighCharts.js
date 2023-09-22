

var defectiveGradingLineChart = null;
$(document).ready(function () {

    createLineChart(defectiveGradingLineChart, "divLineChart");
  

    $("#btnViewReport").click(function () {

        createLineChart(defectiveGradingLineChart, "divLineChart");
    });
});


//function to get the asset liability chart
function createLineChart(chart, containerID)
{
   // alert("stateCode"+ $("#StateCode").val()+ "year"+ $("#Year").val());
    //var valueType = $('input[name=ValueType]:checked', '#qmDefectiveGradingReportForm').val();
    $.ajax({
        url: '/QMSSRSReports/QMSSRSReports/QMQualityProfileLineChart/',
        type: 'POST',
        data: { "stateCode": $("#StateCode").val(), "year": $("#Year").val() },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null;
            var series2 = null;
            var series3 = null;
            var series4 = null;
            var series5 = null;

            if (data == "")
            {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }

            else
            {

                series1 = {
                    name: "Total Inspection",
                    data: []
                };

                series2 = {
                    name: "Total ATRs Required (RI+U)",
                    data: []
                };

                series3 = {
                    name: "Total ATRs Submitted (RI+U)",
                    data: []
                };

                series4 = {
                    name: "Total ATRs Required (U)",
                    data: []
                };

                series5 = {
                    name: "Total ATRs Submitted (U)",
                    data: []
                };
                                
                $.each(data, function (item) {
                  //  alert(this.INSP_MONTH_NAME + ": " + item.INSP_MONTH_NAME);
                    //alert(this.PARAM_TYPE);
                    if (this.PARAM_TYPE == "TOT_INSP") {
                        // series1.data.push({  y: this.TOT_INSP });
                        series1.data.push({ y: this.APR });
                        series1.data.push({ y: this.MAY });
                        series1.data.push({ y: this.JUN });
                        series1.data.push({ y: this.JUL });
                        series1.data.push({ y: this.AUG });
                        series1.data.push({ y: this.SEP });
                        series1.data.push({ y: this.OCT });
                        series1.data.push({ y: this.NOV });
                        series1.data.push({ y: this.DEC });
                        series1.data.push({ y: this.JAN });
                        series1.data.push({ y: this.FEB });
                        series1.data.push({ y: this.MAR });
                    }
                  //  series1.dashStyle = 'ShortDash';
                });

                $.each(data, function (item) {
                    // series2.data.push({  y: this.SRI_U_REQUIRED });
                    if (this.PARAM_TYPE == "SRI_U_REQUIRED") {
                        series2.data.push({ y: this.APR });
                        series2.data.push({ y: this.MAY });
                        series2.data.push({ y: this.JUN });
                        series2.data.push({ y: this.JUL });
                        series2.data.push({ y: this.AUG });
                        series2.data.push({ y: this.SEP });
                        series2.data.push({ y: this.OCT });
                        series2.data.push({ y: this.NOV });
                        series2.data.push({ y: this.DEC });
                        series2.data.push({ y: this.JAN });
                        series2.data.push({ y: this.FEB });
                        series2.data.push({ y: this.MAR });
                    }
                });

                $.each(data, function (item) {
                    //    series3.data.push({ y: this.SRI_U_SUBMITTED });
                    if (this.PARAM_TYPE == "SRI_U_SUBMITTED") {

                        series3.data.push({ y: this.APR });
                        series3.data.push({ y: this.MAY });
                        series3.data.push({ y: this.JUN });
                        series3.data.push({ y: this.JUL });
                        series3.data.push({ y: this.AUG });
                        series3.data.push({ y: this.SEP });
                        series3.data.push({ y: this.OCT });
                        series3.data.push({ y: this.NOV });
                        series3.data.push({ y: this.DEC });
                        series3.data.push({ y: this.JAN });
                        series3.data.push({ y: this.FEB });
                        series3.data.push({ y: this.MAR });
                    }
                   // series3.dashStyle = 'ShortDash';
                });

                $.each(data, function (item) {
                    // series4.data.push({  y: this.U_REQUIRED });
                    if (this.PARAM_TYPE == "U_REQUIRED") {

                        series4.data.push({ y: this.APR });
                        series4.data.push({ y: this.MAY });
                        series4.data.push({ y: this.JUN });
                        series4.data.push({ y: this.JUL });
                        series4.data.push({ y: this.AUG });
                        series4.data.push({ y: this.SEP });
                        series4.data.push({ y: this.OCT });
                        series4.data.push({ y: this.NOV });
                        series4.data.push({ y: this.DEC });
                        series4.data.push({ y: this.JAN });
                        series4.data.push({ y: this.FEB });
                        series4.data.push({ y: this.MAR });
                    }
                });
                $.each(data, function (item) {
                    //series5.data.push({  y: this.U_SUBMITTED });
                    if (this.PARAM_TYPE == "U_SUBMITTED") {
                        series5.data.push({ y: this.APR });
                        series5.data.push({ y: this.MAY });
                        series5.data.push({ y: this.JUN });
                        series5.data.push({ y: this.JUL });
                        series5.data.push({ y: this.AUG });
                        series5.data.push({ y: this.SEP });
                        series5.data.push({ y: this.OCT });
                        series5.data.push({ y: this.NOV });
                        series5.data.push({ y: this.DEC });
                        series5.data.push({ y: this.JAN });
                        series5.data.push({ y: this.FEB });
                        series5.data.push({ y: this.MAR });
                    }
                });

                optionsPie = CommonOptionsForDefectiveGradingLineChart(containerID);
                optionsPie.series.push(series1);
                optionsPie.series.push(series2);
                optionsPie.series.push(series3);
                optionsPie.series.push(series4);
                optionsPie.series.push(series5);

                //optionsPie.xAxis.categories.push("");

                $.each(data, function (item) {
                   // series5.data.push({ x: this.INSP_MONTH_NAME, y: this.U_SUBMITTED });
                    // optionsPie.xAxis.categories.push(this.INSP_MONTH_NAME);
                    optionsPie.xAxis.categories.push("APR");
                    optionsPie.xAxis.categories.push("MAY");
                    optionsPie.xAxis.categories.push("JUN");
                    optionsPie.xAxis.categories.push("JUL");
                    optionsPie.xAxis.categories.push("AUG");
                    optionsPie.xAxis.categories.push("SEP");
                    optionsPie.xAxis.categories.push("OCT");
                    optionsPie.xAxis.categories.push("NOV");
                    optionsPie.xAxis.categories.push("DEC");
                    optionsPie.xAxis.categories.push("JAN");
                    optionsPie.xAxis.categories.push("FEB");
                    optionsPie.xAxis.categories.push("MAR");


                });
                //optionsPie.xAxis.categories.push("Quarter 1");
                //optionsPie.xAxis.categories.push("Quarter 2");
                //optionsPie.xAxis.categories.push("Quarter 3");
                //optionsPie.xAxis.categories.push("Quarter 4");


                //if (valueType == 'P') {
                //    optionsPie.yAxis.max = 100;
                //}

                chart = new Highcharts.Chart(optionsPie);
                //// code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
                chart.series[3].setVisible(true, true);
                chart.series[4].setVisible(true, true);

                //chart.setTitle({ text: "Quarterly Defective Grading" });
            }
        }
    });
}



function CommonOptionsForDefectiveGradingLineChart(containerDivID) {

  //  var valueType = $('input[name=ValueType]:checked', '#qmDefectiveGradingReportForm').val();

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }
    //$('#ddlQMPahse option:selected').text()
    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: 'line',
            },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                },
                //enableMouseTracking: false
            }
        },
        title: {
            text: 'Quality Profile Graph for year ' + $("#Year").val() + "-" + (parseInt($("#Year").val()) + 1),
            style: {
                color: '#3E576F',
                fontSize: '13px'
            }
        },
        subtitle: {
            text: null
        },
        xAxis:
      {
          categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
      },
        xAxis: {
            gridLineWidth: 1,
            categories: [],
        },

        yAxis: {
            title: {
                text: ''
            },
            min: 0,

            labels: {
                format: '{value}',
                style: {
                    color: '#4572A7'
                }
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        },
      

        legend: {
            symbolWidth: 30
        },
        //tooltip:
        //    {
        //        formatter: function () {
        //            var param1, param2;
        //            if (this. x != undefined)
        //                param1 = this.x;
        //            if (this.y != undefined)
        //                param2 = this.y;

        //            return '<span style="padding:0;font-size:11px;"> <b>' + this.series.name + '</b><br/>' + (valueType == 'V' ? 'Value for ' : 'Percentage for ') + param1 + ' is <b>' + param2 + '</b>' + (valueType == 'V' ? '' : '%') + '</span>';
        //        }
        //    },

        series: [],
        colors: ['#FFA500', '#87CEFA', '#ADFF2F', '#4B0082', '#FF0000'],

    };

    return options;
}

//---------- Line Chart ---------------//