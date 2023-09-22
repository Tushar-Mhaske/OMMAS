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

    $('#btnSave').click(function () {
       SaveDetails();
    });

    $('#btnCancel').click(function () {

         $('#accordion').hide();  //close the form
         $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
    });

    // Saving Master Only
    $('#btnSave1').click(function () {
      //  alert("Clicked master button")
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
                url: '/Execution/AddRSAdetails',
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
                       LoadInspDetailsList(RsaCode);
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
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
                    alert("Error Occured.");
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
                       LoadInspDetailsList(RsaCode);
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
                    // alert(xhr.responseText);
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
    alert("l")
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
        caption: "&nbsp;&nbsp; ATR By PIU",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
    

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured.");
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


//AddDetailsByPIU


function AddDetailsByPIU(urlparameter)
{
    $("#divHabitationDetails").show();
}


function AddDetailsByPIU(urlparameter) {
 

    blockPage();
    $("#divAddExecution").unload();
    $("#divAddExecution").load('/Execution/AddATRByPIU/' + urlparameter, function (data)
    {
        unblockPage();
        if (data.success == false)
        {
            alert(data.message);
        }
    });
    $('#divAddExecution').show('slow');
    $('#tabs-1').show('slow');
    $('#divCdWorks').show('slow');
    $('#tbCDWorksList').show('slow');
    
    $("#divAddExecution").css('height', 'auto');

   
}





function doNothing() {
    return false;
}

function imageFormatter(cellvalue, options, rowObject)
{
    var PictureURL = cellvalue.replace('/thumbnails', '');

    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px;' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}
function ValidateImageDescription(value, colname)
{
    if (!value.match("^[a-zA-Z0-9 ]+$")) {
        return [" Invalid Image Description Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}
function AnchorFormatter(cellvalue, options, rowObject)
{

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
