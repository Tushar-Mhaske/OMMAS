/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CQCFilters.js
        * Description   :   onchange of dropdowns fill corrosponding dropdowns & related functionality for CQC Lists
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#3TierFilterForm'));

    $("#MAST_STATE_CODE").change(function () {

        $("#ADMIN_QM_CODE").empty();
        if ($(this).val() == 0) {
            $("#ADMIN_QM_CODE").append("<option value='0'>Select Monitor</option>");
        }

        if ($("#MAST_STATE_CODE").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetScheduledMonitors',
                    type: 'POST',
                    data: { selectedState: $("#MAST_STATE_CODE").val(), inspMonth: $("#FROM_MONTH").val(), inspYear: $("#FROM_YEAR").val(), value: Math.random() },
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


    $("#FROM_MONTH").change(function () {

        $("#ADMIN_QM_CODE").empty();
        if ($("#MAST_STATE_CODE").val() == 0) {
            $("#ADMIN_QM_CODE").append("<option value='0'>Select Monitor</option>");
        }

        if ($("#MAST_STATE_CODE").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetScheduledMonitors',
                    type: 'POST',
                    data: { selectedState: $("#MAST_STATE_CODE").val(), inspMonth: $("#FROM_MONTH").val(), inspYear: $("#FROM_YEAR").val(), value: Math.random() },
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


    $("#FROM_YEAR").change(function () {

        $("#ADMIN_QM_CODE").empty();
        if ($("#MAST_STATE_CODE").val() == 0) {
            $("#ADMIN_QM_CODE").append("<option value='0'>Select Monitor</option>");
        }

        if ($("#MAST_STATE_CODE").val() > 0) {

            if ($("#ADMIN_QM_CODE").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetScheduledMonitors',
                    type: 'POST',
                    data: { selectedState: $("#MAST_STATE_CODE").val(), inspMonth: $("#FROM_MONTH").val(), inspYear: $("#FROM_YEAR").val(), value: Math.random() },
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
    //$('#btn3TierListInspectionDetails').click(function () {

    //    InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());

    //});//btn3TierListDetails ends here


});//doc.ready ends here


