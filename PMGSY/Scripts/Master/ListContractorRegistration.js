$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddLokSabhaTerm');
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $(function () {
        $("#accordionView").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    


    $("#dvSearchContractorReg").load('/Master/SearchContractorRegistration', function () {
    });

    $('#btnCreateNew').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvSearchContractorReg").is(":visible")) {
            $('#dvSearchContractorReg').hide('slow');
        }
        if (!$("#dvDetailsContractorReg").is(":visible")) {
            $("#dvDetailsContractorReg").load("/Master/AddEditMasterContractorReg/");
            $('#dvDetailsContractorReg').show('slow');
            $('#btnCreateNew').hide();
            $('#btnSearch').show();
            
        }
        $.unblockUI();
    });

    $('#btnSearch').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvDetailsContractorReg").is(":visible")) {
            $('#dvDetailsContractorReg').hide('slow');
            $('#btnSearch').hide();
            $('#btnCreateNew').show();
        }
        
        if (!$("#dvSearchContractorReg").is(":visible")) {
            $('#dvSearchContractorReg').load('/Master/SearchContractorRegistration/', function () {
                $('#tblstContractorReg').trigger('reloadGrid');
                var data = $('#tblstContractorReg').jqGrid("getGridParam", "postData");            
            
                if (!(data === undefined)) {
                    $('#State').val(data.stateCode);                   
                }
                $('#dvSearchContractorReg').show('slow');
                //Added By Abhishek kamble 20-Feb-2014

                $('#btnCreateNew').show();
                $('#btnSearch').hide();
            });
        }
        //Added By Abhishek kamble 20-Feb-2014
        if ($("#dvlstBankDetails").is(":visible"))
        {
            $("#dvlstBankDetails").hide("slow");
        }
        if ($("#accordionView").is(":visible")) {
            $("#accordionView").hide("slow");
        }
  

        jQuery("#tblstContractorReg").jqGrid('setGridState', 'visible');

        $.unblockUI();
    });

    


});
function LoadContractorRegistrationList() {
    jQuery("#tblstContractorReg").jqGrid({
        url: '/Master/GetContractorRegList',
        datatype: "json",
        mtype: "POST",
        //postData: { stateCode: ($("#MAST_DESIG_TYPE").val()) },
        colNames: ['Contractor Name', 'Company Name', 'Pan No.', 'Registration No', 'Registration Office', 'State Name', 'Valid From', 'Valid To', 'Registration Status', 'Contractor Status', 'Class Type', 'Edit', 'Delete', 'Bank Details', 'View', 'Change Fund Type'],
        colModel: [
                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_CON_REG_NO', index: 'MAST_CON_REG_NO', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_CON_PAN', index: 'MAST_CON_PAN', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'MAST_REG_OFFICE', index: 'MAST_REG_OFFICE', height: 'auto', width: 120, align: "left", sortable: true },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 120, align: "left", sortable: true },
                            { name: 'MAST_CON_VALID_FROM', index: 'MAST_CON_VALID_FROM', height: 'auto', width: 90, align: "left", sortable: true },
                            { name: 'MAST_CON_VALID_TO', index: 'MAST_CON_VALID_TO', height: 'auto', width: 90, align: "left", sortable: true },
                            { name: 'MAST_REG_STATUS', index: 'MAST_REG_STATUS', height: 'auto', width: 70, align: "left", sortable: true },
                            { name: 'MAST_CON_STATUS', index: 'MAST_CON_STATUS', height: 'auto', width: 70, align: "left", sortable: true },
                            { name: 'MASTER_CON_CLASS_TYPE', index: 'MASTER_CON_CLASS_TYPE', height: 'auto', width: 100, align: "left", sortable: true },
                            { name: 'a', width: 40, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                            { name: 'b', width: 40, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false },
                            { name: 'c', width: 40, sortable: false, resize: false, formatter: FormatColumn1, align: "center", sortable: false },
                            { name: 'd', width: 40, sortable: false, resize: false, formatter: FormatColumn2, align: "center", sortable: false },
                             // Added on 25-01-2022 by Srishti Tyagi
                            { name: 'FUND_TYPE', width: 40, sortable: false, resize: false, formatter: FormatColumnChangeFund, align: "center" }
        ],
        postData: { stateCode: $('#State option:selected').val(), status: $('#Status option:selected').val(), contractorName: $('#txtContractor').val(), conStatus: $('#ContractorStatus option:selected').val(), panNo: $("#txtPan").val(), classType: $("#ClassType option:selected").val(), regNo: $("#txtRegNo").val(), companyName: $("#txtCompanyName").val() },
        pager: jQuery('#pglstContractorReg'),
        rowNum: 15,
        sortname: 'CONTRACTOR_NAME',
        sortorder: "asc",
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Contractor Registration List",
        height: 'auto',
        shrinkToFit: false,
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
        loadComplete: function () {
            if ($("#roleCode").val() == 22 || $("#roleCode").val() == 38) {
                $('#tblstContractorReg').jqGrid('hideCol', 'a');
                $('#tblstContractorReg').jqGrid('hideCol', 'b');
            }
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
}

function FormatColumnEdit(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "-") {
        return "<center><table><tr><td style='border:none;'><span>-</span></td></tr></table></center>";
    }
    else {
        return "<span class='ui-icon ui-icon-pencil ui-align-center' title='Edit Contractor Registration Details' onClick ='editContractorReg(\"" + cellvalue.toString() + "\")'></span>";
    }
}

function FormatColumnDelete(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "-") {
        return "<center><table><tr><td style='border:none;'><span>-</span></td></tr></table></center>";
    }
    else {
        return "<span class='ui-icon ui-icon-trash ui-align-center' title='Delete Contractor Registration Details' onClick =deleteRegData(\"" + cellvalue.toString() + "\");></span>";
    }
}

