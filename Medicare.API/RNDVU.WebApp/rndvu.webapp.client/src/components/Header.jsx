import "./Header.scss";
import UserAvatar from "./UserAvatar";
import HeaderMenu from "./HeaderMenu";

function Header() {
    return (
        <header>
            <UserAvatar />
            <HeaderMenu />
        </header>
    );
}

export default Header;
