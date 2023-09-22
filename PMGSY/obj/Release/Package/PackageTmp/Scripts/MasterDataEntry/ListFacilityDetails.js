this.imagePreview = function () {

    xOffset = 10;
    yOffset = 10;
    var Mx = 1000;// $(document).width();
    var My = 600;// $(document).height();

    var callback = function (event, param) {
        var $img = $("#preview");

        var trc_x = xOffset + $img.width();
        var trc_y = yOffset + $img.height();

        trc_x = Math.min(trc_x + event.pageX, Mx);
        trc_y = Math.min(trc_y + event.pageY, My);
        $img
			.css("top", (trc_y - $img.height()) + "px")
			.css("left", (trc_x - $img.width()) + "px");
 };


    $("a.preview").hover(function (e) {
        Mx = $(this).offset().left + 40; // * 2;//600
        My = $(this).offset().top - 10; //600;

        var arrLink = this.href.split("$");
        var lnkHref = arrLink[0];
        // ---------------------------------------------------------
        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img  style='height: 500px; width: 500px;' height='800' width='600' src='" + lnkHref + "' alt='Image Not Available' />" + c + "</p>");
        callback(e, 200);
        $("#preview").fadeIn("slow");
    },
		function () {
		    this.title = this.t;
		    $("#preview").remove();
		}
	)
};
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {
    $("#dvhdRemark").hide("fast");
    $('#btnCreateNew').show();
    $.unblockUI();

    $("#dvhdSearchFacilityHeader").click(function () {

        if ($("#hdrSearchFromDiv").is(":visible")) {

            $("#spCollapseIconCNSearch").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#hdrSearchFromDiv").slideToggle(300);
        }

        else {
            $("#spCollapseIconCNSearch").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#hdrSearchFromDiv").slideToggle(300);
        }
    });


    $("#btnSearchNew").click(function () {
        $("#dvPanchayatDetails").hide();
        $("#dvPanchayatDetails").html(" ");
        $("#divSearchFacilityForm").show();
        $("#dvSearchPanchayat").show("slow");
        $("#dvPanchayatDetails").hide();
        $("#ddlFacilitySearch").on("change");
    });

    $("#btnSearch").click(function ()
    {
        if ($("#dvhdRemark").is(":visible") || $("#IdRemarkForm").is(":visible"))
        {
            $("#dvhdRemark").hide("slow");
            $("#IdRemarkForm").hide("slow");
        }

        LoadGrid();
        $('#dvPanchyatList').show('slow');
    });


    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchPanchayat").is(":visible")) {
            $('#dvSearchPanchayat').hide('slow');
        }

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        dvhdSearchFacilityDetails
        if ($("#dvhdSearchFacilityDetails").is(":visible")) {
            $('#dvhdSearchFacilityDetails').hide();
        }

        if (!$("#dvPanchayatDetails").is(":visible")) {
            $('#dvPanchayatDetails').load("/LocationMasterDataEntry/AddFacilityLayout");
            $('#dvPanchayatDetails').show('slow');

            //$('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
        $('#tbPanchyatList').jqGrid("setGridState", "visible");
    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        $('#dvPanchayatDetails').hide('slow');
        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        $('#tbPanchyatList').jqGrid("setGridState", "visible");

        if (!$("#dvSearchPanchayat").is(":visible")) {

            $('#dvSearchPanchayat').load('/LocationMasterDataEntry/SearchPanchayat', function () {

                // $('#tbPanchyatList').trigger('reloadGrid');

                var data = $('#tbPanchyatList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);

                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                  "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

                    setTimeout(function () {

                        $('#ddlSearchDistrict').val(data.districtCode);

                        $('#ddlSearchBlocks').empty();
                        FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                                    "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());

                    }, 1000);
                    setTimeout(function () {
                        $('#ddlSearchBlocks').val(data.blockCode);
                    }, 2000);
                }
                $('#dvSearchPanchayat').show('slow');
            });

        }
        $.unblockUI();
    });

    $("#dvShiftPanchayat").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Panchayat'
    });


    $('#ddlFacilitySearch').change(function () {
        LoadFacilityName();
    });

    $("#ddlBlocksSearch").change(function () {
        $("#ddlBlocksSearch").addClass("valid");
        var BlockCode = $('#ddlBlocksSearch option:selected').val();
        $.ajax({
            url: '/LocationMasterDataEntry/GetHabitationBlockCodeSearch/',
            data: { blockCode: BlockCode },
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlHabitationSearch').empty();
                $.each(data, function () {
                    $('#ddlHabitationSearch').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });
    });

    $("#ddlFacilityNameListSearch").change(function () {
        $("#ddlFacilityNameListSearch").addClass("valid");
    });

    $("#ddlHabitationSearch").change(function () {
        $("#ddlHabitationSearch").addClass("valid");
    });


});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

    // return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Panchayat Details' onClick ='DeletePanchayatDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnShift(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Panchayat' onClick ='ShiftPanchayat(\"" + cellvalue.toString() + "\");' >Shift Panchayat</a></td></tr></table></center>";

}


