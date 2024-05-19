import SignInPage from "./pages/SignUp/SignInPage";
import SignUpPage from "./pages/SignUp/SignUpPage";
import {Routes, Route, Navigate} from 'react-router-dom'
import { useSelector } from "react-redux";
import AuthorizedRoutes from "./components/AuthorizedRoutes";

function App() {
    const accessToken =  useSelector(state => state.auth.token) ||localStorage.getItem("token") ;

    if (!accessToken) {
        return (<Routes>
            <Route exact path="/login" element={<SignInPage />} />
            <Route exact path="/register" element={<SignUpPage />} />
            <Route path='*' element={<Navigate to="/login" />} />
        </Routes>);
    }

    return (<AuthorizedRoutes/>);

}

export default App;
