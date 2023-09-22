$(document).ready(function () {


    $('#btnCreateNew_SplitWorkDetails').click(function () {

        if (!$('#dvNewSplitWorkDetails').is(':visible')) {
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

            $.ajax({
                url: "/SplitWork/SplitWorkDetails/" + encryptedIMSPRCode,
                type: "GET",
                dataType: "html",
                async: false,
                success: function (data) {
                    $("#dvSplitWorkDetails").html(data);
                    $('#dvNewSplitWorkDetails').show('slow');
                    $('#dvSplitWorkDetails').show('slow');                  
                    $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }

            });

        }
    });
    $('#btnSplitWorkDetailsCancel').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvSplitWork").animate({
            scrollTop: 0
        });

    });

    $('#imgCloseAgreementDetails').click(function () {


        if ($("#accordion").is(":visible")) {
            $('#accordion').hide('slow');
        }

        ViewSearchDiv();
        $('#tbProposedRoadList').jqGrid("setGridState", "visible");

        $("#dvSplitWork").animate({
            scrollTop: 0
        });

    });
});

function ViewSearchDiv() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!$("#dvSearchProposedRoad").is(":visible")) {

        var data = $('#tbProposedRoadList').jqGrid("getGridParam", "postData");

        if (!(data === undefined)) {

            $('#ddlFinancialYears').val(data.sanctionedYear);
            $('#ddlBlocks').val(data.blockCode);
            $('#ddlPackages').val(data.packageID);
        }

        $("#dvSearchProposedRoad").show('slow');
        $.unblockUI();
    }
    $.unblockUI();

}