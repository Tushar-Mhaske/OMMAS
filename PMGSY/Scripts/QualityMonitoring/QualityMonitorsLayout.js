/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityMonitorsLayout.js
        * Description   :   Handles events for Quality Layout for Monitors
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
    //$.validator.unobtrusive.parse($('#3TierFilterForm'));

    $.ajaxSetup({ cache: false });

    $("#tabs-MonitorLayout").tabs();
    $('#tabs-MonitorLayout ul').removeClass('ui-widget-header');
    //$("#tabs-MonitorLayout .ui-widget-header").css("background-color", "#000");


    $(function () {
        $("#accordionMonitorsInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionMonitorsSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionObsMonitors").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionBankDetails").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //Tour Claim
    $(function () {
        $("#accordionNQMSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //showMonitorsInspectionDetails();
    InspectionListGrid($("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());

});//doc.ready ends here


//-----------------------------Base Function of Tab Click---------------------------------------------------
function closeDivError() {
    $('#divError').hide('slow');
}

function closeMonitorsInspectionDetails() {
    //alert("closeMonitorsInspectionDetails");
    $('#accordionMonitorsInspection').hide('slow');
    $('#divMonitorsInspCorrectionDetails').hide('slow');
    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'visible');
}


function closeMonitorsScheduleDetails() {
    //alert("closeMonitorsScheduleDetails");
    $('#accordionMonitorsSchedule').hide('slow');
    $('#divMonitorsScheduleDetails').hide('slow');
    $("#tbMonitorsScheduleList").jqGrid('setGridState', 'visible');
}


function closeMonitorsObsDetails() {
    //alert("closeMonitorsObsDetails");
    $('#accordionObsMonitors').hide('slow');
    $('#divMonitorsObservationDetails').hide('slow');
    $("#tbMonitorsObsList").jqGrid('setGridState', 'visible');
}

function closeBankDetailsList() {
    //alert("closeBankDetailsList");
    $('#accordionBankDetails').hide('slow');
    $('#divBankDetails').hide('slow');
    $("#tbMonitorsObsList").jqGrid('setGridState', 'visible');
}

//Tour Claim
function closeTourClaimList() {
    $('#accordionNQMSchedule').hide('slow');
    $('#divTourClaimList').hide('slow');
}

function showMonitorsInspectionDetails() {
    closeBankDetailsList();
    closeMonitorsObsDetails();
    InspectionListGrid($("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());
}



function showMonitorsObservationList() {
    closeBankDetailsList();
    closeMonitorsInspectionDetails();
    blockPage();
    $("#divMonitorsObsDetails").load('/QualityMonitoring/QualityMonitorsObsDetails', function () {
        $.validator.unobtrusive.parse($('#frmQualityFilters'));
        unblockPage();
    });
    $('#divMonitorsObsDetails').show('slow');
}

function showBankDetailsList() {
    closeMonitorsInspectionDetails();
    closeMonitorsObsDetails();
    closeMonitorsScheduleDetails();

    $('#divMonitorsObsDetails').html('');
    blockPage();
    $("#divBankDetails").load('/QualityMonitoring/QMListBankDetails', function () {
        //$.validator.unobtrusive.parse($('#frmQualityFilters'));
        unblockPage();
    });
    $('#accordionBankDetails').show('slow');
    $('#divBankDetails').show('slow');
}

//Tour Claim
function showTourClaimList() {
    closeMonitorsInspectionDetails();
    closeMonitorsObsDetails();
    closeMonitorsScheduleDetails();
    closeBankDetailsList();

    blockPage();
    $("#divNqmScheduleList").load('/TourClaim/ScheduleFiltersNQM', function () {
        unblockPage();
    });
    /*$('#accordionTourClaim').show('slow');*/
    $('#divNqmScheduleList').show('slow');
}

//-----------------------------Base Function of Tab Ends---------------------------------------------------



function showMonitorsAssignScheduleListGrid(month, year) {
    closeBankDetailsList();
    $("#tbMonitorsScheduleList").jqGrid('GridUnload');

    jQuery("#tbMonitorsScheduleList").jqGrid({
        url: '/QualityMonitoring/GetMonitorsCurrScheduleList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Month & Year of visit", "State", "District 1", "District 2", "District 3", "Inspection Status", "Add Road", "Tour Details", "View"],
        colModel: [
            { name: 'MonthYearOfVisit', index: 'MonthYearOfVisit', width: 250, sortable: false, align: "center", search: false },
            { name: 'State', index: 'State', width: 150, sortable: false, align: "left", search: false },
            { name: 'District1', index: 'District1', width: 130, sortable: false, align: "left", search: false },
            { name: 'District2', index: 'District2', width: 130, sortable: false, align: "left", search: false },
            { name: 'District3', index: 'District3', width: 130, sortable: false, align: "left", search: false },
            { name: 'InspStatus', index: 'InspStatus', width: 120, sortable: false, align: "center", search: false },
            { name: 'AddRoad', index: 'AddRoad', width: 100, sortable: false, align: "center", search: false },
            { name: 'UpdateTourDetails', index: 'UpdateTourDetails', width: 100, sortable: false, align: "center", search: false },
            { name: 'View', index: 'View', width: 100, sortable: false, align: "center", search: false }
        ],
        postData: { "month": month, "year": year },
        pager: jQuery('#dvMonitorsScheduleListPager'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Schedule List",
        height: 'auto',
        autowidth: true,
        sortname: 'Monitor',
        rowList: [5, 10, 15],
        loadComplete: function () {
            if ($("#hdnRoleCodeOnMonitorLayout").val() == 7)  //for SQM
            {
                $('#tbMonitorsScheduleList').jqGrid('hideCol', 'District2');
                $('#tbMonitorsScheduleList').jqGrid('hideCol', 'District3');

                $('#tbMonitorsScheduleList').setGridWidth(($('#divMonitorsSchedulePreparation').width() - 10), true);
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


//replace method ->InspectionListGrid

function InspectionListGrid(fromInspMonth, fromInspYear, toInspMonth, toInspYear) {

    fromInspMonth = parseInt(fromInspMonth);
    fromInspYear = parseInt(fromInspYear);
    toInspMonth = parseInt(toInspMonth);
    toInspYear = parseInt(toInspYear);

    var date = new Date();

    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    if ((fromInspYear > toInspYear) || (fromInspYear == toInspYear && fromInspMonth > toInspMonth)) {
        alert("From-Month or From-Year must be less than To-Month or To-Year");
    }
    else if ((toInspYear == year && toInspMonth > month) || (fromInspYear == year && fromInspMonth > month)) {
        alert("Selected Month or Year cannot be greater than Current Month or Year");
    }
    else {


        $("#tbMonitorsInspectionList").jqGrid('GridUnload');

        jQuery("#tbMonitorsInspectionList").jqGrid({
            url: '/QualityMonitoring/QMMonitorInspList?' + Math.random(),
            datatype: "json",
            mtype: "POST",
            colNames: ["State", "District", "Block", "Package", "Sanction Year", "Road Name", "Type",
                "From Chainage (Km)", "To Chainage (Km)", "Schedule Date", "Inspection Date", "Upload Date", "Work Status", "Enquiry Inspection", "Scheme", "Overall Grade", "Uploaded By", "Images Uploaded",
                "Upload Image", "Upload Work Inspection Report", "View Details", 'Tree Plant', "Upload Multiple PDF(Optional)"],
            colModel: [
                { name: 'State', index: 'State', width: 60, sortable: false, align: "center", search: false },
                { name: 'District', index: 'District', width: 35, sortable: false, align: "center", search: false },
                { name: 'Block', index: 'Block', width: 30, sortable: false, align: "left", search: false },
                { name: 'Package', index: 'Package', width: 25, sortable: false, align: "left", search: false },
                { name: 'SanctionYear', index: 'SanctionYear', width: 25, sortable: false, align: "center", search: false },
                { name: 'RoadName', index: 'RoadName', width: 70, sortable: false, align: "left", search: false },
                { name: 'PropType', index: 'PropType', width: 30, sortable: false, align: "left", search: false },
                { name: 'InspFrmChainage', index: 'InspFrmChainage', width: 20, sortable: false, align: "center", search: false },
                { name: 'InspToChainage', index: 'InspToChainage', width: 20, sortable: false, align: "center", search: false },
                { name: 'SCHEDULE_DATE', index: 'SCHEDULE_DATE', width: 60, sortable: false, align: "center", search: false },
                { name: 'InspDate', index: 'InspDate', width: 30, sortable: false, align: "center", search: false },
                { name: 'UploadDate', index: 'UploadDate', width: 30, sortable: false, align: "center", search: false },
                { name: 'RdStatus', index: 'RdStatus', width: 30, sortable: false, align: "center", search: false },
                { name: 'IsEnquiry', index: 'IsEnquiry', width: 30, sortable: false, align: "center", search: false },
                { name: 'Scheme', index: 'Scheme', width: 30, sortable: false, align: "center", search: false },
                { name: 'OverallGrade', index: 'OverallGrade', width: 30, sortable: false, align: "center", search: false },
                { name: 'UploadBy', index: 'UploadBy', width: 30, sortable: false, align: "center", search: false, hidden: true },
                { name: 'NoOfImagesUploaded', index: 'NoOfImagesUploaded', width: 25, sortable: false, align: "center", search: false },
                { name: 'UploadImage', index: 'UploadImage', width: 25, sortable: false, align: "center", search: false },
                { name: 'UploadReport', index: 'UploadReport', width: 25, sortable: false, align: "center", search: false },
                { name: 'View', index: 'View', width: 25, sortable: false, align: "center", search: false },
                { name: 'Tree Plant', width: 25, sortable: false, formatter: FormatColumnTreePlantation, resize: false, align: "center" },
                { name: 'UploadPDF', index: 'UploadPDF', width: 30, sortable: false, align: "center", search: false },
            ],
            postData: { "fromInspMonth": fromInspMonth, "fromInspYear": fromInspYear, "toInspMonth": toInspMonth, "toInspYear": toInspYear },
            pager: jQuery('#dvMonitorsinspectionListPager'),
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
                $('#tbMonitorsInspectionList').setGridWidth(($('#divMonitorsInspectionDetails').width() - 10), true);
                $("#tbMonitorsInspectionList #dvMonitorsinspectionListPager").css({ height: '40px' });
                $("#dvMonitorsinspectionListPager_left").html("<label style='margin-left:1%;'><b>Note: </b>For Missing Images, go to 'Upload Images' click on 'Upload Missing Images' against the respective Image.<label/>")
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

        //jQuery("#tbMonitorsInspectionList").jqGrid('setGroupHeaders', {
        //    useColSpanStyle: true,
        //    groupHeaders: [{ startColumnName: 'InspFrmChainage', numberOfColumns: 2, titleText: 'Chainage (in Km)' }]
        //});

    }
}






//Displays Individual Monitor Data
function QMAssignRoads(scheduleCode) {

    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Assign Roads</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionMonitorsSchedule').show('slow', function () {
        blockPage();
        $("#divMonitorsScheduleDetails").load('/QualityMonitoring/QMAssignRoads/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divMonitorsScheduleDetails').show('slow');
    $("#divMonitorsScheduleDetails").css('height', 'auto');


    //$("#tbMonitorsScheduleList").jqGrid('setGridState', 'hidden');
}


function ViewSchDetails(scheduleCode) {
    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Districtwise Schedule Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsSchedule').show('slow', function () {
        blockPage();
        $("#divMonitorsScheduleDetails").load('/QualityMonitoring/QMDistrictwiseSchDetails/' + scheduleCode + "$" + number, function () {
            unblockPage();
        });
    });

    $("#divMonitorsScheduleDetails").css('height', 'auto');
    $('#divMonitorsScheduleDetails').show('slow');
}





function QMFillObservations(scheduleCode, prRoadCode, roadStatus) {
    //alert("innnn" + roadStatus)
    //alert(scheduleCode);
    jQuery('#tbMonitorsObsList').jqGrid('setSelection', prRoadCode);

    $("#accordionObsMonitors div").html("");
    $("#accordionObsMonitors h3").html(
        "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsObsDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionObsMonitors').show('slow', function () {
        blockPage();
        //$("#divMonitorsObservationDetails").load('/QualityMonitoring/QMFillObservations/' + scheduleCode + "/" + prRoadCode + "/" + roadStatus , function () {
        //$("#divMonitorsObservationDetails").load('/QualityMonitoring/QMFillObservations?id1=' + scheduleCode + '&id2' + prRoadCode + '&id3' + roadStatus, function () {
        $("#divMonitorsObservationDetails").load('/QualityMonitoring/QMFillObservations/?data=' + scheduleCode + "$" + prRoadCode + "$" + roadStatus, function () {
            unblockPage();
        });
    });

    $("#divMonitorsObservationDetails").css('height', 'auto');
    $('#divMonitorsObservationDetails').show('slow');

    $("#tbMonitorsObsList").jqGrid('setGridState', 'hidden');


    //$("#idObsDetailsNoteDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    //$("#divlFillObsDetailsFilters").toggle("slow");
}

function UploadMonitorsFile(obsId, scheduleCode, prRoadCode) {

    var random = Math.random();

    jQuery('#tbMonitorsInspectionList').jqGrid('setSelection', obsId);

    $("#accordionObsMonitors div").html("");

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divMonitorsInspCorrectionDetails").load('/QualityMonitoring/ImageUpload/' + scheduleCode + "$" + prRoadCode + "$" + obsId, function () {
            unblockPage();
        });
    });

    $("#divMonitorsInspCorrectionDetails").css('height', 'auto');
    $('#divMonitorsInspCorrectionDetails').show('slow');

    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'hidden');
}


function ShowObservationDetails(obsId) {

    jQuery('#tbMonitorsInspectionList').jqGrid('setSelection', obsId);

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Inspection Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    // details /delete
    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divMonitorsInspCorrectionDetails").load('/QualityMonitoring/QMObservationDetails/' + obsId, function () {

        });
        unblockPage();
    });

    $("#divMonitorsInspCorrectionDetails").css('height', 'auto');
    $('#divMonitorsInspCorrectionDetails').show('slow');

    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'hidden');
    //$("#3TierFilterForm").hide();
}

function ShowInspReportFile(obsId) {
    var random = Math.random();

    jQuery('#tbMonitorsInspectionList').jqGrid('setSelection', obsId);

    $("#accordionObsMonitors div").html("");

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divMonitorsInspCorrectionDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divMonitorsInspCorrectionDetails").css('height', 'auto');
    $('#divMonitorsInspCorrectionDetails').show('slow');

    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'hidden');
}

