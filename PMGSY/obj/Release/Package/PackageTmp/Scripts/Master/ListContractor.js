var isContractorListLoaded = false;
$(document).ready(function () {

    if ($('#frmAdd') != null) {
        $.validator.unobtrusive.parse('#frmAdd');
    }

    if ($('#frmListContractor') != null) {
        $.validator.unobtrusive.parse('#frmListContractor');
    }

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#dvhdCreateNewContRegDetails").hide();

    if ($("#registrationDetails").is(":visible")) {
        $("#registrationDetails").hide();
    }

    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });


    $("#btnAddNew").click(function () {
        $("#contractorRegistration").jqGrid('GridUnload');
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/Master/AddContractor/',
            dataType: "html",
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterDataEntry").load("/Master/AddContractor");
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            }

        })
    });
    loadContractorList();
    $("#loadGrid").click(function (e) {
        if (CheckFilterValidation() == true) {
            SearchDetails();
            $('#contractorRegistration').GridUnload();
        }
    });


    $("#btnAddNewReg").click(function () {


        if (!$("#dvContRegDetails").is(":visible")) {

            $("#dvContRegDetails").load("/Master/AddEditMasterContractorReg");
            $("#dvContRegDetails").show('slow');
        }
        $("#dvhdCreateNewContRegDetails").hide();
    });

    $("#btnRegCancel").click(function () {
        if (($("#registrationDetails").is(":visible"))) {
            $("#registrationDetails").hide('slow');
        }
        $('#Contractor').jqGrid("setGridState", "visible");

        $("#btnAddNew").show('slow');

        if (!($("#searchContractor").is(":visible"))) {
            $("#searchContractor").show('slow');
        }

        $("#mainDiv").animate({
            scrollTop: 0
        });
    });
    $("#State").change(function () {
      LoadPANNumber();
    });
    $("#State").trigger('change');
    /*
        //$("#State").change(function () {
    
        //    var message = '';
        //    message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
        //    $.blockUI({ message: message });
        //    var val = $("#State").val();
        //    $.ajax({
        //        type: 'POST',
        //        url: "/Master/getDistricts/",
        //        data: { id: val },
        //        async: false,
        //        success: function (data) {
        //            $("#District").empty();
        //            $.each(data, function () {
        //                $("#District").append("<option value=" + this.Value + ">" +
        //                                                    this.Text + "</option>");
        //            });
        //            $.unblockUI();
        //        }
        //    })
    
        //    $.ajax({
        //        type: 'POST',
        //        url: "/Master/GetPanNumbers/" + val + "/0",
        //        data: { state: val },
        //        async: false,
        //        success: function (data) {
        //            $("#ddlPan").empty();
        //            $.each(data, function () {
        //                $("#ddlPan").append("<option value=" + this.Value + ">" +
        //                                                        this.Text + "</option>");
        //            })
        //            $("#ddlPan").append("<option value=" + "0" + " selected>Select PAN Number</option>");
        //        }
        //    })
        //});
    
    
    
        //$("#District").change(function () {
        //    $("#contractorRegistration").jqGrid('GridUnload');
        //    var distVal = $("#District").val();
        //    var stateVal = $("#State").val();
        //    var selectText = "Select District";
        //    var selectValue = "0";
        //    $.ajax({
        //        type: 'POST',
        //        url: "/Master/getPanNumbers/" + stateVal + "/" + distVal,
        //        data: { state_code: stateVal, dist_code: distVal },
        //        async: false,
        //        success: function (data) {
        //            $("#ddlPan").empty();
    
        //            $.each(data, function () {
    
        //                $("#ddlPan").append("<option value=" + this.Value + ">" +
        //                                                        this.Text + "</option>");
        //            })
        //            $("#ddlPan").append("<option value=" + "0" + " selected>Select PAN Number</option>");
        //        }
        //    })
        //});
        */

});

function triggerGrid() {
    $("#loadGrid").trigger("click");
}
function CheckFilterValidation() {
    if ($('#State').val() == 0) {
        if ($('#txtPAN').val() == "" && $('#txtContractor').val() == "") {
            ///Changed by SAMMED A. PATIL on 08MAY2017 to provide contractor/supplier screen at MORD
            if ($('#Status').val() == 'B' || $('#Status').val() == 'E')
            {
                return true;
            }
            if ($('#txtPAN').val() == "" ) {
                //alert("Please Enter  PAN / TAN Or Contractor / Supplier Name");
                alert("Either 'Enter PAN/TAN Or Contractor/Supplier Name' Or 'Select a particular State'");
                return false;
            }
            if ($('#txtContractor').val() == "") {
                alert("Please Enter  Contractor / Supplier Name");
                return false;
            }
           
        }
      
        return true;
    }
    
    return true;
}

