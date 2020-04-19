let storageAreaId;
loadCreateView();

function createView(form) {
    let jwt = localStorage.getItem('JWT');

    if (jwt != undefined) {
        let viewName = form.viewName.value;
        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/CreateView`, true);

        let data = {
            "ViewName": viewName,
            "StorageAreaId": storageAreaId,
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
            window.location.replace(`/getStorageAreaViews.html?ID=${storageAreaId}`);
        };

        request.onerror = function () { };

    } else {
        redirectToLogin();
    }
}

function loadCreateView() {
    isUserAuthenticated();
    let params = getGetParameters();

    if (params.ID != undefined) {
        storageAreaId = params.ID[0];

    } else {
        window.location.replace(`../getStorageAreaViews.html?ID=${storageAreaId}`);
    }
    insertCreateViewLinks()
}

function insertCreateViewLinks() {
    let params = getGetParameters();

    if (params.ID != undefined) {
        let storageAreaId = params.ID[0];
        let html = "";

        html += `<a href="/getStorageAreaViews.html?ID=${storageAreaId}">Return to Edit Views</a><br>`;
        html += `<a href="/getStorageArea.html?ID=${storageAreaId}">Storage Area</a><br>`;
        html += `<a href="/index.html">Home</a><br>`;
        document.getElementById('createViewLinks').innerHTML = html;
    }
};