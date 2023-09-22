
$(document).ready(function(){

    loadEquipmentGrid();
    
});//end of document ready

function formatColumnEdit(cellvalue, options, rowObject) {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Details' onClick='EditTenderEquipmentDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";    
}

function formatColumnDelete(cellvalue, options, rowObject) {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteTenderEquipmentDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}


function EditTenderEquipmentDetails(urlparam) {
    $.ajax({
        url: '/FortyPointChecklist/EditTenderEquipmentDetails/' + urlparam,
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

function DeleteTenderEquipmentDetails(urlParam) {
    
    if (confirm("Are you sure you want to delete equipment details ? ")) {
        $.ajax({

            url: '/FortyPointChecklist/DeleteTenderEquipmentDetails/' + urlParam,
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
                    $("#btnResetTenderEquipmentDetails").trigger("click");
                    $('#tableListEquipmentDetails').trigger('reloadGrid');
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

function addEquipmentDetails() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#dvFrmAddDetails").load("/FortyPointChecklist/AddEditTenderEquipmentDetails", function () {
        $("#dvErrorMessage").hide("slow");
        $('#dvFrmAddDetails').show();        
        $.unblockUI();
    });
}

function loadEquipmentGrid()
{
    grid = jQuery("#tableListEquipmentDetails").jqGrid({
        url: '/FortyPointChecklist/ListTenderEquipmentDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Equipment','Type Of Equipment', 'Number Of Equipment', 'Edit', 'Delete'],
        colModel: [
                          { name: 'TEND_EQUIPMENT_FLAG', index: 'TEND_EQUIPMENT_FLAG', height: 'auto', width: 100, align: 'center', sortable: true, editable: false },
                           { name: 'TEND_EQUIPMENT_TYPE', index: 'TEND_EQUIPMENT_TYPE', height: 'auto', width: 200, align: 'center', sortable: true, editable: false },
                           { name: 'TEND_EQUIPMENT_NUMBERS', index: 'TEND_EQUIPMENT_NUMBERS', width: 50, align: 'center', sortable: true, editable: true },
                           { name: 'Edit', width: '13px', sortable: false, resize: false, align: 'center', formatter: formatColumnEdit },
                           { name: 'Delete', width: '13px', sortable: false, resize: false, align: 'center', formatter: formatColumnDelete }
        ],
        postData: { equipmentFlag: "L" },
        pager: jQuery('#dvPagerEquipmentDetails'),
        rowNum: 10,
        gridview: true,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Equipment Details",
        height: 'auto',
        width: 1000,
        //shrinkToFit: true,
        rownumbers: true,
        sortname: 'TEND_EQUIPMENT_FLAG',
        sortorder: "asc",
        
        //grouping: true,
        //groupingView: {
        //    groupField: ["TEND_EQUIPMENT_FLAG"],
        //    groupColumnShow:false,
        //    groupText:["<h4>{0}</h4>"]
        //},
        hidegrid: true,
        loadComplete: function () {
            $('#tableListEquipmentDetails_rn').html("Sr.<br/>No");
            $("#tableListEquipmentDetails #dvPagerEquipmentDetails").css({ height: '31px' });
            $("#dvPagerEquipmentDetails_left").html("<input type='button' style='margin-left:27px' id='btnShowEquipmentDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'addEquipmentDetails();return false;' value='Add Equipment Details'/>");
        }
    });
}