/// <reference path="../jquery-1.9.1-vsdoc.js" />
var AgreementNumber;
var ContractorName;
var AgreementCode;
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmFilterbankGuaranteeExpired'));

    setTimeout(LoadExpiredAgreementList, 1000);

    $("#btnListExpAgreementsBankGuarantee").click(function () {
        if ($('#dvBankGuaranteeUploads').is(":visible"))
        {
            $('#dvBankGuaranteeUploads').hide();
        }
        if ($("#frmFilterbankGuaranteeExpired").valid()) {
            LoadExpiredAgreementList();
        }
    });

    $("#ddYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddYears").find(":selected").val() },
                    "#ddPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddYears option:selected').val() + "&blockCode=" + $('#ddBlocks option:selected').val());
    }); //


    $("#ddBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddBlocks").find(":selected").val() },
                    "#ddPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddYears option:selected').val() + "&blockCode=" + $('#ddBlocks option:selected').val());
    });

    $('#ddldistricts').change(function () {
        FillInCascadeDropdown({ districtCode: $("#ddldistricts").find(":selected").val() },
                 "#ddBlocks", "/Agreement/PopulateBlocks");
    });

    $("#dvViewBankDiadlog").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Bank Guarantee Details',

    });

    //show/hide filter search 
    $("#exCollapseIconS").click(function () {

        if ($("#exfilterForm").is(":visible")) {
            $("#exCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#exfilterForm").slideToggle(300);
        }
        else {
            $("#exCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#exfilterForm").slideToggle(300);
        }
    });


    
    $('#ddActiveStatus > option').each(function () {
        var item = $(this).val();
        if (item !== 'select')
        {
            var tootipText = MouseHover(item);
            $(this).attr('title', tootipText);
        }

    })

});

function MouseHover(item)
{
    switch (item)
    {
        case "E":
            return "Bank Guarantee/FDR expiring in current month";
            break;
        case "A":
             return "Active Bank Guarantee/FDR"
             break;
        case "T":
            return "Expired Bank Guarentee/FDR";
        case "0":
            return "All"
        default:
            return ""
    }
}



function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function LoadExpiredAgreementList() {
    $("#tblstExpAgreementsBankGuarantee").jqGrid('GridUnload');
    var pageWidth = jQuery("#tblstExpAgreementsBankGuarantee").parent().width() - 100;

    jQuery("#tblstExpAgreementsBankGuarantee").jqGrid({
        url: '/Agreement/GetExpBankGuaranteeList',
        datatype: "json",
        mtype: "POST",
        postData: { YearCode: $('#ddYears option:selected').val(), District: $('#ddldistricts option:selected').val(), BlockCode: $('#ddBlocks option:selected').val(), Package: $('#ddPackages option:selected').val(), ProposalType: $('#ddProposalType option:selected').val(), AgreementStatus: $('#ddAgreementStatus option:selected').val(), ActiveStatus: $('#ddActiveStatus option:selected').val() },
        colNames: ['AgreementCode', 'District', 'Agreement Number', 'Contractor Name', 'PAN', 'Agreement Date', 'Agreement Amount', 'Expiry Date', 'Bank', 'View'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: (pageWidth * (5 / 100)), align: "left", sortable: false, hidden: true },
                            { name: 'District', index: 'District', width: (pageWidth * (6 / 100)), sortable: true, resizable: false },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: (pageWidth * (9 / 100)), sortable: false, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: (pageWidth * (11 / 100)), sortable: false, resizable: false },
                            { name: 'PAN', index: 'PAN', height: 'auto', width: (pageWidth * (6 / 100)), sortable: false, align: "center", resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: (pageWidth * (8 / 100)), sortable: false, resizable: false, align: 'center' },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: (pageWidth * (7 / 100)), sortable: false, align: "right", resizable: false },
                            { name: 'ExpiryDate', index: 'ExpiryDate', height: 'auto', width: (pageWidth * (8 / 100)), sortable: false, align: "center", resizable: false },//, hidden: isHiddenMaintenenceCost },
                            { name: 'Bank', index: 'Bank', height: 'auto', width: (pageWidth * (8 / 100)), sortable: false, align: "left", resizable: false },
                            { name: 'View', index: 'View', width: (pageWidth * (5 / 100)), sortable: false, formatter: FormatColumnView, align: "center", resizable: false },

                        //  { name: 'Action', index: 'Action', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false }, /* formatter: FormatColumnFinalize,*/
                        //  { name: 'Status', index: 'Status', width: (pageWidth * (5 / 100)), sortable: false, align: "center", resizable: false, hidden: true },
        ],
        pager: jQuery('#dvlstPagerExpAgreementsBankGuarantee'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Bank Guarantee/FDR List",
        height: 'auto',
        width: 'auto',
        // autowidth: true,
        rownumbers: true,
        hidegrid: true,
        sortname: 'District',
        sortorder: "asc",
        onSelectRow: function (id) {
            //var selectedRowId = $('#tblstExpAgreementsBankGuarantee').jqGrid('getGridParam', 'selrow');
            //AgreementNumber = $('#tblstExpAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementNumber');
            //ContractorName = $('#tblstExpAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'ContractorName');
            //AgreementCode = $('#tblstExpAgreementsBankGuarantee').jqGrid('getCell', selectedRowId, 'AgreementCode');
            //alert(AgreementNumber + " " + ContractorName)
        },
        loadComplete: function () {
            var reccount = $('#tblstExpAgreementsBankGuarantee').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerAgreementsBankGuarantee_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2."NA"-Not Available  ]');
            }

        }
    });

}

