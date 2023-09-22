/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   DefectiveGradingReport.js
    * Description   :   js functions related to Defective Grading Line Chart Report
    * Author        :   Shyam Yadav
    * Creation Date :   02/Dec/2014
*/

var defectiveGradingLineChart = null;
$(document).ready(function () {

    createDefectiveGradingLineChart(defectiveGradingLineChart, "divDefectiveGradingLineChart");

    $("#btnViewReport").click(function () {

        createDefectiveGradingLineChart(defectiveGradingLineChart, "divDefectiveGradingLineChart");
    });
});


//function to get the asset liability chart
function createDefectiveGradingLineChart(chart, containerID) {
    var valueType = $('input[name=ValueType]:checked', '#qmDefectiveGradingReportForm').val();
    $.ajax({
        type: "POST",
        url: '/QualityMonitoring/DefectiveGradingLineChart/',
        data: { "state": $("#ddlViewDefectiveGradingState").val(), "year": $("#ddlViewDefectiveGradingYear").val(), "rdStatus": $("#ddlViewDefectiveGradingRdStatus").val(), "valueType": valueType },
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
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                series1 = {
                    name: "NQM U",
                    data: []
                };

                series2 = {
                    name: "NQM SRI+U",
                    data: []
                };

                series3 = {
                    name: "SQM U",
                    data: []
                };

                series4 = {
                    name: "SQM SRI+U",
                    data: []
                };

                $.each(data, function (item) {
                    series1.data.push({ x: this.Quarter, y: this.NQMUCount });
                    series1.dashStyle = 'ShortDash';
                });

                $.each(data, function (item) {
                    series2.data.push({ x: this.Quarter, y: this.NQMSRICount });
                });

                $.each(data, function (item) {
                    series3.data.push({ x: this.Quarter, y: this.SQMUCount });
                    series3.dashStyle = 'ShortDash';
                });

                $.each(data, function (item) {
                    series4.data.push({ x: this.Quarter, y: this.SQMSRICount });
                });
               
                optionsPie = CommonOptionsForDefectiveGradingLineChart(containerID);
                optionsPie.series.push(series1);
                optionsPie.series.push(series2);
                optionsPie.series.push(series3);
                optionsPie.series.push(series4);
                optionsPie.xAxis.categories.push("");
                optionsPie.xAxis.categories.push("Quarter 1");
                optionsPie.xAxis.categories.push("Quarter 2");
                optionsPie.xAxis.categories.push("Quarter 3");
                optionsPie.xAxis.categories.push("Quarter 4");
                if (valueType == 'P') {
                    optionsPie.yAxis.max = 100;
                }

                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
                chart.series[3].setVisible(true, true);
                chart.setTitle({ text: "Quarterly Defective Grading" });
            }
        }
    });
}



function CommonOptionsForDefectiveGradingLineChart(containerDivID) {

    var valueType = $('input[name=ValueType]:checked', '#qmDefectiveGradingReportForm').val();
    
    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: 'line',
            },
        title: {
            text: 'Quarterly Defective Grading',
            style: {
                color: '#3E576F',
                fontSize: '13px'
            }
        },
        subtitle: {
            text: null
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
            symbolWidth:30
        },
        tooltip:
            {
                formatter: function () {
                    var param1, param2;
                    if (this.x != undefined)
                        param1 = this.x;
                    if (this.y != undefined)
                        param2 = this.y;

                    return '<span style="padding:0;font-size:11px;"> <b>' + this.series.name + '</b><br/>' + (valueType == 'V' ? 'Value for ' : 'Percentage for ') + param1 + ' is <b>' + param2 + '</b>' + (valueType == 'V' ? '' : '%') + '</span>';
                }
            },
        series: [],
        colors: ['#FA5858', '#FA5858', '#20B2AA', '#20B2AA'],

    };

    return options;
}

//---------- Line Chart ---------------//