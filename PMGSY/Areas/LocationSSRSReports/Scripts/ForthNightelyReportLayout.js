$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmForthNightelyLayout'));

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmForthNightelyLayout").toggle("slow");
    });



    $("#btnView").click(function () {

        if (parseInt($('#ddlFromYear').val()) > parseInt($('#ddlToYear').val())) {
            alert('From Year should be less than To Year');
            return false;
        }

        if (parseInt($('#ddlFromYear').val()) == parseInt($('#ddlToYear').val())) {
            if (parseInt($('#ddlFromMonth').val()) > parseInt($('#ddlToMonth').val())) {

                alert('From Month should be less than To Month');
                return false;
            }
        }

        if ($('#frmForthNightelyLayout').valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/LocationSSRSReports/LocationSSRSReports/ForthNightelyReport/',
                type: 'POST',
                catche: false,
                data: $("#frmForthNightelyLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvForthNightelyReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

});