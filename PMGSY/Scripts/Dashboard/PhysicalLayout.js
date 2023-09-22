/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   PhysicalLayout.js
    * Description   :   All Tecnical Details related to Works, Length & Costs
    * Author        :   Shyam Yadav
    * Creation Date :   10/Oct/2013  
*/

var WorkLenghtExpYearWiseColumnChart = null;
//var worksColumnChart = null;
//var rdLengthColumnChart = null;
//var lsbLengthColumnChart = null;
//var costColumnChart = null;
//var isFirstCallToLoadLengthChartDetails = true;
//var isFirstCallToLoadCostChartDetails = true;

//State Chart
var Newchart;
var StateWiseoptionsColumn;
var drillSeriesStateData;
var flagShowStateDrillChart = true;
var chartLabel;
var chartLabelFlag = true;

//Yearly Chart
var chartYearly;
var YearWiseOptionsColumn;
var drillSeriesYearlyData;
var flagShowYearlyDrillChart = true;
var chartLabelYearly;
var chartLabelYearlyFlag = true;


var workLengthExpStateWiseColumnChart = null;
var isFirstCallToLoadWorkLengthExpStateWiseChartDetails = true;
$(document).ready(function () {
        
    chartLabel = $("#divChartLabel").text();
    chartLabelYearly=$("#divChartLabel").text();
    //alert($("#divChartLabel").text());

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#divPhysicalSecion2Tab").tabs();
    $('#divPhysicalSecion2Tab ul').removeClass('ui-widget-header');


    if ($("#hdnLevelId").val() == 6 || $("#hdnLevelId").val() == 3 || $("#hdnLevelId").val() == 2) //mord, National,NRRDA
    {
        createStateDetailsGrid();
    }
    else if ($("#hdnLevelId").val() == 4) //state
    {
        createDistrictDetailsGrid($("#hdnStateCode").val(), $("#hdnStateName").val());
    }
    else if ($("#hdnLevelId").val() == 5) //District
    {
        createBlockDetailsGrid($("#hdnDistrictCode").val(), $("#hdnDistrictName").val());
    }
    //temp commented
    //chartWorksColumnChart(worksColumnChart, "divWorksChartContainer");

    //Added By Abhishek kamble 12Mar2015
    chartWorkLenghtExpYearWiseColumnChart(WorkLenghtExpYearWiseColumnChart, "divWorkLengthExpYearWiseChartContainer");
    WorkLenExpYearWiseDetailsGrid();

    $("#ddlCollaborationsForPhysical").change(function () {

        $("#tbTechnicalDetailsDistrictList").jqGrid('GridUnload');
        $("#tbTechnicalDetailsBlockList").jqGrid('GridUnload');

        $("#MAST_STATE_CODE_PHYSICAL").val(0);
        $("#MAST_DISTRICT_CODE_PHYSICAL").val(0);

        if ($("#hdnLevelId").val() == 6 || $("#hdnLevelId").val() == 3 || $("#hdnLevelId").val() == 2) //mord, National,NRRDA
        {
            createStateDetailsGrid();
        }
        else if ($("#hdnLevelId").val() == 4) //state
        {
            createDistrictDetailsGrid($("#hdnStateCode").val(), $("#hdnStateName").val());
        }
        else if ($("#hdnLevelId").val() == 5) //District
        {
            createBlockDetailsGrid($("#hdnDistrictCode").val(), $("#hdnDistrictName").val());
        }

        commonFunctions();
    });

    //$("input[name=PROPOSAL_TYPE]:radio").change(function () {

    //     if ($("#rdoRoad").attr("checked")) {
    //         chartRdLengthColumnChart(rdLengthColumnChart, "divLengthChartContainer");

    //     }
    //     else if ($("#rdoBridge").attr("checked")) {
    //         chartLSBLengthColumnChart(lsbLengthColumnChart, "divLengthChartContainer");
    //     }
    // });


    $.unblockUI();
});


function commonFunctions() {
    //chartWorksColumnChart(worksColumnChart, "divWorksChartContainer");
    //chartRdLengthColumnChart(rdLengthColumnChart, "divLengthChartContainer");
    //chartCostColumnChart(costColumnChart, "divCostChartContainer");

    //Added By Abhishek kamble 12Mar2015    
    chartWorkLenghtExpYearWiseColumnChart(WorkLenghtExpYearWiseColumnChart, "divWorkLengthExpYearWiseChartContainer");
    chartWorkLengthExpStateWiseChart(workLengthExpStateWiseColumnChart, "divWorkLengthExpStateWiseChartContainer");
    WorkLenExpYearWiseDetailsGrid();
    WorkLenExpStateWiseDetailsGrid();
}

//function loadLengthChartDetails()
//{
//    if (isFirstCallToLoadLengthChartDetails) {

//        chartRdLengthColumnChart(rdLengthColumnChart, "divLengthChartContainer");
//        isFirstCallToLoadLengthChartDetails = false;

//    }
//}


//function loadCostChartDetails() {
//    if (isFirstCallToLoadCostChartDetails) {

//        chartCostColumnChart(costColumnChart, "divCostChartContainer");
//        isFirstCallToLoadCostChartDetails = false;

//    }
//}

//Added By Abhishek kamble 13Mar2015
function loadWorkLengthExpStateWiseChartDetails() {
    $("#divChartLabel").html("<b>" + chartLabel + "</b>");
    if (isFirstCallToLoadWorkLengthExpStateWiseChartDetails) {
        chartWorkLengthExpStateWiseChart(workLengthExpStateWiseColumnChart, "divWorkLengthExpStateWiseChartContainer");
        isFirstCallToLoadWorkLengthExpStateWiseChartDetails = false;

        WorkLenExpStateWiseDetailsGrid();
    }
}

function loadWorkLengthExpYearWiseChartDetails()
{
    $("#divChartLabel").html("<b>" + chartLabelYearly + "</b>");
}

//---------------------------- Grid Code Starts Here ---------------------------------//

function createStateDetailsGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#tbTechnicalDetailsStateList").jqGrid('GridUnload');

    jQuery("#tbTechnicalDetailsStateList").jqGrid({
        url: '/Dashboard/ListAllStatesTechnicalDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["State", "Works", "Length", "Cost", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colNames: ["State", "Works", "Length (Km.)", "Cost", "Works", "Length (Km.)", "Expenditure", "Works", "Length (Km.)", "Expenditure", "Maintenance Expenditure"],
        colModel: [
                        { name: 'State', index: 'State', width: 120, sortable: true, align: "left" },

                        { name: 'SanctionWorks', index: 'SanctionWorks', width: 70, sortable: true, align: "right" },
                        { name: 'SanctionLength', index: 'SanctionLength', width: 70, sortable: true, align: "right" },
                        { name: 'SanctionCost', index: 'SanctionCost', width: 80, sortable: true, align: "right" },

                        //{ name: 'AwardedWorks', index: 'AwardedWorks', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedLength', index: 'AwardedLength', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedCost', index: 'AwardedCost', width: 80, sortable: true, align: "right" },

                        { name: 'CompletedWorks', index: 'CompletedWorks', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedLength', index: 'CompletedLength', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedCost', index: 'CompletedCost', width: 80, sortable: true, align: "right" },

                        { name: 'OngoingWorks', index: 'OngoingWorks', width: 70, sortable: true, align: "right" },
                        { name: 'OngoingLength', index: 'OngoingLength', width: 70, sortable: true, align: "right" },
                        { name: 'OngoingCost', index: 'OngoingCost', width: 80, sortable: true, align: "right" },

                        { name: 'MaintenanceExp', index: 'MaintenanceExp', width: 80, sortable: true, align: "right", hidden:true, }

                        //{ name: 'TotalLength', index: 'TotalLength', width: 80, sortable: true, align: "right" },
                        //{ name: 'TotalExpn', index: 'TotalExpn', width: 80, sortable: true, align: "right" },

                        //{ name: 'TotalLiability', index: 'TotalLiability', width: 80, sortable: true, align: "right" },
                        //{ name: 'CurrYearLiability', index: 'CurrYearLiability', width: 80, sortable: true, align: "right" }
        ],
        postData: { "fundingAgency": $("#ddlCollaborationsForPhysical").val() },
        pager: jQuery('#dvTechnicalDetailsStateListPager'),
        rowNum: 999,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All States",
        //height: 170,
        autowidth: true,
        sortname: 'State',
        rownumbers: true,
        footerrow: true,
        //hidegrid: false,
        loadComplete: function () {

            var totalSanctionWorks = $(this).jqGrid('getCol', 'SanctionWorks', false, 'sum');
            var totalSanctionLength = $(this).jqGrid('getCol', 'SanctionLength', false, 'sum').toFixed(2);
            var totalSanctionCost = $(this).jqGrid('getCol', 'SanctionCost', false, 'sum').toFixed(2);

            //var totalAwardedWorks = $(this).jqGrid('getCol', 'AwardedWorks', false, 'sum');
            //var totalAwardedLength = $(this).jqGrid('getCol', 'AwardedLength', false, 'sum').toFixed(2);
            //var totalAwardedCost = $(this).jqGrid('getCol', 'AwardedCost', false, 'sum').toFixed(2);

            var totalCompletedWorks = $(this).jqGrid('getCol', 'CompletedWorks', false, 'sum');
            var totalCompletedLength = $(this).jqGrid('getCol', 'CompletedLength', false, 'sum').toFixed(2);
            var totalCompletedCost = $(this).jqGrid('getCol', 'CompletedCost', false, 'sum').toFixed(2);

            var totalOngoingWorks = $(this).jqGrid('getCol', 'OngoingWorks', false, 'sum');
            var totalOngoingLength = $(this).jqGrid('getCol', 'OngoingLength', false, 'sum').toFixed(2);
            var totalOngoingCost = $(this).jqGrid('getCol', 'OngoingCost', false, 'sum').toFixed(2);

            var totalMaintenanceExp = $(this).jqGrid('getCol', 'MaintenanceExp', false, 'sum').toFixed(2);

            //var totalTotalLength = $(this).jqGrid('getCol', 'TotalLength', false, 'sum').toFixed(2);
            //var totalTotalExpn = $(this).jqGrid('getCol', 'TotalExpn', false, 'sum').toFixed(2);

            //var totalTotalLiability = $(this).jqGrid('getCol', 'TotalLiability', false, 'sum').toFixed(2);
            //var totalCurrYearLiability = $(this).jqGrid('getCol', 'CurrYearLiability', false, 'sum').toFixed(2);


            $(this).jqGrid('footerData', 'set', { State: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { SanctionWorks: totalSanctionWorks });
            $(this).jqGrid('footerData', 'set', { SanctionLength: totalSanctionLength });
            $(this).jqGrid('footerData', 'set', { SanctionCost: totalSanctionCost });

            //$(this).jqGrid('footerData', 'set', { AwardedWorks: totalAwardedWorks });
            //$(this).jqGrid('footerData', 'set', { AwardedLength: totalAwardedLength });
            //$(this).jqGrid('footerData', 'set', { AwardedCost: totalAwardedCost });

            $(this).jqGrid('footerData', 'set', { CompletedWorks: totalCompletedWorks });
            $(this).jqGrid('footerData', 'set', { CompletedLength: totalCompletedLength });
            $(this).jqGrid('footerData', 'set', { CompletedCost: totalCompletedCost });

            $(this).jqGrid('footerData', 'set', { OngoingWorks: totalOngoingWorks });
            $(this).jqGrid('footerData', 'set', { OngoingLength: totalOngoingLength });
            $(this).jqGrid('footerData', 'set', { OngoingCost: totalOngoingCost });

            $(this).jqGrid('footerData', 'set', { MaintenanceExp: totalMaintenanceExp });

            //$(this).jqGrid('footerData', 'set', { TotalLength: totalTotalLength });
            //$(this).jqGrid('footerData', 'set', { TotalExpn: totalTotalExpn });

            //$(this).jqGrid('footerData', 'set', { TotalLiability: totalTotalLiability });
            //$(this).jqGrid('footerData', 'set', { CurrYearLiability: totalCurrYearLiability });

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


    $("#tbTechnicalDetailsStateList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'SanctionWorks', numberOfColumns: 3, titleText: 'Sanctioned Works' },
                        //{ startColumnName: 'AwardedWorks', numberOfColumns: 3, titleText: 'Award Details' },
                        { startColumnName: 'CompletedWorks', numberOfColumns: 3, titleText: 'Completed Works' },
                        { startColumnName: 'OngoingWorks', numberOfColumns: 3, titleText: 'Ongoing Works' }
        ]
    });
}



function viewDistrictwiseTechnicalDetails(stateCode, stateName) {
    //$("#tbTechnicalDetailsBlockList").jqGrid('GridUnload');
    //$("#MAST_DISTRICT_CODE_PHYSICAL").val(0);
    //$("#MAST_STATE_CODE_PHYSICAL").val(stateCode);
    //$("#divChartLabel").html(stateName);
    //createDistrictDetailsGrid(stateCode, stateName);
    //commonFunctions();
}


function createDistrictDetailsGrid(stateCode, stateName) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbTechnicalDetailsStateList').jqGrid('setSelection', stateCode);
    $('#tbTechnicalDetailsStateList').jqGrid('setGridState', 'hidden');

    $("#tbTechnicalDetailsDistrictList").jqGrid('GridUnload');

    jQuery("#tbTechnicalDetailsDistrictList").jqGrid({
        url: '/Dashboard/ListDistrictwiseTechnicalDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["District", "Works", "Length", "Cost", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colNames: ["District", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colModel: [
                        { name: 'District', index: 'District', width: 120, sortable: true, align: "left" },

                        { name: 'SanctionWorks', index: 'SanctionWorks', width: 70, sortable: true, align: "right" },
                        { name: 'SanctionLength', index: 'SanctionLength', width: 70, sortable: true, align: "right" },
                        { name: 'SanctionCost', index: 'SanctionCost', width: 80, sortable: true, align: "right" },

                        //{ name: 'AwardedWorks', index: 'AwardedWorks', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedLength', index: 'AwardedLength', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedCost', index: 'AwardedCost', width: 80, sortable: true, align: "right" },

                        { name: 'CompletedWorks', index: 'CompletedWorks', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedLength', index: 'CompletedLength', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedCost', index: 'CompletedCost', width: 80, sortable: true, align: "right" },

                        { name: 'OngoingWorks', index: 'OngoingWorks', width: 70, sortable: true, align: "right" },
                        { name: 'OngoingLength', index: 'OngoingLength', width: 70, sortable: true, align: "right" },
                        { name: 'OngoingCost', index: 'OngoingCost', width: 80, sortable: true, align: "right" },

                        { name: 'MaintenanceExp', index: 'MaintenanceExp', width: 80, sortable: true, align: "right" }

        ],
        postData: { "stateCode": stateCode, "fundingAgency": $("#ddlCollaborationsForPhysical").val() },
        pager: jQuery('#dvTechnicalDetailsDistrictListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All Districts of " + stateName,
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalSanctionWorks = $(this).jqGrid('getCol', 'SanctionWorks', false, 'sum');
            var totalSanctionLength = $(this).jqGrid('getCol', 'SanctionLength', false, 'sum').toFixed(2);
            var totalSanctionCost = $(this).jqGrid('getCol', 'SanctionCost', false, 'sum').toFixed(2);

            //var totalAwardedWorks = $(this).jqGrid('getCol', 'AwardedWorks', false, 'sum');
            //var totalAwardedLength = $(this).jqGrid('getCol', 'AwardedLength', false, 'sum').toFixed(2);
            //var totalAwardedCost = $(this).jqGrid('getCol', 'AwardedCost', false, 'sum').toFixed(2);

            var totalCompletedWorks = $(this).jqGrid('getCol', 'CompletedWorks', false, 'sum');
            var totalCompletedLength = $(this).jqGrid('getCol', 'CompletedLength', false, 'sum').toFixed(2);
            var totalCompletedCost = $(this).jqGrid('getCol', 'CompletedCost', false, 'sum').toFixed(2);

            var totalOngoingWorks = $(this).jqGrid('getCol', 'OngoingWorks', false, 'sum');
            var totalOngoingLength = $(this).jqGrid('getCol', 'OngoingLength', false, 'sum').toFixed(2);
            var totalOngoingCost = $(this).jqGrid('getCol', 'OngoingCost', false, 'sum').toFixed(2);

            var totalMaintenanceExp = $(this).jqGrid('getCol', 'MaintenanceExp', false, 'sum').toFixed(2);

            $(this).jqGrid('footerData', 'set', { State: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { SanctionWorks: totalSanctionWorks });
            $(this).jqGrid('footerData', 'set', { SanctionLength: totalSanctionLength });
            $(this).jqGrid('footerData', 'set', { SanctionCost: totalSanctionCost });

            //$(this).jqGrid('footerData', 'set', { AwardedWorks: totalAwardedWorks });
            //$(this).jqGrid('footerData', 'set', { AwardedLength: totalAwardedLength });
            //$(this).jqGrid('footerData', 'set', { AwardedCost: totalAwardedCost });

            $(this).jqGrid('footerData', 'set', { CompletedWorks: totalCompletedWorks });
            $(this).jqGrid('footerData', 'set', { CompletedLength: totalCompletedLength });
            $(this).jqGrid('footerData', 'set', { CompletedCost: totalCompletedCost });

            $(this).jqGrid('footerData', 'set', { OngoingWorks: totalOngoingWorks });
            $(this).jqGrid('footerData', 'set', { OngoingLength: totalOngoingLength });
            $(this).jqGrid('footerData', 'set', { OngoingCost: totalOngoingCost });

            $(this).jqGrid('footerData', 'set', { MaintenanceExp: totalMaintenanceExp });

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


    $("#tbTechnicalDetailsDistrictList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'SanctionWorks', numberOfColumns: 3, titleText: 'Sanction Details' },
                        //{ startColumnName: 'AwardedWorks', numberOfColumns: 3, titleText: 'Award Details' },
                        { startColumnName: 'CompletedWorks', numberOfColumns: 3, titleText: 'Completed Details' },
                        { startColumnName: 'OngoingWorks', numberOfColumns: 3, titleText: 'Ongoing Details' }
        ]
    });
}


function viewBlockwiseTechnicalDetails(districtCode, districtName, stateCode) {

    $("#MAST_DISTRICT_CODE_PHYSICAL").val(districtCode);
    createBlockDetailsGrid(districtCode, districtName, stateCode);
    $("#divChartLabel").html(districtName);
    commonFunctions();
}


function createBlockDetailsGrid(districtCode, districtName, stateCode) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#tbTechnicalDetailsDistrictList').jqGrid('setSelection', districtCode);
    $('#tbTechnicalDetailsDistrictList').jqGrid('setGridState', 'hidden');

    $("#tbTechnicalDetailsBlockList").jqGrid('GridUnload');

    jQuery("#tbTechnicalDetailsBlockList").jqGrid({
        url: '/Dashboard/ListBlockwiseTechnicalDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["State", "Works", "Length", "Cost", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colNames: ["Block", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colModel: [
                        { name: 'Block', index: 'Block', width: 120, sortable: true, align: "left" },

                        { name: 'SanctionWorks', index: 'SanctionWorks', width: 70, sortable: true, align: "right" },
                        { name: 'SanctionLength', index: 'SanctionLength', width: 70, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 3 } },
                        { name: 'SanctionCost', index: 'SanctionCost', width: 80, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },

                        //{ name: 'AwardedWorks', index: 'AwardedWorks', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedLength', index: 'AwardedLength', width: 70, sortable: true, align: "right" },
                        //{ name: 'AwardedCost', index: 'AwardedCost', width: 80, sortable: true, align: "right" },

                        { name: 'CompletedWorks', index: 'CompletedWorks', width: 70, sortable: true, align: "right" },
                        { name: 'CompletedLength', index: 'CompletedLength', width: 70, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 3 } },
                        { name: 'CompletedCost', index: 'CompletedCost', width: 80, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },

                        { name: 'OngoingWorks', index: 'OngoingWorks', width: 70, sortable: true, align: "right" },
                        { name: 'OngoingLength', index: 'OngoingLength', width: 70, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 3 } },
                        { name: 'OngoingCost', index: 'OngoingCost', width: 80, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } },

                        { name: 'MaintenanceExp', index: 'MaintenanceExp', width: 80, sortable: true, align: "right", formatter: "number", formatoptions: { decimalPlaces: 2 } }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "fundingAgency": $("#ddlCollaborationsForPhysical").val() },
        pager: jQuery('#dvTechnicalDetailsBlockListPager'),
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;All Blocks of " + districtName,
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalSanctionWorks = $(this).jqGrid('getCol', 'SanctionWorks', false, 'sum');
            var totalSanctionLength = $(this).jqGrid('getCol', 'SanctionLength', false, 'sum').toFixed(2);
            var totalSanctionCost = $(this).jqGrid('getCol', 'SanctionCost', false, 'sum').toFixed(2);

            //var totalAwardedWorks = $(this).jqGrid('getCol', 'AwardedWorks', false, 'sum');
            //var totalAwardedLength = $(this).jqGrid('getCol', 'AwardedLength', false, 'sum').toFixed(2);
            //var totalAwardedCost = $(this).jqGrid('getCol', 'AwardedCost', false, 'sum').toFixed(2);

            var totalCompletedWorks = $(this).jqGrid('getCol', 'CompletedWorks', false, 'sum');
            var totalCompletedLength = $(this).jqGrid('getCol', 'CompletedLength', false, 'sum').toFixed(2);
            var totalCompletedCost = $(this).jqGrid('getCol', 'CompletedCost', false, 'sum').toFixed(2);

            var totalOngoingWorks = $(this).jqGrid('getCol', 'OngoingWorks', false, 'sum');
            var totalOngoingLength = $(this).jqGrid('getCol', 'OngoingLength', false, 'sum').toFixed(2);
            var totalOngoingCost = $(this).jqGrid('getCol', 'OngoingCost', false, 'sum').toFixed(2);

            var totalMaintenanceExp = $(this).jqGrid('getCol', 'MaintenanceExp', false, 'sum').toFixed(2);

            $(this).jqGrid('footerData', 'set', { State: '<b>Total</b>' });

            $(this).jqGrid('footerData', 'set', { SanctionWorks: totalSanctionWorks });
            $(this).jqGrid('footerData', 'set', { SanctionLength: totalSanctionLength });
            $(this).jqGrid('footerData', 'set', { SanctionCost: totalSanctionCost });

            //$(this).jqGrid('footerData', 'set', { AwardedWorks: totalAwardedWorks });
            //$(this).jqGrid('footerData', 'set', { AwardedLength: totalAwardedLength });
            //$(this).jqGrid('footerData', 'set', { AwardedCost: totalAwardedCost });

            $(this).jqGrid('footerData', 'set', { CompletedWorks: totalCompletedWorks });
            $(this).jqGrid('footerData', 'set', { CompletedLength: totalCompletedLength });
            $(this).jqGrid('footerData', 'set', { CompletedCost: totalCompletedCost });

            $(this).jqGrid('footerData', 'set', { OngoingWorks: totalOngoingWorks });
            $(this).jqGrid('footerData', 'set', { OngoingLength: totalOngoingLength });
            $(this).jqGrid('footerData', 'set', { OngoingCost: totalOngoingCost });

            $(this).jqGrid('footerData', 'set', { MaintenanceExp: totalMaintenanceExp });

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


    $("#tbTechnicalDetailsBlockList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'SanctionWorks', numberOfColumns: 3, titleText: 'Sanction Details' },
                        //{ startColumnName: 'AwardedWorks', numberOfColumns: 3, titleText: 'Award Details' },
                        { startColumnName: 'CompletedWorks', numberOfColumns: 3, titleText: 'Completed Details' },
                        { startColumnName: 'OngoingWorks', numberOfColumns: 3, titleText: 'Ongoing Details' }
        ]
    });
}

