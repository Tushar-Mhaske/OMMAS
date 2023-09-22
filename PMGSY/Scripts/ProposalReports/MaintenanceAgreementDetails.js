$(function () {

});
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_MaintenAgreementDetails").attr("disabled", "disabled");
    }
    $("#StateList_MaintenAgreementDetails").change(function () {

        $("#DistrictList_MaintenAgreementDetails").val(0);
        $("#DistrictList_MaintenAgreementDetails").empty();
        $("#BlockList_MaintenAgreementDetails").val(0);
        $("#BlockList_MaintenAgreementDetails").empty();
        $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Block</option>");
        // $("#DistrictList").append("<option value='0'>Select District</option>");

        if ($(this).val() > 0) {
           
            if ($("#DistrictList_MaintenAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#DistrictList_MaintenAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                        //$('#DistrictList_MaintenAgreementDetails').find("option[value='0']").remove();
                        //$("#DistrictList_MaintenAgreementDetails").append("<option value='0'>Select District</option>");
                        //$('#DistrictList_MaintenAgreementDetails').val(0);

                        //For Disable if District Login
                        if ($("#MAST_DISTRICT_CODE").val() > 0) {
                            $("#DistrictList_MaintenAgreementDetails").val($("#MAST_DISTRICT_CODE").val());
                            $("#DistrictList_MaintenAgreementDetails").attr("disabled", "disabled");
                            $("#DistrictList_MaintenAgreementDetails").trigger('change');
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

            $("#DistrictList_MaintenAgreementDetails").append("<option value='0'>All District</option>");
            $("#BlockList_MaintenAgreementDetails").empty();
            $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Block</option>");

        }
    });

    $("#DistrictList_MaintenAgreementDetails").change(function () {

        $("#BlockList_MaintenAgreementDetails").val(0);
        $("#BlockList_MaintenAgreementDetails").empty();
       

        if ($(this).val() > 0) {
            if ($("#BlockList_MaintenAgreementDetails").length > 0) {
                $.ajax({
                    url: '/ProposalReports/AllBlockDetails',
                    type: 'POST',
                    data: { "StateCode": $("#StateList_MaintenAgreementDetails").val(), "DistrictCode": $("#DistrictList_MaintenAgreementDetails").val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#BlockList_MaintenAgreementDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }

                        //$('#BlockList_MaintenAgreementDetails').find("option[value='0']").remove();
                        //$("#BlockList_MaintenAgreementDetails").append("<option value='0'>Select Block</option>");
                        //$('#BlockList_MaintenAgreementDetails').val(0);


                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        } else {
            $("#BlockList_MaintenAgreementDetails").append("<option value='0'>All Block</option>");
        }
    });

    $("#MaintenAgreementDetailsButton").click(function () {
        var stateCode = $("#StateList_MaintenAgreementDetails").val();
        var districtCode = $("#DistrictList_MaintenAgreementDetails").val();
        var blockCode = $("#BlockList_MaintenAgreementDetails").val();
        var year = $("#YearList_MaintenAgreementDetails").val();
        var batch = $("#BatchList_MaintenAgreementDetails").val();
        var collaboration = $("#CollaborationList_MaintenAgreementDetails").val();
        var status = $("#StatusList_MaintenAgreementDetails").val();  


        MaintenAgreementListing(blockCode, districtCode, stateCode, year, batch, collaboration, status);
    });


    $("#StateList_MaintenAgreementDetails").trigger('change');

     //$("#MaintenAgreementDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});


function MaintenAgreementListing(blockCode, districtCode, stateCode, year, batch, collaboration, status) {
    if (stateCode > 0) {
        
        LoadMaintenAgreementGrid(blockCode, districtCode, stateCode, year, batch, collaboration, status);
    }
    else {
        alert("Please Select State");
    }

}

function LoadMaintenAgreementGrid(blockCode, districtCode, stateCode, year, batch, collaboration, status) {


    $("#tbMaintenAgreementDetails").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tbMaintenAgreementDetails").jqGrid({
        url: '/ProposalReports/MaintenanceAgreementReportListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Package No.', 'Sanctioned Year', 'Road Name', 'Construction Compltetion Date (DD/MM/YYYY)', 'Maintenance Start Date (DD/MM/YYYY)', 'Contractor', 'Total Amount'],
        colModel: [
            { name: 'IMS_PACKAGE_ID', width: 150, align: 'left', height: 'auto', sortable: false },
            { name: 'IMS_YEAR', width: 100, align: 'center', height: 'auto', sortable: true },
            { name: 'IMS_ROAD_NAME', width: 200, align: 'left', height: 'auto', sortable: false },
            { name: 'ConstrCompDate', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'MaintStartDate', width: 120, align: 'center', height: 'auto', sortable: false },
            { name: 'Contractor', width: 400, align: 'left', height: 'auto', sortable: false },
            { name: 'TotalAmount', width: 120, align: 'right', height: 'auto', sortable: false, formatter: 'number', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, defaulValue: "N.A" } },
          
        ],
        postData: { "StateCode": stateCode, "DistrictCode": districtCode, "BlockCode": blockCode, "Year": year, "Batch": batch, "Collaboration": collaboration, "Status": status, "Package": "%", "Agreement": "%" },
        pager: $("#dvMaintenAgreementDetailsPager"),
        pgbuttons: true,
        sortname: 'IMS_PACKAGE_ID',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '490',
        viewrecords: true,
        footerrow: true,
        caption: 'Maintenance Agreement  Details',
        loadComplete: function () {

            //Total of Columns
            var TotalAmount_T = $(this).jqGrid('getCol', 'TotalAmount', false, 'sum');
            TotalAmount_T = parseFloat(TotalAmount_T).toFixed(2);
            //var TN_LEN_T = $(this).jqGrid('getCol', 'TN_LEN', false, 'sum');
            //TN_LEN_T = parseFloat(TN_LEN_T).toFixed(2);


            $(this).jqGrid('footerData', 'set', { IMS_PACKAGE_ID: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { TotalAmount: TotalAmount_T }, true);
            //$(this).jqGrid('footerData', 'set', { TN_LEN: TN_LEN_T }, true);

            $('#tbMaintenAgreementDetails_rn').html('Sr.<br/>No.');
            $("#dvMaintenAgreementDetailsPager_left").html("<span style='float:left' class='ui-icon ui-icon-info'></span> <font color='black' style='font-weight:bold;margin-left:5px;'>Amount Rs in Lacs</font>");

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

    $("#tbMaintenAgreementDetails").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'ConstrCompDate', numberOfColumns: 4, titleText: '<em> Contractor</em>' },
        ]
    });
}

