$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnTestSearch").click(function () {
        SearchTesDetail();
    });

    $("#dvhdSearch").click(function () {
        if ($("#dvSearchTestParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchTestParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchTestParameter").slideToggle(300);
        }
    });
    $("#btnTestSearch").trigger('click')
    {
        LoadTestDetailsList();
    }

});

function SearchTesDetail() {

    $('#tblTestDetails').setGridParam({
        url: '/Master/TestDetailsList', datatype: 'json'
    });
    $('#tblTestDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlTestStatus option:selected').val() } });
    $('#tblTestDetails').trigger("reloadGrid", [{ page: 1 }]);
}