function FormatColumnView(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Bank Guarantee Details' onClick ='ViewBankGuaranteeUploadDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Bank Guarantee Details not available' ></span></td></tr></table></center>";
    }

}

function ViewBankGuaranteeUploadDetails(urlparameter)
{
    if ($('#dvBankGuaranteeUploads').is(":hidden")) {
        $('#dvBankGuaranteeUploads').show();
    }
    $("#tblstExpAgreementsBankGuarantee").jqGrid('setGridState', 'hidden');
    $('#tblstBankGuaranteeUploads').jqGrid('GridUnload');

    var pageWidth = jQuery("#tblstBankGuaranteeUploads").parent().width() - 100;
    $grid = $("#tblstBankGuaranteeUploads");
    // alert($grid[0].id)
    jQuery("#tblstBankGuaranteeUploads").jqGrid({
        url: '/Agreement/GetAgreementDetailsListForContractor/' + urlparameter,
        datatype: "json",
        mtype: "POST",
        postData: { },
        colNames: ['AgreementCode', 'Agreement Number', 'Contractor Name','Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status',/*'Change Status',*/'Edit', 'Bank Guarantee Amount', 'Bank Guarantee Status', 'View', 'Download'],
        colModel: [
                            { name: 'AgreementCode', index: 'AgreementCode', height: 'auto', width: (pageWidth * (5 / 100)), align: "left", sortable: false, hidden: true },
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: (pageWidth * (8 / 100)), sortable: false, resizable: false },
                            { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: (pageWidth * (15 / 100)), sortable: false, resizable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: (pageWidth * (7 / 100)), sortable: false, resizable: false, align: 'center' },
                            { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: (pageWidth * (6 / 100)), sortable: false, align: "right", resizable: false },
                            { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: (pageWidth * (6 / 100)), sortable: false, align: "right", resizable: false },//, hidden: isHiddenMaintenenceCost },
                            { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: (pageWidth * (8 / 100)), sortable: false, align: "left", resizable: false },
                            { name: 'Action', index: 'Action', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false, hidden:true },  
                            { name: 'BGAmount', index: 'BGAmount', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: 'right', resizable: false },
                            { name: 'BGStatus', index: 'BGStatus', width: (pageWidth * (5 / 100)), sortable: false, resize: false, align: "center", resizable: false,hidden:true },
                            { name: 'View', index: 'View', width: (pageWidth * (5 / 100)), sortable: false, formatter: FormatColView, align: "center", resizable: false ,hidden:true},
                            { name: 'Download', index: 'Download', width: (pageWidth * (5 / 100)), sortable: false, align: "center", resizable: false },
        ],
        pager: jQuery('#dvlstPagerBankGuaranteeUploads'),
        rowNum: 15,
        rowList: [15, 20, 25],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;Bank Guarantee/FDR Details",
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
            var reccount = $('#tblstBankGuaranteeUploads').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvlstPagerBankGuaranteeUploads_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2."NA"-Not Available  ]');
            }

        },
        onHeaderClick: function () {
            //$(this).closest('.ui-jqgrid').hide('blind', {}, 500);
            $('#dvBankGuaranteeUploads').hide('slow');
            //           
            $('#gview_tblstExpAgreementsBankGuarantee .ui-jqgrid-titlebar-close>span').trigger('click');
        }
    });
    $('#gview_' + $.jgrid.jqID($grid[0].id) + ' .ui-jqgrid-titlebar-close>span')
               .removeClass('ui-icon-circle-triangle-n')
               .addClass('ui-icon-closethick');


}

function FormatColView(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Bank Guarantee Details' onClick ='NOAction(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Bank Guarantee Details not available' ></span></td></tr></table></center>";
    }

}

 