$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmOpeningBalance');

    $("#btnGenerateXML").click(function () {
        GenerateXMLFile();
    });
    $('#OpeningDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'OpeningBalance',
        maxDate: "0D",
        buttonImageOnly: true,
        buttonText: 'Opening Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#OpeningDate').trigger('blur');
        }
    });

});
var lastValid = $('#OpeningBalance').val();
var validNumber = new RegExp(/^\d*\.?\d{0,2}$/);
function validateNumber(elem) {
   
    if (validNumber.test(elem.value)) {
        lastValid = elem.value;

    } else {
        elem.value = lastValid;
    }
}

function GenerateXMLFile() {
    var formdata = new FormData(document.getElementById("frmOpeningBalance"));
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    $.ajax({
        url: '/Reat/Reat/GenerateXMLForOpeningBalance',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (jsonData) {
            // if (jsonData.success)
            alert(jsonData.message);
            $("#OpeningBalance").val('');
            $.unblockUI();
            $("#mainDiv").load("/REAT/REAT/GetOpeningBalanceLayout/", function () {
                unblockPage();
            });
        },
        error: function (err) {
            alert("Error occureed while processing your request.");
            $("#OpeningBalance").val('');
            $.unblockUI();
        }
    });
}
