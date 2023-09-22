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
  

    $("#btnViewGradingAndATRDetails").click(function () {

       

        if ($('#frmQMATRLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMOngoingWorksInspectionsReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMATRLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvQMWorksInpections").html(response);
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
