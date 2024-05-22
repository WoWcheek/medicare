import { useState } from "react";
import { HiOutlineMoon, HiOutlineSun } from "react-icons/hi2";
import ButtonIcon from "./ButtonIcon";

function DarkModeToggle() {
    // const { isDarkMode, toggleDarkMode } = useDarkMode();
    const [isDarkMode, setIsDarkMode] = useState(false);

    return (
        <ButtonIcon onClick={() => setIsDarkMode((x) => !x)}>
            {isDarkMode ? <HiOutlineSun /> : <HiOutlineMoon />}
        </ButtonIcon>
    );
}

export default DarkModeToggle;
