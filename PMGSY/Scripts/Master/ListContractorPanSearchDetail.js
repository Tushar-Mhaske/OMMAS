$(document).ready(function () {
    //Accordion intialised
    $(function () {
        $("#accordionContractor").accordion({
            collapsible: false,
        });
    });

    //Tab Initialised
    $("#tabMain").tabs();

    $('#btnloadGrid').click(function () {
        var pan = $('#txtPAN').val();
        if ($('#txtPAN').val() != "") {
            LoadContractorDetailGrid(pan);
            $("#tabMain").hide();
        }
        else {
            alert("Please Enter PAN / TAN");
            $("#tbContractorDetailReport").jqGrid('GridUnload');
            $("#tabMain").hide();
        }
    });
});

function LoadContractorDetailGrid(pan) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbContractorDetailReport").jqGrid('GridUnload');
    jQuery("#tbContractorDetailReport").jqGrid({
        url: '/Master/GetContractorByPanSearchList/',
        datatype: "json",
        mtype: "POST",
        colNames: ["Contractor/Supplier Name", "Contractor/Supplier Status", 'PAN / TAN', 'Company Name', 'District', 'State', 'Mobile','Email','Registration','Bank Detail','Edit','Delete','View'],
        colModel: [
            { name: "CONTRACTOR_NAME", width: 150, align: 'left', height: 'auto', frozen: false, sortable: true },
            { name: "CONTRACTOR_Status", width: 100, align: 'center', height: 'auto', frozen: false, sortable: true },
            { name: "MAST_CON_PAN", width: 120, align: 'center', height: 'auto', frozen: false, sortable: true },
            { name: "MAST_CON_COMPANY_NAME", width: 150, align: 'left', height: 'auto', sortable: true },
            { name: "MAST_DISTRICT_CODE", width: 80, align: 'center', height: 'auto', sortable: true },
            { name: "MAST_STATE_CODE", width: 120, align: 'center', height: 'auto', sortable: true },
            { name: "MAST_CON_MOBILE", width: 80, align: 'center', height: 'auto', sortable: true },
            { name: "MAST_CON_EMAIL", width: 80, align: 'center', height: 'auto', sortable: true },
            { name: "Registration", width: 80, align: 'left', height: 'auto', sortable: true, hidden: true, formatter: FormatColumn2 },
            { name: "BankDetails", width: 80, align: 'left', height: 'auto', sortable: true, hidden: true, formatter: FormatColumn_BankDetails, },
            { name: "Edit", width: 80, align: 'left', height: 'auto', sortable: true, hidden: true, formatter: FormatColumn },
            { name: "Delete", width: 80, align: 'left', height: 'auto', sortable: true, hidden: true, formatter: FormatColumn3 },
            { name: "View", width: 80, align: 'left', height: 'auto', sortable: true, hidden: false, formatter: FormatColumn4 },

        ],
        postData: { "PAN": pan },
        pager: jQuery('#dvContractDetailReportPager'),
        rowList: [5, 10, 20, 30],
        rowNum: 5,
        rownumbers: true,
        viewrecords: true,
        caption: "&nbsp;&nbsp;Contractor/Supplier Details List",
        height: 170,
        autowidth: true,
        // width: '100%',
        sortname: 'CONTRACTOR_NAME',
        footerrow: false,
        loadComplete: function (data) {
            if (data.records < 6) {
                $("#tbContractorDetailReport").jqGrid('setGridHeight', 'auto');
            }
           
            $('#tbContractorDetailReport_rn').html('Sr.<br/>No.');
            $("#tbContractorDetailReport").jqGrid('setGridWidth', $('#gbox_tbContractorDetailReport').width(), true);

            $.unblockUI();
        },      
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }


    }); //end of grid
    jQuery("#tbContractorDetailReport").jqGrid('setFrozenColumns');
}

