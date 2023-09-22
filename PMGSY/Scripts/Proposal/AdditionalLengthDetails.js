$(document).ready(function () {

    $("#dvProgressDetails").dialog({
        autoOpen: false,
        // height:550,
        width: '1050px',
        modal: true,
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        }

    });


    ShowPropAddLengthList();

    $('#rdbIncrease,#rdbDecrease').click(function () {
        $('#IMS_PERCENTAGE_CHANGE').trigger('blur');
    });

    $('#IMS_PERCENTAGE_CHANGE').blur(function () {

        var currentLength = $('#IMS_PAV_LENGTH').val();
        
        if ($(this).val() > 0)
        {
            var changedLength = 0;

            if ($('#rdbIncrease').is(':checked'))
            {
                changedLength = parseFloat(parseFloat($('#IMS_PAV_LENGTH').val()) + ((parseFloat(currentLength) * parseFloat($(this).val())) / 100)).toFixed(2);
            }
            else if ($('#rdbDecrease').is(':checked'))
            {
                changedLength = parseFloat(parseFloat($('#IMS_PAV_LENGTH').val()) - ((parseFloat(currentLength) * parseFloat($(this).val())) / 100)).toFixed(2);
                if (changedLength <= 0)
                {
                    changedLength = 0;
                }
            }

            $('#IMS_CHANGED_LENGTH').val(changedLength);

            return false;
        }

    });


    $('#btnSave').click(function () {

        if ($('#frmPropAddLength').valid()) {

            $('#IMS_CHANGED_LENGTH').attr('disabled',false);

            $.ajax({

                type: 'POST',
                url: '/Proposal/SaveAdditionalLengthDetails',
                data: $('#frmPropAddLength').serialize(),
                async: false,
                cache: false,
                success: function (response) {
                    if (response.success == true) {
                        alert("Length details saved successfully.");
                        $("#divAddPropAddLength").load('/Proposal/AddAdditionLength?id=' + $('#EncProposalCode').val(), function () {
                            unblockPage();
                        });
                        $('#divAddPropAddLength').show('slow');
                        $("#divAddPropAddLength").css('height', 'auto');
                        return false;
                    }
                    else if (response.success == false) {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                    }
                },
                error: function () { }

            });
        }
        else {
            return false;
        }

    });


    $('#btnUpdate').click(function () {

        if ($('#frmPropAddLength').valid()) {

            $('#IMS_CHANGED_LENGTH').attr('disabled', false);

            $.ajax({

                type: 'POST',
                url: '/Proposal/UpdateAdditionalLengthDetails',
                data: $('#frmPropAddLength').serialize(),
                async: false,
                cache: false,
                success: function (response) {
                    if (response.success == true) {
                        alert("Length details updated successfully.");
                        $("#divAddPropAddLength").load('/Proposal/AddAdditionLength?id=' + $('#EncProposalCode').val(), function () {
                            unblockPage();
                        });
                        $('#divAddPropAddLength').show('slow');
                        $("#divAddPropAddLength").css('height', 'auto');
                        return false;
                    }
                    else if (response.success == false) {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.errorMessage);
                    }
                },
                error: function () { }

            });
        }
        else {
            return false;
        }

    });


});
function ShowPropAddLengthList() {

    IMS_PR_ROAD_CODE = $('#IMS_PR_ROAD_CODE').val();

    jQuery("#tbPropAddLengthList").jqGrid({
        url: '/Proposal/GetAdditionalLengthList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Change Type', '% Change', 'Updated Length', 'Is MoRD Approved', 'MoRD Approved Date', 'Edit', 'Delete', 'Approve', 'Reject'], //'Sr.No.', 
        colModel: [
            //{ name: 'IMS_TRANSACTION_CODE', index: 'IMS_TRANSACTION_CODE', width: '100px', sortable: true, align: 'center' },
            { name: 'IMS_CHANGE_TYPE', index: 'IMS_CHANGE_TYPE', width: '100px', sortable: true, align: 'center' },
            { name: 'IMS_PERCENTAGE_CHANGE', index: 'IMS_PERCENTAGE_CHANGE', width: '120px', sortable: false, align: "center" },
            { name: 'IMS_CHANGED_LENGTH', index: 'IMS_CHANGED_LENGTH', width: '120px', sortable: true, align: "center" },
            { name: 'IMS_IS_MRD_APPROVED', index: 'IMS_IS_MRD_APPROVED', width: '100px', sortable: false, align: "center" },
            { name: 'IMS_MRD_APPROVED_DATE', index: 'IMS_MRD_APPROVED_DATE', width: '200px', sortable: true, align: "center" },
            { name: 'Edit', width: '50px', sortable: false, resize: false, align: "center", hidden: $('#RoleID').val() == 25 ? true : false },
            { name: 'Delete', width: '50px', sortable: false, resize: false, align: "center", hidden: $('#RoleID').val() == 25 ? true : false },
            { name: 'Approve', width: '50px', sortable: false, resize: false, align: "center", hidden: ($('#RoleID').val() == 2 ? true : ($('#RoleID').val() == 45 ? true : false)) },
            { name: 'Reject', width: '50px', sortable: false, resize: false, align: "center", hidden: ($('#RoleID').val() == 2 ? true : ($('#RoleID').val() == 45 ? true : false)) }
        ],
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        pager: $("#dvPropAddLengthListPager"),
        sortorder: "asc",
        sortname: "IMS_SAMPLE_ID",
        rowNum: 5,
        pginput: true,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: ' Progress Additional Length Details',
        height: 'auto',
        width: '100%',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {

        },
        loaderror: function (xhr, status, error) {

            if (xhr.responseText == 'session expired') {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else { }
        },
    });

}

