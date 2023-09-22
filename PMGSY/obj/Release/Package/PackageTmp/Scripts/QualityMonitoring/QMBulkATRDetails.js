/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMBulkATRDetails.js
        * Description   :   Handles events for in Quality Bulk ATR Regrade
        * Author        :   Shyam Yadav 
        * Creation Date :   15/Jul/2014
 **/

$(document).ready(function () {

    ShowBulkATRList($("#ddlBlkATRRegradeStates").val());

    $("#btnListBulkATRList").click(function () {
        ShowBulkATRList($("#ddlBlkATRRegradeStates").val(), $("#ddlBlkATRDuration").val());
    });
    
});

function ShowBulkATRList(stateCode, duration) {
    $("#tbBulkATRList").jqGrid('GridUnload');
    
    jQuery("#tbBulkATRList").jqGrid({
        url: '/QualityMonitoring/QMViewBulkATRList?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Monitor", "District", "Package", "Road Name", "Inspection Date", "Road Status", "Overall Grade"],
        colModel: [
                    { name: 'Monitor', index: 'Monitor', width: 90, sortable: false, align: "left" },
                    { name: 'District', index: 'District', width: 40, sortable: false, align: "left", search: false },
                    { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
                    { name: 'RoadName', index: 'RoadName', width: 130, sortable: false, align: "left", search: false },
                    { name: 'InspDate', index: 'InspDate', width: 60, sortable: false, align: "center", search: false },
                    { name: 'RdStatus', index: 'RdStatus', width: 50, sortable: false, align: "center", search: false },
                    { name: 'OverallGrade', index: 'OverallGrade', width: 50, sortable: false, align: "left", search: false }
                    //{ name: 'Regrade', index: 'Regrade', width: 45, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, duration: duration },
        pager: jQuery('#dvBulkATRListPager'),
        rowNum: 20000,
        sortorder: 'asc',
        sortname: 'Monitor',
        viewrecords: true,
        pgbuttons: false,
        pgtext: null,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;ATR List",
        height: '250',
        autowidth: true,
        rownumbers: true,
        multiselect: true,
        beforeSelectRow: function (rowid, e) {
            
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", $("#tbBulkATRList")[0]);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },
        onSelectAll: function (aRowids, status) {
            if (status) {
                // uncheck "protected" rows
                var cbs = $("tr.jqgrow > td > input.cbox:disabled", $("#tbBulkATRList")[0]);
                cbs.removeAttr("checked");

                //modify the selarrrow parameter
                $("#tbBulkATRList")[0].p.selarrrow = $("#tbBulkATRList").find("tr.jqgrow:has(td > input.cbox:checked)")
                    .map(function () { return this.id; }) // convert to set of ids
                    .get(); // convert to instance of Array
            }
        },
        loadComplete: function (data) {
            $("#gview_tbBulkATRList > .ui-jqgrid-titlebar").hide();
            if (data.rows != null) {
                for (var i = 0; i < data.rows.length; i++) {
                    var rowData = data.rows[i];
                    if (rowData.cell[1] != '-') {//update this to have your own check
                        var checkbox = $("#jqg_tbBulkATRList_" + rowData['id']);//update this with your own grid name
                        //checkbox.css("visibility", "hidden");
                        //checkbox.attr("disabled", true);
                    }
                }
            }
            
            if (data["records"] > 0) {
                $("#dvBulkATRListPager").css({ height: '31px' });

                $("#dvBulkATRListPager_left").html(
                    //"<input id='rdoAcceptATR' type='radio' value='A' onclick='displayRemarks();' name='rdoRegradeStatus' checked='True' style='margin-left:25px;margin-top:2px;'>&nbsp;Accept</input>" +
                    //"<input id='rdoRejectATR' type='radio' value='R' onclick='displayRemarks();' name='rdoRegradeStatus' style='margin-left:5px;margin-top:2px;'>&nbsp;Reject</input>" +
                    //"<textarea id='txtAreaRemarks' style='width: 250px;margin-left:5px;margin-top:2px;display:none;' name='ATR_REGRADE_REMARKS' maxlength='255' data-val-regex-pattern='^[a-zA-Z0-9 ,.()-]+$' data-val-regex='Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]' data-val='true' cols='20'></textarea>" +
                    "<input type='button' style='margin-left:25px;margin-top:2px;' id='btnRegrade' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RegradeATR();return false;' value='Regrade'/>"
                    );
            }

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

}


function displayRemarks() {
    if ($("#rdoRejectATR").is(":checked")) {
        $("#txtAreaRemarks").show();
    }
    else {
        $("#txtAreaRemarks").hide();
    }
}


function RegradeATR()
{
    var regradeData = $("#tbBulkATRList").jqGrid('getGridParam', 'selarrrow');
    if (regradeData == "") {
        alert('Please select records to regrade.');
        return false;
    }
    //// May be in future requirement of reject ATRs in bulk may come so provision of remarks & regradeStatus is here
    //var regradeStatus = $('input:radio[name=rdoRegradeStatus]:checked').val();
    //var remarks = $('textarea#txtAreaRemarks').val();
    
    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/QMBulkRegrade',
        async: false,
        data: { regradeData: regradeData },
        //data: { regradeData: regradeData, regradeStatus: regradeStatus, remarks: remarks },
        beforeSend: function () {
            blockPage();
        },
        success: function (data) {
            if (data.Success) {
                alert(data.Message);
                $("#tbBulkATRList").trigger('reloadGrid');
                unblockPage();
            }
            else {
                alert(data.Message);
                $("#tbBulkATRList").trigger('reloadGrid');
               unblockPage();
            }
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            unblockPage();
        }
    })
}

