/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayoutSQC.js
        * Description   :   Handles events for in Quality Layout for SQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/


$(document).ready(function () {
    
    $.ajaxSetup({ cache: false });
    
    $("#tabs-3TierDetailsSqc").tabs();
    $('#tabs-3TierDetailsSqc ul').removeClass('ui-widget-header');

    $(function () {
        $("#accordion3TierSqcInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $(function () {
        $("#accordion3TierSqcSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionATR3TierSqc").accordion({
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


    $("#btn3Tier").click(function () {
        window.location = "/QualityMonitoring/QualityLayout";
    });

    $("#btn2Tier").click(function () {
        window.location = "/QualityMonitoring/QualityLayoutSQC";
    });

    //New change added by deepak 16-Sept- 2014
    $("#btn1TierPIU").click(function () {
        blockPage();
        $("#tabs-3TierDetailsSqc").html('');
        $("#tabs-3TierDetailsSqc").load('/QualityMonitoring/QM1TierPIU/', function () {
            //unblockPage();
        });
        $('#tabs-3TierDetailsSqc').show('slow');
        unblockPage();
    });
    //End Change added by deepak 16-Sept- 2014

    $("#spn3TierATRHtml").click(function () {
        toggleATRDetails();
    });

    Show3TierSqcInspectionDetails();

});


function closeDivError() {
    $('#divError').hide('slow');
}

function Close3TierCqcScheduleDetails() {
    $('#accordion3TierCqcSchedule').hide('slow');
    $('#div3TierCqcScheduleDetails').hide('slow');
    $("#tb3TierCqcScheduleList").jqGrid('setGridState', 'visible');
}


function Close3TierSqcScheduleDetails()
{
    $('#accordion3TierSqcSchedule').hide('slow');
    $('#div3TierSqcScheduleDetails').hide('slow');
    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'visible');
}

function CloseATR3TierSqcDetails() {
    $('#accordionATR3TierCqc').hide('slow');
    $('#divATR3TierCqcDetails').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}


function Close3TierSqcInspectionDetails() {
    $('#accordion3TierSqcInspection').hide('slow');
    $('#div3TierSqcInspDetails').hide('slow');
    $("#tb3TierSqcInspList").jqGrid('setGridState', 'visible');
}


function Show3TierSqcScheduleListGrid(month, year)
{
    Sqc3TierScheduleListGrid(month, year);
}


//Newly added for HTMl Report on 17/07/2014
function ShowATR() {

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

function Show3TierSqcInspectionDetails() {
    blockPage();
    $("#div3TierSQCInspFilters").load('/QualityMonitoring/Quality3TierSQCInspFilters', function () {
        $.validator.unobtrusive.parse($('#3TierSQCInspFilterForm'));
        unblockPage();
    });
    $('#div3TierSQCInspFilters').show('slow');

    //Call To Grid
}


function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear) {

    $("#tb3TierSqcInspList").jqGrid('GridUnload');

    jQuery("#tb3TierSqcInspList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetailsSQCPIU?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                    "Start Chainage (Km.)", "End Chainage (Km.)", "Schedule Date", "Inspection Date", "Upload Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
                    "Uploded by", "Upload / View Report", "View Details", "Upload / View PDF", ],
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
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "qmType" : "I" },
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
            $('#tb3TierSqcInspList').setGridWidth(($('#div3TierSqcInspectionDetails').width() - 10), true);
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


function Sqc3TierScheduleListGrid(month, year) {

    $("#tb3TierSqcScheduleList").jqGrid('GridUnload');

    jQuery("#tb3TierSqcScheduleList").jqGrid({
        url: '/QualityMonitoring/GetSqc3TierScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "District 1", "District 2", "District 3",
                    "Inspection Status", "Add Work", "View", "Forward To Monitor"],
        colModel: [
                            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
                            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 200, sortable: false, align: "center", search: false },
                            { name: 'District1', index: 'District1', width: 120, sortable: false, align: "left", search: false },
                            { name: 'District2', index: 'District2', width: 120, sortable: false, align: "left", search: false },
                            { name: 'District3', index: 'District3', width: 120, sortable: false, align: "left", search: false },
                            { name: 'InspStatus', index: 'InspStatus', width: 120, sortable: false, align: "center", search: false },
                            { name: 'AddRoad', index: 'AddRoad', width: 100, sortable: false, align: "center", search: false },
                            { name: 'View', index: 'View', width: 100, sortable: false, align: "center", search: false },
                            { name: 'Forward', index: 'Forward', width: 100, sortable: false, align: "center", search: false }
        ],
        postData: { month: month, year: year },
        pager: jQuery('#dv3TierSqcScheduleListPager'),
        rowNum: 10000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: '300',
        autowidth: true,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        sortname: 'Monitor',
        //rowList: [5, 10, 15],
        grouping: true,
        groupingView: {
            groupField: ['Monitor'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {

            $('#tb3TierSqcScheduleList').setGridWidth(($('#div3TierSqcSchedulePreparation').width() - 10), true);

            $("#tb3TierSqcScheduleList #dv3TierSqcScheduleListPager").css({ height: '35px' });
            $("#dv3TierSqcScheduleListPager_left").html("<input type='button' style='margin-left:1px' id='idPreviousSchedules' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowPreviousSchedulesSqc3Tier();return false;' value='Previous Schedules'/>");
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


function ATRListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, atrStatus, rdStatusATR) {

    $("#tb3TierATRList").jqGrid('GridUnload');

    jQuery("#tb3TierATRList").jqGrid({
        url: '/QualityMonitoring/QMViewATRDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                    "Start Chainage (Km.)", "End Chainage (Km.)", "Inspection Date", "Road Status", "Enquiry Inspection", "Overall Grade", "Observation Details",
                    "Submitted", "Upload / View", "Date of Upload", "Acceptance", "Remarks", "Date", "Regrade", "Delete"],
        colModel: [
                        { name: 'Monitor', index: 'Monitor', width: 90, sortable: false, align: "left" },
                        { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
                        { name: 'District', index: 'District', width: 40, sortable: false, align: "left", search: false },
                        { name: 'Block', index: 'Block', width: 40, sortable: false, align: "left", search: false },
                        { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
                        { name: 'SanctionYear', index: 'SanctionYear', width: 45, sortable: false, align: "center", search: false },
                        { name: 'RoadName', index: 'RoadName', width: 130, sortable: false, align: "left", search: false },
                        { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
                        { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
                        { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
                        { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
                        { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
                        { name: 'IsEnquiry', index: 'IsEnquiry', width: 30, sortable: false, align: "center", search: false },
                        { name: 'OverallGrade', index: 'OverallGrade', width: 50, sortable: false, align: "center", search: false },
                        { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },

                        { name: 'Submitted', index: 'Submitted', width: 40, sortable: false, align: "center", search: false },
                        { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center", search: false },
                        { name: 'UploadDate', index: 'UploadDate', width: 45, sortable: false, align: "center", search: false },
                        { name: 'RegradeStatus', index: 'RegradeStatus', width: 45, sortable: false, align: "center", search: false },
                        { name: 'RegradeRemarks', index: 'RegradeRemarks', width: 45, sortable: false, align: "center", search: false },
                        { name: 'RegradeDate', index: 'RegradeDate', width: 60, sortable: false, align: "center", search: false },
                        { name: 'Regrade', index: 'Regrade', width: 45, sortable: false, align: "center", search: false },
                        { name: 'Delete', index: 'Delete', width: 30, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "atrStatus": atrStatus, "rdStatusATR": rdStatusATR },
        pager: jQuery('#dv3TierATRListPager'),
        rowNum: 20000,
        pgbuttons: false,
        pgtext: null,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;ATR List",
        height: '300',
        autowidth: true,
        //rowList: [5, 10, 15],
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            if ($("#hdnRoleCodeOnSqcLayout3Tier").val() == 8 || $("#hdnRoleCodeOnSqcLayout3Tier").val() == 69)
            {
                $('#tb3TierATRList').jqGrid('hideCol', 'State');
                $('#tb3TierATRList').jqGrid('hideCol', 'Delete');

                $('#tb3TierATRList').setGridWidth(($('#div3TierATRDetails').width() - 10), true);
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

    $("#tb3TierATRList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{
            startColumnName: 'Submitted', numberOfColumns: 8, //titleText: 'Action Taken Report (ATR)'
            titleText: '<table style="width:100%;border-spacing:0px;">' +
                        '<tr><td id="h0" colspan="8"><center>Action Taken Report (ATR)</center></td></tr>' +
                        '<tr>' +
                            '<td id="h1" style="width:35.5%;border-top:1px solid #CCC;border-right:1px solid #CCC;"></td>' +
                            '<td id="h4" style="width:55.6%;border-top:1px solid #CCC;border-right:1px solid #CCC;text-align:center;">Regrade</td>' +
                            '<td id="h5" style="width:8.9%;border-top:1px solid #CCC;"></td>' +
                        '</tr>' +
                        '</table>'
        },

                       { startColumnName: 'Acceptance', numberOfColumns: 4, titleText: 'Regrade' }
        ]
    });
}



//Displays Individual Monitor Data
function ShowMonitorData(monitorId) {

    $("#accordion3TierSqcSchedule div").html("");
    $("#accordion3TierSqcSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Monitor Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion3TierSqcSchedule').show('slow', function () {
        blockPage();
        $("#div3TierSqcScheduleDetails").load('/QualityMonitoring/MonitorDetails/' + monitorId, function () {
            unblockPage();
        });
    });

    $('#div3TierSqcScheduleDetails').show('slow');
    $("#div3TierSqcScheduleDetails").css('height', 'auto');


    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'hidden');
}



//Displays Individual Montor Data
function QMAssignRoads(scheduleCode) {
    jQuery('#tb3TierSqcScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion3TierSqcSchedule div").html("");
    $("#accordion3TierSqcSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion3TierSqcSchedule').show('slow', function () {
        blockPage();
        $("#div3TierSqcScheduleDetails").load('/QualityMonitoring/QMAssignRoads/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#div3TierSqcScheduleDetails').show('slow');
    $("#div3TierSqcScheduleDetails").css('height', 'auto');

    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'hidden');
}


function FinalizeSchedule(scheduleCode) {
    if (confirm("Please confirm the schedule and then only finalize it.\nAre You Sure?")) {
        $.ajax({
            url: '/QualityMonitoring/FinalizeDistricts/',
            type: 'POST',
            data: { adminSchCode: scheduleCode, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Schedule finalized successfully");
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
                    $("#tb3TierSqcScheduleList").trigger("reloadGrid");
                    Close3TierSqcScheduleDetails();
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


function ViewDetails(scheduleCode) {
    jQuery('#tb3TierSqcScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordion3TierSqcSchedule div").html("");
    $("#accordion3TierSqcSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Districtwise Schedule Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion3TierSqcSchedule').show('slow', function () {
        blockPage();
        $("#div3TierSqcScheduleDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#div3TierSqcScheduleDetails").css('height', 'auto');
    $('#div3TierSqcScheduleDetails').show('slow');

    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewPrevSchDetails(scheduleCode) {
    blockPage();

    var number = Math.floor((Math.random() * 99999999) + 1);

    $("#dvPrevSchDistrictwiseDetails").show();
    $("#dvPrevSchDistrictwiseDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
        unblockPage();
    });
}


function ShowPreviousSchedulesSqc3Tier()
{
    $("#accordion3TierSqcSchedule div").html("");
    $("#accordion3TierSqcSchedule h3").html(
            "<a href='#' style= 'font-size:.9em;' >Previous Schedules</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcScheduleDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion3TierSqcSchedule').show('slow', function () {
        blockPage();
        $("#div3TierSqcScheduleDetails").load('/QualityMonitoring/QMPreviousSchedules3TierSQC/', function () {
            unblockPage();
        });
    });
    $('#div3TierSqcScheduleDetails').show('slow');
    $("#div3TierSqcScheduleDetails").css('height', 'auto');


    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'hidden');
}



//-------------------------------------------------------------------------------
function CloseATR3TierCqcDetails() {
    $('#accordionATR3TierCqc').hide('slow');
    $('#divATR3TierCqcDetails').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}

function showATRFilter() {
    if ($('#div3TierATRFilterForm').is(":hidden")) {
        $("#div3TierATRFilterForm").show("slow");
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function ShowObservationDetails(obsId) {

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierSqcInspection div").html("");
    $("#accordion3TierSqcInspection h3").html(
            "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion3TierSqcInspection').show('slow', function () {
        blockPage();
        $("#div3TierSqcInspDetails").load('/QualityMonitoring/QMObservationDetails3TierSQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierSqcInspDetails").css('height', 'auto');
    $('#div3TierSqcInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


function DownloadATR(atrId) {
    window.location = "/QualityMonitoring/DownloadFile/" + atrId;
}
// Added for Downloading of maintenance ATRs in SQC login
function DownloadMaintenanceATR(atrId) {
    window.location = "/QualityMonitoring/DownloadMaintenanceATRFile/" + atrId;
}

/* ATR Functions Starts Here*/

function UploadATR(obsId) {
    
    $("#accordionATR3TierCqc div").html("");
    $("#accordionATR3TierCqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload ATR</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/PdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();

}


//Observation Details for QM Obs Entry (Original Grading)
function ShowATRObsDetails(obsId) {

    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionATR3TierCqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/false', function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();
}

//Observation Details for (Corrected Grading)
function ShowATRGradingDetails(obsId) {

    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionATR3TierCqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/true', function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();
}

function RegradeATR(obsId) {

    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionATR3TierCqc h3").html(
            "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/QMATRRegrade/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();
}


function DeleteATR(obsId, atrId) {
    if (confirm("Are you sure to delete the ATR details?")) {
        $.ajax({
            url: '/QualityMonitoring/QMDeleteATRDetails/',
            type: 'POST',
            data: { obsId: obsId, atrId: atrId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("ATR Details deleted successfully");
                    //$("#tb3TierATRList").trigger("reloadGrid");
                    viewATRDetails();
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

function regradeMaintenanceATRAsUploaded(obsId) {
    if (confirm("Are you sure to update ATR?")) {
        $.ajax({
            url: '/QualityMonitoringHelpDesk/UpdateMaintnenanceATRStatus/',
            type: 'POST',
            data: { obsId: obsId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("ATR updated successfully");
                    viewMaintenanceATRDetails();
                }
                else {
                    alert(response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}

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

/* ATR Functions Ends Here*/

//-------------------------------------------------------------------------------

function ShowInspReportFile(obsId) {

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierSqcInspection div").html("");
    $("#accordion3TierSqcInspection h3").html(
            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion3TierSqcInspection').show('slow', function () {
        blockPage();
        $("#div3TierSqcInspDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#div3TierSqcInspDetails").css('height', 'auto');
    $('#div3TierSqcInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}


//added by abhinav pathak on 13-aug-2019
function ShowInspReportFilePDF(obsId , prRoadCode) {

    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

    $("#accordion3TierSqcInspection div").html("");
    $("#accordion3TierSqcInspection h3").html(
            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordion3TierSqcInspection').show('slow', function () {
        blockPage();
        $("#div3TierSqcInspDetails").load('/QualityMonitoring/PdfFileUploadView/' + obsId , function () {
            unblockPage();
        });
    });

    $("#div3TierSqcInspDetails").css('height', 'auto');
    $('#div3TierSqcInspDetails').show('slow');

    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
    $('#id3TierSQCInspFilterDiv').trigger('click');
}