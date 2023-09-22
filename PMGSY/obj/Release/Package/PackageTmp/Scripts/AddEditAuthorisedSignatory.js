$(function () {
                $.validator.unobtrusive.adapters.add(
                'isdateafter', ['propertytested', 'allowequaldates'], function (options) {
                    options.rules['isdateafter'] = options.params;
                    options.messages['isdateafter'] = options.message;
                });

                $.validator.addMethod("isdateafter", function (value, element, params)
                {
                var parts = element.name.split(".");
                var prefix = "";
                if (parts.length > 1)
                    prefix = parts[0] + ".";
                var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();
                    
                if (!value || !startdatevalue)
                    return true;
                return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
                (Date.parse(startdatevalue) < Date.parse(value));
            });
});


$(function () {
    $.validator.unobtrusive.adapters.add(
    'dategreaterthanvalidation', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['dategreaterthanvalidation'] = options.params;
        options.messages['dategreaterthanvalidation'] = options.message;
    });

    $.validator.addMethod("dategreaterthanvalidation", function (value, element, params) {
        //old Validation 
        //var parts = element.name.split(".");
        //var prefix = "";
        //if (parts.length > 1)
        //    prefix = parts[0] + ".";
        //var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();

        //if (!value || !startdatevalue)
        //    return true;
        //return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
        //   (Date.parse(startdatevalue) > Date.parse(value));
        

        //added by abhishek kamble
        var fromDate = $('#START_DATE').val();
        var toDate = $('#END_DATE').val();

        var frommonthfield = fromDate.split("/")[1];
        var fromdayfield = fromDate.split("/")[0];
        var fromyearfield = fromDate.split("/")[2];

        var tomonthfield = toDate.split("/")[1];
        var todayfield = toDate.split("/")[0];
        var toyearfield = toDate.split("/")[2];

        var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
        var eDate = new Date(toyearfield, tomonthfield, todayfield);

        sDate.setMonth(sDate.getMonth() - 1);
        eDate.setMonth(eDate.getMonth() - 1);

        if (sDate > eDate) {
            return false;
        }
        else {
            return true;
        }
    });

});

$(function () {
    $.validator.addMethod("isdateafter", function (value, element, params) {
        //old Validation 
        //var parts = element.name.split(".");
        //var prefix = "";
        //if (parts.length > 1)
        //    prefix = parts[0] + ".";
        //var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();

        //if (!value || !startdatevalue)
        //    return true;
        //return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
        //   (Date.parse(startdatevalue) > Date.parse(value));
 
        //added by ashish markande
        var fromDate = $('#START_DATE').val();
        var currDate = $("#CURRENT_DATE").val();       

        var frommonthfield = fromDate.split("/")[1];
        var fromdayfield = fromDate.split("/")[0];
        var fromyearfield = fromDate.split("/")[2];

        var curmonthfield = currDate.split("/")[1];
        var curdayfield = currDate.split("/")[0];
        var curyearfield = currDate.split("/")[2];

        var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
        var eDate = new Date(curyearfield, curmonthfield, curdayfield);

        sDate.setMonth(sDate.getMonth() - 1);
        eDate.setMonth(eDate.getMonth() - 1);

        if (sDate > eDate) {
            return false;
        }
        else {
            return true;
        }
    });
});


$(function () {
    $.validator.addMethod("isdateafter", function (value, element, params) {
        //old Validation 
        //var parts = element.name.split(".");
        //var prefix = "";
        //if (parts.length > 1)
        //    prefix = parts[0] + ".";
        //var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();

        //if (!value || !startdatevalue)
        //    return true;
        //return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
        //   (Date.parse(startdatevalue) > Date.parse(value));


        //added by ashish markande

      
        var endDate = $('#END_DATE').val();

        var currDate = $("#CURRENT_DATE").val();

        if (endDate != undefined) { //changes by Koustubh Nakate on 16/10/2013  

            var frommonthfield = endDate.split("/")[1];
            var fromdayfield = endDate.split("/")[0];
            var fromyearfield = endDate.split("/")[2];

            var curmonthfield = currDate.split("/")[1];
            var curdayfield = currDate.split("/")[0];
            var curyearfield = currDate.split("/")[2];

            var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
            var eDate = new Date(curyearfield, curmonthfield, curdayfield);

            sDate.setMonth(sDate.getMonth() - 1);
            eDate.setMonth(eDate.getMonth() - 1);

            if (sDate > eDate) {
                return false;
            }
            else {
                return true;
            }
        }
        return true;
    });
});



jQuery.validator.unobtrusive.adapters.addBool("isdateafter");



//client validation for cheque series 
jQuery.validator.addMethod("DesignationRequired", function (value, element) {

    if (value == "")
    {
        return false;
    }
    else if ((parseFloat(value) == 0))
    {
        return false;
    }
    else {
        return true;
    }

}, "");

//added by PP
var authSignatory = {};

