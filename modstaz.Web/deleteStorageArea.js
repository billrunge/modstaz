function deleteStorageArea(storageAreaId) {
    let jwt = localStorage.getItem('JWT');
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/DeleteStorageArea`, true);

    let data = {
        "StorageAreaID": storageAreaId,
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
        getStorageAreas();
    };

    request.onerror = function () { };
}