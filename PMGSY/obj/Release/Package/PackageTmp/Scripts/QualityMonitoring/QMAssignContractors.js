/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMAssignRoads.js
        * Description   :   Handles events, grids in schedule assign process for Monitors
        * Author        :   Shyam Yadav 
        * Creation Date :   11/Jun/2013
 **/

var arrIsEnquiryContractors = [];
var arrAddRoadContractors = [];
$(document).ready(function () {

    //District Code Change
    $("#MAST_DISTRICT_CODE").change(function () {

        if (($('#IMS_YEAR').val() != "")) {
            ContractorRoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#ADMIN_SCHEDULE_CODE").val(), $('#IMS_YEAR').val());
        }
        else {
            alert("Please select Sanction Year");
        }

    });//District Code Change Ends here

    $("#IMS_YEAR").change(function () {

        if (($('#IMS_YEAR').val() != "") && ($("#MAST_DISTRICT_CODE").val() > 0)) {

            ContractorRoadListGrid($("#MAST_DISTRICT_CODE").val(), $("#ADMIN_SCHEDULE_CODE").val(), $('#IMS_YEAR').val());
        }
        else {
            alert("Please select both District and Sanction Year");
        }

    });

}); //doc.ready ends here


function closeDivError() {
    $("#divError").hide();
}

$("#tbSanctionRoadListContractors").click(function (e) {

    var el = e.target; // DOM of the HTML element which was clicked
    if (el.nodeName !== "TD") {
        // in case of the usage of the custom formatter we should go to the next
        // parent TD element
        el = $(el, this.rows).closest("td");
        var iCol = $(el).index();
        var row = $(el, this.rows).closest("tr.jqgrow");
        var rowId = row[0].id;
        // now you can do what you need. You have iCol additionally to rowId
        if ($(el, this.rows).closest("td").attr('aria-describedby') === "tbSanctionRoadListContractors_chkboxEnquiry")//column for IsEnquiry
        {
            if ($(el, this.rows).closest("td").find('input[type=checkbox]').prop('checked')) {
                arrIsEnquiryContractors.push(rowId);
            }
            else {
                arrIsEnquiryContractors = jQuery.grep(arrIsEnquiryContractors, function (value) {
                    return value != rowId;
                });
            }
        }
        else if ($(el, this.rows).closest("td").attr('aria-describedby') === "tbSanctionRoadListContractors_chkboxAdd")//column for Add Road
        {

            if ($(el, this.rows).closest("td").find('input[type=checkbox]').prop('checked')) {

                arrAddRoadContractors.push(rowId);
            }
            else {
                arrAddRoadContractors = jQuery.grep(arrAddRoadContractors, function (value) {
                    return value != rowId;
                });
            }
        }
    }
});//Grid Click ends here

function QMViewProgress(prRoadCode) {
    $("#dlgPhyProgress").load("/QualityMonitoring/ViewPhysicalProgress/" + prRoadCode, function () {

        setTimeout(function () {
            $("#dlgPhyProgress").dialog({
                autoOpen: true,
                modal: true,
                height: 500,
                width: 1050,
                title: "Physical Progress"
            });
            $("#dlgPhyProgress").show();
        }, 100);
    });
}


function CheckContractorCount() {

    if (arrAddRoadContractors.length == 0) {

        var data = $('input[type=checkbox]');
        for (i = 0; i < data.length; i++) {
            if (data[i].value != "isenquiry" && data[i].checked) {
                console.log(data[i].id);
                arrAddRoadContractors.push(data[i].id);
                console.log(arrAddRoadContractors);
            }
        }
    }
  
}

