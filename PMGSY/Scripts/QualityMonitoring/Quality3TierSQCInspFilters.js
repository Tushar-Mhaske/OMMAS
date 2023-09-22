/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   Quality2TierSQCInspFilters.js
        * Description   :   Handles events for 3Tier SQC Inspection Details
        * Author        :   Shyam Yadav 
        * Creation Date :   18/Jun/2014
 **/

$(document).ready(function () {

    // #Added For Searchable Dropdown on 24-01-2023 cqcAdmin
  //  $("#monitorCode3TierSQCInsp").chosen();


    $("#id3TierSQCInspFilterDiv").click(function () {
        $("#id3TierSQCInspFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierSQCInspFilterForm").toggle("slow");

    });

    selectedNameVal = 0;
    $("#stateCode3TierSQCInsp").change(function () {
         
       // $("#monitorCode3TierSQCInsp").empty();
       // alert($(this).val());
        if ($(this).val() == 0) {
            $("#monitorCode3TierSQCInsp").append("<option value='0'>All Monitors</option>");
        }

        if ($("#stateCode3TierSQCInsp").val() > 0) {

            if ($("#monitorCode3TierSQCInsp").length > 0) {

                $.ajax({
                    url: '/QualityMonitoring/GetNQMNames',
                    type: 'POST',
                    data: { selectedState: $("#stateCode3TierSQCInsp").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#monitorCode3TierSQCInsp").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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

    //  InspectionListGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());

    //Add for CQCAdmin on 28-06/2022 by vikky
    // InspectionListGridCQCAdmin($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());

    //button in Quality3TierSQCInspFilters.cshtml
    $('#btn3TierSQCInspDetails').click(function () {

        InspectionListGrid($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());

    });

    //Add for CQCAdmin on 28-06/2022 by vikky
    $('#btn3TierCQCAdminInspDetails').click(function () {

        InspectionListGridCQCAdmin($("#stateCode3TierSQCInsp").val(), $("#monitorCode3TierSQCInsp").val(), $("#frmMonth3TierSQCInsp").val(), $("#frmYear3TierSQCInsp").val(), $("#toMonth3TierSQCInsp").val(), $("#toYear3TierSQCInsp").val(), $("#schemeType").val(), $("#ROAD_STATUS").val(), $("#roadOrBridge3TierSQCInsp").val(), $("#gradeType3TierSQCInsp").val(), $("#eFormStatusType3TierSQCInsp").val());

    });

});//doc.ready ends here

//function ShowInspReportFile(obsId) {

//    jQuery('#tb3TierSqcInspList').jqGrid('setSelection', obsId);

//    $("#accordion3TierSqcInspection div").html("");
//    $("#accordion3TierSqcInspection h3").html(
//            "<a href='#' style= 'font-size:.9em;'>Inspection Details</a>" +

//            '<a href="#" style="float: right;">' +
//            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="Close3TierSqcInspectionDetails();" /></a>' +
//            '<span style="float: right;"></span>'
//            );

//    var number = Math.floor((Math.random() * 99999999) + 1);

//    $('#accordion3TierSqcInspection').show('slow', function () {
//        blockPage();
//        $("#div3TierSqcInspDetails").load('/QualityMonitoring/InspPdfFileUpload/' + obsId, function () {
//            unblockPage();
//        });
//    });

//    $("#div3TierSqcInspDetails").css('height', 'auto');
//    $('#div3TierSqcInspDetails').show('slow');

//    $("#tb3TierSqcInspList").jqGrid('setGridState', 'hidden');
//    $('#id3TierSQCInspFilterDiv').trigger('click');
//}

$("#monitorCode3TierSQCInsp").chosen();

/*Quality ATR Missing File Upload*/
function UploadMissingATR(obsId) {

    $("#accordionATR3TierCqc div").html("");
    $("#accordionATR3TierCqc h3").html(
        "<a href='#' style= 'font-size:.9em;' >Upload Missing ATR File</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseATR3TierCqcDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordionATR3TierCqc').show('slow', function () {
        blockPage();
        $("#divATR3TierCqcDetails").load('/QualityMonitoring/ATRPdfMissingFileUpload/' + obsId, function () {
            unblockPage();
        });
    });

    $("#divATR3TierCqcDetails").css('height', 'auto');
    $('#divATR3TierCqcDetails').show('slow');

    toggleATRDetails();

}