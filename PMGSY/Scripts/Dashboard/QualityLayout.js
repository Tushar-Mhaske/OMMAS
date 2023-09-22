/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   QualityLayout.js
    * Description   :   All Quality Monitoring Details related to No of Inspections, % Grading, NQM/SQM wise count of inspections, etc.
    * Author        :   Shyam Yadav
    * Creation Date :   11/Oct/2013  
*/

var allStatesGradingPie = null;
var statewiseGradingPie = null;
var allStatesGradingLineChart = null;
var statewiseGradingLineChart = null;
var monitorGradingInAllStates = null;
var monitorGradingInAllDistricts = null;

$(document).ready(function () {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divQualityDeatilsTab").tabs();
    $('#divQualityDeatilsTab ul').removeClass('ui-widget-header');

    //$("#divQualityDeatilsTab").tabs({ disabled: [1] });
    
    if ($("#hdnQualityLevelId").val() == 6 || $("#hdnQualityLevelId").val() == 3 || $("#hdnQualityLevelId").val() == 2) //mord, National,NRRDA
    {
        createStatewiseInspectionDetailsGrid();
        createMonitorwiseCompAndProgressDetailsGrid(0, "All States", "tbMonitorwiseCompAndProgressGradeList", "dvMonitorwiseCompAndProgressGradeListPager");
        createAllStatesGradingPieChart(0, allStatesGradingPie, "divQualityPieChart");
        createYearlyGradingLineChart(0, allStatesGradingLineChart, "divQualityLineChart");
    }
    else if ($("#hdnQualityLevelId").val() == 4) //state
    {
        createDistrictwiseInspectionDetailsGrid($("#hdnQualityStateCode").val(), $("#hdnQualityStateName").val());
        createMonitorwiseCompAndProgressDetailsGrid($("#hdnQualityStateCode").val(), $("#hdnQualityStateName").val(), "tbMonitorsStatewiseCompAndProgressGradeList", "dvMonitorsStatewiseCompAndProgressGradeListPager");
        createAllStatesGradingPieChart($("#hdnQualityStateCode").val(), statewiseGradingPie, "divQualityStatePieChart");
        createYearlyGradingLineChart($("#hdnQualityStateCode").val(), statewiseGradingLineChart, "divQualityStateLineChart");
    }
    //else if ($("#hdnLevelId").val() == 5) //District
    //{
    //    createBlockDetailsGrid($("#hdnDistrictCode").val(), $("#hdnDistrictName").val());
    //}
    
    $("#ddlQmTypeQuality").change(function () {

        if ($("#hdnQualityLevelId").val() == 6 || $("#hdnQualityLevelId").val() == 3 || $("#hdnQualityLevelId").val() == 2) //mord, National,NRRDA
        {
            createMonitorwiseCompAndProgressDetailsGrid(0, "All States", "tbMonitorwiseCompAndProgressGradeList", "dvMonitorwiseCompAndProgressGradeListPager");
            createAllStatesGradingPieChart($("#hdnQualityStateCode").val(), allStatesGradingPie, "divQualityPieChart");
            createYearlyGradingLineChart($("#hdnQualityStateCode").val(), allStatesGradingLineChart, "divQualityLineChart");
        }
        else if ($("#hdnQualityLevelId").val() == 4) //state
        {
            createMonitorwiseCompAndProgressDetailsGrid($("#hdnQualityStateCode").val(), $("#hdnQualityStateName").val(), "tbMonitorsStatewiseCompAndProgressGradeList", "dvMonitorsStatewiseCompAndProgressGradeListPager");
            createAllStatesGradingPieChart($("#hdnQualityStateCode").val(), statewiseGradingPie, "divQualityStatePieChart");
            createYearlyGradingLineChart($("#hdnQualityStateCode").val(), statewiseGradingLineChart, "divQualityStateLineChart");
        }
    });

    $.unblockUI();
});


function createStatewiseInspectionDetailsGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbStatewiseInspectionDetailsList").jqGrid('GridUnload');

    jQuery("#tbStatewiseInspectionDetailsList").jqGrid({
        url: '/Dashboard/ListAllStatesInspectionDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "NQM", "SQM", "Satisfactory", "Required Improvement", "Unsatisfactory", "Satisfactory", "Required Improvement", "Unsatisfactory"],
        colModel: [
                        { name: 'State', index: 'State', width: 120, sortable: true, align: "left" },
                        { name: 'NQMTotal', index: 'NQMTotal', width: 70, sortable: true, align: "right" },
                        { name: 'SQMTotal', index: 'SQMTotal', width: 80, sortable: true, align: "right" },

                        { name: 'NQMSatisfactory', index: 'NQMSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'NQMRI', index: 'NQMRI', width: 70, sortable: true, align: "right" },
                        { name: 'NQMUnsatisfactory', index: 'NQMUnsatisfactory', width: 80, sortable: true, align: "right" },

                        { name: 'SQMSatisfactory', index: 'SQMSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'SQMRI', index: 'SQMRI', width: 70, sortable: true, align: "right" },
                        { name: 'SQMUnsatisfactory', index: 'SQMUnsatisfactory', width: 80, sortable: true, align: "right" },

        ],
        //postData: { "fundingAgency": $("#ddlCollaborationsForPhysical").val() },
        pager: jQuery('#dvStatewiseInspectionDetailsListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        autowidth: true,
        sortname: 'State',
        rownumbers: true,
        footerrow: true,
        hidegrid: false,
        loadComplete: function () {

            var totalNQMTotal = $(this).jqGrid('getCol', 'NQMTotal', false, 'sum');
            var totalSQMTotal = $(this).jqGrid('getCol', 'SQMTotal', false, 'sum');
            
            $(this).jqGrid('footerData', 'set', { State: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { NQMTotal: totalNQMTotal });
            $(this).jqGrid('footerData', 'set', { SQMTotal: totalSQMTotal });
              
            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/Error";
            }
            else {
                alert(xhr.responseText);
                //window.location.href = "/Login/Error";
            }

            $.unblockUI();
        }
    }); //end of grid


    $("#tbStatewiseInspectionDetailsList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'NQMTotal', numberOfColumns: 2, titleText: 'No. of Inspections' },
                        { startColumnName: 'NQMSatisfactory', numberOfColumns: 3, titleText: '% Grading by NQMs' },
                        { startColumnName: 'SQMSatisfactory', numberOfColumns: 3, titleText: '% Grading by SQMs' }
        ]
    });
}


function createDistrictwiseInspectionDetailsGrid(stateCode, stateName) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbDistrictwiseInspectionDetailsList").jqGrid('GridUnload');

    jQuery("#tbDistrictwiseInspectionDetailsList").jqGrid({
        url: '/Dashboard/ListDistrictwiseInspectionDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "NQM", "SQM", "Satisfactory", "Required Improvement", "Unsatisfactory", "Satisfactory", "Required Improvement", "Unsatisfactory"],
        colModel: [
                        { name: 'District', index: 'State', width: 120, sortable: true, align: "left" },
                        { name: 'NQMTotal', index: 'NQMTotal', width: 70, sortable: true, align: "right" },
                        { name: 'SQMTotal', index: 'SQMTotal', width: 80, sortable: true, align: "right" },

                        { name: 'NQMSatisfactory', index: 'NQMSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'NQMRI', index: 'NQMRI', width: 70, sortable: true, align: "right" },
                        { name: 'NQMUnsatisfactory', index: 'NQMUnsatisfactory', width: 80, sortable: true, align: "right" },

                        { name: 'SQMSatisfactory', index: 'SQMSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'SQMRI', index: 'SQMRI', width: 70, sortable: true, align: "right" },
                        { name: 'SQMUnsatisfactory', index: 'SQMUnsatisfactory', width: 80, sortable: true, align: "right" },

        ],
        postData: { "stateCode": stateCode },
        pager: jQuery('#dvDistrictwiseInspectionDetailsListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All Districts of " + stateName,
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        hidegrid: false,
        loadComplete: function () {

            var totalNQMTotal = $(this).jqGrid('getCol', 'NQMTotal', false, 'sum');
            var totalSQMTotal = $(this).jqGrid('getCol', 'SQMTotal', false, 'sum');

            $(this).jqGrid('footerData', 'set', { State: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { NQMTotal: totalNQMTotal });
            $(this).jqGrid('footerData', 'set', { SQMTotal: totalSQMTotal });

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/Error";
            }
            else {
                alert(xhr.responseText);
                //window.location.href = "/Login/Error";
            }

            $.unblockUI();
        }
    }); //end of grid


    $("#tbDistrictwiseInspectionDetailsList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'NQMTotal', numberOfColumns: 2, titleText: 'No. of Inspections' },
                        { startColumnName: 'NQMSatisfactory', numberOfColumns: 3, titleText: '% Grading by NQMs' },
                        { startColumnName: 'SQMSatisfactory', numberOfColumns: 3, titleText: '% Grading by SQMs' }
        ]
    });
}


