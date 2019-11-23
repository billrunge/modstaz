getStorageAreas();

function getStorageAreas() {
    let jwt = localStorage.getItem('JWT');

    if (jwt != undefined){

        var request = new XMLHttpRequest();
        request.open('POST', `${apiBaseUrl}/api/GetStorageAreas`, true);
    
        let data = {
            "JWT": jwt
        };
    
        request.send(JSON.stringify(data));
    
        request.onload = function () {
            let resp = this.response;
            if (resp == "Invalid JWT")
            {
                redirectToLogin();
            }
            var html = storageAreasJsonToTable(JSON.parse(resp), "storageAreasList");
    
            document.getElementById('storageAreas').innerHTML = html;
        };
    
        request.onerror = function () {     
            redirectToLogin();
        };
    }
    else {
        redirectToLogin();
    }
}