$(document).ready(function () {
    //added by PP 
    authSignatory.fname = $("#ADMIN_FNAME").val().trim();
    authSignatory.mname = $("#ADMIN_MNAME").val().trim();
    authSignatory.lname = $("#ADMIN_LNAME").val().trim();
   // authSignatory.mobile = $("#ADMIN_MOBILE").val().trim();
    authSignatory.email = $("#ADMIN_EMAIL").val().trim();
    //authSignatory.aadhar = $("#ADMIN_AADHAR_NO").val().trim();
    //authSignatory.mobile = $("#ADMIN_PAN_NO").val().trim() ;

    //Added 6May2015

    $("#rdoAadhaarNo").click(function () {
        $("#spnAadharNumber").show('slow');
        $("#spnPanNumber").hide('slow');
    });

    $("#rdoPanNo").click(function () {
        $("#spnAadharNumber").hide('slow');
        $("#spnPanNumber").show('slow');
    });

    $("#ADMIN_FNAME").focus();

    $("#ADMIN_NO_DESIGNATION").css("width", "80%");
  
    $("#ADMIN_NO_DESIGNATION").attr("disabled", "disabled");

    if (operation == "V") {

        $("#mainDiv :input").prop("readonly", 'readonly');
        $("#mainDiv  select").prop("disabled", true);
        $("#btnUpdate").prop("disabled", false);
        $("#btnCancel").prop("disabled", false);
        $("#START_DATE").removeClass("ui-datepicker-trigger");
        $("#END_DATE").removeClass("ui-datepicker-trigger");
    }
    else {

        $("#START_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            onClose: function () {
                $(this).focus().blur();
            }

        });

        $("#END_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate:new Date(),
            onClose: function () {
                $(this).focus().blur();
            }

        });

    }
   

    //$("#END_DATE").val($("#EndDate").val());
    //$("#START_DATE").val($("#startDate").val());


    $("#lblBack").click(function () {
        blockPage();
        $("#mainDiv").load("/AuthorizedSignatory/GetAuthorizedSignatoryDetails/", function () {
            unblockPage();
        });
      

    });

    $("#btnCancel").click(function () {

        blockPage();
        $("#mainDiv").load("/AuthorizedSignatory/GetAuthorizedSignatoryDetails/", function () {
            unblockPage();
        });


    });

    

    //add button click event
    $("#btnSubmit").click(function () {

       // alert(validateDate($("#START_DATE").val()));

        $('#ADMIN_NO_DESIGNATION').rules('add', {
            DesignationRequired: true,
            messages: {
                DesignationRequired: 'Designation Required'
            }
        });

        if ($("#authSigForm").valid()) {

            AddUpdateAuthSignatory();

            //$.ajax({
            //    url: "/ChequeBook/CheckBankDetailsExist/",
            //    type: "GET",
            //    //dataType: "html",
            //    async: false,
            //    cache: false,
            //    success: function (data) {
            //        var isBankDetailsExist = data.exist;

            //        if (isBankDetailsExist == true) {

            //            AddUpdateAuthSignatory();
            //        }
            //        else {
            //            alert('Bank details does not exist, so You can not add Authorized Signatory details.');
            //        }
            //    },
            //    error: function (xhr, ajaxOptions, thrownError) {
            //        alert(xhr.responseText);
            //    }

            //});

          
        }

    });


    //Update Button click event
    $("#btnUpdate").click(function () {

        $('#ADMIN_NO_DESIGNATION').rules('add', {
            DesignationRequired: true,
            messages: {
                DesignationRequired: 'Designation Required'
            }
        });

        if ($("#authSigForm").valid()) {

            AddUpdateAuthSignatory();

            //$.ajax({
            //    url: "/ChequeBook/CheckBankDetailsExist/",
            //    type: "GET",
            //    //dataType: "html",
            //    async: false,
            //    cache: false,
            //    success: function (data) {
            //        var isBankDetailsExist = data.exist;

            //        if (isBankDetailsExist == true) {

            //            AddUpdateAuthSignatory();
            //        }
            //        else {
            //            alert('Bank details does not exist, so You can not update Authorized Signatory details.');
            //        }
            //    },
            //    error: function (xhr, ajaxOptions, thrownError) {
            //        alert(xhr.responseText);
            //    }

            //});

            
        }

    });


});//document.ready ends

//function to add or edit the authorized signatory details
//here $("#encryptedNdCode").val() contains the encrypted values of adminndcode,stateCode,districtCode,districtCode,opeartion(A/E),officerCode(for update) 

function ValidateAuthSignatory() {

    var fname = $("#ADMIN_FNAME").val().trim();
    var mname = $("#ADMIN_MNAME").val().trim();
    var lname = $("#ADMIN_LNAME").val().trim();
    // var mobile = $("#ADMIN_MOBILE").val().trim();
    var email = $("#ADMIN_EMAIL").val().trim();

    if (authSignatory.fname == fname && authSignatory.mname == mname && authSignatory.lname == lname && authSignatory.email == email) { //*&& authSignatory.mobile == mobile  */
        return true;
    } else {
        return false;
    }
}


function AddUpdateAuthSignatory()
{
    if ($("#btnUpdate").is(":visible")) {
        if (!ValidateAuthSignatory()) {
            if (!confirm('You will again need to sign "Xml for PFMS" at epay login as Authorized Signatory details are updated.Are you sure to continue?')) {
                return false;
            }
            else {
                $("#IsDscDetailChanged").val("Y");
            }
        }

    }

    $("#ADMIN_NO_DESIGNATION").removeAttr("disabled");
    blockPage();
    $.ajax({
        type: "POST",
        url: "/AuthorizedSignatory/AddEditAuthSignatory/" + $("#encryptedNdCode").val(),
        //async: false,
        data: $("#authSigForm").serialize(),
        error: function (xhr, status, error) {
            unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data.Success==true) {
                //$('#errorSpan').text("asdsdadsdasddsdsdadadasd");

                if (data.Operation == "A")
                {
                    alert("Authorized Signatory Details Added successfully.")
                }
                else
                {
                    alert("Authorized Signatory Details Updated successfully.")
                }

                $("#mainDiv").load("/AuthorizedSignatory/GetAuthorizedSignatoryDetails/", function () {
                });
            }
            else if (data.Success == false) {

                $('#divError').show('slow');
                $('#errorSpan').html(data.message);
            }
            else {
                unblockPage();

                $("#mainDiv").html(data);
                $.validator.unobtrusive.parse($("#mainDiv"));

            }

        }
    });
}