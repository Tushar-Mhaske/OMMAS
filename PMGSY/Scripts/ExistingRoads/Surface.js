

//$.validator.addMethod("compareyear", function (value, element, params) {

//    if (($("#MAST_CONS_YEAR").val()) < ($("#MAST_RENEW_YEAR").val())) {
//        if ($("#MAST_ROAD_CAT_CODE").val() == 6) {
//            return true;
//        }
//        return true;
//    }
//    return false;
//});
//jQuery.validator.unobtrusive.adapters.addBool("compareyear");


//$.validator.addMethod("comparechainagevalidation", function (value, element, params) {

    


//});

//jQuery.validator.unobtrusive.adapters.addBool("comparechainagevalidation");




$(document).ready(function () {

    $.validator.unobtrusive.parse(('#frmSurfaceType'));

    GetSurfaceList($("#MAST_ER_ROAD_CODE").val());

    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }


    if ($("#Operation").val() == "A") {

        $("#spnStartChainnage").html($("#MAST_ER_STR_CHAIN").val());
        $(function () {
            $("#MAST_ER_END_CHAIN").focus();
        });
    }


    $("#btnReset").click(function () {
        //hide error alert
        if ($("#divError").is(":visible")) {
            $("#divError").hide("slow");
        }
        $("#spnSurfaceLEngth").html('');
        $("#errMsgEndChain").html('');
        //$("#errMsgEndChain").hide();
    });


    //allow only digits and .

    $("#MAST_ER_END_CHAIN").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $("#MAST_ER_END_CHAIN").blur(function () {

        var startChainage = parseFloat($("#MAST_ER_STR_CHAIN").val()).toFixed(3);
        var endChainage = parseFloat($("#MAST_ER_END_CHAIN").val()).toFixed(3);

        var SumOfAllSurfaceLength = parseFloat($("#SumOfAllSurfaceLength").val()).toFixed(3);

        var StartChainageOfRoad = parseFloat($("#StartChainageOfRoad").val()).toFixed(3);

        if ($("#Operation").val() == "A") {

            if ($("#MAST_ER_END_CHAIN").val() == "") {
                $("#spnSurfaceLEngth").html(0);
            } else {
                var surfaceLength = (parseFloat($("#MAST_ER_END_CHAIN").val()).toFixed(3) - parseFloat($("#MAST_ER_STR_CHAIN").val()).toFixed(3)).toFixed(3);
                $("#spnSurfaceLEngth").html(surfaceLength);
                $("#MAST_ER_SURFACE_LENGTH").val(parseFloat(surfaceLength).toFixed(3));
            }
        }

        $("#Remaining_Length").val(parseFloat($("#spanRemainingLength").html()).toFixed(3));

        if (!isNaN(endChainage) && !isNaN(startChainage)) {

            if (parseFloat($("#spanRemainingLength").html()) == 0) {
                $("#errMsgEndChain").show("slow");
                $("#errMsgEndChain").html("<span style='color:red'><b>Remaining Length is zero,surface details can not be added.</b></span>");
                return false;
            } else {
                
                if (parseFloat(parseFloat(endChainage).toFixed(3)) <= parseFloat(parseFloat(startChainage).toFixed(3))) {
                    
                    $("#errMsgEndChain").show("slow");
                    $("#errMsgEndChain").html("<span style='color:red'><b>End Chainage should be greater than Start Chainage.</b></span>");
                    return false;
                }

                var SurfaceLEngth = parseFloat($("#spnSurfaceLEngth").html()).toFixed(3);
                var RemaininigLength = parseFloat($("#spanRemainingLength").html()).toFixed(3);
               
                if (SurfaceLEngth > RemaininigLength) {
                    $("#errMsgEndChain").show("slow");
                    $("#errMsgEndChain").html("<span style='color:red'><b>Surface Length(" + SurfaceLEngth + ") exceeds the remaining length (" + RemaininigLength + ") please check end chainage</b></span>");
                    return false;
                }
            }
        }

    });



    $('#btnSave').click(function (evt) {
        evt.preventDefault();

       
        if ($('#frmSurfaceType').valid()) {
            if (validateForm() == true) {                            
                $.ajax({
                    url: "/ExistingRoads/AddSurfaceDetails/",
                    type: "POST",
                    cache: false,
                    data: $("#frmSurfaceType").serialize(),
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
                    
                        if (response.Success) {
                            $("#tbSurfaceType").trigger('reloadGrid');
                            $("#frmSurfaceType").trigger('reset');
                            $("#spnSurfaceLEngth").html('');
                         
                            DisplaySurfaceStartChainage($("#MAST_ER_ROAD_CODE").val());
                            
                            $("#spanRemainingLength").html(response.RemainingLength);//update label(span)
                            $("#Remaining_Length").val(response.RemainingLength);
                            $(".spanPavementLength").html(response.SurfaceLengthEntered);
                            $("#SurfaceLenghEntered").val(parseFloat($(".spanPavementLength").html()).toFixed(3));

                            alert(response.message);
                            $("#divError").hide('slow');
                            //$("#divAddSurface").load('/ExistingRoads/SurfaceAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {

                            //    $.validator.unobtrusive.parse("divAddCdWorks");

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
            }

        }
        else {
            
        }
            
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();
    
        if ($('#frmSurfaceType').valid()) {
            $.ajax({
                url: "/ExistingRoads/EditSurfaceDetails/",
                type: "POST",
                cache: false,
                data: $("#frmSurfaceType").serialize(),
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
                    $("#tbSurfaceType").trigger('reloadGrid');
                    if (response.success == true) {
                        alert(response.message);
                        $("#divError").hide('slow');
                        //$("#divAddSurface").load('/ExistingRoads/SurfaceAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {

                        //    $.validator.unobtrusive.parse("divAddCdWorks");

                        //});
                        LoadAddView();
                    } else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                    }

                }
            });

            var urlparamater = $("#MAST_ER_ROAD_CODE").val() + "$" + $("#MAST_SURFACE_SEG_NO").val();

            //$('#accordion').show('fold', function () {

            //    $("#divExistingRoadsForm").load('/ExistingRoads/ShowAddSurfaceDetails/' + urlparamater, function () {
            //        $.validator.unobtrusive.parse($('#frmSurfaceType'));

            //    });
            //    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
            //    $('#divExistingRoadsForm').show('slow');
            //    $("#divExistingRoadsForm").css('height', 'auto');
            //});
            unblockPage();
        }
    });

    $('#btnCancel').click(function () {

        //var MAST_ER_ROAD_CODE = $("#MAST_ER_ROAD_CODE").val();

        //$("#accordion div").html("");

        //$("#accordion h3").html(
        //        "<a href='#' style= 'font-size:.9em;' >Surface Details</a>" +
        //        '<a href="#" style="float: right;">' +
        //        '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
        //        );

        //$('#accordion').show('fold', function () {
        //    blockPage();

        //    $("#divExistingRoadsForm").load('/ExistingRoads/SurfaceCancel/' + MAST_ER_ROAD_CODE, function () {

        //        $.validator.unobtrusive.parse($('#frmSurfaceType'));
        //        unblockPage();
        //    });
        //    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        //    $('#divExistingRoadsForm').show('slow');
        //    $("#divExistingRoadsForm").css('height', 'auto');
        //});

        LoadAddView();

    });
});