//---------------------------- Grid Code Ends Here ---------------------------------//


//---------------------------- Works Column Chart Starts Here ---------------------------------//

//function CommonOptionsForWorksColumnChart(containerDivID) {

//    if ($('#' + containerDivID).highcharts()) {
//        $('#' + containerDivID).highcharts().destroy();
//    }

//    var options = {
//        chart:
//            {
//                renderTo: containerDivID
//            },
//        title: {
//            text: '',
//            x: -20 //center
//        },
//        subtitle: {
//            text: '',
//            x: -20
//        },
//        xAxis: {
//            gridLineWidth: 1,
//            categories: []
//        },
//        yAxis: [{
//            title: {
//                text: 'No. of Works'
//            },
//            min: 0,
//            labels: {
//                format: '{value}',
//                style: {
//                    color: '#4572A7'
//                }
//            }
//        }],
//        tooltip:
//            {
//                headerFormat: '<span style="font-size:11px"> Year: {point.key} </span>' +
//                              '<table>',
//                pointFormat: 
//                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
//                             '<td style="padding:0;font-size:10px;"><b>{point.y}</b></td></tr>',
//                footerFormat: '</table>',
//                shared: true,
//                useHTML: true
//            },
//        //legend: {
//        //    layout: 'horizontal',
//        //    align: 'right',
//        //    verticalAlign: 'middle',
//        //    borderWidth: 0
//        //},
//        series: [],
//        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248']

