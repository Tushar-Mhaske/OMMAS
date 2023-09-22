$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    var PMGSYScheme = $('#hdnPMGSYScheme').val();
   
    var Year;
    if (PMGSYScheme == '1') {
        Year = 2001;
    }
    else {
        Year = 2011;
    }
    $.ajax({
        url: "/LocationMasterDataEntry/SearchVillage",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchVillage").html(data);
            //$("#ddlSearchStates").trigger('change');
            
            ////$("#ddlSearchDistrict").trigger('change');
           
            //End Change by Deepak 2-Sept-2014
            if ($("#RoleCode").val() != 22) {  //PIU RoledCode=22
                setTimeout(function () {
                    $('#btnSearch').trigger('click');

                }, 700);
            }


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });

    LoadGrid( Year);

    $.unblockUI();


    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchVillage").is(":visible")) {
            $('#dvSearchVillage').hide('slow');
        }

        if (!$("#dvVillageDetails").is(":visible")) {
            $('#dvVillageDetails').load("/LocationMasterDataEntry/CreateVillage");
            $('#dvVillageDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }


    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvVillageDetails").is(":visible")) {
            $('#dvVillageDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }

        if (!$("#dvSearchVillage").is(":visible")) {


            $('#dvSearchVillage').load('/LocationMasterDataEntry/SearchVillage', function () {

                //$('#tbVillageList').trigger('reloadGrid');

                var data = $('#tbVillageList').jqGrid("getGridParam", "postData");
               
                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);

                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                  "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

                    setTimeout(function () {
                     
                        $('#ddlSearchDistrict').val(data.districtCode);

                        $('#ddlSearchBlocks').empty();
                        FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                                    "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());

                    }, 1000);

                  
                    setTimeout(function () {
                                             
                        $('#ddlSearchBlocks').val(data.blockCode);

                    }, 2000);
                } 
                $('#dvSearchVillage').show('slow');
               
            });          
           
        }

       
        $.unblockUI();
    });


    
    $("#dvShiftVillage").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Village'
    });


    $("#dvShiftVillageBlock").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Village Block'
    });


});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

   // return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit Village Details' onClick ='EditVillageDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Village Details' onClick ='EditVillageDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Village Details' onClick ='DeleteVillageDetails(\"" + cellvalue.toString() + "\");'></span></td>  </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function FormatColumnShift(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Village' onClick ='ShiftVillage(\"" + cellvalue.toString() + "\");' >Shift Village</a></td></tr></table></center>";

}



