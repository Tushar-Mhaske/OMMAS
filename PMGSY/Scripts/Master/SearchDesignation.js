$(document).ready(function () {
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnDesigSearch").click(function () {
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
});

function searchDesig() {
         $('#desigCategory').setGridParam({
        url: '/Master/GetDesignationList', datatype: 'json'
    });
    $('#desigCategory').jqGrid("setGridParam", { "postData": { desigCode: $('#Designation option:selected').val()} });
    $('#desigCategory').trigger("reloadGrid", [{ page: 1 }]);

}