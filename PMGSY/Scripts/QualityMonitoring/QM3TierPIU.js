
/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QM3TierPIU.js
        * Description   :   Handles events, grids in PIU Login for NQM data
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/


$(document).ready(function () {

    $("#tabs-3TierDetailsPIU").tabs();
    $('#tabs-3TierDetailsPIU ul').removeClass('ui-widget-header');

    $(function () {
        $("#accordion3TierPIUInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordion3TierPIUSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordion3TierATRDetailsPIU").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#spn3TierATRHtml").click(function () {
        toggleATRDetails();
    });

    /*Changes for CQCAdmin*/
    $(function () {
        $("#accordionSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionTeamSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $(function () {
        $("#accordionATR3TierCqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionSQCLetter").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionBulkATRDetails").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    /*Changes for CQCAdmin Ends*/
    $(function () {
        $("#accordionATR2TierSqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    Show3TierPIUInspectionDetails();
});





//------------------------------- 3 Tier PIU Starts here-----------------//


function closeDivError() {
    $('#divError').hide('slow');
}

function Show3TierPIUInspectionDetails() {
    blockPage();
    $("#div3TierPIUInspFilters").load('/QualityMonitoring/Quality3TierSQCInspFilters', function () {
        $.validator.unobtrusive.parse($('#3TierSQCInspFilterForm'));
        unblockPage();
    });
    $('#div3TierPIUInspFilters').show('slow');
    unblockPage();
}

function Show3TierPIUScheduledetails() {
    blockPage();
    $("#div3TierPIUInspFilters").load('/QualityMonitoring/Quality3TierSQCInspFilters', function () {
        $.validator.unobtrusive.parse($('#3TierSQCInspFilterForm'));
        unblockPage();
    });
    $('#div3TierPIUInspFilters').show('slow');
    unblockPage();
}

//Newly added for HTMl Report on 17/07/2014
function Show3TierPIUATR() {
    blockPage();
    $("#div3TierATRQualityFiltersHtml").load('/QualityMonitoring/QualityATRFilters', function () {
        $.validator.unobtrusive.parse($('#3TierFilterForm'));
        unblockPage();
    });
    $('#div3TierATRQualityFiltersHtml').show('slow');
}


function toggleATRDetails() {
    $("#spn3TierATRHtml").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#div3TierATRDetailsHtml").toggle("slow");
}


function Close3TierPIUScheduleDetails() {
    $('#accordion3TierPIUSchedule').hide('slow');
    $('#div3TierPIUScheduleDetails').hide('slow');
    $("#tb3TierPIUScheduleList").jqGrid('setGridState', 'visible');
}


function Close3TierATRDetailsPIU() {
    $('#accordion3TierATRDetailsPIU').hide('slow');
    $('#div3TierATRDetailsPIU').hide('slow');
    toggleATRDetails();
}

function Close3TierPIUInspectionDetails() {
    $('#accordion3TierPIUInspection').hide('slow');
    $('#div3TierPIUInspDetails').hide('slow');
    $("#tb3TierSqcInspList").jqGrid('setGridState', 'visible');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear) {
  
    $("#tb3TierSqcInspList").jqGrid('GridUnload');

    jQuery("#tb3TierSqcInspList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetailsSQCPIU?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
            "Start Chainage (Km.)", "End Chainage (Km.)", "Schedule Date", "Inspection Date", "Upload Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
            "Uploded by", "Upload / View Report", "View Details", "Upload / View PDF"],
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
            { name: 'UploadReport', index: 'UploadReport', width: 25, sortable: false, align: "center", search: false },
            { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },
            { name: 'UploadPDF', index: 'UploadPDF', width: 30, sortable: false, align: "center", search: false },
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType": "I", "roadOrBridge": "0", "gradeType": "0", "eFormStatus": "0" },
        pager: jQuery('#dv3TierSqcInspListPager'),
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
            $('#tb3TierSqcInspList').setGridWidth(($('#tabs-3TierDetailsPIU').width() - 50), true);
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


//Add InspectionListGridCQCAdmin for CQCAdmin on 28-06/2022 by vikky
//Changes Make By Hrishikesh To Add 'View Test Report' Coloumn to CQCAdmin Login 
function InspectionListGridCQCAdmin(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatusType) {

    $("#tb3TierSqcInspList").jqGrid('GridUnload');

    jQuery("#tb3TierSqcInspList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetailsSQCPIU?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Road/Bridge Name", "Package No", "Road/Bridge", "Scheme", "Work Status (As inspected)", "Sanctioned Length(km) / Bridge Length(m)", "From(km)", "To(km)", "Inspection Date",
            "Overall Grade", "Normal/ Enquiry Inspection", "View abstract/ Images", "View Report (pdf)", "View Test Report (pdf)", /*"Images Uploaded",*/ "E-form Status", "View E-form", /*"View E-report",*/ "View E-Test Report", "View E-Test Report", "View Combined E-Report"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 80, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 160, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 30, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
            { name: 'SancLength', index: 'SancLength', width: 50, sortable: false, align: "center", search: false },
            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
            { name: 'InspectionType', index: 'InspectionType', width: 100, sortable: false, align: "center", search: false },
            { name: 'viewImages', index: 'UploadReport', width: 50, sortable: false, align: "center", search: false },

            { name: 'viewreport', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
            //{
            //    name: 'Images uploaded', index: 'View', width: 30, sortable: false, align: "center", search: false, hidden:true
            //},
            { name: 'TRScanPDF', index: 'TRScanPDF', width: 30, sortable: false, align: "center", search: false },
            { name: 'eformStatus', index: 'eformStatus', width: 60, sortable: false, align: "center", search: false },
            { name: 'vieweformpdf', index: 'vieweformpdf', width: 30, sortable: false, align: "center", search: false },
            // { name: 'vieweformdetails', index: 'vieweformdetails', width: 30, sortable: false, align: "center", search: false },
            { name: 'viewetrdetails', index: 'viewtrdetails', width: 30, sortable: false, align: "center", search: false, hidden: true },
            { name: 'viewetrpreviewdetails', index: 'viewetrpreviewdetails', width: 30, sortable: false, align: "center", search: false },
            { name: 'viewCombinedreport', index: 'viewCombinedreport', width: 30, sortable: false, align: "center", search: false },
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType": "I", "schemeType": schemeType, "roadStatus": roadStatus, "roadOrBridge": roadOrBridge, "gradeType": gradeType, "eFormStatus": eFormStatusType },
        pager: jQuery('#dv3TierSqcInspListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        height: '300',
        //autowidth: true,
        autoResizing: { adjustGridWidth: false },  //
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            $('#tb3TierSqcInspList').setGridWidth(($('#tabs-3TierDetailsPIU').width() - 50), true);
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
    jQuery("#tb3TierSqcInspList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: '<center>Inspection Chainage</center>' },
            { startColumnName: 'viewreport', numberOfColumns: 2, titleText: '<center>Report</center>' },
            { startColumnName: 'eformStatus', numberOfColumns: 5, titleText: '<center>E-form</center>' }
            //  { startColumnName: 'viewetrdetails', numberOfColumns: 2, titleText: '<center>E-form (Test report)</center>' }
        ]
    });

}



function viewCombinePdfData(eid) {
    window.open("/EFORM/ViewCombinePdfSavedData?eid=" + eid);
}


function viewCombinePdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewCombinePdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function viewCombinedPart_1_2_Pdf(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/isPart1Part2PdfAvail/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open("/EFORM/viewCombinedPdf12/" + id);
            }
            else {
                alert(response.Message);
            }


        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function viewTRPdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewTRPdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function previewTestReport(encIdtemp) {
    window.open("/EFORM/TestReportPreview?encIdtemp=" + encIdtemp);
}

function PIU3TierScheduleListGrid() {

    $("#tb3TierPIUScheduleList").jqGrid('GridUnload');

    jQuery("#tb3TierPIUScheduleList").jqGrid({
        url: '/QualityMonitoring/GetPIU3TierScheduleList?' + Math.random(),
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
        pager: jQuery('#dv3TierPIUScheduleListPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List ( 3 Tier )",
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

            $('#tb3TierPIUScheduleList').setGridWidth(($('#div3TierPIUSchedulePreparation').width() - 10), true);

            $("#tb3TierPIUScheduleList #dv3TierPIUScheduleListPager").css({ height: '35px' });
            $("#dv3TierPIUScheduleListPager_left").html("<input type='button' style='margin-left:1px' id='idPreviousSchedules' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPreviousSchedulesSqc3Tier();return false;' value='Previous Schedules'/>");
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

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/QMObservationDetails3TierSQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}

//---------------Schedule Details Starts Here --------------------//


//Displays Individual Monitor Data
function ShowMonitorData(monitorId) {

    //alert(monitorId);
    $("#accordion3TierPIUSchedule div").html("");
    $("#accordion3TierPIUSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Monitor Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div3TierPIUScheduleDetails").load('/QualityMonitoring/MonitorDetails/' + monitorId, function () {
            unblockPage();
        });
    });

    $('#div3TierPIUScheduleDetails').show('slow');
    $("#div3TierPIUScheduleDetails").css('height', 'auto');


    //$("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


//Displays Individual Montor Data
function QMAssignRoadsPIU(scheduleCode) {

    jQuery('#tb3TierPIUScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion3TierPIUSchedule div").html("");
    $("#accordion3TierPIUSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div3TierPIUScheduleDetails").load('/QualityMonitoring/QMAssignRoads/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#div3TierPIUScheduleDetails').show('slow');
    $("#div3TierPIUScheduleDetails").css('height', 'auto');


    //$("#tb3TierPIUScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewDetailsPIU3Tier(scheduleCode) {
    jQuery('#tb3TierPIUScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion3TierPIUSchedule div").html("");
    $("#accordion3TierPIUSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Districtwise Schedule Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion3TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div3TierPIUScheduleDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUScheduleDetails").css('height', 'auto');
    $('#div3TierPIUScheduleDetails').show('slow');
}


function ViewPrevSchDetails(scheduleCode) {
    blockPage();

    var number = Math.floor((Math.random() * 99999999) + 1);

    $("#dvPrevSchDistrictwiseDetails").show();
    $("#dvPrevSchDistrictwiseDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
        unblockPage();
    });
}


function ShowPreviousSchedulesSqc3Tier() {
    var random = Math.random();
    $("#accordion3TierPIUSchedule div").html("");
    $("#accordion3TierPIUSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Previous Schedules</a>" +

        '<a href="#" style="float: right;">' +
        '<img src="" style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierPIUSchedule').show('slow', function () {
        blockPage();
        $("#div3TierPIUScheduleDetails").load('/QualityMonitoring/QMPreviousSchedules3TierSQC/', function () {
            unblockPage();
        });
    });
    $('#div3TierPIUScheduleDetails').show('slow');
    $("#div3TierPIUScheduleDetails").css('height', 'auto');


    $("#tb3TierPIUScheduleList").jqGrid('setGridState', 'hidden');
}




//---------------Schedule Details Ends Here ----------------------//



//------ ATR Details ---------------------------------------//

function DownloadATR(atrId) {
    window.location = "/QualityMonitoring/DownloadFile/" + atrId;
}

//Observation Details for QM Obs Entry (Original Grading)
function ShowATRObsDetails(obsId) {

    $("#accordion3TierATRDetailsPIU div").html("");
    $("#accordion3TierATRDetailsPIU h3").html(
        "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierATRDetailsPIU();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierATRDetailsPIU').show('slow', function () {
        blockPage();
        $("#div3TierATRDetailsPIU").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/false', function () {
            unblockPage();
        });
    });

    $("#div3TierATRDetailsPIU").css('height', 'auto');
    $('#div3TierATRDetailsPIU').show('slow');

    toggleATRDetails();
}


//Observation Details for (Corrected Grading)
function ShowATRGradingDetails(obsId) {

    $("#accordion3TierATRDetailsPIU div").html("");
    $("#accordion3TierATRDetailsPIU h3").html(
        "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierATRDetailsPIU();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion3TierATRDetailsPIU').show('slow', function () {
        blockPage();
        $("#div3TierATRDetailsPIU").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/true', function () {
            unblockPage();
        });
    });

    $("#div3TierATRDetailsPIU").css('height', 'auto');
    $('#div3TierATRDetailsPIU').show('slow');

    toggleATRDetails();
}
//------ ATR Details Ends Here ---------------------------------------//


function ShowInspReportFile(obsId) {

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);
    //alert(obsId);
    $('#accordion3TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


/*Addiotional Functionality for CQC in PIU code*/
function ShowMonitorsDetails() {
    //alert("Test Monitor");
    $("#accordionInspection div").html("");
    //CloseInspectionDetails();

    $("#accordionATR3TierCqc div").html("");
    //CloseATR3TierCqcDetails();

    blockPage();
    $("#div3TierAddMonitor").load('/Master/MasterQualityMonitor/', function () {

        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_ADDRESS1');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PIN');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PHONE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_MOBILE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_REMARKS');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PAN');

        unblockPage();
    });
    $('#div3TierAddMonitor').show('slow');

    $('#tblQualityMonitorList').setGridParam({ rowNum: 10 });


}

function MaintenanceInspection() {
    blockPage();
    $("#div3TierAddMaintenanceInspection").load('/QualityMonitoring/MaintenanceInspection', function () {
        $.validator.unobtrusive.parse($('#frmMaintenanceInspection'));
        unblockPage();
    });
}
//--------------------------------------------------------------------------------------------------------

function LoadSQCDetails() {
    blockPage();
    $("#div3TierSQCDetails").load("/Master/ListAdminSqc/", function () {
        unblockPage();
    });
}


function LoadTourDetails() {
    blockPage();
    $("#div3TierTourDetails").load("/QualityMonitoring/QualityTourFilters/", function () {
        unblockPage();
    });
}

///Changes for CQC
//Schedule List for CQC
function ScheduleListGrid(month, year) {
    //  alert("SA 1")

    $("#tb3TierScheduleList").jqGrid('GridUnload');

    jQuery("#tb3TierScheduleList").jqGrid({
        url: '/QualityMonitoring/GetScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "State", "District 1", "District 2", "District 3",
            "Inspection Status", "Add District", "Add Work", "Add Contractors", "View and Delete works", "Delete Schedules", "Finalize District", "View Letter", "Forward To Monitor", "Unlock", "Tour Claim"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 150, sortable: false, align: "center", search: false },
            { name: 'State', index: 'State', width: 110, sortable: false, align: "left", search: false },
            { name: 'District1', index: 'District1', width: 90, sortable: false, align: "left", search: false },
            { name: 'District2', index: 'District2', width: 90, sortable: false, align: "left", search: false },
            { name: 'District3', index: 'District3', width: 90, sortable: false, align: "left", search: false },
            { name: 'InspStatus', index: 'InspStatus', width: 80, sortable: false, align: "center", search: false },
            { name: 'AddDistrict', index: 'AddDistrict', width: 80, sortable: false, align: "center", search: false },
            { name: 'AddRoad', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false },
            { name: 'AddContractors', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false },
            { name: 'View', index: 'View', width: 70, sortable: false, align: "center", search: false },
            { name: 'Delete', index: 'Delete', width: 70, sortable: false, align: "center", search: false },
            { name: 'Finalize', index: 'Finalize', width: 80, sortable: false, align: "center", search: false, hidden: true },
            { name: 'ViewLetter', index: 'ViewLetter', width: 40, sortable: false, align: "center", search: false },
            { name: 'Forward', index: 'Forward', width: 80, sortable: false, align: "center", search: false },
            { name: 'Unlock', index: 'Unlock', width: 80, sortable: false, align: "center", search: false, hidden: true },
            { name: 'Tour', index: 'Tour', width: 80, sortable: false, align: "center", search: false }
        ],
        postData: { month: month, year: year },
        pager: jQuery('#dv3TierScheduleListPager'),
        rowNum: 10000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: '300',
        autowidth: true,
        sortname: 'Monitor',
        grouping: true,
        groupingView: {
            groupField: ['Monitor'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            //$("#gview_tb3TierScheduleList > .ui-jqgrid-titlebar").hide();
            //alert($("#hdnRoleCodeOnLayout").val());




            $("#tb3TierScheduleList #dv3TierScheduleListPager").css({ height: '35px' });

            if (($("#hdnRoleCodeOnSqcLayout3Tier").val() == 8)) //for SQC hide these two columns
            {
                $('#tb3TierScheduleList').jqGrid('hideCol', 'District2');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'District3');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'AddDistrict');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'ViewLetter');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'Unlock');

                $("#dv3TierScheduleListPager_left").html("<input type='button' style='margin-left:1px' id='idPreviousSchedules' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPreviousSchedules();return false;' value='Previous Schedules'/>" +
                    "<input type='button' style='margin-left:5px' id='idCreateSchedule' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowCreateSchedule();return false;' value='Create New'/>");
            }
            else {
                $("#dv3TierScheduleListPager_left").html("<input type='button' style='margin-left:1px' id='idPreviousSchedules' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPreviousSchedules();return false;' value='Previous Schedules'/>" +
                    "<input type='button' style='margin-left:5px' id='idCreateSchedule' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowCreateSchedule();return false;' value='Create New'/>" +
                    "<input type='button' style='margin-left:5px' id='btnViewSQCLetter' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowQMLetter();return false;' value='Generate Letter'/>" +

                    "<input type='button' style='margin-left:5px' id='btnCreateTeam' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowCreateTeam();return false;' value='Team Inspections'/>" +
                    "<input type='button' style='margin-left:5px' id='btnCreateTeam' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPriorityList();return false;' value='Shedule Priority List'/>");
            }

            $('#tb3TierScheduleList').setGridWidth(($('#div3TierSchedulePreparation').width() - 10), true);

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

/// -------------------- Grid Column Click Functions -----------------------------------
function ShowCreateSchedule() {
    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Create Schedule</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/QMCreateSchedule', function () {
            $.validator.unobtrusive.parse($('#frmCreateSchedule'));
            unblockPage();
        });
    });

    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');

    //CloseSQCLetter();
    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}
