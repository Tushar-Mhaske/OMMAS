$(document).ready(function () {
    $("#closeErrorSummary").click(function () {
        if (confirm('Are you sure to close error summary?') == true) {
            $("#errSummary").hide("slow");
        }
    });

    $("#closeErrorSummaryadobedwnldDiv").click(function () {
        if (confirm('Are you sure to close error summary?') == true) {
            $("#adobedwnldDiv").hide("slow");
        }
    });
    $('#ddlstate').change(function () {
        $("#ddldistrict").empty();
        $.ajax({
            url: '/EFORM/PopulateDistrictsbyStateCode',
            type: 'POST',
            data: { stateCode: $("#ddlstate").val(), },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddldistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            },
            error: function (err) {
            }
        });
    });



    $("#btnGetPIUWorkList").click(function () {
        if ($("#frmEformIndex").valid()) {
            LoadExecutionGrid();
        }

    });

    $("#btnGetQMWorkList").click(function () {
        if ($("#frmEformIndex").valid()) {
            LoadQMWorkListGrid();
        }
    });


    $('#ddlStatus').change(function () {
        LoadExecutionGrid();
    });

    $('#ddlStatusQM').change(function () {
        LoadQMWorkListGrid();
    });


    //jqgrid
    function LoadExecutionGrid() {
        $("#adobedwnldDiv1").show();
        $("#adobedwnldDiv").hide();
        $("#errSummary").hide();
        jQuery("#tbWorkList").jqGrid('GridUnload');
        $("#tbWorkList").jqGrid({
            url: "/EFORM/GetWorkList",
            datatype: 'json',
            mtype: 'POST',
            cache: false,
            //add below line on 12-07-2022
            postData: { month: $("#ddlmonth option:selected").val(), year: $("#ddlyear option:selected").val(), stateCode: $("#ddlstate option:selected").val() != undefined ? $("#ddlstate option:selected").val() : $("#ddl1state option:selected").val(), districtCode: $("#ddldistrict option:selected").val() != undefined ? $("#ddldistrict option:selected").val() : $("#ddl1district").val(), eformStatus: $("#ddlStatus option:selected").val(), eFormType: $("#ddleformType option:selected").val() },
            colNames: ['IMS Pr Road Code', "e-Form Id", 'District', 'Block', 'PIU Name', 'Road/bridge', 'Road/bridge Name', "Package Number", "Sanction Year", "Sanctioned Length{Road(Km)/Bridge(m)}", "Executed Length", "Date of Completion", "Work Status", 'Download e-Form', 'Upload e-Form', "Preview Report", 'View e-Form', 'Delete uploaded e-Form', 'Finalize e-Form'],
            colModel: [
                { key: true, hidden: true, name: 'ImsPrRoadCode', index: 'ImsPrRoadCode', editable: true },
                { key: false, name: 'EFORM_ID', index: 'EFORM_ID', editable: true, width: 30, align: "right", resizable: true },
                { name: 'District', index: 'District', width: 50, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'Block', index: 'Block', width: 50, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'PIU_Name', index: 'PIU_Name', width: 60, align: "center", resize: false, sortable: true, resizable: true },
                { key: false, name: 'PROPOSAL_TYPE', index: 'PROPOSAL_TYPE', editable: true, width: 50, align: "center", resizable: true },
                { key: false, name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', editable: true, width: 100, align: "left", resizable: true },
                { key: false, name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', editable: true, width: 60, align: "left", resizable: true },
                { key: false, name: 'IMS_YEAR', index: 'IMS_YEAR', editable: true, width: 35, align: "right", resizable: true },
                { key: false, name: 'SANCTION_length', index: 'SANCTION_length', editable: true, width: 45, align: "right", resizable: true },
                { key: false, name: 'EXECUTED_length', index: 'EXECUTED_length', editable: true, width: 40, align: "right", resizable: true },
                { key: false, name: 'Completion_Date', index: 'Completion_Date', editable: true, width: 60, align: "left", resizable: true },
                { key: false, name: 'Work_Status', index: 'Work_Status', formatter: fontColorFormat, editable: true, width: 60, align: "left", resizable: true },
                { name: 'Downloaded_eForm', index: 'Downloaded_eForm', width: 70, align: "center", resize: false, sortable: false, resizable: true },
                { name: 'Upload_eForm', index: 'Upload_eForm', width: 210, align: "center", search: false, sortable: false, resizable: true },
                { name: 'ViewDetails', index: 'ViewDetails', width: 45, align: "center", search: false, sortable: false, resizable: true },
                { name: 'ViewPDF', index: 'ViewPDF', width: 70, align: "center", search: false, sortable: false, resizable: true },
                { name: 'Delete', index: 'Delete', width: 35, align: "center", search: false, sortable: false, resizable: true },
                { name: 'Finalize', index: 'Finalize', width: 35, align: "center", search: false, sortable: false, resizable: true },

            ],
            pager: jQuery('#pagerExecutionQM').width(20),
            rowNum: 10,
            rowList: [10, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: "EFORM_ID",
            sortorder: "asc",
            caption: "&nbsp;&nbsp; Work List",
            height: 'auto',
            autowidth: true,
            hidegrid: true,
            rownumbers: true,
            cmTemplate: { title: false },
            onSelectRow: function (id) {
                var selectedRowId = $('#tbExecutionList_NQM').jqGrid('getGridParam', 'selrow');
                road_code = $('#tbExecutionList_NQM').jqGrid('getCell', selectedRowId, 'IMS_ROAD_CODE');
            },
            loadComplete: function (data) {

                $("#tbExecutionList_NQM #pagerExecution").css({ height: '40px' });
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
        })



    }

    function fontColorFormat(cellvalue, options, rowObject) {
        var color = "blue";
        var cellHtml = "<span style='color:" + color + ";font-weight:bold' originalValue='" + cellvalue + "'>" + cellvalue + "</span>";
        return cellHtml;
    }


    function LoadQMWorkListGrid() {
        $("#adobedwnldDiv1").show();
        $("#adobedwnldDiv").hide();
        $("#errSummary").hide();
        jQuery("#tbWorkList").jqGrid('GridUnload');
        $("#tbWorkList").jqGrid({
            url: "/EFORM/GetQMWorkList",
            datatype: 'json',
            mtype: 'POST',
            cache: false,
            //add below line on 12-07-2022
            postData: { month: $("#ddlmonth option:selected").val(), year: $("#ddlyear option:selected").val(), stateCode: $("#ddlstate option:selected").val() != undefined ? $("#ddlstate option:selected").val() : $("#ddl1state option:selected").val(), districtCode: $("#ddldistrict option:selected").val() != undefined ? $("#ddldistrict option:selected").val() : $("#ddl1district").val(), eformStatus: $("#ddlStatusQM option:selected").val(), eFormType: $("#ddleformType option:selected").val() },
            colNames: ['IMS Pr Road Code', "e-Form Id", 'District', 'Block', 'QM Name', 'Road/Bridge', 'Road Name', "Package Number", "Sanction Year", "Sanctioned Length{Road(Km)/Bridge(m)}", "Executed Length", "Date of Completion", "Work Status", 'PIU Uploaded e-Form',
                'Download Test Report', 'Upload Test Report', 'View Test Report', 'View Test Report', 'Delete Test Report', 'Finalize Test Report', 'Upload Test Report', 'View Test Report', 'Delete Test Report', 'Finalize Test Report',
                'Download e-Form', 'Upload e-Form', "Preview QM-Report", 'QM Uploaded e-Form', 'Delete uploaded e-Form', 'Finalize e-Form', 'QM Uploaded e-Form', 'View combined e-Form', "Preview Report", "View (Part1+ Part2+ Test Report) pdf"],
            colModel: [
                { key: true, hidden: true, name: 'ImsPrRoadCode', index: 'ImsPrRoadCode', editable: true },
                { key: false, name: 'EFORM_ID', index: 'EFORM_ID', editable: true, width: 30, align: "right", resizable: true },
                { name: 'District', index: 'District', width: 50, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'Block', index: 'Block', width: 50, align: "center", resize: false, sortable: true, resizable: true },
                { name: 'QM_NAME', index: 'QM_NAME', width: 60, align: "center", resize: false, sortable: true, resizable: true },
                { key: false, name: 'PROPOSAL_TYPE', index: 'PROPOSAL_TYPE', editable: true, width: 50, align: "center", resizable: true },
                { key: false, name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', editable: true, width: 100, align: "left", resizable: true },
                { key: false, name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', editable: true, width: 60, align: "left", resizable: true },
                { key: false, name: 'IMS_YEAR', index: 'IMS_YEAR', editable: true, width: 35, align: "right", resizable: true },
                { key: false, name: 'SANCTION_length', index: 'SANCTION_length', editable: true, width: 45, align: "right", resizable: true },
                { key: false, name: 'EXECUTED_length', index: 'EXECUTED_length', editable: true, width: 40, align: "right", resizable: true },
                { key: false, name: 'Completion_Date', index: 'Completion_Date', editable: true, width: 60, align: "left", resizable: true },
                { key: false, name: 'Work_Status', index: 'Work_Status', editable: true, formatter: fontColorFormat, width: 60, align: "left", resizable: true },

                { name: 'ViewPIUPDF', index: 'ViewPIUPDF', width: 45, align: "center", search: false, sortable: false, resizable: true },


                //e - Form TR
                { name: 'Downloaded_eFormTR', index: 'Downloaded_eFormTR', width: 60, align: "center", resize: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },

                { name: 'testreportfile_Upload', index: 'Upload_test_report', width: 150, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'Preview', index: 'Preview', width: 50, align: "center", search: false, sortable: false, resizable: true },
                { name: 'ViewTestReportPDF', index: 'View_Test_Report_PDF', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: true },
                { name: 'Delete', index: 'Deletetr', width: 35, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'FinalizeTR', index: 'Finalize', width: 35, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },



                //ajinkya
                { name: 'testreportfile_UploadScan', index: 'Upload_test_report', width: 210, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'ViewTestReportPDFScan', index: 'View_Test_Report_PDF', width: 50, align: "center", search: false, sortable: false, resizable: true },
                { name: 'DeleteScan', index: 'Deletetr', width: 35, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },

                { name: 'FinalizeTRScan', index: 'Finalize', width: 35, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },





                { name: 'Downloaded_eForm', index: 'Downloaded_eForm1', width: 60, align: "center", resize: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'Upload_eForm', index: 'Upload_eForm', width: 150, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'ViewDetails', index: 'ViewDetails', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'ViewPDF', index: 'ViewPDF', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'Delete', index: 'Delete', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },
                { name: 'Finalize', index: 'Finalize', width: 35, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },

                { name: 'ViewPDF', index: 'ViewPDF', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? false : true) },

                { name: 'ViewCombinePDF', index: 'ViewPDF', width: 50, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? false : true) },
                { name: 'ViewCombineDetails', index: 'ViewDetails', width: 45, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? false : true) },

                { name: 'ViewAllCombinePDF', index: 'ViewAllCombinePDF', width: 65, align: "center", search: false, sortable: false, resizable: true, hidden: (($("#RoleCode").val() == 9 || $("#RoleCode").val() == 5 || $("#RoleCode").val() == 8 || $("#RoleCode").val() == 48 || $("#RoleCode").val() == 69) ? true : false) },


            ],
            pager: jQuery('#pagerExecutionQM').width(20),
            rowNum: 10,
            rowList: [10, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: "EFORM_ID",
            sortorder: "asc",
            caption: "&nbsp;&nbsp; Work List",
            height: 'auto',
            autowidth: true,
            hidegrid: true,
            rownumbers: true,
            cmTemplate: { title: false },
            onSelectRow: function (id) {
                var selectedRowId = $('#tbExecutionList_NQM').jqGrid('getGridParam', 'selrow');
                road_code = $('#tbExecutionList_NQM').jqGrid('getCell', selectedRowId, 'IMS_ROAD_CODE');


            },
            loadComplete: function (data) {

                $("#tbExecutionList_NQM #pagerExecution").css({ height: '40px' });
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
        })

        jQuery("#tbWorkList").jqGrid('setGroupHeaders', {
            useColSpanStyle: true,
            groupHeaders: [
                { startColumnName: 'Downloaded_eForm', numberOfColumns: 6, titleText: '<center><b><span style="font-size:15px;">e-Form Part-2</span></b></center>' },
                { startColumnName: 'Downloaded_eFormTR', numberOfColumns: 6, titleText: '<center><b><span style="font-size:15px;">e-Form Test Report</span></b></center>' },
                { startColumnName: 'testreportfile_UploadScan', numberOfColumns: 4, titleText: '<center><b><span style="font-size:15px;">Test Report(Scan Copy)</span></b></center>' },
                { startColumnName: 'ViewCombinePDF', numberOfColumns: 2, titleText: '<center><b><span style="font-size:15px;">e-Form (Part1+Part2)</span></b></center>' }
                //  { startColumnName: 'ViewAllCombinePDF', numberOfColumns: 1, titleText: '<center><b>e-Form (Part1+Part2+Test Report)</b></center>' }
            ]
        });

    }



});


function viewPart1PdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewPart1PdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function viewPdfData(encId) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/ViewPdfSavedData?encId=" + encId);
}


function deletePdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to delete pdf') == true) {

        $.ajax({
            type: "POST",
            url: '/EFORM/deleteEformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,

            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                }
                else {
                    alert(response.message);
                }
                $('#tbWorkList').trigger('reloadGrid');
            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}


function finalizePdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to finalize the pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/finalizeEformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,

            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record finalized successfully");
                }
                else {
                    alert("Error occured while delete file");
                }
                $('#tbWorkList').trigger('reloadGrid');
            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}




// uploadBridgePIUPdfFile

function uploadBridgePIUPdfFile(id) {
    $("#adobedwnldDiv").hide();
    $("#errSummary").hide();
    //  var fileID = "file" + id.split('$')[0];
    var uplFile = $("#file" + id.split('$')[7]).val();   //change on 08-07-2022
    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();
    var file = document.getElementById("file" + id.split('$')[7]).files[0];   //change on 08-07-2022
    formData.append("MyFile", file);

    $.ajax({
        type: "POST",
        url: '/EFORM/IsPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                $("#file" + id.split('$')[7]).val(null);   //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/UploadBridgePdfFilePIU/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {
                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);
                                $.unblockUI();


                                //added on17-05-2022
                                $.ajax({
                                    type: "POST",
                                    url: '/EFORM/DeleteTempFilePIU/' + id,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    beforeSend: function () {
                                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                                    },
                                    success: function (response) {
                                        if (response.success) {
                                            $('#tbWorkList').trigger('reloadGrid');
                                            $.unblockUI();
                                        }
                                        else {
                                            $.unblockUI();
                                        }
                                    },
                                    error: function (error) {
                                        $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                                    }
                                });


                            }
                            else {
                                if (response.responseMsg == "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.") {
                                    alert("Please Fill the downloaded e - Form Pdf in adobe acrobat reader dc and then upload.");
                                    $("#adobedwnldDiv").show();

                                }
                                else {
                                    alert(response.responseMsg);
                                    $("#errSummary").show();
                                    $("#ErroMsg").empty();
                                    $("#ErroMsg").html(response.data);
                                }


                                $("#file" + id.split('$')[7]).val(null); //change on 21-07-2022
                                $.unblockUI();
                            }
                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);//change on 21-07-2022
                }
            }

        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });

}


