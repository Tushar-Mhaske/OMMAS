
$(document).ready(function(){

    LoadTenderCostGrid();
});//end of document ready

function formatColumnEdit(cellvalue, options, rowObject) {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Details' onClick='EditTenderCostDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";    
}

function formatColumnDelete(cellvalue, options, rowObject) {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteTenderCostDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}


function EditTenderCostDetails(urlparam) {
    $.ajax({
        url: '/FortyPointChecklist/EditTenderCostInformationDetails/' + urlparam,
        Type: 'GET',
        catche: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        },
        error: function (xhr, status, error) {        
            $.unblockUI();
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $('#dvFrmAddDetails').html('');
            $("#dvFrmAddDetails").html(response);
            $("#dvFrmAddDetails").show();
            $.unblockUI();
        }
    });

    //$('#btnCreateNew').hide('slow');
}

function DeleteTenderCostDetails(urlParam) {
    
    if (confirm("Are you sure you want to delete tender cost details ? ")) {
        $.ajax({

            url: '/FortyPointChecklist/DeleteTenderCostInformationDetails/' + urlParam,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert("Request can not be processed at this time, please try after some time...");
                return false;
            },
            beforeSend: function () {                
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);                    
                    $("#btnResetTenderCostDetails").trigger("click");
                    $('#tblListGridDetails').trigger('reloadGrid');
                    $.unblockUI();
                }
                else {
                    alert(response.message);
                }
                $.unblockUI();
            }
        });//end of delete ajax call
    }
}


function showTenderCostDetailsForm()
{
    if (($("#dvFrmAddDetails").is(":visible"))) {
        
        return false;

    } else {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $("#dvFrmAddDetails").load("/FortyPointChecklist/AddEditTenderCostInformationDetails", function () {
            $("#dvErrorMessage").hide("slow");
            $('#dvFrmAddDetails').show();
            $("#tblFortyPointCheckList").jqGrid('setGridState', "hidden");
            $.unblockUI();
        });
    }
}


function LoadTenderCostGrid()
{
    
    grid = jQuery("#tblListGridDetails").jqGrid({
        url: '/FortyPointChecklist/GetTenderCostInformation',
        datatype: "json",
        mtype: "POST",
        colNames: ['Works Costing From ( In Rs. )', 'Works Costing To ( In Rs. )', 'Tender Price ( In Rs.)', 'Edit', 'Delete'],
        colModel: [
                           { name: 'TEND_WORKS_COSTING_FROM', index: 'TEND_WORKS_COSTING_FROM', height: 'auto', width: 100, align: 'right', sortable: true, editable: false },
                           { name: 'TEND_WORKS_COSTING_TO', index: 'TEND_WORKS_COSTING_TO', width: 100, align: 'right', sortable: true, editable: true },
                           { name: 'TEND_SALE_PRICE', index: 'TEND_SALE_PRICE', width: 100, align: 'right', sortable: true, editable: true },
                           { name: 'Edit', width: '13px', sortable: false, resize: false, align: 'center', formatter: formatColumnEdit },
                           { name: 'Delete', width: '13px', sortable: false, resize: false, align: 'center', formatter: formatColumnDelete }
        ],
        pager: jQuery('#dvPagerGridDetails'),
        rowNum: 10,
        gridview: true,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Tender Cost Details",
        height: 'auto',
        width: 1135,
        rownumbers: true,
        sortname: 'TEND_WORKS_COSTING_FROM',
        sortorder: "asc",
        hidegrid: true,
        loadComplete: function () {
            $('#tblListGridDetails_rn').html("Sr.<br/>No");
            $("#tblListGridDetails #dvPagerGridDetails").css({ height: '31px' });
            $("#dvPagerGridDetails_left").html("<input type='button' style='margin-left:27px' id='btnShowTenderCostDetailsForm' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'showTenderCostDetailsForm();return false;' value='Add Tender Cost Details'/>");
        }
    });


}