var isValidImage;

$(function () {
    'use strict';
 
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 20 - $("#NumberofFiles").val(),
        acceptFileTypes: /(\.|\/)(jpe?g)$/i,
        maxFileSize: 4194304
    });

    //Enable iframe cross-domain access via redirect option:
    $('#fileupload').fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );

    //Load existing files:
    $.ajax({
        url: $('#fileupload').fileupload('option', 'url'),
        dataType: 'json',
        context: $('#fileupload')[0]
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, null, { result: result });        
    });

    $("#fileupload").bind('fileuploadfail', function (e, data) {
        //alert('Error occurred while processing your request or Geo Location details are not present for this image.');
    });



    $("#fileupload").bind("fileuploaddone", function (e, data) {

        

        $("#divError").html("");
        $("#divError").hide("slow");

        $("#divSuccess").show("slow");

        $("#tbFilesList").trigger('reloadGrid');
        $(".preview").remove();
        //window.location.reload();

    });

    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        var flagRemarks = true;
        var stageStatus = true;
        $("#tblPresentation textarea[name='remark[]']").each(function () {
            var regExRemarks = /^[a-zA-Z0-9a-zA-Z0-9 ,.()-]+$/;
            var Remarks = $(this).val();
            if (Remarks=="") {
                flagRemarks = false;
                $("#divError").html("Please enter image description.");
                $("#divError").show('slow');
                $(this).focus();
                return false;
            }

            if (!regExRemarks.test(Remarks)) {
                $("#divError").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                $("#divError").show('slow');
                flagRemarks = false;
                return false;
            }

        });

        $("#tblPresentation select[name='stage[]']").each(function () {
            
            if ($(this).val() == 0) {

                stageStatus = false;
                if ($('#divError').is(':visible')) {
                    $("#divError").html("<br> Please select Stage");
                   // $("#divError").append("<br> Please select Stage");
                    return false;
                }
                else {
                    $("#divError").html("<br> Please select Stage");
                    $("#divError").show('slow');
                    return false;
                }
            }

        });

        var inputs = data.context.find(':input');
        var text = data.context.find('textarea');
        data.formData = inputs.serializeArray();
        //CheckGeoPositions(data);
        //if (isValidImage == true) {

        //}
        //else {
        //    return false;
        //}
        
        if (flagRemarks && stageStatus) {
            return true;
        }
        return false;
    });

    $('#fileupload').bind('fileuploadadd', function (e, data)
    {

        //console.log(data.files[0]);

            $.ajax({

                type: 'POST',
                url: '/Execution/ReadGeoPositions/',
                data: data.files[0],
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    if (data.success == true) {
                        //alert(data.latitude);
                        $('#lblLatitude').text(data.latitude);
                        $('#lblLongitude').text(data.longitude);
                        //alert(data.longitude);
                    }
                    else if (data.success == false) {
                        alert(data.message);
                        $('#dvErrorMessage').show('slow');
                        $('#rdoImage').trigger('click');
                    }
                    else {
                        alert(data.message);
                    }
                },
                error: function () { }

            });




        $("#divSuccess").hide("slow");
        
    });

    //$('#fileupload').on('submit', function (event) {

    //    event.stopPropagation(); // Stop stuff happening call double avoid to action
    //    event.preventDefault(); // call double avoid to action

    //    var form_data = new FormData();

    //    $.each($("input[type='file']"), function () {

    //        var id = $(this).attr('id');
    //        var objFiles = $("input#" + id).prop("files");
    //        form_data.append(id, (objFiles[0]));
    //    });

    //    var data = $("#frmRequestDocument").serializeArray();

    //    for (var i = 0; i < data.length; i++) {
    //        form_data.append(data[i].name, data[i].value);
    //    }

    //    $.ajax({

    //        type: 'POST',
    //        url: '/Execution/ReadGeoPositions',
    //        data: form_data,
    //        cache: false,
    //        processData: false,
    //        contentType: false,
    //        success: function (data) {
    //            if (data.Success == true) {
    //                alert('Documents uploaded successfully');
    //                UploadDetails($("#EncryptedRequestId").val());
    //            }
    //            else if (data.Success == false) {
    //                $('#message').html(data.ErrorMessage);
    //                $('#dvErrorMessage').show('slow');
    //            }
    //            else {
    //                alert(data.ErrorMessage);
    //            }
    //        },
    //        error: function () { }

    //    });



    //});




}); 


this.imagePreview = function () {

    xOffset = 10;
    yOffset = 10;
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
        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
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
};

$(document).ready(function () {
    ListProposalFiles($("#IMS_PR_ROAD_CODE").val());
});