//function GetSurfaceList(MAST_ER_ROAD_CODE) {
//    //alert(MAST_ER_ROAD_CODE);

//    jQuery("#tbSurfaceType").jqGrid({
//        url: '/ExistingRoads/GetSurfaceTypeList/',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ['Surface Type', 'Start Chainage(in Kms.)', 'End Chainage(in Kms.)', "Road Condition", "Length", "Edit", "Delete"],
//        colModel: [
//                    { name: 'SurfaceName', index: 'SurfaceName', width: '180%', sortable: true, align: "left" },
//                    { name: 'StartChainage', index: 'StartChainage', width: '180%', sortable: true, align: "center" },
//                    { name: 'EndChainage', index: 'EndChainage', width: '200%', sortable: true, align: "center" },
//                    { name: 'SurfaceCondition', index: 'SurfaceCondition', width: '150%', sortable: true, align: "left" },
////                  { name: 'SurfaceCondition', index: 'SurfaceCondition', width: 200, sortable: false, align: "center", formatter: 'number', summaryType: 'sum' },
//                    { name: 'SurfaceLength', index: 'SurfaceLength', width: '150%', sortable: true, align: "center", formatter: "number", summaryType: 'sum' },
//                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" ,formatter: FormatColumnSurfaceEdit },//, formatter: FormatColumnSurfaceEdit 
//                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnSurfaceDelete }//, formatter: FormatColumnSurfaceDelete
//        ],
//        pager: jQuery('#dvSurfaceTypePager'),
//        rowNum: 8,
//        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: "Surface List",
//        sortname: 'EndChainage',
//        sortorder:'asc',
//        height: 'auto',
//        width: '100%',
//        //autowidth:true,
//        sutowidth: true,
//        rownumbers: true,
//        //footerrow: true,
//        //userDataOnFooter: true,
//        loadComplete: function () {
//            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SurfaceLength', false);
//            var RoadLength = 0;
//            for (i = 0 ; i < RoadLengthColumn.length; i++) {

