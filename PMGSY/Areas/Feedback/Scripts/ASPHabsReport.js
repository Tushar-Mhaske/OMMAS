$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmASPHabs');

    //$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val(), $.unblockUI());
    LoadReport();
    $("#btnViewASPHabs").click(function () {
        LoadReport();
    });

});

function LoadReport() {   
        if ($("#frmASPHabs").valid()) {
            $.ajax({
                url: "/Feedback/Feedback/AspirationalDistrictHabsReport/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmASPHabs").serialize(),
                success: function (data) {
                    $("#loadReport").html('');
                    $("#loadReport").html(data);
                },
                error: function () {
                    alert("error");
                }
            })
        }
        $.unblockUI();
}