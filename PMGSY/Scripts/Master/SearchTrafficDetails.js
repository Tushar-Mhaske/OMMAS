$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnTrafficSearch").click(function () {
        SearchTechDetail();
    });

    $("#dvhdSearch").click(function () {
        if ($("#dvSearchTrafficParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchTrafficParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchTrafficParameter").slideToggle(300);
        }
    });
    $("#btnTrafficSearch").trigger('click')
    {
        LoadTrafficDetailsList();
    }

});

function SearchTechDetail() {

    $('#tblTrafficDetails').setGridParam({
        url: '/Master/GetTrafficTypeList', datatype: 'json'
    });
    $('#tblTrafficDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlTrafficStatus option:selected').val() } });
    $('#tblTrafficDetails').trigger("reloadGrid", [{ page: 1 }]);
}