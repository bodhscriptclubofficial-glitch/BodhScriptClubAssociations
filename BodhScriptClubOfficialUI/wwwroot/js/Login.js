

$("#btn_Login").click(function () {
        var Username = $("#Username").val().trim();
        var Password = $("#Password").val().trim();

        if (!Username || !Password) {
            Swal.fire("Please enter username and password");
            return;
        }

       
        $.ajax({
            url: '/Home/Login',
            contentType: 'application/json',
            type: 'POST',
            data: JSON.stringify({ Username: Username, Password: Password }),

            success: function (response) {
                console.log(response); 
                if (response.success) {
                    Swal.fire({
                        title: 'Login Successful!',
                        text: 'Redirecting to dashboard...',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: false
                    }).then(() => {
                        window.location.href = response.redirectUrl; // will navigate to /Home/DashBoard
                    });

                } else {
                    Swal.fire({
                        title: 'Login failed!',
                        text: 'Redirecting to Home...',
                        icon: 'error',
                        timer: 2000,
                        showConfirmButton: false
                    }).then(() => {
                        //window.location.href = response.redirectUrl; // will navigate to /Home/DashBoard
                    });
                }
            },
            error: function (xhr) {
                console.error("Error:", xhr.status);
            }
        });
    });
