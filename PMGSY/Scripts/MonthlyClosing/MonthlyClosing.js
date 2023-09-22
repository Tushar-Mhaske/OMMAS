jQuery.validator.addMethod("TODetailsRequired", function (value, element) {

    if (value == "") {
        return false;
    }

    if (parseInt(value) == 0) {
        return false;

    }
    else {
        return true;
    }


}, "");

jQuery.validator.addMethod("TOYearGreaterEqualToFromYear", function (value, element) {

    var toYear = parseInt(value);

    var fromYear = parseInt($("#FROM_YEAR").val());

    if (toYear == 0) {
        return false;
    }

    if (toYear >= fromYear) {
        return true;
    }
    else {
        return false;
    }


}, "");

//Validation Added By Abhishek kamble 18-July-2014 start
jQuery.validator.addMethod("ToMonthYearGreaterThanFromMonthYear", function (value, element) {


    var toMonth = parseInt($("#TO_MONTH").val());
    var fromMonth = parseInt($("#FROM_MONTH").val());

    var toYear = parseInt(value);
    var fromYear = parseInt($("#FROM_YEAR").val());

    //if (toYear == 0) {
    //    return false;
    //}
    //if (fromMonth == 0)
    //{
    //    return false;
    //}

    //validation to check from month and Year should not be same as to month and year.
    // alert("from Month : " + fromMonth + "to Month:" + toMonth);
    //alert("from Year : " + fromYear+ "to Year:" + toYear);

    if ((fromMonth == toMonth) && (fromYear == toYear)) {
        return false;
    } else {
        return true;
    }

}, "");
//Validation Added By Abhishek kamble 18-July-2014 end

jQuery.validator.addMethod("fromMonthYearLessThanEqualToCurrentMonthYear", function (value, element) {

    var fromMonth = parseInt($('#FROM_MONTH').val());
    var fromYear = parseInt($('#FROM_YEAR').val());


    var currentMonth = parseInt($('#CURRENT_MONTH').val());
    var currentYear = parseInt($('#CURRENT_YEAR').val());




    if (currentYear < fromYear) {

        return false;
    }
    else if (currentYear == fromYear) {

        if (currentMonth < fromMonth) {

            return false;
        }
        else {
            return true;
        }

    } else return true;
}, "");


jQuery.validator.addMethod("ToMonthYearLessThanEqualToCurrentMonthYear", function (value, element) {


    var toMonth = parseInt($('#TO_MONTH').val());
    var toYear = parseInt($("#TO_YEAR").val());

    var currentMonth = parseInt($('#CURRENT_MONTH').val());
    var currentYear = parseInt($('#CURRENT_YEAR').val());


    if (currentYear < toYear) {
        return false;
    }
    else if (currentYear == toYear) {

        if (currentMonth < toMonth) {
            return false;
        }
        else {
            return true;
        }

    }
    else return true;



}, "");

jQuery.validator.addMethod("CheckFinancialYear", function (value, element) {

    if (CheckFinanacialYearForMonthAndYear()) {
        return true;
    }

    else {
        return false;
    }

}, "");


