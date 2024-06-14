import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { DateCalendar, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import {
    FormControl,
    FormLabel,
    FormControlLabel,
    Radio,
    RadioGroup,
    ListItemButton,
    ListItemText,
    List,
    ListItem
} from "@mui/material";
import {
    useGetDoctorTimesMutation,
    useMakeAppointmentMutation
} from "../../redux/Api/apiSlice";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";
import "./Appointment.scss";

function Appointment() {
    const params = useParams();
    const [times, setTimes] = useState([]);
    const [pickedDate, setPickedDate] = useState(null);
    const timesInitial = [
        "10:00",
        "10:30",
        "11:00",
        "11:30",
        "12:00",
        "12:30",
        "13:00",
        "13:30",
        "14:00",
        "14:30",
        "15:00",
        "15:30",
        "16:00",
        "16:30",
        "17:00",
        "17:30"
    ];
    const isShort = useRef(null);
    const user = useSelector((state) => state.auth);
    const [doctorTimes, setDoctorTimes] = useState(null);
    const [getDoctorTimes] = useGetDoctorTimesMutation();
    const currDate = () => {
        const d = new Date(Date.now());

        return new Date(d.toISOString().split("T")[0]);
    };
    const history = useNavigate();

    if (!params.id) {
        history("/");
    }

    const shouldDisableDate = useCallback(
        (date) => {
            const currentDate = date.toISOString().split("T")[0];

            return (
                date.toDate() < currDate() ||
                (!doctorTimes ? true : dayAvailable(currentDate))
            );
        },
        [doctorTimes]
    );

    const dayAvailable = (date) => {
        let dateee = date;
        if (doctorTimes.some((x) => x.date.split("T")[0] == dateee)) {
            let gs = null;
            if (isShort.current.checked) {
                gs = timesInitial.filter((time, index) =>
                    !doctorTimes.some((x) => x.date.split("T")[0] == dateee)
                        ? true
                        : !doctorTimes.find(
                              (x) => x.date.split("T")[0] == dateee
                          ).available[index]
                );
            } else {
                const gdf = (time, index) => {
                    const curr = doctorTimes.find(
                        (x) => x.date.split("T")[0] == dateee
                    );
                    return !doctorTimes.some(
                        (x) => x.date.split("T")[0] == dateee
                    )
                        ? true
                        : !curr.available[index] &&
                              !(index == 15 ? true : curr.available[index + 1]);
                };
                gs = timesInitial.filter(gdf);
            }
            return gs.length == 0;
        } else {
            return false;
        }
    };

    const onChange = (date) => {
        if (!date.bubbles) {
            setPickedDate(date.toDate());
        }
        if ((!date.bubbles && date.toDate()) || pickedDate) {
            let dateee = (!date.bubbles && date.toDate()) || pickedDate;
            if (
                doctorTimes.some(
                    (x) =>
                        x.date.split("T")[0] ==
                        dateee.toISOString().split("T")[0]
                )
            ) {
                let gs = null;
                if (isShort.current.checked) {
                    gs = timesInitial.filter((time, index) =>
                        !doctorTimes.some(
                            (x) =>
                                x.date.split("T")[0] ==
                                dateee.toISOString().split("T")[0]
                        )
                            ? true
                            : !doctorTimes.find(
                                  (x) =>
                                      x.date.split("T")[0] ==
                                      dateee.toISOString().split("T")[0]
                              ).available[index]
                    );
                } else {
                    const gdf = (time, index) => {
                        const curr = doctorTimes.find(
                            (x) =>
                                x.date.split("T")[0] ==
                                dateee.toISOString().split("T")[0]
                        );
                        return !doctorTimes.some(
                            (x) =>
                                x.date.split("T")[0] ==
                                dateee.toISOString().split("T")[0]
                        )
                            ? true
                            : !curr.available[index] &&
                                  !(index == 15
                                      ? true
                                      : curr.available[index + 1]);
                    };
                    gs = timesInitial.filter(gdf);
                }
                setTimes(gs);
            } else {
                setTimes(
                    isShort.current.checked
                        ? timesInitial
                        : timesInitial.filter((time, index) => index != 15)
                );
            }
        }
    };

    const [makeAppointment] = useMakeAppointmentMutation();
    const makeApp = useCallback(
        async (time) => {
            try {
                await makeAppointment({
                    date: pickedDate,
                    time,
                    isShort: isShort.current.checked,
                    doctorId: params.id,
                    userId: user.id
                }).unwrap();
                toast.success("Appointment is created!");
                history("/");
            } catch {
                console.error("error occurred while making an appointment");
            }
        },
        [pickedDate]
    );

    useEffect(() => {
        (async () => {
            try {
                const doctorr = await getDoctorTimes(params.id).unwrap();
                setDoctorTimes(doctorr);
            } catch {
                history("/");
            }
        })();
    }, []);

    return (
        <>
            <h1 className="mb-5">Choose date and time of your appointment</h1>
            <div className="d-flex justify-content-between w-100">
                <div>
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <DateCalendar
                            shouldDisableDate={shouldDisableDate}
                            onChange={onChange}
                        />
                    </LocalizationProvider>
                    <div className="mx-4">
                        <FormControl>
                            <FormLabel id="demo-radio-buttons-group-label">
                                Meeting duration
                            </FormLabel>
                            <RadioGroup
                                aria-labelledby="demo-radio-buttons-group-label"
                                defaultValue="short"
                                name="radio-buttons-group"
                                onChange={onChange}
                            >
                                <FormControlLabel
                                    value="short"
                                    control={<Radio inputRef={isShort} />}
                                    label="Short (30 min)"
                                />
                                <FormControlLabel
                                    value="long"
                                    control={<Radio />}
                                    label="Long (1 hour)"
                                />
                            </RadioGroup>
                        </FormControl>
                    </div>
                </div>
                <div className="times">
                    <h5 className="mb-3">Available start times</h5>
                    <List>
                        {times.map((x, i) => (
                            <ListItem disablePadding key={`listitem-${i}`}>
                                <ListItemButton
                                    onClick={() => makeApp(x)}
                                    component="a"
                                    href="#simple-list"
                                >
                                    <ListItemText primary={x} />
                                </ListItemButton>
                            </ListItem>
                        ))}
                    </List>
                </div>
            </div>
        </>
    );
}

export default Appointment;
