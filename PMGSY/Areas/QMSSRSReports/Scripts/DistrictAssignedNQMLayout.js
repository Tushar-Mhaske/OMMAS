$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAllocateDistrictsToNQMLayout");

    $('#btnView').click(function () {
        //if (!$('#frmAllocateDistrictsToNQMLayout').valid()) {
        //    return false;
        //}

        $("#StateName").val($("#ddlState option:selected").text());

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/DistrictAssignedNQMReport/',
            type: 'POST',
            catche: false,
            data: $("#frmAllocateDistrictsToNQMLayout").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#divAllocateDistrictsToNQMReport").html(response);
            },
            error: function () {
                $.unblockUI();
                alert("Error ocurred");
                return false;
            },
        });
    });

});