$(document).ready(function () {

    $.validator.unobtrusive.parse($("#monthlyClosingForm"));

    //added by Abhishek to show month start/close details at srrda level start
    $(function () {

        var countOwn = 0;
        var countLower = 0;

        $("#rdOwn").click(function () {
            countLower = 0;
            
            if (($("#rdOwn").prop("checked"))) {
                countOwn++;
                if (countOwn == 1) {
                    $("#ddlDPIU").hide("slow");
                    GetClosedMonthAndYear();
                    GetAccountStartMonthYear();
                }
            }

        });

        $("#rdDPIU").click(function () {
            countOwn = 0;
            
            if (($("#rdDPIU").prop("checked"))) {
                countLower++;
                if (countLower == 1) {
                    $("#ddlDPIU").show("slow");
                    GetClosedMonthAndYear();
                    GetAccountStartMonthYear();
                }
            }
        });

        $("#ddlDPIU").change(function () {
            GetClosedMonthAndYear();
            GetAccountStartMonthYear();
        });       

    });
    //added by Abhishek to show month start/close details at srrda level end


    //get monthly closing details
    GetClosedMonthAndYear();

    GetAccountStartMonthYear();

    $("#rdSingle").click(function () {

        $("#singleMonthTr").show('slow');

        $(".TdMulti").hide('slow');

        $("#divFromMonth").css("margin-left", "30%");

        $("#TO_MONTH").val(0);

        $("#TO_YEAR").val(0);

        $('#TO_MONTH').rules("remove", "messages");

        $('#TO_YEAR').rules("remove", "messages");

        $("#TO_MONTH,#TO_YEAR").removeClass("input-validation-error").addClass("input-validation-valid");
        $("#TO_MONTH,#TO_YEAR").next('span').removeClass("field-validation-error").addClass("field-validation-valid");
        $("#TO_MONTH,#TO_YEAR").parent('div').find('span:eq(1)').text("");


    });



    $("#rdMultiple").click(function () {

        $("#singleMonthTr").show('slow');
        $(".TdMulti").show('slow');
        $("#divFromMonth").css("margin-left", "5%");

        $('#TO_MONTH').rules('add', {
            TODetailsRequired: true,
            ToMonthYearLessThanEqualToCurrentMonthYear: true,
            messages:
              {
                  TODetailsRequired: 'To Month Required',
                  ToMonthYearLessThanEqualToCurrentMonthYear: 'TO Month and year <br> should be less than  <br> equal to current month and year'
              }
        });

        $('#TO_YEAR').rules('add', {
            TODetailsRequired: true,
            TOYearGreaterEqualToFromYear: true,
            ToMonthYearLessThanEqualToCurrentMonthYear: true,
            ToMonthYearGreaterThanFromMonthYear: true,
            messages:
              {
                  TODetailsRequired: 'To Year Required',
                  TOYearGreaterEqualToFromYear: "To Year Should be <br> Grater than or equal <br> to From Year",
                  ToMonthYearLessThanEqualToCurrentMonthYear: 'TO Month and year <br> should be less  <br> than equal to current month and year',
                  ToMonthYearGreaterThanFromMonthYear: "To Month or Year should be <br> Greater than From <br> Month or Year."//Validation Added by Abhishek kamble 18-July-2014
              }
        });


    });

    $("#rdSingle").trigger('click');


    $("#btnSubmitDetails").click(function () {

        //Added By Abhishek kamble to hide PIU Chq Ack details start 21-July-2014
        $("#dvShowPIUChequeAckStatus").html('');
        //Added By Abhishek kamble to hide PIU Chq Ack details start


        $('#FROM_MONTH').rules('add', {
           // required: true,
            fromMonthYearLessThanEqualToCurrentMonthYear: true,
            messages:
              {
                  //required: 'Month (From) is Required',
                  fromMonthYearLessThanEqualToCurrentMonthYear: 'From Month and year <br>should be less than <br> equal to current month and year'

              }
        });

        $('#FROM_YEAR').rules('add', {

            fromMonthYearLessThanEqualToCurrentMonthYear: true,
            messages:
              {

                  fromMonthYearLessThanEqualToCurrentMonthYear: 'From Month and year <br> should be less than <br> equal to current month and year'
              }
        });

        $('#TO_YEAR').rules('add', {
            CheckFinancialYear: true,
            messages:
              {
                  CheckFinancialYear: "From Month & Year <br> and To month & year <br> should be in same financial year"
              }
        });


        if ($("#monthlyClosingForm").valid()) {

            //state
            if ($("#levelId").val() == 4) {


            } //else if ($("#levelId").val() == 5)
            {
                //district

                //   if (validatePIUChequeAcknowledgementStatus()) {//if Condition Added By abhishek kamble to check PIU Cheque Acknowledgement Status

                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/MonthlyClosing/CloseMonth/",
                    // async: false,
                    data: $("#monthlyClosingForm").serialize(),
                    //data: { fromMonth: $("#FROM_MONTH").val(), fromYear: $("#FROM_YEAR").val(), toMonth: $("#TO_MONTH").val(), toYear:$("#TO_YEAR").val() },
                    error: function (xhr, status, error) {
                        unblockPage();
                        $('#errorSpan').text(xhr.responseText);
                        $('#divError').show('slow');
                        return false;

                    },
                    success: function (data) {
                        unblockPage();
                        $('#divError').hide('slow');
                        $('#errorSpan').html("");
                        $('#errorSpan').hide();



                        if (data != "") {

                            //Added By Abhishek  kamble To show DPIU chq Ack Status at srrda level 24-July-2014 start
                            if ($("#levelId").val() == 4) {                                                             

                                if ((data.monthlyClosingStatus === undefined) && (data.result===undefined)) {
                                    //if (confirm("Cheques Acknowledge is not done for One or more dpiu's. Do you want to see the list ?")) {
                                        $("#dvShowPIUChequeAckStatus").html(data);
                                    //}
                                    return false;
                                }
                            }                         
                            //Added By Abhishek  kamble To show DPIU chq Ack Status at srrda level end 
                            
                            //Added By Abhishek kamble 26Nov2014 to show aleet if exception occured. start
                            //if(data.result==null)
                            //{
                            //    alert("An Error occured while proccessing your request.");
                            //    return false;
                            //}
                            //Added By Abhishek kamble 26Nov2014 to show aleet if exception occured. end
                            
                            if (data.result !== undefined) {
                                if (data.result == "1") {
                                    alert("Selected Month(s) and Year(s) has been closed.");
                                    GetClosedMonthAndYear();
                                    return false;
                                }
                            }

                          //  alert(data.monthlyClosingStatus);
                            var monthlyClosingStatus = data.monthlyClosingStatus.split('$');

                          

                            if (monthlyClosingStatus.length == 1) {


                                if (monthlyClosingStatus[0] == "-666") {
                                    if ($("#rdSingle").is(":checked")) {
                                        alert("Selected Month and Year is already closed.");
                                        return false;
                                    } else {
                                        alert("one of the Month and Year from selected month & year duration is already closed.");
                                        return false;
                                    }
                                }

                                //month closed success
                                if (monthlyClosingStatus[0] == "1") {

                                    alert("Selected Month(s) and Year(s) has been closed.");
                                    GetClosedMonthAndYear();
                                    return false;
                                }


                                if (monthlyClosingStatus[0] == "-111") {

                                    alert("Please Close Previous month first.");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-123") {

                                    alert("SRRDA has already closed the selected month.This month cant be closed.");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-444") {

                                    alert("One of the Authorization request is not finalized by Authorized Signatory");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-555") {
                                    alert("Please Close Previous month first.");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-777") {
                                    alert("Month and year should be greater than or equal to Month and Year in which account has been started.");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-5555") {
                                    alert("Account is not yet started.Monthly closing cant be done. ");
                                    return false;
                                }

                                if (monthlyClosingStatus[0] == "-999") {
                                    if (confirm("One or more dpiu's has not closed their month.Do you want to see the list ?")) {

                                        $("#DpiuList").jqGrid("GridUnload");
                                        loadDPIUNOTCLOSEDMONTH();
                                    }
                                    else {
                                        return false;
                                    }


                                }

                            }
                            else {

                                //unfinalized Voucher
                                if (monthlyClosingStatus[0] == "-333") {

                                    switch (monthlyClosingStatus[1]) {
                                        case "O": alert("Opening Balance entry is not finalized in " + monthlyClosingStatus[2] + "  " + monthlyClosingStatus[3] + " ")
                                            return false;
                                        case "R": alert("Receipt entry is not finalized in " + monthlyClosingStatus[2] + "  " + monthlyClosingStatus[3] + " ")
                                            return false;
                                        case "P": alert("Payment entry is not finalized in " + monthlyClosingStatus[2] + "  " + monthlyClosingStatus[3] + " ")
                                            return false;
                                        case "J": alert("Transfer Entry Order is not finalized in " + monthlyClosingStatus[2] + " " + monthlyClosingStatus[3] + " ")
                                            return false;
                                        case "A": alert("Authorization Request is not finalized in " + monthlyClosingStatus[2] + "  " + monthlyClosingStatus[3] + " ")
                                            return false;
                                    }
                                }

                            }

                            //if (data.result === undefined) {
                            //    $("#mainDiv").html(data);
                            //    return false;
                            //}


                          


                            return false;
                        }

                        else {

                            alert("Error While Closing Month");
                            return false;
                        }

                    }
                });

                // } 
            }

        }

    });

    //$("#TO_MONTH").change(function () {

    //     jQuery("#gridDiv").hide('slow');

    //});

    //$("#TO_YEAR").change(function () {

    //     jQuery("#gridDiv").hide('slow');

    //});

    //$("#FROM_MONTH").change(function () {

    //     jQuery("#gridDiv").hide('slow');

    //});

    //$("#FROM_YEAR").change(function () {

    //     jQuery("#gridDiv").hide('slow');
    //});




});

