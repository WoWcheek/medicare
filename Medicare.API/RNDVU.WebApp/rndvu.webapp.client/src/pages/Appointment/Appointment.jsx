import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { DateCalendar, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import de  from 'date-fns/locale/de';
const Appointment = () => {
    const params = useParams()

    const history = useNavigate();

    if (!params.id) {
        history('/')
    }

    // const [doctor, setDoctor] = useState(null);
    // const [getDoctor] = useGetDoctorMutation();

    // useEffect(() => {
    //     (async () => {
    //         try {
    //             const doctorr = await getDoctor(params.id).unwrap();
    //             setDoctor(doctorr);
    //         }
    //         catch {
    //             history('/')
    //         }
    //     })();
    // }, []);


    // if (!doctor)
    //     return (<div className="d-flex justify-content-center w-100">
    //         <div className="spinner-grow" style={{ "width": "3rem", "height": "3rem" }} role="status">
    //         </div>
    //     </div>);

    return (
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={de}>
        <DateCalendar
        label="Select a date"
        // value={selectedDate}
        // onChange={newValue => setSelectedDate(newValue)}
        // shouldDisableDate={shouldDisableDate}
        // renderInput={params => <TextField {...params}
        //  />}
      />
        </LocalizationProvider>

    );
};

export default Appointment;
