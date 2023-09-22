$(document).ready(function () {


    if ($("#IMS_ISCOMPLETED").val() == "H") {
        $("#divHabStatus").show('slow');
    }
    else {
        $("#divHabStatus").hide('hide');
    }


    $("#MAST_HAB_CODE").multiselect({
        //header: "Select Habitations",
        nonSelectedText: "Select Habitations",
        minWidth: 350,
        height: 'auto'
    });
    $("#MAST_HAB_CODE").multiselect("uncheckAll");

    $("#MAST_CLUSTER_CODE").multiselect({
        //header: "Select Habitations",
        nonSelectedText: "Select Habitations",
        minWidth: 350,
        height: 'auto'
    });
    $("#MAST_CLUSTER_CODE").multiselect("uncheckAll");

    


    $("#MAST_HAB_CODE").change(function () {

        if ($("#MAST_HAB_CODE").val() != "") {
            $.ajax({
                url: '/Proposal/GetHabitationDetails/',
                type: "POST",
                async: false,
                cache: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                data: { selectedHabitation: $(this).val() },
                success: function (response) {
                    $("#txtPopulation").val(response.habpopulation);

                    unblockPage();
                }
            });
        }
        else {
            $("#txtPopulation").val("");
        }
    });

    PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_RODE_CODE").val());
    //PopulateCluster($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_RODE_CODE").val());

    $("#rdbCluster").click(function () {
        $('.habitation').hide();
        $('.cluster').show('slow');
        if ($("#MAST_CLUSTER_CODE > option").length == 0) {
            $("#MAST_CLUSTER_CODE").multiselect('disable');
            $("#showHabError").html("No Cluster to Map.");
        }
        PopulateCluster($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_RODE_CODE").val());
    });

    $("#rdbHabitations").click(function () {
        $('.cluster').hide();
        $('.habitation').show('slow');
        if ($("#MAST_HAB_CODE > option").length == 0) {
            $("#MAST_HAB_CODE").multiselect('disable');
            $("#showHabError").html("No Habitations to Map.");
        }
        PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_RODE_CODE").val());
    });

});
function AddCluster()
{
    //alert("Test");
    if (validateCluster()) {

        if ($("#MASTER_HABITATION").val() == 0) {
            alert("Please Select Habitation");
            return false;
        }
        else {

            if ($('#frmMapHabitation').valid()) {

                $.ajax({
                    url: "/Proposal/AddCluster/",
                    type: "POST",
                    async: false,
                    cache: false,
                    data: $("#frmMapHabitation").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    //data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), MAST_HAB_CODE: $("#MASTER_HABITATION").val() },
                    success: function (data) {
                        if (data.success) {
                            $("#CLUSTER_CODES_LIST").val("");
                            $("#txtPopulation").val("");
                            $('#tbHabitation').trigger("reloadGrid");
                            PopulateCluster($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_ROAD_CODE").val());
                            $("#divHabStatus").hide('slow');
                            alert('habitation added successfully.');
                        }
                        else if(data.message != ""){
                            alert(data.message);
                        }
                        else {
                            alert("There is an error processing your request.");
                        }

                        unblockPage();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(thrownError);
                        unblockPage();
                    }
                });
            }
        }

    }
}
function FinalizeHabitations() {
    var RequiredTotalPopulation = 500;

    if ($("#MAST_STATE_TYPE").val() == "N" || $("#MAST_STATE_TYPE").val() == "H" || $("#MAST_STATE_TYPE").val() == "X") {
        RequiredTotalPopulation = 250;
    }

    if ($("#MAST_IAP_DISTRICT").val() == "Y") {
        RequiredTotalPopulation = 250;
    }

    if ($("#MAST_IS_TRIBAL").val() == "Y") {
        RequiredTotalPopulation = 250;
    }

    var Population = $('#tbHabitation').jqGrid('getCol', 'Population', false);
    var clusters = $('#tbHabitation').jqGrid('getCol', 'Cluster', false);

    var totalPopulation = 0;

    if (Population.length == 0) {
        alert("Please Map Atleast One Habitation, to Finalize Habitation.");
        return false;
    }
    else {
        for (i = 0 ; i < Population.length; i++) {
            totalPopulation = (Population[i]);

            if (totalPopulation < RequiredTotalPopulation && clusters[i] == "Cluter Not Allocated") {
                alert("Population of Habitation must be Greator than " + RequiredTotalPopulation + ".\nHabitation can not be Finalized.");
                return false;
            }
        }
    }

    $.ajax({
        url: '/Proposal/FinalizeHabitaion/',
        type: "POST",
        async: false,
        cache: false,
        data: $("#frmMapHabitation").serialize(),
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        // data: { IMS_PR_ROAD_CODE: $(IMS_PR_ROAD_CODE).val() , value:Math.random() },
        success: function (response) {
            unblockPage();
            if (response.Success) {
                alert("Habitations Finalized Successfully.");
                $("#divHabStatus").html("");
                $("#divHabStatus").html("Habitation Finalized Successfully");

                $("#divHabStatus").show('slow');
                $("#idFinalizeHabitaitons").hide('slow');
                $("#tbHabitation").trigger('reloadGrid');

            } else {
                alert(response.errorMessage);
            }
        }
    });
}
// show add habitations form
function AddHabitation() {
    //alert("Test");
    if (validateHabitations()) {

        if ($("#MASTER_HABITATION").val() == 0) {
            alert("Please Select Habitation");
            return false;
        }
        else {

            if ($('#frmMapHabitation').valid()) {

                $.ajax({
                    url: "/Proposal/AddHabitation/",
                    type: "POST",
                    async: false,
                    cache: false,
                    data: $("#frmMapHabitation").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    //data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), MAST_HAB_CODE: $("#MASTER_HABITATION").val() },
                    success: function (data) {
                        if (data.success) {
                            $("#HAB_CODES_LIST").val("");
                            $("#txtPopulation").val("");
                            $('#tbHabitation').trigger("reloadGrid");
                            PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_ROAD_CODE").val());
                            $("#divHabStatus").hide('slow');
                            alert('habitation added successfully.');
                        }
                        else
                        {
                            alert(data.message);

                            $("#HAB_CODES_LIST").val("");
                            $("#txtPopulation").val("");
                            $('#tbHabitation').trigger("reloadGrid");
                            PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), $("#IMS_PR_ROAD_CODE").val());
                            $("#divHabStatus").hide('slow');

                            
                         //   alert("There is an error processing your request.");
                        }

                        unblockPage();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(thrownError);
                        unblockPage();
                    }
                });
            }
        }

    }
}

