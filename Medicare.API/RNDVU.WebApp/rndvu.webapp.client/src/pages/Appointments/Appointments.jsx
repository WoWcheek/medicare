import React, { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate, useNavigation, useParams } from "react-router-dom";
import { DateCalendar, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs} from "@mui/x-date-pickers/AdapterDayjs";
import { FormControl,FormLabel, FormControlLabel, Radio, RadioGroup, ListItemButton, ListItemText, List, ListItem, Box } from "@mui/material";
import { useGetAppointmentsMutation, useGetDoctorTimesMutation, useMakeAppointmentMutation } from "../../redux/Api/apiSlice";
import { useSelector } from "react-redux";

const Appointments = () => {
    const user = useSelector((state) => state.auth);
    const [getAppointments] = useGetAppointmentsMutation();
    const [apps, setApps] = useState([]);
    const history = useNavigate();
    useEffect(() => {
        (async () => {
            try {
                const doctorr = await getAppointments(user.id).unwrap();
                setApps(doctorr);
            }
            catch (e) {
                history('/')
            }
        })();
    }, [user.id]);



    return (
        <div className="d-flex justify-content-between w-100">
          {apps.length ==0 && <div>No appointments</div>}
          {apps.map(x=>(
              <div className="">{x.appointment.doctor.fullName}</div>


          ))}
        </div>
    );
};

export default Appointments;
