$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#ddlFinancialYears').val('0');
    LoadAgreementDetails();
    
    


    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');

    $("#dvIncompleteReason").dialog({
        autoOpen: false,
        height: '160',
        width: '370',
        modal: true,
        title: 'Incomplete Reason.'
    });



    $("#dvViewAgreementMaster").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Agreement Details'
    });


    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchAgreementDetails").is(":visible")) {
            $('#dvSearchAgreementDetails').hide('slow');
        }

        if (!$("#dvAgreementDetails").is(":visible")) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            if (Mast_Con_Sup_Flag == "S") {
                $('#dvAgreementDetails').load('/Agreement/AddAgreementWithoutRoad/', function () {
                    $('#dvAgreementDetails').show('slow');
                    $.unblockUI();
                });
            }
            else if (Mast_Con_Sup_Flag == "D") {
                $('#dvAgreementDetails').load('/Agreement/AddAgreementWithoutRoad_DPR/', function () {
                    $('#dvAgreementDetails').show('slow');
                    $.unblockUI();
                });
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }


    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvAgreementDetails").is(":visible")) {
            $('#dvAgreementDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchAgreementDetails").is(":visible")) {

        
            $('#dvSearchAgreementDetails').show('slow');

            var data = $('#tbAgreementList').jqGrid("getGridParam", "postData");

            if (!(data === undefined)) {
                $('#ddlFinancialYears').val(data.agreementYear);
                $('#status').val(data.status);
            }

        }

        $.unblockUI();



    });


});


function SearchDetails() {

    $('#tbAgreementList').setGridParam({
        url: '/Agreement/GetAgreementMasterDetailsList_WithoutRoad',
        datatype: 'json'
    });

    $('#tbAgreementList').jqGrid("setGridParam", { "postData": { AgreementType: $('#EncryptedAgreementType').val(), agreementYear: $('#ddlFinancialYears option:selected').val(), status: $('#ddlStatus option:selected').val() } });
    $('#tbAgreementList').trigger("reloadGrid", [{ page: 1 }]);

}

