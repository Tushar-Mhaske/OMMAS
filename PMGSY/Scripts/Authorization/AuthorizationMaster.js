
var MasterTriggerWhenError = false;

function clearValidation(formElement) {

    
    if (MasterTriggerWhenError == false) {

        var id = $(formElement).attr('id');

        //Removes validation from input-fields
        $("#" + id + " .input-validation-error").addClass('input-validation-valid');
        $("#" + id + " .input-validation-error").removeClass('input-validation-error');
        //Removes validation message after input-fields
        $("#" + id + " .field-validation-error").addClass('field-validation-valid');
        $("#" + id + " .field-validation-error").removeClass('field-validation-error');
        //Removes validation summary 
        $("#" + id + " .validation-summary-errors").addClass('validation-summary-valid');
        $("#" + id + " .validation-summary-errors").removeClass('validation-summary-errors');
    }
  }

$(function () {
    $.validator.unobtrusive.adapters.add('isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });

    $.validator.addMethod("isdateafter", function (value, element, params) {
        var fullDate = new Date();
        var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + fullDate.getFullYear();
        return (params.allowequaldates) ? process(currentDate) >= process(value) : process(currentDate) > process(value);
    });

    $.validator.unobtrusive.adapters.add('isvaliddate', ['month', 'year', 'chqepay'], function (options) {
        options.rules['isvaliddate'] = options.params;
        //options.messages['isvaliddate'] = options.message;
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
$(function () {
    $.validator.unobtrusive.adapters.add(
    'isdateafter', ['propertytested', 'allowequaldates'], function (options) {
        options.rules['isdateafter'] = options.params;
        options.messages['isdateafter'] = options.message;
    });

    $.validator.addMethod("isdateafter", function (value, element, params) {
        var parts = element.name.split(".");
        var prefix = "";
        if (parts.length > 1)
            prefix = parts[0] + ".";
        var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();

        if (!value || !startdatevalue)
            return true;
        return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
            Date.parse(startdatevalue) < Date.parse(value);
    });

});

//new change done by Vikram on 27-09-2013

$.validator.addMethod("comparecashamount", function (value, element, params) {

    if (parseFloat($("#CHEQUE_AMOUNT").val()) >= parseFloat($("#CASH_AMOUNT").val())) {
        return true;
    }
    else {
        return false;
    }
});
jQuery.validator.unobtrusive.adapters.addBool("comparecashamount");

//end of change

$(document).ready(function () {

    //Added By Abhishek kamble 20-jan-2014 start
    var currentDate = $("#Hidden_CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());
    //Added By Abhishek kamble 20-jan-2014 end

    GetClosedMonthAndYear();
    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD')) 
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    });
    
    $.validator.unobtrusive.parse($("#MasterDataEntryForm"));
    
    
    //new change done by Vikram on 01-Jan-2014
    $("#CHEQUE_AMOUNT").focus();
    //end of change

    /*
    $("#input[type=text]").on("keypress", function () {
        if(e.keyCode==13 || event.keyCode === 8 )
        {
            return false;

        }

    });
      */
    //for add operation only get the lattest authorization number
    if (opeartion == "A") {

        // get the authorization number
        $.ajax({
            type: "POST",
            url: "/Authorization/GetAuthorizationNumber/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(),
            async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                return false;
            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#AUTH_NO').val(data).attr("readonly", "readonly");

            }
        });
    }

    //Modified By Abhishek kamble 21-jan-2014 start

    if ($("#AUTH_DATE").val() == "") {
        $("#AUTH_DATE").datepicker({
            showOn: 'button',
            buttonImage: '/Content/images/calendar_2.png',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            onClose: function () {
                $(this).focus().blur();
            }
            //});
        }).datepicker('setDate', process(ModifiedCurrentDate));
    } else {
        $("#AUTH_DATE").datepicker({
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
    }
    //Modified By Abhishek kamble 21-jan-2014 end

    // event to hide the master form details
    $("#btnCancel").click(function () {

        $("#masterPaymentForm").toggle('slow');

        $("#MasterDataEntryDiv").toggle('slow');

        $(this).find('span:first').toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
       

    });;

    $("#lblBack").click(function () {
        //blockPage();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#mainDiv").load("/Authorization/GetAuthorizationRequest/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(), function () {
            // unblockPage();
            $.unblockUI();

        });
        //$.unblockUI();

    });


    $("#AUTH_MONTH").change(function () {
        
        //new change done by Abhishek kamble on 21-jan-2014 start
        if ($("#AUTH_MONTH").val() == 0 || $("#AUTH_YEAR").val() == 0) {
            $("#AUTH_DATE").datepicker('setDate', process($("#Hidden_CURRENT_DATE").val()));
        } else {
            if ($("#AUTH_DATE").val() != '') {
                var selectedDate = $("#AUTH_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());
                $("#AUTH_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());
                $("#AUTH_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end

        //add the changed value of Year to the session
        //new change done by Vikram on 01-Jan-2014
        UpdateAccountSession($("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());

        if ($("#AUTH_MONTH").val() != 0  && $("#AUTH_YEAR").val() != 0) {
            $.ajax({
                type: "POST",
                url: "/Authorization/GetAuthorizationNumber/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#AUTH_NO').val(data).attr("readonly", "readonly");
                   
                }
            });
        } else {

            // alert("Please  select month and year");
            return false;
        }

    });


    $("#AUTH_YEAR").change(function () {

        //new change done by Abhishek kamble on 21-jan-2014 start
        if ($("#AUTH_MONTH").val() == 0 || $("#AUTH_YEAR").val() == 0) {
            $("#AUTH_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));
        } else {
            if ($("#AUTH_DATE").val() != '') {
                var selectedDate = $("#AUTH_DATE").val().split('/');
                var day = selectedDate[0];
                ModifiedCurrentDate = ModifiedDate(day, $("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());
                $("#AUTH_DATE").datepicker('setDate', process(ModifiedCurrentDate));

            } else {
                ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());
                $("#AUTH_DATE").datepicker('setDate', process(ModifiedCurrentDate));
            }
        }
        //new change done by Abhishek kamble on 21-jan-2014 end

        //add the changed value of Year to the session
        //new change done by Vikram on 01-Jan-2014
        UpdateAccountSession($("#AUTH_MONTH").val(), $("#AUTH_YEAR").val());

        if ($("#AUTH_YEAR").val() != 0  && $("#AUTH_YEAR").val() != 0) {

            $.ajax({
                type: "POST",
                url: "/Authorization/GetAuthorizationNumber/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    $('#divError').show('slow');
                    $('#errorSpan').text(xhr.responseText);
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                   
                    $('#AUTH_NO').val(data).attr("readonly", "readonly");
                   
                }
            });
        } else {

            //alert("Please  select month and year");
            return false;
        }
    });


    //get the contractor name details
    $("#MAST_CON_ID_C").change(function () {

        if ($("#MAST_CON_ID_C").val() == '0' || $("#MAST_CON_ID_C").val() == "") {
           
            $("#PAYEE_NAME").val("");
            

        } else {
            setContractorSupplierName($("#MAST_CON_ID_C").val(), "C", true);
        }
    });

    //cancel button click event for master data entry    
    $("#btnReset").click(function (e) {
               
        $("#TXN_ID").removeAttr("disabled");
        $("#AUTH_NO").removeAttr("disabled");
        $("#btnSubmit").show();
        $("#btnUpdate").hide();
        $(':input', '#MasterDataEntryForm').not("#AUTH_NO").not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
        $("#AUTH_DATE").val($("#Hidden_CURRENT_DATE").val());


        //added by abhishek kamble 28-oct-2013

        $('#CASH_AMOUNT').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');

        $('#AUTH_DATE').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#CHEQUE_AMOUNT').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#PAYEE_NAME').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#MAST_CON_ID_C').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#AUTH_MONTH').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#AUTH_YEAR').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        $('#TXN_ID').removeClass('pmgsy-textbox input-validation-error').addClass('pmgsy-textbox valid');
        
        $('#spnErrCashAmount').html('');
        $('#spnErrAuthDate').html('');
        $('#spnErrChequeAmount').html('');
        $('#spnErrPayeeName').html('');
        $('#spnErrConName').html('');        
        $('#spnErrMonth').html('');
        $('#spnErrYear').html('');
        $('#spnErrTransactionType').html('');
    })


    //event to show new form to add master details
    $("#AddNewMasterDetails").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        opeartion = "A";

        $('#btnSubmit').show();
        $('#btnReset').show();
        $('#btnUpdate').hide();
        $('#btnCancel').hide();

        $("#MasterGrid").hide('slow');

        $("#TXN_ID").removeAttr('disabled');

        $("#AUTH_NO").removeAttr('disabled');

        $("#MasterDataEntryForm").show('slow');

        $("#tblTransaction").show('slow');

        $("#MasterDataEntryDiv").show('slow');

        $("#TransactionForm").hide('slow');

        $("#trnsShowtable").hide('slow');

        $(':input', '#MasterDataEntryForm').not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

        $("#AUTH_DATE").val($("#Hidden_CURRENT_DATE").val());

        $("#AuthDetailsEntryDiv").html('');

        $("#trShowHideLinkTable").hide();

        $('#AuthBalance').hide();

        $('#DetailsGrid').hide();

        $("#AUTH_DATE").datepicker("enable").removeAttr("readonly");
        $("#MasterDataEntryForm :input ").removeAttr("readonly");
        $("#MasterDataEntryForm  select").prop("disabled", false);
        $("#MasterDataEntryForm :radio").prop("disabled", false);


        // get the authorization number
        $.ajax({
            type: "POST",
            url: "/Authorization/GetAuthorizationNumber/" + $("#AUTH_MONTH").val() + "$" + $("#AUTH_YEAR").val(),
            async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $.unblockUI();

                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);
                return false;
            },
            success: function (data) {
                $.unblockUI();

                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#AUTH_NO').val(data).attr("readonly", "readonly");

            }
        });
        
     //   $.unblockUI();

        
    });


    //get the page design parameters based on the transaction type selected
    $("#TXN_ID").change(function () {

      

        if ($("#TXN_ID").val() == 0)
        {
            $('.ContracorTr').hide();
            return false;
        }
        blockPage();

        $.ajax({
            type: "POST",
            url: "/payment/GetMasterDesignParams/" + $("#TXN_ID").val(),
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");

                clearValidation($("#MasterDataEntryForm"));


                if (data != "") {

                    if (data.BankAuthRequired == "Y") {

                    }
                   if (data.DedRequired == "N") {
                      
                        $("#CASH_AMOUNT").val(0);
                        $("#CASH_AMOUNT").attr("readonly", "readonly").val(0);
                        clearValidation($("#masterPaymentForm"));
                    }
                    if (data.DedRequired == "Y" || data.DedRequired == "B") {
                      
                        $("#CASH_AMOUNT").removeAttr("readonly");

                    }
                                                          
                    if (data.ContractorRequired == "Y") {

                      

                       $('.ContracorTr').show('slow');

                        $('#MAST_CON_ID_C').rules('add', {
                            required: true,
                            messages: {
                                required: 'Company Name (Contractor) is Required'
                            }
                        });

                        $('#PAYEE_NAME').rules('add', {
                            required: true,
                            messages: {
                                required: 'Payee Name(Contractor) is Required'
                            }
                        });

                        

                    }
                    else
                    {
                        $('.ContracorTr').hide('slow');


                    }
                 
                   
                }
                else {
                    alert("design parameters not entered for this head... !!!");

                }

            }
        });

    });


    //function add master details
    $("#btnSubmit").click(function (e) {

        e.preventDefault();

        if ($("#MasterDataEntryForm").valid()) {

            $("#TXN_ID").removeAttr('disabled');

            //blockPage();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


            $.ajax({
                type: "POST",
                url: "/Authorization/PostAddMasterAuthorization",
                async: false,
                data: $("#MasterDataEntryForm").serialize(),
                error: function (xhr, status, error) {
                    //unblockPage();
                    $.unblockUI();

                    $('#errorSpan').text(xhr.responseText);
                    $('#divError').show('slow');
                    $("#errorSpan").show('slow');
                    return false;
                },
                success: function (data) {
                    //unblockPage();
                    $.unblockUI();

                    $('#divError').hide('slow');
                    $('#errorSpan').html("");
                    $('#errorSpan').hide();

                   

                    if (data.Success === undefined)
                    {
                                             
                        unblockPage();
                        $("#masterListGrid").hide();
                        $("#AuthMasterEntryDiv").html(data);
                        MasterTriggerWhenError = true;
                        $.validator.unobtrusive.parse($("#MasterDataEntryForm"));
                        $('#TXN_ID').trigger('change');
                        $("#btnSubmit").show();

                    }

                   else if (data.Success) {
                       
                        alert("Authorization Details Added successfully.");

                        blockPage();
                                              
                        $("#MasterGrid").show('slow');
                       

                        $("#AuthorizationList").jqGrid().setGridParam
                              ({ url: '/Authorization/ListAuthorizationRequestForDataEntry/' + data.Auth_Id }).trigger("reloadGrid");

                        // get the transaction form 
                        $('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + data.Auth_Id, function () {
                                                     
                            $("#trShowHideLinkTable").show();

                            $('#AuthBalance').show();

                            //get the amount table updates
                            GetAuthorizationAmountDetails(data.Auth_Id);
                            
                            //hide master data entry form
                            $("#MasterDataEntryDiv").toggle('slow');

                           
                            //clear & hide the master dataentry form 
                            $(':input', '#MasterDataEntryForm').not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
                           
                            //show details grid
                            $('#DetailsGrid').show();

                            //load the transaction grid

                            loadPaymentGrid(data.Auth_Id);

                            //show details Table for dataentry
                            $("#AuthDetailsEntryDiv").show('slow');

                            //show trasnsaction form
                            $("#TransactionForm").show('slow');

                            unblockPage();

                        });


                   } else if (data.Success == false && data.Auth_Id == "-2") {
                        alert("Authorization Number allready exist.please try again");
                        return false;
                   }
                   else if (data.Success == false && data.Auth_Id == "-3") {
                       alert("Bank details does not found.Please check the bank details");
                       return false;
                   }
                else if (data.Success == false && data.Bill_ID == "-4") {
                    alert("Opening Balance entry is not finalized.Please finalize it and try again");
                    return false;
                }
                    else {
                        

                    }

                }
            });
            $.unblockUI();

        }


    });


    // button event to update the master details
    $("#btnUpdate").click(function (e) {


        if (parseFloat($("#TotalAmtEnteredChqAmount").text()) != 0 || parseFloat($("#TotalAmtEnteredDedAmount").text()) != 0 || parseFloat($("#TotalAmtEnteredGrossAmount").text()) != 0) {
            alert("Please delete all the transaction details entered for this authorization.");
            return false;
        }
              

        e.preventDefault();

        if ($("#MasterDataEntryForm").valid()) {

            if (confirm("Are you sure you want to update the details ?")) {

                $("#TXN_ID").removeAttr('disabled');
                              

                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/Authorization/PostEditAuthorizationMasterDetails/" + Bill_ID,
                    async: false,
                    data: $("#MasterDataEntryForm").serialize(),
                    error: function (xhr, status, error) {
                        unBlockPage();
                        $('#errorSpan').text(xhr.responseText);
                        $('#divError').show('slow');
                        $("#errorSpan").show('slow');
                        return false;
                    },
                    success: function (data) {
                        unblockPage();
                        $('#divError').hide('slow');
                        $('#errorSpan').html("");
                        $('#errorSpan').hide();

                        if (data.Success === undefined ) {

                            unblockPage();
                            $("#AuthMasterEntryDiv").html(data);
                            $.validator.unobtrusive.parse($("#MasterDataEntryForm"));
                            $('#TXN_ID').trigger('change');
                            $("#AuthMasterEntryDiv").show();
                            MasterTriggerWhenError = true;

                        }

                        else if (data.Success) {
                            unblockPage();


                            //show the master data grid with new entry 

                            $("#AuthorizationList").jqGrid().setGridParam
                              ({ url: '/Authorization/ListAuthorizationRequestForDataEntry/' + data.Bill_ID }).trigger("reloadGrid");

                            // loadPaymentGrid("ShowAddedEntry", data.Bill_ID);

                            $("#masterListGrid").show('slow');

                            alert("Authorization Request Details Updated Successfully.");

                            // get the transaction form 
                        

                            $('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + data.Bill_ID, function () {
                                
                                //get the amount table updates
                                GetAuthorizationAmountDetails(data.Bill_ID);

                                

                                loadPaymentGrid(data.Bill_ID);

                                //clear & hide the master dataentry form 
                                //clear & hide the master dataentry form 
                                $(':input', '#MasterDataEntryForm').not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');


                                $("#MasterDataEntryForm").toggle('slow');

                                //show details Table for dataentry
                                $("#TransactionForm").show('slow');

                                unblockPage();

                            });

                        } else if (data.Bill_ID == "-1") {
                            alert("This Authorization cant be edited,its already Finalized");
                            return false;

                        } else if (data.Success == false && data.data.Bill_ID == "-2") {
                            alert("Authorization Number allready exist.please try again");
                            return false;
                        }
                        else if (data.Success == false && data.Auth_Id == "-3") {
                            alert("Bank details does not found.Please check the bank details");
                            return false;
                        }
                     else if (data.Success == false && data.Bill_ID == "-4") {
                        alert("Opening Balance entry is not finalized.Please finalize it and try again");
                        return false;
                    }

                    }
                });
               
            }

        }
    });


});//doc .ready


//function to edit the authorization master details

function ViewMasterAuthorization(urlParam) {

    return false;
    /*

    blockPage();
    $("#TransactionForm").hide('slow');
    //get the amount table updates
    GetAuthorizationAmountDetails(urlParam);

    $("#AuthDetailsEntryDiv").hide('slow');

    $("#trnsShowtable").show('slow');

    $("#trShowHideLinkTable").show();

    // $('#masterPaymentForm').trigger("reset");
    $(':input', '#MasterDataEntryForm').not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

    blockPage();
    $('#AuthMasterEntryDiv').load('/Authorization/EditAuthorizationMasterDetails/' + urlParam, function () {
        blockPage();
        //get the selected transaction
        $('#TXN_ID').trigger('change');

        //get the payee name 
        $('#MAST_CON_ID_C').trigger('change');

        //hide all buttons
        $('#btnUpdate').hide();

        $('#btnReset').hide();

        $('#btnCancel').hide();

        $("#MasterDataEntryForm :input ").prop("readonly", 'readonly');

        $("#MasterDataEntryForm  select").prop("disabled", true);

        $("#MasterDataEntryForm :radio").prop("disabled", true);

        $("#AUTH_DATE").datepicker("disable").attr("readonly", "readonly");

        //populate trqansaction grid
        loadPaymentGrid(urlParam);

        unblockPage();

        //$('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + urlParam, function () {

        //    //show authorization daata entry form
        //    $("#AuthMasterEntryDiv").show();

        //    if (parseFloat($("#TotalAmtToEnterCachAmount").text()) == 0) {
        //        $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        //    }
        //    else {
        //        $("#AMOUNT_C").val(0).removeAttr("readonly");
        //    }

        //    // $('#MasterDataEntryForm').hide();

        //    unblockPage();

        //});

    });*/
}

function EditAuthorization(urlParam) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    //blockPage();
    //get the amount table updates
    GetAuthorizationAmountDetails(urlParam);
   
   // $("#TransactionForm").hide('slow');

   // $("#AuthDetailsEntryDiv").hide('slow');

    $("#trnsShowtable").show('slow');

    $("#trShowHideLinkTable").show();

    // $('#masterPaymentForm').trigger("reset");
    $(':input', '#MasterDataEntryForm').not("#AUTH_MONTH").not("#AUTH_YEAR").not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

         
    $('#AuthMasterEntryDiv').load('/Authorization/EditAuthorizationMasterDetails/' + urlParam, function () {

       //get the selected transaction
        $('#TXN_ID').trigger('change');

        //get the payee name 
        $('#MAST_CON_ID_C').trigger('change');
        
        $('#btnUpdate').show();
        $('#btnReset').hide();
        $('#btnCancel').show();

        //show authorization daata entry form
        $("#AuthMasterEntryDiv").show('slow');

        $.unblockUI();
        //unblockPage();

        //$('#AuthDetailsEntryDiv').load('/Authorization/GetAuthDetailsEntryForm/' + urlParam, function () {
                   

        //    if (parseFloat($("#TotalAmtToEnterCachAmount").text()) == 0)
        //    {
        //        $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        //    }
        //    else
        //    {
        //        $("#AMOUNT_C").val(0).removeAttr("readonly");
        //    }

                 

        //});

    });


}


