$(document).ready(function () {

    if ($("#frmSearchMpMembers") != null) {
        $.validator.unobtrusive.parse("#frmSearchMpMembers");
    }
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });

    $('#btnSearch').click(function (e) {
        if ($('#frmSearchItemtype').valid()) {
            SearchDetails();
        }
    });


});

function SearchDetails() {

    $('#tblList').setGridParam({
        url: '/Master/GetMasterExecutionDetails', datatype: 'json'
    });
    $('#tblList').jqGrid("setGridParam", { "postData": {typeCode: $('#ddlSearchType option:selected').val() } });
    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}