function FormatColumnShiftVillageBlockDetails(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Village to new Block' onClick ='FormatColumnShiftVillageBlock(\"" + cellvalue.toString() + "\");' >Shift Village Block</a></td></tr></table></center>";

}


function EditVillageDetails(paramater) {


    $.ajax({
        url: "/LocationMasterDataEntry/EditVillage/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            //  $("#mainDiv").html(data);

            if ($("#dvSearchVillage").is(":visible")) {
                $('#dvSearchVillage').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();

            $("#dvVillageDetails").html(data);
            $('#dvVillageDetails').show('slow');
            $("#ddlBlocks").trigger('change');
            $("#MAST_VILLAGE_NAME").focus();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });

   
}

function LoadGrid(Year) {
    jQuery("#tbVillageList").jqGrid({
        url: '/LocationMasterDataEntry/GetVillageDetailsList',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Village Name', 'Block Name', 'District Name', 'State Name','Is IAP', 'Total Population', 'SC/ST Population', 'Is Schedule5','Active', 'Shift Village','Shift Village Block', 'Action','Lock Status'],
        colModel: [
                             { name: 'VillageName', index: 'VillageName', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 120, sortable: true, align: "left", hidden: false },
                             { name: 'StateName', index: 'StateName', height: 'auto', width: 150, sortable: true, align: "left" },
                             { name: 'ISIAP', index: 'IsIAP', height: 'auto', width: 70, sortable: true, align: "center" },
                             { name: 'TotalPopulation', index: 'TotalPopulation', width: 120, sortable: true, align: "right" },
                             { name: 'SCSTPopulation', index: 'SCSTPopulation', width: 120, sortable: true, align: "right" },
                             { name: 'IsSchedule5', index: 'IsSchedule5', width: 80, sortable: true, align: "center" },
                             { name: 'Active', index: 'Active', width: 80, sortable: true, align: "center" },
                             //{ name: 'Shift', width: 130, sortable: false, resize: false, formatter: FormatColumnShift, align: "center", hidden: (($("#RoleCode").val() == 23 || $("#RoleCode").val() == 36) ? false : true) }, //RoleCode=23 MasterAdmin
                             { name: 'Shift', width: 130, sortable: false, resize: false, formatter: FormatColumnShift, align: "center", hidden: false }, //RoleCode=23 MasterAdmin

                             { name: 'ShiftVillage', width: 130, sortable: false, resize: false, align: "center", hidden: (($("#RoleCode").val() == 36) || ($("#RoleCode").val() == 23) ? false : true) }, //RoleCode=23 MasterAdmin
                            // { name: 'ShiftVillage', width: 130, sortable: false, resize: false, formatter: FormatColumnShiftVillageBlockDetails, align: "center", hidden: (($("#RoleCode").val() == 23) ? false : true) }, //RoleCode=23 MasterAdmin
                            

                           // { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
                             { name: 'a', width: 80, sortable: false, resize: false,  align: "center", sortable: false },
                             { name: 'lockStatus',index: 'lockStatus', width: 80, sortable: true, resize: false, align: "center", sortable: true,hidden: ($("#RoleCode").val() == 23 ? false : true) }

        ],
        pager: jQuery('#dvVillageListPager'),
        rowNum: 100,
        //altRows: true,
        rowList: [ 100, 200, 300, 400],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName,BlockName,VillageName,lockStatus',
        sortorder: "asc",
        caption: 'Village List ( Census Year - '+ Year +' )',
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
      /*  grouping: true,
        groupingView:
            {
                groupField: ['StateName', 'DistrictName', 'BlockName'],//'BlockName'
                groupColumnShow: [false, false, false],
                groupText: ['<b>State: {0}</b>', '<b>District: {0}</b>', '<b>Block: {0}</b>']
                //groupCollapse: true,
                //groupOrder: ['asc']
            },*/
        loadComplete: function (data) {
            var recordCount = jQuery('#tbVillageList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {

                var button = '<input type="button" id="btnFinalizeVillage" name="btnFinalizeVillage" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize State" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeVillage()" />'
                $('#dvVillageListPager_left').html(button);

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


//function FormatColumnShiftVillageBlock(parameter) {

//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $('#dvShiftVillageBlock').empty();
//    $("#dvShiftVillageBlock").load("/LocationMasterDataEntry/ShiftVillageBlock?id=" + parameter, function () {

//        $("#dvShiftVillageBlock").dialog('open');
//        $.unblockUI();
//    })

//}
function ShiftVillageBlockNew(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftVillageBlock').empty();
    $("#dvShiftVillageBlock").load("/LocationMasterDataEntry/ShiftVillageBlock?id=" + parameter, function () {

        $("#dvShiftVillageBlock").dialog('open');
        $.unblockUI();
    })

}


function ShiftVillage(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftVillage').empty();
    $("#dvShiftVillage").load("/LocationMasterDataEntry/ShiftVillage?id=" + parameter, function () {

        $("#dvShiftVillage").dialog('open');
        $.unblockUI();
    })

}

function DeleteVillageDetails(urlparamater) {


    if (confirm("Are you sure you want to delete village details ?,  Population details for all census year will be deleted after deletion of village details.")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteVillageDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchVillage").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbVillageList').trigger('reloadGrid');
                    //}
                    if ($("#dvVillageDetails").is(":visible")) {
                        $('#dvVillageDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }
                    if (!$("#dvSearchVillage").is(":visible")) {
                        $('#dvSearchVillage').show('slow');
                        $('#tbVillageList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbVillageList').trigger('reloadGrid');

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
function FinalizeVillage() {
    var id = $('#tbVillageList').jqGrid('getGridParam', 'selrow');

    if ($('#tbVillageList').jqGrid('getGridParam', 'selrow')) {

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizeVillage/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('Village Finalized Successfully.');
                    $("#tbVillageList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('Error occurred while processing your request.');
                }
            },
            error: function () { }
        });
    }
    else {
        alert('Please select village to finalize.');
    }
}