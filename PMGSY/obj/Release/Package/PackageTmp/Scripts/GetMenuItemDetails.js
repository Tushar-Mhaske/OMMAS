$(document).ready(function() {
    alert("fdsff");
    alert($("#tblChilds input:radio").length);

    if ($("#tblChilds input:radio").length == 1) {
        //$("#" + $("#tblChilds").find("input").attr('id')).trigger("click");
        //alert("$(#" + $("#tblChilds").find("input").attr('id') + ")");
        $("#tblChilds").find("input").attr('checked', true);

        alert($("#tblChilds").find("input").attr('id').split(','));

        var levels = $("#tblChilds").find("input").attr('id').split(',');

        alert();

        for (var i = 1; i < levels.length; i++)
        {
            alert(levels[i]);
            $("#tblAddLevelComb").find("input[value=" + levels[i] + "]").attr('checked', true);
        }
    }

    $("#tblChilds input:radio").click(function() {
        if ($(this).is(':checked')) {
            $("#tblAddLevelComb :checkbox").each(function() {
                if (this.checked) {
                    $(this).attr('checked', false);
                }
            });
            var levels = $(this).attr('id').split('_');

            for (var i = 1; i < levels.length; i++) {
                $("#tblAddLevelComb").find("input[value=" + levels[i] + "]").attr('checked', true);
            }
        }
        else {
            //alert('unchk');
        }
    });


    $("#btnSubmit").click(function() {
        $.ajax({
            type: "POST",
            url: "/UserManager/AddLevelCombination/",
            async: false,
            data: $("#frmCombination").serialize(),
            error: function(xhr, status, error) {
                $('#ErrorMessageBox').show();
                document.getElementById('ErrorMessage').innerHTML = (xhr.responseText);
                $("#ErrorMessage").stop().show('slow');
            },
            success: function(data) {
                $('#ErrorMessageBox').hide();
                alert('Level Combination Added Successfully.');
                return false;
            }
        });
    });

});