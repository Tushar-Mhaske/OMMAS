$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmPFMSDownloadOtherStateXMLLayout');


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
        if (!$("#frmPFMSDownloadOtherStateXMLLayout").valid()) {
            return false;
        }
        GetBeneficiaryList();
    });

    $('#btnDownloadXML').click(function () {

        if (!$("#frmPFMSDownloadOtherStateXMLLayout").valid()) {
            return false;
        }

        //window.open('/PFMS/GenerateXML' + $("#frmPFMSDownloadOtherStateXMLLayout").serialize(), '_blank');

        //window.open('/PFMS/GenerateXML?param=' + $('#ddlState option:selected').val() + "$" + $("#ddlAgency option:selected").val(), '_blank');

        $.ajax({
            type: 'GET',
            url: '/PFMS1/GenerateXML',
            data: $("#frmPFMSDownloadOtherStateXMLLayout").serialize(),
            success: function (data) {
                alert(data.message);
                //window.open(data, '_blank');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert("Error occurred while processing the request.");
            }
        })

        //$.ajax(
        //    '/PFMS/GenerateXML?param=' + $('#ddlState option:selected').val() + "$" + $("#ddlAgency option:selected").val(), {
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json',
        //    type: 'POST',
        //    //data: data,
        //    success: function (d) {
        //        //if (d.success) {
        //        //    window.location = getUrl + "?fName=" + d.fName;
        //        //}
        //        //window.location = d;
        //        window.open(d, '_blank');
        //    },
        //    error: function () {

        //    }
        //}).always(function () {
        //    //$('#adcExportToExcel').spin(false);
        //});

        //$.ajax({
        //    //type: 'GET',
        //    url: '/PFMS/GenerateXML?param=' + $('#ddlState option:selected').val() + "$" + $("#ddlAgency option:selected").val(),
        //    contentType: 'application/json; charset=utf-8',
        //    success: function (data) {
        //        //alert(data.message);
        //        //window.open(data, '_blank');
        //        var a = document.createElement('a');
        //        var url = window.URL.createObjectURL(data);
        //        a.href = url;
        //        a.download = 'myfile.pdf';
        //        a.click();
        //        window.URL.revokeObjectURL(url);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        //alert(xhr.responseText);
        //        alert("Error occurred while processing the request.");
        //    }
        //})

    });

});

function GetBeneficiaryList() {

    jQuery("#tblContractorsList").jqGrid('GridUnload');

    jQuery("#tblContractorsList").jqGrid({
        url: '/PFMS1/GetBeneficiaryDetails/',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlState option:selected").val(), districtCode: $("#ddlDistrict option:selected").val(), agencyCode: $("#ddlAgency option:selected").val() },
        colNames: ['Contractor &nbsp;&nbsp;<input id="selectAll" type="checkbox" name="AllContractors" value="SelectAll"/>', 'Contractor Name', 'PAN No.', 'Company Name', "Bank Name", 'Account Id', "IFSC Code", 'Account Number', 'Status', 'Generated Date'],
        colModel: [
                        { name: 'ContractorId', index: 'ContractorId', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'ContractorName', index: 'ContractorName', height: 'auto', width: 90, align: "left", sortable: true, search: false },
                        { name: 'PanNo', index: 'PanNo', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'CompanyName', index: 'CompanyName', width: 150, sortable: true, align: "left", search: false }, //New
                        { name: 'BankName', index: 'BankName', width: 120, sortable: true, align: "left", search: false },
                        { name: 'AccountId', index: 'AccountId', width: 90, sortable: true, align: "left", search: true },
                        { name: 'IFSCCode', index: 'IFSCCode', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'AccountNumber', index: 'AccountNumber', width: 100, sortable: true, align: "left", search: false },
                        { name: 'Agency', index: 'Agency', width: 100, sortable: true, align: "left", search: false, hidden: true },
                        { name: 'Date', index: 'Date', width: 100, sortable: true, align: "left", search: false },
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
                $("#dvPagerContractorsList_left").html("<input type='button' style='margin-left:27px;margin-top:5px;' id='btnDownloadXML' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DownloadXml();return false;' value='Download XML'/>");
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
    if (!$("#frmPFMSDownloadOtherStateXMLLayout").valid()) {
        return false;
    }

    //window.open('/PFMS/GenerateXML' + $("#frmPFMSDownloadOtherStateXMLLayout").serialize(), '_blank');

    //window.open('/PFMS/GenerateXML?param=' + $('#ddlState option:selected').val() + "$" + $("#ddlAgency option:selected").val(), '_blank');

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
    //alert(parseInt($('#Level').val()));
    if ((DropApproveArrayAll.length > 0 && parseInt($('#Level').val()) == 1)
                ||
            (parseInt($('#Level').val()) == 2)
       ) {

        $.ajax({
            type: 'GET',
            url: '/PFMS1/GenerateXML',
            //data: $("#frmPFMSDownloadOtherStateXMLLayout").serialize(),
            cache: false,
            async: true,
            traditional: true,
            data: { level: $('#Level').val(), stateCode: $('#ddlState').val(), districtCode: $('#ddlDistrict').val(), agencyCode: $('#ddlAgency').val(), Contractors: DropApproveArray },
            success: function (data) {
                alert(data.message);
                GetBeneficiaryList();
                //window.open(data, '_blank');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
                alert("Error occurred while processing the request.");
            }
        })
    }
    else {
        alert('No Contractor selected, please select Contractor');
    }
}