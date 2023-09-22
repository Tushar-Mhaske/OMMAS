$(document).ready(function () {
    $("#tabs").tabs();
    LoadHabitationGrid();
    var road = $("#PLAN_CN_ROAD_CODE").val();
    //ListFiles(road);
    LoadProposalList(road);
    LoadMappedRoadsListDetails(road);
    // Finalize button click
    $("#btnFinalize").click(function () {

        $.ajax({
            url: '/CoreNetwork/GetCoreNetworkChecks',
            type: "POST",
            datatype: "Json",
            cache: false,
            async: false,
            beforeSend: function () {
                blockPage();
            },
            data: { PLAN_CN_ROAD_CODE: $("#PLAN_CN_ROAD_CODE").val() },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!" + xhr.responseText);
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Success) {
                    if (confirm("Once Core Network is Finalized, it can not be Edited and Deleted.\nAre you sure to Finalize it ?")) {
                        $.ajax({
                            url: '/CoreNetwork/FinalizeCoreNetwork',
                            type: "POST",
                            cache: false,
                            beforeSend: function () {
                                blockPage();
                            },
                            data: { PLAN_CN_ROAD_CODE: $("#PLAN_CN_ROAD_CODE").val() },
                            error: function (xhr, status, error) {
                                unblockPage();
                                Alert("Request can not be processed at this time,please try after some time!!!");
                                return false;
                            },
                            success: function (response) {
                                if (response.Success) {
                                    alert("Finalized successfully...");
                                    $("#networkCategory").trigger('reloadGrid');;
                                    $("#btnFinalize").hide();                                    
                                }
                                else {
                                    alert("Error occured while finalize the record...");
                                }
                                
                                unblockPage();
                                
                            }
                        });

                    } else {
                        return false;
                    }
                }
                else {

                    alert(response.ErrorMessage);
                }
            }
        });
    });


});

