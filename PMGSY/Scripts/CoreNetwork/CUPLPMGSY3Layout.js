$(document).ready(function () {

    //Initialize Modal
    $("#dvModal").dialog({
        autoOpen: false,
        height: '220',
        width: "420",
        modal: true,
        title: 'Generate CUPL PMGSY3'
    });

    $('#btnView').click(function () {
        $('#loadReport').html('');
        $('#dvModal').html('');
        jQuery("#tbCUPLList").jqGrid('GridUnload');
        LoadBlocks();
    });
});

function LoadBlocks() {

    jQuery("#tbBlockList").jqGrid('GridUnload');
    jQuery("#tbBlockList").jqGrid({
        url: '/CoreNetwork/GetBlockListCUPLPMGSY3/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Block', 'View', 'Generate CUPL', 'Copy TR/MRL Exemption to Batch2'],
        colModel: [
                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'View', index: 'View', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'Generate', index: 'Generate', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: false, },
                        { name: 'Copy', index: 'View', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: $('#ddlBatch').val() == 1 ? true : false, },
        ],
        postData: { districtCode: $("#ddlDistrict option:selected").val(), Year: $("#ddlYear option:selected").val(), Batch: $("#ddlBatch option:selected").val() },
        pager: jQuery('#dvBlockListPager'),
        rowNum: 10,
        sortorder: "asc",
        sortname: 'Block',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Block List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function (data) {
            if (data == null) {
                alert('Error occured');
            }
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert('Error occured');
                window.location.href = "/Login/Login";
            }
            else {
                alert('Error occured');
                //window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
}

//function ViewCUPLDetails(urlparameter) {
//    $('#loadReport').html('');
//    $('#dvModal').html('');
//    jQuery("#tbCUPLList").jqGrid('GridUnload');
//    jQuery("#tbCUPLList").jqGrid({
//        url: '/CoreNetwork/GetCUPLPMGSY3List/' + urlparameter,
//        datatype: "json",
//        mtype: "GET",
//        colNames: ['District', 'Block', 'CN Road Number', 'Road Length', 'Eligible Length', 'Road Name', 'Score', 'Score per unit', 'DRRP Road Name', 'DRRP Road Number', 'From Chainage', 'To Chainage', 'DRRP Score', 'PCI', 'Eligible Length', 'Completed', 'Eligible', 'Date Difference', /*'Trace map rank',*/ 'Total Hab Server', 'Finalize Date'],
//        colModel: [
//                        { name: 'District', index: 'District', width: 60, sortable: true, align: "left", search: false }, //New
//                        { name: 'Block', index: 'Block', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'CNRoadNo', index: 'CNRoadNo', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'RoadLength', index: 'RoadLength', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'EligibleLength', index: 'EligibleLength', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'RoadName', index: 'RoadName', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'Score', index: 'Score', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'Scoreperunit', index: 'Scoreperunit', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'DRRPRoadName', index: 'DRRPRoadName', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'DRRPRoadNo', index: 'DRRPRoadNo', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'FromChainage', index: 'FromChainage', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'ToChainage', index: 'ToChainage', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'DRRPScore', index: 'DRRPScore', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'PCI', index: 'PCI', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'EligibleLength', index: 'EligibleLength', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'Completed', index: 'Completed', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'Eligible', index: 'Eligible', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'DateDifference', index: 'DateDifference', width: 60, sortable: true, align: "left", search: false },
//                        //{ name: 'TraceMap', index: 'TraceMap', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'TotHabServer', index: 'TotHabServer', width: 60, sortable: true, align: "left", search: false },
//                        { name: 'Finalizedate', index: 'Finalizedate', width: 60, sortable: true, align: "left", search: false },
//        ],
//        postData: { urlparameter: urlparameter },
//        pager: jQuery('#dvCUPLListPager'),
//        rowNum: 10,
//        sortorder: "asc",
//        sortname: 'Block',
//        rowList: [5, 10, 15, 20],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "&nbsp;&nbsp;Block List",
//        height: 'auto',
//        autowidth: true,
//        cmTemplate: { title: false },
//        rownumbers: true,
//        loadComplete: function (data) {
//            if (data == null) {
//                alert('Error occured');
//            }
//            unblockPage();
//        },
//        loadError: function (xhr, ststus, error) {
//            if (xhr.responseText == "session expired") {
//                alert('Error occured');
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert('Error occured');
//                //window.location.href = "/Login/LogIn";
//            }
//        }
//    }); //end of grid

//    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
//}

function ViewCUPLDetails(urlParameter) {
    $('#loadReport').html('');
    $('#dvModal').html('');
    jQuery("#tbCUPLList").jqGrid('GridUnload');
    if ($('#frmCUPLLayout').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/CoreNetwork/CUPLPMGSY3GeneratedReport/' + urlParameter,
            type: 'POST',
            catche: false,
            //data: $("#frmPhyProgessWork").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    }
    else {

    }
};

function LoadCUPLReport(urlParameter) {
    $('#loadReport').html('');
    $('#dvModal').html('');
    jQuery("#tbCUPLList").jqGrid('GridUnload');
    if ($('#frmCUPLLayout').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/CoreNetwork/CUPLPMGSY3Report/' + urlParameter,
            type: 'POST',
            catche: false,
            //data: $("#frmPhyProgessWork").serialize(),
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    }
    else {

    }
};

function GenerateCUPLModal(urlParameter) {
    $('#loadReport').html('');
    $('#dvModal').html('');
    jQuery("#tbCUPLList").jqGrid('GridUnload');
    if ($('#frmCUPLLayout').valid()) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/CoreNetwork/GenerateCUPLPMGSY3Layout/' + urlParameter,
            type: 'GET',
            catche: false,
            data: { Year: $("#ddlYear option:selected").val(), Batch: $("#ddlBatch option:selected").val() }/*$("#frmPhyProgessWork").serialize()*/,
            async: false,
            success: function (response) {
                $.unblockUI();
                $("#loadReport").html(response);

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    }
    else {

    }
};


function CopyTRMRLExemptionBatch2(urlParameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    $.ajax({
        url: '/CoreNetwork/CopyTRMRLExemptiontoBatch2/' + urlParameter,
        type: 'POST',
        catche: false,
        async: false,
        success: function (response) {
            $.unblockUI();
            //$("#loadReport").html(response);
            alert(response.message);
            $("#tbBlockList").trigger('reloadGrid');
        },
        error: function () {
            $.unblockUI();
            alert("An Error occured");
            return false;
        },
    });

};