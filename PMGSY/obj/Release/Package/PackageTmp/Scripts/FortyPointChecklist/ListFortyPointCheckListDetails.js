
$(document).ready(function(){

    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    grid = jQuery("#tblFortyPointCheckList").jqGrid({
        url: '/FortyPointChecklist/ListFortyPointChecklistDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Check List Point ID','Issues ', 'Reply Given','Edit','Delete'],
        colModel: [        

                           { name: 'MAST_CHECKLIST_POINTID', index: 'MAST_CHECKLIST_POINTID', height: 'auto', width: 15, align: 'center', sortable: false, editable: false },
                           { name: 'MAST_CHECKLIST_ISSUES', index: 'MAST_CHECKLIST_ISSUES', height: 'auto', width: 150, align: 'left', sortable: false, editable: false },
                           { name: 'MAST_ACTION_TAKEN', index: 'MAST_ACTION_TAKEN', width: 100, align: 'left', sortable: false, editable: true },
                           {
                               name: 'act', index: 'act', width: 15, align: 'center', sortable: false, formatter: 'actions',
                               formatoptions:{
                                   keys: true,              // we want use [Enter] key to save the row and [Esc] to cancel editing.
                                   //editformbutton: true,
                                   editbutton: true,
                                   delbutton: false,
                                   onEdit:function(rowid) {
                                       $("#dvErrorMessage").hide("slow");
                                       
                                       if (rowid == 11 || rowid == 17 || rowid == 18) {
                                           jQuery('#tblFortyPointCheckList').restoreRow(rowid);
                                           $("#tblFortyPointCheckList").trigger("reloadGrid");
                                           $("#tblFortyPointCheckList").jqGrid("setGridState", "hidden");

                                           if (rowid == 11)//tender Cost Details
                                           {
                                               $("#accordion div").html("");
                                               $("#accordion h3").html(
                                                       "<a href='#' style= 'font-size:.9em;' >Tender Cost Details</a>" +
                                                       '<a href="#" style="float: right;">' +
                                                       '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
                                                       );

                                               $('#accordion').show('fold', function () {
                                                   blockPage();
                                                   $("#divViewCheckListDetails").load('/FortyPointChecklist/TenderCostInformationDetails/', function () {
                                                       $('#divViewCheckListDetails').show('slow');
                                                       $("#divViewCheckListDetails").css('height', 'auto');
                                                       unblockPage();
                                                   });
                                               });
                                           } else if (rowid == 18)//employment Details
                                           {
                                               $("#accordion div").html("");
                                               $("#accordion h3").html(
                                                       "<a href='#' style= 'font-size:.9em;' >Employment Information Details</a>" +
                                                       '<a href="#" style="float: right;">' +
                                                       '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
                                                       );

                                               $('#accordion').show('fold', function () {
                                                   blockPage();
                                                   $("#divViewCheckListDetails").load('/FortyPointChecklist/EmploymentInformationDetails/', function () {
                                                       $('#divViewCheckListDetails').show('slow');
                                                       $("#divViewCheckListDetails").css('height', 'auto');
                                                       unblockPage();
                                                   });
                                               });
                                           }
                                           else if (rowid == 17)//Equipment Details
                                           {
                                               $("#accordion div").html("");
                                               $("#accordion h3").html(
                                                       "<a href='#' style= 'font-size:.9em;' >Equipment Details</a>" +
                                                       '<a href="#" style="float: right;">' +
                                                       '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
                                                       );

                                               $('#accordion').show('fold', function () {
                                                   blockPage();
                                                   $("#divViewCheckListDetails").load('/FortyPointChecklist/TenderEquipmentDetails/', function () {
                                                       $('#divViewCheckListDetails').show('slow');
                                                       $("#divViewCheckListDetails").css('height', 'auto');
                                                       unblockPage();
                                                   });
                                               });
                                           }

                                       }
                                   },
                                   onSuccess: function (response) {
                                       
                                       $("#dvErrorMessage").show("slow");
                                       $("#message").html(response.responseText);

                                       jQuery("#tblFortyPointCheckList").trigger('reloadGrid');
                                       return true;
                                   },
                                   onError:function(rowid, jqXHR, textStatus) {
                                       $("#message").html("An Error Occured while proccessing your request.");
                                       $("#dvErrorMessage").show("slow");
                                       
                                   },
                                   afterSave:function(rowid) {
                                   },
                                   afterRestore: function (rowid) {
                                       $("#dvErrorMessage").hide("slow");
                                   },
                               }
                           }, { name: 'Delete', width: '10px', sortable: false, resize: false, align: 'center', formatter: formatColumnDelete },
        ],
        rowNum: 40,
        gridview: true,
        rowList: [5, 10,15,20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "40 Point Check List Details",        
        height: 'auto',
        width: 1170,
        rownumbers: true,
        sortname: 'MAST_CHECKLIST_ISSUES',
        editurl: '/FortyPointChecklist/AddEditFortyPointChecklistDetails',
        sortorder: "asc",
        loadComplete: function () {
            $('#tblFortyPointCheckList_rn').html("Sr.<br/>No");
        },
    });

});//end of document ready

function formatColumnDelete(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        return "<center><span style=' border-color:white;cursor:pointer;''>-</span></center>";
    } else {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function DeleteDetails(urlParam) {
    
    if (confirm("Are you sure you want to delete details ? ")) {
        $.ajax({

            url: '/FortyPointChecklist/DeleteFortyPointChecklistDetails/' + urlParam,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert("Request can not be processed at this time, please try after some time...");
                $("#dvErrorMessage").hide("slow");
                return false;
            },
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);                    
                    $('#tblFortyPointCheckList').trigger('reloadGrid');
                    $("#dvErrorMessage").hide("slow");
                    $.unblockUI();
                }
                else {
                    $("#dvErrorMessage").hide("slow");
                    alert(response.message);
                }
                $.unblockUI();
            }
        });//end of delete ajax call
    }
}

function CloseDetails() {
    $('#accordion').hide('slow');
    $('#divViewCheckListDetails').hide('slow');
    $("#tblFortyPointCheckList").jqGrid('setGridState', 'visible');
}