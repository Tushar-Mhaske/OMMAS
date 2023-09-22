$(document).ready(function () {


    //LoadCompletedRoads();
    $("#spCollapseIconS").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            //$(this).next("#dvSearchParameter").slideToggle(300);
            $("#dvSearchParameter").slideToggle(300);
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnSearch').click(function (e) {
        SearchDetails();
    });

    $('#btnSearch').trigger('click');

    $("#ddlFinancialYears").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlFinancialYears").find(":selected").val() },
            "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

    $("#ddlBlocks").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlBlocks").find(":selected").val() },
            "#ddlPackages", "/Agreement/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlFinancialYears option:selected').val() + "&blockCode=" + $('#ddlBlocks option:selected').val());



    }); //end function block change

});

//Added on 25-07-2022
//DPIU Level
$("#btnViewDLPRoadDetails").click(function () {

    if ($("#ddlMaintTypeList option:selected").val() == 2) {
        $("#noteDLP").attr("hidden", true);
        $("#notePostDLP").attr("hidden", false);
    }
    else {
        $("#noteDLP").attr("hidden", false);
        $("#notePostDLP").attr("hidden", true);
    }

    function LoadCompletedRoads() {
        //alert("EmargRoadList.js -->Line No.61");
        //alert($("#ddlMaintTypeList option:selected").val());

        $("#tbProposedRoadList").jqGrid("GridUnload");

        if ($("#ddlMaintTypeList option:selected").val() == -1) {
            alert("Please Select Maintenance Type.");
            return false;
        }
        if ($("#ddlPIUList option:selected").val() == 0) {
            alert("Please Select DPIU.");
            return false;
        }
        var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();
        var ddlPIUCode = $('#ddlPIUList option:selected').val();

        blockPage();

        jQuery("#tbProposedRoadList").jqGrid({
            url: '/MaintenanceAgreement/GetEmargDLPFinalList',
            datatype: "json",
            mtype: "POST",

            colNames: $("#ddlMaintTypeList option:selected").val() == 2 ? ['Package ID', 'Road Name', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'] : ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            //colNames: ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Edit", 'Finalize Road', 'Status', 'Definalize Road', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            colModel: [
                { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 200, sortable: false, align: "center" },
                { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 250, sortable: false, align: "left" },

                { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, sortable: false, align: "center" },
                { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', height: 'auto', width: 80, sortable: false, align: "center" },
                { name: 'IMS_PAV_LENGTH', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "center" },

                // { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },

                { name: 'a', width: 50, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'b', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'c', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'f', width: 150, sortable: false, resize: false, align: "center", sortable: false },// Road Definalization
                { name: 'g', width: 100, sortable: false, resize: false, align: "center", sortable: false, hidden: ddlMaintTypeCode == 2 ? false : true },// Road Push To emarg

                // REJECTION_REASON
                { name: 'REJECTION_REASON', index: 'REJECTION_REASON', width: 150, sortable: false, align: "center" },
                { name: 'd', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'e', width: 50, sortable: false, resize: false, align: "center", sortable: false, hidden: true }



            ],
            //    postData: { "Block": $("#BlockID option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val() },
            postData: { "MaintTypeCode": ddlMaintTypeCode, "PIUCode": ddlPIUCode },
            pager: jQuery('#dvProposedRoadListPager'),
            rowNum: 5000,
            rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
            viewrecords: true,
            recordtext: '{2} records found',
            //caption: "&nbsp;&nbsp;Emarg Road Details."
            caption: $("#ddlMaintTypeList option:selected").val() == 2 ? "&nbsp;&nbsp;Post DLP Road Details." : "&nbsp;&nbsp;DLP Road Details.",
            height: 'auto',
            width: 'auto',
            sortname: $("#ddlMaintTypeList option:selected").val() == 2 ? 'IMS_PACKAGE_ID' : false,
            //autowidth: true,
            rownumbers: true,
            //grouping: true,
            grouping: $("#ddlMaintTypeList option:selected").val() == 2 ? false : true,
            groupingView:

                $("#ddlMaintTypeList option:selected").val() == 2
                    ? false
                    :
                    {
                        groupField: ["IMS_PACKAGE_ID"],
                        groupColumnShow: [false],
                        groupText: [


                            "<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'
                            + "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"

                            //"<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px;' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                        ],
                        groupOrder: ["asc"],
                        groupSummary: [true],
                        groupSummaryPos: ['header'],
                        groupCollapse: false
                    },
            loadComplete: function (data) {

                //  "<b>Package ID : {0}</b>" + "<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px; color:ButtonColor({9})' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                //for (var i = 0; i < data.rows.length; i++) {
                //    var rowData = data.rows[i];
                //    console.log(data.rows.length);
                //    if (rowData.cell[8] == 'True') {
                //        var checkbox = $("#jqg_tbCNRoadList_" + rowData['id']);
                //        checkbox.attr("disabled", false);
                //    }
                //    $("#cb_tbCNRoadList").attr("disabled", false);
                //}


                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    window.location.href = "/Login/SessionExpire";
                }
                else {
                    window.location.href = "/Login/SessionExpire";
                }
            }
        }); //end of grid
    }

    LoadCompletedRoads();
})

//---------------------new added by rahul on 14-01-2022--------------------------------
//level srda
$("#btnViewEmargRoadList").click(function () {

    if ($("#ddlMaintTypeList option:selected").val() == 2) {
        $("#noteDLP").attr("hidden", true);
        $("#notePostDLP").attr("hidden", false);
    }
    else {
        $("#noteDLP").attr("hidden", false);
        $("#notePostDLP").attr("hidden", true);
    }
    //if ($("#ddlPIUList").val() == 0) {
    //    alert("Please Select DPIU.");
    //    return false;
    //}
    //alert($('#ddlPIUList option:selected').val());
    if ($('#ddlMaintTypeList option:selected').val() == -1) {
        alert("Please Select Maintenance Type.");
        return false;
    }

    if ($("#ddlPIUList option:selected").val() == 0) {
        alert("Please Select DPIU.");
        return false;
    }
    var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();
    var ddlPIUCode = $('#ddlPIUList option:selected').val();

    function LoadCompletedRoads() {
        $("#tbProposedRoadList3").jqGrid("GridUnload");

        blockPage();

        jQuery("#tbProposedRoadList3").jqGrid({
            //url: '/MaintenanceAgreement/GetEmargFinalList',
            url: '/MaintenanceAgreement/GetEmargDLPFinalList',
            datatype: "json",
            mtype: "POST",

            colNames: $("#ddlMaintTypeList option:selected").val() == 2 ? ['Package ID', 'Road Name', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'] : ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            //colNames: ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Edit", 'Finalize Road', 'Status', 'Definalize Road', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            colModel: [
                { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 200, sortable: false, align: "center" },
                { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 250, sortable: false, align: "left" },

                { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, sortable: false, align: "center" },
                { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', height: 'auto', width: 80, sortable: false, align: "center" },
                { name: 'IMS_PAV_LENGTH', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "center" },

                // { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },

                { name: 'a', width: 50, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'b', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'c', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'f', width: 150, sortable: false, resize: false, align: "center", sortable: false },// Road Definalization
                { name: 'g', width: 100, sortable: false, resize: false, align: "center", sortable: false, hidden: ddlMaintTypeCode == 2 ? false : true },// Road Push To emarg

                // REJECTION_REASON
                { name: 'REJECTION_REASON', index: 'REJECTION_REASON', width: 150, sortable: false, align: "center" },
                { name: 'd', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'e', width: 50, sortable: false, resize: false, align: "center", sortable: false, hidden: true }



            ],
            //postData: { "Block": $("#BlockID option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val() },
            postData: { "MaintTypeCode": ddlMaintTypeCode, "PIUCode": ddlPIUCode },

            pager: jQuery('#dvProposedRoadListPager'),
            rowNum: 5000,
            rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
            viewrecords: true,
            recordtext: '{2} records found',
            //caption: "&nbsp;&nbsp;Emarg Road Details.",
            caption: $("#ddlMaintTypeList option:selected").val() == 2 ? "&nbsp;&nbsp;Post DLP Road Details." : "&nbsp;&nbsp;DLP Road Details.",
            height: 'auto',
            width: 'auto',
            // sortname: 'RoadName',
            sortname: $("#ddlMaintTypeList option:selected").val() == 2 ? 'IMS_PACKAGE_ID' : false,
            //autowidth: true,
            rownumbers: true,
            //grouping: true,
            grouping: $("#ddlMaintTypeList option:selected").val() == 2 ? false : true,
            groupingView:
                $("#ddlMaintTypeList option:selected").val() == 2
                    ? false
                    :
                    {
                        groupField: ["IMS_PACKAGE_ID"],
                        groupColumnShow: [false],
                        groupText: [

                            //"<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'
                            //+ "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"


                            "<b>Package ID : {0} </b>" + '<input type="button" disabled="disabled" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" />'
                            + "     " + '<input type="button" disabled="disabled" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" />' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"


                            //"<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px;' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                        ],
                        groupOrder: ["asc"],
                        groupSummary: [true],
                        groupSummaryPos: ['header'],
                        groupCollapse: false
                    },
            loadComplete: function (data) {

                //  "<b>Package ID : {0}</b>" + "<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px; color:ButtonColor({9})' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                //for (var i = 0; i < data.rows.length; i++) {
                //    var rowData = data.rows[i];
                //    console.log(data.rows.length);
                //    if (rowData.cell[8] == 'True') {
                //        var checkbox = $("#jqg_tbCNRoadList_" + rowData['id']);
                //        checkbox.attr("disabled", false);
                //    }
                //    $("#cb_tbCNRoadList").attr("disabled", false);
                //}


                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    window.location.href = "/Login/SessionExpire";
                }
                else {
                    window.location.href = "/Login/SessionExpire";
                }
            }
        }); //end of grid
    }


    LoadCompletedRoads();

})

//leve mord
$("#btnViewEmargRoadListMor").click(function () {

    if ($("#ddlMaintTypeList option:selected").val() == 2) {
        $("#noteDLP").attr("hidden", true);
        $("#notePostDLP").attr("hidden", false);
    }
    else {
        $("#noteDLP").attr("hidden", false);
        $("#notePostDLP").attr("hidden", true);
    }

    if ($('#ddlMaintTypeList option:selected').val() == -1) {
        alert("Please Select Maintenance Type.");
        return false;
    }
    if ($("#ddlPIUList option:selected").val() == 0) {
        alert("Please Select DPIU.");
        return false;
    }
    var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();
    var ddlPIUCode = $('#ddlPIUList option:selected').val();


    function LoadCompletedRoads() {
        $("#tbProposedRoadList2").jqGrid("GridUnload");

        blockPage();

        jQuery("#tbProposedRoadList2").jqGrid({
            //url: '/MaintenanceAgreement/GetEmargFinalList',
            url: '/MaintenanceAgreement/GetEmargDLPFinalList',
            datatype: "json",
            mtype: "POST",
            //data: { "PIUCode": ddlPIUCode },

            colNames: $("#ddlMaintTypeList option:selected").val() == 2 ? ['Package ID', 'Road Name', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'] : ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            // colNames: ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Edit", 'Finalize Road', 'Status', 'Definalize Road', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
            colModel: [
                { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 200, sortable: false, align: "center" },
                { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 250, sortable: false, align: "left" },

                { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, sortable: false, align: "center" },
                { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', height: 'auto', width: 80, sortable: false, align: "center" },
                { name: 'IMS_PAV_LENGTH', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "center" },

                // { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },

                { name: 'a', width: 50, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'b', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'c', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'f', width: 150, sortable: false, resize: false, align: "center", sortable: false },// Road Definalization
                { name: 'g', width: 100, sortable: false, resize: false, align: "center", sortable: false, hidden: ddlMaintTypeCode == 2 ? false : true },// Road Push To emarg


                // REJECTION_REASON
                { name: 'REJECTION_REASON', index: 'REJECTION_REASON', width: 150, sortable: false, align: "center" },
                { name: 'd', width: 150, sortable: false, resize: false, align: "center", sortable: false },
                { name: 'e', width: 50, sortable: false, resize: false, align: "center", sortable: false, hidden: true }



            ],
            //postData: { "Block": $("#BlockID option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val() },
            postData: { "MaintTypeCode": ddlMaintTypeCode, "PIUCode": ddlPIUCode },

            pager: jQuery('#dvProposedRoadListPager'),
            rowNum: 5000,
            rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
            viewrecords: true,
            recordtext: '{2} records found',
            //caption: "&nbsp;&nbsp;Emarg Road Details.",
            caption: $("#ddlMaintTypeList option:selected").val() == 2 ? "&nbsp;&nbsp;Post DLP Road Details." : "&nbsp;&nbsp;DLP Road Details.",
            height: 'auto',
            width: 'auto',
            // sortname: 'RoadName',
            sortname: $("#ddlMaintTypeList option:selected").val() == 2 ? 'IMS_PACKAGE_ID' : false,
            //autowidth: true,
            rownumbers: true,
            //grouping: true,
            grouping: $("#ddlMaintTypeList option:selected").val() == 2 ? false : true,
            groupingView:
                $("#ddlMaintTypeList option:selected").val() == 2
                    ? false
                    :
                    {
                        groupField: ["IMS_PACKAGE_ID"],
                        groupColumnShow: [false],
                        groupText: [

                            //"<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'

                            "<b>Package ID : {0} </b>" + '<input type="button" disabled="disabled" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'
                            + "     " + '<input type="button" disabled="disabled" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"
                            //+ "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"




                            //"<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px;' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                        ],
                        groupOrder: ["asc"],
                        groupSummary: [true],
                        groupSummaryPos: ['header'],
                        groupCollapse: false
                    },
            loadComplete: function (data) {

                //  "<b>Package ID : {0}</b>" + "<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px; color:ButtonColor({9})' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                //for (var i = 0; i < data.rows.length; i++) {
                //    var rowData = data.rows[i];
                //    console.log(data.rows.length);
                //    if (rowData.cell[8] == 'True') {
                //        var checkbox = $("#jqg_tbCNRoadList_" + rowData['id']);
                //        checkbox.attr("disabled", false);
                //    }
                //    $("#cb_tbCNRoadList").attr("disabled", false);
                //}


                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    window.location.href = "/Login/SessionExpire";
                }
                else {
                    window.location.href = "/Login/SessionExpire";
                }
            }
        }); //end of grid
    }


    LoadCompletedRoads();

})



//---------------------------End--------------------------------------------


function FillInCascadeDropdown(map, dropdown, action) {

    //message = '<img src="/Content/images/busy.gif"/>';
    var message = '';
    message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()


// to load execution details



function LoadCompletedRoads() {
    $("#tbProposedRoadList").jqGrid("GridUnload");

    if ($("#ddlMaintTypeList option:selected").val() == 2) {
        $("#noteDLP").attr("hidden", true);
        $("#notePostDLP").attr("hidden", false);
    }
    else {
        $("#noteDLP").attr("hidden", false);
        $("#notePostDLP").attr("hidden", true);
    }
    //alert("Line No : 465");
    //alert($("#ddlPIUList option:selected").val())
    //alert($("#ddlMaintTypeList option:selected").val());
    blockPage();

    var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();
    var ddlPIUCode = $('#ddlPIUList option:selected').val();

    jQuery("#tbProposedRoadList").jqGrid({

        //url: '/MaintenanceAgreement/GetEmargFinalList',
        url: '/MaintenanceAgreement/GetEmargDLPFinalList',
        datatype: "json",
        mtype: "POST",

        //colNames: ['Road Name', 'Package ID', 'State Name', 'Disrtict Name','Sanctioned Length (Kms)', "Edit",'Finalize Road','Status','Definalize Road','System Rejected / PIU Correction Request','View','Push to Emarg'],
        colNames: $("#ddlMaintTypeList option:selected").val() == 2 ? ['Package ID', 'Road Name', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'] : ['Road Name', 'Package ID', 'State Name', 'Disrtict Name', 'Sanctioned Length (Kms)', "Add/Edit", 'Finalize Road', 'Status', 'Definalize Road', 'Push to Emarg', 'System Rejected / PIU Correction Request', 'View', 'Push to Emarg'],
        colModel: [
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 200, sortable: false, align: "center" },
            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 250, sortable: false, align: "left" },

            { name: 'STATE_NAME', index: 'STATE_NAME', width: 80, sortable: false, align: "center" },
            { name: 'DISTRICT_NAME', index: 'DISTRICT_NAME', height: 'auto', width: 80, sortable: false, align: "center" },
            { name: 'IMS_PAV_LENGTH', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "center" },

            // { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },

            { name: 'a', width: 50, sortable: false, resize: false, align: "center", sortable: false },
            { name: 'b', width: 150, sortable: false, resize: false, align: "center", sortable: false },
            { name: 'c', width: 150, sortable: false, resize: false, align: "center", sortable: false },
            { name: 'f', width: 150, sortable: false, resize: false, align: "center", sortable: false },// Road Definalization
            { name: 'g', width: 100, sortable: false, resize: false, align: "center", sortable: false, hidden: ddlMaintTypeCode == 2 ? false : true },// Road Push To emarg

            // REJECTION_REASON
            { name: 'REJECTION_REASON', index: 'REJECTION_REASON', width: 150, sortable: false, align: "center" },
            { name: 'd', width: 150, sortable: false, resize: false, align: "center", sortable: false },
            { name: 'e', width: 50, sortable: false, resize: false, align: "center", sortable: false, hidden: true }



        ],
        //    postData: { "Block": $("#BlockID option:selected").val(), value: Math.random, IMSYEAR: $("#Year option:selected").val() },
        postData: { "MaintTypeCode": ddlMaintTypeCode, "PIUCode": ddlPIUCode },
        pager: jQuery('#dvProposedRoadListPager'),
        rowNum: 5000,
        rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
        viewrecords: true,
        recordtext: '{2} records found',
        //caption: "&nbsp;&nbsp;Emarg Road Details.",
        caption: $("#ddlMaintTypeList option:selected").val() == 2 ? "&nbsp;&nbsp;Post DLP Road Details." : "&nbsp;&nbsp;DLP Road Details.",
        height: 'auto',
        width: 'auto',
        // sortname: 'RoadName',
        sortname: $("#ddlMaintTypeList option:selected").val() == 2 ? 'IMS_PACKAGE_ID' : false,
        //autowidth: true,
        rownumbers: true,
        //grouping: true,
        grouping: $("#ddlMaintTypeList option:selected").val() == 2 ? false : true,
        groupingView:
            $("#ddlMaintTypeList option:selected").val() == 2
                ? false
                :
                {
                    groupField: ["IMS_PACKAGE_ID"],
                    groupColumnShow: [false],
                    groupText: [


                        "<b>Package ID : {0} </b>" + '<input type="button" title="After finalizing all individual roads, Click here to finalize Package." value="Finalize" onclick="FinalizeEmargPackage(\'' + "{0}" + '\')">'
                        + "     " + '<input type="button" title="After finalizing {0} package, click here to push details to Emarg." value="Push To Emarg" onclick="PushToEmarg(\'' + "{0}" + '\')">' //+ "     ( Note : After finalizing {0} package, click on Push to Emarg button to send this package to Emarg.)"

                        //"<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px;' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

                    ],
                    groupOrder: ["asc"],
                    groupSummary: [true],
                    groupSummaryPos: ['header'],
                    groupCollapse: false
                },
        loadComplete: function (data) {

            //  "<b>Package ID : {0}</b>" + "<input type = 'button'  title='Click here to finalize Package and its Road details' value = 'Finalize' style = 'width:80px; hieght:10px; color:ButtonColor({9})' onclick='FinalizeEmargPackage({0})' />"//class='ui-icon-unlocked'

            //for (var i = 0; i < data.rows.length; i++) {
            //    var rowData = data.rows[i];
            //    console.log(data.rows.length);
            //    if (rowData.cell[8] == 'True') {
            //        var checkbox = $("#jqg_tbCNRoadList_" + rowData['id']);
            //        checkbox.attr("disabled", false);
            //    }
            //    $("#cb_tbCNRoadList").attr("disabled", false);
            //}


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}



//function FormatColumn1(cellvalue, options, rowObject) {


//    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Click here to finalize Road Details' onClick ='finalizeRoadDetailsForEmarg(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}


//function FormatColumn(cellvalue, options, rowObject) {


//    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Click here for correction of Emarg Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
//}

//FinalizeEmargPackage


//function FinalizeEmargPackage(urlparameter) {

//    alert("Ckicked")

//    if (confirm("Are you sure you want to 'Finalize' Package details ?")) {

//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        $.ajax({
//            url: "/MaintenanceAgreement/FinalizePackageEmargCorrection/" + urlparameter,
//            type: "POST",
//            dataType: "json",
//            success: function (data) {
//                console.log(data);
//                if (data.success) {
//                    LoadCompletedRoads();
//                    // $("#tbProposedRoadList").trigger('reloadGrid');
//                    $.unblockUI();
//                    alert(data.message);
//                }
//                else {
//                    alert(data.message);
//                }
//                $.unblockUI();
//            },
//            error: function (xht, ajaxOptions, throwError) {
//                alert(xht.responseText);
//                $.unblockUI();
//            }

//        });
//    }
//    else {
//        return false;
//    }

//}

function FinalizeEmargPackage(paramData) {
    if (confirm("Are you sure to finalize Package Details ?")) {
        $.ajax({
            url: '/MaintenanceAgreement/FinalizePackageEmargCorrection',
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
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
                LoadCompletedRoads();

                if (response.success) {
                    unblockPage();
                    alert(response.message);
                }
                else {
                    alert(response.message);
                    unblockPage();
                }
            }
        });
    }
}

function DeleteNonPackageFinalizedRoad(urlparameter) {

    if (confirm("Are you sure you want to definalize Road details?")) {
        $.ajax({
            type: 'POST',
            url: '/MaintenanceAgreement/DeleteRoadBeforePackageFinalization/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Road details deleted successfully");
                    LoadCompletedRoads();
                    //$("#tbFinancialList").trigger('reloadGrid');
                    //$("#divAddFinancialProgress").html('');
                }
                else if (data.success == false) {
                    alert("Road details can not be deleted.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }

}





function PushToEmarg(paramData) {
    //alert("Line No:707");
    //alert(paramData);
    if (confirm("Are you sure to push Package Details to Emarg ?")) {
        $.ajax({
            url: '/MaintenanceAgreement/PushToEmargDetails',
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
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
                LoadCompletedRoads();

                if (response.success) {

                    unblockPage();
                    alert(response.message);
                }
                else {
                    alert(response.message);
                    unblockPage();
                }
            }
        });
    }
}


function PushToEmargPostDLP(paramData) {

    //alert("Line No:737 ,PushToEmargPostDLP")
    if (confirm("Are you sure to push Package Details to Emarg ?")) {
        $.ajax({
            url: '/MaintenanceAgreement/PushToEmargDetailsPostDLP',
            type: "POST",
            cache: false,
            data: { "Data": paramData, value: Math.random },
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
                LoadCompletedRoads();

                if (response.success) {

                    unblockPage();
                    alert(response.message);
                }
                else {
                    alert(response.message);
                    unblockPage();
                }
            }
        });
    }
}

// PushToEmarg

//function PushToEmarg(urlparameter) {

//    if (confirm("Are you sure you want to push this Package details to Emarg?")) {

//        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

//        $.ajax({
//            url: "/MaintenanceAgreement/PushToEmargDetails/" + urlparameter,
//            type: "POST",
//            dataType: "json",
//            success: function (data) {
//                console.log(data);
//                if (data.success) {
//                    LoadCompletedRoads();
//                    // $("#tbProposedRoadList").trigger('reloadGrid');
//                    $.unblockUI();
//                    alert(data.message);
//                }
//                else {
//                    alert(data.message);
//                }
//                $.unblockUI();
//            },
//            error: function (xht, ajaxOptions, throwError) {
//                alert(xht.responseText);
//                $.unblockUI();
//            }

//        });
//    }
//    else {
//        return false;
//    }

//}

function FinalizeEmargCorrectionRoad(urlparameter) {
    //alert("Line No : 775");
    //alert($("#ddlMaintTypeList option:selected").val());

    var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();

    if (confirm("Are you sure you want to 'Finalize' Road details ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: ddlMaintTypeCode == 2 ? "/MaintenanceAgreement/FinalizeRoadAfterEmargCorrectionPostDLP/" + urlparameter : "/MaintenanceAgreement/FinalizeRoadAfterEmargCorrection/" + urlparameter,
            type: "POST",
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.success) {
                    LoadCompletedRoads();
                    // $("#tbProposedRoadList").trigger('reloadGrid');
                    $.unblockUI();
                    alert(data.message);
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }

        });
    }
    else {
        return false;
    }

}


function AddorEditEmargCorrectionDetails(urlparameter) {

    //alert("Here")
    //  window.scrollTo(0, 0);

    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/MaintenanceAgreement/EditEmarg/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if (data == null || data == '') {
                alert("Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details. Then only proceed with these Correction Details.")
            }


            //if ($("#dvSearchParameter").is(":visible")) {

            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //    //$(this).next("#dvSearchParameter").slideToggle(300);
            //    $("#dvSearchParameter").slideToggle(300);
            //}

            //else {
            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            //    //$(this).next("#dvSearchParameter").slideToggle(300);
            //    $("#dvSearchParameter").slideToggle(300);
            //}


            $("#emargUpdateForm").show('slow');
            $("#emargUpdateForm").html(data);

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("Error occurred while processing your request.");
            return false;
        }

    })
}


function AddorEditEmargPostDLPCorrectionDetails(urlparameter) {

    //alert("Here")
    //  window.scrollTo(0, 0);

    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/MaintenanceAgreement/EditEmargPostDLP/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if (data == null || data == '') {
                alert("Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details. Then only proceed with these Correction Details.")
            }


            //if ($("#dvSearchParameter").is(":visible")) {

            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            //    //$(this).next("#dvSearchParameter").slideToggle(300);
            //    $("#dvSearchParameter").slideToggle(300);
            //}

            //else {
            //    $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            //    //$(this).next("#dvSearchParameter").slideToggle(300);
            //    $("#dvSearchParameter").slideToggle(300);
            //}


            $("#emargUpdateForm").show('slow');
            $("#emargUpdateForm").html(data);

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("Error occurred while processing your request.");
            return false;
        }

    })
}

