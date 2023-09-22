this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 100; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};

$(document).ready(function () {

    $("#tabs").tabs();
    $("#trTEchnicalDetails").removeClass("ui-state-hover");

    if ($("#hdnRoleCode").val() == "4") {

    }
    //   $("#txtScrutinyDate").datepicker();

    //ShowHabitations($("#IMS_PR_ROAD_CODE").val());

    GetTrafficIntensity($("#IMS_PR_ROAD_CODE").val());
    GetCBRValues($("#IMS_PR_ROAD_CODE").val());
    ListProposalFiles($("#IMS_PR_ROAD_CODE").val());
    ListPDFFiles($("#IMS_PR_ROAD_CODE").val());
    ListSTASRRDAPDFFiles($("#IMS_PR_ROAD_CODE").val());
    LoadTechnologyList($("#IMS_PR_ROAD_CODE").val());
    ListPDFFilesForest($("#encrProposalCode").val());
    $("#btnFinalize").click(function () {

        $.ajax({
            url: '/Proposal/GetProposalChecks',
            type: "POST",
            datatype: "Json",
            cache: false,
            async: false,
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), IMS_LOCK_STATUS: $("#IMS_LOCK_STATUS").val() },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!" + xhr.responseText);
                return false;
            },
            success: function (response) {
                unblockPage();

                if (response.Success) {
                    if (confirm("Once Proposal is Finalized, it can not be Edited and Deleted.\nAre you sure to Finalize it ?")) {
                        $.ajax({
                            url: '/Proposal/DPIUFinalizeProposal',
                            type: "POST",
                            cache: false,
                            beforeSend: function () {
                                blockPage();
                            },
                            data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
                            error: function (xhr, status, error) {
                                unblockPage();
                                Alert("Request can not be processed at this time,please try after some time!!!");
                                return false;
                            },
                            success: function (response) {
                                unblockPage();
                                if (response.Success) {
                                    alert('Proposal Finalized Successfully.');
                                    CloseProposalDetails();
                                    $("#tbProposalList").trigger("reloadGrid");
                                    $("#btnFinalize").hide();
                                }
                                else {
                                    alert(response.ErrorMessage);
                                }
                                
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

    $("#btnLockFinalize").click(function () {

        $.ajax({
            url: '/Proposal/GetProposalChecks',
            type: "POST",
            datatype: "Json",
            cache: false,
            async: false,
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!" + xhr.responseText);
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.Success) {
                    if (confirm("Once Proposal is Finalized it will be Locked, hense it can not be Edited and Deleted.\nAre you sure to Lock and Finalize it ?")) {
                        $.ajax({
                            url: '/Proposal/FinalizeUnlockedProposal',
                            type: "POST",
                            cache: false,
                            beforeSend: function () {
                                blockPage();
                            },
                            data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
                            error: function (xhr, status, error) {
                                unblockPage();
                                Alert("Request can not be processed at this time,please try after some time!!!");
                                return false;
                            },
                            success: function (response) {
                                unblockPage();
                                if (response.Success) {
                                    alert('Proposal Locked and Finalized Successfully.');
                                    CloseProposalDetails();
                                    $("#tbProposalList").trigger("reloadGrid");
                                }
                                $("#btnFinalize").hide();
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

function LoadStaSanctionForm() {

    blockPage();
    $("#divStaSanctionDetails").load('/Proposal/GetStaScritiny?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmSTAScrutiny'));
        unblockPage();
    });
    $('#divStaSanctionDetails').show('slow');
    unblockPage();
}


function LoadPtaSanctionForm() {

    blockPage();
    $("#divPtaSanctionDetails").load('/Proposal/GetPtaScrutiny?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmPTAScrutiny'));
        unblockPage();
    });
    $('#divPtaSanctionDetails').show('slow');
    unblockPage();
}

function LoadMordSanctionForm() {
    blockPage();
    $("#divMordSanctionDetails").load('/Proposal/GetMordSanctionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divMordSanctionDetails').show('slow');
    unblockPage();
}


//--------------------------------------------------------
function LoadAgreementDetails() {
    blockPage();
    $("#divAgreementDetails").load('/Agreement/ViewProposalAgreementDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divAgreementDetails').show('slow');
    unblockPage();
}

function LoadExecutionDetails() {
    blockPage();


    $("#divExecutionDetails").load('/Execution/ViewExecutionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });

    //if ($("#ddlImsProposalTypes").val() == "P") {
    //    $('#tbPhysicalRoadList').jqGrid('GridUnload');
    //    LoadPhysicalRoadDetails($("#IMS_PR_ROAD_CODE").val());

    //} else if ($("#ddlImsProposalTypes").val() == "L") {
    //    $('#tbLSBPhysicalRoadList').jqGrid('GridUnload');
    //    LoadPhysicalLSBDetails($("#IMS_PR_ROAD_CODE").val());

    //}
    //else {
    //    $('#tbPhysicalRoadList').jqGrid('GridUnload');
    //    $('#tbLSBPhysicalRoadList').jqGrid('GridUnload');
    //    LoadPhysicalRoadDetails($("#IMS_PR_ROAD_CODE").val());
    //    LoadPhysicalLSBDetails($("#IMS_PR_ROAD_CODE").val());

    //}
    //$('#tbFinancialList').jqGrid('GridUnload');
    //LoadFinancialDetails($("#IMS_PR_ROAD_CODE").val(), '');
    //unblockPage();
}
//--------------------------------------------------------
function LoadPerformanceReport() {
    blockPage();
    $("#divMaintenanceDetails").load('/MaintenanceAgreement/ViewProposalMaintenanceDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divMaintenanceDetails').show('slow');
    unblockPage();
}

function LoadMaintenanceDetails() {
    blockPage();
    $("#divMaintenanceDetails").load('/MaintenanceAgreement/ViewProposalMaintenanceDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divMaintenanceDetails').show('slow');
    unblockPage();
}



function LoadRevisedCostLengthForm() {
    blockPage();
    //$("#divRevisedRoadlength").load('/Proposal/RevisedCostLength?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
    $("#divRevisedRoadlength").load('/Proposal/RevisedCostLength', function () {
        $.validator.unobtrusive.parse($('#divRevisedRoadlength'));
        unblockPage();
    });
    $('#divRevisedRoadlength').show('slow');
    unblockPage();
}


function doNothing() {
    return false;
}

function ShowHabitations() {

    IMS_PR_ROAD_CODE = $("#IMS_PR_ROAD_CODE").val();
    //$('#tbHabitation').trigger("reloadGrid");

    jQuery("#tbHabitation").jqGrid('GridUnload');
    jQuery("#tbHabitation").jqGrid({

        url: '/Proposal/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        colNames: ['HabCode', 'Block', 'Village', 'Habitation', "SC/ST Population", "Population", "Delete", "Create New Cluster", "Cluster", "Edit Cluster"],
        colModel: [
                            { name: 'HabCode', index: 'HabCode', width: 100, sortable: false, align: "center", hidden: false },
                            { name: 'Block', index: 'Block', width: 150, sortable: false, align: "center" },
                            { name: 'Village', index: 'Village', width: 150, sortable: false, align: "center" },
                            { name: 'Habitation', index: 'Habitation', width: 200, sortable: false, align: "center" },
                            { name: 'SCSTPopulation', index: 'EditCluster', width: 200, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum' },
                            { name: 'Population', index: 'Population', width: 200, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum' },
                            { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center", hidden: $('#hdnRoleCode').val() == 25 ? false : true },
                            { name: 'CreateCluster', index: 'CreateCluster', width: 120, sortable: false, align: "center", hidden: true },
                            { name: 'Cluster', index: 'Cluster', width: 120, sortable: false, align: "center", hidden: true },
                            { name: 'EditCluster', index: 'EditCluster', width: 150, sortable: false, align: "center", edittype: "select", hidden: true }
        ],
        pager: jQuery('#dvHabitationPager'),
        rowNum: 1000,
        //rowList: [10, 15, 20],
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,        
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Habitation Details",
        height: 'auto',
        //width: 'auto',
        grouping: true,
        groupingView: {
            groupText: ["<span style='font-weight:bold'>{0}</span>"],
            groupField: ['Cluster'],
            groupOrder: ['desc', 'asc'],
            groupSummary: [true],
            //groupColumnShow: [false],
        },

        rownumbers: true,
        //footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {
            var Clusters = $('#tbHabitation').jqGrid('getCol', 'Cluster', false);
            var flag = 0;
            for (i = 0 ; i < Clusters.length; i++) {
                //HabitationCodes[i];
                if (Clusters[i] != "Cluster Not Allocated") {
                    flag = 1;
                }
            }
            if (flag == 1) {
                doGrouping(true);
            }
            if (flag == 0) {
                doGrouping(false);
                $("#tbHabitation").hideCol("Cluster");
                $("#tbHabitation").hideCol("EditCluster");
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
            //groupColumnShow: [false],
            groupCollapse: false
        });
    }
}

function GetTrafficIntensity(IMS_PR_ROAD_CODE) {

    jQuery("#tbTraffic").jqGrid({
        url: '/Proposal/GetTrafficIntensityList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Year', 'Total Motarised Traffic/day', 'ESAL', "Edit", "Delete"],
        colModel: [
                    { name: 'Year', index: 'Year', width: 230, sortable: false, align: "center" },
                    { name: 'TotalMotarisedTrafficday', index: 'TotalMotarisedTrafficday', width: 250, sortable: false, align: "center" },
                    { name: 'CCVPDESAL', index: 'CCVPDESAL', width: 250, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#dvTrafficPager'),
        rowNum: 10000,
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,   
        sortname: 'Year',
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Traffic Intensity Details",
        height: 'auto',
        width: 'auto',
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}

function GetCBRValues(IMS_PR_ROAD_CODE) {

    jQuery("#tbCBR").jqGrid({
        url: '/Proposal/GetCBRList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Segment Number', 'Start Chainage(in Kms.)', 'End Chainage(in Kms.)', "Segment Length", "CBR Value", "Edit", "Delete"],
        colModel: [
                    { name: 'SegmentNumber', index: 'SegmentNumber', width: 150, sortable: false, align: "center", hidden: true },
                    { name: 'StartChainage', index: 'StartChainage', width: 150, sortable: false, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 150, sortable: false, align: "center" },
                    { name: 'SegmentLength', index: 'SegmentLength', width: 150, sortable: false, align: "center", formatter: 'number', summaryType: 'sum' },
                    { name: 'CBRValue', index: 'CBRValue', width: 140, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#dvCBRPager'),
        rowNum: 8,
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,        
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "CBR Details",
        height: 'auto',
        width: 'auto',
        rownumbers: true,
        sortname: 'SegmentNumber',
        //footerrow: true,
        userDataOnFooter: true,
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
        }
    });

}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}

function ListProposalFiles(IMS_PR_ROAD_CODE) {

    jQuery("#tbFilesList").jqGrid({
        url: '/Proposal/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Chainage", "Description", "Download"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Chainage', index: 'Chainage', width: 80, sortable: false, align: "center" },
                    { name: 'Description', index: 'Description', width: 300, sortable: false, align: "center" },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Proposal Images",
        height: 'auto',
        //autowidth: true,
        sortname: 'image',
        rownumbers: true,
        loadComplete: function () {
            imagePreview();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }
    }); //end of grid    
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/Proposal/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function ListPDFFiles(IMS_PR_ROAD_CODE) {

    blockPage();

    jQuery("#tbPDFFilesList").jqGrid({
        url: '/Proposal/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Edit", "Delete", "Save"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 470, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 } },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "C Proforma",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
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

function ListSTASRRDAPDFFiles(IMS_PR_ROAD_CODE) {

    blockPage();

    jQuery("#tbSTASRRDAPDFFilesList").jqGrid({
        url: '/Proposal/ListSTASRRDAPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Edit", "Delete", "Save"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 470, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 } },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvSTASRRDAPDFFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Performance Report",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
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

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;
    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            alert('File Not Found.');
            return false;

        }
        else if (response.Success === undefined) {
            window.location = paramurl;
        }
    });
}

function DownLoadImage(cellvalue) {
    var url = "/Proposal/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}


function LoadTestResultForm() {

    var IMS_PR_ROAD_CODE = $("#IMS_PR_ROAD_CODE").val();

    $.ajax({
        url: '/Proposal/TestResultDetails/' + IMS_PR_ROAD_CODE,
        type: 'GET',
        catche: false,
        error: function (xhr, status, error) {

            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $("#dvTestResultForm").html(response);
            if ($("#dvError").is(":visible")) {
                $("#divError").hide("slow");
                $("#divError span:eq(1)").html('');
            }
        }
    });
}

//new change done by Vikram on 26-09-2013

function LoadTechnologyList(IMS_PR_ROAD_CODE) {

    jQuery("#tblistTechnologyDetails").jqGrid({
        url: '/Proposal/GetTechnologyDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { proposalCode: IMS_PR_ROAD_CODE },
        colNames: ['Segment No.', 'Start Chainage', 'End Chainage', 'Technical Cost', 'Layer Cost', 'Layer', 'Technology', 'Technology Type', 'Edit', 'Delete'],
        colModel: [
                            { name: 'IMS_SEGMENT_NO', index: 'IMS_SEGMENT_NO', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_START_CHAINAGE', index: 'IMS_START_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_END_CHAINAGE', index: 'IMS_END_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_TECH_COST', index: 'IMS_TECH_COST', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_LAYER_COST', index: 'IMS_LAYER_COST', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'MAST_HEAD_DESC', index: 'MAST_HEAD_DESC', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_TECH_NAME', index: 'MAST_TECH_NAME', height: 'auto', width: 120, align: "left", search: true },
                            { name: 'MAST_TECH_TYPE', index: 'MAST_TECH_TYPE', height: 'auto', width: 120, align: "left", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#dvpagerTechnology').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "IMS_SEGMENT_NO",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Technology Details List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ['MAST_TECH_TYPE'],
            groupDataSorted: true,
            //groupColumnShow: false
        },
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


//end of change


function DeleteHabitation(IMS_PR_RODE_CODE, IMS_HAB_CODE) {
    if (confirm("Are you sure to Delete the Habitation ?")) {
        $.ajax({
            url: "/Proposal/UnMapHabitation/",
            type: "POST",
            async: false,
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: IMS_PR_RODE_CODE, MAST_HAB_CODE: IMS_HAB_CODE },
            success: function (data) {
                if (data.Success) {
                    //$("#IMS_ISCOMPLETED").val('E');
                    //$('#tbHabitation').trigger("reloadGrid");
                    //PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), IMS_PR_RODE_CODE);
                    //$("#divHabStatus").hide('slow');
                    alert('Habitation Deleted Successfully');
                    setTimeout(function () {
                        //$('#tbHabitation').trigger("reloadGrid");
                        ShowHabitations();
                    }, 300);


                }
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
                unblockPage();
            }
        });
    }
    else {
        return;
    }
}

function ListPDFFilesForest(IMS_PR_ROAD_CODE) {
    blockPage();
    jQuery("#tbPDFFilesListForest").jqGrid({
        url: '/Proposal/PMGSYIIIListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["View / Download Uploaded file", "Road Name", "Delete"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 80, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'RoadName', index: 'RoadName', width: 70, sortable: false, align: "center", editable: true/*, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription }*/ },
                    //{ name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    //{ name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
            $('#tbPDFFilesListForest').jqGrid('setGridWidth', '600');
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