//function to check whether  the TO month and year  falls in same financial month of from month and year 
function CheckFinanacialYearForMonthAndYear() {

    var fromMonth = parseInt($('#FROM_MONTH').val());
    var fromYear = parseInt($('#FROM_YEAR').val());
    var toMonth = parseInt($('#TO_MONTH').val());
    var toYear = parseInt($("#TO_YEAR").val());

    //from year should be less than equal to to year
    if (fromYear > toYear) {
        return false;
    }

    //difference of years should be atmost 1
    if (toYear - fromYear == 1) {

        if (fromMonth >= 4) {

            if (fromYear != toYear && (toMonth <= 3 && toYear == fromYear + 1)) {
                return true;//same financial year
            } else
                if (fromYear == toYear && fromMonth > toMonth) {
                    return true;
                }

        } else if (toMonth < fromMonth && (fromYear == toYear)) {
            return false;
        }
    }
    else if (toYear == fromYear) {
        if (toMonth < fromMonth) {

            return false;
        }
        else {
            if (fromMonth >= 3 && toMonth >= 3) {
                return true;
            }
            else if (fromMonth <= 3 && toMonth <= 3) {
                return true;
            }
            else return false;
        }
    }
    else {
        return false;
    }


}


function GetClosedMonthAndYear() {
    blockPage();

    var OwnDPIU;
    if ($("#rdDPIU").is(":checked"))
    {
        OwnDPIU = "D";
    }else{
        OwnDPIU = "O";
    }

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,
        data: { DPIU_CODE: $("#ddlDPIU").val(), OWN_DPIU: OwnDPIU },
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');

            return false;

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);

                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });


}

