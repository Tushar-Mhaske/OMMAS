$(document).ready(function () {
    

    //$("info_dialog").css("text-align","center");
    
    $.validator.unobtrusive.parse($('#frmChangeDPIUStatus'));

    LoadDPIUList();

    $(".editable valid").datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "click",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {

        }
    }).focus(function () {
        $('.editable valid').datepicker('show');
    });

    $('.editable valid').attr('placeholder','Enter Date(dd/mm/yyyy)...');


});
function EditDetails(paramFileID) {

    //modified by abhishek kamble 27-8-2013 

    var allRowsOnCurrentPage = $('#tbDPIUDetailsList').jqGrid('getDataIDs');

    var myArray = allRowsOnCurrentPage.toString().split(',');

    for (var i = 0; i < myArray.length; i++) {

        if (myArray[i] == paramFileID) {
        
            //added by vikram start
            var colName = $("#tbDPIUDetailsList").jqGrid('getGridParam', 'colModel');
            var epayValue = jQuery("#tbDPIUDetailsList").getCell(paramFileID, "ADMIN_EPAY_ENABLE_DATE");

            if (epayValue != "") {
                jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EPAY_ENABLE_DATE', { editable: false });
            }
            else {
                jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EPAY_ENABLE_DATE', { editable: true });
            }

            var remittenceValue = jQuery("#tbDPIUDetailsList").getCell(paramFileID, "ADMIN_EREMIT_ENABLED_DATE");

            if (remittenceValue != "") {
                jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EREMIT_ENABLED_DATE', { editable: false });
            }
            else {
                jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EREMIT_ENABLED_DATE', { editable: true });
            }

            //var authorizationValue = jQuery("#tbDPIUDetailsList").getCell(paramFileID, "ADMIN_BA_ENABLE_DATE");
            //if (authorizationValue != "") {
            jQuery("#tbDPIUDetailsList").setColProp('ADMIN_BA_ENABLE_DATE', { editable: false });
            //}
            //else {
            //    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_BA_ENABLE_DATE', { editable: true });
            //}
            jQuery('#tbDPIUDetailsList').jqGrid('editRow', paramFileID, true, pickdates);
            $("#tbDPIUDetailsList").jqGrid('showCol', 'Action');
            //added by vikram end            
        }
        else {            
            CancelSaveDetails(myArray[i]);
        }
    }
    
  
}

function SaveDetails(paramFileID) {
    jQuery("#tbDPIUDetailsList").saveRow(paramFileID, checksave);    
}

function CancelSaveDetails(paramFileID) {
    
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EPAY_ENABLE_DATE', { editable: true });
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EREMIT_ENABLED_DATE', { editable: true });
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_BA_ENABLE_DATE', { editable: true });

    jQuery("#tbDPIUDetailsList").restoreRow(paramFileID);
}
function checksave(result) {

    if (result.responseText == "true") {
        alert('Details updated successfully.');
        jQuery("#tbDPIUDetailsList").trigger('reloadGrid');
        return true;
    }
    else if (result.responseText != "") {
        //alert(result.responseText.replace('"', "").replace('"', ""));
        alert(result.responseText);
        return false;
    }
}

function ValidateDate(value, colname) {

    if (colname == "Bank Authorization Date")
        {
        if (value == "")
        {   
            return [false, "Please enter Bank Authorization Date."];

        }
    }
  
    var regEx = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$/;
    
    if (!(regEx.test(value)))
    {
        return ["Invalid Date,Please enter Date in dd/mm/yyyy format."];
    }
    else {
        return [true, ""];
    }
}
function LoadDPIUList()
{
    jQuery("#tbDPIUDetailsList").jqGrid({
        url: '/Bank/DisplayDPIUDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['DPIU Name', 'Epay Enabled Date', 'Remittance Date', 'Bank Authorization Date', 'Change Authorization Status', 'Edit', 'Action'],
        colModel: [
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: 200, align: "left", search: false },
                            { name: 'ADMIN_EPAY_ENABLE_DATE', index: 'ADMIN_EPAY_ENABLE_DATE', height: 'auto', width: 150, align: "center", search: false, editable: true, sorttype:'date',editoptions: { maxlength: 100,size:'20' }, editrules: { custom: true, custom_func: ValidateDate } },
                            { name: 'ADMIN_EREMIT_ENABLED_DATE', index: 'ADMIN_EREMIT_ENABLED_DATE', height: 'auto', width: 150, align: "center", search: false, editable: true, sorttype:'date', editoptions: { maxlength: 100, size: '20' }, editrules: { custom: true, custom_func: ValidateDate } },
                            { name: 'ADMIN_BA_ENABLE_DATE', index: 'ADMIN_BA_ENABLE_DATE', height: 'auto', width: 150, align: "center", search: true, editable: true, sorttype: 'date', editoptions: { maxlength: 100, size: '20' }, editrules: { custom: true, custom_func: ValidateDate } },
                            { name: 'ADMIN_BANK_AUTH_ENABLED',index:'ADMIN_BANK_AUTH_ENABLED', width: 150, sortable: false, resize: false, align: "center", search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'Action', width: 80, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerDPIUList'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ADMIN_ND_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; DPIU List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth:true,
        editurl: "/Bank/UpdateDPIUDetails",
        loadComplete: function (data) {
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else
            {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


    jQuery("#tbDPIUDetailsList").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'ADMIN_BA_ENABLE_DATE', numberOfColumns: 2, titleText: 'Bank Authorization' },
        ]
    });

}
function pickdates(paramFileID) {
    
    $("#" + paramFileID + "_ADMIN_EPAY_ENABLE_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true
    });

    $("#" + paramFileID + "_ADMIN_EREMIT_ENABLED_DATE").datepicker({    
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true
    });

    $("#" + paramFileID + "_ADMIN_BA_ENABLE_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true
    });
}


function ChangeAuthorizationStatus(paramFileID)
{
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EPAY_ENABLE_DATE', { editable: false });
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_EREMIT_ENABLED_DATE', { editable: false });
    jQuery("#tbDPIUDetailsList").setColProp('ADMIN_BA_ENABLE_DATE', { editable: true });


    jQuery('#tbDPIUDetailsList').jqGrid('editRow', paramFileID, true, pickdates);
}

