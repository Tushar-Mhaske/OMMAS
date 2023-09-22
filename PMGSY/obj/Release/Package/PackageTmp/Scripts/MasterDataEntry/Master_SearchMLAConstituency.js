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


        $('#tbMLAConstituencyList').jqGrid("setGridState", "visible");
        // $('#tbMLAConstituencyList').setGridParam({ hidegrid: false });

        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        }

        SearchDetails();

    });
    $('#btnSearch').trigger('click')
    {
        loadGrid();
    };
});

function SearchDetails() {

    $('#tbMLAConstituencyList').setGridParam({
        url: '/LocationMasterDataEntry/GetMLAConstituencyDetailsList'
    });
    $('#tbMLAConstituencyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tbMLAConstituencyList').trigger("reloadGrid", [{ page: 1 }]);

}