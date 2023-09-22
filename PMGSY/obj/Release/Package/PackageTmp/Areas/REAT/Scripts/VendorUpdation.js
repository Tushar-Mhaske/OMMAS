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
        url: '/REAT/Reat/GetBeneficiaryUpdation',
        datatype: "json",
        mtype: "POST",
        postData:
            { stateCode: $("#ddlState option:selected").val(), districtCode: $("#ddlDistrict option:selected").val(), agencyCode: $("#ddlAgency option:selected").val() },
        colNames: ['Contractor ID', 'Contractor Name', 'PAN No.', 'Company Name', "Bank Name", 'Account Id', "IFSC Code", 'Account Number', 'Update'],
        colModel: [
                        { name: 'MAST_CON_ID', index: 'MAST_CON_ID', height: 'auto', width: 40, align: "center", search: false },
                        { name: 'reat_CON_NAME', index: 'reat_CON_NAME', height: 'auto', width: 90, align: "left", sortable: true, search: false },
                        { name: 'MAST_CON_PAN', index: 'MAST_CON_PAN', height: 'auto', width: 60, align: "center", search: false },
                        { name: 'MAST_CON_COMPANY_NAME', index: 'MAST_CON_COMPANY_NAME', width: 150, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_BANK_NAME', index: 'MAST_BANK_NAME', width: 120, sortable: true, align: "left", search: false },
                        { name: 'MAST_ACCOUNT_ID', index: 'MAST_ACCOUNT_ID', width: 90, sortable: true, align: "left", search: true, hidden: true },
                        { name: 'MAST_IFSC_CODE', index: 'MAST_IFSC_CODE', width: 90, sortable: true, align: "left", search: false, hidden:true }, //New
                        { name: 'MAST_ACCOUNT_NUMBER', index: 'MAST_ACCOUNT_NUMBER', width: 100, sortable: true, align: "left", search: false },
                        { name: 'update', index: 'update', width: 50, sortable: true, align: "left", search: false },
                      
        ],
        pager: jQuery('#dvPagerContractorsList'),
        rowNum: 100,
        sortorder: "desc",
        sortname: 'ContractorName',
        rowList: [100,200,300,400,500,600],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Beneficiary List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        //loadComplete: function () {

        //    $('#selectAll').hide();
        //    $('#selectAll').parent().removeClass('ui-jqgrid-sortable');//to make header checkbox clickable

        //    $('#selectAll').change(function () {
        //        if ($(this).prop('checked')) {
        //            $('.cbxCon').not(":disabled").prop('checked', true);
        //        }
        //        else {
        //            $('.cbxCon').not(":disabled").prop('checked', false);
        //        }
        //    });

        //    var droppedBefore = []
        //    $.each($("input[name='cbContractor']:checked"), function (i, value) {
        //        droppedBefore[i] = $(this).val().trim();
        //    })

        //    var records = jQuery("#tblContractorsList").jqGrid('getGridParam', 'records');

        //    if (records == droppedBefore.length) {
        //        $('#selectAll').prop('checked', true);
        //        $('#selectAll').prop('disabled', true);
        //    }

        //    $("input[name = 'cbContractor']").change(function () {

        //        var dropped = []
        //        $.each($("input[name='cbContractor']:checked"), function (i, value) {
        //            dropped[i] = $(this).val().trim();
        //        })

        //        if (records != dropped.length) {
        //            $('#selectAll').prop('checked', false);
        //        }
        //        else {
        //            $('#selectAll').prop('checked', true);
        //        }
        //    });

        //    $("#tblContractorsList #dvPagerContractorsList").css({ height: '31px' });
        //    if ($('#tblContractorsList').jqGrid('getGridParam', 'records')) {
        //        $("#dvPagerContractorsList_left").html("<input type='button' style='margin-left:27px;margin-top:2px;' id='btnDownloadXML' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DownloadXml();return false;' value='Download XML'/>");
        //    }
        //    unblockPage();
        //},
        loadError: function (xhr, ststus, error)
        {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else
            {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            } 
        }

    }); //end of grid

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}


//function EditContractor(ConId) {
//   // alert(ConId);
//   // alert("d");
//   // debugger;
//    $('#dvhdPFMSDownloadXMLLayout').hide('slow');
//    $('#dvPFMSDownloadXMLLayout').hide('slow');
//    blockPage();
//    $("#divContractor").load('/REAT/Reat/EditContractor?id=' + ConId, function () {
//        alert(ConId);
//        $.validator.unobtrusive.parse($('#frmPFMSDownloadXMLLayout'));
//        unblockPage();
//    });
//    $('#divContractor').show('slow');
//}

function EditContractor(urlparameter) {

//    alert(urlparameter)

   // $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/REAT/Reat/EditContractor?id=' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data)

        {
            //if (data == null || data == '')
            //{
            //    alert("Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details. Then only proceed with these Correction Details.")
            //}

            
            $("#dvPFMSDownloadXMLLayout").hide('slow');

            $("#dvhdPFMSDownloadXMLLayout").hide('slow');
            

            $("#benUpdate").show('slow');
            $("#benUpdate").html(data); 

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("Error occurred while processing your request.");
            return false;
        }

    })
}