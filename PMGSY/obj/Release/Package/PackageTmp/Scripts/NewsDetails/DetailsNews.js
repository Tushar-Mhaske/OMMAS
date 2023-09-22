$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmDetailsNews');
    loadNewsDetails();

    $("#accordion").accordion({
        icons: false,
        heightStyle: "content",
        autoHeight: false
    });

    $("#btnSubmitNews").click(function () {
        if ($("#frmDetailsNews").valid()) {
            loadNewsDetails();
        }
    });

    $("#btnAddNews").click(function () {
        loadCreateNews("0");
    });
    if ($("#hdnRole").val() == 25) {
        $("#rdbtnSRRDA").click(function () {
            fillStateDDL();
        });
    }
    $("#rdbtnNRRDA").click(function () {
        fillNRRDADDL();
    });

    $("#ddlState").change(function () {
        fillSRRDADDL();
    });

    //if ($("#hdnRole").val() == 25)
    //{
    //    $("#rdbtnNRRDA").show();
    //    $("#rdbtnDPIU").hide();
    //}
    if ($("#hdnRole").val() == 2) {
        $("#ddlState").hide('slow');
        //$("#ddlSRRDA").show();

        $("#rdbtnDPIU").click(function () {
            fillDPIUDDL();
        });

        $("#rdbtnSRRDA").click(function () {
            fillSRRDADrpodown();
        });
    }

    $("#ddlApproved").change(function () {
        fillDropdownStatus();
    });

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvViewNewsMain").toggle("slow");

    });

});


function loadNewsDetails() {
    //$("#tbATRJqGrid").jqgrid('GridUnload');
    //alert("5");
    $('#accordion').hide('slow');
    $("#btnAddNews").show('slow');
    $("#tbNewsDetailsJqGrid").jqGrid('GridUnload');
    $("#tbNewsDetailsJqGrid").jqGrid({
        url: '/NewsDetails/NewsDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { FOR_MONTH: $("#ddlMonths").val(), FOR_YEAR: $("#ddlYears").val(), APPR: $("#ddlApproved").val(), STATUS: $("#ddlStatus").val(), NRRDA: $("#ddlNRRDA").val(), State: $("#ddlState").val(), SRRDA: $("#ddlSRRDA").val(), DPIU: $("#ddlDPIU").val() },
        colNames: ["Upload Date", "Title", "Publish Start Date", "Publish End Date", "Status", "Edit", "Delete", "Upload", "State", "User", "Publish/Archive", "Finalize", "Approval Status", "View"],
        colModel: [
            { name: 'News Upload Date', index: 'NewsUploadDate', width: 70, sortable: true, align: "center", search: false },
            { name: 'Title', index: 'Name', width: 70, sortable: true, align: "center", search: false },
            { name: 'Publish Start Date', index: 'PublishStartDate', width: 90, sortable: true, align: "center", search: false },
            { name: 'Publish End Date', index: 'PublishEndDate', width: 85, sortable: true, align: "center", search: false },
            { name: 'Status', index: 'Status', width: 100, sortable: true, align: "center", search: false },
            { name: 'Update', index: 'Update', width: "70", sortable: false, align: "center" },
            { name: 'Delete', index: 'Delete', width: "70", sortable: false, align: "center" },
            { name: 'Upload', index: 'Upload', width: "70", sortable: false, align: "center" },
            { name: 'State', index: 'State', width: "70", sortable: false, align: "center" },
            { name: 'User', index: 'User', width: "70", sortable: false, align: "center" },
            { name: 'Publish/Archive', index: 'Publish/Archive', width: "100", sortable: false, align: "center" },
            { name: 'Finalize', index: 'Finalize', width: "70", sortable: false, align: "center" },
            { name: 'ApprDetails', index: 'ApprDetails', width: 85, sortable: true, align: "center" },
            { name: 'ShowDetails', index: 'ShowDetails', width: 40, sortable: false, align: "center" }
        ],
        pager: jQuery("#divNewsDetailsReportPager"),
        rownum: 100,
        viewrecords: true,
        recordtext: '{2} Records Found',
        caption: "&nbsp &nbsp; News Details",
        height: "auto",
        autowidth: true,
        sortname: 'SrNo.',
        sortorder: 'asc',
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ['State', 'User'],
            groupColumnShow: [false, false],
            groupSummary: [false],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupCollapse: false,
            groupOrder: ['asc', 'asc'],
            //showSummaryOnHide: true
        },
        //footerrow: true,
        //userDataOnFooter: true,

        loadComplete: function () {
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                alert('Error occurred');
            }
            else {
                alert('Error occurred');
            }
        }
    });//end of grid
}

