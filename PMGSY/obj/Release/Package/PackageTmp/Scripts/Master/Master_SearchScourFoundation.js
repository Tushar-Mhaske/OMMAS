$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

  
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
        SearchDetails();
    });

});

function SearchDetails() {
    
    $('#tblMasterScourFoundationTypeList').setGridParam({
        url: '/Master/GetMasterScourFoundationTypeList', datatype: 'json'
    });

    $('#tblMasterScourFoundationTypeList').jqGrid("setGridParam", { "postData": { SfTypeCode: $('#ddlScourFoundationType option:selected').val()} });

    $('#tblMasterScourFoundationTypeList').trigger("reloadGrid", [{ page: 1 }]);
}