function createMonitorwiseCompAndProgressDetailsGrid(stateCode, stateName, gridId, pagerId) {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#" + gridId).jqGrid('GridUnload');

    jQuery("#" + gridId).jqGrid({
        url: '/Dashboard/ListMonitorwiseComplAndProgressDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Satisfactory", "Required Improvement", "Unsatisfactory", "Total", "Satisfactory", "Required Improvement", "Unsatisfactory", "Total"],
        colModel: [
                        { name: 'Monitor', index: 'Monitor', width: 120, sortable: true, align: "left" },

                        { name: 'CompletedSatisfactory', index: 'CompletedSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedRI', index: 'CompletedRI', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedUnsatisfactory', index: 'CompletedUnsatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedTotal', index: 'CompletedTotal', width: 70, sortable: true, align: "right" },

                        { name: 'ProgressSatisfactory', index: 'ProgressSatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'ProgressRI', index: 'ProgressRI', width: 70, sortable: true, align: "right" },
                        { name: 'ProgressUnsatisfactory', index: 'ProgressUnsatisfactory', width: 70, sortable: true, align: "right" },
                        { name: 'ProgressTotal', index: 'ProgressTotal', width: 70, sortable: true, align: "right" }

        ],
        postData: { "stateCode": stateCode, "qmType": $("#ddlQmTypeQuality").val() },
        pager: jQuery('#' + pagerId),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Independent Monitor Inspection Details - " + stateName,
        autowidth: true,
        sortname: 'Monitor',
        rownumbers: true,
        footerrow: true,
        hidegrid: false,
        loadComplete: function () {

            var totalCompletedSatisfactory = $(this).jqGrid('getCol', 'CompletedSatisfactory', false, 'sum');
            var totalCompletedRI = $(this).jqGrid('getCol', 'CompletedRI', false, 'sum');
            var totalCompletedUnsatisfactory = $(this).jqGrid('getCol', 'CompletedUnsatisfactory', false, 'sum');
            var totalCompletedTotal = $(this).jqGrid('getCol', 'CompletedTotal', false, 'sum');
            
            var totalProgressSatisfactory = $(this).jqGrid('getCol', 'ProgressSatisfactory', false, 'sum');
            var totalProgressRI = $(this).jqGrid('getCol', 'ProgressRI', false, 'sum');
            var totalProgressUnsatisfactory = $(this).jqGrid('getCol', 'ProgressUnsatisfactory', false, 'sum');
            var totalProgressTotal = $(this).jqGrid('getCol', 'ProgressTotal', false, 'sum');

            $(this).jqGrid('footerData', 'set', { Monitor: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { CompletedSatisfactory: totalCompletedSatisfactory });
            $(this).jqGrid('footerData', 'set', { CompletedRI: totalCompletedRI });
            $(this).jqGrid('footerData', 'set', { CompletedUnsatisfactory: totalCompletedUnsatisfactory });
            $(this).jqGrid('footerData', 'set', { CompletedTotal: totalCompletedTotal });

            $(this).jqGrid('footerData', 'set', { ProgressSatisfactory: totalProgressSatisfactory });
            $(this).jqGrid('footerData', 'set', { ProgressRI: totalProgressRI });
            $(this).jqGrid('footerData', 'set', { ProgressUnsatisfactory: totalProgressUnsatisfactory });
            $(this).jqGrid('footerData', 'set', { ProgressTotal: totalProgressTotal });

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                alert(xhr.responseText);
                //window.location.href = "/Login/Error";
            }

            $.unblockUI();
        }
    }); //end of grid


    $("#" + gridId).jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'CompletedSatisfactory', numberOfColumns: 4, titleText: 'Completed' },
                        { startColumnName: 'ProgressSatisfactory', numberOfColumns: 4, titleText: 'In Progress' }
        ]
    });
}