function uploadPIUPdfFile(id) {
    $("#adobedwnldDiv").hide();
    $("#errSummary").hide();
    //  var fileID = "file" + id.split('$')[0];
    var uplFile = $("#file" + id.split('$')[7]).val();   //change on 08-07-2022
    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();
    var file = document.getElementById("file" + id.split('$')[7]).files[0];   //change on 08-07-2022
    formData.append("MyFile", file);

    $.ajax({
        type: "POST",
        url: '/EFORM/IsPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                $("#file" + id.split('$')[7]).val(null);   //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/UploadPdfFilePIU/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {
                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);
                                $.unblockUI();


                                //added on17-05-2022
                                $.ajax({
                                    type: "POST",
                                    url: '/EFORM/DeleteTempFilePIU/' + id,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    beforeSend: function () {
                                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                                    },
                                    success: function (response) {
                                        if (response.success) {
                                            $('#tbWorkList').trigger('reloadGrid');
                                            $.unblockUI();
                                        }
                                        else {
                                            $.unblockUI();
                                        }
                                    },
                                    error: function (error) {
                                        $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                                    }
                                });









                            }
                            else {
                                if (response.responseMsg == "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.") {
                                    alert("Please Fill the downloaded e - Form Pdf in adobe acrobat reader dc and then upload.");
                                    $("#adobedwnldDiv").show();

                                }
                                else {
                                    alert(response.responseMsg);
                                    $("#errSummary").show();
                                    $("#ErroMsg").empty();
                                    $("#ErroMsg").html(response.data);
                                }

                                $("#file" + id.split('$')[7]).val(null); //change on 21-07-2022
                                $.unblockUI();
                            }
                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);//change on 21-07-2022
                }
            }



        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });

}



function deleteBridgePIUPdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to delete pdf') == true) {

        $.ajax({
            type: "POST",
            url: '/EFORM/deleteEformBridgePIUDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,

            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                }
                else {
                    alert(response.message);
                }
                $('#tbWorkList').trigger('reloadGrid');
            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}

// added by rohit borse on 29-08-2022
function viewBridgePIUData(encId) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/PreviewBridgePIU_SavedData?encId=" + encId);
}







// uploadBridgeQMPdfFile

function uploadBridgeQMPdfFile(id) {
    $("#adobedwnldDiv").hide();
    $("#errSummary").hide();
    var fileID = "file2" + id.split('$')[0];
    var uplFile = $("#file2" + id.split('$')[7]).val();//change on 08-07-2022
    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();
    var file = document.getElementById("file2" + id.split('$')[7]).files[0];//change on 08-07-2022
    formData.append("MyFile", file);
    $.ajax({
        type: "POST",
        url: '/EFORM/IsQMPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert("Pdf is already uploaded. If you want to upload again, please delete the existing file ");
                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/UploadBRIDGEPdfFileQM/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {

                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
                                $.unblockUI();



                                //added on18-05-2022
                                $.ajax({
                                    type: "POST",
                                    url: '/EFORM/DeleteTempFileQM/' + id,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    beforeSend: function () {
                                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                                    },
                                    success: function (response) {
                                        if (response.success) {
                                            $('#tbWorkList').trigger('reloadGrid');
                                            $.unblockUI();
                                        }
                                        else {
                                            $.unblockUI();
                                        }
                                    },
                                    error: function (error) {
                                        $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                                    }
                                });



                            }
                            else {
                                if (response.responseMsg == "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.") {
                                    alert("Please Fill the downloaded e - Form Pdf in adobe acrobat reader dc and then upload.");
                                    $("#adobedwnldDiv").show();

                                }
                                else {
                                    alert(response.responseMsg);
                                    $("#errSummary").show();
                                    $("#ErroMsg").empty();
                                    $("#ErroMsg").html(response.data);
                                }

                                $("#file" + id.split('$')[7]).val(null);           //change on 21-07-2022
                                $.unblockUI();
                            }
                            $("#fileID").val(null);

                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);  //change on 21-07-2022
                }
            }



        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });
}



function deleteBridgeQMPdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to delete pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/deleteBridgeQMEformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }
                else {
                    alert(response.message);
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}


function viewBridgeQMData(encIdtemp) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/PreviewBridgeQM_SavedData?encIdtemp=" + encIdtemp);
}




/*QM Region*/


function uploadQMPdfFile(id) {
    $("#adobedwnldDiv").hide();
    $("#errSummary").hide();
    var fileID = "file2" + id.split('$')[0];
    // alert($("#file" + id.split('$')[7]).val());
    var uplFile = $("#file2" + id.split('$')[7]).val();//change on 08-07-2022
    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();
    var file = document.getElementById("file2" + id.split('$')[7]).files[0];//change on 08-07-2022
    formData.append("MyFile", file);
    $.ajax({
        type: "POST",
        url: '/EFORM/IsQMPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert("Pdf is already uploaded. If you want to upload again, please delete the existing file ");
                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/UploadPdfFileQM/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {

                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
                                $.unblockUI();



                                //added on18-05-2022
                                $.ajax({
                                    type: "POST",
                                    url: '/EFORM/DeleteTempFileQM/' + id,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    beforeSend: function () {
                                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                                    },
                                    success: function (response) {
                                        if (response.success) {
                                            $('#tbWorkList').trigger('reloadGrid');
                                            $.unblockUI();
                                        }
                                        else {
                                            $.unblockUI();
                                        }
                                    },
                                    error: function (error) {
                                        $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                                    }
                                });



                            }
                            else {
                                if (response.responseMsg == "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.") {
                                    alert("Please Fill the downloaded e - Form Pdf in adobe acrobat reader dc and then upload.");
                                    $("#adobedwnldDiv").show();

                                }
                                else {
                                    alert(response.responseMsg);
                                    $("#errSummary").show();
                                    $("#ErroMsg").empty();
                                    $("#ErroMsg").html(response.data);
                                }

                                $("#file" + id.split('$')[7]).val(null);           //change on 21-07-2022
                                $.unblockUI();
                            }
                            $("#fileID").val(null);

                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);  //change on 21-07-2022
                }
            }



        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });





}

