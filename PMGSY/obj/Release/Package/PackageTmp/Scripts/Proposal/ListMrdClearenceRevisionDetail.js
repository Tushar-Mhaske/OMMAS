$(document).ready(function () {
    LoadMrdClearenceRevisionLetterGrid();
    //add accordion
    $("#tabs").tabs();

    $(function () {
        $("#accordiondivClearancRevsionAddEdit").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnCreateRevision').click(function () {
        AddEditClearanceRevisionDetail($('#hdClearanceCodeEncrypted').val());
    });

    

});
function LoadMrdClearenceRevisionLetterGrid() {
    jQuery("#tblFileDownloadClearancedRevisionDetail").jqGrid('GridUnload');
    jQuery("#tblFileDownloadClearancedRevisionDetail").jqGrid({
        url: '/Proposal/GetMrdClearenceRevisionList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Sanctioned Year', 'Batch', 'Agency', 'Collaboration', 'Revision Date', 'Number of  Roads', 'Number of  Bridges', 'Road MoRD share (in Cr.)', 'Road State share (in Cr.)',
                   'Total Road Sanctioned Amount (in Cr.)', 'Bridge MoRD share (in Cr.)', 'Bridge  State share (in Cr.)', 'Total Bridge Sanctioned Amount (in Cr.)', 'Total MoRD share (in Cr.)', 'Total State share (in Cr.)', 'Total Sanctioned Amount (in Cr.)',
                   'Total Road Length (in KM.)', 'Total Bridge  Length (in Mtr.)', 'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100', 'Download File', 'Edit', 'Delete','View'], //27
        colModel: [

                    { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                    { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "center", sortable: true },
                    { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                    { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                    { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: 100, align: "center", sortable: true },
                    { name: 'MRD_CLEARANCE_DATE', index: 'MRD_CLEARANCE_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: 70, align: "center", sortable: false, hidden: false },
                    { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_ROAD_MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_ROAD_STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_ROAD_TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_MORD_SHARE_AMT', index: 'MRD_LSB_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_STATE_SHARE_AMT', index: 'MRD_LSB_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_TOTAL_AMT', index: 'MRD_LSB_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_MORD_SHARE_AMT', index: 'MRD_TOTAL_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false},
                    { name: 'MRD_TOTAL_STATE_SHARE_AMT', index: 'MRD_TOTAL_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_SANCTIONED_AMT', index: 'MRD_TOTAL_SANCTIONED_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "left", sortable: false, hidden: false },
                    { name: 'Edit', index: 'Edit', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },
                    { name: 'Delete', index: 'Delete', width: 50, resize: true },
                    { name: 'View', index: 'View', width: 50, resize: true }

        ],
        postData: { "ClearanceCodeEncrypted": $('#hdClearanceCodeEncrypted').val() },
        pager: jQuery('#divPagerFileDownloadCleranceRevisionDetail'),
        rowNum: 4,
        rowList: [4, 8, 16, 32],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'TEMP',
        sortorder: "desc",
        caption: "Clearance Revision List",
        height: 'auto',
        autowidth: true,
        //width:'250',
        shrinkToFit: false,
        footerrow:true,
        rownumbers: true,
        loadComplete: function (data) {
         
            //if (data.records == 1) {
                $('#tblMrdClearenceLetter').trigger('reloadGrid');
            //}
                $("#tblFileDownloadClearancedRevisionDetail").jqGrid('setGridWidth', $("#MrdClearenceLetterSearchDetails").width() - 200, true);

            //Total of Columns
                
                var MRD_TOTAL_ROADS = $(this).jqGrid('getCol', 'MRD_TOTAL_ROADS', false, 'sum');
                var MRD_TOTAL_LSB = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB', false, 'sum');
                var MRD_TOTAL_LSB_SANCTION_AMT = $(this).jqGrid('getCol', 'MRD_LSB_TOTAL_AMT', false, 'sum');
                var MRD_TOTAL_MORD_SHARE_AMT = $(this).jqGrid('getCol', 'MRD_TOTAL_MORD_SHARE_AMT', false, 'sum');
                var MRD_TOTAL_STATE_SHARE_AMT = $(this).jqGrid('getCol', 'MRD_TOTAL_STATE_SHARE_AMT', false, 'sum');
                var MRD_TOTAL_SANC_SHARE_AMT = $(this).jqGrid('getCol', 'MRD_TOTAL_SANCTIONED_AMT', false, 'sum');

                $(this).jqGrid('footerData', 'set', { MRD_CLEARANCE_DATE: '<b>Total</b>' });
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROADS: MRD_TOTAL_ROADS }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB: MRD_TOTAL_LSB }, true);
                $(this).jqGrid('footerData', 'set', { MRD_LSB_TOTAL_AMT: parseFloat(MRD_TOTAL_LSB_SANCTION_AMT).toFixed(2) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_MORD_SHARE_AMT: parseFloat(MRD_TOTAL_MORD_SHARE_AMT).toFixed(2) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_STATE_SHARE_AMT: parseFloat(MRD_TOTAL_STATE_SHARE_AMT).toFixed(2) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_SANCTIONED_AMT: parseFloat(MRD_TOTAL_SANC_SHARE_AMT).toFixed(2) }, true);

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

function LoadMrdOriginalClearenceLetterGrid() {
    jQuery("#tblstClearanceDetails").jqGrid('GridUnload');
    jQuery("#tblstClearanceDetails").jqGrid({
        url: '/Proposal/GetOriginalMrdClearenceList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Sanctioned Year', 'Batch', 'Agency', 'Collaboration', 'Revision Date', 'Number of  Roads', 'Number of  Bridges', 'Road MoRD share (in Cr.)', 'Road State share (in Cr.)',
                   'Total Road Sanctioned Amount (in Cr.)', 'Bridge MoRD share (in Cr.)', 'Bridge  State share (in Cr.)', 'Total Bridge Sanctioned Amount (in Cr.)', 'Total MoRD share (in Cr.)', 'Total State share (in Cr.)', 'Total Sanctioned Amount (in Cr.)',
                   'Total Road Length (in KM.)', 'Total Bridge  Length (in Mtr.)', 'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100', 'Download File', 'Edit', 'Delete', 'View'], //27
        colModel: [

                    { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                    { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "center", sortable: true },
                    { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                    { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                    { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: 100, align: "center", sortable: true },
                    { name: 'MRD_CLEARANCE_DATE', index: 'MRD_CLEARANCE_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: 70, align: "center", sortable: false, hidden: false },
                    { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_ROAD_MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_ROAD_STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_ROAD_TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_MORD_SHARE_AMT', index: 'MRD_LSB_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_STATE_SHARE_AMT', index: 'MRD_LSB_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_LSB_TOTAL_AMT', index: 'MRD_LSB_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_MORD_SHARE_AMT', index: 'MRD_TOTAL_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_STATE_SHARE_AMT', index: 'MRD_TOTAL_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_SANCTIONED_AMT', index: 'MRD_TOTAL_SANCTIONED_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                    { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false, hidden: true },
                    { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "left", sortable: false, hidden: true },
                    { name: 'Edit', index: 'Edit', height: 'auto', width: 50, align: "center", sortable: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 50, resize: true, hidden: true },
                    { name: 'View', index: 'View', width: 50, resize: true, hidden: true }

        ],
        postData: { "ClearanceCodeEncrypted": $('#hdClearanceCodeEncrypted').val() },
        pager: jQuery('#dvpagerClearanceDetails'),
        rowNum: 4,
        rowList: [4, 8, 16, 32],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'TEMP',
        sortorder: "desc",
        caption: "Original Clearance List",
        height: 'auto',
        autowidth: true,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        loadComplete: function (data) {

            //if (data.records == 1) {
            $('#tblMrdClearenceLetter').trigger('reloadGrid');
            //}
            $("#tblstClearanceDetails").jqGrid('setGridWidth', $("#MrdClearenceLetterSearchDetails").width() - 200, true);


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

function EditClearanceRevisionDetail(urlparameter) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //$.ajax({
    //    type: 'GET',
    //    url: '/Proposal/EditMrdClearenceRevsionDetail/' + urlparameter,
    //    dataType: "html",
    //    async: false,
    //    cache: false,
    //    success: function (data) {
    //        $("#dvhdAddNewMrdCleranceRevisionDetail").html('');
    //        $("#dvAddNewMrdCleranceRevisionDetail").html('');
    //        $("#dvAddNewMrdCleranceRevisionDetail").html(data);
    //        $.unblockUI();
    //    },
    //    error: function (xhr, ajaxOptions, thrownError) {
    //        $.unblockUI();
    //    }
    //})
  
    $("#accordiondivClearancRevsionAddEdit h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Clearance Revision Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevsionAddEditDetails();" /></a>'
            );

    $('#accordiondivClearancRevsionAddEdit').show('fold', function () {
        blockPage();
        $("#divClearancRevsionAddEdit").load('/Proposal/EditMrdClearenceRevsionDetail/' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divClearancRevsionAddEdit').show('slow');
        $("#divClearancRevsionAddEdit").css('height', 'auto');
    });


    $('#divbtnCreateRevision').hide('slow');
}

function DeleteClearancRevisioneDetail(urlparameter) {
    if (confirm("Are you sure you want to delete Clearance Revision details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Proposal/DeleteMrdClearenceRevisionLetter/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);


                    //if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
                    //    $('#MrdClearenceLetterAddDetails').hide('slow');
                    //    $('#btnSearch').hide();
                    //    $('#btnAdd').show();
                    //}

                    //if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                    //    $("#MrdClearenceLetterSearchDetails").show('slow');
                    //    $('#tblMrdClearenceLetter').trigger('reloadGrid');
                    //}
                    //else {
                    //    $('#tblMrdClearenceLetter').trigger('reloadGrid');
                    //}
                    //if ($("#dvAccordianMRD").is(":visible")) {
                    //    CloseClearanceRevisionDetails();
                    //}
                    $('#tblFileDownloadClearancedRevisionDetail').trigger('reloadGrid');
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

function CloseClearanceRevsionAddEditDetails() {

    // $("#tblFileDownloadClearanceDetail").jqGrid('GridUnload');
    $("#accordiondivClearancRevsionAddEdit").hide('slow');
    $("#tblFileDownloadClearancedRevisionDetail").jqGrid('setGridState', 'visible');
    $('#divbtnCreateRevision').show('slow'); //button Create Revision show

}

function AddEditClearanceRevisionDetail(urlparameter) {
    
    //var id = urlparameter.replace(/\s/g, '');;
    var id = urlparameter.trim();
    $("#accordiondivClearancRevsionAddEdit div").html("");
    $("#accordiondivClearancRevsionAddEdit h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Clearance Revision Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevsionAddEditDetails();" /></a>'
            );

    $('#accordiondivClearancRevsionAddEdit').show('fold', function () {
        blockPage();
       
        $("#divClearancRevsionAddEdit").load('/Proposal/AddEditMrdClearenceRevsionDetail/' + id, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divClearancRevsionAddEdit').show('slow');
        $("#divClearancRevsionAddEdit").css('height', 'auto');
    });
 

    $('#divbtnCreateRevision').hide('slow');
}

function DownLoadListViewClearanceRevisionPDFFile(urlparameter) {

    //var id = urlparameter.replace(/\s/g, '');;
    var id = urlparameter.trim();
    $("#accordiondivClearancRevsionAddEdit div").html("");
    $("#accordiondivClearancRevsionAddEdit h3").html(
            "<a href='#' style= 'font-size:.9em;' >Clearance Revision List Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevsionAddEditDetails();" /></a>'
            );

    $('#accordiondivClearancRevsionAddEdit').show('fold', function () {
        blockPage();

        $("#divClearancRevsionAddEdit").load('/Proposal/ListMrdClearenceFiles?ClearanceCodeEncrypted=' + id, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divClearancRevsionAddEdit').show('slow');
        $("#divClearancRevsionAddEdit").css('height', 'auto');
    });

    $('#divbtnCreateRevision').hide('slow');
}

function ViewClearanceRevisionDetail(urlparameter) {

    //var id = urlparameter.replace(/\s/g, '');;
    var id = urlparameter.trim();
    $("#accordiondivClearancRevsionAddEdit div").html("");
    $("#accordiondivClearancRevsionAddEdit h3").html(
            "<a href='#' style= 'font-size:.9em;' >View Clearance Revision Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevsionAddEditDetails();" /></a>'
            );

    $('#accordiondivClearancRevsionAddEdit').show('fold', function () {
        blockPage();

        $("#divClearancRevsionAddEdit").load('/Proposal/ViewMrdClearenceRevisionDetail/' + id, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divClearancRevsionAddEdit').show('slow');
        $("#divClearancRevsionAddEdit").css('height', 'auto');
    });

    $('#divbtnCreateRevision').hide('slow');
}
