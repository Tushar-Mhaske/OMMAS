var statCode;
var districtCode;
var blockCode;
$(document).ready(function () {
  //  $.validator.unobtrusive.parse($('#frmSearchCluster'));
    if (!$("#ClusterSearchDetails").is(":visible")) {

        $('#ClusterSearchDetails').load('/Master/SearchCluster');
        $('#ClusterSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddCluster');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
       
        if ($("#ClusterSearchDetails").is(":visible")) {
            $('#ClusterSearchDetails').hide('slow');
        }

        $('#ClusterAddDetails').load("/Master/AddEditCluster");

        $('#ClusterAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();
        if ($("#dvViewClusterHabiationDetails").is(":visible")) {
            $("#dvViewClusterHabiationDetails").hide('slow');
        }
        $("#tblCluster").jqGrid('GridUnload');
    });

    $('#btnSearch').click(function (e) {
       
  
        if ($("#ClusterAddDetails").is(":visible")) {
            $('#ClusterAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
            if ($("#loadViewAddHabitationCluster").is(":visible")) {
                $("#loadViewAddHabitationCluster").hide('slow');
            }
            if ($("#tdbtnUpdateCluster").is(":visible")) {
                $("#tdbtnUpdateCluster").hide('slow');
            }
            if ($("#dvViewClusterHabiationDetails").is(":visible")) {
                $("#dvViewClusterHabiationDetails").hide('slow');
            }
        }

        if (!$("#ClusterSearchDetails").is(":visible")) {

            $('#ClusterSearchDetails').load('/Master/SearchCluster', function () {
               // var data = $('#tblCluster').jqGrid("getGridParam", "postData");

                //if (!(data === undefined)) {

                //    $('#StateList_ClusterDetails').val(data.StateCode);
                //    $('#DistrictList_ClusterDetails').val(data.DistrictCode);
                //    $('#BlockList_ClusterDetails').val(data.BlockCode);
                

                //}
                //LoadClusterGrid();

                $('#ClusterSearchDetails').show('slow');
                //$("#tblCluster").jqGrid('GridUnload');
                MaintainStateDistrictBlockDropDownFilterOfSearchScreen();

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

    //*****Region Update Cluster Name  
    //For Popup of Udate Cluster Name 

    $("#dvEditCluster").dialog({
        autoOpen: false,
        height: 'auto',
        width: "550",
        modal: true,
        title: 'Edit Cluster Name'
    });
    
    //End Region Cluster Name Update of Popup
}); //End document.ready()


function MaintainStateDistrictBlockDropDownFilterOfSearchScreen() {
    if ($("#StateList_ClusterAddDetails").val() > 0) {
        $("#StateList_ClusterDetails").val($("#StateList_ClusterAddDetails").val());

        loadDistrict($("#StateList_ClusterDetails").val());

        //alert($("#DistrictList_ClusterDetails").val());
        if ($("#DistrictList_ClusterAddDetails").val() > 0) {

            loadBlock($("#StateList_ClusterDetails").val(), $("#DistrictList_ClusterAddDetails").val());

            setTimeout(function () {
                // alert($("#DistrictList_ClusterDetails").val());
                $("#DistrictList_ClusterDetails").val($("#DistrictList_ClusterAddDetails").val());

            }, 1000);
        }
        if ($("#BlockList_ClusterAddDetails").val() > 0) {
            setTimeout(function () {
                $("#BlockList_ClusterDetails").val($("#BlockList_ClusterAddDetails").val());
            }, 1200);
            setTimeout(function () {
                $('#btnClusterSearch').trigger('click');
            }, 1500);
        }


    }
}


function LoadClusterGrid() {
    if ($('#frmSearchCluster').valid()) {
        $("#tblCluster").jqGrid('GridUnload');
        jQuery("#tblCluster").jqGrid({
            url: '/Master/GetClusterList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State ','District','Block','Cluster Name', 'No of Habitation', 'Total Population', 'Total SC/ST Population', 'Cluster Active Status', 'Edit', 'Delete', 'View'],
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 150, align: "left", sortable: true, hidden: true },
                        { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'MAST_CLUSTER_NAME', index: 'MAST_CLUSTER_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                        { name: 'NoOfHabiation', index: 'NoOfHabiation', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'TotPopulation', index: 'TotPopulation', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'SCSTPopulation', index: 'SCSTPopulation', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'Status', index: 'Status', height: 'auto', width: 100, align: "center", sortable: true,hidden:true },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 40, align: "left", sortable: false },
                        { name: 'Delete', index: 'Delete', height: 'auto', width: 40, align: "left", sortable: false },
                        { name: 'View', index: 'View', height: 'auto', width: 40, align: "left", sortable: true }
                      //  { name: 'ActionCluster', width: 60, resize: false, formatter: FormatColumn_Cluster, align: "center" }
            ],
            postData: { StateCode: $('#StateList_ClusterDetails option:selected').val(), DistrictCode: $('#DistrictList_ClusterDetails option:selected').val(), BlockCode: $('#BlockList_ClusterDetails option:selected').val(), Status: $('#StatusList_ClusterDetails option:selected').val() },
            pager: jQuery('#divClusterPager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_DISTRICT_NAME',
            sortorder: "asc",
            caption: "Cluster List",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            loadComplete: function () {
                var Tot_TotPopulation = $(this).jqGrid('getCol', 'TotPopulation', false, 'sum');
                var TotSCSTPopulation = $(this).jqGrid('getCol', 'SCSTPopulation', false, 'sum');
                //Commented By Abhishek kamble 20-Feb-2014
                //var SRDARowID = $('#SRDARowID').val();
                //if (SRDARowID != '') {
                //    $("#adminCategory").expandSubGridRow(SRDARowID);
                //}
                $(this).jqGrid('footerData', 'set', { ClusterName: '<b>Total</b>' });
                $(this).jqGrid('footerData', 'set', { TotPopulation: Tot_TotPopulation }, true);
                $(this).jqGrid('footerData', 'set', { SCSTPopulation: TotSCSTPopulation }, true);


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



        });

    }



}

function FormatColumn_Cluster(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Cluster Details' onClick ='EditClusterDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete DPIU Details' onClick ='DeleteClusterDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



function DeleteClusterDetails(urlparameter) {
    if (confirm("Are you sure you want to delete Cluster details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteCluster/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    if ($("#ClusterSearchDetails").is(":visible")) {
                        $('#btnClusterSearch').trigger('click');

                    }
                    else {
                        $('#tblCluster').trigger('reloadGrid');
                    }
                   // $("#ClusterAddDetails").load("/Master/AddEditPIUDepartment");
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

        //if (!$("#ClusterAddDetails").is(':visible')) {
        //    $('#btnClusterSearch').trigger('click');
        //    $('#ClusterSearchDetails').show();
        //    $('#trAddNewSearch').show();
        //}

    }
    else {
        return false;
    }

}

//*****Region Update Cluster Name  
function EditClusterDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvEditCluster').empty();
    $("#dvEditCluster").load("/Master/EditCluster/" + urlparameter, function () {

        $("#dvEditCluster").dialog('open');
        $.unblockUI();
    })

}

function ViewClusterDetails(urlparameter) {
    $("#dvViewClusterHabiationDetails").show('slow');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/ViewClusterHabitation/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            //if ($("#ClusterSearchDetails").is(":visible")) {
            //    $('#ClusterSearchDetails').hide('slow');
            //}
            //$('#btnAdd').hide();
           // $('#btnSearch').show();

            $("#dvViewClusterHabiationDetails").html(data);
           

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

//***End Region Cluster Name Update







