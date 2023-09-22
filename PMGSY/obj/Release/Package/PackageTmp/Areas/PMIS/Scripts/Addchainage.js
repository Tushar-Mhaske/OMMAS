$(document).ready(function () {

    $('.ECD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Chainage Entry Date',
        maxDate: null,   //"0D",  to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Chainage Entry Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            ////    $('.ECD').trigger('blur');   
            $('.input-validation-error').addClass('input-validation-valid');
            $('.input-validation-error').removeClass('input-validation-error');
            //Removes validation message after input-fields
            $('.field-validation-error').addClass('field-validation-valid');
            $('.field-validation-error').removeClass('field-validation-error');
            //Removes validation summary 
            $('.validation-summary-errors').addClass('validation-summary-valid');
            $('.validation-summary-errors').removeClass('validation-summary-errors');

        }
    });

    //$('#btnSubmit').click(function () {
    //    debugger;
    //    var formdata = $("#frmAddChainage");
    //    //var DataToSend = [];
    //    //$('#tblAddActuals tr[id]').each(function () {
    //    //    DataToSend.push({
    //    //        "earthworklist": $(this).attr('id'),
    //    //        "subgradelist": $(this).find("input[id^='quantity']").val(),
    //    //        "granularsubbaselist": $(this).find("input[id^='actquantity']").val(),
    //    //        "wbmgrading2list": $(this).find("input[id^='cost']").val(),
    //    //        "wbmgrading3list": $(this).find("input[id^='StartDate']").val(),
    //    //        "wetmixmacadamlist": $(this).find("input[id^='completionDate']").val(),
    //    //        "bituminousmacadamlist": $(this).find("input[id^='startedDate']").val(),
    //    //        "surfacecourselist": $(this).find("input[id^='finishedDate']").val(),
    //            //"CompletedRoadLength": $("#txtCompletedRoadLength").val(),
    //            //"ProjectStatus": $("#ProjectStatus").val(),
    //            //"IMS_PR_ROAD_CODE": $("#RoadCode").val()
    //       // });
    //   // });
    //    $.ajax({
    //        type: 'POST',
    //        url: '/PMIS/PMIS/SubmitChainageDetails',
    //        data: formdata.serialize(),//$("#frmAddChainage").serialize(),
    //        //data: JSON.stringify(DataToSend),
    //        //contentType: 'application/json; charset=utf-8',
    //        success: function (data) {
    //            alert(data.message);
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert("Error occurred while processing the request.");
    //        }
    //    })

    //});



    $("#btnCancel").click(function () {
        ClosePMISRoadDetails();
    });

});

function OnSuccess(response) {
    alert(response.Message);
    ClosePMISRoadDetails();
}
function OnFailure(response) {
    alert("Error occurred while processing the request.");
}