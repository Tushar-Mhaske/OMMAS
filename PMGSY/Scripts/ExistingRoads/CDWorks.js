//$.validator.addMethod("comparevalidation", function (value, element, params) {

//    alert(parseInt($("#MAST_CONSTRUCT_YEAR").val()));
//    alert(parseInt($("#MAST_REHAB_YEAR").val()));
//    if (parseInt($("#MAST_CONSTRUCT_YEAR").val()) < parseInt($("#MAST_REHAB_YEAR").val())) {
//        return true;
//    }
//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("comparevalidation");
$(document).ready(function () {
    $.validator.unobtrusive.parse(('#frmCdWorks'));

//    $('input:visible:enabled:first').focus();
   // $('frmCdWorks:first *:input[type!=hidden]:first').focus();
  //  $("#MAST_CD_LENGTH").focus();
    //$("input:text:visible:first").focus();
    GetCdWorks($("#MAST_ER_ROAD_CODE").val());

    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }


    if ($("#Operation").val() == "A") {

        $(function () {
            $("#MAST_CD_LENGTH").focus();
        });
    }

    $("#btnReset").click(function () {

        //hide error alert
        if ($("#divError").is(":visible")) {
            $("#divError").hide("slow");
        }

        $(".pmgsy-textbox").removeClass('input-validation-error');
        $(".field-validation-error").html('');
        $("#MAST_CDWORKS_CODE").removeClass('input-validation-error');
        $("#MAST_CONSTRUCT_YEAR").removeClass('input-validation-error');
        $("#MAST_REHAB_YEAR").removeClass('input-validation-error');
        resetForm();
    });


    //allow only digits and .

    $("#MAST_CD_LENGTH").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CD_DISCHARGE").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CD_CHAINAGE").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#MAST_ER_SPAN").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });
    $("#MAST_CARRIAGE_WAY").keypress(function (e) {
        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });

    $("#MAST_REHAB_YEAR").change(function () {

       
        var RoadConstructionYear =$("#MAST_CONSTRUCT_YEAR option:selected").val();
        var RoadRehabilitationYear =$("#MAST_REHAB_YEAR option:selected").val();



        if (RoadConstructionYear != 0 && RoadConstructionYear >= RoadRehabilitationYear) {
            $("#roadRehabilitationYear").show("slow");
            $("#roadRehabilitationYear").html("<span style='color:red'><b>Road Rehabilitation Year must be greater than road construction year.</b></span>");
        }
    });

    $("#MAST_CONSTRUCT_YEAR").change(function () {

        var RoadConstructionYear = $("#MAST_CONSTRUCT_YEAR option:selected").val();
        var RoadRehabilitationYear = $("#MAST_REHAB_YEAR option:selected").val();

        if (RoadRehabilitationYear != 0 && (RoadConstructionYear >= RoadRehabilitationYear)) {
            $("#roadConstructionYear").show("slow");
            $("#roadConstructionYear").html("<span style='color:red'><b>Road Construction Year must be less than road rehabilitation year.</b></span>");
        }
    });

    $('#btnSave').click(function (evt) {                    
        //evt.preventDefault();
        if ($('#frmCdWorks').valid()) {
            //if (validateForm() == true) {

                $.ajax({
                    url: "/ExistingRoads/AddCdWorkDetails/",
                    type: "POST",
                    cache: false,
                    data: $("#frmCdWorks").serialize(),
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
                        if (response.success == true) {
                            alert(response.message);
                            $("#tbCdWorks").trigger('reloadGrid');
                            $("#frmCdWorks").trigger('reset');
                            $("#divError").hide("slow");
                            $("#divCdWorks").html('');
                            //$("#divCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {
                            //    $.validator.unobtrusive.parse("divCdWorks");
                            //});
                            LoadAddView();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                            unblockPage();
                        }
                    }
                });
            //}

        }
        else {
            
        }
            
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        $("#divError").hide("slow");
        if ($('#frmCdWorks').valid()) {
                $.ajax({
                url: "/ExistingRoads/EditCdWorkDetails/",
                type: "POST",
                cache: false,
                data: $("#frmCdWorks").serialize(),
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

                    $("#tbCdWorks").trigger('reloadGrid');

                    $("#frmCdWorks").trigger('reset');
                          
                    if (response.success == true) {
                        alert(response.message);
                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();
                        resetForm();
                        $("#divCdWorks").html('');
                        //$("#divCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {
                        //    $.validator.unobtrusive.parse("divCdWorks");
                        //});
                        LoadAddView();
                       
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }
                }
            });
        }
    });

    $('#btnCancel').click(function () {
        $("#Operation").val("A");
        $("#rowAdd").show();
        $("#rowUpdate").hide();
        $("#MAST_CDWORKS_CODE").attr('disabled', false);
        resetForm();

    });

});

