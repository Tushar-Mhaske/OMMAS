$(document).ready(function () {


    

    if ($("#rdoVideo").is(":checked")) {
        $("#divVideoUpload").show();
        $("#divImageUpload").hide();

    }

    if ($("#rdoImage").is(":checked")) {
        $("#divImageUpload").show();
        $("#divVideoUpload").hide();

    }

    $("#rdoVideo").click(function () {
        $("#divImageUpload").html("");

        $("#divVideoUpload").show('slow');
        $("#divImageUpload").hide('slow');

        blockPage();
        $("#divVideoUpload").load('/Execution/VideoUpload/' + $("#Urlparameter").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
        });
        unblockPage();
    });

    $("#rdoImage").click(function () {
        $("#divVideoUpload").html("");

        $("#divImageUpload").show('slow');
        $("#divVideoUpload").hide('slow');

        blockPage();
        $("#divImageUpload").load('/Execution/ImageUpload/' + $("#Urlparameter").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
        });
        unblockPage();
    });

    $("#rdoImage").trigger('click');
});




