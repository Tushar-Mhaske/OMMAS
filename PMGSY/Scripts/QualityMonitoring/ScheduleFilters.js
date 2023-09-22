/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ScheduleFilters.js
        * Description   :   Handles events for Filters in Schedule List screen
        * Author        :   Shyam Yadav 
        * Creation Date :   27/Nov/2014
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#scheduleFiltersForm'));

    //button in QualityFilters.cshtml
    $('#btnViewSchedule').click(function () {
        ScheduleListGrid($("#ddlViewScheduleMonth").val(), $("#ddlViewScheduleYear").val());

    });


});//doc.ready ends here
