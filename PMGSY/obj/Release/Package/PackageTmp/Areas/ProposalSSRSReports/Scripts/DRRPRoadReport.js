$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDRRPRoadReport'));

    $("#btnViewDRRPRoadReport").click(function () {      
        loadReport();
    });

    

    $("#ddlStates").change(function () {
        loadDistrict($("#ddlStates option:selected").val());
    });

    loadReport();
 
});

function loadReport() {

    if ($("#frmDRRPRoadReport").valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/ProposalSSRSReports/ProposalSSRSReports/DRRPRoadReport/',
            type: 'POST',
            catche: false,
            data: $("#frmDRRPRoadReport").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadDRRPRoadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("Error");
                return false;
            },
        });

    }
    else {
        return false;
    }

}
function loadDistrict(statCode) {
    $("#ddlDistricts").val(0);
    $("#ddlDistricts").empty();

    if (statCode > 0) {
        if ($("#ddlDistricts").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistricts").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //For Disable if District Login
                    if ($("#DistrictCode").val() > 0) {
                        $("#ddlDistricts").val($("#DistrictCode").val());                     
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

        $("#ddlDistricts").append("<option value='0'>All Districts</option>");
      
    }
}

