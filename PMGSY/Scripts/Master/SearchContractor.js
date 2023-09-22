$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keycode == 13) {
            return false;
        }
    });
    
    $.validator.unobtrusive.parse('#frmSearchContractor');

    $("#loadGrid").click(function (e) {
        SearchDetails();
        $('#contractorRegistration').GridUnload();
    });

});

function SearchDetails() {
    $('#Contractor').setGridParam({
        url: '/Master/GetList1', datatype: 'json'
    });

    var data = $('#Contractor').jqGrid("getGridParam", "postData");
    data._search = true;
    delete data.searchField;
    data.searchField = $("#frmListContractor").serialize();
    $('#Contractor').jqGrid("setGridParam", { "postData": data });
    $('#Contractor').trigger("reloadGrid", [{ page: 1 }]);
}





