import { Route, Routes } from "react-router-dom";
import RegisterPage from "./Register";
import LoginPage from "./Login";
import PrivateRoute from "../PrivateRoute";
import MyAccount from "./MyAccounts";
import Navigation from "./Navigation";
import Footer from "./Footer";
import { Box } from "@mui/material";
import Dashboard from "./Dashboard";
import HealthDependencyTree from "./HealthCheck";
import LogsDashboard from "./LogsDashboard";
import UsersAdminPanel from "./UserManagement";
// import { MatchLocateToLanguage } from '../localization';
// import { useTranslation } from "react-i18next";

function AppRoutes() {
    // const location = useLocation();
    // const { i18n } = useTranslation();

    // const segments = location.pathname.split("/").filter(Boolean);
    // const maybeLocale = segments[0];

    // useEffect(() => {
    //     const language = MatchLocateToLanguage(maybeLocale);

    //     if (i18n.language !== language)
    //         i18n.changeLanguage(language);
    // }, [maybeLocale]);

    const isAdminUser = true;

    const isUnauth = () => !isAdminUser;
    const isAuth = () => !!isAdminUser;

    return <>
        <Navigation />
        <Box component="main" sx={{ flex: 1 }}>
            <Routes>
                <Route path="my-account" index element={<PrivateRoute element={<MyAccount />} toPath="/login" validate={isAuth} />} />
                <Route path="sales-statistics" element={<PrivateRoute element={<Dashboard />} toPath="/login" validate={isAuth} />} />
                <Route path="health-check" element={<PrivateRoute element={<HealthDependencyTree />} toPath="/login" validate={isAuth} />} />
                <Route path="logs" element={<PrivateRoute element={<LogsDashboard />} toPath="/login" validate={isAuth} />} />
                <Route path="user-management" element={<PrivateRoute element={<UsersAdminPanel />} toPath="/login" validate={isAuth} />} />
                <Route path="login" element={<PrivateRoute element={<LoginPage />} toPath="/" validate={isUnauth} />} />
                <Route path="first-register" element={<PrivateRoute element={<RegisterPage />} toPath="/" validate={isUnauth} />} />
            </Routes>
        </Box>
        <Footer />
    </>;
}

export default AppRoutes;