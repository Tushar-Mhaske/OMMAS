/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMCompletedInpectionDetails.js
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   29/Aug/2014
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmQMCompletedInspDetails'));

    if ($("#hdnRoleCode").val() == "8") {
        $("#StateName").val($('#ddlStateCompleted option:selected').text());
        loadReport();
    }

    $("#btnCompletedView").click(function () {
        if ($('#frmQMCompletedInspDetails').valid()) {
          
            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlStateCompleted option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });
});


function loadReport() {
    $.ajax({
        url: '/QualityMonitoring/QMCompletedInspDetailsReport/',
        type: 'POST',
        catche: false,
        data: $("#frmQMCompletedInspDetails").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadReport2").html(response);
        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}