function FormatColumn2(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='View Contractor Registration Details' onClick ='ViewContractorRegDetails(\"" + cellvalue.toString() + "\")'></span></center>";

}

function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-plusthick' title='Add Contractor Bank Details' onClick ='AddContractorBankDetails(\"" + cellvalue.toString() + "\")'></span></center>";

}

// Added on 25-01-2022 by Srishti Tyagi
function FormatColumnChangeFund(cellvalue, option, rowObject) {

    return "<span class='ui-icon ui-icon-pencil ui-align-center' title='Edit Fund Type' onClick ='editFundType(\"" + cellvalue.toString() + "\")'></span>";
}

function editFundType(id) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterContractorRegFundType/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if ($("#dvSearchContractorReg").is(":visible")) {
                $('#dvSearchContractorReg').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearch').show();
            $("#dvDetailsContractorReg").show();
            $("#dvDetailsContractorReg").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}
// End changes made by Srishti Tyagi

function editContractorReg(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterContractorReg/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if ($("#dvSearchContractorReg").is(":visible")) {
                $('#dvSearchContractorReg').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearch').show();
            $("#dvDetailsContractorReg").show();
            $("#dvDetailsContractorReg").html(data);
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteRegData(urlParam) {
    if (confirm("Are you sure you want to delete Contractor/Supplier registration details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/DeleteMasterContractorReg/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#tblstContractorReg").trigger('reloadGrid');
                    if ($("#dvDetailsContractorReg").is(":visible")) {
                        $('#dvDetailsContractorReg').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnCreateNew').show();
                    }

                    if (!$("#dvSearchContractorReg").is(":visible")) {
                        $('#dvSearchContractorReg').show('slow');
                        $("#tblstContractorReg").trigger('reloadGrid');
                    }
                    else {
                        $("#tblstContractorReg").trigger('reloadGrid');
                    }
                    
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }


}

function AddContractorBankDetails(id) {
    
    $('dvlstBankDetails').empty();

    $("#dvlstBankDetails").load("/Master/ListBankDetails?id=" + id, function () {
        $("#dvlstBankDetails").show('slow');

        if ($('#registrationDetails').is(":visible")) {
            $('#registrationDetails').hide();

        }
        if ($("#dvSearchContractorReg").is(":visible")) {
            $("#dvSearchContractorReg").hide('slow');
            $("#btnCreateNew").hide();
        }

        if (($("#dvDetailsContractorReg").is(":visible"))) {
            $("#dvDetailsContractorReg").hide();
        }

        $('#tblstContractorReg').jqGrid("setGridState", "hidden");


    });

}

function ViewContractorRegDetails(id)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#accordionView div").html("");
    $("#accordionView h3").html(
            "<a href='#' style= 'font-size:.9em;' >Contractor Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseContractorDetails();" /></a>'
            );

    $('#accordionView').show('fold', function () {
        blockPage();
        $("#dvlstRegDetails").load('/Master/ViewRegistrationDetails/' + id, function (data) {
            unblockPage();
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#dvlstRegDetails'));
            }
        });
        $('#dvlstRegDetails').show('slow');
        $("#dvlstRegDetails").css('height', 'auto');
        $("#dvDetailsContractorReg").hide();
        $("#dvSearchContractorReg").hide('slow');
        $("#btnCreateNew").hide('slow');

    });
    $("#tblstContractorReg").jqGrid('setGridState', 'hidden');

    $.unblockUI();
    

    //$.ajax({
    //    url: "/Master/ViewRegistrationDetails/" + id,
    //    type: "GET",
    //    async: false,
    //    dataType: "html",
    //    catche: false,
    //    contentType: "application/json; charset=utf-8",
    //    success: function (data) {

    //        if ($("#dvSearchContractorReg").is(":visible")) {
    //            $('#dvSearchContractorReg').hide('slow');
    //        }
    //        $('#btnCreateNew').hide();
    //        $('#btnSearch').show();
    //        $("#dvlstRegDetails").show();
    //        $("#dvlstRegDetails").html(data);
    //        $.unblockUI();
    //    },
    //    error: function (xht, ajaxOptions, throwError) {
    //        alert(xht.responseText);
    //        $.unblockUI();
    //    }
    //});
}

function CloseContractorDetails()
{

    $("#accordionView").hide('slow');
    $('#tblstContractorReg').jqGrid("setGridState", "visible");

    $("#btnCreateNew").show();
    $("#btnSearch").hide();
    $("#dvSearchContractorReg").show("slow");
}
