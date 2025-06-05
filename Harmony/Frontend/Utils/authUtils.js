export function isTokenExpired(token) {
    if (!token) return true;
    
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const exp = payload.exp;
        if (!exp) return true;
        return Date.now() >= exp * 1000;
    } catch (e) {
        return true;
    }
}

export function checkTokenAndRedirect() {
    const token = localStorage.getItem('token');
    if (!token || isTokenExpired(token)) {
        localStorage.clear();
        alert('Ваша сессия истекла. Пожалуйста, войдите снова.');
        window.location.href = '../UserFr/login_user.html';
        return false;
    }
    return true;
}
