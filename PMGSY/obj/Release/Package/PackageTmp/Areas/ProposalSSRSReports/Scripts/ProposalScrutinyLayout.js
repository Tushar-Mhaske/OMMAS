$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmProposalScrutinyLayout'));
    
    if ($("#State").val() > 0) {

        $("#ddState_PropScruitinyDetails").attr("disabled", "disabled");
    }

    $("#ddState_PropScruitinyDetails").change(function () {

        loadAgencyList();
    });
    $("#ddStaPta_PropScruitinyDetails").change(function () {

        loadAgencyList();
    });
    $("#ddStaPta_PropScruitinyDetails").trigger('change');

    $('#btnGoScruitiny').click(function () {

        $('#State').val($("#ddState_PropScruitinyDetails option:selected").val());
        $('#Type').val($("#ddStaPta_PropScruitinyDetails option:selected").val());
        $('#Agency').val($("#ddAgency_PropScruitinyDetails option:selected").val());
        $('#Year').val($("#ddYear_PropScruitinyDetails option:selected").val());
        $('#Batch').val($("#ddBatch_PropScruitinyDetails option:selected").val());
        $('#Collaboration').val($("#ddScheme_PropScruitinyDetails option:selected").val());

        $('#StateName').val($("#ddState_PropScruitinyDetails option:selected").text());
        $('#AgencyName').val($("#ddAgency_PropScruitinyDetails option:selected").text());
        $('#BatchName').val($("#ddBatch_PropScruitinyDetails option:selected").text());
        $('#CollabName').val($("#ddScheme_PropScruitinyDetails option:selected").text());

        if ($('#frmProposalScrutinyLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/ProposalScrutinyReport/',
                type: 'POST',
                catche: false,
                data: $("#frmProposalScrutinyLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadProposalScrutinyReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });

        }
    });

    $('#btnGoScruitiny').trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvProposalScrutinyLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});

function loadAgencyList() {
    $("#ddAgency_PropScruitinyDetails").val(0);
    $("#ddAgency_PropScruitinyDetails").empty();
    $.ajax({
        url: '/ProposalSSRSReports/ProposalSSRSReports/GetTechAgencyName_ByAgencyType?type=' + $("#ddStaPta_PropScruitinyDetails").val() + '&' + 'stateCode=' + $("#ddState_PropScruitinyDetails").val(),
        type: 'POST',
        // data: { "Type": $("#ddStaPta_PropScruitinyDetails").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddAgency_PropScruitinyDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}