function viewPart2PdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewPart2PdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function viewQMPdfData(encIdtemp) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/ViewQMPdfSavedData?encIdtemp=" + encIdtemp);
}



function viewCombinePdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewCombinePdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function viewCombinedPart_1_2_Pdf(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/isPart1Part2PdfAvail/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open("/EFORM/viewCombinedPdf12/" + id);
            }
            else {
                alert(response.Message);
            }


        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}




function viewCombinedPart_1_2_TR_Pdf(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/isAllPdfAvail/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open("/EFORM/viewCombinedPdf12TR/" + id);
            }
            else {
                alert(response.Message);
            }


        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}




function viewCombinePdfData(eid) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/ViewCombinePdfSavedData?eid=" + eid);
}


function PreviewBridgeCQC(eid) {
    $("#adobedwnldDiv").hide();
    window.open("/EFORM/Preview_BRIDGE_CQC?eid=" + eid);
}

function deleteQMPdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to delete pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/deleteQMEformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }
                else {
                    alert(response.message);
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}


function finalizeQMPdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to finalize the pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/finalizeQMEformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record finalized successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                }
                else {
                    alert(response.responseMsg);
                    $('#tbWorkList').trigger('reloadGrid');
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}
/*End QM Region Here*/




//Ajinkya
function uploadTestRepotPdfFile(id) {

    $("#errSummary").hide();
    // var fileID = "fileScan" + id.split('$')[0];

    var uplFile = $("#file" + id.split('$')[7]).val();//change on 08-07-2022

    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();

    var file = document.getElementById("file" + id.split('$')[7]).files[0];//change on 08-07-2022
    formData.append("MyFile", file);
    $.ajax({
        type: "POST",
        url: '/EFORM/IsTestReportPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert("Test report is already uploaded .To upload again Pdf is already uploaded. If you want to upload again, please delete the existing file ");
                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/uploadTestRepotPdfFile/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {

                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {
                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
                                $.unblockUI();



                            }
                            else {
                                alert(response.responseMsg);
                                $("#errSummary").show();
                                $("#ErroMsg").empty();
                                $("#ErroMsg").html(response.data);
                                $("#file" + id.split('$')[7]).val(null);           //change on 21-07-2022
                                $.unblockUI();
                            }
                            $("#fileID").val(null);

                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);  //change on 21-07-2022
                }
            }



        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });
}

