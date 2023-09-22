/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityFilters2TierCQC.js
        * Description   :   Handles events for Filters in Quality module for 2Tier in CQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#3TierFilterForm'));


    selectedNameVal = 0;
    $("#MAST_STATE_CODE").change(function () {

        $("#ADMIN_QM_CODE").empty();
        if ($(this).val() == 0) {
            $("#ADMIN_QM_CODE").append("<option value='0'>All Monitors</option>");
        }

        if ($("#MAST_STATE_CODE").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetSQMNames',
                    type: 'POST',
                    data: { selectedState: $("#MAST_STATE_CODE").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#ADMIN_QM_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });


    //button in QualityFilters.cshtml
    $('#btn3TierListInspectionDetails').click(function () {

        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(),$("#ROAD_STATUS").val(), $("#schemeType").val());

    });//btn3TierListDetails ends here


    

});//doc.ready ends here


