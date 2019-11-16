function deleteStorageArea(storageAreaId) {
    console.log(`storage area id = ${storageAreaId}`);

    console.log("Deleting Storage Area!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/DeleteStorageArea`, true);

    let data = {
        "StorageAreaID": storageAreaId
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
        getStorageAreas();
    };

    request.onerror = function () { };
}