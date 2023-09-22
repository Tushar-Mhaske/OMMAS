/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityFilters.js
        * Description   :   Handles events for Filters in Quality module
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#3TierFilterForm'));

    InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#QM_TYPE_CODE").val());

    selectedNameVal = 0;
    $("#MAST_STATE_CODE").change(function () {

        $("#ADMIN_QM_CODE").empty();
        if ($("#QM_TYPE_CODE").val() === "I") {
            populateNQMs();
        }
        if ($(this).val() == 0) {
            
        }
        else
        {
             if ($("#ADMIN_QM_CODE").length > 0) {
                 if ($("#QM_TYPE_CODE").val() === "I") {
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
                 else if ($("#QM_TYPE_CODE").val() === "S") {
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
        }
    });

    
    //button in QualityFilters.cshtml
    $('#btn3TierListInspectionDetails').click(function () {

        InspectionListGrid($("#MAST_STATE_CODE").val(), $("#ADMIN_QM_CODE").val(), $("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val(), $("#QM_TYPE_CODE").val());

    });//btn3TierListDetails ends here


    $("#id3TierFilterDiv").click(function () {
        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierFilterForm").toggle("slow");
    });

});//doc.ready ends here


function populateNQMs()
{
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


