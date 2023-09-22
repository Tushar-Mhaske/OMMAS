$(document).ready(function () {

    $("#ddlStates").change(function () {

        if ($("#ddlStates").val() > 0) {

            $("#ddlDistrict").empty();

            $.ajax({
                url: '/QualityMonitoring/GetDistrictsByNRIDA',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { stateCode: $("#ddlStates").val() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

    $("#ddlDistrict").change(function () {

        $.ajax({
            url: '/Proposal/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { districtCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                $("#ddlBlock").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });

    });

    $("#btnListDetails").click(function () {

        if ($("#searchExecution").valid()) {

            if ($('#RoleCode').val() == 25 || $('#RoleCode').val() == 5 || $('#RoleCode').val() == 9)
                stateCode = $('#ddlStates option:selected').val();
            else if ($('#RoleCode').val() == 2)
                stateCode = $('#ddlState option:selected').val();
            districtCode = $('#ddlDistrict option:selected').val();
            blockCode = $('#ddlBlock option:selected').val();
            yearCode = $("#ddlYear option:selected").val();
            batch = $('#ddlBatch option:selected').val();
            scheme = $('#ddlScheme option:selected').val();
            proposalType = $('#ddlPropType option:selected').val();
            roleCode = $('#RoleCode').val();

            LoadExecutionGrid(stateCode, districtCode, yearCode, blockCode, yearCode, batch, scheme, proposalType, roleCode);

        }
    });

    $("#spCollapseIconCN").click(function () {

        $('#tbExecutionList').trigger('reloadGrid');
        $('#gview_tbExecutionList .ui-jqgrid-titlebar-close>span').trigger('click');
    });
});

$('#dateOfInsp').datepicker({
    dateFormat: 'dd/mm/yy',
    showOn: "button",
    buttonImage: "/Content/Images/calendar_2.png",
    buttonImageOnly: true,
    maxDate: "+0D",
    changeMonth: true,
    changeYear: true,
    buttonText: 'Inspection Date',
    onSelect: function (selectedDate) {
        jQuery.validator.methods["date"] = function (value, element) { return true; }
        $('#dateOfInsp').trigger('blur');
    },
});

//$("#idFilterDiv").click(function () {
//    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-n").toggleClass("ui-icon-circle-triangle-s");
//    $("#searchExecution").toggle("slow");
//});

// Display main grid
function LoadExecutionGrid(stateCode, districtCode, yearCode, blockCode, yearCode, batch, scheme, proposalType, roleCode) {

    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/QualityMonitoring/GetInspRoadList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { stateCode: stateCode, districtCode: districtCode, yearCode: yearCode, blockCode: blockCode, yearCode: yearCode, batch: batch, scheme: scheme, proposalType: proposalType },
        colNames: ['IMS_PR_ROAD_CODE', 'State', 'District', 'Block', 'Package No.', 'Sanctioned Year', 'Scheme', 'Batch', 'Proposal Type', 'Work Name', 'Total Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(in Mtrs)', (roleCode == 25 || roleCode == 5 || roleCode == 9) ? 'Upload/View Inspection Report' : 'Upload ATR', (roleCode == 25 || roleCode == 5 || roleCode == 9) ? 'View uploaded ATR' : "View uploaded inspection report", 'Color'],
        colModel: [
            { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', height: 'auto', width: 80, align: "center", search: false, hidden: true },
            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center", search: false },
            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center", search: false },
            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center", search: false },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
            { name: 'SCHEME', index: 'SCHEME', width: 70, sortable: true, align: "center" },
            { name: 'BATCH', index: 'BATCH', width: 70, sortable: true, align: "center" },
            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 80, align: "center", search: false },
            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: false },
            { name: 'TOTAL_COST', index: 'TOTAL_COST', height: 'auto', width: 100, align: "right", search: false },
            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: false },
            { name: 'UPLOAD', index: 'UPLOAD', width: 70, align: "center", resize: false, sortable: false, resizable: false },
            { name: 'VIEW', index: 'VIEW', width: 70, align: "center", resize: false, sortable: false, resizable: false },
            { name: 'COLORFLAG', index: 'COLORFLAG', width: 70, sortable: false, align: "center", search: false, hidden: true },
        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "IMS_PR_ROAD_CODE",
        sortorder: "asc",
        caption: (roleCode == 25 || roleCode == 5 || roleCode == 9) ? "&nbsp;&nbsp; Upload Inspection Report" : "&nbsp;&nbsp; Upload ATR",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
            var selectedRowId = $('#tbExecutionList').jqGrid('getGridParam', 'selrow');
            road_code = $('#tbExecutionList').jqGrid('getCell', selectedRowId, 'IMS_PR_ROAD_CODE');
        },
        loadComplete: function (data) {
            $("#tbExecutionList #pagerExecution").css({ height: '40px' });
            $("#pagerExecution_left").html("<label style='margin-left:8%; font-size: 120%;'><b>Note: </b>If Inspection Report Grade is 'S', then no ATR to be uploaded.<label/>");

            var ids = jQuery("#tbExecutionList").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var curretRowId = ids[i];
                var rowData = jQuery("#tbExecutionList").getRowData(curretRowId);
                var colorflag = rowData['COLORFLAG'];

                if (colorflag == "Y") {
                    $('#' + ids[i]).find("td").css("background-color", "#fdebb4");
                }

            }

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        },
        onHeaderClick: function () {
            $('#dvInspDetails').hide('slow');
            $('#dvAddInspDetails').hide('slow');
            $('#divListExecutionPDF').hide('slow');

            $('#divListExecutionUploaded').hide();
        }
    });

}

// Displays Uploaded Inspection Report to view self uploaded data 
function AddIRforMORD(urlparameter) {

    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#divAddInspByNRIDA').show('slow');

    $('#divAddInspByNRIDA').load("/QualityMonitoring/AddInspByNRIDA?idtemp=" + urlparameter, function () {
        $('#tbExecutionList').trigger('reloadGrid');
        $('#inspOperation').html('&nbsp;&nbsp;Add Inspection Details');

        $('#divListExecutionPDF').append(function () {

            jQuery("#tbExecutionListPDF").jqGrid('GridUnload');
            jQuery("#tbExecutionListPDF").jqGrid({
                url: "/QualityMonitoring/GetInspByNRIDADetailsList?idtemp=" + urlparameter,
                datatype: "json",
                mtype: "POST",
                async: false,
                cache: false,
                colNames: ['Inspection Id', 'Road Code', 'State', 'District', 'Block', 'Year', 'Scheme', 'Batch', 'Package No.', 'Proposal Type', 'Road Name', 'Inspecting Officer Name (Designation)', 'Grade', 'Inspection Date', 'Accepted/Rejected', 'Remark', 'View', 'Delete', 'Finalize'],
                colModel: [

                    { name: 'INSPECTION_ID', index: 'INSPECTION_ID', height: 'auto', width: 80, align: "center", hidden: true },
                    { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', height: 'auto', width: 80, align: "center", hidden: true },
                    { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
                    { name: 'SCHEME', index: 'SCHEME', width: 70, sortable: true, align: "center" },
                    { name: 'BATCH', index: 'BATCH', width: 70, sortable: true, align: "center" },
                    { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
                    { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
                    { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },
                    { name: 'NAME', index: 'NAME', height: 'auto', width: 150, align: "center" },
                    { name: 'GRADE', index: 'GRADE', height: 'auto', width: 50, align: "center" },
                    { name: 'INSPECTION_DATE', index: 'INSPECTION_DATE', height: 'auto', width: 80, align: "center" },
                    { name: 'ACCEPTED', index: 'ACCEPTED', height: 'auto', width: 80, align: "center", hidden: true },
                    { name: 'REMARK', index: 'REMARK', height: 'auto', width: 100, align: "center", hidden: true },
                    { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
                    { name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center" },
                    { name: 'Finalize', index: 'Finalize', height: 'auto', width: 80, align: "center" },
                ],
                pager: jQuery('#pagerExecutionPDF').width(20),
                rowNum: 10,
                rowList: [100, 200, 300],
                viewrecords: true,
                recordtext: '{2} records found',
                sortname: "INSPECTION_ID",
                sortorder: "asc",
                caption: "&nbsp;&nbsp; Uploaded InspectionReport Details",
                height: 'auto',
                autowidth: true,
                hidegrid: true,
                rownumbers: true,
                cmTemplate: { title: false },
                loadError: function (xhr, ststus, error) {

                    if (xhr.responseText == "session expired") {
                        alert(xhr.responseText);
                        window.location.href = "/Login/Login";
                    }
                    else {
                        alert("Invalid data.Please check and Try again!")
                    }
                }
            });

        });
        $.unblockUI();
    });
}

// Displays Uploaded ATR to view self uploaded data 
function AddATRforState(urlparameter) {

    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#divAddInspByNRIDA').show('slow');

    $('#divAddInspByNRIDA').load("/QualityMonitoring/AddInspByNRIDA?idtemp=" + urlparameter, function () {

        $('#inspOperation').html('&nbsp;&nbsp;Add Inspection Details');

        $('#divListExecutionPDF').append(function () {

            jQuery("#tbExecutionListPDF").jqGrid('GridUnload');
            jQuery("#tbExecutionListPDF").jqGrid({
                url: "/QualityMonitoring/GetInspByNRIDADetailsList?idtemp=" + urlparameter,
                datatype: "json",
                mtype: "POST",
                async: false,
                cache: false,
                colNames: ['Inspection Id', 'Road Code', 'State', 'District', 'Block', 'Year', 'Scheme', 'Batch', 'Package No.', 'Proposal Type', 'Road Name', 'Inspecting Officer Name (Designation)', 'Grade', 'Uploaded Date', 'Accepted/Rejected', 'Remark', 'View', 'Delete', 'Finalize'],
                colModel: [

                    { name: 'INSPECTION_ID', index: 'INSPECTION_ID', height: 'auto', width: 80, align: "center", hidden: true },
                    { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', height: 'auto', width: 80, align: "center", hidden: true },
                    { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
                    { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
                    { name: 'SCHEME', index: 'SCHEME', width: 70, sortable: true, align: "center" },
                    { name: 'BATCH', index: 'BATCH', width: 70, sortable: true, align: "center" },
                    { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
                    { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
                    { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },
                    { name: 'NAME', index: 'NAME', height: 'auto', width: 150, align: "center", hidden: true },
                    { name: 'GRADE', index: 'GRADE', height: 'auto', width: 50, align: "center", hidden: true },
                    { name: 'UPLOADED_DATE', index: 'UPLOADED_DATE', height: 'auto', width: 80, align: "center" },
                    { name: 'ACCEPTED', index: 'ACCEPTED', height: 'auto', width: 80, align: "center" },
                    { name: 'REMARK', index: 'REMARK', height: 'auto', width: 150, align: "center" },
                    { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
                    { name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center" },
                    { name: 'Finalize', index: 'Finalize', height: 'auto', width: 80, align: "center" },
                ],
                pager: jQuery('#pagerExecutionPDF').width(20),
                rowNum: 10,
                rowList: [100, 200, 300],
                viewrecords: true,
                recordtext: '{2} records found',
                sortname: "INSPECTION_ID",
                sortorder: "asc",
                caption: "&nbsp;&nbsp; Uploaded ATR Details",
                height: 'auto',
                autowidth: true,
                hidegrid: true,
                rownumbers: true,
                cmTemplate: { title: false },
                loadError: function (xhr, ststus, error) {

                    if (xhr.responseText == "session expired") {
                        alert(xhr.responseText);
                        window.location.href = "/Login/Login";
                    }
                    else {
                        alert("Invalid data.Please check and Try again!")
                    }
                }
            });

        });
        $.unblockUI();
    });
}

// View uploaded Inspection Report Data for state login
function LoadUploadedStateList(urlparameter) {

    $("#divListExecutionUploaded").show('slow');
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');

    jQuery("#tbExecutionListUploaded").jqGrid('GridUnload');
    jQuery("#tbExecutionListUploaded").jqGrid({
        url: "/QualityMonitoring/GetInspUploadedDetailsList?idtemp=" + urlparameter,
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        colNames: ['Inspection Id', 'Road Code', 'State', 'District', 'Block', 'Year', 'Package No.', 'Proposal Type', 'Work Name', 'Inspecting Officer Name (Designation)', 'Grade', 'Inspection Date', 'View Inspection Report'],
        colModel: [

            { name: 'INSPECTION_ID', index: 'INSPECTION_ID', height: 'auto', width: 80, align: "center", hidden: true },
            { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', height: 'auto', width: 80, align: "center", hidden: true },
            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 150, align: "center" },
            { name: 'GRADE', index: 'GRADE', height: 'auto', width: 50, align: "center" },
            { name: 'FILE_UPLOADED_DATE', index: 'FILE_UPLOADED_DATE', height: 'auto', width: 80, align: "center" },
            { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
        ],
        pager: jQuery('#pagerExecutionUploaded').width(20),
        rowNum: 10,
        rowList: [100, 200, 300],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "INSPECTION_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Uploaded Inspection Report Details",
        height: 'auto',
        autowidth: true,
        shrinkToFit: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbExecutionListUploaded").setGridWidth(1800);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}

// View uploaded ATR Data for Mord, cqc and cqcadmin login
function LoadUploadedList(urlparameter) {

    $("#divListExecutionUploaded").show('slow');
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');

    jQuery("#tbExecutionListUploaded").jqGrid('GridUnload');
    jQuery("#tbExecutionListUploaded").jqGrid({
        url: "/QualityMonitoring/GetInspUploadedDetailsList?idtemp=" + urlparameter,
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        colNames: ['Inspection Id', 'Road Code', 'State', 'District', 'Block', 'Year', 'Package No.', 'Proposal Type', 'Work Name', 'Inspecting Officer Name (Designation)', 'Grade', 'Uploaded Date', 'View ATR', 'Accept/Reject', 'Remark', 'Add'],
        colModel: [

            { name: 'INSPECTION_ID', index: 'INSPECTION_ID', height: 'auto', width: 80, align: "center", hidden: true },
            { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', height: 'auto', width: 80, align: "center", hidden: true },
            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 150, align: "center", hidden: true },
            { name: 'GRADE', index: 'GRADE', height: 'auto', width: 50, align: "center", hidden: true },
            { name: 'FILE_UPLOADED_DATE', index: 'FILE_UPLOADED_DATE', height: 'auto', width: 80, align: "center" },
            { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
            { name: 'ACCEPT', index: 'ACCEPT', width: 80, align: "center", search: false, sortable: false, editable: true, formatter: generateRadioBtn },
            { name: 'REMARK', index: 'REMARK', height: 'auto', width: 180, align: "center", search: false, edittype: "text", formatter: generateTextBoxForRemark },
            { name: 'Add', index: 'Add', height: 'auto', width: 80, align: "center", search: false, editable: true, formatter: generateCheckoutBtn }
        ],
        pager: jQuery('#pagerExecutionUploaded').width(20),
        rowNum: 10,
        rowList: [100, 200, 300],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "INSPECTION_ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Uploaded ATR Details",
        height: 'auto',
        autowidth: true,
        shrinkToFit: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbExecutionListUploaded").setGridWidth(1800);
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}

function generateRadioBtn(cellvalue, options, rowObject) {
    //alert("generateRadioBtn, " + cellvalue);
    //myArray = cellvalue.split("$");
    //var inspectionId = myArray[0];
    //var acceptStatus = myArray[1];
    //var urlparameter = myArray[2];
    var status;

    if (cellvalue == "R")
        status = "Rejected";
    else if (cellvalue == "A")
        status = "Accepted";

    if (cellvalue == "R" || cellvalue == "A") {
        return status;
    }
    else {
        //return '<button onclick="return getTextBoxValueA(' + inspectionId + ')">Accept</button>' + " " + '<button onclick="return getTextBoxValueR(' + inspectionId  + ')">Reject</button>';
        return '<input checked="true" type="radio" id="accept" name="status_val" value="A"> <label for="accept">ACCEPT</label>' + " " + '<input type="radio" id="reject" name="status_val" value="R"> <label for="reject">REJECT</label>';
    }

}

function generateTextBoxForRemark(cellvalue, options, rowObject) {
    //alert("generateTextBoxForRemark, " + cellvalue);
    myArray = cellvalue.split("$");
    var isAccepted = myArray[0];
    var remark = myArray[1];

    if (isAccepted == "R" || isAccepted == "A") {
        if (remark == "")
            return "--";
        else
            return remark;
    }
    else
        return "<textarea size=\"10\" maxlength=\"50\" id=\"textboxRem\"></textarea>";
}

function generateCheckoutBtn(cellvalue, options, rowObject) {
    //alert("generateCheckoutBtn, " + cellvalue)
    myArray = cellvalue.split("$");
    var inspId = myArray[0];
    var isAccepted = myArray[1];

    if (isAccepted == "R" || isAccepted == "A")
        return "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>";
    else
        return '<button onclick="return getTextBoxValue(' + inspId + ')">Submit</button>';
}

function getTextBoxValue(inspId) {

    var radioVal = $('input[name=status_val]:checked').val();
    var remark = $("#textboxRem").val();
    var message;

    //alert("in getTextBoxValue");
    //alert("inspId : " + inspId);
    //alert("radioVal : " + radioVal);
    //alert("remark : " + remark);

    if (radioVal == "R")
        message = "Do you want to reject ATR ?";
    else
        message = "Do you want to accept ATR ?";

    if (confirm(message)) {
        $.ajax({
            url: "/QualityMonitoring/AcceptOrRejectATR",
            type: "POST",
            async: false,
            cache: false,
            data: { inspId: inspId, acceptedStatus: radioVal, remark: remark },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#tbExecutionListUploaded").trigger('reloadGrid');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }

}

function getTextBoxValueA(inspId) {

    var acceptedStatus = "A";
    if (confirm("Do you want to accept the ATR ?")) {
        $.ajax({
            url: "/QualityMonitoring/AcceptOrRejectATR",
            type: "POST",
            async: false,
            cache: false,
            data: { inspId: inspId, acceptedStatus: acceptedStatus },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#tbExecutionListUploaded").trigger('reloadGrid');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
}

function getTextBoxValueR(inspId) {

    var acceptedStatus = "R";
    if (confirm("Do you want to reject the ATR ?")) {
        $.ajax({
            url: "/QualityMonitoring/AcceptOrRejectATR",
            type: "POST",
            async: false,
            cache: false,
            data: { inspId: inspId, acceptedStatus: acceptedStatus },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#tbExecutionListUploaded").trigger('reloadGrid');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("error");
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
}

$('.btnAddInspDetails').click(function () {

    if (confirm("Are you sure to add details ?")) {
        var form = $('#formAddInspDetails');
        var formadata = new FormData(form.get(0));
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]
        formadata.append("BGFile", FileBG);
        formadata.append("UploadedDate", $("#txtUploaddate").val());

        if ($('.btnAddInspDetails').valid()) {
            if ($("#formAddInspDetails").valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/AddInspByNRIDADetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        alert(response.message);
                        $("#formAddInspDetails")[0].reset();
                        if (response.success) {
                            $("#formAddInspDetails")[0].reset();
                            $('#tbExecutionListPDF').trigger('reloadGrid');
                            $('#tbExecutionList').trigger('reloadGrid');
                        }
                        if (response.file == false)
                            $('#BGFile').val('');
                        $.unblockUI();

                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error");
                        return false;
                    },
                });
            }
        }
    }
    else
        return false;

});

function DeleteFile(urlparameter) {

    if (confirm("Do you want to delete the file ?")) {
        $.ajax({
            url: "/QualityMonitoring/DeleteInspByNRIDAPdf?id=" + urlparameter,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $("#formAddInspDetails").show('slow');
                    $('#tbExecutionListPDF').trigger('reloadGrid'); //LoadExecutionGrid
                    $('#tbExecutionList').trigger('reloadGrid');
                }
            },
            error: function () {
                $.unblockUI();
                alert("An Error occured, please try again.");
                return false;
            },
        });
    }
    else {
        $.unblockUI();
    }
}

function Finalize(urlparameter) {


    myArray = urlparameter.split("$");
    var inspectionId = myArray[0];
    var roleCode = myArray[1];

    if (confirm("Do you want to finalize the changes ?")) {
        $.ajax({
            url: "/QualityMonitoring/FinalizeInspByNRIDAPDetails?id=" + inspectionId,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    //if (roleCode == 25) {
                    /* alert("in 25");*/
                    $("#formAddInspDetails").hide('slow')
                    $('#tbExecutionListPDF').trigger('reloadGrid');
                    $('#divAddInspByNRIDA').hide('slow');

                    $("#tbExecutionList").trigger('reloadGrid');
                    $('#divListExecution').show('slow');
                    $("#tbExecutionList").jqGrid('setGridState', 'visible');
                    //}
                    //else if (roleCode == 2) {
                    //    alert("in 2");
                    //    $("#formAddInspDetails").show('slow')
                    //    $('#tbExecutionListPDF').trigger('reloadGrid');

                    //    $("#tbExecutionList").trigger('reloadGrid');
                    //}

                    //$('#dvInspDetails').hide('slow');
                    //$('#dvAddInspDetails').hide('slow');
                    //$('#divListExecutionPDF').hide('slow');
                    //$('#divListExecutionUploaded').hide();
                }
            },
            error: function () {
                $.unblockUI();
                alert("An Error occured, please try again.");
                return false;
            },
        });
    }
    else {
        $.unblockUI();
    }
}

