$(document).ready(function () {
    //  $.validator.unobtrusive.parse($('#frmSearchCluster'));   
    $(function () {
        $("#accordionATR3TierCqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $("#id3TierATRFilterDiv").click(function () {
        $("#id3TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierATRFilterForm").toggle("slow");

    });
    $("#spn3TierATRHtmlList").click(function () {
        $("#spn3TierATRHtmlList").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierATRDetailsHtml").toggle("slow");

    });
    selectedNameVal = 0;
    $("#stateCodeATR").change(function () {

        $("#monitorCodeATR").empty();

        if ($(this).val() == 0) {
            $("#monitorCodeATR").append("<option value='0'>All Monitors</option>");
        }

        if ($("#stateCodeATR").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCodeATR").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#monitorCodeATR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });

    viewATRDetails();
    //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());

    $('#btn3TierListATRDetails').click(function () {
        viewATRDetails();
       // ATRListGrid($("#MAST_STATE_CODE").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());
    });

    $('#btnBulkRegrade').click(function () {
        ShowBulkATRDetails();
        $("#id3TierATRFilterDiv").trigger('click');
    });


});//doc.ready ends here

function viewATRDetails() {
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/QmAtrOaHtmlList',
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
            if ($("#hdnRoleCodeOnSqcLayout3Tier").val() == 8) {
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


function DownloadATR(atrId) {

    url = "/QualityMonitoring/DownloadFile/" + atrId,

    //window.location = url;


    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("File not available.");
                return false;
            }
            else {
                window.location = url;
            }
        }
    });

    //window.location = paramurl;
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

function toggleATRDetails() {
    $("#spn3TierATRHtml").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#div3TierATRDetailsHtml").toggle("slow");
}

function closeDivError() {
    $('#divError').hide('slow');
}

function Close3TierCqcScheduleDetails() {
    $('#accordion3TierCqcSchedule').hide('slow');
    $('#div3TierCqcScheduleDetails').hide('slow');
    $("#tb3TierCqcScheduleList").jqGrid('setGridState', 'visible');
}


function Close3TierSqcScheduleDetails() {
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


function Show3TierSqcScheduleListGrid() {
    Sqc3TierScheduleListGrid();
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

/* ATR Functions Ends Here*/
