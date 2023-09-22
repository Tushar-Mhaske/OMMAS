$(document).ready(function () {
    $("#btnSearch").click(function () {
        searchStreamType();
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

function searchStreamType() {
     $('#tblList').setGridParam({
        url: '/Master/GetMasterStreamsList', datatype: 'json'
    });
    $('#tblList').jqGrid("setGridParam", { "postData": { StreamType: $('#StreamType option:selected').val() } });
    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}