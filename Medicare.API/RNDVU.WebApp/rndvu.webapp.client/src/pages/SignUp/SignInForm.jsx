import { useRef } from "react";
import { TextField, Stack, Link } from "@mui/material";
import PropTypes from "prop-types";
import Logo from "../../components/Logo";
import SubmitButton from "../../components/SubmitButton";
import { NavLink, useNavigate } from "react-router-dom";
import { useLoginMutation } from "../../redux/Api/apiSlice";
import { useDispatch } from "react-redux";
import { setUser } from "../../redux/Auth/authSlice";
import asyncLocalStorage from "../../helpers/asyncLocalStorage";
import { toast } from "react-toastify";

function SignInForm() {
   const emailEl = useRef(null);
   const passwordEl = useRef(null);
 
   const [signIn, { isLoading }] = useLoginMutation();
   const dispatch = useDispatch();
   const history = useNavigate();

   const login = async (e) => {
      e.preventDefault();

      try {
         const userData = await signIn({
            email: emailEl.current.value,
            password: passwordEl.current.value
         }).unwrap();

         if (!userData.id) {
            throw new Error("Login Failed!");
         }

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
                     label="Password"
                     variant="standard"
                     size="small"
                     type="password"
                     name="password "
                     inputRef={passwordEl}
                  />
               </Stack>
            </form>
         </div>

         <div className="half-y-div">
            <div className="btns-container">
               <div className="link-container">
                  <Link
                     underline="hover"
                     to="/register"
                     component={NavLink}
                  >
                     Sign up
                  </Link>
               </div>
               <SubmitButton variant="contained" onClick={(e) => login(e)}>
                  Sign in
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

export default SignInForm;

SignInForm.propTypes = {
   setIsRegistering: PropTypes.func
};