function LoadAgreementDetails() {


    var gridCaption = "";
    var columnName = "";

    if (Mast_Con_Sup_Flag == "S") {

        gridCaption = 'Supplier Agreement List';
        columnName = 'Supplier';
        $('#aHeading').text('Add Agreement Details for Supplier');
    }
    else {
        gridCaption = 'DPR Agreement List';
        columnName = 'Contractor';

        $('#aHeading').text('Add Agreement Details for DPR');

    }

    jQuery("#tbAgreementList").jqGrid({
        url: '/Agreement/GetAgreementMasterDetailsList_WithoutRoad',
        datatype: "local",//"json",
        mtype: "POST",
        postData: { AgreementType: $('#EncryptedAgreementType').val() },
        colNames: ['Agreement Number', columnName+ ' Name', 'Agreement Type', 'Agreement Date', 'Agreement Amount', 'Maintenance Amount', 'Agreement Status','Change Status To Complete','Change Status To Incomplete', 'Finalize','DeFinalize','View', 'Edit', 'Delete'],
        colModel: [                                            
                           { name: 'AgreementNumber', index: 'AgreementNumber', width: 180, sortable: true },
                           { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 170, sortable: true, },
                           { name: 'AgreementType', index: 'AgreementType', width: 120, sortable: true, align: "left" },
                           { name: 'AgreementDate', index: 'AgreementDate', width: 120, sortable: true },
                           { name: 'AgreementAmount', index: 'AgreementAmount', height: 'auto', width: 150, sortable: false, align: "right" },
                           { name: 'MaintenanceAmount', index: 'MaintenanceAmount', height: 'auto', width: 110, sortable: false, align: "right", hidden:true },
                           { name: 'AgreementStatus', index: 'AgreementStatus', height: 'auto', width: 150, sortable: false, align: "left" },
                           { name: 'Change Status To Complete', index: 'Change Status To Complete', width: 100, sortable: false, formatter: FormatColumnChangeStatusToComplete, align: "center", hidden: true },
                           { name: 'Change Status', index: 'Change Status', width: 100, sortable: false, formatter: FormatColumnChangeStatus, align: "center", hidden: true },
                           { name: 'Finalize', index: 'Finalize', width: 50, sortable: false, resize: false, align: "center" }, /* formatter: FormatColumnFinalize,*/
                           { name: 'DeFinalize', index: 'Finalize', width: 50, sortable: false, resize: false, align: "center" },
                            { name: 'View', index: 'View', width: 50, sortable: false, formatter: FormatColumnView, align: "center", resizable: false },
                          { name: 'Edit', index: 'Edit', width: 50, sortable: false, formatter: FormatColumnEdit, align: "center" },
                            { name: 'Delete', index: 'Edit', width: 50, sortable: false, align: "center", formatter: FormatColumnDelete }
                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#dvAgreementListPager'),
        rowNum: 10,
        rowList: [5, 10,20,30],
        viewrecords: true,
        recordtext: '{2} records found',
        //caption: "Agreement Details List",
        caption:gridCaption,
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        sortname: 'ContractorName,AgreementNumber',
        sortorder: "asc",
        hidegrid: false,

        loadComplete: function () {

          
            var reccount = $('#tbAgreementList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvAgreementListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs. 2.All Lengths are in Kms.  ]');
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
       

    }); //end of grid
}

function FormatColumnEdit(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Agreement Details' onClick ='EditAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnView(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Agreement Details' onClick ='ViewAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Agreement Details' onClick ='DeleteAgreementMasterDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


function EditAgreementMasterDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/EditAgreementMasterDetails_WithoutRoad/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {


            if ($("#dvSearchAgreementDetails").is(":visible")) {
                $('#dvSearchAgreementDetails').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();


            $("#dvAgreementDetails").html(data);
            $("#dvAgreementDetails").show();

            $("#TEND_AGREEMENT_NUMBER").focus();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}


function ViewAgreementMasterDetails(urlparameter) {

    var EncryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Agreement/ViewAgreementMasterDetails_WithoutRoad/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvViewAgreementMaster").html(data);

            $("#dvViewAgreementMaster").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteAgreementMasterDetails(urlparameter) {
    if (confirm("Are you sure you want to delete agreement details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Agreement/DeleteAgreementMasterDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tbAgreementList').trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {
                        $("#btnCancelAgreementDetails").trigger('click');
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

function FinalizeAgreement(urlparameter) {

    if (confirm("Are you sure you want to 'Finalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/FinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
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
function FormatColumnChangeStatus(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><a href='#' title='Incomplete Agreement' onClick ='ChangeAgreementStatusToInComplete(\"" + cellvalue.toString() + "\");'>Incomplete</a></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnChangeStatusToComplete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><a href='#' title='Complete Agreement' onClick ='ChangeAgreementStatusToComplete(\"" + cellvalue.toString() + "\");'>Complete</a></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function ChangeAgreementStatusToInComplete(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvIncompleteReason').empty();
    $("#dvIncompleteReason").load("/Agreement/IncompleteReason/" + urlparameter, function () {

        $("#dvIncompleteReason").dialog('open');
        $.unblockUI();
    })



}

function DeFinalizeAgreement(urlparameter)
{
    if (confirm("Are you sure you want to 'DeFinalize' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/DeFinalizeAgreement/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
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

function ChangeAgreementStatusToComplete(urlparameter) {

    if (confirm("Are you sure you want to 'Complete' agreement ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Agreement/ChangeAgreementStatusToComplete/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tbAgreementList").trigger('reloadGrid');

                    if ($('#btnUpdateAgreementDetails').is(':visible')) {

                        $("#btnCancelAgreementDetails").trigger('click');
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

