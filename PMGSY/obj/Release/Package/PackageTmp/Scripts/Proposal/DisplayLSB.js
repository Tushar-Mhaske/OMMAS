this.imagePreview = function () {

    /* CONFIG */
    xOffset = 10;
    yOffset = 10;
    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    /* END CONFIG */
    var callback = function (event, param) {
        var $img = $("#preview");

        // top-right corner coords' offset
        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);

        //alert("left: " + (trc_y - $img.height()) + "   Top " + (trc_x - $img.width()));

        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 100; //600;

        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + this.href + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
    //.mousemove(callback);
};

$(document).ready(function () {
    $("#tabs").tabs();
    $("#trTEchnicalDetails").removeClass("ui-state-hover");

    //ShowComponentType($("#IMS_PR_ROAD_CODE").val());
    //ListProposalFiles($("#IMS_PR_ROAD_CODE").val());
    //ListPDFFiles($("#IMS_PR_ROAD_CODE").val());

    $("#btnFinalize").click(function () {

        if ($("#isAllDetailsEntered").val() != "True")
        {
            alert("Some of the details against this proposal are missing.\nPlease first fill all the details, then only proposal can be finalized.");
            return;
        }

        if (confirm("Once Proposal is Finalized, it can not be Edited and Deleted.\nAre you sure to Finalize it ?")) {
            $.ajax({
                url: '/Proposal/DPIUFinalizeProposal',
                type: "POST",
                cache: false,
                beforeSend: function () {
                    blockPage();
                },
                data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    if (response.Success) {
                        alert('Proposal Finalized Successfully.');
                        $('#tbLSBProposalList').jqGrid('GridUnload');
                        LoadProposals($("#ddlImsYear").val(), $("#ddlMastBlockCode").val(), $("#ddlImsBatch").val(), $("#ddlImsStreams").val(), $("#ddlImsProposalTypes").val());
                        CloseProposalDetails();
                        $("#btnFinalize").hide();
                    }
                    else {
                        alert(response.ErrorMessage);
                    }
                }
            });

        } else {
            return false;
        }
    });

    $("#btnLockFinalize").click(function () {

        if ($("#isAllDetailsEntered").val() != "True") {
            alert("Some of the details against this proposal are missing.\nPlease first fill all the details, then only proposal can be finalized.");
            return;
        }

        if (confirm("Once Proposal is Finalized it will be Locked, hense it can not be Edited and Deleted.\nAre you sure to Lock and Finalize it ?")) {
            $.ajax({
                url: '/Proposal/FinalizeUnlockedProposal',
                type: "POST",
                cache: false,
                beforeSend: function () {
                    blockPage();
                },
                data: { IMS_PR_ROAD_CODE: $("#IMS_PR_ROAD_CODE").val() },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    alert('Proposal Locked and Finalized Sucessfully.');
                    CloseProposalDetails();                    
                }
            });
        } else {
            return false;
        }
    });

    ListLSBSTASRRDAPDFFiles($("#IMS_PR_ROAD_CODE").val());
});


function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}

function ListProposalFiles() {
    $('#tbFilesList').jqGrid('GridUnload');
    jQuery("#tbFilesList").jqGrid({
        url: '/Proposal/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Image", "Chainage", "Description", "Download"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Chainage', index: 'Chainage', width: 80, sortable: false, align: "center" },
                    { name: 'Description', index: 'Description', width: 300, sortable: false, align: "center" },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false }
        ],
        postData: { "IMS_PR_ROAD_CODE": $("#IMS_PR_ROAD_CODE").val(), "value" : Math.random() },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Proposal Images",
        height: 'auto',
        //autowidth: true,
        sortname: 'image',
        rownumbers: true,
        loadComplete: function () {
            imagePreview();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid    
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/Proposal/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function ListPDFFiles(IMS_PR_ROAD_CODE) {
    $('#tbPDFFilesList').jqGrid('GridUnload');
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/Proposal/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Edit", "Delete", "Save"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 470, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 } },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": $("#IMS_PR_ROAD_CODE").val() },
        pager: jQuery('#dvPDFFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "C Proforma",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                unblockPage();
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
            unblockPage();
        }
    }); //end of grid    
}

