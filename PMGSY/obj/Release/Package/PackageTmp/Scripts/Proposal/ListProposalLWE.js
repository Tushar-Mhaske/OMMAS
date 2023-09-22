$(document).ready(function () {

    blockPage();

    if ($("#ddlState").is(":visible")) {
        if ($("#RoleID").val() != 3) {
            $("#ddlState").val(0);
        }
    }

    if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55') {            //SRRDA or SRRDARCPLWE - listing Only 

        $("#divSRRDAProposal").show();
        $("#btnAddProposal").hide();
        LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
    }
    else if ($("#RoleID").val() == '3') //STA
    {
        $("#divStaProposal").show();
        $("#btnAddProposal").hide();

        if ($("#ddlState").val() > 0) {
            STAListRoadProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());

        }
    }
    else if ($("#RoleID").val() == '15') {
        $("#btnAddProposal").hide();
        $("#divPtaProposal").show();

        PTAListRoadProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
    }

    else if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') { ///Changes for RCPLWE
        $("#dvProposal").show();
        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }

    unblockPage();

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

    });

    $('#btnAddProposal').click(function () {
        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Add " + $("#ddlImsProposalTypes option:selected").text() + " Proposal Details</a>" +

                '<a href="#" style="float: right;">' +
                '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseProposalDetails();" /></a>' +
                '<span style="float: right;"></span>'
                );

        $('#accordion').show('slow', function () {
            blockPage();

            if ($("#ddlImsProposalTypes").val() == "P") {

                $("#divProposalForm").load('/Proposal/Create/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }
            else if ($("#ddlImsProposalTypes").val() == "L") {

                $("#divProposalForm").load('/LSBProposal/CreateLSB/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }
            else if ($("#ddlImsProposalTypes").val() == "B") {
                $("#divProposalForm").load('/BuildingProposal/BuildingCreate/' + $("#ddlImsYear").val() + "$" + $("#ddlMastBlockCode").val() + "$" + $("#ddlImsBatch").val() + "$" + $("#ddlImsStreams").val(), function () {
                    $.validator.unobtrusive.parse($('#divProposalForm'));
                    unblockPage();
                });
            }

            $('#divProposalForm').show('slow');
            $("#divProposalForm").css('height', 'auto');
        });

        $("#tbProposalList").jqGrid('setGridState', 'hidden');
        $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
        $('#idFilterDiv').trigger('click');
    });

    $("#btnListProposal").click(function () {

        blockPage();
        if ($("#RoleID").val() == '2' || $("#RoleID").val() == '37' || $("#RoleID").val() == '55') {            //SRRDA or SRRDARCPLWE - listing Only

            LoadSRRDAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val());
        }
        else if ($("#RoleID").val() == '3') //STA
        {
            if ($("#ddlState").val() > 0) {
                LoadSTAProposals($("#ddlImsYear").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
            else {
                alert("Please Select State");
            }
        }
        else if ($("#RoleID").val() == '15') {
            if (validateFilter()) {
                LoadPTAProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val());
            }
        }
        else if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') {  ///Changes for RCPLWE
            CloseProposalDetails();
            //LoadProposals() called in CloseProposalDetails()
        }
        else if ($("#RoleID").val() == '25' || $("#RoleID").val() == '65') {//Changes by SAMMED A. PATIL for Mord View
            if (validateFilter()) {
                LoadMordProposals($("#ddlImsYear").val(), $("#ddlState").val(), $("#ddlDistrict").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus").val(), $("#ddlImsConnectivity").val(), $("#ddlImsAgencies").val());
            }
        }
        unblockPage();
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

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

    $("#ddlImsProposalTypes").change(function () {
        if ($(this).val() == "A") {
            $("#btnAddProposal").hide("blink");
        }
        else {
            $("#btnAddProposal").show("fold");
        }
    });

    $("#ddlDistrict").change(function () {

        $.ajax({
            url: '/Proposal/PopulateBlocks',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { districtCode: $("#ddlDistrict").val(), value: Math.random() },
            success: function (jsonData) {
                $("#ddlMastBlockCode").empty();
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlMastBlockCode").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlMastBlockCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

    });
    validateDateListProposal();
    $('#ddlImsYear').change(function () {
        validateDateListProposal();
        //if ($('#ddlImsYear').val() < 2016)
        //{
        //    $('#btnAddProposal').hide('slow');
        //}
        //else
        //{
        //    $('#btnAddProposal').show('slow');
        //}
    });

    setTimeout(function () {
        $('#btnListProposal').trigger('click');
    }, 300);

});

function validateDateListProposal() {
    var currentYear = (new Date).getFullYear();
    var currentMonth = (new Date).getMonth() + 1;
    var currentDay = (new Date).getDate();

    var currFinancialYear = parseInt(currentMonth) <= 3 ? parseInt(currentYear - 1) : parseInt(currentYear);

    if (parseInt($('#ddlImsYear').val()) >= parseInt(currFinancialYear)) {
        $('#btnAddProposal').show('slow');
    }
    else {
        $('#btnAddProposal').hide('slow');
    }
}

function PopulateAgenciesStateWise() {
    $.ajax({
        url: '/Proposal/PopulateAgencies',
        type: 'POST',
        beforeSend: function () {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        },
        data: { stateCode: $("#ddlState").val(), value: Math.random() },
        success: function (jsonData) {
            $("#ddlImsAgencies").empty();
            for (var i = 0; i < jsonData.length; i++) {
                if (jsonData[i].Selected == true) {
                    $("#ddlImsAgencies").append("<option value='" + jsonData[i].Value + "' selected=true>" + jsonData[i].Text + "</option>");
                }
                else {
                    $("#ddlImsAgencies").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
            }

            $.unblockUI();
        },
        error: function (err) {
            //alert("error " + err);
            $.unblockUI();
        }
    });
}
function validateFilter() {
    if ($("#ddlState").val() == "0") {
        alert("Please Select State");
        return false;
    }
    if ($("#ddlDistrict").val() == "0") {
        alert("Please Select District");
        return false;
    }
    return true;
}

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divProposalForm').hide('slow');
    $("#tbProposalList").jqGrid('setGridState', 'visible');
    $("#tbLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbStaProposalList').jqGrid('setGridState', 'visible');
    $("#tbStaLSBProposalList").jqGrid('setGridState', 'visible');
    $('#tbPtaProposalList').jqGrid('setGridState', 'visible');      //change by Ujjwal Saket on 1-11-2013
    $('#tbPtaLSBProposalList').jqGrid('setGridState', 'visible');
    $("#tbMORDProposalList").jqGrid('setGridState', 'visible');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'visible');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'visible');
    $("#tbBuildingProposalList").jqGrid('setGridState', 'visible');


    // For DPIU Login Reload the Jqgrid 
    if ($("#RoleID").val() == '22' || $("#RoleID").val() == '38' || $("#RoleID").val() == '54') {  ///Changes for RCPLWE

        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
    }

    showFilter();
}
// Show Habitation Details Form
function EditHabitationsDetails(urlparamater) {

    var arr = urlparamater.split("$");
    //alert(urlparamater);
    //alert(arr[0]);
    jQuery('#tbProposalList').jqGrid('setSelection', arr[0]);

    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add Habitation Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fast', function () {

        //$("#divProposalForm").load("/Proposal/AddHabitation/" + urlparamater, function () { commented by Vikram for changing the mapping of habitations in two ways like 'Habitation' and 'Clusterwise'
        $("#divProposalForm").load("/Proposal/AddHabitationWithCluster/" + urlparamater, function () {
            ShowHabitations($("#IMS_PR_ROAD_CODE").val());
            $.validator.unobtrusive.parse($('#divProposalForm'));
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//Show Traffic Details form
function EditTrafficDetails(urlparamater) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlparamater);
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Traffic Intensity Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/TrafficIntensity/' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function EditCBRDetails(urlParameter) {

    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >CBR Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/CBRValue/' + urlParameter, function () {
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

function UploadFile(urlParameter) {
    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(///Changes for RCPLWE
            "<a href='#' style= 'font-size:.9em;' >" + (($("#RoleID").val() == 22 || $("#RoleID").val() == 54) ? "Upload C Proforma" : $("#RoleID").val() == 3 ? "Joint Inspection" : "Upload Performance Report") + "</a>" +

            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>' +
            '<span style="float: right;"></span>'

            //'<a href="#" style="float: right;">' +
            //    '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseProposalDetails();" /></a>' +
            //    '<span style="float: right;"></span>'
            );

    $('#accordion').show('fold', function () {

        $("#divProposalForm").load('/Proposal/FileUpload/' + urlParameter, function () {

        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');

}

function JIUploadFile(urlParameter) {
    jQuery('#tbProposalList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >" + "Joint Inspection" + "</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {

        //$("#divProposalForm").load('/Proposal/FileUpload/' + urlParameter, function () {

        //});

        $.ajax({
            url: '/Proposal/FileUpload/' + urlParameter,
            type: "GET",
            //data: { ISPFTYPE: "J" },
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                $("#divProposalForm").html('');
                $("#divProposalForm").html(response);

                //if (response.success == true) {
                //    alert("Proposal Deleted Successfully.");
                //    CloseProposalDetails();
                //    $("#tbProposalList").trigger('reloadGrid');
                //}
                //else if (response.success == false) {
                //    alert(response.errorMessage);
                //}
                //else {
                //    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                //        alert(response.errorMessage)
                //    }
                //    else {
                //        alert("Error Occured while processing your request.");
                //    }

                //}
                unblockPage();
            }
        });

        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');

}

//show the Details of Road Proposal
function ShowDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/Details?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));

            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#tbStaProposalList').jqGrid('setGridState', 'hidden');
    $('#tbPtaProposalList').jqGrid('setGridState', 'hidden');          //Change by Ujjwal Saket on 1-11-2013
    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');


    $('#idFilterDiv').trigger('click');

}

// Editing the Road Proposal
function EditDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/Edit/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');


}

// Edit Unlocked Road Proposal
function EditUnlockedProposal(id) {

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit UnLocked Road Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/EditUnLockedProposal/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

// Delete Road Proposal
function DeleteDetails(id) {

    $.ajax({
        url: "/Proposal/IsProposalDeleted/" + id,
        type: "POST",
        cache: false,
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
                if (confirm("Are you sure to Delete Road Proposal Details ? ")) {

                    $.ajax({
                        url: '/Proposal/DeleteConfirmed/' + id,
                        type: "POST",
                        cache: false,
                        beforeSend: function () {
                            blockPage();
                        },
                        error: function (xhr, status, error) {
                            unblockPage();
                            Alert("Request can not be processed at this time,please try after some time!!!");
                            return false;
                        },
                        success: function (response) {

                            if (response.success == true) {
                                alert("Proposal Deleted Successfully.");
                                CloseProposalDetails();
                                $("#tbProposalList").trigger('reloadGrid');
                            }
                            else if (response.success == false) {
                                alert(response.errorMessage);
                            }
                            else {
                                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                                    alert(response.errorMessage)
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
            else {
                if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                    alert(response.errorMessage)
                }
                else {
                    alert("Error Occured while processing your request.");
                }
            }
        }
    });
}


// Delete Building Proposal
function BuildingDelete(id) {

    if (confirm("Are you sure to Delete Road Proposal Details ? ")) {
        blockPage();
        $.post("/BuildingProposal/BuildingProposalDelete/", { id: id }, function (data) {
            unblockPage();
            Alert(data.errorMessage);
            LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());

        });

    }
}

function LoadSRRDAProposals(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbSRRDALSBProposalList").hide();
        $("#dvSRRDALSBProposalListPager").hide();
        $("#tbSRRDAProposalList").show();
        $("#dvSRRDAProposalListPager").show();
        $('#tbSRRDAProposalList').jqGrid('GridUnload');
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');





        SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbSRRDALSBProposalList").show();
        $("#dvSRRDALSBProposalListPager").show();
        $("#tbSRRDAProposalList").hide();
        $("#dvSRRDAProposalListPager").hide();
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');
        $('#tbSRRDAProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbSRRDAProposalList").show();
        $("#dvSRRDAProposalListPager").show();
        $("#tbSRRDALSBProposalList").show();
        $("#dvSRRDALSBProposalListPager").show();
        $('#tbSRRDALSBProposalList').jqGrid('GridUnload');
        $('#tbSRRDAProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
}


// Jqgrid of Proposals
function LoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#btnAddProposal").show();

        $("#tbLSBProposalList").hide();
        $("#dvLSBProposalListPager").hide();
        $("#tbProposalList").show();
        $("#dvProposalListPager").show();
        $('#tbProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "L") {

        $("#btnAddProposal").show();

        $("#tbLSBProposalList").show();
        $("#dvLSBProposalListPager").show();
        $("#tbProposalList").hide();
        $("#dvProposalListPager").hide();
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');

        LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "B") {

        $("#btnAddProposal").show();

        $("#tbLSBProposalList").hide();
        $("#dvLSBProposalListPager").hide();
        $("#tbProposalList").hide();
        $("#dvProposalListPager").hide();

        $("#tbBuildingProposalList").show();
        $("#dvBuildingProposalListPager").show();


        $('#tbBuildingProposalList').jqGrid('GridUnload');
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbProposalList').jqGrid('GridUnload');


        BuildingProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
    else if (IMS_PROPOSAL_TYPE == "A") {
        $("#btnAddProposal").hide();

        $("#tbProposalList").show();
        $("#dvProposalListPager").show();
        $("#tbLSBProposalList").show();
        $("#dvLSBProposalListPager").show();
        $('#tbLSBProposalList').jqGrid('GridUnload');
        $('#tbProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").show();
        $("#dvBuildingProposalListPager").show();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }
}

//---------------------------ujjwal changes---------------------------------//

function LoadPTAProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbPtaLSBProposalList").hide();
        $("#dvPtaLSBProposalListPager").hide();
        $("#tbPtaProposalList").show();
        $("#dvPtaProposalListPager").show();
        $('#tbPtaProposalList').jqGrid('GridUnload');
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        //STAListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbPtaLSBProposalList").show();
        $("#dvPtaLSBProposalListPager").show();
        $("#tbPtaProposalList").hide();
        $("#dvPtaProposalListPager").hide();
        $('#tbPtaProposalList').jqGrid('GridUnload');
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbPtaProposalList").show();
        $("#dvPtaProposalListPager").show();
        $("#tbPtaLSBProposalList").show();
        $("#dvPtaLSBProposalListPager").show();
        $('#tbPtaLSBProposalList').jqGrid('GridUnload');
        $('#tbPtaProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }
}

//----------------------------Ujjwal Changes End----------------------------------------------//


//-----------------------------Shyam Changes ---------------------------------------------------//

function LoadSTAProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbStaLSBProposalList").hide();
        $("#dvStaLSBProposalListPager").hide();
        $("#tbStaProposalList").show();
        $("#dvStaProposalListPager").show();
        $('#tbStaProposalList').jqGrid('GridUnload');
        $('#tbStaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        //STAListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbStaLSBProposalList").show();
        $("#dvStaLSBProposalListPager").show();
        $("#tbStaProposalList").hide();
        $("#dvStaProposalListPager").hide();
        $('#tbStaProposalList').jqGrid('GridUnload');
        $('#tbStaLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbStaProposalList").show();
        $("#dvStaProposalListPager").show();
        $("#tbStaLSBProposalList").show();
        $("#dvStaLSBProposalListPager").show();
        $('#tbStaLSBProposalList').jqGrid('GridUnload');
        $('#tbStaProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');



        STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
    }
}

function LoadMordProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY) {

    if (IMS_PROPOSAL_TYPE == "P") {

        $("#tbMORDLSBProposalList").hide();
        $("#dvMORDLSBProposalListPager").hide();
        $("#tbMORDProposalList").show();
        $("#dvMORDProposalListPager").show();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        // MordListRoadProposals(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE);
        MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY);
    }

    if (IMS_PROPOSAL_TYPE == "L") {

        $("#tbMORDLSBProposalList").show();
        $("#dvMORDLSBProposalListPager").show();
        $("#tbMORDProposalList").hide();
        $("#dvMORDProposalListPager").hide();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY);
    }

    if (IMS_PROPOSAL_TYPE == "B") {


        $("#tbMORDLSBProposalList").hide();
        $("#dvMORDLSBProposalListPager").hide();
        $("#tbMORDProposalList").hide();
        $("#dvMORDProposalListPager").hide();
        $('#tbMORDProposalList').jqGrid('GridUnload');
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');



        $("#tbMoRDBuildingProposalList").show();
        $("#dvMoRDBuildingProposalListPager").show();
        $('#tbMoRDBuildingProposalList').jqGrid('GridUnload');

        MoRDBuildingProposalGrid(MAST_STATE_ID, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY);
        // BuildingProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
    }

    if (IMS_PROPOSAL_TYPE == "A") {

        $("#tbMORDProposalList").show();
        $("#dvMORDProposalListPager").show();
        $("#tbMORDLSBProposalList").show();
        $("#dvMORDLSBProposalListPager").show();
        $('#tbMORDLSBProposalList').jqGrid('GridUnload');
        $('#tbMORDProposalList').jqGrid('GridUnload');

        $("#tbBuildingProposalList").hide();
        $("#dvBuildingProposalListPager").hide();
        $('#tbBuildingProposalList').jqGrid('GridUnload');


        MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY);
        MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY);
    }
}

//-----------------------------Shyam Changes End---------------------------------------------------//

function RedirectFinalizeProposal() {

    if ($('#tbProposalList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbProposalList'),
        selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
        cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        ShowDetails(selectedRowId);
    }
    else {
        alert("Please click on Proposal to select.");
        return false;
    }
}

function RedirectScrutinizeProposal() {
    if ($("#RoleID").val() == '3') {
        if ($('#tbStaProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbStaProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }
        //added by Ujjwal Saket for PTA Login on 1-11-2013
    else if ($("#RoleID").val() == '15') {
        if ($('#tbPtaProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbPtaProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }   //finish addition
}

/**********************************    PIU Region         **********************************************************************************************/
function RoadProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {


    blockPage();
    jQuery("#tbProposalList").jqGrid('GridUnload');
    jQuery("#tbProposalList").jqGrid({
        url: '/Proposal/GetProposals',
        datatype: "json",
        mtype: "POST",
        //colNames: ['Block', "Package Number", "Road Name", "Habitations", "Traffic Intensity", "CBR Details", "Upload", "Technology Details", "View", "Edit", "Delete", "Proposal Status"],
        //colModel: [
        //                    { name: 'Block', index: 'Block', width: 140, sortable: false, align: "left", search: false },
        //                    { name: 'PackageNumber', index: 'PackageNumber', width: 140, sortable: false, align: "left", search: true },
        //                    { name: 'RoadName', index: 'RoadName', width: 290, sortable: false, align: "left", search: true },
        //                    { name: 'Habitations', index: 'Habitations', width: 100, sortable: false, align: "center", search: false },
        //                    { name: 'TrafficIntensity', index: 'TrafficIntensity', width: 90, sortable: false, align: "center", search: false },
        //                    { name: 'CBRDetails', index: 'CBRDetails', width: 90, sortable: false, align: "center", search: false },
        //                    { name: 'Upload', index: 'Upload', width: 50, sortable: false, align: "center", hidden: false, search: false },
        //                    { name: 'TechDetails', index: 'TechDetails', width: 60, sortable: false, align: "center", search: false },
        //                    { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center", search: false },
        //                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", search: false },
        //                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", search: false },
        //                    { name: 'ProposalStatus', index: 'ProposalStatus', width: 40, sortable: false, align: "center", hidden: true, search: false }
        //],
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", $('#PMGSYScheme').val() == 2 ? "State Share Excluding higher Specification (in Lakhs)" : "State Share (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Total Cost (Rs. in Lakhs)", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", /*"Higher Specification Cost (in Lakhs)",*/ "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "Habitations", "Traffic Intensity", "CBR Details", "Upload", "Technology Details", "View", "Edit", "Delete", "Proposal Status"],
        colModel: [
                    { name: 'District', index: 'District', width: 60, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 60, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 70, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
                    //{ name: 'habs', index: 'habs', width: 100, sortable: false, align: "right" },
                    { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
                    { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
                    { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
                    { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
                    { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
                    { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
                    { name: 'MordCost', index: 'MordCost', width: 60, sortable: false, align: "right" },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                    { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1) ? true : false) },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right" },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },

                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 60, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'STA_SANCTIONED_BY', index: 'STA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'STA_SANCTIONED_DATE', index: 'STA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'PTA_SANCTIONED_BY', index: 'PTA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'PTA_SANCTIONED_DATE', index: 'PTA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'Habitations', index: 'Habitations', width: 50, sortable: false, align: "center", search: false },
                    { name: 'TrafficIntensity', index: 'TrafficIntensity', width: 50, sortable: false, align: "center", search: false },
                    { name: 'CBRDetails', index: 'CBRDetails', width: 50, sortable: false, align: "center", search: false },
                    { name: 'Upload', index: 'Upload', width: 50, sortable: false, align: "center", hidden: false, search: false },
                    { name: 'TechDetails', index: 'TechDetails', width: 50, sortable: false, align: "center", search: false },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false },
                    { name: 'Edit', index: 'Edit', width: 50, sortable: false, align: "center", search: false },
                    { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center", search: false },
                    { name: 'ProposalStatus', index: 'ProposalStatus', width: 50, sortable: false, align: "center", hidden: true, search: false }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_BLOCK_ID": MAST_BLOCK_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        width: 'auto',
        rowList: [15, 30, 45],
        rowNum: 15,
        shrinkToFit: false,
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbProposalList #dvProposalListPager").css({ height: '31px' });

            $("#dvProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeProposal();return false;' value='Finalize Proposal'/>")

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionProposal").show("slow");
                $("#tbProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionProposal").hide("slow");
                $("#tbProposalList").jqGrid('hideCol', 'cb');
            }

            var lengthTotal = jQuery("#tbProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
            var costTotal = jQuery("#tbProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
            var habs1000Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
            var habs999Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
            var habs499Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
            var habs250Total = jQuery("#tbProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
            var habsTotal = jQuery("#tbProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
            var stateCost = jQuery("#tbProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
            var maintenanceCost = jQuery("#tbProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            var renewalCost = jQuery("#tbProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
            var mordCost = jQuery("#tbProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');

            var totFundShareRatio = jQuery("#tbProposalList").jqGrid('getCol', 'FundShareRatio', false, 'sum');
            var totFundStateShareCost = jQuery("#tbProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var totMordShareCost = jQuery("#tbProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totTotalStateShare = jQuery("#tbProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            jQuery("#tbProposalList").jqGrid('footerData', 'set',
            {
                RoadName: 'Page Total:',
                PavementLength: parseFloat(lengthTotal).toFixed(3),
                PavementCost: parseFloat(costTotal).toFixed(2),
                Hab1000: habs1000Total,
                Hab999: habs1000Total,
                Hab499: habs1000Total,
                Hab250: habs1000Total,
                HabTotal: habsTotal,
                StateCost: parseFloat(stateCost).toFixed(2),
                MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                RENEWAL_AMT: parseFloat(renewalCost).toFixed(2),
                MordCost: parseFloat(mordCost).toFixed(2),
                HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),

                //FundShareRatio: parseFloat(totFundShareRatio).toFixed(2),
                //StateShareCost: parseFloat(totFundStateShareCost).toFixed(2),
                //MordShareCost: parseFloat(totMordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totTotalStateShare).toFixed(2),
            });

            jQuery("#tbProposalList").jqGrid('footerData', 'set',
            {
                RoadName: 'Grand Total:',
                PavementLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                PavementCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                Hab1000: data.TotalColumn.TOT_HAB1000,
                Hab999: data.TotalColumn.TOT_HAB999,
                Hab499: data.TotalColumn.TOT_HAB499,
                Hab250: data.TotalColumn.TOT_HAB250,
                HabTotal: data.TotalColumn.TOT_HABS,
                StateCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                MAINT_AMT: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                RENEWAL_AMT: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                MordCost: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),

                //FundShareRatio: parseFloat(totFundShareRatio).toFixed(2),
                //StateShareCost: parseFloat(totFundStateShareCost).toFixed(2),
                //MordShareCost: parseFloat(totMordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totTotalStateShare).toFixed(2),
            });


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
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
    }); //end of grid
    // $("#tbProposalList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

    jQuery("#tbProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}
/**********************************    PIU Region END      **********************************************************************************************/

/**********************************    PTA Region         **********************************************************************************************/
function PTAListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    //alert("Year :" + IMS_YEAR + "  District : " + MAST_DISTRICT_ID + "  Batch : " + IMS_BATCH + "  funding agency : " + IMS_STREAM + "  Proposal Type : " + IMS_PROPOSAL_TYPE + " Proposal Status :  " + IMS_PROPOSAL_STATUS);
    blockPage();

    jQuery("#tbPtaProposalList").jqGrid({
        url: '/Proposal/GetPTARoadProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', "Package Number", "Road Name", "Pavement Length", "Pavement Cost", "Upload", "View"],
        colModel: [
                    { name: 'Block', index: 'Block', width: 150, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "left" },
                    { name: 'RoadName', index: 'RoadName', width: 290, sortable: false, align: "left" },
                    { name: 'PavementLength', index: 'PavementLength', width: 200, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 200, sortable: false, align: "right" },
                    { name: 'UploadDetails', index: 'UploadDetails', width: 70, sortable: false, align: "center" },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 70, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS },
        pager: jQuery('#dvPtaProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        loadComplete: function () {
            $("#tbPtaProposalList #dvPtaProposalListPager").css({ height: '31px' });

            $("#dvPtaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid


}
/**********************************    PTA Region End     **********************************************************************************************/



/**********************************    STA Region         **********************************************************************************************/
function STAListRoadProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {

    if ($("#ddlState").val() > 0) {
        //alert("Year :" + IMS_YEAR + "  District : " + MAST_DISTRICT_ID + "  Batch : " + IMS_BATCH + "  funding agency : " + IMS_STREAM + "  Proposal Type : " + IMS_PROPOSAL_TYPE + " Proposal Status :  " + IMS_PROPOSAL_STATUS);
        blockPage();

        jQuery("#tbStaProposalList").jqGrid({
            url: '/Proposal/GetSTARoadProposals',
            datatype: "json",
            mtype: "POST",
            colNames: ['Block', "Package Number", "Road Name", "Pavement Length", "Pavement Cost", "Upload", "View"],
            colModel: [
                        { name: 'Block', index: 'Block', width: 150, sortable: false, align: "left" },
                        { name: 'PackageNumber', index: 'PackageNumber', width: 150, sortable: false, align: "left" },
                        { name: 'RoadName', index: 'RoadName', width: 290, sortable: false, align: "left" },
                        { name: 'PavementLength', index: 'PavementLength', width: 150, sortable: false, align: "right" },
                        { name: 'PavementCost', index: 'PavementCost', width: 150, sortable: false, align: "right" },
                        { name: 'UploadDetails', index: 'UploadDetails', width: 100, sortable: false, align: "center" },
                        { name: 'ShowDetails', index: 'ShowDetails', width: 100, sortable: false, align: "center" }

            ],
            postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_STATE": $("#ddlState").val(), "value": Math.random() },
            pager: jQuery('#dvStaProposalListPager'),
            rowList: [15, 30, 45],
            rowNum: 15,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;Road Proposals",
            height: 'auto',
            width: 'auto',
            //autowidth: true,
            sortname: 'Block',
            rownumbers: true,
            loadComplete: function () {
                $("#tbStaProposalList #dvStaProposalListPager").css({ height: '31px' });

                $("#dvStaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")


                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    //window.location.href = "/Login/SessionExpire";
                }
                else {
                    //alert("Session Timeout !!!");
                    alert('Error occurred');
                }
            }

        }); //end of grid

    }
    //else {
    //    alert("Please Select State");
    //}
}
/**********************************    STA Region End     **********************************************************************************************/

/// Bulk Finalize Functionality
function BulkFinalizeProposals() {
    var Proposals = $("#tbMORDProposalList").jqGrid('getGridParam', 'selarrrow').toString().split(',');

    if (Proposals == "" || Proposals == null || Proposals == undefined) {
        alert("Please Select at least one Proposal.");
        return false;
    }
    else if (Proposals.length == 0) {
        alert("Please Select at least one Proposal.");
        return false;
    }
    else if (Proposals.length == 1) {
        ShowDetails(Proposals[0]);
    }
    else if (Proposals.length > 1) {
        ShowBulkDetails(Proposals);
    }
}

function ShowBulkDetails(Proposals) {

    //    return false;    

    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Cumulative Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        blockPage();

        //code commented by Vikram as for more than 200 proposals error occurred.
        //$("#divProposalForm").load('/Proposal/BulkDetails?id=' + Proposals, function () {
        //    $.validator.unobtrusive.parse($('#divProposalForm'));
        //    unblockPage();
        //});

        $.post('/Proposal/BulkDetails/', { Proposals: Proposals }, function (data) {
            $("#divProposalForm").html(data);
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });


        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#idFilterDiv').trigger('click');
    //jQuery('#tbMORDProposalList').jqGrid('setGridState', 'hidden');
}

/**********************************    MORD Region        **********************************************************************************************/
function MordListRoadProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY) {

    blockPage();
    jQuery("#tbMORDProposalList").jqGrid({
        url: '/Proposal/GetMORDRoadProposalsLWE',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (in Lakhs)" : "State Share Excluding Higher Specifications (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Total Cost", "5 Years Estimated Maintenance Cost (Rs.in lakhs)", "6th Year Renwal Cost (Rs.in lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View"],
        colModel: [
                    { name: 'District', index: 'District', width: 60, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 60, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 80, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
                    //{ name: 'habs', index: 'habs', width: 100, sortable: false, align: "right" },
                    { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
                    { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
                    { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
                    { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
                    { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
                    { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 60, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                    { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 /*|| $("#PMGSYScheme").val() == 3*/) ? true : false) },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 60, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'STA_SANCTIONED_BY', index: 'STA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'STA_SANCTIONED_DATE', index: 'STA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    //{ name: 'PTA_SANCTIONED_BY', index: 'PTA_SANCTIONED_BY', width: 60, sortable: false, align: "right" },
                    //{ name: 'PTA_SANCTIONED_DATE', index: 'PTA_SANCTIONED_DATE', width: 60, sortable: false, align: "right" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT, "IMS_AGENCY": IMS_AGENCY },
        pager: jQuery('#dvMORDProposalListPager'),
        rowList: [100, 200, 300, 400, 500],
        rowNum: 100,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals for LWE",
        height: '400px',
        //width: 'auto',
        multiselect: true,
        footerrow: true,
        footerrow1: true,
        sortname: 'District',
        autowidth: true,
        shrinkToFit: false,
        rownumbers: true,
        loadComplete: function (data) {
            //$("#tbStaProposalList #dvStaProposalListPager").css({ height: '31px' });
            //$("#dvStaProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeProposal();return false;' value='Scrutinize Proposal'/>")
            $("#tbMORDProposalList #dvProposalListPager").css({ height: '31px' });

            $("#dvMORDProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idSanctionProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'BulkFinalizeProposals();return false;' value='Sanction Proposal'/>")

            if (data.records < 15) {
                $("#tbMORDProposalList").jqGrid('setGridHeight', 'auto');
            }

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionProposal").show("slow");
                $("#tbMORDProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionProposal").hide("slow");
                $("#tbMORDProposalList").jqGrid('hideCol', 'cb');
            }

            if ($("#RoleID").val() == '65') {
                $("#idSanctionProposal").hide("slow");
                $("#tbMORDProposalList").jqGrid('hideCol', 'cb');
            }

            var lengthTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
            var costTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
            var habs1000Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
            var habs999Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
            var habs499Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
            var habs250Total = jQuery("#tbMORDProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
            var habsTotal = jQuery("#tbMORDProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
            var stateCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
            var maintenanceCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
            var renewalCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
            var higherSpecCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbMORDProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbMORDProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            jQuery("#tbMORDProposalList").jqGrid('footerData', 'set',
            {
                RoadName: 'Page Total:',
                PavementLength: parseFloat(lengthTotal).toFixed(3),
                PavementCost: parseFloat(costTotal).toFixed(2),
                Hab1000: habs1000Total,
                Hab999: habs1000Total,
                Hab499: habs1000Total,
                Hab250: habs1000Total,
                HabTotal: habsTotal,
                StateCost: parseFloat(stateCost).toFixed(2),
                MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                RENEWAL_AMT: parseFloat(renewalCost).toFixed(2),
                HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2),

                //TotalCost: parseFloat(totalCost).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
            });

            jQuery("#tbMORDProposalList").jqGrid('footerData', 'set',
            {
                RoadName: 'Grand Total:',
                PavementLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                PavementCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                Hab1000: data.TotalColumn.TOT_HAB1000,
                Hab999: data.TotalColumn.TOT_HAB999,
                Hab499: data.TotalColumn.TOT_HAB499,
                Hab250: data.TotalColumn.TOT_HAB250,
                HabTotal: data.TotalColumn.TOT_HABS,
                StateCost: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                MAINT_AMT: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                RENEWAL_AMT: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
            });

            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid   


    jQuery("#tbMORDProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: false,
        groupHeaders: [
          { startColumnName: 'Hab1000', numberOfColumns: 5, titleText: 'Habitations with Population Range' },
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });


}
/**********************************    MORD Region End    **********************************************************************************************/

//------------ LSB ---------------------------------
function LSBProposalGrid(IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    jQuery("#tbLSBProposalList").jqGrid({
        url: '/LSBProposal/GetLSBProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Road Name", "LSB Name", "LSB Length (mtrs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (Rs Lakhs)" : "State Share Excluding Higher Specification (Rs Lakhs)", "Higher Specification Cost (Rs Lakhs)",
                    "MoRD Cost (Rs Lakhs)", "Total Cost", "Maintenance Cost (Rs Lakhs)", "Renewal Cost (Rs Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Component Details", "Other Details", "Upload", "View", "Edit", "Delete"],
        colModel: [
                            { name: 'Block', index: 'Block', width: 80, sortable: false, align: "left" },
                            { name: 'PackageNumber', index: 'PackageNumber', width: 70, sortable: false, align: "left" },
                            { name: 'RoadName', index: 'RoadName', width: 200, sortable: false, align: "left" },
                            { name: 'LSBName', index: 'LSBName', width: 160, sortable: false, align: "left" },
                            { name: 'LSBLength', index: 'LSBLength', width: 80, sortable: false, align: "right" },
                            { name: 'EstimatedCostState', index: 'EstimatedCostState', width: 70, sortable: false, align: "right" },
                            { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                            { name: 'EstimatedCost', index: 'EstimatedCost', width: 70, sortable: false, align: "right" },
                            { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                            { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 70, sortable: false, align: "right" },
                            { name: 'RenewalCost', index: 'RenewalCost', width: 70, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                            { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                            { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                            { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                            { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                            { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
                            { name: 'ComponentDetails', index: 'ComponentDetails', width: 70, sortable: false, align: "center" },
                            { name: 'OtherDetails', index: 'OtherDetails', width: 40, sortable: false, align: "center" },
                            { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center" },
                            { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
                            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
                            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_BLOCK_ID": MAST_BLOCK_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvLSBProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: 'auto',
        width: 'auto',
        autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbLSBProposalList #dvLSBProposalListPager").css({ height: '31px' });

            $("#dvLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeLSBProposal();return false;' value='Finalize Proposal'/>")

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionLSBProposal").show("slow");
                $("#tbLSBProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionLSBProposal").hide("slow");
                $("#tbLSBProposalList").jqGrid('hideCol', 'cb');
            }


            var lengthTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'BridgeLength', false, 'sum');
            var statecostTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'StateShare', false, 'sum');
            var mordcostTotal = jQuery("#tbLSBProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbLSBProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbLSBProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');
            // alert(stateShareCost);
            jQuery("#tbLSBProposalList").jqGrid('footerData', 'set',
            {
                LSBName: 'Grand Total:',
                LSBLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                EstimatedCostState: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                EstimatedCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                MaintenanceCost: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
            });



            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbLSBProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

    jQuery("#tbLSBProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}


//------------ Building Proposal List ---------------------------------
function BuildingProposalGrid(syear, block, batch, stream, proptype, propstatus, propconnect) {
    /*$.post('/BuildingProposal/GetProposals?' + Math.random(), { "syear": syear, "block": block, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect }, function (data) {
        alert("Loaded: "+ JSON.stringify(data));

    });
    */

    jQuery("#tbBuildingProposalList").jqGrid({
        url: '/BuildingProposal/GetProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Year", "Work Name", "Total Cost", "Batch", "Funding Agency", "Edit", "Delete"],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 150, sortable: false, align: "left" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 150, sortable: false, align: "left" },
                            { name: 'IMS_YEAR_FINANCIAL', index: 'IMS_YEAR_FINANCIAL', width: 100, sortable: false, align: "left" },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 300, sortable: false, align: "left" },
                            { name: 'IMS_PAV_EST_COST', index: 'IMS_PAV_EST_COST', width: 100, sortable: false, align: "right" },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 60, sortable: false, align: "right" },
                            { name: 'MAST_FUNDING_AGENCY_NAME', index: 'MAST_FUNDING_AGENCY_NAME', width: 100, sortable: false, align: "center" },
                            { name: 'Edit', index: 'Edit', width: 50, sortable: false, align: "center" },
                            { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center" }

        ],
        postData: { "syear": syear, "block": block, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect },
        pager: jQuery('#dvBuildingProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Building Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth:true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbBuildingProposalList #dvBuildingProposalListPager").css({ height: '31px' });

            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbBuildingProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

}

//---------------------------------PTA LSB Region Starts-----------------------//

function PTAListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {
    //alert(MAST_DISTRICT_ID + IMS_STREAM);
    blockPage();
    jQuery("#tbPtaLSBProposalList").jqGrid({
        url: '/LSBProposal/GetPTALSBProposals',
        datatype: "json",
        mtype: "POST",
        colNames: ['Block', "Package Number", "Road Name", "LSB Name", "LSB Length (mtrs)", "State Share (lakhs)", "MoRD Cost (lakhs)", "View", "Upload"],
        colModel: [
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "left" },
                    { name: 'RoadName', index: 'RoadName', width: 280, sortable: false, align: "left" },
                    { name: 'BridgeName', index: 'BridgeName', width: 260, sortable: false, align: "left" },
                    { name: 'BridgeLength', index: 'BridgeLength', width: 100, sortable: false, align: "right" },
                    { name: 'StateShare', index: 'StateShare', width: 80, sortable: false, align: "right" },
                    { name: 'MordCost', index: 'MordCost', width: 80, sortable: false, align: "right" },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
                    { name: 'UploadDetails', index: 'UploadDetails', width: 40, sortable: false, align: "center" },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "value": Math.random() },
        pager: jQuery('#dvPtaLSBProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth: true,
        sortname: 'Block',
        rownumbers: true,
        loadComplete: function () {
            $("#tbPtaLSBProposalList #dvPtaLSBProposalListPager").css({ height: '31px' });
            $("#dvPtaLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeLSBProposal();return false;' value='Scrutinize Proposal'/>")
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid


}
//--------------------------------------------    PTA LSB Region End    ----------------------------------------------//


function STAListLSBProposals(IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS) {
    //alert(MAST_DISTRICT_ID + IMS_STREAM);

    if ($("#ddlState").val() > 0) {

        blockPage();
        jQuery("#tbStaLSBProposalList").jqGrid({
            url: '/LSBProposal/GetSTALSBProposals',
            datatype: "json",
            mtype: "POST",
            colNames: ['Block', "Package Number", "Road Name", "LSB Name", "LSB Length (mtrs)", "State Share (lakhs)", "MoRD Cost (lakhs)", "View", "Upload", "Joint Inspection"],
            colModel: [
                        { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                        { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "left" },
                        { name: 'RoadName', index: 'RoadName', width: 280, sortable: false, align: "left" },
                        { name: 'BridgeName', index: 'BridgeName', width: 260, sortable: false, align: "left" },
                        { name: 'BridgeLength', index: 'BridgeLength', width: 100, sortable: false, align: "right" },
                        { name: 'StateShare', index: 'StateShare', width: 100, sortable: false, align: "right" },
                        { name: 'MordCost', index: 'MordCost', width: 100, sortable: false, align: "right" },
                        { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" },
                        { name: 'UploadDetails', index: 'UploadDetails', width: 40, sortable: false, align: "center" },
                        { name: 'JointInspections', index: 'JointInspections', width: 40, sortable: false, align: "center" }
            ],
            postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_STATE": $("#ddlState").val(), "value": Math.random() },
            pager: jQuery('#dvStaLSBProposalListPager'),
            rowList: [15, 30, 45],
            rowNum: 15,
            viewrecords: true,
            recordtext: '{2} records found',
            caption: "&nbsp;&nbsp;LSB Proposals",
            height: 'auto',
            width: 'auto',
            //autowidth: true,
            sortname: 'Block',

            rownumbers: true,
            loadComplete: function () {
                $("#tbStaLSBProposalList #dvStaLSBProposalListPager").css({ height: '31px' });
                $("#dvStaLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idScrutinizeLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectScrutinizeLSBProposal();return false;' value='Scrutinize Proposal'/>")
                unblockPage();
            },
            loadError: function (xhr, status, error) {
                unblockPage();
                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    //window.location.href = "/Login/SessionExpire";
                }
                else {
                    //alert("Session Timeout !!!");
                    alert('Error occurred');
                }
            }
        }); //end of grid

    }
    //else {
    //    alert("Please select State");
    //}
}
//--------------------------------------------    STA LSB Region End    ----------------------------------------------//

function BulkFinalizeLSBProposals() {
    var Proposals = $("#tbMORDLSBProposalList").jqGrid('getGridParam', 'selarrrow').toString().split(',');

    if (Proposals == "" || Proposals == null || Proposals == undefined) {
        alert("Please Select at least one Proposal.");
        return false;
    }
    else if (Proposals.length == 0) {
        alert("Please Select at least one Proposal.");
        return false;
    }
    else if (Proposals.length == 1) {
        ShowLSBDetails(Proposals[0]);
    }
    else if (Proposals.length > 1) {
        ShowBulkLSBDetails(Proposals);
    }
}

function ShowBulkLSBDetails(Proposals) {
    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Cumulative Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/BulkLSBDetails?id=' + Proposals, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });
    $('#idFilterDiv').trigger('click');
    //jQuery('#tbMORDProposalList').jqGrid('setGridState', 'hidden');
}
//--------------------------------------------    MORD LSB Region     ----------------------------------------------//
function MordListLSBProposals(IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, IMS_AGENCY) {

    blockPage();
    jQuery("#tbMORDLSBProposalList").jqGrid({
        url: '/Proposal/GetMORDLSBProposalsLWE',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "LSB Name", "LSB Length (mtrs)", "MoRD Cost (lakhs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (lakhs)" : "State Share Excluding Higher Specifications (lakhs)", "Higher Specification Cost (in Lacs)", "Total Cost", "Maintenance Cost (in Lacs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View"],
        colModel: [
                    { name: 'District', index: 'District', width: 100, sortable: false, align: "left" },
                    { name: 'Block', index: 'Block', width: 100, sortable: false, align: "left" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 100, sortable: false, align: "center" },
                    { name: 'Year', index: 'Year', width: 100, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                    { name: 'BridgeName', index: 'BridgeName', width: 150, sortable: false, align: "left" },
                    { name: 'BridgeLength', index: 'BridgeLength', width: 90, sortable: false, align: "right" },
                    { name: 'MordCost', index: 'MordCost', width: 90, sortable: false, align: "right" },
                    { name: 'StateShare', index: 'StateShare', width: 90, sortable: false, align: "right" },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                    { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                    { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 90, sortable: false, align: "right" },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 150, sortable: false, align: "left" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 150, sortable: false, align: "left" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 150, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" }
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_STATE_ID": MAST_STATE_ID, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT, "IMS_AGENCY": IMS_AGENCY },
        pager: jQuery('#dvMORDLSBProposalListPager'),
        rowList: [50, 100, 150],
        rowNum: 50,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals for LWE",
        height: '400px',
        //width: 'auto',
        autowidth: true,
        sortname: 'District',
        shrinkToFit: false,
        multiselect: true,
        footerrow: true,
        rownumbers: true,
        loadComplete: function (data) {
            $("#tbMORDLSBProposalList #dvMORDLSBProposalListPager").css({ height: '31px' });
            $("#dvMORDLSBProposalListPager_left").html("<input type='button' style='margin-left:27px' id='idSanctionLSBProposal' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'BulkFinalizeLSBProposals();return false;' value='Sanction Proposal'/>")
            unblockPage();

            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionLSBProposal").show("slow");
                $("#tbMORDLSBProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionLSBProposal").hide("slow");
                $("#tbMORDLSBProposalList").jqGrid('hideCol', 'cb');
            }


            var lengthTotal = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'BridgeLength', false, 'sum');
            var statecostTotal = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'StateShare', false, 'sum');
            var mordcostTotal = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbMORDLSBProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            //  alert(stateShareCost);

            jQuery("#tbMORDLSBProposalList").jqGrid('footerData', 'set',
            {
                BridgeName: 'Grand Total:',
                BridgeLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                StateShare: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                MordCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                MaintenanceCost: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
            });

            //jQuery("#tbMORDLSBProposalList").jqGrid('footerData', 'set',
            //{
            //    BridgeName: 'Total:',
            //    BridgeLength: parseFloat(lengthTotal).toFixed(3),
            //    StateShare: parseFloat(statecostTotal).toFixed(2),
            //    MordCost: parseFloat(mordcostTotal).toFixed(2),
            //    HIGHER_SPECS : parseFloat(higherSpecCost).toFixed(2),
            //});

        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        }
    }); //end of grid   


    jQuery("#tbMORDLSBProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });
}

//--------------------------------------------    MORD LSB Region End    ----------------------------------------------//
//Show Details
function ShowLSBDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >LSB Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/LSBDetails?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbStaLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbPtaLSBProposalList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

// Edit unLocked Proposal         
function EditUnlockedLSBDetails(id) {
    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit UnLocked LSB Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/EditUnlockedLSB?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

// Editing the Proposal
function EditLSBDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit LSB Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/LSBProposal/EditLSB?id=' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//Delete LSB Proposal
function DeleteLSBDetails(id) {

    if (confirm("Are you sure to delete proposal details ? ")) {

        $.ajax({
            url: '/LSBProposal/DeleteLSBConfirmed?id=' + id,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success == "true") {
                    alert("Proposal Deleted Successfully.");
                    $("#tbLSBProposalList").trigger('reloadGrid');
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.errorMessage)
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

//Edit Other Details for LSB
function EditLSBOtherDetails(urlparamater) {

    jQuery('#tbLSBProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >LSB Other Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/LSBProposal/LSBOtherDetails/" + urlparamater, function () {
            //ShowHabitations($("#IMS_PR_ROAD_CODE").val(), 0);
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//Edit Component Details for LSB
function EditComponentDetails(urlparamater) {

    jQuery('#tbLSBProposalList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >LSB Component Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );
    blockPage();
    $('#accordion').show('fast', function () {
        $("#divProposalForm").load("/LSBProposal/ShowLSBComponentList/" + urlparamater, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
    });
    $("#divProposalForm").css('height', 'auto');
    $('#divProposalForm').show('fast');

    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function RedirectFinalizeLSBProposal() {

    //alert("Test");
    if ($('#tbLSBProposalList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbLSBProposalList'),
        selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
        cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        //alert(cellValue);
        //alert($("#IMS_LOCK_STATUS").val());

        ShowLSBDetails(selectedRowId);
    }
    else {
        alert("Please click on Proposal to select.");
        return false;
    }
}

function RedirectScrutinizeLSBProposal() {
    if ($("#RoleID").val() == '3') {

        if ($('#tbStaLSBProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbStaLSBProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowLSBDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }
    else if ($("#RoleID").val() == '15') {
        if ($('#tbPtaLSBProposalList').jqGrid('getGridParam', 'selrow')) {

            var myGrid = $('#tbPtaLSBProposalList'),
            selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
            cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

            ShowLSBDetails(selectedRowId);
        }
        else {
            alert("Please click on Proposal to select.");
            return false;
        }
    }
}

function RedirectSanctionLSBProposal() {
    if ($('#tbMORDLSBProposalList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbMORDLSBProposalList'),
        selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
        cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        ShowLSBDetails(selectedRowId);
    }
    else {
        alert("Please click on Proposal to select.");
        return false;
    }
}
//-----------LSB End ------------------------------

//new change done by Vikram on 17-09-2013

function AddTechnologyDetails(id) {

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Technology Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/ListTechnologyDetails/' + id, function (response) {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}



//end of change


//-------------------------------   SRRDA Details --------------------------------------//



function SRRDARoadProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    
    blockPage();
    jQuery("#tbSRRDAProposalList").jqGrid({
        url: '/Proposal/GetProposalsForSRRDALWE',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'Block', "Package Number", "Year", "Road Name", "1000+", "999-500", "499-250", "Less Than 250", "Total Habitations", "Pavement Length (in Kms.)", "MoRD Share (in Lakhs)", ($('#PMGSYScheme').val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Share (in Lakhs)" : "State Share Excluding Higher Specification (in Lakhs)", "Higher Specification Cost (in Lakhs)", "Total Cost", "Maintenance Cost (in Lakhs)", "Renewal Amount (in Lakhs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "Stage Construction", "STA (Scrutiny Date)", "PTA (Scrutiny Date)", "Status", "View", "Delete"],
        colModel: [
                    { name: 'District', index: 'District', width: 50, sortable: false, align: "center" },
                    { name: 'Block', index: 'Block', width: 50, sortable: false, align: "center" },
                    { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "center" },
                    { name: 'IMS_YEAR', index: 'IMS_YEAR', width: 50, sortable: false, align: "center" },
                    { name: 'RoadName', index: 'RoadName', width: 180, sortable: false, align: "left" },
                    { name: 'Hab1000', index: 'Hab1000', width: 50, sortable: false, align: "right" },
                    { name: 'Hab999', index: 'Hab999', width: 50, sortable: false, align: "right" },
                    { name: 'Hab499', index: 'Hab499', width: 50, sortable: false, align: "right" },
                    { name: 'Hab250', index: 'Hab250', width: 50, sortable: false, align: "right" },
                    { name: 'HabTotal', index: 'HabTotal', width: 50, sortable: false, align: "right" },
                    { name: 'PavementLength', index: 'PavementLength', width: 50, sortable: false, align: "right" },
                    { name: 'PavementCost', index: 'PavementCost', width: 60, sortable: false, align: "right" },
                    { name: 'StateCost', index: 'StateCost', width: 60, sortable: false, align: "right" },
                    { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                    { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                    { name: 'MAINT_AMT', index: 'MAINT_AMT', width: 60, sortable: false, align: "right" },
                    { name: 'RENEWAL_AMT', index: 'RENEWAL_AMT', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 /*|| $("#PMGSYScheme").val() == 3*/) ? true : false) },
                    { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                    { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                    { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                    { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },

                    { name: 'STAGE_CONST', index: 'STAGE_CONST', width: 60, sortable: false, align: "center" },
                    { name: 'STA_SCRUTINY', index: 'STA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    { name: 'PTA_SCRUTINY', index: 'PTA_SCRUTINY', width: 120, sortable: false, align: "left" },
                    { name: 'PROPOSAL_STATUS', index: 'PROPOSAL_STATUS', width: 60, sortable: false, align: "center", hidden: true },
                    { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", search: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center", search: false, hidden: (parseInt($("#RoleID").val()) == 25 ? true : false) },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvSRRDAProposalListPager'),
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Road Proposals for LWE",
        height: '400px',
        width: 'auto',
        rowList: [50, 100, 150, 200],
        rowNum: 50,
        autowidth: true,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        shrinkToFit: false,
        loadComplete: function (data) {

            if (data.records == 0) {
                $("#tbSRRDAProposalList").css('ui-jqgrid-bdiv');
            }


            if (data.TotalColumn != null) {
                var lengthTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'PavementLength', false, 'sum');
                var costTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'PavementCost', false, 'sum');
                var habs1000Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab1000', false, 'sum');
                var habs999Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab999', false, 'sum');
                var habs499Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab499', false, 'sum');
                var habs250Total = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'Hab250', false, 'sum');
                var habsTotal = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'HabTotal', false, 'sum');
                var stateCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'StateCost', false, 'sum');
                var maintenanceCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'MAINT_AMT', false, 'sum');
                var renewalCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'RENEWAL_AMT', false, 'sum');
                var higherSpecCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

                var totalCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
                var stateShareCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
                var mordShareCost = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
                var totalStateShare = jQuery("#tbSRRDAProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

                jQuery("#tbSRRDAProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Page Total:',
                    PavementLength: parseFloat(lengthTotal).toFixed(3),
                    PavementCost: parseFloat(costTotal).toFixed(2),
                    Hab1000: habs1000Total,
                    Hab999: habs1000Total,
                    Hab499: habs1000Total,
                    Hab250: habs1000Total,
                    HabTotal: habsTotal,
                    StateCost: parseFloat(stateCost).toFixed(2),
                    MAINT_AMT: parseFloat(maintenanceCost).toFixed(2),
                    RENEWAL_AMT: parseFloat(renewalCost).toFixed(2),
                    HIGHER_SPECS: parseFloat(higherSpecCost).toFixed(2),
                    TotalCost: parseFloat(totalCost).toFixed(2),
                    StateShareCost: parseFloat(stateShareCost).toFixed(2),
                    MordShareCost: parseFloat(mordShareCost).toFixed(2),
                    TotalStateShare: parseFloat(totalStateShare).toFixed(2),
                });

                jQuery("#tbSRRDAProposalList").jqGrid('footerData', 'set',
                {
                    RoadName: 'Grand Total:',
                    PavementLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                    PavementCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                    Hab1000: data.TotalColumn.TOT_HAB1000,
                    Hab999: data.TotalColumn.TOT_HAB999,
                    Hab499: data.TotalColumn.TOT_HAB499,
                    Hab250: data.TotalColumn.TOT_HAB250,
                    HabTotal: data.TotalColumn.TOT_HABS,
                    StateCost: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                    MAINT_AMT: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),
                    RENEWAL_AMT: parseFloat(data.TotalColumn.TOT_RENEWAL_COST).toFixed(2),
                    HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                    TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                    StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                    MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                    TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                    //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                    //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                    //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
                });
            }


            unblockPage();
        },
        loadError: function (xhr, status, error) {
            unblockPage();
            if (xhr.responseText == "session expired") {
                alert(error);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert(error);
                alert('Error occurred');
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
    }); //end of grid

    jQuery("#tbSRRDAProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });

}



function SRRDALSBProposalGrid(IMS_YEAR, MAST_DISTRICT_ID, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAM, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT) {
    jQuery("#tbSRRDALSBProposalList").jqGrid({
        //url: '/LSBProposal/GetLSBProposalsForSRRDA?' + Math.random(),
        url: '/Proposal/GetLSBProposalsForSRRDALWE?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Package", "Road Name", "LSB Name", "LSB Length (mtrs)", ($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? "State Cost (Lacs)" : "State Cost Excluding Higher Specification (Lacs)", "Higher Specification Cost", "Mord Cost (Lacs)", "Total Cost", "Maintenance Cost (Lacs)", "Fund Sharing Ratio", "State Share Cost (in Lakhs)", "Mord Share Cost (in Lakhs)", "Total State Share (in Lakhs)", "Total Share Cost (in Lakhs)", "View", "Delete"],
        colModel: [
                        { name: 'District', index: 'District', width: 80, sortable: false, align: "left" },
                        { name: 'Block', index: 'Block', width: 80, sortable: false, align: "left" },
                        { name: 'PackageNumber', index: 'PackageNumber', width: 80, sortable: false, align: "left" },
                        { name: 'RoadName', index: 'RoadName', width: 250, sortable: false, align: "left" },
                        { name: 'LSBName', index: 'LSBName', width: 180, sortable: false, align: "left" },
                        { name: 'LSBLength', index: 'LSBLength', width: 80, sortable: false, align: "right" },
                        { name: 'StateShare', index: 'StateShare', width: 80, sortable: false, align: "right" },
                        { name: 'HIGHER_SPECS', index: 'HIGHER_SPECS', width: 60, sortable: false, align: "right", hidden: (($("#PMGSYScheme").val() == 1 || $("#PMGSYScheme").val() == 3) ? true : false) },
                        { name: 'MordCost', index: 'MordCost', width: 80, sortable: false, align: "right" },
                        { name: 'TotalCost', index: 'TotalCost', width: 60, sortable: false, align: "right" },
                        { name: 'MaintenanceCost', index: 'MaintenanceCost', width: 80, sortable: false, align: "right" },
                        { name: 'FundShareRatio', index: 'FundShareRatio', width: 60, sortable: false, align: "right", },
                        { name: 'StateShareCost', index: 'StateShareCost', width: 60, sortable: false, align: "right", },
                        { name: 'MordShareCost', index: 'MordShareCost', width: 60, sortable: false, align: "right", },
                        { name: 'TotalStateShare', index: 'TotalStateShare', width: 60, sortable: false, align: "right", },
                        { name: 'TotalShareCost', index: 'TotalShareCost', width: 60, sortable: false, align: "right", hidden: true },
                        { name: 'ShowDetails', index: 'ShowDetails', width: 50, sortable: false, align: "center", hidden: true },
                        { name: 'Delete', index: 'Delete', width: 50, sortable: false, align: "center" },
        ],
        postData: { "IMS_YEAR": IMS_YEAR, "MAST_DISTRICT_ID": MAST_DISTRICT_ID, "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "IMS_BATCH": IMS_BATCH, "IMS_STREAM": IMS_STREAM, "IMS_PROPOSAL_TYPE": IMS_PROPOSAL_TYPE, "IMS_PROPOSAL_STATUS": IMS_PROPOSAL_STATUS, "IMS_UPGRADE_CONNECT": IMS_UPGRADE_CONNECT },
        pager: jQuery('#dvSRRDALSBProposalListPager'),
        rowList: [25, 50, 75, 100],
        rowNum: 25,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Proposals for LWE",
        height: '400px',
        //width: 'auto',
        autowidth: true,
        shrinkToFit: false,
        sortname: 'District',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            unblockPage();
            if ($("#ddlProposalStatus").val() == "N") {
                $("#idSanctionLSBProposal").show("slow");
                $("#tbSRRDALSBProposalList").jqGrid('showCol', 'cb');
            }
            else {
                $("#idSanctionLSBProposal").hide("slow");
                $("#tbSRRDALSBProposalList").jqGrid('hideCol', 'cb');
            }


            var lengthTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'BridgeLength', false, 'sum');
            var statecostTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'StateShare', false, 'sum');
            var mordcostTotal = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'MordCost', false, 'sum');
            var higherSpecCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'HIGHER_SPECS', false, 'sum');

            var totalCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'TotalCost', false, 'sum');
            var stateShareCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'StateShareCost', false, 'sum');
            var mordShareCost = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'MordShareCost', false, 'sum');
            var totalStateShare = jQuery("#tbSRRDALSBProposalList").jqGrid('getCol', 'TotalStateShare', false, 'sum');

            jQuery("#tbSRRDALSBProposalList").jqGrid('footerData', 'set',
            {
                LSBName: 'Grand Total:',
                LSBLength: parseFloat(data.TotalColumn.TOT_PAV_LENGTH).toFixed(3),
                StateShare: parseFloat(data.TotalColumn.TOT_STATE_COST).toFixed(2),
                MordCost: parseFloat(data.TotalColumn.TOT_MORD_COST).toFixed(2),
                HIGHER_SPECS: parseFloat(data.TotalColumn.TOT_HIGHER_SPEC).toFixed(2),
                MaintenanceCost: parseFloat(data.TotalColumn.TOT_MANE_COST).toFixed(2),

                TotalCost: parseFloat(data.TotalColumn.TOT_COST).toFixed(2),
                StateShareCost: parseFloat(data.TotalColumn.STATE_SHARE_COST).toFixed(2),
                MordShareCost: parseFloat(data.TotalColumn.MORD_SHARE_COST).toFixed(2),
                TotalStateShare: parseFloat(data.TotalColumn.TOTAL_STATE_SHARE).toFixed(2),
                //StateShareCost: parseFloat(stateShareCost).toFixed(2),
                //MordShareCost: parseFloat(mordShareCost).toFixed(2),
                //TotalStateShare: parseFloat(totalStateShare).toFixed(2),
            });



        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbLSBProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid


    jQuery("#tbSRRDALSBProposalList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'FundShareRatio', numberOfColumns: 5, titleText: 'Fund Sharing' },
        ]
    });
}


//Edit Other Details for LSB
function BuildingUpdate(id) {




    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Building Proposal Update</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/BuildingProposal/BuildingEdit/" + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


}




//-------------------------------- SRRDA Details Ends Here --------------------------------//




//show the Details of Building Proposal
function ShowBuildingDetails(id) {
    //alert("In Building Details");

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Building Proposal Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/BuildingProposal/BuildingDetails/' + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            //if ($("#RoleID").val() == '25')
            unblockPage();
        });
        $('#divProposalForm').show('slow');
        $("#divProposalForm").css('height', 'auto');
    });

    $("#tbProposalList").jqGrid('setGridState', 'hidden');
    $('#tbStaProposalList').jqGrid('setGridState', 'hidden');
    $('#tbPtaProposalList').jqGrid('setGridState', 'hidden');
    $("#tbMORDProposalList").jqGrid('setGridState', 'hidden');
    $("#tbSRRDAProposalList").jqGrid('setGridState', 'hidden');
    $("#tbMORDLSBProposalList").jqGrid('setGridState', 'hidden');
    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


    $('#idFilterDiv').trigger('click');


}



// Delete Building Proposal
function BuildingDelete(id) {

    if (confirm("Are you sure to Delete Road Proposal Details ? ")) {
        blockPage();
        $.post("/BuildingProposal/BuildingProposalDelete/", { id: id }, function (data) {
            unblockPage();
            Alert(data.errorMessage);
            LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());

        });

    }
}

//Edit Other Details for LSB
function BuildingUpdate(id) {




    $("#accordion div").html("");
    $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Building Proposal Update</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img  class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                    );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load("/BuildingProposal/BuildingEdit/" + id, function () {
            $.validator.unobtrusive.parse($('#divProposalForm'));
            unblockPage();
        });

        $("#divProposalForm").css('height', 'auto');
        $('#divProposalForm').show('slow');
    });

    $("#tbBuildingProposalList").jqGrid('setGridState', 'hidden');


}



//------------ Building Proposal List ---------------------------------
function BuildingProposalGrid(syear, block, batch, stream, proptype, propstatus, propconnect) {
    /*$.post('/BuildingProposal/GetProposals?' + Math.random(), { "syear": syear, "block": block, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect }, function (data) {
        alert("Loaded: "+ JSON.stringify(data));

    });
    */
    // alert(syear + " : " + block + " : " + batch + " : " + stream + " : " + proptype + " : " + propstatus + " : " + propconnect);

    jQuery("#tbBuildingProposalList").jqGrid({
        url: '/BuildingProposal/GetProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["Block", "Package", "Year", "Work Name", "Total Cost", "Batch", "MoRD (Clearance Date)", "View", "PIU Finalized", "Edit", "Delete"],//"Upload",
        colModel: [

                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 80, sortable: false, align: "left" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 70, sortable: false, align: "left" },
                            { name: 'IMS_YEAR_FINANCIAL', index: 'IMS_YEAR_FINANCIAL', width: 70, sortable: false, align: "left" },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 150, sortable: false, align: "left" },
                            { name: 'IMS_PAV_EST_COST', index: 'IMS_PAV_EST_COST', width: 70, sortable: false, align: "right" },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 80, sortable: false, align: "right" },
                            { name: 'MoRD', index: 'MoRD', width: 100, sortable: false, align: "center" },
                            { name: 'Display', index: 'Display', width: 40, sortable: false, align: "center" },
                            { name: 'Finalize', index: 'Finalize', width: 70, sortable: false, align: "center" },
                        //    { name: 'Upload', index: 'Upload', width: 40, sortable: false, align: "center"},
                            { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
                            { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }

        ],
        postData: { "syear": syear, "block": block, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect },
        pager: jQuery('#dvBuildingProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Building Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth:true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbBuildingProposalList #dvBuildingProposalListPager").css({ height: '31px' });

            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbBuildingProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

}



function PIUFinalizeBuildingDetails(id) {
    blockPage();
    $.post("/BuildingProposal/PIUFinalizedBuilding/", { id: id }, function (data) {
        if (data.Success) {
            LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val(), $("#ddlProposalStatus option:selected").val(), $("#ddlImsConnectivity option:selected").val());
            alert("Building Proposal has been finalized");

        }
        else {
            alert("Processing Error Occur!");
        }
        unblockPage();
    });
}

//-----------MoRD Login

function MoRDBuildingProposalGrid(state, district, syear, batch, stream, proptype, propstatus, propconnect, eacode) {
    // alert("state : " + state + " district : " + district + " syear : " + syear + " batch : " + batch + " stream : " + stream + " proptype : " + proptype + " propstatus : " + propstatus + " propconnect : " + propconnect + " eacode : " + eacode);
    jQuery("#tbMoRDBuildingProposalList").jqGrid({
        url: '/BuildingProposal/GetMoRDProposals?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["District", "Block", "Package", "Year", "Work Name", "Total Cost", "Batch", "MoRD (Clearance Date)", "View"],
        colModel: [
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', width: 80, sortable: false, align: "left" },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', width: 80, sortable: false, align: "left" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', width: 70, sortable: false, align: "left" },
                            { name: 'IMS_YEAR_FINANCIAL', index: 'IMS_YEAR_FINANCIAL', width: 70, sortable: false, align: "left" },
                            { name: 'IMS_ROAD_NAME', index: 'IMS_ROAD_NAME', width: 150, sortable: false, align: "left" },
                            { name: 'IMS_PAV_EST_COST', index: 'IMS_PAV_EST_COST', width: 70, sortable: false, align: "right" },
                            { name: 'IMS_BATCH', index: 'IMS_BATCH', width: 80, sortable: false, align: "right" },
                            { name: 'MoRD', index: 'MoRD', width: 100, sortable: false, align: "center" },
                            { name: 'Display', index: 'Display', width: 40, sortable: false, align: "center" }

        ],
        postData: { "state": state, "district": district, "syear": syear, "batch": batch, "stream": stream, "proptype": proptype, "propstatus": propstatus, "propconnect": propconnect, "eacode": eacode },
        pager: jQuery('#dvMoRDBuildingProposalListPager'),
        rowList: [15, 30, 45],
        rowNum: 15,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Building Proposals",
        height: 'auto',
        width: 'auto',
        //autowidth:true,
        sortname: 'Block',
        rownumbers: true,
        footerrow: true,
        loadComplete: function (data) {
            $("#tbBuildingProposalList #dvMoRDBuildingProposalListPager").css({ height: '31px' });

            unblockPage();
        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                //window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                alert('Error occurred');
            }
        },
        beforeSelectRow: function (rowid, e) {

            var $link = $('a', e.target);

            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {

                $('#tbBuildingProposalList').jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }); //end of grid

}



// Delete Road Proposal
function DeleteLWE(id) {
    //alert(id);
    if (confirm("Are you sure to Delete LWE Road Proposal Details ? ")) {

        $.ajax({
            //url: '/Proposal/DeleteLWEProposal?proposalCode=' + id,
            url: '/Proposal/DeleteLWEProposal/' + id,
            type: "POST",
            cache: false,
            data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {

                if (response.success == true) {
                    alert("LWE Proposal Deleted Successfully.");
                    CloseProposalDetails();
                    $("#tbSRRDAProposalList").trigger('reloadGrid');
                    $("#tbSRRDALSBProposalList").trigger('reloadGrid');
                }
                else if (response.success == false) {
                    alert(response.message);
                }
                else {
                    if (response.message != "" || response.message != undefined || response.message != null) {
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