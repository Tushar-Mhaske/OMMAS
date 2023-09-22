$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmQMComplainCreateForm");

    $("#btnSubmit").click(function () {
        if ($("#frmQMComplainCreateForm").valid()) {
            var formData = $("#frmQMComplainCreateForm").serialize();
            // alert(JSON.stringify(formData));
            var postURL = "/QualityMonitoring/QMComplainCreate";
            $.post(postURL, formData, function (responseData) {
                if (responseData.Success) {
                    alert("Complaint Details Saved Successfully.");
                    LoadDiv("/QualityMonitoring/GetQMComplainList");
                }
                else {
                    alert('Error occured on Complaint Details Save');
                }
            });
        }
    });

    $('#txtComplainRecievedDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        maxDate: 0,
        onSelect: function (selectedDate) {
            $(function () {
                $('#txtComplainRecievedDate').focus();
            })
        }
    });

});

function CloseQMComplainCreate() {
    LoadDiv("/QualityMonitoring/GetQMComplainList");
}