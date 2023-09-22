/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMUnsatisfactoryWorkDetails.js
        * Description   :   Handles events for QMUnsatisfactoryWorkDetails Report
        * Author        :   Shyam Yadav 
        * Creation Date :   23/July/2014
 **/

$(document).ready(function () {

    $("#spnHideUnsatisfactoryWorks").click(function () {
        $("#spnHideUnsatisfactoryWorks").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divUnsatisfactoryWorksTbl").toggle("slow");
    });

    $(function () {
        $("#accordionUnsatisfactoryWorks").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function CloseUnsatisfactoryWorksObsDetails() {
    $('#accordionUnsatisfactoryWorks').hide('slow');
    $('#divUnsatisfactoryWorksObsDetails').hide('slow');
    $("#divUnsatisfactoryWorksTbl").toggle("slow");
}

function ShowObsDetails(obsId) {

    window.open("/QualityMonitoring/QMObservationDetailsRpt/" + obsId);
}