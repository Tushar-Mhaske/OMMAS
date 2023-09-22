/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityLayout.js
        * Description   :   Handles events for in Quality Layout
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    //$.validator.unobtrusive.parse($('#3TierFilterForm'));
    $("#spnModuleName").html("Quality Monitoring");

    $("#tabs-3TierDetails").tabs();
    $('#tabs-3TierDetails ul').removeClass('ui-widget-header');


    $('#tabOne1').trigger('click');

    $(function () {
        $("#accordionSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionTeamSchedule").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionInspection").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $(function () {
        $("#accordionATR3TierCqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionSQCLetter").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionBulkATRDetails").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionATR2TierSqc").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });



});//doc.ready ends here





// Target Length
function ShowTargetLength() {

    blockPage();
    $("#div1").load('/Dashboard/TargetLengthLayout', function () {
        $.validator.unobtrusive.parse($('#frmHabitationClusterReport'));
        unblockPage();
    });
    $('#div1').show('slow');


}


// Balance Habs
function ShowHabsToBeConnected() {

    blockPage();
    $("#div2").load('/Dashboard/HabsToBeConnectedLayout', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout1'));
        unblockPage();
    });



    $('#div2').show('slow');

}


// Pending Length 
function ShowPendingLength() {

    blockPage();
    $("#div3").load('/Dashboard/PendingLength', function () {
        $.validator.unobtrusive.parse($('#frmGepnicTenderDetailsLayout2'));
        unblockPage();
    });


    $('#div3').show('slow');

}

// Pending Works 
function ShowPendingWorks() {

    blockPage();
    $("#div4").load('/Dashboard/PendingWorks', function () {
        $.validator.unobtrusive.parse($('#frmPendingWorks'));
        unblockPage();
    });


    $('#div4').show('slow');

}




// Road Length 
function ShowRoadLength() {

    blockPage();
    $("#div5").load('/Dashboard/RoadLength', function () {
        $.validator.unobtrusive.parse($('#frmRoadLength'));
        unblockPage();
    });


    $('#div5').show('slow');

}

// ShowAchievements

function ShowAchievements() {

    blockPage();
    $("#div6").load('/Dashboard/Achievements', function () {
        $.validator.unobtrusive.parse($('#frmShowAchievementsDetails'));
        unblockPage();
    });


    $('#div6').show('slow');

}

// ShowPhyProgress

function ShowPhyProgress() {

    blockPage();
    $("#div7").load('/Dashboard/ShowPhyProgressDetails', function () {
        $.validator.unobtrusive.parse($('#frmPhyProgress'));
        unblockPage();
    });


    $('#div7').show('slow');

}



// AwardWorks

function ShowAwardWorks() {

    blockPage();
    $("#div8").load('/Dashboard/AwardWorksLayout', function () {
        $.validator.unobtrusive.parse($('#frmAwardWorks'));
        unblockPage();
    });


    $('#div8').show('slow');

}


//ShowGreenTech

function ShowGreenTech() {

    blockPage();
    $("#div9").load('/Dashboard/ShowGreenTechDetails', function () {
        $.validator.unobtrusive.parse($('#frmShowGreenTech'));
        unblockPage();
    });


    $('#div9').show('slow');

}

//GreenTechAcheivement

function GreenTechAcheivement() {

    blockPage();
    $("#div10").load('/Dashboard/GreenTechAcheivementDetails', function () {
        $.validator.unobtrusive.parse($('#frmGreenTechAcheivement'));
        unblockPage();
    });


    $('#div10').show('slow');

}



function TechWork() {

    blockPage();
    $("#div11").load('/Dashboard/TechWorkDetails', function () {
        $.validator.unobtrusive.parse($('#frmTechWorkDetails'));
        unblockPage();
    });


    $('#div11').show('slow');

}

function FinacialCl() {
    blockPage();
    $("#div12").load('/Dashboard/FCDetails', function () {
        $.validator.unobtrusive.parse($('#frmFCDetails'));
        unblockPage();
    });
    $('#div12').show('slow');
}

function UnderDLP() {

    blockPage();
    $("#div13").load('/Dashboard/UnderDLPDetails', function () {
        $.validator.unobtrusive.parse($('#frmUnderDLPDetails'));
        unblockPage();
    });


    $('#div13').show('slow');

}

