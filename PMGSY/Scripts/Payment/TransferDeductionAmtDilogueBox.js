
$("#btnSubmitTransfer").click(function () {

    if (confirm("Details can not be modified after submit.Do you want to submit? ")) {
        var formData = $('#BILL_NO').val() + "$" + $('#vDate').val() + "$" + $('#EPAY_NO').val() + "$" + $('#DEDUCTION_AMOUNT').val() + "@" + $('#idTextConcat').val();
        //alert(formData);
        $.ajax({
            url: '/Payment/SubmitTransferDeductionAmountToHoldingAcc/?formData=' + formData,
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            error: function (xhr, status, error) {
                $('#divError').show('slow');
                $('#errorSpan').text(xhr.responseText);

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                alert(data.message);
                $("#PaymentList").jqGrid().setGridParam({ url: '/Payment/GetTransferDeductionAmtList/' }).trigger("reloadGrid");
                $("#transfer").dialog('close');
            }
        });

    }
})