function loadContractorList() {
    //alert("H")
    if (CheckFilterValidation()==true) {
        if (isContractorListLoaded) {
            $('#Contractor').GridUnload();
            isContractorListLoaded = false;
        }

        $('#Contractor').jqGrid({
            caption: 'Contractor / Supplier List.',
            datatype: 'json',
            mtype: 'POST',
            //footerrow: true,
            height: '100%',
            pager: '#pager',
            pgbuttons: true,
            pginput: true,
            rowList: [30, 60, 90, 120],
            rowNum: 30,
            rownumbers: true,
            viewsortcols: [false, 'vertical', true],
            //  shrinkToFit: false,
            url: '/Master/GetList1/',
            viewrecords: true,
            sortname: 'CONTRACTOR_NAME',
            sortorder: "asc",
            autowidth: true,
            multipleSearch: true,
            loadComplete: function (e) {
                //fixPositionsOfFrozenDivs.call(this);
                isContractorListLoaded = true;
            },
            postData:
                {
                    searchField: $("#frmListContractor").serialize()
                },
            colModel: [
             {
                 align: 'left',
                 name: 'CONTRACTOR_NAME',
                 label: 'Contractor/Supplier Name',
                 width: 150,
                 height: 'auto',
                 index: 'CONTRACTOR_NAME',
                 frozen: false,
                 sortable: true
             },
              {
                  align: 'left',
                  name: 'CONTRACTOR_Status',
                  label: 'Contractor/Supplier Status',
                  width: 150,
                  height: 'auto',
                  index: 'CONTRACTOR_Status',
                  frozen: false,
                  sortable: true
              },
             {
                 align: 'left',
                 name: 'MAST_CON_PAN',
                 label: 'PAN / TAN',
                 height: 'auto',
                 width: 80,
                 frozen: false,
                 index: 'MAST_CON_PAN',
                 sortable: true
             }, {
                 align: 'left',
                 name: 'MAST_CON_COMPANY_NAME',
                 label: 'Company Name',
                 width: 150,
                 index: 'MAST_CON_COMPANY_NAME',
                 sortable: true

             },

             {
                 align: 'left',
                 name: 'MAST_DISTRICT_CODE',
                 label: 'District',
                 width: 80,
                 index: 'MAST_DISTRICT_CODE',
                 sortable: true
             }, {
                 align: 'left',
                 name: 'MAST_STATE_CODE',
                 label: 'State',
                 width: 130,
                 index: 'MAST_STATE_CODE',
                 sortable: true
             }, {
                 align: 'left',
                 name: 'MAST_CON_MOBILE',
                 label: 'Mobile',
                 width: 80,
                 index: 'MAST_CON_MOBILE',
                 sortable: true
             }, {
                 align: 'left',
                 name: 'MAST_CON_EMAIL',
                 label: 'Email',
                 width: 110,
                 index: 'MAST_CON_EMAIL',
                 sortable: true
             },

             {
                 classes: 'ui-align-right',
                 align: 'center',
                 name: 'Registration',
                 search: false,
                 sortable: false,
                 width: 80,
                 index: 'Registration',
                 formatter: FormatColumn2,
                 hidden: true

             },
             {
                 classes: 'ui-align-right',
                 align: 'center',
                 name: 'BankDetails',
                 search: false,
                 sortable: false,
                 width: 80,
                 index: 'BankDetails',
                 label: 'Bank Details',
                 formatter: FormatColumn_BankDetails,
                 hidden: true
             }

             ,
             {
                 classes: 'ui-align-right',
                 align: 'center',
                 name: 'Edit',
                 search: false,
                 sortable: false,
                 width: 35,
                 index: 'Edit',
                 formatter: FormatColumn
             },
             {
                 classes: 'ui-align-right',
                 align: 'center',
                 name: 'Delete',
                 search: false,
                 sortable: false,
                 width: 35,
                 index: 'Delete',
                 formatter: FormatColumn3
             },
             {
                 classes: 'ui-align-right',
                 align: 'center',
                 name: 'View',
                 search: false,
                 sortable: false,
                 width: 35,
                 index: 'View',
                 formatter: FormatColumn4
             }
            ]
        });

        jQuery("#Contractor").jqGrid('setFrozenColumns');
    }
}
function SearchDetails() {
   
        $('#Contractor').setGridParam({
            url: '/Master/GetList1', datatype: 'json'
        });

        var data = $('#Contractor').jqGrid("getGridParam", "postData");
        data._search = true;
        delete data.searchField;
        data.searchField = $("#frmListContractor").serialize();
        $('#Contractor').jqGrid("setGridParam", { "postData": data });
        $('#Contractor').trigger("reloadGrid", [{ page: 1 }]);
    
}
function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue != '') {
        return "<center><span class='ui-icon ui-icon-pencil' title='Edit Contractor/Supplier Details' onClick='editData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-locked' title='Locked' ></span></center>";
    }
}
function FormatColumn1(cellvalue, options, rowObject) {
    if ((cellvalue.toString() != "")) {
        return "<center><span title='Registration Details' onClick='detailsData(\"" + cellvalue.toString() + "\");'>View Details</span></center>";

    }
    else {
        return "<center>Not Exist</center>";
    }
}
function FormatColumn2(cellvalue, options, rowObject) {
    return "<center><a href='#' title='Registration Details' onClick='registerContractor(\"" + cellvalue.toString() + "\");'>Register</a></center>";
}
function FormatColumn3(cellvalue, options, rowObject) {
    if ((cellvalue.toString() == "")) {
        return "<center><span>-</span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-trash' title='Delete Contractor/Supplier Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
function FormatColumn4(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-zoomin' title='View Contractor/Supplier Details' onClick ='viewData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function FormatColumn_BankDetails(cellvalue, options, rowObject) {
    return "<center><a href='#' title='Bank Details' onClick='BankDetails(\"" + cellvalue.toString() + "\");'>Bank Details</a></center>";
}

function loadContractorRegGrid(urlparameter) {

    $("#contractorRegistration").jqGrid({
        caption: 'Contractor Registration List',
        datatype: 'json',
        mtype: 'POST',
        emptyrecords: 'No Records Found',
        height: '100%',
        pager: '#pager1',
        pgbuttons: true,
        pginput: true,
        rowList: [5, 10, 15, 20],
        rowNum: 5,
        rownumbers: true,
        //viewsortcols: [false, 'vertical', true],
        //shrinkToFit: false,
        url: '/Master/DetailsRegistration/' + urlparameter,
        viewrecords: true,
        autowidth: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        multipleSearch: true,
        loadComplete: function () {
            var count = $("#contractorRegistration").getGridParam('reccount');
            if (count == 0) {

                alert("No details found");
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $("#contractorRegistration").GridUnload();
                $.unblockUI();
            }
        },
        colModel: [
         {
             align: 'center',
             name: 'MAST_REG_CODE',
             key: true,
             label: 'Registration No',
             width: 150,
             index: 'MAST_CON_ID'
         },
         {
             align: 'center',
             name: 'MAST_STATE_NAME',
             label: 'Registration State',
             width: 100,
             index: 'MAST_STATE_NAME'
         },
         {
             align: 'center',
             name: 'MAST_CON_VALID_FROM',
             label: 'Valid From',
             width: 100,
             index: 'MAST_CON_VALID_FROM'
         },
         {
             align: 'center',
             name: 'MAST_CON_VALID_TO',
             label: 'Valid To',
             width: 100,
             index: 'MAST_CON_VALID_TO'
         },
         {
             align: 'center',
             name: 'MAST_REG_OFFICE',
             label: 'Registration Office',
             width: 150,
             index: 'MAST_REG_OFFICE'
         },
         {
             align: 'center',
             name: 'MAST_REG_STATUS',
             label: 'Registration Status',
             width: 120,
             index: 'MAST_REG_STATUS'
         },
         {
             align: 'center',
             name: 'MAST_CON_CLASS_TYPE_NAME',
             label: 'Contrator Class',
             width: 125,
             index: 'MAST_CON_CLASS_TYPE_NAME'
         }
        ]
    });//.navGrid("pager1", { edit: false, add: false, del: false, search: true });;

}

function editData(urlparameter) {
    $("#contractorRegistration").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditContractor/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $("#dvMasterDataEntry").html(data);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function viewData(urlparameter) {
    $("#contractorRegistration").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/ViewContractor/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $("#dvMasterDataEntry").html(data);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}
function deleteData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (confirm("Are you sure you want to delete Contractor/Supplier details?")) {
    
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteContractor/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();
                if (data.success == "true") {
                    alert("Contractor/Supplier details deleted successfully.");
                    //alert(data.message);
                    $("#Contractor").trigger('reloadGrid');
                }
                else {
                    alert("You can not delete this Contractor/Supplier details.");
                    //alert(data.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();

                alert(xhr.responseText);
            }
        });
    }
    else {
        $.unblockUI();

        return false;
    }

}
function detailsData(urlparameter) {
    jQuery("#contractorRegistration").jqGrid('GridUnload');
    loadContractorRegGrid(urlparameter);
}

function registerContractor(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#searchContractor").hide('slow');


    $('#Contractor').jqGrid("setGridState", "hidden");


    if (($("#dvContRegDetails").is(":visible"))) {
        $("#dvContRegDetails").hide();
    }

    if (($("#dvlstBankDetails").is(":visible"))) {
        $("#dvlstBankDetails").hide();
    }

    $("#regDetails").load("/Master/MasterContractorReg/" + urlparameter);
    $("#registrationDetails").show();
    $("#btnAddNew").hide('slow');
    $.unblockUI();
}

var fixPositionsOfFrozenDivs = function () {
    var $rows;
    if (typeof this.grid.fbDiv !== "undefined") {
        $rows = $('>div>table.ui-jqgrid-btable>tbody>tr', this.grid.bDiv);
        $('>table.ui-jqgrid-btable>tbody>tr', this.grid.fbDiv).each(function (i) {
            var rowHight = $($rows[i]).height(), rowHightFrozen = $(this).height();
            if ($(this).hasClass("jqgrow")) {
                $(this).height(rowHight);
                rowHightFrozen = $(this).height();
                if (rowHight !== rowHightFrozen) {
                    $(this).height(rowHight + (rowHight - rowHightFrozen));
                }
            }
        });
        $(this.grid.fbDiv).height(this.grid.bDiv.clientHeight);
        $(this.grid.fbDiv).css($(this.grid.bDiv).position());
    }
    if (typeof this.grid.fhDiv !== "undefined") {
        $rows = $('>div>table.ui-jqgrid-htable>thead>tr', this.grid.hDiv);
        $('>table.ui-jqgrid-htable>thead>tr', this.grid.fhDiv).each(function (i) {
            var rowHight = $($rows[i]).height(), rowHightFrozen = $(this).height();
            $(this).height(rowHight);
            rowHightFrozen = $(this).height();
            if (rowHight !== rowHightFrozen) {
                $(this).height(rowHight + (rowHight - rowHightFrozen));
            }
        });
        $(this.grid.fhDiv).height(this.grid.hDiv.clientHeight);
        $(this.grid.fhDiv).css($(this.grid.hDiv).position());
    }
};


function BankDetails(id) {
    $('dvlstBankDetails').empty();

    $("#dvlstBankDetails").load("/Master/ListBankDetails?id=" + id, function () {
        $("#dvlstBankDetails").show('slow');

        if ($('#registrationDetails').is(":visible")) {
            $('#registrationDetails').hide();

        }
        if ($("#searchContractor").is(":visible")) {
            $("#searchContractor").hide('show');
            $("#btnAddNew").hide();
        }

        if (($("#dvContRegDetails").is(":visible"))) {
            $("#dvContRegDetails").hide();
        }

        $('#Contractor').jqGrid("setGridState", "hidden");


    });

}


function LoadPANNumber() {
    
    $("#txtPAN").val('');
    var stateVal = $("#State").val();
 
    $.ajax({
        url: "/Master/GetStatePanNumbers/" + stateVal,
        cache: false,
        type: "POST",
        async: false,
        success: function (data) {

            var rows = new Array();
            for (var i = 0; i < data.length; i++) {

                rows[i] = { data: data[i].Text, value: data[i].Text, id: data[i].Value };
            }

            $('#txtPAN').autocomplete({
                source: rows,
                dataType: 'json',
                formatItem: function (row, i, n) {
                    return row.Text;
                },
                width: 150,
                highlight: true,
                minChars: 3,
                selectFirst: true,
                max: 10,
                scroll: true,
                width: 100,
                maxItemsToShow: 10,
                maxCacheLength: 10,
                mustMatch: true
            })

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert("An error occurred while executing this request.\n" + xhr.responseText);
            if (xhr.responseText == "session expired") {
                //$('#frmECApplication').submit();
                alert(xhr.responseText);
                window.location.href = "/Login/LogIn";
            }
        }
    })


}
