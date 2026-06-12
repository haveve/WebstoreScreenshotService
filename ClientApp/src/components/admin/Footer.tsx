import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";

import {
  AppBar,
  Box,
  Button,
  Chip,
  Container,
  Divider,
  Stack,
  Toolbar,
  Typography,
} from "@mui/material";

import {
  Dashboard as DashboardIcon,
  HealthAndSafety as HealthIcon,
  ReceiptLong as LogsIcon,
  PeopleAlt as UsersIcon,
  ManageAccounts as AccountIcon,
  Cookie as CookieIcon,
} from "@mui/icons-material";

import CookieBar from "../Cookiebar";
import cookieStore from "../../behavior/cookie/store";

const Footer = () => {
  const cookieConsent = cookieStore.getCookieConsent();

  const [isVisible, setIsVisible] = useState(
    cookieConsent === null
  );

  const currentYear = new Date().getFullYear();

  /* ---------------- NAVIGATION ---------------- */

  const footerLinks = [
    {
      label: "Панель керування",
      path: "/admin/sales-statistics",
      icon: <DashboardIcon fontSize="small" />,
    },
    {
      label: "Перевірка системи",
      path: "/admin/health-check",
      icon: <HealthIcon fontSize="small" />,
    },
    {
      label: "Журнали",
      path: "/admin/logs",
      icon: <LogsIcon fontSize="small" />,
    },
    {
      label: "Користувачі",
      path: "/admin/user-management",
      icon: <UsersIcon fontSize="small" />,
    },
    {
      label: "Мій акаунт",
      path: "/admin/my-account",
      icon: <AccountIcon fontSize="small" />,
    },
  ];

  return (
    <>
      {/* FOOTER */}
      <AppBar
        position="relative"
        color="primary"
        sx={{
          top: "auto",
          mt: 8,
        }}
      >
        <Container maxWidth="xl">
          {/* MAIN FOOTER */}
          <Toolbar
            sx={{
              py: 4,
              display: "flex",
              flexDirection: {
                xs: "column",
                md: "row",
              },
              justifyContent: "space-between",
              alignItems: {
                xs: "flex-start",
                md: "center",
              },
              gap: 4,
            }}
          >
            {/* BRAND */}
            <Box>
              <Stack
                direction="row"
                spacing={1.5}
                alignItems="center"
                mb={1}
              >
                <Box
                  component="img"
                  src="/logo.png"
                  alt="Логотип"
                  sx={{
                    height: 42,
                  }}
                />

                <Typography variant="h6" fontWeight={700}>
                  Адмін-панель Screenshot
                </Typography>
              </Stack>

              <Typography
                variant="body2"
                sx={{
                  opacity: 0.8,
                  maxWidth: 340,
                }}
              >
                Адміністративна панель для моніторингу,
                аналітики, журналів, управління користувачами та
                інфраструктурних операцій.
              </Typography>
            </Box>

            {/* NAVIGATION */}
            <Stack spacing={1}>
              <Typography variant="subtitle2" fontWeight={700}>
                Адміністрування
              </Typography>

              <Stack direction="row" flexWrap="wrap" gap={1}>
                {footerLinks.map((item) => (
                  <Button
                    key={item.label}
                    component={RouterLink}
                    to={item.path}
                    color="inherit"
                    startIcon={item.icon}
                    sx={{
                      justifyContent: "flex-start",
                    }}
                  >
                    {item.label}
                  </Button>
                ))}
              </Stack>
            </Stack>

            {/* STATUS */}
            <Stack spacing={1}>
              <Typography variant="subtitle2" fontWeight={700}>
                Статус системи
              </Typography>

              <Chip
                label="РЕЖИМ АДМІНІСТРАТОРА"
                color="warning"
                sx={{
                  fontWeight: 700,
                  color: "#fff",
                  width: "fit-content",
                }}
              />

              <Chip
                label="УСІ СЕРВІСИ ПРАЦЮЮТЬ"
                color="success"
                sx={{
                  fontWeight: 700,
                  color: "#fff",
                  width: "fit-content",
                }}
              />
            </Stack>
          </Toolbar>

          <Divider sx={{ borderColor: "rgba(255,255,255,0.12)" }} />

          {/* COPYRIGHT */}
          <Toolbar
            sx={{
              justifyContent: "space-between",
              flexDirection: {
                xs: "column",
                md: "row",
              },
              gap: 1,
              py: 2,
            }}
          >
            <Typography variant="body2" sx={{ opacity: 0.8 }}>
              © {currentYear} Платформа адміністрації Screenshot
            </Typography>

            <Typography variant="caption" sx={{ opacity: 0.7 }}>
              Моніторинг • Аналітика • Інфраструктура • Адміністрування
            </Typography>
          </Toolbar>
        </Container>
      </AppBar>

      {/* COOKIE BAR */}
      <CookieBar setVisibility={setIsVisible} showCookieBar={isVisible} />
    </>
  );
};

export default Footer;