//    };

//    return options;
//}


//function chartWorksColumnChart(chart, containerID) {
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $.ajax({
//        type: "POST",
//        url: '/Dashboard/WorksColumnChart/',
//        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val() },
//        error: function (xhr, status, error) {
//            $.unblockUI();
//            $('#errorSpan').text(xhr.responseText);
//            $('#divError').show('slow');
//            return false;
//        },
//        success: function (data) {
//            var series1 = null; var series2 = null; var series3 = null; var series4 = null;

//            if (data == "") {
//                if ($('#' + containerID).highcharts()) {
//                    $('#' + containerID).highcharts().destroy();
//                }
//            }
//            else {

//                series1 = {
//                    data: []
//                };
//                series2 = {
//                    data: []
//                };
//                series3 = {
//                    data: []
//                };
//                series4 = {
//                    data: []
//                };


//                $.each(data, function (item) {
//                    series1.name = "Sanction Works";
//                    series1.type = "column";
//                    series1.data.push({ x: this.Year, y: parseFloat(this.SanctionWorks) });

//                    series2.name = "Awarded Works";
//                    series2.type = "column";
//                    series2.data.push({ x: this.Year, y: parseFloat(this.AwardedWorks) });

//                    series3.name = "Completed Works";
//                    series3.type = "column";
//                    series3.data.push({ x: this.Year, y: parseFloat(this.CompletedWorks) });

//                    series4.name = "Progress Works";
//                    series4.type = "column";
//                    series4.data.push({ x: this.Year, y: parseFloat(this.ProgWorks) });
//                });

//                optionsColumn = CommonOptionsForWorksColumnChart(containerID);
//                optionsColumn.series.push(series1);
//                optionsColumn.series.push(series2);
//                optionsColumn.series.push(series3);
//                optionsColumn.series.push(series4);

//                chart = new Highcharts.Chart(optionsColumn);
//                // code to display animation
//                chart.series[0].setVisible(true, true);
//                chart.series[1].setVisible(true, true);
//                chart.series[2].setVisible(true, true);
//                chart.series[3].setVisible(true, true);
//            }

//            $.unblockUI();
//        }
//    });


//}

//---------------------------- Works Column Chart Ends Here ---------------------------------//


//---------------------------- Length Column Chart Starts Here ---------------------------------//


//function CommonOptionsForRdLengthColumnChart(containerDivID) {

//    if ($('#' + containerDivID).highcharts()) {
//        $('#' + containerDivID).highcharts().destroy();
//    }

//    var options = {
//        chart:
//            {
//                renderTo: containerDivID,
//                type:'column'
//            },
//        title: {
//            text: ''
//        },
//        subtitle: {
//            text: ''
//        },
//        xAxis: {
//            gridLineWidth: 1,
//            categories: []
//        },
//        yAxis: [{           //Primary Axis
//                title: {
//                    text: 'Road Length (Kms.)'
//                },
//                min: 0,
//                labels: {
//                    format: '{value}',
//                    style: {
//                        color: '#4572A7'
//                    }
//                }
//            }],
//        tooltip:
//            {
//                headerFormat: '<span style="font-size:11px"> Year: {point.key} </span>' +
//                              '<table>',
//                pointFormat:
//                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
//                             '<td style="padding:0;font-size:10px;"><b>{point.y:.2f}Kms.</b></td></tr>',
//                footerFormat: '</table>',
//                shared: true,
//                useHTML: true
//            },
//        legend: {
//            itemStyle: {
//                //color: '#000',
//                //fontFamily: 'font-family: Trebuchet MS,Tahoma,Verdana,Arial,sans-serif;',
//                //fontSize: '.9em'
//            }
//        },
//        series: [],
//        colors: ['#9966CC', '#20B2AA', '#9ACD32', '#3090C7', '#FCB319', '#7B68EE', '#006400', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248', '#AA8C30']

//    };

//    return options;
//}


//function chartRdLengthColumnChart(chart, containerID) {
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//    var proposalType = $('input:radio[name="PROPOSAL_TYPE"]:checked').val();

//    $.ajax({
//        type: "POST",
//        url: '/Dashboard/LengthColumnChart/',
//        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'proposalType': proposalType },
//        error: function (xhr, status, error) {
//            $.unblockUI();
//            $('#errorSpan').text(xhr.responseText);
//            $('#divError').show('slow');
//            return false;
//        },
//        success: function (data) {
//            var series1 = null; var series2 = null; var series3 = null; var series4 = null;
//            if (data == "") {
//                if ($('#' + containerID).highcharts()) {
//                    $('#' + containerID).highcharts().destroy();
//                }
//            }
//            else {

//                series1 = {
//                    data: []
//                };
//                series2 = {
//                    data: []
//                };
//                series3 = {
//                    data: []
//                };
//                series4 = {
//                    data: []
//                };

//                $.each(data, function (item) {

//                    series1.name = "Sanction Length";
//                    series1.data.push({ x: this.Year, y: parseFloat(this.SanctionRdLength) });

//                    series2.name = "Awarded Length";
//                    series2.data.push({ x: this.Year, y: parseFloat(this.AwardedRdLength) });

//                    series3.name = "Completed Length";
//                    series3.data.push({ x: this.Year, y: parseFloat(this.CompletedRdLength) });

//                    series4.name = "Progress Length";
//                    series4.data.push({ x: this.Year, y: parseFloat(this.ProgRdLength) });

//                });

//                optionsColumn = CommonOptionsForRdLengthColumnChart(containerID);
//                optionsColumn.series.push(series1);
//                optionsColumn.series.push(series2);
//                optionsColumn.series.push(series3);
//                optionsColumn.series.push(series4);

//                chart = new Highcharts.Chart(optionsColumn);
//                chart.series[0].setVisible(true, true);
//                chart.series[1].setVisible(true, true);
//                chart.series[2].setVisible(true, true);
//                chart.series[3].setVisible(true, true);
//            }

//            $.unblockUI();
//        }
//    });
//}



//function CommonOptionsForBridgeLengthColumnChart(containerDivID) {

//    if ($('#' + containerDivID).highcharts()) {
//        $('#' + containerDivID).highcharts().destroy();
//    }

//    var options = {
//        chart:
//            {
//                renderTo: containerDivID,
//                type: 'column'
//            },
//        title: {
//            text: ''
//        },
//        subtitle: {
//            text: ''
//        },
//        xAxis: {
//            gridLineWidth: 1,
//            categories: []
//        },
//        yAxis: [{  
//                title: {
//                    text: 'Bridge Length (Mtrs)',
//                    style: {
//                        color: '#89A54E'
//                    }
//                },
//                min: 0,
//                labels: {
//                    format: '{value}',
//                    style: {
//                        color: '#89A54E'
//                    }
//                }
//            }
//        ],
//        tooltip:
//            {
//                headerFormat: '<span style="font-size:11px"> Year: {point.key} </span>' +
//                              '<table>',
//                pointFormat:
//                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
//                             '<td style="padding:0;font-size:10px;"><b>{point.y:.2f}mtrs.</b></td></tr>',
//                footerFormat: '</table>',
//                shared: true,
//                useHTML: true
//            },
//        legend: {
//            itemStyle: {
//                //color: '#000',
//                //fontFamily: 'font-family: Trebuchet MS,Tahoma,Verdana,Arial,sans-serif;',
//                //fontSize: '.9em'
//            }
//        },
//        series: [],
//        colors: ['#9966CC', '#20B2AA', '#9ACD32', '#3090C7', '#FCB319', '#7B68EE', '#006400', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248', '#AA8C30']

//    };

//    return options;
//}


//function chartLSBLengthColumnChart(chart, containerID) {
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
//    var proposalType = $('input:radio[name="PROPOSAL_TYPE"]:checked').val();

//    $.ajax({
//        type: "POST",
//        url: '/Dashboard/LengthColumnChart/' ,
//        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'proposalType': proposalType },
//        error: function (xhr, status, error) {
//            $.unblockUI();
//            $('#errorSpan').text(xhr.responseText);
//            $('#divError').show('slow');
//            return false;
//        },
//        success: function (data) {
//            var series1 = null; var series2 = null; var series3 = null; var series4 = null;
//            if (data == "") {
//                if ($('#' + containerID).highcharts()) {
//                    $('#' + containerID).highcharts().destroy();
//                }
//            }
//            else {

//                series1 = {
//                    data: []
//                };
//                series2 = {
//                    data: []
//                };
//                series3 = {
//                    data: []
//                };
//                series4 = {
//                    data: []
//                };

//                $.each(data, function (item) {

//                        series1.name = "Sanction Length";
//                        series1.data.push({ x: this.Year, y: parseFloat(this.SanctionLSBLength) });

//                        series2.name = "Awarded Length";
//                        series2.data.push({ x: this.Year, y: parseFloat(this.AwardedLSBLength) });

//                        series3.name = "Completed Length";
//                        series3.data.push({ x: this.Year, y: parseFloat(this.CompletedLSBLength) });

//                        series4.name = "Progress Length";
//                        series4.data.push({ x: this.Year, y: parseFloat(this.ProgLSBLength) });
//                });

//                optionsColumn = CommonOptionsForBridgeLengthColumnChart(containerID);
//                optionsColumn.series.push(series1);
//                optionsColumn.series.push(series2);
//                optionsColumn.series.push(series3);
//                optionsColumn.series.push(series4);

//                chart = new Highcharts.Chart(optionsColumn);
//                chart.series[0].setVisible(true, true);
//                chart.series[1].setVisible(true, true);
//                chart.series[2].setVisible(true, true);
//                chart.series[3].setVisible(true, true);
//            }

//            $.unblockUI();
//        }
//    });


//}


//---------------------------- Length Column Chart Ends Here ---------------------------------//


//---------------------------- Cost Column Chart Starts Here ---------------------------------//


//function CommonOptionsForCostColumnChart(containerDivID) {

//    if ($('#' + containerDivID).highcharts()) {
//        $('#' + containerDivID).highcharts().destroy();
//    }

//    var options = {
//        chart:
//            {
//                renderTo: containerDivID,
//                type : "column"
//            },
//        title: {
//            text: ''
//        },
//        subtitle: {
//            text: ''
//        },
//        xAxis: {
//            gridLineWidth: 1,
//            categories: []
//        },
//        yAxis: [{
//            title: {
//                text: 'Cost (in Lacs)'
//            },
//            min: 0,
//            labels: {
//                format: '{value}',
//                style: {
//                    color: '#4572A7'
//                }
//            }
//        }],
//        tooltip:
//            {
//                headerFormat: '<span style="font-size:11px"> Year: {point.key} </span>' +
//                              '<table>',
//                pointFormat:
//                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
//                             '<td style="padding:0;font-size:10px;"><b>{point.y:.2f}Lacs</b></td></tr>',
//                footerFormat: '</table>',
//                shared: true,
//                useHTML: true
//            },
//        series: [],
//        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248']

//    };

//    return options;
//}


//function chartCostColumnChart(chart, containerID) {
//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $.ajax({
//        type: "POST",
//        url: '/Dashboard/CostColumnChart/',
//        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val() },
//        error: function (xhr, status, error) {
//            $.unblockUI();
//            $('#errorSpan').text(xhr.responseText);
//            $('#divError').show('slow');
//            return false;

//        },
//        success: function (data) {
//            var series1 = null; var series2 = null; var series3 = null; 

//            if (data == "") {
//                if ($('#' + containerID).highcharts()) {
//                    $('#' + containerID).highcharts().destroy();
//                }
//            }
//            else {

//                series1 = {
//                    data: []
//                };
//                series2 = {
//                    data: []
//                };
//                series3 = {
//                    data: []
//                };


//                $.each(data, function (item) {
//                    series1.name = "Sanction Cost";
//                    series1.data.push({ x: this.Year, y: parseFloat(this.SanctionTotal) });

//                    series2.name = "Awarded Cost";
//                    series2.data.push({ x: this.Year, y: parseFloat(this.AwardedTotal) });

//                    series3.name = "Expenditure";
//                    series3.data.push({ x: this.Year, y: parseFloat(this.Exp) });

//                });

//                optionsColumn = CommonOptionsForCostColumnChart(containerID);
//                optionsColumn.series.push(series1);
//                optionsColumn.series.push(series2);
//                optionsColumn.series.push(series3);

//                chart = new Highcharts.Chart(optionsColumn);
//                // code to display animation
//                chart.series[0].setVisible(true, true);
//                chart.series[1].setVisible(true, true);
//                chart.series[2].setVisible(true, true);
//            }

//            $.unblockUI();
//        }
//    });


//}


//---------------------------- Cost Column Chart Ends Here ---------------------------------//


//----------------------Work Len Exp Year Wise Grid Start-------------------//


function WorkLenExpYearWiseDetailsGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tblWorkLengthExpYearWiseGrid").jqGrid('GridUnload');

    jQuery("#tblWorkLengthExpYearWiseGrid").jqGrid({
        url: '/Dashboard/WorkLenghtExpYearWiseStateWiseGrid?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "Financial Year", "Works (in no.)", "Length (in Km.)", "Expenditure (in Crore.)"],
        colModel: [
                        { name: 'LOCATION_NAME', index: 'LOCATION_NAME', width: 300, sortable: false, align: "center", hidden: true },
                        { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 300, sortable: false, align: "center" },
                        { name: 'PROPOSALS', index: 'PROPOSALS', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { thousandsSeparator: "," } },
                        { name: 'LENGTH_COMPLETED', index: 'LENGTH_COMPLETED', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 } },
                        { name: 'EXPENDITURE', index: 'EXPENDITURE', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 } }
        ],        
        postData: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'isYearWise': true },
        pager: jQuery('#dvWorkLengthExpYearWiseGridPager'),
        pgbuttons: false,
        pginput:false,
        rowNum: 999,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Year Wise Details",
        autowidth: true,        
        shrinkToFit:true,
        sortname: 'LOCATION_NAME',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalWorks = $(this).jqGrid('getCol', 'PROPOSALS', false, 'sum');
            var totalLength = $(this).jqGrid('getCol', 'LENGTH_COMPLETED', false, 'sum').toFixed(2);
            var totalExp = $(this).jqGrid('getCol', 'EXPENDITURE', false, 'sum').toFixed(2);
                        
            $(this).jqGrid('footerData', 'set', { IMS_YEAR: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PROPOSALS: totalWorks });
            $(this).jqGrid('footerData', 'set', { LENGTH_COMPLETED: totalLength});
            $(this).jqGrid('footerData', 'set', { EXPENDITURE: totalExp });

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

function WorkLenExpStateWiseDetailsGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //$('#tbTechnicalDetailsDistrictList').jqGrid('setSelection', districtCode);
    //$('#tbTechnicalDetailsDistrictList').jqGrid('setGridState', 'hidden');

    //$("#tblWorkLengthExpYearWiseGrid").jqGrid('GridUnload');

    $("#tblWorkLengthExpStateWiseGrid").jqGrid('GridUnload');

    jQuery("#tblWorkLengthExpStateWiseGrid").jqGrid({
        url: '/Dashboard/WorkLenghtExpYearWiseStateWiseGrid?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        //colNames: ["State", "Works", "Length", "Cost", "Works", "Length", "Cost", "Works", "Length", "Expenditure", "Works", "Length", "Expenditure", "Maintenance Expenditure"],
        colNames: ["State", "Financial Year", "Works (in no.)", "Length (in Km.)", "Expenditure (in Crore.)"],
        colModel: [
                        { name: 'LOCATION_NAME', index: 'LOCATION_NAME', width: 300, sortable: false, align: "center" },
                        { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 300, sortable: false, align: "center", hidden: true },
                        { name: 'PROPOSALS', index: 'PROPOSALS', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { thousandsSeparator: "," } },
                        { name: 'LENGTH_COMPLETED', index: 'LENGTH_COMPLETED', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 } },
                        { name: 'EXPENDITURE', index: 'EXPENDITURE', width: 235, sortable: false, align: "right", formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2 } }
        ],
        postData: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'isYearWise': false },
        pager: jQuery('#dvWorkLengthExpStateWiseGridPager'),
        rowNum: 999,
        viewrecords: true,
        pgbuttons: false,
        pginput: false,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;State Wise Details",
        autowidth: true,                
        sortname: 'LOCATION_NAME',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

            var totalWorks = $(this).jqGrid('getCol', 'PROPOSALS', false, 'sum');
            var totalLength = $(this).jqGrid('getCol', 'LENGTH_COMPLETED', false, 'sum').toFixed(2);
            var totalExp = $(this).jqGrid('getCol', 'EXPENDITURE', false, 'sum').toFixed(2);

            $(this).jqGrid('footerData', 'set', { LOCATION_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { PROPOSALS: totalWorks });
            $(this).jqGrid('footerData', 'set', { LENGTH_COMPLETED: totalLength });
            $(this).jqGrid('footerData', 'set', { EXPENDITURE: totalExp });

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

//----------------------Work Len Exp Year Wise Grid end-------------------//



//---------------------------- Works Length Exp Year Wise Column Chart Starts Here ---------------------------------//


function CommonOptionsForWorkLenghtExpYearWiseColumnChart(containerDivID) {

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: 'column',
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
            //type: 'category',
            gridLineWidth: 1,
            categories: [],
            //rotation:-20
            //title: {
            //    rotation: 0,
            //    // text: "Financial Year"
            //     text: ""
            //},
            labels: {
                rotation: 0,
                //style: {
                //    fontSize: '13px',
                //    fontFamily: 'Verdana, sans-serif'
                //}
            }
        },
        yAxis: [{
            title: {
                //  text: 'No. of Works,Length,Expenditure'
                text: ''
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
                headerFormat: '<span style="font-size:11px"> {point.key} </span>' +
                              '<table>',
                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y:.2f}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },       
        series: [],
        exporting: {
            enabled: true
        },
        plotOptions: {
            column: {              
                cursor: 'pointer',
                point: {
                    events: {
                        click: function (e) {

                         //   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                            chartYearly.showLoading('Loadding ...');

                            var YearName = e.point.code;
                            //Ajax call to get data start
                            var drillSerisArray = [];

                            if (flagShowYearlyDrillChart) {
                                $.ajax({
                                    type: "POST",
                                    url: '/Dashboard/WorkLenghtExpYearWiseStateWiseColumnChart/',
                                    data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'ImsYear': e.point.code, 'isYearWise': false },
                                    error: function (xhr, status, error) {
                                        $.unblockUI();
                                        $('#errorSpan').text(xhr.responseText);
                                        $('#divError').show('slow');
                                        return false;
                                    },
                                    success: function (data) {

                                        if (data == "") {
                                            alert("Details are not present.");
                                            chartYearly = new Highcharts.Chart(YearWiseOptionsColumn);
                                            return false;
                                        }
                                        else {

                                            var drillSeries1 = {
                                                name: 'Works',
                                                data: [],
                                                color: '#9ACD32'
                                            };

                                            var drillSeries2 = {
                                                name: 'Length',
                                                data: [],
                                                color: '#FCB319'
                                            };

                                            var drillSeries3 = {
                                                name: 'Expenditure',
                                                data: [],
                                                color: '#20B2AA'
                                            };
                                            $.each(data, function (item) {
                                                drillSeries1.data.push({ name: this.LOCATION_NAME, y: this.PROPOSALS });
                                                drillSeries2.data.push({ name: this.LOCATION_NAME, y: this.LENGTH_COMPLETED });
                                                drillSeries3.data.push({ name: this.LOCATION_NAME, y: this.EXPENDITURE });
                                            });

                                            // alert('test');
                                            drillSerisArray.push(drillSeries1);
                                            drillSerisArray.push(drillSeries2);
                                            drillSerisArray.push(drillSeries3);
                                        }

                                        //Reload Grid  
                                        $('#tblWorkLengthExpYearWiseGrid').setGridParam({ postData: { ImsYear:YearName, 'isYearWise': false } });
                                        $("#tblWorkLengthExpYearWiseGrid").hideCol("IMS_YEAR");
                                        $("#tblWorkLengthExpYearWiseGrid").showCol("LOCATION_NAME");
                                        //$("#tblWorkLengthExpYearWiseGrid").jqGrid('setGridWidth', parseInt($("#divPhysicalSection2Tab").width()) + 400);
                                        $('#tblWorkLengthExpYearWiseGrid').trigger("reloadGrid");
                                        //gbox_tblWorkLengthExpYearWiseGrid
                                        //alert($('#divPhysicalSection2Tab-4').width());
                                        $("#tblWorkLengthExpYearWiseGrid").jqGrid('setGridWidth', parseInt($("#dvWorkLengthExpYearWiseGridContainer").width())-50);

                                        //$("#tblWorkLengthExpYearWiseGrid").setGridWidth(parseInt($("#divPhysicalSection2Tab").width()) + 200);
                                        //divPhysicalSection2Tab-4

                                    },
                                    complete: function () {
                                        drillSeriesYearlyData.series = drillSerisArray;
                                        var drilldown = drillSeriesYearlyData;
                                        if (chartLabelYearlyFlag) {
                                            chartLabelYearly = $("#divChartLabel").text();
                                        }
                                        chartLabelYearlyFlag = false;                                       
                                            flagShowYearlyDrillChart = false;                                            
                                            setYearWiseChart(null, drilldown.categories, drilldown);
                                            $("#divChartLabel").html("<b>" + parseInt(YearName) + "-" + (parseInt(YearName) + 1) + "</b>");
                                            chartYearly.hideLoading();

                                            chartYearly.setTitle({
                                                text: parseInt(YearName) + "-" + (parseInt(YearName)+1),
                                                style: {
                                                    //color: '#FF00FF',
                                                    fontWeight: 'bold',
                                                    fontSize: '10px',
                                                }
                                            })

                                            $.unblockUI();
                                    }
                                });
                            } else {
                                flagShowYearlyDrillChart = true;
                                chartYearly = new Highcharts.Chart(YearWiseOptionsColumn);
                                $("#divChartLabel").html("<b>" + chartLabelYearly + "</b>");
                                chartYearly.hideLoading();
                                // code to display animation

                                //Reload Grid                                               

                                $('#tblWorkLengthExpYearWiseGrid').setGridParam({ postData: { ImsYear: 0, 'isYearWise': true } });
                                $("#tblWorkLengthExpYearWiseGrid").showCol("IMS_YEAR");
                                $("#tblWorkLengthExpYearWiseGrid").hideCol("LOCATION_NAME");
                                //$("#tblWorkLengthExpYearWiseGrid").jqGrid('setGridWidth', parseInt($("#divPhysicalSection2Tab").width()) + 400);


                                $('#tblWorkLengthExpYearWiseGrid').trigger("reloadGrid");
                                //$("#tblWorkLengthExpYearWiseGrid").setGridWidth(parseInt($("#divPhysicalSection2Tab").width())+200);

                            }                         
                        }
                    }
                },

            }
        },
       
        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248']

    };

    return options;
}

