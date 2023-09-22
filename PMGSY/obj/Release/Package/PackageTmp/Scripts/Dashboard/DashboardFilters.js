/*
    * Project Id    :
    * Project Name  :   OMMAS II
    * Name          :   DashboardFilters.js
    * Description   :   Manage Common Filters for Workld Bank Dashboard
    * Author        :   Shyam Yadav
    * Creation Date :   18/Sep/2013    
*/

$(document).ready(function () {

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-carat-1-e").toggleClass("ui-icon-circle-triangle-s");
        
        if ($("#divFilterForm").is(":visible")) {
            $("#divFilterForm").hide("slow");
        }
        else {
            $("#divFilterForm").show("fast");
        }
    });

    populateDistricts();


    $("#btnViewDetails").click(function () {
        
        //Initially load Financial Report
        createFinancialReportGrid($("#STATE_ND_CODE").val(), $("#MAST_DISTRICT_CODE").val(), $("#ADMIN_ND_CODE").val(), $("#IMS_COLLABORATION").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val());

        createStatusMonitoringReportGrid($("#STATE_ND_CODE").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val());

        createGeneralInformationReportGrid($("#STATE_ND_CODE").val(), $("#ddlFundTypeGeneralInfo").val());

        // For Annual Account, If PIU is not selected, Load SRRDA Report otherwise load PIU Report
        if ($("#ADMIN_ND_CODE").val() > 0) {
            createAnnualAccReportGrid("tbWBankAnnualAccCreditList", "dvWBankAnnualAccCreditListPager", true, $("#ADMIN_ND_CODE").val(), $("#YEAR").val(), $("#ddlFundTypeAnnualAcc").val(), "C");
            createAnnualAccReportGrid("tbWBankAnnualAccDebitList", "dvWBankAnnualAccDebitListPager", true, $("#ADMIN_ND_CODE").val(), $("#YEAR").val(), $("#ddlFundTypeAnnualAcc").val(), "D");
        }
        else
        {
            createAnnualAccReportGrid("tbWBankAnnualAccCreditList", "dvWBankAnnualAccCreditListPager", false, $("#STATE_ND_CODE").val(), $("#YEAR").val(), $("#ddlFundTypeAnnualAcc").val(), "C");
            createAnnualAccReportGrid("tbWBankAnnualAccDebitList", "dvWBankAnnualAccDebitListPager", false, $("#STATE_ND_CODE").val(), $("#YEAR").val(), $("#ddlFundTypeAnnualAcc").val(), "D");
        }


        // For Balance Sheet, If PIU is not selected, Load SRRDA Report otherwise load PIU Report
        if ($("#ADMIN_ND_CODE").val() > 0) {
            createBalanceSheetReportGrid("tbWBankBalanceSheetLiabilitiesList", "dvWBankBalanceSheetLiabilitiesListPager", true, $("#ADMIN_ND_CODE").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val(), "L");
            createBalanceSheetReportGrid("tbWBankBalanceSheetAssetsList", "dvWBankBalanceSheetAssetsListPager", true, $("#ADMIN_ND_CODE").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val(), "A");
        }
        else {
            createBalanceSheetReportGrid("tbWBankBalanceSheetLiabilitiesList", "dvWBankBalanceSheetLiabilitiesListPager", false, $("#STATE_ND_CODE").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val(), "L");
            createBalanceSheetReportGrid("tbWBankBalanceSheetAssetsList", "dvWBankBalanceSheetAssetsListPager", false, $("#STATE_ND_CODE").val(), $("#YEAR").val(), $("#DURATION_TYPE").val(), $("#DURATION").val(), "A");
        }


        createExpenditureLineChart(yearwiseCumExpLineChart, "yearwiseCumExpChartContainer");

    });


    //------------------------ Dropdown Onchange Events ---------------------------//

    $("#STATE_ND_CODE").change(function () {
        populateDistricts();
       
    });

    $("#MAST_DISTRICT_CODE").change(function () {
        populateDPIUs();
    });


    $("#DURATION_TYPE").change(function () {
        populateDuration();
    });

    //------------------------ Dropdown Onchange Events ---------------------------//



});



function populateDistricts() {
    $("#MAST_DISTRICT_CODE").empty();

    if ($("#STATE_ND_CODE").val() == 0) {
            $("#MAST_DISTRICT_CODE").append("<option value='0'>All Districts</option>");
     }

    if ($("#STATE_ND_CODE").val() > 0) {

        if ($("#MAST_DISTRICT_CODE").length > 0) {

            $.ajax({
                url: '/Dashboard/PopulateDistricts',
                type: 'POST',
                data: { selectedValue: $("#STATE_ND_CODE").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#MAST_DISTRICT_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
}



function populateDPIUs() {

    $("#ADMIN_ND_CODE").empty();

    if ($("#MAST_DISTRICT_CODE").val() == -1) {
        $("#ADMIN_ND_CODE").append("<option value='0'>All PIUs</option>");
    }

    if ($("#MAST_DISTRICT_CODE").val() > 0) {

        if ($("#ADMIN_ND_CODE").length > 0) {

            $.ajax({
                url: '/Dashboard/PopulateDPIU',
                type: 'POST',
                data: { selectedValue: $("#MAST_DISTRICT_CODE").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ADMIN_ND_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
}



function populateDuration() {

    $("#DURATION").empty();
    $.ajax({
        url: '/Dashboard/PopulateFinancialDuration',
        type: 'POST',
        data: { selectedValue: $("#DURATION_TYPE").val(), value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#DURATION").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}