//function to delete the authorization request
function DeleteAuthorization(urlParam) {

    var Todelete = confirm('Are you sure you want to delete the Authorization Request ?');

    if (Todelete) {

        // blockPage();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


        $.ajax({
            type: "POST",
            url: "/Authorization/DeleteAuthorizationRequest/" + urlParam,
            // async: false,
            data: $("form").serialize(),
            error: function (xhr, status, error) {
                //unblockPage();
                $.unblockUI();

                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');

                return false;

            },
            success: function (data) {
                //unblockPage();
                $.unblockUI();

                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();

                if (data.result == 1) {

                    $('#btnUpdate').hide();
                    $('#btnReset').show();
                    $('#btnSubmit').show();
                    $('#btnCancel').hide();

                    alert("Authorization request Deleted Successfuly.");

                    if ($("#AddNewMasterDetails").is(':visible')) {
                        $("#AddNewMasterDetails").trigger('click');
                    }
                    else if($("#lblBack").is(':visible'))
                    {
                        $("#lblBack").trigger('click');
                    }
                   

                    return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else {

                    alert("Error while deleting  Authorization ");
                    return false;
                }
            }
        }); //end of ajax
    }

}

//function to get the contractor supplier name according to id
function setContractorSupplierName(contractorId, contractorOrsupllier, disableName) {
    $.ajax({
        type: "POST",
        url: "/payment/GetContractorSupplierName/" + contractorId + '$' + $("#TXN_ID").val(),
        async: false,
        // data: $("#authSigForm").serialize(),
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            $("#errorSpan").show('slow');
            return false;

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data != "") {
              
                    $("#PAYEE_NAME").val(data);
                    if (disableName) {
                        $("#PAYEE_NAME").attr('readonly', 'readonly');
                    } else {
                        $("#PAYEE_NAME").removeAttr('readonly');
                    }
               
            }
        }
    });

}

//Added By Abhishek kamble 14-jan-2014
//function to get the account  Close month and year
function GetClosedMonthAndYear() {
    blockPage();

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,

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

function ModifiedDate(day, month, year) {
    return day + "/" + month + "/" + year;
}

function process(date) {
    var parts = date.split(' ')[0].split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}