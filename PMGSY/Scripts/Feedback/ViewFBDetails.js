$(document).ready(function () {
    $('#frmFBFilesDisplay').html('');
    $("#dvTabs").tabs();
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false,
            //beforeActivate: function (event, ui) {

            //    if (ui.newHeader.index() == 0) {
            //        $('#headPropertyNumberDetails').hide();
            //    }

            //}
        });
    });
});

function LoadFBDetails() {
   // $("#dvTabs").tabs();
    blockPage();
    $("#divFBDetailsForm").load('/FeedbackDetails/ViewFBDetails?fId=' + $("#hdnFeedId").val(), function () {
        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
        unblockPage();
    });
    $('#divFBDetailsForm').show('slow');
    unblockPage();
}

function LoadFBFiles() {
    //$("#dvTabs").tabs();
    //$("#divFBDetailsForm").hide();
    blockPage();
    
    $("#divFBDetailsForm").load('/FeedbackDetails/DisplayFBFiles?fId=' + $("#hdnFeedId").val(), function () {
        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
        unblockPage();
    });
    $('#divFBDetailsForm').show('slow');
    unblockPage();
}

function LoadFBApproval() {
    //$("#dvTabs").tabs();
    //$("#divFBDetailsForm").hide();
    blockPage();
    $("#divFBDetailsForm").load('/FeedbackDetails/FBApprovalDetails?fId=' + $("#hdnFeedId").val(), function () {
        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
        unblockPage();
    });
    $('#divFBDetailsForm').show('slow');
    unblockPage();
}

function LoadFBRepStatus() {
    //$("#dvTabs").tabs();
    //alert("hi");
    //$("#dvDetailsFB").hide();
    blockPage();
    $("#divFBDetailsForm").load('/FeedbackDetails/FBRepStatus?fId=' + $("#hdnFeedId").val(), function () {
        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
        unblockPage();
    });
    $('#divFBDetailsForm').show('slow');
    unblockPage();
}