function viewDistrictwiseQualityDetails(stateCode, stateName)
{
    $("#divQualityDeatilsTab").tabs("option", "active", 1);
    $('#tbStatewiseInspectionDetailsList').jqGrid('setSelection', stateCode);
    $("#MAST_STATE_CODE_QUALITY").val(stateCode);
    createDistrictwiseInspectionDetailsGrid(stateCode, stateName);
    createMonitorwiseCompAndProgressDetailsGrid(stateCode, stateName, "tbMonitorsStatewiseCompAndProgressGradeList", "dvMonitorsStatewiseCompAndProgressGradeListPager");
    createAllStatesGradingPieChart(stateCode, statewiseGradingPie, "divQualityStatePieChart");
    createYearlyGradingLineChart(stateCode, statewiseGradingLineChart, "divQualityStateLineChart");
}

function viewMonitorwiseGradingColumnChart(monitorCode, monitorName, monitorType, stateCode) {

    if (stateCode == 0) {

        $("#divQualityColumnChartParent").show();
        $('#mainDiv').animate({ scrollTop: $('#mainDiv').height() }, 'slow');
        $('#tbMonitorwiseCompAndProgressGradeList').jqGrid('setSelection', monitorCode);
        createGradingColumnChart(stateCode, monitorCode, monitorName, monitorType, monitorGradingInAllStates, "divQualityColumnChart");
    }
    else {

        $("#divQualityStateColumnChartParent").show();
        $('#mainDiv').animate({ scrollTop: $('#mainDiv').height() }, 'slow');
        $('#tbMonitorsStatewiseCompAndProgressGradeList').jqGrid('setSelection', monitorCode);
        createGradingColumnChart(stateCode, monitorCode, monitorName, monitorType, monitorGradingInAllStates, "divQualityStateColumnChart");
    }
   
}


function commonOptionsForPie(containerDivID)
{
    var colourArray = [];

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }
    
    //colourArray = ['#5485BC', '#FCB319', '#20B2AA', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37'];
    colourArray = ['#20B2AA', '#FCB319', '#D40D66', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37'];

    var optionsPie =
        {
            chart: {
                type: 'pie',
                renderTo: containerDivID
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Grading Percentage',
                style: {
                    color: '#3E576F',
                    fontSize: '13px'
                }
            },
            subtitle: {
                text: ''
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            tooltip:
            {
                pointFormat: '<table><tr><td>{point.key}:</td><td style="padding:0;font-size:10px;"><b>{point.y:.1f}%</b></td><tr><table>',
                shared: true,
                useHTML: true 
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: []
        }

    optionsPie.colors = Highcharts.map(colourArray, function (color) {
        return {
            radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
            stops: [
                [0, color],
                [1, Highcharts.Color(color).brighten(-0.1).get('rgb')] // darken
            ]
        };
    });

    return optionsPie;
}


function createAllStatesGradingPieChart(stateCode, chart, containerID) {
  
    $.ajax({
        type: "POST",
        url: '/Dashboard/GetAllStatesGradingPieChart?' + Math.random(),
        data: { "stateCode": stateCode, "qmType": $("#ddlQmTypeQuality").val() },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {

            var series1 = null;
           
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {
                series1 = {
                    data: []
                };
              
                $.each(data, function (item) {
                    series1.data.push([this.Name, this.Value ]);
                });
                
                optionsPie = commonOptionsForPie(containerID);
                optionsPie.series.push(series1);
               
                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                //chart.legend.group.hide();
                if (stateCode == 0) {
                    chart.setTitle({ text: "Grading Percentage for All States" });
                }
                else {
                    chart.setTitle({ text: "Grading Percentage" });
                }
            }
        }
    });


}




//--------------- Line Chart -------------//

function CommonOptionsForYearlyGradingLineChart(containerDivID) {


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
            text: 'Yearly Grading Percentage',
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
            //labels: { rotation: -45, align: 'right' }
        },
        yAxis: {
            title: {
                text: 'Grading (%)'
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
        tooltip:
            {
                formatter: function () {
                    var param1, param2;
                    if (this.x != undefined)
                        param1 = this.x;
                    if (this.y != undefined)
                        param2 = this.y;

                    return '<span style="padding:0;font-size:11px;">' + param1 + ' : ' + param2 + '%' + '</span>' ;
                }
            },
        //legend: {
        //    layout: 'horizontal',
        //    align: 'middle',
        //    verticalAlign: 'top',
        //    borderWidth: 0
        //},

        series: [],
        //colors: ['#AA8C30', '#3090C7', '#5485BC', '#5C9384', '#AA8C30', '#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#B2C248'],
        colors: ['#20B2AA', '#FCB319', '#D40D66', '#614931', '#AA8C30', '#86A033', '#614931', '#981A37'],

    };

    return options;
}


//function to get the asset liability chart
function createYearlyGradingLineChart(stateCode, chart, containerID) {
    $.ajax({
        type: "POST",
        url: '/Dashboard/GetYearlyGradingLineChart/',
        data: { "stateCode": stateCode, "qmType": $("#ddlQmTypeQuality").val() },
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
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                series1 = {
                    name: "Satisfactory",
                    data: []
                };

                series2 = {
                    name: "Required Improvement",
                    data: []
                };

                series3 = {
                    name: "UnSatisfactory",
                    data: []
                };

                $.each(data, function (item) {
                    //var sPercent = parseFloat(this.SPercent).toFixed(1);
                    series1.data.push({ x: this.Year, y: this.SPercent });
                });

                $.each(data, function (item) {
                    //var sriPercent = parseFloat(this.SRIPercent).toFixed(1);
                    series2.data.push({ x: this.Year, y: this.SRIPercent });
                });

                $.each(data, function (item) {
                    //var uPercent = parseFloat(this.UPercent).toFixed(1);
                    series3.data.push({ x: this.Year, y: this.UPercent });
                });

                optionsPie = CommonOptionsForYearlyGradingLineChart(containerID);
                optionsPie.series.push(series1);
                optionsPie.series.push(series2);
                optionsPie.series.push(series3);

                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
                if (stateCode == 0) {
                    chart.setTitle({ text: "Yearly Grading Percentage for All States" });
                }
                else {
                    chart.setTitle({ text: "Yearly Grading Percentage" });
                }
            }
        }
    });
}


