$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchAgreement');

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
                $("#btnListAgreement").show();
                break;
            case "D":
                $("#ddlDistrictSearch").show();
                $("#lblDistrict").show();
                $("#ddlYearSearch").val('');
                $("#ddlBlockSearch").hide();
                $("#lblBlock").hide();
                $("#btnListAgreement").show();
                break;
            case "R":
                $("#ddlStateSearch").show();
                $("#ddlDistrictSearch").show();
                $("#ddlYearSearch").show();
                $("#lblDistrict").show();
                $("#btnListAgreement").show();
                break;
        }
    });

    $("#ddlStateSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlDistrictSearch", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val());

    });


    $("#btnListAgreement").click(function () {

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

        blockPage();
        $("#divAgreement").show();
        $("#tbAgreementList").jqGrid('GridUnload');
        LoadAgreementData();
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