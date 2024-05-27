import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useGetAppointmentsMutation } from "../../redux/Api/apiSlice";
import { useSelector } from "react-redux";
import AppointmentCard from "./AppointmentCard";
import "./Appointments.scss";

const Appointments = () => {
    const user = useSelector((state) => state.auth);
    const [getAppointments] = useGetAppointmentsMutation();
    const [apps, setApps] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        (async () => {
            try {
                const doctorr = await getAppointments(user.id).unwrap();
                setApps(doctorr);
            } catch (e) {
                navigate("/");
            }
        })();
    }, [user.id]);

    return (
        <div className="d-flex flex-column justify-content-between w-100">
                <section class="content-section container">
                <h2>Upcomming appointments</h2></section>
                <div className="d-flex flex-wrap justify-content-between w-100">

            {apps.length == 0 ? (
                <p className="no-apps">No appointments yet ...</p>
            ) : (
                apps.map((x) => <AppointmentCard appointment={x} isPatient={user?.isPatient} key={x.id} />)
            )}
        </div>
        </div>
    );
};

export default Appointments;
