/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityMonitorFilters.js
        * Description   :   Handles events for in filters in Monitors login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $("#id3TierFilterDiv").click(function () {

        $("#id3TierFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#div3TierFilterForm").toggle("slow");

    });

    //button in QualityFilters.cshtml
    $('#btn3TierListInspectionDetails').click(function () {

        InspectionListGrid($("#FROM_MONTH").val(), $("#FROM_YEAR").val(), $("#TO_MONTH").val(), $("#TO_YEAR").val());

    });//btn3TierListDetails ends here


});//doc.ready ends here