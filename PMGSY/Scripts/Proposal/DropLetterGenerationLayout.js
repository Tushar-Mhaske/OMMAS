$(document).ready(function () {

    //LoadProposalsToDropList();
    LoadDropProposalsList();
});

function LoadDropProposalsList() {
    $("#tblLetterRequest").jqGrid('GridUnload');
    $('#tblLetterRequest').jqGrid({
        url: '/Proposal/ListDropppingWorks',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Request Letter No.', /*'Drop Letter No.',*/ 'Request Date', 'No. of Works for Dropping', 'No. of Works Approved for Dropping', /*'Approved',*/ 'Generate Request', 'View Request Letter'],
        colModel: [
                      { name: 'State', index: 'State', height: 'auto', width: 30, align: "center", sortable: false, hidden: false },
                      { name: 'RequestLetterNo', index: 'RequestLetterNo', height: 'auto', width: 30, align: "center", sortable: false, hidden: false },
                      //{ name: 'DropLetterNo', index: 'DropLetterNo', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'RequestDate', index: 'RequestDate', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'DroppingWorks', index: 'DroppingWorks', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'ApprovedWorks', index: 'ApprovedWorks', height: 'auto', width: 30, align: "center", sortable: false },
                      //{ name: 'Approved', index: 'Approved', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'GenerateRequest', index: 'GenerateRequest', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'View', index: 'View', height: 'auto', width: 30, align: "center", sortable: false },
        ],
        pager: jQuery('#dvPagerLetterRequest'),
        rowNum: 40,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Works for Dropping",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            //if (parseInt($("#tblLetterGen").getGridParam("reccount")) > 0) {
            //    $('#dvPagerLetterGen').html("<input type='button' style='margin-left:27px' id='btnGenLetter' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateDropLetter();return false;' value='Generate Drop Letter'/>")
            //}
        },
    });
}

function LoadProposalsToDropList() {
    //function ShowDropDetails(param) {
    $("#tblLetterGen").jqGrid('GridUnload');
    $('#tblLetterGen').jqGrid({
        url: '/Proposal/ListWorksForDroppping/',
        datatype: "json",
        mtype: "POST",
        colNames: ['REQ ID', 'Road Code', 'District', 'Block', 'Road Name', 'Pavement Length [in Kms.]', 'Total Cost', 'Expenditure Incurred', 'Recoup Amount','Delete'],
        colModel: [
                      { name: 'REQID', index: 'REQID', height: 'auto', width: 30, align: "center", sortable: false, hidden: true },
                      { name: 'RoadCode', index: 'RoadCode', height: 'auto', width: 30, align: "center", sortable: false, hidden: true },
                      { name: 'District', index: 'District', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'Block', index: 'Block', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'RoadName', index: 'RoadName', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'PavementLength', index: 'PavementLength', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'TotalCost', index: 'TotalCost', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'ExpenditureIncurred', index: 'ExpenditureIncurred', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'RecoupAmount', index: 'RecoupAmount', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'a', width: 30, sortable: false, resize: false, formatter: FormatColumn11, align: "center", sortable: false }
        ],
        //postData: { reqCode: param },
        pager: jQuery('#dvPagerLetterGen'),
        rowNum: 100,
        rowList: [50, 75, 100],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'REQID',
        sortorder: "asc",
        caption: "Works for Dropping",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            if (parseInt($("#tblLetterGen").getGridParam("reccount")) > 0) {
                $('#dvPagerLetterGen_left').html("<input type='button' style='margin-left:27px' id='btnGenLetter' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateDropLetter();return false;' value='Generate Request Letter'/>")
            }
        },
    });
}

function FormatColumn11(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Details' onClick ='DeleteReqDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function DeleteReqDetails(urlparameter) {
    if (confirm("Are you sure you want to delete this Work details?")) {
        $.ajax({
            type: 'POST',
            url: '/Proposal/DeleteRequestDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Work details deleted successfully.');
                    $('#tblLetterGen').trigger('reloadGrid');
                 //   $("#dvLayoutofAddChapterView").load('/ARRR/AddEditCategoryDetails');
                }
                else if (data.success == false) {
                    alert('Work details are in use and can not be deleted.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

function GenerateDropLetter() {
    var arr = [];
    var myIDs = $('#tblLetterGen').jqGrid('getDataIDs');

    for (var i = 1; i <= myIDs.length; i++) {
        //// DO YOUR STUFF HERE
        var reqId = $('#tblLetterGen').jqGrid('getCell', i, 'REQID');
        var roadCode = $('#tblLetterGen').jqGrid('getCell', i, 'RoadCode');
        var exp = $('#tblLetterGen').jqGrid('getCell', i, 'ExpenditureIncurred');
        var recoup = $('#tblLetterGen').jqGrid('getCell', i, 'RecoupAmount');

        arr.push(reqId + ',' + roadCode + ',' + exp + ',' + recoup);
    }

    $.ajax({
        type: 'POST',
        url: '/Proposal/DropLetterRequestLayout/',
        async: false,
        cache: false,
        data: { DropDetails: arr, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
        traditional: true,
        success: function (data) {
            //arr.splice(0, arr.length); //Clear the preveious value
            ////alert(data.success);
            //if (data.success) {
            //    LoadmatrixParametersWeightageDetailsList();
            //}
            //if (data.success == false) {
            //    /**/
            //}
            //alert(data.message);

            $('#dvRequestLetter').html('');
            $('#dvRequestLetter').html(data);

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //arr.splice(0, arr.length);//Clear the preveious value
            alert(data.message);
            $.unblockUI();
        }
    });
}