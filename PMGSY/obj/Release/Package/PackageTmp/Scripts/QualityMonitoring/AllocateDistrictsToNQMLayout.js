$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmAllocateDistrictsToNQMLayout");

    $('#btnView').click(function () {
        GetTargetDistrictList();
    });

});

function GetTargetDistrictList() {

    jQuery("#tblQMDistrictList").jqGrid('GridUnload');

    jQuery("#tblQMDistrictList").jqGrid({
        url: '/QualityMonitoring/QMGetInspectionTargetList/',
        datatype: "json",
        mtype: "GET",
        postData: { stateCode: $('#ddlState').val(), month: $('#ddlMonth').val(), year: $('#ddlYear').val() },
        colNames: ['State', 'Target', 'Allocated Monitors', 'Allocate District'],
        colModel: [
                        { name: 'State', index: 'State', height: 'auto', width: 90, align: "center", search: false },
                        { name: 'Target', index: 'Target', height: 'auto', width: 90, align: "center", sortable: true, search: false },
                        { name: 'AllocateMonitors', index: 'AllocateMonitors', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'AllocateDistrict', index: 'AllocateDistrict', height: 'auto', width: 40, align: "center", search: false },
        ],
        //postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvPagerQMDistrictList'),
        rowNum: 100,
        sortorder: "desc",
        sortname: 'ContractorName',
        //rowList: [50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Target District List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {

            //unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            alert(xhr.responseText);
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



function QMAssignDistricts(urlparameter) {

    //$("#accordion div").html("");
    //$("#accordion h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Add Payment Details</a>" +
    //        '<a href="#" style="float: right;">' +
    //        '<img class="ui-icon ui-icon-closethick" onclick="ClosePaymentDetails();" /></a>'
    //        );

    //$('#accordion').show('fold', function () {
    //    blockPage();
    //    $("#divAddTourPayment").load('/QualityMonitoring/QMAssignTargetDistricts?id=' + urlparameter, function () {
    //        //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
    //        unblockPage();
    //    });
    //    $('#divAddTourPayment').show('slow');
    //    $("#divAddTourPayment").css('height', 'auto');
    //});
    //$("#tblTourPaymentInvoiceList").jqGrid('setGridState', 'hidden');
    //$('#idFilterDiv').trigger('click');


    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/QMAssignTargetDistricts/' + urlparameter,
        data: $("#frmAllocateDistrictsToNQMLayout").serialize(),
        success: function (data) {
            alert(data.message);
            //window.open(data, '_blank');
            $("#tblQMDistrictList").trigger('reloadGrid');
        },
        //error: function (xhr, ajaxOptions, thrownError) {
        //    //alert(xhr.responseText);
        //    alert("Error occurred while processing the request.");



            error: function (data) {
                //alert(xhr.responseText);
                alert(data.message);
        }
    })

}