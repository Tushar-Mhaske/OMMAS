$(document).ready(function () {
    if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {

        $('#MrdClearenceLetterSearchDetails').load('/Proposal/SearchMrdClearenceLetter');
        $('#MrdClearenceLetterSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddMrdClearenceLetter');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#dvAccordianMRD").is(":visible")) {
            CloseClearanceRevisionDetails();
        }
        if ($("#MrdClearenceLetterSearchDetails").is(":visible")) {
            $('#MrdClearenceLetterSearchDetails').hide('slow');
        }

        $('#MrdClearenceLetterAddDetails').load("/Proposal/AddEditMrdClearenceLetter");
        $('#MrdClearenceLetterAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();
        setTimeout(function () {
            $("#ddlState").val($('#ddlStateSerach option:selected').val());
            $("#ddlState").trigger('change');
            $("#ddlPhaseYear").val($('#ddlPhaseYearSerach option:selected').val());
            $("#ddlBatch").val($('#ddlBatchSerach option:selected').val());
            setTimeout(function () {
                $("#ddlAgency").val($('#ddlAgencySerach option:selected').val());
            }, 1000);
        }, 500);
    });

    $('#btnSearch').click(function (e) {
        if ($("#dvAccordianMRD").is(":visible")) {
            CloseClearanceRevisionDetails();
        }
        if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
            $('#MrdClearenceLetterAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {

            $('#MrdClearenceLetterSearchDetails').load('/Proposal/SearchMrdClearenceLetter', function () {
                var data = $('#tblMrdClearenceLetter').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {
                    $('#ddlStateSerach').val(data.stateCode);
                    $('#ddlAgencySerach').val(data.agency);
                    $('#ddlPhaseYearSerach').val(data.year);
                    $('#ddlBatchSerach').val(data.batch);
                    $('#ddlCollaborationSerach').val(data.collaboration);
                }
                $('#MrdClearenceLetterSearchDetails').show('slow');
            });
        }
        $.unblockUI();
    });



    //add accordion
    $(function () {
        $("#dvAccordianMRD").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

});





function LoadMrdClearenceLetterGrid() {
    if ($("#frmSearchMrdClearenceLetter").valid()) {
        jQuery("#tblMrdClearenceLetter").jqGrid({
            url: '/Proposal/GetMrdClearenceLetterList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Sanctioned Year', 'Batch', 'Agency', 'Collaboration', 'Clerance Letter Number', 'Clearance Date', 'Final Revised Clearance Letter Number', 'Final Revised Clearance Date', 'Number of  Roads', 'Total Road Length (in KM.)', 'Number of  Bridges', 'Total Bridge  Length (in Mtr.)', 'Road MoRD share (in Cr.)', 'Road State share (in Cr.)',
                       'Total Road Sanctioned Amount (in Cr.)', 'Bridge MoRD share (in Cr.)', 'Bridge  State share (in Cr.)', 'Total Bridge Sanctioned Amount (in Cr.)', 'Total MoRD share (in Cr.)', 'Total State share (in Cr.)', 'Total Sanctioned Amount (in Cr.)',
                        'Hab >1000', 'Hab >500', 'Hab >250', 'Hab >100','Clearance Status', 'Download File', 'Clearance Revision', 'Edit', 'Delete','View'], //26
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'Collaboration', index: 'Collaboration', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'CleranceLetterNo', index: 'CleranceLetterNo', height: 'auto', width: 100, align: "center", sortable: false }, //new1
                        { name: 'MRD_CLEARANCE_DATE', index: 'MRD_CLEARANCE_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_CLEARANCE_REVISED_Number', index: 'MRD_CLEARANCE_REVISED_Number', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_CLEARANCE_REVISED_DATE', index: 'MRD_CLEARANCE_REVISED_DATE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROADS', index: 'MRD_TOTAL_ROADS', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_ROAD_LENGTH', index: 'MRD_TOTAL_ROAD_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB', index: 'MRD_TOTAL_LSB', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_LSB_LENGTH', index: 'MRD_TOTAL_LSB_LENGTH', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_MORD_SHARE_AMT', index: 'MRD_ROAD_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_STATE_SHARE_AMT', index: 'MRD_ROAD_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_ROAD_TOTAL_AMT', index: 'MRD_ROAD_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_MORD_SHARE_AMT', index: 'MRD_LSB_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_STATE_SHARE_AMT', index: 'MRD_LSB_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_LSB_TOTAL_AMT', index: 'MRD_LSB_TOTAL_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_MORD_SHARE_AMT', index: 'MRD_TOTAL_MORD_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_STATE_SHARE_AMT', index: 'MRD_TOTAL_STATE_SHARE_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_TOTAL_SANCTIONED_AMT', index: 'MRD_TOTAL_SANCTIONED_AMT', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_1000', index: 'MRD_HAB_1000', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_500', index: 'MRD_HAB_500', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_250_ELIGIBLE', index: 'MRD_HAB_250_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_HAB_100_ELIGIBLE', index: 'MRD_HAB_100_ELIGIBLE', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'MRD_CLEARANCE_STATUS', index: 'MRD_CLEARANCE_STATUS', height: 'auto', width: 70, align: "center", sortable: false },
                        { name: 'DownLoad', index: 'DownLoad', height: 'auto', width: 50, align: "left", sortable: false, hidden: false },
                        { name: 'AddClearanceRevision', index: 'AddClearanceRevision', height: 'auto', width: 50, align: "left", sortable: false, hidden: false },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 50, align: "center", sortable: false, hidden: false },
                        { name: 'Delete', index: 'Delete', width: 50, resize: false },
                        { name: 'View', index: 'View', width: 50, resize: false }

            ],
            postData: { stateCode: $('#ddlStateSerach option:selected').val(), agency: $('#ddlAgencySerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val(), batch: $('#ddlBatchSerach option:selected').val(), collaboration: $('#ddlCollaborationSerach option:selected').val() },
            pager: jQuery('#divMrdClearenceLetterpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "Clearance List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            cmTemplate:{title:false},
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);

                //Total of Columns
                var CleranceLetterNo_T = $(this).jqGrid('getCol', 'CleranceLetterNo', false, 'sum');
                var MRD_TOTAL_ROADS_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROADS', false, 'sum');
                var MRD_TOTAL_ROAD_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_ROAD_LENGTH', false, 'sum');
                var MRD_TOTAL_LSB_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB', false, 'sum');
                var MRD_TOTAL_LSB_LENGTH_T = $(this).jqGrid('getCol', 'MRD_TOTAL_LSB_LENGTH', false, 'sum');
                var MRD_ROAD_MORD_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_ROAD_MORD_SHARE_AMT', false, 'sum');
                var MRD_ROAD_STATE_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_ROAD_STATE_SHARE_AMT', false, 'sum');
                var MRD_ROAD_TOTAL_AMT_T = $(this).jqGrid('getCol', 'MRD_ROAD_TOTAL_AMT', false, 'sum');
                var MRD_LSB_MORD_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_LSB_MORD_SHARE_AMT', false, 'sum');
                var MRD_LSB_STATE_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_LSB_STATE_SHARE_AMT', false, 'sum');
                var MRD_LSB_TOTAL_AMT_T = $(this).jqGrid('getCol', 'MRD_LSB_TOTAL_AMT', false, 'sum');
                var MRD_TOTAL_MORD_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_TOTAL_MORD_SHARE_AMT', false, 'sum');
                var MRD_TOTAL_STATE_SHARE_AMT_T = $(this).jqGrid('getCol', 'MRD_TOTAL_STATE_SHARE_AMT', false, 'sum');
                var MRD_TOTAL_SANCTIONED_AMT_T = $(this).jqGrid('getCol', 'MRD_TOTAL_SANCTIONED_AMT', false, 'sum');
                var MRD_HAB_1000_T = $(this).jqGrid('getCol', 'MRD_HAB_1000', false, 'sum');
                var MRD_HAB_500_T = $(this).jqGrid('getCol', 'MRD_HAB_500', false, 'sum');
                var MRD_HAB_250_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_250_ELIGIBLE', false, 'sum');
                var MRD_HAB_100_ELIGIBLE_T = $(this).jqGrid('getCol', 'MRD_HAB_100_ELIGIBLE', false, 'sum');
                //
                $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
                $(this).jqGrid('footerData', 'set', { CleranceLetterNo: CleranceLetterNo_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROADS: MRD_TOTAL_ROADS_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_ROAD_LENGTH: parseFloat(MRD_TOTAL_ROAD_LENGTH_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB: MRD_TOTAL_LSB_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_LSB_LENGTH: parseFloat(MRD_TOTAL_LSB_LENGTH_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_ROAD_MORD_SHARE_AMT: parseFloat(MRD_ROAD_MORD_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_ROAD_STATE_SHARE_AMT: parseFloat(MRD_ROAD_STATE_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_ROAD_TOTAL_AMT: parseFloat(MRD_ROAD_TOTAL_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_LSB_MORD_SHARE_AMT: parseFloat(MRD_LSB_MORD_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_LSB_STATE_SHARE_AMT: parseFloat(MRD_LSB_STATE_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_LSB_TOTAL_AMT: parseFloat(MRD_LSB_TOTAL_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_MORD_SHARE_AMT: parseFloat(MRD_TOTAL_MORD_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_STATE_SHARE_AMT: parseFloat(MRD_TOTAL_STATE_SHARE_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_TOTAL_SANCTIONED_AMT: parseFloat(MRD_TOTAL_SANCTIONED_AMT_T).toFixed(4) }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_1000: MRD_HAB_1000_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_500: MRD_HAB_500_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_250_ELIGIBLE: MRD_HAB_250_ELIGIBLE_T }, true);
                $(this).jqGrid('footerData', 'set', { MRD_HAB_100_ELIGIBLE: MRD_HAB_100_ELIGIBLE_T }, true);

                $('#tblMrdClearenceLetter_rn').html('Sr.<br/>No.');

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

function EditClearanceDetail(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Proposal/EditMrdClearenceLetter/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#MrdClearenceLetterSearchDetails").is(":visible")) {
                $('#MrdClearenceLetterSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#MrdClearenceLetterAddDetails").html(data);
            $("#MrdClearenceLetterAddDetails").show('slow');

            if ($("#dvAccordianMRD").is(":visible")) {
                CloseClearanceRevisionDetails();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteClearanceDetail(urlparameter) {
    if (confirm("Are you sure you want to delete Clearance details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Proposal/DeleteMrdClearenceLetter/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#MrdClearenceLetterSearchDetails").is(":visible")) {
                    //    $('#btnMrdClearenceLetterSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblMrdClearenceLetter').trigger('reloadGrid');
                    //}
                    //$("#MrdClearenceLetterAddDetails").load("/Proposal/AddEditMrdClearenceLetter");

                    if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
                        $('#MrdClearenceLetterAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
                        $("#MrdClearenceLetterSearchDetails").show('slow');
                        $('#tblMrdClearenceLetter').trigger('reloadGrid');
                    }
                    else {
                        $('#tblMrdClearenceLetter').trigger('reloadGrid');
                    }
                    if ($("#dvAccordianMRD").is(":visible")) {
                        CloseClearanceRevisionDetails();
                    }
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

function DownLoadClearancePDFFile(paramurl) {

    url = "/Proposal/DownloadMrdClearenceLetter/" + paramurl,

    //window.location = url;


    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("File not available.");
                return false;
            }
            else {
                window.location = url;
            }
        }
    });

    //window.location = paramurl;
}
//returns the view of Physical progress of Proposal
function AddClearanceRevisionDetail(urlparameter) {
   
   
    $("#dvAccordianMRD div").html("");
    $("#dvAccordianMRD h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Clearance Revision Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevisionDetails();" /></a>'
            );

    $('#dvAccordianMRD').show('fold', function () {
        blockPage();
        $("#divMrdRevisionClearance").load('/Proposal/ListMrdClearenceRevisionDetail?ClearanceCodeEncrypted=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divMrdRevisionClearance').show('slow');
        $("#divMrdRevisionClearance").css('height', 'auto');
    });
    $("#tblMrdClearenceLetter").jqGrid('setGridState', 'hidden');
    $('#dvhdSearch').trigger('click');
    $('#btnAdd').hide();

    if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
        $('#MrdClearenceLetterAddDetails').html('');
        $('#MrdClearenceLetterAddDetails').hide('slow');
        $('#btnSearch').hide();        
    }
    if (!$("#MrdClearenceLetterSearchDetails").is(":visible")) {
        $("#MrdClearenceLetterSearchDetails").show('slow');
    }
}

function DownLoadListViewClearancePDFFile(urlparameter) {

    $("#dvAccordianMRD div").html("");
    $("#dvAccordianMRD h3").html(
            "<a href='#' style= 'font-size:.9em;' >Clerance File Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevisionDetails();" /></a>'
            );

    $('#dvAccordianMRD').show('fold', function () {
        blockPage();
        $("#divMrdRevisionClearance").load('/Proposal/ListMrdClearenceFiles?ClearanceCodeEncrypted=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divMrdRevisionClearance').show('slow');
        $("#divMrdRevisionClearance").css('height', 'auto');
    });

    //$('#divFileDownloadDetail').show('slow');
    //LoadClearanceFileLetterList(urlparameter);
    $("#tblMrdClearenceLetter").jqGrid('setGridState', 'hidden');
    $('#dvhdSearch').trigger('click');
    $('#btnAdd').hide();
    if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
        $('#MrdClearenceLetterAddDetails').html('');
        $('#MrdClearenceLetterAddDetails').hide('slow');
        $('#btnSearch').hide();
    }

}

function ViewClearanceDetail(urlparameter) {

    $("#dvAccordianMRD div").html("");
    $("#dvAccordianMRD h3").html(
            "<a href='#' style= 'font-size:.9em;' >View Clearance Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseClearanceRevisionDetails();" /></a>'
            );

    $('#dvAccordianMRD').show('fold', function () {
        blockPage();
        $("#divMrdRevisionClearance").load('/Proposal/ViewMrdClearenceDetail/' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divMrdRevisionClearance').show('slow');
        $("#divMrdRevisionClearance").css('height', 'auto');
    });

    //$('#divFileDownloadDetail').show('slow');
    //LoadClearanceFileLetterList(urlparameter);
    $("#tblMrdClearenceLetter").jqGrid('setGridState', 'hidden');
    $('#dvhdSearch').trigger('click');
    $('#btnAdd').hide();
    if ($("#MrdClearenceLetterAddDetails").is(":visible")) {
        $('#MrdClearenceLetterAddDetails').hide('slow');
        $('#btnSearch').hide();
    }

}

function CloseClearanceRevisionDetails() {

    $("#tblFileDownloadClearanceDetail").jqGrid('GridUnload');
    $("#dvAccordianMRD").hide('slow');
    $("#divMrdRevisionClearance").hide('slow');
    $('#divFileDownloadDetail').hide('slow');
    $("#tblMrdClearenceLetter").jqGrid('setGridState', 'visible');
    ShowFilter();
}

function ShowFilter() {
    //$("#divSearchPropAddCost").show('slow');
    $('#btnAdd').show();
    $("#dvhdSearch").toggleClass("ui-icon-circle-triangle-s");
    $('#dvhdSearch').trigger('click');

}

function EditClearanceRevisionDetailPartial(urlparameter) {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Proposal/AddClearanceRevisionDetailsPartial/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#MrdClearenceLetterSearchDetails").is(":visible")) {
                $('#MrdClearenceLetterSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#MrdClearenceLetterAddDetails").html(data);
            $("#MrdClearenceLetterAddDetails").show('slow');

            if ($("#dvAccordianMRD").is(":visible")) {
                CloseClearanceRevisionDetails();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })

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
                    $('#frmSearchMrdClearenceLetter').trigger('reloadGrid');
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