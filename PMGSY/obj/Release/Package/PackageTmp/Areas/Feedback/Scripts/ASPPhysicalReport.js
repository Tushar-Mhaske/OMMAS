$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmASPPhysical');

    //$("#loadReport").load("/MPR/MPR/MPR1Report/" + $("#ddlYearMPR1").val() + "$" + $("#ddlMonthMPR1").val() + "$" + $("#ddlCollabMPR1").val(), $.unblockUI());
    LoadReport();
    $("#btnViewASPPhysical").click(function () {
        LoadReport();
    });

});

function LoadReport() {
    if ($("#frmASPPhysical").valid()) {
        $.ajax({
            url: "/Feedback/Feedback/AspirationalDistrictPhysicalReport/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmASPPhysical").serialize(),
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