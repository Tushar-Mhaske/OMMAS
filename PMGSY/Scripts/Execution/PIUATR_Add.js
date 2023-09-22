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

        //$img
		//	.css("top", (trc_y - $img.height()) + "px")
		//	.css("left", (trc_x - $img.width()) + "px");
    };


    $("a.preview").hover(function (e) {


        Mx = $(this).offset().left + 400; // * 2;//600
        My = $(this).offset().top - 50; //600;

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
var isRoad;
$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmRoadSafetyLayout1');

    // Date Inspection date
    var agrdate = $('#Agreementdate').text().split('/');;
    var Agdate = new Date(agrdate[2], (parseInt(agrdate[1]) - 1), agrdate[0]);
    //  alert(Agdate)
    var end = new Date();
    //var diff = new Date(end - Agdate);
    var start = new Date(2000, 0, 1);

    var diff = new Date(end - start);
    var days = diff / 1000 / 60 / 60 / 24;
    // alert(Math.floor(days))
    $('#txtAuditDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a Audit date',
        maxDate: "0D",
        minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Audit Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtAuditDate').trigger('blur');
        }
    });

    //




    $("#tabs").tabs();
    var ProposalCode = $("#proposalCode").val();
    CheckProposalType(ProposalCode);
    if ( isRoad == true) {
        LoadPhysicalRoadDetails(ProposalCode);
    }
    else if(isRoad == false)
    {
        LoadPhysicalLSBDetails(ProposalCode);
    }
    var Progress = $("#progressType").val();
    var RsaCode = $("#EXEC_RSA_CODE").val();

    LoadInspDetailsList(RsaCode);

   // ListProposalVideoFiles(ProposalCode);
    
  //  LoadFinancialDetails(ProposalCode,Progress);
    //ListProposalFiles(ProposalCode);
    //LoadListRemarks(ProposalCode);
    //LoadExecutingOfficerDetails(ProposalCode);
    //LoadTechnologyProgressDetails(ProposalCode);
    //LoadMappedHabitationGrid(ProposalCode);
    //LoadRoadSafetyDetailsGrid(ProposalCode);   //by Pradip patil [05/05/2017]
   // ListExecTechFiles(ProposalCode);           //by SAMMED A. PATIL [17/08/2017]

    $('#btnSave').click(function () {
       SaveDetails();
    });


    $('#btnCancel').click(function () {

         $('#accordion').hide();  //close the form
         $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
    });

    // Saving Master Only
    $('#btnSave1').click(function () {
        alert("Clicked master button")
        SaveRoadSafety1();

      
    });

    //btnSave11
    $('#btnSaveByPIU').click(function () {
        SaveDetailsByPIU();
    });

});



