$(document).ready(function () {

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    isGridLoaded = true;

    $('#btnGrievanceListForSrrda').click(function () {
        if ($("#FilterForm").valid()) {
            LoadGrievanceListForSrrda($("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlYear").val());
        }
    });

    $('#btnGrievanceListForPiu').click(function () {
        if ($("#FilterForm").valid()) {
            LoadGrievanceListForPiu($("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlYear").val());
        }
    });

    $("#ddlState").change(function () {
        $("#ddlDistrict").empty();
        $.ajax({
            url: '/ContractorGrievances/ContractorGrievances/PopulateDistrictListContractor',
            type: 'GET',
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

    $('#btnForward').click(function (evt) {
        evt.preventDefault();
        debugger;

        if ($('#frmForwardGrievance').valid()) {
            if (checkFilevalidation("add")) {
                var formdata = new FormData(document.getElementById("frmForwardGrievance"));
                $.ajax({
                    url: '/ContractorGrievances/ContractorGrievances/SaveGrievanceTrackingBySrrda',
                    type: "POST",
                    cache: false,
                    data: formdata,
                    //dataType: 'json',
                    contentType: false,
                    processData: false,
                    //contentType: 'application/json; charset=utf-8',
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.Success) {
                            alert("This Grievance has been Forwarded to PIU.");
                            //ResetForm();
                            CloseForwardGrievenceForm();
                            unblockPage();
                            LoadGrievanceListForSrrda(0, 0, 0);
                            //LoadGrievanceListForSrrda($("#ddlState").val(), $("ddlDistrict").val(), $("ddlYear").val());
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html(response.ErrorMessage);
                            $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                            unblockPage();
                        }
                    }
                });
            }
        }
        else {
            return false;
        }
    });

    $('#btnSubmit').click(function (evt) {
        evt.preventDefault();
        debugger;

        if ($('#frmForwardGrievance').valid()) {
            if (checkFilevalidation("add")) {
                var formdata = new FormData(document.getElementById("frmForwardGrievance"));
                $.ajax({
                    url: '/ContractorGrievances/ContractorGrievances/SavePIUactionOnGrievance',
                    type: "POST",
                    cache: false,
                    data: formdata,
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        Alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        unblockPage();
                        if (response.Success) {
                            alert("Details are submitted successfully");
                            //ResetForm();
                            CloseForwardGrievenceForm();
                            unblockPage();
                            LoadGrievanceListForPiu(0, 0, 0);
                            //LoadGrievanceListForPiu($("#ddlState").val(), $("ddlDistrict").val(), $("ddlYear").val());
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html(response.ErrorMessage);
                            $('#mainDiv').animate({ scrollTop: 0 }, 'slow');
                            unblockPage();
                        }
                    }
                });
            }
        }
        else {
            return false;
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordion2").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function CloseForwardGrievenceForm() {
    $('#accordion').hide('slow');
    $('#accordion2').hide('slow');
    $("#tbDownloadGrievanceFileList").jqGrid('setGridState', 'hidden');
    $('#divForwardGrievanceForm').hide('slow');
    $("#tbTrackGrievanceList").jqGrid('setGridState', 'visible');
    $('#divFilterForm').show('slow');
}

function LoadGrievanceListForSrrda(state, district, year) {
    if (isGridLoaded) {
        $("#tbTrackGrievanceList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbTrackGrievanceList").jqGrid({
        url: '/ContractorGrievances/ContractorGrievances/GetGrievanceListForSrrdaPiu',
        datatype: "json",
        mtype: "POST",
        colNames: ['Grievance ID', 'Contractor Firm Name', 'Submitted On', 'Reopened', 'Agreement Number', 'Road Name', 'State', 'District', 'Year', 'Type', 'Category', 'Status', 'Forward To PIU', 'Action Taken By PIU', 'Close Grievance'],
        colModel: [
                            { name: 'GrievanceID', index: 'GrievanceID', width: 80, align: 'left', sortable: false },
                            { name: 'ContractorFirmName', index: 'ContractorFirmName', width: 80, align: 'left', sortable: false },
                            { name: 'SubmittedOn', index: 'SubmittedOn', width: 80, align: 'left', sortable: true },
                            { name: 'Reopened', index: 'Reopened', width: 80, align: 'center', sortable: false },
                            { name: 'AgreementNumber', index: 'Agreement Number', width: 80, align: 'left', sortable: false },
                            { name: 'RoadName', index: 'RoadName', width: 80, align: 'left', sortable: false },
                            { name: 'State', index: 'State', width: 80, align: 'left', sortable: false },
                            { name: 'District', index: 'District', width: 80, align: 'left', sortable: false },
                            { name: 'Year', index: 'Year', width: 80, align: 'left', sortable: false },
                            { name: 'Type', index: 'Type', width: 80, align: 'left', sortable: false },
                            { name: 'Category', index: 'Category', width: 80, align: 'left', sortable: false },
                            { name: 'Status', index: 'Status', width: 80, align: 'left', sortable: false },
                            { name: 'ForwardToPiu', index: 'ForwardToPiu', width: 80, align: 'center', sortable: false },
                            { name: 'ActionTakenByPiu', index: 'ActionTakenByPiu', width: 80, align: 'center', sortable: false },
                            { name: 'CloseGrievance', index: 'CloseGrievance', width: 80, align: 'center', sortable: false },
        ],
        pager: jQuery('#dvTrackGrievanceListPager'),
        rowNum: 10,
        postData: {
            'state': state,
            'district': district,
            'package_year': year,
            'login': "srrda"
        },
        altRows: false,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AgreementDate',
        sortorder: "desc",
        caption: "Grievance List",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function (data) {
            isGridLoaded = true;
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();
        }

    }); //end of documents grid

}

function LoadGrievanceListForPiu(state, district, year) {
    if (isGridLoaded) {
        $("#tbTrackGrievanceList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbTrackGrievanceList").jqGrid({
        url: '/ContractorGrievances/ContractorGrievances/GetGrievanceListForSrrdaPiu',
        datatype: "json",
        mtype: "POST",
        colNames: ['Grievance ID', 'Contractor Firm Name', 'Submitted On', 'Reopened', 'Agreement Number', 'Road Name', 'State', 'District', 'Year', 'Type', 'Category', 'Status', 'Take Action', 'Finalize'],
        colModel: [
                            { name: 'GrievanceID', index: 'GrievanceID', width: 80, align: 'left', sortable: false },
                            { name: 'ContractorFirmName', index: 'ContractorFirmName', width: 80, align: 'left', sortable: false },
                            { name: 'SubmittedOn', index: 'SubmittedOn', width: 80, align: 'left', sortable: true },
                            { name: 'Reopened', index: 'Reopened', width: 80, align: 'center', sortable: false },
                            { name: 'AgreementNumber', index: 'Agreement Number', width: 80, align: 'left', sortable: false },
                            { name: 'RoadName', index: 'RoadName', width: 80, align: 'left', sortable: false },
                            { name: 'State', index: 'State', width: 80, align: 'left', sortable: false },
                            { name: 'District', index: 'District', width: 80, align: 'left', sortable: false },
                            { name: 'Year', index: 'Year', width: 80, align: 'left', sortable: false },
                            { name: 'Type', index: 'Type', width: 80, align: 'left', sortable: false },
                            { name: 'Category', index: 'Category', width: 80, align: 'left', sortable: false },
                            { name: 'Status', index: 'Status', width: 80, align: 'left', sortable: false },
                            { name: 'Take Action', index: 'Take Action', width: 80, align: 'center', sortable: false },
                            { name: 'Finalize', index: 'Finalize', width: 80, align: 'center', sortable: false },
                            //{ name: 'CloseGrievance', index: 'CloseGrievance', width: 80, align: 'center', sortable: false },
        ],
        pager: jQuery('#dvTrackGrievanceListPager'),
        rowNum: 10,
        postData: {
            'state': state,
            'district': district,
            'package_year': year,
            'login': "piu"
        },
        altRows: false,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AgreementDate',
        sortorder: "desc",
        caption: "Grievance List",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function (data) {
            isGridLoaded = true;
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();
        }

    }); //end of documents grid

}

function ForwardGrievanceToPiu(cellvalue) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Forward Grievance To Piu</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseForwardGrievenceForm();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divForwardGrievanceForm").load('/ContractorGrievances/ForwardGrievanceToPiuForm/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divForwardGrievanceForm'));

        });

        $('#divForwardGrievanceForm').show('slow');
        $("#divForwardGrievanceForm").css('height', 'auto');
    });

    $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbDownloadGrievanceFileList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function LoadDownloadFileGrid(detailId, uploadedBy) {
    debugger;
    if (isGridLoaded) {
        $("#tbDownloadGrievanceFileList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbDownloadGrievanceFileList").jqGrid({
        url: '/ContractorGrievances/ContractorGrievances/GetGrievanceFilesList',
        datatype: "json",
        mtype: "POST",
        colNames: ['File Name', 'File Uploaded By', 'File Upload Date', 'Download File'],
        colModel: [
                            { name: 'FileName', index: 'FileName', width: 150, align: 'left', sortable: true },
                            { name: 'FileUploadedBy', index: 'FileUploadedBy', width: 150, align: 'left', sortable: true },
                            { name: 'FileUploadDate', index: 'FileUploadDate', width: 100, align: 'left', sortable: false },
                            { name: 'DownloadFile', index: 'DownloadFile', width: 100, align: "center", sortable: false, formatter: AnchorFormatter },
        ],
        pager: jQuery('#dvDownloadGrievanceFileListPager'),
        rowNum: 10,
        postData: {
            'detailId': detailId,
            'uploadedBy': uploadedBy
        },
        altRows: false,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'GrievanceID',
        sortorder: "desc",
        caption: "Contractor Grievance Files List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function (data) {
            isGridLoaded = true;
            $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');
            $('#idFilterDiv').trigger('click');
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            $.unblockUI();
        }

    }); //end of documents grid

}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    debugger;
    var url = "/ContractorGrievances/DownloadGrievanceFile/" + cellvalue;
    return "<center><table><tr><td style='border:none;cursor:pointer;'><a href='#' title = 'Download Grievance Files' class = 'ui-icon ui-icon-arrowthickstop-1-s' onclick=downloadFileFromAction('" + url + "'); return false;></a></td></tr></table></center>";
}

function checkFilevalidation(operation) {
    debugger;
    var file = $("#grievanceFile").val();
    if (operation == "add") {
        if (file = "" || file == undefined || file.length == 0) {
            alert("Please select a file");
            return false;
        }
    }
    else {
        if (file = "" || file == undefined || file.length == 0) {
            return true;
        }
    }
    var ext = $("#grievanceFile").val().split('.').pop();
    if (ext.toLowerCase() != "pdf" && ext.toLowerCase() != "jpeg" && ext.toLowerCase() != "png" && ext.toLowerCase() != "jpg") {
        alert("only pdf , jpeg, jpg and png file is allowed.");
        $("#grievanceFile").val('');
        return false;
    }
    var fileSizeKb = $("#grievanceFile")[0].files[0].size;
    var fileSizeMb = fileSizeKb / 1048576;
    if (fileSizeMb > 4) {
        alert("File size should be less than or equal to 4 MB.")
        $(this).val('');
        return false;
    }
    return true;
}

function PiuActionOnGrievance(cellvalue) {
    debugger;
    $("#accordion2 div").html("");
    $("#accordion2 h3").html(
            "<a href='#' style= 'font-size:.9em;' >Piu Action On Grievance</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseForwardGrievenceForm();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion2').show('slow', function () {

        $("#divPiuActionOnGrievanceForm").load('/ContractorGrievances/PiuActionOnGrievanceForm/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divPiuActionOnGrievanceForm'));

        });

        $('#divPiuActionOnGrievanceForm').show('slow');
        $("#divPiuActionOnGrievanceForm").css('height', 'auto');
    });

    $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbDownloadGrievanceFileList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function FinalizeGrievanceTracking(cellvalue) {
    if (confirm("Are you sure you want to Finalize?")) {
        $.ajax({
            url: "/ContractorGrievances/FinalizeGrievanceTrackingByPIU/" + cellvalue,
            type: "POST",
            dataType: "json",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    LoadGrievanceListForPiu(0, 0, 0);
                    //LoadGrievanceListForPiu($("#ddlState").val(), $("ddlDistrict").val(), $("ddlYear").val());
                }
                else {
                    if (data.message != "") {
                        $('#errmessage').html(data.message);
                        $('#dvErrorMessage').show('slow');
                    }
                }
            }
        });
    }
}

function CloseGrievance(cellvalue) {
    if (confirm("Are you sure you want to close this grievance?")) {
        $.ajax({
            url: "/ContractorGrievances/CloseGrievance/" + cellvalue,
            type: "POST",
            dataType: "json",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    LoadGrievanceListForPiu(0, 0, 0);
                    //LoadGrievanceListForPiu($("#ddlState").val(), $("ddlDistrict").val(), $("ddlYear").val());
                }
                else {
                    if (data.message != "") {
                        alert(data.message);
                        $('#errmessage').html(data.message);
                        $('#dvErrorMessage').show('slow');
                    }
                }
            }
        });
    }
}