jQuery.validator.addMethod("customexistrequired", function (value, element, param) {
    if ($("#PMGSYScheme").val() == 2) {
        if ($("#ProposalType").val() == "P") {
            if ($('#' + element.id).val() == "") {
                return false;
            }
        }
    }
    else {
        return true;
    }
    return true;
});

jQuery.validator.unobtrusive.adapters.addBool("customexistrequired");

jQuery.validator.addMethod("comparefieldvalidator", function (value, element, param) {

    var startChainage = parseFloat($('#TEND_START_CHAINAGE_Existing').val());
    var endChainage = parseFloat($('#TEND_END_CHAINAGE_Existing').val());
    if (startChainage >= endChainage)
        return false;
    else
        return true;
});
jQuery.validator.unobtrusive.adapters.addBool("comparefieldvalidator");

$.validator.unobtrusive.adapters.add('chainagerequired', ['chainage'], function (options) {
    options.rules['chainagerequired'] = options.params;
    options.messages['chainagerequired'] = options.message;
});

jQuery.validator.addMethod("chainagerequired", function (value, element, params) {

    if (params.chainage == "TEND_START_CHAINAGE_Existing") {
        if ($('#TEND_START_CHAINAGE_Existing').val() != '' && $('#TEND_END_CHAINAGE_Existing').val() == '')
            return false;
        else
            return true;
    }

    if (params.chainage == "TEND_END_CHAINAGE_Existing") {
        if ($('#TEND_END_CHAINAGE_Existing').val() != '' && $('#TEND_START_CHAINAGE_Existing').val() == '')
            return false;
        else
            return true;
    }
});

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmAddAgreementDetails_Existing'));

    $("#TEND_AGREEMENT_AMOUNT_NEW").trigger('blur');

    if ($('#ddlProposalWorks_Existing option').length > 1) {

        $("#ddlProposalWorks_Existing").find("option[value='0']").remove();
        $("#tdlblProposalWork_Existing").show('slow');
        $("#tdddlProposalWork_Existing").show('slow');
        $('#tdtxtAmountYear5').attr('colspan', '1');
    }
    else if ($('#ddlProposalWorks_Existing option').length == 1) {
    }
    else {
        if ($('#IMS_WORK_CODE').html() == '') {
        }
    }

    $('#btnSaveAgreementDetails_Existing').click(function (e) {

        if ($('#frmAddAgreementDetails_Existing').valid()) {
            $('#EncryptedIMSPRRoadCode_Existing').val($('#EncryptedIMSPRRoadCode').val());

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Agreement/AddExistingAgreementDetails",
                type: "POST",
                data: $("#frmAddAgreementDetails_Existing").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        if ($("#dvErrorMessage").is(":visible")) {
                            $('#dvErrorMessage').hide('slow');
                            $('#message').html('');
                        }

                        $('#dvNewExistingAgreement').empty();
                        $('#dvNewExistingAgreement').load('/Agreement/ExistingAgreement?contractorID=' + $('#ddlContractors option:selected').val() + '&agreementCode=' + $('#ddlAgreementNumbers option:selected').val() + '&IMSPRRoadCode=' + $('#EncryptedIMSPRRoadCode').val(), function () {

                            if ($('#ddlProposalWorks_Existing option').length == 1) {
                                $('#dvAgreementDetails').hide();
                            }
                            else {
                                $('#dvNewExistingAgreement').show('slow');
                            }
                            $.unblockUI();
                        });

                        $('#tbAgreementDetailsList').jqGrid("setGridParam", { "postData": { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() } });
                        $('#tbAgreementDetailsList').trigger('reloadGrid', [{ page: 1 }]);
                        CheckforActiveAgreement();

                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvNewExistingAgreement").html(data);

                        $('#dvNewAgreement').hide();
                        $("#dvAgreementDetails").show('slow');
                        $("#dvNewExistingAgreement").show('slow');
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });

    $('#btnResetAgreementDetails_Existing').click(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }


    });

    
    $("#btnUpdateAgreementDetails_Existing").click(function (e) {


        if ($("#frmAddAgreementDetails_Existing").valid()) {

            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode_Existing').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Agreement/EditSpecialAgreementDetails/',
                async: false,
                data: $("#frmAddAgreementDetails_Existing").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $('#dvAgreementDetails').hide();

                        $("#dvAgreementDetails").load("/Agreement/SpecialAgreementDetails/" + encryptedIMSPRCode, function () {
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                        });

                        $('#tbAgreementDetailsList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {

                        $("#dvNewExistingAgreement").html(data);
                        $("#tblExistingAgreement").hide();
                        $("#tblContractor").hide();
                        $('#dvNewAgreement').hide();
                        $("#dvAgreementDetails").show('slow');
                        $("#dvNewExistingAgreement").show('slow');
                        $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                        $("#TEND_AGREEMENT_NUMBER").focus();
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $('#btnCancelAgreementDetails_Existing').click(function (e) {

        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
        $.ajax({
            url: "/Agreement/AgreementDetails/" + encryptedIMSPRCode,
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#dvAgreementDetails").hide();
                $("#dvAgreementDetails").html(data);
                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);


            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });


    $("#TEND_STATE_SHARE_NEW,#TEND_MORD_SHARE_NEW,#TEND_HIGHER_SPEC_AMT_NEW").blur(function () {

        var stateAmount = 0.0;
        var mordAmount = 0.0;
        var specAmount = 0.0;

        if (Number($("#TEND_STATE_SHARE_NEW").val()) != NaN) {

            stateAmount = Number($("#TEND_STATE_SHARE_NEW").val());
        }

        if (Number($("#TEND_MORD_SHARE_NEW").val()) != NaN) {

            mordAmount = Number($("#TEND_MORD_SHARE_NEW").val());
        }

        if (Number($("#TEND_HIGHER_SPEC_AMT_NEW").val()) != NaN) {

            specAmount = Number($("#TEND_HIGHER_SPEC_AMT_NEW").val());
        }

        var totalAmount = stateAmount + mordAmount;
        
    });

});


function IsPartAgreementYes_Existing() {

    if ($("#rdoIsPartAgreementYes_Existing").is(":checked")) {

        $("#trChainage_Existing").show('slow');
        $("#trEmpty_Existing").show('slow');
    }
}

function IsPartAgreementNo_Existing() {

    if ($("#rdoIsPartAgreementNo_Existing").is(":checked")) {

        $("#trChainage_Existing").hide('slow');
        $("#trEmpty_Existing").hide('slow');
        $("#TEND_START_CHAINAGE_Existing").val('');
        $("#TEND_END_CHAINAGE_Existing").val('');
    }
}