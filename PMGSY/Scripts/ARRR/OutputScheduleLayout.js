$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmOutputSchedule');

    $("#idFilterDivOutput").click(function () {
        $("#idFilterDivOutput").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
        $("#frmOutputSchedule").toggle("slow");
    });

    $("#btnSaveOutput").click(function () {

        if ($('#frmOutputSchedule').valid()) {
            $('#User_Action').val('A');
            $.ajax({
                url: '/ARRR/AddScheduleOutputDetails/',
                async: false,
                type: 'POST',
                data: $("#frmOutputSchedule").serialize(),
                success: function (data) {
                    alert(data.message);
                    if (data.success == true) {

                        $("#btnResetOutput").trigger('click');
                        //LoadLMMScheduleGrid();
                        LoadTaxScheduleGrid();
                    }
                }
            })
        }
    });

    $("#btnResetOutput").click(function () {

    });

});

