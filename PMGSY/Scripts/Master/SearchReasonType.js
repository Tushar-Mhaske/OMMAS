$(document).ready(function () {
    $("#btnSearch").click(function () {
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
    $('#tblMasterReasonList').setGridParam({
        url: '/Master/GetMasterReasonList', datatype: 'json'
    });
    $('#tblMasterReasonList').jqGrid("setGridParam", { "postData": { ReasonType: $('#ReasonType option:selected').val() } });
    $('#tblMasterReasonList').trigger("reloadGrid", [{ page: 1 }]);
}