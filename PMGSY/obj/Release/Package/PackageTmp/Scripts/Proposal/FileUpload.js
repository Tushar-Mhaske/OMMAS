

$(document).ready(function () {
    

    if ($("#rdoPdf").is(":checked")) {
        $("#divImageUpload").show();
        $("#divPDF").show();

    }

    if ($("#rdoImage").is(":checked")) {
        $("#divImageUpload").hide();
        $("#divPDF").show();

    }

    $("#rdoPdf").click(function () {
        $("#divImageUpload").html("");

        $("#divPDF").show('slow');
        $("#divImageUpload").hide('slow');       

        blockPage();
        $("#divPDF").load('/Proposal/PdfFileUpload/' + $("#IMS_PR_ROAD_CODE").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
        });
        unblockPage();
    });


    $("#rdoImage").click(function () {
        $("#divPDF").html("");

        $("#divImageUpload").show('slow');
        $("#divPDF").hide('slow');

        blockPage();
        $("#divImageUpload").load('/Proposal/ImageUpload/' + $("#IMS_PR_ROAD_CODE").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
        });
        unblockPage();
    });    
});




