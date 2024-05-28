function AppointmentCard({ appointment, isPatient }) {
    return (
        <div className="card mb-3 app-card">
            <img
                className="card-img-top"
                src={
                    (isPatient
                        ? appointment.appointment.doctor.avatar
                        : appointment.appointment.user.avatar) ??
                    "src/assets/images/default-user-image.png"
                }
                alt={`${appointment?.appointment?.doctor?.fullName}'s avatar`}
            />
            <div className="card-body">
                <h5 className="card-title">
                    {isPatient
                        ? appointment.appointment.doctor.fullName
                        : appointment.appointment.user.fullName}
                </h5>
                {isPatient && (
                    <p className="card-text mb-2">
                        {appointment.specializations.join(", ")}
                    </p>
                )}
            </div>
            <div className="card-body">
                <div className="date-time-flex">
                    <p>
                        {new Date(appointment.appointment.date.split("T")[0])
                            .toDateString()
                            .substring(4)}
                    </p>
                    <p>{appointment.appointment.time}</p>
                </div>
                <div className="duration-link-flex">
                    <p className="m-0">
                        Duration:
                        {appointment.appointment.isShort
                            ? " 30 min"
                            : " 60 min"}
                    </p>
                    <a href={appointment.appointment.url} className="zoom-link">
                        Access zoom link
                    </a>
                </div>
            </div>
        </div>
    );
}

export default AppointmentCard;
