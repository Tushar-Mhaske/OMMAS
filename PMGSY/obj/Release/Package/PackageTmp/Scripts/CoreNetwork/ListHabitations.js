$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddEditCoreNetworks');


    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    if ($('#PMGSYScheme').val() == 1) {
        LoadMapHabitationList();
        LoadHabitationGrid();
    }

    if ($('#PMGSYScheme').val() == 2) {
        LoadMapHabitationListScheme2();
        LoadScheme2MappedHabitations();
    }
    // map habitation to core network //pp
    $('#btnSaveHabitation').click(function (e) {
        var blockCodes;
        if ($('#PMGSYScheme').val() == 2) {
           
                 blockCodes = $("#mapHabitationCandidateList").jqGrid('getGridParam', 'selarrrow');// : ("#mapHabitationList").jqGrid('getGridParam', 'selarrrow');
            }
            else
            {
                blockCodes = $("#mapHabitationList").jqGrid('getGridParam', 'selarrrow');
        }
        if (confirm("Are you sure you want to map Habitation Details?")) {
            if (blockCodes != '') {
                $('#EncryptedHabCodes').val(blockCodes);

                blockPage();

                $.ajax({
                    url: "/CoreNetWork/MapHabitationsToNetwork",
                    type: "POST",
                    dataType: "json",
                    data: $("#frmListHabitations").serialize(),
                    success: function (data) {
                        unblockPage();
                        if (!(data.success)) {
                            alert("Habitations not added successfully.");
                        }
                        else {
                            alert(data.message);
                            if ($('#PMGSYScheme').val() == 1) {
                                $("#mapHabitationList").jqGrid('resetSelection');
                                $("#habitationCategory").trigger('reloadGrid');
                                //$('#ddlSearchDistrict').val('0');
                                $("#mapHabitationList").trigger('reloadGrid');
                            }

                            if ($('#PMGSYScheme').val() == 2) {
                                $("#mapHabitationCandidateList").trigger('reloadGrid');
                                $("#tblScheme2Habitations").trigger('reloadGrid');
                            }
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        unblockPage();
                    }

                });
            }
            else {

                alert('Please select Habitations to map with Core Network.');
            }
        }
        else {
            return false;
        }
    });

    // cancel button click
    $('#btnMapCancel').click(function (e) {


        CloseProposalDetails();
        if ($("#divAddForm").is(":visible")) {
            $('#divAddForm').hide('slow');
        }

        $('#btnSearchView').trigger('click');
        $('#networkCategory').jqGrid("setGridState", "visible");

    });

    $("#ddlHabitations").change(function () {

        GetTotalPopulation();
    });

    GetRoadName();

    $('#ddlBlocksToMap').change(function () {
        if ($(this).val() > 0) {
            LoadMapHabitationList();
        }
    });

    $('#ddlRoadsToMap').change(function () {
        if ($(this).val() > 0) {
            LoadMapHabitationListScheme2();
        }
    });
});

function LoadMapHabitationList() {

    jQuery("#mapHabitationList").jqGrid('GridUnload');
    jQuery("#mapHabitationList").jqGrid({
        url: '/CoreNetwork/GetHabitationListToMap',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val(), blockCode: $('#ddlBlocksToMap option:selected').val() },
        colNames: ['Habitation Name', 'Village', 'Total Population'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 180, sortable: true, align: "center" }, ],
        pager: jQuery('#mapHabitationPager'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Habitation List",
        height: '130px',
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] == 0) {
                $("#btnSaveHabitation").hide();
                $("#divError").show("slow");

            }
            else {
                $("#divError").hide("slow");
                $("#btnSaveHabitation").show();
            }
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}

