$(document).ready(function () {


    if ($("#ddlSearchStates").val() == 0) {
        $("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);
    }
    //for expand and collpase Document Details 
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

        $('#tbMPConstituencyList').jqGrid("setGridState", "visible");

        if ($("#dvMapMPConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMPConstituencyBlockDetails').hide('slow');
        }

        SearchDetails();
       
    });

    $('#btnSearch').trigger('click')
    {
        loadGrid();
    };
});

function SearchDetails() {

    $('#tbMPConstituencyList').setGridParam({
        url: '/LocationMasterDataEntry/GetMPConstituencyDetailsList'
    });
   /* var data = $('#tbMPConstituencyList').jqGrid("getGridParam", "postData");
    data._search = true;
    data.searchField = $("#frmSearchMPConstituency").serialize();
    $('#tbMPConstituencyList').jqGrid("setGridParam", { "postData": data });*/

    $('#tbMPConstituencyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tbMPConstituencyList').trigger("reloadGrid", [{ page: 1 }]);

}