// ViewDetailsAfterCorrection


function ViewDetailsAfterCorrection(urlparameter) {
    //alert("Line No:970");
    //alert($("#ddlMaintTypeList option:selected").val());
    var ddlMaintTypeCode = $('#ddlMaintTypeList option:selected').val();

    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: ddlMaintTypeCode == 2 ? '/MaintenanceAgreement/ViewEditEmargPostDLP/' + urlparameter : '/MaintenanceAgreement/ViewEditEmarg/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if (data == null || data == '') {
                alert("Details are not available for selected Road.")
            }

            $("#emargUpdateForm").show('slow');
            $("#emargUpdateForm").html(data);

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("Error occurred while processing your request.");
            return false;
        }

    })
}



function SearchDetails() {

    $('#tbProposedRoadList').setGridParam({
        url: '/MaintenanceAgreement/GetCompletedRoadList',
        datatype: 'json'
    });

    $('#tbProposedRoadList').jqGrid("setGridParam", { "postData": { sanctionedYear: $('#ddlFinancialYears option:selected').val(), blockCode: $('#ddlBlocks option:selected').val(), packageID: $('#ddlPackages option:selected').val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() } });
    $('#tbProposedRoadList').trigger("reloadGrid", [{ page: 1 }]);

}

function FormatColumnView(cellvalue, options, rowObject) {

    // return "<center><table><tr><td  style='border-color:white'><a href='#' title='View Agreement' onClick ='ViewAgreementDetails(\"" + cellvalue.toString() + "\");' >View</a></td></tr></table></center>";

    return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='View Maintenance Agreement' onClick ='ViewMaintenanceAgreementDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
}

