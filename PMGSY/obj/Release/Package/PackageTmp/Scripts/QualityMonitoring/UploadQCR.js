

var yearCode = $("#ddlYear option:selected").val();
var districtCode = $('#ddlDistrict option:selected').val();

$("#btnListDetails").click(function () {
  
    if ($("#searchExecution").valid()) {     
        districtCode = $('#ddlDistrict option:selected').val();
        yearCode = $("#ddlYear option:selected").val();
        LoadExecutionGrid();

    }
});

$("#btnListDetailsCQC").click(function () {

    if ($("#searchExecution").valid()) {
        districtCode = $('#ddlDistrict option:selected').val();
        yearCode = $("#ddlYear option:selected").val();
        LoadExecutionGridCQC();
    }
});

// Added on 23-02-2022 by Srishti Tyagi
$('#ddlStates').change(function () {
    $("#ddlDistrict").empty();
    // alert("hi");

    $.ajax({
        url: '/QualityMonitoring/PopulateDistrictsbyStateCode',
        type: 'POST',
        async: false,
        cache: false,
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlStates").val(), },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {

                $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");

            }
            districtCode = $('#ddlDistrict option:selected').val();
            $("#btnListDetails").trigger('click');
            LoadExecutionGrid();

            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });

});


var road_code;
$("#idFilterDiv").click(function () {
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-n").toggleClass("ui-icon-circle-triangle-s");
    $("#searchExecution").toggle("slow");
});

function LoadExecutionGrid() {

    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/QualityMonitoring/GetExecutionProgressList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { yearCode: yearCode, districtCode: districtCode },
        colNames: ['Block', 'Sanctioned Year', 'Batch', 'Package No.', 'Road Code', 'Road Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Proposal Type', 'Upload QCR Part-I PDF'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },                          
                            { name: 'IMS_ROAD_CODE', index: 'IMS_ROAD_CODE', height: 'auto', width: 80, align: "center", search: false,hidden:true},
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 100, align: "center", search: true },
                             { name: 'AddQCR', index: 'AddQCR', width: 70, align: "center", resize: false, sortable: false, resizable: false },
                          
        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; QCR Grading Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
            var selectedRowId = $('#tbExecutionList').jqGrid('getGridParam', 'selrow');
            road_code = $('#tbExecutionList').jqGrid('getCell', selectedRowId, 'IMS_ROAD_CODE');
            
        
        },
        loadComplete: function (data) {

            $("#tbExecutionList #pagerExecution").css({ height: '40px' });
            $("#pagerExecution_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>");           
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
            $('#dvQCRDetails').hide('slow');
            $('#dvAddQCRDetails').hide('slow');
            $('#divListExecutionPDF').hide('slow');
        }
    });

}

function LoadExecutionGridCQC() {

    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/QualityMonitoring/GetExecutionProgressList',
        datatype: "json",
        mtype: "POST",
        async: false,
        cache: false,
        postData: { yearCode: yearCode, districtCode: districtCode },
        colNames: ['Block', 'Sanctioned Year', 'Batch', 'Package No.', 'Road Code', 'Road Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Proposal Type', 'View QCR Part-I PDF'/*, 'View PDF'*/],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'IMS_ROAD_CODE', index: 'IMS_ROAD_CODE', height: 'auto', width: 80, align: "center", search: false, hidden: true },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 100, align: "center", search: true },
                             { name: 'AddQCR', index: 'AddQCR', width: 70, align: "center", resize: false, sortable: false, resizable: false },
                             // Commented on 28-01-2022 by Srishti Tyagi
                          /*  { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },  */

        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        // Changes done on 28-01-2022 by Srishti Tyagi
        //caption: "&nbsp;&nbsp; Execution Details List",
        caption: "&nbsp;&nbsp; QCR Grading Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        onSelectRow: function (id) {
            var selectedRowId = $('#tbExecutionList').jqGrid('getGridParam', 'selrow');
            road_code = $('#tbExecutionList').jqGrid('getCell', selectedRowId, 'IMS_ROAD_CODE');


        },
        loadComplete: function (data) {

            $("#tbExecutionList #pagerExecution").css({ height: '40px' });
            $("#pagerExecution_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>");
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        onHeaderClick: function () {
            $('#dvQCRDetails').hide('slow');
            $('#dvAddQCRDetails').hide('slow');
            $('#divListExecutionPDF').hide('slow');
        }
    });

}


