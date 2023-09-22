$(document).ready(function () {


    $.validator.unobtrusive.parse($("#CancelationForm"));

    //Advice No Change 13Apr2015   
    if ($("#CHQ_EPAY").val() == "A") {
        $("#btnChq_Cancel_Submit").val("Cancel Advice");
    }

    $("#btnChq_Cancel_reset").click(function () {

        $(':input', '#CancelationForm').not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

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


    //function to cancel cheques

    $("#btnChq_Cancel_Submit").click(function () {

        $.validator.unobtrusive.parse($("#CancelationForm"));

        if ($("#CancelationForm").valid()) {

            blockPage();

            $.ajax({
                type: "POST",
                url: "/payment/CancelCheque/" + $("#str_bill_id").val(),
                async: false,
                data: $("#CancelationForm").serialize(),
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
                        $(':input', '#CancelationForm').not(':button, :submit, :reset, :hidden,:radio').val('').removeAttr('selected');

                        //clear the grid 
                        $('#ChqList').jqGrid('clearGridData');

                        //reload payment master grid to show updated entries
                        //$('#PaymentList').jqGrid('GridUnload');
                        $('#PaymentList').trigger('reloadGrid');
                        $("#PaymentList").jqGrid('setGridState', 'visible');
                        //loadPaymentGrid("view");

                        if ($("#CHQ_EPAY").val() == "A") {
                            alert("Advice has been cancelled successfully");
                        } else {
                            alert("Cheque has been cancelled successfully");
                        }

                        // delete the from (as per testing by madam on 10/06/2013)
                        $("#RenewChqDiv").empty();

                        return false;
                    }
                    //else if (data.Result == "-111") {
                    //    alert("Selected Cheque is acknowledged.it cant be cancelled.");
                    //    return false;
                    //}
                    if (data.Result == "-222") {
                        alert("Selected Cheque is already encashed by bank.it cant be cancelled.");
                        return false;
                    }
                    else if (data.Result == "-333") {
                        alert("Selected Cheque is not new.it cant be cancelled.");
                        return false;
                    }
                    else if (data.Result == "-444") {
                        alert("cheque date is greater than cheque cancellation date.");
                        return false;
                    }
                    else if (data.Result == "-777") {
                        alert("Imprest payment Cant be cancelled as it has been already settled.");
                        return false;
                    }
                    else if (data.Result == "-888") {
                        alert("Asset details has been entered for this voucher.It Cant be cancelled");
                        return false;
                    }
                        //else if (data.Result == "-999")
                        //{
                        //    alert("Cheque cannot be cancelled after 3 months of cheque issue date  ");
                        //    return false;
                        //}
                    else if (data.Result == "-11" || data.Result == "-22" || data.Result == "-999") {
                        alert(data.message);
                        return false;
                    }
                    else if (data.Success === undefined) {

                        unblockPage();
                        $("#chqCancelFormDiv").html(data);
                        $.validator.unobtrusive.parse($("#RenewChqDiv"));
                        $.validator.unobtrusive.parse($("#CancelationForm"));
                    }
                }
            });
        }
    });
});