//Ajinkya
function deleteTestReportPdf(id) {

    if (confirm('Do you want to delete pdf') == true) {

        $.ajax({
            type: "POST",
            url: '/EFORM/deleteTestReportPdf/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,

            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                }
                else {
                    alert("Test Report file not found");
                }
                $('#tbWorkList').trigger('reloadGrid');
            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}

//Ajinkya
function ViewTestRepotPdfFile(eid) {
    window.open("/EFORM/ViewTestRepotPdfFile?eid=" + eid);
}


function viewTRScanPdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewTRScanPdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}


function finalizeScanTRPdf(id) {
    if (confirm('Do you want to finalize the pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/finalizeScanTestReport/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record finalized successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                }
                else {
                    alert(response.responseMsg);
                    $('#tbWorkList').trigger('reloadGrid');
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}







//e-Form Test Report
function uploadTRPdfFile(id) {
    $("#adobedwnldDiv").hide();
    $("#errSummary").hide();

    var fileID = "file1" + id.split('$')[0];
    var uplFile = $("#file1" + id.split('$')[7]).val();//change on 08-07-2022
    if (uplFile == '') {
        alert('Please select a file.');
        return false;
    }
    var formData = new FormData();
    var file = document.getElementById("file1" + id.split('$')[7]).files[0];//change on 08-07-2022
    formData.append("MyFile", file);
    $.ajax({
        type: "POST",
        url: '/EFORM/IsTRPDFFileavaialble/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,

        cache: false,
        success: function (response) {
            if (response.success) {
                alert("Pdf is already uploaded. If you want to upload again, please delete the existing file ");
                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
            }
            else {
                if (confirm('Do you want to upload pdf') == true) {
                    $.ajax({
                        type: "POST",
                        url: '/EFORM/UploadTRPdfFile/' + id,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        cache: false,
                        beforeSend: function () {

                            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                        },
                        success: function (response) {

                            if (response.success) {
                                alert("File uploaded successfully.");
                                $('#tbWorkList').trigger('reloadGrid');
                                console.log(response.data);
                                $("#file" + id.split('$')[7]).val(null);    //change on 21-07-2022
                                $.unblockUI();



                                //added on18-05-2022
                                $.ajax({
                                    type: "POST",
                                    url: '/EFORM/DeleteTempFileQM/' + id,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    beforeSend: function () {
                                        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                                    },
                                    success: function (response) {
                                        if (response.success) {
                                            $('#tbWorkList').trigger('reloadGrid');
                                            $.unblockUI();
                                        }
                                        else {
                                            $.unblockUI();
                                        }
                                    },
                                    error: function (error) {
                                        $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                                    }
                                });



                            }
                            else {
                                if (response.responseMsg == "Please Fill the downloaded Test report e-Form Pdf in adobe acrobat reader dc and then upload.") {
                                    alert("Please Fill the downloaded Test report e-Form Pdf in adobe acrobat reader dc and then upload.");
                                    $("#adobedwnldDiv").show();
                                    $("#file1" + id.split('$')[7]).val(null);

                                }
                                else {
                                    alert(response.responseMsg);
                                    $("#errSummary").show();
                                    $("#ErroMsg").empty();
                                    $("#ErroMsg").html(response.data);
                                }

                                $("#file1" + id.split('$')[7]).val(null);           //change on 21-07-2022
                                $.unblockUI();
                            }
                            $("#fileID").val(null);

                        },
                        error: function (error) {
                            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                        }
                    });
                }
                else {

                    $("#file" + id.split('$')[7]).val(null);  //change on 21-07-2022
                }
            }



        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
        }
    });





}


function deleteTRPdf(id) {
    $("#adobedwnldDiv").hide();
    if (confirm('Do you want to delete pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/deleteTREformDetail/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record deleted successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }
                else {
                    alert(response.message);
                    $('#tbWorkList').trigger('reloadGrid');
                    LoadQMWorkListGrid();
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}


function previewTestReport(encIdtemp) {
    window.open("/EFORM/TestReportPreview?encIdtemp=" + encIdtemp);
}

function viewTRPdfVirtualDir(id) {
    $("#adobedwnldDiv").hide();


    $.ajax({
        type: "POST",
        url: '/EFORM/viewTRPdfVirtualDir/' + id,
        dataType: 'json',
        contentType: false,
        processData: false,
        cache: false,
        success: function (response) {
            if (response.success) {
                window.open(response.Message);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (error) {
            $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
            $('#tbWorkList').trigger('reloadGrid');
        }


    });
}

function finalizeETRPdf(id) {
    if (confirm('Do you want to finalize the pdf') == true) {
        $.ajax({
            type: "POST",
            url: '/EFORM/finalizeETestReport/' + id,
            dataType: 'json',
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                if (response.success) {
                    alert("Record finalized successfully");
                    $('#tbWorkList').trigger('reloadGrid');
                }
                else {
                    alert(response.responseMsg);
                    $('#tbWorkList').trigger('reloadGrid');
                }

            },
            error: function (error) {
                $("#myAlert").html('<div class="alert alert-danger">' + error + '</div>');
                $('#tbWorkList').trigger('reloadGrid');
            }
        });
    }

}