function FormatColumnMap(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Map Habitations' onClick ='MapHabitations(\"" + cellvalue.toString() + "\");' >Map Habitations</a></td></tr></table></center>";

}

function FormatColumnMapped(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Habitations' onClick ='MappedHabitations(\"" + cellvalue.toString() + "\");' >Mapped Habitations</a></td></tr></table></center>";

}

function EditPanchayatDetails(paramater) {

    //  alert(paramater);
    $.ajax({
        url: "/LocationMasterDataEntry/EditPanchayat/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            //$("#mainDiv").html(data);

            if ($("#dvSearchPanchayat").is(":visible")) {
                $('#dvSearchPanchayat').hide('slow');
            }

            if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                $('#dvMapPanchayatHabitationsDetails').hide('slow');
            }

            if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                $('#dvMappedPanchayatHabitationDetails').hide('slow');
            }

            //$('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();
            $("#dvPanchayatDetails").html(data);
            $('#dvPanchayatDetails').show('slow');
            $("#MAST_PANCHAYAT_NAME").focus();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An Error Occurred while proccesing Request");
        }

    });
}

function LoadGrid() {
    $('#tbPanchyatList').jqGrid('GridUnload');
    jQuery("#tbPanchyatList").jqGrid({
        url: '/LocationMasterDataEntry/GetFacilityDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Facility ID', 'Facility Category ID', 'District Name ', 'Block Name', 'Habitation Name', 'Facility Category', 'Facility type', 'Facility Name', 'Address', 'Pin Code', "PhotoGraph", 'Finalize', 'Upload Photograph',
            'Edit', 'Delete', 'Finalize', 'View Details','Definalize'],
        colModel: [
                             { name: 'FacilityID', index: 'FacilityID', height: 'auto', width: 150, align: "center", sortable: true, hidden: true },
                             { name: 'FacilityCategoryID', index: 'FacilityCategoryID', height: 'auto', width: 150, align: "center", sortable: true, hidden: true },
                             { name: 'HabitationName', index: 'HabitationName', height: 'auto', width: 150, align: "center", hidden: false },
                             { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 150, align: "center", hidden: false },
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 100, align: "center", hidden: false },
                             { name: 'FacilityCategory', index: 'FacilityCategory', height: 'auto', width: 100, align: "center", hidden: false },
                             { name: 'FacilityName', index: 'FacilityName', height: 'auto', width: 150,  align: "center", hidden: false },
                             { name: 'FacilityDesc', index: 'FacilityDesc', height: 'auto', width: 150,  align: "center", hidden: false },
                             { name: 'Address', index: 'Address', height: 'auto', width: 200, sortable: false, align: "center", hidden: false },
                             { name: 'PinCode', index: 'PinCode', width: 80, sortable: false, resize: false, align: "center" },
                             { name: 'ActionThumbnail', index: 'Action', width: 80, sortable: false, resize: false, align: "center", formatter: imageDisplayFormatter, hidden: false },

                             { name: 'Finalize', index: 'Finalize', width: 140, sortable: false, resize: false, align: "center", hidden: true },
                             { name: 'UploadPhoto', index: 'UploadPhoto', width: 45, sortable: false, resize: false, align: "center" },
                             { name: 'ActionEdit', index: 'ActionEdit', width: 40, sortable: false, resize: false, align: "center" },
                             { name: 'Action', index: 'Action', width: 40, sortable: false, resize: false, align: "center" },

                             { name: 'FinalizeDetails', index: 'FinalizeDetails', width: 40, sortable: false, resize: false, align: "center" },
                             { name: 'ActionView', index: 'Action', width: 40, sortable: false, resize: false, align: "center" },
                             { name: 'DeFinalizeDetails', index: 'DeFinalizeDetails', width: 40, sortable: false, resize: false, align: "center", hidden:true },
                            
        ],
        postData: { formdata: $("#frmSearchFacility").serialize() },
        pager: jQuery('#dvPanchyatListPager'),
        rowNum: 15,
        rowList: [15, 20, 30],
        //rowList: [50,100],
        viewrecords: true,
        //loadonce: true,
        //navOptions: { reloadGridOptions: { fromServer: true } },
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'FacilityID',
        caption: "Facility List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,

        loadComplete: function () {
            var ids = jQuery("#tbPanchyatList").jqGrid('getDataIDs');

            for (var i = 0; i < ids.length; i++) {

                //if (IsFinalized = "Yes") {

                cl = ids[i];
                var IsFinalized = $('#tbPanchyatList').jqGrid('getCell', cl, 'Finalize');
                var EncryptedID = $('#tbPanchyatList').jqGrid('getCell', cl, 'FacilityID');
                if (IsFinalized === "Yes")
                {
                    be = "<center><span class='ui-icon ui-icon-locked' title='Locked' onClick ='abc(\"" + cl + "\");'></span></center>";
                    edit = "<center><span class='ui-icon ui-icon-locked' title='Locked' onClick ='abc(\"" + cl + "\");'></span></center>";
                    Finalize = "<center><span class='ui-icon ui-icon-locked' title='Locked' onClick ='abc(\"" + cl + "\");'></span></center>";
                    btnView = "<center><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Facility Details' onClick ='ViewFacilityDetails(\"" + cl + "\");'></span></center>";
                    DeFinalize = "<center><span class='ui-icon ui-icon-locked' title='DeFinalize Facility Details' onClick ='definalizeFacility(\"" + EncryptedID + "\");'></span></center>";
                }
                else if (IsFinalized === "-")
                {
                    be = "<center><span class='ui-icon ui-icon-locked' title='Locked' onClick ='abc(\"" + cl + "\");'></span></center>";
                    edit = "<center><span class='ui-icon ui-icon-locked' title='Locked' onClick ='abc(\"" + cl + "\");'></span></center>";
                    Finalize = "-";
                    btnView = "<center><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Facility Details' onClick ='ViewFacilityDetails(\"" + cl + "\");'></span></center>";
                    DeFinalize = "-";
                }
                else
                {
                    be = "<center><span class='ui-icon ui-icon-trash' id = 'edit" + i + "'" + " " + " title = 'Delete Facility Details' onClick ='UpdateLink(\"" + cl + "\");'></span></center>";
                    edit = "<center><span class='ui-icon ui-icon-pencil' title='Edit Facility Details' onClick ='EditFacilityDetails(\"" + cl + "\");'></span></center>";
                    Finalize = "<center><span class='ui-icon ui-icon-unlocked' title='Finalize Facility Details' onClick ='finalizeFacility(\"" + EncryptedID + "\");'></span></center>";
                    btnView = "<center><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Facility Details' onClick ='ViewFacilityDetails(\"" + cl + "\");'></span></center>";
                    DeFinalize = "-";
                }


                jQuery("#tbPanchyatList").jqGrid('setRowData', ids[i], { Action: be });
                jQuery("#tbPanchyatList").jqGrid('setRowData', ids[i], { ActionEdit: edit });
                jQuery("#tbPanchyatList").jqGrid('setRowData', ids[i], { ActionView: btnView });
                jQuery("#tbPanchyatList").jqGrid('setRowData', ids[i], { FinalizeDetails: Finalize });
                jQuery("#tbPanchyatList").jqGrid('setRowData', ids[i], { DeFinalizeDetails: DeFinalize });


            }
            imagePreview();
        },

        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    }); //end of documents grid
}

