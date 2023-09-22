$(document).ready(function () {

    $('#btnSearch').trigger("click");
    //for expand and collpase Document Details 
    $("#dvhdSearchDistrict").click(function () {

        if ($("#dvSearchDistrictParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$("#dvDocumentDetails").css('margin-bottom','10px');

            $(this).next("#dvSearchDistrictParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchDistrictParameter").slideToggle(300);
        }
    });

    if ($("#ddlSearchStates").val() == 0) {
        $("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);
    }

    $('#btnSearch').click(function (e) {

        SearchDistrictDetails();
       
    });
    loadGrid();
});

function SearchDistrictDetails() { 
    
    $('#tbDistrictList').setGridParam({
        url: '/LocationMasterDataEntry/GetDistrictDetailsList'
    });
        //var data = $('#tbDistrictList').jqGrid("getGridParam", "postData");
        //data._search = true;
        //data.searchField = { stateCode: $('#ddlSearchStates option:selected').val() }; //$("#frmSearchDistrict").serialize();
       // $('#tbDistrictList').jqGrid("setGridParam", { "postData": data });

    $('#tbDistrictList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tbDistrictList').trigger("reloadGrid", [{ page: 1 }]);
    
}