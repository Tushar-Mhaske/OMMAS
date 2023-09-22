$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmUpdateBeneficiaryIFSCLayout');
    
    $('#btnGetBeneficiaryList').click(function () {

        if (!$('#frmUpdateBeneficiaryIFSCLayout').valid())
        {
            return false;
        }

        GetBeneficiaryListforUpdate();
    });
});

function GetBeneficiaryListforUpdate() {
    //alert($("#txtPanNumber").val());
    jQuery("#tblContractorsList").jqGrid('GridUnload');

    jQuery("#tblContractorsList").jqGrid({
        url: '/PFMS1/GetBeneficiaryDetailsForUpdate/',
        datatype: "json",
        mtype: "GET",
        postData: { panNo: $("#txtPanNumber").val() },
        colNames: ['Contractor &nbsp;&nbsp;<input id="selectAll" type="checkbox" name="AllContractors" value="SelectAll"/>', 'Contractor Name', 'State', 'Agency', 'PAN No.', 'Company Name', "Bank Name", 'Account Id', "OMMAS IFSC Code", "PFMS IFSC Code", 'Account Number', 'Status', 'Edit', 'Action'],
        colModel: [
                        { name: 'ContractorId', index: 'ContractorId', height: 'auto', width: 40, align: "center", search: false, hidden:true },
                        { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 90, align: "left", sortable: true, search: false },
                        { name: 'State', index: 'State', width: 100, sortable: true, align: "left", search: false },
                        { name: 'Agency', index: 'Agency', width: 150, sortable: true, align: "left", search: false },
                        { name: 'PanNo', index: 'PanNo', height: 'auto', width: 70, align: "center", search: false },
                        { name: 'CompanyName', index: 'CompanyName', width: 120, sortable: true, align: "left", search: false }, //New
                        { name: 'BankName', index: 'BankName', width: 120, sortable: true, align: "left", search: false },
                        { name: 'AccountId', index: 'AccountId', width: 50, sortable: true, align: "right", search: true },
                        { name: 'BIFSCCode', index: 'BIFSCCode', width: 70, sortable: true, align: "center", search: false, }, //New
                        { name: 'IFSCCode', index: 'IFSCCode', width: 70, sortable: true, align: "center", search: false, editable: true, editoptions: { maxlength: 11, /*style: "width:50px !important;"*/ }, },
                        { name: 'AccountNumber', index: 'AccountNumber', width: 80, sortable: true, align: "right", search: false },
                        { name: 'Status', index: 'Status', width: 70, sortable: true, align: "left", search: false },
                        { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                        { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true },
        ],
        //postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvPagerContractorsList'),
        rowNum: 100,
        sortorder: "desc",
        sortname: 'ContractorName',
        rowList: [100],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Beneficiary List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {
           
            unblockPage();
        },
        //editData: {
        //    pocmId: 1
        //},
        editurl: "/PFMS1/GenerateUpdateXML",
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

    //$("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

/*Added for Editing Grid Row*/
function EditRoadProgressDetails(paramFileID) {
    //alert(paramFileID);
    //alert(("#" + paramFileID + '_IFSCCode'));
    jQuery("#tblContractorsList").editRow(paramFileID);
    $("#" + paramFileID + '_IFSCCode').width(90);
    $('#tblContractorsList').jqGrid('showCol', 'Save');
    //alert(paramFileID);
}

function SaveRoadProgressDetails(paramFileID) {
    //alert(paramFileID);
    var id = new Array();
    id = paramFileID.split('$');
    //alert(id[0]);
    //$('#YEAR').val(id[1]);
    //$('#MONTH').val(id[2]);
    jQuery("#tblContractorsList").saveRow(id[0], checksave);
    
    //saveparameters = {
    //    "successfunc": null,
    //    "url": null,
    //    "extraparam": { __RequestVerificationToken: jQuery('input[name=__RequestVerificationToken]').val() },
    //    "aftersavefunc": null,
    //    "errorfunc": null,
    //    "afterrestorefunc": null,
    //    "restoreAfterError": true,
    //    "mtype": "POST"
    //}

    //jQuery("#tblContractorsList").saveRow(id[0], checksave, saveparameters);
}

function CancelRoadProgressDetails(paramFileID) {
    var id = new Array();
    id = paramFileID.split('$');

    $('#tblContractorsList').jqGrid('hideCol', 'Save');
    jQuery("#tblContractorsList").restoreRow(id[0]);
    //alert(paramFileID);
}

function checksave(result) {
    $('#tblContractorsList').jqGrid('hideCol', 'Save');
    //console.log(result.responseText.success);
    //console.log(result.responseText.message);
    //console.log(JSON.stringify(result.responseText.message));
    //console.log(JSON.parse(result.responseText));
    console.log(JSON.parse(result.responseText).message);
    //console.log(JSON.parse(data.jqXHR.responseText));
    //console.log(result);
   /* if (result.responseText == "true") {
        alert('Details Updated Successfully.');
        jQuery("#tblContractorsList").trigger('reload');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }*/
    //alert(JSON.parse(data.jqXHR.responseText).success);

    //alert(result.responseText);
    //alert(result);
    alert(JSON.parse(result.responseText).message);
    //jQuery("#tblContractorsList").trigger('reload');
    GetBeneficiaryListforUpdate();
    return true;
}

function ValidateLength(value, colname) {
    //alert('v=' + value.trim().length);
    //alert(value.match(/^[a-zA-Z0-9]+$/));
    if (value.trim().length == 0) {
        //alert("1" + value.trim().length == 0);
        return ["Please Enter " + colname + "."];
    }
    else if (!value.match(/^[a-zA-Z0-9]+$/)) {
        //alert('2' + !value.match(/^[a-zA-Z0-9]+$/));
        return ["No Special Characters Allowed."];
    }
    else {
        return [true, ""];
    }
}

/*Editing Grid Row Ends*/