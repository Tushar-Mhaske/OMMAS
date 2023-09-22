$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmUpoadMissingImages');


    $('#MissingImageFile').on('change', missingFileUpload);

    $('#imgMissingImage').click(function () {
        $('#MissingImageFile').val(null);
        $('#MissingImageFile').show('slow');
        $('#imgMissingImage').hide('slow');
        $('#lblMissingImage').text('');
    });

    $('#btnUpload').click(function (event) {
        if ($('#MissingImageFile').get(0).files.length === 0)
        {
            alert('Please select a jpg file');
            return false;
        }

        //if ($("#frmUpoadMissingImages").valid()) {

        //event.stopPropagation(); // Stop stuff happening call double avoid to action
        //event.preventDefault(); // call double avoid to action

        var form_data = new FormData();

        var objMissingImageFile = $("input#MissingImageFile").prop("files");
        form_data.append("file", objMissingImageFile[0]);

        form_data.append("qmFileName", $("#qmFileName").val());
        form_data.append("PrRoadCode", $("#PrRoadCode").val());
        form_data.append("QMObsId", $("#QMObsId").val());
        form_data.append("QMSchCode", $("#QMSchCode").val());
        form_data.append("__RequestVerificationToken", $("#frmUpoadMissingImages input[name=__RequestVerificationToken]").val());

        var data = $("#frmUpoadMissingImages").serialize();

        //for (var i = 0; i < data.length; i++) {
        //    form_data.append(data[i].name, data[i].value);
        //}

        $.ajax({
            type: 'POST',
            url: '/QualityMonitoring/UploadMissingImageFile/',
            async: false,
            //data: {file : form_data,model :data},
            data: form_data,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $('#imgMissingImage').trigger('click');
                    $("#tbFilesList").trigger('reloadGrid');
                    //$("#tbFilesList").jqGrid('setGridState', 'visible');
                    closeDiv();
                }
                else {
                    alert(data.message);
                }
                
            }
        })///ajax call Ends..
        event.stopPropagation(); // Stop stuff happening call double avoid to action
        event.preventDefault(); // call double avoid to action
        //}
    });///btnUpload click Ends..

});///document ready Ends..

function missingFileUpload(event) {

    MissingImageFile = event.target.files;

    if (MissingImageFile != null) {

        $('#lblMissingImage').text($('#MissingImageFile').val());
        $('#imgMissingImage').show('slow');
        $('#MissingImageFile').hide('slow');
    }
}

