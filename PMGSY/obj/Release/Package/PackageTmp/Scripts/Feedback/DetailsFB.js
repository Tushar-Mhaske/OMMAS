$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmDetailsFB');
    $("#dvTabs").tabs();

    loadFBDetails();


    $("#btnSubmit").click(function () {
        if ($("#frmDetailsFB").valid()) {

            loadFBDetails();
        }
    });

    $("#ddlApproved").change(function () {
        fillFBStatus();
    });
});

$("#idFilterDiv").click(function () {

    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#divFeedbackMain").toggle("slow");

});


function loadFBDetails() {
    //$("#tbATRJqGrid").jqgrid('GridUnload');
    // alert("1");
    $("#tbFBDetailsJqGrid").jqGrid('GridUnload');

    $("#dvTabs").tabs({ active: 0 });
    $('#accordion').hide('slow');
    $("#tbFBDetailsJqGrid").jqGrid('setGridState', 'visible');
    $("#tbFBDetailsJqGrid").trigger('reloadGrid');
    $("#divFBDetails").show("slow");

    $("#tbFBDetailsJqGrid").jqGrid({
        url: '/FeedbackDetails/FBList',
        datatype: "json",
        mtype: "POST",
        postData: { FOR_MONTH: $("#ddlMonths").val(), FOR_YEAR: $("#ddlYears").val(), FOR_STATES: $("#ddlStates").val(), FOR_CATEGORY: $("#ddlCategories").val(), APPR: $("#ddlApproved").val(), STATUS: $("#ddlStatus").val(), fbThrough: $('#ddlfbThrough').val() },
        colNames: ["Feedback No.", "Feedback Date", "Name", "Category", "Feedback For", "Against", "Feedback Through", "Status","Action to be Taken By", "Acceptance Status", "View"],
        colModel: [
            { name: 'SrNo.', index: 'SrNo.', width: 40, sortable: true, align: "center" },
            { name: 'FeedbackDate', index: 'FeedbackDate', width: 40, sortable: true, align: "center", search: false },
            { name: 'Name', index: 'Name', width: 70, sortable: true, align: "center", search: false },
            { name: 'Category', index: 'Category', width: 40, sortable: true, align: "center", search: false },
            { name: 'Feedback For', index: 'Feedback For', width: 50, sortable: true, align: "center", search: false },
            { name: 'Subject', index: 'Subject', width: 80, sortable: true, align: "center", search: false },
            { name: 'FeedFrom', index: 'FeedFrom', width: 40, sortable: true, align: "center", search: false },
            { name: 'Status', index: 'Status', width: 40, sortable: true, align: "center", search: false },
            { name: 'ActionTaken', index: 'ActionTaken', width: 70, sortable: true, align: "center", search: false },
            { name: 'ApprDetails', index: 'ApprDetails', width: 70, sortable: true, align: "center" },
            { name: 'ShowDetails', index: 'ShowDetails', width: 20, sortable: false, align: "center" }
        ],
        pager: jQuery("#divFBDetailsReportPager"),
        rownum: 100,
        viewrecords: true,
        recordtext: '{2} Records Found',
        caption: "&nbsp &nbsp; Feedback Details",
        height: "auto",
        autowidth: true,
        sortname: 'SrNo.',
        sortorder: 'asc',
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ['Feedback For'],
            groupColumnShow: [false],
            groupSummary: [true],
            groupText: ['<b>{0}</b>'],
            groupCollapse: false,
            groupOrder: ['asc'],
            showSummaryOnHide: true
        },

        loadComplete: function () {
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                //window.location.href = "/Login/SessionExpire";
                alert('Error occurred');
            }
            else {
                //window.loc.href = "/Login/SessionExpire";
            }
        }
    });//end of grid
}

