$(document).ready(function () {
    
    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })
    var RoadCode = $("#ProposalCode").val();
    var ContractCode = $("#ContractCode").val();
    $("#divAddTechnologyDetails").load('/MaintenanceAgreement/AddTechnologyDetails?RoadCode='+RoadCode+'&ContractCode='+ ContractCode , function (response) {
 
        //alert("ok success");
          $.validator.unobtrusive.parse($('#frmAddTechnology'));
        unblockPage();
    });

    LoadTechnologyList(RoadCode, ContractCode);

});
function LoadTechnologyList(RoadCode, ContractCode) {

    jQuery("#tblistTechnologyDetails").jqGrid({
        url: '/MaintenanceAgreement/GetTechnologyDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { RoadCode: RoadCode, ContractCode: ContractCode },
        colNames: ['Segment No.', 'Start Chainage', 'End Chainage', 'Technical Cost', 'Layer Cost', 'Layer', 'Technology', 'Technology Type', 'Edit', 'Delete'],
        colModel: [
                            { name: 'IMS_SEGMENT_NO', index: 'IMS_SEGMENT_NO', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_START_CHAINAGE', index: 'IMS_START_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_END_CHAINAGE', index: 'IMS_END_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_TECH_COST', index: 'IMS_TECH_COST', height: 'auto', width: 150, align: "right", search: false },
                            { name: 'IMS_LAYER_COST', index: 'IMS_LAYER_COST', height: 'auto', width: 150, align: "right", search: false },
                            { name: 'MAST_HEAD_DESC', index: 'MAST_HEAD_DESC', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_TECH_NAME', index: 'MAST_TECH_NAME', height: 'auto', width: 100, align: "left", search: true },
                            { name: 'MAST_TECH_TYPE', index: 'MAST_TECH_TYPE', height: 'auto', width: 20, align: "left", search: true},
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false,  align: "center", search: false },

        ],
        pager: jQuery('#dvpagerTechnology').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "IMS_SEGMENT_NO",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Execution Details List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        grouping: true,
        groupingView: {
            groupField: ['MAST_TECH_TYPE'],
            groupDataSorted: true,
            groupColumnShow:false
        },
        loadComplete: function (data) {
           // alert("Success");
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
function EditTechnologyDetails(urlparameter)
{
   // alert(urlparameter);
    $("#divAddTechnologyDetails").load('/MaintenanceAgreement/GetTechnologyDetailsForEdit/' + urlparameter, function (response) {
       // $.validator.unobtrusive.parse($('#divProposalForm'));
        unblockPage();
    });
}
function DeleteTechnologyDetails(urlparameter) {
 
    var RoadCode = $("#ProposalCode").val();
    var ContractCode = $("#ContractCode").val();
    //$("#divAddTechnologyDetails").load('/MaintenanceAgreement/AddTechnologyDetails?RoadCode='+RoadCode+'&ContractCode='+ ContractCode , function (response) {
 
    if (confirm("Are you sure you want to delete technology details?")) {
        $.ajax({
            type: 'POST',
            url: '/MaintenanceAgreement/DeleteTechnologyDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Technology details deleted successfully");
                    $("#tblistTechnologyDetails").trigger('reloadGrid');
                    $("#divAddTechnologyDetails").load('/MaintenanceAgreement/AddTechnologyDetails?RoadCode='+RoadCode+'&ContractCode='+ ContractCode , function (response) {
                       // $.validator.unobtrusive.parse($('#divProposalForm'));
                        unblockPage();
                    });
                }
                else if (data.success == false) {
                    alert("Technology details is in use and can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

