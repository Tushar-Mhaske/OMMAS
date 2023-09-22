var T_TotalPopulation;
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmViewClusterHabiationDetails'));
    LoadViewHabitaionClusterGrid();
    $("#btnAddClusterHabiation").click(function () {
        LoadViewHabitaionAddInClusterGrid();
        $("#tblViewHabitationCluster").jqGrid('setGridState', 'hidden');

    });

    $("#btnUpdateClusterHabiation").click(function () {

        UpdateClusterHabitation();
    });
    $("#btnFinalizeCluster").click(function () {
        FinalizeClusterHabitation();

    });
    $("#btnCancelClusterHabiation").click(function () {
        if ($("#dvViewClusterHabiationDetails").is(":visible")) {
            $("#dvViewClusterHabiationDetails").hide('slow');
        }
        if ($("#loadViewAddHabitationCluster").is(":visible")) {
            $("#loadViewAddHabitationCluster").hide('slow');
        }
        if ($("#tdbtnUpdateCluster").is(":visible")) {
            $("#tdbtnUpdateCluster").hide('slow');
        }
        $("#tblCluster").jqGrid('setGridState', 'visible');


    });
    $("#btnUpdateCancelClusterHabiation").click(function () {

        if ($("#loadViewAddHabitationCluster").is(":visible")) {
            $("#loadViewAddHabitationCluster").hide('slow');
        }
        if ($("#tdbtnUpdateCluster").is(":visible")) {
            $("#tdbtnUpdateCluster").hide('slow');
        }
        $("#tblViewHabitationCluster").jqGrid('setGridState', 'visible');
    });

});
function LoadViewHabitaionClusterGrid() {

    if ($('#frmViewClusterHabiationDetails').valid()) {
        $("#tblCluster").jqGrid('setGridState', 'hidden');
        $("#tblViewHabitationCluster").jqGrid('GridUnload');
        $('#tblViewHabitationCluster').jqGrid({

            url: '/Master/GetViewClusterHabitatonList',
            datatype: "json",
            mtype: "POST",
            colNames: ["Habitation", "Village", "Connectivity Status", "Total Population", "SC/ST Population", "Delete"],
            colModel: [
                        { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: '200', align: "left", sortable: true },
                       { name: 'VillageName', index: 'VillageName', height: 'auto', width: '200', align: "left", sortable: true },
                       { name: 'ConnStatus', index: 'ConnStatus', height: 'auto', width: '100', align: "center", sortable: true },
                       { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '150', align: "center", sortable: true },
                       { name: 'STPopulation', index: 'STPopulation', height: 'auto', width: '150', align: "center", sortable: true },
                       { name: 'Delete', index: 'Delete', height: 'auto', width: '50', align: "center", sortable: false }
            ],
            postData: { StateCode: $('#StatCode').val(), DistrictCode: $('#DistrictCode').val(), BlockCode: $('#BlockCode').val(), EncryptedClusterCode: $('#EncryptedClusterCode').val() },
            // postData: $('#frmViewClusterHabiationDetails').serialize(),
            pager: jQuery('#divViewHabitationClusterPager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'HabitationName',
            sortorder: "asc",
            caption: "Cluster Habitation List",
            height: 'auto',
            //autowidth: true,
            width: '100%',
            shrinToFit: true,
            rownumbers: true,
            pginput: true,
            hidegrid: true,
            footerrow: true,
            loadComplete: function () {
                //  $("#tblViewHabitationCluster").jqGrid('setGridWidth', $('#dvhdAddNewClusterDetails').width()-20, true);
                 T_TotalPopulation = $(this).jqGrid('getCol', 'TotalPopulation', false, 'sum');
                var T_STPopulation = $(this).jqGrid('getCol', 'STPopulation', false, 'sum');
                //Commented By Abhishek kamble 20-Feb-2014
                //var SRDARowID = $('#SRDARowID').val();
                //if (SRDARowID != '') {
                //    $("#adminCategory").expandSubGridRow(SRDARowID);
                //}
                $(this).jqGrid('footerData', 'set', { VillageName: '<b>Total</b>' });
                $(this).jqGrid('footerData', 'set', { TotalPopulation: T_TotalPopulation }, true);
                $(this).jqGrid('footerData', 'set', { STPopulation: T_STPopulation }, true);


            }
        });

    }


}


function LoadViewHabitaionAddInClusterGrid() {

    if ($('#frmViewClusterHabiationDetails').valid()) {
        $("#loadViewAddHabitationCluster").show('slow');

        $("#tblViewAddHabitationCluster").jqGrid('GridUnload');
        $('#tblViewAddHabitationCluster').jqGrid({

            url: '/Master/GetViewAddHabitationListIntoCluster',
            datatype: "json",
            mtype: "POST",
            colNames: ["Habitation", "Village", "Connectivity Status", "Total Population", "SC/ST Population"],
            colModel: [
                       { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: '200', align: "left", sortable: true },
                       { name: 'VillageName', index: 'VillageName', height: 'auto', width: '200', align: "left", sortable: true },
                       { name: 'ConnStatus', index: 'ConnStatus', height: 'auto', width: '150', align: "center", sortable: true },
                       { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: '150', align: "center", sortable: true },
                       { name: 'STPopulation', index: 'STPopulation', height: 'auto', width: '150', align: "center", sortable: true }

            ],
            postData: { StateCode: $('#StatCode').val(), DistrictCode: $('#DistrictCode').val(), BlockCode: $('#BlockCode').val(), EncryptedClusterCode: $('#EncryptedClusterCode').val() },
            //postData: $('#frmAddCluster').serialize(),
            pager: jQuery('#divViewAddHabitationClusterPager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'HabitationName',
            sortorder: "asc",
            caption: "Habitation List",
            height: 'auto',
            //autowidth: true,
            width: '100%',
            shrinToFit: true,
            rownumbers: true,
            pginput: true,
            hidegrid: true,
            multiselect: true,
            shrinkToFit: true,
            onSelectRow: function (id, e) {

            },
            loadComplete: function () {
                $('#divViewAddHabitationClusterPager_left').html('[<b> Note</b>:Select checkbox to add Habitation in Cluster.]');
                //$("#gview_tblViewAddHabitationCluster > .ui-jqgrid-titlebar").hide();
                $("#tdbtnUpdateCluster").show('slow');
                // $("#tblViewAddHabitationCluster").jqGrid('setGridWidth', $('#dvhdAddNewClusterDetails').width()-20, true);
                $("#tblViewAddHabitationCluster").jqGrid('setGridWidth', $('#loadViewHabitationCluster').width(), true);


            }
        });

    }


}

function DeleteClusterHabiationDetails(urlparameter) {
    if (confirm("Are you sure you want to delete Cluster Habitation details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteClusterHabitation/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#ClusterSearchDetails").is(":visible")) {
                    //    $('#btnClusterSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblCluster').trigger('reloadGrid');
                    //}
                    //$('#tblViewHabitationCluster').trigger('reloadGrid');
                    $('#tblCluster').trigger('reloadGrid');
                    $("#tblViewHabitationCluster").trigger('reloadGrid');
                    $("#tblViewHabitationCluster").jqGrid('setGridState', 'visible');
                    $("#tblCluster").jqGrid('setGridState', 'hidden');

                    if ($("#loadViewAddHabitationCluster").is(":visible")) {
                        $("#loadViewAddHabitationCluster").hide('slow');
                    }
                    if ($("#tdbtnUpdateCluster").is(":visible")) {
                        $("#tdbtnUpdateCluster").hide('slow');
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

function UpdateClusterHabitation() {
    if ($('#frmViewClusterHabiationDetails').valid()) {

        var HabCode = $("#tblViewAddHabitationCluster").jqGrid('getGridParam', 'selarrrow');

        if (HabCode == "") {

            alert("Please select Habitation.");
        }
        else {
            if (confirm("Are you sure want to Add habitation.")) {
                $.ajax({
                    type: 'POST',
                    url: "/Master/UpdateClusterHabitation?HabCode=" + HabCode,
                    data: { ClusterName: $('#ClusterName').val(), StateCode: $('#StatCode').val(), DistrictCode: $('#DistrictCode').val(), BlockCode: $('#BlockCode').val(), EncryptedClusterCode: $('#EncryptedClusterCode').val() },
                    async: false,
                    success: function (data) {
                        if (data.success) {
                            $("input:checkbox").removeAttr('checked');
                            alert(data.message);
                            //if ($("#ClusterSearchDetails").is(":visible")) {
                            //    $('#btnClusterSearch').trigger('click');

                            //}
                            //else {
                            //    $('#tblCluster').trigger('reloadGrid');
                            //}
                            //if ($("#dvViewClusterHabiationDetails").is(":visible")) {
                            //    $("#dvViewClusterHabiationDetails").hide('slow');
                            //}
                            $('#tblCluster').trigger('reloadGrid');
                            $("#tblViewHabitationCluster").trigger('reloadGrid');
                            if ($("#loadViewAddHabitationCluster").is(":visible")) {
                                $("#loadViewAddHabitationCluster").hide('slow');
                            }
                            if ($("#tdbtnUpdateCluster").is(":visible")) {
                                $("#tdbtnUpdateCluster").hide('slow');
                            }
                            $("#tblViewHabitationCluster").jqGrid('setGridState', 'visible');
                            $("#tblCluster").jqGrid('setGridState', 'hidden');
                          
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }
    }
    else {
        return false;
    }
}

function FinalizeClusterHabitation() {
   
    if ($('#frmViewClusterHabiationDetails').valid()) {
        if (parseFloat(T_TotalPopulation) >= 250) {
            if (confirm("Are you sure want to finalize cluster.")) {
                $.ajax({
                    url: "/Master/FinalizeClusterHabitation",
                    type: "POST",
                    dataType: "json",
                    data: $("#frmViewClusterHabiationDetails").serialize(),
                    success: function (data) {
                        if (data.success) {
                            alert(data.message);
                            if ($("#ClusterSearchDetails").is(":visible")) {
                                $('#btnClusterSearch').trigger('click');

                            }
                            else {
                                $('#tblCluster').trigger('reloadGrid');
                            }
                            $('#tblViewHabitationCluster').trigger('reloadGrid');
                            $("#loadViewAddHabitationCluster").hide('slow');
                            $("#tdbtnUpdateCluster").hide('slow');
                            $("#tblCluster").jqGrid('setGridState', 'visible');
                        }
                        else {
                            alert(data.message);
                        }
                    }

                });
            }
        }
        else {
            alert("Total poulation should require more than equal to 250");
        }

    }
    else {
        return false;
    }
}