function CommonOptionsForWorkLenghtExpStateWiseColumnChart(containerDivID) {

    if ($('#' + containerDivID).highcharts()) {
        $('#' + containerDivID).highcharts().destroy();
    }

    var options = {
        chart:
            {
                renderTo: containerDivID,
                type: 'column',
            },
        title: {
            text: '',
           // x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {            
            gridLineWidth: 1,
            categories: [],
            //title: {
            //    text: 'State Wise details of ',                
            //},            
            //rotation:-20
            //title: {
            //    rotation: 0,
            //    // text: "Financial Year"
            //     text: ""
            //},
            labels: {
                rotation: 0,                
            }
        },
        yAxis: [{
            title: {                
                text: ''
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
                headerFormat: '<span style="font-size:11px"> {point.key} </span>' +
                              '<table>',
                pointFormat:
                             '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                             '<td style="padding:0;font-size:10px;"><b>{point.y:.2f}</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true,
                //toolTip
            },
        
        series: [],
        exporting: {
            enabled: true
        },
        plotOptions: {
            column: {                
                cursor: 'pointer',
                point: {
                    events: {
                        click: function (e) {
                            Newchart.showLoading('Loadding ...');

                            var StateName = e.point.name;
                            //Ajax call to get data start
                            var drillSerisArray = [];

                            if (flagShowStateDrillChart)
                            {
                                $.ajax({
                                    type: "POST",
                                    url: '/Dashboard/WorkLenghtExpYearWiseStateWiseColumnChart/',
                                    data: { 'stateCode': e.point.code, 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'isYearWise': true },
                                    error: function (xhr, status, error) {
                                        $.unblockUI();
                                        $('#errorSpan').text(xhr.responseText);
                                        $('#divError').show('slow');
                                        return false;
                                    },
                                    success: function (data) {
                                        if (data == "") {                                        
                                            alert("Details are not present.");
                                            return false;
                                        }
                                        else {
                                            var drillSeries1 = {
                                                name: 'Works',
                                                data: [],
                                                color: '#9ACD32'
                                            };
                                            var drillSeries2 = {
                                                name: 'Length',
                                                data: [],
                                                color: '#FCB319'

                                            };
                                            var drillSeries3 = {
                                                name: 'Expenditure',
                                                data: [],
                                                color: '#20B2AA'
                                            };
                                            $.each(data, function (item) {
                                                drillSeries1.data.push({ name: (this.IMS_YEAR.toString().split('-')[0]), y: this.PROPOSALS });
                                                drillSeries2.data.push({ name: (this.IMS_YEAR.toString().split('-')[0]), y: this.LENGTH_COMPLETED });                                            
                                                drillSeries3.data.push({ name: (this.IMS_YEAR.toString().split('-')[0]), y: this.EXPENDITURE });
                                            });

                                            drillSerisArray.push(drillSeries1);
                                            drillSerisArray.push(drillSeries2);
                                            drillSerisArray.push(drillSeries3);
                                        }

                                        //Reload Grid  
                                        $('#tblWorkLengthExpStateWiseGrid').setGridParam({ postData: { stateCode: e.point.code, 'isYearWise': true } });
                                        $("#tblWorkLengthExpStateWiseGrid").showCol("IMS_YEAR");
                                        $("#tblWorkLengthExpStateWiseGrid").hideCol("LOCATION_NAME");                                        
                                        $('#tblWorkLengthExpStateWiseGrid').trigger("reloadGrid");
                                        $("#tblWorkLengthExpStateWiseGrid").jqGrid('setGridWidth', parseInt($("#dvWorkLengthExpStateWiseGridContainer").width())-60);
                                        $.unblockUI();
                                    },
                                    complete: function () {
                                        drillSeriesStateData.series = drillSerisArray;
                                        var drilldown = drillSeriesStateData;

                                        if (chartLabelFlag) {
                                            chartLabel = $("#divChartLabel").text();
                                        }
                                        chartLabelFlag = false;                                       
                                            flagShowStateDrillChart = false;
                                            setStateWiseChart(null, drilldown.categories, drilldown);
                                            $("#divChartLabel").html("<b>" + StateName + "</b>");
                                            Newchart.hideLoading();

                                            Newchart.setTitle({
                                                text: StateName,
                                                style: {
                                                    //color: '#FF00FF',
                                                    fontWeight: 'bold',
                                                    fontSize: '10px',
                                                }
                                            })
                                    }
                                
                            });
                            } else {
                                Newchart = new Highcharts.Chart(StateWiseoptionsColumn);
                                flagShowStateDrillChart = true;
                                $("#divChartLabel").html("<b>" + chartLabel + "</b>");
                                Newchart.hideLoading();
                                //Reload Grid  
                                $('#tblWorkLengthExpStateWiseGrid').setGridParam({ postData: { stateCode: $("#MAST_STATE_CODE_PHYSICAL").val(), 'isYearWise': false } });
                                $("#tblWorkLengthExpStateWiseGrid").hideCol("IMS_YEAR");
                                $("#tblWorkLengthExpStateWiseGrid").showCol("LOCATION_NAME");
                                //$("#tblWorkLengthExpStateWiseGrid").jqGrid('setGridWidth', parseInt($("#divPhysicalSection2Tab").width()) + 400);
                                $('#tblWorkLengthExpStateWiseGrid').trigger("reloadGrid");
                            }
                        
                        }
                    }
                },
                dataLabels: {
                    enabled: false,
                    color: 'black',
                    style: {
                        fontWeight: 'normal'
                    },
                    formatter: function () {
                        return this.y + ' ';
                    },
                    formatter: function () {
                        return Highcharts.numberFormat(this.y, 2, '.')
                    }
                }
            }
        },       
        colors: ['#9ACD32', '#FCB319', '#20B2AA', '#AA8C30', '#006400', '#7B68EE', '#3090C7', '#DC143C', '#00526F', '#594266', '#cb6828', '#aaaaab', '#B2C248']

    };

    return options;
}

function chartWorkLenghtExpYearWiseColumnChart(chart, containerID) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: '/Dashboard/WorkLenghtExpYearWiseStateWiseColumnChart/',
        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'isYearWise': true },
        error: function (xhr, status, error) {
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;

        },
        success: function (data) {
            var series1 = null; var series2 = null; var series3 = null;
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
                drillSeriesYearlyData = {
                    series: []
                };

                $.each(data, function (item) {
                    //Xcategories.push(this.IMS_YEAR);
                    series1.name = "Works";                    
                    series1.data.push({ x: this.IMS_YEAR.split('-')[0] ,code: this.IMS_YEAR.toString().split('-')[0], y: parseFloat(this.PROPOSALS), drilldown: drillSeriesYearlyData });                    
                    series2.name = "Length";                    
                    series2.data.push({ x: this.IMS_YEAR.split('-')[0], code: this.IMS_YEAR.toString().split('-')[0], y: parseFloat(this.LENGTH_COMPLETED), drilldown: drillSeriesYearlyData });
                    series3.name = "Expenditure";                    
                    series3.data.push({ x: this.IMS_YEAR.split('-')[0], code: this.IMS_YEAR.toString().split('-')[0], y: parseFloat(this.EXPENDITURE), drilldown: drillSeriesYearlyData });
                });
                
                YearWiseOptionsColumn = CommonOptionsForWorkLenghtExpYearWiseColumnChart(containerID);
                YearWiseOptionsColumn.series.push(series1);
                YearWiseOptionsColumn.series.push(series2);
                YearWiseOptionsColumn.series.push(series3);

                YearWiseOptionsColumn.xAxis.labels.rotation = -07;

                chartYearly = new Highcharts.Chart(YearWiseOptionsColumn);
                // code to display animation
                chartYearly.series[0].setVisible(true, true);
                chartYearly.series[1].setVisible(true, true);
                chartYearly.series[2].setVisible(true, true);

                //chartYearly.series[0].setVisible();


                
            }

            $.unblockUI();
        }
    });


}