function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseNewsDetails() {
    $('#accordion').hide('slow');
    $("#tbNewsDetailsJqGrid").jqGrid('setGridState', 'visible');
    $("#tbNewsDetailsJqGrid").trigger('reloadGrid');
    $("#dvNewsDetails").hide("slow");
    $("#btnAddNews").show('slow');
}

//function ShowDetails(id) {

//    //$("#dvTabs").show();
//    $("#tbFBDetailsForm").html('');

//    $("#accordion h3").html(
//            "<a href='#' style= 'font-size:.9em;' >Feedback Details</a>" +
//            '<a href="#" style="float: right;">' +
//            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseNewsDetails();" /></a>'
//            );

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#tbNewsDetailsForm").load('/FeedbackDetails/ViewFBDetails?fId=' + id, function () {
//            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));

//            unblockPage();
//        });
//        $('#tbNewsDetailsForm').show('slow');
//        $("#tbNewsDetailsForm").css('height', 'auto');
//    });

//    $("#tbFBDetailsJqGrid").jqGrid('setGridState', 'hidden');
//    $('#tbFBDetailsForm').jqGrid('setGridState', 'hidden');
//}

function DisplayNewsDetails(id) {
    $("#btnAddNews").hide();
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >View News Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseNewsDetails();" /></a>'
            );

    //$("#accordion").accordion({
    //    icons: false,
    //    heightStyle: "content",
    //    autoHeight: false
    //});

    $('#accordion').show('fold', function () {
        //  blockPage();

        $("#dvNewsDetails").load('/NewsDetails/DisplayNewsFiles/' + id, function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));

            //   unblockPage();
        });
        $("#tbNewsDetailsJqGrid").jqGrid('setGridState', 'hidden');
        $('#dvNewsDetails').show('slow');
        $("#dvNewsDetails").css('height', 'auto');
    });
};

function UploadNews(id) {
    $("#btnAddNews").hide();
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Files</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseNewsDetails();" /></a>'
            );

    //$("#accordion").accordion({
    //    icons: false,
    //    heightStyle: "content",
    //    autoHeight: false
    //});

    $('#accordion').show('fold', function () {
        blockPage();

        $("#dvNewsDetails").load('/NewsDetails/NewsUpload/' + id, function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));

            unblockPage();
        });
        $("#tbNewsDetailsJqGrid").jqGrid('setGridState', 'hidden');
        $('#dvNewsDetails').show('slow');
        $("#dvNewsDetails").css('height', 'auto');
    });
};

function loadCreateNews(id) {
    $("#btnAddNews").hide();
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Create News</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseNewsDetails();" /></a>'
            );

    //$("#accordion").accordion({
    //    icons: false,
    //    heightStyle: "content",
    //    autoHeight: false
    //});

    $('#accordion').show('fold', function () {
        blockPage();

        $("#dvNewsDetails").load('/NewsDetails/CreateNews/' + id, function () {
            //  $.validator.unobtrusive.parse($('#divFBDetailsForm'));

            unblockPage();
        });
        $("#tbNewsDetailsJqGrid").jqGrid('setGridState', 'hidden');
        $('#dvNewsDetails').show('slow');
        $("#dvNewsDetails").css('height', 'auto');
    });
}

