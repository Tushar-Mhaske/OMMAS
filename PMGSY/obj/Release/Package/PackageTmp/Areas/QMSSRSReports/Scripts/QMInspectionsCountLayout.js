$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMInspectionsCountLayout'));
    
    $("#btnViewQMInspCount").click(function () {
        if ($('#frmQMInspectionsCountLayout').valid()) {

            //$('#StateName').val($('#ddlStateMonthwiseComparision option:selected').text());

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMInspectionsCountReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMInspectionsCountLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadQMInspectionsCountReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });


    $("#btnViewQMInspCount").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMInspectionsCountLayout").toggle("slow");

    });
});
