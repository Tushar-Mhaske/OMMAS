$(document).ready(function () {
    if ($("#ddlAllState").val() == 0) {
        $("#ddlAllState").val($("#ddlAllState")[0].options[1].value);
    }
    $("#btnSearchInfoDetails").click(function () {
            searchInfoDetails();
    });

    $("#iconSearchClose").click(function () {

        if ($("#frmSearchInfoDetails").is(":visible")) {
            $("#iconSearchClose").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#frmSearchInfoDetails").slideToggle(100);
        }
        else {
            $("#iconSearchClose").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#frmSearchInfoDetails").slideToggle(100);
        }
    });
    if ($("#dvSearchInfoDetails").is(":visible")) {
        $("#btnSearchInfoDetails").trigger('click')
        {
            LoadInfoDetailsList();
        }
    }
});

function searchInfoDetails() {
    $('#tblInfoDetails').setGridParam({
        url: '/Master/InfoDetailsList', datatype: 'json'
    });
    $('#tblInfoDetails').jqGrid("setGridParam", { "postData": { StateCode: $("#ddlAllState").val() } });
    $('#tblInfoDetails').trigger("reloadGrid", [{ page: 1 }]);
}