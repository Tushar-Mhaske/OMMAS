
$(document).ready(function () {


    $('#btnCreateNew_AgreementDetails').click(function () {

        if (!$('#dvNewAgreement').is(':visible')) {
            
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

            $.ajax({
                url: "/MaintenanceAgreement/SpecialAgreementDetails/" + encryptedIMSPRCode,
                type: "GET",
                dataType: "html",
                async: false,
                success: function (data) {

                    $("#dvAgreementDetails").html(data);
                    $('#trAgreementType').show('slow');
                    $('#dvNewAgreement').show('slow');
                    $('#dvNewExistingAgreement').hide();
                    $('#dvAgreementDetails').show('slow');
                    $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }

            });

        }

    });

    $('#btnAgreementDetailsCancel').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvAgreement").animate({
            scrollTop: 0
        });

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


function ViewSearchDiv() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
        }

        $("#dvSearchProposedRoad").show('slow');
        $.unblockUI();
    }
    $.unblockUI();

}