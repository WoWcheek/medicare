import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { store } from "./redux/store";
import { Provider } from "react-redux";
import App from "./App";
import "./assets/index.scss";

ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <ToastContainer />
        <Provider store={store}>
            <BrowserRouter>
                <App />
            </BrowserRouter>
        </Provider>
    </React.StrictMode>
);
