$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmClusterReportLayout'));

    $("#btnViewCluster").trigger('click');

    $("#spCollapseIconClusterReport").click(function () {

        $("#spCollapseIconClusterReport").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmClusterReportLayout").toggle("slow");

    });

    $('#ddlStateCluster').change(function () {
        loadDistricts();
    });

    $("#btnViewCluster").click(function () {
        if ($('#frmClusterReportLayout').valid()) {

            //if ($('#ddlStateCluster').val() > 0) {
            //    $('#stateName').val($("#ddlStateCluster option:selected").text());
            //}
            //if ($('#ddlDistrictCUPLPriority').val() > 0) {
            //    $('#districtName').val($("#ddlDistrictCUPLPriority option:selected").text());
            //}
            if ($("#ddlStateCluster").val() <=0) {
                alert("Please Select State");
                return false;
            }


            if ($("#ddlDistrictCluster").val() <= 0) {
                alert("Please Select District");
                return false;
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Feedback/Feedback/DistrictBriefReport/',
                type: 'POST',
                catche: false,
                data: $("#frmClusterReportLayout").serialize(),
                async: false,
                success: function (response) {
                    //if ($("#hdnRole").val() == 8) {
                    //    $("#ddlStateCluster").attr('disabled', true);
                    //}
                    $.unblockUI();
                    $("#loadClusterReport").html(response);
                },
                error: function () {
                    if ($("#hdnRole").val() == 8) {
                        $("#ddlStateCluster").attr('disabled', true);
                    }
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });
});

function loadDistricts() {
    $("#ddlDistrictCluster").empty();
    if ($('#ddlStateCluster').val() > 0) {

        if ($("#ddlDistrictCluster").length > 0) {
            $.ajax({
                url: '/Feedback/Feedback/DistrictDetails',
                type: 'POST',
                data: { "StateCode": $('#ddlStateCluster').val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrictCluster").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$("#ddlDistrictCluster option:eq(0)").val(-1);
                    //$("#ddlDistrictCluster option:eq(0)").text("Select District");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#ddlDistrictCluster").append("<option value='0'>Select District</option>");
        //$("#ddlBlockCUPLPriority").empty();
        //$("#ddlBlockCUPLPriority").append("<option value='0'>All Blocks</option>");
    }
}