/// <reference path="../jquery-1.9.1.js" />
var AgreementNumber;
var ContractorName;
var AgreementCode;
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFilterbankGuarantee'));

    $("#btnListAgreementsBankGuarantee").click(function () {

        if ($("#frmFilterbankGuarantee").valid()) {
            LoadAgreementList();
        }
    });

    $("#ddlYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlYears").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());

    }); //


    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
                    "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());

    });

    $("#dvViewBankDiadlog").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Bank Guarantee Details',

    });

    //show/hide filter search 
    $("#spCollapseIconS").click(function () {

        if ($("#filterForm").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#filterForm").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#filterForm").slideToggle(300);
        }
    });
})

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function LoadAgreementList() {
    $("#tblstAgreementsBankGuarantee").jqGrid('GridUnload');
    var pageWidth = jQuery("#tblstAgreementsBankGuarantee").parent().width() - 100;

    jQuery("#tblstAgreementsBankGuarantee").jqGrid({
        url: '/Agreement/GetAgreementDetailsListForBG',
        datatype: "json",
        mtype: "POST",
        postData: { YearCode: $('#ddlYears option:selected').val(), BlockCode: $('#ddlBlocks option:selected').val(), Package: $('#ddlPackages option:selected').val(), ProposalType: $('#ddlProposalType option:selected').val(), AgreementStatus: $('#ddlAgreementStatus option:selected').val(), AgreementType: $('#ddlAgreementType option:selected').val(), Finalize: $('#ddlFinalize option:selected').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Add', 'View'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: (pageWidth * (5 / 100)), align: "left", sortable: false, hidden: true },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: (pageWidth * (8 / 100)), sortable: true, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: (pageWidth * (15 / 100)), sortable: true, resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: (pageWidth * (13 / 100)), sortable: true, resizable: false, align: 'center' },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: (pageWidth * (10 / 100)), sortable: false, align: "right", resizable: false },
                            { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: (pageWidth * (10 / 100)), sortable: false, align: "right", resizable: false },//, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: (pageWidth * (11 / 100)), sortable: false, align: "left", resizable: false },
                            { name: 'Action', index: 'Action', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false }, /* formatter: FormatColumnFinalize,*/
                            { name: 'View', index: 'View', width: (pageWidth * (5 / 100)), sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
        ],
        pager: jQuery('#dvlstPagerAgreementsBankGuarantee'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;Agreement List",
        height: 'auto',
        width: 'auto',
        // autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        onSelectRow: function (id) {
            var selectedRowId = $('#tblstAgreementsBankGuarantee').jqGrid('getGridParam', 'selrow');
            AgreementNumber = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementNumber');
            ContractorName = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'ContractorName');
            AgreementCode = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementCode');
            //alert(AgreementNumber + " " + ContractorName)
        },
        loadComplete: function () {

            var reccount = $('#tblstAgreementsBankGuarantee').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerAgreementsBankGuarantee_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2."NA"-Not Available  ]');
            }

        }
    })

}

function FormatColumnView(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Bank Guarantee Details' onClick ='ViewBankGuaranteeDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Bank Guarantee Details not available' ></span></td></tr></table></center>";
    }

}
function FormatColView(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Bank Guarantee Details' onClick ='ViewBankGuaranteeDetailsSingle(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Bank Guarantee Details not available' ></span></td></tr></table></center>";
    }

}

function AddBankGuarantee(urlparameter) {
    $("#tblstAgreementsBankGuarantee").jqGrid('setGridState', 'hidden');  //to hide for view of add form

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvViewAgreementMasterBankGuarantee').show('slow');
    $('#dvViewAgreementMasterBankGuarantee').load("/Agreement/AddBankGuarantee", function () {

        //var selectedRowId = $('#tblstAgreementsBankGuarantee').jqGrid('getGridParam', 'selrow');
        //AgreementNumber = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementNumber');
        //ContractorName = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'ContractorName');
        //var AgreementCode = $('#tblstAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementCode');
        // alert(AgreementNumber + " " + AgreementCode)
        $('#bgOperation').html('&nbsp;&nbsp;Add Bank Guarantee/FDR Details');
        $('#bkagreement').text(AgreementNumber);
        $('#lblContName').text(ContractorName);
        $('#AGREEMENT_CODE').val(AgreementCode);

        
        $.unblockUI();
    });
   
}

