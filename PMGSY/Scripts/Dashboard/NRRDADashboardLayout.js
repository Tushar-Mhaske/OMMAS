var scrolled = 0;
$(document).ready(function () {


    $("#STATE_CODES").multiselect({
        nonSelectedText: "Select States",
        minWidth: 200,
        height: 200
    });
    $("#STATE_CODES").multiselect("uncheckAll");

    $("#btnViewDetails").click(function () {

        blockPage();

        if ($("#frmFilterDetails").valid()) {
        GenericChartDefinition("dvChartContainerSanctioned", '/Dashboard/GetTrendsOfProposalsSanctioned', 'line', 'Length in Km', 'State', 'Trends of last 5 years Sanctions of Roads',1);
        GenericChartDefinition("dvChartContainerCompleted", '/Dashboard/GetTrendsOfProposalsCompleted', 'line', 'Length in Km', 'State', 'Trends of last 5 years Completion of Roads',2);
        GenericChartDefinition("dvChartContainerExpenditure", '/Dashboard/GetTrendsOfProposalsExpenditure', 'line', 'Rs. in Crore', 'State', 'Trends of last 5 years Expenditure of Roads and LSB',3);
        GetHabDetailsMPR();
        GetMaintenanceDetailsMPR();
        }
        else {
            unblockPage();
            return false;
        }

        unblockPage();
    });

    $("#btnDown").click(function () {
        scrolled = scrolled + 640;
        $('#mainDiv').scrollTop(scrolled);
    });

    $("#btnup").click(function () {
        scrolled = scrolled - 640;
        $('#mainDiv').scrollTop(scrolled);
    });

    $("#printButton").click(function () {

        var htmlCode = $("#charts").html();
        var height = $("#charts").height();
        var width = $("#charts").width();
        
        var windowSummary = window.open('', 'Graphical Reports', 'height='+height+',width='+width);
        windowSummary.document.write(htmlCode);
        windowSummary.print();
    });

    $("#dvMaintenanceDetails").click(function () {

        $src=$('#dvMaintenanceDetails');
  
        var div=$('<div>').append($src.clone());
        div.width($src.width());
        div.height($src.height());
        html2canvas($('#dvMaintenanceDetails'), {
            onrendered: function (canvas) {
                div.remove();
                var imgURL = canvas.toDataURL();
                var img = new Image();
                img.src = imgURL;
                Canvas2Image.saveAsPNG(canvas);
            }
        });
        });
});

var yearCount;
function GenericChartDefinition(containerID, url, typeOfChart, yAxisText, xAxisText, title, reportType) {

    $.ajax({
        type: "POST",
        url: url,
        data: $("#frmFilterDetails").serialize(),
        error: function (xhr, status, error) {
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series = [];
            var categories = [];
            yearCount = data.YEARS.length;
            var chartTitle;
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                if (reportType == 1) {
                    chartTitle = 'Trends of last ' + yearCount + ' years Sanctions of Roads (Length in Km.)'
                }
                else if (reportType == 2) {
                    chartTitle = 'Trends of last ' + yearCount + ' years Completion of Roads (Length in Km.)'
                }
                else if (reportType == 3) {
                    chartTitle = 'Trends of last ' + yearCount + ' years Expenditure of Roads and LSB (in Rs. Crore)'
                }


                optionsPie = GenericCommonOptions(containerID, typeOfChart, yAxisText, xAxisText, chartTitle);
                chart = new Highcharts.Chart(optionsPie);

                $.each(series, function (index, value) {

                    optionsPie.series.push(series[index]);
                });

                $.each(data, function (index, value) {

                    if (index == "LOCATION_NAME") {
                        for (var i = 0; i < data.LOCATION_NAME.length; i++) {
                            chart.addSeries({
                                name: data.LOCATION_NAME[i],
                                data: data.ROAD_LEN[i]
                            }, false);
                        }
                    }
                    if (index == "YEARS") {

                        for (var i = 0; i < data.YEARS.length; i++) {
                            optionsPie.xAxis.categories.push(data.YEARS[i]);
                        }
                    }
                });

                $(chart.series).each(function (index) {
                    chart.series[index].setVisible(true, true);
                });
            }
        }
    });
    
}
function GenericCommonOptions(containerDivID, typeOfChart, yAxisText, xAxisText, title)
{
    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }
    var colorArray = ['#78AFE6', '#D14719', '#59BD59', '#E0A3C2', '#AA8C30', '#86A033', '#614931', '#981A37', '#4572A7', '#AA4643', '#89A54E', '#80699B', '#3D96AE', '#FF0000', '#20B2AA', '#3090C7', '#FCB319', '#594266', '#00526F', '#B87F74', '#9ACD32', '#C0C0C0', '#7B68EE', '#006400', '#DC143C', '#9966CC', '#20B2AA', '#3090C7', '#FCB319', '#594266', '#cb6828', '#aaaaab', '#B2C248', '#AA8C30'];
    
    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: typeOfChart
            },
        title: {
            text: title
        },
        subtitle: {
            text: null
        },
        xAxis: {
            title: {
                text: xAxisText,
                style: {
                    color: '#89A54E'
                }
            },
            gridLineWidth: 1,
            labels: { rotation: -45, align: 'right' },
            categories: []
        },
        yAxis: [{

            title: {
                text: yAxisText,
                style: {
                    color: '#89A54E'
                }
            },
            min: 0,
            labels: {
                format: '{value}',
                style: {
                    color: '#89A54E'
                }
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        }
        ],
        exporting: {
            button: {
                type: ['image/png', 'image/jpeg', 'application/pdf'],
                filename: title
            }
        },
        plotOptions: {
            column: {
                allowPointSelect: true,
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    color: ['white'],
                    style: {
                    }
                }
            },
            series: {
                pointWidth: 30
            }
        },
        tooltip:
            {
                headerFormat: '<span style="font-size:12px;font-weight:bold"> Financial Year : {point.key} </span>' +
                              '<table>',
                pointFormat:
                             '<tr><td style="color:{series.color};padding:0;font-size:12px;font-weight:bold">{series.name}: </td>' +
                             '<td style="padding:0;font-size:12px;font-weight:bold;"><b>{point.y}</b></td></tr>' +
                             '<tr></tr>',
                footerFormat: '</table>',
                shared: false,
                useHTML: true
            },
        legend: {
            itemStyle: {
            }
        },
        series: [],
        colors: colorArray
    };

    return options;
}

function GetHabDetailsMPR() {

    $.ajax({

        type: 'POST',
        url: '/Dashboard/HabitationDetailsMPR',
        data: $("#frmFilterDetails").serialize(),
        error: function () { },
        success: function (data) {

            $("#dvHabDetails").html(data);
        }
    });
}

function GetMaintenanceDetailsMPR() {

    $.ajax({

        type: 'POST',
        url: '/Dashboard/MaintenanceFundStatus',
        data: $("#frmFilterDetails").serialize(),
        error: function () { },
        success: function (data) {

            $("#dvMaintenanceDetails").html(data);
        }
    });
}

function GetTrendsValuesInTabularForm(reportType) {
    $.ajax({

        type: 'POST',
        url: '/Dashboard/GetTabularTrendValues' + $.param({ "ReportType": reportType }),
        data: $("#frmFilterDetails").serialize(),
        error: function () { },
        success: function (data) {

            if (data != "") {
                if (reportType == 1) {
                    $("#dvTableSanctioned").html(data);
                }
                else if (reportType == 2) {
                    $("#dvTableCompleted").html(data);
                }
                else if (reportType == 3) {
                    $("#dvTableExpenditure").html(data);
                }
            }
        }
    });
}