// Added on 12 May 2016 
function ShowPriorityList() {
    $("#accordionPriorityList div").html("");
    $("#accordionPriorityList h3").html(
        "<a href='#' style= 'font-size:.9em;' >Schedule Priority List</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePriorityList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionPriorityList').show('slow', function () {
        blockPage();
        $("#divPriorityList").load('/QMSSRSReports/QMSSRSReports/SchedulePriorityLayout/', function () {
            unblockPage();
        });
    });

    $('#divPriorityList').show('slow');
    $("#divPriorityList").css('height', 'auto');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}



//

function ShowCreateTeam() {
    $("#accordionTeamSchedule div").html("");
    $("#accordionTeamSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Team Inspection</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseTeamDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionTeamSchedule').show('slow', function () {
        blockPage();
        $("#divTeamScheduleDetails").load('/QMSSRSReports/QMSSRSReports/QualityTeam/', function () {
            unblockPage();
        });
    });

    $('#divTeamScheduleDetails').show('slow');
    $("#divTeamScheduleDetails").css('height', 'auto');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

function ShowPreviousSchedules() {
    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Previous Schedules</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/QMPreviousSchedules/', function () {
            unblockPage();
        });
    });
    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');


    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

//Displays Individual Monitor Data
function ShowMonitorData(monitorId) {

    //alert(monitorId);
    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Monitor Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/MonitorDetails/' + monitorId, function () {
            unblockPage();
        });
    });

    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');


    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


