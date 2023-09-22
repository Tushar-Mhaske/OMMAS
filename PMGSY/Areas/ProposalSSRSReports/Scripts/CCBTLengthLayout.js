$(function () {

    $.validator.unobtrusive.parse($('#frmCCBTLengthLayout'));

    $('#btnCCBTLengthDetails').click(function () {


        $('#Year').val($('#ddYear_CCBTDetails').val()); //var year = $('#ddYear_PropListDetails').val();
        $('#Batch').val($('#ddBatch_CCBTDetails').val());
        $('#Agency').val($('#ddAgency_CCBTDetails').val());
        $('#Status').val($('#ddStatus_CCBTDetails').val());

        $('#CollabName').val($('#ddAgency_CCBTDetails option:selected').text());
        $('#StatusName').val($('#ddStatus_CCBTDetails option:selected').text());

        if ($('#frmCCBTLengthLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/CCBTLengthReport/',
                type: 'POST',
                catche: false,
                data: $("#frmCCBTLengthLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadCCBTLengthReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnCCBTLengthDetails').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvCCBTLengthLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
