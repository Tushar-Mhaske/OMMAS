var isValid;
 
$(document).ready(function () {
 
    //disable enter key
    $(":input").bind('keypress', function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    })
   
        
  

    var IMS_ROAD_CODE = $("#prRoadCode").val();

    //LoadHabitationDetailsToMap();
    LoadHabitationGrid();

    // map habitation to Road
    $('#btnSaveHabitation').click(function (e) {


        if (confirm("Are you sure you want to map Habitation Details?")) {
            //var blockCodes = $("#tbHabitationRoadList").jqGrid('getGridParam', 'selarrrow');// : ("#mapHabitationList").jqGrid('getGridParam', 'selarrrow');
            //if (blockCodes != '') {
             
            if (validate(arrHabitations)) {
                //$('#EncryptedHabCodes').val(blockCodes);
                $('#EncryptedHabCodes').val(arrHabitations);
                console.log(arrHabitations);
                blockPage();

                $.ajax({
                    url: "/Execution/MapHabitations",
                    type: "POST",
                    dataType: "json",
                    data: $("#frmListHabitation").serialize(),
                    success: function (data) {
                        unblockPage();
                        if (!(data.success)) {
                            //alert("Habitations not added successfully.");
                            alert(data.message);
                        }
                        else {
                            alert(data.message);

                            //resetHabitationList();
                            //CloseExecutionDetails();

                            $("#accordion").hide('slow');
                            $("#divAddExecution").hide('slow');
                            $("#tbExecutionList").jqGrid('setGridState', 'visible');

                            //alert($('#EncryptedRoadCode').val());
                            AddHabitationDetails($('#EncryptedRoadCode').val());
                            $("#tbHabitationRoadList").jqGrid('resetSelection');
                            $("#tbMappedHabitationRoadList").trigger('reloadGrid');
                            $("#tbHabitationRoadList").trigger('reloadGrid');
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        unblockPage();
                    }
                });
            }
            else {
                alert('Please select Habitations to Map.');
            }
        }
        else {
            return false;
        }
    });

    // map habitation to core network
    $('#btnSaveCluster').click(function (e) {

        if (confirm("Are you sure you want to map Cluster Details?")) {

            if (parseInt($('#ddlCluster option:selected').val()) <= 0) {
                alert('Please select a Cluster to map');
                return false;
            }
            //$('#EncryptedHabCodes').val(arrHabitations);

            blockPage();
            $.ajax({
                url: "/Execution/MapClusterHabitations",
                type: "POST",
                dataType: "json",
                data: { roadCode: $('#prRoadCode').val(), clusterCode: $('#ddlCluster option:selected').val() },//$("#frmListHabitation").serialize(),
                success: function (data) {
                    unblockPage();
                    if (!(data.success)) {
                        //alert("Habitations not added successfully.");
                        alert(data.message);
                    }
                    else {
                        alert(data.message);

                        //resetHabitationList();
                        //CloseExecutionDetails();

                        $("#accordion").hide('slow');
                        $("#divAddExecution").hide('slow');
                        $("#tbExecutionList").jqGrid('setGridState', 'visible');

                        //alert($('#EncryptedRoadCode').val());
                        AddHabitationDetails($('#EncryptedRoadCode').val());
                        $("#tbHabitationRoadList").jqGrid('resetSelection');
                        $("#tbMappedHabitationRoadList").trigger('reloadGrid');
                        $("#tbHabitationRoadList").trigger('reloadGrid');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    unblockPage();
                }
            });
        }
        else {
            return false;
        }
    });

    // cancel button click
    $('#btnMapCancel').click(function (e) {

      //  alert("Map Cancel")



        CloseExecutionDetails();

        CloseProposalDetails();
        if ($("#divAddForm").is(":visible")) {
            $('#divAddForm').hide('slow');
        }

        $('#btnSearchView').trigger('click');

        $('#tbExecutionList').jqGrid("setGridState", "visible");
        $('#idFilterDiv').trigger('click');
        $("#tbExecutionList").trigger('reloadGrid');
    });


    //------------------ MultiSelect Code ----------------------------




    var arrHabitations = [];
    var arrHabCnt = 0;
    selectedNameVal = 0; //for selected Monitor Value
    //Selected items in Habitation List
    $('#HABITATION_LIST').multiSelect({
        keepOrder: true,
        selectableHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Sanctioned Habitations</strong></div>",
        selectionHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Connected Habitations</strong></div>",

        afterInit: function (values) {
            $('#HABITATION_LIST').multiSelect('deselect_all');
        },
        afterSelect: function (values) {
            arrHabitations.push(values);
        },
        afterDeselect: function (values) {
            arrHabitations.pop(values);
        }

    });


    $('#HABITATION_LIST').multiSelect('refresh');


    //-------------------------------------------------------------

    $("#rdbCluster").click(function () {
        $('.tdHabs').hide();
        $('.tdCluster').show('slow');

        if ($('#ddlCluster').children().length == 1) {
            jQuery('#tdDate').hide();
        } else {
            jQuery('#tdDate').show();
        }

        $('#btnSaveHabitation').hide('slow');
        $('#btnSaveCluster').show('slow');

        //PopulateCluster($("#PLAN_CN_ROAD_CODE").val(), $("#prRoadCode").val());

        //$("#MAST_CLUSTER_CODE").multiselect({
        //    //header: "Select Habitations",
        //    nonSelectedText: "Select Habitations",
        //    minWidth: 350,
        //    height: 'auto',
        //    //position: absolute,
        //    position: ({
        //        my: "center top",
        //        at: "center bottom",
        //        collision: 'flipfit',
        //        within: window
        //    }),
        //    display: 'inline-flex',
        //});
        //$("#MAST_CLUSTER_CODE").multiselect("uncheckAll");
        //$(".ui-multiselect").trigger('click');

        //if ($("#MAST_CLUSTER_CODE > option").length == 0) {
        //    $("#MAST_CLUSTER_CODE").multiselect('disable');
        //    $("#showHabError").html("No Cluster to Map.");
        //}

    });

    $(function () {
        if ($("#rdbHabitation").val() == "H") {
           // alert($("#HABITATION_LIST").children().length);
            if (($("#HABITATION_LIST").children().length) == 0) {
                $('#tdDate').hide();
            }
        }
        else {
            
        }
    })

    $("#rdbHabitation").click(function () {

        if (($("#HABITATION_LIST").children().length) == 0) {
            $('#tdDate').hide();
        }
    else {
            $('#tdDate').show();
       }


        $('.tdCluster').hide();
        $('.tdHabs').show('slow');
        if ($("#MAST_HAB_CODE > option").length == 0) {
            //$("#MAST_HAB_CODE").multiselect('disable');
            $("#showHabError").html("No Habitations to Map.");
        }
 
        $('#btnSaveHabitation').show('slow');
        $('#btnSaveCluster').hide('slow');

        //PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_RODE_CODE").val());
    });

    //$("#rdbHabitation").trigger('click');
    $('.tdCluster').hide();
    $('.tdHabs').show('slow');
});

function LoadHabitationDetailsToMap() {

    jQuery("#tbHabitationRoadList").jqGrid('GridUnload');
    jQuery("#tbHabitationRoadList").jqGrid({
        url: '/Execution/GetHabitationListToMap',
        datatype: "json",
        mtype: "POST",
        postData: { prRoadCode: $('#prRoadCode').val(), },
        colNames: ['Habitation Name', 'Village', 'Total Population'],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 180, sortable: true, align: "center" },
                  ],
        pager: jQuery('#pagerHabitationRoadList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_HAB_NAME',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Habitation List",
        height: '130px',
        hidegrid: true,
        rownumbers: true,
        multiselect: true,
        loadComplete: function (data) {

            //if (data["records"] == 0) {
            //    $("#btnSaveHabitation").hide();
            //    $("#divError").show("slow");

            //}
            //else {
            //    $("#divError").hide("slow");
            //    $("#btnSaveHabitation").show();
            //}
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
        }
    });
}

