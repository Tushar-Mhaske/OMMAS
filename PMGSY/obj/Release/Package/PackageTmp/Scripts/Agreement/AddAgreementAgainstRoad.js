$(document).ready(function () {

    //if (isAgreementActive == 'True') {
    //    $('#tblCreateNew_AgreementDetails').hide();
    //   // $('#tblCreateNew_AgreementDetails').empty();
    //}

    CheckforActiveAgreement();

    $('#btnCreateNew_AgreementDetails').click(function () {

        if (!$('#dvNewAgreement').is(':visible')) {
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
            $("#dvAgreementDetails").load("/Agreement/AgreementDetails/" + encryptedIMSPRCode, function ()
            {
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

    //  alert('a');
    var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
    var agreementType = $('#EncryptedAgreementType').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/CheckforActiveAgreement/',
        //dataType: "html",
        async: false,
        cache: false,
        data: { EncryptedIMSPRCode: encryptedIMSPRCode, AgreementType: agreementType },
        success: function (data) {
            //  alert("data exist :" + data.exist + " Proposal type :" + $("#ddlProposalTypes option:selected").val());
            if ($("#ddlProposalTypes option:selected").val() == "B") 
            {
                // Allow Multiple Agreements if Proposal is Building
                $('#tblCreateNew_AgreementDetails').show();

            }

            //if (data.exist == true) {
            //    if (data.isAgreementAvailable == true)
            //    {
            //        $('#tblCreateNew_AgreementDetails').hide();
            //    }
            //    else
            //    {
            //        $('#tblCreateNew_AgreementDetails').show();
            //    }
            //    //Hide add agreement button if no previous agreement exists
                
            //}
            //else
            //{
            //    if (data.AgreementAllowOrNot == 'Y') {
            //        $('#tblCreateNew_AgreementDetails').show();
                   
            //    }
            //    else
            //    {
            //        $('#tblCreateNew_AgreementDetails').hide();
            //    }

            //}
            if (data.exist == false) {
                $('#btnCreateNew_AgreementDetails').hide();
            }
            else {
                $('#btnCreateNew_AgreementDetails').show();
            }

            if (data.exist == true)
            {
                $('#tblCreateNew_AgreementDetails').show();
            }
            else
            {
                $('#tblCreateNew_AgreementDetails').hide();
            }


         


            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })

}


