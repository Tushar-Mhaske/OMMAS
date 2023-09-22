$(document).ready(function () {
    /*Module : PMIS
       Created : October 2020
       Author  : Aditi
    */
    $('#reasonLabel').hide();
    $('#remarks').hide();


    // Changes Started by saurabh here

    var currentDate = new Date();
    var currDay = currentDate.getDate();

    //Change by Avinash on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
    var startDate = parseInt(currDay) <= 30 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);
    //if max Date is Zero it is defaultly taking Today's Date
    //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.

    //var maxDate = (date.getMonth() == 3 && parseInt(currDay) <= 10) ? lastDay : 0;
    var maxDate = (date.getMonth() == 3 && parseInt(currDay) <= 13) ? lastDay : 0;  // change
    //alert("lastDay " + lastDay)
    //alert("firstDay " + firstDay)
    //alert("date " + date)
    //alert("maxDate " + maxDate)

    var currentTime = new Date();
    // First Date Of the month 
    var startDateFrom = new Date(currentTime.getFullYear(), currentTime.getMonth(), 1);
    // Last Date Of the Month 
    var startDateTo = new Date(currentTime.getFullYear(), currentTime.getMonth() + 1, 0);

    //if (currentTime.getMonth() + 1 == 4 && currentTime.getDate() > 13)  // change 
    //{
    //    $('.EPD').datepicker({
    //        dateFormat: 'dd/mm/yy',
    //        showOn: "button",
    //        buttonImage: "/Content/Images/calendar_2.png",
    //        showButtonText: 'Progress Entry Date',
    //        maxDate: "0D",   //null,  to disable future dates
    //        minDate: startDateFrom,    //"0D",  to disable past dates
    //        buttonImageOnly: true,
    //        buttonText: 'Progress Entry Date',
    //        changeMonth: false,
    //        changeYear: false,
    //        stepMonths: false,
    //        onSelect: function (selectedDate) {

    //            $('.EPD').trigger('blur');
    //        }
    //    });
    //}
    //else
    //{
    //    $('.EPD').datepicker({
    //        dateFormat: 'dd/mm/yy',
    //        showOn: "button",
    //        buttonImage: "/Content/Images/calendar_2.png",
    //        showButtonText: 'Progress Entry Date',
    //        maxDate: maxDate,   //null,  to disable future dates
    //        minDate: lastDay,    //"0D",  to disable past dates
    //        buttonImageOnly: true,
    //        buttonText: 'Progress Entry Date',
    //        changeMonth: false,
    //        changeYear: false,
    //        stepMonths: false,
    //        onSelect: function (selectedDate) {

    //            $('.EPD').trigger('blur');
    //        }

    //    });

    //}


    $('.EPD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Progress Entry Date',
        maxDate: "0D",   //null,  to disable future dates
        minDate: "0D",    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Progress Entry Date',
        changeMonth: false,
        changeYear: false,
        stepMonths: false,
        onSelect: function (selectedDate) {

            $('.EPD').trigger('blur');
        }

    });


    $('.TPS').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Planned Date',
        maxDate: "0D",  //to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Planned Date',
        changeMonth: true,
        changeYear: true,
        stepMonths: false,
        onSelect: function (selectedDate) {
            debugger;
            //$('.TPS').trigger('blur');
            //$("#frmAddProjectPlan").valid();
            //alert("Value = " + $(this).val() + " \nid = " + $(this).attr("id") + " \nselectedDate = " + selectedDate);
            var index = $(this).attr("id").split("startedDate");
            //alert(index[1]);
            //if ($('#actquantity' + index[1]).val() == "") {
            //    $('#schedule' + index[1]).val("");
            //}
            //else {
            //In case if only the actual start date is entered for an activity, the schedule is calculated wrt. However, if the actual completion date is also entered for an activity, the schedule has to be calculated wrt (Actual completion - planned completion). 
            var startDate = $('input[id^=StartDate' + index[1] + ']').val();
            var startedDate = $('input[id^=startedDate' + index[1] + ']').val();
            if (startDate !== undefined && startedDate !== undefined) {
                var startDateSplit = startDate.split("/");
                var startedDateSplit = startedDate.split("/");
                var date1 = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
                var date2 = new Date(startedDateSplit[2], startedDateSplit[1] - 1, startedDateSplit[0]);
                var diffTime = Math.abs(date2 - date1);
                var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            }

            if (diffDays == 0) {
                $('#schedule' + index[1]).val("0");
            }
            else {
                if (date2 - date1 < 0) {
                    $('#schedule' + index[1]).val("Leading by " + diffDays + " days.");
                }
                else {
                    $('#schedule' + index[1]).val("Lagging by " + diffDays + " days.");
                }
            }

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
    $('.TCD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Planned Date',
        maxDate: '0D', //null to allow all dates,   //"0D",  to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Planned Date',
        changeMonth: true,
        changeYear: true,
        stepMonths: false,
        onSelect: function (selectedDate) {
            debugger;
            var index = $(this).attr("id").split("finishedDate");

            //In case if only the actual start date is entered for an activity, the schedule is calculated wrt. However, if the actual completion date is also entered for an activity, the schedule has to be calculated wrt (Actual completion - planned completion). 
            //New logic added on 15 Jan 2021
            var compDate = $('input[id^=completionDate' + index[1] + ']').val();
            var finishedDate = $('input[id^=finishedDate' + index[1] + ']').val();
            if (compDate !== undefined && finishedDate !== undefined) {
                var compDateSplit = compDate.split("/");
                var finishedDateSplit = finishedDate.split("/");
                var date3 = new Date(compDateSplit[2], compDateSplit[1] - 1, compDateSplit[0]);
                var date4 = new Date(finishedDateSplit[2], finishedDateSplit[1] - 1, finishedDateSplit[0]);
                var diffTime2 = Math.abs(date4 - date3);
                var diffDays2 = Math.ceil(diffTime2 / (1000 * 60 * 60 * 24));
            }

            if (diffDays2 == 0) {
                $('#schedule' + index[1]).val("0");
            }
            else if (!isNaN(diffDays2)) {
                if (date4 - date3 < 0) {
                    $('#schedule' + index[1]).val("Leading by " + diffDays2 + " days.");
                }
                else {
                    $('#schedule' + index[1]).val("Lagging by " + diffDays2 + " days.");
                }
            }

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

    $('#btnSave').click(function (evt) {
        evt.preventDefault();
        debugger;
        $('#txtCompletedRoadLength').rules('add', {
            required: true,
            messages: {
                required: 'Completed Road Length is Required'
            }
        });
        $('#progessEntryDate').rules('add', {
            required: true,
            messages: {
                required: 'Date of progess entry is Required'
            }
        });
        $('#ProjectStatus').rules('add', {
            required: true,
            messages: {
                required: 'Project Status is Required'
            }
        });
        var DataToSend = [];
        $('#tblAddActuals tr[id]').each(function () {
            DataToSend.push({
                "ACTIVITY_DESC": $(this).attr('id'),
                "QUANTITY": $(this).find("input[id^='quantity']").val(),
                "ACTUAL_QUANTITY": $(this).find("input[id^='actquantity']").val(),
                "AGREEMENT_COST": $(this).find("input[id^='cost']").val(),
                "PLANNED_START_DATE": $(this).find("input[id^='StartDate']").val(),
                "PLANNED_COMPLETION_DATE": $(this).find("input[id^='completionDate']").val(),
                "STARTED_DATE": $(this).find("input[id^='startedDate']").val(),
                "FINISHED_DATE": $(this).find("input[id^='finishedDate']").val(),
                "CompletedRoadLength": $("#txtCompletedRoadLength").val(),
                "Date_of_progress_entry": $("#progessEntryDate").val(),
                "ProjectStatus": $("#ProjectStatus").val(),
                "IMS_PR_ROAD_CODE": $("#RoadCode").val(),
                "Remarks": $("#remarks").val()
            });
        });

        $.ajax({
            url: '/PMIS/PMIS/SaveBridgeActuals',
            type: "POST",
            cache: false,
            //data: $("#frmAddProjectPlan").serialize(),
            data: JSON.stringify(DataToSend),
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Success) {
                    alert("Actuals Saved Successfully.");

                    //ResetForm();
                    ClosePMISBridgeDetails();
                    unblockPage();
                    LoadPMISBridgeList();
                  //  LoadPMISRoadList();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html(response.ErrorMessage);
                    $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                    unblockPage();
                }

            }
        });

    });

    $('#btnSumit').click(function (evt) {
        evt.preventDefault();
        debugger;
        var DataToSend = [];
        $('#tblAddActuals tr[id]').each(function () {
            DataToSend.push({
                "ACTIVITY_DESC": $(this).attr('id'),
                "QUANTITY": $(this).find("input[id^='quantity']").val(),
                "ACTUAL_QUANTITY": $(this).find("input[id^='actquantity']").val(),
                "AGREEMENT_COST": $(this).find("input[id^='cost']").val(),
                "PLANNED_START_DATE": $(this).find("input[id^='StartDate']").val(),
                "PLANNED_COMPLETION_DATE": $(this).find("input[id^='completionDate']").val(),
                "STARTED_DATE": $(this).find("input[id^='startedDate']").val(),
                "FINISHED_DATE": $(this).find("input[id^='finishedDate']").val(),
                "CompletedRoadLength": $("#txtCompletedRoadLength").val(),
                "Date_of_progress_entry": $("#progessEntryDate").val(),
                "ProjectStatus": $("#ProjectStatus").val(),
                "IMS_PR_ROAD_CODE": $("#RoadCode").val(),
                "Remarks": $("#remarks").val()
            });
        });

        $.ajax({
            url: '/PMIS/PMIS/SubmitBridgeActuals',
            type: "POST",
            cache: false,
            //data: $("#frmAddProjectPlan").serialize(),
            data: JSON.stringify(DataToSend),
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Success) {
                    alert("Actuals Submitted Successfully.");

                    //ResetForm();
                    ClosePMISBridgeDetails();
                    unblockPage();
                    LoadPMISBridgeList();
                  //  LoadPMISRoadList();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html(response.ErrorMessage);
                    $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                    unblockPage();
                }

            }
        });

    });

    $("#btnCancel").click(function () {
        if (confirm("Are you sure to cancel and close?")) {
            ClosePMISBridgeDetails();
        }
    });

    $("#ProjectStatus").click(function () {
        if ($('#ProjectStatus').val() == "O") {
            $('#reasonLabel').show();
            $('#remarks').show();
        }
        if ($('#ProjectStatus').val() != "O") {
            $('#reasonLabel').hide();
            $('#remarks').hide();
        }
    });

    $(":text[class~=TPS]").blur(function () {
        debugger;
        var index = $(this).attr("id").split("startedDate");
        var startDate = $('input[id^=StartDate' + index[1] + ']').val();
        var startedDate = $('input[id^=startedDate' + index[1] + ']').val();
        if (startDate !== undefined && startedDate !== undefined) {
            var startDateSplit = startDate.split("/");
            var startedDateSplit = startedDate.split("/");
            var date1 = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
            var date2 = new Date(startedDateSplit[2], startedDateSplit[1] - 1, startedDateSplit[0]);
            var diffTime = Math.abs(date2 - date1);
            var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        }

        if (diffDays == 0) {
            $('#schedule' + index[1]).val("0");
        }
        else {
            if (date2 - date1 < 0) {
                $('#schedule' + index[1]).val("Leading by " + diffDays + " days.");
            }
            else {
                $('#schedule' + index[1]).val("Lagging by " + diffDays + " days.");
            }
        }
    });

    $(":text[class~=TCD]").blur(function () {
        debugger;
        var index = $(this).attr("id").split("finishedDate");
        var compDate = $('input[id^=completionDate' + index[1] + ']').val();
        var finishedDate = $('input[id^=finishedDate' + index[1] + ']').val();
        if (compDate !== undefined && finishedDate !== undefined) {
            var compDateSplit = compDate.split("/");
            var finishedDateSplit = finishedDate.split("/");
            var date3 = new Date(compDateSplit[2], compDateSplit[1] - 1, compDateSplit[0]);
            var date4 = new Date(finishedDateSplit[2], finishedDateSplit[1] - 1, finishedDateSplit[0]);
            var diffTime2 = Math.abs(date4 - date3);
            var diffDays2 = Math.ceil(diffTime2 / (1000 * 60 * 60 * 24));
        }

        if (diffDays2 == 0) {
            $('#schedule' + index[1]).val("0");
        }
        else if (!isNaN(diffDays2)) {
            if (date4 - date3 < 0) {
                $('#schedule' + index[1]).val("Leading by " + diffDays2 + " days.");
            }
            else {
                $('#schedule' + index[1]).val("Lagging by " + diffDays2 + " days.");
            }
        }
    });

    /*$(":text[class~=AQTY]").blur(function () {     //This validation is handled from controller class
        debugger;
        //parseFloat(cellValue).toFixed(2)
        var index = $(this).attr("id").split("actquantity");
        var planQty = parseFloat($('input[id^=quantity' + index[1] + ']').val());
        var actualQty = parseFloat($('input[id^=actquantity' + index[1] + ']').val());
        if (planQty !== undefined && actualQty !== undefined) {
            if (actualQty > planQty) {
                alert("Quantity executed as on date can't be greater than planned quantity.");
            }
        }
    });*/
});

