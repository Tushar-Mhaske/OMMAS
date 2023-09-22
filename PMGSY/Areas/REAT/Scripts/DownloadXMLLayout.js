$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmPFMSDownloadXMLLayout');


    $('#ddlState').change(function () {
        $("#ddlAgency").empty();
        $.ajax({
            url: '/PFMS1/PopulateAgencybyStateCode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlState").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });

        $("#ddlDistrict").empty();
        $.ajax({
            url: '/PFMS1/PopulateDistrictsbyStateCode',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlState").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $('#btnGetBeneficiaryList').click(function () {
        if (!$("#frmPFMSDownloadXMLLayout").valid()) {
            return false;
        }
        GetBeneficiaryList();
    });

    $('#btnDownloadXML').click(function () {

        if (!$("#frmPFMSDownloadXMLLayout").valid()) {
            return false;
        }

        $.ajax({
            type: 'GET',
            url: '/REAT/Reat/GenerateXML',
            data: $("#frmPFMSDownloadXMLLayout").serialize(),
            success: function (data) {
                alert(data.message);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        })

    });

});

function GetBeneficiaryList() {

    jQuery("#tblContractorsList").jqGrid('GridUnload');

    jQuery("#tblContractorsList").jqGrid({
        url: '/REAT/Reat/GetBeneficiaryDetails',
        datatype: "json",
        mtype: "POST",
        postData:
            { stateCode: $("#ddlState option:selected").val(), districtCode: $("#ddlDistrict option:selected").val(), agencyCode: $("#ddlAgency option:selected").val() },
        //Below Condition is modified on 13-06-2022 
        //colNames: ['Contractor &nbsp;&nbsp;<input id="selectAll" type="checkbox" name="AllContractors" value="SelectAll"/>', 'Contractor Name', 'PAN No.', 'Company Name', "Bank Name", 'Account Id', "IFSC Code", 'Account Number', 'Status', 'Generated Date', 'DBT Status'],
        colNames: ['Contractor &nbsp;&nbsp;<input id="selectAll" type="checkbox" name="AllContractors" value="SelectAll"/>', 'Contractor Name', 'PAN No.', 'Company Name', "Bank Name", 'Account Id', "IFSC Code", 'Account Number', /*'Status',*/ 'DBT Status','Generated Date' ],
        colModel: [
                        { name: 'ContractorId', index: 'ContractorId', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 90, align: "left", sortable: true, search: false },
                        { name: 'PanNo', index: 'PanNo', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'CompanyName', index: 'CompanyName', width: 150, sortable: true, align: "left", search: false }, //New
                        { name: 'BankName', index: 'BankName', width: 120, sortable: true, align: "left", search: false },
                        { name: 'AccountId', index: 'AccountId', width: 90, sortable: true, align: "left", search: true },
                        { name: 'IFSCCode', index: 'IFSCCode', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'AccountNumber', index: 'AccountNumber', width: 100, sortable: true, align: "left", search: false },

                        //Below Condition is modified on 13-06-2022
                        //{ name: 'Agency', index: 'Agency', width: 100, sortable: true, align: "left", search: false, hidden: true },
                        { name: 'PFMSStatus', index: 'PFMSStatus', width: 100, sortable: true, align: "left", search: false },
                        { name: 'Date', index: 'Date', width: 100, sortable: true, align: "left", search: false },
        ],
        pager: jQuery('#dvPagerContractorsList'),
        rowNum: 50,
        sortorder: "desc",
        sortname: 'ContractorName',
        rowList: [50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Beneficiary List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {

            $('#selectAll').hide();
            $('#selectAll').parent().removeClass('ui-jqgrid-sortable');//to make header checkbox clickable

            $('#selectAll').change(function () {
                if ($(this).prop('checked')) {
                    $('.cbxCon').not(":disabled").prop('checked', true);
                }
                else {
                    $('.cbxCon').not(":disabled").prop('checked', false);
                }
            });

            var droppedBefore = []
            $.each($("input[name='cbContractor']:checked"), function (i, value) {
                droppedBefore[i] = $(this).val().trim();
            })

            var records = jQuery("#tblContractorsList").jqGrid('getGridParam', 'records');

            if (records == droppedBefore.length) {
                $('#selectAll').prop('checked', true);
                $('#selectAll').prop('disabled', true);
            }

            $("input[name = 'cbContractor']").change(function () {

                var dropped = []
                $.each($("input[name='cbContractor']:checked"), function (i, value) {
                    dropped[i] = $(this).val().trim();
                })

                if (records != dropped.length) {
                    $('#selectAll').prop('checked', false);
                }
                else {
                    $('#selectAll').prop('checked', true);
                }
            });

            $("#tblContractorsList #dvPagerContractorsList").css({ height: '31px' });
            if ($('#tblContractorsList').jqGrid('getGridParam', 'records')) {
                $("#dvPagerContractorsList_left").html("<input type='button' style='margin-left:27px;margin-top:2px;' id='btnDownloadXML' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DownloadXml();return false;' value='Download XML'/>");
            }
            unblockPage();
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

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

function DownloadXml() {
    if (!$("#frmPFMSDownloadXMLLayout").valid()) {
        return false;
    }

    var postData = $("#tblContractorsList").jqGrid('getGridParam', 'postData');

    var records = $('#tblContractorsList').jqGrid('getGridParam', 'records');

    var DropApproveArray = [];
    $.each($("input[name='cbContractor']:checked:not(:disabled)"), function (i, value) {
        DropApproveArray[i] = $(this).val().trim();
    })

    var DropApproveArrayAll = [];
    $.each($("input[name='cbContractor']:checked"), function (i, value) {
        DropApproveArrayAll[i] = $(this).val().trim();
    })

    if ($('#cbAllContractors').prop('checked')) {
        $("#tblContractorsList").jqGrid('hideCol', 'ContractorId');
        $('#Level').val(2);
    }
    else {
        $("#tblContractorsList").jqGrid('showCol', 'ContractorId');
        $('#Level').val(1);
    }

    if ((DropApproveArrayAll.length > 0 && parseInt($('#Level').val()) == 1)
                ||
            (parseInt($('#Level').val()) == 2)
       ) {

        $.ajax({
            type: 'POST',
            url: '/REAT/Reat/GenerateXML',
            cache: false,
            async: true,
            traditional: true,
            data: { level: $('#Level').val(), stateCode: $('#ddlState').val(), districtCode: $('#ddlDistrict').val(), agencyCode: $('#ddlAgency').val(), Contractors: DropApproveArray },
            success: function (data) {
                alert(data.message);
                GetBeneficiaryList();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        })
    }
    else {
        alert('No Contractor selected, please select Contractor');
    }
}