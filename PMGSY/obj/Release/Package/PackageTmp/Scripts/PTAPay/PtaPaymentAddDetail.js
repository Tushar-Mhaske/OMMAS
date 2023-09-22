$.validator.unobtrusive.adapters.add('datecomparefieldvalidator', ['date'], function (options) {
    options.rules['datecomparefieldvalidator'] = options.params;
    options.messages['datecomparefieldvalidator'] = options.message;
});
jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, params) {



    var fromDate = $('#Invoice_Generate_DATE').val();
    //var toDate = $('#Current_Date').val();
    var dateToValidate = $('#IMS_Payment_DATE').val();



    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    // var tomonthfield = toDate.split("/")[1];
    // var todayfield = toDate.split("/")[0];
    //var toyearfield = toDate.split("/")[2];

    var dateToValidatemonthfield = dateToValidate.split("/")[1];
    var dateToValidatedayfield = dateToValidate.split("/")[0];
    var dateToValidateyearfield = dateToValidate.split("/")[2];


    //alert("m : " + dateToValidatemonthfield);
    //alert("d : " + dateToValidatedayfield);
    //alert("y : " + dateToValidateyearfield);

    var sDate = new Date(fromyearfield, frommonthfield - 1, fromdayfield);
    // var eDate = new Date(toyearfield, tomonthfield - 1, todayfield);

    var ptaPaymentDate = new Date(dateToValidateyearfield, dateToValidatemonthfield - 1, dateToValidatedayfield);

    //alert("test insStartDate" + ptaPaymentDate);
    //alert("start" + sDate);
    //alert("end" + eDate);


    if ((ptaPaymentDate < sDate)) {
        // alert("test ptaPaymentDate false");
        return false;
    }
    else {
        // alert("test ptaPaymentDate true");

        return true;
    }
});
$(document).ready(function () {
    //Load test result List
    ShowPtaPaymentList();
    //validation 
    //$.validator.unobstrusive.parse($("#frmPTAPayment"));
    $.validator.unobtrusive.parse($('#frmPTAPayment'));


    $('#IMS_Payment_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    //Save Details

    $("#btnSave").click(function () {


        if ($("#frmPTAPayment").valid()) {

            $.ajax({

                url: '/PtaPay/AddPTAPaymentDetails',
                type: 'POST',
                catche: false,
                data: $("#frmPTAPayment").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be  processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {

                    if (response.success === undefined) {
                        $("#dvPTAPaymentForm").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success) {
                        alert(response.message);
                        $("#divPtaPayementAdd").hide("slow");                      
                        $('#tbPTAPaymentList').trigger('reloadGrid');
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                        unblockPage();
                    }


                },
            });
        }

    });//end of save


    $("#btnUpdate").click(function () {

        if ($("#frmPTAPayment").valid()) {

            $.ajax({
                url: '/PtaPay/UpdatePTAPaymentDetails/',
                type: 'POST',
                catche: false,
                data: $("#frmPTAPayment").serialize(),
                beforeSend: function () {

                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {

                    if (response.success === undefined) {
                        $("#dvPTAPaymentForm").html(response);
                        unblockPage();
                    } else if (response.success) {

                        alert(response.message);

                        //alert($("#hidden_ims_pr_road_code").val());

                        //$("#dvPTAPaymentForm").load("/Proposal/PTAPaymentDetails/", $("#hidden_ims_pr_road_code").val());

                        //if ($("#dvError").is(":visible")) {
                        //    $("#divError").hide("slow");
                        //    $("#divError span:eq(1)").html('');
                        //}                       

                        // LoadPTAPaymentForm();
                        $("#divPtaPayementAdd").hide("slow");                       
                        $('#tbPTAPaymentList').trigger('reloadGrid');
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong> " + response.message);
                        unblockPage();
                    }


                }
            });//end
        }
    });

    $("#btnReset").click(function () {
        $("#divError").hide('slow');
        $("#divError span:eq(1)").html('');
        $('#radioPayment_TypeCheque').attr('checked', true);
        $('#lblPaymentTypeNumber').text("Cheque Number");

    });

    $("#btnCancel").click(function () {
        //LoadPTAPaymentForm();

        //$("#dvPTAPaymentForm").load("/Proposal/PTAPaymentDetails/");

        //if ($("#dvError").is(":visible")) {
        //    $("#divError").hide("slow");
        //    $("#divError span:eq(1)").html('');
        //}
        $("#divPtaPayementAdd").hide("slow");
        $('#divNote').hide('slow');
    });

    $('#radioPayment_TypeCheque').click(function () {
        $('#lblPaymentTypeNumber').text("Cheque Number");
    });
    $('#radioPayment_TypeNEFT').click(function () {
        $('#lblPaymentTypeNumber').text("NEFT Number");
    });

});