$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmDSCDetailsForDeregister');

    GetDSCDerigesterDetailsList();
});


function GetDSCDerigesterDetailsList() {

    jQuery("#tblDSCDetailsForDeregister").jqGrid('GridUnload');

    jQuery("#tblDSCDetailsForDeregister").jqGrid({
        url: '/PFMS1/GetDSCDetailsListForDeregister/',
        datatype: "json",
        mtype: "POST",
        //postData: { stateCode: $("#ddlState option:selected").val(), districtCode: $("#ddlDistrict option:selected").val() },
        colNames: ['DPIU Name', 'Authorised Signatory Name', "Start Date", 'Mobile', "Email", 'Registered DSC', 'Un-Register DSC from PFMS'],
        colModel: [
                        { name: 'DPIUName', index: 'DPIUName', height: 'auto', width: 90, align: "left", sortable: true, search: false, hidden: true, },
                        { name: 'AuthSigName', index: 'AuthSigName', width: 150, sortable: true, align: "left", search: false }, //New
                        { name: 'StartDate', index: 'StartDate', width: 120, sortable: true, align: "left", search: false },
                        { name: 'Mobile', index: 'Mobile', width: 90, sortable: true, align: "left", search: true },
                        { name: 'Email', index: 'Email', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'RegisteredDSC', index: 'RegisteredDSC', width: 100, sortable: true, align: "left", search: false, hidden: true, },
                        { name: 'UnRegisteredDSC', index: 'UnRegisteredDSC', width: 100, sortable: true, align: "left", search: false }

        ],
        //postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvPagerDSCDetailsForDeregister'),
        rowNum: 10,
        sortorder: "desc",
        sortname: 'ContractorName',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Deregister DSC for Nodal Officer",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {
            //$("#tblDSCDetailsForDeregister #dvPagerContractorsList").css({ height: '31px' });
            //if ($('#tblDSCDetailsForDeregister').jqGrid('getGridParam', 'records')) {
            //    $("#dvPagerContractorsList_left").html("<input type='button' style='margin-left:27px;margin-top:5px;' id='btnDownloadXML' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DownloadXml();return false;' value='Download XML'/>");
            //}
            //unblockPage();
        },
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

function DeRegister(urlparam) {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //var Todelete = confirm('Are you sure you want to un-register digital certificate ?');

    //if (Todelete) {

    //    blockPage();
    //    $.ajax({
    //        type: "POST",
    //        url: "/AuthorizedSignatory/DeRegisterDSC/" + urlparam,

    //        data: $("form").serialize(),
    //        error: function (xhr, status, error) {
    //            unblockPage();
    //            $('#errorSpan').text(xhr.responseText);
    //            $('#divError').show('slow');

    //            return false;

    //        },
    //        success: function (data) {
    //            unblockPage();
    //            $('#divError').hide('slow');
    //            $('#errorSpan').html("");
    //            $('#errorSpan').hide();

    //            if (data.result == 1) {
    //                alert("Digital Signature un-registered Successfuly.");


    //                $("#AuthorizedSigList").jqGrid().setGridParam({ url: '/AuthorizedSignatory/GetAuthorizedSignatoryList/' }).trigger("reloadGrid");
    //                return false;
    //            }

    //            else {

    //                alert("Error while un-registering Digital Certificate  ");
    //                return false;
    //            }
    //        }
    //    }); //end of ajax
    //}


    //$.unblockUI();

    $.ajax({
        url: '/PFMS1/SignEpaymentDSCXml',
        type: 'POST',
        cache: false,
        data: { operation : 'D', officerCode : urlparam },
        success: function (response) {
            $("#containerDsc").html(response);
        },
        complete: function () {

        },
        error: function () {
            alert("Error occured while processing your request");
        },
    });
}