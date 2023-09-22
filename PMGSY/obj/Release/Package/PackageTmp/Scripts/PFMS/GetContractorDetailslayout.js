$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmPFMSDownloadXMLLayout');
    $('#btnGetBeneficiaryList').click(function () {
        if (!$("#frmPFMSDownloadXMLLayout").valid()) {
            return false;
        }
        GetBeneficiaryList();
    });

});

function changeLabel()
{
    if ($("input[name='Status']:checked").val() == 'R')
    {
        return "Activate Account";
        //jQuery("#tblContractorsList").jqGrid('setLabel', 10, 'NewLabel');
        //console.log('new');
    }
    else
    {
        return "Deactivate Account";
    }
}

function GetBeneficiaryList() {

    jQuery("#tblContractorsList").jqGrid('GridUnload');

    jQuery("#tblContractorsList").jqGrid({
        url: '/PFMS1/DeactivateContractorAccount/',
        datatype: "json",
        mtype: "GET",
        postData: { stateCode: $("#ddlState option:selected").val(), panNumber: $("#tbPanNumber").val(), status: $("input[name='Status']:checked").val() },
        colNames: ['pocmid', 'Contractor ID', 'Account id', 'District code', 'Agency code', 'Contractor Name', "State","District Name", "Agency Name" ,"Bank Name", "IFSC Code", 'Account Number', changeLabel()],
        colModel: [
                        { name: 'pocmID', index: 'pocmID', height: 'auto', width: 40, align: "center", search: false, hidden: true },
                        { name: 'ContractorId', index: 'ContractorId', height: 'auto', width: 40, align: "center", search: false, hidden: true },
                        { name: 'AccountId', index: 'AccountId', width: 90, sortable: true, align: "left", search: true, hidden:true },
                        { name: 'DistrictLGDCode', index: 'DistrictLGDCode', height: 'auto', width: 40, align: "center", search: false, hidden: true },
                        { name: 'AgencyCode', index: 'AgencyCode', height: 'auto', width: 40, align: "center", search: false, hidden: true },
                        { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 90, align: "left", sortable: true, search: false },
                        { name: 'StateName', index: 'StateName', height: 'auto', width: 60, align: "center", search: false },
                        { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 60, align: "center", search: false },
                        { name: 'AgencyName', index: 'AgencyName', height: 'auto', width: 60, align: "center", search: false },
                        { name: 'BankName', index: 'BankName', width: 100, sortable: true, align: "left", search: false },
                        { name: 'IFSCCode', index: 'IFSCCode', width: 80, sortable: true, align: "left", search: false }, //New
                        { name: 'AccountNumber', index: 'AccountNumber', width: 100, sortable: true, align: "left", search: false },
                        { name: 'DeactivateAccount', index: 'DeactivateAccount', width: 100, sortable: true, align: "left", search: false , hidden: false, formatter: changeLabel() },
                      
        ],
    
        pager: jQuery('#dvPagerContractorsList'),
        rowNum: 100,
        sortorder: "desc",
        sortname: 'ContractorName',
        rowList: [100],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Contractor Details",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {
            var ids = jQuery("#tblContractorsList").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                //console.log(ids);
                cl = ids[i];
                be = "<center><span class='ui-icon ui-icon-pencil' id = 'edit" + i +"'"+ " "+" title = 'Update Account Details' onClick ='UpdateLink(\"" + cl + "\");'></span></center>";
                var temp = "#edit" + i;
               jQuery("#tblContractorsList").jqGrid('setRowData', ids[i], { DeactivateAccount: be });
             }
            
        },

        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function UpdateLink(rowdata)
{
    var pocmID = $('#tblContractorsList').jqGrid('getCell', rowdata, 'pocmID');
    var token = $('input[name=__RequestVerificationToken]').val();
    
    if(confirm("Are You Sure You want to"+" "+($("input[name='Status']:checked").val()=="A"?"Deactivate" : "Activate")+" "+"this account ?"))
    {
        $.ajax({
            type: 'POST',
            url: '/PFMS1/UpdateContractorAccount',
            cache: false,
            async: true,
            traditional: true,
            data: {pocmid : pocmID,"__RequestVerificationToken": token },
            success: function (data) {
                alert(data.message);
                GetBeneficiaryList();
           },
            error: function (xhr, ajaxOptions, thrownError) {
                        
                alert("Error occurred while processing the request.");
            }
        })
    }
}












