
$(document).ready(function () {

    $("#tabs").tabs();
  
    
    GetTrafficIntensity($("#MAST_ER_ROAD_CODE").val());

    GetCBRValues($("#MAST_ER_ROAD_CODE").val());

    GetSurfaceDetails($("#MAST_ER_ROAD_CODE").val());

    GetHabitationDetails($("#MAST_ER_ROAD_CODE").val());
    
    GetCdWorksDetails($("#MAST_ER_ROAD_CODE").val());
    
    LoadCoreNetwork($("#MAST_ER_ROAD_CODE").val());

    $("#btnFinalizeExistingRoad").click(function () {
        
        $.ajax({
            url: '/ExistingRoads/GetExistingRoadChecks/',
            type: 'POST',
            dataType: 'Json',
            catche: false,
            async: false,
            beforeSend: function () {
                blockPage();
            },
            data: { MAST_ER_ROAD_CODE: $("#MAST_ER_ROAD_CODE").val() },
            error: function (xhr, status, error) {
                unblockPage();
                alert("Request Can not be processed at this time, Please try after some time!!!" + xhr.responseText);
                return false;
            },
            success: function (response)
            {
                unblockPage();
                if (response.Success) {
                    if (confirm("Once Existing Road is Finalize, it can not be Edited and Deleted.\nAre you sure to finalize it ?")) {
                        $.ajax({
                            url: '/ExistingRoads/FinalizeExistingRoad/',
                            type: 'POST',
                            catche: false,
                            beforeSend: function () {
                                blockPage();
                            },
                            data: { MAST_ER_ROAD_CODE: $("#MAST_ER_ROAD_CODE").val() },
                            error: function (xhr, status, error) {
                                unblockPage();
                                alert("Request Can not be processed at this time, Please try after some time!!!" + xhr.responseText);
                                return false;
                            },
                            success: function (response) {
                                unblockPage();

                             
                                $("#btnFinalizeExistingRoad").hide();
                                if (response.Success) {
                                    alert("Existing Road Details Finalized Successfully.");
                                }
                                else {
                                    alert(response.ErrorMessage);
                                }
                                $("#tbExistingRoadsList").trigger("reloadGrid");

                            },
                        });//end of finalize ajax call
                    } else {
                        return false;
                    }
                }
                else {
                    alert(response.ErrorMessage);
                }
            }
        });//end of Existing Road Details check ajax call


    }); //end of  btnFinalizeExistingRoad

});//end of Document.Ready()

