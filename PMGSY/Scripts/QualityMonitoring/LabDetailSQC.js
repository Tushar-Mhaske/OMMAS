/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LabDetail.js
        * Description   :   List Lab Detail
        * Author        :   Anand Singh
        * Creation Date :   27/Aug/2014
 **/


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    //alert("SQC Ready");
    LoadGrid();
   
});


function CloseLabPhoto() {
    $('#accordionLabPhoto').hide('slow');
    $('#divLabPhoto').hide('slow');
    //$('#PIULabDetailList').show('slow');
    $("#tblQualityLab").jqGrid('setGridState', 'visible');
    LoadGrid();
}

function CloseLabPositionMap() {
    $('#accordionLabPositionMap').hide('slow');
    $('#divLabPositionMap').hide('slow');
   // $('#PIULabDetailList').show('slow');
    $("#tblQualityLab").jqGrid('setGridState', 'visible');
    
    // alert("OK");
}


function ShowLabPositionMap(id) {
    // alert(url);

   // $('#PIULabDetailCreate').hide();
   // $('#PIULabDetailList').hide();
    $("#tblQualityLab").jqGrid('setGridState', 'hidden');

    $('#accordionLabPositionMap').show('slow');
    $("#accordionLabPositionMap div").html("");
    $("#accordionLabPositionMap h3").html(
            "<a href='#' style= 'font-size:.9em;' >Lab Photo</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseLabPositionMap();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionLabPositionMap').show('slow', function () {
       // blockPage();

        $("#divLabPositionMap").load("/QualityMonitoring/LabPositoinGoogleMapTest/67630", function () {
          //  unblockPage();
           // alert("OK");
        });
    });

    //  $('#accordionLabPhoto').show('slow');
    $("#divLabPositionMap").css('height', 'auto');



}


function ShowLabPhoto(id) {
   // alert(url);
    
   // $('#PIULabDetailCreate').hide();
    // $('#PIULabDetailList').hide();
    $("#tblQualityLab").jqGrid('setGridState', 'hidden');
   
    $('#accordionLabPhoto').show('slow');
      $("#accordionLabPhoto div").html("");
      $("#accordionLabPhoto h3").html(
              "<a href='#' style= 'font-size:.9em;' >Lab Photo</a>" +

              '<a href="#" style="float: right;">' +
              '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseLabPhoto();" /></a>' +
              '<span style="float: right;"></span>'
              );

      $('#accordionLabPhoto').show('slow', function () {
          blockPage();
          
          $("#divLabPhoto").load("/QualityMonitoring/LabImageUpload/"+id, function () {
              unblockPage();
              //alert("OK");
          });
      });

      $('#divLabPhoto').show('slow');
      $("#divLabPhoto").css('height', 'auto');

      
    
}


  
function LoadGrid() {

    
    //url: '/QualityMonitoring/GetLabDetailListJQGrid/',
    $("#tblQualityLab").jqGrid('GridUnload');

   $('#tblQualityLab').jqGrid({
       url: '/QualityMonitoring/GetLabDetailList/',
        datatype: 'json',
        mtype: "POST",

        colNames: ['Sanction Year', 'District', 'Block Name', 'Package', 'Lab Eshtablished Date', 'No of Lab Photo', 'Latitude', 'Longitude', 'SQC Finalized (Y/N)', 'View in Google Map'],
        colModel: [
             { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 120, align: "left" },
             { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "left" },
             { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left" },
            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "left" },
            { name: 'QM_LAB_ESTABLISHMENT_DATE', index: 'QM_LAB_ESTABLISHMENT_DATE', height: 'auto', width: 80, align: "left", formatter: returnLabEshteblishedLink },
            { name: 'PHOTO', index: 'PHOTO', height: 'auto', width: 80, align: "left", formatter: returnPhotoLink },
            { name: 'Latitude', index: 'Latitude', height: 'auto', width: 80, align: "left", formatter: returnLabLatitude },
            { name: 'Longitude', index: 'Longitude', height: 'auto', width: 80, align: "left", formatter: returnLabLongitude },
            { name: 'QM_SQC_APPROVAL', index: 'QM_SQC_APPROVAL', height: 'auto', width: 80, align: "left", formatter: returnSQCFinalize },
            { name: 'QM_LAB_ID', index: 'QM_LAB_ID', height: 'auto', width: 80, align: "left", formatter: returnGoogleLink }        
        ],
        postData: { level:"SQC" },
        pager: $('#divPagerQualityLab'),
        rowNum: 10,
        //rowList: [10, 15, 20, 30],
        viewrecords: true,
        //recordtext: '{1} records found',
        caption: 'Lab Detail',
        height: 'auto',
        autowidth: true,
        shrinkToFit: true,
        rownumbers: true,
        
        loadComplete: function () {
            //  imagePreview();
           // alert("Complete");
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again!");
            }
        }
    });
  
}



function returnPhotoLink(cellValue, options, rowdata, action) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
        if (rowdata[9]==0)
        {
             return "---";
        }
        else
        {
            // return "<a href='/QualityMonitoring/LabImageUpload/" + rowdata[7] + "' >" + rowdata[4] + "</a>";
            return  rowdata[5];
            //alert("ShowLabPhoto('/QualityMonitoring/LabImageUpload/" + rowdata[7] + "')");
        }
   
}

function returnLabLatitude(cellValue, options, rowdata, action) {
    if (rowdata[9] == 0) {
        return "---";
    }
    else {
        return rowdata[6];
        
    }

}

