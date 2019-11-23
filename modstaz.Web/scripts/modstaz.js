var apiBaseUrl = "http://localhost:7071";


function createInstance() {

    console.log("Creating instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseSetup`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function destroyInstance() {
    console.log("Destroying instance!");
    var request = new XMLHttpRequest();
    request.open('GET', `${apiBaseUrl}/api/DatabaseDestroy`, true);

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
    request.send();
}

function isUserAuthenticated() {
    var jwt = localStorage.getItem('JWT');

    if (jwt === null) {
        redirectToLogin();
    }

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/ParseJWT`, true);

    let data = {
        "JWT": jwt
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        if (resp == "Invalid JWT")
        {
            redirectToLogin();
        }
    };

    request.onerror = function () { };
}


function redirectToLogin() {
    window.location.replace("/login.html");
}

function redirectToHome() {
    window.location.replace("/index.html");
}

function getGetParameters() {
    var qd = {};
    if (location.search) location.search.substr(1).split("&").forEach(function (item) {
        var s = item.split("="),
            k = s[0],
            v = s[1] && decodeURIComponent(s[1]); //  null-coalescing / short-circuit
        (qd[k] = qd[k] || []).push(v) // null-coalescing / short-circuit
    })

    return qd;
}