function imageDisplayFormatter(cellvalue, options, rowObject) {
    var PictureURL = cellvalue.split('#')[0];
    var facilityid = cellvalue.split('#')[1];
    return " <a href='" + PictureURL + "' onclick='doNothing(); return false;'  class='preview'><img style='height: 75px; width: 100px; border:solid 1px black;' src='" + PictureURL + "' alt='Image not Available' /> </a>"
    + getButton(facilityid);
}
function getButton(id) {
    if (id) {
        return id;
    }
    else {
        return " ";
    }
}

function doNothing() {
    return false;
}

function MapHabitations(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/LocationMasterDataEntry/MapPanchayatHabitations/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMapPanchayatHabitationsDetails").html(data);
            $('#dvMapPanchayatHabitationsDetails').show('slow');


            $('#btnSearch_Map').trigger('click');

            $('#trAddNewSearch').hide();
            $('#dvSearchPanchayat').hide();
            $('#dvPanchayatDetails').hide();

            if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                $('#dvMappedPanchayatHabitationDetails').hide('slow');
            }

            $('#tbPanchyatList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An error occcurred while proccessing the request");
            $.unblockUI();
        }

    });
}

function ShiftPanchayat(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
        $('#dvMapPanchayatHabitationsDetails').hide('slow');
    }

    if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
        $('#dvMappedPanchayatHabitationDetails').hide('slow');
    }

    $('#btnSearchView').trigger('click');
    $('#trAddNewSearch').show();

    $('#dvShiftPanchayat').empty();
    $("#dvShiftPanchayat").load("/LocationMasterDataEntry/ShiftPanchayat?id=" + parameter, function () {

        $("#dvShiftPanchayat").dialog('open');
        $.unblockUI();
    })

}
function DeletePanchayatDetails(urlparamater) {
    if (confirm("Are you sure you want to delete panchayat details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
        $.ajax({
            url: "/LocationMasterDataEntry/DeletePanchayatDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                        $('#dvMapPanchayatHabitationsDetails').hide('slow');
                    }

                    if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                        $('#dvMappedPanchayatHabitationDetails').hide('slow');
                    }
                    $('#dvPanchayatDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchPanchayat").is(":visible")) {
                        $("#dvSearchPanchayat").show('slow');
                        $('#tbPanchyatList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbPanchyatList').trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("An error occurred");
                $.unblockUI();
            }

        });

        if (!$("#dvPanchayatDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}

function MappedHabitations(parameter) {
    // alert(parameter);

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/LocationMasterDataEntry/MappedPanchayatHabitations/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMappedPanchayatHabitationDetails").html(data);
            $('#dvMappedPanchayatHabitationDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#dvSearchPanchayat').hide();
            $('#dvPanchayatDetails').hide();

            if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                $('#dvMapPanchayatHabitationsDetails').hide('slow');
            }

            $('#tbPanchyatList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("An error occurred");
            $.unblockUI();
        }

    });


}