function LoadHabitationGrid() {

    jQuery("#tbMappedHabitationRoadList").jqGrid('GridUnload');
    jQuery("#tbMappedHabitationRoadList").jqGrid({
        url: '/Execution/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        postData: { prRoadCode: $('#prRoadCode').val() },
        colNames: ['Name of Habitation', 'Village', 'Total Population', 'Date', 'Cluster','Delete' ],
        colModel: [
                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 170, align: "left", sortable: true },
                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 170, sortable: true, align: "center" },
                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 170, sortable: true, align: "center" },
                            { name: 'EXEC_PROGRESS_DATE', index: 'EXEC_PROGRESS_DATE', width: 150, sortable: false, align: "center" },
                            { name: 'Cluster', index: 'Cluster', width: 50, sortable: true, align: "center", hidden: true },
                            { name: 'Delete', index: 'Delete', width: 80, sortable: true, align: "center", hidden: false },
        ],
        pager: jQuery('#pagerMappedHabitationRoadList'),
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_HAB_CONNECTED_ORDER',
        sortorder: "asc",
        grouping: true,
        groupingView: {
            //    groupText: ["<span style='font-weight:bold'>{0}</span>"],
            groupColumnShow: [false],
            groupField: ['Cluster'],
            groupOrder: ['desc'],
            groupSummary: [true]
        },
        caption: "&nbsp;&nbsp; Connected Habitation List",
        height: '180px',
        width: 'auto',
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {

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
        }
    });
}

