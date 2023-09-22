$(document).ready(function () {

    ListAuthorizedSignatoryDetails();

    //$("#btnGenerateKeyDetails").click(function () {

    //    $.ajax({
    //        url: '/Bank/GenerateKey/',
    //        async: false,
    //        catche:false,
    //        method: 'POST',
    //        success: function (response) {
    //            if (response.success == true)
    //            {   
    //                $("#dvKeyLabel").show();
    //                $("#dvGeneratedKey").html(response.key);
    //                $("#hdnGeneratedKey").val(response.key);
    //                $("#dvGeneratedKey").show();
    //            }
    //            else if (response.success == false)
    //            {
    //                alert(response.message);
    //                return false;
    //            }
    //        },
    //        error: function (xhr, status, code) {
    //            alert("An error occured while processing your request.");
    //            return false;
    //        }
    //    });

    //}); 

    $("#btnSaveKeyDetails").click(function () {
        $.ajax({
            url: '/Bank/SaveAuthSigKey/',
            async: false,
            catche: false,
            method: 'POST',
            data: $("#frmGenerateKey").serialize(),
            success: function (response) {
                if (response.success == true) {
                    alert(response.message);
                    $("#hdnGeneratedKey").val('');
                    $("#dvDialogGenerateKey").dialog("close");
                }
                else if (response.success == false) {
                    alert(response.message);
                    return false;
                }
            },
            error: function (xhr, status, code) {
                alert("An error occured while processing your request.");
                return false;
            }
        });
    });
});
function ListAuthorizedSignatoryDetails()
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tblstAuthSignatory").jqGrid({

        url: '/Bank/ListAuthorizedSignatories/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 0,
        rownumbers: true,
        //autowidth: true,
        pginput: false,
        pgbuttons: false,
        loadComplete: function () {
            $.unblockUI();
            var recordCount = jQuery("#tblstAuthSignatory").jqGrid('getGridParam', 'reccount');
            if (recordCount > 25) {
                $("#tblstAuthSignatory").jqGrid('setGridHeight', '500');
            } else {
                $("#tblstAuthSignatory").jqGrid('setGridHeight', 'auto');
            }
            $('#tblstAuthSignatory_rn').html('Sr.<br/>No.');
        },
        colNames: ['DPIU/SRRDA Name', 'Authorized Signatory Name', 'Start Date', 'Mobile', 'Email', 'Is Authorization Key Present', 'Generate Key'],
        colModel: [
            {
                name: 'Agency_name',
                index: 'Agency_name',
                width: 260,
                align: "left"
            },
            {
                name: 'auth_sig_name',
                index: 'auth_sig_name',
                width: 260,
                align: "left",
                sortable: false
            },
        
            {
                name: 'Start_Date',
                index: 'Start_Date',
                width: 140,
                align: "Center",
                sortable: false
            }, 
            {
                name: 'mobile_no',
                index: 'mobile_no',
                width: 150,
                align: "Center",
                sortable: false
            },
            {
                name: 'Email',
                index: 'Email',
                width: 150,
                align: "left",
                sortable: false
            },
                {
                    name: 'IsAuthKeyPresent',
                    index: 'IsAuthKeyPresent',
                    width: 80,
                    align: "Center",
                    sortable: false,
                }, {
                    name: 'Add_Edit',
                    index: 'Add_Edit',
                    width: 80,
                    align: "Center",
                    sortable: false,
                }


        ],
        pager: "#dvpgAuthSignatory",
        viewrecords: true,

        sortname: 'Agency_name',
        sortorder: "asc",

        caption: "Authorized Signatory Details",
        hidegrid: false
    });

}
function GenerateAuthCode(param)
{   
    $(function () {
        $("#dvKeyLabel").hide();
        $("#dvGeneratedKey").html('');
        $("#hdnGeneratedKey").val('');

        $("#dvDialogGenerateKey").dialog({
            modal: true,
            //closeText:"Hide",
            //draggable:true,
            //autoOpen: false,
            //show: {
            //    effect:"blind",
            //    duration:1000,
            //},
            //hide: {
            //    effect: "explode",
            //    duration: 1000
            //}
        });

        $("#hdnAdminNoOfficerCode").val(param);

        $(function () {
            GenerateKey();
        });

        
    });
}
function AlertWrongEmail()
{
    alert('Email entered is wrong. Please correct the email address.');
    return false;
}
function AlertEmailEntry()
{
    alert('Please enter Email Address.');
    return false;
}


function GenerateKey()
{

    $.ajax({
        url: '/Bank/GenerateKey/',
        async: false,
        catche: false,
        method: 'POST',
        success: function (response) {
            if (response.success == true) {
                $("#dvKeyLabel").show();
                $("#dvGeneratedKey").html(response.key);
                $("#hdnGeneratedKey").val(response.key);
                $("#dvGeneratedKey").show();
            }
            else if (response.success == false) {
                alert(response.message);
                return false;
            }
        },
        error: function (xhr, status, code) {
            alert("An error occured while processing your request.");
            return false;
        }
    });

}