function isNumericKeyStroke(event) {
    var returnValue = false;
    var keyCode = (event.which) ? event.which : event.keyCode;
    if (((keyCode >= 48) && (keyCode <= 57)) || (keyCode == 46) || (keyCode == 8) || (keyCode == 9) || (keyCode == 37) || (keyCode == 39))// All numerics
    {
        returnValue = true;
    }
    if (event.returnValue)
        event.returnValue = returnValue;
    return returnValue;
}

//Row-wise Planned Completion AutoCalculation
function addDays(days, id) {
    debugger;
    var StartDate = $("#StartDate" + id).datepicker("getDate");
    StartDate.setDate(StartDate.getDate() + parseInt(days));
    var month = "0" + (StartDate.getMonth() + 1);
    var date = "0" + StartDate.getDate();
    month = month.slice(-2);
    date = date.slice(-2);
    var date = date + "/" + month + "/" + StartDate.getFullYear();
    $("#completionDate" + id).val(date);  //$(this).find("input[id$='completionDate").val(date); 
    $(".TCD").trigger('click');
}

//$(document).on('change', 'input[id^="actquantity"]', function() {
//    //alert("Value = " + $(this).val() + " id = " + $(this).attr("id"));
//    var index = $(this).attr("id").split("actquantity");
//    //alert(index[1]);
//    if ($(this).val() == "") {
//        $('#schedule' + index[1]).val("");
//    }
//    else {
//        var startDate = $('input[id^=StartDate' + index[1] + ']').val();
//        var startedDate = $('input[id^=startedDate' + index[1] + ']').val();
//        var startDateSplit = startDate.split("/");
//        var startedDateSplit = startedDate.split("/");
//        var date1 = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
//        var date2 = new Date(startedDateSplit[2], startedDateSplit[1] - 1, startedDateSplit[0]);
//        var diffTime = Math.abs(date2 - date1);
//        var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
//        if (diffDays == 0) {
//            $('#schedule' + index[1]).val("");
//        }
//        else {
//            if (date2 - date1 < 0) {
//                $('#schedule' + index[1]).val("Leading by " + diffDays + " days.");
//            }
//            else {
//                $('#schedule' + index[1]).val("Lagging by " + diffDays + " days.");
//            }
//        }        
//    }
//});

