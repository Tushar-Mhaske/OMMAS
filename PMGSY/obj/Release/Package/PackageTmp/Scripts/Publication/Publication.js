$(document).ready(function () {
    //alert("Publication is Ready!");
    $('#divPublicationList').show('slow');
    
    $("#btnView").click(function () {
        //alert("change");
        //alert($("#ddPublication").val() + " : " + $("#ddFinalize").val() + " : " + $("#ddPublishe").val());
        LoadPublication($("#ddPublication").val(), $("#ddFinalize").val(), $("#ddPublishe").val());
    });
    $(function () {
        $("#accordionPublication").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $(function () {
        $("#accordionPubUpload").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
    $("#btnCreate").click(function () {
        ShowPublication(0);
    });
    
    LoadPublication("0", "0", "0");

 
});


function ClosePublication() {
    $("#divError").hide('slow');
    $("#divError span:eq(1)").html('');
    $('#accordionPublication').hide('slow');
    $('#divPublication').hide('slow');
     $('#divPublicationList').show('slow');
     LoadPublication($("#ddPublication").val(), $("#ddFinalize").val(), $("#ddPublishe").val());
    
}


function ClosePubUpload() {
    $('#accordionPubUpload').hide('slow');
    $('#divPubUpload').hide('slow');
    $('#divPublicationList').show('slow');    
    
}
function DeletePublication(pubId,action) {
    var txtMessage = "Are you sure you want to delete record?";;
    
    var flag = confirm(txtMessage);
    if (flag == true) {
        $.post("/publication/PublicationAction/", { pubid: pubId, action: action }, function (data) {
            alert("Publication details deleted successfully.");
            LoadPublication($("#ddPublication").val(), $("#ddFinalize").val(), $("#ddPublishe").val());
        });
    }
}

function FinalizePublication(pubId, finalized, published) {
    if (published == "N") {
        var action = "F"+finalized;
        var txtMessage = "Are you sure want to De-finalize record.";
        
        if (action == "FN") {
            txtMessage = "Are you sure want to finalize record.";
        }
        var r = confirm(txtMessage);
        if (r == true) {
            $.post("/publication/PublicationAction/", { pubid: pubId, action: action }, function (data) {
                if (action == "FN") {
                    alert("Publication details successfully finalize.");
                }
                else {
                    alert("Publication details successfully de-finalize.");
                }
                LoadPublication($("#ddPublication").val(), $("#ddFinalize").val(), $("#ddPublishe").val());
            });
        }
    }
    else if (published == "Y") {
        alert("Before De-finalizing first un-publish publication details.");
    }
    
   
}

function PublishedPublication(pubId, published, finalized) {
    
    if (finalized == "Y") {
        var action = "P" + published;
        var txtMessage = "Are you sure want to un-publish record?";
        if (action == "PN") {
            txtMessage = "Are you sure want to publish record?";
        }
        var r = confirm(txtMessage);
        if (r == true) {
            $.post("/publication/PublicationAction/", { pubid: pubId, action: action }, function (data) {
                if (action == "PN") {
                    alert("Publication details successfully publish.");
                }
                else {
                    alert("Publication details successfully un-publish.");
                }
                LoadPublication($("#ddPublication").val(),$("#ddPublishe").val(),$("#ddFinalize").val());
            });
        }
    }
    else if (finalized == "N") {
        alert("Before publishing first finalize publication details.");
    }

}


function ShowPublication(pubId) {
   // alert(pubId);

    $('#divPublicationList').hide('slow');
    $('#accordionPublication').show('slow');

    $("#accordionPublication div").html("");
    $("#accordionPublication h3").html(
            "<a href='#' style= 'font-size:.9em;' >Publication</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePublication();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionPublication').show('slow', function () {
        blockPage();
        if (pubId == 0) {
            $("#divPublication").load("/publication/PublicationAddEdit/", function () {
                unblockPage();

            });
        }
        else {
            //$("#divPublication").load("/publication/PublicationAddEdit/" + pubId, function () {
            //    unblockPage();

            //});

            $.ajax({
                url: '/publication/PublicationAddEdit/' + pubId,
                Type: 'POST',
                catche: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("An error occured while processing your request.");

                    return false;
                },
                success: function (response) {

                    $('#divPublication').html('');
                    $("#divPublication").html(response);
                    $("#divPublication").show('slow');

                    if ($('#Date_Type').val() == "D") {
                        $('#trDateTypeParam').show();
                        $('#tdLblDate').show();
                        $('#tdTxtDate').show();
                        $('#tdLblYear').hide();
                        $('#tdDdYear').hide();
                        $('#tdLblMonth').hide();
                        $('#tdDdMonth').hide();
                    }
                    else if ($('#Date_Type').val() == "Y") {
                        $('#trDateTypeParam').show();
                        $('#tdLblYear').show();
                        $('#tdDdYear').show();
                        $('#tdLblDate').hide();
                        $('#tdTxtDate').hide();
                        $('#tdLblMonth').hide();
                        $('#tdDdMonth').hide();
                    }
                    else if ($('#Date_Type').val() == "M") {
                        $('#trDateTypeParam').show();
                        $('#tdLblYear').show();
                        $('#tdDdYear').show();
                        $('#tdLblMonth').show();
                        $('#tdDdMonth').show();
                        $('#tdLblDate').hide();
                        $('#tdTxtDate').hide();
                    } else {

                        $('#trDateTypeParam').hide();
                        $('#tdLblDate').hide();
                        $('#tdTxtDate').hide();
                        $('#tdLblYear').hide();
                        $('#tdDdYear').hide();
                        $('#tdLblMonth').hide();
                        $('#tdDdMonth').hide();
                    }

                    
                    unblockPage();
                }
            });
        }
    });

    $('#divPublication').show('slow');
    $("#divPublication").css('height', 'auto');

}

function ShowPubUpload(pubId) {
    // alert(pubId);

    $('#divPublicationList').hide('slow');
    $('#accordionPubUpload').show('slow');

    $("#accordionPubUpload div").html("");
    $("#accordionPubUpload h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Publication</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="ClosePubUpload();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionPubUpload').show('slow', function () {
        blockPage();        
        
        $("#divPubUpload").load("/publication/publicationupload/" + pubId, function () {
                unblockPage();

            });
       
    });

    $('#divPubUpload').show('slow');
    $("#divPubUpload").css('height', 'auto');

}


function LoadPublication(publication, published, finalized) { 
  
    var headerText = "";
    if ($("#ddPublication").val() == 0) {
        headerText=  $("#spanHeader").text(" Publication");
    }
    else {
        headerText= $("#spanHeader").text($("#ddPublication option:selected").text() + " Publication");
    }
    
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
   
    $("#tblPublication").jqGrid('GridUnload');
   
    jQuery("#tblPublication").jqGrid({
        url: '/Publication/PublicationGetJQGrid',
        datatype: "json",
        mtype: "POST",
        colNames: ["PUBLICATION_CODE",'Publication', "Titile", "Author", "Publication Date", "Volume", "Publisher Name", "Pagination", "Description", "Upload", "Edit", "Delete", "Finalize / De-finalize", "Publish / Un-publish"],
        colModel: [
                        { name: 'PUBLICATION_CODE', index: 'PUBLICATION_CODE', width: 150, sortable: true, align: "left", hidden: true },
                        { name: 'MAST_PUB_CAT_NAME', index: 'MAST_PUB_CAT_NAME', width: 180, sortable: true, align: "left" },
                        { name: 'PUBLICATION_TITLE', index: 'PUBLICATION_TITLE', width: 300, sortable: true, align: "left" },
                        { name: 'PUBLICATION_AUTHOR', index: 'PUBLICATION_AUTHOR', width: 180, sortable: true, align: "left" },
                        { name: 'PUBLICATION_DATE', index: 'PUBLICATION_DATE', width: 150, sortable: true, align: "center" },
                        { name: 'PUBLICATION_VOLUME', index: 'PUBLICATION_VOLUME', width: 200, sortable: false, align: "left" },
                        { name: 'PUBLICATION_NAME', index: 'PUBLICATION_NAME', width: 200, sortable: true, align: "left" },
                        { name: 'PUBLICATION_PAGINATION', index: 'PUBLICATION_PAGINATION', width: 200, sortable: false, align: "left" },
                        { name: 'PUBLICATION_DESCRIPTION', index: 'PUBLICATION_DESCRIPTION', width: 200, sortable: false, align: "left" },
                        { name: 'Upload', index: 'Upload', width: 100, sortable: true, align: "left" },
                        { name: 'Edit', index: 'Edit', width: 100, sortable: true, align: "left", hidden: false },
                        { name: 'Delete', index: 'Delete', width: 100, sortable: true, align: "left" },
                        { name: 'Finalize', index: 'Finalize', width: 130, sortable: true, align: "left" },
                        { name: 'Publish', index: 'Publish', width: 130, sortable: true, align: "left", hidden: false }
        ],
        postData: {
            publication: $("#ddPublication").val(), published: $("#ddPublishe").val(), finalized: $("#ddFinalize").val()
        },
        pager: jQuery('#divPagerPublication'),
        rowNum:200,       
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Publication",
        autowidth: true,
        height: 280,
        sortname: 'MAST_PUB_CAT_NAME',
        rownumbers: true,
        headerrow:true,
        loadComplete: function () {
           
            //  alert("Load");

            //$("#divPagerPublication_left").html("<table style='float:left'><tr><td  style='border:none;font-weight:bold;margin-left:5px;'><span  class='ui-icon ui-icon-locked'>assa  </span></td> <td> <span  class='ui-icon ui-icon-unlocked'></span> </td></tr></table>");
            $("#divPagerPublication_left").html("<span  class='ui-icon ui-icon-info' style='float:left'></span><span style='float:left'>1) </span><span style='float:left'  class='ui-icon ui-icon-locked'></span><span style='text-align:left;color:black;float:left'>: Finalize and Published Button </span><span style='float:left;margin-left:5px;'> 2) </span> <span style='float:left'  class='ui-icon ui-icon-unlocked'></span><span style='text-align:left;color:black;float:left'>: De-finalize and Un-published Button</span>");
            $('#tblPublication_rn').html('Sr.<br/>No.');

            $.unblockUI();
        }
    }); //end of grid

}