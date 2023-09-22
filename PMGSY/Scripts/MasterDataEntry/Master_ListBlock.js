

$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });



$(document).ready(function () {

    // $('#dvSearchBlock').load('/LocationMasterDataEntry/SearchBlock');


    $.ajax({
        url: "/LocationMasterDataEntry/SearchBlock",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchBlock").html(data);
            $('#btnSearch').trigger('click');

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });

    LoadGrid();

    $.unblockUI();


    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchBlock").is(":visible")) {
            $('#dvSearchBlock').hide('slow');
        }

        if (!$("#dvBlockDetails").is(":visible")) {
            $('#dvBlockDetails').load("/LocationMasterDataEntry/CreateBlock");
            $('#dvBlockDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }


    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvBlockDetails").is(":visible")) {
            $('#dvBlockDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchBlock").is(":visible")) {

            $('#dvSearchBlock').load('/LocationMasterDataEntry/SearchBlock', function () {

                // $('#tbBlockList').trigger('reloadGrid');

                var data = $('#tbBlockList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);

                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                  "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

                    setTimeout(function () {
                        // $("#ddlSearchDistrict").find("option[value='0']").text('All Districts');
                        $('#ddlSearchDistrict').val(data.districtCode);
                    }, 1000);
                }
                $('#dvSearchBlock').show('slow');
            });
        }

        $.unblockUI();


    });

    $("#dvShiftBlock").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Block'
    });


});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    //return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit District Details' onClick ='EditBlockDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Block Details' onClick ='EditBlockDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Block Details' onClick ='DeleteBlockDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnShift(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Block' onClick ='ShiftBlock(\"" + cellvalue.toString() + "\");' >Shift Block</a></td></tr></table></center>";

}

function EditBlockDetails(paramater) {


    $.ajax({
        url: "/LocationMasterDataEntry/EditBlock/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            //  $("#mainDiv").html(data);
            if ($("#dvSearchBlock").is(":visible")) {
                $('#dvSearchBlock').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            
            $("#dvBlockDetails").html(data);
            $('#dvBlockDetails').show('slow');
            $("#MAST_BLOCK_NAME").focus();
            $("#ddlDistricts").trigger('change');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });
}

function LoadGrid() {
    
    jQuery("#tbBlockList").jqGrid({
        url: '/LocationMasterDataEntry/GetBlockDetailsList',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Block Id', 'Block Name', 'District Name', 'State name', 'Is District IAP', 'Is Most Affected IAP', 'Is Desert', 'Is Tribal', 'Is Included In PMGSY', 'Is Schedule5', 'Active', 'Shift Block', 'Lock Status', 'Is Border Area', 'Action'],
        colModel: [
                            { name: 'MastBlockId', index: 'MastBlockId', height: 'auto', width: 90, align: "center", sortable: true },
                            { name: 'BlockName', index: 'BlockName', height: 'auto', width: 180, align: "left", sortable: true },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 180, sortable: true, align: "left" },
                            { name: 'IsDistrictIAP', index: 'IsDistrictIAP', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'IsIAP', index: 'IsIAP', height: 'auto', width: 150, sortable: true, align: "left" },
                            { name: 'IsDESERT', index: 'IsDESERT', width: 110, sortable: true },
                            { name: 'IsTRIBAL', index: 'IsTRIBAL', width: 110, sortable: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 150, sortable: true },
                            { name: 'IsSchedule5', index: 'IsSchedule5', width: 80, sortable: true },
                            { name: 'Active', index: 'Active', width: 80, sortable: true },
                            { name: 'Shift', width: 140, sortable: false, resize: false, formatter: FormatColumnShift, align: "center", hidden: (($("#RoleCode").val() == 23 || $("#RoleCode").val() == 36) ? false : true) }, //RoleCode=23 MasterAdmin
                            //{ name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                            
                            { name: 'lockStatus',index: 'lockStatus', width: 80, sortable: true, resize: false, align: "center", hidden: ($("#RoleCode").val() == 23 ? false : true) },
                            { name: 'IsBADB', index: 'IsBADB', width: 110, sortable: true, align: "center" },
                            { name: 'a', width: 80, sortable: false, resize: false, align: "center", sortable: false },
        ],
        pager: jQuery('#dvBlockListPager'),
        rowNum: 15,
        //  altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,DistrictName,BlockName,lockStatus',
        sortorder: "asc",
        caption: "Block List",
        height: 'auto',
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        /*grouping: true,
        groupingView:
            {
                groupField: ['StateName', 'DistrictName'],
                groupColumnShow: [false, false],
                groupText: ['<b>State: {0}</b>', '<b>District: {0}</b>']
                //groupCollapse: true,
                //groupOrder: ['asc']
            },*/
        loadComplete: function () {
            //$.unblockUI();
            var recordCount = jQuery('#tbBlockList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {

                var button = '<input type="button" id="btnFinalizeBlock" name="btnFinalizeBlock" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize Block" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeBlock()" />'
                $('#dvBlockListPager_left').html(button);

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

function ShiftBlock(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftBlock').empty();
    $("#dvShiftBlock").load("/LocationMasterDataEntry/ShiftBlock?id=" + parameter, function () {

        $("#dvShiftBlock").dialog('open');
        $.unblockUI();
    })

}

function DeleteBlockDetails(urlparamater) {


    if (confirm("Are you sure you want to delete block details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteBlockDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchBlock").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbBlockList').trigger('reloadGrid');
                    //}
                    if ($("#dvBlockDetails").is(":visible")) {
                        $('#dvBlockDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }
                    if (!$("#dvSearchBlock").is(":visible")) {
                        $('#dvSearchBlock').show('slow');
                        $('#tbBlockList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbBlockList').trigger('reloadGrid');

                    }

                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

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
function FinalizeBlock() {
    var id = $('#tbBlockList').jqGrid('getGridParam', 'selrow');

    if ($('#tbBlockList').jqGrid('getGridParam', 'selrow')) {

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizeBlock/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Block Finalized Successfully.');
                    $("#tbBlockList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('Error occurred while processing your request.');
                }
            },
            error: function () { }
        });
    }
    else {
        alert('Please select block to finalize.');
    }
}