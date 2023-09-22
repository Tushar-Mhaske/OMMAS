$(document).ready(function () {

    //CheckforActiveAgreement();

    $('#btnCreateNew_AgreementDetails').click(function () {

        if (!$('#dvNewAgreement').is(':visible')) {
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
            $("#dvAgreementDetails").load("/Agreement/SpecialAgreementDetails/" + encryptedIMSPRCode, function () {
                if ($('#AgreementType').val() == "C") {
                    $('#trAgreementType').show('slow');
                    $('#trEmptyAgreementType').show('slow');
                }
                else {
                    $('#trMainAmountYear1').hide();
                    $('#trMainAmountYear2').hide();
                    $('#trMainAmountYear3').hide();
                    $('#trChainage').hide();
                }
                $('#dvNewAgreement').show('slow');
                $('#tblContractor').show('slow');
                $('#dvNewExistingAgreement').hide();
                $('#dvAgreementDetails').show('slow');
                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

            });
        }
    });

    $("#dvIncompleteReason").dialog({
        autoOpen: false,
        height: '220',
        width: "420",
        modal: true,
        title: 'Incomplete Reason.'
    });

    $("#dvViewAgreementMaster").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Agreement Details'
    });



});

function CheckforActiveAgreement() {

    var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
    var agreementType = $('#EncryptedAgreementType').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/CheckforActiveAgreement/',
        async: false,
        cache: false,
        data: { EncryptedIMSPRCode: encryptedIMSPRCode, AgreementType: agreementType },
        success: function (data) {
            if (data.exist == true) {
                $('#tblCreateNew_AgreementDetails').hide();
            }
            else if (data.exist == false) {
                $('#tblCreateNew_AgreementDetails').show();
            }

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })

}


