$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAwardedInProgressRoadworks');

    $("#btnViewReport").click(function () {
        debugger;
        if ($("#frmAwardedInProgressRoadworks").valid()) {
            $.ajax({
                url: "/ProgressReport/Progress/AwardedInProgressRoadworksReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmAwardedInProgressRoadworks").serialize(),
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