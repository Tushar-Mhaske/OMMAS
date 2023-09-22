$(document).ready(function () {

    $.validator.unobtrusive.parse($('#dvpiuDetails'));

   

    $("#btnUpdatePIU").click(function () {
        if ($("#dvpiuDetails").valid()) {
            $.ajax({

                url: '/Proposal/UpdateProposalPIUDetails',
                type: 'POST',
                cache: false,
                async: false,
                data: $("#dvpiuDetails").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert('Proposal PIU details update successfully');
                        jQuery("#tbUpdateProposalList").trigger('reloadGrid');
                        $("#dvPIUUpdate").dialog('close');
                        return false;
                    }
                    else {
                        //alert('Error occurred while processing your request.');
                        alert(data.message);
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