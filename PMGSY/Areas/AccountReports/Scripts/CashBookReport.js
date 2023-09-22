
//validate DPIU required at SRRDA && Mord level
jQuery.validator.addMethod("isdpiurequired", function (value, element) {
    if ($("#ddlDPIU option:selected").val() == 0) {
        return false;
    }
    else {
        return true;
    }
},"");

var pdfMessage = "";
$(document).ready(function () {

    $("#dvDetails").hide();

    //added by abhishek kamble 3-oct-2013 start
    if ($("#levelId").val() == 4) {
        $("#ddlSRRDA").val($("#AdminNdCode").val());
    }
    $(function () {
        if ($("#rdoSRRDA").is(":checked")) {
            $('#ddlSRRDA').trigger('change');
            $("#ddlDPIU").hide();
        }
    });

    $("#rdoSRRDA").click(function () {
        $("#ddlDPIU").hide("slow");
        $("#ddlDPIU option:first").attr("selected", "selected");
        //hide report category button
        $("#cat_icon").hide();
    });

    $("#rdoDPIU").click(function () {
        $("#ddlDPIU").show("slow");
        //hide report category button
        $("#cat_icon").hide();
    });

    $("#ddlDPIU").change(function () {
        $("#cat_icon").hide();
    });

    $("#ddlSRRDA").change(function () {
        var adminNdCode = $('#ddlSRRDA option:selected').val();
        $.ajax({
            url: '/Reports/PopulateDPIUForCashBook/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlDPIU').empty();
                $.each(data, function () {
                    $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });

    });

    //added by abhishek kamble 3-oct-2013 end

    $('#pay_ico').click(function () {
        $("#loadSingleCashBookReport").hide();
        $("#loadPaymentSideBookReport").show();
        $("#loadReceiptSideCashBookReport").hide();
        if ($("#loadPaymentSideBookReport").html() == "") {
            $("#CashbookType").val("P");
            ViewPaymentCashBookReportDetails();
        }
    });

    $('#rec_ico').click(function () {
        $("#loadSingleCashBookReport").hide();
        $("#loadPaymentSideBookReport").hide();
        $("#loadReceiptSideCashBookReport").show();
        if ($("#loadReceiptSideCashBookReport").html() == "") {
            $("#CashbookType").val("R");
            ViewReceiptCashBookReportDetails();
        }
    });

    $('#singleCB_ico').click(function () {
        //Hide report other div
        $("#loadSingleCashBookReport").show();
        $("#loadPaymentSideBookReport").hide();
        $("#loadReceiptSideCashBookReport").hide();

        //if div is empty then only load div
        if ($("#loadSingleCashBookReport").html() == "") {
            $("#CashbookType").val("S");
            ViewSingleCashBookReportDetails();
        }
    });

    $("#btnView").click(function () {

        //Hide div
        $("#loadSingleCashBookReport").show();
        $("#loadPaymentSideBookReport").hide();
        $("#loadReceiptSideCashBookReport").hide();

        //Clear Div
        $("#loadSingleCashBookReport").html('');
        $("#loadReceiptSideCashBookReport").html('');
        $("#loadPaymentSideBookReport").html('');

        $("#CashbookType").val("S");
        ViewSingleCashBookReportDetails();
    });

    $("#Month").change(function () {
        //hide report cat button
        $("#cat_icon").hide();
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });

    $("#Year").change(function () {
        //hide report cat button
        $("#cat_icon").hide();
        UpdateAccountSession($("#Month").val(), $("#Year").val());
    });
});
function UpdateAccountSession(month, year) {
    $.ajax({
        url: "/Reports/UpdateAccountSession",
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            return false;
        },
        error: function () { }
    });
    return false;
}

function ViewSingleCashBookReportDetails() {
    //Validation start
    //if ($("#rdoDPIU").is(":checked")) {
    //    if ($("#ddlDPIU").val() == "0") {
    //        alert('Please Select DPIU');
    //        return false;
    //    }
    //}

    //DPIU Validation for Mord and SRRDA login to select DPIU
    if ($("#rdoDPIU").is(":checked") && ($("#levelId").val() != 5)) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").rules('add', {
            isdpiurequired: true,
            messages: {
                isdpiurequired: 'Please select DPIU'
            }
        });
    }

    if ($("#frmCashBook").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //blockPage();
        var DPIU = $("#ddlDPIU option:selected").val();

        if ($("#levelId").val() == 4) {
            $("#ddlSRRDA").attr("disabled", false);
        }

        $.ajax({
            url: "/AccountReports/Account/CashBookReport/",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmCashBook").serialize(),
            success: function (data) {
                //load Result Report                
                $("#loadSingleCashBookReport").html(data);
                $("#cat_icon").show();

                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
            },
            complete: function (xhr, status) {
                $.unblockUI();
            },
            error: function (xhr, status, error) {
                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
                $.unblockUI();
            }
        });
    }
}


