var pageWidth = 1680;
$(document).ready(function () {

    $("#tabs").tabs();
    LoadInprogressRequests();
    LoadCompletedRequests();
    LoadActionRequiredRequests();
    LoadListForUONumberRequests();
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#draggable").load('/OnlineFund/ViewRequestStatusDetails');

    setTimeout(function () {

        $("#draggable").css({
            "position": "absolute",
            "width": "600px",
            "height": "300px",
            "top": "500px",
            "left": "650px",
            //"border": "1px solid black"
        });
    }, 500);

});
 
function LoadActionRequiredRequests() {

    $("#tbActionRequiredList").jqGrid('GridUnload');
   
    jQuery("#tbActionRequiredList").jqGrid({
        url: '/OnlineFund/GetActionRequiredRequestList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Year', 'Batch', 'Agency', 'Scheme', 'Installment No.', 'Total No. of Road Works', 'Total No. of Bridge Works', 'Total Sanctioned Cost', 'File No.', 'File Date', 'Request Amount', 'View'],
        colModel: [


                            { name: 'STATE', index: 'STATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'BATCH', index: 'BATCH', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", search: false },
                            { name: 'AGENCY', index: 'AGENCY', height: 'auto', width: (pageWidth * (11 / 100)), align: "center", search: false },
                            { name: 'PMGSY_SCHEME', index: 'PMGSY_SCHEME', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'Installment', index: 'Installment', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'TOTAL_ROAD_PROPOSALS', index: 'TOTAL_ROAD_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_BRIDGE_PROPOSALS', index: 'TOTAL_BRIDGE_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_SANCTION_AMOUNT', index: 'TOTAL_SANCTION_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'FILE_NO', index: 'FILE_NO', height: 'auto', width: (pageWidth * (12 / 100)), align: "center", search: false },
                            { name: 'FILE_DATE', index: 'FILE_DATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'RELEASE_AMOUNT', index: 'RELEASE_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'VIEW', index: 'VIEW', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", search: false },


        ],
        pager: jQuery('#pgActionRequiredList'),
        rowNum: 20,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FILE_NO',
        sortorder: "desc",
        caption: 'Online Fund Requests',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        footerrow: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {

            $("#tbActionRequiredList").jqGrid('setGridWidth', $("#tbActionRequiredList").width() - 200, true);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}
function LoadInprogressRequests() {
    $("#tbInprogressList").jqGrid('GridUnload');
   
    jQuery("#tbInprogressList").jqGrid({
        url: '/OnlineFund/GetInProgressRequestList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Year', 'Batch', 'Agency', 'Scheme', 'Installment No.', 'Total No. of Road Works', 'Total No. of Bridge Works', 'Total Sanctioned Cost', 'File No.', 'File Date', 'Request Amount', 'View'],
        colModel: [

                              { name: 'STATE', index: 'STATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'BATCH', index: 'BATCH', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'AGENCY', index: 'AGENCY', height: 'auto', width: (pageWidth * (11 / 100)), align: "center", search: false },
                            { name: 'PMGSY_SCHEME', index: 'PMGSY_SCHEME', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'Installment', index: 'Installment', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'TOTAL_ROAD_PROPOSALS', index: 'TOTAL_ROAD_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_BRIDGE_PROPOSALS', index: 'TOTAL_BRIDGE_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_SANCTION_AMOUNT', index: 'TOTAL_SANCTION_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'FILE_NO', index: 'FILE_NO', height: 'auto', width: (pageWidth * (12 / 100)), align: "center", search: false },
                            { name: 'FILE_DATE', index: 'FILE_DATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'RELEASE_AMOUNT', index: 'RELEASE_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'VIEW', index: 'VIEW', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },

        ],
        pager: jQuery('#pgInprogressList'),
        rowNum: 20,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FILE_NO',
        sortorder: "desc",
        caption: 'Online Fund Requests',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit:false,
        footerrow: false,
        cmTemplate: { title: false },
        grouping: true,
        groupingView: {
            groupField: ['STATE', 'YEAR'],
            groupColumnShow: [true],
            groupText: ['<b>{0}</b>'],
            groupCollapse: false,
            groupOrder: ['desc']
        },
        loadComplete: function (data) {

            $("#tbInprogressList").jqGrid('setGridWidth', $("#tbInprogressList").width() - 200, true);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}
function LoadCompletedRequests() {
    $("#tbCompletedList").jqGrid('GridUnload');

    jQuery("#tbCompletedList").jqGrid({
        url: '/OnlineFund/GetCompletedRequestList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Year', 'Batch', 'Agency', 'Scheme', 'Installment No.', 'Total No. of Road Works', 'Total No. of Bridge Works', 'Total Sanctioned Cost', 'File No.', 'File Date', 'Request Amount', 'View'],
        colModel: [

                            { name: 'STATE', index: 'STATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'BATCH', index: 'BATCH', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'AGENCY', index: 'AGENCY', height: 'auto', width: 200, align: "center", search: false },
                            { name: 'PMGSY_SCHEME', index: 'PMGSY_SCHEME', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'Installment', index: 'Installment', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'TOTAL_ROAD_PROPOSALS', index: 'TOTAL_ROAD_PROPOSALS', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_BRIDGE_PROPOSALS', index: 'TOTAL_BRIDGE_PROPOSALS', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'TOTAL_SANCTION_AMOUNT', index: 'TOTAL_SANCTION_AMOUNT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'FILE_NO', index: 'FILE_NO', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'FILE_DATE', index: 'FILE_DATE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RELEASE_AMOUNT', index: 'RELEASE_AMOUNT', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'VIEW', index: 'VIEW', height: 'auto', width: 50, align: "center", search: false },

        ],
        pager: jQuery('#pgCompletedList'),
        rowNum: 20,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FILE_NO',
        sortorder: "desc",
        caption: 'Online Fund Requests',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        footerrow: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
            $("#tbCompletedList").jqGrid('setGridWidth', $("#tbCompletedList").width() - 200, true);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}
function LoadListForUONumber()
{
    $("#tbUOList").jqGrid('GridUnload');
     
    jQuery("#tbUOList").jqGrid({
        url: '/OnlineFund/GetApprovedRequestList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Year', 'Batch', 'Agency', 'Scheme', 'Installment No.', 'Total No. of Road Works', 'Total No. of Bridge Works', 'Total Sanctioned Cost', 'File No.', 'File Date', 'Request Amount', 'View'],
        colModel: [

                            { name: 'STATE', index: 'STATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'YEAR', index: 'YEAR', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'BATCH', index: 'BATCH', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", search: false },
                            { name: 'AGENCY', index: 'AGENCY', height: 'auto', width: (pageWidth * (11 / 100)), align: "center", search: false },
                            { name: 'PMGSY_SCHEME', index: 'PMGSY_SCHEME', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'Installment', index: 'Installment', height: 'auto', width: (pageWidth * (7 / 100)), align: "center", search: false },
                            { name: 'TOTAL_ROAD_PROPOSALS', index: 'TOTAL_ROAD_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_BRIDGE_PROPOSALS', index: 'TOTAL_BRIDGE_PROPOSALS', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'TOTAL_SANCTION_AMOUNT', index: 'TOTAL_SANCTION_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'FILE_NO', index: 'FILE_NO', height: 'auto', width: (pageWidth * (12 / 100)), align: "center", search: false },
                            { name: 'FILE_DATE', index: 'FILE_DATE', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'RELEASE_AMOUNT', index: 'RELEASE_AMOUNT', height: 'auto', width: (pageWidth * (9 / 100)), align: "center", search: false },
                            { name: 'VIEW', index: 'VIEW', height: 'auto', width: (pageWidth * (6 / 100)), align: "center", search: false },

        ],
        pager: jQuery('#pgUOList'),
        rowNum: 20,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FILE_NO',
        sortorder: "desc",
        caption: 'Online Fund Requests',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: true,
        shrinkToFit: false,
        footerrow: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
            $("#tbUOList").jqGrid('setGridWidth', $("#tbUOList").width() - 200, true);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}

function ViewRequestDetails(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseObservationDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $('#divObservationDetails').load('/OnlineFund/ViewRequestDetails/' + requestId, function (data) {
            $.validator.unobtrusive.parse($('#divObservationDetails'));
            unblockPage();
        });
        $('#divObservationDetails').show('slow');
        $("#divObservationDetails").css('height', 'auto');
    });
}
function CloseObservationDetails() {
    $('#accordion').hide('slow');
    $('#divObservationDetails').hide('slow');
    $('#imgCloseDetails').trigger('click');
}
function LoadInprogressDetailsList()
{
    CloseObservationDetails();
    LoadInprogressRequests();
}
function LoadCompletedDetailsList() {
    CloseObservationDetails();
    LoadCompletedRequests();
}
function LoadActionRequiredDetailsList()
{
    CloseObservationDetails();
    LoadActionRequiredRequests();
}
function LoadListForUONumberRequests()
{
    CloseObservationDetails();
    LoadListForUONumber();
}
function AddObservationDetails(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseObservationDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divObservationDetails").load('/OnlineFund/AddObservationDetails/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#frmAddObservation'));
            unblockPage();
        });
        $('#divObservationDetails').show('slow');
        $("#divObservationDetails").css('height', 'auto');
    });
    //$('#tbActionRequiredList').jqGrid('setGridState', 'hidden');
}
function AddRequestUODetails(requestId)
{
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Observation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseObservationDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divObservationDetails").load('/OnlineFund/AddUODetails/' + requestId, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divObservationDetails').show('slow');
        $("#divObservationDetails").css('height', 'auto');
    });
}