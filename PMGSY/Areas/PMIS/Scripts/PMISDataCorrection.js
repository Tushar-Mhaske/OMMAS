$(document).ready(function () {

    /// Hide Delete PlanProgress grid div
    $('#dvPMISDataDeleteProgressPlan').hide();

    /// Hide Update form
    $('#dvPMISDataDeleteUpdateDetails').hide();

    /// btn on PMISDataDeletion.cshtml
    $("#btnPMISDataDetail").click(function () {
        
        if ((parseInt($("#ddlState option:selected").val()) > 0) && (parseInt($("#ddlDistrict option:selected").val()) >= 0) && (parseInt($("#ddlBlock option:selected").val()) >= 0)) {

            /// Hide Delete PlanProgress grid div
            $('#dvPMISDataDeleteProgressPlan').hide();

            /// Hide Update form
            $('#dvPMISDataDeleteUpdateDetails').hide();

           LoadPMISDataCorrectionList();
        }
        else {
            alert("Please Select State");
        }
    }); // btnPMISDataDetails END


    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

   

   
});     // doc ready end


$("#ddlState").change(function () {

    $("#ddlDistrict").empty();
    $("#ddlBlock").empty();

    $.ajax({
        url: '/PMIS/PMIS/PopulateDistrictForPMISDataDeletion',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { StateCode: $("#ddlState option:selected").val() },
        success: function (data) {
            if (!(data.success == false)) {

                var lstDistrict = data.DistrictList;
                var lstBlock = data.BlockList;

                for (var i = 0; i < lstDistrict.length; i++) {
                    $("#ddlDistrict").append("<option value='" + lstDistrict[i].Value + "'>" + lstDistrict[i].Text + "</option>");
                }

                for (var i = 0; i < lstBlock.length; i++) {
                    $("#ddlBlock").append("<option value='" + lstBlock[i].Value + "'>" + lstBlock[i].Text + "</option>");
                }
                $.unblockUI();
            }
            else {
                alert(data.errorMessage);
                $.unblockUI();
            }
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});         // state change end


$("#ddlDistrict").change(function () {

    $("#ddlBlock").empty();

    $.ajax({
        url: '/PMIS/PMIS/PopulateBlocks',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { DistrictCode: $("#ddlDistrict").val() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            $.unblockUI();
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});            // district change end


function LoadPMISDataCorrectionList() {

    jQuery("#tbPMISDataCorrectionList").jqGrid('GridUnload');

    jQuery("#tbPMISDataCorrectionList").jqGrid({
        url: '/PMIS/PMIS/PMISDataCorrectionList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'District', 'Block', 'Package Name', 'Sanction Year', 'Sanction Date', 'Batch', 'Length', 'Agreement Number', 'Agreement Cost', 'MoRD Share', 'State Share', 'Total Sanctioned Cost', 'Agreement Start Date', 'Agreement End Date', 'Road | Bridge Name', 'Delete Plan & Progress', 'Update Completion Length & Date'],
        colModel: [
                         { name: 'STATE', index: 'STATE', width: 100, align: "left" },
                         { name: 'DISTRICT', index: 'DISTRICT', width: 100, align: "left" },
                         { name: 'BLOCK', index: 'BLOCK', width: 100, align: "left" },
                         { name: 'PACKAGE_NAME', index: 'PACKAGE_NAME', width: 120, align: "left" },
                         { name: 'SANCTION_YEAR', index: 'SANCTION_YEAR', width: 70, align: "left" },
                         { name: 'SANCTION_DATE', index: 'SANCTION_Date', width: 70, align: "left" },
                         { name: 'BATCH', index: 'BATCH', width: 70, align: "left" },
                         { name: 'LENGHT', index: 'LENGHT', width: 60, align: "left" },
                         { name: 'AGREEMENT_NUMBER', index: 'AGREEMENT_NUMBER', width: 100, align: "left" },
                         { name: 'AGREEMENT_COST', index: 'AGREEMENT_COST', width: 80, align: "left" },
                         { name: 'MoRD_SHARE', index: 'MoRD_SHARE', width: 80, align: "left" },
                         { name: 'STATE_SHARE', index: 'STATE_SHARE', width: 80, align: "left" },
                         { name: 'TOTAL_SANCTIONED_COST', index: 'TOTAL_SANCTIONED_COST', width: 80, align: "left" },
                         { name: 'AgreementStartDate', index: 'AgreementStartDate', width: 80, align: "left" },
                         { name: 'AgreementEndDate', index: 'AgreementEndDate', width: 80, align: "left" },                         
                         { name: 'ROAD_NAME', index: 'ROAD_NAME', width: 120, align: "left" },
                         { name: 'DELETE_PLAN', width: 50, resize: false, align: "center" },
                         { name: 'Update', index: 'Update', width: 50, resize: false, align: "center", sortable: false }

        ],
        postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val(), listType: $("#ddlListType option:selected").val() },
        pager: jQuery('#dvPMISDataCorrectionListPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp; PMIS Road List",
        ShrinkToFit: true,
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            
            $("#tbPMISDataCorrectionList #dvPMISDataCorrectionListPager").css({ height: '31px'});
            unblockPage();
            
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!");

            }
        }

    }); // End of grid

}       // End of gridFunction


function UpdateCompletionLengthDateDetails(RoadCode) {

    if (confirm("Are you sure to update details ?")) {

        /// Delete plan progress div hide on update action
        $('#dvPMISDataDeleteProgressPlan').hide('slow');

        $.ajax({
            url: '/PMIS/PMIS/GetPlanDetailsToEdit',
            data: { 'RoadCode': RoadCode},
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            success: function (data) {
                if (data.success != false) {

                    $("#tbPMISDataCorrectionList").jqGrid('setGridState', 'hidden');

                    $('#dvPMISDataDeleteUpdateDetails').show('slow');

                    $("#dvPMISDataDeleteUpdateForm").html(data);
                    $.unblockUI();
                }
                else {
                   
                    $.unblockUI();
                    alert("Error : " + data.errorMessage);
                }
            },
            error: function (xhr) {
                alert('error: ' + xhr.statustext);
                $.unblockUI();
            },
            dataType: "html"
        });
    }
}

function CloseDataDeleteUpdateForm() {

    $('#dvPMISDataDeleteUpdateDetails').hide('slow');

    $("#tbPMISDataCorrectionList").jqGrid('setGridState', 'visible');
    
}


function DeleteProjectProgressPlan(RoadCode) {
  
    /// Hide Update window on delete action
    $('#dvPMISDataDeleteUpdateDetails').hide('slow');

    jQuery("#tbPMISDataDeleteProgressPlan").jqGrid('GridUnload');

    jQuery("#tbPMISDataDeleteProgressPlan").jqGrid({
        url: '/PMIS/PMIS/DataDeleteProgressPlanList',
        datatype: "json",
        mtype: "POST",
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        colNames: ['Plan No', 'ROAD', 'Revision', 'Delete Plan', 'Delete Progress'],
        colModel: [
                    { name: 'PLAN_ID', index: 'PLAN_ID', hidden: true, align: "center" },
                    { name: 'IMS_PR_ROAD_CODE', index: 'IMS_PR_ROAD_CODE', hidden: true, align: "center" },
                    { name: 'BASELINE', index: 'BASELINE', align: "center", sortable: false },
                    { name: 'DeletePlanId', index: 'DeletePlanId', resize: false, align: "center", sortable: false, formatter: DeletePlanIcon, editoptions: { value: "True:False" }},
                    { name: 'DeleteProgressId', index: 'DeleteProgressId', resize: false, align: "center", sortable: false, formatter: DeleteProgressIcon, editoptions: { value: "True:False" } }
        ],
        postData: { 'RoadCode': RoadCode },
        pager: jQuery('#dvPMISDataDeleteProgressPlanPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "",
        height: 'auto',
        autowidth: true,
        sortname: 'BASELINE_NO',
        sortorder: "desc",
        cmTemplate: { title: false },
        rownumbers: true,
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "PLAN_ID",
        },
        loadComplete: function () {

            var recs = parseInt($("#tbPMISDataDeleteProgressPlan").getGridParam("records"), 10);

            if (isNaN(recs) || recs == 0) {
                $("#dvPMISDataDeleteProgressPlan").hide();

                if ((parseInt($("#ddlState option:selected").val()) > 0) && (parseInt($("#ddlDistrict option:selected").val()) >= 0) && (parseInt($("#ddlBlock option:selected").val()) >= 0)) {
                    $("#tbPMISDataCorrectionList").jqGrid('setGridState', 'visible');
                    LoadPMISDataCorrectionList();
                }
                               
                alert("No Records Found !!");
            }
            else {
                $("#tbPMISDataCorrectionList").jqGrid('setGridState', 'hidden');                          // on delete progress jqgrid table state hidden

                $('#dvPMISDataDeleteProgressPlan').show();

                $("#tbPMISDataDeleteProgressPlan #dvPMISDataDeleteProgressPlanPager").css({ height: '31px' });

                unblockPage();                
            }                                 
            
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!");
            }
        }
    }); // End of grid

   
 }
 
//---------------------------------------------

function DeletePlanIcon(cellvalue, options, rowObject) {
        
    if (rowObject.DeletePlanId) {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon 	ui-icon-minusthick' title='Delete Project Plan'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + rowObject.PLAN_ID + "\" , \"" + rowObject.IMS_PR_ROAD_CODE + "\");'></span></td></tr></table></center>";
    }                  
}

function DeleteProgressIcon(cellvalue, options, rowObject) {
   
    if (rowObject.DeleteProgressId) {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Progress' onClick ='DeleteProjectProgress(\"" + rowObject.PLAN_ID + "\", \"" + rowObject.IMS_PR_ROAD_CODE + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon 	ui-icon-minusthick' title='Delete Project Progress'></span></td></tr></table></center>";
    }
}

function DeleteProjectPlan(planId, RoadCode) {
   

    if (confirm("Are you sure to delete Plan ?")) {
        $.ajax({
            url: '/PMIS/PMIS/DataDeletePlan',
            type: "POST",
            cache: false,
            async: false,
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { 'planId': planId },
            success: function (response) {
                if (response.success) {
                    alert("Plan deleted successfully.");
                    
                    // To close div of Plans and progress list and show Roadlist div
                    //$('#dvPMISDataDeleteProgressPlan').hide('slow');
                    //$("#tbPMISDataCorrectionList").jqGrid('setGridState', 'visible');
                    DeleteProjectProgressPlan(RoadCode);
                }
                else {
                    alert(response.errorMessage); 
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }   // end of if block
}
function DeleteProjectProgress(planId, RoadCode) {
   
    if (confirm("Are you sure to delete Progress ?")) {
        $.ajax({
            url: '/PMIS/PMIS/DataDeleteProgress',
            type: "POST",
            cache: false,
            async: false,
            data: { 'planId': planId },
            success: function (response) {
                if (response.success) {
                    alert("Progress deleted successfully.");
                    DeleteProjectProgressPlan(RoadCode);
                }
                else {
                    alert(response.errorMessage);
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }   // end of if block
}



function CloseDataDeletePlanProgress() {
    
        $('#dvPMISDataDeleteProgressPlan').hide('slow');
        $("#tbPMISDataCorrectionList").jqGrid('setGridState', 'visible');
}

