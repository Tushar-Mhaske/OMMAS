$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmSanctionedRoadList'));


    $("#btnView").click(function () {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/ProposalSSRSReports/ProposalSSRSReports/SanctionedRoadListReport/',
            type: 'POST',
            catche: false,
            data: $("#frmSanctionedRoadList").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    });
});