$(document).ready(function () {

    $('#btnUpdate').click(function (e) {
        SaveContractorDetails();
    });

});

function SaveContractorDetails() {

    $.ajax({
        url: "/ContractorGrievances/ContractorGrievances/SaveContractorBankDetails",
        type: "POST",
        dataType: "json",
        async: false,
        cache: false,
        data: $("#frmContractorProfileLayout").serialize(),
        success: function (data) {
            if (data.success) {

                alert(data.message);
                //$("#btnReset").trigger('click');

            }
            else {
                if (data.message != "") {
                    $('#errmessage').html(data.message);
                    $('#dvErrorMessage').show('slow');
                }
            }
        }
    });
}

