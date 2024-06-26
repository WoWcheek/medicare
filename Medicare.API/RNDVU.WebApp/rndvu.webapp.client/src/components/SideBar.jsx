import { useSelector } from "react-redux";
import { NavLink } from "react-router-dom";
import Logo from "./Logo";
import "./SideBar.scss";
import { AiFillHome } from "react-icons/ai";
import { FaUserDoctor } from "react-icons/fa6";
import { MdCalendarMonth } from "react-icons/md";
import { MdOutlineHelp } from "react-icons/md";

function SideBar() {
    const currentYear = 1900 + new Date().getYear();

    const isPatient =
        useSelector((state) => state.auth.isPatient) ||
        localStorage.getItem("isPatient") === "true";

    return (
        <>
            <nav>
                <Logo fontSize={"1.5rem"} logoSize={40} />
                <hr />
                <ul>
                    <li>
                        <NavLink to="/">
                            <AiFillHome />
                            <span>Home</span>
                        </NavLink>
                    </li>
                    {isPatient && (
                        <li>
                            <NavLink to="/doctors">
                                <FaUserDoctor />
                                <span>Doctors</span>
                            </NavLink>
                        </li>
                    )}
                    <li>
                        <NavLink to="/appointments">
                            <MdCalendarMonth />
                            <span>Appointments</span>
                        </NavLink>
                    </li>
                    <li>
                        <NavLink to="/help">
                            <MdOutlineHelp />
                            <span>Help</span>
                        </NavLink>
                    </li>
                </ul>
            </nav>
            <p className="copyright">&copy; {currentYear} Medicare</p>
        </>
    );
}

export default SideBar;
