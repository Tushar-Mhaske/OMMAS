/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayout.js
        * Description   :   Handles events for in Quality Layout
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    //$.validator.unobtrusive.parse($('#3TierFilterForm'));
    $("#spnModuleName").html("Quality Monitoring");

    $("#tabs-3TierDetails").tabs();
    $('#tabs-3TierDetails ul').removeClass('ui-widget-header');

  
    $('#tabOne1').trigger('click');

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

    $(function () {
        $("#accordionATR2TierSqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#btn3Tier").click(function () {
        $("#btn3Tier").addClass("ui-state-highlight");
        $("#btn2Tier").addClass("ui-state-highlight");
        window.location = "/QualityMonitoring/QualityLayout";
    });

    $("#btn2Tier").click(function () {
        window.location = "/QualityMonitoring/QualityLayout2TierCQC";
    });

    $("#btn3TierSQC").click(function () {
        window.location = "/QualityMonitoring/QualityLayout";
    });

    $("#btn2TierSQC").click(function () {
        window.location = "/QualityMonitoring/QualityLayoutSQC";
    });

    $("#spn3TierATRHtml").click(function () {
        toggleATRDetails();
    });


    //New change added by deepak 16-Sept- 2014
    $("#btn1TierPIU").click(function () {

        blockPage();
        $("#tabs-3TierDetails").html('');
        $("#tabs-3TierDetails").load('/QualityMonitoring/QM1TierPIU/', function () {
            //unblockPage();
        });
        $('#tabs-3TierDetails').show('slow');
        unblockPage();

        // window.location = "/QualityMonitoring/QualityLayoutPIU";
    });
    //End Change added by deepak 16-Sept- 2014

    InspectionListGrid(0, 0, $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());

});//doc.ready ends here


//-----------------------------Base Function of Tab Click---------------------------------------------------

