/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   DashboardLayout.js
    * Description   :   Financial Report, Status Monitoring, General Information, Balancesheet, Annual account, Cummulative Expenditure, Implementation Summary . 
    * Author        :   Shyam Yadav
    * Creation Date :   20/Sep/2013  
*/


yearwiseCumExpLineChart = null;
isFirstCallToLoadPhysicalDetails = true;
isFirstCallToLoadQualityDetails = true;
$(document).ready(function () {

    
    //Added By Abhishek kamble 4-Apr-2014 start
    $("#ddlWBankFundType").change(function () {
       
        if ($("#ddlWBankFundType").val() == "A") {
            $("#IMS_COLLABORATION option[value='-1']").remove();
        }
        else {
            var optText = "All Funding Agency";
            var optVal = $("#IMS_COLLABORATION option:contains('" + optText + "')").attr('value');
            if (optVal ===undefined) {                
                $("#IMS_COLLABORATION").prepend("<option value='-1' selected='selected'> All Funding Agency</option>");
            }
        }
    });
    //Added By Abhishek kamble 4-Apr-2014 end

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tabs-wbank-dashboard").tabs();
    $('#tabs-wbank-dashboard ul').removeClass('ui-widget-header');

    $("#divSection3Tab").tabs();
    $('#divSection3Tab ul').removeClass('ui-widget-header');


    //To hide Show ddlWBankFundType Div on selction of Tab
    //$('#tabs-wbank-dashboard').tabs({
    //    select: function (event, ui) {
    //        var tabNumber = ui.index;
    //        if (tabNumber == 0)                         // for Financial
    //        {
    //            $('#dvFundTypeSelection').show();
    //            $('#dvCollaborationForPhysical').hide();
    //            $('#dvForQuality').hide();
    //        }
    //        else if (tabNumber == 1) {
    //            $('#dvCollaborationForPhysical').show();// for Physical
    //            $('#dvFundTypeSelection').hide();
    //            $('#dvForQuality').hide();
    //        }
    //        else if (tabNumber == 2) {                                    // for Quality
    //            $('#dvForQuality').show();  
    //            $('#dvFundTypeSelection').hide();
    //            $('#dvCollaborationForPhysical').hide();
    //        }
            
    //        //var tabName = $(ui.tab).text();
    //    }
    //});

    $('#tabs-wbank-dashboard').tabs({
        select: function (event, ui) {
            var tabNumber = ui.index;
            if (tabNumber == 0)                         
            {               
                $('#dvCollaborationForPhysical').show();// for Physical
                $('#dvFundTypeSelection').hide();
                $('#dvForQuality').hide();
            }
            else if (tabNumber == 1) {     // for Quality
                $('#dvForQuality').show();
                $('#dvFundTypeSelection').hide();
                $('#dvCollaborationForPhysical').hide();
            }         
            //var tabName = $(ui.tab).text();
        }
    });


    //To hide Show ddlWBankFundType Div on selction of Tab
    $('#divSection3Tab').tabs({
        select: function (event, ui) {
            var tabNumber = ui.index;
            if (tabNumber > 0)
            {
                $('#spnSRRDAName').show();
            }
            else
            {
                $('#spnSRRDAName').hide();
            }
            //var tabName = $(ui.tab).text();
        }
    });


    
    //$("#divSection3Tab").tabs("option", "active", 0);
    //createFundVsExpenditureReportGrid();
    //createExpenditureSummaryGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val(), $("#PIU_ND_CODE").val(), $("#IMS_COLLABORATION").val());
    //createStatusMonitoringGrid();
    //createStatusMonitoringPIUReportGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val());
    //createExpenditureLineChart(yearwiseCumExpLineChart, "divExpTrendChartContainer");
    
    
   
    $("#ddlWBankFundType").change(function () {

        createExpenditureLineChart(yearwiseCumExpLineChart, "divExpTrendChartContainer");
        createFundVsExpenditureReportGrid();
        createExpenditureSummaryGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val(), $("#PIU_ND_CODE").val(), $("#IMS_COLLABORATION").val());
        createStatusMonitoringGrid();
        createStatusMonitoringPIUReportGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val());
  
        $(window).trigger('resize');
    });



    $("#IMS_COLLABORATION").change(function () {
        createExpenditureSummaryGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val(), $("#PIU_ND_CODE").val(), $("#IMS_COLLABORATION").val());
        createExpenditureLineChart(yearwiseCumExpLineChart, "divExpTrendChartContainer");
        $(window).trigger('resize');
    });


    //------------------- Expenditure Trend Maximize / Minimize function Starts Here---------------------//

    $("#spnExpTrendPlus").click(function () {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tdSection1").hide();
        $("#divHeaderExpSummary").hide();
        $("#divSection2").hide();
        $("#tdSection3").hide();

        $("#tdSection2").css('width', '99%');
        $("#divExpSummary").css('height', '100%');
        $("#divExpSummary").css('width', '99%');

        $("#divSection4").css('width', '99%');

        $("#divExpTrendChartContainer").css('width', '99%');
        $("#divExpTrendChartContainer").css('height', '530px');

        $("#spnExpTrendPlus").hide();
        $("#spnExpTrendMinus").show();
        $("#divSection4").show();

        $(window).trigger('resize');

        $.unblockUI();
    });


    $("#spnExpTrendMinus").click(function () {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tdSection1").show();
        $("#divHeaderExpSummary").show();
        $("#divSection2").show();
        $("#tdSection3").show();

        $("#tdSection2").css('width', '40%');
        $("#divExpSummary").css('height', '96%');
        $("#tdSection4").css('width', '39.9%');

        $("#spnExpTrendPlus").show();
        $("#spnExpTrendMinus").hide();
        $("#divExpTrendChartContainer").css('width', '39.5%');
        $("#divExpTrendChartContainer").css('height', '240px');

        $(window).trigger('resize');

        $.unblockUI();
    });

    //------------------- Expenditure Trend Maximize / Minimize function End Here---------------------//

    
    if ($("#hdnRole").val() == 5)
    {
        $('#tabs-wbank-dashboard').tabs('select', 1);
    }
    else {
        $('#tabs-wbank-dashboard').tabs('select', 0);
    }


    $.unblockUI();

});


