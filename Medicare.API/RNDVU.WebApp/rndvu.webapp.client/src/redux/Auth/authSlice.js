import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    id: null,
    token: ""
};

export const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        setUser: (state, action) => ({
            ...state,
            ...action.payload
        }),
        refreshToken: (state, action) => ({
            ...state,
            token: action.payload.token
        }),
        logout: () => initialState
    }
});

export const { setUser, refreshToken, logout } = authSlice.actions;

export default authSlice.reducer;
