$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmBenefittedHabsFacilityLayout'));

    $("#btnView").click(function () {
        $("#stateName").val($('#ddlState option:selected').text());

        window.open('/OtherReports/OtherReports/BenefittedHabsFacilityExcelReport?rpt=' + $('#ddlState').val() + "$" + $("#ddlYear option:selected").val()
                       + "$" + $("#ddlBatch option:selected").val() + "$" + $("#ddlState option:selected").text(), '_blank');

        //$.ajax({
        //    url: '/OtherReports/OtherReports/BenefittedHabsFacilityExcelReport',
        //    type: 'POST',
        //    beforeSend: function () {
        //        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        //    },
        //    data: $("#frmBenefittedHabsFacilityLayout").serialize(),
        //    success: function (jsonData) {
        //        window.open(jsonData, blank);

        //        $.unblockUI();
        //    },
        //    error: function (err) {
        //        //alert("error " + err);
        //        $.unblockUI();
        //    }
        //});
    });

});
