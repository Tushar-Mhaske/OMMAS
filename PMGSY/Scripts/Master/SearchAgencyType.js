$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnSearch").click(function () {
        SearchAgency();
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

function SearchAgency() {
        $('#tblMasterAgencyList').setGridParam({
        url: '/Master/GetMasterAgencyList', datatype: 'json'
         });
         $('#tblMasterAgencyList').jqGrid("setGridParam", { "postData": { AgencyType: $('#AgencyType option:selected').val() } });
         $('#tblMasterAgencyList').trigger("reloadGrid", [{ page: 1 }]);
  }