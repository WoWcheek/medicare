import { useState, useEffect, useRef, useCallback } from "react";
import { Button } from "react-bootstrap";
import "./PersonalPage.scss";
import defaultIcon from "../../assets/images/default-user-image.png";
import {
    Checkbox,
    FormControl,
    InputLabel,
    ListItemText,
    MenuItem,
    OutlinedInput,
    Select
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import {
    useChangePasswordMutation,
    useEditProfileMutation,
    useSetAvatarMutation
} from "../../redux/Api/apiSlice";
import { logout, setUser } from "../../redux/Auth/authSlice";
import asyncLocalStorage from "../../helpers/asyncLocalStorage";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const PersonalPage = () => {
    const user = useSelector((state) => state.auth);
    const [userForm, setUserForm] = useState(user);
    const catalog = useSelector((state) => state.catalog);
    const [editProfile] = useEditProfileMutation();

    const inputOldPasswordRef = useRef(null);
    const inputNewPasswordRef = useRef(null);
    const [selectedSpecs, setSelectedSpecs] = useState(
        user.specializations || []
    );
    const [personName, setPersonName] = useState([]);
    const [setAvatar, { isLoading }] = useSetAvatarMutation();
    const [ChangePassword] = useChangePasswordMutation();
    const dispatch = useDispatch();
    const history = useNavigate();
    useEffect(() => {
        if (catalog.specializations?.length > 0)
            setPersonName(
                user?.specializations?.map(
                    (x) =>
                        catalog?.specializations?.find((y) => y.id === x)?.name
                )
            );
    }, [catalog]);
    useEffect(() => {
        setUserForm(user);
        setSelectedSpecs(user.specializations);
    }, [user]);
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

    const saveChanges = useCallback(async () => {
        try {
            let t = { ...user, ...userForm, specializations: selectedSpecs };
            const userData = await editProfile(t).unwrap();
            dispatch(setUser(t));

            toast.success("Updated!", {
                position: "top-right"
            });
        } catch (e) {
            toast.error(e.data, {
                position: "top-right"
            });
        }
    }, [user, selectedSpecs, userForm]);

    const changePass = useCallback(async () => {
        try {
            let t = {
                new: inputNewPasswordRef.current.value,
                old: inputOldPasswordRef.current.value
            };
            const userData = await ChangePassword(t).unwrap();

            toast.success("Updated!", {
                position: "top-right"
            });
            inputNewPasswordRef.current.value = "";
            inputOldPasswordRef.current.value = "";
        } catch (e) {
            toast.error(e.data, {
                position: "top-right"
            });
        }
    }, [user, selectedSpecs, userForm]);

    const logoutHandler = async (e) => {
        e.preventDefault();
        localStorage.removeItem("isPatient");
        asyncLocalStorage.removeItem("token").then(() => {
            Promise.resolve()
                .then(function () {
                    return dispatch(logout());
                })
                .then(() => history("/login"));
        });
    };

    const changePic = (evt) => {
        const file = evt.target.files[0];
        const fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = async () => {
            const base64 = fileReader.result;
            const userData = await setAvatar({
                base64
            }).unwrap();
            dispatch(setUser({ avatar: base64 }));
        };
    };

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

    return (
        <>
            <div className="personal d-flex justify-content-center">
                <div className="d-flex flex-column align-items-center justify-content-center">
                    <img src={user.avatar || defaultIcon} />
                    <div className="container-xl px-4">
                        <div className="row">
                            <div className="col-12">
                                <div className="card mt-5 mb-4">
                                    <div className="card-header">
                                        {"Account details"}
                                    </div>
                                    <div className="card-body">
                                        <form>
                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputUsername"
                                                >
                                                    {"Full Name"}
                                                </label>
                                                <input
                                                    className="form-control"
                                                    id="inputUsername"
                                                    type="text"
                                                    onChange={(x) =>
                                                        setUserForm({
                                                            ...userForm,
                                                            fullName:
                                                                x.target.value
                                                        })
                                                    }
                                                    placeholder={"Type..."}
                                                    value={userForm.fullName}
                                                />
                                            </div>

                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputEmailAddress"
                                                >
                                                    {"Email"}
                                                </label>
                                                <input
                                                    className="form-control"
                                                    id="inputEmailAddress"
                                                    type="email"
                                                    disabled
                                                    value={userForm.email}
                                                />
                                            </div>

                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputPhone"
                                                >
                                                    {"Phone number"}
                                                </label>
                                                <input
                                                    className="form-control"
                                                    id="inputPhone"
                                                    type="tel"
                                                    onChange={(x) =>
                                                        setUserForm({
                                                            ...userForm,
                                                            phoneNumber:
                                                                x.target.value
                                                        })
                                                    }
                                                    placeholder={"Type..."}
                                                    value={userForm.phoneNumber}
                                                />
                                            </div>

                                            {catalog?.specializations.length >
                                                0 &&
                                                !user.isPatient && (
                                                    <div className="mb-3 w-100 pt-1">
                                                        <FormControl
                                                            sx={{ width: 424 }}
                                                        >
                                                            <InputLabel id="demo-multiple-checkbox-label">
                                                                Specialize on:
                                                            </InputLabel>
                                                            <Select
                                                                labelId="demo-multiple-checkbox-label"
                                                                id="demo-multiple-checkbox"
                                                                multiple
                                                                value={
                                                                    personName
                                                                }
                                                                onChange={
                                                                    handleChange
                                                                }
                                                                input={
                                                                    <OutlinedInput label="Specialize on:" />
                                                                }
                                                                renderValue={(
                                                                    selected
                                                                ) =>
                                                                    selected.join(
                                                                        ", "
                                                                    )
                                                                }
                                                                MenuProps={
                                                                    MenuProps
                                                                }
                                                            >
                                                                {catalog?.specializations.map(
                                                                    (name) => (
                                                                        <MenuItem
                                                                            key={
                                                                                name.name
                                                                            }
                                                                            value={
                                                                                name.name
                                                                            }
                                                                        >
                                                                            <Checkbox
                                                                                checked={
                                                                                    personName.indexOf(
                                                                                        name.name
                                                                                    ) >
                                                                                    -1
                                                                                }
                                                                            />
                                                                            <ListItemText
                                                                                primary={
                                                                                    name.name
                                                                                }
                                                                            />
                                                                        </MenuItem>
                                                                    )
                                                                )}
                                                            </Select>
                                                        </FormControl>
                                                    </div>
                                                )}

                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputPhone"
                                                >
                                                    {"Description"}
                                                </label>
                                                <textarea
                                                    className="form-control"
                                                    id="inputPhone"
                                                    onChange={(x) =>
                                                        setUserForm({
                                                            ...userForm,
                                                            description:
                                                                x.target.value
                                                        })
                                                    }
                                                    placeholder={"Type..."}
                                                    value={userForm.description}
                                                />
                                            </div>

                                            <div className="d-flex w-100 justify-content-center">
                                                <button
                                                    className="w-50 btn btn-outline-dark save-btn"
                                                    onClick={saveChanges}
                                                    type="button"
                                                >
                                                    {"Save Changes"}
                                                </button>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>

                            <div className="col-12">
                                <div className="card mb-4">
                                    <div className="card-header">
                                        {"Profile picture"}
                                    </div>
                                    <div className="card-body text-center mb-2">
                                        <input
                                            className="form-control"
                                            accept="image/*"
                                            onChange={changePic}
                                            type="file"
                                            id="formFile"
                                        />
                                    </div>
                                </div>
                            </div>

                            <div className="col-12">
                                <div className="card mb-4">
                                    <div className="card-header">
                                        {"Change password"}
                                    </div>
                                    <div className="card-body">
                                        <form>
                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputOldPassword"
                                                >
                                                    {"Old password"}
                                                </label>
                                                <input
                                                    className="form-control"
                                                    id="inputOldPassword"
                                                    type="password"
                                                    placeholder={
                                                        "Enter old password"
                                                    }
                                                    ref={inputOldPasswordRef}
                                                />
                                            </div>

                                            <div className="mb-3">
                                                <label
                                                    className="mb-1"
                                                    htmlFor="inputNewPassword"
                                                >
                                                    {"New password"}
                                                </label>
                                                <input
                                                    className="form-control"
                                                    id="inputNewPassword"
                                                    type="password"
                                                    placeholder={
                                                        "Enter new password"
                                                    }
                                                    ref={inputNewPasswordRef}
                                                />
                                            </div>
                                            <div className="d-flex w-100 justify-content-center">
                                                <button
                                                    onClick={changePass}
                                                    className="w-50 btn btn-outline-dark save-btn"
                                                    type="button"
                                                >
                                                    {"Change"}
                                                </button>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                                <Button
                                    onClick={logoutHandler}
                                    variant="outline-danger"
                                    className="mb-5 w-100"
                                >
                                    Log out
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default PersonalPage;