// Populate the Grid 
function ShowHabitations(IMS_PR_ROAD_CODE) {

    jQuery("#tbHabitation").jqGrid({

        url: '/Proposal/GetHabitationList',
        datatype: "json",
        mtype: "POST",
        colNames: ['HabCode', 'Block','Village', 'Habitation', "SC/ST Population", "Population", /*"Habitation Direct", "Habitation Verified",*/ "Delete", "Create New Cluster", "Cluster", "Edit Cluster", "ClusterCode"],
        colModel: [
                            //{ name: 'srno', index: 'srno', width: 40, sortable: false, align: "center", hidden: false },
                            { name: 'HabCode', index: 'HabCode', width: 100, sortable: false, align: "center", hidden: true },
                            { name: 'Block', index: 'Block', width: 150, sortable: false, align: "center" },
                            { name: 'Village', index: 'Village', width: 150, sortable: false, align: "center" },
                            { name: 'Habitation', index: 'Habitation', width: 140, sortable: false, align: "center" },
                            { name: 'SCSTPopulation', index: 'EditCluster', width: 120, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum' },
                            { name: 'Population', index: 'Population', width: 100, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum' },
                            //{ name: 'HabDirect', index: 'HabDirect', width: 100, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum', hidden: ($("#PMGSYScheme").val() == 4) ? false : true },
                            //{ name: 'HabVerified', index: 'HabVerified', width: 100, sortable: false, align: "center", formatter: 'interger', summaryType: 'sum', hidden: ($("#PMGSYScheme").val() == 4) ? false : true },
                            { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center" },
                            { name: 'CreateCluster', index: 'CreateCluster', width: 130, sortable: false, align: "center", hidden: true },
                            { name: 'Cluster', index: 'Cluster', width: 130, sortable: true, align: "center", hidden: true },
                            { name: 'EditCluster', index: 'EditCluster', width: 150, sortable: false, align: "center", edittype: "select",hidden:true },
                            { name: 'ClusterCode', index: 'ClusterCode', width: 10, align: "center", hidden: true },
        ],
        pager: jQuery('#dvHabitationPager'),
        rowList: [25, 40, 50],
        rowNum: 50,
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,        
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Habitation Details",
        height: 'auto',
        width: 'auto',
        sortname: 'Habitation',
        sortorder: 'asc',
        //grouping: true$("#PMGSYScheme").val() == 2,
        //grouping: $("#PMGSYScheme").val() == 2 ? false : true,
        grouping: true,
        groupingView: {
            //    groupText: ["<span style='font-weight:bold'>{0}</span>"],
            groupColumnShow: [false],
            groupField: ['Cluster'],
            groupOrder: ['desc'],
            groupSummary: [true]
        },

        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {

            if ($("#PMGSYScheme").val() == 2) {
                $("#tbHabitation").hideCol("EditCluster");
                //$("#tbHabitation").setGridParam('grouping', false);
            }

            // Enable or Disable Grouping 
            var Clusters = $('#tbHabitation').jqGrid('getCol', 'ClusterCode', false);
            var flag = 0;

            var count = 0;

            $("#tbHabitation select").each(function () {
                $(this).val(Clusters[count]);
                ++count;
            });



            if ($('#IsStageTwoProposal').val() == "True") {

                var myGrid = $('#tbHabitation');
                myGrid.jqGrid('hideCol', 'Delete');
                myGrid.jqGrid('hideCol', 'CreateCluster');
                myGrid.jqGrid('hideCol', 'EditCluster');
                return false;
            }
            $("#divHabitation #dvHabitationPager").css({ height: '31px' });

            $("#dvHabitationPager_left").html("<input type='button' id='idFinalizeHabitaitons' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeHabitations();return false;' value='Finalize Habitations'/>")
            if ($("#IMS_ISCOMPLETED").val() == "H") {
                $("#idFinalizeHabitaitons").hide('slow');
            }
            else {
                $("#idFinalizeHabitaitons").show('slow');
            }

            // Enable or Disable Grouping 
            //var Clusters  = $('#tbHabitation').jqGrid('getCol', 'Cluster', false);
            //var flag = 0;
            //for (i = 0 ; i < Clusters.length; i++) {
            //    //HabitationCodes[i];
            //    if (Clusters[i] != "Cluster Not Allocated") {
            //        flag = 1;                   
            //    }
            //}


            //if (flag == 1){
            //    doGrouping(true);
            //}

            //if (flag == 0) {
            //    doGrouping(false);
            //    $("#tbHabitation").hideCol("Cluster");
            //    $("#tbHabitation").hideCol("EditCluster");
            //}



            var selected = false;
            $("#tbHabitation input:checkbox").each(function () {
                if (!($(this).is(':disabled'))) {
                    selected = true;
                    return false; // break the Loop
                }
            });
            if (!selected) {
                $("#CreateCluster").attr("disabled", "disabled");
            }

            $("#CreateCluster").click(function () {

                var clusterCodes = "";
                var Clusterselected = false;

                $("#tbHabitation input:checkbox").each(function () {
                    if ($(this).attr('checked') && !$(this).is(':disabled')) {
                        Clusterselected = true;
                        return false;// break the Loop
                    }
                });

                $("#tbHabitation input:checkbox").each(function () {
                    if ($(this).attr('checked') && !$(this).is(':disabled')) {

                        if (clusterCodes == "") {
                            clusterCodes += $(this).val();
                        } else {
                            clusterCodes += "$" + ($(this).val());
                        }
                    }

                });

                if (!Clusterselected) {
                    alert('Please check atleast one cluster');
                }
                else {
                    $.ajax({
                        url: '/Proposal/CreateCluster',
                        type: "POST",
                        cache: false,
                        beforeSend: function () {
                            blockPage();
                        },
                        data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, CLUSTER_CODES: clusterCodes },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {
                            if (response.Success) {
                                $("#divHabStatus").hide('slow');
                                $("#IMS_ISCOMPLETED").val('E');
                                $('#tbHabitation').trigger("reloadGrid");
                            }
                            else {

                            }
                            unblockPage();
                        }
                    });
                }

            });
            $("#EditCluster").click(function () {
                var clusterCodes = "";
                var selectedHab = false;

                $("#tbHabitation select").each(function () {

                    if (parseInt($(this).val()) > 0 || parseInt($(this).val()) == -1) {
                        selectedHab = true;
                        return false; // break the Loop
                    }
                });

                if (selectedHab) {

                    $("#tbHabitation select").each(function () {

                        if ($(this).is(':disabled') == false) {
                            if (clusterCodes == "") {
                                clusterCodes += $(this).val();
                            } else {
                                clusterCodes += "," + ($(this).val());
                            }
                        }
                    });

                    var HabitationCodes = $('#tbHabitation').jqGrid('getCol', 'HabCode', false);
                    var ArrHabitationCodes;
                    for (i = 0 ; i < HabitationCodes.length; i++) {
                        if (i == 0) {
                            ArrHabitationCodes = HabitationCodes[i];
                        }
                        else {
                            ArrHabitationCodes = ArrHabitationCodes + ',' + HabitationCodes[i];
                        }
                    }
                    $.ajax({
                        url: '/Proposal/UpdateCluster',
                        type: "POST",
                        cache: false,
                        beforeSend: function () {
                            blockPage();
                        },
                        data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, IMS_HAB_CODES: ArrHabitationCodes, CLUSTER_CODES: clusterCodes },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {
                            if (response.Success) {
                                $("#divHabStatus").hide('slow');
                                $("#IMS_ISCOMPLETED").val('E');
                                $('#tbHabitation').trigger("reloadGrid");
                            }
                            else {
                                alert(response.ErrorMessage);
                            }
                            unblockPage();
                        }
                    });
                }
                else {
                    alert("Please select at least one cluster.");
                }

            });
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }

    });//.navGrid('#dvHabitationPager', { edit: false, add: false, del: false, search: false,refresh :false })
    //.navButtonAdd('#dvHabitationPager',{
    //    caption: "Finalize",

    //    onClickButton: function () {
    //        alert("Adding Row");
    //    },
    //    position: "last"
    //});

}

function doGrouping(isGrouping) {
    alert("called");

    if (!isGrouping) {
        jQuery("#tbHabitation").jqGrid('groupingRemove');
    }
    else {
        jQuery("#tbHabitation").jqGrid('groupingGroupBy', "Cluster", {
            groupOrder: ['desc'],
            groupColumnShow: [false],
            groupCollapse: false
        });
    }
}

function PopulateHabitation(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE) {
    //$('#MAST_HAB_CODE').children('option:not(:first)').remove();
    $("#MAST_HAB_CODE").empty();
    //alert($("#IMS_PR_ROAD_CODE").val());
    $.ajax({
        url: '/Proposal/GetHabitations/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { PLAN_CN_ROAD_CODE: PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), value: Math.random() },
        success: function (jsonData) {

            if (jsonData.length > 0) {

                for (var i = 0; i < jsonData.length; i++) {
                    $("#MAST_HAB_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                $("#MAST_HAB_CODE").multiselect("enable");
                $('#btnAdd').removeAttr('disabled');
                $("#showHabError").html("");
                $("#showHabError").removeClass("field-validation-error");

            } else {
                $("#MAST_HAB_CODE").multiselect("disable");
                $("#showHabError").html("No Habitations to Map.");
                $("#showHabError").addClass("field-validation-error");
                $("#btnAdd").attr('disabled', 'disabled');
            }

            $("#MAST_HAB_CODE").multiselect("uncheckAll");

            $("#MAST_HAB_CODE").multiselect('refresh');
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}
function PopulateCluster(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE) {
    //$('#MAST_HAB_CODE').children('option:not(:first)').remove();
    $("#MAST_CLUSTER_CODE").empty();
    //alert($("#IMS_PR_ROAD_CODE").val());
    $.ajax({
        url: '/Proposal/GetHabitationCluster/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { PLAN_CN_ROAD_CODE: PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val(), value: Math.random() },
        success: function (jsonData) {

            if (jsonData.length > 0) {

                for (var i = 0; i < jsonData.length; i++) {
                    $("#MAST_CLUSTER_CODE").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                $("#MAST_CLUSTER_CODE").multiselect("enable");
                $('#btnAdd').removeAttr('disabled');
                $("#showHabError").html("");
                $("#showHabError").removeClass("field-validation-error");

            } else {
                $("#MAST_CLUSTER_CODE").multiselect("disable");
                $("#showHabError").html("No Cluster to Map.");
                $("#showHabError").addClass("field-validation-error");
                $("#btnAdd").attr('disabled', 'disabled');
            }

            $("#MAST_CLUSTER_CODE").multiselect("uncheckAll");

            $("#MAST_CLUSTER_CODE").multiselect('refresh');
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}

function DeleteHabitation(IMS_PR_RODE_CODE, IMS_HAB_CODE) {
    if (confirm("Are you sure to UnMap the Habitation ?")) {
        $.ajax({
            url: "/Proposal/UnMapHabitation/",
            type: "POST",
            async: false,
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: IMS_PR_RODE_CODE, MAST_HAB_CODE: IMS_HAB_CODE },
            success: function (data) {
                if (data.Success) {
                    alert(data.Message);
                    $("#IMS_ISCOMPLETED").val('E');
                    $('#tbHabitation').trigger("reloadGrid");
                    PopulateHabitation($("#PLAN_CN_ROAD_CODE").val(), IMS_PR_RODE_CODE);
                    $("#divHabStatus").hide('slow');
                }
                else {
                    alert(data.Message);
                }
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
                unblockPage();
            }
        });
    }
    else {
        return;
    }
}
function DeleteHabitationWithCluster(IMS_PR_RODE_CODE, IMS_HAB_CODE,MAST_CLUSTER_CODE) {
    if (confirm("All the habitations within this cluster will be unmapped. Are you sure to UnMap the Habitation ?")) {
        $.ajax({
            url: "/Proposal/UnMapHabitationCluster/",
            type: "POST",
            async: false,
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            data: { IMS_PR_ROAD_CODE: IMS_PR_RODE_CODE, MAST_HAB_CODE: IMS_HAB_CODE , MAST_CLUSTER_CODE : MAST_CLUSTER_CODE },
            success: function (data) {
                if (data.Success) {
                    $("#IMS_ISCOMPLETED").val('E');
                    $('#tbHabitation').trigger("reloadGrid");
                    PopulateCluster($("#PLAN_CN_ROAD_CODE").val(), IMS_PR_RODE_CODE);
                    $("#divHabStatus").hide('slow');
                }
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
                unblockPage();
            }
        });
    }
    else {
        return;
    }
}


function validateHabitations() {

    //Get all selected values for Group Code
    if ($('#MAST_HAB_CODE :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#MAST_HAB_CODE :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();

            //Set Hideen varible value for Level Id
            if ($("#HAB_CODES_LIST").val() == "")
                $("#HAB_CODES_LIST").val(selectednumbers[i]);
            else
                $("#HAB_CODES_LIST").val($("#HAB_CODES_LIST").val() + "," + selectednumbers[i]);


            $("#showHabError").html("");
            $("#showHabError").removeClass("field-validation-error");

        });

        return true;
    }
    else {
        $("#showHabError").html("Map at least one Habitation.");
        $("#showHabError").addClass("field-validation-error");
        return false;
    }
}
function validateCluster() {

    //Get all selected values for Group Code
    if ($('#MAST_CLUSTER_CODE :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#MAST_CLUSTER_CODE :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();
            
            //Set Hideen varible value for Level Id
            if ($("#CLUSTER_CODES_LIST").val() == "")
                $("#CLUSTER_CODES_LIST").val(selectednumbers[i]);
            else
                $("#CLUSTER_CODES_LIST").val($("#CLUSTER_CODES_LIST").val() + "," + selectednumbers[i]);


            $("#showHabError").html("");
            $("#showHabError").removeClass("field-validation-error");

        });

        return true;
    }
    else {
        $("#showHabError").html("Map at least one cluster.");
        $("#showHabError").addClass("field-validation-error");
        return false;
    }
}