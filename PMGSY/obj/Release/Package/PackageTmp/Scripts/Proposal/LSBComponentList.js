$(document).ready(function () {
    blockPage();
    LSBComponentGrid();
    unblockPage();



    //Load add new Component Form
    $("#dvComponentAdd").load('/LSBProposal/AddLSBComponentDetails/' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#dvComponentAdd'));
        unblockPage();
    });

    $('#btnAddComponent').click(function (evt) {
        evt.preventDefault();

        blockPage();
        $("#dvComponentAdd").load('/LSBProposal/AddLSBComponentDetails/' + $("#IMS_PR_ROAD_CODE").val(), function () {
            $.validator.unobtrusive.parse($('#dvComponentAdd'));
            unblockPage();
        });
        $('#dvComponentAdd').show('slow');
        $("#dvComponentAdd").css('height', 'auto');

        //Hide add button TD
        $("#tdBtnAddComponent").hide();
        $("#OPERATION").val("C");

    });//btnAddComponent ends here

});


//Component Grid Function
function LSBComponentGrid() {
    jQuery("#tbComponentList").jqGrid({
        url: '/LSBProposal/LSBComponentList',
        datatype: "json",
        mtype: "POST",
        postData: { roadId: $("#IMS_PR_ROAD_CODE").val(), value: Math.random() },
        colNames: ["", "Component Description", "Quantity", "Cost (In lakhs)", "Grade Concrete (In lakhs)", "Edit", "Delete"],
        colModel: [
                    { name: 'ComponentCode', index: 'ComponentCode', width: 10, sortable: false, align: "center", hidden: true },
                    { name: 'ComponentDesc', index: 'ComponentDesc', width: 180, sortable: false, align: "left" },
                    { name: 'Quantity', index: 'Quantity', width: 100, sortable: false, align: "center" },
                    { name: 'Cost', index: 'Cost', width: 100, sortable: false, align: "center" },
                    { name: 'GradeConcrete', index: 'GradeConcrete', width: 100, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Action', width: 60, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center" }
        ],
        pager: jQuery('#dvComponentListPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Component Details",
        height: 'auto',
        //width: 'auto',
        sortname: 'ComponentCode',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            //Hide Title bar
            $("#gview_tbComponentList > .ui-jqgrid-titlebar").hide();
        
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/Login";
            }
        }

    }); //end of grid

}//End of LSBComponentGrid



// Editing the Proposal
function EditCurrentLSBComponent(prRoadCode, componentCode) {

    jQuery('#tbComponentList').jqGrid('setSelection', componentCode);

    blockPage();
    $("#dvComponentAdd").load('/LSBProposal/EditLSBComponentDetails?roadCode=' + prRoadCode + "&componentCode=" + componentCode, function () {
        $.validator.unobtrusive.parse($('#dvComponentAdd'));
        unblockPage();
    });
    $('#dvComponentAdd').show('slow');
    $("#dvComponentAdd").css('height', 'auto');

    //Show add button TD
    $("#tdBtnAddComponent").show();
    $("#OPERATION").val("U");

}

//Delete LSB Component Details
function DeleteCurrentLSBComponent(prRoadCode, componentCode) {

    if (confirm("Are you sure to delete component details?")) {

        $.ajax({
            url: '/LSBProposal/DeleteLSBComponent?roadCode=' + prRoadCode + "&componentCode=" + componentCode,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success) {
                    alert("Component details deleted successfully.");
                    $("#tbComponentList").trigger('reloadGrid');
                    $("#divProposalForm").load("/LSBProposal/ShowLSBComponentList/" + $("#IMS_PR_ROAD_CODE").val());
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.errorMessage)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }
                }                
                unblockPage();
            }
        });
    } else {
        return;
    }
}