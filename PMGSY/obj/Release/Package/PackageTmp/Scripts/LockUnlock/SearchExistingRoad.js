$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchExistingRoad');


    $("#ddlDistrictSearch").hide();
    $("#ddlBlockSearch").hide();
    $("#lblDistrict").hide();
    $("#lblBlock").hide();
    $("#btnListExistingRoad").hide();

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
                $("#lblState").show();
                $("#ddlStateSearch").show();
                $("#btnListExistingRoad").show();
                break;
            case "D":
                $("#ddlDistrictSearch").show();
                $("#lblDistrict").show();
                $("#ddlBlockSearch").val('');
                $("#ddlBlockSearch").hide();
                $("#lblBlock").hide();
                $("#btnListExistingRoad").show();
                break;
            case "R":
                $("#ddlStateSearch").show();
                $("#ddlDistrictSearch").show();
                $("#ddlBlockSearch").show();
                $("#lblDistrict").show();
                $("#lblBlock").show();
                $("#btnListExistingRoad").show();
                break;
            default:
                $("#lblDistrict").hide();
                $("#lblBlock").hide();
                $("#lblState").hide();
                $("#ddlStateSearch").hide();
                $("#ddlBlockSearch").hide();
                $("#ddlDistrictSearch").hide();
                $("#btnListExistingRoad").hide();
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

    $("#btnListExistingRoad").click(function () {

        if ($("#ddlStateSearch").val() == "") {
            alert("Please select state.");
            return false;
        }

        blockPage();
        if ($("#divProposal").is(":visible")) {
            $("#divProposal").hide();
        }

        if ($("#divCoreNetwork").is(":visible")) {
            $("#divCoreNetwork").hide();
        }
        $("#divExistingRoad").show();
        $("#divUnlockExistingRoads").show();
        $("#tbExistingRoadList").jqGrid('GridUnload');
        $("#tbExistingRoadsUnlockList").jqGrid('GridUnload');
        LoadExistingRoadData();
        LoadExistingRoadUnlockData();
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
