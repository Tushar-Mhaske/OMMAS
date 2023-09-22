$(document).ready(function () {
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#AgencyType").val("P");

    if ($("#roleCode").val() == 36) {
        $("#AgencyType").val("S");
        $("#AgencyType").attr("disabled", true);
        $("#btnAddAgency").hide('slow');
    }
    $('#btnViewAgency').show('slow');
    //if ($("#roleCode").val() == 36 || $("#roleCode").val() == 25) {
    //    $('#btnViewAgency').show('slow');
    //}
    //if ($("#roleCode").val() == 25) {
    //    $("#AgencyType").val("P");
    //    $("#AgencyType").attr("disabled", true);
    //    $("#btnAddAgency").hide('slow');
    //}

    $("#btnSearch").click(function () {
        searchDesig();
    });

    $("#btnSearch").trigger('click')
    {
        
       // LoadGrid();
        LoadSearchGrid();

    }

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
    $('#tblList').setGridParam({
        url: '/Master/GetAdminTechnicalAgencyDetails', datatype: 'json'
    });
    $('#tblList').jqGrid("setGridParam", { "postData": { AgencyType: $('#AgencyType option:selected').val(), AgencyName: $('#AgencyName').val() } });
    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);
}
function LoadSearchGrid() {
    $('#tblList').jqGrid({

        url: '/Master/GetAdminTechnicalAgencyDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Technical Agency Name', 'Contact Name', 'Designation', 'Level', 'Map States/Districts', 'Mapped States/Districts', 'Action', 'View'], //, 'Address', 'State Name', 'District Name', 'Contact No 1', 'Contact No 2', 'FAX', 'Mobile Number', 'Email', 'Website', 'Remark'
        colModel: [
                           { name: 'TAName', index: 'TAName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'ContactName', index: 'ContactName', height: 'auto', width: 150, align: "left", sortable: true },

                           { name: 'TADesignation', index: 'TADesignation', height: 'auto', width: 150, align: "left", sortable: true },
                           { name: 'Level', index: 'Level', height: 'auto', width: 80, align: "left", sortable: true },
                           { name: 'Map', width: 90, sortable: false, resize: false, align: "center" },
                           { name: 'Mapped', width: 90, sortable: false, resize: false, align: "center" },
                           { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                           { name: 'View', width: 60, sortable: false, resize: false, formatter: FormatColumnView, align: "center", sortable: false }

        ],
        postData: { AgencyType: $('#AgencyType option:selected').val(), AgencyName: $('#AgencyName').val() },
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Level,TAName',
        sortorder: "asc",
        caption: "Technical Agency List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {

        },
    });
}