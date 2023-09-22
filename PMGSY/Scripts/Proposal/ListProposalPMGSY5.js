$(document).ready(function () {

    blockPage();

    if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') { ///Changes for RCPLWE
        $("#dvProposal").show();
        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }



    /*Added By Hrishikesh For STA and PTA--start--12 - 07 - 2023 */

    if ($("#ddlState").is(":visible")) {
        if ($("#RoleID").val() != 3) {
            $("#ddlState").val(0);
        }
    }

    if ($("#RoleID").val() == '3') //STA
    {
        //alert($("#RoleID").val())

        $("#divStaProposal").show();
        $("#btnAddProposal").hide();

        if ($("#ddlState").val() > 0) {
            STAListRoadProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());

        }
    }
    else if ($("#RoleID").val() == '15') //PTA
    {
        $("#btnAddProposal").hide();
        $("#divPtaProposal").show();

        PTAListRoadProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
    }

    $("#btnListProposal").click(function () {
        //alert($("#RoleID").val());
        if ($("#RoleID").val() == '3') //STA
        {
            if ($("#ddlState").val() > 0) {
                LoadSTAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
            else {
                alert("Please Select State");
            }
        }
        else if ($("#RoleID").val() == '15') {
            if (validateFilter()) {
                LoadPTAProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
        }
    });

    setTimeout(function () {
        $('#btnListProposal').trigger('click');
    }, 300);

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });
    /*Added By Hrishikesh For STA and PTA--End--12 - 07 - 2023 */

    unblockPage();

    //Added By Hrishikesh For STA and PTA--End--12 - 07 - 2023
    //for getting distric list on chanhge of state name
    $("#ddlState").change(function () {
        //alert("hdasd")
        if ($("#ddlState").val() > 0) {

            $("#ddlDistrict").empty();

            $.ajax({
                url: '/Proposal/GetDistricts',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { MAST_STATE_CODE: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    //PopulateAgenciesStateWise();
                    //alert("PopulateAgenciesStateWise")
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

});


/*Added By Hrishikesh For STA and PTA--Start--12 - 07 - 2023 */

//Scrutinize button click on Pager of Grid
function RedirectScrutinizeProposal() {
    
    if ($("#RoleID").val() == '3') {
        if ($('#tbStaProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbStaProposalList'),
                selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
                cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }
    //added by Ujjwal Saket for PTA Login on 1-11-2013
    else if ($("#RoleID").val() == '15') {
        if ($('#tbPtaProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbPtaProposalList'),
                selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
                cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }   //finish addition
}

/**********************************    STA Region         **********************************************************************************************/
function STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if ($("#ddlState").val() > 0) {
        //alert("Year :" + IMS_YEAR + "  District : " + MAST_DISTRICT_ID + "  Batch : " + IMS_BATCH + "  funding agency : " + IMS_STREAM + "  Proposal Type : " + IMS_PROPOSAL_TYPE + " Proposal Status :  " + IMS_PROPOSAL_STATUS);
        blockPage();

        jQuery("#tbStaProposalList").jqGrid({
            url: '/Proposal/GetSTARoadProposals',
            datatype: "json",
            mtype: "POST",
            colNames: ['Block', "Package Number", "Road Name", "Pavement Length", "Pavement Cost", "Upload", "View"],
            colModel: [
                { name: 'Block', index: 'Block', width: 150, sortable: false, align: "left" },
                { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "left" },
                { name: 'RoadName', index: 'RoadName', width: 290, sortable: false, align: "left" },
                { name: 'PavementLength', index: 'PavementLength', width: 150, sortable: false, align: "right" },
                { name: 'PavementCost', index: 'PavementCost', width: 150, sortable: false, align: "right" },
                { name: 'UploadDetails', index: 'UploadDetails', width: 100, sortable: false, align: "center" },
                { name: 'ShowDetails', index: 'ShowDetails', width: 100, sortable: false, align: "center" }

            ],
            postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_STATE": $("#ddlState").val(), "value": Math.random() },
            pager: jQuery('#dvStaProposalListPager'),
            rowList: [15, 30, 45],
            rowNum: 15,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Road Proposals",
            height: 'auto',
            width: 'auto',
            //autowidth: true,
            sortname: 'Block',
            rownumbers: true,
            loadComplete: function () {
                $("#tbStaProposalList #dvStaProposalListPager").css({ height: '31px' });

                $("#dvStaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")


                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    //window.location.href = "/Login/SessionExpire";
                }
                else {
                    //alert("Session Timeout !!!");
                    alert('Error occurred');
                }
            }

        }); //end of grid

    }
    //else {
    //    alert("Please Select State");
    //}
}
/**********************************    STA Region End     **********************************************************************************************/


function STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {
    //alert(MAST_DISTRICT_ID + IMS_STREAM);

    if ($("#ddlState").val() > 0) {

        blockPage();
        jQuery("#tbStaLSBProposalList").jqGrid({
            url: '/LSBProposal/GetSTALSBProposals',
            datatype: "json",
            mtype: "POST",
            colNames: ['Block', "Package Number", "Road Name", "LSB Name", "LSB Length (mtrs)", "State Share (lakhs)", "MoRD Cost (lakhs)", "View", "Upload", "Joint Inspection"],
            colModel: [
                { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "left" },
                { name: 'RoadName', index: 'RoadName', width: 280, sortable: false, align: "left" },
                { name: 'BridgeName', index: 'BridgeName', width: 260, sortable: false, align: "left" },
                { name: 'BridgeLength', index: 'BridgeLength', width: 100, sortable: false, align: "right" },
                { name: 'StateShare', index: 'StateShare', width: 100, sortable: false, align: "right" },
                { name: 'MordCost', index: 'MordCost', width: 100, sortable: false, align: "right" },
                { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
                { name: 'UploadDetails', index: 'UploadDetails', width: 40, sortable: false, align: "center" },
                { name: 'JointInspections', index: 'JointInspections', width: 40, sortable: false, align: "center" }
            ],
            postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_STATE": $("#ddlState").val(), "value": Math.random() },
            pager: jQuery('#dvStaLSBProposalListPager'),
            rowList: [15, 30, 45],
            rowNum: 15,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;LSB Proposals",
            height: 'auto',
            width: 'auto',
            //autowidth: true,
            sortname: 'Block',

            rownumbers: true,
            loadComplete: function () {
                //alert("sta loadcompalate")
                $("#tbStaLSBProposalList #dvStaLSBProposalListPager").css({ height: '31px' });
                $("#dvStaLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeLSBProposal();return false;' value='Scrutinize Proposal'/>")
                //alert("sta scrutinize button add")
                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    //window.location.href = "/Login/SessionExpire";
                }
                else {
                    //alert("Session Timeout !!!");
                    alert('Error occurred');
                }
            }
        }); //end of grid

    }
    //else {
    //    alert("Please select State");
    //}
}
//--------------------------------------------    STA LSB Region End    ----------------------------------------------//

function LoadSTAProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {
    //alert("jgdf");
    //alert(IMS_PROPOSAL_TYPE);
    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbStaLSBProposalList").hide();
        $("#dvStaLSBProposalListPager").hide();
        $("#tbStaProposalList").show();
        $("#dvStaProposalListPager").show();
        $('#tbStaProposalList').jqGrid('GridUnload');
        $('#tbStaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        //STAListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbStaLSBProposalList").show();
        $("#dvStaLSBProposalListPager").show();
        $("#tbStaProposalList").hide();
        $("#dvStaProposalListPager").hide();
        $('#tbStaProposalList').jqGrid('GridUnload');
        $('#tbStaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbStaProposalList").show();
        $("#dvStaProposalListPager").show();
        $("#tbStaLSBProposalList").show();
        $("#dvStaLSBProposalListPager").show();
        $('#tbStaLSBProposalList').jqGrid('GridUnload');
        $('#tbStaProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');



        STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }
}


/**********************************    PTA Region         **********************************************************************************************/
function PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    //alert("Year :" + IMS_YEAR + "  District : " + MAST_DISTRICT_ID + "  Batch : " + IMS_BATCH + "  funding agency : " + IMS_STREAM + "  Proposal Type : " + IMS_PROPOSAL_TYPE + " Proposal Status :  " + IMS_PROPOSAL_STATUS);
    blockPage();

    jQuery("#tbPtaProposalList").jqGrid({
        url: '/Proposal/GetPTARoadProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', "Package Number", "Road Name", "Pavement Length", "Pavement Cost", "Upload", "View"],
        colModel: [
            { name: 'Block', index: 'Block', width: 150, sortable: false, align: "left" },
            { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "left" },
            { name: 'RoadName', index: 'RoadName', width: 290, sortable: false, align: "left" },
            { name: 'PavementLength', index: 'PavementLength', width: 200, sortable: false, align: "right" },
            { name: 'PavementCost', index: 'PavementCost', width: 200, sortable: false, align: "right" },
            { name: 'UploadDetails', index: 'UploadDetails', width: 70, sortable: false, align: "center" },
            { name: 'ShowDetails', index: 'ShowDetails', width: 70, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS },
        pager: jQuery('#dvPtaProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        loadComplete: function () {
            $("#tbPtaProposalList #dvPtaProposalListPager").css({ height: '31px' });

            $("#dvPtaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid


}
/**********************************    PTA Region End     **********************************************************************************************/

function LoadPTAProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbPtaLSBProposalList").hide();
        $("#dvPtaLSBProposalListPager").hide();
        $("#tbPtaProposalList").show();
        $("#dvPtaProposalListPager").show();
        $('#tbPtaProposalList').jqGrid('GridUnload');
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        //STAListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbPtaLSBProposalList").show();
        $("#dvPtaLSBProposalListPager").show();
        $("#tbPtaProposalList").hide();
        $("#dvPtaProposalListPager").hide();
        $('#tbPtaProposalList').jqGrid('GridUnload');
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbPtaProposalList").show();
        $("#dvPtaProposalListPager").show();
        $("#tbPtaLSBProposalList").show();
        $("#dvPtaLSBProposalListPager").show();
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');
        $('#tbPtaProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }
}

//---------------------------------PTA LSB Region Starts-----------------------//

function PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {
    //alert(MAST_DISTRICT_ID + IMS_STREAM);
    blockPage();
    jQuery("#tbPtaLSBProposalList").jqGrid({
        url: '/LSBProposal/GetPTALSBProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', "Package Number", "Road Name", "LSB Name", "LSB Length (mtrs)", "State Share (lakhs)", "MoRD Cost (lakhs)", "View", "Upload"],
        colModel: [
            { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
            { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "left" },
            { name: 'RoadName', index: 'RoadName', width: 280, sortable: false, align: "left" },
            { name: 'BridgeName', index: 'BridgeName', width: 260, sortable: false, align: "left" },
            { name: 'BridgeLength', index: 'BridgeLength', width: 100, sortable: false, align: "right" },
            { name: 'StateShare', index: 'StateShare', width: 80, sortable: false, align: "right" },
            { name: 'MordCost', index: 'MordCost', width: 80, sortable: false, align: "right" },
            { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
            { name: 'UploadDetails', index: 'UploadDetails', width: 40, sortable: false, align: "center" },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "value": Math.random() },
        pager: jQuery('#dvPtaLSBProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        loadComplete: function () {
            $("#tbPtaLSBProposalList #dvPtaLSBProposalListPager").css({ height: '31px' });
            $("#dvPtaLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeLSBProposal();return false;' value='Scrutinize Proposal'/>")
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid


}
//--------------------------------------------    PTA LSB Region End    ----------------------------------------------//


function validateFilter() {
    if ($("#ddlState").val() == "0") {
        alert("Please Select State");
        return false;
    }
    if ($("#ddlDistrict").val() == "0") {
        alert("Please Select District");
        return false;
    }
    return true;
}
/*Added By Hrishikesh For STA and PTA--End--12 - 07 - 2023 */


function LoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    
    if (IMS_PROPOSAL_TYPE == "P") {

        $("#btnAddProposal").show();

        $("#tbLSBProposalList").hide();
        $("#dvLSBProposalListPager").hide();

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();

        $("#tbProposalList").show();
        $("#dvProposalListPager").show();

        $('#tbProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbBuildingProposalList').jqGrid('GridUnload');

        RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "L") {

        $("#btnAddProposal").show();

        $("#tbProposalList").hide();
        $("#dvProposalListPager").hide();

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();

        $("#tbLSBProposalList").show();
        $("#dvLSBProposalListPager").show();

        $('#tbProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbBuildingProposalList').jqGrid('GridUnload');

        LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "B") {

        $("#btnAddProposal").show();

        $("#tbProposalList").hide();
        $("#dvProposalListPager").hide();

        $("#tbLSBProposalList").hide();
        $("#dvLSBProposalListPager").hide();

        $("#tbBuildingProposalList").show();
        $("#dvBuildingProposalListPager").show();

        $('#tbProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbBuildingProposalList').jqGrid('GridUnload');

        BuildingProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "A") {
        $("#btnAddProposal").hide();

        $("#tbProposalList").show();
        $("#dvProposalListPager").show();

        $("#tbLSBProposalList").show();
        $("#dvLSBProposalListPager").show();

        $("#tbBuildingProposalList").show();
        $("#dvBuildingProposalListPager").show();

        $('#tbProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbBuildingProposalList').jqGrid('GridUnload');

        RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        BuildingProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
}

function RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    
    blockPage();
    jQuery("#tbProposalList").jqGrid('GridUnload');
    jQuery("#tbProposalList").jqGrid({
        url: '/Proposal/GetProposalsVibrantVillage',
        datatype: "json",
        mtype: "POST",
        
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)",
            "MoRD Share (in Lakhs)", ($('#PMGSYScheme').val() == 2 || $('#PMGSYScheme').val() == 4) ? "State Share Excluding higher Specification (in Lakhs)" : "State Share (in Lakhs)", "Higher Specification Cost (in Lakhs)",
            "Total Cost (Rs. in Lakhs)", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)",
            "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)",
            "Status", "Habitations", "Traffic Intensity", "CBR Details", "Upload", "Technology Details", "View", "Edit", "Delete", "Proposal Status"],

        colModel: [
            { name: 'District', index: 'District', width: 60, sortable: false, align: "left" },
            { name: 'Block', index: 'Block', width: 60, sortable: false, align: "left" },
            { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: false, align: "center" },
            { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 70, sortable: false, align: "center" },
            { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
            { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
            { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
            { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
            { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
            { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
            { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
            { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
            { name: 'MordCost', index: 'MordCost', width: 60, sortable: false, align: "right" },
            { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right" },
            { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
            { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
            { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
            { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
            { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
            { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
            { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right" },
            { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
            { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 60, sortable: false, align: "center" },
            { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
            { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
            { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
            { name: 'Habitations', index: 'Habitations', width: 50, sortable: false, align: "center", search: false },
            { name: 'TrafficIntensity', index: 'TrafficIntensity', width: 50, sortable: false, align: "center", search: false },
            { name: 'CBRDetails', index: 'CBRDetails', width: 50, sortable: false, align: "center", search: false },
            { name: 'Upload', index: 'Upload', width: 50, sortable: false, align: "center", hidden: false, search: false },
            { name: 'TechDetails', index: 'TechDetails', width: 50, sortable: false, align: "center", search: false },
            { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false },
            { name: 'Edit', index: 'Edit', width: 50, sortable: false, align: "center", search: false },
            { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center", search: false },
            { name: 'ProposalStatus', index: 'ProposalStatus', width: 50, sortable: false, align: "center", hidden: true, search: false },
            //{ name: 'ForestClearence', index: 'ForestClearence', width: 50, sortable: false, align: "center", search: false },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_BLOCK_ID": MAST_BLOCK_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals Vibrant Village",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbProposalList #dvProposalListPager").css({ height: '31px' });

            $("#dvProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeProposal();return false;' value='Finalize Proposal'/>")

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionProposal").show("slow");
                $("#tbProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionProposal").hide("slow");
                $("#tbProposalList").jqGrid('hideCol', 'cb');
            }

            var lengthTotal = jQuery("#tbProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
            var costTotal = jQuery("#tbProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
            var habs1000Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
            var habs999Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
            var habs499Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
            var habs250Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
            var habsTotal = jQuery("#tbProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
            var stateCost = jQuery("#tbProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
            var maintenanceCost = jQuery("#tbProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            var renewalCost = jQuery("#tbProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
            var mordCost = jQuery("#tbProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');

            var totFundShareRatio = jQuery("#tbProposalList").jqGrid('getCol', 'FundShareRatio', false, 'sum');
            var totFundStateShareCost = jQuery("#tbProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var totMordShareCost = jQuery("#tbProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totTotalStateShare = jQuery("#tbProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            jQuery("#tbProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Page Total:',
                    PavementLength: parseFloat(lengthTotal).toFixed(3),
                    PavementCost: parseFloat(costTotal).toFixed(2),
                    Hab1000: habs1000Total,
                    Hab999: habs1000Total,
                    Hab499: habs1000Total,
                    Hab250: habs1000Total,
                    HabTotal: habsTotal,
                    StateCost: parseFloat(stateCost).toFixed(2),
                    MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                    RENEWAL_AMT: parseFloat(renewalCost).toFixed(2),
                    MordCost: parseFloat(mordCost).toFixed(2),
                    HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2),

                    TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                    StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                    MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                    TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                });

            jQuery("#tbProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Grand Total:',
                    PavementLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                    PavementCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                    Hab1000: data.TotalColumn.TOT_HAB1000,
                    Hab999: data.TotalColumn.TOT_HAB999,
                    Hab499: data.TotalColumn.TOT_HAB499,
                    Hab250: data.TotalColumn.TOT_HAB250,
                    HabTotal: data.TotalColumn.TOT_HABS,
                    StateCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                    MAINT_AMT: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                    RENEWAL_AMT: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                    MordCost: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                    HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),

                    TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                    StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                    MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                    TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                });


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
            }
            else {
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

    jQuery("#tbProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}

function LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {

    jQuery("#tbLSBProposalList").jqGrid({
        url: '/LSBProposal/GetLSBProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Road Name", "LSB Name", "LSB Length (mtrs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (Rs Lakhs)" : "State Share Excluding Higher Specification (Rs Lakhs)", "Higher Specification Cost (Rs Lakhs)",
            "MoRD Cost (Rs Lakhs)", "Total Cost", "Maintenance Cost (Rs Lakhs)", "Renewal Cost (Rs Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Component Details", "Other Details", "Upload", "View", "Edit", "Delete"],
        colModel: [
            { name: 'Block', index: 'Block', width: 80, sortable: false, align: "left" },
            { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: false, align: "left" },
            { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
            { name: 'LSBName', index: 'LSBName', width: 160, sortable: false, align: "left" },
            { name: 'LSBLength', index: 'LSBLength', width: 80, sortable: false, align: "right" },
            { name: 'EstimatedCostState', index: 'EstimatedCostState', width: 70, sortable: false, align: "right" },
            { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right" },
            { name: 'EstimatedCost', index: 'EstimatedCost', width: 70, sortable: false, align: "right" },
            { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 70, sortable: false, align: "right" },
            { name: 'RenewalCost', index: 'RenewalCost', width: 70, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
            { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
            { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
            { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
            { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
            { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
            { name: 'ComponentDetails', index: 'ComponentDetails', width: 70, sortable: false, align: "center" },
            { name: 'OtherDetails', index: 'OtherDetails', width: 40, sortable: false, align: "center" },
            { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center" },
            { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_BLOCK_ID": MAST_BLOCK_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvLSBProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: 'auto',
        width: 'auto',
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbLSBProposalList #dvLSBProposalListPager").css({ height: '31px' });

            $("#dvLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeLSBProposal();return false;' value='Finalize Proposal'/>")

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionLSBProposal").show("slow");
                $("#tbLSBProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionLSBProposal").hide("slow");
                $("#tbLSBProposalList").jqGrid('hideCol', 'cb');
            }


            var lengthTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'BridgeLength', false, 'sum');
            var statecostTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'StateShare', false, 'sum');
            var mordcostTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbLSBProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');
            
            jQuery("#tbLSBProposalList").jqGrid('footerData', 'set',
                {
                    LSBName: 'Grand Total:',
                    LSBLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                    EstimatedCostState: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                    EstimatedCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                    HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                    MaintenanceCost: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),

                    TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                    StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                    MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                    TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                    RenewalCost: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                });



            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
            }
            else {
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbLSBProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

    jQuery("#tbLSBProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
            { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}

function BuildingProposalGrid(syear, block, batch, stream, proptype, propstatus, propconnect) {
    
    jQuery("#tbBuildingProposalList").jqGrid({
        url: '/BuildingProposal/GetProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Year", "Work Name", "Total Cost", "Batch", "MoRD (Clearance Date)", "View", "PIU Finalized", "Edit", "Delete"],//"Upload",
        colModel: [

            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 80, sortable: false, align: "left" },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 70, sortable: false, align: "left" },
            { name: 'IMS_YEAR_FINANCIAL', index: 'IMS_YEAR_FINANCIAL', width: 70, sortable: false, align: "left" },
            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 150, sortable: false, align: "left" },
            { name: 'IMS_PAV_EST_COST', index: 'IMS_PAV_EST_COST', width: 70, sortable: false, align: "right" },
            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 80, sortable: false, align: "right" },
            { name: 'MoRD', index: 'MoRD', width: 100, sortable: false, align: "center" },
            { name: 'Display', index: 'Display', width: 40, sortable: false, align: "center" },
            { name: 'Finalize', index: 'Finalize', width: 70, sortable: false, align: "center" },
            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }

        ],
        postData: { "syear": syear, "block": block, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect },
        pager: jQuery('#dvBuildingProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Building Proposals",
        height: 'auto',
        width: 'auto',
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbBuildingProposalList #dvBuildingProposalListPager").css({ height: '31px' });

            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
            }
            else {
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbBuildingProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

}

$("#btnListProposal").click(function () {
    
    blockPage();

    CloseProposalDetails();
    showFilter();

    unblockPage();
});

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

// -------------- Add Proposal ---------------

$(function () {
    $("#accordion").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });
});

$('#btnAddProposal').click(function () {
    $("#accordion div").html("");
    

    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Add " + $("#ddlImsProposalTypes option:selected").text() + " Proposal Details Vibrant Village</a>" +

        '<a href="#" style="float: right;">' +
        '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseProposalDetails();" /></a>' +
        '<span style="float: right;"></span>'
    );

    $('#accordion').show('slow', function () {
        blockPage();

        if ($("#ddlImsProposalTypes").val() == "P") {
            
            $("#divProposalForm").load('/Proposal/CreateVibrantVillage/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                $.validator.unobtrusive.parse($('#divProposalForm'));
                unblockPage();
            });
        }
        else if ($("#ddlImsProposalTypes").val() == "L") {

            $("#divProposalForm").load('/LSBProposal/CreateLSB/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                $.validator.unobtrusive.parse($('#divProposalForm'));
                unblockPage();
            });
        }
        else if ($("#ddlImsProposalTypes").val() == "B") {
            $("#divProposalForm").load('/BuildingProposal/BuildingCreate/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                $.validator.unobtrusive.parse($('#divProposalForm'));
                unblockPage();
            });
        }

        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
});

function CloseProposalDetails() {

    //alert("in CloseProposalDetails");

    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
    $("#tbLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbStaProposalList').jqGrid('setGridState', 'visible');
    $("#tbStaLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbPtaProposalList').jqGrid('setGridState', 'visible');     
    $('#tbPtaLSBProposalList').jqGrid('setGridState', 'visible');
    $("#tbMORDProposalList").jqGrid('setGridState', 'visible');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbBuildingProposalList").jqGrid('setGridState', 'visible');

    if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') {  ///Changes for RCPLWE

        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }

    showFilter();
}

// -------------- Edit Proposal ---------------

function EditDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Edit Road Proposal Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/EditProposalPMGSY5/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');


}

// -------------- Delete Proposal ---------------

function DeleteDetails(id) {

    $.ajax({
        url: "/Proposal/IsProposalDeleted/" + id,
        type: "POST",
        cache: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {
            unblockPage();

            if (response.success) {
                if (confirm("Are you sure to Delete Road Proposal Details ? ")) {

                    $.ajax({
                        url: '/Proposal/DeletePMGSY3Proposal/' + id,
                        type: "POST",
                        cache: false,
                        data: { __RequestVerificationToken: $("#FilterForm input[name=__RequestVerificationToken]").val() },
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {

                            if (response.success == true) {
                                alert("Proposal Deleted Successfully.");
                                CloseProposalDetails();
                                $("#tbProposalList").trigger('reloadGrid');
                            }
                            else if (response.success == false) {
                                alert(response.errorMessage);
                            }
                            else {
                                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                                    alert(response.errorMessage)
                                }
                                else {
                                    alert("Error Occured while processing your request.");
                                }

                            }
                            unblockPage();
                        }
                    });
                } else {
                    return;
                }
            }
            else {
                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                    alert(response.errorMessage)
                }
                else {
                    alert("Error Occured while processing your request.");
                }
            }
        }
    });
}

// -------------- View Proposal Details ---------------

function ShowDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Road Proposal Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/ViewDetailsPMGSY5Proposal?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#tbStaProposalList').jqGrid('setGridState', 'hidden');
    $('#tbPtaProposalList').jqGrid('setGridState', 'hidden');     
    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

// -------------- Edit Habitations Details ---------------

function EditHabitationsDetails(urlparamater) {

    var arr = urlparamater.split("$");

    jQuery('#tbProposalList').jqGrid('setSelection', arr[0]);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Add Habitation Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fast', function () {

        $("#divProposalForm").load("/Proposal/AddHabitation/" + urlparamater, function () {   //WithCluster
            ShowHabitations($("#IMS_PR_ROAD_CODE").val());
            $.validator.unobtrusive.parse($('#divProposalForm'));
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

// -------------- Edit Traffic Details ---------------

function EditTrafficDetails(urlparamater) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlparamater);
    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Traffic Intensity Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/TrafficIntensity/' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

// -------------- Edit CBR Details ---------------

function EditCBRDetails(urlParameter) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >CBR Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/CBRValue/' + urlParameter, function () {
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

// -------------- Upload File ---------------

function UploadFile(urlParameter) {
    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >" + (($("#RoleID").val() == 22 || $("#RoleID").val() == 54) ? "Upload C Proforma" : $("#RoleID").val() == 3 ? "Joint Inspection" : "Upload Performance Report") + "</a>" +

        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>' +
        '<span style="float: right;"></span>'

    );

    $('#accordion').show('fold', function () {

        $("#divProposalForm").load('/Proposal/FileUpload/' + urlParameter, function () {

        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');
}

function JIUploadFile(urlParameter) {
    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >" + "Joint Inspection" + "</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {

        $.ajax({
            url: '/Proposal/FileUpload/' + urlParameter,
            type: "GET",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                $("#divProposalForm").html('');
                $("#divProposalForm").html(response);

                unblockPage();
            }
        });

        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');

}

// -------------- Add Technology Details ---------------

function AddTechnologyDetails(id) {

    $("#accordion div").html("");

    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Add Technology Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/ListTechnologyDetails/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function RedirectFinalizeProposal() {

    if ($('#tbProposalList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        ShowDetails(selectedRowId);
    }
    else {
        alert("Please click on Proposal to select.");
        return false;
    }
}

//-------------------- Edit Component Details for LSB ------------------------

function EditComponentDetails(urlparamater) {

    jQuery('#tbLSBProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >LSB Component Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );
    blockPage();
    $('#accordion').show('fast', function () {
        $("#divProposalForm").load("/LSBProposal/ShowLSBComponentList/" + urlparamater, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
    });
    $("#divProposalForm").css('height', 'auto');
    $('#divProposalForm').show('fast');

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//------------------------- Edit Other Details for LSB ------------------------
function EditLSBOtherDetails(urlparamater) {

    jQuery('#tbLSBProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >LSB Other Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/LSBProposal/LSBOtherDetails/" + urlparamater, function () {
            //ShowHabitations($("#IMS_PR_ROAD_CODE").val(), 0);
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//----------------- Show LSB Details -------------------
function ShowLSBDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >LSB Proposal Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/LSBDetails?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

//------------------- Editing LSB Proposal -------------------------------
function EditLSBDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Edit LSB Proposal Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/EditLSB?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//----------------- Delete LSB Proposal -----------------------------
function DeleteLSBDetails(id) {

    if (confirm("Are you sure to delete proposal details ? ")) {

        $.ajax({
            url: '/LSBProposal/DeleteLSBConfirmed?id=' + id,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success == "true") {
                    alert("Proposal Deleted Successfully.");
                    $("#tbLSBProposalList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.errorMessage)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }

                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}

//----------------- Finalize Bridge Proposal ----------------------

function RedirectFinalizeLSBProposal() {

    //alert("Test");
    if ($('#tbLSBProposalList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbLSBProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        //alert(cellValue);
        //alert($("#IMS_LOCK_STATUS").val());

        ShowLSBDetails(selectedRowId);
    }
    else {
        alert("Please click on Proposal to select.");
        return false;
    }
}

// ----------------- Building -------------------

function ShowBuildingDetails(id) {
    //alert("In Building Details");

    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Building Proposal Details</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/BuildingProposal/BuildingDetails/' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            //if ($("#RoleID").val() == '25')
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#tbStaProposalList').jqGrid('setGridState', 'hidden');
    $('#tbPtaProposalList').jqGrid('setGridState', 'hidden');
    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


    $('#idFilterDiv').trigger('click');


}

function PIUFinalizeBuildingDetails(id) {
    blockPage();
    $.post("/BuildingProposal/PIUFinalizedBuilding/", { id: id }, function (data) {
        if (data.Success) {
            LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
            alert("Building Proposal has been finalized");

        }
        else {
            alert("Processing Error Occur!");
        }
        unblockPage();
    });
}

function BuildingUpdate(id) {




    $("#accordion div").html("");
    $("#accordion h3").html(
        "<a href='#' style= 'font-size:.9em;' >Building Proposal Update</a>" +
        '<a href="#" style="float: right;">' +
        '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/BuildingProposal/BuildingEdit/" + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


}

function BuildingDelete(id) {

    if (confirm("Are you sure to Delete Road Proposal Details ? ")) {
        blockPage();
        $.post("/BuildingProposal/BuildingProposalDelete/", { id: id }, function (data) {
            unblockPage();
            Alert(data.errorMessage);
            LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());

        });

    }
}