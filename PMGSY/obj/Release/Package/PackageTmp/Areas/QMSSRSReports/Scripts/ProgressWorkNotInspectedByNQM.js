$(document).ready(function () {

    // $.validator.unobtrusive.parse($('#frmQMMonthwiseInspectionsLayout'));

    $("#btnViewCluster").click(function () {
       
        if ($("#ddlStateCluster option:selected").val() > 0) {
            $('#StateName').val($("#ddlStateCluster option:selected").text());
        }
        if ($('#frmClusterReportLayout').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/ProgressWorkByNQMReportPost',
                type: 'POST',
                catche: false,
                data: $("#frmClusterReportLayout").serialize(),
                async: false,
                success: function (response) {

                    $.unblockUI();
                    $("#loadClusterReport").html(response);
                },
                error: function () {

                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });

    
});