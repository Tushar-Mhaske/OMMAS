$(document).ready(function () {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    $.ajax({
        url: '/QMSSRSReports/QMSSRSReports/DistwiseInspectionReport/',
        type: 'POST',
        catche: false,
        //data: $("#frmQMATRDetailsLayout").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#dvLoadDistwiseInspectionReport").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("Error ocurred");
            return false;
        },
    });

})