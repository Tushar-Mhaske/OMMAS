$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDistrictLayout'));

    $("#DistrictDetailsButton").click(function () {

        $('#Territory').val($("#StateList_DistrictDetails option:selected").val());
        $('#StateType').val($("#PMGSY_INCLUDED_DistrictDetails option:selected").val()); 
        $('#iapDistrict').val($("#IAP_DISTRICT_DistrictDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_DistrictDetails option:selected").val());

        if ($('#frmDistrictLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/DistrictReport/',
                type: 'POST',
                catche: false,
                data: $("#frmDistrictLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadDistrictReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#DistrictDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmDistrictLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
