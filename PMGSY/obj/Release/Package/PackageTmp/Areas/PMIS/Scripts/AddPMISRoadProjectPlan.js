$(document).ready(function () {
    /*Module : PMIS
      Created : October 2020
      Author  : Aditi
   */
    $('.TPS').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Planned Date',
        maxDate: null,   //"0D",  to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Planned Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {

            $('.TPS').trigger('blur');
            $("#frmAddProjectPlan").valid();
            $(this).change();
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
        maxDate: null,   //"0D",  to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Planned Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {

            $('.TCD').trigger('blur');
            $("#frmAddProjectPlan").valid();
            $(this).change();
            $('.input-validation-error').addClass('input-validation-valid');
            $('.input-validation-error').removeClass('input-validation-error');
            //Removes validation message after input-fields
            $('.field-validation-error').addClass('field-validation-valid');
            $('.field-validation-error').removeClass('field-validation-error');
            //Removes validation summary 
            $('.validation-summary-errors').addClass('validation-summary-valid');
            $('.validation-summary-errors').removeClass('validation-summary-errors');
        }

    }).on("change", function () {
        duration($(this).attr('id'));
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        debugger;
        var DataToSend = [];
        $('#tblAddProjectPlan tr[id]').each(function () {
            DataToSend.push({
                "ACTIVITY_DESC": $(this).attr('id'),
                "QUANTITY": $(this).find("input[id^='quantity']").val(),
                "AGREEMENT_COST": $(this).find("input[id$='cost']").val(),
                "PLANNED_START_DATE": $(this).find("input[id^='StartDate']").val(),
                "PLANNED_DURATION": $(this).find("input[id^='duration']").val(),
                "PLANNED_COMPLETION_DATE": $(this).find("input[id^='completionDate']").val(),
                //Form: $('#frmAddProjectPlan').serializeArray(),
                "IMS_PR_ROAD_CODE": $('#roadCode').val()   //$(this).find("input[id$='roadcode']").val(),
                //"IMS_ROAD_NAME": $('#roadName').val() //$(this).find("input[id$='roadName']").val()                
            });
        });

        //if ($('#frmAddProjectPlan').valid()) {
        //if (validate()) {
        //CalculateTotalCost();
        if ($("#hdnOperation").val() == "A") {

            $.ajax({
                url: '/PMIS/PMIS/SaveRoadProjectPlan',
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
                        alert("Plan Added Successfully.");

                        //ResetForm();
                        ClosePMISRoadDetails();
                        unblockPage();
                        LoadPMISRoadList();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html(response.ErrorMessage);
                        $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                        unblockPage();
                    }

                }
            });
        }

        if ($("#hdnOperation").val() == "U") {

            $.ajax({
                url: '/PMIS/PMIS/UpdatePMISRoadProjectPlan',
                type: "POST",
                cache: false,
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
                    if (response.Success) {
                        alert("Plan Updated Successfully.");
                        //ResetForm();
                        ClosePMISRoadDetails();
                        unblockPage();
                        LoadPMISRoadList();
                    }
                    else {
                        unblockPage();
                        $('#mainDiv').animate({ scrollTop: 0 }, 'slow');

                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html(response.ErrorMessage);
                        $.validator.unobtrusive.parse($('#mainDiv'));
                    }
                    unblockPage();
                }
            });
        }

        //}
        //else {
        //    return false;
        //}
        //return false;

    });

    //Total Agreement Cost
    $(":text[class~=TAC]").blur(function () {
        var fltTPEC = 0.0;
        $(":text[class~=TAC]").each(function () {
            var tempValue = $(this).val().replace(new RegExp("\,", "g"), "");
            if (Number(tempValue) != NaN) {
                fltTPEC += Number(tempValue);
                $("#txtTotalAgreementCost").val(parseFloat(fltTPEC).toFixed(2));
            }
        });
    });

    //Minimum Total Planned Start Date
    $(":text[class~=TPS]").blur(function () {
        debugger;
        var minDate;
        for (var i = 0; i < $(".TPS").length; i++) {
            var notnull = $("#StartDate" + i).datepicker("getDate");
            if (notnull != null) {
                minDate = notnull;
                break;
            }
        }
        //var minDate = $("#StartDate" + 0).datepicker("getDate");
        for (var i = 0; i < $(".TPS").length; i++) {
            var selectedDate = $("#StartDate" + i).datepicker("getDate");
            if (selectedDate !== null) {
                if (minDate > selectedDate) {
                    minDate = selectedDate;
                    var result = $("#StartDate" + i).datepicker("getDate");
                    var date = minDate.getDate();
                    var month = (minDate.getMonth() + 1);
                    var year = minDate.getFullYear();
                    var formattedDate = date + "/" + month + "/" + year;
                    $("#txtTotalPlannedStartDate").val(formattedDate);

                }
                else {
                    var date = minDate.getDate();
                    var month = (minDate.getMonth() + 1);
                    var year = minDate.getFullYear();
                    var formattedDate = date + "/" + month + "/" + year;
                    $("#txtTotalPlannedStartDate").val(formattedDate);
                }
            }
        }
    });

    //Maximum Total Planned Completion Date
    $(":text[class~=TCD]").blur(function () {
        debugger;
        var maxDate = $("#completionDate" + 0).datepicker("getDate");
        //var maxDate = new Date(stringDate[2], stringDate[1] - 1, stringDate[0]);

        for (var i = 0; i < $(".TCD").length; i++) {
            var selectedDate = $("#completionDate" + i).datepicker("getDate");
            //var selectedDate = new Date(strDate[2], strDate[1] - 1, strDate[0]);
            if (selectedDate !== null) {
                if (maxDate < selectedDate) {
                    maxDate = selectedDate;
                    var result = $("#completionDate" + i).datepicker("getDate");
                    var date = maxDate.getDate();
                    var month = (maxDate.getMonth() + 1);
                    var year = maxDate.getFullYear();
                    var formattedDate = date + "/" + month + "/" + year;
                    $("#txtTotalPlannedCompletion").val(formattedDate);
                }
                else {
                    var date = maxDate.getDate();
                    var month = (maxDate.getMonth() + 1);
                    var year = maxDate.getFullYear();
                    var formattedDate = date + "/" + month + "/" + year;
                    $("#txtTotalPlannedCompletion").val(formattedDate);
                }
            }
        }
        $("#txtTotalPlannedCompletion").trigger('click');
    });

    //$(":text[class~=TCD]").click(function () {
    //    debugger;
    //    var stringDate = $("#completionDate" + 0).val().split("/");
    //    var maxDate = new Date(stringDate[2], stringDate[1] - 1, stringDate[0]);

    //    for (var i = 0; i < $(".TCD").length; i++) {
    //        var strDate = $("#completionDate" + i).val().split("/");
    //        var selectedDate = new Date(strDate[2], strDate[1] - 1, strDate[0]);
    //        if (selectedDate !== null) {
    //            if (maxDate < selectedDate) {
    //                maxDate = selectedDate;
    //                //var result = new Date($("#completionDate" + i).val());
    //                var date = maxDate.getDate();
    //                var month = (maxDate.getMonth() + 1);
    //                var year = maxDate.getFullYear();
    //                var formattedDate = date + "/" + month + "/" + year;
    //                $("#txtTotalPlannedCompletion").val(formattedDate);
    //            }
    //            else {
    //                var date = maxDate.getDate();
    //                var month = (maxDate.getMonth() + 1);
    //                var year = maxDate.getFullYear();
    //                var formattedDate = date + "/" + month + "/" + year;
    //                $("#txtTotalPlannedCompletion").val(formattedDate);
    //            }
    //        }
    //    }
    //    $("#txtTotalPlannedCompletion").trigger('click');
    //});

    //Total Duration
    $("#txtTotalPlannedCompletion").click(function () {

        var duration = 0;
        for (var i = 0; i < $(".TDR").length; i++) {
            var strDate = $("#txtTotalPlannedStartDate").val().split("/");
            var minStartDate = new Date(strDate[2], strDate[1] - 1, strDate[0]);
            var stringDate = $("#txtTotalPlannedCompletion").val().split("/");
            var maxComplDate = new Date(stringDate[2], stringDate[1] - 1, stringDate[0]);
            var duration = parseInt((maxComplDate - minStartDate) / (24 * 3600 * 1000));
            $("#txtTotalPlannedDuration").val(duration);
        }
    });

    $("#btnCancel").click(function () {
        ClosePMISRoadDetails();
    });

    /*$(":text[class~=QKm]").blur(function () {
        debugger;
        //var index = $(this).attr("id").split("quantity");
        var SanctionLength = $("#txtSancLength").text();
        for (var i = 0; i < $(".QKm").length; i++) {
            var Qtykm = $("#quantity" + i).val();
            if (!isNaN(Qtykm)) {
                if (Qtykm !== undefined && SanctionLength !== undefined) {
                    if (Qtykm > SanctionLength) {
                        alert("Quantity in Km can't be greater than sanctioned length.");
                        return false;
                    }
                }
            }
        }
    });*/

    $(":text[class~=Qm]").blur(function () {
        debugger;
        for (var i = 0; i < $(".Qm").length; i++) {
            var Qtym = $("#quantity" + i).val();
            if (!isNaN(Qtym)) {
                if (Qtym !== undefined) {
                    if (Qtym > 100) {
                        alert("Quantity in m can't be greater than 100.");
                    }
                }
            }
        }
    });

    $(":text[class~=QNo]").blur(function () {
        debugger;
        for (var i = 0; i < $(".QNo").length; i++) {
            var QtyNo = $("#quantity" + i).val();
            if (!isNaN(QtyNo)) {
                if (QtyNo !== undefined) {
                    if (QtyNo > 99) {
                        alert("Quantity in No. can't be greater than 99.");
                    }
                }
            }
        }
    });
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

