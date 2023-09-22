$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmPhyProgessWork'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#loadFilters").toggle("slow");

    });

    $.validator.unobtrusive.parse($('#frmPhyProgessWork'));
    $("#StateCode").change(function () {
        loadDistrict($("#StateCode").val());

    });

    $("#DistrictCode").change(function () {
        loadBlock($("#StateCode").val(), $("#DistrictCode").val());

    });
    $("#btnGo").click(function () {
        if ($('#frmPhyProgessWork').valid()) {
            $("#loadReport").html("");

            if ($("#StateCode").is(":visible")) {

                $("#Statename").val($("#StateCode option:selected").text());
            }

            if ($("#DistrictCode").is(":visible")) {

                //$('#DistrictList_PhyProgressWorkDetails').attr("disabled", false);
                $("#districtname").val($("#DistrictCode option:selected").text());
            }
            if ($("#BlockID").is(":visible")) {

                $("#blockname").val($("#BlockID option:selected").text());
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/CoreNetwork/GetPCIDetailsReportPOST/',
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
    $("#DistrictCode").val(0);
    $("#DistrictCode").empty();
    $("#BlockID").val(0);
    $("#BlockID").empty();
    $("#BlockID").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        $.ajax({
                //url: '/NationalArea/National/DistrictDetails',
                url: '/ProgressReport/Progress/GetDistricts',
                type: 'POST',
                data: { "MAST_STATE_CODE": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Value != 0)
                            $("#DistrictCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        else
                            $("#DistrictCode").append("<option value='" + jsonData[i].Value + "'>" + "Select District" + "</option>");
                    }
                    //$('#DistrictList_PhyProgressWorkDetails').find("option[value='0']").remove();
                    //$("#DistrictList_PhyProgressWorkDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_PhyProgressWorkDetails').val(0);

                    //For Disable if District Login
                    //if ($("#Mast_District_Code").val() > 0) {
                    //    $("#DistrictList_PhyProgressWorkDetails").val($("#Mast_District_Code").val());
                    //    // $("#DistrictList_PhyProgressWorkDetails").attr("disabled", "disabled");
                    //    $("#DistrictList_PhyProgressWorkDetails").trigger('change');
                    //}


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        
    }
    else {

        //$("#DistrictCode").append("<option value='0'>All Districts</option>");
        //$("#BlockID").empty();
        //$("#BlockID").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockID").val(0);
    $("#BlockID").empty();

    if (districtCode > 0) {
            $.ajax({
                //url: '/NationalArea/National/BlockDetails',
                url: '/ProgressReport/Progress/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        if (jsonData[i].Value != 0)
                            $("#BlockID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        else
                            $("#BlockID").append("<option value='" + jsonData[i].Value + "'>" + "Select Block" + "</option>");

                    }

                    //if ($("#Mast_Block_Code").val() > 0) {
                    //    $("#BlockList_PhyProgressWorkDetails").val($("#Mast_Block_Code").val());
                    //    // $("#BlockList_PhyProgressWorkDetails").attr("disabled", "disabled");
                    //    //$("#BlockList_PhyProgressWorkDetails").trigger('change');
                    //}
                    //$('#BlockList_PhyProgressWorkDetails').find("option[value='0']").remove();
                    //$("#BlockList_PhyProgressWorkDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_PhyProgressWorkDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        
    } else {
        $("#BlockID").append("<option value='0'>All Blocks</option>");
    }
}