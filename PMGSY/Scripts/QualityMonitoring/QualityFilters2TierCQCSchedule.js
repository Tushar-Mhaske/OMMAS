/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityFilters2TierCQCSchedule.js
        * Description   :   Handles events for Filters in Quality module for 2Tier in CQC Login
        * Author        :   Shyam Yadav 
        * Creation Date :   14/Jan/2015
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#2TierScheduleFilterForm'));

    $('#btn2TierScheduleListDetails').click(function () {
      //  alert("p")
        ScheduleListGrid($("#ddlState2TierCQCSchedule").val(), $("#ddlMonth2TierCQCSchedule").val(), $("#ddlYear2TierCQCSchedule").val());

    });

});//doc.ready ends here


