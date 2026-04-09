import { Route, Routes } from "react-router-dom";
import MainPage from "./MainPage";
import MakeScreenshotPage from "./makeScreenshot/MakeScreenshotPage";
import ScreenshotsList from "./screenshotsList/List";
import ScreenshotDetailsPage from "./screenshotDetails/ScreenshotDetailsPage";
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

    const isUnauth = () => !user;
    const isAuth = () => !!user;

    return <Routes>
        <Route path="/my-account" element={<PrivateRoute element={<MyAccount />} toPath="/login" validate={isAuth} />} />
        <Route path="/privacy-policy" element={<TermsAndConditions />} />
        <Route path="/" element={<MainPage />} />
        <Route path="/make-screenshot" element={<PrivateRoute element={<MakeScreenshotPage />} toPath="/login" validate={isAuth} />} />
        <Route path="/screenshots" element={<PrivateRoute element={<ScreenshotsList />} toPath="/login" validate={isAuth} />} />
        <Route path="/screenshot/:id" element={<PrivateRoute element={<ScreenshotDetailsPage />} toPath="/login" validate={isAuth} />} />
        <Route path="/login" element={<PrivateRoute element={<LoginPage />} toPath="/" validate={isUnauth} />} />
        <Route path="/register" element={<PrivateRoute element={<RegisterPage />} toPath="/" validate={isUnauth} />} />
    </Routes>
}

export default AppRoutes;