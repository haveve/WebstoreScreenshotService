import { useState } from "react";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  IconButton,
  Box,
  Container,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Divider,
  Stack,
  Chip,
  Tooltip,
} from "@mui/material";

import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  HealthAndSafety as HealthIcon,
  ReceiptLong as LogsIcon,
  PeopleAlt as UsersIcon,
  ManageAccounts as AccountIcon,
  Logout as LogoutIcon,
  Brightness4 as DarkModeIcon,
  Brightness7 as LightModeIcon,
} from "@mui/icons-material";

import { useThemeMode } from "../ThemeSettings";

type NavItem = {
  label: string;
  path?: string;
  action?: () => void;
  icon?: React.ReactNode;
};

const Navigation = () => {
  const navigate = useNavigate();
  const { mode, toggleTheme } = useThemeMode();
  const [drawerOpen, setDrawerOpen] = useState(false);

  /* ---------------- MOCK ADMIN ---------------- */

  const admin = {
    nickname: "secret-admin",
    email: "ipz224_pis@student.ztu.edu.ua",
    role: "АДМІНІСТРАТОР",
  };

  /* ---------------- ACTIONS ---------------- */

  const handleLogout = () => {
    navigate("/login");
  };

  /* ---------------- NAVIGATION ---------------- */

  const navItems: NavItem[] = [
    {
      label: "Панель керування",
      path: "/sales-statistics",
      icon: <DashboardIcon fontSize="small" />,
    },
    {
      label: "Перевірка стану системи",
      path: "/health-check",
      icon: <HealthIcon fontSize="small" />,
    },
    {
      label: "Журнали",
      path: "/logs",
      icon: <LogsIcon fontSize="small" />,
    },
    {
      label: "Користувачі",
      path: "/user-management",
      icon: <UsersIcon fontSize="small" />,
    },
    {
      label: "Мій акаунт",
      path: "/my-account",
      icon: <AccountIcon fontSize="small" />,
    },
  ];

  const functionalButtons = (
    <Stack direction="row" spacing={1} alignItems="center">
      {/* THEME */}
      <IconButton
        color="inherit"
        onClick={toggleTheme}
        aria-label="перемикання теми"
      >
        {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
      </IconButton>

      {/* ADMIN */}
      <Tooltip
        arrow
        title={
          <Box>
            <Typography variant="body2">
              <strong>{admin.nickname}</strong>
            </Typography>

            <Typography variant="caption">
              {admin.email}
            </Typography>
          </Box>
        }
      >
        <Chip
          label={admin.role}
          color="warning"
          sx={{
            fontWeight: 700,
            color: "#fff",
          }}
        />
      </Tooltip>

      {/* LOGOUT */}
      <Button
        variant="outlined"
        color="inherit"
        startIcon={<LogoutIcon />}
        onClick={handleLogout}
      >
        Вийти
      </Button>
    </Stack>
  );

  return (
    <>
      {/* APP BAR */}
      <AppBar position="sticky">
        <Container maxWidth="xl">
          <Toolbar sx={{ justifyContent: "space-between" }}>
            {/* LOGO */}
            <Typography
              variant="h6"
              component={RouterLink}
              to="/sales-statistics"
              sx={{
                textDecoration: "none",
                color: "inherit",
                fontWeight: 700,
                display: "flex",
                alignItems: "center",
                gap: 1,
              }}
            >
              <Box
                component="img"
                src="/logo.png"
                alt="Логотип"
                sx={{ height: 40 }}
              />

              Адмін-панель Screenshot
            </Typography>

            {/* DESKTOP NAV */}
            <Box
              sx={{
                display: { xs: "none", md: "flex" },
                alignItems: "center",
                gap: 1,
              }}
            >
              {navItems.map((item) => (
                <Button
                  key={item.label}
                  component={RouterLink}
                  to={item.path!}
                  color="inherit"
                  startIcon={item.icon}
                >
                  {item.label}
                </Button>
              ))}

              <Divider
                orientation="vertical"
                flexItem
                sx={{ mx: 1 }}
              />

              {functionalButtons}
            </Box>

            {/* MOBILE MENU */}
            <Box sx={{ display: { xs: "flex", md: "none" } }}>
              <IconButton
                color="inherit"
                onClick={() => setDrawerOpen(true)}
              >
                <MenuIcon />
              </IconButton>
            </Box>
          </Toolbar>
        </Container>
      </AppBar>

      {/* MOBILE DRAWER */}
      <Drawer
        anchor="right"
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
      >
        <Box
          sx={{ width: 280 }}
          role="presentation"
          onClick={() => setDrawerOpen(false)}
        >
          {/* HEADER */}
          <Box sx={{ p: 3 }}>
            <Typography variant="h6" fontWeight={700}>
              Адмін-консоль
            </Typography>

            <Typography
              variant="body2"
              color="text.secondary"
            >
              {admin.email}
            </Typography>
          </Box>

          <Divider />

          {/* NAVIGATION */}
          <List>
            {navItems.map((item) => (
              <ListItem key={item.label} disablePadding>
                <ListItemButton
                  component={RouterLink}
                  to={item.path!}
                >
                  <Stack
                    direction="row"
                    spacing={1.5}
                    alignItems="center"
                  >
                    {item.icon}

                    <ListItemText primary={item.label} />
                  </Stack>
                </ListItemButton>
              </ListItem>
            ))}
          </List>

          <Divider />

          {/* FOOTER ACTIONS */}
          <Box sx={{ p: 2 }}>
            <Stack spacing={2}>
              <Button
                fullWidth
                variant="outlined"
                startIcon={
                  mode === "dark" ? (
                    <LightModeIcon />
                  ) : (
                    <DarkModeIcon />
                  )
                }
                onClick={toggleTheme}
              >
                Перемкнути тему
              </Button>

              <Button
                fullWidth
                color="error"
                variant="contained"
                startIcon={<LogoutIcon />}
                onClick={handleLogout}
              >
                Вийти
              </Button>
            </Stack>
          </Box>
        </Box>
      </Drawer>
    </>
  );
};

export default Navigation;