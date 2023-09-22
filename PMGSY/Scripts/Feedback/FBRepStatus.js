$(document).ready(function () {
    $('#frmFBFilesDisplay').html('');
    $("#btnAdd").click(function () {
        ShowDetails1($("#hdnFeedId").val());
    });
    loadFBRepStatus();
});

function loadFBRepStatus() {
    //$("#tbATRJqGrid").jqgrid('GridUnload');
    $("#tbFBReplyStatusJqGrid").jqGrid({
        url: '/FeedbackDetails/FBRepStatList',
        datatype: "json",
        mtype: "POST",
        //postData: { FOR_MONTH: $("#ddlMonths option:selected").val(), FOR_YEAR: $("#ddlYears option:selected").val(), FOR_STATES: $("#ddlStates option:selected").val(), FOR_CATEGORY: $("#ddlCategories option:selected").val(), APPR: $("#ddlApproved option:selected").val(), STATUS: $("#ddlStatus option:selected").val() },
        postData: { id: $("#hdnFeedId").val() },
        colNames: ["Reply Id", "Reply Date", "Comment", "Status", "Edit", "Delete"],
        colModel: [
            { name: 'Reply Id', index: 'ReplyId', width: "90", hidden: true, sortable: true, align: "center" },
            { name: 'Reply Date', index: 'ReplyDate', width: "100", sortable: true, align: "center", search: false },
            { name: 'Comment', index: 'Comment', width: "490", sortable: true, align: "center", search: false },
            { name: 'Status', index: 'Status', width: "150", sortable: true, align: "center", search: false },
            //{ name: 'Add', index: 'Add', width: "150", sortable: false, align: "center" },
            { name: 'Update', index: 'Update', width: "150", sortable: false, align: "center" },
            { name: 'Delete', index: 'Delete', width: "150", sortable: false, align: "center" }
        ],
        pager: jQuery("#divFBRepStatusReportPager"),
        rownum: 100,
        viewrecords: true,
        recordtext: '{2} Records Found',
        caption: "Feedback Reply Status",
        height: "auto",
        autowidth: true,
        sortname: 'SrNo.',
        sortorder: 'asc',
        rownumbers: true,
        grouping: false,
        shrinkToFit: false,
        width: '100%',
        loadComplete: function () {
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //window.loc.href = "/Login/SessionExpire";
            }
        }
    });//end of grid
}

function ShowDetails1(id) {

    $("#dvReplyStatus").load('/FeedbackDetails/FeedbackReply/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));
        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
        $('#btnAdd').hide();
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');
}

function UpdateFBRep(id) {
    $('#btnAdd').hide();
    $("#dvReplyStatus").load('/FeedbackDetails/FeedbackReplyUpdate/' + id, function () {
        $.validator.unobtrusive.parse($('#dvReplyStatus'));

        $('#dvReplyStatus').show('slow');
        $("#dvReplyStatus").css('height', 'auto');
    });
    $('#tbFBReplyStatusJqGrid').jqGrid('setGridState', 'hidden');
}

function DeleteFBRep(id) {
    //alert(paramater);
    if (confirm("Are you sure you want to Delete Feedback Reply?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/FeedbackDetails/deleteFeedBackRep/" + id,
            type: "GET",
            dataType: "json",
            //data: $("frmFeedbackRep").serialize(),
            success: function (data) {
                if (data.status == true) {
                    alert("Feedback Reply Deleted Successfully");
                    $('#tbFBReplyStatusJqGrid').trigger('reloadGrid');
                }
                else {
                    alert("Please Delete in sequence");
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error occured on Delete");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}


