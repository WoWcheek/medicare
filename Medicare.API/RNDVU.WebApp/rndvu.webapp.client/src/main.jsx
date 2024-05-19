import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./assets/index.scss";
import { store } from "./redux/store";
import { Provider } from "react-redux";

import {BrowserRouter} from 'react-router-dom'
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

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