function ViewReceiptCashBookReportDetails() {
    //Validation start
    //DPIU Validation for Mord and SRRDA login to select DPIU
    if ($("#rdoDPIU").is(":checked") && ($("#levelId").val() != 5)) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").rules('add', {
            isdpiurequired: true,
            messages: {
                isdpiurequired: 'Please select DPIU'
            }
        });
    }

    if ($("#frmCashBook").valid()) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //blockPage();
        var DPIU = $("#ddlDPIU option:selected").val();

        if ($("#levelId").val() == 4) {
            $("#ddlSRRDA").attr("disabled", false);
        }

        $.ajax({
            url: "/AccountReports/Account/CashBookReport/",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmCashBook").serialize(),
            success: function (data) {

                //load Result Report                
                $("#loadReceiptSideCashBookReport").html(data);

                $("#cat_icon").show();

                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
            },
            complete: function (xhr, status) {
                $.unblockUI();
            },
            error: function (xhr, status, error) {
                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
                $.unblockUI();
            }
        });
    }

}



function ViewPaymentCashBookReportDetails() {
    //Validation start    
    //DPIU Validation for Mord and SRRDA login to select DPIU
    if ($("#rdoDPIU").is(":checked") && ($("#levelId").val() != 5)) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").rules('add', {
            isdpiurequired: true,
            messages: {
                isdpiurequired: 'Please select DPIU'
            }
        });
    }

    if ($("#frmCashBook").valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        var DPIU = $("#ddlDPIU option:selected").val();

        if ($("#levelId").val() == 4) {
            $("#ddlSRRDA").attr("disabled", false);
        }

        $.ajax({
            url: "/AccountReports/Account/CashBookReport/",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmCashBook").serialize(),
            success: function (data) {
                //load Result Report                
                $("#loadPaymentSideBookReport").html(data);

                $("#cat_icon").show();

                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
            },
            complete: function (xhr, status) {
                $.unblockUI();
            },
            error: function (xhr, status, error) {
                if ($("#levelId").val() == 4) {
                    $("#ddlSRRDA").attr("disabled", true);
                }
                $.unblockUI();
            }
        });
    }
}












































/*

$('#pay_ico').click(function () {

        if ($('#receiptside').is(':visible')) {
           
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 500, function () { $('#receiptside').fadeOut(); });
            $('#paymentside').css('width', '98%').css('float', 'none');

            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 1000, function () { $('#paymentside').fadeIn(); });
           
        }
        else if ($('#paymentside').css('float')=='rigth')
        {
            $('#paymentside').css('width', '98%').css('float', 'none');
            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 1000, function () { $('#paymentside').fadeIn(); });
        }
    });

    $('#rec_ico').click(function () {
        if ($('#paymentside').is(':visible')) {
            $('#paymentside').toggle('slide', {
                direction: 'right'
            }, 500, function () { $('#paymentside').fadeOut(); });

            $('#receiptside').css('width', '98%').css('float', 'none');
           
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 1000, function () { $('#receiptside').fadeIn(); });
        }
        else if ($('#receiptside').css('float') == 'left')
        {
            $('#receiptside').css('width', '98%').css('float', 'none');
            $('#receiptside').toggle('slide', {
                direction: 'left'
            }, 1000, function () { $('#receiptside').fadeIn(); });
        }
    });


    $('#singleCB_ico').click(function () {
        $('#receiptside').css('width', '50%').css('float', 'left');
        $('#paymentside').css('width', '50%').css('float', 'right');
       
        $('#receiptside').toggle('slide', {
            direction: 'left'
        }, 1000, function () { $('#receiptside').fadeIn(); });
        $('#paymentside').toggle('slide', {
            direction: 'right'
        }, 1000, function () { $('#paymentside').fadeIn(); });
    });

    $("#btnView").click(function () {
        $.ajax({
            url: "/Reports/CashBook/",
            type: "POST",
            async: false,
            cache: false,
            data: $("#frmCashBook").serialize(),
            success: function (data) {
                $("#mainDiv").html(data);
                $("#receiptCB").load("/Reports/ReceiptCashBook/" + $("#Month").val() + "/" + $("#Year").val(), function () {
                   
                });
                $("#paymentCB").load("/Reports/PaymentCashBook/" + $("#Month").val() + "/" + $("#Year").val(), function () {                   
                   
                });
                $('#singleCB_ico').trigger('click');
            }
        });       
    });


*/