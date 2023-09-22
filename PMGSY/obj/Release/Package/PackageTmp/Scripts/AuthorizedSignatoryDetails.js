
//var txtSearch = "";
//var StutusType = "T";

$(document).ready(function () {

    fundType = $("#FundType").val().toString();

    $("#iconClose").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#SearchTable").hide('slow');
        $('#SearchCriteria').val("");
        $("#ddlStatus").val("Y");
        $('#AuthorizedSigList').jqGrid('GridUnload');
        loadGrid("", "Y");

        //modified by abhishek kamble 24-oct-2013
        $("#Search").show('slow');
        $.unblockUI();
    })

    loadGrid("", "Y");

    //$("#AuthorizedSigList").parents('div.ui-jqgrid-bdiv').css("max-height", "10%");

    $("#btnSearch").click(function () {

       // trimFields();

        if (!CheckUserName($('#SearchCriteria').val()) == true)
        {
            alert("Please enter valid Name to search. Only alphabets are allowed.");
            $("#SearchCriteria").focus();
            return false;
        }

        var txtSearch = $("#SearchCriteria").val();
              

       var StutusType = $("#ddlStatus").val();

       

        $('#AuthorizedSigList').jqGrid('GridUnload');

        loadGrid(txtSearch, StutusType)

        
    });


    $("#Search").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#SearchTable").toggle('slow', function () {

        });
        //modified by abhishek kamble 24-oct-2013
        $("#Search").hide('slow');
        $.unblockUI();
    });


    $("#btnCancel").click(function () {

        $("#SearchTable").toggle('slow', function () {

        });

        $('#SearchCriteria').val("");
        $("#ddlStatus").val("Y");

        $('#AuthorizedSigList').jqGrid('GridUnload');

        loadGrid("", "Y");

        //modified by abhishek kamble 24-oct-2013
        $("#Search").show('slow');
    });

    

});//end of the document.ready

function loadGrid(txtSearch,StutusType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (txtSearch == undefined) {
        txtSearch = "";
    }
    else if (StutusType==undefined) {
        StutusType ="Y"
    }
    
   

    jQuery("#AuthorizedSigList").jqGrid({

        url: '/AuthorizedSignatory/GetAuthorizedSignatoryList/' ,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',        
        
        rowNum: 0,
        //scrollOffset:18,
        postData: { 'varSearch': txtSearch, 'activeStatus': StutusType },
        rownumbers :true,
        //width: 1100,
        autowidth: true,
        //shrinkToFit: false,
        pginput: false,
        pgbuttons: false,        
        //rowList: [15, 20, 30],
        loadComplete:function(){
          
          /*  if (StutusType == "Y") {
                jQuery("#AuthorizedSigList").jqGrid('hideCol', 'End_date');
            }
            else {
                jQuery("#AuthorizedSigList").jqGrid('showCol', 'End_date');
            }*/


            //$("#AuthorizedSigList").parents('div.ui-jqgrid-bdiv').css("max-height", "550");

            var recordCount = jQuery("#AuthorizedSigList").jqGrid('getGridParam', 'reccount');

            if (recordCount > 25    ) {
                $("#AuthorizedSigList").jqGrid('setGridHeight', '500');
            } else {
                $("#AuthorizedSigList").jqGrid('setGridHeight', 'auto');
            }

            //$("#gview_AuthorizedSigList > .ui-jqgrid-bdiv").css('height', window.innerHeight * .7);

            $('#AuthorizedSigList_rn').html('Sr.<br/>No.');
            
        },
        colNames: [ 'DPIU Name', 'Authorized Signatory Name', 'Designation', 'Start Date', 'End Date', 'Mobile','Email','Status','Registered DSC','Un-Register DSC','Action'],
        colModel: [
            {
                name: 'Agency_name',
                index: 'Agency_name',
                width:20,
                align: "left"

            },
            {
                name: 'auth_sig_name',
                index: 'auth_sig_name',
                width: 20,
                align: "left",
                sortable: false

            },
        {
            name: 'Designation_name',
            index: 'Designation_name',
            width: 20,
            align: "left",
            hidden: true,
            sortable: false
        },
            {
                name: 'Start_Date',
                index: 'Start_Date',
                width: 10,
                align: "Center",
                sortable:false

            }, {
                name: 'End_date',
                index: 'End_date',
                width: 10,
                hidden:true,
                align: "Center",
                hidden: (StutusType == "Y" ? true:false)

            },
            //{
            //    name: 'addres_1',
            //    index: 'addres_1',
            //    width: 130,
            //    align: "Center"


            //}, {
            //    name: 'addres_2',
            //    index: 'addres_2',
            //    width: 120,
            //    align: "Center"

            //},
            {
                name: 'mobile_no',
                index: 'mobile_no',
                width:10,
                align: "Center",
                sortable:false
            },
            {
                name: 'Email',
                index: 'Email',
                width:30,
                align: "left",
                sortable:false
            },
            {
                name: 'Status',
                index: 'Status',
                width: 20,
                align: "left",
                sortable:false

            },
            {
                name: 'DSCRegistered',
                index: 'DSCRegistered',
                width: 15,
                align: "left",
                sortable: false
            },
            {
                name: 'UnRegisterDSC',
                index: 'UnRegisterDSC',
                width: 15,
                align: "left",
                sortable: false
            },

            {
                name: 'Add_Edit',
                index: 'Add_Edit',
                width: 10,
                align: "Center",
                sortable: false,
                //Below Line commented on 15-11-2021 to enable Action for Admin Fund 
                //hidden: (fundType == "P" ? false : true)

                //Below Line Added on 15-11-2021 to enable Action for Admin Fund
                hidden: ((fundType == "P" || fundType == "A") ? false : true)
            }

        ],
        pager: "#pager",
        viewrecords: true,
       
        sortname: 'Agency_name',
        sortorder: "asc",
        
        caption: "Authorized Signatory Details",
        hidegrid:false
    });

    $.unblockUI();
}

