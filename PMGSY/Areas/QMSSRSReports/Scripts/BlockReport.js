$(document).ready(function () {

   // $.validator.unobtrusive.parse($('#frmQMMonthwiseInspectionsLayout'));

    $("#btnViewCluster").click(function () {
        if ($('#frmClusterReportLayout').valid()) {
            if ($("#ddlStateCluster option:selected").val() > 0) {
                $('#StateName').val($("#ddlStateCluster option:selected").text());
                $('#DistName').val($("#ddlDistrictCluster option:selected").text());
                $('#BlockName').val($("#ddlBlockCluster option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/BlockReportPost',
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




$("#ddlDistrictCluster").change(function () {
    $("#ddlBlockCluster").val(0);
    $("#ddlBlockCluster").empty();
    //$("#DistrictList").append("<option value='0'>Select District</option>");
    if ($(this).val() > 0) {
        if ($("#ddlBlockCluster").length > 0) {
            $.ajax({
                url: '/MasterReports/HabitationDetails',
                type: 'POST',
                data: { "StateCode": $("#ddlStateCluster").val(), "DistrictCode": $("#ddlDistrictCluster").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlBlockCluster").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {
        $("#ddlBlockCluster").val(0);
        $("#ddlBlockCluster").empty();
        $("#ddlBlockCluster").append("<option value='0'>Select Block</option>");
       
    }
});

$('#ddlStateCluster').change(function () {
    loadDistricts();
});

function loadDistricts() {
    $("#ddlDistrictCluster").empty();
    if ($('#ddlStateCluster').val() > 0) {

        if ($("#ddlDistrictCluster").length > 0) {
            $.ajax({
                url: '/ECBriefReport/ECBriefReport/DistrictDetails',
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

        $("#ddlDistrictCluster").append("<option value='0'>All Districts</option>");
        //$("#ddlBlockCUPLPriority").empty();
        //$("#ddlBlockCUPLPriority").append("<option value='0'>All Blocks</option>");
    }
}

});


