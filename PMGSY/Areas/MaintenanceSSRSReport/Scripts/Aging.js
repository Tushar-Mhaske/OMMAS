$(document).ready(function () {



    $.validator.unobtrusive.parse($('#frmPackageWiseReport'));
    $("#StateList_PackageWiseDetails").change(function () {
        loadDistrict($("#StateList_PackageWiseDetails").val());

    });

    $("#ID_EXP_END_RANGE").val(100000000);


    $("#DistrictList_PackageWiseDetails").change(function () {
        loadBlock($("#StateList_PackageWiseDetails").val(), $("#DistrictList_PackageWiseDetails").val());

    });
    $("#btnViewPackageWiseDetails").click(function () {


        var start = $("#ID_EXP_START_RANGE").val();
        var end = $("#ID_EXP_END_RANGE").val();

        if (start < 0 || start > 100000000)
        {
            alert("Exp. Start Range can be between 0 and 100000000");
            return false;

        }
        else if (start > end)
        {
            alert("Exp. Start Range can not be greater than Exp. End Range");
            return false;
        }
        else if (end < 0 || end > 100000000)
        {
            alert("Exp. End Range can be between 0 and 100000000");
            return false;
        }
        if ($('#RoadWiseCheck_PackageWiseDetails').prop("checked") == true) {
            if ($("#StateList_PackageWiseDetails").is(":visible")) {
                if ($("#StateList_PackageWiseDetails").val() == 0) {
                    alert("Please select State , District and Block");
                    return false;
                }
            }
            if ($("#DistrictList_PackageWiseDetails").is(":visible")) {
                if ($("#DistrictList_PackageWiseDetails").val() == 0) {
                    alert("Please select District and Block.");
                    return false;
                }
            }
            if ($("#BlockList_PackageWiseDetails").is(":visible")) {
                if ($("#DistrictList_PackageWiseDetails").val() == 0) {
                    alert("Please select Block.");
                    return false;
                }
            }
        }
        if ($('#frmPackageWiseReport').valid()) {
            $("#loadPackageWiseReport").html("");

            if ($("#StateList_PackageWiseDetails").is(":visible")) {

                $("#StateName").val($("#StateList_PackageWiseDetails option:selected").text());
            }

            if ($("#DistrictList_PackageWiseDetails").is(":visible")) {

                //$('#DistrictList_PackageWiseDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_PackageWiseDetails option:selected").text());
            }
            if ($("#BlockList_PackageWiseDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_PackageWiseDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/AgingReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPackageWiseReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadPackageWiseReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }

    });

    $("#btnViewPackageWiseDetails").trigger('click');

    // $("#btnViewPackageWiseDetails").trigger('click');
    if ($('#Mast_Block_Code').val() > 0) {
        $("#btnViewPackageWiseDetails").trigger('click');
    }
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPackageWiseDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_PackageWiseDetails").val(0);
    $("#DistrictList_PackageWiseDetails").empty();
    $("#BlockList_PackageWiseDetails").val(0);
    $("#BlockList_PackageWiseDetails").empty();
    $("#BlockList_PackageWiseDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_PackageWiseDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/DistrictSelectDetailsForExp',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_PackageWiseDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_PackageWiseDetails').find("option[value='0']").remove();
                    //$("#DistrictList_PackageWiseDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_PackageWiseDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_PackageWiseDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_PackageWiseDetails").attr("disabled", "disabled");
                        $("#DistrictList_PackageWiseDetails").trigger('change');
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

        $("#DistrictList_PackageWiseDetails").append("<option value='0'>Select Districts</option>");
        $("#BlockList_PackageWiseDetails").empty();
        $("#BlockList_PackageWiseDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_PackageWiseDetails").val(0);
    $("#BlockList_PackageWiseDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_PackageWiseDetails").length > 0) {
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/BlockDetailsForExp',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_PackageWiseDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_PackageWiseDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_PackageWiseDetails").attr("disabled", "disabled");
                        //$("#BlockList_PackageWiseDetails").trigger('change');
                    }
                    //$('#BlockList_PackageWiseDetails').find("option[value='0']").remove();
                    //$("#BlockList_PackageWiseDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_PackageWiseDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_PackageWiseDetails").append("<option value='0'>All Blocks</option>");
    }
}