
jQuery.validator.addMethod("comparefieldhabitationpopvalidator", function (value, element, param) {

    if (parseInt($('#MAST_HAB_TOT_POP').val()) < parseInt($('#MAST_HAB_SCST_POP').val()))
        return false;
    else
        return true;


});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldhabitationpopvalidator");

var suceessFlag;

$(document).ready(function () {
    
   

    $('#MAST_HAB_TOT_POP').focus();

    LoadHabitationDetails();


    $.validator.unobtrusive.parse($('#frmAddHabitationDetails'));

    //if ($('#EncryptedHabitationDetailsCode').val() == '') {
    //    $('#ddlYears').prepend("<option value=0  selected='selected'>--Select--</option>");
    //}


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    //added by Vikram in order to uncheck all radio buttons

    //if ($("#EncryptedHabitationCode_OtherDetails").val() == null) {
    //    $('form[id^="frmAddHabitationDetails"]').find("input:radio:checked").prop('checked', false);
    //}


    $('#btnSaveOtherDetails').click(function (e) {


        if ($('#frmAddHabitationDetails').valid()) {

            var habName = $('#HabitationName').text();
            var yearID = $('#ddlYears option:selected').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/AddOtherHabitationDetails",
                type: "POST",
                //dataType: "json",
                data: $("#frmAddHabitationDetails").serialize(),
                success: function (data) {


                    if (data.success==true) {
                        suceessFlag = true;
                      
                        alert(data.message);
                       
                        ClearDetails();

                       // $("#dvOtherHabitationDetails").hide('slow');

                        $('#tbOtherHabitationList').jqGrid("setGridParam", { "postData": { HabCode: $('#EncryptedHabitationCode_OtherDetails').val() } });
                        $('#tbOtherHabitationList').trigger('reloadGrid', [{ page: 1 }]);

                    }
                    else if (data.success==false) {
                        suceessFlag = false;
                      
                        if (data.message != "") {
                            $('#message_otherDetails').html(data.message);
                            $('#dvErrorMessage_OtherDetails').show('slow');
                        }

                    }
                    else {
                        suceessFlag = false;
                      
                        $("#dvOtherHabitationDetails").html(data);
                        $('#HabitationName').text(habName);
                        $('#ddlYears').val(yearID);
                    }

                    $.unblockUI();

                },
                complete: function () {
                    
                    if (suceessFlag) {
                        $("#dvOtherHabitationDetails").hide('slow');
                    }                
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });


    $('#btnUpdateOtherDetails').click(function (e) {

        if ($('#frmAddHabitationDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
         
            var yearID = $('#ddlYears option:selected').val();

            $("#ddlYears").attr("disabled", false);


            $.ajax({
                url: "/LocationMasterDataEntry/EditOtherHabitationDetails",
                type: "POST",
               // dataType: "json",
                data: $("#frmAddHabitationDetails").serialize(),
                success: function (data) {

                    var HabCode = $('#EncryptedHabitationCode_OtherDetails').val();
                    var HabName = $('#HabitationName').text();

                    if (data.success==true) {
                        alert(data.message);
                        $('#tbOtherHabitationList').jqGrid("setGridParam", { "postData": { HabCode: $('#EncryptedHabitationCode_OtherDetails').val() } });
                        $('#tbOtherHabitationList').trigger('reloadGrid', [{ page: 1 }]);

                        //Commented By Abhishek kamble 24-feb-2014
                        //$('#dvOtherHabitationDetails').load("/LocationMasterDataEntry/AddOtherHabitationDetails", function () {
                        //    $('#EncryptedHabitationCode_OtherDetails').val(HabCode);
                        //    $('#HabitationName').text(HabName);
                        //    $.unblockUI();
                        //}
                        //);

                        //Added By Abhishek kamble 24-Feb-2014
                        $.unblockUI();
                        $("#dvOtherHabitationDetails").hide('slow');
                    }
                    else if (data.success == false) {
                    
                        if (data.message != "") {
                            $("#ddlYears").attr("disabled", true);
                            $('#message_otherDetails').html(data.message);
                            $('#dvErrorMessage_OtherDetails').show('slow');
                            $.unblockUI();
                        }
                    }
                    else {
                        $("#dvOtherHabitationDetails").html(data);
                        $("#ddlYears").attr("disabled", true);
                        $('#HabitationName').text(HabName);
                        $('#ddlYears').val(yearID);
                        $.unblockUI();
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancelOtherDetails').click(function (e) {
 
        var HabCode = $('#EncryptedHabitationCode_OtherDetails').val();
        var HabName = $('#HabitationName').text();
 
        $.ajax({
            url: "/LocationMasterDataEntry/AddOtherHabitationDetails",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $('#dvOtherHabitationDetails').html(data);
                $('#EncryptedHabitationCode_OtherDetails').val(HabCode);
                $('#HabitationName').text(HabName);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });

    $('#btnResetOtherDetails').click(function (e) {
        //e.preventDefault();
        //ClearDetails(); commented by Vikram as the default radio buttons should be unchecked
        
        //Added By Abhishek kamble 24-Feb-2014
        $("#dvErrorMessage_OtherDetails").html('');
        $("#dvErrorMessage_OtherDetails").hide();

        
        $("#frmAddHabitationDetails").each(function () {
            $(this).find(".field-validation-error").empty();
            $(this).trigger('reset.unobtrusiveValidation');
        });
        $('form[id^="frmAddHabitationDetails"]').find("input:radio:checked").prop('checked', false);
    });

    $("#ddlYears").change(function () {
        if ($("#dvErrorMessage_OtherDetails").is(":visible")) {
            $('#dvErrorMessage_OtherDetails').hide('slow');
            $('#message_otherDetails').html('');
        }

    });

    $('#btnBackOtherDetails').click(function (e)
    {
        $('#dvOtherHabitationDetails').hide('slow');
        //if ($("#dvHabitationDetails").is(":visible")) {

        //    $("#dvHabitationDetails").hide('slow');
        //    $('#btnSearchView').hide();
        //    $('#btnCreateNew').show();
        //}
        //if (!$("#dvSearchHabitation").is(":visible")) {
        //    $("#dvSearchHabitation").show('slow');
        //}

    });


    //Added By Abhishek kamble 29-Apr-2014 start
    $(function () {       

        if ($("#isHabConnected").val() == "False") {
            $('#rdoHasHabConnectedNo').trigger("click");
        }
        
    });
    $('#rdoHasHabConnectedNo').click(function () {
        $("#tdSchemeRequeired").hide();
        $("#tdSchemeNotRequeired").show();        
    });

    $('#rdoHasHabConnectedYes').click(function () {
        $("#tdSchemeRequeired").show();
        $("#tdSchemeNotRequeired").hide();
    });
    //Added By Abhishek kamble 29-Apr-2014 end
});

function ClearDetails() {

    $('#ddlYears').val('0');
    $('#MAST_HAB_TOT_POP').val('');
    $('#MAST_HAB_SCST_POP').val('');
    $('#rdoHasHabConnectedNo').attr('checked', true);
    $('#rdoISPanchayatHQNo').attr('checked', true);
    $('#rdoISSchemeNo').attr('checked', true);
    $('#rdoHasPrimarySchoolNo').attr('checked', true);
    $('#rdoHasMiddleSchoolNo').attr('checked', true);
    $('#rdoHasHighSchoolNo').attr('checked', true);
    $('#rdoHasIntermediateSchoolNo').attr('checked', true);
    $('#rdoHasDegreeCollegeNo').attr('checked', true);


    $('#rdoHasHealthServiceNo').attr('checked', true);
    $('#rdoHasDespensaryNo').attr('checked', true);
    $('#rdoHasPHCSNo').attr('checked', true);
    $('#rdoHasVetnaryHospitalNo').attr('checked', true);
    $('#rdoHasMCWCentersNo').attr('checked', true);
    $('#rdoHasTelegraphOfficeNo').attr('checked', true);

    $('#rdoHasTelephoneConnectionNo').attr('checked', true);
    $('#rdoHasBusServiceNo').attr('checked', true);
    $('#rdoHasRailwayStationNo').attr('checked', true);
    $('#rdoHasElectricityNo').attr('checked', true);
    $('#rdoIsTouristPlaceNo').attr('checked', true);

    //added by Ujjwal Saket on 19-10-2013 for PMGSY II
    $('#rdoHasITINo').attr('checked', true);
    $('#rdoHasPetrolPumpNo').attr('checked', true);
    $('#rdoHasAdditionalPetrolPumpNo').attr('checked', true);
    $('#rdoHasAdditionalElectricityNo').attr('checked', true);
    $('#rdoHasMandiNo').attr('checked', true);
    $('#rdoHasWarehouseNo').attr('checked', true);
    $('#rdoHasRetailShopNo').attr('checked', true);
    $('#rdoHasSubTehsilNo').attr('checked', true);
    $('#rdoHasBlockHeadquarterNo').attr('checked', true);


    if ($("#dvErrorMessage_OtherDetails").is(":visible")) {
        $('#dvErrorMessage_OtherDetails').hide('slow');
        $('#message_otherDetails').html('');
    }

}

function LoadHabitationDetails() {

    jQuery("#tbOtherHabitationList").jqGrid({
        url: '/LocationMasterDataEntry/GetOtherHabitationDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { HabCode: $('#EncryptedHabitationCode_OtherDetails').val() },
        colNames: ['Habitation Name','Is Habitation Connected','Is Included In PMGSY Scheme', 'Census Year', 'Total Population', 'SC/ST Population', 'Action','Lock Status'],
        colModel: [
                             { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: 200, align: "left", sortable: false },
                             { name: 'IsHabitationConnected', index: 'IsHabitationConnected', height: 'auto', width: 150, align: "left", sortable: false },//Added By Abhishek kamble 14-Mar-2014
                             { name: 'IsIncludedInPMGSYScheme', index: 'IsIncludedInPMGSYScheme', height: 'auto', width: 150, align: "left", sortable: false },//Added By Abhishek kamble 14-Mar-2014
                             { name: 'Year', index: 'Year', height: 'auto', width: 90, sortable: true, align: "left" },
                             { name: 'TotalPopulation', index: 'TotalPopulation', height: 'auto', width: 100, align: "left", sortable: true },
                             { name: 'SC/STPopulation', index: 'SCSTPopulation', height: 'auto', width: 100, align: "left", sortable: true },
                             /*{ name: 'MPContituency', index: 'MPContituency', height: 'auto', width: 150, sortable: false, align: "left" },
                             { name: 'MLAContituency', index: 'MLAContituency', height: 'auto', width: 150, sortable: false, align: "left" },
                             { name: 'IsSchedule5', index: 'IsSchedule5', height: 'auto', width: 100, sortable: false, align: "left" },*/
                            { name: 'a', width: 80, sortable: false, resize: false, align: "center", sortable: false }, //formatter: FormatColumn
                            { name: 'LockStatus', width: 80, sortable: false, resize: false, align: "center", sortable: true, hidden: $("#RoleCode").val() == 23 ? false : true }

        ],
        pager: jQuery('#dvOtherHabitationListPager'),
        rowNum: 5,
       // altRows: true,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'HabitationName',
        sortorder: "asc",
        caption: "Habitation Other Details",
        height: 'auto',
        //width: 'auto',
        autowidth:true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function (data) {
            var recordCount = jQuery('#tbOtherHabitationList').jqGrid('getGridParam', 'reccount');

           // alert(recordCount +' '+ data.userdata.lockStatus);
            if (recordCount > 0 && (data.userdata.lockStatus == "N"||data.userdata.lockStatus == "M")) {

                var button = '<input type="button" id="btnFinalizeHabitation" name="btnFinalizeHabitation" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize Habitation" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeHabitation()" />'
                $("#btnCreateNew_OtherDetails").hide();
                $('#dvOtherHabitationListPager_left').html(button);

            }
            else if (recordCount > 0 && (data.userdata.lockStatus == "Y" ||data.userdata.lockStatus == "M")) {
                $('#btnCreateNew_OtherDetails').hide();
                $('#btnCreateNew_OtherDetails').html('');
                $('#dvOtherHabitationListPager_left').html('');
            }
            else if (recordCount == 0) {
                $('#dvOtherHabitationListPager_left').html('');
                $('#btnCreateNew_OtherDetails').show();
            }
            else {
            }
            /*if (recordCount == 0) {
                $('#dvOtherHabitationDetailsList').hide();
            }
            else {
                $('#dvOtherHabitationDetailsList').show();
            }*/

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },

    }); //end of grid

}

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Other Habitation Details' onClick ='EditOtherHabitationDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='View Habitation Other Details' onClick ='ViewHabitationOtherDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Other Details' onClick ='DeleteHabitationOtherDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function EditOtherHabitationDetails(paramater) {

    var HabCode = $('#EncryptedHabitationCode_OtherDetails').val();
    $('#dvOtherHabitationDetails').show('slow');
    $.ajax({
        url: "/LocationMasterDataEntry/EditOtherHabitationDetails/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {
           
            $("#dvOtherHabitationDetails").html(data);
            $('#EncryptedHabitationCode_OtherDetails').val(HabCode);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });
}

function DeleteHabitationOtherDetails(urlparamater) {

    if (confirm("Are you sure you want to delete habitation other details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteHabitationOtherDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    $('#tbOtherHabitationList').trigger('reloadGrid');
                    $('#btnCancelOtherDetails').trigger('click');

                    //Added By Abhishek kamble 25-Feb-2014
                    $("#dvOtherDetails").hide("slow");
                    $("#tbHabitationList").jqGrid('setGridState', 'visible');

                    
                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}

function ViewHabitationOtherDetails(paramater) {

    var HabCode = $('#EncryptedHabitationCode_OtherDetails').val();
    $('#dvOtherHabitationDetails').show('slow');
    $.ajax({
        url: "/LocationMasterDataEntry/ViewOtherHabitationDetails/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            //$("#dvOtherHabitationDetails").html(data);
            $("#dvOtherHabitationDetails").html(data);
            $('#EncryptedHabitationCode_OtherDetails').val(HabCode);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });
}
function FinalizeHabitation() {
    var message = '';
    message = "once you finalize the habitation you can not modify details, Are you sure you want to finalize habitation?";

    var urlparameter = $('#EncryptedHabitationCode_OtherDetails').val();
    if (confirm(message)) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizeHabitation/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success==true) {
                    alert(data.message);

                    //$('#tbOtherHabitationList').trigger('reloadGrid');

                    //$('#tbHabitationList').trigger('reloadGrid');


                    $("#dvOtherHabitationDetails").hide();
                    if ($("#dvHabitationDetails").is(":visible")) {

                        $("#dvHabitationDetails").hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                    }
                    if (!$("#dvSearchHabitation").is(":visible")) {
                        $("#dvSearchHabitation").show('slow');
                        $('#tbHabitationList').trigger('reloadGrid');
                        $('#tbOtherHabitationList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbHabitationList').trigger('reloadGrid');
                        $('#tbOtherHabitationList').trigger('reloadGrid');
                    }

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}