import "./ButtonIcon.scss";

function ButtonIcon({ children, onClick }) {
    return (
        <button className="btn-icon" onClick={onClick}>
            {children}
        </button>
    );
}

export default ButtonIcon;
