$(document).ready(function () {
    
    $.ajax({
        url: "/Master/SearchNodalOfficer/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchNodalOfficer").html(data);
         },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });

   
   
    //function for loading Add form.
    $('#btnCreateNew').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvSearchNodalOfficer").is(":visible")) {
             $('#dvSearchNodalOfficer').hide('slow');
         }
        if (!$("#dvDetailsNodalOfficer").is(":visible")) {
           $("#dvDetailsNodalOfficer").load("/Master/AddNodalOfficer/");
           $('#dvDetailsNodalOfficer').show('slow');
           $('#btnCreateNew').hide();
           $('#btnSearchView').show();

        }
        $.unblockUI();
    });

    //function for loading search form.  
    $('#btnSearchView').click(function (e) {
       $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvDetailsNodalOfficer").is(":visible")) {
            $('#dvDetailsNodalOfficer').hide('slow');
            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
        }
        if (!$("#dvSearchNodalOfficer").is(":visible")) {
            $('#dvSearchNodalOfficer').load('/Master/SearchNodalOfficer/', function () {
                $('#tblList').trigger('reloadGrid');
                var data = $('#tblList').jqGrid("getGridParam", "postData");
                //alert(data.officeCode);
                if (!(data === undefined)) {
                    if (data.StateCode != undefined) {
                        $('#ddlSearchState').val(data.StateCode);
                        //FillInCascadeDropdown({ userType: $("#ddlSearchState").find(":selected").val() },
                        //                      "#ddlSearchOffice", "/Master/PopulateAdminNd_ByStateCode?stateCode=" + $('#ddlSearchState option:selected').val());

                    }
                   
                    if (data.designationCode != undefined) {
                        $('#ddlSearchDesignation').val(data.designationCode);
                    }
                    if (data.NoTypeCode != undefined) {
                        $('#ddlSearchNOType').val(data.NoTypeCode)
                    }
                    if (data.ModuleType != undefined) {
                        $('#ddlModuleType').val(data.ModuleType)
                    }
                    setTimeout(function () {
                        //alert(data.officeCode);
                        $('#ddlSearchState').val(data.StateCode);
                        // $('#ddlSearchOffice').val(data.officeCode);                       
                        $('#ddlSearchDesignation').val(data.designationCode);
                        $('#ddlSearchNOType').val(data.NoTypeCode);
                        $('#ddlModuleType').val(data.ModuleType);


                    }, 2500);
              }
                $('#dvSearchNodalOfficer').show('slow');
               
            });            
          
        }
       
        $.unblockUI();
    });

});

//function for edit functionality.
function editNodalOfficer(urlParam) {
   
    $.ajax({
        url: "/Master/EditNodalOfficer/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchNodalOfficer").is(":visible")) {
                $('#dvSearchNodalOfficer').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $("#dvDetailsNodalOfficer").show();
            $("#dvDetailsNodalOfficer").html(data);
            $("#trStatus").show("slow");
            

        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
        }

    });

}

