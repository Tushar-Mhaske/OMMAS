$(document).ready(function () {


    $.validator.unobtrusive.parse('#frmSearchCoreNetworks');
    $("#btnRoadSearch").click(function () {
        searchDetails();
    });

    $("#ddlDistricts").change(function () {




    });


});


function searchDetails() {

    $('#networkCategory').setGridParam({
        url: '/CoreNetwork/GetCoreNetWorksList', datatype: 'json'
    });
    $('#networkCategory').jqGrid("setGridParam", { "postData": { blockCode: $('#ddlBlocks option:selected').val(), roadCode: $('#ddlRoadCategory option:selected').val() } });
    $('#networkCategory').trigger("reloadGrid", [{ page: 1 }]);
}