/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMUnsatisfactoryWorkDetailsFilters.js
        * Description   :   Handles events for QMUnsatisfactoryWorkDetails Report
        * Author        :   Shyam Yadav 
        * Creation Date :   23/July/2014
 **/

$(document).ready(function () {

    $("#btnViewUnsatisfactoryWorks").click(function () {
        if ($("#ddlStateUnsatisfactoryWork").val() == 0)
        {
            alert("Please select state");
            return;
        }

        viewUnsatisfactoryWorksDetails();
    });

});


function viewUnsatisfactoryWorksDetails() {
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/QMUnsatisfactoryWorkDetails',
        type: 'POST',
        data: { stateCode: $("#ddlStateUnsatisfactoryWork").val(), qmType: $("#ddlQMTypeUnsatisfactoryWork").val() },
        success: function (response) {
            $("#divUnstifactoryWorksDetails").html('');
            $("#divUnstifactoryWorksDetails").html(response);
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);

        }
    });
}

