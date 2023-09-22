/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   scheduleFilters3TierSQC.js
        * Description   :   Handles events for Filters in Schedule List screen
        * Author        :   Shyam Yadav 
        * Creation Date :   27/Nov/2014
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#scheduleFilters3TierSQCForm'));

    //button in QualityFilters.cshtml
    $('#btnViewSchedule').click(function () {
        Show3TierSqcScheduleListGrid($("#ddlViewScheduleMonth").val(), $("#ddlViewScheduleYear").val());

    });


});//doc.ready ends here
