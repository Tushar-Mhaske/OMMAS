/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMCommencedWorkDetails.js
        * Description   :   Handles events for QMCommencedWorkDetails Report
        * Author        :   Shyam Yadav 
        * Creation Date :   25/July/2014
 **/
$(document).ready(function () {
    $(function () {
        $("#accordionCommencedObsDetails").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function CloseCommencedObsDetails() {
    $('#accordionCommencedObsDetails').hide('slow');
    $('#divCommencedObsDetails').hide('slow');
    $('#divCommencedInspTbl').show('slow');
}


function CloseCommencedInspDetails() {
    $('#divCommencedInspDetails').hide('slow');
    $("#divCommencedInspDetailsHeader").hide('slow');
    $("#divCommencedWorksTbl").show('slow');
}

function showCommencementDetails(stateCode, duration)
{
    blockPage();
    $("#divCommencedInspDetails").load('/QualityMonitoring/QMCommencedRoadDetails/' + stateCode + '$' + duration, function () {
        $("#lblCommencedInspDetailsHeader").html("");
        $("#lblCommencedInspDetailsHeader").html($("#STATE_NAME").val() + " - " + $("#DURATION").val());
        unblockPage();
    });
    
    
    $('#divCommencedInspDetailsHeader').show('slow');
    $('#divCommencedInspDetails').show('slow');
    $("#divCommencedWorksTbl").hide('slow');
}

function showInspDetails(stateCode, duration, qmType)
{
    blockPage();
    $("#divCommencedInspDetails").load('/QualityMonitoring/QMCommencedInspDetails/' + stateCode + '$' + duration + '$' + qmType, function () {
        $("#lblCommencedInspDetailsHeader").html("");
        $("#lblCommencedInspDetailsHeader").html($("#STATE_NAME").val() + " - " + $("#DURATION").val());
        unblockPage();
    });
    
   
    $('#divCommencedInspDetailsHeader').show('slow');
    $('#divCommencedInspDetails').show('slow');
    $("#divCommencedWorksTbl").hide('slow');
}


function ShowObsDetails(obsId) {

    window.open("/QualityMonitoring/QMObservationDetailsRpt/" + obsId);
}