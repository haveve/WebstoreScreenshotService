import React, { useEffect, useState } from "react";
import {
  Container,
  Card,
  Typography,
  Box,
  Stack,
  Chip,
  CircularProgress,
} from "@mui/material";

/* ---------------- MOCK ADMIN ---------------- */

const mockAdmin = {
  id: "admin-1",
  email: "ipz224_pis@student.ztu.edu.ua",
  nickName: "secret-admin",
  isActive: true,
  createdAt: new Date().toISOString(),
  twoFactorEnabled: false,
};
const AdminMyAccount = () => {
  const [admin, setAdmin] = useState<any | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // simulate API delay
    const timer = setTimeout(() => {
      setAdmin(mockAdmin);
      setLoading(false);
    }, 600);

    return () => clearTimeout(timer);
  }, []);

  if (loading) {
    return (
      <Container maxWidth="sm" sx={{ mt: 6, textAlign: "center" }}>
        <CircularProgress />
        <Typography mt={2} color="text.secondary">
          Завантаження акаунта адміністратора...
        </Typography>
      </Container>
    );
  }

  if (!admin) return null;

  return (
    <Container maxWidth="sm" sx={{ mt: 4 }}>
      <Card
        sx={{
          borderRadius: 3,
          boxShadow: "0 6px 24px rgba(0,0,0,0.06)",
        }}
      >
        <Box sx={{ p: 3 }}>
          {/* HEADER */}
          <Typography variant="h4" fontWeight={700} gutterBottom>
            Мій акаунт адміністратора
          </Typography>

          <Stack spacing={2}>
            {/* EMAIL */}
            <Typography>
              <strong>Електронна адреса:</strong> {admin.email}
            </Typography>

            {/* NICKNAME */}
            <Typography>
              <strong>Нікнейм:</strong> {admin.nickName}
            </Typography>

            {/* STATUS */}
            <Typography>
              <strong>Статус:</strong>{" "}
              {admin.isActive ? (
                <Chip size="small" color="success" label="АКТИВНИЙ" />
              ) : (
                <Chip size="small" color="error" label="ВИМКНЕНО" />
              )}
            </Typography>

            {/* 2FA */}
            {/* <Typography>
              <strong>Двофакторна автентифікація:</strong>{" "}
              {admin.twoFactorEnabled ? (
                <Chip size="small" color="success" label="УВІМКНЕНО" />
              ) : (
                <Chip size="small" color="warning" label="НЕ НАЛАШТОВАНО" />
              )}
            </Typography> */}

            {/* CREATED AT */}
            <Typography>
              <strong>Створено:</strong>{" "}
              {new Date(admin.createdAt).toLocaleString()}
            </Typography>

            {/* LINK */}
            {/* <Typography>
              <Link
                href="/admin/security"
                sx={{
                  fontWeight: 600,
                  textDecoration: "none",
                  "&:hover": { textDecoration: "underline" },
                }}
              >
                Перейти до налаштувань безпеки
              </Link>
            </Typography> */}
          </Stack>
        </Box>
      </Card>
    </Container>
  );
};

export default AdminMyAccount;