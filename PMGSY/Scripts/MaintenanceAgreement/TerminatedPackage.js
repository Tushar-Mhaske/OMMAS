
var stateCode = $('#stateCode').val(); 
var districtCode = $('#districtCode').val();
var blockCode = $('#ddlBlock option:selected').val();
var yearCode = $("#ddlYear option:selected").val();

$("#btnListDetails").click(function () {
    if ($("#searchExecution").valid()) {
        stateCode = $('#stateCode').val(); 
        districtCode = $('#districtCode').val(); 
        blockCode = $('#ddlBlock option:selected').val();
        yearCode = $("#ddlYear option:selected").val();

        LoadExecutionGrid();
    }
});

$("#idFilterDiv").click(function () {
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-n").toggleClass("ui-icon-circle-triangle-s");
    $("#searchExecution").toggle("slow");
});

function LoadExecutionGrid() {

    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/MaintenanceAgreement/GetTerminatedPackageList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { yearCode: yearCode, stateCode: stateCode, districtCode: districtCode, blockCode: blockCode },
        colNames: ['District', 'Sanctioned Year', 'Batch', 'OMMAS Package No.', 'EMARG Package No.', 'Road Name', 'Length', 'Funding Agency', 'Maintenance Amount', 'Add', 'View', 'Delete', 'Finalize'],
        colModel: [
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, sortable: true, align: "left", search: false },
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', width: 70, align: "center" },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, sortable: true, align: "center", search: false },
                            { name: 'EMARG_PACKAGE_NO', index: 'EMARG_PACKAGE_NO', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 250, sortable: true, align: "left", search: true },
                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 80, align: "center", search: true },
                            { name: 'MASTER_FUNDING_AGENCY', index: 'MASTER_FUNDING_AGENCY', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'MASTER_YEAR_AMOUNT', index: 'MASTER_YEAR_AMOUNT', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: true, formatter: FormatColumnView },
                            { name: 'View', index: 'View', height: 'auto', width: 80, align: "center", search: true },
                            { name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center", search: true },
                            { name: 'Finalize', index: 'Finalize', height: 'auto', width: 80, align: "center", search: true }
        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "MAST_DISTRICT_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Terminated Package Agreement List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbExecutionList #pagerExecution").css({ height: '40px' });           
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
        onHeaderClick: function () {
            $('#dvRoadDetails').hide('slow');
            $('#frmAddAgreementDetails').hide('slow');
            $('#ViewDataDiv').hide('slow');
        }
    });

}

function FormatColumnView(cellvalue, options, rowObject) {
    
    if (cellvalue == "N")
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Details already added'></span></td></tr></table></center>"
    else
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plus' title='Click here to add details' onClick ='AddConstructionDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
}

function AddConstructionDetails(urlparameter) {
    
        $.ajax({
            url: "/MaintenanceAgreement/AddContractorDetails/" + urlparameter,
            type: "GET",
            async: false,
            cache: false,

            success: function (data) {
              
                $("#searchExecution").hide('slow')
                $("#tbExecutionList").jqGrid('setGridState', 'hidden');
                $('#divAddQCR').show('slow');
                $('#divAddQCR').html(data);
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        }); 
}

function ViewContractorDetails(urlparameter) {
    
    $.ajax({
        url: "/MaintenanceAgreement/ViewContractorDetails/" + urlparameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {
          
            $("#searchExecution").hide('slow')
            $("#tbExecutionList").jqGrid('setGridState', 'hidden');
            $('#viewData').show('slow');
            $('#viewData').html(data);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function DeleteContractorDetails(urlparameter) {
  
    if (confirm("Do you want to delete the changes ?")) {
        $.ajax({
            url: "/MaintenanceAgreement/DeleteDetails/" + urlparameter,
            type: 'GET',
            dataType: 'json',
            success: function (response) {

                alert(response.message);

                $('#tbExecutionList').trigger('reloadGrid');
            },
            error: function () {
                $.unblockUI();
                alert("An Error occured, please try again.");
                return false;
            },
        });
    }
    else {
        $.unblockUI();
    }
}

function FinalizeDetails(urlparameter) {
    
    if (confirm("Do you want to finalize the changes ?")) {
        $.ajax({
            url: "/MaintenanceAgreement/FinalizeDetails/" + urlparameter,
            type: 'GET',
            dataType: 'json',
            success: function (response) {

                alert(response.message);

                $('#tbExecutionList').trigger('reloadGrid');
            },
            error: function () {
                $.unblockUI();
                alert("An Error occured, please try again.");
                return false;
            },
        });
    }
    else {
        $.unblockUI();
    }
}
