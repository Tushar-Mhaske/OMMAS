
/*Date Validation Lab Established Date must be greater than or equal to agreement  date and less than or equal to Current date.*/
$.validator.unobtrusive.adapters.add('labcommencedatevalidator', ['date'], function (options) {
    options.rules['labcommencedatevalidator'] = options.params;
    options.messages['labcommencedatevalidator'] = options.message;
});
jQuery.validator.addMethod("labcommencedatevalidator", function (value, element, params) {


    
    var fromDate = $('#hdAgreementDate').val();
    var toDate = $('#Current_Date').val();
    var dateToValidate = $('#AGREEMENT_EST_DATE').val();



    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var dateToValidatemonthfield = dateToValidate.split("/")[1];
    var dateToValidatedayfield = dateToValidate.split("/")[0];
    var dateToValidateyearfield = dateToValidate.split("/")[2];


    //alert("m : " + dateToValidatemonthfield);
    //alert("d : " + dateToValidatedayfield);
    //alert("y : " + dateToValidateyearfield);

    var sDate = new Date(fromyearfield, frommonthfield - 1, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield - 1, todayfield);

    var inspectionEndDate = new Date(dateToValidateyearfield, dateToValidatemonthfield - 1, dateToValidatedayfield);

    //alert("test insStartDate" + inspectionEndDate);
    //alert("start" + sDate);
    //alert("end" + eDate);


    if ((inspectionEndDate < sDate) || (inspectionEndDate > eDate)) {
        // alert("test inspectionEndDate false");
        return false;
    }
    else {
        // alert("test inspectionEndDate true");

        return true;
    }
});

/*End Date Vliadtion*/

$(document).ready(function () {



  
    $.validator.unobtrusive.parse($('#frmPropAddCost'));


    $('#AGREEMENT_EST_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        maxDate: new Date(),
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    //Save Details

    $("#btnSave").click(function () {      
        if ($("#frmPropAddCost").valid()) {

            $.ajax({

                url: '/QualityMonitoring/AddLabSaveDetails',
                type: 'POST',
                catche: false,
                data: $("#frmPropAddCost").serialize(),
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
                        $("#dvPropAddCostForm").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success) {
                        alert(response.message);
                        CloseLabPhoto();
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




    $("#btnReset").click(function () {
        $("#divError").hide('slow');
        $("#divError span:eq(1)").html('');
    });


});














