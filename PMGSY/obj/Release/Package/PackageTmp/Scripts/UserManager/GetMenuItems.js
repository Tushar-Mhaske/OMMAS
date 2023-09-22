//$(document).ready(function () {

//    if ($("#tblChilds input:radio").length == 1) {
//        //$("#" + $("#tblChilds").find("input").attr('id')).trigger("click");
//        //alert("$(#" + $("#tblChilds").find("input").attr('id') + ")");

//        //$("#tblChilds").find("input").attr('checked', true);



//       // var levels = $("#tblChilds").find("input").attr('id').split('_');



//        //$("#tblAddLevelComb").find("input[value=" + 1 + "]").attr('checked', true);

//        //for (var j = 1; j <= levels.length; j++) {
//        //alert($("#tblAddLevelComb").find("input[value=" + levels[j] + "]").attr('id'));


//        //}

//        //$.each(levels, function () {
//        //    // alert($("#tblAddLevelComb").find("input[value=" + this + "]").attr("id"));
//        //    $("#tblAddLevelComb").find("input[value=" + this + "]").attr('checked', true);
//        //});

//    }

//    //$("#tblChilds input:radio").click(function () {
//    //    if ($(this).is(':checked')) {
//    //        $("#tblAddLevelComb :checkbox").each(function () {
//    //            if (this.checked) {
//    //                $(this).attr('checked', false);
//    //            }
//    //        });
//    //        var levels = $(this).attr('id').split('_');

//    //        for (var i = 1; i <= levels.length; i++) {
//    //            $("#tblAddLevelComb").find("input[value=" + levels[i] + "]").attr('checked', true);
//    //        }
//    //    }
//    //    else {
//    //        //alert('unchk');
//    //    }
//    //});


//    $("#btnSubmit").click(function () {
//        $.ajax({
//            type: "POST",
//            url: "/Menu/AddLevelCombination/",
//            async: false,
//            data: $("#frmCombination").serialize(),
//            error: function (xhr, status, error) {
//                $('#ErrorMessageBox').show();
//                document.getElementById('ErrorMessage').innerHTML = (xhr.responseText);
//                $("#ErrorMessage").stop().show('slow');
//            },
//            success: function (data) {
//                $('#ErrorMessageBox').hide();
//                Alert('Level Combination Added Successfully.');
//                return false;
//            }
//        });
//    });
//});