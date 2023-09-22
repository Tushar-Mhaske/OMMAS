var isContractorListLoaded = false;
$(document).ready(function () {

    
    if ($('#frmAdd') != null) {
        $.validator.unobtrusive.parse('#frmAdd');
    }
    if ($('#frmEdit') != null) {
        $.validator.unobtrusive.parse('#frmEdit');
    }
    if ($('#frmListContractor') != null) {
        $.validator.unobtrusive.parse('#frmEdit');
    }


    //triggerGrid();


    $("#State").change(function () {
        $("#contractorRegistration").jqGrid('GridUnload');
        var val = $("#State").val();
        $.ajax({
            type: 'POST',
            url: "/Master/getDistricts/",
            data: { id: val },
            async: false,
            success: function (data) {
                $("#District").empty();
                $.each(data, function () {
                    $("#District").append("<option value=" + this.Value + ">" +
                                                        this.Text + "</option>");
                });
            }
        })

        $.ajax({
            type: 'POST',
            url: "/Master/GetPanNumbers/" + val + "/0",
            data: { state: val },
            async: false,
            success: function (data) {
                $("#ddlPan").empty();
                $.each(data, function () {
                    $("#ddlPan").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                })
                $("#ddlPan").append("<option value=" + "0" + " selected>Select PAN Number</option>");
            }
        })
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
                //$("#mainDiv").html(data);
                //$.validator.unobtrusive.parse('#mainDiv');
                $("#dvMasterDataEntry").load("/Master/AddContractor");

                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            }

        })

    });

    $("#District").change(function () {
        $("#contractorRegistration").jqGrid('GridUnload');
        var distVal = $("#District").val();
        var stateVal = $("#State").val();
        var selectText = "Select District";
        var selectValue = "0";
        $.ajax({
            type: 'POST',
            url: "/Master/getPanNumbers/" + stateVal + "/" + distVal,
            data: { state_code: stateVal, dist_code: distVal },
            async: false,
            success: function (data) {
                $("#ddlPan").empty();

                $.each(data, function () {

                    $("#ddlPan").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");
                })
                $("#ddlPan").append("<option value=" + "0" + " selected>Select PAN Number</option>");
            }
        })

    });

    loadContractorList();

    

    $("#loadGrid").click(function (e) {
       // alert('test');
        //if ($("#State").val() == 0) {
        //    $('#message').html("Please select state.");
        //    $('#dvErrorMessage').show('slow');
            
        //}
        //else {
        //    $("#contractorRegistration").jqGrid('GridUnload');
            SearchDetails();
        // }

        //if (!isContractorListLoaded) {
        //    var data = $('#Contractor').jqGrid("getGridParam", "postData");
        //    data._search = true;
        //    data.searchField = $("#frmListContractor").serialize();
        //    $('#Contractor').jqGrid("setGridParam", { "postData": data });
        //    isContractorListLoaded = true;
        //}
        //else {
        //    //jQuery("#Contractor").jqGrid('setFrozenColumns');
            
        //}
        //loadContractorList();
    });
});

function triggerGrid() {
    $("#loadGrid").trigger("click");
}


function loadContractorList() {
    if (isContractorListLoaded) {
        $('#Contractor').GridUnload();
        isContractorListLoaded = false;
    }

    $('#Contractor').jqGrid({
        caption: 'Contractor List',
        datatype: 'json',
        mtype: 'POST',
        emptyrecords: 'No Records Found',
        footerrow: true,
        height: '100%',
        loadtext: 'Loading the Grid Please Wait...',
        pager: '#pager',
        pgbuttons: true,
        pginput: true,
        rowList: [30, 60, 90, 120],
        rowNum: 30,
        rownumbers: true,
        viewsortcols: [false, 'vertical', true],
        shrinkToFit: false,
        url: '/Master/GetList1/',
        viewrecords: true,
        width: '900',
        multipleSearch: true,
        loadComplete: function (e) {
            fixPositionsOfFrozenDivs.call(this);
            isContractorListLoaded = true;

        },
        postData:
            {
                searchField : $("#frmListContractor").serialize()//,filters:$("#Contractor").jqGrid("getGridParam", "postData")
            },
        colModel: [
         {
             align: 'left',
             name: 'CONTRACTOR_NAME',
             label: 'Contractor Name',
             width: 200,
             height: 'auto',
             index: 'CONTRACTOR_NAME',
             frozen: true
         }, {
             align: 'left',
             name: 'MAST_CON_PAN',
             label: 'PAN No.',
             height: 'auto',
             width: 70,
             frozen: true,
             index: 'MAST_CON_PAN'
         }, {
             align: 'left',
             name: 'MAST_CON_COMPANY_NAME',
             label: 'Company Name',
             width: 200,
             index: 'MAST_CON_COMPANY_NAME'
         }, {
             align: 'left',
             name: 'ADDR',
             label: 'Address',
             width: 250,
             index: 'ADDR'
         }, {
             align: 'center',
             name: 'MAST_DISTRICT_CODE',
             label: 'District',
             width: 80,
             index: 'MAST_DISTRICT_CODE'
         }, {
             align: 'center',
             name: 'MAST_STATE_CODE',
             label: 'State',
             width: 80,
             index: 'MAST_STATE_CODE'
         }, {
             align: 'center',
             name: 'MAST_CON_MOBILE',
             label: 'Mobile',
             width: 80,
             index: 'MAST_CON_MOBILE'
         }, {
             align: 'center',
             name: 'MAST_CON_EMAIL',
             label: 'Email',
             width: 120,
             index: 'MAST_CON_EMAIL'
         },
         {
             classes: 'ui-align-right',
             align: 'center',
             name: 'Edit',
             search: false,
             sortable: false,
             width: 40,
             index: 'Edit',
             formatter: FormatColumn
         },
         {
             classes: 'ui-align-right',
             align: 'center',
             name: 'Delete',
             search: false,
             sortable: false,
             width: 40,
             index: 'Delete',
             formatter: FormatColumn3
         },
         {
             classes: 'ui-align-right',
             align: 'center',
             name: 'Details',
             search: false,
             sortable: false,
             width: 80,
             index: 'Details',
             formatter: FormatColumn1
         },
         {
             classes: 'ui-align-right',
             align: 'center',
             name: 'Registration',
             search: false,
             sortable: false,
             width: 80,
             index: 'Registration',
             formatter: FormatColumn2
         }
        ]
    });
    jQuery("#Contractor").jqGrid('filterToolbar', {
        stringResult: true,
        searchOnEnter: true,
        defaultSearch: "cn",
        autosearch: true,   
        enableClear: true,
        beforeSearch: function () {
            $("#loadGrid").trigger('click');
        }
    });
    //jQuery("#Contractor").jqGrid('setFrozenColumns');
}


