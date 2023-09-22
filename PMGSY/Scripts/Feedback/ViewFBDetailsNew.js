$(document).ready(function () {
    $('#frmFBFilesDisplay').html('');
    $("#dvTabs").tabs();
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false,
           
        });
    });








    $.ajax({
        url: '/FeedbackDetails/DisplayFBFilesNew/' + $("#hdnFeedId").val(), // $("#hdnFeedId").val()
        async: false,
        cache: false,
        type: "GET",
        dataType: "html",
        success: function (data) {
            // $('#frmFBFilesDisplay').html(''); 
            $('#tbFBFiles').html(data);
            $('#tbFBFiles').show('fade');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error occurred');
            $.unblockUI();
        }
    });

    //$.ajax({
    //    url: '/FeedbackDetails/AddPIUReplay/' + $("#hdnFeedId").val(),
    //    async: false,
    //    cache: false,
    //    type: "GET",
    //    dataType: "html",
    //    success: function (data) {
    //        $('#tbFBFiles').show('fade');
    //        $('#tbReplyStatus').html(data);
    //        $('#tbReplyStatus').show('fade');
    //        $.unblockUI();
    //    },
    //    error: function (xhr, ajaxOptions, thrownError) {
    //        alert(xhr.responseText);
    //        $.unblockUI();
    //    }
    //});


});

//function LoadFBDetails() {
   
//    blockPage();
//    $("#divFBDetailsForm").load('/FeedbackDetails/ViewFBDetailsNew?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
//        unblockPage();
//    });
//    $('#divFBDetailsForm').show('slow');
//    unblockPage();
//}

//function LoadFBFiles() {
   
//    blockPage();
    
//    $("#divFBDetailsForm").load('/FeedbackDetails/DisplayFBFilesNew?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
//        unblockPage();
//    });
//    $('#divFBDetailsForm').show('slow');
//    unblockPage();
//}

//function LoadFBApproval() {
 
//    blockPage();
//    $("#divFBDetailsForm").load('/FeedbackDetails/FBApprovalDetails?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
//        unblockPage();
//    });
//    $('#divFBDetailsForm').show('slow');
//    unblockPage();
//}

//function LoadFBRepStatus() {
  
//    blockPage();
//    $("#divFBDetailsForm").load('/FeedbackDetails/FBRepStatus?fId=' + $("#hdnFeedId").val(), function () {
//        $.validator.unobtrusive.parse($('#divFBDetailsForm'));
//        unblockPage();
//    });
//    $('#divFBDetailsForm').show('slow');
//    unblockPage();
//}