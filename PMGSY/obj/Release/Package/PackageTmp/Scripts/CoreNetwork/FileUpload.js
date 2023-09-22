$(document).ready(function () {

    blockPage();


    $("#divAddForm").load('/CoreNetWork/CoreNetworkFileUpload/' + $("#PLAN_CN_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#fileupload'));
        unblockPage();
    });

    unblockPage();
});


   



