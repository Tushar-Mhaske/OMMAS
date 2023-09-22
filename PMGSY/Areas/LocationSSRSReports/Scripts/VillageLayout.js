$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmVillageLayout'));

    if ($("#Mast_State_Code").val() > 0) {
        $("#StateList_VillageDetails").attr("disabled", "disabled");
    }
    if ($("#Mast_District_Code").val() > 0) {
        $("#DistrictList_VillageDetails").attr("disabled", "disabled");
    }

    $("#StateList_VillageDetails").change(function () {

        //$("#DistrictList_VillageDetails").val(0);
        $("#DistrictList_VillageDetails").empty();
        $("#BlockList_VillageDetails").val(0);
        $("#BlockList_VillageDetails").empty();
        $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");

        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#DistrictList_VillageDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/VillageLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_VillageDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_VillageDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        if ($("#Mast_District_Code").val() > 0) {
                            $("#DistrictList_VillageDetails").val($("#Mast_District_Code").val());
                            $("#DistrictList_VillageDetails").attr("disabled", "disabled");
                            $("#DistrictList_VillageDetails").trigger('change');
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
            $("#DistrictList_VillageDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_VillageDetails").val(0);
            $("#BlockList_VillageDetails").empty();
            $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#DistrictList_VillageDetails").change(function () {
        $("#BlockList_VillageDetails").val(0);
        $("#BlockList_VillageDetails").empty();
        //$("#DistrictList").append("<option value='0'>Select District</option>");
        if ($(this).val() > 0) {
            if ($("#BlockList_VillageDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/VillageLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_VillageDetails").val(), "DistrictCode": $("#DistrictList_VillageDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_VillageDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#BlockList_VillageDetails").append("<option value='0'>All Blocks</option>");
        }
    });

    $("#VillageDetailsButton").click(function () {

        $('#State').val($("#StateList_VillageDetails option:selected").val());
        $('#Block').val($("#BlockList_VillageDetails option:selected").val());
        $('#District').val($("#DistrictList_VillageDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_VillageDetails option:selected").val());
        $('#CensusYear').val($("#CENSUS_YEAR_VillageDetails option:selected").val());
        $('#Schedule').val($("#IS_SCHEDULE5_VillageDetails option:selected").val());

        $('#StateName').val($("#StateList_VillageDetails option:selected").text());
        $('#DistName').val($("#DistrictList_VillageDetails option:selected").text());
        $('#BlockName').val($("#BlockList_VillageDetails option:selected").text());

        if ($('#frmVillageLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/VillageReport/',
                type: 'POST',
                catche: false,
                data: $("#frmVillageLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadVillageReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
        
    });
    //$('#StateList_VillageDetails').trigger('change'); //change 11/12/2013
    $("#VillageDetailsButton").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmVillageLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
