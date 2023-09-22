$(document).ready(function () {

    $("#ddlState").change(function () {
        if ($("#ddlState").val() > 0) {

            $("#ddlDistrict").empty();

            $.ajax({
                url: '/Proposal/GetDistricts',
                type: 'POST',
                beforeSend: function () {
                    blockPage();
                },
                data: { MAST_STATE_CODE: $("#ddlState").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddlDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }

                    PopulateAgenciesStateWise();
                    unblockPage();
                },
                error: function (err) {
                    alert("error " + err);
                    unblockPage();
                }
            });

        }
    });

    $("#btnListProposal").click(function () {
       
        if ($("#ddlState").val() == 0 || $("#ddlState").val() == -1) {
            alert("Please select State")
            return false;
        }

        blockPage();

        LoadMordProposals($("#ddlState").val(), $("#ddlDistrict").val())

        unblockPage();

    });
});

function LoadMordProposals(STATE, MAST_DISTRICT_ID) { 
    
    $("#divMORDProposal").show();

    $("#tbMORDProposalList").show();
    $("#dvMORDProposalListPager").show();
    $('#tbMORDProposalList').jqGrid('GridUnload');

    MordProposalGrid(STATE, MAST_DISTRICT_ID) 

}

function MordProposalGrid(STATE, MAST_DISTRICT_ID) {
    
    jQuery("#tbMORDProposalList").jqGrid('GridUnload');
    
    jQuery("#tbMORDProposalList").jqGrid({
        url: '/Proposal/GetFreezeUnfreezeList',
        datatype: "json",
        mtype: "POST",
        multiselect: true,
        postData: { "STATE": STATE, "MAST_DISTRICT_ID": MAST_DISTRICT_ID },
        colNames: ['Road Code','State', 'District', "Block", "Year", "Road Name", "LSB Name", "Length", "Award Status", "Physical Progress", "Financial Progress", 'Unfreeze'],
        colModel: [
                        { name: 'SANCTION_CODE', index: 'SANCTION_CODE', height: 'auto', width: 80, align: "center", hidden: true, editable: true, key: true },                    
                        { name: 'State', index: 'State', width: 80, sortable: false, align: "center" },
                        { name: 'District', index: 'District', width: 80, sortable: false, align: "center" },
                        { name: 'Block', index: 'Block', width: 80, sortable: false, align: "center" },
                        { name: 'SANCTION_YEAR', index: 'SANCTION_YEAR', width: 80, sortable: false, align: "center" },
                        { name: 'Road_Name', index: 'Road_Name', width: 180, sortable: false, align: "left" },
                        { name: 'Bridge_Name', index: 'Bridge_Name', width: 180, sortable: false, align: "left" },
                        { name: 'Sanctioned_Length', index: 'Sanctioned_Length', width: 80, sortable: false, align: "center" },
                        { name: 'Is_Awarded', index: 'Is_Awarded', width: 80, sortable: false, align: "center", hidden: false },
                        { name: 'Completed_Length', index: 'Completed_Length', width: 80, sortable: false, align: "center" },
                        { name: 'Expenditure_till_date__Lakhs_', index: 'Expenditure_till_date__Lakhs_', width: 80, sortable: false, align: "center" },
                        { name: 'flag', index: 'flag', height: 'auto', width: 80, align: "center", search: false, formatter: setCheckBox },

                  ],
        pager: jQuery('#dvMORDProposalListPager'),       
        sortorder: "desc",
        sortname: 'State',
        height: '400px',
        width: 'auto',
        rowList: [10, 20, 30, 50, 100],
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp; Freeze Unfreeze Proposal Sanctioned on/after 1-April-2020",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        jsonReader: {
            Id: "SANCTION_CODE",
        },

        onSelectAll: function (aRowids, status) {

            var userdata = jQuery("#tbMORDProposalList").getGridParam('userData');

            //alert("userdata.ids.length : " + userdata.ids.length);
            //alert("status : " + status);
            //alert("aRowids : " + aRowids);

            //if (userdata.ids.length == 0)
            //{
            //    alert("in length if");
            //    var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
            //    cbs.removeAttr("checked");
            //}

            //for (var i = 0; i < userdata.ids.length; i++) {

            //    if (($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) && ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('disabled', true))) {
            //        jQuery("#jqg_tbMORDProposalList_" + userdata.ids[i]).attr("disabled", true);
            //    }
            //}

            for (var i = 0; i < userdata.ids.length; i++) {

                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tbMORDProposalList_" + userdata.ids[i]).attr("disabled", true);
                }
            }

        },
        loadComplete: function () {           

            var userdata = jQuery("#tbMORDProposalList").getGridParam('userData');

            idsOfSelectedRows = userdata.ids;
            var count = 0;
          
             $("#cb_tbMORDProposalList").attr('disabled', false);

            for (var i = 0; i < userdata.ids.length; i++) {

                if ($('#' + userdata.ids[i] + ' input[type=checkbox]').attr('checked', true)) {
                    jQuery("#jqg_tbMORDProposalList_" + userdata.ids[i]).attr("disabled", true);
                }
            }

            for (var i = 0 ; i < userdata.ids.length ; i++)
            {
                if (($("#jqg_tbMORDProposalList_" + userdata.ids[i]).attr("disabled")))
                {
                    count = count + 1;
                }
            }

            $("#tbMORDProposalList #dvMORDProposalListPager").css({ height: '31px' });
            if ($('#tbMORDProposalList').jqGrid('getGridParam', 'records')) {
                $("#dvMORDProposalListPager_left").html("<input type='button' style='margin-left:27px;margin-top:2px;' id='btnfreeze' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GetDataInArray();return false;' value='Freeze'/>");
            }

            if (count == userdata.ids.length) {
                $("#cb_tbMORDProposalList").attr('checked', true);
                $("#cb_tbMORDProposalList").attr('disabled', true);
                $('#btnfreeze').hide();
            }

            unblockPage();
        },
        beforeSelectRow: function (rowId, e) {
            
            if ($("#jqg_tbMORDProposalList_" + rowId).attr("disabled")) {
                return false;
            }
            else
                return true;          
           
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

}

function GetDataInArray() {
   
    var submitArray = [];

    var selRowIds = jQuery('#tbMORDProposalList').jqGrid('getGridParam', 'selarrrow');
    
    if (selRowIds.length > 0) {
        for (var i = 0; i < selRowIds.length; i++) {

            rowdata = jQuery("#tbMORDProposalList").getRowData(selRowIds[i]);

            if (!$("#jqg_tbMORDProposalList_" + selRowIds[i]).attr("disabled")) {             
                submitArray.push(rowdata["SANCTION_CODE"]);
            }
        }
        
        FreezeDetails(submitArray);
    }
    else
        alert("No records to freeze");

}

function FreezeDetails(submitArray) {
   
    if (confirm("Are you sure to Freeze the Proposal details ? ")) {

        $.ajax({
            type: "POST",
            url: "/Proposal/FreezeMordDetails/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ 'submitarray': submitArray }),
            success: function (response) {
                unblockPage();
                if (response.success) {
                    alert("Details freezed successfully.");

                    $("#tbMORDProposalList").trigger('reloadGrid');

                    closeMonitorsInspectionDetails();
                    $(".ui-icon-closethick").trigger("click");

                }
                else {
                    alert("Something went wrong.");
                }
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            }

        });

    }
    else {
        return;
    }
}

function UnFreezeDetails(id) {
    
    if (confirm("Are you sure to Unfreeze the Proposal details ? ")) {

        $.ajax({
            url: "/Proposal/UnFreezeDetails/",
            type: "POST",
            cache: false,
            data: { ProposalCode: id },
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
                if (response.success) {
                    alert("Proposal details unfreezed successfully.");

                    $("#tbMORDProposalList").trigger('reloadGrid');
                 
                    $(".ui-icon-closethick").trigger("click");

                }
                else {
                    alert("Something went wrong.");
                }
            }
        });

    }
    else {
        return;
    }
}


function setCheckBox(cellValue, options, rowObject) {

    var myArray = cellValue.split("$");
    
    if (myArray[0] == "U") {
        return "-";
    }
    else {
        return ("<a href='#' title='Click here to unfreeze Proposal Details' class='ui-icon ui-icon-locked ui-align-center' onClick=UnFreezeDetails('" + myArray[1] + "'); return false;>Unfreeze</a>");
    }
 
}