//Displays Individual Montor Data
function CQCAddDistrict(scheduleCode) {

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Add Districts</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/CQCAddDistrict/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


//Displays Individual Montor Data
function DeleteDistrict(scheduleCode) {

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    //if ($("#hdnRoleCodeOnLayout").val() == 5) //for CQC Three Districts
    if ($("#hdnRoleCodeOnSqcLayout3Tier").val() == 9 || $("#hdnRoleCodeOnSqcLayout3Tier").val() == 5) {
        $("#accordionSchedule div").html("");
        $("#accordionSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Delete Districts</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
        );

        $('#accordionSchedule').show('slow', function () {
            blockPage();
            $("#divScheduleDetails").load('/QualityMonitoring/QMDeleteDistrict/' + scheduleCode, function () {
                unblockPage();
            });
        });

        $('#divScheduleDetails').show('slow');
        $("#divScheduleDetails").css('height', 'auto');

    }
}


//Displays Individual Montor Data
function QMAssignRoads(scheduleCode) {

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/QMAssignRoads/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewDetails(scheduleCode) {
    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Districtwise Schedule Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#divScheduleDetails").css('height', 'auto');
    $('#divScheduleDetails').show('slow');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewPrevSchDetails(scheduleCode) {
    //jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);
    blockPage();

    var number = Math.floor((Math.random() * 99999999) + 1);

    $("#dvPrevSchDistrictwiseDetails").show();
    $("#dvPrevSchDistrictwiseDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
        unblockPage();
    });
}


function FinalizeSchedule(scheduleCode, qmType) {
    if (confirm("Before finalization, please confirm the districts assigned in schedule. \nAfter finalization, you can not modify it. If you are sure, click Ok to finalize.")) {
        $.ajax({
            url: '/QualityMonitoring/FinalizeDistricts/',
            type: 'POST',
            data: { adminSchCode: scheduleCode, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Schedule finalized successfully");

                    //Generate letter only for NQM
                    if (qmType === "I") {
                        QMGenerateNQMLetter(scheduleCode);
                    }
                    $("#tb3TierScheduleList").trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}


function ForwardSchedule(scheduleCode) {

    if (confirm("Please confirm the schedule. Once schedule is forwarded, \nit can't be modified by CQC or SQC. All roads in the schedule \nwill be available to the monitor for inspection.\nAre You Sure?")) {
        $.ajax({
            url: '/QualityMonitoring/ForwardSchedule/',
            type: 'POST',
            data: { adminSchCode: scheduleCode, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Schedule forwarded to monitor successfully");
                    $("#tb3TierScheduleList").trigger("reloadGrid");
                    CloseScheduleDetails();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}

/*  Generate NQM Letter Code Starts Here  */

function QMGenerateNQMLetter(scheduleCode) {
    blockPage();

    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/AddLetterDetails',
        async: false,
        data: { scheduleCode: scheduleCode, userType: 'I' },
        beforeSend: function () {
            blockPage();
        },
        success: function (data) {
            if (data.success) {
                alert('NQM Letter Generated Successfully.');
                $("#tbSQCLetterList").trigger('reloadGrid');
                QMSendMailToNQM(scheduleCode);
                QMOpenGeneratedNQMLetter(scheduleCode);
                unblockPage();
            }
            else {
                if (data.status == "T") {
                    alert(data.message);
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    $.validator.unobtrusive.parse($('#mainDiv'));
                }
                unblockPage();
            }
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            unblockPage();
        }
    })
}


function QMSendMailToNQM(scheduleCode) {
    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/SendLetter',
        async: false,
        data: { scheduleCode: scheduleCode, userType: 'I' },
        beforeSend: function () {
            blockPage();
        },
        success: function (data) {
            if (data.Success) {
                alert(data.Message);
                unblockPage();
            }
            else {
                $("#divError").show("slow");
                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.Message);
                $.validator.unobtrusive.parse($('#mainDiv'));
                unblockPage();
            }
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            unblockPage();
        }
    })
}


function UnlockSchedule(scheduleCode) {

    if (confirm("Once schedule is unlocked, it can't be downloaded by monitor.\nAlso schedule will be unavailable for SQC to add works.\nAre you sure to Unlock?")) {
        $.ajax({
            url: '/QualityMonitoring/UnlockSchedule/',
            type: 'POST',
            data: { adminSchCode: scheduleCode, value: Math.random() },

            success: function (result) {
                if (result) {
                    alert("Schedule unlocked successfully");
                    $("#tb3TierScheduleList").trigger("reloadGrid");
                    CloseScheduleDetails();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }

        });
    }
    else {
        return false;
    }
}



function QMOpenGeneratedNQMLetter(scheduleCode) {

    window.open('/QualityMonitoring/DownloadLetter?' + $.param({ id: scheduleCode, isLettterId: false, userType: 'I' }), '_blank');

}

/*  Generate NQM Letter Code Ends Here  */



function UploadQMFile(obsId, scheduleCode, prRoadCode) {

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionATR3TierCqc div").html("");
    CloseATR3TierCqcDetails();
    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionInspection').show('slow', function () {
        blockPage();
        $("#divInspectionDetails").load('/QualityMonitoring/ImageUpload/' + scheduleCode + "$" + prRoadCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}


function ShowCorrectionDetails(obsId) {

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Grade Correction</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionInspection').show('slow', function () {
        blockPage();
        $("#divInspectionDetails").load('/QualityMonitoring/QMGradingCorrection/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}


function DeleteInspection(obsId) {
    if (confirm("Are you sure to delete the observation?")) {
        $.ajax({
            url: '/QualityMonitoring/QMDeleteObservation/',
            type: 'POST',
            data: { obsId: obsId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Observations deleted successfully");
                    $("#tb3TierInspectionList").trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}


//function ShowObservationDetails(obsId) {

//    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

//    $("#accordionInspection div").html("");
//    $("#accordionInspection h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
//            '<span style="float: right;"></span>'
//            );

//    $('#accordionInspection').show('slow', function () {
//        //blockPage();
//        $("#div3TierPIUInspDetails").load('/QualityMonitoring/QMObservationDetails/' + obsId, function () {
//            //unblockPage();
//        });
//    });

//    $("#div3TierPIUInspDetails").css('height', 'auto');
//    $('#div3TierPIUInspDetails').show('slow');

//    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
//    $('#id3TierFilterDiv').trigger('click');
//}


function ShowQMLetter() {
    $("#accordionSQCLetter div").html("");
    $("#accordionSQCLetter h3").html(
        "<a href='#' style= 'font-size:.9em;'>SQC Letter</a>" +
        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseSQCLetter();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSQCLetter').show('slow', function () {
        blockPage();
        $("#divSQCLetter").load('/QualityMonitoring/QMSQCLetter', function () {
            $.validator.unobtrusive.parse($('#frmSQCLetter'));
            unblockPage();
        });
    });

    $('#divSQCLetter').show('slow');
    $("#divSQCLetter").css('height', 'auto');


    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

function DownloadATR(atrId) {
    window.location = "/QualityMonitoring/DownloadFile/" + atrId;
}


function CloseScheduleDetails() {
    $('#accordionSchedule').hide('slow');
    $('#divScheduleDetails').hide('slow');
    $("#tb3TierScheduleList").jqGrid('setGridState', 'visible');
}

function CloseTeamDetails() {
    $('#accordionTeamSchedule').hide('slow');
    $('#divTeamScheduleDetails').hide('slow');
    $("#tb3TierScheduleList").jqGrid('setGridState', 'visible');
}


function ClosePriorityList() {
    $('#accordionPriorityList').hide('slow');
    $('#divPriorityList').hide('slow');
    $("#tb3TierScheduleList").jqGrid('setGridState', 'visible');
}

function CloseSQCLetter() {
    $('#accordionSQCLetter').hide('slow');
    $('#divSQCLetter').hide('slow');
    $("#tb3TierScheduleList").jqGrid('setGridState', 'visible');
}


function CloseInspectionDetails() {
    $('#accordionInspection').hide('slow');
    $('#div3TierPIUInspDetails').hide('slow');
    $("#tb3TierInspectionList").jqGrid('setGridState', 'visible');

    showFilter();
}

function CloseBulkATRDetails() {
    $('#accordionBulkATRDetails').hide('slow');
    $('#divBulkATRDetails').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}

function CloseATR3TierCqcDetails() {
    $('#accordionATR3TierCqc').hide('slow');
    $('#divATR3TierCqcDetails').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}

function closeDivError() {
    $('#divError').hide('slow');
}
//Added by deendayal 
function ShowSQCATR() {

    $('#tabs-3TierDetails-4').hide();
    CloseScheduleDetails();
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
function DownloadMaintenanceATR(atrId) {
    window.location = "/QualityMonitoring/DownloadMaintenanceATRFile/" + atrId;
}

function UploadSQCATR(obsId) {

    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    CloseInspectionDetails();

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
function RegradeSQCATR(obsId) {

    // jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionATR2TierSqc h3").html(
        "<a href='#' style= 'font-size:.9em;' >Regrade ATR</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close2TierSQCATRDetailsPIU();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSqc').show('slow', function () {
        blockPage();
        $("#divATR2TierSqcDetails").load('/QualityMonitoring/QMATRRegrade/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR2TierSqcDetails").css('height', 'auto');
    $('#divATR2TierSqcDetails').show('slow');

    toggleSQCATRDetails();
}
function DeleteSQCATR(obsId, atrId) {
    if (confirm("Are you sure to delete the ATR details?")) {
        $.ajax({
            url: '/QualityMonitoring/QMDeleteATRDetails/',
            type: 'POST',
            data: { obsId: obsId, atrId: atrId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("ATR Details deleted successfully");
                    viewMaintenanceATRDetails();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
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

//Displays Contractors which are not inspected even once
function QMAssignContractors(scheduleCode, sanctionYear) {

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/QualityMonitoring/QMAssignContractors/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divScheduleDetails').show('slow');
    $("#divScheduleDetails").css('height', 'auto');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

//Added by deendayal  for Lab details 
function ShowLabDetails() {

    var id = arguments[0];


    $("#accordionATR3TierSqcLab div").html("");
    $("#accordionATR3TierSqcLab h3").html(
        "<a href='#' style= 'font-size:1.2em;' >Lab Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcLabDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR3TierSqcLab').show('slow', function () {
        blockPage();
        $("#divATR3TierSqcLabDetails").load('/QualityMonitoring/LabImageUpload/' + id, function () {
            unblockPage();
            $("#dvFiles #gbox_tbFilesList").css("margin-left", "20%")
        });
    });

    $("#divATR3TierSqcLabDetails").css('height', 'auto');
    $('#divATR3TierSqcLabDetails').show('slow');
    $("#dvFiles #gbox_tbFilesList").css("margin-left", "20%")
    toggleATRDetails();

}

function CloseATR3TierCqcLabDetails() {
    $('#accordionATR3TierSqcLab').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}

//added by abhinav pathak
function ShowInspReportFilePDF(obsId, prRoadCode) {

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierPIUInspection div").html("");
    $("#accordion3TierPIUInspection h3").html(
        "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierPIUInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);
    //alert(obsId);
    $('#accordion3TierPIUInspection').show('slow', function () {
        blockPage();
        $("#div3TierPIUInspDetails").load('/QualityMonitoring/PdfFileUploadView/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierPIUInspDetails").css('height', 'auto');
    $('#div3TierPIUInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}

//------------------ Tour Claim ----------------------

function showTourClaimNotification() {

    var currentDate = new Date();

    // Get the current month (0-11, where 0 represents January)
    var currentMonth = currentDate.getMonth() + 1;

    // Get the current year
    var currentYear = currentDate.getFullYear();

    blockPage();
    $("#divNotificationCQC").load("/TourClaim/GetNotificationCQC/?currMonthYear=" + currentMonth + "$" + currentYear, function () {
        unblockPage();
    });

}