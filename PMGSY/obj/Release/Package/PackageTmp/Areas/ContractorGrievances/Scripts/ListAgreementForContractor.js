$(document).ready(function () {

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    //LoadAgreementList($("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlYear").val());
    isGridLoaded = true;

    $('#btnList').click(function () {
        if ($("#FilterForm").valid()) {
            LoadAgreementList($("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlYear").val());
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

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function LoadAgreementList(state, district, year) {
    if (isGridLoaded) {
        $("#tbRegisterGrievanceList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbRegisterGrievanceList").jqGrid({
        url: '/ContractorGrievances/ContractorGrievances/GetContractorAgreementList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Agreement Number', 'State', 'District', 'Agreement Date', 'Agreement Amount', 'Package Number', 'Road Name', 'Register Grievances', 'Track Grievances Status'],
        colModel: [
                            { name: 'AgreementNumber', index: 'AgreementNumber', width: 80, align: 'left', sortable: false },
                            { name: 'State', index: 'State', width: 80, align: 'left', sortable: true },
                            { name: 'District', index: 'District', width: 80, align: 'left', sortable: false },
                            { name: 'AgreementDate', index: 'AgreementDate', width: 80, align: 'left', sortable: false },
                            { name: 'AgreementAmount', index: 'AgreementAmount', width: 80, align: 'right', sortable: false },
                            { name: 'PackageNumber', index: 'PackageNumber', width: 80, align: 'left', sortable: false },
                            { name: 'RoadName', index: 'RoadName', width: 80, align: 'left', sortable: false },
                            { name: 'RegisterGrievances', index: 'RegisterGrievances', width: 80, align: 'center', sortable: false },
                            { name: 'TrackGrievancesStatus', index: 'TrackGrievancesStatus', width: 80, align: 'center', sortable: false }
        ],
        pager: jQuery('#dvRegisterGrievanceListPager'),
        rowNum: 10,
        postData: {
            'state': state,
            'district': district,
            'year': year
        },
        altRows: false,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'AgreementDate',
        sortorder: "desc",
        caption: "Work List",
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

function CloseRegisterGrievenceForm() {
    $('#accordion').hide('slow');
    $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbDownloadGrievanceFileList").jqGrid('setGridState', 'hidden');
    $('#divRegisterGrievanceListForm').hide('slow');
    $("#tbRegisterGrievanceList").jqGrid('setGridState', 'visible');
    $('#divFilterForm').show('slow');
}

function RegisterGrievance(cellvalue) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Register Grievances</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseRegisterGrievenceForm();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divRegisterGrievanceListForm").load('/ContractorGrievances/RegisterGrievanceForm/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divRegisterGrievanceListForm'));

        });

        $('#divRegisterGrievanceListForm').show('slow');
        $("#divRegisterGrievanceListForm").css('height', 'auto');
    });

    $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbDownloadGrievanceFileList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function TrackGrievance(roadCode) {
    if (isGridLoaded) {
        $("#tbTrackGrievanceList").GridUnload();
        isGridLoaded = false;
    }
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbTrackGrievanceList").jqGrid({
        url: '/ContractorGrievances/ContractorGrievances/GetContractorGrievanceList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Grievance ID', 'Submitted On', 'Agreement Number', 'Type', 'Category', 'Status', 'Edit / View Details', 'Finalize Details', 'Download File'],
        colModel: [
                            { name: 'GrievanceID', index: 'GrievanceID', width: 80, align: 'left', sortable: false },
                            { name: 'SubmittedOn', index: 'SubmittedOn', width: 80, align: 'left', sortable: true },
                            { name: 'Agreement Number', index: 'Agreement Number', width: 80, align: 'left', sortable: false },
                            { name: 'Type', index: 'Type', width: 80, align: 'left', sortable: false },
                            { name: 'Category', index: 'Category', width: 80, align: 'left', sortable: false },
                            { name: 'Status', index: 'Status', width: 80, align: 'left', sortable: false },
                            { name: 'EditViewDetails', index: 'EditViewDetails', width: 80, align: 'center', sortable: false },
                            { name: 'FinalizeDetails', index: 'FinalizeDetails', width: 80, align: 'center', sortable: false },
                            { name: 'DownloadFile', index: 'DownloadFile', width: 80, align: "center", sortable: false /*, formatter: AnchorFormatter*/ },
        ],
        pager: jQuery('#dvTrackGrievanceListPager'),
        rowNum: 10,
        postData: {
            'roadCode': roadCode
        },
        altRows: false,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'GrievanceID',
        sortorder: "desc",
        caption: "Contractor Grievances List",
        height: 'auto',
        //width: '1080px',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function (data) {
            isGridLoaded = true;
            $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');
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

function ViewEditGrievance(cellvalue) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Register Grievances</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseRegisterGrievenceForm();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divRegisterGrievanceListForm").load('/ContractorGrievances/ViewEditGrievanceForm/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divRegisterGrievanceListForm'));

        });

        $('#divRegisterGrievanceListForm').show('slow');
        $("#divRegisterGrievanceListForm").css('height', 'auto');
    });

    $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');
    $("#tbTrackGrievanceList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function FinalizeGrievance(cellvalue) {
    if (confirm("Are you sure you want to Finalize?")) {
        $.ajax({
            url: "/ContractorGrievances/FinalizeContractorGrievance/" + cellvalue,
            type: "POST",
            dataType: "json",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    TrackGrievance(data.roadCode);
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

function LoadDownloadFileGrid(detailId) {
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
            'detailId': detailId
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
            $("#tbRegisterGrievanceList").jqGrid('setGridState', 'hidden');
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