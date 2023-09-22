/// <reference path="../jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmListDistrict'))
    var arrDistrict = [];

    if ($('#PMGSYScheme').val() == 2) {
        LoadMappedDistrictsDetailsList();
    };

    $('#DistrictCode').multiSelect({
        keepOrder: true,
        selectableHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Districts to Map</strong></div>",
        selectionHeader: "<div class='ui-widget-header ui-corner-top' style='text-align:center'><strong>Selected Districts</strong></div>",

        afterInit: function (values) {
            $('#DistrictCode').multiSelect('deselect_all');
        },
        afterSelect: function (values) {
            arrDistrict.push(values);
        },
        afterDeselect: function (values) {
            arrDistrict.pop(values);
        }
    });

    // map  District
    $('#btnSaveDistrict').click(function (e) {
        e.preventDefault();
        if (validate(arrDistrict)) {
            if (confirm("Are you sure you want to map District Details?")) {


                //$('#EncryptedHabCodes').val(blockCodes);
                $('#hiddenDistrictCode').val(arrDistrict);
                console.log(arrDistrict);
                blockPage();

                $.ajax({
                    url: "/Proposal/MapDistrict",
                    type: "POST",
                    dataType: "json",
                    data: $("#frmListDistrict").serialize(),
                    success: function (data) {
                        unblockPage();
                        if (data.success) {
                            alert(data.message);
                            $("#tblMappedDistricts").trigger('reloadGrid');

                            $('#dvDistrictMapping').load('/Proposal/DistrictMappingLayout', function () {
                                //$(this).fadeIn(5000);
                            });
                            //$("select.multiselect").multiselect("refresh");

                            //$('#DistrictCode').multiselect('destroy');
                            //initMultiSelect();
                            //$('#DistrictCode').multiselect().trigger('reset');
                        }
                        else {
                            alert(data.message);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        unblockPage();
                    }
                });
            }
            else {
                return false;
            }
        }
        else {
            alert('Please select Districts to Map.');
        }
    });
    function validate(arrHabitations) {
        //debugger;
        //arrDistricts.forEach(alert(elem));
        if (arrHabitations.length == 0) {
            $("#showDistrictError").html("Select at least one of the District");
            $("#showDistrictError").addClass("field-validation-error");
            return false;
        }
        else {
            return true;
        }
    }
});

function LoadMappedDistrictsDetailsList() {
    $("#tblMappedDistricts").jqGrid('GridUnload');
    $('#tblMappedDistricts').jqGrid({
        url: '/Proposal/ListMappedDistrictDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['District', 'District mapping Id', 'District Name', 'Delete'],
        colModel: [
                      { name: 'MAST_DISTRICT_CODE', index: 'MAST_DISTRICT_CODE', height: 'auto', width: 10, align: "center", sortable: true, hidden: true },
                      { name: 'MATRIX_DISTRICT_MAPPING_ID', index: 'MATRIX_DISTRICT_MAPPING_ID', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 70, align: "center", sortable: false },
                      //{ name: 'Delete', index: 'Delete', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'Delete', index: 'Delete', width: 10, sortable: false, align: "center", search: false },
        ],
        //postData: {   },
        pager: jQuery('#dvPagerMappedDistricts'),
        rowNum: 40,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        //loadonce: true,
    });
}

function DeleteMappedDistrictDetails(urlparameter) {
    //alert(urlparameter);
    if (confirm("Are you sure you want to delete Mapped District?")) {
        $.ajax({
            type: 'POST',
            url: '/Proposal/DeleteMappedDistricts/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $("#tblMappedDistricts").trigger('reloadGrid');
                    $('#dvDistrictMapping').load('/Proposal/DistrictMappingLayout', function () {
                        //$(this).fadeIn(5000);
                    });
                    //$('#DistrictCode').multiselect('destroy');
                    //$('#DistrictCode').multiselect('rebuild');
                    //$("select.multiselect").multiselect("refresh");

                    //$('#yourMultiselectId').multiselect('destroy');
                    //initMultiSelect();
                    //$('#DistrictCode').multiselect().trigger('reset');
                    //location.reload();
                }
                else if (data.success == false) {
                    alert(data.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

//function initMultiSelect() {

//    $('#DistrictCode').multiselect({
//        includeSelectAllOption: true,
//        selectAllValue: 'select-all-value'
//    });
//}