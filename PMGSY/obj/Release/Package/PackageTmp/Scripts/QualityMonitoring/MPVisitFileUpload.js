

$(document).ready(function () {


    if ($("#rdoPdf").is(":checked"))
    {
        $("#divImageUpload").show();
        $("#divPDF").show();

    }

    if ($("#rdoImage").is(":checked"))
    {
        $("#divImageUpload").hide();
        $("#divPDF").show();

    }

    $("#rdoPdf").click(function ()
    {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#divImageUpload").html("");

        $("#divPDF").show('slow');
        $("#divImageUpload").hide('slow');

       
        $("#divPDF").load('/QualityMonitoring/PdfFileUploadMPVisit/' + $("#MP_VISIT_ID").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            $.unblockUI();

        });
        $.unblockUI();

    });


    $("#rdoImage").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#divPDF").html("");

        $("#divPDF").hide('slow');

        //blockPage();
        $("#divImageUpload").load('/QualityMonitoring/ImageUploadMPVisit/' + $("#MP_VISIT_ID").val(), function () {
            // $.validator.unobtrusive.parse($('#fileupload'));
            $("#divImageUpload").show('slow');

            $.unblockUI();

        });
        $.unblockUI();

    });
});




