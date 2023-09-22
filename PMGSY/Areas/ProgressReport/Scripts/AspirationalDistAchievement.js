$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAspirationalDistrictAchievement');

    $("#btnViewReport").click(function () {
        debugger;
        if ($("#frmAspirationalDistrictAchievement").valid()) {
            $.ajax({
                url: "/ProgressReport/Progress/AspirationalDistAchievementReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmAspirationalDistrictAchievement").serialize(),
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