function formatColumnEdit(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Test Result Details' onClick='EditPropAddCostDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";
    }
}

function formatColumDelete(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style=' border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Test Result Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePropAddCostDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function EditLengthDetails(urlParameter)
{
    $("#divAddPropAddLength").load('/Proposal/EditAdditionalLength/' + urlParameter, function () {
        unblockPage();
    });
}
function DeleteLengthDetails(urlParameter)
{
    $.ajax({

        type: 'POST',
        url: '/Proposal/DeleteAdditionalLengthDetails/' + urlParameter,
        async: false,
        cache: false,
        success: function (data) {
            if (data.success == true) {
                alert('Progress Length details deleted successfully.');
                jQuery("#tbPropAddLengthList").trigger('reloadGrid');
            }
            else {
                alert("Error occurred while processing your request.");
            }
        },
        error: function () { }


    });
}

function ApproveLengthDetails(urlParameter)
{
    $.ajax({

        type: 'POST',
        url: '/Proposal/ApproveRejectAdditionalLengthDetails/' + urlParameter,
        async: false,
        cache: false,
        data: { ApproveReject: "Y" },
        success: function (data) {
            if (data.success == true) {
                alert('Progress Length details approved successfully.');
                jQuery("#tbPropAddLengthList").trigger('reloadGrid');
            }
            else {
                alert("Error occurred while processing your request.");
            }
        },
        error: function () { }


    });
}

function DiscardLengthDetails(urlParameter) {
    $.ajax({

        type: 'POST',
        url: '/Proposal/ApproveRejectAdditionalLengthDetails/' + urlParameter,
        async: false,
        cache: false,
        data: { ApproveReject : "N" },
        success: function (data) {
            if (data.success == true) {
                alert('Progress Length details rejected successfully.');
                jQuery("#tbPropAddLengthList").trigger('reloadGrid');
            }
            else {
                alert("Error occurred while processing your request.");
            }
        },
        error: function () { }


    });
}
function ViewPhysicalProgress()
{
    jQuery("#tbPhysicalRoadList").jqGrid('GridUnload');
    LoadPhysicalRoadDetails($('#IMS_PR_ROAD_CODE').val());
    $("#dvProgressDetails").dialog('open');
}
function LoadPhysicalRoadDetails(IMS_ROAD_CODE) {

    jQuery("#tbPhysicalRoadList").jqGrid({
        url: '/Execution/GetRoadPhysicalProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status', 'Preparatory Work (Length in Km.)', 'Subgrade Stage (Length in Km.)', 'Subbase (Length in Km.)', 'Base Course (Length in Km.)', 'Surface Course (Length in Km.)', 'Road Signs Stones (in Nos.)', 'CDWorks (in Nos.)', 'LS Bridges (in Nos.)', 'Miscellaneous (Length in Km.)', 'Completed (Length in Km.)', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'EXEC_PREPARATORY_WORK', index: 'EXEC_PREPARATORY_WORK', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_EARTHWORK_SUBGRADE', index: 'EXEC_EARTHWORK_SUBGRADE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SUBBASE_PREPRATION', index: 'EXEC_SUBBASE_PREPRATION', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BASE_COURSE', index: 'EXEC_BASE_COURSE', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_SURFACE_COURSE', index: 'EXEC_SURFACE_COURSE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SIGNS_STONES', index: 'EXEC_SIGNS_STONES', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_CD_WORKS', index: 'EXEC_CD_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_LSB_WORKS', index: 'EXEC_LSB_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_MISCELANEOUS', index: 'EXEC_MISCELANEOUS', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'a', width: 40, align: "center", search: false, sortable: false, hidden: true },
                            { name: 'b', width: 40, align: "center", search: false, sortable: false, hidden: true },

        ],
        pager: jQuery('#pagerPhysicalRoadList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Physical Road Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            //$("#gview_tbPhysicalRoadList > .ui-jqgrid-titlebar").hide();
            //$("#tbPhysicalRoadList #pagerPhysicalRoadList").css({ height: '40px' });
            //$("#pagerPhysicalRoadList_left").html("<input type='button' style='margin-left:27px' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + IMS_ROAD_CODE + ");return false;' value='Add Road Progress'/>")

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}