//Displays Individual Monitor Data
function QMViewTourDetails(scheduleCode) {

    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Tour Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionMonitorsSchedule').show('slow', function () {
        blockPage();
        $("#divMonitorsScheduleDetails").load('/QualityMonitoring/QMTourDetails/' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divMonitorsScheduleDetails').show('slow');
    $("#divMonitorsScheduleDetails").css('height', 'auto');
}


function FormatColumnTreePlantation(cellvalue, options, rowObject) {
    //alert('AddFinancialProgressDetails(' + cellvalue.toString() + ');');;
    return "<span class='ui-icon ui-icon-plusthick ui-align-center' title='Tree Plant Details' onClick ='AddTreePlantationDetails(\"" + cellvalue.toString() + "\");'></span>";
}

$(function () {
    $("#accordionTreePlant").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });
});

function AddTreePlantationDetails(id) {

    $.ajax({
        url: "/TreePlant/VerifyIndex/" + id,
        type: "GET",
        async: false,
        cache: false,
        dataType: 'html',
        success: function (data) {
            //alert("Success" + id);
            $("#accordionTreePlant h3").html(
                "<a href='#' style= 'font-size:.9em;' >Add Tree Plant Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img class="ui-icon ui-icon-closethick" onclick="CloseTreePlantationDetails();" /></a>'
            );
            $("#dvAddTreePlant").html(data);
            $('#accordionTreePlant').show('slow');
            $('#dvAddTreePlant').show('slow');
            $('#tbMonitorsInspectionList').jqGrid("setGridState", "hidden");

            $("#tblTreePlant").css({ 'margin-left': '12%' });


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error occurred while processing your request.');
        }
    });
}

