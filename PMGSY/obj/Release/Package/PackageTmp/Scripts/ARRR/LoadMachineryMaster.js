$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmLoadMachineryMaster');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnAdd").click(function () {
        $.ajax({
            url: '/ARRR/MachineryMasterLayout/',
            type: 'POST',
            //data: { useraction: "A" },
            success: function (jsonData) {

                $('#dvLoadMachineryMaster').html('');
                $('#dvLoadMachineryMaster').html(jsonData);
                $('#dvLoadMachineryMaster').show('slow');
                $("#btnAdd").hide('slow');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });

    });

    LoadMachineryMasterGrid();
});

function LoadMachineryMasterGrid() {
    if ($("#frmLoadMachineryMaster").valid()) {
        $("#tblLoadMachineryMasterList").jqGrid('GridUnload');

        jQuery("#tblLoadMachineryMasterList").jqGrid({
            url: '/ARRR/GetMachineryList',
            datatype: "json",
            mtype: "POST",
            colNames: ['Machinery Name', 'Activity Description', 'Output Master', 'Output Unit', 'Usage Unit','LMM Code','Category', 'Active', 'Edit', 'Delete'], //26
            colModel: [
                        { name: 'MachineryName', index: 'Machinery Name', height: 'auto', width: 200, align: "left", sortable: true },
                        { name: 'Desc', index: 'Desc', height: 'auto', width: 160, align: "center", sortable: true },
                        { name: 'OutputMaster', index: 'Output Master', height: 'auto', width: 120, align: "center", sortable: false },
                        { name: 'OutputUnit', index: 'Output Unit', height: 'auto', width: 120, align: "center", sortable: false },
                        { name: 'UsageUnit', index: 'Usage Unit', height: 'auto', width: 150, align: "center", sortable: false },
                        { name: 'LMM Code', index: 'LMM Code', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'Category', index: 'Category', height: 'auto', width: 120, align: "center", sortable: false },
                        { name: 'Activity', index: 'Activity', height: 'auto', width: 80, align: "center", sortable: false },
                        { name: 'Edit', index: 'Edit', height: 'auto', width: 80, align: "center", sortable: false },
                        { name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center", sortable: false },

            ],
            //            postData: { stateCode: $('#ddlMrdDropState option:selected').val(), agency: $('#ddlMrdDropAgency option:selected').val(), year: $('#ddlMrdDropPhaseYear option:selected').val(), batch: $('#ddlMrdDropBatch option:selected').val(), collaboration: $('#ddlMrdDropCollaboration option:selected').val() },
            pager: jQuery('#dvLoadMachineryMasterListpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_ITEM_CODE',
            sortorder: "asc",
            caption: "Machinery Master List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            shrinkToFit: false,
            rownumbers: true,
            cmTemplate: { title: false },
            loadComplete: function () {
                //$("#tblMrdClearenceLetter").jqGrid('setGridWidth', $("#MrdClearenceLetterList").width(), true);
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

function loadEditMachineryMasterDetails(param) {
    //alert(param);
    //$('#tblConditionMasterList').jqGrid('setGridState', 'hidden');

    $("#btnAdd").hide('slow');
    $.ajax({
        url: '/ARRR/MachineryMasterLayout/' + param,
        type: 'POST',
        data: { User_Action: "E" },
        success: function (jsonData) {
            $('#dvLoadMachineryMaster').html('');
            $('#dvLoadMachineryMaster').html(jsonData);
            $('#dvLoadMachineryMaster').show('slow');
            //alert(jsonData);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteMachineryMaster(urlparameter) {
    if (confirm("Are you sure you want to delete Machinery Master details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/ARRR/delMaterialMasterDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    $("#btnCancel").trigger('click');
                    LoadMachineryMasterGrid();
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

function changeMachineryMasterstatus(urlparameter) {
    //alert(urlparameter);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/ARRR/changeMachineryMasterstatus/' + urlparameter,
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            if (data.success) {

                alert(data.message);
                $("#btnCancel").trigger('click');
                LoadMachineryMasterGrid();
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