function AddQCR(urlparameter) {
    
    $("#searchExecution").hide('slow')  
   $("#tbExecutionList").jqGrid('setGridState', 'hidden'); 
   $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
   $('#divAddQCR').show('slow');
 
   
   $('#divAddQCR').load("/QualityMonitoring/AddQCR?idtemp=" + urlparameter, function () {
       
         $('#qcrOperation').html('&nbsp;&nbsp;Add QCR Details');
         $('#IMS_PR_ROAD_CODE').val(road_code);

         $('#divListExecutionPDF').append(function () {                                             
             
             jQuery("#tbExecutionListPDF").jqGrid('GridUnload');
             jQuery("#tbExecutionListPDF").jqGrid({
                 url: "/QualityMonitoring/GetQCRDetailsList?idtemp=" + urlparameter,
                 datatype: "json",
                 mtype: "POST",
                 async: false,
                 cache: false,               
                 colNames: ['District', 'Block', 'Year', 'Batch', 'Package No.', 'Proposal Type', 'Road Name', 'Work Started', 'Uploaded Date', 'Remark', 'QCR Part-I Grading By SE', 'QCR Part-I Grading By SQC', 'View', 'Delete', 'Finalize'],
                 colModel: [

                                     { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
                                     { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
                                     { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
                                     { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 60, align: "center" },
                                     { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
                                     { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
                                     { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },
                                     { name: 'MAST_MONTH_FULL_NAME', index: 'MAST_MONTH_FULL_NAME', height: 'auto', width: 70, align: "center" },
                                     { name: 'FILE_UPLOADED_DATE', index: 'FILE_UPLOADED_DATE', height: 'auto', width: 80, align: "center" },
                                     { name: 'UPLOAD_REMARK', index: 'UPLOAD_REMARK', height: 'auto', width: 150, align: "center" },
                                     { name: 'GRADE_SE', index: 'GRADE_SE', height: 'auto', width: 80, align: "center" },
                                     { name: 'GRADE_SQC', index: 'GRADE_SQC', height: 'auto', width: 80, align: "center" },
                                     { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
                                     { name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center" },
                                     { name: 'Finalize', index: 'Finalize', height: 'auto', width: 80, align: "center" },
                 ],
                 pager: jQuery('#pagerExecutionPDF').width(20),
                 rowNum: 10,
                 rowList: [10, 20, 30],
                 viewrecords: true,
                 recordtext: '{2} records found',
                 sortname: "FILE_NAME",
                 sortorder: "asc",
                 caption: "&nbsp;&nbsp; Uploaded QCR Part-I Grading Details",
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

function ViewQCRcqc(urlparameter) {

    $("#searchExecution").hide('slow')
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#divAddQCR').show('slow');
    // $('#dvQCRDetails').hide();

    $('#divAddQCR').load("/QualityMonitoring/AddQCR?idtemp=" + urlparameter, function () {

        $('#dvQCRDetails').hide();
        $('#dvAddQCRDetails').hide();
        // $('#qcrOperation').html('&nbsp;&nbsp;Add QCR Details');
        $('#IMS_PR_ROAD_CODE').val(road_code);

        // Added on 07-02-2022 by Srishti Tyagi
        $('#divListExecutionPDF').append(function () {

            jQuery("#tbExecutionListPDF").jqGrid('GridUnload');
            jQuery("#tbExecutionListPDF").jqGrid({
                url: "/QualityMonitoring/GetQCRDetailsList?idtemp=" + urlparameter,
                datatype: "json",
                mtype: "POST",
                async: false,
                cache: false,
                colNames: ['District', 'Block', 'Year', 'Batch', 'Package No.', 'Proposal Type', 'Road Name', 'Work Started',/* 'QCR Part 1 PDF',*/ 'Uploaded Date', 'Remark', 'QCR Part-I Grading By SE', 'QCR Part-I Grading By SQC', 'View'/*, 'Delete', 'Finalize'*/],
                colModel: [

                                    { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "center" },
                                    { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "center" },
                                    { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 50, align: "center" },
                                    { name: 'IMS_BATCH', index: 'IMS_BATCH', height: 'auto', width: 60, align: "center" },
                                    { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 60, align: "center" },
                                    { name: 'IMS_PROPOSAL_TYPE', index: 'IMS_PROPOSAL_TYPE', height: 'auto', width: 70, align: "center" },
                                    { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', height: 'auto', width: 150, align: "center" },

                                    { name: 'MAST_MONTH_FULL_NAME', index: 'MAST_MONTH_FULL_NAME', height: 'auto', width: 70, align: "center" },

                                 //   { name: 'FILE_NAME', index: 'FILE_NAME', height: 'auto', width: 80, align: "center" },
                                    { name: 'FILE_UPLOADED_DATE', index: 'FILE_UPLOADED_DATE', height: 'auto', width: 80, align: "center" },
                                    { name: 'UPLOAD_REMARK', index: 'UPLOAD_REMARK', height: 'auto', width: 150, align: "center" },
                                    { name: 'GRADE_SE', index: 'GRADE_SE', height: 'auto', width: 80, align: "center" },
                                    { name: 'GRADE_SQC', index: 'GRADE_SQC', height: 'auto', width: 80, align: "center" },
                                    { name: 'viewPDF', index: 'viewPDF', width: 70, align: "center", search: false, sortable: false },
                                    //{ name: 'Delete', index: 'Delete', height: 'auto', width: 80, align: "center" },
                                    //{ name: 'Finalize', index: 'Finalize', height: 'auto', width: 80, align: "center" },
                ],
                pager: jQuery('#pagerExecutionPDF').width(20),
                rowNum: 10,
                rowList: [10, 20, 30],
                viewrecords: true,
                recordtext: '{2} records found',
                sortname: "FILE_NAME",
                sortorder: "asc",
                caption: "&nbsp;&nbsp; Uploaded QCR Part-I Grading Details",
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


function DeleteFile(urlparameter) {

    if (confirm("Do you want to delete the file ?")) {
        $.ajax({
            url: "/QualityMonitoring/DeleteQCRPdf?id=" + urlparameter,
            type: 'GET',
            dataType: 'json',                    
            success: function (response) {

                alert(response.message);

                if (response.success) {              
                    $("#formAddQCR").show('slow');
                    $('#tbExecutionListPDF').trigger('reloadGrid');
                
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

    if (confirm("Do you want to finalize the changes ?")) {
        $.ajax({
            url: "/QualityMonitoring/FinalizeDetails?id=" + urlparameter,
            type: 'GET',
            dataType: 'json',
            success: function (response) {

                alert(response.message);

                if (response.success) {
                    $("#formAddQCR").show('slow')
                    $('#tbExecutionListPDF').trigger('reloadGrid');

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