function showFilter() {
    if ($('#div3TierFilterForm').is(":hidden")) {
        $("#div3TierFilterForm").show("slow");
        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function showATRFilter() {
    if ($('#div3TierATRFilterForm').is(":hidden")) {
        $("#div3TierATRFilterForm").show("slow");
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
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
    $('#divInspectionDetails').hide('slow');
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

function ShowInspectionDetails() {
    blockPage();
    $("#div3TierInspectionQualityFilters").load('/QualityMonitoring/QualityFilters', function () {
        $.validator.unobtrusive.parse($('#frmQualityFilters'));
        unblockPage();
    });
    $('#div3TierInspectionQualityFilters').show('slow');
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


///   ATR Details ends here

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


function ShowATRDetails() {

    blockPage();
    $("#div3TierATRQualityFilters").load('/QualityMonitoring/QualityATRFilters', function () {
        $.validator.unobtrusive.parse($('#3TierFilterForm'));
        unblockPage();
    });
    $('#div3TierATRQualityFilters').show('slow');
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


function ShowMonitorsDetails() {
    //alert("Test Monitor");
    $("#accordionInspection div").html("");
    CloseInspectionDetails();

    $("#accordionATR3TierCqc div").html("");
    CloseATR3TierCqcDetails();

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


//Schedule List for CQC
function ScheduleListGrid(month, year) {


    $("#tb3TierScheduleList").jqGrid('GridUnload');

    jQuery("#tb3TierScheduleList").jqGrid({
        url: '/QualityMonitoring/E?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "State", "District 1", "District 2", "District 3",
                    "Inspection Status", "Add District", "Add Work", "Add Contractors", "View", "Delete", "Finalize District", "View Letter", "Forward To Monitor", "Unlock"],
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
                            { name: 'Finalize', index: 'Finalize', width: 80, sortable: false, align: "center", search: false },
                            { name: 'ViewLetter', index: 'ViewLetter', width: 40, sortable: false, align: "center", search: false },
                            { name: 'Forward', index: 'Forward', width: 80, sortable: false, align: "center", search: false },
                            { name: 'Unlock', index: 'Unlock', width: 80, sortable: false, align: "center", search: false }
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


            if (($("#hdnRoleCodeOnLayout").val() == 8)) //for SQC hide these two columns
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


function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear) {

    $("#tb3TierInspectionList").jqGrid('GridUnload');

    jQuery("#tb3TierInspectionList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                    "Start Chainage (Km.)", "End Chainage (Km.)", "Schedule Date", "Inspection Date", "Upload Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
                    "Uploaded by", "View Details", "Grade Correction", "Upload Image", "Upload / View Report", "Delete"],
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
                            { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },
                            { name: 'Correction', index: 'Correction', width: 30, sortable: false, align: "center", search: false },
                            { name: 'UploadImage', index: 'UploadImage', width: 30, sortable: false, align: "center", search: false },
                            { name: 'UploadReport', index: 'UploadReport', width: 25, sortable: false, align: "center", search: false },
                            { name: 'Delete', index: 'Delete', width: 30, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear },
        pager: jQuery('#dv3TierInspectionListPager'),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        pgbuttons: false,
        pgtext: null,
        height: '300',
        autowidth: true,
        //sortname: 'Monitor',
        //rowList: [5, 10, 15],
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            $('#tb3TierInspectionList').jqGrid('hideCol', 'Delete');
            $('#tb3TierInspectionList').setGridWidth(($('#div3TierInspectionList').width() - 10), true);
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
        height: '250',
        autowidth: true,
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {

            //if ($("#hdnRoleCodeOnLayout").val() == 5) //for CQCAdmin Login
            //{
            //    $("#tb3TierATRList #dv3TierATRListPager").css({ height: '35px' });
            //    $("#dv3TierATRListPager_left").html("<input type='button' style='margin-left:1px' id='idBulkRegrade' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowBulkATRDetails();return false;' value='Regrade ATR'/>");
            //}

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
}//ATR List Ends Here




//function UnlockSchedule(id) {
//    alert('unlockSchedule');
//    alert(id);
//    var r = confirm('Are you Sure to Definalize the Schedule Details!!!');
//    if (r == true) {
//        if (unlock(id)) {
//            //alert(res);
//            alert('Schedule Definalized Successfully ');
//        }
//        else {
//            alert('Sorry, No Such Record Exists!!!!');

//        }
//    }
//    else {
//        $(this).hide();
//    }

//}



//function unlock(id) {
//    alert('unlock');
//    return jQuery.ajax({
//        url: "/QualityMonitoringHelpDesk/UnlockScheduleData/",
//        data: { selectRowId: function () { return id }, yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() },
//        cache: false,
//        type: 'POST',
//        success: function (result) {

//            if (result) {
//                //alert(result);
//                jQuery("#tableScheduleDetails").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListScheduleResult/", page: 1, postData: { yr: $("#yearList").val(), mnth: $("#monthList").val(), qm: $("#qmTypeList").val() } }).trigger("reloadGrid");

//                return true;
//            }
//            else {
//                //alert(result+"inelse");
//                return false;
//            }
//        },

//    });
//}

/// -------------------- Grid Column Click Functions -----------------------------------


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
    if ($("#hdnRoleCodeOnLayout").val() == 9) {
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


    if ($("#hdnRoleCodeOnLayout").val() == 8) //for SQC Only One District
    {
        if (confirm("Are you sure to delete the schedule?")) {
            $.ajax({
                url: '/QualityMonitoring/QMDeleteSchedule/',
                type: 'POST',
                data: { adminSchCode: scheduleCode, value: Math.random() },
                success: function (response) {
                    if (response.Success) {
                        alert("Schedule deleted successfully");
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


function ShowObservationDetails(obsId) {

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
            "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionInspection').show('slow', function () {
        //blockPage();
        $("#divInspectionDetails").load('/QualityMonitoring/QMObservationDetails/' + obsId, function () {
            //unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}


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




/* ATR Functions Starts Here*/

function ShowBulkATRDetails() {
    $("#accordionBulkATRDetails div").html("");
    $("#accordionBulkATRDetails h3").html(
            "<a href='#' style= 'font-size:.9em;' >Regrade ATR</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseBulkATRDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionBulkATRDetails').show('slow', function () {
        blockPage();
        $("#divBulkATRDetails").load('/QualityMonitoring/QMBulkATRDetails/', function () {
            unblockPage();
        });
    });

    $('#divBulkATRDetails').show('slow');
    $("#divBulkATRDetails").css('height', 'auto');

    toggleATRDetails();
}


function UploadATR(obsId) {

    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    CloseInspectionDetails();
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



function regradeAsUploaded(obsId) {
    if (confirm("Are you sure to update ATR?")) {
        $.ajax({
            url: '/QualityMonitoringHelpDesk/UpdateATRStatus/',
            type: 'POST',
            data: { obsId: obsId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("ATR updated successfully");
                    viewATRDetails();
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



/* ATR Functions Ends Here*/


function ShowInspReportFile(obsId) {

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionInspection').show('slow', function () {
        blockPage();
        $("#divInspectionDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}






// Show Workitem
function ShowWorkItemDetails() {
    
    blockPage();
    $("#div1").load('/Gepnic/GetGepnicTenderDetailsLayout', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout'));
        unblockPage();
    });
    $('#div1').show('slow');

    
}


// Show Tender 
function ShowTenderByPublishDateDetails() {
   
    blockPage();
    $("#div2").load('/Gepnic/GetTenderXMLFromGepenicLayout', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout1'));
        unblockPage();
    });


  
    $('#div2').show('slow');
   
}


// Show Corrigendum 
function ShowCorrigendumByPublishDateDetails() {
   
    blockPage();
    $("#div3").load('/Gepnic/GetCorrInfoByPublishDateLayout', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout2'));
        unblockPage();
    });
    
   
    $('#div3').show('slow');
    
}

// Show AOC 
function ShowAOCByCreatedDateDetails() {
   
    blockPage();
    $("#div4").load('/Gepnic/GetAOCInfoByPublishDateLayout', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout3'));
        unblockPage();
    });
  
    $('#div4').show('slow');
   
}