$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmPavmentCondThroghReport'));
    $("#StateList_PavmentCondThroghDetails").change(function () {
        loadDistrict($("#StateList_PavmentCondThroghDetails").val());

    });

    $("#DistrictList_PavmentCondThroghDetails").change(function () {
        loadBlock($("#StateList_PavmentCondThroghDetails").val(), $("#DistrictList_PavmentCondThroghDetails").val());

    });
    $("#btnViewPavmentCondThroghDetails").click(function () {
        //if ($('#RoadWiseCheck_PavmentCondThroghDetails').prop("checked") == true) {
        //    if ($("#StateList_PavmentCondThroghDetails").is(":visible")) {
        //        if ($("#StateList_PavmentCondThroghDetails").val() == 0) {
        //            alert("Please select State , District and Block");
        //            return false;
        //        }
        //    }
        //    if ($("#DistrictList_PavmentCondThroghDetails").is(":visible")) {
        //        if ($("#DistrictList_PavmentCondThroghDetails").val() == 0) {
        //            alert("Please select District and Block.");
        //            return false;
        //        }
        //    }
        //    if ($("#BlockList_PavmentCondThroghDetails").is(":visible")) {
        //        if ($("#DistrictList_PavmentCondThroghDetails").val() == 0) {
        //            alert("Please select Block.");
        //            return false;
        //        }
        //    }
        //}
       
        if ($('#frmPavmentCondThroghReport').valid()) {
            $("#loadPavmentCondThroghReport").html("");

            if ($("#StateList_PavmentCondThroghDetails").is(":visible")) {

                $("#StateName").val($("#StateList_PavmentCondThroghDetails option:selected").text());
            }

            if ($("#DistrictList_PavmentCondThroghDetails").is(":visible")) {

                //$('#DistrictList_PavmentCondThroghDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_PavmentCondThroghDetails option:selected").text());
            }
            if ($("#BlockList_PavmentCondThroghDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_PavmentCondThroghDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/PavementConditionThroughRouteReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPavmentCondThroghReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadPavmentCondThroghReport").html(response);

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

   // $("#btnViewPavmentCondThroghDetails").trigger('click');

    if ($('#Mast_Block_Code').val() > 0) {
        $("#btnViewPavmentCondThroghDetails").trigger('click');
    }
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPavmentCondThroghDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_PavmentCondThroghDetails").val(0);
    $("#DistrictList_PavmentCondThroghDetails").empty();
    $("#BlockList_PavmentCondThroghDetails").val(0);
    $("#BlockList_PavmentCondThroghDetails").empty();
    $("#BlockList_PavmentCondThroghDetails").append("<option value='0'>Select Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_PavmentCondThroghDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_PavmentCondThroghDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_PavmentCondThroghDetails').find("option[value='0']").remove();
                    //$("#DistrictList_PavmentCondThroghDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_PavmentCondThroghDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_PavmentCondThroghDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_PavmentCondThroghDetails").attr("disabled", "disabled");
                        $("#DistrictList_PavmentCondThroghDetails").trigger('change');
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

        $("#DistrictList_PavmentCondThroghDetails").append("<option value='0'>Select Districts</option>");
        $("#BlockList_PavmentCondThroghDetails").empty();
        $("#BlockList_PavmentCondThroghDetails").append("<option value='0'>Select Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_PavmentCondThroghDetails").val(0);
    $("#BlockList_PavmentCondThroghDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_PavmentCondThroghDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_PavmentCondThroghDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_PavmentCondThroghDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_PavmentCondThroghDetails").attr("disabled", "disabled");
                        //$("#BlockList_PavmentCondThroghDetails").trigger('change');
                    }
                    //$('#BlockList_PavmentCondThroghDetails').find("option[value='0']").remove();
                    //$("#BlockList_PavmentCondThroghDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_PavmentCondThroghDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_PavmentCondThroghDetails").append("<option value='0'>Select Blocks</option>");
    }
}