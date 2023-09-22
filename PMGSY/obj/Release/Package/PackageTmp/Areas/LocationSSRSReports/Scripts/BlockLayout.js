
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmBlockLayout'));

    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_BlockDetails").attr("disabled", "disabled");
    }

    //if ($("#Mast_District_Code").val() > 0) {

    //    $("#DistrictList_BlockDetails").attr("disabled", "disabled");
    //}

    $("#StateList_BlockDetails").change(function () {
        $("#DistrictList_BlockDetails").val(0);
        $("#DistrictList_BlockDetails").empty();
        if ($(this).val() > 0) {
            if ($("#DistrictList_BlockDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/DistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_BlockDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_BlockDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#Mast_District_Code").val() > 0) {
                            $("#DistrictList_BlockDetails").val($("#Mast_District_Code").val());
                            $("#DistrictList_BlockDetails").attr("disabled", "disabled");
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
            $("#DistrictList_BlockDetails").append("<option value='0'>All Districts</option>");
        }
    });

    $("#BlockDetailsButton").click(function () {

        $('#stateCode').val($("#StateList_DistrictDetails option:selected").val());
        $('#pmgsyIncluded').val($("#PMGSY_INCLUDED_DistrictDetails option:selected").val());
        $('#districtCode').val($("#DistrictList_BlockDetails option:selected").val());
        $('#activeType').val($("#ActiveType_BlockDetails option:selected").val());
        $('#isDesert').val($("#IS_DESERT_BlockDetails option:selected").val());
        $('#isTribal').val($("#IS_TRIBAL_BlockDetails option:selected").val());
        $('#schedule5').val($("#IS_SCHEDULE5_BlockDetails option:selected").val());

        if ($('#frmBlockLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/BlockReport/',
                type: 'POST',
                catche: false,
                data: $("#frmBlockLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadBlockReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#StateList_BlockDetails").trigger('change');
    $("#BlockDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmBlockLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});