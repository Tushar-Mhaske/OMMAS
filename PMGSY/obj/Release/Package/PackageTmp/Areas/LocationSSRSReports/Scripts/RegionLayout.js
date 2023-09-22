$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmRegionLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_RegionDetails").attr("disabled", "disabled");
    }
    $("#RegionDetailsButton").click(function () {

        $('#State').val($("#StateList_RegionDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_RegionDetails option:selected").val());

        $('#StateName').val($("#StateList_RegionDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_RegionDetails option:selected").text());

        if ($('#frmRegionLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/RegionReport/',
                type: 'POST',
                catche: false,
                data: $("#frmRegionLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadRegionReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });
    $("#RegionDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmRegionLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

