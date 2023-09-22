$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmTargetAchievement');

    var maxDt = new Date();
    maxDt.setDate(new Date().getDate() - 1);

    $('#txtFromDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        buttonText: 'From Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        maxDate: maxDt,
        minDate: new Date(2018, 0, 11),
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            $("#txtToDate").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#txtFromDate').focus();
                $('#txtToDate').focus();
            })
            $('#txtFromDate').trigger('blur');
        }
    });

    $('#txtToDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a to date',
        buttonImageOnly: true,
        buttonText: 'To Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        maxDate: maxDt,
        minDate: new Date(2018, 0, 11),
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            //$(function () {
            //    $('#txtNewsPublishSt').focus();
            //    $('#txtNewsPublishEnd').focus();
            //})
            $('#txtToDate').trigger('blur');
        }
    });

    $('#btnViewTargetAchievementReport').click(function () {
        var arrdtfrm = $('#txtFromDate').val().split('/');
        var arrdtto = $('#txtToDate').val().split('/');

        //if (new Date($('#txtFromDate').val().getMonth()) != new Date($('#txtToDate').val().getMonth()))
        if (arrdtfrm[1] != arrdtto[1])
        {
            alert('Please select dates for same month');
            return false;
        }

        if ($("#frmTargetAchievement").valid()) {
            $.ajax({
                url: "/ProgressReport/Progress/TargetAchievementReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmTargetAchievement").serialize(),
                success: function (data) {
                    if (data.success == false) {
                        alert(data.message);
                    }
                    else {
                        $("#loadReport").html('');
                        $("#loadReport").html(data);
                    }
                },
                error: function () {
                    alert("error");
                }
            })
        }
    });

});