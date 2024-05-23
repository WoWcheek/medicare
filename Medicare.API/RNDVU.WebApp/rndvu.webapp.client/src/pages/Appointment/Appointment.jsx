import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { DateCalendar, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs} from "@mui/x-date-pickers/AdapterDayjs";
import { FormControl,FormLabel, FormControlLabel, Radio, RadioGroup, ListItemButton, ListItemText, List, ListItem, Box } from "@mui/material";

const Appointment = () => {
    const params = useParams()
    const currDate = ()=>{
      const d=   new Date(Date.now());
      
      return new Date(d.toISOString().split('T')[0]);
    }  
    const history = useNavigate();

    if (!params.id) {
        history('/')
    }
    const shouldDisableDate = date=>{
        const currentDate = date.toISOString().split("T")[0];

        return date.toDate() < currDate();
       }
       const onChange = date=>{
        return Math.random() > 0.7;
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
        <div className="d-flex justify-content-between w-100">
            <div className="m-0">
            <LocalizationProvider  dateAdapter={AdapterDayjs}>
                <DateCalendar  shouldDisableDate={shouldDisableDate} onChange={onChange} />
            </LocalizationProvider>
            </div>
            <FormControl>
                <FormLabel id="demo-radio-buttons-group-label">Option</FormLabel>
                <RadioGroup
                    aria-labelledby="demo-radio-buttons-group-label"
                    defaultValue="male"
                    name="radio-buttons-group"
                >
                    <FormControlLabel value="male" control={<Radio />} label="Short (30 min)" />
                    <FormControlLabel value="other" control={<Radio />} label="Long (1 hour)" />
                </RadioGroup>
            </FormControl>
            <Box sx={{ maxWidth: 360, bgcolor: 'background.paper' }}>
                <List>
                    <ListItem disablePadding>
                        <ListItemButton disablePadding component="a" href="#simple-list">
                            <ListItemText primary="11:00" />
                        </ListItemButton>
                    </ListItem><ListItem disablePadding>
                        <ListItemButton disablePadding component="a" href="#simple-list">
                            <ListItemText primary="11:30" />
                        </ListItemButton></ListItem><ListItem disablePadding>
                        <ListItemButton disablePadding component="a" href="#simple-list">
                            <ListItemText primary="12:30" />
                        </ListItemButton></ListItem><ListItem disablePadding>
                        <ListItemButton disablePadding component="a" href="#simple-list">
                            <ListItemText primary="13:00" />
                        </ListItemButton>
                    </ListItem>
                </List></Box>
</div>

    );
};

export default Appointment;
