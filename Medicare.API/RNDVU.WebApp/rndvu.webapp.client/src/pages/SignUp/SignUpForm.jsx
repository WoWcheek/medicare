import { useRef } from "react";
import { TextField, Stack, Link, Checkbox, RadioGroup, FormControlLabel, Radio } from "@mui/material";
import Logo from "../../components/Logo";
import SubmitButton from "../../components/SubmitButton";

import { useDispatch } from "react-redux";
import { setUser } from "../../redux/Auth/authSlice";
import {  useRegisterMutation } from "../../redux/Api/apiSlice";
import { NavLink, useNavigate } from "react-router-dom";
import asyncLocalStorage from "../../helpers/asyncLocalStorage";
import { toast } from "react-toastify";


function SignUpForm() {
   const emailEl = useRef(null);
   const fullnameEl = useRef(null);
   const passwordEl = useRef(null);
   const isPatient = useRef(null);

   const [register, { isLoading }] = useRegisterMutation();
   const dispatch = useDispatch();
   const history = useNavigate();
   const registration = async (e) => {
      e.preventDefault();

      try {
         const userData = await register({
            email: emailEl.current.value,
            password: passwordEl.current.value,
            fullName: fullnameEl.current.value,
            isPatient: isPatient.current.checked
         }).unwrap();
         dispatch(setUser(userData));
         asyncLocalStorage.setItem("token", userData.token)
         .then(()=>history('/'));
      } catch (e) {
         toast.error(e.data, {
            position: 'top-right',
          });
      }
   };

   return (
      <>
         <div className="half-y-div">
            <Logo />
            <form noValidate className="login-form" method="POST" action="">
               <Stack spacing={2} width={280}>
                  <TextField
                     label="Email"
                     variant="standard"
                     size="small"
                     type="email"
                     name="email"
                     inputRef={emailEl}
                  />
                 
                         <TextField
                     label="Full Name"
                     variant="standard"
                     size="small"
                     type="fullname"
                     name="fullName "
                     inputRef={fullnameEl}
                       />
                        <TextField
                     label="Password"
                     variant="standard"
                     size="small"
                     type="password"
                     name="password "
                     inputRef={passwordEl}
                       />
                       <RadioGroup
                        aria-labelledby="demo-radio-buttons-group-label"
                        defaultValue="patient"
                        name="radio-buttons-group"
                     >
                        <FormControlLabel value="patient" control={<Radio inputRef={isPatient}/>} label="I'm a patient" />
                        <FormControlLabel value="doctor" control={<Radio />} label="I'm a doctor" />
                     </RadioGroup>
               </Stack>
            </form>
         </div>

         <div className="half-y-div">
            <div className="btns-container">
               <div className="link-container">
                  <Link
                     underline="hover"
                     to="/login"
                     component={NavLink}
                  >
                     Cancel
                  </Link>
               </div>
               <SubmitButton
                  variant="contained"
                onClick={registration}
               >
                  Sign up
               </SubmitButton>
            </div>
            <div className="line-through-text">
               <hr />
               <h5>or</h5>
            </div>
         </div>
      </>
   );
}

export default SignUpForm;

