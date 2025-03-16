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

function AppRoutes() {
    const user = useAppSelector(state => state.user);
    const dispatch = useDispatch();

    useEffect(() => {
        if (user === undefined)
            dispatch(getReceiveUserAction());
    }, [user]);

    if (user === undefined)
        return null;

    var redirectIfUnAuth = () => user !== null;
    var redirectIfAuth = () => !redirectIfUnAuth();

    return <Routes>
        <Route path="/" element={<PrivateRoute element={<MainPage />} toPath="/login" validate={redirectIfUnAuth} />} />
        <Route path="/login" element={<PrivateRoute element={<LoginPage />} toPath="/" validate={redirectIfAuth} />} />
        <Route path="/register" element={<PrivateRoute element={<RegisterPage />} toPath="/" validate={redirectIfAuth} />} />
    </Routes>
}

export default AppRoutes;