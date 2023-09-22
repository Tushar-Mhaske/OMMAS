$(document).ready(function () {
    var date = new Date();
    var yearValue = date.getYear();
   // alert(1900 + yearValue);
    var selectedDate = 1900 + yearValue;
    $('#ddlYearInsp').val(selectedDate);
    LoadContractorWiseReport();

    $('#btn_View_Report').click(function () {

        LoadContractorWiseReport();
    });



});
function LoadContractorWiseReport() {

        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/LoadContractorwiseReport',
            type: 'POST',
            data: $('#frmLoadContractorwiseReport').serialize(),
            success: function (response) {
                if (response != null) {
                    $("#LoadContractorwiseReport").html(response);
                }
                else {
                    alert("An error occurred during processing");
                }
            }
            // $("#ddlDistMaintenanceInsp").remove(0);
        });


  
}