$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmRegionDistrictLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_RegionDistrictDetails").attr("disabled", "disabled");
    }
    $("#StateList_RegionDistrictDetails").change(function () {
        $("#RegionList_RegionDistrictDetails").val(0);
        $("#RegionList_RegionDistrictDetails").empty();
        if ($(this).val() > 0) {
            if ($("#RegionList_RegionDistrictDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/RegionDistrictLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_RegionDistrictDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#RegionList_RegionDistrictDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#RegionList_RegionDistrictDetails").append("<option value='0'>All Regions</option>");
        }
    });

    $("#RegionDistrictDetailsButton").click(function () {
        $('#State').val($("#StateList_RegionDistrictDetails option:selected").val());
        $('#Region').val($("#RegionList_RegionDistrictDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_RegionDistrictDetails option:selected").val());
        
        $('#StateName').val($("#StateList_RegionDistrictDetails option:selected").text());
        $('#RegionName').val($("#RegionList_RegionDistrictDetails option:selected").text());
        //$('#ActiveFlagName').val($("#ActiveType_RegionDistrictDetails option:selected").text());

        if ($('#frmRegionDistrictLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/RegionDistrictReport/',
                type: 'POST',
                catche: false,
                data: $("#frmRegionDistrictLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadRegionDistrictReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }

    });


    //$("#StateList_RegionDistrictDetails").trigger('change');
    $("#RegionDistrictDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmRegionDistrictLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");

});
