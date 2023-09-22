$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    if ($('#ddlSearchStates').val() == 0) {
        $("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);
    }

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
        $('#tblRegionList').jqGrid("setGridState", "visible");
            if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
            $('#dvMapRegionDistrictsDetails').hide('slow');
        }
        SearchDetails();

    });

    $('#btnSearch').trigger('click')
    {
        LoadRegionGrid();
    };
});

function SearchDetails() {

    $('#tblRegionList').setGridParam({
        url: '/Master/GetMasterRegionList/'
    });
    $('#tblRegionList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tblRegionList').trigger("reloadGrid", [{ page: 1 }]);

}