function CloseTreePlantationDetails() {
    $('#accordionTreePlant').hide('slow');



    $('#tbMonitorsInspectionList').jqGrid("setGridState", "visible");

    $("#dvAgreement").animate({
        scrollTop: 0
    });

}

//added by abhinav pathak 0n 13-aug-2019

function ShowInspReportFilePDF(obsId, prRoadCode) {
    var random = Math.random();

    jQuery('#tbMonitorsInspectionList').jqGrid('setSelection', obsId);

    $("#accordionObsMonitors div").html("");

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divMonitorsInspCorrectionDetails").load('/QualityMonitoring/PdfFileUploadView/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divMonitorsInspCorrectionDetails").css('height', 'auto');
    $('#divMonitorsInspCorrectionDetails').show('slow');

    $("#tbMonitorsInspectionList").jqGrid('setGridState', 'hidden');
}

// ----------------------------- TOUR CLAIM ----------------------------

function AddTourDetails(scheduleCode) {
   
    $("#divTourClaimList").hide();

    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionMonitorsSchedule').show('slow', function () {
        blockPage();
        $("#divMonitorsScheduleDetails").load('/TourClaim/ViewNQMDEtails/' + scheduleCode, function () {
            $('.tab').hide();
            $('#districtDiv').hide();
            unblockPage();
        });
    });

    $('#divMonitorsScheduleDetails').show('slow');
    $("#divMonitorsScheduleDetails").css('height', 'auto');
}

