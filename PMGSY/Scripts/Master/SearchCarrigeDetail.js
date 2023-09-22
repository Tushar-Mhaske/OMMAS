$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnCarriageSearch").click(function () {
        SearchCarrDetail();
    });

    $("#dvhdSearch").click(function () {
        if ($("#dvSearchCarriageParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchCarriageParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchCarriageParameter").slideToggle(300);
        }
    });
    $("#btnCarriageSearch").trigger('click')
    {
        LoadCarriageDetailsList();
    }
   
});

function SearchCarrDetail() {
   
    $('#tblCarriageDetails').setGridParam({
        url: '/Master/CarriageDetailsList', datatype: 'json'
    });
    $('#tblCarriageDetails').jqGrid("setGridParam", { "postData": { Status: $('#ddlCarriageStatus option:selected').val() } });
    $('#tblCarriageDetails').trigger("reloadGrid", [{ page: 1 }]);
}