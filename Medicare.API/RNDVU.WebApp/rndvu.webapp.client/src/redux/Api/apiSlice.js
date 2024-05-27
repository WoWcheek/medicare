import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { setUser } from "../Auth/authSlice";

const baseQuery = fetchBaseQuery({
    baseUrl: "https://localhost:7012/api",
    credentials: "same-origin",
    prepareHeaders: (headers, { getState }) => {
        const token = localStorage.getItem("token") || getState().auth.token;

        if (token) {
            headers.set("authorization", `Bearer ${token}`);
        }
        // headers.set( 'Content-Type', 'application/json');
        return headers;
    }
});

const baseQueryWithReauth = async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions);
    if (result?.error?.originalStatus === 403) {
        const refreshResult = await baseQuery("/refresh", api, extraOptions);

        if (refreshResult?.data) {
            const user = api.getState().auth.user;
            api.dispatch(setCredentials({ ...refreshResult.data, user }));

            result = await baseQuery(args, api, extraOptions);
        } else {
            api.dispatch(logout());
        }
    }

    return result;
};

export const apiSlice = createApi({
    baseQuery: baseQueryWithReauth,
    endpoints: (builder) => ({})
});

export const authApiSlice = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        register: builder.mutation({
            query: (creds) => ({
                url: "/auth/register",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        login: builder.mutation({
            query: (creds) => ({
                url: "/auth/login",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        getUser: builder.mutation({
            query: () => ({
                url: "/user/getUser",
                method: "GET"
            }),
            extraOptions: {}
        }),
        setAvatar: builder.mutation({
            query: (creds) => ({
                url: "/user/ChangeAvatar",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        changePassword: builder.mutation({
            query: (creds) => ({
                url: "/user/ChangePassword",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        editProfile: builder.mutation({
            query: (creds) => ({
                url: "/user/EditUser",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        getDoctors: builder.mutation({
            query: (creds) => ({
                url: "/user/GetDoctors",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        makeAppointment: builder.mutation({
            query: (creds) => ({
                url: "/appointment/makeAppointment",
                method: "POST",
                body: {
                    ...creds
                }
            }),
            extraOptions: {}
        }),
        getAppointments: builder.mutation({
            query: (creds) => ({
                url: "/appointment/getAppointments?id=" + creds,
                method: "GET"
            }),
            extraOptions: {}
        }),
        getDoctor: builder.mutation({
            query: (creds) => ({
                url: "/user/GetDoctor?id=" + creds,
                method: "GET"
            }),
            extraOptions: {}
        }),
        getDoctorTimes: builder.mutation({
            query: (creds) => ({
                url: "/appointment/GetDoctorAppointments?id=" + creds,
                method: "GET"
            }),
            extraOptions: {}
        })
    })
});

export const {
    useLoginMutation,
    useRegisterMutation,
    useGetUserMutation,
    useMakeAppointmentMutation,
    useSetAvatarMutation,
    useGetDoctorTimesMutation,
    useGetAppointmentsMutation,
    useChangePasswordMutation,
    useEditProfileMutation,
    useGetDoctorMutation,
    useGetDoctorsMutation
} = authApiSlice;
