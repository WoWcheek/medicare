import { useNavigate } from "react-router-dom";
import { HiOutlineUser } from "react-icons/hi2";
import ButtonIcon from "./ButtonIcon";
import Logout from "./Logout";
import DarkModeToggle from "./DarkModeToggle";

function HeaderMenu() {
    const navigate = useNavigate();

    return (
        <ul className="header-menu">
            <li>
                <ButtonIcon onClick={() => navigate("/personal")}>
                    <HiOutlineUser />
                </ButtonIcon>
            </li>
            <li>
                <DarkModeToggle />
            </li>
            <li>
                <Logout />
            </li>
        </ul>
    );
}

export default HeaderMenu;
