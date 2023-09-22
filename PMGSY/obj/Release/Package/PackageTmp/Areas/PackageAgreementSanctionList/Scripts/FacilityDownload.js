$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));
    $("#StateList_StateListRoadDetails").change(function ()
    {
        loadDistrict($("#StateList_StateListRoadDetails").val());

    });

    $("#DistrictList_StateListRoadDetails").change(function ()
    {
        loadBlock($("#StateList_StateListRoadDetails").val(), $("#DistrictList_StateListRoadDetails").val());

    });
    $("#btnViewStateListRoad").click(function () {

        //alert("State= " + $("#StateList_StateListRoadDetails").val())
        //alert("Dist= " + $("#DistrictList_StateListRoadDetails").val())
        //alert("Block= " + $("#BlockList_StateListRoadDetails").val())

        //if ($("#StateList_StateListRoadDetails").val() == 0 || $("#StateList_StateListRoadDetails").val() == -1)
        //{
        //    alert("Please Select State");
        //    return null;
        //}

        //if ($("#DistrictList_StateListRoadDetails").val() == 0 || $("#DistrictList_StateListRoadDetails").val() == -1) {
        //    alert("Please Select District");
        //    return null;
        //}


        if ($("#BlockList_StateListRoadDetails").val() == 0 || $("#BlockList_StateListRoadDetails").val() == -1) {
            alert("Please Select Block");
            return null;
        }



        if ($('#frmStateListRoadLayout').valid()) {
            $("#dvloadSLRReport").html("");
          //  $("#BatchName").val($("#BatchList_StateListRoadDetails option:selected").text());
           // $("#CollaborationName").val($("#FundingAgencyList_StateListRoadDetails option:selected").text());
            //$("#StatusName").val($("#StatusList_StateListRoadDetails option:selected").text());

            if ($("#StateList_StateListRoadDetails").is(":visible")) {

                $("#StateName").val($("#StateList_StateListRoadDetails option:selected").text());
            }

            if ($("#DistrictList_StateListRoadDetails").is(":visible")) {

                //$('#DistrictList_StateListRoadDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_StateListRoadDetails option:selected").text());
            }
            if ($("#BlockList_StateListRoadDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_StateListRoadDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/FacilityDownloadReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateListRoadLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvloadSLRReport").html(response);

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



    if ($('#Mast_State_Code').val() > 0) {
        $("#btnViewStateListRoad").trigger('click');
    }
    //this function call  on layout.js
    closableNoteDiv("divStateListRoad", "spnStateListRoad");


    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmStateListRoadLayout").toggle("slow");

    });
});

//State Change Fill District DropDown List
function loadDistrict(statCode) {
    $("#DistrictList_StateListRoadDetails").val(0);
    $("#DistrictList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").val(0);
    $("#BlockList_StateListRoadDetails").empty();
    $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    //$('#DistrictList_StateListRoadDetails').find("option[value='0']").remove();
                    //$("#DistrictList_StateListRoadDetails").append("<option value='0'>Select District</option>");
                    //$('#DistrictList_StateListRoadDetails').val(0);

                    //For Disable if District Login
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_StateListRoadDetails").val($("#Mast_District_Code").val());
                        // $("#DistrictList_StateListRoadDetails").attr("disabled", "disabled");
                        $("#DistrictList_StateListRoadDetails").trigger('change');
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

        $("#DistrictList_StateListRoadDetails").append("<option value='0'>All Districts</option>");
        $("#BlockList_StateListRoadDetails").empty();
        $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    }
}

//District Change Fill Block DropDown List
function loadBlock(stateCode, districtCode) {
    $("#BlockList_StateListRoadDetails").val(0);
    $("#BlockList_StateListRoadDetails").empty();

    if (districtCode > 0) {
        if ($("#BlockList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/BlockDetails',
                type: 'POST',
                data: { "StateCode": stateCode, "DistrictCode": districtCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    if ($("#Mast_Block_Code").val() > 0) {
                        $("#BlockList_StateListRoadDetails").val($("#Mast_Block_Code").val());
                        // $("#BlockList_StateListRoadDetails").attr("disabled", "disabled");
                        //$("#BlockList_StateListRoadDetails").trigger('change');
                    }
                    //$('#BlockList_StateListRoadDetails').find("option[value='0']").remove();
                    //$("#BlockList_StateListRoadDetails").append("<option value='0'>Select Block</option>");
                    //$('#BlockList_StateListRoadDetails').val(0);


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");
    }
}