////Row-wise Planned Completion AutoCalculation
//function addDays(days, id) {
//    debugger;
//    var StartDate = $("#StartDate" + id).datepicker("getDate");
//    StartDate.setDate(StartDate.getDate() + parseInt(days));
//    var month = "0" + (StartDate.getMonth() + 1);
//    var date = "0" + StartDate.getDate();
//    month = month.slice(-2);
//    date = date.slice(-2);
//    var date = date + "/" + month + "/" + StartDate.getFullYear();
//    $("#completionDate" + id).val(date);  //$(this).find("input[id$='completionDate").val(date); 
//    $(".TCD").trigger('click');
//}

//Row-wise Planned Duration AutoCalculation
function duration(id) {
    debugger;
    var rowid = id.replace(/[^0-9]/g, '');
    var StartDate = $("#StartDate" + rowid).datepicker("getDate");
    var dates = StartDate.getDate();
    var months = (StartDate.getMonth() + 1);
    var years = StartDate.getFullYear();
    var formattedStrDate = months + "/" + dates + "/" + years;
    //var minStartDate = new Date(StartDate[2], StartDate[1] - 1, StartDate[0]);
    var CompletionDate = $("#completionDate" + rowid).datepicker("getDate");
    var datec = CompletionDate.getDate();
    var monthc = (CompletionDate.getMonth() + 1);
    var yearc = CompletionDate.getFullYear();
    var formattedCmpDate = monthc + "/" + datec + "/" + yearc;
    //var maxComplDate = new Date(CompletionDate[2], CompletionDate[1] - 1, CompletionDate[0]);
    var datestr = new Date(formattedStrDate);
    var datecmp = new Date(formattedCmpDate);
    var duration = parseInt((datecmp - datestr) / (24 * 3600 * 1000));
    $("#duration" + rowid).val(duration);
    $(".TCD").trigger('click');
}




