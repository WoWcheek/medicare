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
                const appointments = await getAppointments(user.id).unwrap();
                setApps(appointments);
            } catch (e) {
                navigate("/");
            }
        })();
    }, [user.id]);

    return (
        <div className="d-flex flex-column justify-content-between w-100">
            <h2 className="appointments-h">
                {apps.length === 0
                    ? "No appointments yet ..."
                    : "Upcoming appointments:"}
            </h2>
            <div className="d-flex flex-wrap justify-content-between w-100">
                {apps.map((x) => (
                    <AppointmentCard
                        appointment={x}
                        isPatient={user?.isPatient}
                        key={x.id}
                    />
                ))}
            </div>
        </div>
    );
};

export default Appointments;
