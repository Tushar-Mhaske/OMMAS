$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //Added By Abhishek kamble 28-Apr-2014
    //$(function () {

    //    if ($("#RoleCode").val() == 36)
    //    {
    //        $("#btnSearch").trigger("click");            
    //    }
    //});

    $("#btnSearch").click(function () {
        SearchContractorClassType();
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
    $("#btnSearch").trigger('click')
    {
        LoadContractorClassTypeList();
    }
});

function SearchContractorClassType() {
    $('#tblMasterContClassTypeList').setGridParam({
        url: '/Master/GetMasterContractorClassTypeList', datatype: 'json'
    });
    $('#tblMasterContClassTypeList').jqGrid("setGridParam", { "postData": { StateCode: $('#StateList option:selected').val() } });
    $('#tblMasterContClassTypeList').trigger("reloadGrid", [{ page: 1 }]);
}