function GetHabitationDetails(MAST_ER_ROAD_CODE) {
    

    jQuery("#tbHabitation").jqGrid({

        url: '/ExistingRoads/GetHabitationList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Habitation System Id','Benifited Habitation Name', 'Village Name', 'SC/ST Population', 'Total Population', "Edit", "Delete"],
            colModel: [
                                ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
                                { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
                                { name: 'habitationName', index: 'habitationName', width: 300, sortable: true, align: "center" },
                                { name: 'villageName', index: 'villageName', width: 300, sortable: true, align: "center" },
                                { name: 'totalSCSTPopulation', index: 'totalSCSTPopulation', width: 300, sortable: true, align: "center" },
                                { name: 'totalPopulation', index: 'totalPopulation', width: 300, sortable: true, align: "center" },
                                { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden: true },
                                { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden: true }
            ],
            pager: jQuery('#dvHabitationPager'),
            rowNum: 8,
            postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE},
            //altRows: true,        
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "Habitation Details",
            height: 'auto',
            sortname: 'habitationName',
            sortorder:'asc',
            autowidth:true,
            rownumbers: true,
            footerrow:true,
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    // alert(xhr.responseText);
                    // alert("Invalid data.Please check and Try again!")
                    //  window.location.href = "/Login/LogIn";
                }
            },
            loadComplete: function () {
                //$("#gview_tbHabitation > .ui-jqgrid-titlebar").hide();
                sum = $(this).jqGrid("getCol", "totalPopulation", false, "sum"),
                //$(this).jqGrid("footerData", "set", { habitationName: "Grand Total SC/ST Population:", totalSCSTPopulation: sum });
                $(this).jqGrid("footerData", "set", { totalSCSTPopulation: "Grand Total Population:", totalPopulation: sum });
                $("#tbHabitation").jqGrid('setLabel', "rn", "Sr.</br> No");

            }
        });
}
function doGrouping(isGrouping) {
    if (!isGrouping) {
        jQuery("#tbHabitation").jqGrid('groupingRemove');
    }
    else {
        jQuery("#tbHabitation").jqGrid('groupingGroupBy', "Cluster", {
            groupOrder: ['desc'],
            groupColumnShow: [false],
            groupCollapse: true
        });
    }
}
function GetTrafficIntensity(MAST_ER_ROAD_CODE) {

    jQuery("#tbTraffic").jqGrid({
        url: '/ExistingRoads/GetTrafficIntensityList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Year', 'Total Motarised Traffic/day', 'Commercial Vehicle Traffic/day', "Edit", "Delete"],
        colModel: [
                    { name: 'Year', index: 'Year', width: 335, sortable: true, align: "center" },
                    { name: 'TotalMotarisedTrafficday', index: 'TotalMotarisedTrafficday', width: 330, sortable: true, align: "center" },
                    { name: 'CommercialVehicleTrafficDay', index: 'CommercialVehicleTrafficDay', width: 340, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden:true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden:true }
        ],
        pager: jQuery('#dvTrafficPager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE},
        sortname: 'Year',
        sortorder: 'asc',
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Traffic Intensity Details",
        height: 'auto',
        //width: 'auto',
        autowidth:true,
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {                
                 alert("Invalid data.Please check and Try again!")                
            }
        },
        loadComplete: function () {
            $("#gview_tbTraffic > .ui-jqgrid-titlebar").hide();
            $("#tbTraffic").jqGrid('setLabel', "rn", "Sr.</br> No");
        }
    });
}

