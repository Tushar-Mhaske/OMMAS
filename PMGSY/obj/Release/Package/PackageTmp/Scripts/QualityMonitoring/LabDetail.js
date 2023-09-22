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
 
    //$('#PIULabDetailCreate').hide();

   

    //$("#LabEshtablishedDate").addClass("pmgsy-textbox");
    // $("#LabEshtablishedDate").datepicker({
    //    changeMonth: true,
    //    changeYear: true,
    //    dateFormat: "dd/mm/yy",
    //    showOn: 'button',
    //    buttonImage: '../../Content/images/calendar_2.png',
    //    buttonImageOnly: true,
    //    maxDate: new Date(),        
    //    onClose: function () {
    //        $(this).focus().blur();

    //    }
    //}).attr('readonly', 'readonly');


    //$("#QM_INSPECTION_DATE").datepicker("option", "minDate", $("#SCHEDULE_MONTH_YEAR_START_DATE").val());
    //$("#QM_INSPECTION_DATE").datepicker("option", "maxDate", $("#CURRENT_DATE").val());
   

    LoadGrid();
    
    $(function () {
        $("#accordionLabPhoto").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $(function () {
        $("#accordionLabPositionMap").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });
});


function CloseLabPhoto() {
    $('#accordionLabPhoto').hide('slow');
    $('#divLabPhoto').hide('slow');
    // $('#PIULabDetailList').show('slow');
    $("#tblQualityLab").jqGrid('setGridState', 'visible');
    LoadGrid();
   
}

function CloseLabPositionMap() {
    $('#accordionLabPositionMap').hide('slow');
    $('#divLabPositionMap').hide('slow');
    //$('#PIULabDetailList').show('slow');
    $("#tblQualityLab").jqGrid('setGridState', 'visible');

    
    // alert("OK");
}


function ShowLabPositionMap(id) {
    // alert(url);

   // $('#PIULabDetailCreate').hide();
    // $('#PIULabDetailList').hide();
    $("#tblQualityLab").jqGrid('setGridState', 'hidden');
    $('#divLabPhoto').hide();

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

    $('#divLabPositionMap').show('slow');
    $("#divLabPositionMap").css('height', 'auto');



}


