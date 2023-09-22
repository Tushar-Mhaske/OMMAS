$(document).ready(function () {

    setTimeout(function () {
        $("#userListLink").trigger('click');
    }, 100);

    //JqGrid to populate Grouping of Levels in tree structure
    $("#treegrid").jqGrid({
        url: '/Usermanager/GetLevels/',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Available Roles'],
        colModel: [
               { name: 'RoleName', index: 'RoleName', width: 80, align: 'left', sortable: false }
        ],
        //autowidth: true,
        sortname: 'RoleName',
        width: 200,
        rowNum: -1,
        height: "auto",
        viewrecords: true,
        imgpath: '/Content/themes/steel/images',
        emptyrecords: "no records to display..",
        treeGrid: true,
        treeGridModel: 'adjacency',
        ExpandColumn: 'RoleName',
        onSelectRow: function (id) {
            // get data from the column 'userCode'
            //var userCode = $(this).jqGrid('getCell', 'userCode'); //returns true of false
            //alert(userCode);
            //alert($(this).getGridParam("selrow"));        // returns selected row
            //alert(id);                                    // returns selected row
            //alert($(this).getCell(id, 'parent'));         // get Parent node
            if (id.indexOf('R') >= 0)                       // Call only for child nodes i.e. for Role 
            {
                $('#roleDetailsDiv div').html('');
                $('#menuMappingDiv div').html('');
                $('#userMappingDiv div').html('');

                loadDetails(id, $(this).getCell(id, 'parent'));
                loadMenus(id, $(this).getCell(id, 'parent'));
                loadUserDeails(id);
                $('#menuDetailsAccordionDiv').show();
                $('#userDetailsAccordionDiv').show();
                closeUpdateDetails();
            }
        }
    });



    //BeforeExpand & Before Collapse Node.
    /*  var orgExpandNode = $.fn.jqGrid.expandNode,
      orgCollapseNode = $.fn.jqGrid.collapseNode;
      $.jgrid.extend({
          expandNode: function (rc) {
              alert('before expandNode: rowid="' + rc._id_ + '", name="' + rc.name + '"');
              return orgExpandNode.call(this, rc);
          },
          collapseNode: function (rc) {
              alert('before collapseNode: rowid="' + rc._id_ + '", name="' + rc.name + '"');
              return orgCollapseNode.call(this, rc);
          }
      }); */


    //-------------------------------------



    //Toggle Panels
    $("#notaccordion").addClass("ui-accordion ui-accordion-icons ui-widget ui-helper-reset")
                    .find("h3")
                    .addClass("ui-accordion-header ui-helper-reset ui-state-default ui-corner-top ui-corner-bottom ui-accordion-icons")
                    .hover(function () { $(this).toggleClass("ui-state-hover"); })
                    .prepend('<span class="ui-icon ui-icon-triangle-1-e ui-accordion-header-icon"></span>')
                    .click(function () {
                        $(this)
                            .toggleClass("ui-accordion-header-active ui-state-active ui-state-default ui-corner-top")
                            .find("> .ui-icon").toggleClass("ui-icon-triangle-1-e ui-icon-triangle-1-s").end()
                            .next().toggleClass("ui-accordion-content-active ui-accordion-icons").slideToggle();
                        return false;
                    })
                    .next()
                        .addClass("ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom")
                        .hide();


    //Expand all Details Div
    $('#roleDetailsAccordionDiv').show();
    //$('#menuDetailsAccordionDiv').show();
    //$('#userDetailsAccordionDiv').show();


    //--------------------------------------------------------------
    //Update Details Section Hide and show condituionally
    //--------------------------------------------------------------

    $("#roleHomePageLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#roleHomePageLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/RoleHomePage", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });


    });


    $("#userListLink").click(function () {


        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#userListLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowUserList", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });


    });



    $("#roleListLink").click(function () {



        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#roleListLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );

        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowRoleList", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });


    });



    $("#menuListLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#menuListLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowMenuList", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });


    });


    //User Log Report Details

    $("#loginListLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#loginListLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowUserLogDetails", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });


    //User Log Access Report Details

    $("#logAccessListLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#logAccessListLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowUserLogAccessDetails", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });
    //--------------------------------------------------------------

    $('#roleDetailsAccordionDiv').accordion(
        {
            fillSpace: true
        });

    $('#ReportLinks').accordion(
        {
            fillSpace: true
        });

    $('#linksTable').accordion(
        {
            fillSpace: true
        });



    // Error Elmah Added By Rohit Jadhav 9-May-2014----------------------------------------------
    $("#elmaherrorLink").click(function () {
     
        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#elmaherrorLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ShowElmahErrorLog", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });
    });


    $("#RefreshDataLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#RefreshDataLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/AccountRefreshDataLayout", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });



    //Test Email

    $("#TestEmailLink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#TestEmailLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/TestMail", function () { unblockPage(); });

        

            $('#addDetailsDiv').show();

        });


    });


    //Definalize BalanceSheet Added By Abhishek kamble 27Nov2014 start 

    $("#DeFinalizeBalSheet").click(function () {

        //alert("test");
        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#DeFinalizeBalSheet").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/RevokeClosing/DefinalizeBalanceSheetView", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });

    });


    //Definalize BalanceSheet Added By Abhishek kamble 27Nov2014 end
 
