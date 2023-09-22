$(document).ready(function () {

    $('#btnSubmit').click(function () {
        if ($('#txtLetterNo').val() == '') {
            alert('Please enter Letter No.');
            return false;
        }

        if (!$('#txtLetterNo').val().match("^[a-zA-Z0-9 -._/()]+$")) {
            ///^[a-zA-Z0-9a-zA-Z0-9 ,.()-]+$/;
            alert("Letter No. contains invalid characters, Letter No should be alphanumeric.");
            return false;
        }

        $.ajax({
            type: 'POST',
            url: '/Proposal/AddDropRequestDetails/',
            async: false,
            cache: false,
            data: { letterNo: $('#txtLetterNo').val(), __RequestVerificationToken: $('#frmDropLetterRequestLayout input[name=__RequestVerificationToken]').val() },
            traditional: true,
            success: function (data) {
                alert(data.message);
                if (data.success) {
                    //$("#tblLetterGen").trigger('reload');
                    LoadDropProposalsList();
                    //LoadProposalsToDropList();
                    $("#tblLetterGen").jqGrid('GridUnload');
                    $('#dvRequestLetter').html('');
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //arr.splice(0, arr.length);//Clear the preveious value
                alert(data.message);
                $.unblockUI();
            }
        });
    });

});

