$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: 1,
            acceptFileTypes: /(\.|\/)(jpeg|JPEG|jpg|JPG)$/i,
            maxFileSize: 10000000,
        });

        //Enable iframe cross-domain access via redirect option:
        $('#fileupload').fileupload(
            'option',
            'redirect',
            window.location.href.replace(
                /\/[^\/]*$/,
                '/cors/result.html?%s'
            )
        );

        //Load existing files:
        $.ajax({
            url: $('#fileupload').fileupload('option', 'url'),
            dataType: 'json',
            context: $('#fileupload')[0]
        }).done(function (result) {
            $(this).fileupload('option', 'done')
                .call(this, null, { result: result });
        });

        // For validation
        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            var inputs = data.context.find(':input');
            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            if (data.result.success) {
                $("#divSuccess").show();
                alert("Photograph saved.");
                $('#accordionMonitorsInspection').hide("fast");
                $("#divDisplayPhotographUploadView").hide("fast");
                $('#tbPanchyatList').trigger("reloadGrid");
                $("#dvPanchyatList").show("slow");
            }
            else
            {
                alert(data.result.message);
                $('#accordionMonitorsInspection').hide("fast");
                $("#divDisplayPhotographUploadView").hide("fast");
                $('#tbPanchyatList').trigger("reloadGrid");
                $("#dvPanchyatList").show("slow");
            }
        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

});


function closeMonitorsInspectionDetails() {
    $('#accordionMonitorsInspection').hide("fast");
    $("#divDisplayPhotographUploadView").hide("fast");
    $('#tbPanchyatList').trigger("reloadGrid");
    $("#dvPanchyatList").show("slow");
}
