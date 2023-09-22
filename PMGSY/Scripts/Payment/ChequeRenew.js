
var availableCheques = [];

jQuery.validator.addMethod("ChequeSeriesdrpRequired", function (value, element) {

    if (parseFloat($("#LevelID").val()) == 4) {
        return true;
    }
    if (value == "")
    {
        return false;
    }
    else if ((parseFloat(value) == 0))
    {
        return false;
    }
    else
    {
        return true;
    }

}, "");

//Added By Abhishek kamble for chqissuedate validation 
$.validator.unobtrusive.adapters.add('isvalidchqissuedate', ['cheissuedate', 'chqepay', 'ischqissuedatevalid'], function (options) {
    options.rules['isvalidchqissuedate'] = options.params;
    options.messages['isvalidchqissuedate'] = options.message;
});

$.validator.addMethod("isvalidchqissuedate", function (value, element, params) {
   
    if (params.chqepay != "" && $("#CHQ_EPAY").val() != "A") {

        //alert("test : " + value + "Issue : " + $("#ChequeBookIssueDate").val());
        //alert(params.chqepay);

        //var IsCheque = $('input:radio[name="' + params.chqepay + '"]:checked').val();

        //alert(IsCheque)
        //if (IsCheque == "Q") {

        var bill_date = value;
        var Issue_date = $("#ChequeBookIssueDate").val();
        bill_date = bill_date.split('/');
        Issue_date = Issue_date.split('/');
        var new_bill_date = new Date(bill_date[2], parseInt(bill_date[1]) - 1, bill_date[0]);
        var new_Issue_date = new Date(Issue_date[2], parseInt(Issue_date[1]) - 1, Issue_date[0]);
        if (new_bill_date < new_Issue_date) {
            return false;
        } else {
            return true;
        }

            //if (Date.parse(value) < Date.parse($("#ChequeBookIssueDate").val())) {
            //    return false;
            //} else {
            //    return true;
            //}
        //}
        //else {
        //    return true;
        //}
    }
    else {
        return true;
    }
});

var lvlID = 0;