//----------------------------------------------------------------------------------



    //Generate PDF Open Key For Bank Added By Abhishek kamble 30Jan2015 start 

    $("#GeneratePDFKeyForBank").click(function () {
        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + "Generate PDF Open Key For Bank" +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/Bank/ViewBankPDFKeyGeneration", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });

    });
    //Generate PDF Open Key For Bank Added By Abhishek kamble 30Jan2015 start 

    //----------------------------------------------------------------------------------


    // Proposal PIU Mapping
    $("#proposalPIULink").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#proposalPIULink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ListDistrictUsers", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });
    });



    //*************** QM Help Desk Link ********************///////

    $('#qmHelpDeskLink').click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#qmHelpDeskLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/QualityMonitoringHelpDesk/QMHelpDesk", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });
    //***********End QM Help Desk Link


    //*************** Proposal Data Gap Link ********************///////

    $('#propDataGapLink').click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#propDataGapLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/ListProposalDataGap", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });
    //***********End Proposal Data Gap Link


    //*************** Proposal Data Gap Link ********************///////

    $('#agrUpdateLink').click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#agrUpdateLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img  class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/Agreement/ListAgreementsForStatusUpdation", function () { unblockPage(); });
            $('#addDetailsDiv').show();

        });


    });
    //***********End Proposal Data Gap Link


}); //doc.ready ends here





//Load Role Details
function loadDetails(nodeId, parentId) {
    $.ajax({
        url: '/UserManager/RoleDetails',
        type: "POST",
        cache: false,
        data: { roleId: nodeId, levelId: parentId, value: Math.random() },
        //beforeSend: function () {
        //    blockPage();
        //},
        error: function (xhr, status, error) {
            // unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {
            if (response.Success) {
                //alert("Success");
                $("#roleDetailsDiv").load("/UserManager/RoleDetails");
                //$("#roleDetailsDiv").show();
            }
            else {
                $('#roleDetailsDiv').html(response);
                // $.validator.unobtrusive.parse($('#mainDiv'));
            }
            // unblockPage();
        }
    }); //ajax req ends here

}// function loadDetails ends here



//Load Menu Details
function loadMenus(nodeId, parentId) {
    $.ajax({
        url: '/UserManager/RoleMenuMapping',
        type: "POST",
        cache: false,
        data: { roleId: nodeId, levelId: parentId },
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {
            if (response.Success) {
                //alert("Success");
                $("#menuMappingDiv").load("/UserManager/RoleMenuMapping");

            }
            else {
                $('#menuMappingDiv').html(response);
                // $.validator.unobtrusive.parse($('#mainDiv'));
            }
            unblockPage();
        }
    }); //ajax req ends here

}




//Load User Details
function loadUserDeails(nodeId) {
    $.ajax({
        url: '/UserManager/RoleUserMapping',
        type: "POST",
        cache: false,
        data: { roleId: nodeId },
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            Alert("Request can not be processed at this time,please try after some time!!!");
            return false;
        },
        success: function (response) {
            if (response.Success) {
                //alert("Success");
                $("#userMappingDiv").load("/UserManager/RoleMenuMapping");
                //$("#menuMappingDiv").show();
            }
            else {
                $('#userMappingDiv').html(response);
                // $.validator.unobtrusive.parse($('#mainDiv'));
            }
            unblockPage();
        }
    }); //ajax req ends here

}


function closeUpdateDetails() {
    //Clear the contents of Div
    $('#formDiv div').html('');
    $("#formDiv").hide();
    //Expand all Details Div
    $('#roleDetailsAccordionDiv').show();
}



