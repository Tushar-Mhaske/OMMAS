$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmNqmTourDetails');

    $("#btnViewReport").click(function () {
        debugger;
        if ($("#frmNqmTourDetails").valid()) {
            $.ajax({
                url: "/QMSSRSReports/QMSSRSReports/TourDetailsReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmNqmTourDetails").serialize(),
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