// Habitation List
function LoadHabitationGrid() {


    jQuery("#habitationDetailsCategory").jqGrid({
        url: '/CoreNetwork/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { habCode: $('#PLAN_CN_ROAD_CODE').val() },
        colNames: ['Habitation System Id', 'Name of Habitation', 'Block', 'Village Name', 'Road Number', 'Total Population', 'SC/ST Population', 'Delete'], //'SC/ST Population', 'Primary School', 'Middle Schools', 'High Schools', 'Intermediate Schools', 'Degree College', 'Health Services','Dispensaries'],//,'MCW Centers','PHCS','Vetarnary Hospitals','Telegraph Office','Telephone Connections','Bus Service','Railway Stations','Electricity','Panchayat Head Quarters','Tourist Place'],
        colModel: [
            ///Changes by SAMMED A. PATIL on 21JULY2017 to display Habitation Code in mapped Habitation List
            { name: 'habitationCode', index: 'habitationCode', width: 100, sortable: true, align: "center" },
            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 300, align: "center", sortable: true },
            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 250, align: "center", sortable: false },
            { name: 'VILLAGE_NAME', index: 'VILLAGE_NAME', width: 250, sortable: false, align: "center", hidden: true },
            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', width: 250, sortable: false, align: "center", hidden: true },
            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 200, sortable: false, align: "center" },
            { name: 'MAST_HAB_SCST_POP', index: 'MAST_HAB_SCST_POP', width: 200, sortable: false, align: "center" },
            { name: 'a', index: 'a', formatter: FormatColumn, width: 50, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#pagerDetailsHabitation').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Habitation List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_habitationDetailsCategory > .ui-jqgrid-titlebar").hide();
            TotHabPop = $(this).jqGrid("getCol", "MAST_HAB_TOT_POP", false, "sum"),
                TotScStPop = $(this).jqGrid("getCol", "MAST_HAB_SCST_POP", false, "sum"),

                $(this).jqGrid("footerData", "set",
                    {
                        MAST_HAB_NAME: "Grand Total Population:",
                        MAST_HAB_TOT_POP: TotHabPop,
                        MAST_HAB_SCST_POP: TotScStPop

                    });

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

// Uploaded File List
function ListFiles(PLAN_CN_ROAD_CODE) {

    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/CoreNetWork/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["File Name", "Start Chainage(in Kms)", "End Chainage(in KMs)", "Upload Date", "Edit", "Delete", "Save"],
        colModel: [
            { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
            { name: 'PLAN_START_CHAINAGE', index: 'PLAN_START_CHAINAGE Chainage', sortable: false, align: "center", search: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateStartChainage }, width: 200 },
            { name: 'PLAN_END_CHAINAGE', index: 'PLAN_END_CHAINAGE', sortable: false, align: "center", search: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateEndChainage }, width: 200 },
            { name: 'PLAN_UPLOAD_DATE', index: 'PLAN_UPLOAD_DATE Date', width: 200, sortable: false, align: "center", search: false, editable: false },
            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
            { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "PLAN_CN_ROAD_CODE": PLAN_CN_ROAD_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowNum: 4,
        sortname: 'PDF',
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        rownumbers: true,
        editurl: "/CoreNetWork/UpdatePDFDetails",
        loadComplete: function () {

            $("#gview_tbPDFFilesList > .ui-jqgrid-titlebar").hide();
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                unblockPage();
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
            unblockPage();
        }
    }); //end of grid    
}
function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/CoreNetWork/DownloadFile/" + cellvalue;

    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/FileIcon.png' /> </a>";
}
function ValidateStartChainage(value, colname) {
    if (!value.match("^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$")) {
        return [" Invalid Start Chainage,Only Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function ValidateEndChainage(value, colname) {
    if (!value.match("^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$")) {
        return [" Invalid Start Chainage,Only Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}
function DownloadFile(paramFileName) {
    $.ajax({
        url: '/CoreNetWork/DownloadFile',
        type: 'GET',
        beforeSend: function () {
            blockPage();
        },
        data: { PLAN_FILE_NAME: paramFileName, value: Math.random() },
        success: function (response) {
            unblockPage();
            if (response.Success) {

            }
        },
        error: function (xhr, AjaxOptions, thrownError) {
            alert("Error occured while processing the request.");
            unblockPage();
        }
    });
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}
function LoadProposalList(PLAN_CN_ROAD_CODE) {
    blockPage();
    jQuery("#tbProposalList").jqGrid({
        url: '/CoreNetWork/ListProposalByCoreNetwork',
        datatype: "json",
        mtype: "GET",
        colNames: ["Proposal Type", "Road / Bridge Name", "Year", "Batch", "Package", "New/Upgrade", "Road / Bridge Cost", "Road / Bridge Status", "Pavement Length"],
        colModel: [
            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', width: 100, sortable: false, align: "center", search: false, editable: false },
            { name: 'WORK_NAME', index: 'WORK_NAME', sortable: false, align: "center", search: false, editable: false, width: 300 },
            { name: 'IMS_YEAR', index: 'IMS_YEAR', sortable: false, align: "center", search: false, editable: false, width: 100 },
            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 100, sortable: false, align: "center", search: false, editable: false },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 100, sortable: false, align: "center", editable: false, hidden: false },
            { name: 'IMS_UPGRADE_CONNECT', index: 'IMS_UPGRADE_CONNECT', width: 80, sortable: false, align: "center", editable: false, hidden: false },
            { name: 'WORK_COST', index: 'WORK_COST', width: 100, sortable: false, align: "center", editable: false, hidden: false },
            { name: 'IMS_ISCOMPLETED', index: 'IMS_ISCOMPLETED', width: 100, sortable: false, align: "center", editable: false, hidden: false },
            { name: 'PAV_LENGTH', index: 'PAV_LENGTH', width: 100, sortable: false, align: "center", editable: false, hidden: false }
        ],
        postData: { "RoadCode": PLAN_CN_ROAD_CODE },
        pager: jQuery('#dvProposalListPager'),
        rowNum: 4,
        sortname: 'WORK_NAME',
        sortorder: "asc",
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Proposal Details",
        height: 'auto',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //$("#gview_tbProposalList > .ui-jqgrid-titlebar").hide();

            TotLength = $(this).jqGrid("getCol", "PAV_LENGTH", false, "sum"),

                $(this).jqGrid("footerData", "set",
                    {
                        IMS_ISCOMPLETED: "Grand Total :",
                        PAV_LENGTH: TotLength,
                    });

            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                unblockPage();
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
            unblockPage();
        }
    }); //end of grid    
}

function LoadMappedRoadsListDetails(PLAN_CN_ROAD_CODE) {
    $("#tbMappedDRRPList").jqGrid('GridUnload');
    jQuery("#tbMappedDRRPList").jqGrid({
        //url: '/CoreNetwork/GetMappedCandidateRoadListPMGSY3',
        url: '/CoreNetwork/GetMappedCandidateRoadList',
        datatype: "json",
        mtype: "GET",
        postData: { RoadCode: PLAN_CN_ROAD_CODE },
        colNames: ['Block', 'Category', 'Road Name', 'Length (in Kms)', 'Partial/Full'],//, 'Action'],
        colModel: [
            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "left", search: false },
            { name: 'MAST_ROAD_CAT_NAME', index: 'MAST_ROAD_CAT_NAME', height: 'auto', width: 250, align: "left", search: true },
            { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', height: 'auto', width: 350, align: "left", search: false },
            { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', height: 'auto', width: 125, align: "right", search: false },
            { name: 'PLAN_RD_LENG', index: 'PLAN_RD_LENG', height: 'auto', width: 125, align: "center", search: false },
            //{ name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
        ],
        pager: jQuery('#dvpgrMappedDRRPList').width(20),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        caption: "&nbsp;&nbsp; Mapped DRRP Road List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
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