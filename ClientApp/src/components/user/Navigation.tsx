import { useState } from "react";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useAppSelector } from "../../behavior/rootReducer";
import { getLogoutAction } from "../../behavior/epic";
import cookieStore from "../../behavior/cookie/store";
import { LinearProgress, Tooltip } from "@mui/material";
import { useTranslation } from "react-i18next";
import LanguageSelector from "../LanguageSelector";
import { useThemeMode } from "../ThemeSettings";
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
import {
    Menu as MenuIcon,
    Brightness4 as DarkModeIcon,
    Brightness7 as LightModeIcon,
    ManageAccounts as AccountIcon,
    ShoppingCart as ShoppingBasketIcon
} from "@mui/icons-material";
import { getPlanDescription, getPointsDescription } from "../componentHelpers";
import { BasketModal } from "./BasketPopover";
import { toggleBasket } from "../../behavior/basket/reducer";

type NavItem = {
    label: string;
    path?: string;
    action?: () => void;
};

const Navigation = () => {
    const dispatch = useDispatch();
    const user = useAppSelector((state) => state.basic.user);
    const loaded = useAppSelector((state) => state.basic.loaded);
    const cookieConsent = cookieStore.getCookieConsent();
    const [drawerOpen, setDrawerOpen] = useState(false);
    const { t } = useTranslation();
    const { mode, toggleTheme } = useThemeMode();
    const navigate = useNavigate();

    const handleLogout = () => dispatch(getLogoutAction());

    const navItems: NavItem[] = [
        { label: t("Navigation.home"), path: "/" },
        { label: t("Navigation.privacyPolicy"), path: "/privacy-policy" },
        { label: "Documentation", path: "/api-doc"}
    ];

    const userItems: NavItem[] = user
        ? [
            { label: t("Navigation.makeScreenshot"), path: "/make-screenshot" },
            { label: t("Navigation.screenshots"), path: '/screenshots' },
            { label: t("Navigation.logout"), action: handleLogout },
        ]
        : [
            { label: t("Navigation.login"), path: "/login" },
            { label: t("Navigation.register"), path: "/register" },
        ];

    const allItems = [...navItems, ...userItems];
    const functionalComponents = <>
        <LanguageSelector />
        <IconButton onClick={toggleTheme} color="inherit" aria-label="toggle theme">
            {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
        </IconButton>
        {user && (<>
            <Tooltip
                arrow
                placement="bottom"
                slotProps={{
                    tooltip: {
                        sx: (theme) => ({
                            backgroundColor: theme.palette.background.paper,
                            color: theme.palette.text.primary,
                            border: `1px solid ${theme.palette.divider}`,
                            boxShadow: theme.shadows[4],
                            padding: theme.spacing(1.5, 2),
                            maxWidth: 320,
                        }),
                    },
                    arrow: {
                        sx: (theme) => ({
                            color: theme.palette.background.paper,
                        }),
                    },
                }}
                title={
                    <Box>
                        <Typography variant="body2">
                            <strong>{t("MyAccount.points")}</strong> {getPointsDescription(user.subscriptionPlan.points)}
                        </Typography>
                        <Typography variant="body2">
                            <strong>{t("MyAccount.plan")}</strong> {getPlanDescription(user.subscriptionPlan.type, t)}
                        </Typography>
                    </Box>
                }
            >
                <IconButton
                    onClick={() => navigate("/my-account")}
                    color="inherit"
                >
                    <AccountIcon />
                </IconButton>
            </Tooltip>
            <IconButton
                onClick={() => dispatch(toggleBasket())}
                color="inherit"
            >
                <ShoppingBasketIcon />
            </IconButton>
            <BasketModal />
        </>)}
    </>

    return (
        <>
            <AppBar position="sticky" color="primary">
                <Container maxWidth="xl">
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
                            {functionalComponents}
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
                <Box sx={{ width: "auto" }} role="presentation" onClick={() => setDrawerOpen(false)}>
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
                        {functionalComponents}
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
