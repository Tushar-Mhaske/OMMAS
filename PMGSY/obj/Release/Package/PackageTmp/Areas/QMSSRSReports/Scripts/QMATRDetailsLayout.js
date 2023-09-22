jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = parseInt($('#ddlFromYearATRDetails').val());
    var toDate = parseInt($('#ddlToYearATRDetails').val());

    var fromMonth = parseInt($('#ddlFromMonthATRDetails').val());
    var toMonth = parseInt($('#ddlToMonthATRDetails').val());

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

    $.validator.unobtrusive.parse($('#frmQMATRDetailsLayout'));

    $("#btnViewATRDetails").click(function () {

        if ($('#frmQMATRDetailsLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/QMATRDetailsReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMATRDetailsLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadQMATRDetailsReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnViewATRDetails").trigger('click');

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMATRDetailsLayout").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});