function ADDAuthSignatory (urlparam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //blockPage();

    $("#mainDiv").load("/AuthorizedSignatory/AddEditAuthSignatory/" + urlparam, function ()
    {
        $.validator.unobtrusive.parse($("#mainDiv"));
        //  unblockPage();
        $.unblockUI();

    });
    $.unblockUI();

}

function DeRegister(urlparam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    var Todelete = confirm('Are you sure you want to un-register digital certificate ?');

    if (Todelete) {
       
        blockPage();
        $.ajax({
            type: "POST",
            url: "/AuthorizedSignatory/DeRegisterDSC/" + urlparam,
           
            data: $("form").serialize(),
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');

                return false;

            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();

                if (data.result == 1) {
                    alert("Digital Signature un-registered Successfuly.");

                   
                    $("#AuthorizedSigList").jqGrid().setGridParam({ url: '/AuthorizedSignatory/GetAuthorizedSignatoryList/' }).trigger("reloadGrid");
                    return false;
                }
                
                else {

                    alert("Error while un-registering Digital Certificate  ");
                    return false;
                }
            }
        }); //end of ajax
    }


    $.unblockUI();
}

function EditAuthSignatory(urlparam) {
  
    // blockPage();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#mainDiv").load("/AuthorizedSignatory/AddEditAuthSignatory/" + urlparam, function () {
        $.validator.unobtrusive.parse($("#mainDiv"));
        //unblockPage();

        if ($("#rdoAadhaarNo").is(":checked")) {
            $("#rdoAadhaarNo").trigger('click');
        }
        else if ($("#rdoPanNo").is(":checked")) {
            $("#rdoPanNo").trigger('click');
        }

        $.unblockUI();
    });
    $.unblockUI();
}

function ViewAuthSignatory(urlparam)
{
    //blockPage();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#mainDiv").load("/AuthorizedSignatory/AddEditAuthSignatory/" + urlparam, function () {
        //unblockPage();
        $.unblockUI();
    });
    $.unblockUI();
}

function CheckUserName(txtValue)    //allows only A-Z','a-z' and '-',and '.'   for user first name ,second name ,third name
{

    var InvalidCharacters = "0123456789/`~!@#$%^&*_+=;:,.-'{[}]|\?<>\"";
    var flag = 0;
    for (var i = 0; i < txtValue.length; i++) {
        if (InvalidCharacters.indexOf(txtValue.charAt(i)) != -1) {
            flag = 1;
        }
    }
    if (flag == 0) {
        return true;
    }
    else {
        return false;
    }

}