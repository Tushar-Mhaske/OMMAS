$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmMrdDropLetter');

    LoadMrdDroppedLetterGrid();


    $('#btnAddMRDDroppedLetter').click(function () {
        //alert($('#EncryptedClearanceCode').val());
        
        loadSaveMrdDroppedLetter($('#EncryptedClearanceCode').val());
        
    })
});


function LoadMrdDroppedLetterGrid() {
    if ($("#frmSearchMrdDropLetter").valid()) {

        //alert($('#MRD_CLEARANCE_CODE').val());
        $("#tblMrdDroppedLetterList").jqGrid('GridUnload');

        jQuery("#tblMrdDroppedLetterList").jqGrid({
            url: '/MRDProposal/GetMrdDroppedLetterList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Agency', 'Clearance Date', 'Clearance Number', 'PDF copy of Dropping Letter', 'Roads/ Bridges Dropped in PDF format', 'Roads/ Bridges Dropped in Excel Sheet format', 'Collaboration', 'Phase', 'Batch', 'Sanction Cost (MoRD Share Rs. in Cr.)', 'Sanction Cost (State Share Rs. in Cr.)', 'Total Sanction Cost (Rs. in Cr.)', 'No. of Roads', 'Road Length in Kms.', 'No. of Bridges', 'LSB Length in Mtrs.)',
                        'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100', 'Edit', 'Delete'], //26
            colModel: [

                        //{ name: 'Add', index: 'Add', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "left", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 80, align: "center", sortable: true },
                        { name: 'Date', index: 'Date', height: 'auto', width: 60, align: "center", sortable: false, hidden: false },
                        { name: 'Number', index: 'Number', height: 'auto', width: 60, align: "center", sortable: false, hidden: false },

                        { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },
                        { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },
                        { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },

                        { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 60, align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: 45, align: "center", sortable: true },
                        { name: 'MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },

                        { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },

                        { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'Edit', index: 'Add', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'Delete', index: 'Add', height: 'auto', width: 50, align: "center", sortable: false },
            ],
            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val(), clearanceCode: $('#MRD_CLEARANCE_CODE').val() },
            pager: jQuery('#divMrdMrdDroppedLetterpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "Dropped Letter List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            cmTemplate: { title: false },
            footerrow: true,
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);

                //Total of Columns
                var MORD_SHARE_AMT_T = $(this).jqGrid('getCol', 'MORD_SHARE_AMT', false, 'sum');
                var STATE_SHARE_AMT_T = $(this).jqGrid('getCol', 'STATE_SHARE_AMT', false, 'sum');
                var TOTAL_AMT_T = $(this).jqGrid('getCol', 'TOTAL_AMT', false, 'sum');

                var MRD_TOTAL_ROADS_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROADS', false, 'sum');
                var MRD_TOTAL_ROAD_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROAD_LENGTH', false, 'sum');
                var MRD_TOTAL_LSB_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB', false, 'sum');
                var MRD_TOTAL_LSB_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB_LENGTH', false, 'sum');

                var MRD_HAB_1000_T = $(this).jqGrid('getCol', 'MRD_HAB_1000', false, 'sum');
                var MRD_HAB_500_T = $(this).jqGrid('getCol', 'MRD_HAB_500', false, 'sum');
                var MRD_HAB_250_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_250_ELIGIBLE', false, 'sum');
                var MRD_HAB_100_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_100_ELIGIBLE', false, 'sum');
                //
                $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });

                $(this).jqGrid('footerData', 'set', { MORD_SHARE_AMT: parseFloat(MORD_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { STATE_SHARE_AMT: parseFloat(STATE_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { TOTAL_AMT: parseFloat(TOTAL_AMT_T).toFixed(4) }, true);

                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROADS: parseFloat(MRD_TOTAL_ROADS_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROAD_LENGTH: parseFloat(MRD_TOTAL_ROAD_LENGTH_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB: parseFloat(MRD_TOTAL_LSB_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB_LENGTH: parseFloat(MRD_TOTAL_LSB_LENGTH_T).toFixed(4) }, true);

                $(this).jqGrid('footerData', 'set', { MRD_HAB_1000: MRD_HAB_1000_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_500: MRD_HAB_500_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_250_ELIGIBLE: MRD_HAB_250_ELIGIBLE_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_100_ELIGIBLE: MRD_HAB_100_ELIGIBLE_T }, true);

                $('#tblMrdDroppedLetterList_rn').html('Sr.<br/>No.');
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")

                }
            },



        });
    }
}

function DownloadDroppedFile(cellvalue) {
    //alert(cellvalue);
    var url = "/MRDProposal/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}



function loadSaveMrdDroppedLetter(param) {
    //$("#dvhdSearch").trigger('click');
    //$('#tblMrdClearanceList').jqGrid('setGridState', 'hidden');
    $('#btnAddMRDDroppedLetter').hide('slow');
    $("#tblMrdDroppedLetterList").jqGrid('setGridState', 'hidden');

    //alert(param);
    $.ajax({
        url: '/MRDProposal/SaveDroppedLetters/' + param,
        type: 'POST',
        data: { User_Action: "A" },
        success: function (jsonData) {
            $('#dvLoadSaveDroppedLetters').html('');
            $('#dvLoadSaveDroppedLetters').html(jsonData);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


function loadEditMrdDroppedLetter(param) {
    //$("#dvhdSearch").trigger('click');

    $('#tblMrdClearanceList').jqGrid('setGridState', 'hidden');

    //alert(param);
    $.ajax({
        url: '/MRDProposal/SaveDroppedLetters/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadSaveDroppedLetters').html('');
            $('#dvLoadSaveDroppedLetters').html(jsonData);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteMrdDroppedLetter(urlparameter) {
    if (confirm("Are you sure you want to delete Dropped Letter details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/MRDProposal/delMrdDroppedLetter/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);

                    $("#dvhdSearch").trigger('click');
                    LoadMrdDroppedLetterGrid();
                    $.unblockUI();
                }
                else {

                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });



    }
    else {
        return false;
    }

}

