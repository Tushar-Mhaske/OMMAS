$(document).ready(function () {

    //$.validator.unobtrusive.parse($("#CancelationForm"));



    $('#btnAdd').click(function () {
        if (!$("#PaymentValidationDetails").is(":visible")) {

        }
        $('#PaymentValidationDetails').html('');
        $("#PaymentValidationDetails").load("/Payment/ExecPaymentValidationLayout/");

        //$('#PaymentValidationDetails').show('slow');

        $('#btnAdd').hide('slow');
        $('#btnSearch').show('slow');
    });

    $('#btnSearch').click(function () {

        $('#PaymentValidationDetails').html('');

        $("#PaymentValidationDetails").load("/Payment/SearchPaymentValidationLayout/");

        $('#btnAdd').show('slow');
        $('#btnSearch').hide('slow');
    });

    $('#btnSearch').trigger('click');

});