import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useAppSelector } from "../behavior/rootReducer";
import { getLogoutAction } from "../behavior/epic";
import cookieStore from "../behavior/cookie/store";
import { LinearProgress } from "@mui/material";
import { useTranslation } from "react-i18next";
import LanguageSelector from "./LanguageSelector";
import { useThemeMode } from "./ThemeSettings";
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    IconButton,
    Box,
    Alert,
    Container,
    Drawer,
    List,
    ListItem,
    ListItemButton,
    ListItemText,
    Divider,
} from "@mui/material";
import { Menu as MenuIcon, Brightness4 as DarkModeIcon, Brightness7 as LightModeIcon } from "@mui/icons-material";

type NavItem = {
    label: string;
    path?: string;
    action?: () => void;
};

const Navigation = () => {
    const dispatch = useDispatch();
    const user = useAppSelector((state) => state.user);
    const loaded = useAppSelector((state) => state.loaded);
    const cookieConsent = cookieStore.getCookieConsent();
    const [drawerOpen, setDrawerOpen] = useState(false);
    const { t } = useTranslation();
    const { mode, toggleTheme } = useThemeMode();

    const handleLogout = () => dispatch(getLogoutAction());

    const navItems: NavItem[] = [
        { label: t("Navigation.home"), path: "/" },
        { label: t("Navigation.privacyPolicy"), path: "/privacy-policy" },
    ];

    const userItems: NavItem[] = user
        ? [
            { label: t("Navigation.makeScreenshot"), path: "/make-screenshot" },
            { label: t("Navigation.myAccount"), path: "/my-account" },
            { label: "Screenshots", path: '/screenshots' },
            { label: t("Navigation.logout"), action: handleLogout },
        ]
        : [
            { label: t("Navigation.login"), path: "/login" },
            { label: t("Navigation.register"), path: "/register" },
        ];

    const allItems = [...navItems, ...userItems];

    return (
        <>
            <AppBar position="sticky" color="primary">
                <Container maxWidth="lg">
                    <Toolbar sx={{ justifyContent: "space-between" }}>
                        {/* Logo and Title */}
                        <Typography
                            variant="h6"
                            component={RouterLink}
                            to="/"
                            sx={{
                                textDecoration: "none",
                                color: "inherit",
                                display: "flex",
                                alignItems: "center",
                            }}
                        >
                            <Box component="img" src="/logo.png" alt="Logo" sx={{ height: 50, mr: 1 }} />
                            {t("Navigation.screenshotService")}
                        </Typography>

                        {/* Desktop Navigation */}
                        <Box sx={{ display: { xs: "none", md: "flex" }, alignItems: "center", gap: 1 }}>
                            {allItems.map((item, index) =>
                                item.path ? (
                                    <Button component={RouterLink} to={item.path} key={index} color="inherit">
                                        {item.label}
                                    </Button>
                                ) : (
                                    <Button key={index} color="inherit" variant="outlined" onClick={item.action}>
                                        {item.label}
                                    </Button>
                                )
                            )}
                            <LanguageSelector />
                            <IconButton onClick={toggleTheme} color="inherit" aria-label="toggle theme">
                                {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
                            </IconButton>
                        </Box>

                        {/* Mobile Burger Menu */}
                        <Box sx={{ display: { xs: "flex", md: "none" } }}>
                            <IconButton color="inherit" onClick={() => setDrawerOpen(true)}>
                                <MenuIcon />
                            </IconButton>
                        </Box>
                    </Toolbar>
                </Container>
            </AppBar>

            {/* Mobile Drawer */}
            <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
                <Box sx={{ width: 250 }} role="presentation" onClick={() => setDrawerOpen(false)}>
                    <List>
                        {allItems.map((item, index) => (
                            <ListItem key={index} disablePadding>
                                <ListItemButton
                                    component={item.path ? RouterLink : "button"}
                                    to={item.path}
                                    onClick={item.action}
                                >
                                    <ListItemText primary={item.label} />
                                </ListItemButton>
                            </ListItem>
                        ))}
                    </List>
                    <Divider />
                    <Box sx={{ p: 2 }}>
                        <LanguageSelector />
                        <IconButton onClick={toggleTheme} color="inherit" aria-label="toggle theme">
                            {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
                        </IconButton>
                    </Box>
                </Box>
            </Drawer>
            {!loaded &&
                <Box sx={{ width: '100%' }}>
                    <LinearProgress />
                </Box>
            }
            {loaded && cookieConsent === false && (
                <Container maxWidth="md" sx={{ mt: 3 }}>
                    <Alert severity="error">{t("Navigation.cookiesDeclined")}</Alert>
                </Container>
            )}
            <Container sx={{ mb: 6 }} />
        </>
    );
};

export default Navigation;