function LoadHabitationGrid() {

    jQuery("#habitationCategory").jqGrid({
        url: '/CoreNetwork/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val(), Flag: $("#UnlockFlag").val() },
        colNames: ['Habitation System Id', 'Name of Habitation', 'Block', 'Village', 'Road Number', 'Total Population', 'SC/ST Population', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
                            { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, align: "left", sortable: true },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 250, sortable: true, align: "center", hidden: true },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 150, sortable: true, align: "center" },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 60, sortable: true, align: "center" },
                            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', width: 60, sortable: true, align: "center" },
                            { name: 'a', index: 'a', width: 50, sortable: true, align: "center" },
        ],
        pager: jQuery('#pagerHabitation'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Mapped Habitation List",
        height: '130px',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
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
    return "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}

function GetTotalPopulation() {

    $.ajax({
        type: 'POST',
        url: '/CoreNetWork/GetTotalPopulationByHabCode',
        data: { habCode: $("#ddlHabitations option:selected").val() },
        async: false,
        cache: false,
        success: function (data) {
            $("#lblPopulation").html(data.population);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    })

}

function deleteHabitationDetails(urlparameter) {

    //alert($("#EncryptedRoadCode").val());
    if (confirm("Are you sure you want to delete Habitation Details?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetWork/DeleteMapHabitation/' + urlparameter,
            dataType: 'json',
            data: { roadCode: $("#EncryptedRoadCode").val() },
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Habitation details deleted successfully ");

                    if ($('#PMGSYScheme').val() == 1) {
                        $("#habitationCategory").trigger('reloadGrid', [{ page: 1 }]);
                        $("#mapHabitationList").trigger('reloadGrid');
                    }

                    if ($('#PMGSYScheme').val() == 2) {
                        $("#tblScheme2Habitations").trigger('reloadGrid');
                        $("#mapHabitationCandidateList").trigger('reloadGrid');
                    }
                }
                else {
                    alert("Habitation details are in use and can't be deleted");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }

}

function PopulateHabitations() {

    $("#ddlHabitations").val(0);
    $("#ddlHabitations").empty();

    $.ajax({
        url: '/CoreNetWork/PopulateHabitations/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { roadCode: $("#EncryptedRoadCode").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlHabitations").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });
}

function GetRoadName() {
    $.ajax({
        type: 'POST',
        url: '/CoreNetWork/GetRoadNameForHabitation/',
        data: { roadCode: $("#EncryptedRoadCode").val() },
        success: function (data) {
            $("#lblRoadName").html(data);
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    })
}

function LoadScheme2MappedHabitations() {

    jQuery("#tblScheme2Habitations").jqGrid('GridUnload');
    jQuery("#tblScheme2Habitations").jqGrid({
        url: '/CoreNetwork/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val() },
        colNames: ['Habitation System Id', 'Name of Habitation', 'Block', 'Village', 'Road Number', 'Total Population', 'SCST Population', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
                            { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 200, align: "left", sortable: true },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "center" },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 250, sortable: true, align: "center", hidden: true },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 200, sortable: true, align: "center" },
                            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', width: 60, sortable: true, align: "center", hidden: true },
                            //{ name: 'a', index: 'a', formatter: FormatColumn, width: 50, sortable: true, align: "center", hidden: false },
                            { name: 'a', index: 'a', width: 50, sortable: true, align: "center", hidden: false },
        ],
        pager: jQuery('#pgScheme2Habitations'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Mapped Habitation List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}


function LoadMapHabitationListScheme2() {

    jQuery("#mapHabitationCandidateList").jqGrid('GridUnload');
    jQuery("#mapHabitationCandidateList").jqGrid({
        url: '/CoreNetwork/GetHabitationListToMap',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#EncryptedRoadCode').val(), erRoadCode: $('#ddlRoadsToMap option:selected').val() },
        colNames: ['Habitation Name', 'Village', 'Total Population'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 180, sortable: true, align: "center" }, ],
        pager: jQuery('#mapHabitationCandidatePager'),
        rowNum: 15,
        rowList: [15, 25, 35],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Habitation List",
        height: '130px',
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            if (data["records"] == 0) {
                $("#btnSaveHabitation").hide();
                $("#divError").show("slow");

            }
            else {
                $("#divError").hide("slow");
                $("#btnSaveHabitation").show();
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}