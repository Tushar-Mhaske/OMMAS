/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QM1TierPIU.js
        * Description   :   Handles events, grids in PIU Login for Lab Details data
        * Author        :   Shyam Yadav 
        * Creation Date :   09/Sept/2014
 **/

$(document).ready(function () {

    $("#tabs-1TierDetailsPIU").tabs();
    $('#tabs-1TierDetailsPIU ul').removeClass('ui-widget-header');

    $(function () {
        $("#accordion1TierPIULab").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});