function ListProposalFiles(IMS_PR_ROAD_CODE) {

    jQuery("#tbFilesList").jqGrid({
        url: '/Execution/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Image","Description","Stage",  "Download" , "Edit","Delete", "Action"],
        colModel: [
                    { name: 'image', index: 'image', width: 125, sortable: false, align: "center", formatter: imageFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 450, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateImageDescription } },
                    { name: 'Stage', index: 'Stage', width: 250, sortable: false, align: "center", search: false, editable: false },
                    { name: 'download', index: 'download', width: 80, sortable: false, align: 'center', editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Action', index: 'Save', width: 40, sortable: false, align: "center", editable: false,hidden:true }
        ],
        postData: { "IMS_PR_ROAD_CODE": IMS_PR_ROAD_CODE },
        pager: jQuery('#dvFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Execution Images",
        height: 'auto',
        sortname: 'image',
        //autowidth: true,
        cmTemplate:false,
        rownumbers: true,
        loadComplete: function () {
            imagePreview();           
        },
        editurl: "/Execution/UpdateImageDetails",
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

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownLoadImage(cellvalue) {
    var url = "/Execution/DownloadFile/" + cellvalue;
    downloadFileFromAction(url);
}

function doNothing(){
    return false;
}

function imageFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.replace('/thumbnails', '');
    
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;' class='preview'><img style='height: 75px; width: 100px; border:solid 1px black' src='" + cellvalue + "' alt='Image not Available' title=''  /> </a>";
}

function CheckGeoPositions(data)
{

    //var form_data = new FormData();

    //$.each($("input[type='file']"), function () {

    //    var id = $(this).attr('id');
    //    alert(id);
    //    var objFiles = $("input#" + id).prop("files");
    //    form_data.append(id, (objFiles[0]));
    //    console.log(objFiles);
    //});

    //var inputData = $("#fileupload").serializeArray();
    //console.log(inputData);
    //for (var i = 0; i < inputData.length; i++) {
    //    form_data.append(inputData[i].name, inputData[i].value);
    //}
    
    //console.log(form_data);
    //console.log($('#fileupload')[0].files);
    //console.log($("#fileupload").get(0).files[0]);
    var file = document.getElementById('file1');//.files[0];
    console.log(file);
    if (file) {
        // create reader
        var reader = new FileReader();
        reader.readAsText(file);
        reader.onload = function (e) {
            // browser completed reading file - display it
            alert(e.target.result);
        };
    }


    return false;

    $.ajax({

        type: 'POST',
        url: '/Execution/ReadGeoPositions/',
        data: form_data ,
        async: false,
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success == true) {
                isValidImage = true;
            }
            else {
                isValidImage = false;
            }
        },
        error: function () { }

    });
}
    
function EditImageDetails(paramFileID){   
    jQuery("#tbFilesList").editRow(paramFileID);
    $("#tbFilesList").jqGrid('showCol', 'Action');
}

function SaveFileDetails(paramFileID) {
    jQuery("#tbFilesList").saveRow(paramFileID, checksave);
    $("#tbFilesList").jqGrid('hideCol', 'Action');
}

function CancelSaveFileDetails(paramFileID) {      
    jQuery("#tbFilesList").restoreRow(paramFileID);
    $("#tbFilesList").jqGrid('hideCol', 'Action');
}

function checksave(result) {
    if (result.responseText == "true")
    {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != ""){
        alert(result.responseText.replace('"',"").replace('"',""));
        return false;
    }    
}

function ValidateImageDescription(value, colname) {
    if (!value.match("^[a-zA-Z0-9 ]+$")) {
        return [" Invalid Image Description Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function DeleteFileDetails(param, IMS_FILE_NAME){
    

    if (confirm("Are you sure to delete the Image and Image details ? ")) {

        $.ajax({
            url: "/Execution/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                IMS_PR_ROAD_CODE: param, IMS_FILE_NAME: IMS_FILE_NAME,value: Math.random()
            },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#tbFilesList").trigger('reloadGrid');
                if (response.Success) {
                    alert("Execution Image and Image Details Deleted Succesfully.");
                    //$("#divAddExecution").load('/Execution/ImageUpload/' + $("#Urlparameter").val(), function (data) {
                    //    $.validator.unobtrusive.parse($('#fileupload'));
                    //    unblockPage();
                    //    if (data.success == false) {
                    //        alert(data.message);
                    //    }
                    //});

                    $('#rdoImage').trigger('click');

                }
                else {
                    alert(response.ErrorMessage);
                }
            }
        });
    }
    else {
        return;
    }
}