//function to get the account  start month and year
function GetAccountStartMonthYear() {

    blockPage();
    var OwnDPIU;
    if ($("#rdDPIU").is(":checked")) {
        OwnDPIU = "D";
    } else {
        OwnDPIU = "O";
    }

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetAccountStartMonthandYear/",
        //async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        data: { DPIU_CODE: $("#ddlDPIU").val(), OWN_DPIU: OwnDPIU },

        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.accountStarted) {
                $("#lblAccMonth").text(data.month);
                $("#lblAccYear").text(data.year);

                $("#TrAccountStatus").show('Slow');
                $("#accountMonthYearTr").hide('Slow');
                return false;
            }
            else if (data.accountStarted == false) {
                $("#accountMonthYearTr").show('Slow');
                $("#TrAccountStatus").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Account Start month and year");
                return false;
            }

        }
    });


}



//function to populate the grid od authorization request
function loadDPIUNOTCLOSEDMONTH() {


    jQuery("#DpiuList").jqGrid({

        url: '/MonthlyClosing/GetDpiuNotClosedMonth/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: { 'fromMonths': $('#FROM_MONTH').val(), 'fromYear': $('#FROM_YEAR').val(), 'toMonth': $('#TO_MONTH').val(), 'toYear': $('#TO_YEAR').val() },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['PIU Name ', 'Last Month Closed'],
        colModel: [

            {
                name: 'Auth_Number',
                index: 'Auth_Number',
                width: 90,
                align: "center",
                frozen: true

            },
            {
                name: 'Auth_date',
                index: 'Auth_date',
                width: 80,
                align: "center",
                frozen: true,

            }
        ],
        pager: "#Piupager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {

            $("#DpiuList").jqGrid('setLabel', "rn", "Sr.</br> No");
            $("#DpiuList").parents('div.ui-jqgrid-bdiv').css("max-height", "400px");

        },
        sortname: 'Auth_Number',
        sortorder: "asc",
        caption: "DPIU Who has not closed the month"
    });




}



//function validatePIUChequeAcknowledgementStatus()
//{
//    //Added By Abhishek kamble 18-July-2014 To check Cheque Ack Status of All PIU's under SRRDA start

//    var status = true;//valid
//    if ($("#levelId").val() == 4) {//SRRDA Level
//        blockPage();

//        //S-Single M-Multiple
//        var MonthCloseSingleOrMultiple;
//        if ($("#rdSingle").is(":checked")) {
//            MonthCloseSingleOrMultiple = "S";
//        }
//        else if ($("#rdMultiple").is(":checked")) {
//            MonthCloseSingleOrMultiple = "M";
//        }

//        $.ajax({
//            url: "/MonthlyClosing/CheckAllPiuForChequeAck/",
//            type: "POST",
//            async: false,
//            data: { FromMonth: $("#FROM_MONTH").val(), FromYear: $("#FROM_YEAR").val(), ToMonth: $("#TO_MONTH").val(), ToYear: $("#TO_YEAR").val(), MonthCloseSingleOrMultiple: MonthCloseSingleOrMultiple },
//            error: function (xhr, status, error) {
//                unblockPage();
//                alert("An Error occured while getting PIU's Cheque Acknowledgement Details.");
//                return false;
//            },
//            success: function (responce) {//Show List of PIU
//                unblockPage();
//                if (responce.status === undefined) {                    
//                    //One or more dpiu's has not Acknowledge their cheques. Do you want to see the list ?
//                    if (confirm("Cheques Acknowledge is not done for One or more dpiu's. Do you want to see the list ?")) {
//                        $("#dvShowPIUChequeAckStatus").html(responce);
//                    }
//                    status = false;
//                }
//            }
//        });
//        //Added By Abhishek kamble 18-July-2014 To check Cheque Ack Status of All PIU's under SRRDA end
//    }   
//    return status;
//}
