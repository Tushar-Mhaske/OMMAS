$(document).ready(function () {
    $.validator.unobtrusive.parse($('#independentUsersRoleForm'));

    //selectedNameVal = 0;
    //$("#UserRoleID").change(function () {

       
    //    $("#UserProfileID").empty();
        
    //    //clear each option if its selected
    //    //$('#UserProfileID option').each(function () {
    //    //    $(this).removeAttr('selected')
    //    //});

    //    //set the first option as selected
    //    $('#UserProfileID option:first').attr('selected', 'selected');

    //    //set the text of the input field to the text of the first option
    //    $('input.ui-autocomplete-input').val($('#UserProfileID option:first').text());
        
 
    //    if ($(this).val() == 0) {
    //        $("#UserProfileID").append("<option value='0'>Select Name</option>");
    //    }

    //    if ($("#UserRoleID").val() > 0) {

    //        if ($("#UserProfileID").length > 0) {

    //            $.ajax({
    //                url: '/UserManager/GetUserProfileNames',
    //                type: 'POST',
    //                data: { selectedRole: $("#UserRoleID").val(), value: Math.random() },
    //                success: function (jsonData) {
    //                    for (var i = 0; i < jsonData.length; i++) {
    //                        $("#UserProfileID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
    //                    }
    //                },
    //                error: function (xhr, ajaxOptions, thrownError) {
    //                    alert(xhr.status);
    //                    alert(thrownError);
    //                }
    //            });
    //        }
    //    }

    //});



    //Button Submit fot User Creation
    $('#btnSubmit').click(function (evt) {
        //alert(selectedNameVal);
        //set selected value to ProfileID
        //$("#UserProfileID").val(selectedNameVal);
      
        //if ($("#UserProfileID").val() == 0) {
        //    $("#showLevelError").html("Map at least one of the User.");
        //    $("#showLevelError").addClass("field-validation-error");
        //    return;
        //}
        //else {
        //    $("#showLevelError").html("");
        //    $("#showLevelError").removeClass("field-validation-error");
        //}

        if ($('#independentUsersRoleForm').valid()) {
            $.ajax({
                url: '/UserManager/IndependentUsersMapping',
                type: "POST",
                cache: false,
                data: $("#independentUsersRoleForm").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert(error);
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },

                success: function (response) {
                    if (response.Success) {
                        alert("User Mapped Successfully");
                        $("#addDetailsDiv").load("/UserManager/ShowUserList", function () { unblockPage(); });
                        $('#addDetailsDiv').show();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                }
            });
        }
       
        
    });//btnSubmit ends here


    //Call to Load Grid
    CreateIndependentUserRoleListGrid();


    // Stat chage fill Monitor Dropdown add by deepak

    $("#Mast_State").change(function () {
        $("#UserProfileID").val(0);
        $("#UserProfileID").empty();
        //populate Monitor
        if ($("#UserProfileID").length > 0) {
            $.ajax({
                url: '/UserManager/GetStateWiseUserProfileNames',
                type: 'POST',
                data: { selectedRole: $("#UserRoleID").val(), selectedState: $("#Mast_State").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#UserProfileID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }





    });

});//doc.ready ends here


function selectedValue(selItem)
{
    alert(selItem);
    //selectedNameVal = selItem;
}

function CreateIndependentUserRoleListGrid() {
    //Admin Home Page -- Edit Role
    $("#tblIndependentUserList").jqGrid({
        url: '/UserManager/IndependentUsersList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["RoleID", "Role", "Id", "Name", "UserName", "Edit"],
        colModel: [
                     { name: 'RoleID', index: 'RoleID', width: 50, align: "left", hidden: true },
                     { name: 'RoleName', index: 'RoleName', width: 100, align: "left" },
                     { name: 'Id', index: 'Id', width: 50, align: "left", hidden: true },
                     { name: 'Name', index: 'Name', width: 180, align: "left" },
                     { name: 'UserName', index: 'UserName', width: 100, align: "center" },
                     { name: 'Edit', index: 'Edit', width: 50, align: "center", search: false, hidden:true }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#divIndependentUserListPager',
        sortname: 'RoleName',
        sortorder: 'asc',
        loadComplete: function (rowid) {
            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();

            var aEdit = $(this).find('a[id^=aEdit]')
            $.each(aEdit, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var flag = confirm('Are you sure to map user name?');
                        if (flag) {
                            var curRoleId = $(this).closest('tr').attr('id');
                            EditRoleViewList(curRoleId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        caption: "Role List for Independent Users"
    });

    $("#tblIndependentUserList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });


}//Function CreateRoleListGrid()  ends here


//-------------------------------------------------------------------------------------------------------

///------------------------------------------------------------------------------------------------------

// Jquery AutoComplete ComboBox
///------------------------------------------------------------------------------------------------------
//(function ($) {
//    $.widget("custom.combobox", {

//        _create: function () {
//            this.wrapper = $("<span>")
//              .addClass("custom-combobox")
//              .insertAfter(this.element);

//            this.element.hide();
//            this._createAutocomplete();
//            this._createShowAllButton();
//        },

//        _createAutocomplete: function () {
//            var selected = this.element.children(":selected"),
//              value = selected.val() ? selected.text() : "Select One";

//            this.input = $("<input>")
//              .appendTo(this.wrapper)
//              .val(value)
//              .attr("title", "")
//              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
//              .autocomplete({
//                  delay: 0,
//                  minLength: 0,
//                  source: $.proxy(this, "_source")
//              })
//              .tooltip({
//                  tooltipClass: "ui-state-highlight"
//              });

//            this._on(this.input, {
//                autocompleteselect: function (event, ui) {
//                    ui.item.option.selected = true;
//                    this._trigger("select", event, {
//                        item: ui.item.option
//                    });
//                },

//                autocompletechange: "_removeIfInvalid"
//            });
//        },

//        _createShowAllButton: function () {
//            var input = this.input,
//              wasOpen = false;

//            $("<a>")
//              .attr("tabIndex", -1)
//              .attr("title", "Show All Items")
//              .tooltip()
//              .appendTo(this.wrapper)
//              .button({
//                  icons: {
//                      primary: "ui-icon-triangle-1-s"
//                  },
//                  text: false
//              })
//              .removeClass("ui-corner-all")
//              .addClass("custom-combobox-toggle ui-corner-right")
//              .mousedown(function () {
//                  wasOpen = input.autocomplete("widget").is(":visible");
//              })
//              .click(function () {
//                  input.focus();

//                  // Close if already visible
//                  if (wasOpen) {
//                      return;
//                  }

//                  // Pass empty string as value to search for, displaying all results
//                  input.autocomplete("search", "");
//              });
//        },

//        _source: function (request, response) {
//            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
//            response(this.element.children("option").map(function () {
//                var text = $(this).text();
//                if (this.value && (!request.term || matcher.test(text)))
//                    return {
//                        label: text,
//                        value: text,
//                        option: this
//                    };
//            }));
//        },

//        _removeIfInvalid: function (event, ui) {

//            // Selected an item, nothing to do
//            if (ui.item) {
//                return;
//            }

//            // Search for a match (case-insensitive)
//            var value = this.input.val(),
//              valueLowerCase = value.toLowerCase(),
//              valid = false;
//            this.element.children("option").each(function () {
//                if ($(this).text().toLowerCase() === valueLowerCase) {
//                    this.selected = valid = true;
//                    return false;
//                }
//            });

//            // Found a match, nothing to do
//            if (valid) {
//                return;
//            }

//            // Remove invalid value
//            this.input
//              .val("")
//              .attr("title", value + " didn't match any item")
//              .tooltip("open");
//            this.element.val("");
//            this._delay(function () {
//                this.input.tooltip("close").attr("title", "");
//            }, 2500);
//            this.input.data("ui-autocomplete").term = "";
//        },

//        _destroy: function () {
//            this.wrapper.remove();
//            this.element.show();
//        }
//    });
//})(jQuery);

///------------------------------------------------------------------------------------------------------