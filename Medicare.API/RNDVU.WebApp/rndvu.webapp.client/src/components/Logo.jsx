import PropTypes from "prop-types";
import { useColorScheme } from "../helpers/useColorScheme";
import { useEffect } from "react";

function Logo({ logoSize, fontSize }) {
    const { isDark } = useColorScheme();

    const flexCssStyle = {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        gap: "10%"
    };

    const h1CssStyle = {
        textTransform: "uppercase",
        fontSize: fontSize || "2rem",
        color: "var(--main-text-color)",
        marginTop: "5px",
        letterSpacing: "2px"
    };

    return (
        <div style={flexCssStyle} className="logo-container">
            <img
                src={
                    isDark
                        ? "src/assets/images/logo-light.png"
                        : "src/assets/images/logo-dark.png"
                }
                alt="medicare logo"
                height={logoSize || 50}
            />
            <h1 style={h1CssStyle}>MediCare</h1>
        </div>
    );
}

export default Logo;

Logo.propTypes = {
    logoSize: PropTypes.number,
    fontSize: PropTypes.string
};
