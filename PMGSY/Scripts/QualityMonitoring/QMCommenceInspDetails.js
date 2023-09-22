/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMCommenceInspDetails.js
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   29/Aug/2014
 **/

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMCommenseInspDetails'));

    if ($("#hdnRoleCode").val() == "8") {
        $("#StateName").val($('#ddlStateCommence option:selected').text());
        loadReport();
    }

    $("#btnCommenceView").click(function () {

        if ($('#frmQMCommenseInspDetails').valid()) {
         
            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlStateCommence option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });
});

function loadReport()
{
    $.ajax({
        url: '/QualityMonitoring/QMCommenceInspDetailsReport/',
        type: 'POST',
        catche: false,
        data: $("#frmQMCommenseInspDetails").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();

            $("#loadReport1").html(response);

        },

        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}