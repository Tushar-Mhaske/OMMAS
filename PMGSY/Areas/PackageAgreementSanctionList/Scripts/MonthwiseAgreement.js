$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmStateListRoadLayout'));
    $("#StateList_StateListRoadDetails").change(function ()
    {
        loadDistrict($("#StateList_StateListRoadDetails").val());
    });

    $("#btnViewStateListRoad").click(function ()
    {
        if ($('#frmStateListRoadLayout').valid()) {
            $("#dvloadSLRReport").html("");
            if ($("#StateList_StateListRoadDetails").is(":visible")) {

                $("#StateName").val($("#StateList_StateListRoadDetails option:selected").text());
            }

            if ($("#DistrictList_StateListRoadDetails").is(":visible")) {

                $("#DistName").val($("#DistrictList_StateListRoadDetails option:selected").text());
            }
            if ($("#BlockList_StateListRoadDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_StateListRoadDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/PackageAgreementSanctionList/PackageAgreement/MonthwiseAgreementReport/',
                type: 'POST',
                catche: false,
                data: $("#frmStateListRoadLayout").serialize(),
                async: false,
                success: function (response)
                {
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

