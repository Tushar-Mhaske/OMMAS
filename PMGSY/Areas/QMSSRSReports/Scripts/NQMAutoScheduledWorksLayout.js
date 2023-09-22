$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmNQMAutoScheduledWorksLayout");

    $('#btnView').click(function () {
        //if (!$('#frmNQMAutoScheduledWorksLayout').valid()) {
        //    return false;
        //}

        $("#StateName").val($("#ddlState option:selected").text());

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/NQMAutoScheduledWorksReport/',
            type: 'POST',
            catche: false,
            data: $("#frmNQMAutoScheduledWorksLayout").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#divNQMAutoScheduledWorksReport").html(response);
            },
            error: function () {
                $.unblockUI();
                alert("Error ocurred");
                return false;
            },
        });
    });

});