function ListLSBSTASRRDAPDFFiles(IMS_PR_ROAD_CODE) {
    $('#tbLSBSTASRRDAPDFFilesList').jqGrid('GridUnload');
    blockPage();
    jQuery("#tbLSBSTASRRDAPDFFilesList").jqGrid({
        url: '/Proposal/ListSTASRRDAPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Edit", "Delete", "Save"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 470, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 } },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "IMS_PR_ROAD_CODE": $("#IMS_PR_ROAD_CODE").val() },
        pager: jQuery('#dvLSBSTASRRDAPDFFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Performance Report",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                unblockPage();
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
            unblockPage();
        }
    }); //end of grid    
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    var url = "/Proposal/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

//Display Image Files & PDFs
 function ListFiles(){
     ListProposalFiles();
     ListPDFFiles();
     unblockPage();
 }


 function ListOthDetails() {
     $("#dvLSBOthDetails").load('/LSBProposal/LSBDisplayOthDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
         //$.validator.unobtrusive.parse($('#frmSTALSBScrutiny'));
         unblockPage();
     });
     $('#dvLSBOthDetails').show('slow');
 }


function LoadStaSanctionForm() {
    $("#divStaLSBSanctionDetails").load('/LSBProposal/GetStaLSBScrutiny?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmSTALSBScrutiny'));
        unblockPage();
    });
    $('#divStaLSBSanctionDetails').show('slow');
}

function LoadPtaSanctionForm() {
    $("#divPtaLSBSanctionDetails").load('/LSBProposal/GetPtaLSBScrutiny?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmPTALSBScrutiny'));
        unblockPage();
    });
    $('#divPtaLSBSanctionDetails').show('slow');
}

function LoadMordSanctionForm() {
    $("#divMordLSBSanctionDetails").load('/LSBProposal/GetMordLSBSanctionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordLSBSanction'));
        unblockPage();
    });
    $('#divMordLSBSanctionDetails').show('slow');
}


function LoadRevisedCostLengthForm() {
    blockPage();
    $("#divRevisedRoadlength").load('/Proposal/RevisedCostLength', function () {
        $.validator.unobtrusive.parse($('#frmAddRevisedCostLength'));
        unblockPage();
    });
    $('#divRevisedRoadlength').show('slow');
    unblockPage();
}

function doNothing() {
    return false;
}

function ShowComponentType() {
    $('#tbComponentDetails').jqGrid('GridUnload');
    jQuery("#tbComponentDetails").jqGrid({
        url: '/LSBProposal/LSBComponentList',
        datatype: "json",
        mtype: "POST",
        postData: { roadId: $("#IMS_PR_ROAD_CODE").val(), value: Math.random() },
        colNames: ["", "Component Description", "Quantity", "Cost (In lakhs)", "Grade Concrete (In lakhs)", "Edit", "Delete"],
        colModel: [
                    { name: 'ComponentCode', index: 'ComponentCode', width: 10, sortable: false, align: "center", hidden: true },
                    { name: 'ComponentDesc', index: 'ComponentDesc', width: 240, sortable: false, align: "left" },
                    { name: 'Quantity', index: 'Quantity', width: 150, sortable: false, align: "center" },
                    { name: 'Cost', index: 'Cost', width: 150, sortable: false, align: "center" },
                    { name: 'GradeConcrete', index: 'GradeConcrete', width: 180, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Action', width: 60, sortable: false, align: "center", hidden:true },
                    { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center", hidden: true }
        ],
        pager: jQuery('#dvComponentDetailsPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;LSB Component Details",
        height: 'auto',
        //width: 'auto',
        sortname: 'ComponentCode',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            //Hide Title bar
            $("#gview_tbComponentDetails > .ui-jqgrid-titlebar").hide();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                //alert(xhr.responseText);
                window.location.href = "/Login/SessionExpire";
            }
            else {
                //alert("Session Timeout !!!");
                window.location.href = "/Login/SessionExpire";
            }
        }

    }); //end of grid
    
}

// ---- Details of Proposal in Agreement , Execution and Maintenance

function LoadAgreementDetails() {
    blockPage();
    $("#divAgreementDetails").load('/Agreement/ViewProposalAgreementDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divAgreementDetails').show('slow');
    unblockPage();
}

function LoadExecutionDetails() {
    blockPage();


    $("#divExecutionDetails").load('/Execution/ViewExecutionDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
}

function LoadMaintenanceDetails() {
    blockPage();
    $("#divMaintenanceDetails").load('/MaintenanceAgreement/ViewProposalMaintenanceDetails?id=' + $("#IMS_PR_ROAD_CODE").val(), function () {
        $.validator.unobtrusive.parse($('#frmMordSanction'));
        unblockPage();
    });
    $('#divMaintenanceDetails').show('slow');
    unblockPage();
}