//function GetCdWorks(MAST_ER_ROAD_CODE) {
//    jQuery("#tbCdWorks").jqGrid({
//        url: '/ExistingRoads/GetCdWorksList/',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ['CD Works Type', 'CD Works Length', "CD Works Discharge", "CD Works Chainage", "Construction Year", "Rehabilitation Year", "Span", "Carriage Way", "Foot Path", "Edit", "Delete"],
//        colModel: [ 
//                    { name: 'CDWorksType', index: 'CDWorksType', width: 160, sortable: true, align: "left" },
//                    { name: 'CDWorksLength', index: 'CDWorksLength', width: 100, sortable: true, align: "center" },
//                    { name: 'CDWorksDischarge', index: 'CDWorksDischarge', width: 100, sortable: true, align: "center" },
//                    { name: 'CDWorksChainage', index: 'CDWorksChainage', width: 100, sortable: true, align: "center" },
//                    { name: 'ConstructionYear', index: 'ConstructionYear', width: 100, sortable: true, align: "center" },
//                    { name: 'RehabilitationYear', index: 'RehabilitationYear', width: 100, sortable: true, align: "center" },
//                    { name: 'Span', index: 'CDWorksChainage', width: 50, sortable: true, align: "center" },
//                    { name: 'CarriageWay', index: 'CarriageWay', width: 50, sortable: true, align: "center" },
//                    { name: 'FootPath', index: 'FootPath', width: 70, sortable: true, align: "center" },
//                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnCdWorksEdit },
//                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnCdWorksDelete }
//        ],
//        pager: jQuery('#dvCdWorksPager'),
//        rowNum: 8,
//        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
//        viewrecords: true,
//        sortname: 'CDWorksType',
//        sortorder:'asc',
//        recordtext: '{2} records found',
//        caption: "CD Works List",
//        height: 'auto',
//        width:'100%',
//        //autowidth: true,
//        sutowidth: true,
//        rownumbers: true,
//        loadError: function (xhr, ststus, error) {
//            if (xhr.responseText == "session expired") {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else {
//                // alert(xhr.responseText);
//                // alert("Invalid data.Please check and Try again!")
//                //  window.location.href = "/Login/LogIn";
//            }
//        },
//        loadComplete: function () {
//            $("#gview_tbCdWorks > .ui-jqgrid-titlebar").hide();
//        }
        
//    });

//}


//function EditCdWorks(key) {
  
//    $("#divCdWorks").html("");

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#divExistingRoadsForm").load('/ExistingRoads/EditCdWorkDetails/' + key, function () {
//            $.validator.unobtrusive.parse($('#frmCdWorks'));

//            if ($("#Operation").val() == "U") {
//                $("#MAST_CD_LENGTH").focus();
//            }
//            unblockPage();
//        });        
//    });

//}

//function DeleteCdWork(key) {

//    if (confirm("Are you sure you want to delete the CD Works details ? ")) {

//        $.ajax({
//            url: "/ExistingRoads/DeleteCDWorksDetails/" + key,
//            type: "POST",
//            cache: false,
//            beforeSend: function () {
//                blockPage();
//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            },
//            success: function (response) {
//                unblockPage();
//                $("#tbCdWorks").trigger('reloadGrid');
              
//                if ($("#Operation").val() == "U") {
//                    $("#btnCancel").trigger('click');
//                }
   

//                if (response.success) {
//                    alert(response.message);
//                    $("#tbExistingRoadsList").trigger('reloadGrid');
//                } else {
//                    alert(response.message)         
//                }
               
//            }
//        });

//    }
//    else {
//        return;
//    }
//}

function validateForm()
{
    var RoadConstructionYear = $("#MAST_CONSTRUCT_YEAR option:selected").val();
    var RoadRehabilitationYear = $("#MAST_REHAB_YEAR option:selected").val();

    if (RoadConstructionYear != 0 && RoadConstructionYear >= RoadRehabilitationYear) {
        $("#roadRehabilitationYear").show("slow");
        $("#roadRehabilitationYear").html("<span style='color:red'><b>Road Rehabilitation Year must be greater than road construction year.</b></span>");
        return false;
    }
    else {
        $("#roadRehabilitationYear").html('');
        return true;
    }
}

//function FormatColumnCdWorksEdit(cellvalue, options, rowObject) {

//    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the CD Works Details' onClick ='EditCdWorks(\"" + cellvalue.toString() + "\");'></span></center>";
//}

//function FormatColumnCdWorksDelete(cellvalue, options, rowObject) {

//    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the CD Works Details' onClick ='DeleteCdWork(\"" + cellvalue.toString() + "\");'></span></center>";
//}

function resetForm()
{   
    $("#MAST_CDWORKS_CODE").val("");
    $("#MAST_CD_LENGTH").val("");
    $("#MAST_CD_DISCHARGE").val("");
    $("#MAST_CD_CHAINAGE").val("");
    $("#MAST_CONSTRUCT_YEAR").val("");
    $("#MAST_REHAB_YEAR").val("");
    $("#MAST_ER_SPAN").val("");
    $("#MAST_CARRIAGE_WAY").val("");
    $("#roadRehabilitationYear").html('');
    $("#roadConstructionYear").html('');

}