function SaveDetailsByPIU() {
    var RsaCode = $("#EXEC_RSA_CODE").val();

    if ($("#frmRoadSafetyLayout1").valid()) {
        if (confirm("Do you want to save Details ?")) {


            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddPIUATRDetails',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr) {
                    alert(data.message)
                    // LoadInspDetailsList(RsaCode);



                    $('#tbCDWorksList').trigger('reloadGrid');

                    document.getElementById("frmRoadSafetyLayout1").reset();

                    if (data.success) {
                        // $('#accordion').hide();  //close the form
                        // $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                        LoadInspDetailsList(RsaCode);
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured");
                    //   alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }

    else {

        return false;
    }

}


$("#ddlAccpetList").change(function ()
{

});


function SaveRoadSafety1() {
    if ($('#frmRoadSafetyLayout1').valid()) {

        if (confirm("Do you want to save Details ? RSA Stage & Inspection Date can not be modified again once the details are saved. ")) {

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSA',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr) {
                    alert(data.message)
                    if (data.success)
                    {
                     $('#accordion').hide();  //close the form
                      $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured");
                    //    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }



    else {

        return false;
    }

}




function SaveDetails() {
    var RsaCode = $("#EXEC_RSA_CODE").val();

    if ($("#frmRoadSafetyLayout1").valid()) {
    if (confirm("Do you want to save Details ?")) {

       
            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSAdetails',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout1').serialize(),
                dataType: 'json',
                success: function (data, status, xhr)
                {
                    alert(data.message)
                    // LoadInspDetailsList(RsaCode);



                    $('#tbCDWorksList').trigger('reloadGrid');

                    document.getElementById("frmRoadSafetyLayout1").reset();

                    if (data.success)
                    {
                        // $('#accordion').hide();  //close the form
                        // $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                        LoadInspDetailsList(RsaCode);
                    }
                    $.unblockUI();
                },
               error: function (xhr, status, err) {
                   alert("Error Occured");
                   //      alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }

    else {

        return false;
    }

}



function LoadInspDetailsList(RSACode) {
  //  alert("RSACode = " + RSACode)
    alert("load");
    jQuery("#tbCDWorksList").jqGrid({
        url: '/Execution/GetInspectionDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { RSACode: RSACode },
        colNames: ['Start Chainage', 'End Chainage', 'Safety Issue', 'RSA Recommendation', 'RSA Grade', 'Edit', 'Delete'],
        colModel: [

                            { name: 'EXEC_RSA_START_CHAINAGE', index: 'EXEC_RSA_START_CHAINAGE', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'EXEC_RSA_END_CHAINAGE', index: 'EXEC_RSA_END_CHAINAGE', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'EXEC_RSA_SAFETY_ISSUE', index: 'EXEC_RSA_SAFETY_ISSUE', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'EXEC_RSA_RECOMMENDATION', index: 'EXEC_RSA_RECOMMENDATION', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'EXEC_RSA_GRADE', index: 'EXEC_RSA_GRADE', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'a', width: 200, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 200, sortable: false, resize: false, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_RCD_CHAINAGE',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Inspection Details List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            //$("#gview_tbCDWorksList > .ui-jqgrid-titlebar").hide();
            //$("#tbCoreNetworkList #pagerCDWorksList").css({ height: '40px' });
            //$("#pagerCDWorksList_left").html("<input type='button' style='margin-left:27px' id='idAddCdWorks' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddCDWorks(" + IMS_PR_ROAD_CODE + ");return false;' value='Add CDWorks'/>")

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured");
                //    alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}


//EditInspectionDetails



function AddDetailsByPIU(urlparameter) {
 

    //$("#divHabitationDetails").show();

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >RSA Inspection ATR</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function ()
    {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/AddATRByPIU?urlparameter=' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#divAddExecution'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}


function LoadPhysicalRoadDetails(IMS_ROAD_CODE) {

    jQuery("#tbPhysicalRoadList").jqGrid({
        url: '/Execution/GetRoadPhysicalProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status (Completion Date)', 'Preparatory Work (Length in Km.)', 'Subgrade Stage (Length in Km.)', 'Subbase (Length in Km.)', 'Base Course (Length in Km.)', 'Surface Course (Length in Km.)', 'Road Signs Stones (in Nos.)', 'CDWorks (in Nos.)', 'LS Bridges (in Nos.)', 'Miscellaneous (Length in Km.)', 'Completed (Length in Km.)', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 50, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 70, align: "left", search: true },
                            { name: 'EXEC_PREPARATORY_WORK', index: 'EXEC_PREPARATORY_WORK', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_EARTHWORK_SUBGRADE', index: 'EXEC_EARTHWORK_SUBGRADE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SUBBASE_PREPRATION', index: 'EXEC_SUBBASE_PREPRATION', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BASE_COURSE', index: 'EXEC_BASE_COURSE', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_SURFACE_COURSE', index: 'EXEC_SURFACE_COURSE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_SIGNS_STONES', index: 'EXEC_SIGNS_STONES', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'EXEC_CD_WORKS', index: 'EXEC_CD_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_LSB_WORKS', index: 'EXEC_LSB_WORKS', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_MISCELANEOUS', index: 'EXEC_MISCELANEOUS', height: 'auto', width: 90, align: "center", search: false },
                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 90, align: "center", search: false },
                            { name: 'a', width: 40, align: "center", search: false, sortable: false, hidden: true },
                            { name: 'b', width: 40, align: "center", search: false, sortable: false, hidden: true },

        ],
        pager: jQuery('#pagerPhysicalRoadList').width(20),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Road Physical Progress List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate:{title:false},
        loadComplete: function (data) {

            //$("#gview_tbPhysicalRoadList > .ui-jqgrid-titlebar").hide();
            //$("#tbPhysicalRoadList #pagerPhysicalRoadList").css({ height: '40px' });
            //$("#pagerPhysicalRoadList_left").html("<input type='button' style='margin-left:27px' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + IMS_ROAD_CODE + ");return false;' value='Add Road Progress'/>")
            //if ($("#status").val() == "C") {
            //    $("#idAddPhysicaRoad").hide();
            //}
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured");
                // alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

function LoadPhysicalLSBDetails(IMS_ROAD_CODE) {

    jQuery("#tbLSBPhysicalRoadList").jqGrid({
        url: '/Execution/GetLSBPhysicalProgressList',
        datatype: "json",
        mtype: "POST",
        postData: { roadCode: IMS_ROAD_CODE },
        colNames: ['Month', 'Year', 'Work Status', 'Cutoff/raft/Individual footing', 'Floor Protection', 'Sinking', 'Bottom Pluggings', 'Top Pluggings', 'Well Caps', 'Pier/Abutment Shaft', 'Pier/Abutment Caps', 'Bearings', 'Deck Slab', 'Wearing Coat', 'Posts & Railing', 'Road Work', 'CD Work', 'Bridge Length Completed', 'Approach Work Completed', 'Edit', 'Delete'],
        colModel: [
                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 40, align: "left", search: false },
                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 50, align: "left", search: true },
                            { name: 'EXEC_RAFT', index: 'EXEC_RAFT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_FLOOR_PROTECTION', index: 'EXEC_FLOOR_PROTECTION', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_SINKING', index: 'EXEC_SINKING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BOTTOM_PLUGGING', index: 'EXEC_BOTTOM_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_TOP_PLUGGING', index: 'EXEC_TOP_PLUGGING', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_WELL_CAP', index: 'EXEC_WELL_CAP', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_PIER_SHAFT', index: 'EXEC_PIER_SHAFT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_PIER_CAP', index: 'EXEC_PIER_CAP', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_BEARINGS', index: 'EXEC_BEARINGS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_DECK_SLAB', index: 'EXEC_DECK_SLAB', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_WEARING_COAT', index: 'EXEC_WEARING_COAT', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'EXEC_POSTS_RAILING', index: 'EXEC_POSTS_RAILING', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_ROAD_WORK', index: 'EXEC_APP_ROAD_WORK', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_CD_WORKS', index: 'EXEC_APP_CD_WORKS', height: 'auto', width: 40, align: "center", search: false },
                            { name: 'EXEC_APP_COMPLETED', index: 'EXEC_APP_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'EXEC_BRIDGE_COMPLETED', index: 'EXEC_BRIDGE_COMPLETED', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'a', width: 30, align: "center", search: false, hidden: true },
                            { name: 'b', width: 40, align: "center", search: false, hidden: true },

        ],
        pager: jQuery('#pagerLSBPhysicalRoadList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'EXEC_PROG_YEAR,EXEC_PROG_MONTH',
        sortorder: "asc",
        caption: "&nbsp;&nbsp;LSB Physical Progress List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

        },
        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured");
                //  alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

//function LoadFinancialDetails(IMS_ROAD_CODE, PROGRESS) {

//    jQuery("#tbFinancialList").jqGrid({
//        url: '/Execution/GetFinancialProgressList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { roadCode: IMS_ROAD_CODE, progressType: PROGRESS },
//        colNames: ['Year', 'Month', 'Upto Last Month', 'During This Month', 'Total', 'Upto Last Month', 'During This Month', 'Total', 'Is Final Payment Made', 'Date', 'Edit', 'Delete'],
//        colModel: [
//                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 55, align: "left", search: false },
//                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 60, align: "center", search: false },
//                            { name: 'EXEC_VALUEOFWORK_LASTMONTH', index: 'EXEC_VALUEOFWORK_LASTMONTH', height: 'auto', width: 100, align: "left", search: true },
//                            { name: 'EXEC_VALUEOFWORK_THISMONTH', index: 'EXEC_VALUEOFWORK_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'TOTAL', index: 'TOTAL', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'EXEC_PAYMENT_LASTMONTH', index: 'EXEC_PAYMENT_LASTMONTH', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'EXEC_PAYMENT_THISMONTH', index: 'EXEC_PAYMENT_THISMONTH', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'TOTAL_PAYMENT', index: 'TOTAL_PAYMENT', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'EXEC_FINAL_PAYMENT_FLAG', index: 'EXEC_FINAL_PAYMENT_FLAG', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'EXEC_FINAL_PAYMENT_DATE', index: 'EXEC_FINAL_PAYMENT_DATE', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'a', width: 50, align: "center", search: false, hidden: true },
//                            { name: 'b', width: 50, align: "center", search: false, hidden: true },

//        ],
//        pager: jQuery('#pagerFinancialList'),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: "EXEC_PROG_YEAR,EXEC_PROG_MONTH",
//        sortorder: "desc",
//        caption: "&nbsp;&nbsp; Financial Progress List",
//        height: 'auto',
//        hidegrid: true,
//        rownumbers: true,
//        cmTemplate: { title: false },
//        loadComplete: function (data) {

//            //$("#gview_tbFinancialList > .ui-jqgrid-titlebar").hide();
//            //$("#tbFinancialList #pagerFinancialList").css({ height: '40px' });
//            //$("#pagerFinancialList_left").html("<input type='button' style='margin-left:27px' id='idAddFinancialProgress' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddFinancialProgress(" + IMS_ROAD_CODE + ");return false;' value='Add Financial Progress'/>")

//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });

//    jQuery("#tbFinancialList").jqGrid('setGroupHeaders', {
//        useColSpanStyle: false,
//        groupHeaders: [
//          { startColumnName: 'EXEC_VALUEOFWORK_LASTMONTH', numberOfColumns: 3, titleText: '<center>Value of Work Done(Rs. in Lakh)</center>' },
//          { startColumnName: 'EXEC_PAYMENT_LASTMONTH', numberOfColumns: 3, titleText: '<center>Payment Made(Rs. in Lakh)</center>' }
//        ]
//    });

//}

//function ListProposalFiles(IMS_PR_ROAD_CODE) {

//    jQuery("#tbFilesList").jqGrid({
//        url: '/Execution/ListFiles',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Image", "Description", "Stage", "Download", "Edit", "Delete", "Save"],
//        colModel: [
//                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
//                    //{ name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", editable: false },
//                    //{ name: 'Size', index: 'Size', width: 50, sortable: false, align: "center", editable: false},                    
//                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
//                    { name: 'Stage', index: 'Stage', width: 300, sortable: false, align: "center", search: false, editable: false, },
//                    //{ name: 'UploadDate', index: 'UploadDate', width: 125, sortable: false, align: "center", search: false, editable: false },
//                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
//                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
//                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
//                    { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true }
//        ],
//        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
//        pager: jQuery('#dvFilesListPager'),
//        rowNum: 4,
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Images",
//        height: 'auto',
//        //autowidth: true,
//        sortname: 'image',
//        rownumbers: true,
//        loadComplete: function () {
//            imagePreview();
//        },
//        editurl: "/Execution/UpdateImageDetails",
//        loadError: function (xhr, ststus, error) {
//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Session Timeout !!!");
//                window.location.href = "/Login/LogIn";
//            }
//        }
//    }); //end of grid    
//}

//function LoadListRemarks(IMS_PR_ROAD_CODE) {
//    jQuery("#tbListRemarks").jqGrid({
//        url: '/Execution/GetRemarksList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { proposalCode: IMS_PR_ROAD_CODE },
//        colNames: ['Remarks', 'Edit', 'Delete'],
//        colModel: [
//                            { name: 'IMS_PROG_REMARKS', index: 'IMS_PROG_REMARKS', height: 'auto', width: 500, align: "left", search: false },
//                            //{ name: 'a', width: 200, align: "center", formatter: FormatColumn, search: false, hidden: true },
//                            //{ name: 'b', width: 200, align: "center", formatter: FormatColumn1, search: false, hidden: true },
//                            { name: 'a', width: 200, align: "center",  search: false, hidden: true },
//                            { name: 'b', width: 200, align: "center",  search: false, hidden: true }

//        ],
//        pager: jQuery('#pagerRemarks'),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: "IMS_PROG_REMARKS",
//        sortorder: "desc",
//        caption: "&nbsp;&nbsp; Remarks",
//        height: 'auto',
//        hidegrid: true,
//        rownumbers: true,
//        cmTemplate: { title: false },
//        loadComplete: function (data) {

//            //if (data["records"] == 0) {
//            //    $("#divAddRemarks").show();
//            //    LoadAddView(IMS_PR_ROAD_CODE);
//            //    $("#dvListremarks").hide();
//            //}
//            //else {
//            //    $("#divAddRemarks").hide();
//            //    $("#dvListremarks").show();
//            //}
//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });
//}

//function LoadExecutingOfficerDetails(IMS_ROAD_CODE) {

//    jQuery("#tbExecutingOfficerList").jqGrid({
//        url: '/Execution/GetExecutingOfficerList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { roadCode: IMS_ROAD_CODE },
//        colNames: ['Month', 'Year', 'Designation', 'Executing Officer', 'Edit', 'Delete'],
//        colModel: [
//                            { name: 'EXEC_MONTH', index: 'EXEC_MONTH', height: 'auto', width: 200, align: "center", search: false },
//                            { name: 'EXEC_YEAR', index: 'EXEC_YEAR', height: 'auto', width: 200, align: "left", search: false },
//                            { name: 'MAST_DESIG_CODE', index: 'MAST_DESIG_CODE', height: 'auto', width: 200, align: "left", search: true },
//                            { name: 'MAST_OFFICER_CODE', index: 'MAST_OFFICER_CODE', height: 'auto', width: 350, align: "left", search: true },
//                            { name: 'edit', width: 50, align: "center", search: false, sortable: false, hidden: true },
//                            { name: 'delete', width: 50, align: "center", search: false, sortable: false, hidden: true },
//        ],
//        pager: jQuery('#pagerExecutingOfficerList').width(20),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: 'EXEC_MONTH,EXEC_YEAR',
//        sortorder: "asc",
//        caption: "&nbsp;&nbsp; Executing Officer List",
//        height: 'auto',
//        hidegrid: true,
//        rownumbers: true,
//        width: '100%',
//        cmTemplate: { title: false },
//        loadComplete: function (data) {
//            //$("#gview_tbExecutingOfficerList > .ui-jqgrid-titlebar").hide();
//            //$("#tbExecutingOfficerList #pagerExecutingOfficerList").css({ height: '30px' });
//            //$("#pagerExecutingOfficerList_left").html("<input type='button' style='margin-left:27px' id='btnAddExecutingOficer' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddExecutingOfficer(" + IMS_ROAD_CODE + ");return false;' value='Add Executing Officer'/>")
//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });

//}

//function LoadTechnologyProgressDetails(IMS_ROAD_CODE) {

//    jQuery("#tbTechnolofyProgressList").jqGrid({
//        url: '/Execution/GetTechnologyProgressList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { roadCode: IMS_ROAD_CODE },
//        colNames: ['Month', 'Year', 'Technology Length', 'Technology Name', 'Completed / In Progress', 'Completed / In Progress Length', 'Date of Completion'],
//        colModel: [
//                            { name: 'EXEC_PROG_MONTH', index: 'EXEC_PROG_MONTH', height: 'auto', width: 100, align: "center", search: false },
//                            { name: 'EXEC_PROG_YEAR', index: 'EXEC_PROG_YEAR', height: 'auto', width: 100, align: "left", search: false },
//                            { name: 'IMS_PAV_LENGTH', index: 'IMS_PAV_LENGTH', height: 'auto', width: 150, align: "left", search: true },
//                            { name: 'MAST_TECH_NAME', index: 'MAST_TECH_NAME', height: 'auto', width: 150, align: "left", search: true },
//                            { name: 'EXEC_ISCOMPLETED', index: 'EXEC_ISCOMPLETED', height: 'auto', width: 150, align: "center", search: false },
//                            { name: 'EXEC_COMPLETED', index: 'EXEC_COMPLETED', height: 'auto', width: 150, align: "center", search: false },
//                            { name: 'EXEC_PROGRESS_DATE', index: 'EXEC_PROGRESS_DATE', height: 'auto', width: 150, align: "center", search: false },
//        ],
//        pager: jQuery('#pagerTechnolofyProgressList').width(20),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: 'EXEC_PROG_MONTH,EXEC_PROG_YEAR',
//        sortorder: "asc",
//        caption: "&nbsp;&nbsp; Technology Progress Details List",
//        height: 'auto',
//        hidegrid: true,
//        rownumbers: true,
//        width: '100%',
//        cmTemplate: { title: false },
//        loadComplete: function (data) {
//            //$("#tbTechnolofyProgressList > .ui-jqgrid-titlebar").hide();
//            //$("#tbTechnolofyProgressList #pagerExecutingOfficerList").css({ height: '30px' });
//            //$("#pagerExecutingOfficerList_left").html("<input type='button' style='margin-left:27px' id='btnAddExecutingOficer' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddExecutingOfficer(" + IMS_ROAD_CODE + ");return false;' value='Add Executing Officer'/>")
//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });

//}

//function ListProposalVideoFiles(IMS_PR_ROAD_CODE) {

//    jQuery("#tbVideoFilesList").jqGrid({
//        url: '/Execution/ListVideoFiles',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Video", "Description", "Download", "Edit", "Delete", "Save"],
//        colModel: [
//                    { name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
//                    //{ name: 'Name', index: 'Name', width: 125, sortable: false, align: "center", editable: false },
//                    //{ name: 'Size', index: 'Size', width: 50, sortable: false, align: "center", editable: false},                    
//                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
//                    //{ name: 'UploadDate', index: 'UploadDate', width: 125, sortable: false, align: "center", search: false, editable: false },
//                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false,hidden:true },
//                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false,hidden:true},
//                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
//                    { name: 'Save', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true }
//        ],
//        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
//        pager: jQuery('#dvVideoFilesListPager'),
//        rowNum: 4,
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Videos",
//        height: 'auto',
//        sortname: 'Name',
//        //autowidth: true,
//        cmTemplate: false,
//        rownumbers: true,
//        loadComplete: function () {
//            imagePreview();
//        },
//        editurl: "/Execution/UpdateImageDetails",
//        loadError: function (xhr, ststus, error) {
//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Session Timeout !!!");
//                window.location.href = "/Login/LogIn";
//            }
//        }
//    }); //end of grid    
//}




function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px;' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}
function ValidateImageDescription(value, colname) {
    if (!value.match("^[a-zA-Z0-9 ]+$")) {
        return [" Invalid Image Description Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}
function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/Execution/DownloadFile/" + cellvalue;

    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); title='Click here to download video' return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/VideoIcon.jpg' /> </a>";
}
function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    var url = "/Execution/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function DownLoadExecTechImage(cellvalue) {
    var url = "/Execution/DownloadExecTechFile/" + cellvalue;
    downloadFileFromAction(url);
}

function CheckProposalType(ProposalCode)
{
    $.ajax({

        type: 'POST',
        url: '/Execution/GetProposalType/' + ProposalCode,
        async: false,
        cache: false,
        datatype: 'json',
        success: function (data)
        {
            if (data.success == true)
            {
                isRoad = true;
                return true;
            }
            else if (data.success == false) {
                isRoad = false;
                return false;
            }
            else
            {
                alert('Error occurred while processing your request.');
            }
        },
        error: function ()
        {
            alert('Error occurred while processing your request.');
        },

    });
}

//Get Mapped Habitation Listing
//function LoadMappedHabitationGrid(ProposalCode) {

//    jQuery("#tbHabitationRoadList").jqGrid('GridUnload');
//    jQuery("#tbHabitationRoadList").jqGrid({
//        url: '/Execution/GetMappedHabitationList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { prRoadCode: ProposalCode },
//        colNames: ['Name of Habitation', 'Village', 'Total Population'],
//        colModel: [
//                            { name: 'MAST_HAB_NAME', index: 'MAST_HAB_NAME', width: 100, align: "left", sortable: true },
//                            { name: 'MAST_VILLAGE_NAME', index: 'MAST_VILLAGE_NAME', width: 150, sortable: true, align: "center" },
//                            { name: 'MAST_HAB_TOT_POP', index: 'MAST_HAB_TOT_POP', width: 180, sortable: true, align: "center" },
//        ],
//        pager: jQuery('#pagerHabitationRoadList'),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: 'MAST_HAB_NAME',
//        sortorder: "asc",
//        caption: "&nbsp;&nbsp; Connected Habitation List",
//        height: '130px',
//        hidegrid: true,
//        rownumbers: true,
//        loadComplete: function (data) {

//        },
//        loadError: function (xhr, ststus, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });
//}
//Get Mapped Habitation Listing Ends

////Road Safety Listing Starts   [by Pradip Patil on 05/05/2017]
//function LoadRoadSafetyDetailsGrid(ProposalCode) {

//    jQuery("#tbRoadSafetyList").jqGrid('GridUnload');
//    jQuery("#tbRoadSafetyList").jqGrid({
//        url: '/Execution/GetRoadSafetyList',
//        datatype: "json",
//        mtype: "POST",
//        postData: { prRoadCode: ProposalCode },
//        colNames: ['Stage', 'Road Safety', 'Audit Date'],
//        colModel: [
//                            { name: 'StageView', index: 'StageView', width: 250, align: "left", sortable: false },
//                            { name: 'CondtuctBy', index: 'CondtuctBy', width: 200, sortable: false, align: "center" },
//                            { name: 'ConductedDate', index: 'ConductedDate', width: 200, sortable: false, align: "center" },
//        ],
//        pager: jQuery('#pagerHabitationRoadList'),
//        rowNum: 5,
//        rowList: [5, 10, 15],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        sortname: 'ConductedDate',
//        sortorder: "asc",
//        caption: "&nbsp;&nbsp; Road Safety List",
//        height: 'auto',
//        //autowidth: true,
//        hidegrid: true,
//        rownumbers: true,
//        loadComplete: function (data) {
//            //make the road safety grid in center

//            $("#gbox_tbRoadSafetyList").css("margin-left", "500px");
//        },
//        loadError: function (xhr, status, error) {

//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        }
//    });

//}

///*Execution Technology Image List Starts*/
//function ListExecTechFiles(IMS_PR_ROAD_CODE) {

//    jQuery("#tbExecTechImageList").jqGrid({
//        url: '/Execution/ListExecTechFiles',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ["Image", "Description", "Stage", "Remarks", "Download", "Edit", "Delete", "Action"],
//        colModel: [
//                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
//                    { name: 'Description', index: 'Description', width: 200, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
//                    { name: 'Stage', index: 'Stage', width: 300, sortable: false, align: "center", search: false, editable: false },
//                    { name: 'Remarks', index: 'Remarks', width: 200, sortable: false, align: "center", editable: false, },
//                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
//                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false, hidden: true },
//                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false, hidden: true },
//                    { name: 'Action', index: 'Save', width: 40, sortable: false, align: "center", editable: false, hidden: true }
//        ],
//        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
//        pager: jQuery('#pagerExecTechImageList'),
//        rowNum: 4,
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Execution Images",
//        height: 'auto',
//        sortname: 'image',
//        //autowidth: true,
//        cmTemplate: false,
//        rownumbers: true,
//        loadComplete: function () {
//            imagePreview();
//        },
//        editurl: "/Execution/UpdateExecTechImageDetails",
//        loadError: function (xhr, ststus, error) {
//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                alert("Session Timeout !!!");
//                window.location.href = "/Login/LogIn";
//            }
//        }
//    }); //end of grid 
//}
///*Execution Technology Image List Ends..*/