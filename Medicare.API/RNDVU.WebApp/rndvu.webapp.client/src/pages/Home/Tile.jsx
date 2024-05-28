function Tile({ color, icon, title, value }) {
    return (
        <div className="tile">
            <div className="icon" style={{ backgroundColor: color }}>
                {icon}
            </div>
            <h5 className="title">{title}</h5>
            <p className="value">{value}</p>
        </div>
    );
}

export default Tile;