//Added by pradip patil 09/03/17
function deleteHabitaion(urlparameter) {
   
    var result = confirm('Are you sure to delete the habitation?');

    if (result)
    {
        $.ajax({
            url: '/Execution/DeleteHabitaion',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { urlparameter: urlparameter },
            success: function (jsonData) {
                if (jsonData.success) {
                    alert('Habitation details deleted successfully');
                    jQuery("#tbMappedHabitationRoadList").trigger('reloadGrid');
                    $('#HABITATION_LIST').append("<option value='" + jsonData.Deleted.Value + "'>" + jsonData.Deleted.Text + "</option>");
                    $('#HABITATION_LIST').multiSelect('refresh');

                    $("#accordion").hide('slow');
                    $("#divAddExecution").hide('slow');
                    $("#tbExecutionList").jqGrid('setGridState', 'visible');

                    //alert($('#EncryptedRoadCode').val());
                    AddHabitationDetails($('#EncryptedRoadCode').val());
                    $("#tbHabitationRoadList").jqGrid('resetSelection');
                    $("#tbMappedHabitationRoadList").trigger('reloadGrid');
                    $("#tbHabitationRoadList").trigger('reloadGrid');

                } else {
                    alert('Habitation details not deleted.');
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    } else
    {
        return;
    }
} 
//resets comma seperated District List
function resetHabitationList() {
    arrHabitations = [];
    $('#HABITATION_LIST').multiSelect('deselect_all');
    $('#HABITATION_LIST').multiSelect('refresh');
    $("#ASSIGNED_HABITATION_LIST").val("");
}

function validate(arrHabitations) {
    //arrDistricts.forEach(alert(elem));
    if (arrHabitations.length == 0) {
        $("#showHabitationError").html("Select at least one of the Habitations");
        $("#showHabitationError").addClass("field-validation-error");
        return false;
    }

   
    $("#showDistrictError").html("");
    $("#showDistrictError").removeClass("field-validation-error");

    var assignedDist = "";
    for (var i = 0; i < arrHabitations.length; ++i) {
        if (i == 0) {
            $("#ASSIGNED_HABITATION_LIST").val(arrHabitations[i]);
        }
        else {
            $("#ASSIGNED_HABITATION_LIST").val($("#ASSIGNED_HABITATION_LIST").val() + "," + arrHabitations[i]);
        }
    }
    $('#EncryptedHabCodes').val($("#ASSIGNED_HABITATION_LIST").val());
    return true;
}


function PopulateCluster(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE) {

    $("#MAST_CLUSTER_CODE").empty();
    $.ajax({
        url: '/Proposal/GetHabitationCluster/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { PLAN_CN_ROAD_CODE: PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        success: function (jsonData) {

            if (jsonData.length > 0) {

                for (var i = 0; i < jsonData.length; i++) {
                    $("#MAST_CLUSTER_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                //$("#MAST_CLUSTER_CODE").multiselect("enable");
                $('#btnAdd').removeAttr('disabled');
                $("#showHabError").html("");
                $("#showHabError").removeClass("field-validation-error");

            } else {
                //$("#MAST_CLUSTER_CODE").multiselect("disable");
                $("#showHabError").html("No Cluster to Map.");
                $("#showHabError").addClass("field-validation-error");
                $("#btnAdd").attr('disabled', 'disabled');
            }

            //$("#MAST_CLUSTER_CODE").multiselect("uncheckAll");

            //$("#MAST_CLUSTER_CODE").multiselect('refresh');
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}