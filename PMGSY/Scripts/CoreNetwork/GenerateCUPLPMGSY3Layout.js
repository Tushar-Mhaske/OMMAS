$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmGenerateCUPLLayout'));

    var date = new Date();
    var cur = date.getDate();
    var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);
    var maxDate = (date.getMonth() == 3 && parseInt(cur) <= 18) ? lastDay : 0;
    var startDate = date.getMonth() == 3 ? (parseInt(cur) <= 18 ? new Date(parseInt(date.getMonth()) == 0 ? parseInt(date.getMonth() - 1) : date.getFullYear(), parseInt(date.getMonth()) == 0 ? 12 : parseInt(date.getMonth() - 1), 1) : new Date(date.getFullYear(), date.getMonth(), 1))
                                         : (parseInt(cur) <= 5 ? new Date(parseInt(date.getMonth()) == 0 ? parseInt(date.getMonth() - 1) : date.getFullYear(), parseInt(date.getMonth()) == 0 ? 12 : parseInt(date.getMonth() - 1), 1) : new Date(date.getFullYear(), date.getMonth(), 1));

    $('#txtGenerationDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        minDate: new Date(),
        maxDate: new Date(),
        //maxDate: maxDate,
        //minDate: startDate,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            //$(function () {
            //    $('#txtNewsPublishSt').focus();
            //    $('#txtNewsPublishEnd').focus();
            //})
            $('#txtGenerationDate').trigger('blur');
        }
    });

    $('#btnSubmit').click(function () {
        if (!$('#frmGenerateCUPLLayout').valid()) {
            return false;
        }
        if (confirm("Once finalized cannot modify CUPL details, Please Confirm")) {
            $.ajax({
                url: '/CoreNetwork/GenerateCUPLPMGSY3',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: $("#frmGenerateCUPLLayout").serialize(),
                success: function (jsonData) {
                    //$("#MAST_ER_SHORT_DESC").val(jsonData.RoadShortName);
                    alert(jsonData.message);
                    $('#btnView').trigger('click');
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });
        }
        else {
            return false;
        }
    });
});