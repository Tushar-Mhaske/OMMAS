$(document).ready(function () {

    LoadTechnologyDetails($('#prRoadCode').val());

    //add accordion
    $(function () {
        $("#accordion1").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});

function LoadTechnologyDetails(IMS_ROAD_CODE) {

    jQuery("#tbTechnologyList").jqGrid({
        url: '/Execution/GetTechnologyProgressDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { ProposalCode: IMS_ROAD_CODE },
        //colNames: ['Block', 'Package', 'Road/Bridge Name', 'Agreement Date', 'Sanction Cost [In Lakhs]', 'Sanction Length [In Kms.]', 'Agreement Cost [In Lakhs]', 'Sanction Year', 'Segment No.', 'Start Chainage', 'End Chainage', 'Technical Cost', 'Layer Cost', 'Layer', 'Technology', 'Technology Type', 'Add'],
        colNames: ['Segment No.', 'Start Chainage', 'End Chainage', 'Technical Cost', 'Layer Cost', 'Layer', 'Technology', 'Technology Type', 'Add'],
        colModel: [
                            //{ name: 'Block', index: 'Block', height: 'auto', width: 100, align: "center", search: false, editable:true, },
                            //{ name: 'Package', index: 'Package', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'WorkName', index: 'WorkName', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'AgreementDate', index: 'AgreementDate', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'SanctionCost', index: 'SanctionCost', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'SanctionLength', index: 'SanctionLength', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'AgreementCost', index: 'AgreementCost', height: 'auto', width: 100, align: "center", search: false },
                            //{ name: 'SanctionYear', index: 'SanctionYear', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_SEGMENT_NO', index: 'IMS_SEGMENT_NO', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'IMS_START_CHAINAGE', index: 'IMS_START_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_END_CHAINAGE', index: 'IMS_END_CHAINAGE', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_TECH_COST', index: 'IMS_TECH_COST', height: 'auto', width: 100, align: "right", search: false },
                            { name: 'IMS_LAYER_COST', index: 'IMS_LAYER_COST', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'MAST_HEAD_DESC', index: 'MAST_HEAD_DESC', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'MAST_TECH_NAME', index: 'MAST_TECH_NAME', height: 'auto', width: 150, align: "left", search: true },
                            { name: 'MAST_TECH_TYPE', index: 'MAST_TECH_TYPE', height: 'auto', width: 150, align: "left", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false, },

        ],
        pager: jQuery('#pagerTechnologyList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Technology Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            //$("#gview_tbTechnologyList> .ui-jqgrid-titlebar").hide();

            //$("#tbTechnologyList #pagerTechnologyList").css({ height: '40px' });
            //$("#pagerTechnologyList_left").html("<input type='button' style='margin-left:27px' id='btnAddTechnologyDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddTechnologyDetails(" + IMS_ROAD_CODE + ");return false;' value='Add Road Progress'/>")

            $("#pagerTechnologyList_left").html("<input type='button' style='margin-left:27px' id='btnAddImage' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddImageDetails(); return false;' value='Upload Image'/>")

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

//returns the view of Physical progress of Proposal
function AddTechnologyProgressDetails(urlparameter) {
    //alert(urlparameter);
    $("#accordion1 div").html("");
    $("#accordion1 h3").html(
            "<a href='#' style= 'font-size:1.1em !Important;'>Add Technology Progress Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecTechnologyDetails();" /></a>'
            );

    $('#accordion1').show('fold', function () {
        //blockPage();
        $("#divAddTechProgressDetails").load('/Execution/ListTechnologyProgressDetails/' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#divAddTechProgressDetails'));
            //unblockPage();
        });
        $('#divAddTechProgressDetails').show('slow');
        $("#divAddTechProgressDetails").css('height', 'auto');
    });
    $("#tbTechnologyList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//close the accordion of physical and financial details
function CloseExecTechnologyDetails() {

    $("#accordion1").hide('slow');
    //$("#divAddExecution").hide('slow');
    $("#tbTechnologyList").jqGrid('setGridState', 'visible');
    ShowFilter();
}
//show the filter view 
function ShowFilter() {

    //$("#divSearchExecution").show('slow');
    //$("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    //$('#idFilterDiv').trigger('click');
}

//returns the view of Image
function AddImageDetails() {
    //alert(urlparameter);
    var urlparameter = $('#encrPrRoadCode').val();
    $("#accordion1 div").html("");
    $("#accordion1 h3").html(
            "<a href='#' style= 'font-size:1.1em !Important;'>Add Image Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecTechnologyDetails();" /></a>'
            );

    $('#accordion1').show('fold', function () {
        //blockPage();
        $("#divAddTechProgressDetails").load('/Execution/ExecTechImageUpload/' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#divAddTechProgressDetails'));
            //unblockPage();
        });
        $('#divAddTechProgressDetails').show('slow');
        $("#divAddTechProgressDetails").css('height', 'auto');
    });
    $("#tbTechnologyList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}