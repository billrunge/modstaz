function createStorageArea(form) {
    let jwt = localStorage.getItem('JWT');

    if (jwt != undefined) {
        var storageAreaName = form.storageAreaName.value;

        console.log("Creating Storage area!");
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/CreateStorageArea`, true);

        let data = {
            "StorageAreaName": storageAreaName,
            "JWT": jwt
        };

        request.send(JSON.stringify(data));

        request.onload = function () {
            var resp = this.response;
            console.log(resp);
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            window.location.replace("/getStorageAreas.html");
        };

        request.onerror = function () { };

    } else {
        redirectToLogin();
    }
}
