$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPhyProgessWork'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#loadFilters").toggle("slow");

    });

    $.validator.unobtrusive.parse($('#frmPhyProgessWork'));
    $("#StateList_PhyProgressWorkDetails").change(function () {
        loadDistrict($("#StateList_PhyProgressWorkDetails").val());

    });

    $("#DistrictList_PhyProgressWorkDetails").change(function () {
        loadBlock($("#StateList_PhyProgressWorkDetails").val(), $("#DistrictList_PhyProgressWorkDetails").val());

    });
    $("#btnViewPhyProgressWork").click(function () {
        if ($('#frmPhyProgessWork').valid()) {
            $("#loadReport").html("");

            if ($("#StateList_PhyProgressWorkDetails").is(":visible")) {

                $("#StateName").val($("#StateList_PhyProgressWorkDetails option:selected").text());
            }

            if ($("#DistrictList_PhyProgressWorkDetails").is(":visible")) {

                //$('#DistrictList_PhyProgressWorkDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_PhyProgressWorkDetails option:selected").text());
            }
            if ($("#BlockList_PhyProgressWorkDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_PhyProgressWorkDetails option:selected").text());
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/ProgressReport/Progress/GetBlockProgressPost/',
                type: 'POST',
                catche: false,
                data: $("#frmPhyProgessWork").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadReport").html(response);

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
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPhyProgressWork").trigger('click');
    //}


    //this function call  on layout.js
    //closableNoteDiv("divPhyProgressWork", "spnPhyProgressWork");
});
//State Change Fill District DropDown List
function loadDistrict(statCode) {
    $("#DistrictList_PhyProgressWorkDetails").val(0);
    $("#DistrictList_PhyProgressWorkDetails").empty();
    $("#BlockList_PhyProgressWorkDetails").val(0);
    $("#BlockList_PhyProgressWorkDetails").empty();
    $("#BlockList_PhyProgressWorkDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_PhyProgressWorkDetails").length > 0) {
            $.ajax({
                //url: '/NationalArea/National/DistrictDetails',
                url: '/ProgressReport/Progress/GetDistricts',
                type: 'POST',
                data: { "MAST_STATE_CODE": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_PhyProgressWorkDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_PhyProgressWorkDetails').find("option[value='0']").remove();
                    //$("#DistrictList_PhyProgressWorkDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_PhyProgressWorkDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_PhyProgressWorkDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_PhyProgressWorkDetails").attr("disabled", "disabled");
                        $("#DistrictList_PhyProgressWorkDetails").trigger('change');
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

        $("#DistrictList_PhyProgressWorkDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_PhyProgressWorkDetails").empty();
        $("#BlockList_PhyProgressWorkDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_PhyProgressWorkDetails").val(0);
    $("#BlockList_PhyProgressWorkDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_PhyProgressWorkDetails").length > 0) {
            $.ajax({
                //url: '/NationalArea/National/BlockDetails',
                url: '/ProgressReport/Progress/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_PhyProgressWorkDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_PhyProgressWorkDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_PhyProgressWorkDetails").attr("disabled", "disabled");
                        //$("#BlockList_PhyProgressWorkDetails").trigger('change');
                    }
                    //$('#BlockList_PhyProgressWorkDetails').find("option[value='0']").remove();
                    //$("#BlockList_PhyProgressWorkDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_PhyProgressWorkDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_PhyProgressWorkDetails").append("<option value='0'>All Blocks</option>");
    }
}