//Start Tab 1 Contractor Information
function ViewContractorInformation(urlparameter) {
    $("#divViewContractorDetail").html('');
    $.ajax({
        type: 'GET',
        url: '/Master/ViewContractorByPanSearch/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $("#divViewContractorDetail").html(data);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}
//End Tab 1  Contractor Information

//Start Tab 2 Contractor Bank Information
function LoadContractorBankDetail(urlparameter)
{
    $("#tbContractorBankDetailDetailReport").jqGrid('GridUnload');
    $('#tbContractorBankDetailDetailReport').jqGrid({
        url: '/Master/GetContractorByIdPanSearchBankDetails/',
            datatype: 'json',
            mtype: "POST",
            postData: { ContractorCode: urlparameter },
            colNames: ['Contractor Name', 'District Name', 'State Name', 'Account Number', 'Bank Name', 'IFSC Code', 'Status', 'Action'],
            colModel: [
            { name: 'ContName', index: 'ContName', height: 'auto', width: 140, align: "left", sortable: true,hidden:true },
            { name: 'District', index: 'District', height: 'auto', width: 100, align: "center", sortable: true },
            { name: 'State', index: 'State', height: 'auto', width: 100, align: "center", sortable: true },
            { name: 'AccNumber', index: 'AccNumber', height: 'auto', width: 120, align: "center", sortable: true },
            { name: 'BankName', index: 'BankName', height: 'auto', width: 160, align: "left", sortable: true },
            { name: 'IfscCode', index: 'IfscCode', height: 'auto', width: 120, align: "center", sortable: true },
            { name: 'AccStatus', index: 'AccStatus', height: 'auto', width: 80, align: "center", sortable: true },
            { name: 'a', width: 60, sortable: false, resize: false, align: "center", sortable: false,hidden:true }
            ],
            pager: jQuery('#divContractorBankDetailReportPager'),
            rowNum: 5,
            rowList: [5,10, 15, 20],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'BankName',
            sortorder: "asc",
            caption: 'Bank Details List',
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            hidegrid: false,
            loadComplete: function () {
                $("#tbContractorBankDetailDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 20, true);
                $('#tbContractorBankDetailDetailReport_rn').html('Sr.<br/>No.');

            },
            loadError: function (xhr, status, error) {

                if (xhr.responseText == "session expired") {

                    alert(xht.responseText);
                    window.location.href = "Login/login";
                }
                else {
                    alert("Invalid Data. Please Check and Try Again.");
                }
            }

        });

}
//End Tab 2  Contractor Bank Information

//Start Tab 3 Contractor Registration Information
function LodaContractorRegistrationDetailGrid(urlparameter) {
    $("#tbContractorRegistationDetailReport").jqGrid('GridUnload');
    jQuery("#tbContractorRegistationDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchRegList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": urlparameter },
        colNames: ['Contractor Name', 'Company Name', 'Pan No.', 'Registration No', 'Registration Office', 'State Name', 'Valid From', 'Valid To', 'Registration Status', 'Contractor Status', 'Class Type', 'Edit', 'Delete', 'Bank Details', 'View'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false,hidden:true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false,hidden:true },
                            { name: 'MAST_CON_PAN', index: 'MAST_CON_PAN', height: 'auto', width: 120, align: "center", sortable: true, frozen: false,hidden:true },
                            { name: 'MAST_CON_REG_NO', index: 'MAST_CON_REG_NO', height: 'auto', width: 120, align: "center", sortable: true, frozen: false },
                            { name: 'MAST_REG_OFFICE', index: 'MAST_REG_OFFICE', height: 'auto', width: 120, align: "left", sortable: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'MAST_CON_VALID_FROM', index: 'MAST_CON_VALID_FROM', height: 'auto', width: 80, align: "center", sortable: true },
                            { name: 'MAST_CON_VALID_TO', index: 'MAST_CON_VALID_TO', height: 'auto', width: 80, align: "center", sortable: true },
                            { name: 'MAST_REG_STATUS', index: 'MAST_REG_STATUS', height: 'auto', width: 70, align: "center", sortable: true },
                            { name: 'MAST_CON_STATUS', index: 'MAST_CON_STATUS', height: 'auto', width: 70, align: "center", sortable: true },
                            { name: 'MASTER_CON_CLASS_TYPE', index: 'MASTER_CON_CLASS_TYPE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'a', width: 40, sortable: false, resize: false, align: "center", sortable: false, hidden: true },
                            { name: 'b', width: 40, sortable: false, resize: false,align: "center", sortable: false, hidden: true },
                            { name: 'c', width: 40, sortable: false, resize: false,align: "center", sortable: false, hidden: true },
                            { name: 'd', width: 40, sortable: false, resize: false,align: "center", sortable: false, hidden: true }
        ],
        pager: jQuery('#dvContractorRegistationDetailReportPager'),       
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Registration Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#tbContractorRegistationDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 25, true);
            $('#tbContractorRegistationDetailReport_rn').html('Sr.<br/>No.');

        },
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
    jQuery("#tbContractorRegistationDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 3 Contractor Registration Information

//Start Tab 4 Contractor Agreement Information
function LodaContractorAgreementDetailGrid(urlparameter) {
    $("#tbContractorAgreementDetailReport").jqGrid('GridUnload');
    jQuery("#tbContractorAgreementDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchAgreementList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": urlparameter },
        colNames: ['Contractor Name', 'Company Name', 'State', 'District', 'Agreement Number', 'Start Date', 'End Date', 'Amount', 'Maintenance Amount', 'Agreement Status', 'Is Finalize'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false,hidden:true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'TEND_AGREEMENT_NUMBER', index: 'TEND_AGREEMENT_NUMBER', height: 'auto', width: 120, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_START_DATE', index: 'TEND_AGREEMENT_START_DATE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_END_DATE', index: 'TEND_AGREEMENT_END_DATE', height: 'auto', width: 80, align: "center", sortable: true },
                            { name: 'TEND_AGREEMENT_AMOUNT', index: 'TEND_AGREEMENT_AMOUNT', height: 'auto', width: 80, align: "right", sortable: false },
                            { name: 'MAINT_Amount', index: 'MAINT_Amount', height: 'auto', width: 70, align: "right", sortable: false },
                            { name: 'TEND_AGREEMENT_STATUS', index: 'TEND_AGREEMENT_STATUS', height: 'auto', width: 100, align: "center", sortable: false },
                            { name: 'TEND_IS_AGREEMENT_FINALIZED', index: 'TEND_IS_AGREEMENT_FINALIZED', height: 'auto', width: 70, align: "center", sortable: true },

        ],
        pager: jQuery('#dvContractorAgreementDetailReportPager'),        
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Agreement Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#tbContractorAgreementDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 40, true);
            $('#tbContractorAgreementDetailReport_rn').html('Sr.<br/>No.');

        },
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
    jQuery("#tbContractorAgreementDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 4 Contractor Agreement Information

//Start Tab 5 Contractor Maintenance Information
function LodaContractorMaintenanceDetailGrid(urlparameter) {
    $("#tbContractorMaintenanceDetailReport").jqGrid('GridUnload');
    jQuery("#tbContractorMaintenanceDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchIMSMaintenanceList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": urlparameter },
        colNames: ['Contractor Name', 'Company Name', 'State', 'Agreement Number', 'Contractor Number', 'Maintenance Agreement Date', 'Maintenance Amount', 'Agreement Status', 'Is Finalize'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MANE_AGREEMENT_NUMBER', index: 'MANE_AGREEMENT_NUMBER', height: 'auto', width: 100, align: "center", sortable: true, frozen: false },
                            { name: 'MANE_CONTRACT_NUMBER', index: 'MANE_CONTRACT_NUMBER', height: 'auto', width: 120, align: "center", sortable: true },
                            { name: 'MANE_AGREEMENT_DATE', index: 'MANE_AGREEMENT_DATE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'MAINT_Amount', index: 'MAINT_Amount', height: 'auto', width: 70, align: "right", sortable: false },
                            { name: 'MANE_CONTRACT_STATUS', index: 'MANE_CONTRACT_STATUS', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'MANE_CONTRACT_FINALIZED', index: 'MANE_CONTRACT_FINALIZED', height: 'auto', width: 80, align: "center", sortable: true },

        ],
        pager: jQuery('#dvContractorMaintenanceDetailReportPager'),       
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Maintenance Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#tbContractorMaintenanceDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 20, true);
            $('#tbContractorMaintenanceDetailReport_rn').html('Sr.<br/>No.');

        },
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
    jQuery("#tbContractorMaintenanceDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 5 Contractor Maintenance Information

//Start Tab 6 Contractor Payment Information
function LodaContractorPaymentDetailGrid(urlparameter) {
    $("#tbContractorPaymentDetailReport").jqGrid('GridUnload');
    jQuery("#tbContractorPaymentDetailReport").jqGrid({
        url: '/Master/GetContractorByIdPanSearchPaymentList',
        datatype: "json",
        mtype: "POST",
        postData: { "ContractorCode": urlparameter },
        colNames: ['Contractor Name', 'Company Name', 'Bill Month', 'Bill Year', 'Bill Number', 'Bill Date', 'Gross Amount', 'Transaction Description', 'Agreement Status', 'Is Finalize'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true, frozen: false, hidden: true },
                            { name: 'BILL_MONTH', index: 'BILL_MONTH', height: 'auto', width: 70, align: "center", sortable: true, frozen: false },
                            { name: 'BILL_YEAR', index: 'BILL_YEAR', height: 'auto', width: 50, align: "center", sortable: true, frozen: false },
                            { name: 'BILL_NO', index: 'BILL_NO', height: 'auto', width: 50, align: "center", sortable: true },
                            { name: 'BILL_DATE', index: 'BILL_DATE', height: 'auto', width: 70, align: "center", sortable: true },
                            { name: 'GROSS_AMOUNT', index: 'GROSS_AMOUNT', height: 'auto', width: 80, align: "right", sortable: false },
                            { name: 'TXN_DESC', index: 'TXN_DESC', height: 'auto', width: 100, align: "center", sortable: false },
                            { name: 'BILL_TYPE', index: 'BILL_TYPE', height: 'auto', width: 100, align: "center", sortable: true },
                            { name: 'BILL_FINALIZED', index: 'BILL_FINALIZED', height: 'auto', width: 50, align: "center", sortable: true },

        ],
        pager: jQuery('#dvContractorPaymentDetailReportPager'),       
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Payment Details List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
            $("#tbContractorPaymentDetailReport").jqGrid('setGridWidth', $('#tabMain').width() - 20, true);
            $('#tbContractorPaymentDetailReport_rn').html('Sr.<br/>No.');

        },
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
    jQuery("#tbContractorPaymentDetailReport").jqGrid('setFrozenColumns');
}
//End Tab 6 Contractor Payment Information

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><span class='ui-icon ui-icon-pencil' title='Edit Contractor/Supplier Details' onClick='editData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-locked' title='Locked' ></span></center>";
    }
}
function FormatColumn1(cellvalue, options, rowObject) {
    if ((cellvalue.toString() != "")) {
        return "<center><span title='Registration Details' onClick='detailsData(\"" + cellvalue.toString() + "\");'>View Details</span></center>";

    }
    else {
        return "<center>Not Exist</center>";
    }
}
function FormatColumn2(cellvalue, options, rowObject) {
    return "<center><a href='#' title='Registration Details' onClick='registerContractor(\"" + cellvalue.toString() + "\");'>Register</a></center>";
}
function FormatColumn3(cellvalue, options, rowObject) {
    if ((cellvalue.toString() == "")) {
        return "<center><span>-</span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-trash' title='Delete Contractor/Supplier Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
function FormatColumn4(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-zoomin' title='View Contractor/Supplier Details' onClick ='viewData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumn_BankDetails(cellvalue, options, rowObject) {
    return "<center><a href='#' title='Bank Details' onClick='BankDetails(\"" + cellvalue.toString() + "\");'>Bank Details</a></center>";
}

function viewData(urlparameter) {
    $("#tabMain").show();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    ViewContractorInformation(urlparameter);
    LoadContractorBankDetail(urlparameter);
    LodaContractorRegistrationDetailGrid(urlparameter);
    LodaContractorAgreementDetailGrid(urlparameter);
    LodaContractorMaintenanceDetailGrid(urlparameter);
    LodaContractorPaymentDetailGrid(urlparameter);
}


