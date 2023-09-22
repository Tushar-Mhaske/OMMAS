$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPCIOverdueReport'));

    $("#StateList_PCIOverdueDetails").change(function () {
        loadDistrict($("#StateList_PCIOverdueDetails").val());

    });

    $("#DistrictList_PCIOverdueDetails").change(function () {
        loadBlock($("#StateList_PCIOverdueDetails").val(), $("#DistrictList_PCIOverdueDetails").val());

    });
    $("#btnViewPCIOverdueDetails").click(function () {

        if ($('#RoadWiseCheck_PCIOverdueDetails').prop("checked") == true) {
            if ($("#StateList_PCIOverdueDetails").is(":visible")) {
                if ($("#StateList_PCIOverdueDetails").val() == 0) {
                    alert("Please select State");
                    return false;
                }
            }
        }
        if ($('#frmPCIOverdueReport').valid()) {
            $("#loadPCIOverdueReport").html("");

            $("#TypeName").val($("#TypeList_PCIOverdueDetailss option:selected").text());
            if ($("#StateList_PCIOverdueDetails").is(":visible")) {

                $("#StateName").val($("#StateList_PCIOverdueDetails option:selected").text());
            }

            if ($("#DistrictList_PCIOverdueDetails").is(":visible")) {

                //$('#DistrictList_PCIOverdueDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_PCIOverdueDetails option:selected").text());
            }
            if ($("#BlockList_PCIOverdueDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_PCIOverdueDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/RoadPCIOverdueMaintReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPCIOverdueReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadPCIOverdueReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        else {

        }
    });

    //$("#btnViewPCIOverdueDetails").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPCIOverdueDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_PCIOverdueDetails").val(0);
    $("#DistrictList_PCIOverdueDetails").empty();
    $("#BlockList_PCIOverdueDetails").val(0);
    $("#BlockList_PCIOverdueDetails").empty();
    $("#BlockList_PCIOverdueDetails").append("<option value='0'>Select Block</option>");

    if (statCode > 0) {
        if ($("#DistrictList_PCIOverdueDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_PCIOverdueDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_PCIOverdueDetails').find("option[value='0']").remove();
                    //$("#DistrictList_PCIOverdueDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_PCIOverdueDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_PCIOverdueDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_PCIOverdueDetails").attr("disabled", "disabled");
                        $("#DistrictList_PCIOverdueDetails").trigger('change');
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

        $("#DistrictList_PCIOverdueDetails").append("<option value='0'>Select District</option>");
        $("#BlockList_PCIOverdueDetails").empty();
        $("#BlockList_PCIOverdueDetails").append("<option value='0'>Select Block</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_PCIOverdueDetails").val(0);
    $("#BlockList_PCIOverdueDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_PCIOverdueDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_PCIOverdueDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_PCIOverdueDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_PCIOverdueDetails").attr("disabled", "disabled");
                        //$("#BlockList_PCIOverdueDetails").trigger('change');
                    }
                    //$('#BlockList_PCIOverdueDetails').find("option[value='0']").remove();
                    //$("#BlockList_PCIOverdueDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_PCIOverdueDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_PCIOverdueDetails").append("<option value='0'>Select Block</option>");
    }
}