/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayout.js
        * Description   :   Handles events for in Quality Layout
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

function UploadMissingATR(obsId) {

    $("#accordionATR3TierCqc div").html("");
    $("#accordionATR3TierCqc h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload Missing ATR File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/ATRPdfMissingFileUpload/' + obsId, function () {

            //Below Code Added on 30-01-2023
            ListPDFFiles(obsId);
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();

}


//add below function by vikky on 08-09-2022
function viewPdf(id) {

    $.ajax({
        type: "POST",
        url: '/EFORM/IsFileAvail?idtemp=' + id,
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {

            if (response.response) {

                window.open("/EFORM/GetCombineReport?idtemp=" + id);
            }
            else {
                alert("File not found");

            }

        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }
    });


}
$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    //$.validator.unobtrusive.parse($('#3TierFilterForm'));
    $("#spnModuleName").html("Quality Monitoring");

    $("#tabs-3TierDetails").tabs();
    $('#tabs-3TierDetails ul').removeClass('ui-widget-header');


    $(function () {
        $("#accordionSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionATR2TierSQC").accordion({
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

    $(function () {    //Added on 25 Jan 2021 by Aditi
        $("#accordionATRInspection").accordion({
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

    // Commented on 04-03-2022 by Srishti Tyagi
    //$("#spn3TierATRHtml").click(function () {
    //    toggleATRDetails();
    //});


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

    //InspectionListGrid(0, 0, $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());    //Commented to fix grid compression issue

});//doc.ready ends here


//-----------------------------Base Function of Tab Click---------------------------------------------------

// Added by CDA
function ShowWorkListDetails() {

    $("#accordionInspection div").html("");
    CloseInspectionDetails();

    $("#accordionATR3TierCqc div").html("");
    CloseATR3TierCqcDetails();

    blockPage();
    $("#stateListFilter").load('/QualityMonitoring/ViewWorkList/', function () {
        unblockPage();
    });

    $('#stateListFilter').show('slow');
}

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
    if (!$("#div3TierATRDetailsHtml").is(':hidden')) {
    }
    else {
        toggleATRDetails();
    }
}

function CloseATRInspection() {     //Added By Aditi on 25 Jan 2021
    //  debugger;
    $('#accordionATRInspection').hide('slow');
    $('#divATRInspectionDetails').hide('slow');
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
    //alert("A")

    $("#CloseATRGrid").hide('slow');
    $("#ATRReport").hide('slow');

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

    $("#CloseATRGrid").hide('slow');
    $("#ATRReport").hide('slow');

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
    //  debugger;
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
    //alert("MaintenanceInspection")
    blockPage();
    $("#div3TierAddMaintenanceInspection").load('/QualityMonitoring/MaintenanceInspection', function () {
        $.validator.unobtrusive.parse($('#frmMaintenanceInspection'));
        unblockPage();
    });
}
//--------------------------------------------------------------------------------------------------------

function LoadSQCDetails() {
    //alert("LoadSQCDetails")
    blockPage();
    $("#div3TierSQCDetails").load("/Master/ListAdminSqc/", function () {
        unblockPage();
    });
}


function QMHelpDesk() {

    blockPage();
    $("#div3TierQMHelpDesk").load("/QualityMonitoringHelpDesk/QMHelpDesk", function () {
        //$.validator.unobtrusive.parse($('#3TierFilterForm'));
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
    //   alert("SA 212")

    $("#tb3TierScheduleList").jqGrid('GridUnload');

    jQuery("#tb3TierScheduleList").jqGrid({
        url: '/QualityMonitoring/GetScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "State", "District 1", "District 2", "District 3",
            "Inspection Status", "Add District", "Add Work", "Add Contractors", "View", "Delete", "Finalize District", "View Letter", "Forward To Monitor", "Unlock", "Tour Claim"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 150, sortable: false, align: "center", search: false },
            { name: 'State', index: 'State', width: 110, sortable: false, align: "left", search: false },

            { name: 'District1', index: 'District1', width: 90, sortable: false, align: "left", search: false },
            { name: 'District2', index: 'District2', width: 90, sortable: false, align: "left", search: false },
            { name: 'District3', index: 'District3', width: 90, sortable: false, align: "left", search: false },

            { name: 'InspStatus', index: 'InspStatus', width: 80, sortable: false, align: "center", search: false },
            { name: 'AddDistrict', index: 'AddDistrict', width: 80, sortable: false, align: "center", search: false, hidden: false },
            { name: 'AddRoad', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false },

            { name: 'AddContractors', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false },
            { name: 'View', index: 'View', width: 70, sortable: false, align: "center", search: false },
            { name: 'Delete', index: 'Delete', width: 70, sortable: false, align: "center", search: false },

            { name: 'Finalize', index: 'Finalize', width: 80, sortable: false, align: "center", search: false },
            { name: 'ViewLetter', index: 'ViewLetter', width: 40, sortable: false, align: "center", search: false },
            { name: 'Forward', index: 'Forward', width: 80, sortable: false, align: "center", search: false },
            { name: 'Unlock', index: 'Unlock', width: 80, sortable: false, align: "center", search: false },
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


            if (($("#hdnRoleCodeOnLayout").val() == 8 || $("#hdnRoleCodeOnLayout").val() == 69)) //for SQC hide these two columns
            {
                // $('#tb3TierScheduleList').jqGrid('hideCol', 'District2');
                //  $('#tb3TierScheduleList').jqGrid('hideCol', 'District3');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'AddDistrict');
                $('#tb3TierScheduleList').jqGrid('hideCol', 'ViewLetter');
                // $('#tb3TierScheduleList').jqGrid('hideCol', 'Unlock');   //Commented on 15 March 2021 to allow unlocking before and after schedule download scenario only for 2nd tier sqc login

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



//replace this function ->QualityLayout.js
function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatusType) {

    $("#tb3TierInspectionList").jqGrid('GridUnload');

    jQuery("#tb3TierInspectionList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetails?' + Math.random(),
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
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "schemeType": schemeType, "roadStatus": roadStatus, "roadOrBridge": roadOrBridge, "gradeType": gradeType, "eFormStatusType": eFormStatusType },  // add parameter on 30-06-2022 by vikky
        pager: jQuery('#dv3TierInspectionListPager'),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
        pgbuttons: false,
        pgtext: null,
        height: '300',
        autowidth: true,
        hidegrid: true,
        autoResizing: { adjustGridWidth: false },
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
            if ($('#hdnRoleCodeOnLayout').val() != 9) {
                $('#tb3TierInspectionList').jqGrid('hideCol', 'Delete');
            }
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

    jQuery("#tb3TierInspectionList").jqGrid('setGroupHeaders', {
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
function viewBridgeCombinePdfData(eid) {
    window.open("/EFORM/Preview_BRIDGE_CQC?eid=" + eid);

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

    // alert(id);
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


function viewTRScanPdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewTRScanPdfVirtualDir/' + id,
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



function viewCombinedPart_1_2_TR_Pdf(id) {
    // alert("1");
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/isAllPdfAvail/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open("/EFORM/viewCombinedPdf12TR/" + id);
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





//in new tab

function ShowInspAgainstRoad(roadCode1) {

    window.open("/QualityMonitoring/QMViewInspectionListAgainstRoadNewTab?roadCode1=" + roadCode1).focus();

}

//function ShowInspAgainstRoad(roadCode1) {
//    blockPage();

//    $("#div3TierInspectionList_Road_tab3").show();
//    //alert("ShowInspAgainstRoad");
//    // alert(roadCode1);
//    var tab = roadCode1.split("$")[1];
//    // alert(tab);
//    var roadCode = roadCode1.split("$")[0];
//    if (tab == "tab3") {
//        //selected_tab = 3;
//        $("#selectedTab").val(3)
//    } else {
//        //selected_tab = 1;
//        $("#selectedTab").val(1)
//    }



//    $("#tb3TierInspectionList_Road_" + tab).jqGrid('GridUnload');

//    jQuery("#tb3TierInspectionList_Road_" + tab).jqGrid({
//        url: '/QualityMonitoring/QMViewInspectionListAgainstRoad?' + Math.random(),
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Monitor Type", "ATR Verification status","Monitor", "State", "District", "Block", "Road/Bridge Name", "Package No", "Road/Bridge", "Scheme", "Work Status (As inspected)", "Sanctioned Length(km) / Bridge Length(m)", "From(km)", "To(km)", "Inspection Date",
//            "Overall Grade", "View Report (pdf)", "View abstract/Images", "Images Uploaded"],
//        colModel: [
//            { name: 'Monitor_Type', index: 'Monitor_Type', width: 50, sortable: false, align: "left" },
//            { name: 'ATRVerificationStatus', index: 'ATRVerificationStatus', width: 60, sortable: false, align: "left" },
//            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "left" },
//            { name: 'State', index: 'State', width: 60, sortable: false, align: "center", search: false },
//            { name: 'District', index: 'District', width: 45, sortable: false, align: "left", search: false },
//            { name: 'Block', index: 'Block', width: 50, sortable: false, align: "left", search: false },
//            { name: 'RoadName', index: 'RoadName', width: 130, sortable: false, align: "left", search: false },
//            { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
//            { name: 'PropType', index: 'PropType', width: 40, sortable: false, align: "left", search: false },
//            { name: 'Scheme', index: 'Scheme', width: 40, sortable: false, align: "center", search: false },
//            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
//            { name: 'SancLength', index: 'SancLength', width: 50, sortable: false, align: "center", search: false },
//            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
//            { name: 'OverallGrade', index: 'OverallGrade', width: 75, sortable: false, align: "center", search: false },
//            { name: 'viewreport', index: 'UploadBy', width: 40, sortable: false, align: "center", search: false },
//            { name: 'viewImages', index: 'UploadReport', width: 50, sortable: false, align: "center", search: false },
//            { name: 'Images uploaded', index: 'View', width: 30, sortable: false, align: "center", search: false }
//        ],
//        postData: { "roadCode": roadCode },
//        pager: jQuery('#dv3TierInspectionListPager_Road_' + tab),
//        rowNum: 20000,
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Inspection List Against Road/Bridge",
//        pgbuttons: false,
//        pgtext: null,
//        height: '300',
//        autowidth: true,
//        sortname: 'Monitor',
//        //rowList: [5, 10, 15],

//        loadComplete: function () {
//            $('.dataTable').css({ display: 'none' });
//            $('#div3TierInspectionList_Road_' + tab).setGridWidth(($('#div3TierInspectionList_Road_' + tab).width() - 10), true);
//            unblockPage();
//        },
//        loadError: function (xhr, status, error) {
//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//        }
//    }); //end of grid

//    jQuery("#tb3TierInspectionList_Road_" + tab).jqGrid('setGroupHeaders', {
//        useColSpanStyle: true,
//        groupHeaders: [
//            { startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: '<center>Inspection Chainage</center>' },
//            { startColumnName: 'viewreport', numberOfColumns: 3, titleText: '<center>Report</center>' }
//            // { startColumnName: 'eformStatus', numberOfColumns: 3, titleText: '<center>E-form</center>' }
//        ]
//    });
//}



//comment by vikky 30-06-2022
//function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, schemeType, roadStatus, roadOrBridge, gradeType, eFormStatusType) {

//    $("#tb3TierInspectionList").jqGrid('GridUnload');

//    jQuery("#tb3TierInspectionList").jqGrid({
//        url: '/QualityMonitoring/QMViewInspectionDetails?' + Math.random(),
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
//            "Start Chainage (Km.)", "End Chainage (Km.)", "Schedule Date", "Inspection Date", "Upload Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
//            "Uploaded by", "View Details", "Grade Correction", "Upload Image", "View Work Inspection Report", "Delete", "View Multiple PDF",],
//        colModel: [
//            { name: 'Monitor', index: 'Monitor', width: 90, sortable: false, align: "left" },
//            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
//            { name: 'District', index: 'District', width: 40, sortable: false, align: "left", search: false },
//            { name: 'Block', index: 'Block', width: 40, sortable: false, align: "left", search: false },
//            { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
//            { name: 'SanctionYear', index: 'SanctionYear', width: 45, sortable: false, align: "center", search: false },
//            { name: 'RoadName', index: 'RoadName', width: 140, sortable: false, align: "left", search: false },
//            { name: 'PropType', index: 'PropType', width: 40, sortable: false, align: "left", search: false },
//            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'InspToChainage', index: 'InspToChainage', width: 35, sortable: false, align: "center", search: false },
//            { name: 'SCHEDULE_DATE', index: 'SCHEDULE_DATE', width: 60, sortable: false, align: "center", search: false },
//            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
//            { name: 'UploadDate', index: 'UploadDate', width: 60, sortable: false, align: "center", search: false },
//            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
//            { name: 'IsEnquiry', index: 'IsEnquiry', width: 50, sortable: false, align: "center", search: false },
//            { name: 'Scheme', index: 'Scheme', width: 50, sortable: false, align: "center", search: false },

//            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
//            { name: 'NoOfImagesUploaded', index: 'NoOfImagesUploaded', width: 40, sortable: false, align: "center", search: false },
//            { name: 'UploadBy', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },

//            { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },
//            { name: 'Correction', index: 'Correction', width: 30, sortable: false, align: "center", search: false },
//            { name: 'UploadImage', index: 'UploadImage', width: 30, sortable: false, align: "center", search: false },
//            { name: 'UploadReport', index: 'UploadReport', width: 25, sortable: false, align: "center", search: false },
//            { name: 'Delete', index: 'Delete', width: 30, sortable: false, align: "center", search: false },
//            { name: 'UploadPDF', index: 'UploadPDF', width: 30, sortable: false, align: "center", search: false },
//        ],
//        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "schemeType": schemeType, "roadStatus": roadStatus, "roadOrBridge": roadOrBridge, "gradeType": gradeType, "eFormStatusType": eFormStatusType },  // add parameter on 30-06-2022 by vikky
//        pager: jQuery('#dv3TierInspectionListPager'),
//        rowNum: 20000,
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Inspection List",
//        pgbuttons: false,
//        pgtext: null,
//        height: '300',
//        autowidth: true,
//        //sortname: 'Monitor',
//        //rowList: [5, 10, 15],
//        grouping: true,
//        groupingView: {
//            groupField: ['State', 'District'],
//            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
//            groupColumnShow: [false, false],
//            groupCollapse: false
//        },
//        loadComplete: function () {
//            if ($('#hdnRoleCodeOnLayout').val() != 9) {
//                $('#tb3TierInspectionList').jqGrid('hideCol', 'Delete');
//            }
//            $('#tb3TierInspectionList').setGridWidth(($('#div3TierInspectionList').width() - 10), true);
//            unblockPage();
//        },
//        loadError: function (xhr, status, error) {
//            if (xhr.responseText == "session expired") {
//                window.location.href = "/Login/SessionExpire";
//            }
//            else {
//                window.location.href = "/Login/SessionExpire";
//            }
//        }
//    }); //end of grid
//}


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


    if ($("#hdnRoleCodeOnLayout").val() == 8 || $("#hdnRoleCodeOnLayout").val() == 69) //for SQC Only One District
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
    //    debugger; 
    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId); Testing
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
            // alert("3");
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();

}

//Below Code Added on 30-01-2023
function UploadATRFile(obsId) {
    //    debugger; 
    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId); Testing
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
        //$("#divATR3TierCqcDetails").load('/QualityMonitoring/PdfFileUploadATR/' + obsId, function () {
        //    ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId($(IMS_PR_ROAD_CODE).val() + "$" + $(QM_INSPECTION_DATE).val() + "$" + $(QM_OBSERVATION_ID).val() + "$" + $(QM_ATR_ID).val());
        //    ListPDFFiles($("#QM_OBSERVATION_ID").val());
        //    unblockPage();           
        //});
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/PdfFileUploadATR/' + obsId, function (response) {
            var result = JSON.parse(response);
            if (result.success == false) {
                alert(result.message);
                unblockPage();
                CloseATR3TierCqcDetails();
                return false;
            }

            ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId($(IMS_PR_ROAD_CODE).val() + "$" + $(QM_INSPECTION_DATE).val() + "$" + $(QM_OBSERVATION_ID).val() + "$" + $(QM_ATR_ID).val());
            ListPDFFiles($("#QM_OBSERVATION_ID").val());
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();

}


function ShowATRObsDetailsNewtab(obsId) {
    // alert("ShowATRObsDetails");

    if (!$("#div3TierATRDetailsHtml").is(':hidden')) {
        toggleATRDetails();
    }
    if (!$("#accordionATR3TierCqc").is(':hidden')) {

        if ($("#div3TierATRDetailsHtml").is(':hidden')) {
        }
        else {
            toggleATRDetails();
        }
    }


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

    // toggleATRDetails();
}


//Observation Details for QM Obs Entry (Original Grading)
function ShowATRObsDetails(obsId) {

    if (!$("#div3TierATRDetailsHtml").is(':hidden')) {
        toggleATRDetails();
    }
    if (!$("#accordionATR3TierCqc").is(':hidden')) {

        if ($("#div3TierATRDetailsHtml").is(':hidden')) {
        }
        else {
            toggleATRDetails();
        }
    }
    // alert("B")
    //  debugger;
    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);
    $("#div3TierATRQualityActionReport").hide();


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

    // toggleATRDetails();
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

// CloseATRGrid


$("#CloseATRGrid").click(function () {

    $("#CloseATRGrid").hide('slow');
    $("#ATRReport").hide('slow');

});


function ViewInspectionReportATR(obsId) {   //Added by Aditi on 25 Jan 2021
    //   debugger;
    //jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);


    //$("#ATRReport").load('/QualityMonitoring/InspPdfFileUpload?id=' + obsId, function () {
    //    unblockPage();
    //});






    //$("#CloseATRGrid").show();
    //$("#ATRReport").load('/QualityMonitoring/InspPdfFileUpload?id=' + obsId, function () {
    //    unblockPage();
    //});
    //$("#ATRReport").show('slow');
    if (!$("#div3TierATRDetailsHtml").is(':hidden')) {
        toggleATRDetails();
    }
    if (!$("#accordionATR3TierCqc").is(':hidden')) {
        // ...
        if ($("#div3TierATRDetailsHtml").is(':hidden')) {
        }
        else {
            toggleATRDetails();
        }

    }


    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionATR3TierCqc h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Report Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/InspPdfFileUpload?id=' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    //toggleATRDetails();



    //$("#ATRReport").hide('slow');


    // $("#accordionATR3TierCqc1").hide();


    //$("#accordionATR3TierCqc1 h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Test Details</a>" +

    //        '<a href="#" style="float: right;">' +
    //        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
    //        '<span style="float: right;"></span>'
    //        );

    //$("#accordionATR3TierCqc1").show();

    //$("#accordionInspection div").html("");
    //CloseInspectionDetails();
    //$("#accordionATRInspection div").html("");
    //$("#accordionATRInspection h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Inspection Report List</a>" +

    //        '<a href="#" style="float: right;">' +
    //        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATRInspection();" /></a>' +
    //        '<span style="float: right;"></span>'
    //        );

    //$('#accordionATRInspection').show('slow', function () {
    //    blockPage();
    //    $("#divATRInspectionDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
    //        unblockPage();
    //    });
    //});

    //$("#divATRInspectionDetails").css('height', 'auto');
    //$('#divATRInspectionDetails').show('slow');

    //toggleATRDetails();

}
/* ATR Functions Ends Here*/


function ShowInspReportFile(obsId) {
    //  debugger;
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

    //  alert("Id" + id)

    // alert("Lab Details")


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


    //$("#CloseATRGrid").show();
    //$("#ATRReport").load('/QualityMonitoring/LabImageUpload?id=' + id, function () {
    //    unblockPage();
    //});
    //$("#ATRReport").show('slow');















}

function CloseATR3TierCqcLabDetails() {
    $('#accordionATR3TierSqcLab').hide('slow');
    $("#tb3TierATRList").jqGrid('setGridState', 'visible');

    showATRFilter();
    toggleATRDetails();
}

//added by abhinav pathak on 13-aug-2019
function ShowInspReportFilePDF(obsId, prRoadCode) {

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
        $("#divInspectionDetails").load('/QualityMonitoring/PdfFileUploadView/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}




function ShowMonitorsDetailsInterState() {
    //   alert("Test Two");
    $("#accordionInspection div").html("");
    CloseInspectionDetails();

    $("#accordionATR3TierCqc div").html("");
    CloseATR3TierCqcDetails();

    blockPage();
    $("#div3TierAddMonitorInsterState").load('/Master/MasterQualityMonitorForMapping/', function () {

        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_ADDRESS1');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PIN');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PHONE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_MOBILE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_REMARKS');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PAN');

        unblockPage();
    });
    $('#div3TierAddMonitorInsterState').show('slow');

    $('#tblQualityMonitorList').setGridParam({ rowNum: 10 });


}

function ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(roadCode_inspdate_ObsId_ATRId) {

    //passing NQM Observation ID and ATR Id for finalize
    const NQM_ObsId_ATrCode = roadCode_inspdate_ObsId_ATRId;//.split("$");

    $('#tblVerificationATR_Grid').jqGrid('GridUnload');

    jQuery("#tblVerificationATR_Grid").jqGrid({
        url: '/QualityMonitoring/ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId',
        datatype: "json",
        mtype: "GET",
        multiselect: true,
        colNames: ["ObservationId", "RoadCode", "Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
            "Start Chainage (Km.)", "End Chainage (Km.)", "Inspection Date", "Total Length (Road(km)/ LSB(mtr)) ", "Road Status (Current Status)", /*"Enquiry Inspection",*/ "Ground Verification Inspection",
            "Scheme", "Overall Grade"],
        colModel: [
            { key: true, name: 'ObservationId', index: 'ObservationId', width: 100, hidden: true, sortable: false, align: "left" },
            { name: 'RoadCode', index: 'RoadCode', width: 100, hidden: true, sortable: false, align: "left" },
            { name: 'Monitor', index: 'Monitor', width: 100, sortable: false, align: "left" },
            { name: 'State', index: 'State', width: 75, sortable: false, align: "left", search: false },
            { name: 'District', index: 'District', width: 75, sortable: false, align: "left", search: false },
            { name: 'Block', index: 'Block', width: 75, sortable: false, align: "left", search: false },
            { name: 'Package', index: 'Package', width: 75, sortable: false, align: "left", search: false },
            { name: 'SanctionYear', index: 'SanctionYear', width: 65, sortable: false, align: "left", search: false },
            { name: 'RoadName', index: 'RoadName', width: 100, sortable: false, align: "left", search: false },
            { name: 'PropType', index: 'PropType', width: 50, sortable: false, align: "left", search: false },
            { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'InspToChainage', index: 'InspToChainage', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'InspDate', index: 'InspDate', width: 100, sortable: false, align: "left", search: false, formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd.m.Y' } },
            { name: 'TotalLength', index: 'TotalLength', width: 75, sortable: false, align: "left", search: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 3, defaultValue: '0.00' } },
            { name: 'RdStatus', index: 'RdStatus', width: 100, sortable: false, align: "left", search: false },
            //{ name: 'EnquiryInspection', index: 'EnquiryInspection', width: 100, sortable: false, align: "center", search: false },            
            { name: 'Ground_Verification_Inspection', index: 'Ground_Verification_Inspection', width: 100, sortable: false, align: "left", search: false },
            { name: 'Scheme', index: 'Scheme', width: 80, sortable: false, align: "left", search: false },
            { name: 'OverallGrade', index: 'OverallGrade', width: 100, sortable: false, align: "left", search: false },
        ],
        postData: { "VerificationATRCode": roadCode_inspdate_ObsId_ATRId },
        pager: '#divPagerVerificationATR_Grid',
        rowNum: 10,
        rowList: [10, 15, 20],
        rownumbers: true,
        pgbuttons: true,
        //pgtext: null,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Verification ATR List",
        //width: 'auto',
        //height: '250',
        autowidth: true,
        shrinkToFit: true,
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "ObservationId",
        },
        loadComplete: function (data) {

            $('.dataTable').css({ display: 'none' });

            $('#dvVerificationATR_Grid').show('slow');
            $('#tblVerificationATR_Grid').jqGrid("setGridState", "visible");

            // disabled multiselect as per requirement
            $('#cb_tblVerificationATR_Grid').attr("disabled", true);

            // Present Codes for ATR Already finalzed in Table Returned from jsonData
            var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

            for (var i = 0; i < userdata.ids.length; i++) {
                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tblVerificationATR_Grid_" + userdata.ids[i]).attr("disabled", true);
                }
            }
            $("#tblVerificationATR_Grid #divPagerVerificationATR_Grid").css({ height: '31px' });
            if (data["records"] > 0 && userdata.ids.length == 0 && userdata.ids.length < 1) {

                /* userdata.ids.length != data["records"]*/

                $('#divPagerVerificationATR_Grid_left').html("<input type='button' style='margin-left:50px; border: 2px outset green;' id='btnFinalizedVerifyATR' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick ='finalizedVerificationATR(\"" + NQM_ObsId_ATrCode + "\"); return false;' value='Finalize'/>");
            }
            unblockPage();
        },
        onSelectAll: function (aRowids, status) {

            // Present Codes in Payment Table Returned from jsonData
            var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

            for (var i = 0; i < userdata.ids.length; i++) {
                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tblVerificationATR_Grid_" + userdata.ids[i]).attr("disabled", true);
                }
            }
        },
        // Disabled to already checked check box
        beforeSelectRow: function (rowId, e) {

            if ($("#jqg_tblVerificationATR_Grid_" + rowId).attr("disabled")) {
                return false;
            }
            else
                return true;
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
} // end of func

// IF MARK ATR VERIFICATION LOCKED

function lock_MarkATRVerification() {
    alert('Please first upload ATR file !!');
    return false;
}

function lock_Mark2TierATRVerification() {
    alert('ATR file is not uploaded by PIU!!');
    return false;
}

function ATR_AlreadyFinilized() {
    alert('ATR verification already finalized !!');
    return false;
}

function alertATRnotMarkForVerification() {
    alert('SQM inspection is not marked for ATR verification by SQC !!');
    return false;
}


//----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
function finalizedVerificationATR(NQM_ObsId_ATrCode) {

    // Present Codes in Payment Table Returned from jsonData
    var userdata = jQuery("#tblVerificationATR_Grid").getGridParam('userData');

    if (userdata.ids.length == 1) {

        alert("Only one SQM inspection ATR can be finilize !!");
    }
    else {

        var submitArray = [];

        var selRowIds = jQuery('#tblVerificationATR_Grid').jqGrid('getGridParam', 'selarrrow');

        // alert("submitted array include Observation Id: " + selRowIds + "" + "selRowIds.length " + selRowIds.length);

        if (selRowIds.length > 0) {

            if (selRowIds.length == 1) {
                for (var i = 0; i < selRowIds.length; i++) {

                    rowdata = jQuery("#tblVerificationATR_Grid").getRowData(selRowIds[i]);
                    if (!$("#jqg_tblVerificationATR_Grid_" + selRowIds[i]).attr("disabled")) {
                        submitArray.push(rowdata["ObservationId"]);
                    }
                }
                // calling save finalized ATRS
                savefinalizedMarkVerificationATR(NQM_ObsId_ATrCode, submitArray);

            } else {
                alert("please select only one record for ATR verification finalize");
            }
        }
        else
            alert("No records to submit, please select any one record to finalize");
    }

}

//----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
function savefinalizedMarkVerificationATR(NQM_ObsId_ATrCode, submitArray) {

    //alert("submitArray: " + submitArray);
    //alert("NQM_ObsId_ATrCode: " + NQM_ObsId_ATrCode);

    if (confirm("Are you sure to finalize Mark For ATR Verification ?")) {
        $.ajax({
            url: "/QualityMonitoring/savefinalizedMarkVerificationATR",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'nqmobsidatrid': NQM_ObsId_ATrCode, 'submitarray': submitArray }), //{'submitSQMId': submitSQMId, 'NQM_ObsId_ATrCode': NQM_ObsId_ATrCode},
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);

                    $('#tblVerificationATR_Grid').trigger('reloadGrid');

                    $('#btnFinalizedVerifyATR').css({ display: 'none' });

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
}

//----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
function closeMarkATRDiv() {

    // to expand previous NQM list
    $('.dataTable').css({ display: 'block' });
    $('#dvVerificationATR_Grid').hide('slow');
    view_ATR_Details();
}


function closeInsplistDiv() {


    $('.dataTable').css({ display: 'block' });
    $('#div3TierInspectionList_Road_tab3').hide('slow');
    view_ATR_Details();
}

function view_ATR_Details() {
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/ATRDetails',
        type: 'POST',
        data: $('#3TierATRFilterForm').serialize(),
        success: function (response) {
            $("#div3TierATRDetailsHtml").html('');
            $("#div3TierATRDetailsHtml").html(response);
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);

        }
    });
}

//--------------- Tour Claim ------------------

function AddTourSanctionAmount(scheduleCode) {

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Add Tour Sanction Amount</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/TourClaim/AddSanctionedAmount/?scheduleCode=' + scheduleCode, function () {
            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $("#divScheduleDetails").css('height', 'auto');
    $('#divScheduleDetails').show('slow');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

function FinalizeTourDetailsCqc(id) {

    const myArray = id.split("$");
    var adminScheduleCode = myArray[0];
    var month = myArray[1];
    var year = myArray[2];

    if ($('#finalizeFlag').val() == 1 && $('#ROUND_SEQUENCE').val() == 1)
        adminScheduleCode = adminScheduleCode + "$FrwdRem$" + $('#remarkForward').val() + "$P3Name$" + $('#officeAssistant').val();
    else if ($('#finalizeFlag').val() == 1)
        adminScheduleCode = adminScheduleCode + "$P3Name$" + $('#officeAssistant').val();

    var monthNum = getMonthFromString(month);

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', adminScheduleCode);

    if ($('#officeAssistant').val() == "")
        alert("Please enter Office Assistant (P-III)");
    else if (confirm("Do you want to finalize tour sanctioned details ?")) {
        $.ajax({
            url: "/TourClaim/FinalizeSanctionTourDetails",
            type: "POST",
            async: false,
            cache: false,
            data: { adminScheduleCode: adminScheduleCode },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    CloseScheduleDetails();
                    ScheduleListGrid(monthNum, year);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else
        return false;
}

function ApproveTourDetailsCqc(id) {

    const myArray = id.split("$");
    var adminScheduleCode = myArray[0] + "$" + $('#remarkApprove').val() + "$" + $('#director').val();
    var month = myArray[1];
    var year = myArray[2];

    var monthNum = getMonthFromString(month);

    jQuery('#tb3TierScheduleList').jqGrid('setSelection', adminScheduleCode);

    if (confirm("Do you want to approve tour sanctioned details ?")) {
        $.ajax({
            url: "/TourClaim/ApproveTourDetails",
            type: "POST",
            async: false,
            cache: false,
            data: { adminScheduleCode: adminScheduleCode },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    CloseScheduleDetails();
                    ScheduleListGrid(monthNum, year);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();

            }
        });
    }
    else
        return false;
}

function getMonthFromString(mon) {
    return new Date(Date.parse(mon + " 1, 2022")).getMonth() + 1
}

function ViewPreviewDetailsCQC(scheduleCode) {

    $("#accordionSchedule div").html("");
    $("#accordionSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Preview Report</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionSchedule').show('slow', function () {
        blockPage();
        $("#divScheduleDetails").load('/TourClaim/ViewSummaryDetailsReportCQC/?scheduleCode=' + scheduleCode, function () {
            unblockPage();
        });
    });

    $("#divScheduleDetails").css('height', 'auto');
    $('#divScheduleDetails').show('slow');

    $("#tb3TierScheduleList").jqGrid('setGridState', 'hidden');
}

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






// ---------------------------------------SQC 2 TIER ATR DETAILS----------------------------------
function SQC2TierATRDetailsGrid() {
    CloseATR2TierSQCDetails();
    blockPage();
    $("#div2TierATRQualityFiltersHtml").load('/QualityMonitoring/Quality2TierATRFilters', function () {
        $.validator.unobtrusive.parse($('#2TierATRFilterForm'));
        unblockPage();
    });
    $('#div2TierATRQualityFiltersHtml').show('slow');
}






function show2TierSQCFilter() {
    if ($('#div3TierFilterForm').is(":hidden")) {
        $("#div3TierFilterForm").show("slow");
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function toggleSQCATRDetails() {
    $("#spn3TierATRHtml").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $('.dataTable').hide("show");
    $('#div2TierATRDetailsHtml').hide("show");
}

function Close2TierSQCInspectionDetails() {
    $('#accordionInspection').hide('slow');
    $('#divInspectionDetails').hide('slow');
    $("#tb3TierInspectionList").jqGrid('setGridState', 'visible');

    show2TierSQCFilter();
}


function CloseATR2TierSQCDetails() {
    $('#accordionATR2TierSQC').hide('slow');
    $('#divATR2TierSQCDetails').hide('slow');
    $('.dataTable').show("show");
    $('#div2TierATRDetailsHtml').show("show");
}


function Regrade2TierATR(obsId) {
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {
        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/QM2TierATRRegrade/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');
    toggleSQCATRDetails();
}

function regrade2TierATRAsUploaded(obsId) {
    if (confirm("Are you sure to update ATR?")) {
        $.ajax({
            url: '/QualityMonitoringHelpDesk/Update2TierATRStatus/',
            type: 'POST',
            data: { obsId: obsId, value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("ATR updated successfully");
                    viewSQCATRDetails();
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

function Show2TierLabDetails(id) {
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:1.2em;' >Lab Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {
        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/LabImageUpload/' + id, function () {
            unblockPage();
            $("#dvFiles #gbox_tbFilesList").css("margin-left", "20%")
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');
    $("#dvFiles #gbox_tbFilesList").css("margin-left", "20%")
    toggleSQCATRDetails();
}


function Show2TierATRObsDetails(obsId) {
    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);
    $("#div3TierATRQualityActionReport").hide();
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {
        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/false', function () {
            unblockPage();
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');

    toggleSQCATRDetails();
}

function ShowATRGradingDetails(obsId) {

    jQuery('#tb3TierATRList').jqGrid('setSelection', obsId);
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >Regrade Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {
        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/QMATRAccpetedObsDetails/' + obsId + '/true', function () {
            unblockPage();
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');

    toggleSQCATRDetails();
}

function ATR_AlreadyFinilized() {
    alert("Atr is already finalized");
}

function noInspAvailableAlert() {
    alert("No satisfactory inspection is available for ATR mark");
}



function ViewUploadedRejectedATR2TierFile(obsId) {
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >ATR Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {

        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/viewPdfFileUpload2TierATR/' + obsId, function (response) {
            if (response != "") {
                unblockPage();
                return false;
            }
            ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier($(IMS_PR_ROAD_CODE).val() + "$" + $(QM_INSPECTION_DATE).val() + "$" + $(QM_OBSERVATION_ID).val() + "$" + $(QM_ATR_ID).val());
            List2TierPDFFiles($(QM_OBSERVATION_ID).val());
            Show2TierSQCATRSelectedObsDetails($("#selectedSgradeObsId").val());
            unblockPage();
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');
    toggleSQCATRDetails();

}

function uploadFromRejectedButtonAlert() {
    alert("Please upload atr file from rejected button");
}

function ViewUploadedATR2TierSQCFile(obsId) {
    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >ATR Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {

        blockPage();

        $("#divATR2TierSQCDetails").load('/QualityMonitoring/viewPdfFileUpload2TierATR/' + obsId, function (response) {
            if (response != "") {
                unblockPage();
                return false;
            }

            ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier($(IMS_PR_ROAD_CODE).val() + "$" + $(QM_INSPECTION_DATE).val() + "$" + $(QM_OBSERVATION_ID).val() + "$" + $(QM_ATR_ID).val());
            List2TierPDFFiles($(QM_OBSERVATION_ID).val());
            Show2TierSQCATRSelectedObsDetails($("#selectedSgradeObsId").val());
            unblockPage();
        });
    });

    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');
    toggleSQCATRDetails();

}



function ViewInspectionReport2TierATR(obsId) {   //Added by Aditi on 25 Jan 2021

    Close2TierSQCInspectionDetails();
    $("#accordionATR2TierSQC div").html("");
    $("#accordionATR2TierSQC h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Report Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR2TierSQCDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR2TierSQC').show('slow', function () {
        blockPage();
        $("#divATR2TierSQCDetails").load('/QualityMonitoring/InspPdfFileUpload?id=' + obsId, function () {

            ListPDFFiles($(QM_OBSERVATION_ID).val());
            unblockPage();
        });
    });
    $("#divATR2TierSQCDetails").css('height', 'auto');
    $('#divATR2TierSQCDetails').show('slow');

    toggleSQCATRDetails();
}



function Show2TierInspAgainstRoad(roadCode1) {

    window.open("/QualityMonitoring/QMView2TierInspectionListAgainstRoadNewTab?roadCode1=" + roadCode1).focus();

}





//END HERE SQC 2 TIER ATR DETAILS