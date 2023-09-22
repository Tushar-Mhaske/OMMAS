var conM = "N";
var aggM = "N";
var supM = "N";
var mtxnM = "N";
var cashchqM = "N";


$(function () {
    $.validator.unobtrusive.adapters.add('isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });

    $.validator.addMethod("isdateafter", function (value, element, params) {
        //var fullDate = new Date();
        var fullDate = process($('#' + params.propertytested).val());
        var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + fullDate.getFullYear();
        return (params.allowequaldates) ? process(currentDate) >= process(value) : process(currentDate) > process(value);
    });

    $.validator.unobtrusive.adapters.add('isdatebeforeob', ['propertytested'], function (options) {
        options.rules['isdatebeforeob'] = options.params;
        options.messages['isdatebeforeob'] = options.message;
    });

    $.validator.addMethod("isdatebeforeob", function (value, element, params) {

        var obDate = new Date($('#' + params.propertytested).val());
        //var obDate = $('#' + params.propertytested).val();
        ////alert(new Date($('#' + params.propertytested).val()));
        //var twoDigitMonth = obDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        //var twoDigitDate = obDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        //var obDateToCompare = twoDigitDate + "/" + twoDigitMonth + "/" + obDate.getFullYear();
        //var obDateToCompare = twoDigitMonth + "/" + twoDigitDate + "/" + obDate.getFullYear();
        //var obDateToCompare = obDate.getDate() + "/" + twoDigitMonth + "/" + obDate.getFullYear();
        //alert(obDateToCompare);
        //alert(process(obDateToCompare) <= process(value));
        //alert(process($('#' + params.propertytested).val()));
        //alert(process(obDateToCompare));
        return (process($('#' + params.propertytested).val()) <= process(value));
    });

    $.validator.unobtrusive.adapters.add('ischeque', ['chequeepay'], function (options) {
        options.rules['ischeque'] = options.params;
        options.messages['ischeque'] = options.message;
    });

    $.validator.addMethod("ischeque", function (value, element, params) {
        var IsCheque = $('input:radio[name="' + params.chequeepay + '"]:checked').val();
        if (IsCheque == "Q") {
            if (value == null || value == "") {
                return false;
            }
        }
        return true;
    });

    $.validator.unobtrusive.adapters.add('ischequedategreater', ['chequeepay'], function (options) {
        options.rules['ischequedategreater'] = options.params;
        options.messages['ischequedategreater'] = options.message;
    });

    $.validator.addMethod("ischequedategreater", function (value, element, params) {
        var IsCheque = $('input:radio[name="' + params.chequeepay + '"]:checked').val();
        if (IsCheque == "Q") {
            var fullDate = new Date();
            var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
            var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
            var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + fullDate.getFullYear();

            //return process(currentDate) >= process(value);
            //Current Date taken from server side current date for validation previously is is Client side date. By Abhishek 4 Sep 2014
            return process($("#CURRENT_DATE").val()) >= process(value);
        }
        return true;
    });


    $.validator.unobtrusive.adapters.add('isvaliddate', ['month', 'year', 'chqepay'], function (options) {
        options.rules['isvaliddate'] = options.params;
        options.messages['isvaliddate'] = options.message;
    });

    $.validator.addMethod("isvaliddate", function (value, element, params) {
        var month = $('#' + params.month).val();
        var year = $('#' + params.year).val();
        if (params.chqepay != "") {
            var IsCheque = $('input:radio[name="' + params.chqepay + '"]:checked').val();
            if (IsCheque == "Q") {
                return ($('#' + params.month).val() == value.split('/')[1].replace(/^0+/, '')) && ($('#' + params.year).val() == value.split('/')[2])
            }
            else {
                return true;
            }
        }
        else {
            return ($('#' + params.month).val() == value.split('/')[1].replace(/^0+/, '')) && ($('#' + params.year).val() == value.split('/')[2])
        }

    }, function (params, element) {
        return "Date must be in " + $('#BILL_MONTH option[value=' + $('#BILL_MONTH').val() + ']').text() + " month and " + $('#BILL_YEAR').val() + " year";
    });
});

function process(date) {
    var parts = date.split(' ')[0].split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

$(document).ready(function () {

    //new change done by Vikram on 01-Jan-2014
    $("#BILL_NO").focus();
    //end of change

    $("#BILL_DATE").addClass("pmgsy-textbox");
    $("#CHQ_DATE").addClass("pmgsy-textbox");
   

    $.validator.unobtrusive.parse($('#frmAddEditReceipt'));

    //Added By Abhishek kamble 20-jan-2014 start    
    var currentDate = $("#CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
    //Added By Abhishek kamble 20-jan-2014 end


   // alert('in receipt master');

   if ($('#BILL_NO').is(':disabled')) {
    }
    else
    {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/R$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                alert(xhr.responseText);
                $.unblockUI();

            },
            success: function (data) {
                unblockPage();
                if (data != "") {
                    $.unblockUI();
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);

                }
            }
        });
    }

    $("#BILL_MONTH").change(function () {
        if ($(this).val() != "0") {
            month = $(this).val();
        }
        //new change done by Vikram on 31-Dec-2013
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());
        if ($('#BILL_NO').is(':disabled')) {
        }
        else {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: "POST",
                url: "/payment/GenerateVoucherNo/R$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
                async: false,
                // data: $("#authSigForm").serialize(),
                error: function (xhr, status, error) {
                    alert(xhr.responseText);
                    $.unblockUI();
                },
                success: function (data) {
                    unblockPage();
                    if (data != "") {
                        $.unblockUI();
                        $("#BILL_NO").val("");
                        $("#BILL_NO").val(data.strVoucherNumber);
                        $("#BILL_NO").attr('readonly', true);

                    }
                }
            });
        }

        //new change done by Abhishek kamble on 20-jan-2014 start
        if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {
            $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
            $("#CHQ_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));            
        } else {
            
            if ($("#BILL_DATE").val() != '') {
                var selectedDate = $("#BILL_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }

            if ($("#CHQ_DATE").val() != '') {
                var selectedDate = $("#CHQ_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 20-jan-2014 end
    });

    $("#BILL_YEAR").change(function () {
        if ($(this).val() != "0") {
            year = $(this).val();
        }

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/R$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                alert(xhr.responseText);
                $.unblockUI();

            },
            success: function (data) {
                unblockPage();
                if (data != "") {
                    $.unblockUI();
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);

                }
            }
        });

        //new change done by Vikram on 31-Dec-2013
        UpdateAccountSession($("#BILL_MONTH").val(), $("#BILL_YEAR").val());
        
        //new change done by Abhishek kamble on 20-jan-2014 start
        if ($("#BILL_MONTH").val() == 0 || $("#BILL_YEAR").val() == 0) {           
            $("#BILL_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
            $("#CHQ_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
        } else {
            if ($("#BILL_DATE").val() != '') {
                var selectedDate = $("#BILL_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#BILL_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }

            if ($("#CHQ_DATE").val() != '') {
                var selectedDate = $("#CHQ_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());
                $("#CHQ_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 20-jan-2014 end
    });
    
    if ($('#BILL_NO').is(':disabled')) {
    }
    else {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/R$" + $("#BILL_MONTH").val() + '$' + $("#BILL_YEAR").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                alert(xhr.responseText);
                $.unblockUI();
            },
            success: function (data) {
                unblockPage();
                if (data != "") {
                    $.unblockUI();
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);

                }
            }
        });
    }


    if ($("#BILL_DATE").val() == "") {
        $("#BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText:'Receipt Date',
            onClose: function () {
                $(this).focus().blur();
            }
            //}).datepicker('setDate', new Date());
            //}).datepicker('setDate', process($("#CURRENT_DATE").val()));
        }).datepicker('setDate', process(ModifiedCurrentDate));
    }
    else {
        $("#BILL_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText: 'Receipt Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });
    }

    if ($("#CHQ_DATE").val() == "") {
        $("#CHQ_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText: 'Cheque/Reference Date',
            onClose: function () {
                $(this).focus().blur();
            }
            //}).datepicker('setDate', new Date());
            // }).datepicker('setDate', process($("#CURRENT_DATE").val()));
        }).datepicker('setDate', process(ModifiedCurrentDate));
    }
    else {
        $("#CHQ_DATE").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            buttonText: 'Cheque/Reference Date',
            onClose: function () {
                $(this).focus().blur();
            }
        });
    }

    $("#BILL_DATE").on('change', function () {

        $("#CHQ_DATE").val($("#BILL_DATE").val());

    })

    $("#rdoCash").click(function () {
        $("#trCheque").hide('slow');
    });

    $("#rdoCheque").click(function () {
        $("#trCheque").show('slow');
    });

    $("#ddlTransMaster").change(function () {
        if ($(this).val() != "0" && $(this).val() != "") {
            $.ajax({
                url: "/Receipt/GetMasterDesignParams/" + $("#ddlTransMaster").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {

                    conM = "N";
                    aggM = "N";
                    supM = "N";
                    mtxnM = "N";
                    cashchqM = "N";

                   
                    if (data.CON_REQ == "Y") {
                        conM = "Y";
                    }
                    if (data.AGREEMENT_REQ == "Y") {
                        aggM = "Y";
                    }
                    if (data.SUP_REQ == "Y") {
                        supM = "Y";
                    }
                    if (data.MTXN_REQ == "Y") {
                        mtxnM = "Y";
                    }
                    if (data.CASH_CHQ.trim() == "C" || data.CASH_CHQ.trim() == "CQ") {
                        $("#tdCash").show('slow');
                    }
                    cashchqM = data.CASH_CHQ == "Y";
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            conM = "N";
            aggM = "N";
            supM = "N";
            mtxnM = "N";
            cashchqM = "N";
        }

        if ($("#BILL_NO").is('[readonly]') == false) {

            //commenetd by Koustubh Nakate on 18/10/2013 
            //$("input:radio:checked").prop('checked', false);
            //end commenetd by Koustubh Nakate on 18/10/2013 

            //$("#CHQ_NO").val("");
            //$("#CHQ_DATE").val("");
        }
        if ($.trim($(this).val().split('$')[1]) == 'C') {
            $("#rdoCash").attr('checked', 'checked');
            $("#rdoCash").trigger('click');
            $("#rdoCash").show('slow');
            $("#lblCash").show('slow');
            $("#lblCheque").hide('slow');
            $("#rdoCheque").hide('slow');
        }
        else if ($.trim($(this).val().split('$')[1]) == 'Q') {
            $("#rdoCheque").attr('checked', 'checked');
            $("#rdoCheque").trigger('click');
            $("#lblCheque").show('slow');
            $("#rdoCheque").show('slow');
            $("#rdoCash").hide('slow');
            $("#lblCash").hide('slow');
        }
        else {
            
          
            $("#lblCheque").show('slow');
            $("#rdoCheque").show('slow');
            $("#rdoCash").show('slow');
            $("#lblCash").show('slow');

            //added by Koustubh Nakate on 10/10/2013

            if ($("#rdoCheque").is(':checked')) {
               
            }
            else if ($("#rdoCash").is(':checked')) {
                $("#rdoCash").trigger('click');
            }

           

        }
    });

    $("#btnSaveReceipt").click(function (evt) {
        evt.preventDefault();

        if ($('#frmAddEditReceipt').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Receipt/AddReceiptMaster/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddEditReceipt").serialize(),
                success: function (data) {                   
                    $.unblockUI();

                    //Added by Abhishek kamble 1-jan-2014

                    IsMultiTranAllowed = data.IsMultiTranAllowed;
                    ReceiptGrossAmount = data.ReceiptGrossAmount;


                    //if (!data.success) {
                    //    if (data.message == "undefined" || data.message == null) {
                    //        //$("#loadReceiptMaster").html(data);
                    //        $("#divReceiptMasterError").show("slide");
                    //        $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    //    }
                    //    else {
                    //        $("#divReceiptMasterError").show("slide");
                    //        $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    //    }                  
                    //    return false;
                    //}
                    if (data.success === undefined) {
                        $("#loadReceiptMaster").html(data);
                        return false;
                    }
                    else if (data.success == false) {
                        $("#divReceiptMasterError").show("slide");
                        $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        //$("#loadReceiptMaster").html(data);                       
                        return false;
                    }

                    else {
                        $("#divReceiptMasterError").hide("slide");
                        $("#divReceiptMasterError span:eq(1)").html('');
                        masterBillID = data.message;
                        LoadMasterReceiptGrid(masterBillID);
                        $.ajax({
                            url: "/Receipt/ReceiptDetails/" + data.message,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#loadReceiptDetails").show();
                                $("#loadReceiptDetails").html(data);
                                $("#btnReset").trigger('click');
                                alert("Receipt Master added.");
                                $("#tblReceiptMaster").hide("slow");

                                //added by koustubh Nakate on 10/10/2013 to hide month and year bar

                                $('#tblViewDetails').hide();

                                LoadDetailsReceiptGrid(masterBillID);

                                return false;
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert("Here: " + xhr.responseText);
                            }
                        });
                        return false;
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert("err");
                    $.unblockUI();

                }
            });
        }

    });

    $("#btnEditReceipt").click(function (evt) {
        evt.preventDefault();
        var status = validateAddReceiptMaster($("#GROSS_AMOUNT").val());
        var masterBillId = $("#tblMasterReceiptList").getDataIDs()[0];
        // masterBillID = masterBillId;
        if ($('#frmAddEditReceipt').valid() && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#BILL_NO").removeAttr("disabled");

            $('#rdoCash').attr('disabled', false);
            $('#rdoCheque').attr('disabled', false);
            //$("#divReceiptMasterError span:eq(1)").html(' ');

            $.ajax({
                url: "/Receipt/EditReceiptMaster/" + $("#tblMasterReceiptList").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddEditReceipt").serialize(),
                success: function (data) {
                   
                    //Added by Abhishek kamble 1-jan-2014
                    $.unblockUI();

                    IsMultiTranAllowed = data.IsMultiTranAllowed;
                    ReceiptGrossAmount = data.ReceiptGrossAmount;

                    //alert("isMul:" + data.IsMultiTranAllowed);
                    //alert("gAmt:" + data.ReceiptGrossAmount);
                    //if (($("#AMOUNT").val() == "") && data.IsMultiTranAllowed == "N")
                    //{
                    //    $("#AMOUNT").val(data.ReceiptGrossAmount);
                    //}

                    
                   
                    if (!data.success) {
                       // $("#divReceiptMasterError span:eq(1)").html('');

                        if (data.message == "undefined" || data.message == null) {
                            $("#loadReceiptMaster").html(data);
                        }
                        else {
                            $("#divReceiptMasterError").show("slide");
                            $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }

                        $('#rdoCash').attr('disabled', true);
                        $('#rdoCheque').attr('disabled', true);
                       // $("#divReceiptMasterError span:eq(1)").html('');

                        return false;
                    }
                    else {
                       // $("#divReceiptMasterError").hide("slide");
                      //  $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        // alert($("#tblMasterReceiptList").getDataIDs()[0]);
                        LoadMasterReceiptGrid($("#tblMasterReceiptList").getDataIDs()[0]);
                        LoadDetailsReceiptGrid(masterBillId);
                        $("#loadReceiptDetails").load("/Receipt/ReceiptDetails/" + masterBillId);
                        $("#btnCancel").trigger('click');
                        alert("Receipt Master updated.");
                        $("#tblReceiptMaster").hide("slow");
                        //addded by Koustubh Nakate on 07/10/2013 to hide month and year bar
                        $("#tblViewDetails").hide("slow");
                        

                        return false;
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    });

    $("#btnCancel").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tblReceiptMaster").hide("slow");
        //addded by Koustubh Nakate on 07/10/2013 to hide month and year bar
        $("#tblViewDetails").hide("slow");
        $.unblockUI();
    });

    $("#btnReset").click(function (e) {
        
        $("#BILL_DATE").datepicker('setDate', new Date());
    });


    $("#ddlTransMaster").trigger('change');

    //  $("#CHQ_DATE").val($("#BILL_DATE").val());

    //  $("#rdoCheque").trigger('click');

    // $("#rdoCheque").prop('checked', true);

});

function validateAddReceiptMaster(amountToValidate) {
    $("#divReceiptMasterError").hide("slide");
    $("#divReceiptMasterError span:eq(1)").html('');

    //masterAmount = $("#tblMasterReceiptList").jqGrid('getCell', $("#tblMasterReceiptList").getDataIDs()[0], 'GrossAmount');
    var detailsAmount = 0;
    if (isDetailsGridLoaded) {
        detailsAmount = $("#tblDetailsReceiptList").jqGrid('getCol', 'Amount', false, "sum");
    }

    if (parseFloat(detailsAmount).toFixed(2) == 0) {
        $("#divReceiptMasterError").hide("slide");
        $("#divReceiptMasterError span:eq(1)").html('');
        return true;
    }
    else if (parseFloat(detailsAmount) > parseFloat(amountToValidate)) {
        $("#divReceiptMasterError").show("slide");
        $("#divReceiptMasterError span:eq(1)").html('<strong>Alert: </strong> Amount must not be less than ' + detailsAmount);
        return false;
    }
    else {
        $("#divReceiptMasterError").hide("slide");
        $("#divReceiptMasterError span:eq(1)").html('');
        return true;
    }
}


function ModifiedDate(day, month, year)
{  
    return day + "/" + month + "/" + year;
}