
/// <reference path="../../areas/eformarea/scripts/jquery-3.6.0.js" />
/// <reference path="../../areas/eformarea/scripts/jquery-3.6.0.intellisense.js" />

//Newly Added By Hrishikesh on 09-05-2023 To show Monitor List(Monitor's Exam) [cqc, cqcAdmin, sqc logins]



$(document).ready(function () {
    //alert("ready")

    //if login with sqm bydefault show state's
    if ($("#nqmSqmDropdownListId").val() == "S") {
        $(".stateRow").show();
    }


    $("#nqmSqmDropdownListId").change(function () {
        //alert('change');
        if ($("#nqmSqmDropdownListId").val() == "S") {
            $(".stateRow").show();
        }
        else {
            $(".stateRow").hide();
            $("#stateListId").val('0');
        }

    });


    function getData() {
        /*alert("zhgdf");*/
        var monitorType = $("#nqmSqmDropdownListId").val();
        var stateCode = $("#stateListId").val();
        var str = monitorType + "$" + stateCode;



        //To load JQGrid  on div
        $("#divMonitorExamDetails").show();
        jQuery("#tblMonitorExamDetailsList").jqGrid('GridUnload');
        //alert("save 4");
        jQuery("#tblMonitorExamDetailsList").jqGrid(
            {
                url: '/QualityMonitoring/GetmonitorDetailsListJSON/?data=' + str,
                datatype: 'json',
                mtype: 'POST',
                async: false,
                cache: false,
                multiselect: true,
                colNames: ["ID", "Monitor Name", "Email", "Mobile Number"],
                colModel: [
                    { name: "ID", index: "id", height: "auto", width: 100, align: "center", search: false, hidden: true },
                    { name: "Monitor Name", index: "monitor_name", height: "auto", width: 100, align: "center", search: false },
                    { name: "Email", index: "email", height: "auto", width: 100, align: "center", search: false },
                    { name: "Mobile Number", index: "phone_number", width: 100, align: "center", search: false },
                ],
                pager: jQuery("#divPagerNav").width(20),
                rowNum: 20000,
                //rowList: [25, 35, 45],
                viewrecords: true,
                recordtext: "{2} records found",
                sortname: "Monitor_Name",
                sortorder: "asc",
                caption: "Monitor Details List",
                height: '400',
                autowidth: true,
                hidegrid: true,
                rownumbers: true,
                cmTemplate: { title: false },
                //onSelectRow: function (rowid, status) {
                //    if (status) {
                //        selectedRecords.push(rowid);
                //    }
                //    else {
                //        var index = selectedRecords.indexOf(rowid);
                //        if (index !== -1) {
                //            selectedRecords.splice(index, 1);
                //        }
                //    }
                //},
                loadComplete: function (data) {
                    //alert("complete");
                    $("#jqgh_tblMonitorExamDetailsList_rn").html("Sr.<br/>No.");  //-- after loading grid Sr.No Will add--

                    //desable checkbox for all one's
                    //-----$("#cb_tblMonitorExamDetailsList").attr("disabled", true);

                    //// Restore the selection state after the grid is loaded or reloaded
                    //for (var i = 0; i < selectedRecords.length; i++) {
                    //    $("#grid").jqGrid("setSelection", selectedRecords[i], false);
                    //}

                    // $("#allDiv").hide("slow");  //----------------
                    $("#generateMonitorId").show();

                    //$('#gview_tblMonitorExamDetailsList').setAttribute('style', 'width: 60%');

                    //var windowWidth = window.innerWidth;
                    //var grid = $("#tblMonitorExamDetailsList");
                    //grid.setGridWidth(windowWidth - 60);
                },
                loadError: function (xhr, status, error) {
                    //alert("error");
                    if (xhr.responseText == "session expired") {

                        alert(xht.responseText);
                        window.location.href = "Login/login";
                    }
                    else {

                        alert("Some Problem Occured. Please Try Again");
                    }
                },
                onHeaderClick: function () {
                }


            }).trigger("reloadGrid");//end jqgrid load
    }

    $("#btnlistMonitorNamesId").click(function () {
        //alert("save cliick")
        if ($("#nqmSqmDropdownListId").val() == "S") {
            if ($("#stateListId").val() == "" || $("#stateListId").val() == 0) {
                alert("Please select a state.");
                return false;
            }
        }
        if ($("#nqmSqmDropdownListId").val() == "" || $("#nqmSqmDropdownListId").val() == 0) {
            alert("Please select Monitor Type.");
            return false;
        }
        //alert("getData start")

        ////empty the array always while fetching list
        //if (selectedRecords.length > 0)
        //{
        //    selectedRecords = [];
        //}
        getData();//calling function for jqGrid
        //$("#tblMonitorExamDetailsList").trigger("reloadGrid"); //---16-05-23

    });

    $("#backToMonitorListId").click(function () {

        //$("#tblMonitorExamDetailsList").trigger("reloadGrid");
        $("#pageDiv").show();
        $('#monitorReportId').hide();
        $("#backToMonitorListId").hide();
    });
});//end ready function

// ------------------- Srishti --------------

// Fetch data in Array
$('#generateMonitorId').click(function () {
    var submitArray = [];

    var selRowIds = jQuery('#tblMonitorExamDetailsList').jqGrid('getGridParam', 'selarrrow');

    if (selRowIds.length > 0) {
        for (var i = 0; i < selRowIds.length; i++) {
            //alert(selRowIds);
            submitArray.push(selRowIds[i]);
        }
        //alert(submitArray);
        CreateReport(submitArray);
        //CreateReport(selectedRecords);
    }
    else
        alert("No records selected");
});

function CreateReport(submitArray) {
    if (confirm("Are you sure to generate list for the selected monitors ? ")) {
        //alert("submitArray-" + submitArray.length);
        //alert("selectedRecords-" + selectedRecords);

        //$('#abcdef').load("/QualityMonitoring/CreateReportForMoniors?submitarray=" + submitArray, function () { alert("hello"); });

        $.ajax({
            url: '/QualityMonitoring/CreateReportForMoniors',
            type: 'POST',
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            //processData: false,
            data: JSON.stringify({ 'submitarray': submitArray }),
            success: function (data) {
                //alert("success!! " + data)

                $("#pageDiv").hide();
                $('#monitorReportId').show();
                $('#monitorReportId').html(data);
                $("#backToMonitorListId").show('slow');

            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });

    }
    else {
        return;
    }

}

