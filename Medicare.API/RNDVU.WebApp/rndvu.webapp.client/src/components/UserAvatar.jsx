import { useSelector } from "react-redux";
import defaultIcon from "../assets/images/default-user-image.png";
import "./UserAvatar.scss";

function UserAvatar() {
    const user = useSelector((state) => state.auth);

    return (
        <div className="user-avatar">
            <img
                src={user.avatar || defaultIcon}
                alt={`Avatar of ${user.fullName}`}
            />
            <span>{user.fullName}</span>
        </div>
    );
}

export default UserAvatar;
