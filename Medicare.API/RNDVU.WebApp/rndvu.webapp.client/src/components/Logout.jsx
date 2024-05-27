import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { HiArrowRightOnRectangle } from "react-icons/hi2";
import { logout } from "../redux/Auth/authSlice";
import asyncLocalStorage from "../helpers/asyncLocalStorage";
import ButtonIcon from "./ButtonIcon";

function Logout() {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const logoutHandler = async (e) => {
        e.preventDefault();
        localStorage.removeItem("isPatient");
        asyncLocalStorage.removeItem("token").then(() => {
            Promise.resolve()
                .then(function () {
                    return dispatch(logout());
                })
                .then(() => navigate("/login"));
        });
    };

    return (
        <ButtonIcon onClick={logoutHandler}>
            <HiArrowRightOnRectangle />
        </ButtonIcon>
    );
}

export default Logout;
