$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchCoreNetwork');


    $("#ddlDistrictSearch").hide();
    $("#ddlBlockSearch").hide();
    $("#lblDistrict").hide();
    $("#lblBlock").hide();


    $("#ddlLevel").change(function (e) {
        
        if (!($("#divFilterForm").is(":visible"))) {
            $("#divFilterForm").show();
        }

        var option = $("#ddlLevel option:selected").val();
        switch (option) {
            case "S":
                $("#ddlDistrictSearch").val('');
                $("#ddlDistrictSearch").hide();
                $("#ddlBlockSearch").val('');
                $("#ddlBlockSearch").hide();
                $("#lblDistrict").hide();
                $("#lblBlock").hide();
                break;
            case "D":
                $("#ddlDistrictSearch").show();
                $("#lblDistrict").show();
                $("#ddlBlockSearch").val('');
                $("#ddlBlockSearch").hide();
                $("#lblBlock").hide();
                break;
            case "R":
                $("#ddlStateSearch").show();
                $("#ddlDistrictSearch").show();
                $("#ddlBlockSearch").show();
                $("#lblDistrict").show();
                $("#lblBlock").show();
                break;
        }
    });



    $("#ddlStateSearch").prepend("<option value='0' selected>--Select State--</option>");
    $("#ddlDistrictSearch").prepend("<option value='0' selected>--Select District--</option>");


    $("#ddlStateSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlDistrictSearch", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val());

    });

    $("#ddlDistrictSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlDistrictSearch").find(":selected").val() },
                           "#ddlBlockSearch", "/LockUnlock/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistrictSearch option:selected').val());

    });

    $("#btnListCoreNetwork").click(function () {

        if ($("#ddlStateSearch").val() == "") {
            alert("Please select state.");
            return false;
        }

        if ($("#divProposal").is(":visible")) {
            $("#divProposal").hide();
        }

        if ($("#divExistingRoad").is(":visible")) {
            $("#divExistingRoad").hide();
        }

        blockPage();
        $("#divCoreNetwork").show();
        $("#tbCoreNetworkList").jqGrid('GridUnload');
        $("#divUnlockCoreNetwork").show();
        $("#tbCoreNetworkUnlockList").jqGrid('GridUnload');
        LoadNetworkGridData();
        LoadNetworkUnlockList();
        unblockPage();
    });


});
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlFundingAgency') {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }

    $(dropdown).empty();
    blockPage();
    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
    unblockPage();
}