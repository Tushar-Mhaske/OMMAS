$(document).ready(function () {


    $('.EPD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Log Entry Date',
        maxDate: "0D",   //null,  to disable future dates
        minDate: null,    //"0D",  to disable past dates
        buttonImageOnly: true,
        buttonText: 'Log Entry Date',
        changeMonth: true,
        changeYear: true,
        stepMonths: false,
        onSelect: function (selectedDate) {

            $('.EPD').trigger('blur');
        }

    });

    /*setInterval(fetchLogContent, 5000); */// Fetch every 5 seconds (adjust interval as needed)

});


$("#btnGetLogDetail").click(function () {
    if ($("#frmErrorLogIndex").valid()) {
        fetchLogContent();
    }
});


function fetchLogContent() {

    var ModuleId = $("#ddlmoduleID").val();
    var LogDate = $("#logDate").val();
    //alert("ModuleId " + ModuleId);
    //alert("LogDate " + LogDate);

    $.ajax({
        url: '/ErrorLogArea/ErrorLog/GetLogContent?moduleID=' + ModuleId + "&logDate=" + LogDate,
        method: 'POST',
        success: function (data) {
            $('#logDiv').html(data); // Update the div content with fetched log data
        },
        error: function () {
            $('#logDiv').html('Error fetching log data.');
        }
    });
}
