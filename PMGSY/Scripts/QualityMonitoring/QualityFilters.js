﻿/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityFilters.js
        * Description   :   Handles events for Filters in Quality module
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
    //Below line added on 05-12-2022 to enable searching in dropdown
    $("#qmCodeList").chosen();

    $.validator.unobtrusive.parse($('#3TierFilterForm'));


    selectedNameVal = 0;
    $("#MAST_STATE_CODE").change(function () {
       
        $("#ADMIN_QM_CODE").empty();
        populateNQMs();
        if ($(this).val() == 0) {

        }
        else {
            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
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

    $("#stateCodeSearch").change(function () {

        $("#qmCodeList").empty();

        if ($(this).val() == 0) {
            $("#qmCodeList").append("<option value='0'>All Monitors</option>");
        }

        if ($("#stateCodeSearch").val() > 0) {

            if ($("#qmCodeList").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetSQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCodeSearch").val(), value: Math.random() },
                    success: function (jsonData) {
                       
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#qmCodeList").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                            
                        }
                        $("#qmCodeList").val('').trigger('chosen:updated');
                        $("#qmCodeList").chosen();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
    });
    //$("#FROM_MONTH").change(function () {
    //    populateNQMs();
    //});

    //$("#FROM_YEAR").change(function () {
    //    populateNQMs();
    //});

    //$("#TO_MONTH").change(function () {
    //    populateNQMs();
    //});

    //$("#TO_YEAR").change(function () {
    //    populateNQMs();
    //});

    //$("#ADMIN_QM_CODE").change(function () {

    //    $("#MAST_STATE_CODE").empty();
    //    if ($(this).val() == 0) {
    //        $("#MAST_STATE_CODE").append("<option value='0'>All States</option>");
    //    }

    //    if ($("#ADMIN_QM_CODE").val() > 0) {

    //        if ($("#MAST_STATE_CODE").length > 0) {

    //            $.ajax({
    //                url: '/QualityMonitoring/GetMonitorsInspectedStates',
    //                type: 'POST',
    //                data: { selectedMonitor: $("#ADMIN_QM_CODE").val(), frmMonth: $("#FROM_MONTH").val(), frmYear: $("#FROM_YEAR").val(), toMonth: $("#TO_MONTH").val(), toYear: $("#TO_YEAR").val(), value: Math.random() },
    //                success: function (jsonData) {
    //                    for (var i = 0; i < jsonData.length; i++) {
    //                        $("#MAST_STATE_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
    //                    }
    //                },
    //                error: function (xhr, ajaxOptions, thrownError) {
    //                    alert(xhr.status);
    //                    alert(thrownError);
    //                }
    //            });
    //        }
    //    }
    //});



    //button in QualityFilters.cshtml
    $('#btn3TierListInspectionDetails').click(function () {
        
   
        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#qmCodeList").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());   //add parameter on 30-06-2022 by vikky

    });//btn3TierListDetails ends here


    $('#btn2TierListInspectionDetailsCQCAdmin').click(function () {
    
        InspectionListGridCQCAdmin($("#stateCodeSearch").val(), $("#qmCodeList").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());   //add parameter on 30-06-2022 by vikky

    });

    $("#id3TierFilterDiv").click(function () {
        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierFilterForm").toggle("slow");
    });

});//doc.ready ends here


function populateNQMs() {
    $.ajax({
        url: '/QualityMonitoring/PopulateNQM',
        type: 'POST',
        data: { frmMonth: $("#FROM_MONTH").val(), frmYear: $("#FROM_YEAR").val(), toMonth: $("#TO_MONTH").val(), toYear: $("#TO_YEAR").val(), value: Math.random() },
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