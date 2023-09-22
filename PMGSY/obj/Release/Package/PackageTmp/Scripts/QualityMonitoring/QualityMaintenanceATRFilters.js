
$(document).ready(function () {

    //  $('#tabs-3TierDetails-4').css("display", "none");

    //  $('#tabs-3TierDetails-3').css("display", "none");

    $("#id2TierATRFilterDiv").click(function () {
        $("#id2TierATRFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div2TierATRFilterForm").toggle("slow");

    });

    selectedNameVal = 0;

    viewMaintenanceATRDetails();
    //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());

    $('#btn2TierListATRDetails').click(function () {
        viewMaintenanceATRDetails();
        //ATRListGrid($("#stateCodeATR").val(), $("#monitorCodeATR").val(), $("#frmMonthATR").val(), $("#frmYearATR").val(), $("#toMonthATR").val(), $("#toYearATR").val(), $("#atrStatus").val(), $("#rdStatusATR").val());
    });

    $('#btnBulk2TierATRRegrade').click(function () {
        ShowBulkATRDetails();
        $("#id3TierATRFilterDiv").trigger('click');
    });


});//doc.ready ends here

function viewMaintenanceATRDetails() {
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/MaintenanceATRDetails',
        type: 'POST',
        data: $('#2TierATRFilterForm').serialize(),
        success: function (response) {
            $("#div2TierATRDetailsHtml").html('');
            $("#div2TierATRDetailsHtml").html(response);
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);

        }
    });
}



