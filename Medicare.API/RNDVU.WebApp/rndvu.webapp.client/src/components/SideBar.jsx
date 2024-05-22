import { NavLink } from "react-router-dom";
import Logo from "./Logo";
import "./SideBar.scss";
import { AiFillHome } from "react-icons/ai";
import { FaUserDoctor } from "react-icons/fa6";
import { IoCalendar } from "react-icons/io5";
import { MdOutlineHelp } from "react-icons/md";

function SideBar() {
    return (
        <nav>
            <Logo />
            <hr />
            <ul>
                <li>
                    <NavLink to="/">
                        <AiFillHome />
                        <span>Home</span>
                    </NavLink>
                </li>
                <li>
                    <NavLink to="/doctors">
                        <FaUserDoctor />
                        <span>Doctors</span>
                    </NavLink>
                </li>
                <li>
                    <NavLink to="/appointments">
                        <IoCalendar />
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
    );
}

export default SideBar;
