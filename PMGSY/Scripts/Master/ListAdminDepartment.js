$(document).ready(function () {
if (!$("#adminSearchDetails").is(":visible")) {

        $('#adminSearchDetails').load('/Master/SearchAdminDepartment');
        $('#adminSearchDetails').show('slow');
       
        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddAdmin');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#adminSearchDetails").is(":visible")) {
            $('#adminSearchDetails').hide('slow');
        }

            $('#adminAddDetails').load("/Master/AddEditAdminDepartment");
            $('#adminAddDetails').show('slow');

            $('#btnAdd').hide();
            $('#btnSearch').show();
      
    });

    $('#btnSearch').click(function (e) {

        if ($("#adminAddDetails").is(":visible")) {
            $('#adminAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#adminSearchDetails").is(":visible")) {

            $('#adminSearchDetails').load('/Master/SearchAdminDepartment', function () {
                    var data = $('#adminCategory').jqGrid("getGridParam", "postData");
                   
                if (!(data === undefined)) {

                    $('#State').val(data.stateCode);
                    $('#Agency').val(data.agency);

               
                }
                $('#adminSearchDetails').show('slow');
            });
        }
        $.unblockUI();
    });



    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });

    //loadGrid();
 

});


function FormatColumn(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit SRRDA Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete SRRDA Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumnView(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-zoomin' title='View SRRDA Details' onClick ='viewData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}
function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-circle-plus' title='Add DPIU Details' onClick ='addDPIUData(\"" + cellvalue.toString() + "\");'>Add DPIU</span></td></tr></table>"
}

function FormatColumn_DPIU(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit DPIU Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete DPIU Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
        $('#dvMappedAgencyDistrictsDetails').hide('slow');
    }
    if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
        $('#dvMapAgencyDistrictsDetails').hide('slow');
    }
    $.ajax({
        type: 'GET',
        url: '/Master/EditAdminDepartment/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#adminSearchDetails").is(":visible")) {
                $('#adminSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();
 
            $("#adminAddDetails").html(data);
            $("#adminAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();
 
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete SRRDA/DPIU details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
            $('#dvMappedAgencyDistrictsDetails').hide('slow');
        }
        if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
            $('#dvMapAgencyDistrictsDetails').hide('slow');
        }

        $.ajax({
            type: 'POST',
            url: '/Master/DeleteAdminDepartment/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                  
                    alert(data.message);
                    //if ($("#adminSearchDetails").is(":visible")) {

                    //    $('#btnAdminSearch').trigger('click');

                    //}
                    //else {
                    //    $('#adminCategory').trigger('reloadGrid');
                    //}
                    if ($("#adminAddDetails").is(":visible")) {
                        $('#adminAddDetails').hide('slow');

                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#adminSearchDetails").is(":visible")) {
                        $("#adminSearchDetails").show('slow');
                        $('#adminCategory').trigger('reloadGrid');
                    }
                    else {
                        $('#adminCategory').trigger('reloadGrid');
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

        if (!$("#adminAddDetails").is(':visible')) {
            $('#btnAdminSearch').trigger('click');
            $('#adminSearchDetails').show();
            $('#trAddNewSearch').show();
        }

    }
    else {
        return false;
    }

}
//View Data of SRRDA
function viewData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
        $('#dvMappedAgencyDistrictsDetails').hide('slow');
    }
    if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
        $('#dvMapAgencyDistrictsDetails').hide('slow');
    }
    $.ajax({
        type: 'GET',
        url: '/Master/ViewAdminDepartment/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#adminSearchDetails").is(":visible")) {
                $('#adminSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#adminAddDetails").html(data);
            $("#adminAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}
function loadGrid() {

    jQuery("#adminCategory").jqGrid({
        url: '/Master/GetDepartmentList',
        datatype: "json",
        mtype: "POST",
      
        colNames: ['SRRDA Name', 'Agency Name',  'State Name',  'SBD Date',  'PDF Key', 'Map Districts','Mapped Districts','Add DPIU','Action','View'],
        colModel: [
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: 300, align: "left", resizable: false },
                            { name: 'MAST_AGENCY_NAME', index: 'MAST_AGENCY_NAME', height: 'auto', width: 150, align: "left", resizable: false },

                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 240, align: "left", resizable: false },

                            { name: 'ADMIN_SBD_DATE', index: 'ADMIN_SBD_DATE', height: 'auto', width: 140, align: "center", resizable: false },

                            { name: 'ADMIN_PDF_KEY', index: 'ADMIN_PDF_KEY', height: 'auto', width: 100, align: "center", hidden: true, resizable: false },
                           // { name: 'Map', width: 140, sortable: false, resize: false, formatter: FormatColumnMap, align: "center", resizable: false },
                            //{ name: 'Mapped', width: 160, sortable: false, resize: false, formatter: FormatColumnMapped, align: "center", resizable: false },
                            { name: 'Map', width: 140, sortable: false, resize: false,align: "center", resizable: false },
                            { name: 'Mapped', width: 160, sortable: false, resize: false, align: "center", resizable: false },

                            { name: 'add', width: 80, resize: false, formatter: FormatColumn1, align: "center", sortable: false, resizable: false,hidden:true },
                            { name: 'a', width: 80, resize: false, formatter: FormatColumn, align: "center", sortable: false, resizable: false },
                            { name: 'View', width: 50, resize: false, formatter: FormatColumnView, align: "center", sortable: false, resizable: false }
                            
        ],     
        postData: { stateCode: $('#State option:selected').val(), agency: $('#Agency option:selected').val() },
        pager: jQuery('#pager'),
        rowNum: 15,      
        rowList: [10, 15, 20,30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME,ADMIN_ND_NAME',
        sortorder: "asc",
        caption: "SRRDA List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        cmTemplate:{title:false},
        shrinktofit: false,
      
        loadComplete: function () {

            //Commented By Abhishek kamble 20-Feb-2014
            //var SRDARowID = $('#SRDARowID').val();
            //if (SRDARowID != '') {
            //    $("#adminCategory").expandSubGridRow(SRDARowID);
            //}

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

        subGrid: false,

        
        subGridRowExpanded: function (subgrid_id, row_id) {
  

            CollapseAllOtherRowsSubGrid(row_id);

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;
         


            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table><div id='" + pager_id + "' ></div>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: '/Master/GetDPIUList_BySRDACode',
                postData: {AdminNDCode: row_id },
                datatype: "json",
                mtype: "POST",
                colNames: ['DPIU Name', 'TAN No.', 'BA Enabled', 'BA Enabled Date', 'E-Pay Enabled', 'E-Pay Enabled Date', 'E-Remittance Enabled', 'E-Remittance Enabled Date', 'Action'],
                colModel: [

                            { name: 'DPIUName', index: 'DPIUName', height: 'auto', width: 180, align: "left", sortable: true },
                            { name: 'TANNo', index: 'TANNo', height: 'auto', width: 100, align: "left", sortable: true },
                            { name: 'BAEnabled', index: 'BAEnabled', height: 'auto', width: 80, align: "left", sortable: true },
                            { name: 'BAEnabledDate', index: 'BAEnabledDate', height: 'auto', width: 130, align: "left", sortable: true },
                            { name: 'EPayEnabled', index: 'EPayEnabled', height: 'auto', width: 80, align: "left", sortable: true },
                            { name: 'EPayEnabledDate', index: 'EPayEnabledDate', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'ERemittanceEnabled', index: 'ERemittanceEnabled', height: 'auto', width: 100 , align: "left", sortable: true },
                            { name: 'ERemittanceEnabledDate', index: 'ERemittanceEnabledDate', height: 'auto', width: 150, align: "left", sortable: true },
                            
                            { name: 'ActionDPIU', width: 60, resize: false, formatter: FormatColumn_DPIU, align: "center" }
                ],
                rowNum: 5,
                pager: pager_id, 
                height: 'auto',
                autowidth: true,
                rownumbers: true,
                rowList: [5, 10],
                sortname: 'DPIUName',
                sortorder: "asc",
                viewrecords: true,
                recordtext: '{2} records found',
                onSelectRow: function () {
                    $('#SRDARowID').val(row_id);
                },
                loadComplete: function () {
                    var reccount = $('#' + subgrid_table_id).getGridParam('reccount');
                    if (reccount == 0) {

                        CollapseAllOtherRowsSubGrid($('#SRDARowID').val());
                        //$("#adminCategory").collapseSubGridRow($('#SRDARowID').val());
                    }


                }

            });
            
        },

        subGridOptions: {
            "plusicon": "ui-icon-triangle-1-s",
            "minusicon": "ui-icon-triangle-1-n",
            "openicon": "ui-icon-arrowreturn-1-e",
         
            "expandOnLoad": false
        },

        onSelectRow: function (id) {

            $('#SRDARowID').val(id);         
          
        }

    });

    


}

function addDPIUData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });



    $.ajax({
        type: 'POST',
        url: '/Master/AddDistrictUnitDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#adminSearchDetails").is(":visible")) {
                $('#adminSearchDetails').hide('slow');
            }
            if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
                $('#dvMappedAgencyDistrictsDetails').hide('slow');
            }
            if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
                $('#dvMapAgencyDistrictsDetails').hide('slow');
            }

      

            $('#btnAdd').show();
            $('#btnSearch').show();
            
            $("#adminAddDetails").html(data);
            $("#adminAddDetails").show();
            $('#trAddNewSearch').show();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}




