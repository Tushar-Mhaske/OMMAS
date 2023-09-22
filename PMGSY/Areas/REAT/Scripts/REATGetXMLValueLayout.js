$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmPFMSGetXMLValueLayout');

    //$('#PFMSFile').on('change', pfmsFileUpload);

    //$('#imgPFMSxml').click(function () {
    //    $('#PFMSFile').val(null);
    //    $('#PFMSFile').show('slow');
    //    $('#imgPFMSxml').hide('slow');
    //    $('#lblPFMSxml').text('');
    //});


    $('#btnGetNodeValue').click(function (event) {
        event.stopPropagation(); // Stop stuff happening call double avoid to action
        event.preventDefault(); // call double avoid to action

        if ($('#PFMSFile').get(0).files.length === 0) {
            alert('Please select a xml file');
            return false;
        }

        var form_data = new FormData();
        var objPFMSFile = $("input#PFMSFile").prop("files");

        form_data.append("file", objPFMSFile[0]);
        //form_data.append("file", objPFMSFile);

        form_data = new FormData(document.getElementById("frmPFMSGetXMLValueLayout"));

        $.ajax({
            type: 'POST',
            url: '/Reat/Reat/GetXMLValue/',
            async: false,
            data: form_data,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $('#imgPFMSxml').trigger('click');
                }
                else {
                    alert(data.message);
                }
            }
        })///ajax call Ends..
    });

});

function pfmsFileUpload(event) {

    PFMSImageFile = event.target.files;

    if (PFMSImageFile != null) {

        $('#lblPFMSxml').text($('#PFMSFile').val());
        $('#imgPFMSxml').show('slow');
        $('#PFMSFile').hide('slow');
    }
}