function addWorksContractors() {
    CheckContractorCount();
    console.log(arrAddRoadContractors);
    if (confirm('Are you sure to assign selected works?')) {
        $.ajax({
            url: '/QualityMonitoring/QMAssignWorks',
            type: 'POST',
            data: { arrWorks: arrAddRoadContractors, arrEnquiry: arrIsEnquiryContractors, adminSchCode: $("#ADMIN_SCHEDULE_CODE").val(), value: Math.random() },
            success: function (response) {
                if (response.Success) {
                    alert("Works assigned successfully in schedule");
                    $("#tbSanctionRoadListContractors").trigger("reloadGrid");
                    QMAssignContractors($("#ADMIN_SCHEDULE_CODE").val(), $("#IMS_YEAR").val());
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
    else {
        return false;
    }
}


//Road List for contractors not inspected even once added by deendayal 
function ContractorRoadListGrid(districtCode, adminSchCode, sanctionYear) {
    // $("#onlyRoad").toggle();
    // $("#dvSanctionRoadListPager").toggle();
    $("#tbSanctionRoadListContractors").jqGrid('GridUnload');
    jQuery("#tbSanctionRoadListContractors").jqGrid({
        url: '/QualityMonitoring/GetRoadListToAssignContractorWise?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Work Priority", "Block", "Package", "Sanction Year", "Road / Bridge", "Type", "Length (Road-Km / LSB-Mtr)", "Contractor Name", "Commneced / Completed Date",
                   "Scheme", "Assigned By", "NQM Inspection Date & Count", "SQM Inspection Date & Count", "Progress Status", "IsEnquiryHdn", "Is Enquiry", "AddWorkHdn", "Add Work"],
        colModel: [
                            { name: 'WorkPriority', index: 'WorkPriority', hidden: true, width: 120, sortable: false, align: "left" },
                            { name: 'Block', index: 'Block', width: 120, sortable: false, align: "left" },
                            { name: 'Package', index: 'Package', width: 120, sortable: false, align: "center" },
                            { name: 'SanctionYear', index: 'SanctionYear', width: 120, sortable: false, align: "center", search: false },

                            { name: 'Road', index: 'Road', width: 300, sortable: false, align: "left" },
                            { name: 'Type', index: 'Type', width: 100, sortable: false, align: "left" },
                            { name: 'RdLength', index: 'RdLength', width: 100, sortable: false, align: "center", search: false },

                            { name: 'CONTRACTOR_NAME', index: 'CONTRACTOR_NAME', width: 300, sortable: false, align: "left", search: false },

                            { name: 'CommencedOrCompDate', index: 'CommencedOrCompDate', width: 100, sortable: false, align: "center", search: false },

                            { name: 'PMGSYScheme', index: 'PMGSYScheme', width: 80, sortable: false, align: "center", search: false },
                            { name: 'AssignedBy', index: 'AssignedBy', width: 80, sortable: false, align: "center", search: false },
                            { name: 'NQMInspCount', index: 'NQMInspCount', width: 100, sortable: false, align: "center", search: false },
                            { name: 'SQMInspCount', index: 'SQMInspCount', width: 100, sortable: false, align: "center", search: false },
                            { name: 'ViewProgress', index: 'ViewProgress', width: 70, sortable: false, align: "center", search: false },

                            { name: 'IsEnquiry', index: 'IsEnquiry', width: 80, sortable: false, align: "center", search: false, hidden: true },
                            {
                                name: 'chkboxEnquiry', index: 'chkboxEnquiry', sortable: true, width: 80, align: 'center',
                                formatter: "checkbox", formatoptions: { disabled: false }, editable: true, search: false,
                                edittype: "checkbox", editoptions: { value: "Y:N" }, hidden: true
                            },
                            { name: 'AddRoad', index: 'AddRoad', width: 70, sortable: false, align: "center", search: false, hidden: true },
                            {
                                name: 'chkboxAdd', index: 'chkboxAdd', sortable: true, width: 80, align: 'center',
                                //formatter: "checkbox", formatoptions: { disabled: false },
                                editable: true, search: false,
                                edittype: "checkbox", editoptions: { value: "Y:N" }
                            },
        ],
        postData: { "districtCode": districtCode, "adminSchCode": adminSchCode, "sanctionYear": sanctionYear },
        pager: jQuery('#dvSanctionRoadListPagerContractors'),
        rowNum: 10000,
        pgbuttons: false,
        pgtext: null,
        sortorder: "asc",
        sortname: "Block",
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road List",
        height: '300',
        grouping: true,
        groupingView: {
            groupField: ['WorkPriority'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
        },
        loadComplete: function () {
            $("#gview_tbSanctionRoadListContractors > .ui-jqgrid-titlebar").hide();
            $("#tbSanctionRoadListContractors").jqGrid('setGridWidth', $("#tblAddRoads").width() + 10, true);

            var ids = jQuery("#tbSanctionRoadListContractors").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var curretRowId = ids[i];
                var rowData = jQuery("#tbSanctionRoadListContractors").getRowData(curretRowId);
                var isEnquiry = rowData['IsEnquiry'];
                var isAlreadyAssigned = rowData['AssignedBy'];

                //console.log($('#' + curretRowId).children().length);
                if (isEnquiry == "Y" && isAlreadyAssigned != "--") {
                    //jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('checked', true);
                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(15) input[type=checkbox]').prop('checked', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(17) input[type=checkbox]').prop('checked', true);
                }
                else if (isAlreadyAssigned != "--") {
                    jQuery(this).find('#' + curretRowId + ' input[type=checkbox]').prop('disabled', true);
                    jQuery(this).find('#' + curretRowId + ' :nth-child(17) input[type=checkbox]').prop('checked', true);
                }

                $("#dvSanctionRoadListPagerContractors_left").html("<input type='button' style='margin-left:5px' class='jqueryButton ui-button ui-widget ui-state-active ui-corner-all ui-button-text-only' onClick = 'addWorksContractors();return false;' value='Assign Work'/>")
            }
            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }


    }); //end of grid

    $("#tbSanctionRoadListContractors").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });


}//Schedule Road Grid Ends Here
