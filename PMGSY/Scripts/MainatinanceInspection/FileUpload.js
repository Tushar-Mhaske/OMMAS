$(document).ready(function () {

    //if ($("#rdoImage").is(":checked")) {
    //    $("#divImageUpload").show();
    //    $("#divVideoUpload").hide();

    //}

    $("#rdoImage").click(function () {
        //$("#divVideoUpload").html("");

        $("#divImageUpload").show('slow');
        //$("#divVideoUpload").hide('slow');

        blockPage();
        $("#divImageUpload").load('/MaintainanceInspection/ImageUpload/' + $("#Urlparameter").val(), function () {
            $.validator.unobtrusive.parse($('#fileupload'));
            unblockPage();
        });
        unblockPage();
    });

    $("#rdoImage").trigger('click');
});




