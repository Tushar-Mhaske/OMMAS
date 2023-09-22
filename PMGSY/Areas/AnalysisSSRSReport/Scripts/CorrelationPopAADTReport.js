$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmCorrelationPopAADTReport'));
    $("#StateList_CorrelationPopAADTDetails").change(function () {
        loadDistrict($("#StateList_CorrelationPopAADTDetails").val());

    });

    $("#DistrictList_CorrelationPopAADTDetails").change(function () {
        loadBlock($("#StateList_CorrelationPopAADTDetails").val(), $("#DistrictList_CorrelationPopAADTDetails").val());

    });
    $("#btnViewCorrelationPopAADTDetails").click(function () {

        if ($('#RoadWiseCheck_CorrelationPopAADTDetails').prop("checked") == true) {
            if ($("#StateList_CorrelationPopAADTDetails").is(":visible")) {
                if ($("#StateList_CorrelationPopAADTDetails").val() == 0) {
                    alert("Please select State , District and Block");
                    return false;
                }
            }
            if ($("#DistrictList_CorrelationPopAADTDetails").is(":visible")) {
                if ($("#DistrictList_CorrelationPopAADTDetails").val() == 0) {
                    alert("Please select District and Block.");
                    return false;
                }
            }
            if ($("#BlockList_CorrelationPopAADTDetails").is(":visible")) {
                if ($("#DistrictList_CorrelationPopAADTDetails").val() == 0) {
                    alert("Please select Block.");
                    return false;
                }
            }
        }
        if ($('#frmCorrelationPopAADTReport').valid()) {
            $("#loadCorrelationPopAADTReport").html("");

            if ($("#StateList_CorrelationPopAADTDetails").is(":visible")) {

                $("#StateName").val($("#StateList_CorrelationPopAADTDetails option:selected").text());
            }

            if ($("#DistrictList_CorrelationPopAADTDetails").is(":visible")) {

                //$('#DistrictList_CorrelationPopAADTDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_CorrelationPopAADTDetails option:selected").text());
            }
            if ($("#BlockList_CorrelationPopAADTDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_CorrelationPopAADTDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/CorrelationBetPopAADTReport/',
                type: 'POST',
                catche: false,
                data: $("#frmCorrelationPopAADTReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadCorrelationPopAADTReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }
        
    });

   // $("#btnViewCorrelationPopAADTDetails").trigger('click');
    if ($('#Mast_Block_Code').val() > 0) {
        $("#btnViewCorrelationPopAADTDetails").trigger('click');
    }
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewCorrelationPopAADTDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});

function loadDistrict(statCode) {
    $("#DistrictList_CorrelationPopAADTDetails").val(0);
    $("#DistrictList_CorrelationPopAADTDetails").empty();
    $("#BlockList_CorrelationPopAADTDetails").val(0);
    $("#BlockList_CorrelationPopAADTDetails").empty();
    $("#BlockList_CorrelationPopAADTDetails").append("<option value='0'>Select Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_CorrelationPopAADTDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/DistrictSelectDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_CorrelationPopAADTDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_CorrelationPopAADTDetails').find("option[value='0']").remove();
                    //$("#DistrictList_CorrelationPopAADTDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_CorrelationPopAADTDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_CorrelationPopAADTDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_CorrelationPopAADTDetails").attr("disabled", "disabled");
                        $("#DistrictList_CorrelationPopAADTDetails").trigger('change');
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

        $("#DistrictList_CorrelationPopAADTDetails").append("<option value='0'>Select Districts</option>");
        $("#BlockList_CorrelationPopAADTDetails").empty();
        $("#BlockList_CorrelationPopAADTDetails").append("<option value='0'>Select Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_CorrelationPopAADTDetails").val(0);
    $("#BlockList_CorrelationPopAADTDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_CorrelationPopAADTDetails").length > 0) {
            $.ajax({
                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockSelectDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_CorrelationPopAADTDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_CorrelationPopAADTDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_CorrelationPopAADTDetails").attr("disabled", "disabled");
                        //$("#BlockList_CorrelationPopAADTDetails").trigger('change');
                    }
                    //$('#BlockList_CorrelationPopAADTDetails').find("option[value='0']").remove();
                    //$("#BlockList_CorrelationPopAADTDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_CorrelationPopAADTDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_CorrelationPopAADTDetails").append("<option value='0'>Select Blocks</option>");
    }
}