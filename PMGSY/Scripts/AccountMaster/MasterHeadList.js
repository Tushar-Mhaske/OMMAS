

$(document).ready(function () {
       
    //Cancel
    $("#btnCreateNew").click(function () {

        $("#btnCreateNew").hide('');

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


            $.ajax({
                type: 'GET',
                url: '/AccountMaster/AddEditMasterHeadDetails/',
                //data: $("#AccountMasterHeadForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $("#dvMasterHeadAddEdit").html(data);
                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert("Request can not be processed at this time.");
                }
            })
    });
    


  

    //List
    LoadHeadDetailsList();
});




function LoadHeadDetailsList() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbHeadDetailsList").jqGrid({
        url: '/AccountMaster/MasterHeadList',
        datatype: "json",
        mtype: "POST",
        //postData: { stateCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        colNames: ['Head Code', 'Head Name', 'Fund Type', 'Credit / Debit', 'Level', 'Is Operational', 'Head Category', 'Edit','Delete'],
        colModel: [
                            { name: 'HEAD_CODE', index: 'HEAD_CODE', height: 'auto', width: 80, align: "center", search: false,sortable:true },
                            { name: 'HEAD_NAME', index: 'HEAD_NAME', height: 'auto', width: 540, align: "left", search: false, sortable: true },
                            { name: 'FUND_TYPE', index: 'FUND_TYPE', height: 'auto', width: 90, align: "center", search: true, sortable: false },
                            { name: 'CREDIT_DEBIT', index: 'CREDIT_DEBIT', height: 'auto', width: 70, align: "center", search: false, sortable: true },
                            { name: 'OP_LVL_ID', index: 'OP_LVL_ID', height: 'auto', width: 70, align: "center", search: false, sortable: true },
                            { name: 'IS_OPERATIONAL', index: 'IS_OPERATIONAL', height: 'auto', width: 70, align: "center", search: true },
                            { name: 'HEAD_CATEGORY_ID', index: 'HEAD_CATEGORY_ID', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'edit', width: 50, sortable: false, resize: false, formatter: FormatColumnEdit, align: "center", sortable: false },
                            { name: 'delete', width: 50, sortable: false, resize: false, formatter: FormatColumnDelete, align: "center", sortable: false }
        ],
        pager: jQuery('#pagerHead'),
        rowNum: 0,
        //rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "HEAD_CODE",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Head Details",
        height: 'auto',
        //width: "100%",
        //shrinkToFit:true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            $('#tbHeadDetailsList_rn').html('Sr.<br/>No.');
            $("#tbHeadDetailsList").parents('div.ui-jqgrid-bdiv').css("max-height", "420px");
            $("#pagerHead_center").html('');
            //jQuery("#tbScheduleCurrentAssetsList").jqGrid('setCell', '(C) Other Items', 'Particulars', "", { 'font-size': '13px', 'font-weight': 'bold' });
            $.unblockUI();
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


function FormatColumnEdit(cellvalue, options, rowObject)
{
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Head Details' onClick ='EditDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumnDelete(cellvalue, options, rowObject)
{
    if (cellvalue == "") {
        return "-";
    } else {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Head Details' onClick ='DeleteDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
}


function EditDetails(urlParam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/AccountMaster/GetMasterHeadDetails/'+urlParam,        
        async: false,
        cache: false,
        success: function (data) {
            $("#dvMasterHeadAddEdit").html(data);
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
            alert("Request can not be processed at this time.");
        }
    })
}


function DeleteDetails(urlParam) {

    if (confirm("Are you sure you want to delete Head Details")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/AccountMaster/DeleteMasterHeadDetails/' + urlParam,            
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();
                if (data.success) {
                    alert(data.message);
                    $("#tbHeadDetailsList").trigger('reloadGrid');
                    //$("#btnCancel").trigger('click');
                    ResetHeadForm();
                }
                else {
                    //$("#divError").show();
                    //$("#errorSpan").html(data.message);

                    alert(data.message);
                }
            },
            error: function () {
                $.unblockUI();
                alert("Request can not be processed at this time.");
            }
        })
    }
}


