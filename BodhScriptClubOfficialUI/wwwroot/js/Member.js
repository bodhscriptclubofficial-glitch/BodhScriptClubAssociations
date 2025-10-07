




document.addEventListener('DOMContentLoaded', function () {
    const pictureInput = document.getElementById('pictureInput');
    const picturePreview = document.getElementById('picturePreview');

    pictureInput.addEventListener('change', function () {
        const file = this.files[0]; // Get the selected file
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                picturePreview.src = e.target.result; // Set preview
            }
            reader.readAsDataURL(file); // Convert file to base64 URL
        } else {
            picturePreview.src = ''; // Clear preview
        }
    });
});




function EditItem(Memberid) {

    $.ajax({
        url: '/Member/Edit',
        type: 'GET',
        data: { Memberid: Memberid },
        success: function (data) {
            console.log(data);
            $("#MemberPicture").val(data.memberPicture);
            $("#picturePreview").attr("src", data.memberPicture || "/images/no-image.png")
            $('#Memberid').val(data.memberid);
            $('#Membername').val(data.membername);
            $('#Memberstream').val(data.memberstream);
            $('#MemberSession').val(data.memberSession);
            $('#MemberRollno').val(data.memberRollno);
            $('#MemberemailId').val(data.memberemailId);
            $('#MembercontactNo').val(data.membercontactNo);
            $('#memberModal').modal('show');
        }
    });
}
function ClearMemberForm() {
    // Clear all input fields
    $('#Memberid').val('');
    $('#Membername').val('');
    $('#Memberstream').val('');
    $('#MemberSession').val('');
    $('#MemberRollno').val('');
    $('#MemberemailId').val('');
    $('#MembercontactNo').val('');
    $('#MemberPicture').val(''); // hidden input for existing image

    // Clear file input
    $('#pictureInput').val('');

    // Reset image preview
    $('#picturePreview').attr('src', '/images/no-image.png');
}


$(document).ready(function () {
    // Your code here
    console.log("Page is ready!");
    ClearMemberForm();

    $("#btn_submit").click(function (e) {
        e.preventDefault();
        var $btn = $(this);
        var $loader = $btn.find(".btn-loader");

        // Start loader
        $btn.addClass("loading");
        $loader.show();
        $btn.prop("disabled", true);
        var memberName = $("#Membername").val().trim();
        var memberStream = $("#Memberstream").val().trim();
        var memberSession = $("#MemberSession").val().trim();
        var memberRollno = $("#MemberRollno").val().trim();
        var memberEmail = $("#MemberemailId").val().trim();
        var memberContact = $("#MembercontactNo").val().trim();
        var existingImage = $("#MemberPicture").val().trim();
        var fileInput = $("#pictureInput")[0];

        // Basic validations
        if (!memberName) { Swal.fire("Please enter Member Name"); return; }
        if (!memberStream) { Swal.fire("Please enter Member Stream"); return; }
        if (!memberSession) { Swal.fire("Please enter Member Session"); return; }
        if (!memberRollno) { Swal.fire("Please enter Member Roll Number"); return; }
        if (!memberEmail) { Swal.fire("Please enter Member Email"); return; }

        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(memberEmail)) { Swal.fire("Please enter a valid Email"); return; }

        if (!memberContact) { Swal.fire("Please enter Member Contact Number"); return; }

        // Picture validation
        if ((!existingImage || existingImage === "") && (!fileInput || fileInput.files.length === 0)) {
            Swal.fire("Please upload a picture for the member.");
            return;
        }

        if (fileInput && fileInput.files.length > 0) {
            var file = fileInput.files[0];
            var allowedTypes = ["image/jpeg", "image/png", "image/gif"];
            if (!allowedTypes.includes(file.type)) {
                Swal.fire("Only JPG, PNG, or GIF images are allowed.");
                return;
            }
            var maxSizeMB = 2;
            if (file.size > maxSizeMB * 1024 * 1024) {
                Swal.fire("Image size must be less than " + maxSizeMB + " MB.");
                return;
            }
        }

        // Prepare FormData
        var formData = new FormData();
        formData.append("MemberPicture", existingImage);
        formData.append("Memberid", $("#Memberid").val());
        formData.append("Membername", memberName);
        formData.append("Memberstream", memberStream);
        formData.append("MemberSession", memberSession);
        formData.append("MemberRollno", memberRollno);
        formData.append("MemberemailId", memberEmail);
        formData.append("MembercontactNo", memberContact);

        if (fileInput && fileInput.files.length > 0) {
            formData.append("PictureFile", fileInput.files[0]);
        }

      
        $.ajax({
            url: '/Member/Index',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                console.log(result);
                if (result.success) {
                    Swal.fire(result.message || "Member saved successfully!").then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire(result.message || "Member save failed: Unknown error");
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr, status, error);
                Swal.fire("An error occurred while saving member: " + error);
            },
            complete: function () {
                // Stop loader
                resetButton();
            }
        });
    });

    let idleTime = 0;
    const logoutAfter = 120; // seconds (2 minutes)

    // increment idle time every second
    setInterval(() => {
        idleTime++;
        if (idleTime >= logoutAfter) {
            window.location.href = '/Home/Logout';
        }
    }, 1000);

    // reset idle time on activity
    $(document).on('mousemove keypress click scroll', function () {
        idleTime = 0;
    });

});
function resetButton() {
    $btn.removeClass("loading");
    $loader.hide();
    $btn.prop("disabled", false);
}

function DeleteItem(Memberid) {
    Swal.fire({
        title: "Are you sure?",
        text: "This member record will be permanently deleted!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Member/Delete',   // your controller action URL
                type: 'POST',
                data: { Memberid: Memberid },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Member record deleted successfully.",
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        });
                        setTimeout(() => {
                            window.location.reload();
                        }, 1500);
                    } else {
                        Swal.fire("Error", response.message || "Unable to delete record.", "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "Something went wrong while deleting.", "error");
                }
            });
        }
    });
}
