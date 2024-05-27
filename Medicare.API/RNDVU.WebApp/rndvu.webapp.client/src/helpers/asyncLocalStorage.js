const asyncLocalStorage = {
    setItem(key, value) {
        return Promise.resolve().then(function () {
            localStorage.setItem(key, value);
        });
    },
    getItem(key) {
        return Promise.resolve().then(function () {
            return localStorage.getItem(key);
        });
    },
    removeItem(key) {
        return Promise.resolve().then(function () {
            return localStorage.removeItem(key);
        });
    }
};
export default asyncLocalStorage;
