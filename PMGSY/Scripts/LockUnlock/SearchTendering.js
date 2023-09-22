$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchTendering');

    $("#ddlDistrictSearch").hide();
    $("#lblDistrict").hide();


    $("#ddlLevel").change(function (e) {

        if (!($("#divFilterForm").is(":visible"))) {
            $("#divFilterForm").show();
        }

        var option = $("#ddlLevel option:selected").val();
        switch (option) {
            case "S":
                $("#ddlYearSearch").show();
                $("#lblYear").show();
                $("#ddlStateSearch").show();
                $("#lblState").show();
                $("#ddlDistrictSearch").val('');
                $("#ddlDistrictSearch").hide();
                $("#lblDistrict").hide();
                $("#btnListTendering").show();
                break;
            case "D":
                $("#ddlDistrictSearch").show();
                $("#lblDistrict").show();
                $("#ddlYearSearch").val('');
                $("#ddlBlockSearch").hide();
                $("#lblBlock").hide();
                $("#btnListTendering").show();
                break;
            case "R":
                $("#ddlStateSearch").show();
                $("#ddlDistrictSearch").show();
                $("#ddlYearSearch").show();
                $("#lblDistrict").show();
                $("#btnListTendering").show();
                break;
            default:
                $("#ddlYearSearch").hide();
                $("#lblYear").hide();
                $("#ddlStateSearch").hide();
                $("#lblState").hide();
                $("#ddlDistrictSearch").hide();
                $("#lblDistrict").hide();
                $("#btnListTendering").hide();
                break;
        }
    });

    $("#ddlStateSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlDistrictSearch", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val());

    });


    $("#btnListTendering").click(function () {

        if ($("#ddlStateSearch").val() == "") {
            alert("Please select state.");
            return false;
        }

        if ($("#ddlYearSearch").val() == "") {
            alert("Please select year.");
            return false;
        }



        if ($("#divProposal").is(":visible")) {
            $("#divProposal").hide();
        }

        if ($("#divExistingRoad").is(":visible")) {
            $("#divExistingRoad").hide();
        }

        if ($("#divCoreNetwork").is(":visible")) {
            $("#divCoreNetwork").hide();
        }

        if ($("#divAgreement").is(":visible")) {
            $("#divAgreement").hide();
        }

        blockPage();
        $("#divTendering").show();
        $("#tbTenderingList").jqGrid('GridUnload');
        LoadTenderingData();
        unblockPage();

    });


});
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

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