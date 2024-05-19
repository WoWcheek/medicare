import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Alert, Button } from 'react-bootstrap';
import defaultIcon from '../../assets/images/default-user-image.png';
import { Checkbox, FormControl, InputLabel, ListItemText, MenuItem, OutlinedInput, Select } from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { useChangePasswordMutation, useEditProfileMutation, useSetAvatarMutation } from '../../redux/Api/apiSlice';
import { logout, setUser } from '../../redux/Auth/authSlice';
import asyncLocalStorage from '../../helpers/asyncLocalStorage';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const PersonalPage = () => {
    const user = useSelector(state => state.auth);
    const [userForm, setUserForm] = useState(user);
    const catalog = useSelector(state => state.catalog);
    const [editProfile] = useEditProfileMutation();

    const [show, setShow] = useState(false);
    const [message, setMessage] = useState("");
    const [variant, setVariant] = useState("");
    const inputOldPasswordRef = useRef(null);
    const inputNewPasswordRef = useRef(null);
    const [selectedSpecs, setSelectedSpecs] = useState(user.specializations || []);
    const [personName, setPersonName] = useState([]);
    const [setAvatar, { isLoading }] = useSetAvatarMutation();
    const [ChangePassword] = useChangePasswordMutation();
    const dispatch = useDispatch();
    const history = useNavigate();
    useEffect(()=>{if(catalog.specializations?.length>0) setPersonName(user?.specializations?.map(x=>catalog?.specializations?.find(y=>y.id === x)?.name));} ,[catalog])
        useEffect(()=>{setUserForm(user);setSelectedSpecs(user.specializations);} ,[user])
    const ITEM_HEIGHT = 48;
    const ITEM_PADDING_TOP = 8;
    const MenuProps = {
      PaperProps: {
        style: {
          maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
          width: 250,
        },
      },
    };

    const saveChanges = useCallback(async () => {
        try {
        let t = {...user, ...userForm, specializations: selectedSpecs};
        const userData = await editProfile(t).unwrap();
        dispatch(setUser(t));
      
        toast.success("Updated!", {
            position: 'top-right',
          });
         } catch (e) {
            toast.error(e.data, {
               position: 'top-right',
             });
         }
     
        //   if (res.status === 200) {
        //     setVariant('success');
        //     setShow(true);
        //     setMessage("Data changed");
        //     refreshData();
        //   } else {
        //     const x = await res.json();
        //     if (x.length > 0) {
        //       setMessage(x[0]);
        //       setVariant('danger');
        //       setShow(true);
        //     } else {
        //       setShow(false);
        //       refreshData();
        //     }
        //   }
        // });
      },[user,selectedSpecs, userForm]);

      const changePass = useCallback(async () => {
        try {
        let t = {new:inputNewPasswordRef.current.value, old:inputOldPasswordRef.current.value};
        const userData = await ChangePassword(t).unwrap();
      
        toast.success("Updated!", {
            position: 'top-right',
          });
          inputNewPasswordRef.current.value ='';
          inputOldPasswordRef.current.value ='';
         } catch (e) {
            toast.error(e.data, {
               position: 'top-right',
             });
         }
     
      
      },[user,selectedSpecs, userForm]);


    const logoutHandler = async (e) => {
        e.preventDefault();
  
        asyncLocalStorage.removeItem("token").then(()=> { Promise.resolve().then(function () {
            return dispatch(logout());
        }).then(()=>history('/login'))});
    }


    const changePic = (evt) => {
        const file = evt.target.files[0];
        const fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = async () => {
            const base64 = fileReader.result;
            const userData = await setAvatar({
                base64
             }).unwrap();
            dispatch(setUser({avatar:base64}));
        };
    };

    //   const ChangePassword = () => {
    //     const obj = JSON.parse(sessionStorage.getItem("user"));
    //     fetch("https://localhost:7102/Account/ChangePassword", {
    //       method: "PUT",
    //       headers: {
    //         "Content-Type": "application/json",
    //         Authorization: "bearer " + obj.token,
    //         oldPassword: inputOldPasswordRef.current.value,
    //         newPassword: inputNewPasswordRef.current.value,
    //       },
    //       body: JSON.stringify(user),
    //     })
    //     .then(async (res) => {
    //       if (res.status === 200) {
    //         setVariant('success');
    //         setShow(true);
    //         setMessage("Password changed");
    //         inputOldPasswordRef.current.value = "";
    //         inputNewPasswordRef.current.value = "";
    //         refreshData();
    //       } else {
    //         const x = await res.json();
    //         if (x.length > 0) {
    //           setMessage(x[0]);
    //           setVariant('danger');
    //           setShow(true);
    //         } else {
    //           setShow(false);
    //           refreshData();
    //         }
    //       }
    //     });
    //   };
    const handleChange = (e) => {
        const {
          target: { value },
        } = e;
        
        setSelectedSpecs((value === 'string' ? value.split(',') : value).map(x=> catalog.specializations.find(y=>y.name === x).id));
        setPersonName(
          typeof value === 'string' ? value.split(',') : value,
        );
      };


    return (
        <>
            <div style={{ marginTop: "100px" }} className="d-flex justify-content-center">
                <div style={{ width: "630px" }} className="d-flex flex-column align-items-center justify-contentf-center">

                    <img
                        style={{
                            width: "180px",
                            height: "180px",
                            objectFit: 'cover',
                            border: "3px solid black",
                            borderRadius: "50%",
                        }}
                        src={user.avatar || defaultIcon}
                    ></img>
                    <div className="container-xl px-4">
                        <div className="row">
                            <div className="col-12">
                                <div className="card mt-5 mb-4">
                                    <div className="card-header">{'Account details'}</div>
                                    <div className="card-body">
                                        <form>
                                            <div className="mb-3">
                                                <label className="mb-1" htmlFor="inputUsername">{'Full Name'}</label>
                                                <input
                                                    className="form-control"
                                                    id="inputUsername"
                                                    type="text"
                                                    onChange={(x) => setUserForm({ ...userForm, fullName: x.target.value })}
                                                    placeholder={'Enter full name'}
                                                    value={userForm.fullName}
                                                />
                                            </div>

                                            <div className="mb-3">
                                                <label className="mb-1" htmlFor="inputEmailAddress">{'Email'}</label>
                                                <input
                                                    className="form-control"
                                                    id="inputEmailAddress"
                                                    type="email"
                                                    disabled
                                                  value={userForm.email}
                                                />
                                            </div>

                                            <div className="mb-3">
                                                <label className="mb-1" htmlFor="inputPhone">{'Phone number'}</label>
                                                <input
                                                    className="form-control"
                                                    id="inputPhone"
                                                    type="tel"
                                                  onChange={(x) => setUserForm({ ...userForm, phoneNumber: x.target.value })}
                                                    placeholder={'Enter phone'}
                                                  value={userForm.phoneNumber}
                                                />
                                            </div>

                                           {catalog?.specializations.length > 0 && <div className="mb-3">
                                                <FormControl sx={{ m: 1, width: 300 }}>
                                                <InputLabel id="demo-multiple-checkbox-label">Specialize on:</InputLabel>
                                                      <Select
                                                        labelId="demo-multiple-checkbox-label"
                                                        id="demo-multiple-checkbox"
                                                        multiple
                                                        value={personName}
                                                        onChange={handleChange}
                                                        input={<OutlinedInput label="Specialize on:" />}
                                                        renderValue={(selected) => selected.join(', ')}
                                                        MenuProps={MenuProps}
                                                    >
                                                    {catalog?.specializations.map((name) => (
                                                        <MenuItem key={name.name} value={name.name}>
                                                        <Checkbox checked={personName.indexOf(name.name) > -1 }/>
                                                        <ListItemText primary={name.name} />
                                                        </MenuItem>
                                                    ))}
                                                    </Select>
                                                    </FormControl>
                                            </div>}

                                            <div className="mb-3">
                                                <label className="mb-1" htmlFor="inputPhone">{'Description'}</label>
                                                <textarea
                                                    className="form-control"
                                                    id="inputPhone"
                                                    onChange={(x) => setUserForm({ ...userForm, description: x.target.value })}
                                                    placeholder={'Enter description'}
                                                    value={userForm.description}
                                                />
                                            </div>

                                            <div className="d-flex w-100 justify-content-center">
                                                <button
                                                    style={{ margin: "auto", alignSelf: "center" }}
                                                    className="w-50 btn btn-outline-dark"
                                                    onClick={saveChanges}
                                                    type="button"
                                                >
                                                    {'Save Changes'}
                                                </button>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>

                            <div className="col-12">
                                <div className="card mb-4">
                                    <div className="card-header">{'Profile picture'}</div>
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

                            {(
                                <div className="col-12">
                                    <div className="card mb-4">
                                        <div className="card-header">{'Change password'}</div>
                                        <div className="card-body">
                                            <form>
                                                <div className="mb-0">
                                                    <label className="mb-1" htmlFor="inputOldPassword">{'Old password'}</label>
                                                    <input
                                                        className="form-control"
                                                        id="inputOldPassword"
                                                        type="password"
                                                        placeholder={'Enter old password'}
                                                        ref={inputOldPasswordRef}
                                                    />
                                                </div>

                                                <div className="mb-3">
                                                    <label className="mb-1" htmlFor="inputNewPassword">{'New password'}</label>
                                                    <input
                                                        className="form-control"
                                                        id="inputNewPassword"
                                                        type="password"
                                                        placeholder={'Enter new password'}
                                                        ref={inputNewPasswordRef}
                                                    />
                                                </div>
                                                <div className="d-flex w-100 justify-content-center">
                                                    <button
                                                        style={{ margin: "auto", alignSelf: "center" }}
                                                        onClick={changePass}
                                                        className="w-50 btn btn-outline-dark"
                                                        type="button"
                                                    >
                                                        {'Change'}
                                                    </button>
                                                </div>
                                              
                                            </form>
                                           
                                        </div>
                                      
                                    </div>
                                    <Button
                                                onClick={ logoutHandler}
                                                variant="outline-danger"
                                                className="mb-5 w-100"
                                                >
                                                Log out
                                                </Button>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default PersonalPage;
