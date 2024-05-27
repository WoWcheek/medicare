import { useEffect, useState } from "react";
import { useGetDoctorsMutation } from "../../redux/Api/apiSlice";
import {
    Checkbox,
    FormControl,
    InputLabel,
    ListItemText,
    MenuItem,
    OutlinedInput,
    Select
} from "@mui/material";
import { useSelector } from "react-redux";
import DoctorCard from "./DoctorCard";
import "./Doctors.scss";

const Doctors = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [page, setPage] = useState(0);
    const [allCount, setAllCount] = useState(-1);
    const [doctors, setDoctors] = useState([]);
    const [getDoctors] = useGetDoctorsMutation();
    const catalog = useSelector((state) => state.catalog);

    const ITEM_HEIGHT = 48;
    const ITEM_PADDING_TOP = 8;
    const MenuProps = {
        PaperProps: {
            style: {
                maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
                width: 250
            }
        }
    };
    const [selectedSpecs, setSelectedSpecs] = useState([]);

    const [personName, setPersonName] = useState([]);

    const handleChange = (e) => {
        const {
            target: { value }
        } = e;

        setSelectedSpecs(
            (value === "string" ? value.split(",") : value).map(
                (x) => catalog.specializations.find((y) => y.name === x).id
            )
        );
        setPersonName(typeof value === "string" ? value.split(",") : value);
    };

    useEffect(() => {
        (async () => {
            let req = { page };
            if (selectedSpecs.length > 0) req.specializations = selectedSpecs;
            const doctorss = await getDoctors(req).unwrap();
            setDoctors(doctorss.doctors);
            setAllCount(doctorss.count);
        })();
    }, []);

    const loadMore = async () => {
        setIsLoading(true);
        let req = { page: page + 1 };
        if (selectedSpecs.length > 0) req.specializations = selectedSpecs;
        const doctorss = await getDoctors(req).unwrap();
        setPage(page + 1);
        setDoctors([...doctors, ...doctorss.doctors]);
        setAllCount(doctorss.count);
        setIsLoading(false);
    };

    const reload = async () => {
        setPage(0);
        let req = { page: 0 };
        if (selectedSpecs.length > 0) req.specializations = selectedSpecs;
        const doctorss = await getDoctors(req).unwrap();
        setDoctors(doctorss.doctors);
        setAllCount(doctorss.count);
    };

    if ((!doctors || doctors.length < 1) && allCount != 0)
        return (
            <div className="d-flex justify-content-center w-100">
                <div
                    className="spinner-grow"
                    style={{ width: "3rem", height: "3rem" }}
                    role="status"
                ></div>
            </div>
        );

    return (
        <div className="d-flex flex-column doctors">
            <div className="d-flex flex-row justify-content-around flex-wrap w-100">
                <h3>Сhoose an expert for your needs</h3>
                {catalog?.specializations.length > 0 && (
                    <FormControl sx={{ width: 424 }}>
                        <InputLabel id="demo-multiple-checkbox-label">
                            Specialize on:
                        </InputLabel>
                        <Select
                            labelId="demo-multiple-checkbox-label"
                            id="demo-multiple-checkbox"
                            multiple
                            value={personName}
                            onChange={handleChange}
                            onClose={reload}
                            input={<OutlinedInput label="Specialize on:" />}
                            renderValue={(selected) => selected.join(", ")}
                            MenuProps={MenuProps}
                        >
                            {catalog?.specializations.map((name) => (
                                <MenuItem key={name.name} value={name.name}>
                                    <Checkbox
                                        checked={
                                            personName.indexOf(name.name) > -1
                                        }
                                    />
                                    <ListItemText primary={name.name} />
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                )}
            </div>

            <hr />

            <div className="d-flex flex-wrap justify-content-around">
                {doctors.map((doc) => (
                    <DoctorCard key={doc.id} doctor={doc} />
                ))}
                {(!doctors || doctors.length < 1) && <p>Empty Query</p>}
            </div>

            {allCount > (doctors?.length ?? allCount + 1) && (
                <div className="d-flex justify-content-center w-100">
                    <button onClick={loadMore} className="btn" type="button">
                        {isLoading ? (
                            <>
                                <span
                                    className="spinner-border spinner-border-sm"
                                    role="status"
                                    aria-hidden="true"
                                ></span>
                                {" Loading..."}
                            </>
                        ) : (
                            "Load More"
                        )}
                    </button>
                </div>
            )}
        </div>
    );
};

export default Doctors;
