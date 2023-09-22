/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayoutPIU.js
        * Description   :   Handles events for in Quality Layout for PIU Login
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    $("#btn3TierPIU").click(function () {
        blockPage();
        $("#divQualityLayoutPIU").html('');
        $("#divQualityLayoutPIU").load('/QualityMonitoring/QM3TierPIU/', function () {
            //unblockPage();
        });
        $('#divQualityLayoutPIU').show('slow');
        unblockPage();
    });

    $("#btn2TierPIU").click(function () {
        blockPage();
        $("#divQualityLayoutPIU").html('');
        $("#divQualityLayoutPIU").load('/QualityMonitoring/QM2TierPIU/', function () {
            //unblockPage();
        });
        $('#divQualityLayoutPIU').show('slow');
        unblockPage();
    });

    $("#btn1TierPIU").click(function () {
        blockPage();
        $("#divQualityLayoutPIU").html('');
        $("#divQualityLayoutPIU").load('/QualityMonitoring/QM1TierPIU/', function () {
            //unblockPage();
        });
        $('#divQualityLayoutPIU').show('slow');
        unblockPage();
    });
});


//function Show2TierPIUSchedule() {
//    blockPage();
//    $("#div2TierPIU").html('');
//    $("#div2TierPIU").load('/QualityMonitoring/QM2TierPIU/', function () {
//        unblockPage();
//    });
//    $('#div2TierPIU').show('slow');
//}


