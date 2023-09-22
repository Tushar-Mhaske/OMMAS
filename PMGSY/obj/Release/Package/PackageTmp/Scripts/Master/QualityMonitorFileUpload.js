

$(document).ready(function () {

   
    blockPage();
    $("#divImageUpload").load('/Master/QualityMonitorImageUpload/' + $("#ADMIN_QualityMonitor_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#fileupload'));
        unblockPage();
    });
    unblockPage();

   
    $("#dvhdFileUpload").click(function () {

        if ($("#dvhdFileUpload").is(":visible")) {

            $("#spCollapseIconQM").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#divQualityMonitorForm").slideToggle(300);
        }
        else {
            $("#spCollapseIconQM").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#divQualityMonitorForm").slideToggle(300);
        }
    });


});




