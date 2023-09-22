$(document).ready(function () {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    $.ajax({
        url: '/QMSSRSReports/QMSSRSReports/WorksNotInspectedSQMReport/',
        type: 'POST',
        catche: false,
        //data: $("#frmQMATRDetailsLayout").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#dvLoadWorksNotInspectedSQMReport").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("Error ocurred");
            return false;
        },
    });

})