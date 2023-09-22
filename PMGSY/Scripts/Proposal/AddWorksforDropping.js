$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddDropOrderLayout'));

    $('#btnAddDroppedWorks').click(function () {
        if ($('#frmAddDropOrderLayout').valid()) {
            if (parseFloat($('#expenditureIncurred').val()) > parseFloat($('#txtRecoupAmt').val())) {
                alert('Please enter Recoup Amount greater than or equal to the Expenditure Incurred');
                return false;
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Proposal/AddDropProposal/',
                data: $("#frmAddDropOrderLayout").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#dvDropProposalModal').dialog('close');
                        $("#tbSRRDAProposalList").trigger('reloadGrid');

                    }
                    else if (data.success == false) {
                        //$("#divError").show();
                        //$("#divError").html('<strong>Alert : </strong>' + data.message);
                        alert(data.message);
                    }
                    $.unblockUI();
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                    $.unblockUI();
                }

            })
        }
    });
});