$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmListHabitations');

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#rdbHabDirectY").prop("checked", true);

    LoadHabitationstoMapListScheme2();
    LoadScheme2MappedHabitations();

    
    // map habitation to core network //pp
    $('#btnSaveHabitation').click(function (e) {
        var blockCodes;

        blockCodes = $("#mapHabitationCandidateList").jqGrid('getGridParam', 'selarrrow');// : ("#mapHabitationList").jqGrid('getGridParam', 'selarrrow');

        if (confirm("Are you sure you want to map Habitation Details?")) {
            if (blockCodes != '') {
                $('#EncryptedHabCodes').val(blockCodes);

                blockPage();

                $.ajax({
                    url: "/CoreNetWork/MapHabitationsToNetworkPMGSY3",
                    type: "POST",
                    dataType: "json",
                    data: $("#frmListHabitations").serialize(),
                    success: function (data) {
                        unblockPage();
                        if (!(data.success)) {
                            //alert("Habitations not added.");
                            alert(data.message);
                        }
                        else {
                            alert(data.message);

                            $("#mapHabitationCandidateList").trigger('reloadGrid');
                            $("#tblScheme2Habitations").trigger('reloadGrid');
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

    //$('#ddlBlocksToMap').change(function () {
    //    if ($(this).val() > 0) {
    //        LoadMapHabitationList();
    //    }
    //});

    $('#ddlRoadsToMap').change(function () {
        if ($(this).val() >= 0) {
            LoadHabitationstoMapListScheme2();
        }
    });

    $('#rdbHabDirectY').click(function () {
        $('#ddlRoadsToMap').trigger('change');
    });
    $('#rdbHabDirectN').click(function () {
        $('#ddlRoadsToMap').trigger('change');
    });
});

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
            url: '/CoreNetWork/DeleteMapHabitationPMGSY3/' + urlparameter,
            dataType: 'json',
            //data: { __RequestVerificationToken: $("#frmSearchCoreNetworks input[name=__RequestVerificationToken]").val() },
            data: { roadCode: $("#EncryptedRoadCode").val(), __RequestVerificationToken: $("#frmListHabitations input[name=__RequestVerificationToken]").val() },
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Habitation details deleted successfully ");

                    $("#tblScheme2Habitations").trigger('reloadGrid');
                    $("#mapHabitationCandidateList").trigger('reloadGrid');

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
        url: '/CoreNetwork/GetHabitationListPMGSY3',
        datatype: "json",
        mtype: "GET",
        postData: { habCode: $('#EncryptedRoadCode').val() },
        colNames: ['Habitation System Id', 'Name of Habitation', 'Block', 'Village', 'Road Number', 'Total Population', 'SCST Population', 'Habitation Direct', 'Habitation Verified', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
                            ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
                            { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 200, align: "left", sortable: true },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 100, sortable: true, align: "center" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "center" },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 250, sortable: true, align: "center", hidden: true },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 80, sortable: true, align: "center" },
                            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', width: 60, sortable: true, align: "center", hidden: true },
                            { name: 'HabDirect', index: 'HabDirect', width: 80, sortable: true, align: "center" },
                            { name: 'HabVerified', index: 'HabVerified', width: 80, sortable: true, align: "center" },
                            //{ name: 'a', index: 'a', formatter: FormatColumn, width: 50, sortable: true, align: "center", hidden: false },
                            { name: 'a', index: 'a', width: 50, sortable: true, align: "center", hidden: false },
        ],
        pager: jQuery('#pgScheme2Habitations'),
        rowNum: 10,
        rowList: [25, 40, 55],
     //   rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Mapped Habitation List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            
            if (!data.isHabFinalized) {
                $("#pgScheme2Habitations_left").html("<input type='button' style='background-color:#fccc6b; margin-left:27px' id='idFinalizeNetwork' class='ui-button ui-widget ui-corner-all ui-button-text-only' onClick = 'FinalizeCNHabs();return false;' value='Finalize'/>")
            }
            else {
                $("#pgScheme2Habitations_left").html("");
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


function LoadHabitationstoMapListScheme2() {

    

    jQuery("#mapHabitationCandidateList").jqGrid('GridUnload');
    jQuery("#mapHabitationCandidateList").jqGrid({
        url: '/CoreNetwork/GetHabitationListToMapPMGSY3',
        datatype: "json",
        mtype: "GET",
        postData: { habCode: $('#EncryptedRoadCode').val(), erRoadCode: $('#ddlRoadsToMap option:selected').val(), habDirect: ($('#rdbHabDirectY').is(':checked') ? 'Y' : 'N') },
        colNames: ['Habitation Name', 'Village', 'Total Population'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 180, sortable: true, align: "center" }, ],
        pager: jQuery('#mapHabitationCandidatePager'),
        rowNum: 55,
        rowList: [55, 65, 75],
       // rowList: [15, 25, 35],
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
                //$("#divError").show("slow");
            }
            else {
                //alert(data.isHabFinalized);
                if ((!data.isHabFinalized)) {
                    $("#divError").hide("slow");
                    $("#btnSaveHabitation").show();
                }
                else {
                    $("#btnSaveHabitation").hide();
                }
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

function FinalizeCNHabs() {
    if (confirm("Make sure that all indirect habitations as per PMGSY3 guidelines are mapped on the TR/MRL road. After finalization user will not be able to Edit/Delete Habitation details?")) {
        $.ajax({
            url: '/CoreNetwork/FinalizeCNHabitationDetailsPMGSY3/' + $('#EncryptedRoadCode').val(),
            type: "POST",
            cache: false,
            data: { __RequestVerificationToken: $("#frmListHabitations input[name=__RequestVerificationToken]").val() },
            beforeSend: function () {
                blockPage();
            },
            //data: { PLAN_CN_ROAD_CODE: $("#PLAN_CN_ROAD_CODE").val() },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();

                alert(response.message);

                $("#tblScheme2Habitations").trigger('reloadGrid');
                $("#mapHabitationCandidateList").trigger('reloadGrid');
                $("#btnFinalize").hide();
            }
        });

    } else {
        return false;
    }
}