function DeleteNews(id) {
    if (confirm("Are you sure you want to Delete News and News Files?")) {
        $.ajax({
            url: "/NewsDetails/NewsDeletion/" + id,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {
                if (data.status == "1") {
                    alert("News Deleted Successfully!");

                    //$("#dvNewsDetails").load('/NewsDetails/CreateNews', function () {
                    //    $.validator.unobtrusive.parse($('#dvNewsDetails'));

                    //    //    unblockPage();
                    //    //});
                    //    $('#dvNewsDetails').show('slow');
                    //    $("#dvNewsDetails").css('height', 'auto');
                    //});
                    loadNewsDetails();
                    $('#accordion').hide();
                }
                    //else {
                    //    alert("Error occured while deleting News.");
                    //}
                else if (data.status == "-1") {
                    alert("Error occured while Deleting News.");
                }
                else if (data.status == "0") {
                    alert("Could not Delete News.");
                }
                else if (data.status == "-2") {
                    alert("News is not Approved");
                }
                else if (data.status == "-3") {
                    alert("Please Delete Files related to this News and then Delete the News");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function FinalizeNews(id) {
    if (confirm("Are you sure you want to Finalize News?")) {
        $.ajax({
            url: "/NewsDetails/NewsFinalization/" + id,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {

                //alert(data.status);
                if (data.status > 0) {
                    alert("News Finalized Successfully");

                    loadNewsDetails();
                    $('#accordion').hide();
                    $("#tbNewsDetailsJqGrid").trigger('reloadGrid');
                }
                else {
                    alert("Error occured while Finalizing News.");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function UnFinalizeNews(id) {
    if (confirm("Are you sure you want to Unfinalize News?")) {
        $.ajax({
            url: "/NewsDetails/NewsUnfinalization/" + id,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {

                //alert(data.status);
                if (data.status > 0) {
                    alert("News Unfinalized Successfully");

                    loadNewsDetails();
                    $('#accordion').hide();
                    $("#tbNewsDetailsJqGrid").trigger('reloadGrid');
                }
                else {
                    alert("Error occured while Unfinalizing News.");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function PublishArchNews(id) {
    if (confirm("Are you sure you want to Publihsh/Archive News?")) {
        $.ajax({
            url: "/NewsDetails/NewspublishArchive/" + id,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {

                //alert(data.status);
                if (data.status > 0) {
                    //alert("News Publish/Archive Successful!");
                    if (data.status == "1") {
                        alert("News Published Successfully.");
                    }
                    else if (data.status == "2") {
                        alert("News Archived Successfully.");
                    }
                    //$("#dvNewsDetails").load('/NewsDetails/CreateNews', function () {
                    //    $.validator.unobtrusive.parse($('#dvNewsDetails'));

                    //    //    unblockPage();
                    //    //});
                    //    $('#dvNewsDetails').show('slow');
                    //    $("#dvNewsDetails").css('height', 'auto');
                    //});
                    loadNewsDetails();
                    $('#accordion').hide();
                    $("#tbNewsDetailsJqGrid").trigger('reloadGrid');
                }
                    //else {
                    //    alert("Error occured on Publihsh/Archive News.");
                    //}
                else if (data.status == "-1") {
                    alert("Error occured while Publihshing News.");
                }
                else if (data.status == "-2") {
                    alert("Error occured while News Archival.");
                }
                else if (data.status == "0") {
                    alert("Could not Publish/Archive News.");
                }
                else if (data.status == "-3") {
                    alert("News is not Approved");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function ApproveNews(id) {
    if (confirm("Are you sure you want to Approve News?")) {
        $.ajax({
            url: "/NewsDetails/NewsApproval/" + id,
            cache: false,
            type: "POST",
            async: false,
            //data: $("#frmCreateNews").serialize(),
            success: function (data) {


                if (data.status == true) {
                    alert("News Approval Successful!");

                    //$("#dvNewsDetails").load('/NewsDetails/CreateNews', function () {
                    //    $.validator.unobtrusive.parse($('#dvNewsDetails'));

                    //    //    unblockPage();
                    //    //});
                    //    $('#dvNewsDetails').show('slow');
                    //    $("#dvNewsDetails").css('height', 'auto');
                    //});
                    loadNewsDetails();
                    $('#accordion').hide();
                }
                    //else {
                    //    alert("Error occured on Publihsh/Archive News.");
                    //}
                else if (data.status == "-1") {
                    alert("Error occured on News Approval.");
                }
                else if (data.status == "0") {
                    alert("Could not Approve News.");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
}

function fillStateDDL() {
    $("#ddlNRRDA").hide('slow');
    $("#ddlState").show('slow');
    $("#ddlSRRDA").show('slow');

    $.ajax({
        url: "/NewsDetails/fillSRRDAstate?Code=" + "0",
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlState").empty();
            $.each(data, function () {
                $("#ddlState").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });

            $.ajax({
                url: "/NewsDetails/fillDDLSRRDA?Code=" + "0",
                cache: false,
                type: "POST",
                async: false,
                //data: $("#frmCreateNews").serialize(),
                success: function (data) {
                    $("#ddlSRRDA").empty();
                    $.each(data, function () {
                        $("#ddlSRRDA").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    });
                },
                error: function () {
                    alert("error");
                }
            })

        },
        error: function () {
            alert("error");
        }
    })
}

function fillSRRDADrpodown() {
    $("#ddlDPIU").hide('slow');

    $.ajax({
        url: "/NewsDetails/fillDDLSRRDA?Code=" + "0",
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlSRRDA").empty();
            $.each(data, function () {
                $("#ddlSRRDA").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });

            $.ajax({
                url: "/NewsDetails/fillDDLDPIU?Code=" + "-1",
                cache: false,
                type: "POST",
                async: false,
                //data: $("#frmCreateNews").serialize(),
                success: function (data) {
                    $("#ddlDPIU").empty();
                    $.each(data, function () {
                        $("#ddlDPIU").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    });
                },
                error: function () {
                    alert("error");
                }
            })
        },
        error: function () {
            alert("error");
        }
    })
}

function fillSRRDADDL() {
    //$("#ddlNRRDA").hide();
    //$("#ddlState").show();
    //$("#ddlSRRDA").show();
    $.ajax({
        url: "/NewsDetails/fillDDLSRRDA?Code=" + $("#ddlState").val(),
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlSRRDA").empty();
            $.each(data, function () {
                $("#ddlSRRDA").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        },
        error: function () {
            alert("error");
        }
    })
}

function fillDPIUDDL() {
    //$("#ddlNRRDA").hide();
    //$("#ddlState").show();
    $("#ddlDPIU").show('slow');
    $.ajax({
        url: "/NewsDetails/fillDDLDPIU?Code=" + $("#ddlState").val(),
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlDPIU").empty();
            $.each(data, function () {
                $("#ddlDPIU").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        },
        error: function () {
            alert("error");
        }
    })
}


function fillNRRDADDL() {
    //$("#ddlNRRDA").show();
    $("#ddlState").hide('slow');
    $("#ddlSRRDA").hide('slow');
    $.ajax({
        url: "/NewsDetails/fillSRRDAstate?Code=" + "-1",
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlState").empty();
            $.each(data, function () {
                $("#ddlState").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });

            $.ajax({
                url: "/NewsDetails/fillDDLSRRDA?Code=" + "-1",
                cache: false,
                type: "POST",
                async: false,
                //data: $("#frmCreateNews").serialize(),
                success: function (data) {
                    $("#ddlSRRDA").empty();
                    $.each(data, function () {
                        $("#ddlSRRDA").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    });
                },
                error: function () {
                    alert("error");
                }
            })

        },
        error: function () {
            alert("error");
        }
    })
}

function fillDropdownStatus() {

    $.ajax({
        url: "/NewsDetails/fillDDLStatus?approval=" + $("#ddlApproved option:selected").val(),
        cache: false,
        type: "POST",
        async: false,
        //data: $("#frmCreateNews").serialize(),
        success: function (data) {
            $("#ddlStatus").empty();
            $.each(data, function () {
                $("#ddlStatus").append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        },
        error: function () {
            alert("error");
        }
    })
}

