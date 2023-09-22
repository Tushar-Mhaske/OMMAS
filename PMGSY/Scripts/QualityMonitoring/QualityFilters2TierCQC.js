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

                        // #Added For Searchable Dropdown on 24-01-2023 
                        $("#ADMIN_QM_CODE").val('').trigger('chosen:updated');
                        $("#ADMIN_QM_CODE").chosen();
                        //$("#ADMIN_QM_CODE_chosen").chosen();

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

        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#ROAD_STATUS").val(), $("#schemeType").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());   //add parameter on 01-07-2022 by vikky

    });//btn3TierListDetails ends here

    $('#btn3TierListInspectionDetails').click(function () {

        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#ROAD_STATUS").val(), $("#schemeType").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());   //add parameter on 01-07-2022 by vikky

    });




});//doc.ready ends here


