jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = parseInt($('#ddlFromYearGradingAndATR').val());
    var toDate = parseInt($('#ddlToYearGradingAndATR').val());

    var fromMonth = parseInt($('#ddlFromMonthGradingAndATR').val());
    var toMonth = parseInt($('#ddlToMonthGradingAndATR').val());

    if (fromDate == toDate) {
        if (fromMonth > toMonth) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        if (fromDate > toDate) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMATRLayout'));
    //if ($("#hdnRole").val() == 8) {
    //    loadQMGradingAndATRDetailsGrid("SQM", "tbGradingAndATRSQMReport", "dvGradingAndATRSQMReportPager", "S");
    //}

    $("#btnViewGradingAndATRDetails").click(function () {

        //loadQMGradingAndATRDetailsGrid("NQM", "tbGradingAndATRNQMReport", "dvGradingAndATRNQMReportPager", "I");

        //if ($("#hdnRole").val() == 8) {
        //    loadQMGradingAndATRDetailsGrid("SQM", "tbGradingAndATRSQMReport", "dvGradingAndATRSQMReportPager", "S");
        //}

        //$('#MAST_PROPOSAL_TYPE').val($("#ddlMAST_PROPOSAL_TYPESearchForm7 option:selected").val());
        //$('#MAST_YEAR').val($("#ddlMAST_YEARSearchForm7 option:selected").val());
        //$('#IMS_BATCH').val($("#ddlIMS_BATCHSearchForm7 option:selected").val());
        //$('#IMS_COLLABORATION').val($("#ddlIMS_COLLABORATIONSearchForm7 option:selected").val());

        //$('#CollabName').val($("#ddlIMS_COLLABORATIONSearchForm7 option:selected").text());

        if ($('#frmQMATRLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMATRReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMATRLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadQMATRReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnViewGradingAndATRDetails").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMATRLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
