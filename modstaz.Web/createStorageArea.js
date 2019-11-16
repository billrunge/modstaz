function createStorageArea(form) {
    var userId = form.userId.value;
    var storageAreaName = form.storageAreaName.value;
    console.log(`user id = ${userId}, storage area name = ${storageAreaName}`);

    console.log("Creating Storage area!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateStorageArea`, true);

    let data = {
        "StorageAreaName": storageAreaName,
        "UserId": userId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}
