/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayout2TierCQC.js
        * Description   :   Handles events for in Quality Layout for 2 Tier in cqc login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/


$(document).ready(function () {

    $("#tabs-3TierDetails").tabs();
    $('#tabs-3TierDetails ul').removeClass('ui-widget-header');

    $(function () {
        $("#accordionInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionSchedule").accordion({
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

    function closeDivError() {
        $('#divError').hide('slow');
    }

    $("#id3TierFilterDiv").click(function () {

        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierFilterForm").toggle("slow");

    });

    $("#btn3Tier").click(function () {
        window.location = "/QualityMonitoring/QualityLayout";
    });

    $("#btn2Tier").click(function () {
        window.location = "/QualityMonitoring/QualityLayout2TierCQC";
    });

    InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#ROAD_STATUS").val(), $("#schemeType").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());   //add on 07-07-2022 by vikky 

});



function CloseInspectionDetails() {
    $('#accordionInspection').hide('slow');
    $('#divInspectionDetails').hide('slow');
    $("#tb3TierInspectionList").jqGrid('setGridState', 'visible');

    showFilter();
}


function CloseScheduleDetails() {
    $('#accordionSchedule').hide('slow');
    $('#divScheduleDetails').hide('slow');
    $("#tb2TierScheduleList").jqGrid('setGridState', 'visible');
}

function showFilter() {
    if ($('#div3TierFilterForm').is(":hidden")) {
        $("#div3TierFilterForm").show("slow");
        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

// Added by deendayal
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


// replace function in script-> QualityLayout2TierCQC 
function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, ROAD_STATUS, schemeType, roadOrBridge, gradeType, eFormStatus) {    //add on 07-07-2022 by vikky 

    $("#tb3TierInspectionList").jqGrid('GridUnload');

    jQuery("#tb3TierInspectionList").jqGrid({
        url: '/QualityMonitoring/QMViewInspectionDetails2TierCQC?' + Math.random(),
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
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, "ROAD_STATUS": ROAD_STATUS, "schemeType": schemeType, "roadOrBridge": roadOrBridge, "gradeType": gradeType, "eFormStatus": eFormStatus },  //add on 07-07-2022 by vikky 
        pager: jQuery('#dv3TierInspectionListPager'),
        rowNum: 20000,
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Inspection List",
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
            //$("#gview_tb3TierInspectionList > .ui-jqgrid-titlebar").hide();
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

    //jQuery("#tb3TierInspectionList").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [{ startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: 'Chainage (in Km)' }]
    //});

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



//2 Tier Schedule List for CQC
function ScheduleListGrid(state, month, year) {
    debugger;
    $("#tb2TierScheduleList").jqGrid('GridUnload');

    jQuery("#tb2TierScheduleList").jqGrid({
        url: '/QualityMonitoring/Get2TierScheduleListCQC?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "Month & Year of visit", "State", "District 1", "District 2", "District 3", "Inspection Status", "View"],
        colModel: [
            { name: 'Monitor', index: 'Monitor', width: 120, sortable: false, align: "center" },
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 110, sortable: false, align: "center", search: false },
            { name: 'State', index: 'State', width: 90, sortable: false, align: "left", search: false },
            { name: 'District1', index: 'District1', width: 90, sortable: false, align: "left", search: false },
            { name: 'District2', index: 'District2', width: 90, sortable: false, align: "left", search: false },
            { name: 'District3', index: 'District3', width: 90, sortable: false, align: "left", search: false },
            { name: 'InspStatus', index: 'InspStatus', width: 80, sortable: false, align: "center", search: false },
            { name: 'View', index: 'View', width: 70, sortable: false, align: "center", search: false },

        ],
        postData: { state: state, month: month, year: year },
        pager: jQuery('#dv2TierScheduleListPager'),
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
            $("#tb2TierScheduleList #dv2TierScheduleListPager").css({ height: '35px' });
            $("#dv2TierScheduleListPager_left").html("<input type='button' style='margin-left:5px' id='idCreateSchedule' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'ShowCreateSchedule() ;return false;' value='Create New'/>");
            //$("#tb3TierScheduleList #dv3TierScheduleListPager").css({ height: '35px' });
            //$('#tb3TierScheduleList').setGridWidth(($('#div3TierSchedulePreparation').width() - 10), true);
            //alert("test 2");
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

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionInspection').show('slow', function () {
        blockPage();
        $("#divInspectionDetails").load('/QualityMonitoring/QMObservationDetails2TierCQC/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divInspectionDetails").css('height', 'auto');
    $('#divInspectionDetails').show('slow');

    $("#tb3TierInspectionList").jqGrid('setGridState', 'hidden');
    $('#id3TierFilterDiv').trigger('click');
}


function ViewDetails(scheduleCode) {
    jQuery('#tb2TierScheduleList').jqGrid('setSelection', scheduleCode);

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

    $("#tb2TierScheduleList").jqGrid('setGridState', 'hidden');
}

//function to be added in script QualityLayout2TierCQC
//sachin added on 20aug 2020
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



//added by abhinav pathak on 13-aug-2019
function ShowInspReportFilePDF(obsId, prRoadCode) {

    jQuery('#tb3TierInspectionList').jqGrid('setSelection', obsId);

    $("#accordionInspection div").html("");
    $("#accordionInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

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
    //  alert("Test New new");
    blockPage();
    $("#div3TierAddMonitorInsterState").load('/Master/MasterQualityMonitorForMapping', function () {

        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_ADDRESS1');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PIN');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PHONE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_MOBILE2');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_REMARKS');
        $('#tblQualityMonitorList').jqGrid('hideCol', 'ADMIN_QM_PAN');

        unblockPage();
    });
    $('#div3TierAddMonitorInsterState').show('slow');

}

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
    $("#tb2TierScheduleList").jqGrid('setGridState', 'hidden');
}

