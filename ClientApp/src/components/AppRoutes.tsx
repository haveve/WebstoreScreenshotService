import { Route, Routes } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import MainPage from "./MainPage";
import RegisterPage from "./RegisterPage";
import LoginPage from "./LoginPage";
import { useEffect } from "react";
import { useDispatch } from 'react-redux'
import { useAppSelector } from '../behavior/rootReducer'
import { getReceiveUserAction } from "../behavior/epic";
import PrivateRoute from "./PrivateRoute";
import TermsAndConditions from "./termsAndConditions/TermsAndConditions";
import MyAccount from "./MyAccount";

function AppRoutes() {
    const user = useAppSelector(state => state.user);
    const dispatch = useDispatch();

    useEffect(() => {
        if (user === undefined)
            dispatch(getReceiveUserAction());
    }, [user]);

    if (user === undefined)
        return null;

    var isUnauth = () => user === null;
    var isAuth = () => user !== null;

    return <Routes>
        <Route path="/my-account" element={<PrivateRoute element={<MyAccount />} toPath="/login" validate={isAuth} />} />
        <Route path="/privacy-policy" element={<TermsAndConditions />} />
        <Route path="/" element={<PrivateRoute element={<MainPage />} toPath="/login" validate={isAuth} />} />
        <Route path="/login" element={<PrivateRoute element={<LoginPage />} toPath="/" validate={isUnauth} />} />
        <Route path="/register" element={<PrivateRoute element={<RegisterPage />} toPath="/" validate={isUnauth} />} />
    </Routes>
}

export default AppRoutes;