function EditBankGuarantee(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#dvViewAgreementMasterBankGuarantee').show('slow');
    $('#dvViewAgreementMasterBankGuarantee').load("/Agreement/EditbankGuarantee?urlparameter=" + urlparameter, function () {

        $('#bgOperation').html('&nbsp;&nbsp;Edit Bank Guarantee/FDR Details');
        $('#bkagreement').text(AgreementNumber);
        $('#lblContName').text(ContractorName);
        $('#AGREEMENT_CODE').val(AgreementCode);

        // for edit hide the broesw file option 

        $('#BGFile').hide();

        $.unblockUI();
    })

}
function ViewBankGuaranteeDetailsSingle(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Agreement/ViewBankGuaranteeDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $("#dvViewBankDiadlog").html(data);
            setTimeout(function () {
                $('#bkagreement_dialog').text(AgreementNumber);
                $('#lblContName_dialog').text(ContractorName);
                $("#dvViewBankDiadlog").dialog('open');
            }, 1000);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        },
        complete: function (xhr, status) {

            $.unblockUI();
        }
    })


}


function ViewBankGuaranteeDetails(urlparameter) {

    $("#tblstAgreementsBankGuarantee").jqGrid('setGridState', 'hidden');
    $('#tblstBankGuarantee').jqGrid('GridUnload');

    var pageWidth = jQuery("#tblstBankGuarantee").parent().width() - 100;
    $grid = $("#tblstBankGuarantee");
    // alert($grid[0].id)
    jQuery("#tblstBankGuarantee").jqGrid({
        url: '/Agreement/GetAgreementDetailsListForContractor/' + urlparameter,
        datatype: "json",
        mtype: "POST",
        postData: { YearCode: $('#ddlYears option:selected').val(), BlockCode: $('#ddlBlocks option:selected').val(), Package: $('#ddlPackages option:selected').val(), ProposalType: $('#ddlProposalType option:selected').val(), AgreementStatus: $('#ddlAgreementStatus option:selected').val(), AgreementType: $('#ddlAgreementType option:selected').val(), Finalize: $('#ddlFinalize option:selected').val() },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Edit', 'Bank Guarantee Amount', 'Bank Guarantee Status', 'View', 'Download'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: (pageWidth * (5 / 100)), align: "left", sortable: false, hidden: true },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: (pageWidth * (8 / 100)), sortable: false, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: (pageWidth * (15 / 100)), sortable: false, resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: (pageWidth * (7 / 100)), sortable: false, resizable: false, align: 'center' },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: (pageWidth * (6 / 100)), sortable: false, align: "right", resizable: false },
                            { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: (pageWidth * (6 / 100)), sortable: false, align: "right", resizable: false },//, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: (pageWidth * (8 / 100)), sortable: false, align: "left", resizable: false },
                            { name: 'Action', index: 'Action', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false }, /* formatter: FormatColumnFinalize,*/
                            { name: 'BGAmount', index: 'BGAmount', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: 'right', resizable: false },
                            { name: 'BGStatus', index: 'BGStatus', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false },
                            { name: 'View', index: 'View', width: (pageWidth * (5 / 100)), sortable: false, formatter: FormatColView, align: "center", resizable: false },
                            { name: 'Download', index: 'Download', width: (pageWidth * (5 / 100)), sortable: false, align: "center", resizable: false },

                          // { name: 'Status', index: 'Status', width: (pageWidth * (5 / 100)), sortable: false, align: "center", resizable: false, hidden: true },
        ],
        pager: jQuery('#dvlstPagerBankGuarantee'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;Bank Guarantee/FDR List",
        height: 'auto',
        width: 'auto',
        // autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        onSelectRow: function (id) {
        },
        //toppager: true,
        //toolbar:[true,"top"],
        loadComplete: function () {

            $('#dvListBankGuarantee').show('slow');
            // jQuery("#tblstAgreementsBankGuarantee").trigger('reloadGrid');
            var reccount = $('#tblstAgreementsBankGuarantee').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerBankGuarantee_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2."NA"-Not Available  ]');
            }

        },
        onHeaderClick: function () {
            //$(this).closest('.ui-jqgrid').hide('blind', {}, 500);
            $('#dvListBankGuarantee').hide('slow');
            //            alert($('#gview_tblstAgreementsBankGuarantee .ui-jqgrid-titlebar-close>span').attr('class'));
            $('#gview_tblstAgreementsBankGuarantee .ui-jqgrid-titlebar-close>span').trigger('click');
        }
    });
    $('#gview_' + $.jgrid.jqID($grid[0].id) + ' .ui-jqgrid-titlebar-close>span')
               .removeClass('ui-icon-circle-triangle-n')
               .addClass('ui-icon-closethick');
}