//---------- Line Chart ---------------//



//--------------- Column Chart -------------//

function CommonOptionsForGradingColumnChart(containerDivID) {

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: 'column'
            },
        title: {
            text: null
        },
        subtitle: {
            text: null
        },
        xAxis: {
            gridLineWidth: 1,
            labels: { rotation: -45, align: 'right' },
            categories: []
        },
        yAxis: [{
            
            title: {
                text: 'Grading (%)',
                style: {
                    color: '#89A54E'
                }
            },
            min: 0,
            max:100,
            labels: {
                format: '{value}',
                style: {
                    color: '#89A54E'
                }
            }
        }
        ],
        tooltip:
            {
                headerFormat: '<span style="font-size:11px"> State : {point.key} </span>' +
                              '<table>',
                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y:.1f}%.</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
        legend: {
            itemStyle: {
                //color: '#000',
                //fontFamily: 'font-family: Trebuchet MS,Tahoma,Verdana,Arial,sans-serif;',
                //fontSize: '.9em'
            }
        },
        series: [],
        colors: ['#9966CC', '#20B2AA', '#9ACD32', '#3090C7', '#FCB319', '#7B68EE', '#006400', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248', '#AA8C30']

    };

    return options;
}


function createGradingColumnChart(stateCode, monitorCode, monitorName, monitorType, chart, containerID) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: "POST",
        url: '/Dashboard/GetMonitorsGradingColumnChart/',
        data: { 'stateCode': stateCode, 'monitorCode': monitorCode, 'qmType': monitorType },
        error: function (xhr, status, error) {
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null; var series2 = null; var series3 = null; var categories = null;
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {
                categories = {
                    data: []
                };
                series1 = {
                    data: []
                };
                series2 = {
                    data: []
                };
                series3 = {
                    data: []
                };
                
                optionsColumn = CommonOptionsForGradingColumnChart(containerID);
                $.each(data, function (item) {

                    optionsColumn.xAxis.categories.push(this.Name);
                    //categories.data.push();
                   
                    series1.name = "Satisfactory";
                    series1.data.push({ y: parseFloat(this.SPercent) });

                    series2.name = "Required Improvement";
                    series2.data.push({ y: parseFloat(this.SRIPercent) });

                    series3.name = "UnSatisfactory";
                    series3.data.push({  y: parseFloat(this.UPercent) });
                });

                optionsColumn.series.push(series1);
                optionsColumn.series.push(series2);
                optionsColumn.series.push(series3);

                chart = new Highcharts.Chart(optionsColumn);
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
                chart.series[2].setVisible(true, true);
            }

            $.unblockUI();
        }
    });


}


//--------------- Column Chart -------------//