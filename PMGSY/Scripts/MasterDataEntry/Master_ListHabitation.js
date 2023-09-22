$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {


    $.ajax({
        url: "/LocationMasterDataEntry/SearchHabitation",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchHabitation").html(data);
           

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

    LoadGrid();

    $.unblockUI();


    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchHabitation").is(":visible")) {
            $('#dvSearchHabitation').hide('slow');
        }

        if (!$("#dvHabitationDetails").is(":visible")) {
            $('#dvHabitationDetails').load("/LocationMasterDataEntry/CreateHabitation");
            $('#dvHabitationDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }

    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


        $('#dvHabitationDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();



        if (!$("#dvSearchHabitation").is(":visible")) {

            $('#dvSearchHabitation').load('/LocationMasterDataEntry/SearchHabitation', function () {

          

                var data = $('#tbHabitationList').jqGrid("getGridParam", "postData");              
              
                if (!(data === undefined)) {
                   
                    $('#txtSearchVillage').val("");                   
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
                        var village = data.villageName;
                        if ((village === '')) {                           
                            $('#btnSearch').trigger('click'); //newly added
                        }
                        else {
                            $('#txtSearchVillage').val(data.villageName);
                        }
                    }, 2000);
                    //if ($('#ddlSearchBlocks').val() > 0) {
                    //             setTimeout(function () {

                    //                $('#ddlSearchBlocks').val(data.blockCode);
                    //                LoadVillages();                                   
                    //             }, 3000);
                    //    // $('#txtSearchVillage').val(data.villageName);
                    //             //$('#txtSearchVillage').val("");
                    //        }
                }

                $('#dvSearchHabitation').show('slow');
            });
            
        }     

        $.unblockUI();

    });

    
  


    $("#dvShiftHabitationToNewVillage").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Habitation To New Village'
    });



});

function FormatColumn(cellvalue, options, rowObject) {


    if (cellvalue != '') {
        return "<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + cellvalue.toString() + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

    
}

function EditHabitationDetails(paramater) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/LocationMasterDataEntry/EditHabitation/" + paramater,
        type: "GET",
        async: false,
        cache: false,
      
        success: function (data) {

         

            if ($("#dvSearchHabitation").is(":visible")) {
                $('#dvSearchHabitation').hide('slow');
            }

            if ($("#dvOtherDetails").is(":visible")) {
                $('#dvOtherDetails').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();

            $("#dvHabitationDetails").html(data);
            $('#dvHabitationDetails').show('slow');
            $('#ddlVillages').trigger('change');
            $("#MAST_HAB_NAME").focus();

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);

            $.unblockUI();
        }

    });

}

