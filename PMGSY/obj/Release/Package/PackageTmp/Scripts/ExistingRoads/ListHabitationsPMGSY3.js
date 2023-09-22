$(document).ready(function () {

    //benifited habitation validation
    //$.validator.unobtrusive.parse('#frmBenifitedHabitations');

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadMapHabitationListPMGSY3();

    LoadHabitationGridPMGSY3();


    $('#btnSaveHabitation').click(function (e) {

        var blockCodes = $("#mapHabitationList").jqGrid('getGridParam', 'selarrrow');
        if (blockCodes != '') {
            $('#EncryptedHabCodes').val(blockCodes);
            //        alert($('#EncryptedBlockCodes').val());

            blockPage();

            $.ajax({
                url: "/ExistingRoads/MapHabitationsToExistingRoadsPMGSY3",
                type: "POST",
                dataType: "json",
                data: $("#frmListHabitations").serialize(),
                success: function (data) {

                    alert(data.message);
                    $("#mapHabitationList").jqGrid('resetSelection');

                    //                    $("#habitationCategory").trigger('reloadGrid');
                    $("#habitationCategory").trigger('reloadGrid');
                    $("#mapHabitationList").trigger('reloadGrid');
                    //$('#ddlSearchDistrict').val('0');

                    unblockPage();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Error Occured');
                    unblockPage();
                }

            });
        }
        else {
            alert('Please select Habitations to map with Existing Road.');
        }
    });

    $('#btnMapCancel').click(function (e) {

        CloseExistingRoadsDetails();

        if ($("#divExistingRoadsForm").is(":visible")) {
            $('#divExistingRoadsForm').hide('slow');
        }

        $('#btnListNetworks').trigger('click');
        $('#tbExistingRoadsList').jqGrid("setGridState", "visible");

    });

    $('#ddlBlocksToMap').change(function () {
        if ($(this).val() > 0) {
            LoadMapHabitationListPMGSY3();
        }
    });


});

function LoadMapHabitationListPMGSY3() {

    //alert($('#EncryptedRoadCode').val());
    //alert($('#ddlBlocksToMap option:selected').val());

    jQuery("#mapHabitationList").jqGrid('GridUnload');
    jQuery("#mapHabitationList").jqGrid({
        url: '/ExistingRoads/GetHabitationListToMapPMGSY3',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val(), blockCode: $('#ddlBlocksToMap option:selected').val() },
        colNames: ['Habitation Name', 'Village', 'Total Population'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            //{ name: 'MAST_HAB_CODE', index: 'MAST_HAB_CODE',width: 150, align: "left", sortable: true,hidden:true },
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: '100%', sortable: true, align: "left" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: '100%', sortable: true, align: "left" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: '210%', sortable: true, align: "center" }, ],
        pager: jQuery('#mapHabitationPager').width(20),
        rowNum: 10,
        //altRows: true,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Habitation List",
        height: 'auto',
        // autowidth: true,
        width: '100%',

        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] == 0) {
                //alert("No Habitations to Map");
                $("#btnSaveHabitation").hide();
                $("#divError").show("slow");

            }
            else {
                $("#divError").hide("slow");
                $("#btnSaveHabitation").show();
            }

            //$("#gview_mapHabitationList > .ui-jqgrid-titlebar").hide();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                alert('Error Occured');
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

function LoadHabitationGridPMGSY3() {

    jQuery("#habitationCategory").jqGrid({
        url: '/ExistingRoads/GetAllHabitationListPMGSY3',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val() },
        colNames: ['Habitation System Id', 'Habitation Name', 'Village', 'Total Population', 'Habitation Direct', 'Habitation Verified', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
                            { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: '100%', align: "left", sortable: true },
                            { name: 'VillageName', index: 'VillageName', width: '100%', align: "left", sortable: true },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: '100%', sortable: true, align: "center" },
                            { name: 'Direct', index: 'Direct', width: '100%', sortable: true, align: "center" },
                            { name: 'Verified', index: 'Verified', width: '100%', sortable: true, align: "center", hidden: false },
                            { name: 'a', index: 'a', formatter: FormatColumn, width: 50, sortable: true, align: "center" },
        ],
        pager: jQuery('#pagerHabitation').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Mapped Habitation List",
        height: 'auto',
        //autowidth: true,
        width: '100%',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function () {
            //$("#gview_habitationCategory > .ui-jqgrid-titlebar").hide();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert('Error Occured');
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}

function FormatColumn(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='deleteHabitationDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}


function deleteHabitationDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Habitation Details?")) {

        $.ajax({
            type: 'POST',
            url: '/ExistingRoads/DeleteMapHabitationPMGSY3/' + urlparameter,
            dataType: 'json',
            //data: {  },
            data: { __RequestVerificationToken: $("#frmListHabitations input[name=__RequestVerificationToken]").val(), roadCode: $("#EncryptedRoadCode").val() },
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Habitation details deleted successfully ");
                    $("#habitationCategory").trigger('reloadGrid');
                    $("#mapHabitationList").trigger('reloadGrid');
                    //$("#frmListHabitations").trigger('reset');

                    //not required
                    // PopulateHabitations();
                }
                else {
                    //alert("Habitation details can't be deleted");
                    alert(data.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert('Error Occured');
            }
        });
    }
    else {
        return false;
    }

}

