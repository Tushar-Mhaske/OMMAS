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

    //$("#tabs-3TierDetails").tabs();
    //$('#tabs-3TierDetails ul').removeClass('ui-widget-header');


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
});
  
    function closeDivError() {
        $('#divError').hide('slow');
    }


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

function ShowInspectionDetails() {
    blockPage();
    $("#div3TierInspectionQualityFilters").load('/Master/QualityFilters', function () {
        $.validator.unobtrusive.parse($('#frmQualityFilters'));
        unblockPage();
    });
    $('#div3TierInspectionQualityFilters').show('slow');
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


function toggleATRDetails()
{
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

//--------------------------------------------------------------------------------------------------------


function InspectionListGrid(stateCode, monitorCode, fromInspMonth, fromInspYear, toInspMonth, toInspYear, qmType) {

    $("#tb3TierInspectionList").jqGrid('GridUnload');

    jQuery("#tb3TierInspectionList").jqGrid({
        url: '/Master/QMViewInspectionDetails?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                    "Start Chainage (Km.)", "End Chainage (Km.)", "Inspection Date", "Road Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Images Uploaded",
                    "Uploaded by", "View Details", "Grade Correction", "Upload Image", "Delete"],
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
                            { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
                            { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
                            { name: 'IsEnquiry', index: 'IsEnquiry', width: 50, sortable: false, align: "center", search: false },
                            { name: 'Scheme', index: 'Scheme', width: 50, sortable: false, align: "center", search: false },
                            { name: 'OverallGrade', index: 'OverallGrade', width: 60, sortable: false, align: "center", search: false },
                            { name: 'NoOfImagesUploaded', index: 'NoOfImagesUploaded', width: 40, sortable: false, align: "center", search: false },
                            { name: 'UploadBy', index: 'UploadBy', width: 50, sortable: false, align: "center", search: false },
                            { name: 'View', index: 'View', width: 30, sortable: false, align: "center", search: false },
                            { name: 'Correction', index: 'Correction', width: 30, sortable: false, align: "center", search: false },
                            { name: 'UploadImage', index: 'UploadImage', width: 30, sortable: false, align: "center", search: false },
                            { name: 'Delete', index: 'Delete', width: 30, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "monitorCode": monitorCode, "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear, 'qmType': qmType },
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
            $('#tb3TierInspectionList').jqGrid('hideCol', 'Correction');
            $('#tb3TierInspectionList').jqGrid('hideCol', 'UploadImage');
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
}//Inspection List Ends Here




/// -------------------- Grid Column Click Functions -----------------------------------


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


