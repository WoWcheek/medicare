import { useEffect } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { useGetInfoMutation, useGetUserMutation } from "../redux/Api/apiSlice";
import { useDispatch } from "react-redux";
import { setUser } from "../redux/Auth/authSlice";
import PersonalPage from "../pages/PersonalPage/PersonalPage";
import SideBar from "./SideBar";
import "./AppLayout.scss";
import { setInfo } from "../redux/Catalog/catalogSlice";
import Header from "./Header";
import Doctors from "../pages/Doctors/Doctors";

const AuthorizedRoutes = ({ isPatient }) => {
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

    return (
        <div className="app-layout">
            <SideBar />
            <Header />
            <main>
                <Routes>
                    <Route exact path="/" element={<div>main content</div>} />
                    <Route exact path="/personal" element={<PersonalPage />} />
                    {isPatient && <Route exact path="/doctors" element={<Doctors />} /> }
                    <Route path="*" element={<Navigate to="/" />} />
                </Routes>
            </main>
        </div>
    );
};

export default AuthorizedRoutes;
