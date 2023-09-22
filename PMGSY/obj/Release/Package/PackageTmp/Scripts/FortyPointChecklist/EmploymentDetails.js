
$(document).ready(function(){

    //myAddOptions = {

    //    onclickSubmit: function (rp_ge, rowid) {
    //        alert("in addition" + rowid);
    //    },
    //    edit: false,
    //    del: false,
    //    search: false,
    //    refresh: false,
    //    processing: true
    //}
    
    grid = jQuery("#tableEmploymentDetails").jqGrid({
        url: '/FortyPointChecklist/GetEmploymentInformation',
        datatype: "json",
        mtype: "POST",
        //postData: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
        colNames: ['Education Qualification ', 'Number of Holders', 'Edit', 'Delete'],
        colModel: [        
                           { name: 'EducationQualification', index: 'EducationQualification', height: 'auto', width: 150, align: 'center', sortable: false, editable: false },
                           { name: 'NoOfHolders', index: 'NoOfHolders', width: 100, align: 'center', sortable: false, editable: true },
                           { name: 'Edit', width: '10px', sortable: false, resize: false, align: 'center', formatter: formatColumnEdit },
                           { name: 'Delete', width: '10px', sortable: false, resize: false, align: 'center', formatter: formatColumnDelete }
                           
                           //{
                           //    name: 'act', index: 'act', width: 55, align: 'center', sortable: false, formatter: 'actions',
                           //    formatoptions:{
                           //        keys: true,              // we want use [Enter] key to save the row and [Esc] to cancel editing.
                           //        onEdit:function(rowid) {
                           //            alert("in onEdit: rowid="+rowid+"\nWe don't need return anything");
                           //        },
                           //        onSuccess:function(jqXHR) {
                                      
                           //            alert("in onSuccess used only for remote editing:"+
                           //                  "\nresponseText="+jqXHR.responseText+
                           //                  "\n\nWe can verify the server response and return false in case of"+
                           //                  " error response. return true confirm that the response is successful");
                                       
                           //            return true;
                           //        },
                           //        onError:function(rowid, jqXHR, textStatus) {
                                      
                           //            alert("in onError used only for remote editing:"+
                           //                  "\nresponseText="+jqXHR.responseText+
                           //                  "\nstatus="+jqXHR.status+
                           //                  "\nstatusText"+jqXHR.statusText+
                           //                  "\n\nWe don't need return anything");
                           //        },
                           //        afterSave:function(rowid) {
                           //            alert("in afterSave (Submit): rowid="+rowid+"\nWe don't need return anything");
                           //        },
                           //        afterRestore:function(rowid) {
                           //            alert("in afterRestore (Cancel): rowid="+rowid+"\nWe don't need return anything");
                           //        },
                           //        //delbutton: false,
                           //        //delOptions: myDelOptions
                           //    }
                           //}
        ],
        pager: jQuery('#navGridEmploymentDetails'),
        rowNum: 10,
        gridview: true,
        rowList: [5, 10,15,20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Employment Information Details",
        height: 'auto',
        width: 1135,
        
        rownumbers: true,
        sortname: 'EmploymentId',
        sortorder: "asc",
        hidegrid: true,        
        loadComplete: function () {                   
            //$("#tableEmploymentDetails").jqGrid('setGridState', "hidden");
            $('#tableEmploymentDetails_rn').html("Sr.<br/>No");                
            $("#tableEmploymentDetails #navGridEmploymentDetails").css({ height: '31px' });
            $("#navGridEmploymentDetails_left").html("<input type='button' style='margin-left:27px' id='btnShowEmploymentDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddEmploymentDetails();return false;' value='Add Employment Information Details'/>");
        }
    });
       // .jqGrid('navGrid', '#navGridEmploymentDetails', myAddOptions, {}, {}, { multipleSearch: false, overlay: false });

    //function FormatColumn(cellvalue, options, rowObject) {
    //    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Education Qualification' onClick ='editEducationQualification(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Education Qualification' onClick = deleteEducationQualification(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";
    //}

    //added by abhishek kamble 12-Nov-2013
    //$('#btnCreateNew').click(function () {
    //    AddEmploymentDetails
    //});

});//end of document ready

function formatColumnEdit(cellvalue, options, rowObject) {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Employment Information Details' onClick='EditEmploymentInformationDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";
}

function formatColumnDelete(cellvalue, options, rowObject) {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Employment Information Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteEmploymentInformationDetails(\"" + cellvalue.toString() + "\");'></span></center>";    
}


function EditEmploymentInformationDetails(urlparam) {    
    $.ajax({
        url: '/FortyPointChecklist/EditEmploymentInformationDetails/' + urlparam,
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

    $('#btnCreateNew').hide('slow');
}

function DeleteEmploymentInformationDetails(urlParam) {
    
    if (confirm("Are you sure you want to delete employment information details ? ")) {
        $.ajax({

            url: '/FortyPointChecklist/DeleteEmploymentInformationDetails/' + urlParam,
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
                    $("#btnResetEmploymentDetails").trigger("click");
                    $('#tableEmploymentDetails').trigger('reloadGrid');                    
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

function AddEmploymentDetails()
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#dvFrmAddDetails").load("/FortyPointChecklist/AddEmploymentInformation", function () {
        $("#dvErrorMessage").hide("slow");

        $('#dvFrmAddDetails').show();
        $("#tblFortyPointCheckList").jqGrid('setGridState', "hidden");
        $("#tableListConstrcuctionEquipmentDetails").jqGrid("setGridState", "hidden");
        $("#tableListLabEquipmentDetails").jqGrid("setGridState", "hidden");
        $("#tableListTenderCostDetails").jqGrid("setGridState", "hidden");

        $.unblockUI();
    });
    //$('#btnCreateNew').hide('slow');
}
