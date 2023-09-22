/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMCompletedWorkFilters.js
        * Description   :   Handles events for Completed Work Filters Report
        * Author        :   Shyam Yadav 
        * Creation Date :   01/Aug/2014
 **/

$(document).ready(function () {

    $("#btnViewCompletedWorks").click(function () {
        viewCompletedWorks();
    });

    $(function () {
        $("#accordionCompletedObsDetails").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

});

$("#FROM_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: "dd/mm/yy",
    showOn: 'button',
    buttonImage: '../../Content/images/calendar_2.png',
    buttonImageOnly: true,
    onClose: function () {
        $(this).focus().blur();
    }
}).attr('readonly', 'readonly');


$("#TO_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: "dd/mm/yy",
    showOn: 'button',
    buttonImage: '../../Content/images/calendar_2.png',
    buttonImageOnly: true,
    onClose: function () {
        $(this).focus().blur();
    }
}).attr('readonly', 'readonly');


function viewCompletedWorks() {
    blockPage();
    $.ajax({
        url: '/QualityMonitoring/QMCompletedWorkDetails',
        type: 'POST',
        data: { frmDate: $("#FROM_DATE").val(), toDate: $("#TO_DATE").val() },
        success: function (response) {
            $("#divCompletedWorksDetails").html('');
            $("#divCompletedWorksDetails").html(response);
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            unblockPage();
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function showInspDetails(roadCode, qmType)
{
    blockPage();
    $("#divCompletedInspDetails").load('/QualityMonitoring/QMCompletedInspDetails/' + roadCode + '$' + qmType , function () {
        $("#lblCompletedInspDetailsHeader").html("");
        $("#lblCompletedInspDetailsHeader").html($("#WORK_NAME").val());
        unblockPage();
    });


    $('#divCompletedInspDetailsHeader').show('slow');
    $('#divCompletedInspDetails').show('slow');
    $("#divCompletedWorksTbl").hide('slow');
}

function CloseCompletedObsDetails() {
    blockPage();
    $('#accordionCompletedObsDetails').hide('slow');
    $('#divCompletedObsDetails').hide('slow');
    $('#divCompletedInspTbl').show('slow');
    unblockPage();
}


function CloseCompletedInspDetails() {
    blockPage();
    $('#divCompletedInspDetails').hide('slow');
    $("#divCompletedInspDetailsHeader").hide('slow');
    $("#divCompletedWorksTbl").show('slow');
    unblockPage();
}


function ShowObsDetails(obsId) {
    window.open("/QualityMonitoring/QMObservationDetailsRpt/" + obsId);
}
