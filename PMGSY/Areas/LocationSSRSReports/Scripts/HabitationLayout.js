$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmHabitationLayout'));

    if ($("#Mast_State_Code").val() > 0) {
        $("#StateList_HabitationDetails").attr("disabled", "disabled");
    }

    if ($("#Mast_District_Code").val() > 0) {

        $("#DistrictList_HabitationDetails").val($("#Mast_District_Code").val());
        $("#DistrictList_HabitationDetails").attr("disabled", "disabled");
    }


    $("#StateList_HabitationDetails").change(function () {
        $("#DistrictList_HabitationDetails").val(0);
        $("#DistrictList_HabitationDetails").empty();
        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#DistrictList_HabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/HabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_HabitationDetails").val() },
                    success: function (jsonData) {

                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#Mast_District_Code").val() > 0) {

                            $("#DistrictList_HabitationDetails").val($("#Mast_District_Code").val());
                            $("#DistrictList_HabitationDetails").attr("disabled", "disabled");
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
            $("#DistrictList_HabitationDetails").append("<option value='0'>Select District</option>");
            $("#BlockList_HabitationDetails").val(0);
            $("#BlockList_HabitationDetails").empty();
            $("#BlockList_HabitationDetails").append("<option value='0'>Select Block</option>");
            $("#VillageList_HabitationDetails").val(0);
            $("#VillageList_HabitationDetails").empty();
            $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
        }
    });

    $("#DistrictList_HabitationDetails").change(function () {
        $("#BlockList_HabitationDetails").val(0);
        $("#BlockList_HabitationDetails").empty();
        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#BlockList_HabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/HabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_HabitationDetails").val(), "DistrictCode": $("#DistrictList_HabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#BlockList_HabitationDetails").val(0);
            $("#BlockList_HabitationDetails").empty();
            $("#BlockList_HabitationDetails").append("<option value='0'>Select Block</option>");
            $("#VillageList_HabitationDetails").val(0);
            $("#VillageList_HabitationDetails").empty();
            $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
        }
    });

    $("#BlockList_HabitationDetails").change(function () {
        $("#VillageList_HabitationDetails").val(0);
        $("#VillageList_HabitationDetails").empty();
        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#VillageList_HabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/HabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_HabitationDetails").val(), "DistrictCode": $("#DistrictList_HabitationDetails").val(), "BlockCode": $("#BlockList_HabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#VillageList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#VillageList_HabitationDetails").val(0);
            $("#VillageList_HabitationDetails").empty();
            $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
        }
    });

    $("#HabitationDetailsButton").click(function () {

        $('#State').val($("#StateList_HabitationDetails option:selected").val());
        $('#Block').val($("#BlockList_HabitationDetails option:selected").val());
        $('#District').val($("#DistrictList_HabitationDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_HabitationDetails option:selected").val());
        $('#CensusYear').val($("#CENSUS_YEAR_HabitationDetails option:selected").val());
        $('#Schedule').val($("#IS_SCHEDULE5_VillageDetails option:selected").val());
        $('#Village').val($("#VillageList_HabitationDetails").val());
        $('#HabitationStatus').val($("#HAB_STATUS_HabitationDetails").val());

        if ($('#frmHabitationLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/HabitationReport/',
                type: 'POST',
                catche: false,
                data: $("#frmHabitationLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadHabitationReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#HabitationDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmHabitationLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});