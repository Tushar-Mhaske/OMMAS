$(function () {

    $.validator.unobtrusive.parse($('#frmPropListLayout'));

    $('#btnPropListDetails').click(function () {


        $('#Year').val($('#ddYear_PropListDetails option:selected').val()); //var year = $('#ddYear_PropListDetails').val();
        $('#Batch').val($('#ddBatch_PropListDetails option:selected').val());
        $('#Agency').val($('#ddAgency_PropListDetails option:selected').val());
        $('#Status').val($('#ddStatus_PropListDetails option:selected').val());

        $('#CollabName').val($('#ddAgency_PropListDetails option:selected').text());
        $('#StatusName').val($('#ddStatus_PropListDetails option:selected').text());


        if ($('#frmPropListLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/ProposalListReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPropListLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadProposalListReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnPropListDetails').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvPropListLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