function reloadPage() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#STATE_ND_CODE").val(0);
    $("#spnSRRDAName").html("");
    $("#PIU_ND_CODE").val(0);
    $("#divDPIUName").html("");
    location.reload();
    $.unblockUI();
}


//-------------------------- Financial functions starts Here ---------------------------//

function createFundVsExpenditureReportGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbWBankFundVsExpenditureList").jqGrid('GridUnload');

    jQuery("#tbWBankFundVsExpenditureList").jqGrid({
        url: '/Dashboard/ListFundVsExpenditureReport?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Department", "Fund Received (Cr.)", "Authorization Received (Cr.)", "Expenditure (Cr.)", "Bank (Cr.)"],
        colModel: [
                        { name: 'AdminNdName', index: 'AdminNdName', width: 200, sortable: true, align: "left" },
                        { name: 'FundReceived', index: 'FundReceived', width: 80, sortable: true, align: "right" },
                        { name: 'AuthReceived', index: 'AuthReceived', width: 80, sortable: true, align: "right" },
                        { name: 'Expenditure', index: 'Expenditure', width: 80, sortable: true, align: "right" },
                        { name: 'Bank', index: 'Bank', width: 80, sortable: true, align: "right" }
        ],
        postData: { "fundType": $("#ddlWBankFundType").val() },
        pager: jQuery('#dvWBankFundVsExpenditureListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        //caption: "&nbsp;&nbsp;Fund vs Expenditure",
        height: 170,
        autowidth: true,
        sortname: 'AdminNdName',
        rownumbers: true,
        footerrow: true,
        hidegrid: false,
        loadComplete: function () {

            //$('#tbWBankFundVsExpenditureList').setGridWidth($("#tabs-wbank-dashboard").width() - 40, true);

            //$("#gview_tbWBankFundVsExpenditureList > .ui-jqgrid-titlebar").hide();

            var totalFundReceived = $(this).jqGrid('getCol', 'FundReceived', false, 'sum').toFixed(2);
            var totalAuthReceived = $(this).jqGrid('getCol', 'AuthReceived', false, 'sum').toFixed(2);
            var totalExpenditure = $(this).jqGrid('getCol', 'Expenditure', false, 'sum').toFixed(2);
            var totalBank = $(this).jqGrid('getCol', 'Bank', false, 'sum').toFixed(2);

            $(this).jqGrid('footerData', 'set', { AdminNdName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { FundReceived: totalFundReceived });
            $(this).jqGrid('footerData', 'set', { AuthReceived: totalAuthReceived });
            $(this).jqGrid('footerData', 'set', { Expenditure: totalExpenditure });
            $(this).jqGrid('footerData', 'set', { Bank: totalBank });

            //$.unblockUI();
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
}


function createExpenditureSummaryGrid(fundType, stateNdCode, piuNdCode, fundingAgency) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbWBankExpenditureSummaryList").jqGrid('GridUnload');

    jQuery("#tbWBankExpenditureSummaryList").jqGrid({
        url: '/Dashboard/ListExpenditureSummary?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Description", "Expenditure (Cr.)"],
        colModel: [
                        { name: 'Desc', index: 'Desc', width: 300, sortable: true, align: "left" },
                        { name: 'Expn', index: 'Expn', width: 150, sortable: true, align: "right" }
        ],
        postData: { "fundType": fundType, "stateNdCode": stateNdCode, "piuNdCode": piuNdCode, "fundingAgency": fundingAgency },
        pager: jQuery('#dvWBankExpenditureSummaryListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        //caption: "&nbsp;&nbsp;Expenditure Summary",
        autowidth: true,
        height: 'auto',
        sortname: 'Desc',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalExpn = $(this).jqGrid('getCol', 'Expn', false, 'sum').toFixed(2);
            $(this).jqGrid('footerData', 'set', { Desc: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { Expn: totalExpn });

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
}


function createStatusMonitoringGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbWBankStatusMonitoringList").jqGrid('GridUnload');

    jQuery("#tbWBankStatusMonitoringList").jqGrid({
        url: '/Dashboard/ListStatusMonitoringReport?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["SRRDA", "Total no of DPIUs", "No. of DPIUs Account Closed Upto Last Month", "Account Closed By State Till", "UnReconciled Bank Authorization (Cr.)", "UnReconciled Fund Transferred (Cr.)"],
        colModel: [
                        { name: 'SrrdaName', index: 'SrrdaName', width: 250, sortable: true, align: "left" },
                        { name: 'NoOfDPIUs', index: 'NoOfDPIUs', width: 150, sortable: true, align: "right" },
                        { name: 'DPIUClosedAccLastMonth', index: 'DPIUClosedAccLastMonth', width: 150, sortable: true, align: "right" },
                        { name: 'AccClosedTillDate', index: 'AccClosedTillDate', width: 150, sortable: true, align: "left" },
                        { name: 'UnReconciledAuth', index: 'UnReconciledAuth', width: 150, sortable: true, align: "right" },
                        { name: 'UnReconciledBalance', index: 'UnReconciledBalance', width: 150, sortable: true, align: "right" }
        ],
        postData: { "fundType": $("#ddlWBankFundType").val() },
        pager: jQuery('#dvWBankStatusMonitoringListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        autowidth: true,
        sortname: 'SrrdaName',
        //height: 150,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalNoOfDPIUs = $(this).jqGrid('getCol', 'NoOfDPIUs', false, 'sum');
            var totalDPIUClosedAccLastMonth = $(this).jqGrid('getCol', 'DPIUClosedAccLastMonth', false, 'sum');
            var totalUnReconciledAuth = $(this).jqGrid('getCol', 'UnReconciledAuth', false, 'sum').toFixed(2);
            var totalUnReconciledBalance = $(this).jqGrid('getCol', 'UnReconciledBalance', false, 'sum').toFixed(2);
            
            $(this).jqGrid('footerData', 'set', { SrrdaName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { NoOfDPIUs: totalNoOfDPIUs });
            $(this).jqGrid('footerData', 'set', { DPIUClosedAccLastMonth: totalDPIUClosedAccLastMonth });
            $(this).jqGrid('footerData', 'set', { UnReconciledAuth: totalUnReconciledAuth });
            $(this).jqGrid('footerData', 'set', { UnReconciledBalance: totalUnReconciledBalance });

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
}



function createStatusMonitoringPIUReportGrid(fundType, stateNdCode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbWBankStatusMonitoringPIUList").jqGrid('GridUnload');

    jQuery("#tbWBankStatusMonitoringPIUList").jqGrid({
        url: '/Dashboard/ListStatusMonitoringPIUReport?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["DPIU Name", "Last Closing Month", "Authorization Received (Cr.)", "Expenditure (Cr.)"],
        colModel: [
                        { name: 'NdName', index: 'NdName', width: 180, sortable: true, align: "left" },
                        { name: 'LastClosingMonth', index: 'LastClosingMonth', width: 150, sortable: true, align: "left" },
                        { name: 'AuthReceived', index: 'AuthReceived', width: 150, sortable: true, align: "right" },
                        { name: 'Expn', index: 'Expn', width: 150, sortable: true, align: "right" }
        ],
        postData: { "stateNdCode": stateNdCode, "fundType": fundType },
        pager: jQuery('#dvWBankStatusMonitoringPIUListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        autowidth: true,
        sortname: 'NdName',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            
            var totalAuthReceived = $(this).jqGrid('getCol', 'AuthReceived', false, 'sum').toFixed(2);
            var totalExpn = $(this).jqGrid('getCol', 'Expn', false, 'sum').toFixed(2);

            $(this).jqGrid('footerData', 'set', { NdName: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { AuthReceived: totalAuthReceived });
            $(this).jqGrid('footerData', 'set', { Expn: totalExpn });

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
}


function viewDPIUDetails(stateNdCode, stateNdName) {
    $(window).trigger('resize');
    $('#tbWBankFundVsExpenditureList').jqGrid('setSelection', stateNdCode);
    $("#STATE_ND_CODE").val(stateNdCode);
    $("#spnSRRDAName").html(stateNdName);

    createExpenditureSummaryGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val(), $("#PIU_ND_CODE").val(), $("#IMS_COLLABORATION").val());
    createStatusMonitoringPIUReportGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val());
    createExpenditureLineChart(yearwiseCumExpLineChart, "divExpTrendChartContainer");

    $("#divDPIUName").html("");
    $("#divSection3Tab").tabs("option", "active", 1);
}


function viewDPIUExpSummaryAndTrend(dpiuNdCode, dpiuNdName)
{
    $(window).trigger('resize');
    $('#tbWBankStatusMonitoringPIUList').jqGrid('setSelection', dpiuNdCode);
    $("#PIU_ND_CODE").val(dpiuNdCode);
    $("#divDPIUName").html(dpiuNdName);

    createExpenditureSummaryGrid($("#ddlWBankFundType").val(), $("#STATE_ND_CODE").val(), $("#PIU_ND_CODE").val(), $("#IMS_COLLABORATION").val());
    createExpenditureLineChart(yearwiseCumExpLineChart, "divExpTrendChartContainer");
}




//--------------- Line Chart -------------//

function CommonOptions(containerDivID) {


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
            text: '',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {
            gridLineWidth: 1,
            //minorGridLineWidth: 1,
            categories: [],
            
            labels: { rotation: -45, align: 'right' }
            //,
            //title: {
            //    text: 'Years'
            //}
        },
        yAxis: {
            //gridLineWidth: 1,
            //minorGridLineWidth: 1,
            title: {
                text: 'Expenditure (Cr.)'
            },
            min: 0,
            labels: {
                //rotation: -45, align: 'right',
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

                    return '' + param1 + ' : ' + param2 + 'Cr.';
                }
            },
        //legend: {
        //    layout: 'horizontal',
        //    align: 'middle',
        //    verticalAlign: 'top',
        //    borderWidth: 0
        //},
        
        series: [],
        colors: ['#AA8C30', '#3090C7', '#5485BC', '#5C9384', '#AA8C30', '#FCB319', '#86A033', '#614931', '#00526F', '#594266', '#cb6828', '#aaaaab', '#a89375', '#B2C248'],

    };

    return options;
}


//function to get the asset liability chart
function createExpenditureLineChart(chart, containerID) {
    $.ajax({
        type: "POST",
        url: '/Dashboard/ExpenditureTrend/',
        data:
            {
                'fundType': $("#ddlWBankFundType").val(),
                'stateCode': $("#STATE_ND_CODE").val(),
                'piuCode': $("#PIU_ND_CODE").val(),
                'collaboration': $("#IMS_COLLABORATION").val(),
                
            },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null;
            var series2 = null;
            if (data == "") {
                if ($('#' + containerID).highcharts()) {
                    $('#' + containerID).highcharts().destroy();
                }
            }
            else {

                series1 = {
                    name: "Yearly",
                    data: []
                };

                series2 = {
                    name: "Cumulative",
                    data: []
                };

                $.each(data, function (item) {
                    var currPeriodAmt = parseFloat(this.YearlyExpn).toFixed(2);
                    series1.data.push({ x: this.MYear, y: parseFloat(currPeriodAmt) });
                });

                $.each(data, function (item) {
                    var cumulativeAmt = parseFloat(this.CumExpn).toFixed(2);
                    series2.data.push({ x: this.MYear, y: parseFloat(cumulativeAmt) });
                });

                optionsPie = CommonOptions(containerID);
                optionsPie.series.push(series1);
                optionsPie.series.push(series2);

                chart = new Highcharts.Chart(optionsPie);
                // code to display animation
                chart.series[0].setVisible(true, true);
                chart.series[1].setVisible(true, true);
            }
        }
    });
}


//---------- Line Chart ---------------//


//-------------------------- Financial functions Ends Here ---------------------------//






//-------------------------- Physical & Quality Details Load functions calls starts Here ---------------------------//



function loadPhysicalDetails()
{
    if (isFirstCallToLoadPhysicalDetails) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tabs-2").load('/Dashboard/PhysicalLayout/', function () {
            $.unblockUI();
            isFirstCallToLoadPhysicalDetails = false;
        });
    }
}



function loadQualityDetails() {
    if (isFirstCallToLoadQualityDetails) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tabs-3").load('/Dashboard/QualityLayout/', function () {
            $.unblockUI();
            isFirstCallToLoadQualityDetails = false;
        });
    }
}




//-------------------------- Physical & Quality Details Load functions calls Ends Here ---------------------------//

