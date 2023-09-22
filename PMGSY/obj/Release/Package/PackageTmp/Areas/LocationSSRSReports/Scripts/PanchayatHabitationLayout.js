$(document).ready(function () {
    if ($("#Mast_State_Code").val() > 0) {

        $("#StateList_PanchayatHabitationDetails").attr("disabled", "disabled");
    }
    $("#StateList_PanchayatHabitationDetails").change(function () {

        $("#DistrictList_PanchayatHabitationDetails").val(0);
        $("#DistrictList_PanchayatHabitationDetails").empty();

        //$("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {
            if ($("#DistrictList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/PanchayatHabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        if ($("#Mast_District_Code").val() > 0) {
                            $("#DistrictList_PanchayatHabitationDetails").val($("#Mast_District_Code").val());
                            $("#DistrictList_PanchayatHabitationDetails").attr("disabled", "disabled");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#DistrictList_PanchayatHabitationDetails").append("<option value='0'>All Districts</option>");
            $("#BlockList_PanchayatHabitationDetails").val(0);
            $("#BlockList_PanchayatHabitationDetails").empty();
            $("#BlockList_PanchayatHabitationDetails").append("<option value='0'>All Blocks</option>");
            $("#PanchayatList_PanchayatHabitationDetails").val(0);
            $("#PanchayatList_PanchayatHabitationDetails").empty();
            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>All Panchayats</option>");
        }
    });

    $("#DistrictList_PanchayatHabitationDetails").change(function () {

        $("#BlockList_PanchayatHabitationDetails").val(0);
        $("#BlockList_PanchayatHabitationDetails").empty();

        if ($(this).val() > 0) {
            if ($("#BlockList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/PanchayatHabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatHabitationDetails").val(), "DistrictCode": $("#DistrictList_PanchayatHabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            //alert("Val=0");
            $("#BlockList_PanchayatHabitationDetails").empty();
            $("#BlockList_PanchayatHabitationDetails").append("<option value='0'>All Blocks</option>");

        }
    });

    $("#BlockList_PanchayatHabitationDetails").change(function () {

        $("#PanchayatList_PanchayatHabitationDetails").val(0);
        $("#PanchayatList_PanchayatHabitationDetails").empty();

        //$("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>Select Panchayat</option>");


        if ($(this).val() > 0) {
            if ($("#PanchayatList_PanchayatHabitationDetails").length > 0) {
                $.ajax({
                    url: '/LocationSSRSReports/LocationSSRSReports/PanchayatHabitationLayout',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_PanchayatHabitationDetails").val(), "DistrictCode": $("#DistrictList_PanchayatHabitationDetails").val(), "BlockCode": $("#BlockList_PanchayatHabitationDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

            $("#PanchayatList_PanchayatHabitationDetails").empty();
            $("#PanchayatList_PanchayatHabitationDetails").append("<option value='0'>All Panchayats</option>");


        }

    });
    $("#PanchayatHabitationDetailsButton").click(function () {
        var stateCode = $("#StateList_PanchayatHabitationDetails").val();
        var districtCode = $("#DistrictList_PanchayatHabitationDetails").val();
        var blockCode = $("#BlockList_PanchayatHabitationDetails").val();
        var activeType = $("#ActiveType_PanchayatHabitationDetails").val();

        if (stateCode == 0) {
            alert("Please Select State");
            return;
        }
        if (districtCode == 0) {
            alert("Please Select District");
            return;
        }
        if (blockCode == 0) {
            alert("Please Select Block");
            return;
        }

        $('#State').val($("#StateList_PanchayatHabitationDetails option:selected").val());
        $('#District').val($("#DistrictList_PanchayatHabitationDetails option:selected").val());
        $('#Block').val($("#BlockList_PanchayatHabitationDetails option:selected").val());
        $('#ActiveFlag').val($("#ActiveType_PanchayatHabitationDetails option:selected").val());
        $('#Panchayat').val($("#PanchayatList_PanchayatHabitationDetails option:selected").val());

        $('#StateName').val($("#StateList_PanchayatHabitationDetails option:selected").text());
        $('#DistName').val($("#DistrictList_PanchayatHabitationDetails option:selected").text());
        $('#BlockName').val($("#BlockList_PanchayatHabitationDetails option:selected").text());
        $('#ActiveFlagName').val($("#ActiveType_PanchayatHabitationDetails option:selected").text());
        $('#PanchayatName').val($("#PanchayatList_PanchayatHabitationDetails option:selected").text());

        if ($('#frmPanchayatHabitationLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/PanchayatHabitationReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPanchayatHabitationLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadPanchayatHabitationReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#StateList_PanchayatHabitationDetails").trigger('change');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmPanchayatHabitationLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
