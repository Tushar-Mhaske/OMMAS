$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnTechnologySearch").click(function () {
        SearchTechDetail();
    });

    $("#dvhdSearch").click(function () {
        if ($("#dvSearchTechnologyParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchTechnologyParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchTechnologyParameter").slideToggle(300);
        }
    });
    $("#btnTechnologySearch").trigger('click')
    {
        LoadTechnologyDetailsList();
    }

});

function SearchTechDetail() {

    $('#tblTechnologyDetails').setGridParam({
        url: '/Master/TechnologyDetailsList', datatype: 'json'
    });
    $('#tblTechnologyDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlTechnologyStatus option:selected').val() } });
    $('#tblTechnologyDetails').trigger("reloadGrid", [{ page: 1 }]);
}