function ViewMaintenanceAgreementDetails(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/MaintenanceAgreement/AddMaintenanceAgreementAgainstRoad/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvAddMaintenanceAgreementAgainstRoad").html(data);
            $('#accordion').show('slow');
            $('#dvAddMaintenanceAgreementAgainstRoad').show('slow');

            if ($("#dvSearchProposedRoad").is(":visible")) {
                $('#dvSearchProposedRoad').hide('slow');
            }
            $('#tbProposedRoadList').jqGrid("setGridState", "hidden");
            $.unblockUI();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


//-------------------New Added 22-12-2021----------------------


var StateSrrdsPIU;

$(document).ready(function () {
    $.unblockUI()


    $(function () {
        $("#ddlNodalAgency").trigger("change");
    });

    $("#btnViewBalanceSheetDetails").click(function () {

        //validation start        

        if ($("#rdYearly").is(":checked")) {
            if ($("#ddlBalYear").val() == -1) {
                alert("Please select Year.");
                return false;
            }
        }
        if ($("#rdMonthly").is(":checked")) {
            if ($("#ddlBalMonth").val() == 0) {
                alert("Please select Month.");
                return false;
            }
            if ($("#ddlBalYear").val() == 0) {
                alert("Please select Year.");
                return false;
            }
        }
        if ($("#ddlFundTypeBalSheet").val() == 0) {
            alert("Please select Fund Type.");
            return false;
        }

        if ($("#LevelIdBalSheet").val() == 5) {
            if ($("#ddlPIUList").val() == 0) {
                alert("Please select DPIU.");
                return false;
            }
        }




        //validation end

        var StateSrrdsPIU;
        var MonthlyYearly;

        if ($("#rdbState").is(":checked")) {
            StateSrrdsPIU = $("#rdbState").val();
        } else if ($("#rdbSRRDA").is(":checked")) {
            StateSrrdsPIU = $("#rdbSRRDA").val();
        } else if ($("#rdbAllDPIU").is(":checked")) {
            StateSrrdsPIU = $("#rdbAllDPIU").val();
        }

        if ($("#rdMonthly").is(":checked")) {
            MonthlyYearly = $("#rdMonthly").val();
        } else if ($("#rdYearly").is(":checked")) {
            MonthlyYearly = $("#rdYearly").val();
        }



        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

        $("#loadReport").html("");

        $("#loadReport").load("/AccountReports/Account/BalanceSheetReport/" +
            $("#LevelIdBalSheet").val() + "$" +
            StateSrrdsPIU + "$" +
            $("#ddlNodalAgency").val() + "$" +
            $("#ddlPIUList").val() + "$" +
            $("#ddlBalMonth").val() + "$" +
            $("#ddlBalYear").val() + "$" +
            "P" + "$" +
            MonthlyYearly
            ,
            $.unblockUI());
    });


    //STATE ddl change
    $("#ddlNodalAgency").change(function () {

        var adminNdCode = $("#ddlNodalAgency option:selected").val();

        $.ajax({
            type: 'POST',
            url: '/Account/PopulateDPIU?id=' + adminNdCode,
            async: false,
            cache: false,
            success: function (data) {
                $("#ddlPIUList").empty();
                $.each(data, function () {
                    $("#ddlPIUList").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            },
            error: function () {
                alert("Request can not be processed at this time.");
            }
        });
    });


    $("#rdoDpiuMonthlyAccount").click(function () {

        $("#ddlSrrdaMonthlyAccount").trigger("change");
        $("#ddlDpiuMonthlyAccount").show();
        $("#lblSelectDpiu").show();
    });

    $("#rdoStateMonthlyAccount").click(function () {
        $("#ddlDpiuMonthlyAccount").hide();
        $("#lblSelectDpiu").hide();
    });

    $("#rdoSrrdaMonthlyAccount").click(function () {
        $("#ddlDpiuMonthlyAccount").hide();
        $("#lblSelectDpiu").hide();
    });

    $("#rdbAllDPIU").click(function () {

        $("#ddlNodalAgency").trigger('change');
        $("#ddlPIUList").show('slow');
        $("#lblShowDPIU").show('slow');
    });

    $("#rdbAllDPIU").load(function () {

        //$("#ddlNodalAgency").trigger('change');
        $("#ddlPIUList").show();
        //$("#lblShowDPIU").show('slow');
    });

    $("#rdbState").click(function () {

        $("#ddlNodalAgency").trigger('change');
        $("#ddlPIUList").hide('slow');
        $("#lblShowDPIU").hide('slow');
    });

    $("#rdbSRRDA").click(function () {

        $("#ddlNodalAgency").trigger('change');
        $("#ddlPIUList").hide('slow');
        $("#lblShowDPIU").hide('slow');
    });



    $("#rdMonthly").click(function () {

        $("#trMonthYear").show();
        $("#trTempddlMonthShow").hide();

        if ($("#rdYearly").is(':checked')) {
            $("#rdYearly").attr('checked', false);
        }

        if ($("#LevelIdBalSheet").val() == 6 || $("#LevelIdBalSheet").val() == 4 || $("#LevelIdBalSheet").val() == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").show();
            $("#ddlBalYear").show();
            $("#trlblMonth").show();
            $("#trddlMonth").show();
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/AccountReports/Account/PopulateYears/");

    });

    $("#rdYearly").click(function () {

        $("#trMonthYear").show('slow');
        $("#trlblMonth").hide();
        $("#trddlMonth").hide();
        $("#trTempddlMonthShow").show();

        trTempddlMonthShow

        if ($("#rdMonthly").is(':checked')) {
            $("#rdMonthly").attr('checked', false);
        }

        if ($("#LevelIdBalSheet").val() == 6 || $("#LevelIdBalSheet").val() == 4 || $("#LevelIdBalSheet").val() == 5) {
            HideAllMonthYears();
            $("#ddlBalMonth").hide('slow');
            $("#ddlBalYear").show('slow');
        }

        FillInCascadeDropdown(null, "#ddlBalYear", "/AccountReports/Account/PopulateFinancialYears/");
    });
    function FillInCascadeDropdown(map, dropdown, action) {

        $(dropdown).empty();
        $.post(action, map, function (data) {

            $.each(data, function () {
                if (this.Selected == true) { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
                else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
            });
        }, "json");
    }

    function HideAllMonthYears() {
        $("#ddlBalMonth").hide();
        $("#ddlBalYear").hide();
        $("#ddlYear").hide();
    }
});
