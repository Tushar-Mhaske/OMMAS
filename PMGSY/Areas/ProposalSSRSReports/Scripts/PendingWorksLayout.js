$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPendingWorksLayout'));

    $("#ddState_PendingWorksDetails").focus();
    if ($("#State").val() > 0) {
        $("#ddState_PendingWorksDetails").attr("disabled", "disabled");
    }
    $('#btnGoPendingWorks').click(function () {

        $('#State').val($('#ddState_PendingWorksDetails option:selected').val()); //var year = $('#ddYear_PropListDetails').val();
        $('#Reason').val($('#ddReason_PendingWorksDetails option:selected').val());

        $('#StateName').val($('#ddState_PendingWorksDetails option:selected').text()); //var year = $('#ddYear_PropListDetails').val();
        $('#ReasonName').val($('#ddReason_PendingWorksDetails option:selected').text());

        if ($('#frmPendingWorksLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/PendingWorksReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPendingWorksLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadPendingWorksReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $('#btnGoPendingWorks').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvPendingWorksLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});