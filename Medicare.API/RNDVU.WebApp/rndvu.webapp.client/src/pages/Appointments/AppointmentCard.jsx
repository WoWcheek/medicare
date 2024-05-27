function AppointmentCard({ appointment, isPatient }) {
    return (
        <div className="card mt-5" style={{"width": "18rem"}}>
            <img className="card-img-top" src={(isPatient ? appointment.appointment.doctor.avatar: appointment.appointment.user.avatar)??"src/assets/images/default-user-image.png"} alt="Card image cap" />
            <div className="card-body">
                <h5 className="card-title">{(isPatient ? appointment.appointment.doctor.fullName : appointment.appointment.user.fullName)}</h5>
                {isPatient && <p className="card-text">{appointment.specializations.join(', ')}</p>}
            </div>
            <ul className="list-group list-group-flush">
                <li className="list-group-item">{appointment.appointment.date.split('T')[0]}</li>
                <li className="list-group-item">{appointment.appointment.time}</li>
                <li className="list-group-item">Duration:{appointment.appointment.isShort ? " 30 min." :" 60 min."}</li>
            </ul>
            <div className="card-body">
                <a href={appointment.appointment.url} className="card-link">Zoom link</a>
            </div>
        </div>
    );
}

export default AppointmentCard;