function fillFBStatus() {

    $.ajax({
        url: "/FeedbackDetails/fillDDLStatus?approval=" + $("#ddlApproved option:selected").val(),
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlStatus").empty();
            $.each(data, function () {
                $("#ddlStatus").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        },
        error: function () {
            alert("error");
        }
    })
}

function ShowDetails(id) {
    //alert("test");

    //$('#tbFBApproval').html('');
    //$('#tbFBDetailsForm').html('');
    //$('#tbReplyStatus').html('');
    $("#dvTabs").tabs({ active: 0 });
    $("#dvTabs").show();
    $("#tbFBDetailsForm").html('');

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Feedback Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseFBDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#tbFBDetailsForm").load('/FeedbackDetails/ViewFBDetails/' + id, function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));

            unblockPage();
        });
        $('#tbFBDetailsForm').show('slow');
        $("#tbFBDetailsForm").css('height', 'auto');
    });

    $("#tbFBDetailsJqGrid").jqGrid('setGridState', 'hidden');
    $('#tbFBDetailsForm').jqGrid('setGridState', 'hidden');

}

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseFBDetails() {
    //$("#dvTabs").tabs("destroy");
    $("#dvTabs").tabs({ active: 0 });
    $('#accordion').hide('slow');
    $("#tbFBDetailsJqGrid").jqGrid('setGridState', 'visible');
    $("#tbFBDetailsJqGrid").trigger('reloadGrid');
    $("#divFBDetails").show("slow");

}

//function LoadFBDetails() {
//    // $("#dvTabs").tabs();
//    blockPage();
//    $("#divFBDetailsForm").load('/FeedbackDetails/ViewFBDetails?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
//        unblockPage();
//    });
//    $('#divFBDetailsForm').show('slow');
//    unblockPage();
//}

//function LoadFBApproval() {
//    // $("#dvTabs").tabs();
//    //$("#divFBDetailsForm").hide();
//    blockPage();
//    $("#tbFBApproval").load('/FeedbackDetails/FBApprovalDetails?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#tbFBApproval'));
//        unblockPage();
//    });
//    $('#tbFBApproval').show('slow');
//    unblockPage();
//}

$(function () {

    $("#dvTabs").tabs({
        active: 0,
        beforeActivate: function (event, ui) {

            switch (ui.newTab.index()) {
                case 0:
                    //alert("1");
                    //event.preventDefault();
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/FeedbackDetails/ViewFBDetails/' + $("#hdnFeedId").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        dataType: "html",
                        success: function (data) {
                            $('#tbFBDetailsForm').html(data);
                            $('#tbFBDetailsForm').show('fade');
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 1:

                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/FeedbackDetails/DisplayFBFiles/' + $("#hdnFeedId").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        dataType: "html",
                        success: function (data) {
                            $('#frmFBFilesDisplay').html('');
                            $('#tbFBFiles').html(data);
                            $('#tbFBFiles').show('fade');
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert('Error occurred');
                            $.unblockUI();
                        }
                    });

                    break;
                case 2:
                    //alert("2");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/FeedbackDetails/FBApprovalDetails/' + $("#hdnFeedId").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        dataType: "html",
                        success: function (data) {
                            $('#tbFBApproval').html(data);
                            $('#tbFBApproval').show('fade');
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                case 3:
                    //alert("3");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/FeedbackDetails/FBRepStatus/' + $("#hdnFeedId").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        dataType: "html",
                        success: function (data) {
                            $('#tbReplyStatus').html(data);
                            $('#tbReplyStatus').show('fade');
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
                    break;
                default:
                    //alert("default");
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        url: '/FeedbackDetails/ViewFBDetails/' + $("#hdnFeedId").val(),
                        async: false,
                        cache: false,
                        type: "GET",
                        dataType: "html",
                        success: function (data) {
                            $('#tbFBDetailsForm').html(data);
                            $('#tbFBDetailsForm').show('fade');
                            $.unblockUI();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            $.unblockUI();
                        }
                    });
            }
        },
        create: function (event, ui) {
            $('#dvTabs .ui-helper-reset').css("line-height", "1");
        },
    });
    $("#dvTabs").tabs().removeClass("ui-widget");
});