function GetCBRValues(MAST_ER_ROAD_CODE) {

    jQuery("#tbCBR").jqGrid({
        url: '/ExistingRoads/GetCBRList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Start Chainage (in Kms.)', 'End Chainage (in Kms.)', "Segment Length (in Kms.)", "CBR Value", "Edit", "Delete"],
        colModel: [
                     { name: 'StartChainage', index: 'StartChainage', width: 200, sortable: true, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 200, sortable: true, align: "center" },
                    { name: 'SegmentLength', index: 'SegmentLength', width: 200, sortable: false, align: "center", formatter: 'number', summaryType: 'sum' },
                    { name: 'CBRValue', index: 'CBRValue', width: 200, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden:true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" , hidden:true}
        ],
        pager: jQuery('#dvCBRPager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE},
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "CBR Details",
        height: 'auto',
        sortname: 'StartChainage',
        sortorder:'asc',
        autowidth:true,
        rownumbers: true,
        loadComplete: function () {

            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SegmentLength', false);
            var RoadLength = 0;
            for (i = 0 ; i < RoadLengthColumn.length; i++) {
                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
            }

        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        loadComplete: function () {
            $("#gview_tbCBR > .ui-jqgrid-titlebar").hide();
            $("#tbCBR").jqGrid('setLabel', "rn", "Sr.</br> No");
        }
    });

}

function GetSurfaceDetails(MAST_ER_ROAD_CODE) {
        jQuery("#tbSurfaceType").jqGrid({
        url: '/ExistingRoads/GetSurfaceTypeList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Surface Name', 'Start Chainage (in Kms.)', 'End Chainage (in Kms.)', "Road Condition", "Length (in Kms.)", "Edit", "Delete"],
        colModel: [
                    { name: 'SurfaceName', index: 'SurfaceName', width: 200, sortable: true, align: "left" },
                    { name: 'StartChainage', index: 'StartChainage', width: 200, sortable: true, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 200, sortable: true, align: "center" },
                    { name: 'SurfaceCondition', index: 'SurfaceCondition', width: 200, sortable: true, align: "left" },
//                  { name: 'SurfaceCondition', index: 'SurfaceCondition', width: 200, sortable: false, align: "center", formatter: 'number', summaryType: 'sum' },
                    { name: 'SurfaceLength', index: 'SurfaceLength', width: 200, sortable: true, align: "center", formatter: "number", summaryType: 'sum' },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#dvSurfaceTypePager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Surface Details",
        height: 'auto',
        sortname: 'SurfaceName',
        sortorder: 'asc',
        sutowidth: true,
        rownumbers: true,
        loadComplete: function () {

            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SurfaceLength', false);
            var RoadLength = 0;
            for (i = 0 ; i < RoadLengthColumn.length; i++) {
               
                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
            }

        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        loadComplete: function () {
            $("#gview_tbSurfaceType > .ui-jqgrid-titlebar").hide();
            $("#tbSurfaceType").jqGrid('setLabel', "rn", "Sr.</br> No");
        }
    });

}

function GetCdWorksDetails(MAST_ER_ROAD_CODE) {
    jQuery("#tbCdWorks").jqGrid({
        url: '/ExistingRoads/GetCdWorksList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['CD Works Type', 'CD Works Length (in Mtrs.)', "CD Works Discharge(in Cu ms.)", "CD Works Chainage(in Kms.)", "Construction Year", "Rehabilitation Year", "Span", "Carriage Way(in Mtrs)", "Foot Path", "Edit", "Delete"],
        colModel: [ 
                    { name: 'CDWorksType', index: 'CDWorksType', width: 150, sortable: true, align: "left" },
                    { name: 'CDWorksLength', index: 'CDWorksLength', width: 95, sortable: true, align: "center" },
                    { name: 'CDWorksDischarge', index: 'CDWorksDischarge', width: 90, sortable: true, align: "center" },
                    { name: 'CDWorksChainage', index: 'CDWorksChainage', width: 90, sortable: true, align: "center" },
                    { name: 'ConstructionYear', index: 'ConstructionYear', width: 90, sortable: true, align: "center" },
                    { name: 'RehabilitationYear', index: 'RehabilitationYear', width: 90, sortable: true, align: "center" },
                    { name: 'Span', index: 'CDWorksChainage', width: 90, sortable: true, align: "center" },
                    { name: 'CarriageWay', index: 'CarriageWay', width: 90, sortable: true, align: "center" },
                    { name: 'FootPath', index: 'FootPath', width: 90, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#dvCdWorksPager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "CD Works Details",
        height: 'auto',
        autowidth: true,
        sortname: 'CDWorksType',
        sortorder:'asc',
        sutowidth: true,
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }

        },
        loadComplete: function () {
            $("#gview_tbCdWorks > .ui-jqgrid-titlebar").hide();
            $("#tbCdWorks").jqGrid('setLabel', "rn", "Sr.</br> No");
        }
    });

}

function LoadCoreNetwork(RoadCode)
{
    jQuery("#tbCoreNetwork").jqGrid({
        url: '/ExistingRoads/ListCoreNetworkByDRRP',
        datatype: "json",
        mtype: "POST",
        postData: { MAST_ER_ROAD_CODE: RoadCode },
        colNames: ['Road No.', 'Road Name', 'Road From', 'Road To', 'Start Chainage [In Km]', 'End Chainage [In Km]', 'Length [In Km]'],
        colModel: [
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 60, align: "left", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 240, align: "left", search: true },
                            { name: 'PLAN_RD_FROM', index: 'PLAN_RD_FROM', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'PLAN_RD_TO', index: 'PLAN_RD_TO', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'PLAN_RD_FROM_CHAINAGE', index: 'PLAN_RD_FROM_CHAINAGE', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 90, align: "center", search: false },
                            { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', height: 'auto', width: 90, align: "center", search: false },
        ],
        pager: jQuery('#dvCoreNetwork').width(20),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "desc",
        sortname: 'PLAN_CN_ROAD_CODE',
        caption: "&nbsp;&nbsp; Core Network List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            $("#tbCoreNetwork").jqGrid('setLabel', "rn", "Sr.</br> No");
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