$(document).ready(function () {

    lvlID = $('#LevelID').val();

    //If condition added By Abhishek kamble 6Apr2015 for Advice No
    if ($("#CHQ_EPAY").val() != "A") {

        if ($("#CHQ_Book_ID").val() == 0) {
            FillInCascadeDropdown(null, "#CHQ_Book_ID", "/Payment/GetchequebookSeries/");
        }
    }
    $.validator.unobtrusive.parse($("#ChqRenewForm"));
   

    $("#ChequeRenewCancelIcon").click(function () {
        // delete the from (as per testing by madam on 10/06/2013)
        $("#RenewChqDiv").empty();
        $("#PaymentList").jqGrid('setGridState', 'visible');
    });




    $("#btnChq_renew_Cancel").click(function () {
    
        $(':input', '#ChqRenewForm').not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');
        
    });    


    $("#btnChq_Cancel_reset").click(function () {

        $(':input', '#CancelationForm').not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

    });


    $("#BILL_DATE").on('change focusout', function () {

        if ($("#BILL_DATE").val() != "") {
            $("#CHQ_DATE").val($("#BILL_DATE").val());
           
        }

    });

    $("#CHQ_DATE").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
        onClose: function () {
            $(this).focus().blur();
        }

    });


    $("#CHEQUE_CANCEL_DATE").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
        onClose: function () {
            $(this).focus().blur();
        }

    });

    

    $("#BILL_DATE").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: new Date(),
        dateFormat: "dd/mm/yy", onClose: function () {
            $(this).focus().blur();
        }

    });


    $("#CHQ_SERIES").change(function () {

        if ($(this).val() != "" && $(this).val() != 0) {
            FillInCascadeDropdown(null, "#CHQ_NO_SELECTION", "/Payment/GetAllFinalizedCheques/" + $("#CHQ_SERIES").val());
            chequeChanged = true;
        }
        else {
            $('#CHQ_NO_SELECTION')
            .empty()
            .append('<option selected="selected" value="0">--Select--</option>');
            
            }
   });


    var selectedDate = $("#BILL_DATE").val().split('/');

    $.ajax({
        type: "POST",
        url: "/payment/GenerateVoucherNo/V$" + selectedDate[1] + '$' + selectedDate[2],
        async: false,
        // data: $("#authSigForm").serialize(),
        error: function (xhr, status, error) {
            //unblockPage();
            $('#divError').show('slow');
            $('#errorSpan').text(xhr.responseText);

        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            if (data != "") {
                // alert(data.strVoucherNumber);
                // $("#BILL_NO").attr("disabled", true);
                $("#BILL_NO").val("");
                $("#BILL_NO").val(data.strVoucherNumber);
                $("#BILL_NO").attr('readonly', true);
                //  $("#VoucherCnt").val(data.strVoucherCnt);
                // alert(data.strVoucherCnt);
            }
        }
    });


    $("#BILL_DATE").blur(function () {
        var billDate = $("#BILL_DATE").val().split('/');
  
        $.ajax({
            type: "POST",
            url: "/payment/GenerateVoucherNo/V$" + billDate[1] + '$' + billDate[2],
            async: false,
            // data: $("#authSigForm").serialize(),
            error: function (xhr, status, error) {
                //unblockPage();
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                if (data != "") {
                    // alert(data.strVoucherNumber);
                    // $("#BILL_NO").attr("disabled", true);
                    $("#BILL_NO").val("");
                    $("#BILL_NO").val(data.strVoucherNumber);
                    $("#BILL_NO").attr('readonly', true);
                    //  $("#VoucherCnt").val(data.strVoucherCnt);
                    // alert(data.strVoucherCnt);
                }
            }
        });
    })


    $("#CHQ_Book_ID").change(function () {



        if ($("#CHQ_Book_ID").val() !== 0 || $("#CHQ_Book_ID").val() !== "") {

            $.ajax({
                type: "POST",
                url: "/payment/GetAllAvailableCheques/" + $("#CHQ_Book_ID").val(),
                async: false,
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
                    $('#errorSpan').hide();
                   
                    availableCheques.length = 0;
                    if (data != "") {
                                             

                        if ($("#CHQ_NO").data('autocomplete')) {
                            $("#CHQ_NO").autocomplete("destroy");
                            $("#CHQ_NO").removeData('autocomplete');
                        }

                        availableCheques = data;

                        $("#CHQ_NO").autocomplete({
                            source: availableCheques,
                            minLength: 3
                        });

                        $.ajax({
                            type: "POST",
                            url: "/payment/GetChequeBookIssueDate?id=" + $("#CHQ_Book_ID").val(),
                            async: false,
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
                                $('#errorSpan').hide();

                                $("#spnChqBookIssueDate").html(data.IssueDate);
                                $("#ChequeBookIssueDate").val(data.IssueDate);
                                $("#lblChqBookIssueDate").show();
                                $("#spnChqBookIssueDate").show();
                            }
                        });
                    }
                    else {
                        alert("No cheques available for selected series.");
                        if ($("#CHQ_NO").data('autocomplete')) {
                            $("#CHQ_NO").autocomplete("destroy");
                            $("#CHQ_NO").removeData('autocomplete');
                        }
                        $("#lblChqBookIssueDate").hide();
                        $("#spnChqBookIssueDate").hide();
                    }
                }
            });



        }

    });


    $("#RdCancel").click(function () {

      
       // if ($("#chqCancelFormDiv").html() == " ")
            {
                    blockPage();
                    $("#chqCancelFormDiv").load("/payment/CancelCheque", function () {
                        unblockPage();
                        return false;
                    });
            }

        $("#CheqCancelTable").show('slow');
        $("#chq_renew_form").hide('slow')

    });

    $("#RdRenew").click(function () {

        $("#CheqCancelTable").hide('slow');
        $("#chq_renew_form").show('slow')

    });


    //function to renew cheques
    $("#btnChq_renewal_Submit").click(function () {
          
        //If condition added By Abhishek kamble 6Apr2015 for Advice No
        if ($("#CHQ_EPAY").val() != "A") {
            $('#CHQ_Book_ID').rules('add', {
                // maxlength: 10,
                ChequeSeriesdrpRequired: true,
                messages: {
                    ChequeSeriesdrpRequired: 'Cheque Series is Required'
                }
            });
        }
        
        if ($("#ChqRenewForm").valid())
        {
            var parmas = $("#BILL_DATE").val().toString().split('/');

            $("#BILL_MONTH").val(parmas[1]);

            $("#BILL_YEAR").val(parmas[2]);

            blockPage();
            $.ajax({
                type: "POST",
                url: "/payment/RenewCheque/" + $("#str_bill_id").val(),
                async: false,
                data: $("#ChqRenewForm").serialize(),
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
                    $('#errorSpan').hide();

                    if (data.Success != "undefined" && data.Success) {

                        //remove selected cheque number
                        $("#CHQ_NO_SELECTION option[value=" + $("#CHQ_NO_SELECTION").val() + "]").remove();

                        //reset the from
                        $(':input', '#ChqRenewForm').not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

                        //clear the grid 
                        $('#ChqList').jqGrid('clearGridData');

                        //reload payment master grid to show updated entries
                        $('#PaymentList').trigger('reloadGrid');
                        $("#PaymentList").jqGrid('setGridState', 'visible');
                        //$('#PaymentList').jqGrid('GridUnload');
                        //loadPaymentGrid("view");
                        if ($("#CHQ_EPAY").val() == "A") {
                            alert("Advice has been renewed successfully");
                        } else {
                            alert("Cheque has been renewed successfully");
                        }
                        //alert("Cheque/Advice has been renewed successfully");

                        // delete the from (as per testing by madam on 10/06/2013)
                        $("#RenewChqDiv").empty();

                        return false;
                    }
                    else if (data.Result == "-111") {
                        alert("Selected Cheque is acknowlwdged.It cant be renewed.");
                        return false;
                    }
                    else if (data.Result == "-222") {
                        alert("Selected Cheque is  already encashed by bank.It cant be renewed.");
                        return false;
                    }
                    else if (data.Result == "-333") {
                        alert("Selected Cheque is not new.It cant be renewed.");
                        return false;
                    }
                    else if (data.Result == "-444") {
                        alert("Cheque date is greater than cheque renewal date.");
                        return false;
                    }
                    else if (data.Result == "-555") {
                        alert("Cheques bill date is greater than renewed cheque bill date.");
                        return false;
                    }
                    else if (data.Result == "-777") {
                        alert("Imprest payment cant be renewed as It has been already settled.");
                        return false;
                    }
                    else if (data.Result == "-888") {
                        alert("Asset details has been entered for this voucher.It Cant be renewed");
                        return false;
                    }
                    else if (data.Result == "-11" || data.Result == "-22" || data.Result == "-999") {
                        alert(data.message);
                        return false;
                    }
                    else if (data.Success === undefined)
                    {
                        unblockPage();
                        $("#RenewChqDiv").html(data);
                        $.validator.unobtrusive.parse($("#RenewChqDiv"));
                        if (lvlID == 5)
                        {
                            $("#CHQ_Book_ID").trigger('change');
                        }
                        $("#lblChqBookIssueDate").show();
                        $("#spnChqBookIssueDate").show();
                    }

                }
            });


        }

    });
    
});

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()
    $.post(action, map, function (data) {

        $.each(data, function () {
            // alert('TEST' + this.Selected);
            // alert("fillCascaded =" + this.Value);
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}

