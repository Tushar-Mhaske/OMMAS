/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   
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
function QualityProfile(stateCode, year)
{
    
    $("#loadChart").load('/QMSSRSReports/QMSSRSReports/QMQualityProgressReport/' + stateCode + '$' + year);
}


function loadReport() {
    $.ajax({
        url: '/QMSSRSReports/QMSSRSReports/QMQualityProfileReport/',
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