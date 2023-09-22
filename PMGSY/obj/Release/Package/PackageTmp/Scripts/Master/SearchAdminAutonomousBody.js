$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    if ($('#ddlSearchStates').val() == 0) {
        $("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);
    }

    $("#btnSearch").click(function () {
    
        searchDesig();
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
        LoadAutonomousGrid();
    }
});

function searchDesig() {
    $('#tblAutonomousBodyList').setGridParam({
        url: '/Master/GetMasterAdminAutonomousBodyList', datatype: 'json'
    });
    $('#tblAutonomousBodyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tblAutonomousBodyList').trigger("reloadGrid", [{ page: 1 }]);
}