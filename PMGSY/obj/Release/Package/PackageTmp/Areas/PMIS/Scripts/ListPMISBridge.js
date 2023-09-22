$(document).ready(function () {
    /*Module : PMIS
      Created : August 2021
      Author  : Bhushan
   */
    LoadPMISBridgeList();

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $("#ddlDistrict").change(function () {
        $("#ddlBlock").empty();

        $.ajax({
            url: '/PMIS/PMIS/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { DistrictCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
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

    $("#btnBridgeList").click(function () {
        if ((parseInt($("#ddlState option:selected").val()) > 0) && (parseInt($("#ddlDistrict option:selected").val()) > 0)) {
            LoadPMISBridgeList();
        }


    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

});

//function FormatPlanColumn(cellvalue, options, rowObject) {

//    if (cellvalue.endsWith("$")) {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick ='AddProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else if (cellvalue.endsWith("&")) {
//        "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick ='EditProjectPlan(\"" + cellvalue.toString() + "\");' ></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }


//}

//function FormatActualsColumn(cellvalue, options, rowObject) {

//    if (cellvalue != '') {
//        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick ='AddActuals(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//    }
//    else {
//        return "<center><table><tr><td  style='border:none;'><span>-</span></td></tr></table></center>";
//    }

//}

function LoadPMISBridgeList() {
    //alert("LoadPMISBridgeList");
    jQuery("#tbPMISBridgeList").jqGrid('GridUnload');

    jQuery("#tbPMISBridgeList").jqGrid({
        url: '/PMIS/PMIS/PMISBridgeList',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'District', 'Block', 'Package Name', 'Sanction Year', 'Sanction Date', 'Batch', 'Length(in Km)', 'Agreement Number', 'Agreement Cost', 'MoRD Share', 'State Share', 'Total Sanctioned Cost', 'Bridge Name', 'Add/Update Project Plan', 'Finalize', 'Revise Plan Details', 'Add/Update Actuals', 'Add Chainage wise Details'], //, 'Revise Plan Details' 
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
                         { name: 'LSB_NAME', index: 'LSB_NAME', width: 120, align: "left" },
                         { name: 'PROJECT_PLAN', width: 120, resize: false, align: "center" }, //formatter: FormatPlanColumn,
                         { name: 'FINALIZE', index: 'FINALIZE', width: 120, align: "center", },
                         { name: 'REVISE_PLAN', index: 'REVISE_PLAN', width: 120, align: "center", },
                         { name: 'ACTUALS', width: 120, resize: false, align: "center", hidden: true },
                         { name: 'CHAINAGE', width: 120, resize: false, align: "center", hidden: true }// formatter: FormatActualsColumn,

        ],
        postData: { state: $('#ddlState option:selected').val(), district: $('#ddlDistrict option:selected').val(), block: $('#ddlBlock option:selected').val(), sanction_year: $('#ddlYear').val(), batch: $('#ddlBatch').val() },
        pager: jQuery('#dvPMISBridgeListPager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;PMIS Bridge List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#tbPMISBridgeList #dvPMISBridgeListPager").css({ height: '31px' });
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

    }); //end of grid



}


function ClosePMISBridgeDetails() {
    $('#accordion').hide('slow');
    $('#divPMISBridgeListForm').hide('slow');
    $("#tbPMISBridgeList").jqGrid('setGridState', 'visible');
    $('#divFilterForm').show('slow');
}

function AddProjectPlan(cellvalue) {
    //debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add PMIS Project Plan</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISBridgeDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divPMISBridgeListForm").load('/PMIS/AddPMISBridgeProjectPlan/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));

        });

        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });

    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function DeleteProjectPlan(urlparameter) {

    //var token = $('input[name=__RequestVerificationToken]').val();
    //debugger;
    if (confirm("Are you sure to delete Plan ?")) {
        $.ajax({
            url: '/PMIS/DeletePmisBridgeProjectPlan/' + urlparameter,
            type: "POST",
            cache: false,
            async: false,
            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
            success: function (response) {
                if (response.success) {
                    alert("Plan deleted successfully.");
                    LoadPMISBridgeList();
                    $('#tbPMISBridgeList').trigger('reloadGrid');

                }
                else {
                    alert(response.errorMessage);
                    $('#tbPMISBridgeList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }


}

function RevisePlanDetails(RoadCode) {

    //var token = $('input[name=__RequestVerificationToken]').val();
    debugger;
    if (confirm("Are you sure to revise Plan ?")) {
        $.ajax({
            url: '/PMIS/RevisePmisBridgeProjectPlan/' + RoadCode,
            type: "POST",
            cache: false,
            async: false,
            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
            success: function (response) {
                if (response.success) {
                    LoadPMISBridgeList();
                    EditProjectPlan(RoadCode);
                    //alert("Plan revised successfully.");
                    //$('#tbPMISBridgeList').trigger('reloadGrid');

                }
                else {
                    alert(response.ErrorMessage)
                    $('#tbPMISBridgeList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }


}

function FinalizeProjectPlan(RoadCode) {

    //var token = $('input[name=__RequestVerificationToken]').val();
    debugger;
    if (confirm("Are you sure to finalize Plan ?")) {
        $.ajax({
            url: '/PMIS/FinalizePmisBridgeProjectPlan/' + RoadCode,
            type: "POST",
            cache: false,
            async: false,
            data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
            success: function (response) {
                if (response.success) {
                    alert("Plan finalized successfully.");
                    LoadPMISBridgeList();
                    $('#tbPMISBridgeList').trigger('reloadGrid');

                }
                else {
                    alert(response.errorMessage)
                    $('#tbPMISBridgeList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }


}

function EditProjectPlan(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit PMIS Project Plan</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISBridgeDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divPMISBridgeListForm").load('/PMIS/UpdatePMISBridgeProjectPlanLayout/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));
            unblockPage();
        });
        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });

    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}

function ViewProjectPlan(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >View PMIS Project Plan</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISBridgeDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divPMISBridgeListForm").load('/PMIS/ViewPMISBridgeProjectPlanLayout/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));
            unblockPage();
        });
        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });
    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

function ViewPartialProjectPlan(BridgeNameParam, stateShareParam, mordShareParam, totalSanctionedCostParam, baselineParam) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >View PMIS Project Plan</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="ClosePMISBridgeDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divPMISBridgeListForm").load('/PMIS/PMIS/ViewPMISBridgeProjectPlan/?BridgeName=' + encodeURIComponent(BridgeNameParam) + '&StateShare=' + stateShareParam + '&MordShare=' + mordShareParam + '&TotalSanctionedCost=' + totalSanctionedCostParam + '&baseline=' + baselineParam, function (response) {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));
            unblockPage();
        });
        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });
    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

function AddActuals(cellvalue) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add / Edit Actuals</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISBridgeDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divPMISBridgeListForm").load('/PMIS/AddActuals/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));

        });

        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });

    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function AddChainage(cellvalue) {
    debugger;
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Chainage wise Details</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePMISBridgeDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordion').show('slow', function () {

        $("#divPMISBridgeListForm").load('/PMIS/AddChainage/' + cellvalue, function () {
            $.validator.unobtrusive.parse($('#divPMISBridgeListForm'));

        });

        $('#divPMISBridgeListForm').show('slow');
        $("#divPMISBridgeListForm").css('height', 'auto');
    });

    $("#tbPMISBridgeList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}
