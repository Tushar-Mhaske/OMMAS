/*---------------------------------------------------------------------------------------------
Poject Name :PMGSY-II
File Name: StateLoginController.cs
Path: PMGSY/Controller/StateLoginController
Created By: Ashish Markande
Ceation Date:04/07/2013
Purpose: To show Habitation list and to change the status of habitation.
-----------------------------------------------------------------------------------------------
*/

$(document).ready(function () {



    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $("#tabs").tabs();

    $(function () {
        $("#ddlState").trigger("change");


    });
    $(function () {
        $("#ddlDistricts").trigger("change");


    });

    if ($("#PMGSYSCHEME").val() == 2) {
        $('#btnHabStatus').attr("title", "Click Here To Change Habitation Status As Per Census 2011");
    }



    $("#ddlDistricts").val($("#ddlDistricts option:first").val());

    $("#ddlDistricts").change(function () {
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading blocks...</label> ' });
        var val = $("#ddlDistricts").val();
        $.ajax({
            type: 'POST',
            url: "/HabitationConnectivity/GetAllBlocksByDistrict?districtCode=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlBlocks").empty();
                $.each(data, function () {
                    $("#ddlBlocks").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");

                });

                $.unblockUI();
            }

        });


    });


    //$("#ddlState").change(function () {
    //    $.blockUI({ message: '<h4><label style="font-weight:normal">loading districts...</label> ' });
    //    var val = $("#ddlState").val();
    //    $.ajax({
    //        type: 'POST',
    //        url: "/StateLogin/GetAllDistrictsByState?stateCode=" + val,
    //        async: false,
    //        success: function (data) {
    //            $.unblockUI();
    //            $("#ddlDistricts").empty();
    //            $.each(data, function () {
    //                $("#ddlDistricts").append("<option value=" + this.Value + ">" +
    //                                                        this.Text + "</option>");

    //       });
    //             $("#ddlDistricts").trigger("change");
    //            $.unblockUI();
    //        }

    //    });


    //});





    $("#ddlBlocks").change(function () {

        // $("#btnSearch").trigger("click");
        $('#accordion').hide('slow');
        $('#tbHabitationList').jqGrid("setGridState", "visible");
        $("#tbStateLoginList").jqGrid('GridUnload');


    });
    $("#ddlDistricts").change(function () {

        $('#accordion').hide('slow');
        $('#tbHabitationList').jqGrid("setGridState", "visible");
        $("#tbStateLoginList").jqGrid('GridUnload');

    });

    $('#btnSearch').click(function (e) {

        if ($("#frmSearchState").valid()) {
            SearchHabitationDetails();
        }

    });

    $("#btnNotFeasible").click(function () {
        var HabCode = $("#tbStateLoginList").jqGrid('getGridParam', 'selarrrow');
        var habStatus = "NF"
        if (HabCode == "") {
            alert("Please select Habitation.");
        }
        else {
            if (confirm("Are you sure want to change habitation status.")) {
                $.ajax({
                    type: 'POST',
                    url: "/HabitationConnectivity/ChangeStatusOfHabitation?HabCode=" + HabCode,
                    data: { "Status": habStatus },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            alert(data.message);
                            $('#tbStateLoginList').trigger("reloadGrid");
                            $('#tbHabitationList').trigger("reloadGrid");
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }


    });

    $("#btnStateConnected").click(function () {
        var HabCode = $("#tbStateLoginList").jqGrid('getGridParam', 'selarrrow');
        var habStatus = "SC"
        if (HabCode == "") {
            alert("Please select Habitation.");
        }
        else {
            if (confirm("Are you sure want to change habitation status.")) {
                $.ajax({
                    type: 'POST',
                    url: "/HabitationConnectivity/ChangeStatusOfHabitation?HabCode=" + HabCode,
                    data: { "Status": habStatus },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            alert(data.message);
                            $('#tbStateLoginList').trigger("reloadGrid");
                            $('#tbHabitationList').trigger("reloadGrid");
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }

    });

    $("#btnUnconnected").click(function () {
        var HabCode = $("#tbStateLoginList").jqGrid('getGridParam', 'selarrrow');
        var habStatus = "UC"
        if (HabCode == "") {
            alert("Please select Habitation.");
        }
        else {
            if (confirm("Are you sure want to change habitation status.")) {
                $.ajax({
                    type: 'POST',
                    url: "/HabitationConnectivity/ChangeStatusOfHabitation?HabCode=" + HabCode,
                    data: { "Status": habStatus },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            alert(data.message);
                            $('#tbStateLoginList').trigger("reloadGrid");
                            $('#tbHabitationList').trigger("reloadGrid");
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }

    });


    $("#btnHabStatus").click(function () {

        var HabCode = $("#tbExistHabsList").jqGrid('getGridParam', 'selarrrow');
        var habStatus = "ASPERSENSUS"
        if (HabCode == "") {
            alert("Please select Habitation.");
        }
        else {
            if (confirm("Are you sure want to change habitation status.")) {
                $.ajax({
                    type: 'POST',
                    url: "/HabitationConnectivity/ChangeStatusAsPerCensusYearOfHabitation?HabCode=" + HabCode,
                    data: { "Status": habStatus },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            alert(data.message);
                            $('#tbExistHabsList').trigger("reloadGrid");
                            $('#tbHabitationList').trigger("reloadGrid");
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }

    });
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#dvSearchParameter").slideToggle(300);
        }

    });




});



function LoadHabitationGrid(BlockCode) {

    $('#tbHabitationList').jqGrid({

        url: '/HabitationConnectivity/GetHabitationDetails',
        postData: { "Blocks": BlockCode },
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Habitation", "Considered in Proposal", "Connected", "UnConnected", "UnConnected", "Not Feasible", "State Connected", "Considered In Proposal", "Balance for Change Status"],
        colModel: [
                            { name: 'Block', index: 'Block', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'TotalHabitation', index: 'TotalHabitation', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'TotalProposal', index: 'TotalProposal', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'Connected', index: 'Connected', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'UnConnected', index: 'UnConnected', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'NotConnected', index: 'NotConnected', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'NotFeasible', index: 'NotFeasible', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'StateConnected', index: 'StateConnected', height: 'auto', width: '80', align: "center", sortable: false, },
                            { name: 'Benifited', index: 'Benifited', height: 'auto', width: '60', align: "center", sortable: false, },
                            { name: 'Balance', index: 'Balance', height: 'auto', width: '60', align: "center", sortable: false, },

        ],
        pager: jQuery('#dvHabitationPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        autowidth: true,
        cmTemplate: { title: false },

        rownumbers: true,

        hidegrid: true,
        loadComplete: function () {
            $("#dvHabitationPager_left").html('[<b> Note</b>:Click on link to get details or to change the status of habitation. ]');

        },


    });

    $("#tbHabitationList").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
              { startColumnName: 'TotalHabitation', numberOfColumns: 2, titleText: '<em> Total </em>' },
              { startColumnName: 'Connected', numberOfColumns: 2, titleText: '<em>(As per Census ' + ($("#PMGSYSCHEME").val() == 1 ? 2001 : 2011) + ' )' + ' </em>' },
              { startColumnName: 'NotConnected', numberOfColumns: 5, titleText: '<em> Current status of UnConnectd Habitation </em>' }


        ]
    });
}

function GetTotalHabsDetails(parameter) {
    $("#btnHabStatus").show();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Total Habitation Details");

    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({
        //url: '/HabitationConnectivity/GetTotalHabsDetails',
        //postData: { "Blocks": parameter },
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "TH" },
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population", "Status"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '80', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '80', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '70', align: "center", sortable: true },
                            { name: 'HabStatus', index: 'HabStatus', height: 'auto', width: '70', align: "left", sortable: true },
        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,
        hidegrid: false,
        pginput: true,
        multiselect: true,
        loadComplete: function () {
            $('#dvExistHabsListPager_left').html('[<b> Note</b>:Select checkbox to change habitation status. ]');
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function LoadHabitationStatusGrid() {


    $('#tbStateLoginList').jqGrid({

        url: '/HabitationConnectivity/GetHabitationList',
        postData: { "Districts": $("#ddlDistricts").val(), "Blocks": $("#ddlBlocks").val() },
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation Name", "Total Population", "SC/ST Population", "Status"],
        colModel: [
                            { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'Status', index: 'Status', height: 'auto', width: '80', align: "left", sortable: true },

        ],
        pager: jQuery('#dvStateLoginPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'HabitationName',
        sortorder: "asc",
        caption: "Habitation List",
        height: '100%',
        width: 1065,
        rownumbers: true,
        pginput: true,
        hidegrid: false,
        multiselect: true,
        loadComplete: function () {
            $('#dvStateLoginPager_left').html('[<b> Note</b>:Select checkbox to change habitation status. ]');
            $("#gview_tbStateLoginList > .ui-jqgrid-titlebar").hide();
        }
    });




}



//New
function GetTotalProposalDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Total Habitation Prposal Details");

    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "BH" },
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '80', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '80', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '70', align: "center", sortable: true },
                           // { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },

        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function GetBenifitedHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Benifited Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        //url: '/HabitationConnectivity/GetBenifitedHabsDetails',
        //postData: { "Blocks": parameter }, //BH
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "UB" },
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                          //  { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },
        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }



    });
}



function GetConnectedHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Connected Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        //url: '/HabitationConnectivity/GetConnectedHabsDetails',
        //postData: { "Blocks": parameter },
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "HC" },

        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                          //  { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },

        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

//New
function GetUnConnectedHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("UnConnected Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "TU" },
        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                          //  { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },

        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function GetNotConnectHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Not Connected Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        //url: '/HabitationConnectivity/GetNotConnectedHabsDetails',
        //postData: { "Blocks": parameter },
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "NC" },

        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                          //  { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },

        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {
            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function GetNotFeasibleHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("Not Feasible Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        //url: '/HabitationConnectivity/GetNotFeasibleHabsDetails',
        //postData: { "Blocks": parameter },
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "IF" },

        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                           // { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },

        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {

            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function GetStateConnectedHabsDetails(parameter) {
    $("#btnHabStatus").hide();
    $("#tabs").tabs({ active: 0 });
    $("#spnHabsTitle").html("State Connected Habitation Details");
    LoadHabitationStatusGrid();
    $('#tbHabitationList').jqGrid("setGridState", "hidden");
    $("#tbExistHabsList").jqGrid('GridUnload');
    $('#accordion').show('slow');
    $('#tbExistHabsList').jqGrid({

        //url: '/HabitationConnectivity/GetStateConnectedHabsDetails',
        //postData: { "Blocks": parameter },
        url: '/HabitationConnectivity/GetHabsDetailsByStatus',
        postData: { "Blocks": parameter, "HabStatus": "SC" },

        datatype: "json",
        mtype: "POST",
        colNames: ["Habitation", "Village", "Total Population", "SC/ST Population", "Remove State Connected"],
        colModel: [
                            { name: 'Habitation', index: 'Block', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'Village', index: 'Village', height: 'auto', width: '100', align: "left", sortable: true },
                            { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                            { name: 'SC/STPopulation', index: 'STPopulation', height: 'auto', width: '80', align: "center", sortable: true },
                          //  { name: 'Status', index: 'Status', height: 'auto', width: '70', align: "left", sortable: true },
                            { name: 'Remove Connected', index: 'RemoveConnected', height: 'auto', width: '70', align: "left", sortable: true },
        ],
        pager: jQuery('#dvExistHabsListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Block',
        sortorder: "asc",
        caption: "Habitation Details",
        height: '100%',
        width: 1065,
        rownumbers: true,

        hidegrid: false,
        loadComplete: function () {

            $("#gview_tbExistHabsList > .ui-jqgrid-titlebar").hide();

        }

    });
}

function SearchHabitationDetails() {

    $('#tbHabitationList').jqGrid('GridUnload');
    LoadHabitationGrid($("#ddlBlocks").val());

}

function CloseHabitationDetails() {

    if ($("#accordion").is(":visible")) {
        $('#accordion').hide('slow');
    }

    $('#tbHabitationList').jqGrid("setGridState", "visible");
}

function GetBalancedHabsDetails() {
    $("#tabs").tabs({ active: 1 });
    $("#spnHabsTitle").html("Total Habitation Details");

    LoadHabitationStatusGrid();
    $('#accordion').show('slow');
}


function EditStatus(urlParam) {
    //alert(urlParam);
    $.ajax({
        type: 'POST',
        url: '/HabitationConnectivity/EditStatus/' + urlParam,
        async: false,
        //data: urlParam,
        success: function (data) {
            alert(data.message);
            $("#tbHabitationList").trigger("reloadGrid");
            $("#tbExistHabsList").trigger("reloadGrid");
            if (data.success === undefined) {

                //$("#divAddForm").html(data)
            } else if (data.success == true) {
            }
            else {
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.responseText);
        }
    })
}