//Function for delete functionality.
function deleteNodalOfficer(urlParam) {
    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete Nodal Officer details?")) {
        $.ajax({

            url: "/Master/DeleteNodalOfficer/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //if ($("#dvSearchNodalOfficer").is(":visible")) {
                    //    $('#btnSearch').trigger('click');
                    //}
                    //else {
                    //    $("#dvDetailsNodalOfficer").load("/Master/AddNodalOfficer/");
                    //    $('#tblList').trigger('reloadGrid');
                    //}

                    if ($("#dvDetailsNodalOfficer").is(":visible")) {
                        $('#dvDetailsNodalOfficer').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }
                    if (!$("#dvSearchNodalOfficer").is(":visible")) {
                        $("#dvSearchNodalOfficer").show();
                        $('#tblList').trigger('reloadGrid');
                    }
                    else {
                        $('#tblList').trigger('reloadGrid');
                    }
                }
                else {
                    alert(data.message);
                }
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }

        });
    }
    else {
        return false;
    }
}
function viewNodalOfficer(urlParam) {

    $.ajax({
        url: "/Master/ViewNodalOfficer/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchNodalOfficer").is(":visible")) {
                $('#dvSearchNodalOfficer').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $("#dvDetailsNodalOfficer").show();
            $("#dvDetailsNodalOfficer").html(data);
            $("#trStatus").show("slow");


        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
        }

    });

}
function LoadGrid() {
    //$('#tblList').jqGrid('GridUnload');
    $('#tblList').jqGrid({
        url: '/Master/GetNodalOfficerDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Name', 'Designation', 'Profile Type', 'Office', 'State Name', 'District Name', 'Start Date', 'End Date', 'Active Status', 'Bank Details', 'Action', 'View'],
        colModel: [
                      { name: 'Name', index: 'Name', height: 'auto', width: 120, align: "left", sortable: true },
                      { name: 'Designation', index: 'Designation', height: 'auto', width: 100, align: "left", sortable: true },
                      { name: 'NoType', index: 'NoType', height: 'auto', width: 120, align: "left", sortable: true, hidden: false },
                      { name: 'NoName', index: 'NoName', height: 'auto', width: 120, align: "left", sortable: true },
                      { name: 'StateName', index: 'StateName', height: 'auto', width: 90, align: "left", sortable: true },
                      { name: 'District', index: 'District', height: 'auto', width: 80, align: "left", sortable: true },
                      { name: 'StartDate', index: 'StartDate', height: 'auto', width:60, align: "left", sortable: true },
                      { name: 'EndDate', index: 'EndDate', height: 'auto', width: 60, align: "left", sortable: true },
                      { name: 'Active', index: 'Active', height: 'auto', width: 40, align: "left", sortable: true },
                      { name: 'BankDetails', width: 40, sortable: false, resize: false, formatter: FormatColumn1, align: "center", sortable: false },
                      { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                      { name: 'View', width: 40, sortable: false, resize: false, formatter: FormatColumnView, align: "center", sortable: false }
        ],
        postData: { StateCode: $('#ddlSearchState option:selected').val(), officeCode: $('#ddlSearchOffice option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), designationCode: $('#ddlSearchDesignation option:selected').val(), NoTypeCode: $('#ddlSearchNOType option:selected').val(), ModuleType: $('#ddlModuleType option:selected').val(), Active: $('#ddlSearchActive option:selected').val() },
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Name',
        sortorder: "asc",
        caption: "Nodal Officer List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: true,
    });

}


function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Nodal Officer Details' onClick ='editNodalOfficer(\"" + cellvalue.toString() + "\")'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Nodal Officer Details' onClick =deleteNodalOfficer(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";

}
function FormatColumnView(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-zoomin' title='View Nodal Officer Details' onClick ='viewNodalOfficer(\"" + cellvalue.toString() + "\")'></span></td></tr></table></center>";

}

function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-plusthick' title='Add Contractor Bank Details' onClick ='AddContractorBankDetailsNO(\"" + cellvalue.toString() + "\")'></span></center>";

}

function AddContractorBankDetailsNO(id) {
    
    $('dvlstBankDetailsNO').empty();

    $("#dvlstBankDetailsNO").load("/Master/ListBankDetailsNO?id=" + id, function () {
        $("#dvlstBankDetailsNO").show('slow');

        if ($('#dvSearchNodalOfficer').is(":visible")) {
            $('#dvSearchNodalOfficer').hide();

        }
        if ($("#dvList").is(":visible")) {
            //$("#dvList").hide('slow');
            $("#btnCreateNew").hide();
        }

        if (($("#dvDetailsNodalOfficer").is(":visible"))) {
            $("#dvDetailsNodalOfficer").hide();
        }

        $('#tblList').jqGrid("setGridState", "hidden");


    });

}