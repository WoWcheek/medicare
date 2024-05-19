import React, { useEffect } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { useGetInfoMutation, useGetUserMutation } from '../redux/Api/apiSlice';
import { useDispatch } from 'react-redux';
import { setUser } from "../redux/Auth/authSlice";
import PersonalPage from '../pages/PersonalPage/PersonalPage';
import { setInfo } from '../redux/Catalog/catalogSlice';

const AuthorizedRoutes = () => {
    const dispatch = useDispatch();
    const [getUser] = useGetUserMutation();
    const [getInfo] = useGetInfoMutation();
   
    useEffect(() => {
        (async () => {
            const userData = await getUser().unwrap();
            dispatch(setUser(userData));
            const allData = await getInfo().unwrap();
            dispatch(setInfo(allData));
        })();
    }, []);
  
    return (<Routes>
        <Route exact path="/" element={<div>wow</div>} />
        <Route exact path="/personal" element={<PersonalPage />} />
           <Route path='*' element={<Navigate to="/" />} /> 
    </Routes>);
}

export default AuthorizedRoutes;
