logout();

function logout(){
    localStorage.removeItem('JWT');
    redirectToLogin();
    
}