function SearchDetails() {
    $('#Contractor').setGridParam({
        url: '/Master/GetList1', datatype: 'json'
    });
   

    var data = $('#Contractor').jqGrid("getGridParam", "postData");
    //alert(data);
    data._search = true;
    delete data.searchField;
    data.searchField = $("#frmListContractor").serialize();
    $('#Contractor').jqGrid("setGridParam", { "postData": data });
    //jQuery("#Contractor").jqGrid('setFrozenColumns');
    $('#Contractor').trigger("reloadGrid", [{ page: 1 }]);
}



function FormatColumn(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Contractor Details' onClick='editData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function FormatColumn3(cellvalue, options, rowObject) {
    if (!(cellvalue.toString() == "")) {
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete     Contractor Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td style='border-color:white'></td></tr></table></center>";
    }
}


function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><table><tr><td style='border-color:white'><span title='Registration Details' onClick='detailsData(\""+cellvalue.toString() + "\");'>Details</span></td></tr></table></center>";

}

function FormatColumn2(cellvalue, options, rowObject) {

    return "<center><table><tr><td style='border-color:white'><span title='Registration Details' onClick='registerContractor(\"" + cellvalue.toString() + "\");'>Register</span></td></tr></table></center>";
}

function loadContractorRegGrid(urlparameter) {
    $("#contractorRegistration").jqGrid({
        caption: 'Contractor Registration List',
        datatype: 'json',
        mtype: 'POST',
        emptyrecords: 'No Records Found',
        footerrow: true,
        height: '100%',
        loadtext: 'Loading the Grid Please Wait...',
        pager: '#pager',
        pgbuttons: true,
        pginput: true,
        rowList: [5, 10, 15, 20],
        rowNum: 5,
        rownumbers: true,
        viewsortcols: [false, 'vertical', true],
        shrinkToFit: false,
        url: '/Master/DetailsRegistration/'+urlparameter,
        viewrecords: true,
        width: 'auto',
        multipleSearch: true,
        colModel: [
         {
             align:'center',
             name:'MAST_REG_CODE',
             key: true,
             label:'Registration No',
             width:150,
             index:'MAST_CON_ID'
         },
         {
             align: 'center',
             name: 'MAST_STATE_NAME',
             label: 'Registration State',
             width: 100,
             index: 'MAST_STATE_NAME'
         },
         
         {
             align:'center',
             name:'MAST_CON_VALID_FROM',
             label:'Valid From',
             width:100,
             index:'MAST_CON_VALID_FROM'
         },
         {
             align:'center',
             name:'MAST_CON_VALID_TO',
             label:'Valid To',
             width:100,
             index:'MAST_CON_VALID_TO'
         },
         
         {
             align: 'center',
             name: 'MAST_REG_OFFICE',
             label: 'Registration Office',
             width: 100,
             index: 'MAST_REG_OFFICE'
         },
         {
             align: 'center',
             name: 'MAST_REG_STATUS',
             label: 'Registration Status',
             width: 100,
             index: 'MAST_REG_STATUS'
         },
         {
             align: 'center',
             name: 'MAST_CON_CLASS_TYPE_NAME',
             label: 'Contrator Class',
             width: 100,
             index: 'MAST_CON_CLASS_TYPE_NAME'
         }
        ]
    }).navGrid("pager", { edit: false, add: false, del: false, search: true });;
    
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
            //$("#mainDiv").html(data);
            //$.validator.unobtrusive.parse('#mainDiv');
            //$("#dvMasterDataEntry").load("/Master/EditContractor"+urlparameter);
            $("#dvMasterDataEntry").html(data);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete contractor details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteContractor/' + urlparameter,
            dataType: 'html',
            async: false,
            cache: false,
            success: function (data) {
                //alert(data);
                //"success":true,"message":"Contractor deleted Successfully"}
                if (data) {
                    alert("Contractor deleted successfully");
                    $("#Contractor").trigger('reloadGrid');

                }
                else if(data.success == false){
                    alert("Contractor registration details exist.");
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


function detailsData(urlparameter) {
    $("#editMsg").hide();
    $("#addMsg").hide();
    jQuery("#contractorRegistration").jqGrid('GridUnload');
    loadContractorRegGrid(urlparameter);
}

function registerContractor(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/MasterContractorReg/'+urlparameter,
        //data: { id: cont_id },
        success: function (data) {
            //$("#mainDiv").html(data);
            $("#dvMasterDataEntry").html(data);
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
        }
    })

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

