import { useNavigate } from "react-router-dom";

function DoctorCard({ doctor }) {
    const navigate = useNavigate();

    return (
        <div className="card">
            <img
                className="card-img-top"
                src={
                    doctor?.avatar ||
                    "./src/assets/images/default-user-image.png"
                }
                alt="Doctor"
            />
            <div className="card-body">
                <h5 className="card-title">{doctor.fullName}</h5>
                <p className="card-text">
                    {doctor?.specializations.length === 0
                        ? "No specialization"
                        : doctor.specializations[0] + " ..."}
                </p>
                <button
                    className="doctor-btn"
                    onClick={() => navigate("/doctor/" + doctor.id)}
                >
                    Doctor page {">>"}
                </button>
            </div>
        </div>
    );
}

export default DoctorCard;
