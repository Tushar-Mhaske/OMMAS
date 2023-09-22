$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#ddlSearchStateUTs option[value='0']").remove();
    $("#ddlSearchStateTypes option[value='0']").remove();

   $("#ddlSearchStateUTs").append("<option value='0' selected> All </option>");
    $("#ddlSearchStateTypes").append("<option value='0' selected>All</option>");

    $("#btnSearch").click(function () {
        searchState();
    });

    $("#dvhdSearch").click(function () {
        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
    $("#btnSearch").trigger('click')
    {
        LoadGrid();
    }

});

function searchState() {
    $('#tbStateList').setGridParam({
        url: '/LocationMasterDataEntry/GetStateDetailsList', datatype: 'json'
    });
    $('#tbStateList').jqGrid("setGridParam", { "postData": { StateUT: $('#ddlSearchStateUTs option:selected').val(), StateType: $('#ddlSearchStateTypes option:selected').val() } });
    $('#tbStateList').trigger("reloadGrid", [{ page: 1 }]);
}