function ViewHabitationDetails(paramater) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/LocationMasterDataEntry/EditHabitation/" + paramater,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {



            if ($("#dvSearchHabitation").is(":visible")) {
                $('#dvSearchHabitation').hide('slow');
            }

            if ($("#dvOtherDetails").is(":visible")) {
                $('#dvOtherDetails').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();

            $("#dvHabitationDetails").html(data);
            $('#dvHabitationDetails').show('slow');
            $('#ddlVillages').trigger('change');
            $("#MAST_HAB_NAME").focus();

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);

            $.unblockUI();
        }

    });

}
function LoadGrid()
{

    jQuery("#tbHabitationList").jqGrid({
        url: '/LocationMasterDataEntry/GetHabitationDetailsList',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Habitation System Id','Habitation Name', 'Village Name', 'Block Name', 'District Name', 'State Name', 'MP Constituency', 'MLA Constituency', 'Is IAP', 'Is Schedule5','Active','Shift Habitation', 'Action', 'View','Lock Status'],
        colModel: [
                             { name: 'HabitationCode', index: 'HabitationCode', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: 150, align: "left", sortable: true },                         
                             { name: 'VillageName', index: 'VillageName', height: 'auto', width: 120, align: "left", sortable: true },
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 120, align: "left", sortable: true, hidden: false, },
                             { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 120, sortable: true, align: "left", hidden: false, },
                             { name: 'StateName', index: 'StateName', height: 'auto', width: 130, sortable: true, align: "left", hidden: false, },
                             { name: 'MPConstituency', index: 'MPContituency', height: 'auto', width: 130, sortable: true, align: "left" },
                             { name: 'MLAConstituency', index: 'MLAContituency', height: 'auto', width: 130, sortable: true, align: "left" },
                             { name: 'IsIAP', index: 'IsIAP', height: 'auto', width: 50, sortable: true, align: "left", hidden: false, },
                             { name: 'IsSchedule5', index: 'IsSchedule5', height: 'auto', width: 100, sortable: true, align: "left" },
                             { name: 'Active', index: 'Active', width: 80, sortable: true, align: "center" },

                            // { name: 'ShiftHabitation', width: 130, sortable: false, resize: false, formatter: FormatColumnShiftHab, align: "center", hidden: (($("#RoleCode").val() == 23) ? false : true) }, //RoleCode=23 MasterAdmin
                             { name: 'ShiftHabitation', width: 130, sortable: false, resize: false, align: "center", hidden: (($("#RoleCode").val() == 36) || ($("#RoleCode").val() == 23) ? false : true) }, //RoleCode=23 MasterAdmin



                             { name: 'a', width: 100, sortable: false, resize: false, align: "center", sortable: false },//formatter: FormatColumn
                             { name: 'View', width: 50, sortable: false, resize: false, align: "center", sortable: false, hidden: true },
                             { name: 'LockStatus', width: 80, sortable: false, resize: false, align: "center", sortable: true, hidden: $("#RoleCode").val()==23?false:true}

        ],
        pager: jQuery('#dvHabitationListPager'),
        rowNum: 100,
      //  altRows: true,
        rowList: [100, 200, 300, 400],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName,BlockName,VillageName,HabitationName',
         sortorder: "asc",
        caption: "Habitation List",
        height: 'auto',
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
     
        loadComplete: function () {

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


function FormatColumnShiftHab(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Habitation to new Village' onClick ='ShiftHabToNewVillage(\"" + cellvalue.toString() + "\");' >Shift Habitation</a></td></tr></table></center>";

}


function ShiftHabitationNew(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftHabitationToNewVillage').empty();
    $("#dvShiftHabitationToNewVillage").load("/LocationMasterDataEntry/ShiftHabitationToNewVillage?id=" + parameter, function () {

        $("#dvShiftHabitationToNewVillage").dialog('open');
        $.unblockUI();
    })

}

//function ShiftHabToNewVillage(parameter) {

//    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//    $('#dvShiftHabitationToNewVillage').empty();
//    $("#dvShiftHabitationToNewVillage").load("/LocationMasterDataEntry/ShiftHabitationToNewVillage?id=" + parameter, function () {

//        $("#dvShiftHabitationToNewVillage").dialog('open');
//        $.unblockUI();
//    })

//}


function AddOtherHabitationDetails(paramater) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $('dvOtherDetails').empty();

    $("#dvOtherDetails").load("/LocationMasterDataEntry/OtherHabitationDetails?id=" + paramater, function () {

        //$("#dvOtherDetails").dialog('open');
        
 
        $("#dvOtherDetails").show('slow');

        $('#trAddNewSearch').hide();
        if ($('#dvSearchHabitation').is(":visible")) {
            $('#dvSearchHabitation').hide();
        }
        else {
            $('#dvHabitationDetails').hide();
        }
        $('#tbHabitationList').jqGrid("setGridState", "hidden");

        $.unblockUI();

   
        
    })
    
}

function DeleteHabitationDetails(urlparamater) {


    if (confirm("Are you sure you want to delete habitation details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvOtherDetails").is(":visible")) {
            $('#dvOtherDetails').hide('slow');
        }

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteHabitationDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchHabitation").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbHabitationList').trigger('reloadGrid');
                    //}
                    if ($("#dvHabitationDetails").is(":visible")) {

                        $("#dvHabitationDetails").hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }
                    if (!$("#dvSearchHabitation").is(":visible")) {
                        $("#dvSearchHabitation").show('slow');
                        $('#tbHabitationList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbHabitationList').trigger('reloadGrid');
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

        if (!$("#dvHabitationDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }

    }
    else {
        return false;
    }
}
