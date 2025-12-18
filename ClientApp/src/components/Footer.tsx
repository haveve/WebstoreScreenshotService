import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import { useAppSelector } from "../behavior/rootReducer";
import { useTranslation } from "react-i18next";
import { Box, Container, Toolbar, Typography, Button, AppBar } from "@mui/material";
import cookieStore from '../behavior/cookie/store';
import CookieBar from "./Cookiebar";

const Footer = () => {
    const user = useAppSelector(state => state.user);
    const { t } = useTranslation();
    const cookieConsent = cookieStore.getCookieConsent();
    const [isVisible, setIsVisible] = useState(cookieConsent === null);

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', mt: 6 }}>
            {/* Основний контент сторінки */}
            <Box component="main" sx={{ flex: 1 }} />

            {/* Футер */}
            <AppBar position="relative" color="primary" sx={{ mt: 'auto', top: 'auto' }}>
                <Container maxWidth="lg">
                    <Toolbar sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, justifyContent: 'space-between', py: 3 }}>
                        {/* Логотип та назва */}
                        <Box sx={{ display: 'flex', alignItems: 'center', mb: { xs: 2, md: 0 } }}>
                            <Box
                                component="img"
                                src="/logo.png"
                                alt="Logo"
                                sx={{ height: 50, mr: 1 }}
                            />
                            <Typography variant="h6" color="inherit" sx={{ textDecoration: 'none' }}>
                                {t('Navigation.screenshotService')}
                            </Typography>
                        </Box>

                        {/* Навігаційні кнопки */}
                        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: { xs: 2, md: 0 } }}>
                            <Button component={RouterLink} to="/" color="inherit">{t('Navigation.home')}</Button>
                            <Button component={RouterLink} to="/privacy-policy" color="inherit">{t('Navigation.privacyPolicy')}</Button>
                            {!user && <Button component={RouterLink} to="/login" color="inherit">{t('Navigation.login')}</Button>}
                            {!user && <Button component={RouterLink} to="/register" color="inherit">{t('Navigation.register')}</Button>}
                            {user && <Button component={RouterLink} to="/make-screenshot" color="inherit">{'Make screenshot'}</Button>}
                            {user && <Button component={RouterLink} to="/my-account" color="inherit">{t('Navigation.myAccount')}</Button>}
                            {user && <Button color="inherit" variant="outlined">{t('Navigation.logout')}</Button>}
                            {!isVisible && (<Button variant="outlined" color="inherit" onClick={() => setIsVisible(true)} sx={{ mr: 2 }} > {t('Navigation.cookiebar')} </Button>)}
                        </Box>
                    </Toolbar>
                    {/* Права */}
                    <Toolbar sx={{ justifyContent: 'center' }}>
                        <Typography variant="body2" color="inherit">
                            {t('Navigation.allRightsReserved', { date: new Date().getFullYear() })}
                        </Typography>
                    </Toolbar>
                </Container>
            </AppBar>
            <CookieBar setVisibility={setIsVisible} showCookieBar={isVisible} />
        </Box>
    );
};

export default Footer;
