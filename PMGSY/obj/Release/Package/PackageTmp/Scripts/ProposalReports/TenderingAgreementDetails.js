$(function () {

});
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_TenderAgreementDetails").attr("disabled", "disabled");
    }
    $("#StateList_TenderAgreementDetails").change(function () {

        $("#DistrictList_TenderAgreementDetails").val(0);
        $("#DistrictList_TenderAgreementDetails").empty();
        $("#BlockList_TenderAgreementDetails").val(0);
        $("#BlockList_TenderAgreementDetails").empty();
        $("#BlockList_TenderAgreementDetails").append("<option value='0'>All Block</option>");
        
        if ($(this).val() > 0) {
            loadContractorListByState($(this).val());
            if ($("#DistrictList_TenderAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_TenderAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_TenderAgreementDetails').find("option[value='0']").remove();
                        //$("#DistrictList_TenderAgreementDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_TenderAgreementDetails').val(0);

                        //For Disable if District Login
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_TenderAgreementDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_TenderAgreementDetails").attr("disabled", "disabled");
                            $("#DistrictList_TenderAgreementDetails").trigger('change');
                        }


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        else {

            $("#DistrictList_TenderAgreementDetails").append("<option value='0'>All District</option>");
            $("#BlockList_TenderAgreementDetails").empty();
            $("#BlockList_TenderAgreementDetails").append("<option value='0'>All Block</option>");

        }
    });

    $("#DistrictList_TenderAgreementDetails").change(function () {

        $("#BlockList_TenderAgreementDetails").val(0);
        $("#BlockList_TenderAgreementDetails").empty();
       
        if ($(this).val() > 0) {
            if ($("#BlockList_TenderAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_TenderAgreementDetails").val(), "DistrictCode": $("#DistrictList_TenderAgreementDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_TenderAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_TenderAgreementDetails').find("option[value='0']").remove();
                        //$("#BlockList_TenderAgreementDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_TenderAgreementDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_TenderAgreementDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#BlockList_TenderAgreementDetails").change(function () {

        $("#PackageList_TenderAgreementDetails").val(0);
        $("#PackageList_TenderAgreementDetails").empty();
       
        if ($(this).val() > 0) {
                 $.ajax({
                    url: '/ProposalReports/GetPackageByStateDistrictBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_TenderAgreementDetails").val(), "DistrictCode": $("#DistrictList_TenderAgreementDetails").val(), "BlockCode": $("#BlockList_TenderAgreementDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#PackageList_TenderAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }             


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
           
        } else {
            $("#PackageList_TenderAgreementDetails").append("<option value='0'>All</option>");
        }
    });

    $("#btnTenderAgreementDetails").click(function () {
        var stateCode = $("#StateList_TenderAgreementDetails").val();
        var districtCode = $("#DistrictList_TenderAgreementDetails").val();
        var blockCode = $("#BlockList_TenderAgreementDetails").val();
        var year = $("#YearList_TenderAgreementDetails").val();
        var batch = $("#BatchList_TenderAgreementDetails").val();
        var collaboration = $("#CollaborationList_TenderAgreementDetails").val();
        var status = $("#StatusList_TenderAgreementDetails").val();
        var conId = $("#ContractorList_TenderAgreementDetails").val();
        var agreement = $("#AgreementList_TenderAgreementDetails").val();
        var packageType = $("#PackageList_TenderAgreementDetails").val();

        TenderAgreementListing(blockCode, districtCode, stateCode, year, batch, collaboration, status,conId,agreement,packageType);
    });


    $("#StateList_TenderAgreementDetails").trigger('change');

    // $("#btnTenderAgreementDetails").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function TenderAgreementListing(blockCode, districtCode, stateCode, year, batch, collaboration, status, conId, agreement, packageType) {
    if (stateCode > 0) {
       
                LoadTenderAgreementGrid(blockCode, districtCode, stateCode, year, batch, collaboration, status, conId, agreement, packageType);
            }         

    
    else {
        alert("Please Select State");
    }

}

function LoadTenderAgreementGrid(blockCode, districtCode, stateCode, year, batch, collaboration, status, conId, agreement, packageType) {


    $("#tbTenderAgreementDetails").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbTenderAgreementDetails").jqGrid({
        url: '/ProposalReports/TenderingAgreementReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['District Name', 'Sanctioned Year', 'Package No.',"Contractor", 'Date of Work Order', 'Date of Completion', 'Agreement No.', 'Date of Agreement', 'Total Agreement Cost'],
        colModel: [
            { name: 'MAST_DISTRICT_NAME', width: 200, align: 'left', height: 'auto', sortable: true },
            { name: 'IMS_YEAR', width: 100, align: 'center', height: 'auto', sortable: false },
            { name: 'IMS_PACKAGE_ID', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'Cotractor', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'DateOfWork', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'DateOfComplete', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'AgreementNo', width: 120, align: 'left', height: 'auto', sortable: false},
            { name: 'AgreementDate', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: "AgreementCost", width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", decimalPlaces: 2, thousandsSeparator: ",", defaulValue: "N.A" } }

        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status, "ConId": conId, "Package": packageType, "Agreement": agreement },
        pager: $("#dvTenderAgreementDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_DISTRICT_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '480',
        viewrecords: true,
        footerrow: true,
        caption: '&nbsp;&nbsp;Tender Agreement  Details',
        loadComplete: function () {

            //Total of Columns
            var AgreementCost_T = $(this).jqGrid('getCol', 'AgreementCost', false, 'sum');
            AgreementCost_T = parseFloat(AgreementCost_T).toFixed(2);
            //var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            //TN_LEN_T = parseFloat(TN_LEN_T).toFixed(2);


            $(this).jqGrid('footerData', 'set', { MAST_DISTRICT_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { AgreementCost: AgreementCost_T }, true);
            //$(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);

            $('#tbTenderAgreementDetails_rn').html('Sr.<br/>No.');
           $("#dvTenderAgreementDetailsPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Cost Rs in Lacs</font>");

            $.unblockUI();

        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });//End of Grid

    $("#tbTenderAgreementDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'PersonalConst', numberOfColumns: 3, titleText: '<em> Personal Required</em>' },
          { startColumnName: 'EquipmentConst', numberOfColumns: 4, titleText: '<em> Equipment</em>' },

        ]
    });
}

function loadContractorListByState(StateCode) {
    $("#ContractorList_TenderAgreementDetails").val(0);
    $("#ContractorList_TenderAgreementDetails").empty();
    $.ajax({
        url: '/ProposalReports/GetContractoreNameByStateDetails',
        type: 'POST',
        data: { "StateCode": $("#StateList_TenderAgreementDetails").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ContractorList_TenderAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });

}