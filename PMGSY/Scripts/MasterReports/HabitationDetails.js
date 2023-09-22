﻿$(function () {
    //$('#tblRptContents').bind('resize', function () {
    //    resizeJqGrid();
    //}).trigger('resize');
    if ($("#MAST_STATE_CODE").val() > 0) {
        $("#StateList_HabitationDetails").attr("disabled", "disabled");
    }
 
    if ($("#MAST_DISTRICT_CODE").val() > 0) {

        $("#DistrictList_HabitationDetails").val($("#MAST_DISTRICT_CODE").val());
        $("#DistrictList_HabitationDetails").attr("disabled", "disabled");
    }
});

$("#StateList_HabitationDetails").change(function () {


    alert("Click")


    $("#DistrictList_HabitationDetails").val(0);
    $("#DistrictList_HabitationDetails").empty();
    //$("#DistrictList").append("<option value='0'>Select District</option>");
    if ($(this).val() > 0) {
        if ($("#DistrictList_HabitationDetails").length > 0) {
            $.ajax({
                url: '/MasterReports/HabitationDetails',
                type: 'POST',
                data: { "StateCode": $("#StateList_HabitationDetails").val() },
                success: function (jsonData) {

                    alert("sucess")
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    if ($("#MAST_DISTRICT_CODE").val() > 0) {
                       

                        alert(" sucess sucess")

                        $("#DistrictList_HabitationDetails").val($("#MAST_DISTRICT_CODE").val());
                        $("#DistrictList_HabitationDetails").attr("disabled", "disabled");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {
        $("#DistrictList_HabitationDetails").append("<option value='0'>Select District</option>");
        $("#BlockList_HabitationDetails").val(0);
        $("#BlockList_HabitationDetails").empty();
        $("#BlockList_HabitationDetails").append("<option value='0'>Select Block</option>");
        $("#VillageList_HabitationDetails").val(0);
        $("#VillageList_HabitationDetails").empty();
        $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
    }
});

$("#DistrictList_HabitationDetails").change(function () {
    $("#BlockList_HabitationDetails").val(0);
    $("#BlockList_HabitationDetails").empty();
    //$("#DistrictList").append("<option value='0'>Select District</option>");
    if ($(this).val() > 0) {
        if ($("#BlockList_HabitationDetails").length > 0) {
            $.ajax({
                url: '/MasterReports/HabitationDetails',
                type: 'POST',
                data: { "StateCode": $("#StateList_HabitationDetails").val(), "DistrictCode": $("#DistrictList_HabitationDetails").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {
        $("#BlockList_HabitationDetails").val(0);
        $("#BlockList_HabitationDetails").empty();
        $("#BlockList_HabitationDetails").append("<option value='0'>Select Block</option>");
        $("#VillageList_HabitationDetails").val(0);
        $("#VillageList_HabitationDetails").empty();
        $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
    }
});

$("#BlockList_HabitationDetails").change(function () {
    $("#VillageList_HabitationDetails").val(0);
    $("#VillageList_HabitationDetails").empty();
    //$("#DistrictList").append("<option value='0'>Select District</option>");
    if ($(this).val() > 0) {
        if ($("#VillageList_HabitationDetails").length > 0) {
            $.ajax({
                url: '/MasterReports/HabitationDetails',
                type: 'POST',
                data: { "StateCode": $("#StateList_HabitationDetails").val(), "DistrictCode": $("#DistrictList_HabitationDetails").val(), "BlockCode": $("#BlockList_HabitationDetails").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#VillageList_HabitationDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {
        $("#VillageList_HabitationDetails").val(0);
        $("#VillageList_HabitationDetails").empty();
        $("#VillageList_HabitationDetails").append("<option value='0'>Select Village</option>");
    }
});


$("#HabitationDetailsButton").click(function () {
    var stateCode = $("#StateList_HabitationDetails").val();
    var districtCode = $("#DistrictList_HabitationDetails").val();
    var blockCode = $("#BlockList_HabitationDetails").val();
    var villageCode = $("#VillageList_HabitationDetails").val();
    var activeType = $("#ActiveType_HabitationDetails").val();


    if (stateCode == 0)
    {
        alert("Please Select State");
        return;
    }
    if (districtCode == 0)
    {
        alert("Please Select District");
        return;
    }

    if (blockCode == 0)
    {
        alert("Please Select Block");
        return;
    }

    var censusYear = $("#CENSUS_YEAR_HabitationDetails").val();
    var habitationStatus = $("#HAB_STATUS_HabitationDetails").val();
    var isSchedule = $("#IS_SCHEDULE5_HabitationDetails").val();

    if(censusYear=='2001')
        Habitation2001MasterReportsListing(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule, activeType);
    else
        Habitation2011MasterReportsListing(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule, activeType);

});


function Habitation2001MasterReportsListing(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule, activeType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HabitationDetailsTable").jqGrid('GridUnload');

    $("#HabitationDetailsTable").jqGrid({
        url: '/MasterReports/HabitationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State','District','Block','Village','Habitation', 'MLA Constituency', 'MP Constituency',
            'Habitation Total Population', 'SC\ST Population', 'Habitation Status', 'Habitation Connected as per '+ censusYear+' Census Year', 'Scheme', 'Primary School', 'Middle School', 'High School', 'Intermediate School', 'Degree College',
            'Health Service', 'Dispensary', 'MCW Centers', 'PHCS', 'Vetenary Hospital', 'Telegraph Office', 'Telephone Connection', 'Bus Service', 'Railway Service',
            'Electricity', 'Panchayat Headquarters', 'Tourist Place','Active'],
        colModel: [
                { name: 'MAST_STATE_NAME', width: 150, align: 'left', height: 'auto' },
                { name: 'MAST_DISTRICT_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_BLOCK_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_VILLAGE_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_NAME', width:100, align: 'left', height: 'auto' },
                { name: 'MAST_MLA_CONST_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_MP_CONST_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_TOT_POP', width: 70, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                { name: 'MAST_HAB_SCST_POP', width: 70, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                { name: 'MAST_HAB_STATUS', width: 80, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_CONNECTED', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_SCHEME', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_PRIMARY_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_MIDDLE_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_HIGH_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_INTERMEDIATE_SCHOOL', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_DEGREE_COLLEGE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_HEALTH_SERVICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_DISPENSARY', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_MCW_CENTERS', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_PHCS', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_VETNARY_HOSPITAL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_TELEGRAPH_OFFICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_TELEPHONE_CONNECTION', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_BUS_SERVICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_RAILWAY_STATION', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_ELECTRICTY', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_PANCHAYAT_HQ', width: 80, align: 'left', height: 'auto' },
                { name: 'MAST_TOURIST_PLACE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_HABITATION_ACTIVE', width: 50, align: 'left', height: 'auto' }
              
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "VillageCode": villageCode, "HAB_STATUS": habitationStatus, "IS_SCHEDULE5": isSchedule, "CENSUS_YEAR": censusYear ,"ActiveType":activeType},
        pager: $("#HabitationDetailsPager"),
        pgbuttons: true,
        footerrow : true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
     //   recordtext: "View {0} - {1} of {2}",
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '500',
        width: 1120,
        shrinkToFit:false,
        viewrecords: true,
        caption: 'Habitation Reports',
        loadComplete: function () {
            //var count = $("#HabitationDetailsTable").getRowData().length;
            //if (count == 0) {
            //    alert(count);
            //   // $(this).setGridParam('pginput', false);
            //    //$(this).jqgrid({
            //    //    rowlist: [],
            //    //    pgbuttons: false,
            //    //    pgtext: "",
            //    //    viewrecords: false,
            //    //    pginput:false

            //    //});              
    
            //    $("#HabitationDetailsPager").hide();
            //}
            //else {
            //     $("#HabitationDetailsPager").show();
               
            //}
            var TotalCountT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
            var SCSTPopulationT = $(this).jqGrid('getCol', 'MAST_HAB_SCST_POP', false, 'sum');
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: TotalCountT }, true);
            $(this).jqGrid('footerData', 'set', { MAST_HAB_SCST_POP: SCSTPopulationT }, true);
            $('#HabitationDetailsTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}

function Habitation2011MasterReportsListing(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule, activeType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#HabitationDetailsTable").jqGrid('GridUnload');

    $("#HabitationDetailsTable").jqGrid({
        url: '/MasterReports/HabitationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State', 'District', 'Block', 'Village', 'Habitation', 'MLA Constituency', 'MP Constituency',
            'Habitation Total Population', 'SC\ST Population', 'Habitation Status', 'Habitation Connected as per ' + censusYear + ' Census Year', 'Scheme', 'Primary School', 'Middle School', 'High School', 'Intermediate School', 'Degree College',
            'Health Service', 'Dispensary', 'MCW Centers', 'PHCS', 'Vetenary Hospital', 'Telegraph Office', 'Telephone Connection', 'Bus Service', 'Railway Service',
            'Electricity', 'Panchayat Headquarters', 'Tourist Place', 'Electric Sub Station above 11 KVA', 'Tehsil / Block Headquarter', 'Sub Tehsil', 'One Diesel / Petrol Authorized Outlet', 'Additional Authorized Diesel Outlet', 'Mandi (based on Turnover)', 'Warehouse / Cold Storage', 'Retail Shops Selling','Active'],
        colModel: [
                { name: 'MAST_STATE_NAME', width: 150, align: 'left', height: 'auto' },
                { name: 'MAST_DISTRICT_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_BLOCK_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_VILLAGE_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_MLA_CONST_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_MP_CONST_NAME', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_TOT_POP', width: 70, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                { name: 'MAST_HAB_SCST_POP', width: 70, align: 'right', height: 'auto', formatter: 'integer', formatoptions: { thousandsSeparator: ",", defaulValue: "N.A" } },
                { name: 'MAST_HAB_STATUS', width: 80, align: 'left', height: 'auto' },
                { name: 'MAST_HAB_CONNECTED', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_SCHEME', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_PRIMARY_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_MIDDLE_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_HIGH_SCHOOL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_INTERMEDIATE_SCHOOL', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_DEGREE_COLLEGE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_HEALTH_SERVICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_DISPENSARY', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_MCW_CENTERS', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_PHCS', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_VETNARY_HOSPITAL', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_TELEGRAPH_OFFICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_TELEPHONE_CONNECTION', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_BUS_SERVICE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_RAILWAY_STATION', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_ELECTRICTY', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_PANCHAYAT_HQ', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_TOURIST_PLACE', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_ELECTRICITY_ADD', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_BLOCK_HQ', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_SUB_TEHSIL', width: 50, align: 'left', height: 'auto' },
                { name: 'MAST_PETROL_PUMP', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_PUMP_ADD', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_MANDI', width: 100, align: 'left', height: 'auto' },
                { name: 'MAST_WAREHOUSE', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_RETAIL_SHOP', width: 70, align: 'left', height: 'auto' },
                { name: 'MAST_HABITATION_ACTIVE', width: 150, align: 'left', height: 'auto' },
           
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "VillageCode": villageCode, "HAB_STATUS": habitationStatus, "IS_SCHEDULE5": isSchedule, "CENSUS_YEAR": censusYear },
        pager: $("#HabitationDetailsPager"),
        pgbuttons: true,
        footerrow: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        //   recordtext: "View {0} - {1} of {2}",
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: false,
        height: '500',
        width: 1120,
        shrinkToFit: false,
        viewrecords: true,
        caption: 'Habitation Reports',
        loadComplete: function () {
            //var count = $("#HabitationDetailsTable").getRowData().length;
            //if (count == 0) {
            //    alert(count);
            //   // $(this).setGridParam('pginput', false);
            //    //$(this).jqgrid({
            //    //    rowlist: [],
            //    //    pgbuttons: false,
            //    //    pgtext: "",
            //    //    viewrecords: false,
            //    //    pginput:false

            //    //});              

            //    $("#HabitationDetailsPager").hide();
            //}
            //else {
            //     $("#HabitationDetailsPager").show();

            //}
            var TotalCountT = $(this).jqGrid('getCol', 'MAST_HAB_TOT_POP', false, 'sum');
            var SCSTPopulationT = $(this).jqGrid('getCol', 'MAST_HAB_SCST_POP', false, 'sum');
            $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { MAST_HAB_TOT_POP: TotalCountT }, true);
            $(this).jqGrid('footerData', 'set', { MAST_HAB_SCST_POP: SCSTPopulationT }, true);
            $('#HabitationDetailsTable_rn').html('Sr.<br/>No.');

            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}