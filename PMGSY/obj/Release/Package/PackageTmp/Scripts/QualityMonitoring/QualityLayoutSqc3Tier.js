/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayoutSqc3Tier.js
        * Description   :   Handles events for in Quality Layout for 3 Toer in SQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $("#tabs-3TierDetailsSqc").tabs();

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


    Sqc3TierScheduleListGrid();

});


function closeDivError() {
    $('#divError').hide('slow');
}

function Close3TierSqcScheduleDetails() {
    $('#accordion3TierSqcSchedule').hide('slow');
    $('#div3TierSqcScheduleDetails').hide('slow');
    $("#tb3TierSqcScheduleList").jqGrid('setGridState', 'visible');
}


function CloseATR3TierSqcDetails() {
    $('#accordionATR3TierSqc').hide('slow');
    $('#divInspectionDetails').hide('slow');
    $("#tb3TierSqcATRList").jqGrid('setGridState', 'visible');
}




function Show3TierSqcScheduleListGrid(month, year)
{

    Sqc3TierScheduleListGrid(month, year);
}


function Show3TierSqcATRDetails() {
    blockPage();
    $("#div3TierSqcATRQualityFilters").load('/QualityMonitoring/QualityFilters', function () {
        $.validator.unobtrusive.parse($('#frmQualityFilters'));
        unblockPage();
    });
    $('#div3TierSqcATRQualityFilters').show('slow');
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


    //$("#tb3TierSqcScheduleList").jqGrid('setGridState', 'hidden');
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

