import { configureStore } from "@reduxjs/toolkit";
import { apiSlice } from "./Api/apiSlice";
import authReducer from "./Auth/authSlice";
import catalogReducer from "./Catalog/catalogSlice";

export const store = configureStore({
    reducer: {
        [apiSlice.reducerPath]: apiSlice.reducer,
        auth: authReducer,
        catalog: catalogReducer
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(apiSlice.middleware),
    devTools: true
});
