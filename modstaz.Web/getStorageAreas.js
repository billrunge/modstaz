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
            var html = storageAreasJsonToTable(JSON.parse(this.response), "storageAreasList");
            console.log(html);
    
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