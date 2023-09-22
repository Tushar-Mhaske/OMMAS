$.validator.unobtrusive.adapters.add('dynamicrange', ['minvalueproperty', 'maxvalueproperty'],
    function (options) {
        options.rules['dynamicrange'] = options.params;
        if (options.message != null) {
            $.validator.messages.dynamicrange = options.message;
        }
    }
);

$.validator.addMethod('dynamicrange', function (value, element, params) {
    var minValue = parseInt($('input[name="' + params.minvalueproperty + '"]').val(), 10);
    var maxValue = parseInt($('input[name="' + params.maxvalueproperty + '"]').val(), 10);
    var currentValue = parseInt(value, 10);
    

    if ( !isNaN(minValue) && !isNaN(maxValue) && !isNaN(currentValue) ){
    
        if( minValue > currentValue || ( currentValue > maxValue )) {
            var message = $(element).attr('data-val-dynamicrange');
            $.validator.messages.dynamicrange = $.format(message, minValue, maxValue);
            return false;
        }
        else {
            return true;
        }
    }
    return true;

}, '');


$(document).ready(function () {
   
    $.validator.unobtrusive.parse($('frmTrafficIntensity'));

    GetTrafficIntensity($("#IMS_PR_ROAD_CODE").val());

    
    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }
    //$("#IMS_TOTAL_TI").val("");
    //$("#IMS_COMM_TI").val("");

    $("#btnReset").click(function () {

        $("#IMS_TOTAL_TI").val("");
        $("#IMS_COMM_TI").val("");
    });


    $('#btnSave').click(function (evt) {                    
        evt.preventDefault();

        if ($('#frmTrafficIntensity').valid()) {
            $.ajax({
                url: "/Proposal/TrafficIntensity/",
                type: "POST",
                cache: false,
                data: $("#frmTrafficIntensity").serialize(),
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
                    $("#tbTraffic").trigger('reloadGrid');

                    $("#frmTrafficIntensity").trigger('reset');

                    PopulateTrafficIntensityYears($("#IMS_PR_ROAD_CODE").val());
                    $("#IMS_TOTAL_TI").val("");
                    $("#IMS_COMM_TI").val("");
                    if (response.success) {
                        alert("Traffic Intensity Details Added Succesfully.");
                    }
                }
            });
        }
        else {
            
        }
            
    });


    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();

        if ($('#frmTrafficIntensity').valid()) {
            $.ajax({
                url: "/Proposal/UpdateTrafficIntensity/",
                type: "POST",
                cache: false,
                data: $("#frmTrafficIntensity").serialize(),
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

                    $("#tbTraffic").trigger('reloadGrid');

                    $("#frmTrafficIntensity").trigger('reset');

                    $("#IMS_TOTAL_TI").val("");
                    $("#IMS_COMM_TI").val("");

                    PopulateTrafficIntensityYears($("#IMS_PR_ROAD_CODE").val());

                    if (response.Success) {
                        alert("Traffic Intensity Details Updated Succesfully.");

                        $("#Operation").val("A");
                        $("#rowAdd").show();
                        $("#rowUpdate").hide();
                    }
                }
            });
        }
    });
});

function PopulateTrafficIntensityYears(IMS_PR_ROAD_CODE) {
    

    $("#IMS_TI_YEAR").val(0);
    $("#IMS_TI_YEAR").empty();   

    $.ajax({
        url: '/Proposal/PopulateTrafficIntensityYears/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        success: function (jsonData) {
            for (var i = 0; i < jsonData.length; i++) {
                $("#IMS_TI_YEAR").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
            }
            unblockPage();
        },
        error: function (err) {
            alert("error " + err);
            unblockPage();
        }
    });

}

function GetTrafficIntensity(IMS_PR_ROAD_CODE) {
    jQuery("#tbTraffic").jqGrid({
        url: '/Proposal/GetTrafficIntensityList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Year', 'Total Motarised Traffic/day', 'ESAL', "Edit", "Delete"],
        colModel: [
                    { name: 'Year', index: 'Year', width: 230, sortable: false, align: "center" },
                    { name: 'TotalMotarisedTrafficday', index: 'TotalMotarisedTrafficday', width: 250, sortable: false, align: "center" },
                    { name: 'CCVPDESAL', index: 'CCVPDESAL', width: 250 , sortable: false, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }
        ],
        pager: jQuery('#dvTrafficPager'),
        rowList: [08, 10, 12],
        rowNum: 08,
         postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,        
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Traffic Intensity Details",
        height: 'auto',
        width: 'auto',
        sortname: 'Year',
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    });    
}

function EditTrafficI(IMS_PR_ROAD_CODE, IMS_TI_YEAR) {
    $("#divTrafficIntensity").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divProposalForm").load('/Proposal/EditTrafficIntensity/' + IMS_PR_ROAD_CODE + "$" + IMS_TI_YEAR , function () {
            $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
            unblockPage();
        });        
    });


    //$.ajax({
    //    url: "/Proposal/EditTrafficIntensity/" ,
    //    type: "POST",
    //    cache: false,
    //    data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, IMS_TI_YEAR: IMS_TI_YEAR, value: Math.random() },
    //    beforeSend: function () {
    //        blockPage();
    //    },
    //    error: function (xhr, status, error) {
    //        unblockPage();
    //        Alert("Request can not be processed at this time,please try after some time!!!");
    //        return false;
    //    },
    //    success: function (response) {
    //        unblockPage();
    //        $("#divTrafficIntensity").html(response);
    //    }
    //});

}

function DeleteTrafficI(IMS_PR_ROAD_CODE, IMS_TI_YEAR) {

    if (confirm("Are you sure to delete the traffic intensity details ? ")) {

        $.ajax({
            url: "/Proposal/DeleteTrafficIntensity/",
            type: "POST",
            cache: false,
            data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, IMS_TI_YEAR: IMS_TI_YEAR, value: Math.random() },
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
                $("#tbTraffic").trigger('reloadGrid');
                PopulateTrafficIntensityYears(IMS_PR_ROAD_CODE);

                if (response.Success) {
                    alert("Traffic Intensity Details Deleted Succesfully.");
                }
               
            }
        });

    }
    else {
        return;
    }
}
