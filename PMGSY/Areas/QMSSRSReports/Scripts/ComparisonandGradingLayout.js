$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmComparisonandGradingLayout'));

    //function populateDistricts() {
    $("#ddlStateGradingComparision").change(function () {

        $("#ddlDistrictGradingComparision").val(0);
        $("#ddlDistrictGradingComparision").empty();
        if ($(this).val() == 0) {
            $("#ddlDistrictGradingComparision").append("<option value='0'>All</option>");
        }

        if ($("#ddlStateGradingComparision").val() > 0) {

            if ($("#ddlDistrictGradingComparision").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/PopulateDistricts/',
                    type: 'GET',
                    data: { selectedState: $("#ddlStateGradingComparision").val() },
                    success: function (jsonData) {
                        $("#ddlDistrictGradingComparision").append("<option value='0'>All</option>");
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ddlDistrictGradingComparision").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            $("#ddlDistrictGradingComparision").append("<option value='0'>All</option>");
        }
    });//stateCode Change Ends here
    //}


    $("#btnViewGradingComparision").click(function () {
        //$('#MAST_PROPOSAL_TYPE').val($("#ddlMAST_PROPOSAL_TYPESearchForm7 option:selected").val());
        //$('#MAST_YEAR').val($("#ddlMAST_YEARSearchForm7 option:selected").val());
        //$('#IMS_BATCH').val($("#ddlIMS_BATCHSearchForm7 option:selected").val());
        //$('#IMS_COLLABORATION').val($("#ddlIMS_COLLABORATIONSearchForm7 option:selected").val());

        $('#StateName').val($("#ddlStateGradingComparision option:selected").text());
        $('#DistName').val($("#ddlDistrictGradingComparision option:selected").text());

        if ($('#frmComparisonandGradingLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/ComparisonandGradingReport/',
                type: 'POST',
                catche: false,
                data: $("#frmComparisonandGradingLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadComparisonandGradingReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    if ($("#hdnRole").val() == 8)
    {
        $("#btnViewGradingComparision").trigger('click');
    }

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmComparisonandGradingLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
