import { useEffect, useState } from "react";
import { useGetDoctorMutation } from "../../redux/Api/apiSlice";
import { useNavigate, useParams } from "react-router-dom";
import "./Doctor.scss";
import defaultAvatar from "../../assets/images/default-user-image.png";

const Doctor = () => {
    const params = useParams();
    const navigate = useNavigate();

    if (!params.id) {
        navigate("/");
    }

    const [doctor, setDoctor] = useState(null);
    const [getDoctor] = useGetDoctorMutation();

    useEffect(() => {
        (async () => {
            try {
                const doctorr = await getDoctor(params.id).unwrap();
                setDoctor(doctorr);
            } catch {
                navigate("/");
            }
        })();
    }, []);

    if (!doctor)
        return (
            <div className="d-flex justify-content-center w-100">
                <div className="spinner-grow" role="status"></div>
            </div>
        );

    return (
        <div className="container doctor">
            <div className="row justify-content-center">
                <div className="col-md-7 col-lg-4 mb-5 mb-lg-0 wow fadeIn">
                    <div className="card border-0 shadow">
                        <img
                            src={doctor?.avatar || defaultAvatar}
                            alt={`${doctor.fullName}'s avatar`}
                        />
                        <div className="card-body p-1-8 p-xl-4">
                            <div className="mb-4">
                                <h4 className="mb-3">{doctor.fullName}</h4>
                                <h5>{doctor.specializations.join(", ")}</h5>
                            </div>
                            <ul className="list-unstyled mb-0">
                                <li>
                                    <i className="far fa-envelope display-25 me-3 text-secondary">
                                        {doctor.email}
                                    </i>
                                </li>
                                {doctor.phoneNumber && (
                                    <li>
                                        <i className="fas fa-mobile-alt display-25 me-3 text-secondary">
                                            {doctor.phoneNumber}
                                        </i>
                                    </li>
                                )}
                            </ul>
                        </div>
                    </div>
                </div>
                <div className="col-lg-8">
                    <div className="ps-lg-1-6 ps-xl-5">
                        <div className="mb-5 wow fadeIn">
                            <div className="text-start mb-1-6 wow fadeIn">
                                <h2 className="h1 mb-0">About me</h2>
                            </div>
                            <p>{doctor.description}</p>
                        </div>
                        <button
                            className="btn"
                            onClick={() =>
                                navigate("/appointment/" + params.id)
                            }
                        >
                            Make an appointment
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Doctor;
