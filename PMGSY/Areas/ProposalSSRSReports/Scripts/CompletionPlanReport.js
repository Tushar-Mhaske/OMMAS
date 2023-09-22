$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmCompletionPlan'));

    $("#btnViewCompletionPlan").click(function () {

        if ($("#ddlStates").val() > 0) {
            $("#StateName").val($("#ddlStates option:selected").text());
        }

        if ($("#ddlYears").val() > 0) {
            $("#YearText").val($("#ddlYears").text());
        }


        if ($("#frmCompletionPlan").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProposalSSRSReports/ProposalSSRSReports/CompletionPlanReport/',
                type: 'POST',
                catche: false,
                data: $("#frmCompletionPlan").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadCompPlanReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });
        }
        else {
            return false;
        }

    });

    closableNoteDiv("divCommonReport", "spnCommonReport");

});