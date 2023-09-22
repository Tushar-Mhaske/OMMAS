/* 
     *  Name : LockUnlock.js
     *  Path : ~\PMGSY\Scripts\LockUnlock\LockUnlock.js
     *  Description : LockUnlock.js used to List, Proposal Details to Freeze/Unfreeze Batch                              
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     * Actions : 1)validateFilter 2)showFilter 3)LoadProposals 4)FreezeProposal 5)UnFreezeProposal
 */
$(document).ready(function () {
    $.validator.unobtrusive.parse($('FilterForm'));
    blockPage();
    //set state,year,batch dropdown to fist value
    $("#ddlImsState").val(1);
    $("#ddlImsBatch").val(1);
    $("#ddlImsYear option:last").attr('selected', 'selected');

    LoadProposals($("#ddlImsYear").val(), $("#ddlImsState").val(), $("#ddlImsBatch").val());
    unblockPage();

    //hide filter search
    $("#idFilterDiv").click(function () {  
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    //list proposals
    $("#btnListProposal").click(function () {
        if ($('#FilterForm').valid()) {
            blockPage();
            if (validateFilter()) {
                $('#tbLockUnlockList').jqGrid('GridUnload');
                LoadProposals($("#ddlImsYear").val(), $("#ddlImsState").val(), $("#ddlImsBatch").val(),$("#ddlScheme option:selected").val());
            }
            unblockPage();
        }
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //change done by Vikram 
    $("#ddlScheme").change(function () {
        if ($("#ddlScheme option:selected").val() == "2")
        {
            $("#ddlImsBatch").find("option[value='4']").hide();
            $("#ddlImsBatch").find("option[value='5']").hide();
        }
        else if ($("#ddlScheme option:selected").val() == "1")
        {
            if ($("#ddlImsBatch option[value='4']").is(':visible') == false) {
                $("#ddlImsBatch").find("option[value='4']").show();
                $("#ddlImsBatch").find("option[value='5']").show();
            }
        }
    });


});

// validation logic to check state,year,batch drop down selected or not
function validateFilter() {
    if ($("#ddlImsYear").val() == "0") {
        alert("Please Select Year");
        return false;
    }
    if ($("#ddlImsState").val() == "0") {
        alert("Please Select State");
        return false;
    }
    if ($("#ddlImsBatch").val() == "0") {
        alert("Please Select Batch");
        return false;
    }
    return true;
}

//show filter search Div
function showFilter()
{    
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

// show Proposal list on jqGrid
function LoadProposals(IMS_YEAR, IMS_MAST_STATE_CODE, IMS_BATCH,Scheme) {
    
    jQuery("#tbLockUnlockList").jqGrid({
        url: '/LockUnlock/GetLockUnlockList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Road Name", "Length", "Sanctioned", "Freeze Status"],
        colModel: [
                            { name: 'District', index: 'District', width: 150, sortable: true, align: "center" },
                            { name: 'Block', index: 'Block', width: 120, sortable: true, align: "center" },
                            { name: 'PackageNumber', index: 'PackageNumber', width: 120, sortable: true, align: "center" },
                            { name: 'RoadName', index: 'RoadName', width: 280, sortable: true, align: "center" },
                            { name: 'Length', index: 'Length', width: 200, sortable: true, align: "center" },
                            { name: 'Sanctioned', index: 'Sanctioned', width: 120, sortable: true, align: "center" },
                             ///Changes by SAMMED A. PATIL on 25JULY2017 for Freeze STA Scrutinized Proposals/RCPLWE
                            { name: 'FreezeStatus', index: 'FreezeStatus', width: 100, sortable: true, align: "center" },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "IMS_MAST_STATE_CODE": IMS_MAST_STATE_CODE, "IMS_BATCH": IMS_BATCH, "STATE_NAME": $('#ddlImsState option:selected').text(),"Scheme":Scheme },
        pager: jQuery('#dvLockUnlockListPager'),
        rowNum: 10,
        sortorder: 'asc',
        sortname: 'District',
        viewrecords: true,
        rowList: [5, 10, 15, 20],
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp; Freeze/Unfreeze Proposals",
        height: 'auto',
        autowidth:true,
        rownumbers: true,
        loadComplete: function (data) {
            console.log(data["records"]);
            if (data["records"] > 0)    {

                //setModel Freeze Status

                

                if (data.rows[0] != undefined) {
                    console.log(data.IsSOGenerated);
                    if (data.IsSOGenerated == false) {
                        $("#FreezeStatus").val(data["rows"]["0"]["IMS_FREEZE_STATUS"]);

                        console.log(data["rows"]["0"]["IMS_FREEZE_STATUS"]);

                        if (data["rows"]["0"]["IMS_FREEZE_STATUS"] == "U") {
                            $("#tbLockUnlockList #dvLockUnlockListPager").css({ height: '31px' });
                            $("#dvLockUnlockListPager_left").html("<input type='button' style='margin-left:27px' id='idFreezeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FreezeProposal();return false;' value='Freeze'/>")
                        } else {
                            $("#dvLockUnlockList #dvLockUnlockListPager").css({ height: '31px' });
                            $("#dvLockUnlockListPager_left").html("<input type='button' style='margin-left:27px' id='idUnFreezeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'UnFreezeProposal();return false;' value='UnFreeze'/>")
                        }
                    }
                    else {

                    }
                }
            }
            unblockPage();
        },
        loadError: function (xhr, status, error) {
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


// FreezeProposal() action is use to freeze selected batch
function FreezeProposal()
{
        //get grid parameters and set it to model valiables 
     var data= jQuery("#tbLockUnlockList").getGridParam('postData');   
     YEAR_CODE = data['IMS_YEAR'];
     STATE_CODE = data['IMS_MAST_STATE_CODE'];
     BATCH_CODE = data['IMS_BATCH'];
     STATE_NAME = data['STATE_NAME'];
        //set model variables
     $("#StateCode").val(STATE_CODE);
     $("#YearCode").val(YEAR_CODE);
     $("#BatchCode").val(BATCH_CODE);

     if (confirm("Are you sure you want to freeze proposal for State: "+STATE_NAME+ " Year: "+YEAR_CODE +" Batch: " + BATCH_CODE + " ? ")) {
        $.ajax({
            url: '/LockUnlock/FreezeProposal/', 
            type: "POST",
            dataType: "json",
            cache: false,
           data:$("#FilterForm").serialize(),
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);
                    $("#tbLockUnlockList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }
                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}
// UnFreezeProposal() action is use to Unfreeze selected batch
function UnFreezeProposal() {
        //get grid parameters and set it to model valiables 
    var data = jQuery("#tbLockUnlockList").getGridParam('postData');
    YEAR_CODE = data['IMS_YEAR'];
    STATE_CODE = data['IMS_MAST_STATE_CODE'];
    BATCH_CODE = data['IMS_BATCH'];
    STATE_NAME = data['STATE_NAME'];
        //set model variables
    $("#StateCode").val(STATE_CODE);
    $("#YearCode").val(YEAR_CODE);
    $("#BatchCode").val(BATCH_CODE);

    if (confirm("Are you sure you want to unfreeze proposal for State: " + STATE_NAME + " Year: " + YEAR_CODE + " Batch: " + BATCH_CODE + " ? ")) {

        $.ajax({
            url: '/LockUnlock/FreezeProposal/',
            type: "POST",
            dataType: "json",
            cache: false,
            data:$("#FilterForm").serialize(),
            beforeSend: function () {
                blockPage();
            },
            postData: { "YEAR_CODE": YEAR_CODE, "STATE_CODE": STATE_CODE, "BATCH_CODE": BATCH_CODE },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);
                    $("#tbLockUnlockList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message);
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }
                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}
