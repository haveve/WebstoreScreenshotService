import { Route, Routes } from "react-router-dom";
import MainPage from "./MainPage";
import MakeScreenshotPage from "../makeScreenshot/MakeScreenshotPage";
import ScreenshotsList from "../screenshotsList/List";
import ScreenshotDetailsPage from "../screenshotDetails/ScreenshotDetailsPage";
import RegisterPage from "./RegisterPage";
import LoginPage from "./LoginPage";
import { useEffect } from "react";
import { useDispatch } from 'react-redux'
import { useAppSelector } from '../../behavior/rootReducer'
import { getReceiveUserAction } from "../../behavior/epic";
import PrivateRoute from "../PrivateRoute";
import TermsAndConditions from "../termsAndConditions/TermsAndConditions";
import MyAccount from "./MyAccount";
import { AccountLayout } from "./AccountLayout";
import { TransactionsPage } from "./TransactionsPage";
import { OrdersPage } from "./Orders";
import { OrderDetailsPage } from "./OrderDetails";
import ApiTokenManager from "./ApiTokenManager";
import Navigation from "./Navigation";
import Footer from "./Footer";
import { Box } from "@mui/material";
import ApiDocumentationPage from "../Documentation";
// import { MatchLocateToLanguage } from '../localization';
// import { useTranslation } from "react-i18next";

function AppRoutes() {
    const user = useAppSelector(state => state.basic.user);
    const dispatch = useDispatch();
    // const location = useLocation();
    // const { i18n } = useTranslation();

    useEffect(() => {
        if (user === undefined)
            dispatch(getReceiveUserAction());
    }, [user]);

    // const segments = location.pathname.split("/").filter(Boolean);
    // const maybeLocale = segments[0];

    // useEffect(() => {
    //     const language = MatchLocateToLanguage(maybeLocale);

    //     if (i18n.language !== language)
    //         i18n.changeLanguage(language);
    // }, [maybeLocale]);

    if (user === undefined)
        return null;

    const isUnauth = () => !user;
    const isAuth = () => !!user;

    return <>
        <Navigation />
        <Box component="main" sx={{ flex: 1 }}>
            <Routes>
                <Route path="my-account" element={<PrivateRoute element={<AccountLayout />} toPath="/login" validate={isAuth} />}>
                    <Route
                        index
                        element={<MyAccount />}
                    />
                    <Route
                        path="transactions"
                        element={<TransactionsPage />}
                    />
                    <Route
                        path="orders"
                        element={<OrdersPage />}
                    />
                    <Route
                        path="token-management"
                        element={<ApiTokenManager />}
                    />
                    <Route
                        path="orders/:id"
                        element={<OrderDetailsPage />}
                    />
                </Route>
                <Route path="api-doc" element={<ApiDocumentationPage />} />
                <Route path="privacy-policy" element={<TermsAndConditions />} />
                <Route path="/" element={<MainPage />} />
                <Route path="make-screenshot" element={<PrivateRoute element={<MakeScreenshotPage />} toPath="/login" validate={isAuth} />} />
                <Route path="screenshots" element={<PrivateRoute element={<ScreenshotsList />} toPath="/login" validate={isAuth} />} />
                <Route path="screenshot/:id" element={<PrivateRoute element={<ScreenshotDetailsPage />} toPath="/login" validate={isAuth} />} />
                <Route path="login" element={<PrivateRoute element={<LoginPage />} toPath="/" validate={isUnauth} />} />
                <Route path="register" element={<PrivateRoute element={<RegisterPage />} toPath="/" validate={isUnauth} />} />
            </Routes>
        </Box>
        <Footer />
    </>;
}

export default AppRoutes;