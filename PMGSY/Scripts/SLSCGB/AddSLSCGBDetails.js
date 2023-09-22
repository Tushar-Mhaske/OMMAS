$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmSLSCGBDetails");

    LoadMeetingList();

    $('#spCollapseIconSLSCGB').click(function () {
        $("#spCollapseIconSLSCGB").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSLSCGBMain").toggle("slow");
    });

    $('#imgCloseSLSCGBDetails').click(function () {
        $('#dvSLSCGBNote').hide('slow');
    });

    $('#rdbSLSC').click(function () {
        $('#lblMeetingFile').text('SLSC');
    });
    $('#rdbGB').click(function () {
        $('#lblMeetingFile').text('GB');
    });

    $('#btnResetSLSCGBDetails').click(function () {
        closeDivErrorSLSCGB();
    });

    $('#txtMeetingDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a meeting date',
        buttonImageOnly: true,
        buttonText: 'Meeting Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtMeetingDate').trigger('blur');
        }
    });

    $('#btnAddSLSCGBDetails').click(function (event) {
        if($("#frmSLSCGBDetails").valid())
        {
            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action
            var form_data = new FormData();

            var objSLSCGBFile = $("input#flSLSCGBFile").prop("files");
            console.log(objSLSCGBFile[0]);
            form_data.append("SLSCGBFile", objSLSCGBFile[0]);

            var data = $("#frmSLSCGBDetails").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            $.ajax({
                url: '/SLSCGB/AddSLSCGB',
                type: "POST",
                cache: false,
                data: form_data,//$("#tourDetailsForm").serialize(),
                contentType: false,
                processData: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {

                    if (response.success) {
                        //alert("SLSC/GB Details added successfully.");
                        alert(response.message);
                        LoadMeetingList();
                        $('#btnResetSLSCGBDetails').trigger('click');
                    }
                    else {
                        $("#divErrorSLSCGB").show("slow");
                        $("#divErrorSLSCGB span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }
                    unblockPage();
                }
            });//ajax call ends here
        }
    });

});

function closeDivErrorSLSCGB() {
    $("#divErrorSLSCGB span:eq(1)").html('');
    $("#divErrorSLSCGB").hide('slow');
}

// to load execution technology progress details
function LoadMeetingList() {
    
    jQuery("#tbMeetingDetailsList").jqGrid('GridUnload');
    jQuery("#tbMeetingDetailsList").jqGrid({
        url: '/SLSCGB/GetMeetingList',
        datatype: "json",
        mtype: "POST",
        //postData: { MeetingCode: meetingCode },
        colNames: ['State', 'Meeting Date', 'Meeting Type', 'File Name', 'Delete'],
        colModel: [
                    { name: 'State', index: 'State', height: 'auto', width: 50, align: "center", search: false, sortable: true },
                    { name: 'MeetingDate', index: 'MeetingDate', height: 'auto', width: 50, align: "center", search: false, sortable: true },
                    { name: 'MeetingType', index: 'MeetingType', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                    { name: 'FileName', index: 'FileName', height: 'auto', width: 50, align: "center", search: false, sortable: false },
                    { name: 'Delete', width: 40, resize: false, align: "center", sortable: false }
        ],
        pager: jQuery('#pgMeetingDetailsList').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "State",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Meeting Details List",
        height: 'auto',
        //width: '98%',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {

            //if ($('#hdnpreviousStatus').val() != 'C') {
            //    $("#tbMeetingDetailsList #pgMeetingDetailsList_left").css({ height: '40px' });
            //    $("#pgMeetingDetailsList_left").html("<input type='button' style='margin-left:27px' id='idAddTechnology' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddTechnologyProgress();return false;' value='Add Technology Progress'/>")
            //}

        },
        //editData: {
        //    ProposalCode: roadCode
        //},
        //editurl: "/Execution/EditTechnologyDetails",
        //loadError: function (xhr, ststus, error) {

        //    if (xhr.responseText == "session expired") {
        //        alert(xhr.responseText);
        //        window.location.href = "/Login/Login";
        //    }
        //    else {
        //        alert("Invalid data.Please check and Try again!")
        //        //  window.location.href = "/Login/LogIn";
        //    }
        //}
    });
}

//function AnchorFormatter(cellvalue, options, rowObject) {
//    var url = "/SLSCGB/DownloadMeetingFile/" + cellvalue;
//    return cellvalue == "-" ? "-" : "<a href='#' onclick=downloadMeetingFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
//}

function downloadMeetingFileFromAction(paramurl) {
    //window.location = paramurl;

    $.get(paramurl).done(function (response) {
        if (response.Success == 'false') {
            alert('File Not Found.');
            return false;

        }
        else if (response.Success === undefined) {
            window.location = paramurl;
        }
    });
}

function DeleteMeetingDetails(param) {
    if (confirm("Are you sure to delete the meeting details?")) {
        $.ajax({
            url: '/SLSCGB/DeleteMeetingDetails/' + param,
            type: 'POST',
            //data: { MeetingCode: meetingCode },
            success: function (response) {
                if (response.Success) {
                    alert("Meeting Details deleted successfully");
                    LoadMeetingList();
                }
                else {
                    $("#divErrorSLSCGB").show("slow");
                    $("#divErrorSLSCGB span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
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
