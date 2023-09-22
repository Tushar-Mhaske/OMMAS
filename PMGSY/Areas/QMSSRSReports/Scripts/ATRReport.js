$(document).ready(function () {

    //# Added For Searchable Dropdown for Monitor on 25-01-2023 
    $("#monitorCodeATR").chosen();

    //alert("Ajinkya")
    $("#AtrReport").click(function () {

        if (parseInt($('#frmYearATR').val()) > parseInt($('#toYearATR').val())) {
            alert('From Year should be less than or equal to To Year');
            return false;
        }
        else if ((parseInt($('#frmYearATR').val()) == parseInt($('#toYearATR').val())) && (parseInt($('#frmMonthATR').val()) > parseInt($('#toMonthATR').val()))) {
            alert('From Month should be less than or equal to To Month');
            return false;
        }



        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/ATRStatusReport/',
            type: 'POST',
            catche: false,
            data: $("#AtrFilterForm").serialize(),
            async: false,
            success: function (response) {
                $("#ATRStatus").html(response);
            },
            error: function () {

                alert("Error ocurred");
                return false;
            },
        });

    });

});