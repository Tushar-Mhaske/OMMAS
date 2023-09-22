$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    if ($('#stateCode').val() > 0) {
        $("#ddlSearchStates").val($('#stateCode').val());
        $("#ddlSearchStates").attr("disabled", true);
       
    }
    else if ($("#ddlSearchStates").val() == 0) {
        //$("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);     

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


    $('#btnSearch').trigger('click')
    {
        LoadGrid();
    };

});

function SearchDetails() {

    $('#tblVidhanSabhaList').setGridParam({
        url: '/Master/GetMasterVidhanSabhaTermList', datatype: 'json'
    });

    $('#tblVidhanSabhaList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });

    $('#tblVidhanSabhaList').trigger("reloadGrid", [{ page: 1 }]);
}