//                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
//            }
//        },
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
//            $("#gview_tbSurfaceType > .ui-jqgrid-titlebar").hide();
//        }
//    });

//}


//function EditSurface(key) {
  
//    $("#divSurfaceType").html("");

//    $('#accordion').show('fold', function () {
//        blockPage();
//        $("#divExistingRoadsForm").load('/ExistingRoads/EditSurfaceDetails/' + key, function () {
//            $.validator.unobtrusive.parse($('#frmSurfaceType'));

//            unblockPage();
//        });        
//    });

//}

//function DeleteSurface(key) {

//    if (confirm("Are you sure you want to delete the Surface details ? ")) {

//        $.ajax({
//            url: "/ExistingRoads/DeleteSurfaceDetails/" + key,
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
               
//                if (response.success) {
                   
//                    $("#tbSurfaceType").trigger('reloadGrid');
//                    DisplaySurfaceStartChainage($("#MAST_ER_ROAD_CODE").val());
//                    $("#spanRemainingLength").html(response.RemainingLength);//update label
//                    $("#Remaining_Length").val(response.RemainingLength);

//                    $(".spanPavementLength").html(response.SurfaceLengthEntered);
//                    $("#SurfaceLenghEntered").val(parseFloat($(".spanPavementLength").html()));

//                    alert("Surface Details Deleted Succesfully.");
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
    var startChainage = parseFloat($("#MAST_ER_STR_CHAIN").val()).toFixed(3);
    var endChainage = parseFloat($("#MAST_ER_END_CHAIN").val()).toFixed(3);

    var SumOfAllSurfaceLength = parseFloat($("#SumOfAllSurfaceLength").val()).toFixed(3);

    var StartChainageOfRoad = parseFloat($("#StartChainageOfRoad").val()).toFixed(3);
    

    if (!isNaN(endChainage) && !isNaN(startChainage)) {
        
        if (parseFloat(parseFloat(endChainage).toFixed(3)) <= parseFloat(parseFloat(startChainage).toFixed(3))) {
           
            $("#errMsgEndChain").show("slow");
            $("#errMsgEndChain").html("<span style='color:red'><b>End Chainage should be greater than Start Chainage.</b></span>");
            return false;
        }
        
        //alert('condition ' + parseFloat($("#MAST_ER_SURFACE_LENGTH").val()) > parseFloat($("#Remaining_Length").val()));
        if (parseFloat($("#spanRemainingLength").html()) == 0)
        {          
            $("#errMsgEndChain").show("slow");
            $("#errMsgEndChain").html("<span style='color:red'><b>Remaining Length is zero,surface details can not be added.</b></span>");
            return false;
        } else if (parseFloat($("#MAST_ER_SURFACE_LENGTH").val()).toFixed(3) > parseFloat($("#Remaining_Length").val()).toFixed(3))
        {
            
            var SurfaceLEngth = parseFloat($("#spnSurfaceLEngth").html()).toFixed(3);
            var RemaininigLength = parseFloat($("#spanRemainingLength").html()).toFixed(3);

            $("#errMsgEndChain").show("slow");
            //$("#errMsgEndChain").html("<span style='color:red'><b>Surface Length Exceeds the Remaining Length.\nPlease Recheck End Chainage.</b></span>");
            $("#errMsgEndChain").html("<span style='color:red'><b>Surface Length(" + SurfaceLEngth + ") exceeds the remaining length (" + RemaininigLength + ") please check end chainage</b></span>");
            
            return false;
        }
    }
    return true;
}

function FormatColumnSurfaceEdit(cellvalue, options, rowObject) {
   
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the Surface Details' onClick ='EditSurface(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnSurfaceDelete(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the Surface Details' onClick ='DeleteSurface(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}


function DisplaySurfaceStartChainage(MAST_ER_ROAD_CODE) {
  
  
    $.ajax({
        url: '/ExistingRoads/SurfaceStartChainageUpdate/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        success: function (jsonData) {

          
            //set start chainage
            if ($("#Operation").val() == "A") {
              
                $("#MAST_ER_STR_CHAIN").val(jsonData.startChainage);

                //$("#spnStartChainnage").html($("#MAST_ER_STR_CHAIN").val());
            
            } 

            unblockPage();
        },
        error: function (err) {
            alert("err");
            alert("error " + err);
            unblockPage();

        }
    });

}