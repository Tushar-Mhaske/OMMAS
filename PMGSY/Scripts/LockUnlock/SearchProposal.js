$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchProposal');


    HideLabels();
    HideDropdowns();
    $("#btnListProposal").hide();


    $("#ddlLevel").change(function () {

        var levelValue = $("#ddlLevel").val();

        if (!($("#divFilterForm").is(":visible"))) {
            $("#divFilterForm").show();
        }

        switch (levelValue) {

            case "S":
                HideLabels();
                HideDropdowns();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblYear").show('slow');
                $("#ddlYearSearch").show('slow');
                $("#btnListProposal").show('slow');
                $("#ddlDistrictSearch").val('');
                $("#ddlPackageSearch").val('');
                $("#ddlBatchSearch").val('');
                break;
            case "D":
                HideLabels();
                HideDropdowns();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblYear").show('slow');
                $("#ddlYearSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblBatch").show('slow');
                $("#ddlBatchSearch").show('slow');
                $("#btnListProposal").show('slow');
                break;
            case "P":
                HideLabels();
                HideDropdowns();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblYear").show('slow');
                $("#ddlYearSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblPackage").show('slow');
                $("#ddlPackageSearch").show('slow');
                $("#btnListProposal").show('slow');
                break;
            case "R":
                HideLabels();
                HideDropdowns();
                $("#lblState").show('slow');
                $("#ddlStateSearch").show('slow');
                $("#lblYear").show('slow');
                $("#ddlYearSearch").show('slow');
                $("#lblDistrict").show('slow');
                $("#ddlDistrictSearch").show('slow');
                $("#lblPackage").show('slow');
                $("#ddlPackageSearch").show('slow');
                $("#btnListProposal").show('slow');
                break;
            default:
                HideLabels();
                HideDropdowns();
                $("#btnListProposal").hide();
                break;
        }
    });

    //$("#ddlStateSearch").prepend("<option value='0' selected>--Select State--</option>");
    $("#ddlDistrictSearch").prepend("<option value='0' selected>--Select District--</option>");


    $("#ddlStateSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlDistrictSearch", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val());

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlPackageSearch", "/LockUnlock/GetPackageByStateCode?stateCode=" + $('#ddlStateSearch option:selected').val() + "&yearCode="+$("#ddlYearSearch option:selected").val());

    });


    $("#ddlDistrictSearch").change(function (e) {

        FillInCascadeDropdown({ userType: $("#ddlStateSearch").find(":selected").val() },
                           "#ddlPackageSearch", "/LockUnlock/GetAllPackageByDistrictCode?stateCode=" + $('#ddlStateSearch option:selected').val() + "&yearCode=" + $("#ddlYearSearch option:selected").val() + "&districtCode=" + $("#ddlDistrictSearch option:selected").val());

    })
    
    $("#btnListProposal").click(function (e) {

        if ($("#ddlYearSearch").val() == "0") {
            alert("Please select year.");
            return false;
        }

        if ($("#ddlStateSearch").val() == "") {
            alert("Please select state.");
            return false;
        }

        if ($("#divExistingRoad").is(":visible")) {
            $("#divExistingRoad").hide();
        }

        if ($("#divCoreNetwork").is(":visible")) {
            $("#divCoreNetwork").hide();
        }

        blockPage();
        $("#divProposal").show();
        $("#tbProposalList").jqGrid('GridUnload');
        $("#divUnlockProposal").show();
        $("#tbProposalUnlockList").jqGrid('GridUnload');
        LoadGridData();
        LoadProposalUnlockList();
        //displayLockDetails();
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

function displayLockDetails() {

    $('#tbProposalList').setGridParam({
        url: '/LockUnlock/GetProposalList', datatype: 'json'
    });
    $('#tbProposalList').jqGrid("setGridParam", { "postData": { yearCode: $('#ddlYearSearch option:selected').val(), stateCode: $("#ddlStateSearch option:selected").val(), districtCode: $('#ddlDistrictSearch option:selected').val(), batchCode: $("#ddlBatchSearch option:selected").val(), packageCode: $("#ddlPackageSearch option:selected").val() } });
    $('#tbProposalList').trigger("reloadGrid", [{ page: 1 }]);

}
function HideLabels() {

    $("#lblState").hide();
    $("#lblYear").hide();
    $("#lblDistrict").hide();
    $("#lblPackage").hide();
    $("#lblBatch").hide();
}
function HideDropdowns() {

    $("#ddlYearSearch").hide();
    $("#ddlStateSearch").hide();
    $("#ddlDistrictSearch").hide();
    $("#ddlPackageSearch").hide();
    $("#ddlBatchSearch").hide();
}