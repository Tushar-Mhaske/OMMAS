/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QM2TierPIU.js
        * Description   :   Handles events, grids in PIU Login for SQM data
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/

$(document).ready(function () {

    $("#tabs-2TierDetailsPIU").tabs();
    $('#tabs-2TierDetailsPIU ul').removeClass('ui-widget-header');

    $(function () {
        $("#accordion2TierPIUInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordion2TierPIUSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionATR2TierSqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    Show2TierPIUInspectionDetails();

});

//-------------------------2 Tier PIU -----------------------//

function closeDivError() {
    $('#divError').hide('slow');
}

function Show2TierPIUInspectionDetails() {
    blockPage();
    $("#div2TierPIUInspFilters").load('/QualityMonitoring/QualityFilters', function () {
        $.validator.unobtrusive.parse($('#3TierSQCInspFilterForm'));
        unblockPage();
        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());
    });
    $('#div2TierPIUInspFilters').show('slow');
    unblockPage();
}


function Close2TierPIUInspectionDetails() {
    $('#accordion2TierPIUInspection').hide('slow');
    $('#div2TierPIUInspDetails').hide('slow');
    $("#tb2TierSqcInspList").jqGrid('setGridState', 'visible');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}

function Close2TierPIUScheduleDetails() {
    $('#accordion2TierPIUSchedule').hide('slow');
    $('#div2TierPIUScheduleDetails').hide('slow');
    $("#tb2TierPIUScheduleList").jqGrid('setGridState', 'visible');
}

//Added by deendayal 
function ShowSQCATR() {

    blockPage();
    $("#div2TierATRQualityFiltersHtml").load('/QualityMonitoring/QualityMaintenanceATRFilters', function () {
        $.validator.unobtrusive.parse($('#2TierATRFilterForm'));
        unblockPage();
    });
    $('#div2TierATRQualityFiltersHtml').show('slow');
}

function ShowSQCATRObsDetails(obsId) {

    $("#accordionATR2TierSqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierSQCATRDetailsPIU();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR2TierSqc').show('slow', function () {
        blockPage();
        $("#divATR2TierSqcDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/false', function () {
            unblockPage();
        });
    });

    $("#divATR2TierSqcDetails").css('height', 'auto');
    $('#divATR2TierSqcDetails').show('slow');

    toggleSQCATRDetails();
}

function Close2TierSQCATRDetailsPIU() {
    $('#accordionATR2TierSqc').hide('slow');
    $('#divATR2TierSqcDetails').hide('slow');
    toggleSQCATRDetails();
}

function toggleSQCATRDetails() {
    $("#spn2TierATRHtml").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#div2TierATRDetailsHtml").toggle("slow");
}

function ShowSQCATRGradingDetails(obsId) {

    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);
    $("#accordionInspection div").html("");
    $("#accordionATR2TierSqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierSQCATRDetailsPIU();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR2TierSqc').show('slow', function () {
        blockPage();
        $("#divATR2TierSqcDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/true', function () {
            unblockPage();
        });
    });

    $("#divATR2TierSqcDetails").css('height', 'auto');
    $('#divATR2TierSqcDetails').show('slow');

    toggleSQCATRDetails();
}

function DownloadATR(atrId) {
    window.location = "/QualityMonitoring/DownloadFile/" + atrId;
}

function DownloadMaintenanceATR(atrId) {
    window.location = "/QualityMonitoring/DownloadMaintenanceATRFile/" + atrId;
}