function setStateWiseChart(name, categories, data, color) {
    
    var len = Newchart.series.length;
    
    for (var i = 0; i < len; i++) {
    
        Newchart.series[0].remove();
    }
    
        if (data.series) {
            for (var i = 0; i < data.series.length; i++) {
                Newchart.addSeries({
                    name: data.series[i].name,
                    data: data.series[i].data,
                    // color: data.series[i].color || 'white'
                });
            }
        }
}

function setYearWiseChart(name, categories, data, color) {
    
    var len = chartYearly.series.length;   
    for (var i = 0; i < len; i++) {        
        chartYearly.series[0].remove();
    }
    
        if (data.series) {
            for (var i = 0; i < data.series.length; i++) {
                chartYearly.addSeries({
                    name: data.series[i].name,
                    data: data.series[i].data,
                    // color: data.series[i].color || 'white'
                });
            }
        }
     chartYearly.hideLoading();    
}



function chartWorkLengthExpStateWiseChart(chart, containerID) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: "POST",
        url: '/Dashboard/WorkLenghtExpYearWiseStateWiseColumnChart/',
        data: { 'stateCode': $("#MAST_STATE_CODE_PHYSICAL").val(), 'districtCode': $("#MAST_DISTRICT_CODE_PHYSICAL").val(), 'fundingAgency': $("#ddlCollaborationsForPhysical").val(), 'isYearWise': false },
        error: function (xhr, status, error) {
            $.unblockUI();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            var series1 = null; var series2 = null; var series3 = null;

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

                drillSeriesStateData = {
                    series: []
                };

                $.each(data, function (item) {
                    series1.name = "Works";                    
                    series1.data.push({ code: this.LOCATION_CODE, name: this.LOCATION_NAME, y: parseFloat(this.PROPOSALS), drilldown: drillSeriesStateData });

                    series2.name = "Length";                    
                    series2.data.push({ code: this.LOCATION_CODE, name: this.LOCATION_NAME, y: parseFloat(this.LENGTH_COMPLETED), drilldown: drillSeriesStateData });

                    series3.name = "Expenditure";                    
                    series3.data.push({  code: this.LOCATION_CODE, name: this.LOCATION_NAME, y: parseFloat(this.EXPENDITURE), drilldown: drillSeriesStateData });
                });

                StateWiseoptionsColumn = CommonOptionsForWorkLenghtExpStateWiseColumnChart(containerID);

                StateWiseoptionsColumn.series.push(series1);
                StateWiseoptionsColumn.series.push(series2);
                StateWiseoptionsColumn.series.push(series3);
                                

                Newchart = new Highcharts.Chart(StateWiseoptionsColumn);
                // code to display animation
                Newchart.series[0].setVisible(true, true);
                Newchart.series[1].setVisible(true, true);
                Newchart.series[2].setVisible(true, true);

                //Newchart.series[1].setVisible(true, true);
                
            }

            $.unblockUI();
        }
    });
}

//---------------------------- Works Length Exp Year Wise Column Chart Ends Here ---------------------------------//

