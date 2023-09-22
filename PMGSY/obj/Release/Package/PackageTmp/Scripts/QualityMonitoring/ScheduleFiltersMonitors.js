/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   scheduleFilters3TierSQC.js
        * Description   :   Handles events for Filters in Schedule List screen
        * Author        :   Shyam Yadav 
        * Creation Date :   29/Jan/2015
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#scheduleFiltersMonitorsForm'));

    //button in QualityFilters.cshtml
    $('#btnViewSchedule').click(function () {
        showMonitorsAssignScheduleListGrid($("#ddlViewScheduleMonth").val(), $("#ddlViewScheduleYear").val());
        closeMonitorsScheduleDetails();
    });


});//doc.ready ends here
