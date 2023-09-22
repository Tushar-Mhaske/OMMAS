$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmCompletedWorksLayout'));

    $("#btnViewCompletedWorks").click(function () {
        //viewCompletedWorks();

        if ($('#frmCompletedWorksLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/CompletedWorksReport/',
                type: 'POST',
                catche: false,
                data: $("#frmCompletedWorksLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadCompletedWorksReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    //$(function () {
    //    $("#accordionCompletedObsDetails").accordion({
    //        icons: false,
    //        heightStyle: "content",
    //        autoHeight: false
    //    });
    //});

});

$("#btnViewCompletedWorks").trigger('click');

$("#spCollapseIconCN").click(function () {

    $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    $("#frmCompletedWorksLayout").toggle("slow");

});

$("#FROM_DATE").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: "dd/mm/yy",
    showOn: 'button',
    buttonImage: '../../Content/images/calendar_2.png',
    buttonImageOnly: true,
    onSelect: function (selectedDate) {
        $("#TO_DATE").datepicker("option", "minDate", selectedDate);
        $(function () {
            $('#FROM_DATE').focus();
            $('#TO_DATE').focus();
        })
    },
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
    onSelect: function (selectedDate) {
        $("#FROM_DATE").datepicker("option", "maxDate", selectedDate);
    },
    onClose: function () {
        $(this).focus().blur();
    }
}).attr('readonly', 'readonly');


//function viewCompletedWorks() {
//    blockPage();
//    $.ajax({
//        url: '/QualityMonitoring/QMCompletedWorkDetails',
//        type: 'POST',
//        data: { frmDate: $("#FROM_DATE").val(), toDate: $("#TO_DATE").val() },
//        success: function (response) {
//            $("#divCompletedWorksDetails").html('');
//            $("#divCompletedWorksDetails").html(response);
//            unblockPage();
//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            unblockPage();
//            alert(xhr.status);
//            alert(thrownError);
//        }
//    });
//}

//function showInspDetails(roadCode, qmType) {
//    blockPage();
//    $("#divCompletedInspDetails").load('/QualityMonitoring/QMCompletedInspDetails/' + roadCode + '$' + qmType, function () {
//        $("#lblCompletedInspDetailsHeader").html("");
//        $("#lblCompletedInspDetailsHeader").html($("#WORK_NAME").val());
//        unblockPage();
//    });


//    $('#divCompletedInspDetailsHeader').show('slow');
//    $('#divCompletedInspDetails').show('slow');
//    $("#divCompletedWorksTbl").hide('slow');
//}

//function CloseCompletedObsDetails() {
//    blockPage();
//    $('#accordionCompletedObsDetails').hide('slow');
//    $('#divCompletedObsDetails').hide('slow');
//    $('#divCompletedInspTbl').show('slow');
//    unblockPage();
//}


//function CloseCompletedInspDetails() {
//    blockPage();
//    $('#divCompletedInspDetails').hide('slow');
//    $("#divCompletedInspDetailsHeader").hide('slow');
//    $("#divCompletedWorksTbl").show('slow');
//    unblockPage();
//}


//function ShowObsDetails(obsId) {
//    window.open("/QualityMonitoring/QMObservationDetailsRpt/" + obsId);
//}
