import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    id: null,
    token: ""
}

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
        logout: (state, action) => (initialState),
        setAuthStepsModel: (state, action) => {
            const stepIndex = action.payload.step - 1;
            const stepModel = action.payload.model;
            const currentStep = action.payload.currentStep;
            const newState = {
                ...state,
                stepModel: [...state.stepModel],
                currentStep
            };
            newState.stepModel[stepIndex] = stepModel;

            return newState;
        }
    }
});

export const { 
    setUser, 
    refreshToken,
    setAuthStepsModel ,
    logout
} = authSlice.actions;

export default authSlice.reducer;