function returnLabLongitude(cellValue, options, rowdata, action) {
    if (rowdata[9] == 0)
        return "---";
    else 
        return rowdata[7];

}
function returnLabEshteblishedLink(cellValue, options, rowdata, action) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
    if (rowdata[9] == 0) {
       
        return "---";
    }
    else {
        
        return rowdata[4];
    }

}

function returnSQCFinalize(cellValue, options, rowdata, action) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
    if (rowdata[9] == 0) {
        return "---";
    }
    else {
        //alert(rowdata[8]);
        if (rowdata[8] == "N") {
            return "<a href='javascript:void(0)' onclick='FinalizeLabDetailSQC(" + rowdata[9] + ")' >Finalize</a>";
        }
        else {
            return "---";
        }
        
    }
   

}

function FinalizeLabDetailSQC(id) {
    //alert("anand" + id);
    //if (confirm("Are you sure to finalize Lab details?")) {
        //$.getJSON("/QualityMonitoring/LabDetailDeleteFinalize/", { id: id, type: "SLF" }, function (data) {
        //    // alert(data);           
        //    if (data == "Success") {
        //        alert("Successfully Record is Finalized!");
        //        LoadGrid();
        //    }
        //    else {
        //        alert("Processing Error!");
        //    }
        //});
    //}

        if (confirm("Are you sure to finalize Lab details?")) {
            var typeUser = "PLF";
            if ($('#RoleCode').val() == 22) {
                typeUser = "PLF";
            } else {
                typeUser = "SLF";
            }
            //$.getJSON("/QualityMonitoring/LabDetailDeleteFinalize/", { id: id, type: typeUser }, function (data) {

            //    if (data.success == true) {
            //       alert(data.message);
            //        LoadGrid();
            //    }
            //    else {
            //        alert(data.message);
            //    }

            //});
            $.ajax({

                url: "/QualityMonitoring/LabDetailDeleteFinalize?id=" + id + "&type=" + typeUser,
                type: 'POST',
                catche: false,
                data: $("#frmPropAddCost").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be  processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        $('#tblQualityLab').trigger('reloadGrid');
                        unblockPage();
                    }
                    else {
                        alert(response.message);
                        unblockPage();
                    }
                },
            });


        }
}

function returnGoogleLink(cellValue, options, rowdata, action) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
    if (rowdata[9] == 0) {
        return "---";
    }
    else {
           // return "<a target='_blank' href='/QualityMonitoring/LabPositoinGoogleMap/' >Correct Geo Position</a>";
            return "<a href='javascript:void(0)' onclick='ShowLabPositionMap(" + rowdata[9] + ")' >View Geo Position</a>";         
       

        
       // return "<a href='javascript:void(0)' onclick='ShowLabPositionMap(" + rowdata[7] + ")' >Correct Geo Position</a>";
    }
    

}







function UploadPhoto(labid) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
    alert("Lab Id: " + labid);
}

//function LabDetailAdd(State,Year, PackageId,AwardDate,CommencementDate, CompletedDate,ContractorId) {
//    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
//    $('#PIULabDetailCreate').show();
//    $('#PIULabDetailList').hide();
//    $('#CommencementDate').text(CommencementDate);
//    $('#AwardDate').text(AwardDate);
//    $('#CompletedDate').text(CompletedDate);
//    $('#State').text(State);
//    $('#SanctionYear').text(Year);
//    $('#Package').text(PackageId);
//    $('#agreementId').text(ContractorId);
    
//   // alert("ContractorId: " + State + "PackageId: " + Year + "CommencementDate: " + CommencementDate + "AwardDate: " + AwardDate + "CompletedDate: " + CompletedDate);
//}


function LabDetailAdd(State, Year, PackageId, AwardDate, CommencementDate, CompletedDate, ContractorId) {


    //$('#PIULabDetailCreate').hide();
    $("#tblQualityLab").jqGrid('setGridState', 'hidden');
    //$('#PIULabDetailList').hide();

    $('#accordionLabPhoto').show('slow');

    $("#accordionLabPhoto div").html("");
    $("#accordionLabPhoto h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Lab Establishment Date</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="CloseLabPhoto();" /></a>' +
            '<span style="float: right;"></span>'
            );

    $('#accordionLabPhoto').show('slow', function () {
        blockPage();

        $("#divLabPhoto").load('/QualityMonitoring/AddLabCommencementDateDetails/', { stateName: State, imsYear: Year, packageId: PackageId, dateOfAwardWork: AwardDate, dateofCommencement: CommencementDate, dateofCompletion: CompletedDate, agreementCode: ContractorId }, function () {
            unblockPage();

        });
    });

    $('#divLabPhoto').show('slow');
    $("#divLabPhoto").css('height', 'auto');

    //$("#accordion div").html("");
    //$("#accordion h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Add Additional Cost Details</a>" +
    //        '<a href="#" style="float: right;">' +
    //        '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
    //        );

    //$('#accordion').show('fold', function () {
    //    blockPage();
    //    $("#divAddExecution").load('/QualityMonitoring/AddLabCommencementDateDetails/', { stateName: State, imsYear: Year, packageId: PackageId, dateOfAwardWork: AwardDate, dateofCommencement: CommencementDate, dateofCompletion: CompletedDate, agreementCode: ContractorId },
    //        function () {
    //        //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
    //        unblockPage();
    //    });
    //    $('#divAddExecution').show('slow');
    //    $("#divAddExecution").css('height', 'auto');
    //});
    //$("#tblQualityLab").jqGrid('setGridState', 'hidden');


}

   