//$(document).on('change', 'input[id^="startedDate"]', function () {
//    debugger;
//    //alert("Value = " + $(this).val() + " id = " + $(this).attr("id"));
//    var index = $(this).attr("id").split("startedDate");
//    //alert(index[1]);
//    //if ($('#actquantity' + index[1]).val() == "") {
//    //    $('#schedule' + index[1]).val("");
//    //}
//    //else {
//        var startDate = $('input[id^=StartDate' + index[1] + ']').val();
//        var startedDate = $('input[id^=startedDate' + index[1] + ']').val();
//        var startDateSplit = startDate.split("/");
//        var startedDateSplit = startedDate.split("/");
//        var date1 = new Date(startDateSplit[2], startDateSplit[1] - 1, startDateSplit[0]);
//        var date2 = new Date(startedDateSplit[2], startedDateSplit[1] - 1, startedDateSplit[0]);
//        var diffTime = Math.abs(date2 - date1);
//        var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
//        if (diffDays == 0) {
//            $('#schedule' + index[1]).val("0");
//        }
//        else {
//            if (date2 - date1 < 0) {
//                $('#schedule' + index[1]).val("Leading by " + diffDays + " days.");
//            }
//            else {
//                $('#schedule' + index[1]).val("Lagging by " + diffDays + " days.");
//            }
//        }
//    //}
//});