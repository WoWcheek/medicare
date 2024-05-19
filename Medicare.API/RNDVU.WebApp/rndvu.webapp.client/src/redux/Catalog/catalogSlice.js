import { createSlice } from "@reduxjs/toolkit"

const initialState = {
   specializations: []
}

export const catalogSlice = createSlice({
    name: "catalog",
    initialState,
    reducers: {
        setInfo: (state, action) => ({
            ...state,
            ...action.payload
        }),
    }
});

export const { setInfo } = catalogSlice.actions;

export default catalogSlice.reducer;