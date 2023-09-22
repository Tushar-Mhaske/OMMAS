/// <reference path="../../../Scripts/jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {
    $.validator.unobtrusive.parse($("#frmMPVisitLayout"));

    $("#ddlStates").change(function () {
        LoadDistrict($("#ddlStates").val());
    });


    $("#btnViewMPVisitReport").click(function () {
        if ($("#frmMPVisitLayout").valid())
        {
            LoadMPVisitReport();
        }
    });
});

function LoadDistrict(state) {

    $("#ddlDistricts").empty();
    $.post("/QMSSRSReports/QMSSRSReports/GetDistricts", { StateCode: state, __RequestVerificationToken: $("#frmMPVisitLayout input[name=__RequestVerificationToken]").val() }, function (data) {
        $.each(data, function (i,v) {
            $("#ddlDistricts").append("<option value=" + v.Value + ">" + v.Text + "</option>");
        });
    }, "json");
 }

function LoadMPVisitReport()
{
  
    $.ajax({

        url: "/QMSSRSReports/QMSSRSReports/MPVisitReport",
        type: "POST",
        cache: false,
        async: true,
        data: $("#frmMPVisitLayout").serialize(),
        success: function (data, status, xhr) {
            $("#dvloadMPVisitReport").html(data);
        },
        error: function (xhr, status, errorThrown) {
        }
    });
}