function UploadSQCATR(obsId) {

    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");

    $("#accordionATR2TierSqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload ATR</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierSQCATRDetailsPIU();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR2TierSqc').show('slow', function () {
        blockPage();
        $("#divATR2TierSqcDetails").load('/QualityMonitoring/PdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR2TierSqcDetails").css('height', 'auto');
    $('#divATR2TierSqcDetails').show('slow');

    toggleSQCATRDetails();
}



//FUNCTION TO BE REPLACED IN QM2TierPIU.js
function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear) {
    $("#tb2TierSqcInspList").jqGrid('GridUnload');

    jQuery("#tb2TierSqcInspList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetailsSQCPIU?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                    "Start Chainage (Km.)", "End Chainage (Km.)", "Schedule Date", "Inspection Date", "Upload Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
                    "Uploded by", "View Work Inspection Report", "View Details", "View Multiple PDF"],
        colModel: [
                            { name: 'Monitor', index: 'Monitor', width: 90, sortable: false, align: "left" },
                            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
                            { name: 'District', index: 'District', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Block', index: 'Block', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
                            { name: 'SanctionYear', index: 'SanctionYear', width: 45, sortable: false, align: "center", search: false },
                            { name: 'RoadName', index: 'RoadName', width: 140, sortable: false, align: "left", search: false },
                            { name: 'PropType', index: 'PropType', width: 40, sortable: false, align: "left", search: false },
                            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
                            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
                             { name: 'SCHEDULE_DATE', index: 'SCHEDULE_DATE', width: 60, sortable: false, align: "center", search: false },
                            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
                            { name: 'UploadDate', index: 'UploadDate', width: 60, sortable: false, align: "center", search: false },
                            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
                            { name: 'IsEnquiry', index: 'IsEnquiry', width: 50, sortable: false, align: "center", search: false },
                            { name: 'Scheme', index: 'Scheme', width: 50, sortable: false, align: "center", search: false },
                            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
                            { name: 'NoOfImagesUploaded', index: 'NoOfImagesUploaded', width: 40, sortable: false, align: "center", search: false },
                            { name: 'UploadBy', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
                            { name: 'UploadReport', index: 'UploadReport', width: 25, sortable: false, align: "center", search: false},
                            { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },
                            { name: 'UploadPDF', index: 'UploadPDF', width: 30, sortable: false, align: "center", search: false },
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType": "S" },
        pager: jQuery('#dv2TierSqcInspListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        height: '300',
        autowidth: true,
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            $('#tb2TierSqcInspList').setGridWidth(($('#tabs-2TierDetailsPIU').width() - 30), true);
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function PIU2TierScheduleListGrid() {

    $("#tb2TierPIUScheduleList").jqGrid('GridUnload');

    jQuery("#tb2TierPIUScheduleList").jqGrid({
        url: '/QualityMonitoring/GetPIU2TierScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "Inspection Status", "Add Road", "View"],
        colModel: [
                            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
                            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 200, sortable: false, align: "center", search: false },
                            { name: 'InspStatus', index: 'InspStatus', width: 120, sortable: false, align: "center", search: false },
                            { name: 'AddRoad', index: 'AddRoad', width: 100, sortable: false, align: "center", search: false },
                            { name: 'View', index: 'View', width: 100, sortable: false, align: "center", search: false }
        ],
        pager: jQuery('#dv2TierPIUScheduleListPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List ( 2 Tier )",
        height: 'auto',
        autowidth: true,
        sortname: 'Monitor',
        rowList: [5, 10, 20, 30],
        grouping: true,
        groupingView: {
            groupField: ['Monitor'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            $('#tb2TierPIUScheduleList').setGridWidth(($('#div2TierPIUSchedulePreparation').width() - 10), true);

            if ($('#hdnRoleCodeOnSqcLayout3Tier').val() != 5) {
                $("#tb2TierPIUScheduleList #dv2TierPIUScheduleListPager").css({ height: '35px' });
                $("#dv2TierPIUScheduleListPager_left").html("<input type='button' style='margin-left:1px' id='idPreviousSchedules' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPreviousSchedulesPIU2Tier();return false;' value='Previous Schedules'/>");
            }
            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid

}


function ShowObservationDetails(obsId) {

    jQuery('#tb2TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion2TierPIUInspection div").html("");
    $("#accordion2TierPIUInspection h3").html(
            "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion2TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div2TierPIUInspDetails").load('/QualityMonitoring/QMObservationDetails2TierCQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div2TierPIUInspDetails").css('height', 'auto');
    $('#div2TierPIUInspDetails').show('slow');

    $("#tb2TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id2TierSQCInspFilterDiv').trigger('click');
}



//Displays Individual Monitor Data
function ShowMonitorDataPIU2Tier(monitorId) {

    //alert(monitorId);
    $("#accordion2TierPIUSchedule div").html("");
    $("#accordion2TierPIUSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Monitor Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion2TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div2TierPIUScheduleDetails").load('/QualityMonitoring/MonitorDetails/' + monitorId, function () {
            unblockPage();
        });
    });

    $('#div2TierPIUScheduleDetails').show('slow');
    $("#div2TierPIUScheduleDetails").css('height', 'auto');


    //$("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


//Displays Individual Montor Data
function QMAssignRoadsPIU2Tier(scheduleCode) {

    jQuery('#tb2TierPIUScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion2TierPIUSchedule div").html("");
    $("#accordion2TierPIUSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

            '<a href="#" style="float: right;">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion2TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div2TierPIUScheduleDetails").load('/QualityMonitoring/QMAssignRoads/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#div2TierPIUScheduleDetails').show('slow');
    $("#div2TierPIUScheduleDetails").css('height', 'auto');


    //$("#tb2TierPIUScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewDetailsPIU2Tier(scheduleCode) {
    jQuery('#tb2TierPIUScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion2TierPIUSchedule div").html("");
    $("#accordion2TierPIUSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Districtwise Schedule Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion2TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div2TierPIUScheduleDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#div2TierPIUScheduleDetails").css('height', 'auto');
    $('#div2TierPIUScheduleDetails').show('slow');
}


function ViewPrevSchDetails(scheduleCode) {
    blockPage();

    var number = Math.floor((Math.random() * 99999999) + 1);

    $("#dvPrevSchDistrictwiseDetails").show();
    $("#dvPrevSchDistrictwiseDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
        unblockPage();
    });
}


function ShowPreviousSchedulesPIU2Tier() {
    var random = Math.random();
    $("#accordion2TierPIUSchedule div").html("");
    $("#accordion2TierPIUSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Previous Schedules</a>" +

            '<a href="#" style="float: right;">' +
            '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion2TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div2TierPIUScheduleDetails").load('/QualityMonitoring/QMPreviousSchedules2TierPIU/', function () {
            unblockPage();
        });
    });
    $('#div2TierPIUScheduleDetails').show('slow');
    $("#div2TierPIUScheduleDetails").css('height', 'auto');


    $("#tb2TierPIUScheduleList").jqGrid('setGridState', 'hidden');
}



//------------------------- 2 Tier PIU Ends Here----------------------------//




function ShowInspReportFile(obsId) {

    jQuery('#tb2TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion2TierPIUInspection div").html("");
    $("#accordion2TierPIUInspection h3").html(
            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion2TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div2TierPIUInspDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div2TierPIUInspDetails").css('height', 'auto');
    $('#div2TierPIUInspDetails').show('slow');

    $("#tb2TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id2TierSQCInspFilterDiv').trigger('click');
}

//added by abhinav pathak
function ShowInspReportFilePDF(obsId, prRoadCode) {

    jQuery('#tb2TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion2TierPIUInspection div").html("");
    $("#accordion2TierPIUInspection h3").html(
            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierPIUInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion2TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div2TierPIUInspDetails").load('/QualityMonitoring/PdfFileUploadView/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div2TierPIUInspDetails").css('height', 'auto');
    $('#div2TierPIUInspDetails').show('slow');

    $("#tb2TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id2TierSQCInspFilterDiv').trigger('click');
}