function ShowLabPhoto(id) {
   // alert(url);
    
    //$('#PIULabDetailCreate').hide();
    //$('#PIULabDetailList').hide();
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
              
          });
      });

      $('#divLabPhoto').show('slow');
      $("#divLabPhoto").css('height', 'auto');

}


  
function LoadGrid() {

    $("#tblQualityLab").jqGrid('GridUnload');

   $('#tblQualityLab').jqGrid({
       url: '/QualityMonitoring/GetLabDetailList/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Sanction Year', 'Package', 'Lab Established Date', 'No of Lab Photo', 'SQC Finalized (Y/N)', 'TEND_AGREEMENT_CODE', 'QM_LAB_ID', 'TEND_DATE_OF_COMPLETION', 'TEND_DATE_OF_AWARD_WORK', 'TEND_DATE_OF_COMMENCEMENT', 'MAST_STATE_NAME', 'QM_LOCK_STATUS', 'Correction of Lab Photo Geo position in Google Map', 'Delete', 'Finalize'],
        colModel: [
        { name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 60, align: "left" },
        //{ name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 60, align: "left" },
        { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width:60, align: "left" },
        { name: 'QM_LAB_ESTABLISHMENT_DATE', index: 'QM_LAB_ESTABLISHMENT_DATE', height: 'auto', width: 60, align: "center" },
        //{ name: 'PHOTO', index: 'PHOTO', height: 'auto', width: 60, align: "left", formatter: returnPhotoLink },
        { name: 'PHOTO', index: 'PHOTO', height: 'auto', width: 60, align: "center" },
       // { name: 'QM_SQC_APPROVAL', index: 'QM_SQC_APPROVAL', height: 'auto', width: 60, align: "left", formatter: returnSQCFinalize },
        { name: 'QM_SQC_APPROVAL', index: 'QM_SQC_APPROVAL', height: 'auto', width: 60, align: "center" },
        { name: 'TEND_AGREEMENT_CODE', index: 'TEND_AGREEMENT_CODE', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'QM_LAB_ID', index: 'QM_LAB_ID', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'TEND_DATE_OF_COMPLETION', index: 'TEND_DATE_OF_COMPLETION', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'TEND_DATE_OF_COMPLETION', index: 'TEND_DATE_OF_AWARD_WORK', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'TEND_DATE_OF_COMPLETION', index: 'TEND_DATE_OF_COMMENCEMENT', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", hidden: true },
        { name: 'QM_LOCK_STATUS', index: 'QM_LOCK_STATUS', height: 'auto', width: 80, align: "left", hidden: true },
        //{ name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 80, align: "left", formatter: returnGoogleLink },
        //{ name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 40, align: "left", formatter: returnDeleteLink },
        //{ name: 'IMS_YEAR', index: 'IMS_YEAR', height: 'auto', width: 40, align: "left", formatter: returnFinalizeLink },
        { name: 'GoogleLink', index: 'GoogleLink', height: 'auto', width: 60, align: "center",hidden:true },
        { name: 'DeleteLink', index: 'DeleteLink', height: 'auto', width: 30, align: "center" },
        { name: 'FinalizeLink', index: 'FinalizeLink', height: 'auto', width: 30, align: "center" }
        ],
        postData: { level:"PIU" },
        pager: $('#divPagerQualityLab'),
        rowNum: 10000,
        viewrecords: true,
        caption: 'Lab Detail',
        pgbuttons: false,
        pgtext: null,
        recordtext: '{2} records found',
        height: '600px',
        //autowidth: true,
        //shrinkToFit: true,
        rownumbers: true,
        loadComplete: function () {
            $('#PIULabDetailList').show();
            $('#tblQualityLab').setGridWidth(($('#div1TierPIULabDetails').width() - 40), true);
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
    alert(rowdata[7]);
    if (rowdata[7]==0)
    {
            return "---";
    }
    else
    {
        return "<a href='#' onclick='ShowLabPhoto(" + rowdata[7] + ")' >" + rowdata[4] + "</a>";
            
    }
   
}


function returnSQCFinalize(cellValue, options, rowdata, action) {
    if (rowdata[7] == 0) {
        return "---";
    }
    else {
        if (rowdata[5] == "N") {
            return "No";
        }
        else {
            return "Yes";
        }
        
    }
   

}
function returnGoogleLink(cellValue, options, rowdata, action) {
    if (rowdata[7] == 0) {
        return "---";
    }
    else {
        if (rowdata[12] == "N") {
            return "<a href='javascript:void(0)' onclick='ShowLabPositionMap(" + rowdata[7] + ")' >Correct Geo Position</a>";
            
        }
        else {

            return "---";
        }

    }
    

}




function returnDeleteLink(cellValue, options, rowdata, action) {
    if (rowdata[7] == 0) {
        return "---";
    }
    else {
        if (rowdata[12] == "N") {
            return "<a href='javascript:void(0)' onclick='DeleteLabDetail(" + rowdata[7] + ")' >Delete</a>";
        }
        else {

            return "---";
        }
        
    }

}

function DeleteLabDetail(id) {
    
    if (confirm("Are you sure to delete Lab details ?")) {
        var typeUser = "PLD";
        //$.getJSON("/QualityMonitoring/LabDetailDeleteFinalize/", { id: id, type: "PLD" },
        //    function (data) {
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


function returnFinalizeLink(cellValue, options, rowdata, action) {
    //return "<a href='/Controller/Action/" + options.rowId + "' >" + rowdata[6] + "-" + rowdata[7] + "</a>";
    if (rowdata[7] == 0) {
        return "---";
    }
    else {
        if (rowdata[12] == "N") {
            return "<a href='javascript:void(0)' onclick='FinalizeLabDetail(" + rowdata[7] + ")' >Finalize</a>";
        }
        else {
            
            return "---";
        }

        
    }

}

function FinalizeLabDetail(id) {
    //alert("anand" + id);

    if (confirm("Are you sure to finalize Lab details?")) {
        var typeUser = "PLF";
        ///Changed by SAMMED A. PATIL on 06NOV2017 for Other PIU
        if ($('#RoleCode').val() == 22 || $('#RoleCode').val() == 38 || $('#RoleCode').val() == 54) { // Added by sachin || $('#RoleCode').val() == 54
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

//function LabDetailAdd(State,Year, PackageId,AwardDate,CommencementDate, CompletedDate,ContractorId) {
//    $('#PIULabDetailCreate').show();
//    $('#PIULabDetailList').hide();
//    $('#CommencementDate').text(CommencementDate);
//    $('#AwardDate').text(AwardDate);
//    $('#CompletedDate').text(CompletedDate);
//    $('#State').text(State);
//    $('#SanctionYear').text(Year+ " - " +(parseInt(Year)+1) );
//    $('#Package').text(PackageId);
//    $('#agreementId').text(ContractorId);
//    $('#LabEshtablishedDate').val("");
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




   