function CollapseAllOtherRowsSubGrid(rowid)
{
    var rowIds = $("#adminCategory").getDataIDs();
    $.each(rowIds, function (index, rowId) {
        $("#adminCategory").collapseSubGridRow(rowId);
    });
}

function FormatColumnMap(cellvalue, options, rowObject) {
    //Added By Ashish Markande on 9/10/2013
    if (cellvalue == "") {
        return "<center><table><tr><td  style='border:none'>-</td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><a href='#' title='Map Districts' onClick ='MapDistricts(\"" + cellvalue.toString() + "\");' >Map Districts</a></td></tr></table></center>";
    }

}

function FormatColumnMapped(cellvalue, options, rowObject) {

    //Added By Abhishek kamble on 20-Feb-2014
    if (cellvalue == "") {
        return "<center><table><tr><td  style='border:none'>-</td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Districts' onClick ='MappedDistricts(\"" + cellvalue.toString() + "\");' >Mapped Districts</a></td></tr></table></center>";
    }
}

function MapDistricts(parameter) {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/MapSRRDADistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            if ($("#dvMappedAgencyDistrictsDetails").is(":visible")) {
                $('#dvMappedAgencyDistrictsDetails').hide('slow');
            }

            $("#dvMapAgencyDistrictsDetails").html(data);
            $('#dvMapAgencyDistrictsDetails').show('slow');



            $('#trAddNewSearch').hide();
            $('#adminSearchDetails').hide();
            $('#adminAddDetails').hide();


            $('#adminCategory').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function MappedDistricts(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $.ajax({
        url: "/Master/MappedSRRDADistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            if ($("#dvMapAgencyDistrictsDetails").is(":visible")) {
                $('#dvMapAgencyDistrictsDetails').hide('slow');
            }

            $("#dvMappedAgencyDistrictsDetails").html(data);
            $('#dvMappedAgencyDistrictsDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#adminSearchDetails').hide();
            $('#adminAddDetails').hide();

          

            $('#adminCategory').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}
