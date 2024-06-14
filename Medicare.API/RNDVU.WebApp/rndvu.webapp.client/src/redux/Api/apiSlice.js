import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const baseQuery = fetchBaseQuery({
    baseUrl: "https://localhost:7012/api",
    credentials: "same-origin",
    prepareHeaders: (headers, { getState }) => {
        const token = localStorage.getItem("token") || getState().auth.token;
        if (token) headers.set("authorization", `Bearer ${token}`);
        return headers;
    }
});

const baseQueryWithReAuth = async (args, api, extraOptions) => {
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
    baseQuery: baseQueryWithReAuth,
    endpoints: () => ({})
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
            })
        }),
        login: builder.mutation({
            query: (creds) => ({
                url: "/auth/login",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        getUser: builder.mutation({
            query: () => ({
                url: "/user/getUser",
                method: "GET"
            })
        }),
        setAvatar: builder.mutation({
            query: (creds) => ({
                url: "/user/ChangeAvatar",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        changePassword: builder.mutation({
            query: (creds) => ({
                url: "/user/ChangePassword",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        editProfile: builder.mutation({
            query: (creds) => ({
                url: "/user/EditUser",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        getDoctors: builder.mutation({
            query: (creds) => ({
                url: "/user/GetDoctors",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        getInfo: builder.mutation({
            query: () => ({
                url: "/catalog/getInfo",
                method: "GET"
            })
        }),
        makeAppointment: builder.mutation({
            query: (creds) => ({
                url: "/appointment/makeAppointment",
                method: "POST",
                body: {
                    ...creds
                }
            })
        }),
        getAppointments: builder.mutation({
            query: (creds) => ({
                url: "/appointment/GetAppointments?id=" + creds,
                method: "GET"
            })
        }),
        getDoctor: builder.mutation({
            query: (creds) => ({
                url: "/user/GetDoctor?id=" + creds,
                method: "GET"
            })
        }),
        getDoctorTimes: builder.mutation({
            query: (creds) => ({
                url: "/appointment/GetAvailableDoctorAppointments?id=" + creds,
                method: "GET"
            })
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
    useGetDoctorsMutation,
    useGetInfoMutation
} = authApiSlice;
