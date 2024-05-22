import { HiArrowRightOnRectangle } from "react-icons/hi2";
import ButtonIcon from "./ButtonIcon";

function Logout() {
    return (
        <ButtonIcon
            onClick={() => {
                console.log("logout");
            }}
        >
            <HiArrowRightOnRectangle />
        </ButtonIcon>
    );
}

export default Logout;
