$(document).ready(function () {
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnSearchAlertDetails").click(function () {
            searchAlertDetails();
    });

    $("#iconSearchClose").click(function () {

        if ($("#frmSearchAlertDetails").is(":visible")) {
            $("#iconSearchClose").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#frmSearchAlertDetails").slideToggle(100);
        }
        else {
            $("#iconSearchClose").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#frmSearchAlertDetails").slideToggle(100);
        }
    });
});

function searchAlertDetails() {
    $('#tblAlertDetails').setGridParam({
             url: '/Master/ListAlertsDetails', datatype: 'json'
    });
    $('#tblAlertDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlStatus option:selected').val() } });
    $('#tblAlertDetails').trigger("reloadGrid", [{ page: 1 }]);
}