function FinalizePanchayat() {
    var id = $('#tbPanchyatList').jqGrid('getGridParam', 'selrow');

    if ($('#tbPanchyatList').jqGrid('getGridParam', 'selrow')) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizePanchayat/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $("#tbPanchyatList").trigger('reloadGrid');
                    $.unblockUI();
                }
                else if (data.success == false) {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function () { $.unblockUI(); }
        });
    }
    else {
        alert('Please select Panchayat to finalize.');
    }
}

function UpdateLink(rowdata) {

    var facilityID = $('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityID');
    var token = $('input[name=__RequestVerificationToken]').val();

    if (confirm("Are you sure you want to delete the facility details?")) {
        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/DeleteFacilityDetails/' + facilityID,
            cache: false,
            async: true,
            traditional: true,
            data: { facilityid: facilityID, "__RequestVerificationToken": token },
            success: function (data) {
                alert(data.message);
                //LoadGrid();
                $('#tbPanchyatList').trigger("reloadGrid");
                //$('#tbPanchyatList').trigger('reloadGrid', { fromServer: true, page: 1 });
                //$('#tbPanchyatList').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        })
    }
    //console.log("gridLoaded");
}

function EditFacilityDetails(rowdata) {
    var facilityID = $('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityID');
    var token = $('input[name=__RequestVerificationToken]').val();

    if (confirm("Are you sure you want to edit the Facility details ?")) {
        $.ajax({
            type: 'GET',
            url: '/LocationMasterDataEntry/EditFacilityDetails/' + facilityID,
            cache: false,
            async: true,
            traditional: true,
            data: { facilityid: facilityID },
            success: function (data) {
                //$('#btnCreateNew').hide();
                $('#btnSearchView').show();
                $('#btnCreateNew').hide();
                $('#trAddNewSearch').show();
                $("#dvSearchPanchayat").hide();
                $('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
                $('#tbFacilityDesc').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityDesc'));
                $('#tbPincode').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'PinCode'));
                $("#dvPanchayatDetails").html(data);
                $('#tbFacilityDesc').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityDesc'));
                $('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
                $('#dvPanchayatDetails').show('slow');
                $('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
                $('#tbPincode').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'PinCode'));
                //console.log($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
                //$('#tbPanchyatList').trigger("reloadGrid");
                //alert(data.message);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        })
    }
}

function ViewFacilityDetails(rowdata) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    var facilityID = $('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityID');
    var token = $('input[name=__RequestVerificationToken]').val();
    $.ajax({
        type: 'GET',
        url: '/LocationMasterDataEntry/DisplayFacilityDetails/' + facilityID,
        cache: false,
        async: true,
        traditional: true,
        data: { facilityid: facilityID },
        success: function (data) {
            $('#divDisplayFacilityDetails').html(data);
            $('#divDisplayFacilityDetails').show("slow");
            //$('#btnCreateNew').hide();
            //$('#btnSearchView').show();
            //$('#btnCreateNew').hide();
            //$('#trAddNewSearch').show();
            //$("#dvSearchPanchayat").hide();
            //$('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
            //$('#tbFacilityDesc').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityDesc'));
            //$('#tbPincode').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'PinCode'));
            //$("#dvPanchayatDetails").html(data);
            //$('#tbFacilityDesc').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'FacilityDesc'));
            //$('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
            //$('#dvPanchayatDetails').show('slow');
            //$('#tbAddress').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
            //$('#tbPincode').val($('#tbPanchyatList').jqGrid('getCell', rowdata, 'PinCode'));
            ////console.log($('#tbPanchyatList').jqGrid('getCell', rowdata, 'Address'));
            ////$('#tbPanchyatList').trigger("reloadGrid");
            ////alert(data.message);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error occurred while processing the request.");
        }
    })
    $.unblockUI();
}

function LoadFacilityName() {
    $.ajax({
        url: "/LocationMasterDataEntry/GetFacilityNameSearch",
        data: { facilityCode: $('#ddlFacilitySearch option:selected').val() },
        type: 'POST',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occurred while processing your request.')
            return false;
        },
        success: function (data) {
            $('#ddlFacilityNameListSearch').empty();
            $.each(data, function () {
                $('#ddlFacilityNameListSearch').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                //console.log("appended");
            });
        }
    });
}


//function DeleteImageFromGrid(id) {
//    if (confirm("Are you sure to delete the photograph ? ")) {
//        var remark = prompt("Please enter the remark for photograph deletion");
//        if (remark != "" && remark != null)
//        {
//            $.ajax({
//                url: "/LocationMasterDataEntry/DeleteImageLatLong/" + id,
//                type: "POST",
//                cache: false,
//                data: {
//                    Remark: remark
//                },
//                beforeSend: function () {
//                    blockPage();
//                },
//                error: function (xhr, status, error) {
//                    unblockPage();
//                    Alert("Request can not be processed at this time,please try after some time!!!");
//                    return false;
//                },
//                success: function (response) {
//                    unblockPage();
//                    if (response.success) {
//                        $("#spnImageDeleteButton").hide();
//                        alert(response.message);
//                        $('#tbPanchyatList').trigger('reloadGrid');
//                    }
//                    else {
//                        alert(response.message);
//                    }
//                }
//            });
//        }
//        else {
//            return;
//        }
//    }
//    else {
//        return;
//    }
//}


// Added By Rohit on 04 Sept 2019
function finalizeFacility(id) {

    if (confirm("Are you sure to finalize this facility details ?")) {
        $.ajax({
            url: '/LocationMasterDataEntry/FinalizeFacility/' + id,
            type: "POST",
            cache: false,
            async: false,
            success: function (response) {
                if (response.Success) {
                    alert("Facility Details Finalized successfully");
                    //$('#divDisplayFacilityDetails').hide("slow");
                    //$('#dvPanchyatList').show();

                    LoadGrid();
                    $('#tbPanchyatList').trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#spnError").html('<strong>Alert : </strong>' + response.ErrorMessage);
                    $("#btnSearchNew").trigger("click");
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}


// definalizeFacility

function definalizeFacility(id) {

    if (confirm("Are you sure to finalize this facility details ?")) {
        $.ajax({
            url: '/LocationMasterDataEntry/DeFinalizeFacility/' + id,
            type: "POST",
            cache: false,
            async: false,
            success: function (response) {
                if (response.Success) {
                    alert("Facility Details Definalized successfully");
                    //$('#divDisplayFacilityDetails').hide("slow");
                    //$('#dvPanchyatList').show();

                    LoadGrid();
                    $('#tbPanchyatList').trigger("reloadGrid");
                }
                else {
                    $("#divError").show("slow");
                    $("#spnError").html('<strong>Alert : </strong>' + response.ErrorMessage);
                    $("#btnSearchNew").trigger("click");
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}



function UploadFacilityPhotoGraph(id) {

    $.ajax({
        url: '/LocationMasterDataEntry/UploadFacilityPhoto/' + id,
        type: "POST",
        cache: false,
        async: false,
        success: function (response) {
            $("#accordionMonitorsInspection div").html("");
            $("#accordionMonitorsInspection h3").html(
                    "<a href='#' style= 'font-size:15px;' >&nbsp;&nbsp;  Upload Photograph</a>" +

                    '<a href="#" style="float: right;">' +
                    '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
                    '<span style="float: right;"></span>'
                    );

            $("#divDisplayPhotographUploadView").html(response);
            $("#dvPanchyatList").hide();
            $('#accordionMonitorsInspection').show();
            $("#divDisplayPhotographUploadView").show("slow");
        },
        error: function () {

            $.unblockUI();
            alert("Error : " + error);
            return false;
        }
    });
}

function DeleteImageFromGrid(id) {
    if (confirm("         Are you sure to delete the photograph ? \n If yes, click OK and Enter the remark to continue. ")) {
        $.ajax({
            url: "/LocationMasterDataEntry/GetRemarkForm/" + id,
            type: "GET",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#IdRemarkForm").html("");
                $("#IdRemarkForm").html(response);
                $('#dvPanchyatList').hide('fast');
                $("#dvhdRemark").show("slow");
                $("#IdRemarkForm").show("slow");
            }
        });
    }
    else {
        return;
    }



}