$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmBlockDetails'));



    $("#btnUpdateBlock").click(function () {
        if ($("#frmBlockDetails").valid()) {
            $.ajax({

                url: '/Proposal/UpdateProposalBlockDetails',
                type: 'POST',
                cache: false,
                async: false,
                data: $("#frmBlockDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert('Proposal Block details updated successfully');
                        jQuery("#tbUpdateProposalList").trigger('reloadGrid');
                        $("#dvBlockUpdate").dialog('close');
                        return false;
                    }
                    else {
                        alert('Error occurred while processing your request.');
                        return false;
                    }
                },
                error: function () {
                    alert('Request can not be processed at this time . please try after some time!');
                }

            });
        }
        else {
            return false;
        }
    });


});