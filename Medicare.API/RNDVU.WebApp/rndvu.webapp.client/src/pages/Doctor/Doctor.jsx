import React, { useEffect, useState } from "react";
import { useGetDoctorMutation } from "../../redux/Api/apiSlice";
import { useNavigate, useParams } from "react-router-dom";

const Doctor = () => {
    const params = useParams()

    const history = useNavigate();

    if (!params.id) {
        history('/')
    }

    const [doctor, setDoctor] = useState(null);
    const [getDoctor] = useGetDoctorMutation();

    useEffect(() => {
        (async () => {
            try {
                const doctorr = await getDoctor(params.id).unwrap();
                setDoctor(doctorr);
            }
            catch {
                history('/')
            }
        })();
    }, []);


    if (!doctor)
        return (<div className="d-flex justify-content-center w-100">
            <div className="spinner-grow" style={{ "width": "3rem", "height": "3rem" }} role="status">
            </div>
        </div>);

    return (
        <div className="container">
            <div className="row justify-content-center">
                <div className="col-md-7 col-lg-4 mb-5 mb-lg-0 wow fadeIn">
                    <div className="card border-0 shadow">
                        <img src={doctor.avatar} alt="..." />
                        <div className="card-body p-1-9 p-xl-5">
                            <div className="mb-4">
                                <h3 className="h4 mb-0">{doctor.fullName}</h3>
                                <h3 className="h4 mb-0">{doctor.specializations.join(
                                    ", "
                                )}</h3>
                            </div>
                            <ul className="list-unstyled mb-4">
                                <li className="mb-3"><a href="#!"><i className="far fa-envelope display-25 me-3 text-secondary"></i>{doctor.email}</a></li>
                                {doctor.phoneNumber && <li className="mb-3"><a href="#!"><i className="fas fa-mobile-alt display-25 me-3 text-secondary"></i>{doctor.phoneNumber}</a></li>}

                            </ul>
                        </div>
                    </div>
                </div>
                <div className="col-lg-8">
                    <div className="ps-lg-1-6 ps-xl-5">
                        <div className="mb-5 wow fadeIn">
                            <div className="text-start mb-1-6 wow fadeIn">
                                <h2 className="h1 mb-0 text-primary">About Me</h2>
                            </div>
                            <p>
                                {doctor.description}</p>
                        </div>
                        <button className="btn btn-primary" onClick={() => history('/appointment/' + params.id)}> Take an appointment</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Doctor;