function LoadTourClaimGrid(adminScheduleCode) {
    $("#accordionMonitorsSchedule").hide();
    $('#divTourClaimList').trigger('reloadGrid');
    $("#divTourClaimList").show();
    LoadTourClaimList(adminScheduleCode);
}

function EditDetails(tourClaimId) {
    
    $("#divTourClaimList").hide();

    $("#accordionMonitorsSchedule div").html("");
    $("#accordionMonitorsSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Edit Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsScheduleDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionMonitorsSchedule').show('slow', function () {

        blockPage();
        $("#divMonitorsScheduleDetails").load('/TourClaim/EditTourDetails/?tourClaimId=' + tourClaimId, function () {
            $('.tab').hide();
            $('#districtDiv').hide();
            unblockPage();
        });
    });

    $('#divMonitorsScheduleDetails').show('slow');
    $("#divMonitorsScheduleDetails").css('height', 'auto');
}

function AddAlert() {
    alert("Bank details not added. Please add bank details for further processing.");
}

function AddNewTourDetails(scheduleCode) {
   
    $("#divTourClaimList").hide();

    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionNQMSchedule div").html("");
    $("#accordionNQMSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionNQMSchedule').show('slow', function () {
        blockPage();

        $("#divNQMScheduleDetails").load('/TourClaim/AddNewTourDetails/' + scheduleCode, function () {

            $('#viewDetailsofNQM').hide();
            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $('#divNQMScheduleDetails').show('slow');
    $("#divNQMScheduleDetails").css('height', 'auto');
}

function AddAllTourDetails(scheduleCode) {
   
    $("#divTourClaimList").hide();

    jQuery('#tbMonitorsScheduleList').jqGrid('setSelection', scheduleCode);

    $("#accordionNQMSchedule div").html("");
    $("#accordionNQMSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionNQMSchedule').show('slow', function () {
        blockPage();

        $("#divNQMScheduleDetails").load('/TourClaim/ViewNQMDEtails/' + scheduleCode, function () {

            $('#viewDetailsofNQM').hide();
            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $('#divNQMScheduleDetails').show('slow');
    $("#divNQMScheduleDetails").css('height', 'auto');
}

function ViewPreviewDetails(scheduleCode) {

    $("#accordionNQMSchedule div").html("");
    $("#accordionNQMSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Preview Report</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionNQMSchedule').show('slow', function () {
        blockPage();

        $("#divNQMScheduleDetails").load('/TourClaim/ViewSummaryDetailsReport/?scheduleCode=' + scheduleCode, function () {
            unblockPage();
        });
    });

    $('#divNQMScheduleDetails').show('slow');
    $("#divNQMScheduleDetails").css('height', 'auto');
}

function ViewTourDetails(scheduleCode) {

    $("#accordionNQMSchedule div").html("");
    $("#accordionNQMSchedule h3").html(
        "<a href='#' style= 'font-size:.9em;' >Tour Claim Details</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeTourClaimList();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionNQMSchedule').show('slow', function () {
        blockPage();
        $("#divNQMScheduleDetails").load('/TourClaim/ViewAllDetails/?scheduleCode=' + scheduleCode, function () {
            document.getElementById("defaultOpen").click();
            unblockPage();
        });
    });

    $('#divNQMScheduleDetails').show('slow');
    $("#divNQMScheduleDetails").css('height', 'auto');
}
