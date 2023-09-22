/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPahseInspection.js
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   29/Aug/2014
 **/
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMPahse'));

    if ($("#hdnRoleCode").val() == "8") {
        $("#StateName").val($('#ddlQMPahse option:selected').text());
        loadReport();
    }

    $("#btnQMPahse").click(function () {
        if ($('#frmQMPahse').valid()) {

            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlQMPahse option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });
});

function loadReport()
{
    $.ajax({
        url: '/QualityMonitoring/QMPhaseProgressReport/',
        type: 'POST',
        catche: false,
        data: $("#frmQMPahse").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadReport3").html(response);

        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}