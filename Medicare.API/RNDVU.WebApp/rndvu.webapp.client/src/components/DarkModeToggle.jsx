import { useEffect } from "react";
import { HiOutlineMoon, HiOutlineSun } from "react-icons/hi2";
import { useColorScheme } from "../helpers/useColorScheme.js";
import ButtonIcon from "./ButtonIcon";

function DarkModeToggle() {
    const { isDark, setIsDark } = useColorScheme();

    useEffect(() => {
        if (isDark) document.body.classList.add("dark");
        else document.body.classList.remove("dark");
    }, [isDark]);

    return (
        <ButtonIcon onClick={() => setIsDark((x) => !x)}>
            {isDark ? <HiOutlineSun /> : <HiOutlineMoon />}
        </ButtonIcon>
    );
}

export default DarkModeToggle;
